using System;
using Alita.Models;
using Alita.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UAlita
{
    [TestClass]
    public class DbTest
    {
        [TestMethod]
        public void GetInstance()
        {
            Pripojeni pripojeni = new Pripojeni();
            string vysledek = pripojeni.sestavPripojeni("tcp:10.217.158.192,49172", "Alita", "AGV_sa", "Omron_192");

            Assert.Equals(vysledek, @"<add name=""Alita"" connectionString=""metadata = res://*/AlitaDb.csdl|res://*/AlitaDb.ssdl|res://*/AlitaDb.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=tcp:10.217.158.192,49172;initial catalog=Alita;user id=AGV_sa;MultipleActiveResultSets=True;App=EntityFramework&quot;"" providerName=""System.Data.EntityClient"" />");
        }

        [TestMethod]
        public void UpdateJobData()
        {
            Job jobData = new Job();
            jobData.CasDokonceni = DateTime.Now;
            jobData.PocatecniGoal = "smtsklad";
            jobData.ID = "Test";
            jobData.KoncovyGoal = "S12";
            jobData.CasVzniku = DateTime.ParseExact("16.08.2019", "dd.MM.yyyy",null);

            OmronRobotServer server = new OmronRobotServer();
            server.Device_ID = 4;
            jobData.Server = server;
            jobData.Stav = Alita.Models.AbstractModels.StavJobu.Finnished;

            LynxModel robot = new LynxModel();
            robot.Device_ID = 1;
            jobData.Robot = robot;

            DatabaseConnection connection = new DatabaseConnection("tcp:10.217.158.192,49172", "Alita", "AGV_sa");
            connection.AktualizujJobDataVDatabazi(jobData);
        }
    }
}
