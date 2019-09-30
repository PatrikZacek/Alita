using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alita.Models.Struct
{
    public struct AgvStatus
    {
        public AgvStatus(StatusType type, double LocalizationScore, Location Location, MapGoal? Goal = null)
        {
            Type = type;
            this.LocalizationScore = LocalizationScore;
            AccurateLocation = Location;
            this.Goal = Goal;
        }

        public StatusType Type { get; }
        public double LocalizationScore { get; }

        public Location AccurateLocation { get; }
        public MapGoal? Goal { get; }
        
    }

    public enum StatusType
    {
        Unknown = -2,
        Docked = -1,
        Docking = 0,
        Parking = 1,
        InWork = 2
    }
}
