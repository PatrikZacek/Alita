using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Alita.Models;
using Alita.Models.Struct;
using Alita.Services;
using Alita.Models.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.IO;

namespace UAlita
{
    [TestClass]
    public class GeneralTests
    {
        private MainViewModel mainViewModel;

        [TestInitialize]
        public void Initialize()
        {
            mainViewModel = new MainViewModel();
        }

        [TestMethod]
        public void RobotModelTest()
        {
            LynxModel lynx = new LynxModel
            {
                Hostname = "Ruda",
                IpAdress = "10.226.174.105",
                Port = 7171
            };
            var resultOfConnection = lynx.Connect();

            Assert.IsFalse(resultOfConnection);
        }

        [TestMethod]
        public void TestRobotPrikaz()
        {
            var robotPrikazProPorovnani = new RobotPrikaz(TypPrikazu.Trigger, "smtsklad", "open",0);
            string temp = "alita_trigger_smtsklad_open_0";
            var prikaz = RobotPrikaz.GetRobotPrikaz(temp);

            Assert.AreEqual(robotPrikazProPorovnani, prikaz);
        }

        private LynxModel odstavenyRobot = new LynxModel {
            Hostname = "Ruda",
            IpAdress = "10.226.174.105"
        };

        private LynxModel spustenyRobot = new LynxModel {
            Hostname = "Jana",
            IpAdress = "10.226.178.252"
        };

        [TestMethod]
        [ExpectedException(typeof(PingNaZarizeniException))]
        public void PingNaVypnuteZarizeni()
        {
            long result = PingNaZarizeni.PingCas(odstavenyRobot);
        }

        [TestMethod]
        public async Task PingNaZapnuteZarizeni()
        {
            spustenyRobot.Connect();
            long result = PingNaZarizeni.PingCas(spustenyRobot);
            spustenyRobot.Ping = result;
            await Task.Delay(1000);
            //var status = spustenyRobot.GetStatus();
            await Task.Delay(1000);
            spustenyRobot.Disconnect();

            Assert.AreNotEqual(0, spustenyRobot.Ping);
        }

        [TestMethod]
        public async Task ZjistiDostupnaMacraAsync()
        {
            spustenyRobot.Connect();
            List<string> odpovedi = await spustenyRobot.PrectiKomunikaciZArclServeru();
            RobotPrikaz getMacros = new RobotPrikaz(TypPrikazu.Task, "getMacros", string.Empty);
            spustenyRobot.ProvedPrikaz(getMacros);
            var task = await spustenyRobot.PrectiKomunikaciZArclServeru();
            Assert.IsTrue(task.Count > 0);
        }

        [TestMethod]
        public void OfflineMoxaTest()
        {
            MoxaModel moxa = new MoxaModel
            {
                Hostname = "laser",
                IpAdress = "10.226.174.111"
            };  

            moxa.Connect();

            Assert.IsFalse(moxa.Status == Stav.Online);
        }

        [TestMethod]
        public void OnlineMoxaTest()
        {
            MoxaModel moxa = new MoxaModel
            {
                Hostname = "smtsklad",
                IpAdress = "10.226.174.110",
                Port = 502
            };

            moxa.Connect();

            Assert.IsTrue(moxa.Status == Stav.Online);
        }

        [TestMethod]
        public void DisconnectMoxaTest()
        {
            MoxaModel moxa = new MoxaModel
            {
                Hostname = "smtsklad",
                IpAdress = "10.226.174.110",
                Port = 8888
            };
            moxa.NastavModbusKlienta();
            moxa.Disconnect();

            Assert.IsFalse(moxa.Status == Stav.Online);
        }

        private MoxaModel smtSklad = new MoxaModel {
            Hostname = "smtsklad",
            IpAdress = "10.226.174.110",
            Port = 502
        };

        [TestMethod]
        public void OtevriVrata()
        {
            smtSklad.Connect();
            MoxaPrikaz otevriVrata = new MoxaPrikaz("smtsklad", 1, true);
            MoxaPrikaz zavriVrata = new MoxaPrikaz("smtsklad", 1, false);

            smtSklad.ProvedPrikaz(otevriVrata);
            //smtSklad.ProvedPrikaz(zavriVrata);

            smtSklad.Disconnect();
        }

        [TestMethod]
        public void PrectiVstupyNaMoxaZarizeniAsync()
        {
            smtSklad.Connect();
            //var result = smtSklad.PrectiHodnotuNaVystupu(1);
            smtSklad.Disconnect();
            //Assert.IsNotNull(result);
        }

        private MoxaPrikaz Prikaz { get; set; }

        [TestMethod]
        public async Task EnterpriseManagerClientTestAsync()
        {
            OmronRobotServer omronRobotServer = new OmronRobotServer {
                Hostname = "omron",
                IpAdress = "10.217.158.194",
                Port = 7170
            };

            omronRobotServer.Connect();
            var robotiServer = await omronRobotServer.PripojeniRoboti();           

            Assert.IsNotNull(robotiServer);
        }

        [TestMethod]
        public void NajdiIpAdresuRobota()
        {
            string hostname = "AGV_Luky";
                         DnsInfo info = GetHostNameAndIPAdress.DnsInfoFromHostname(hostname);

            Assert.IsTrue(info.IP.ToString() == "10.226.178.252");
        }

        [TestMethod]
        public async Task ZjistiStavRobota()
        {
            LynxModel lynx = new LynxModel
            {
                Hostname = "Jana",
                IpAdress = "10.226.178.252"
            };
            lynx.Connect();           
            AgvStatus status = await lynx.StatusRobota();

            Assert.IsNotNull(status);
        }
    }
}
