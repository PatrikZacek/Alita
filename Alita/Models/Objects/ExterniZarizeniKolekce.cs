using Alita.Models.AbstractModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alita.Models.Objects
{
    public class ExterniZarizeniKolekce : AbstractKolekce<AbstractExternalDevice>
    {
        public ExterniZarizeniKolekce()
        {
        }

        public ExterniZarizeniKolekce(IEnumerable<AbstractExternalDevice> vstupniKolekce) : base(vstupniKolekce)
        {
        }
    }
}
