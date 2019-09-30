using Alita.Models.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Alita.Models.AbstractModels;
using Alita.Models;
using Alita.Models.Interfaces;
using System.Data.Entity.Validation;

namespace Alita.Services
{
    public class DatabaseConnection : IDatabaseConn
    {
        private AlitaDatabase Alita { get; set; }

        protected DatabaseConnection()
        { }

        public DatabaseConnection(bool NovaInstance = true)
        {
            if (NovaInstance) VytvorInstanci();
        }

        public DatabaseConnection(string DataSource, string InitialCatalog, string UserId)
        {
            this.DataSource = DataSource;
            this.InitialCatalog = InitialCatalog;
            this.UserId = UserId;
            VytvorInstanci();
        }

        private string dataSource;
        public string DataSource
        {
            get { return dataSource; }
            set { dataSource = value; }
        }

        private string initialCatalog;
        public string InitialCatalog
        {
            get { return initialCatalog; }
            set { initialCatalog = value; }
        }

        private string userId;
        public string UserId
        {
            get { return userId; }
            set { userId = value; }
        }
        
        private AlitaDatabase VytvorInstanci()
        {
            AlitaDatabase alita = Pripojeni.VytvorInstanci(true,DataSource, InitialCatalog, UserId, "Omron_192");
            alita.Configuration.LazyLoadingEnabled = true;
            alita.Configuration.AutoDetectChangesEnabled = true;
            return alita;
        }

        private void SetOnline(AlitaDatabase database) => database?.Database.Connection.Open();
        private void SetOffline(AlitaDatabase database) => database?.Database.Connection.Close();

        public RobotKolekce FlotilaDatabaze(bool IsActive = true)
        {
            List<AbstractRobot> vyslednaFlotila = new List<AbstractRobot>();
            using (AlitaDatabase db = VytvorInstanci())
            {
                SetOnline(db);
                var obsah = db.Agv.Include(x => x.Device).Include(x => x.Platform).Where(x => x.Device.IsActive == IsActive);
                List<Agv> agvs = obsah.ToList();
                agvs.ForEach(delegate (Agv agv)
                {
                    Device zarizeni = agv.Device;
                    long supplierId = zarizeni.Supplier_FK ?? 0;
                    AbstractRobot robot = RobotModelPodleDodoavatele(supplierId);
                    AbstractZarizeni.DeviceToZarizeni(robot, zarizeni);
                    robot.Platform = AbstractRobot.GetPlatformType(agv.Platform);
                    robot.Hostname = agv.NameOnServer;
                    vyslednaFlotila.Add(robot);
                });
                SetOffline(db);
            }
            return new RobotKolekce(vyslednaFlotila);
        }
        private AbstractRobot RobotModelPodleDodoavatele(long SupplierId)
        {
            AbstractRobot vysledek = null;
            switch(SupplierId)
            {
                case 1:
                    vysledek = new LynxModel();
                    break;
                default: throw new ArgumentException(nameof(SupplierId));
            }
            return vysledek;
        }

        public ExterniZarizeniKolekce ExterniZarizeniZDatabaze(bool IsActive = true)
        {
            List<AbstractExternalDevice> vyslednaZarizeni = new List<AbstractExternalDevice>();
            using (AlitaDatabase db = VytvorInstanci())
            {
                SetOnline(db);
                var obsah = db.ExternalDevice.Where(x => x.Device.IsActive == IsActive).Include(x => x.Device);
                List<ExternalDevice> externalDevices = obsah.ToList();
                externalDevices.ForEach(delegate (ExternalDevice externalDevice)
                {
                    Device zarizeni = externalDevice.Device;
                    long supplierId = zarizeni.Supplier_FK ?? 0;
                    AbstractExternalDevice externiZarizeni = ExterniZarizeniModelPodleDodavatele(supplierId);
                    AbstractZarizeni.DeviceToZarizeni(externiZarizeni, zarizeni);
                    externiZarizeni.Hostname = externalDevice.Callname;
                    vyslednaZarizeni.Add(externiZarizeni);
                });
                SetOffline(db);
            }
            return new ExterniZarizeniKolekce(vyslednaZarizeni);
        }
        private AbstractExternalDevice ExterniZarizeniModelPodleDodavatele(long SupplierId)
        {
            AbstractExternalDevice vysledek = null;
            switch (SupplierId)
            {
                case 5:
                    vysledek = new MoxaModel();
                    break;
                default: throw new ArgumentException(nameof(SupplierId));
            }
            return vysledek;
        }

        public RobotServerKolekce ServeryZDatabaze(bool IsActive = true)
        {
            List<AbstractRobotServer<AbstractRobot>> vysledneServery = new List<AbstractRobotServer<AbstractRobot>>();
            using (AlitaDatabase db = VytvorInstanci())
            {
                SetOnline(db);
                var obsah = db.RobotServer.Include(x => x.Device).Where(x => x.Device.IsActive == IsActive);
                List<RobotServer> servers = obsah.ToList();
                servers.ForEach(delegate (RobotServer server)
                {
                    Device zarizeni = server.Device;
                    long supplierId = zarizeni.Supplier_FK ?? 0;
                    AbstractRobotServer<AbstractRobot> robotServer = RobotServerModelPodleDodavatele(supplierId);
                    AbstractZarizeni.DeviceToZarizeni(robotServer, zarizeni);
                    vysledneServery.Add(robotServer);
                });
                SetOffline(db);
            }
            return new RobotServerKolekce(vysledneServery);
        }
        private AbstractRobotServer<AbstractRobot> RobotServerModelPodleDodavatele(long SupplierId)
        {
            AbstractRobotServer<AbstractRobot> vysledek = null;
            switch (SupplierId)
            {
                case 1:
                    vysledek = new OmronRobotServer();
                    break;
                default: throw new ArgumentException(nameof(SupplierId));
            }
            return vysledek;
        }

        public void ZapisJobInformaciDoDatabaze(Models.Job JobData)
        {
            using (AlitaDatabase db = VytvorInstanci())
            {
                SetOnline(db);
                Job jobDataDatabaze = new Job();                
                JobModelToJobEntity(JobData, ref jobDataDatabaze);
                if (JobData.CasDokonceni != DateTime.MinValue)
                    jobDataDatabaze.Finnished = JobData.CasDokonceni;
                var serverZDatabaze = db.RobotServer.Single(x => x.Device_Id == JobData.Server.Device_ID);
                jobDataDatabaze.Server_Id = JobData.Server.Device_ID;
                serverZDatabaze.Job.Add(jobDataDatabaze);
                jobDataDatabaze.RobotServer = serverZDatabaze;
                jobDataDatabaze.Id = VolneJobId();
                db.Job.Add(jobDataDatabaze);
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    db.Job.Remove(jobDataDatabaze);
                    jobDataDatabaze.Id++;
                    db.Job.Add(jobDataDatabaze);
                    db.SaveChanges();

                }
                
                SetOffline(db);
            }
        }

        private long posledniId = 1;

        private long VolneJobId()
        {
            long vysledek = posledniId;
            using (AlitaDatabase db = VytvorInstanci())
            {
                List<long> allJobIds = db?.Job.Select(x => x.Id).ToList();
                while (allJobIds.Contains(vysledek))
                    vysledek++;
            }
            posledniId = vysledek;
            return vysledek;
        }

        private void JobModelToJobEntity(Models.Job JobModel, ref Job JobEntity)
        {
            JobEntity.IdOnServer = JobModel.ID;
            JobEntity.SourceGoal = JobModel.PocatecniGoal;
            JobEntity.TargetGoal = JobModel.KoncovyGoal;
            JobEntity.Queued = JobModel.CasVzniku;
            if (JobModel.CasDokonceni != DateTime.MinValue)
                JobEntity.Finnished = JobModel.CasDokonceni;
        }

        public void AktualizujJobDataVDatabazi(Models.Job JobData)
        {
            using (AlitaDatabase db = VytvorInstanci())
            {
                var obsah = db.Job.Where(x => x.IdOnServer == JobData.ID);
                Job jobEntity = obsah.First();
                JobModelToJobEntity(JobData, ref jobEntity);
                var agv = db.Agv.Include(x => x.Device_Id)
                    .Where(x => x.Device_Id == JobData.Robot.Device_ID)
                    .First();
                jobEntity.Agv_Id = agv.Id;
                jobEntity.Agv = agv;
                db.SaveChanges();
            }
        }
    }
}
