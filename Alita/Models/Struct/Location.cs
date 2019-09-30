using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alita.Models.Struct
{
    public struct Location
    {
        public Location(long X, long Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public long X { get; }
        public long Y { get; }
    }
}
