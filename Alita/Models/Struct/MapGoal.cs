using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alita.Models.Struct
{
    public struct MapGoal
    {
        public MapGoal(String Name, Location location, GoalType type)
        {
            this.Name = Name;
            Location = location;
            Type = type;
        }

        public string Name { get; }
        public Location Location { get; }
        public GoalType Type { get; }
    }

    public enum GoalType
    {
        Goal = 0,
        Dock = 1
    }
}
