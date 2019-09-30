using Alita.Models.Interfaces;
using Alita.Models.Objects;
using Alita.Models.Struct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows_MVVM.Framework_MVVM;

namespace Alita.Models.AbstractModels
{
    public abstract class AbstractExternalDevice : AbstractZarizeni, IPraceSDigitalInputsOutputs<bool>, IPrikazKonzole<MoxaPrikaz>
    {
        protected AbstractExternalDevice() : base()
        { }        

        public abstract PrikazHandler<MoxaPrikaz> ZarizeniZasilaPrikaz { get; set; }

        public abstract void ProvedPrikaz(MoxaPrikaz prikaz);   

        public abstract Task<bool[]> PrectiVstupyNaZarizeni();

       
    }
}
