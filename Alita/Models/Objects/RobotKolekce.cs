using Alita.Models.AbstractModels;
using Alita.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alita.Models.Objects
{
    public class RobotKolekce : AbstractKolekce<AbstractRobot>
    {
        public RobotKolekce()
        {
        }

        public RobotKolekce(IEnumerable<AbstractRobot> vstupniKolekce) : base(vstupniKolekce)
        {
        }

    }
}
