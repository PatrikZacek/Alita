using Alita.Models.AbstractModels;
using Alita.Models.Struct;
using Mobile.Communication.Client;
using Mobile.Communication.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Alita.Models.Interfaces;
using Mobile.Communication.Common.Event;
using System.Windows;
using Alita.Services;
using System.Threading;
using Alita.Models.Objects;

namespace Alita.Models
{
    public class LynxModel : AbstractRobot
    {
        public LynxModel()
        {
            
        }

        public LynxModel(ZmenaJobParameteruHandler jobParameteruHandler) : base(jobParameteruHandler)
        {
            
        }

        IRobotClient RobotClient { get; set; }        
        private HlidaciPes<VstupVystup> HlidaciPes { get; set; }

        public override PrikazHandler<RobotPrikaz> ZarizeniZasilaPrikaz { get; set; }

        public override bool Connect()
        {
            try
            {
                if (RobotClient == null) RobotClientSetup();
                if(ZarizeniJeDlePinguOnline) RobotClient?.Connect();
                bool status = false;
                status = RobotClient.Connected;
                if (status)
                {
                    Status = Stav.Online;
                    NastaveniHlidacihoPsa();
                    HlidaciPes.Zapni();
                    
                }
                return status;                
            }
            catch (Exception exception)
            {
                var zarizeniArgs = new ZarizeniArgs(Status, Ping, $"!CHYBA!", exception);
                OnStatusChanged(zarizeniArgs);
                return false;
            }
        }

        private void NastaveniHlidacihoPsa()
        {
            if (HlidaciPes != null) return;
            HlidaciPes = new HlidaciPes<VstupVystup>(this);
            HlidaciPes.HlidaciPesNecoZaznamenal += (object Nalez) =>
            {
                Hlaseni hlaseniPsa = (Hlaseni)Nalez;
                VstupVystup vstupVystup = (VstupVystup)hlaseniPsa.Zprava;
                if (vstupVystup.Nazev.ToLower() == "error")
                    if (vstupVystup.Hodnota) Status = Stav.InError;
                    else Status = Stav.Online;
                
            };
        }

        private void RobotClientSetup()
        {
            PingniNaZarizeni();
            RobotClient = new RobotClient(Hostname, IPAddress.Parse(IpAdress), Port)
            {
                ManageJobs = ArclBase.JobManagement.Full,
                QueuedResponses = ArclBase.ResponseFlag.All,
                QueueAll = true,
                Password = "adept",
                ResponseTimeout = 1000,
                AutoReconnect = true
            };
            RobotClient.Responses.MaxLength = 200;
            RobotClient.Responses.QueueChanged += Responses_QueueChanged;
            RobotClient.NewJobQueued += RobotClient_NewJobQueued;
        }

        protected override void PingniNaZarizeni()
        {
            base.PingniNaZarizeni();

            if (RobotClient != null)
                if (RobotClient.Connected == false) RobotClient.Connect();
        }

        public override async Task<AgvStatus> StatusRobota()
        {            
            RobotClient.RefreshStatus(out RobotStatus robotStatus, out string error);
            string StatusForHumans = robotStatus.ExtendedStatusForHumans;
            StatusType agvStatusType = StatusType.Unknown;
            if (StatusForHumans.ToLower().Contains("going to")) agvStatusType = StatusType.InWork;
            if (StatusForHumans.ToLower().Contains("parking")) agvStatusType = StatusType.Parking;
            if (StatusForHumans.ToLower().Contains("docking")) agvStatusType = StatusType.Docking;
            if (StatusForHumans.ToLower().Contains("docked")) agvStatusType = StatusType.Docked;
            Struct.Location accurateLocationOfRobot = new Struct.Location(robotStatus.Location.X, robotStatus.Location.Y);
            MapGoal? goal = null;
            if (agvStatusType == StatusType.Docked)
            {
                foreach (MapGoal dock in await GoalsInfo())
                {
                    long robotX = accurateLocationOfRobot.X;
                    long robotY = accurateLocationOfRobot.Y;

                    if (dock.Location.X >= robotX - 1000 && dock.Location.X <= robotX + 1000)
                        if (dock.Location.Y >= robotY - 400 && dock.Location.Y <= robotY + 400)
                        {
                            goal = dock;
                            break;
                        }                           
                }
            }
            
            AgvStatus result = new AgvStatus(agvStatusType, robotStatus.LocalizationScore, accurateLocationOfRobot, goal);

            return result;
        }

        private async Task<List<MapGoal>> GoalsInfo()
        {
            List<MapGoal> result = new List<MapGoal>();
            ProvedPrikaz(new RobotPrikaz(TypPrikazu.Macro, "mapObjectList", "DockLynx"));
            await Task.Delay(1000);
            List<string> komunikaceZArclu = await PrectiKomunikaciZArclServeru();
            int indexOfCommand = komunikaceZArclu.IndexOf("mapObjectList DockLynx");
            List<string> seznamDockLynx = komunikaceZArclu.Where(x => x.ToLower().Contains("mapobjectlist")).ToList();
            seznamDockLynx.RemoveAt(0);
            seznamDockLynx.Remove(seznamDockLynx.Last());
            foreach (string command in seznamDockLynx)
            {
                //"MapObjectList: \"Dobijec_stanice_smd\" DockLynx"
                var commandSplitted = command.Split(' ');
                string lynxDock = commandSplitted[1];
                ProvedPrikaz(new RobotPrikaz(TypPrikazu.Macro, "mapObjectInfo", lynxDock));
                await Task.Delay(1000);
                List<string> tempKomunikace = await PrectiKomunikaciZArclServeru();
                List<string> objectInfo = tempKomunikace.GetRange(tempKomunikace.Count - 2, 1);
                //"MapObjectInfoCoord: \"Dobijec_stanice_smd\" -3914 1929 -90.000" 

                var objectInfoCoord = objectInfo.First().Split(' ');
                string goalName = objectInfoCoord[1];
                goalName = goalName.Replace("\"", "");
                int.TryParse(objectInfoCoord[2], out int XCoord);
                int.TryParse(objectInfoCoord[3], out int YCoord);

                MapGoal dock = new MapGoal(goalName, new Struct.Location(XCoord, YCoord), GoalType.Dock);
                result.Add(dock);
            }
            return result;
        }

        private void RobotClient_NewJobQueued(ArclBase arcl, JobEventArgs args)
        {           
            args.Job.PropertyChanged += Job_PropertyChanged;
            JobChanged(args.Job);            
        }

        private void JobChanged(JobData jobData)
        {
            Job job = new Job
            {
                ID = jobData.ID
            };

            if (jobData.Segments.ToList().Exists(x => x.IsPickup))
                job.PocatecniGoal = jobData.Segments.Where(x => x.IsPickup).First().Goal;

            if (jobData.Segments.ToList().Exists(x => x.IsDropoff))
                job.KoncovyGoal = jobData.Segments.Where(x => x.IsDropoff)?.First().Goal;

            if (jobData.Finished != DateTime.MinValue)
                job.CasDokonceni = jobData.Finished;

            ZmenaJob?.Invoke(this, job);
        }

        private void Job_PropertyChanged(JobData job, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(job.Finished))
            {
                JobChanged(job);
            }
        }

        private void Responses_QueueChanged(object sender, QueueChangedEventArgs<string> e)
        {
            try
            {
                if (e.Changed != QueueChangedEventArgs<string>.Change.Enqueued) return;
                if (e.Item.ToLower().Contains("arclsendtext")) return;
                if (e.Item.ToLower().Contains("alita"))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var prikaz = RobotPrikaz.GetRobotPrikaz(e.Item);
                        ZarizeniZasilaPrikaz?.Invoke(this,prikaz);
                    });
                }
            }
            catch (Exception exception)
            {
                string s = exception.Data.ToString();
            }
        }

        public override bool Disconnect()
        {
            try
            {
                RobotClient?.Disconnect();
                bool status = false;
                status = (bool)RobotClient?.Connected ? status : false;
                Status = Stav.Offline;
                return true;
            }
            catch (Exception exception)
            {
                Status = Stav.InError;
                var zarizeniArgs = new ZarizeniArgs(Status, Ping, $"!CHYBA!", exception);
                OnStatusChanged(zarizeniArgs);
                return false;
            }            
        }

        public async Task<List<string>> PrectiKomunikaciZArclServeru()
        {   
            List<string> KomunikaceZArclServeru = new List<string>();
            await Task.Delay(millisecondsDelay: 100);
            KomunikaceZArclServeru = RobotClient?.Responses.ToList();

            return KomunikaceZArclServeru;
        }

        public override void ProvedPrikaz(RobotPrikaz prikaz)
        {
            RobotClient?.SendCommand(prikaz.ToString());            
        }

        public override Task<VstupVystup[]> PrectiVstupyNaZarizeni()
        {
            return Task.Factory.StartNew(() => {
                List<VstupVystup> vysledneVstupy = new List<VstupVystup>();
                var CollectionZRobota = RobotClient.Inputs ?? new InputCollection();
                foreach (Input robotInput in CollectionZRobota.IOs)
                {
                    bool hodnotaNaVstupu = false;
                    if (robotInput.State == Mobile.Communication.Common.IO.Switch.Off) hodnotaNaVstupu = false;
                    if (robotInput.State == Mobile.Communication.Common.IO.Switch.On) hodnotaNaVstupu = true;
                    if (robotInput.State == Mobile.Communication.Common.IO.Switch.Unknown) throw new Exception("Unknown state of robot input", new Exception($"{Hostname}|{IpAdress}|{robotInput.Label}"));

                    VstupVystup vstupVystup = new VstupVystup(Objects.IO.Input, robotInput.Label, hodnotaNaVstupu);
                    vysledneVstupy.Add(vstupVystup);
                }
                return vysledneVstupy.ToArray();
            });
        }
    }    
}
