using Alita.Models.AbstractModels;
using Alita.Models.Struct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alita.Models.Interfaces
{
    public interface IRobotServer<T> : IPrikazKonzole<RobotPrikaz> where T: AbstractRobot
    {
        Task<T[]> PripojeniRoboti();
        event ZalozenNovyJob NovyJobPrirazen;
    }
    public delegate void ZalozenNovyJob(Job novyJob);
}
