using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alita.Models.Objects
{
    public abstract class DIO
    {
        public DIO(IO Typ)
        {
            typ = Typ;
        }

        public DIO(IO Typ, string Nazev) : this(Typ)
        {
            nazev = Nazev;
        }

        public DIO(IO Typ, string Nazev, bool Hodnota) :  this(Typ, Nazev)
        {
            hodnota = Hodnota;
        }

        private readonly IO typ;
        public IO Typ => typ;


        private readonly string nazev;
        public string Nazev => nazev;

        private readonly bool hodnota;
        public bool Hodnota => hodnota;

    }

    public enum IO
    {
        Output = 0,
        Input = 1
    }
}
