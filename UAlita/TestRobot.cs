using Alita.Models.AbstractModels;
using Alita.Models.Interfaces;
using Alita.Models.Objects;
using Alita.Models.Struct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAlita
{
    public class TestRobot : AbstractRobot
    {
        public TestRobot()
        { }

        private long deviceId;
        public long Device_ID
        {
            get
            {
                return deviceId;
            }
            set
            {
                deviceId = value;
            }
        }

        public override PrikazHandler<RobotPrikaz> ZarizeniZasilaPrikaz { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override bool Connect()
        {
            throw new NotImplementedException();
        }

        public override bool Disconnect()
        {
            throw new NotImplementedException();
        }

        public override Task<VstupVystup[]> PrectiVstupyNaZarizeni()
        {
            throw new NotImplementedException();
        }

        public override void ProvedPrikaz(RobotPrikaz prikaz)
        {
            throw new NotImplementedException();
        }

        public override Task<AgvStatus> StatusRobota()
        {
            throw new NotImplementedException();
        }
    }
}
