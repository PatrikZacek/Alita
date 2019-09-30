using Alita.Models.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alita.Models.Objects
{
    public class VstupVystup : DIO
    {
        public VstupVystup(IO Typ) : base(Typ)
        {
        }

        public VstupVystup(IO Typ, string Nazev) : base(Typ, Nazev)
        {
        }

        public VstupVystup(IO Typ, string Nazev, bool Hodnota) : base(Typ, Nazev, Hodnota)
        {
        }
    }
}
