using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows_MVVM.Framework_MVVM;

namespace Alita.Models.AbstractModels
{
    public abstract class AbstractJob : ObservableObject
    {
        protected AbstractJob()
        { }

        private string id;
        public string ID { get => id; set => id = value; }

        private DateTime casVzniku = DateTime.Now;
        public DateTime CasVzniku { get => casVzniku; set => casVzniku = value; }

        private DateTime casDokonceni;
        public DateTime CasDokonceni
        {
            get => casDokonceni;
            set
            {
                casDokonceni = value;
                OnPropertyChanged();
            }
        }

        private string pocatecniGoal = string.Empty;
        public string PocatecniGoal { get => pocatecniGoal; set => pocatecniGoal = value; }

        private string koncovyGoal = string.Empty;
        public string KoncovyGoal { get => koncovyGoal; set => koncovyGoal = value; }

        private StavJobu stav = StavJobu.Queued;
        public StavJobu Stav
        {
            get => stav;
            set
            {
                stav = value;
                OnPropertyChanged();
            }
        }

        private AbstractRobot robot;
        public AbstractRobot Robot
        {
            get => robot;
            set
            {
                robot = value;
                OnPropertyChanged();
            }
        }

        private AbstractRobotServer<AbstractRobot> server;
        public AbstractRobotServer<AbstractRobot> Server
        {
            get
            {
                return server;
            }
            set
            {
                server = value;
            }
        }     
    }

    public enum StavJobu
    {
        Queued = 0,
        InProgress = 1,
        Finnished = 2,
        Cancelled = 3
    }
}
