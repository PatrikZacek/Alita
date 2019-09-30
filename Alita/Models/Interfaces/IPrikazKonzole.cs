using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alita.Models.Interfaces
{
    public interface IPrikazKonzole<T> where T : struct
    {
        void ProvedPrikaz(T prikaz);
        PrikazHandler<T> ZarizeniZasilaPrikaz { get; set; }
    }

    public delegate void PrikazHandler<T>(IZarizeni sender, T prikaz);
}
