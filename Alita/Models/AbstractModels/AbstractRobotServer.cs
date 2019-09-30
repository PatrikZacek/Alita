using Alita.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alita.Models.Struct;
using Windows_MVVM.Framework_MVVM;
using Alita.Models.Objects;

namespace Alita.Models.AbstractModels
{
    public abstract class AbstractRobotServer<T> : AbstractZarizeni, IRobotServer<T> where T : AbstractRobot
    {      
        public abstract Task<T[]> PripojeniRoboti();

        public PrikazHandler<RobotPrikaz> ZarizeniZasilaPrikaz { get; set; }

        public abstract void ProvedPrikaz(RobotPrikaz prikaz);

        public event ZalozenNovyJob NovyJobPrirazen;
        protected virtual void OnNovyJobPrirazen(Job job)
        {
            NovyJobPrirazen?.Invoke(job);
        }
        
    }
}
