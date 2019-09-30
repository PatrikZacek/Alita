using Alita.Models.AbstractModels;
using Alita.Models.Interfaces;
using Alita.Models.Objects;
using Alita.Models.Struct;
using Alita.Services;
using Mobile.Communication.Common.Event;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows_MVVM.Framework_MVVM;

namespace Alita.Models
{
    public class MainViewModel : ObservableObject
    {
        IDatabaseConn Database { get; set; }
        ConfigData Config { get; set; }

        public MainViewModel()
        {
            UkonciAplikaci = new RelayCommand(OnUkonciAplikaci, () => true);
            PohniSaplikaci = new RelayCommandWithParametr<MainWindow>(this, oknoAplikace => OnPohniSAplikaci(oknoAplikace), () => true);
            ZmenWindowsState = new RelayCommandWithParametr<object>(this, windowState => OnZmenWindowState(windowState), () => true);
            Config = new ConfigData();
            Database = new DatabaseConnection(Config.DataSource, Config.InitialCatalog, Config.UserId);
        }

        public MainViewModel(bool test) : this()
        {
            NactiDataDb();
            foreach (AbstractRobotServer<AbstractRobot> server in Servery.Collection)
            {
                server.NovyJobPrirazen += Server_NovyJobPrirazen;
            }            
            FlotilaRobotu = Database.FlotilaDatabaze(true);
            foreach (AbstractRobot robot in FlotilaRobotu.Collection)
            {
                robot.ZarizeniZasilaPrikaz = RobotZasilaPrikaz;
                robot.ZmenaJob = Robot_PrijalNovyJob;
            }
            FlotilaRobotu.PripojVse();
            Servery.PripojVse();
            PripojenaExterniZarizeni.PripojVse();
        }

        protected void NactiDataDb()
        {
            Servery = Database.ServeryZDatabaze(true);
            PripojenaExterniZarizeni = Database.ExterniZarizeniZDatabaze(true);
            FlotilaRobotu = Database.FlotilaDatabaze(true);
        }

        private RobotKolekce flotilaRobotu = new RobotKolekce();
        public RobotKolekce FlotilaRobotu
        {
            get { return flotilaRobotu; }
            set { flotilaRobotu = value; OnPropertyChanged(); }
        }

        private ExterniZarizeniKolekce pripojenaExterniZarizeni = new ExterniZarizeniKolekce();
        public ExterniZarizeniKolekce PripojenaExterniZarizeni
        {
            get { return pripojenaExterniZarizeni; }
            set { pripojenaExterniZarizeni = value; OnPropertyChanged(); }
        }

        private RobotServerKolekce servery = new RobotServerKolekce();
        public RobotServerKolekce Servery
        {
            get => servery;
            set { servery = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Job> seznamJobu = new ObservableCollection<Job>();
        public ObservableCollection<Job> SeznamJobu
        {
            get => seznamJobu;
            set
            {
                seznamJobu = value;
                OnPropertyChanged();
            }
        }

        private bool? winState = false;
        public bool? WinState
        {
            get { return winState; }
            set { winState = value; OnPropertyChanged(); }
        }

        public RelayCommand UkonciAplikaci { get; }
        protected void OnUkonciAplikaci()
        {
            Application.Current.Shutdown();
        }

        public RelayCommandWithParametr<MainWindow> PohniSaplikaci { get; }
        protected void OnPohniSAplikaci(MainWindow mainWindow)
        {
            mainWindow.DragMove();
        }

        public RelayCommandWithParametr<object> ZmenWindowsState { get; }
        protected void OnZmenWindowState(object WindowState)
        {
            if (!(WindowState is String)) return;
            String _parametr = (String)WindowState;
            switch (_parametr)
            {
                case "ZvetsiZmensiOkno":
                    WinState = !WinState;
                    break;
                case "MinimalizujOkno":
                    WinState = null;
                    break;
                default:
                    break;
            }
        }

        public void RobotZasilaPrikaz(IZarizeni sender, RobotPrikaz prikaz)
        {
            AbstractRobot robot = (AbstractRobot)sender;
            if (prikaz.Typ == TypPrikazu.Trigger)
            {
                bool.TryParse(prikaz.Hodnota, out bool hodnotaProExterniZarizeni);
                if (prikaz.Hodnota.ToLower() == "open") hodnotaProExterniZarizeni = true;
                if (prikaz.Hodnota.ToLower() == "close") hodnotaProExterniZarizeni = false;
                MoxaPrikaz prikazProExterniZarizeni = new MoxaPrikaz(prikaz.Telo, prikaz.Pin, hodnotaProExterniZarizeni);
                AbstractExternalDevice externalDevice = PripojenaExterniZarizeni.Collection.Single(x => x.Hostname.ToLower() == prikaz.Telo.ToLower());
                externalDevice.ProvedPrikaz(prikazProExterniZarizeni);
            }
            if (prikaz.Typ == TypPrikazu.Goal)
            {
                RobotPrikaz zrusitCekani = new RobotPrikaz(TypPrikazu.Task, "waitTaskCancel");
                RobotPrikaz robotPrikaz = new RobotPrikaz(TypPrikazu.Macro, "executeMacro", $"{prikaz.Telo}_{robot.Platform}_{prikaz.Hodnota}");                
                robot.ProvedPrikaz(robotPrikaz);
            }
        }

        private void Server_NovyJobPrirazen(Job novyJob)
        {
            if (!SeznamJobu.ToList().Exists(x => x.ID == novyJob.ID))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SeznamJobu.Add(novyJob);
                    Database.ZapisJobInformaciDoDatabaze(novyJob);
                });
            }
            else
            {
                var job = SeznamJobu.Single(x => x.ID == novyJob.ID);
                job.CasDokonceni = novyJob.CasDokonceni;
                Database.AktualizujJobDataVDatabazi(job);
            }
        }

        private async Task NajdiVolnehoRobotaDoSkladu(ICollection<AbstractRobot> ostatniRoboti)
        {
            foreach (AbstractRobot robot in ostatniRoboti)
            {
                AgvStatus status = await robot.StatusRobota();
                if (status.Type != StatusType.InWork)
                {
                    robot.ProvedPrikaz(new RobotPrikaz(TypPrikazu.Macro, "qd", "smtsklad"));
                    break;
                }
            }
        }

        private void Robot_PrijalNovyJob(object sender, object args)
        {
            Job JobArgs = (Job)args;
            if (SeznamJobu.ToList().Exists(x => x.ID == JobArgs.ID))
            {
                var job = SeznamJobu.Single(x => x.ID == JobArgs.ID);
                job.Robot = (AbstractRobot)sender;
                job.CasDokonceni = JobArgs.CasDokonceni;
                Database.AktualizujJobDataVDatabazi(job);
                if (JobArgs.PocatecniGoal.ToLower() == "smtsklad" && Config.SwitchLocation)
                {
                    Task.Factory.StartNew(async() => {
                        await Task.Delay(TimeSpan.FromSeconds(30));
                        var ostatniRoboti = FlotilaRobotu.Collection.Where(x => x != (AbstractRobot)sender && x.Status == Stav.Online).ToList();
                        await NajdiVolnehoRobotaDoSkladu(ostatniRoboti);
                    });
                }
            }
        }
    }
}
