using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alita.Models.Interfaces
{
    public interface IHlidaciPes<T>
    {
       IPraceSDigitalInputsOutputs<T> DigitalInputsOutputs { get; }

        void Zapni();
        void Vypni();

        event NahlasNalez HlidaciPesNecoZaznamenal;
    }

    public delegate void NahlasNalez(object Nalez); 
}
