using Alita.Models.AbstractModels;
using Alita.Models.Struct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAlita
{
    public class TestServer : AbstractRobotServer<AbstractRobot>
    {
        public TestServer()
        { }

        private long deviceId;
        public long Device_ID
        {
            get { return deviceId; }
            set { deviceId = value; }
        }

        public override bool Connect()
        {
            throw new NotImplementedException();
        }

        public override bool Disconnect()
        {
            throw new NotImplementedException();
        }

        public override Task<AbstractRobot[]> PripojeniRoboti()
        {
            throw new NotImplementedException();
        }

        public override void ProvedPrikaz(RobotPrikaz prikaz)
        {
            throw new NotImplementedException();
        }
    }
}
