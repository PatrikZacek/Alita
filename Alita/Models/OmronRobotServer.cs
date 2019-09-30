using Alita.Models.AbstractModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alita.Models.Struct;
using Mobile.Communication.Client;
using Alita.Models.Objects;
using Alita.Services;
using System.Net;
using Mobile.Communication.Common;
using Alita.Models.Interfaces;

namespace Alita.Models
{
    public class OmronRobotServer : AbstractRobotServer<AbstractRobot>
    {
        private IEnterpriseManagerClient EnterpriseManagerClient { get; set; }
        private string[] EnterpriseManagerRobotIdentifikatory { get; set; }

        public OmronRobotServer()
        { }        

        public override bool Connect()
        {
            try
            {
                if (EnterpriseManagerClient == null) EnterpriseManagerSetup();
                if (ZarizeniJeDlePinguOnline) EnterpriseManagerClient?.Connect();
                bool status = false;
                status = EnterpriseManagerClient.Connected;
                if (status) Status = Stav.Online;
                return status;
            }
            catch (Exception exception)
            {
                var zarizeniArgs = new ZarizeniArgs(Status, Ping, $"!CHYBA!", exception);
                OnStatusChanged(zarizeniArgs);
                return false;
            }
        }

        private void EnterpriseManagerSetup()
        {
            PingniNaZarizeni();
            EnterpriseManagerClient = new EnterpriseManagerClient(Hostname, IPAddress.Parse(IpAdress), Port)
            {
                ManageJobs = ArclBase.JobManagement.Full,
                QueuedResponses = ArclBase.ResponseFlag.All
            };
            EnterpriseManagerClient.Responses.MaxLength = 500;
            EnterpriseManagerClient.Password = "viscom";
            EnterpriseManagerClient.ResponseTimeout = 2000;
            EnterpriseManagerClient.AutoReconnect = true;  

            EnterpriseManagerClient.ConnectedRobotsComplete += EnterpriseManagerClient_ConnectedRobotsComplete;
            EnterpriseManagerClient.NewJobQueued += EnterpriseManagerClient_NewJobQueued;   
        }

        protected override void PingniNaZarizeni()
        {
            base.PingniNaZarizeni();

            if (EnterpriseManagerClient != null)
                if (EnterpriseManagerClient.Connected == false)
                    EnterpriseManagerClient.Connect();            
        }

        private void EnterpriseManagerClient_NewJobQueued(ArclBase arcl, Mobile.Communication.Common.Event.JobEventArgs args)
        {
            Job job = new Job
            {
                ID = args.Job.ID,
                CasDokonceni = args.Job.Finished,
                CasVzniku = args.Job.Queued,
                Server = this
            };

            if (args.Job.Segments.ToList().Exists(x => x.IsPickup))
                job.PocatecniGoal = args.Job.Segments.Where(x => x.IsPickup).First().Goal;
            if (args.Job.Segments.ToList().Exists(x => x.IsDropoff))
                job.KoncovyGoal = args.Job.Segments.Where(x => x.IsDropoff)?.First().Goal;

            OnNovyJobPrirazen(job);
        }

        private void EnterpriseManagerClient_ConnectedRobotsComplete(EnterpriseManager server, Mobile.Communication.Common.Event.StringsCompleteEventArgs args)
        {
            EnterpriseManagerRobotIdentifikatory = args.Strings;
        }

        public override async Task<AbstractRobot[]> PripojeniRoboti()
        {
            if(EnterpriseManagerClient == null)
            {
                Connect();
            }
            var identifikatoryRobotuPripojenychNaServer = EnterpriseManagerClient.RefreshConnectedRobots();
            await Task.Delay(10);
            LynxModel[] vysledek = new LynxModel[identifikatoryRobotuPripojenychNaServer.Where(x => x != null).Count()];
            int indexPole = 0;
            foreach (string identifikator in identifikatoryRobotuPripojenychNaServer)
            {
                DnsInfo robotDnsInfo = GetHostNameAndIPAdress.DnsInfoFromHostname($"AGV_{identifikator}");
                LynxModel pripojenyRobot = new LynxModel
                {
                    Hostname = identifikator,
                    IpAdress = robotDnsInfo.IP.ToString()
                };
                vysledek[indexPole] = pripojenyRobot;
                indexPole++;
            }
            return vysledek;
        }

        public override bool Disconnect()
        {
            try
            {
                EnterpriseManagerClient?.Disconnect();
                bool status = false;
                status = (bool)EnterpriseManagerClient?.Connected ? status : false;
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

        public override void ProvedPrikaz(RobotPrikaz prikaz)
        {
            EnterpriseManagerClient?.SendCommand(prikaz.ToString());
        }        
    }
}
