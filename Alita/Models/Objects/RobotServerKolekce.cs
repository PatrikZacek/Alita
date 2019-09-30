using Alita.Models.AbstractModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alita.Models.Objects
{
    public class RobotServerKolekce : AbstractKolekce<AbstractRobotServer<AbstractRobot>>
    {
        public RobotServerKolekce()
        {
        }

        public RobotServerKolekce(IEnumerable<AbstractRobotServer<AbstractRobot>> vstupniKolekce) : base(vstupniKolekce)
        {
        }
    }
}
