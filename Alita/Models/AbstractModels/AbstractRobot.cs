using Alita.Models.Interfaces;
using Alita.Models.Objects;
using Alita.Models.Struct;
using Alita.Services;
using Mobile.Communication.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows_MVVM.Framework_MVVM;

namespace Alita.Models.AbstractModels
{
    public abstract class AbstractRobot : AbstractZarizeni, IPraceSDigitalInputsOutputs<VstupVystup>, IPrikazKonzole<RobotPrikaz>
    {
        protected AbstractRobot() : base()
        { }

        private PlatformType platform = PlatformType.CTS;
        public PlatformType Platform
        {
            get => platform;
            set => platform = value;
        }

        protected AbstractRobot(ZmenaJobParameteruHandler jobParameteruHandler) : this()
        {
            ZmenaJob = jobParameteruHandler;
        }

        private ZmenaJobParameteruHandler zmenaJob;
        public virtual ZmenaJobParameteruHandler ZmenaJob { get => zmenaJob; set => zmenaJob = value; }

        public abstract void ProvedPrikaz(RobotPrikaz prikaz);

        public abstract PrikazHandler<RobotPrikaz> ZarizeniZasilaPrikaz { get; set; }              

        public abstract Task<VstupVystup[]> PrectiVstupyNaZarizeni();

        public abstract Task<AgvStatus> StatusRobota();

        public static PlatformType GetPlatformType(Platform platform)
        {
            try
            {
                PlatformType vysledek = (PlatformType)Enum.Parse(typeof(PlatformType), platform.Type);
                return vysledek;
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException($"Neznámý typ platformy: {platform.Type}. Zkontrolujte zdali máte aktuální verzi programu", e);
            }
            catch
            {
                throw;
            }
        }
    }

    public delegate void ZmenaJobParameteruHandler(object sender, object args);    
    public enum PlatformType
    {
        CTS,
        ASYS
    }
}
