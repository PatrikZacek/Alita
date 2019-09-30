using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alita.Models.Interfaces
{
    public interface IPraceSDigitalInputsOutputs<T>
    {
        Task<T[]> PrectiVstupyNaZarizeni();
    }
}
