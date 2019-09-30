using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Alita.Models.Struct
{
    public struct DnsInfo
    {
        public DnsInfo(string Hostname, IPAddress IP)
        {
            this.Hostname = Hostname;
            this.IP = IP;
        }

        public string Hostname { get; }
        public IPAddress IP { get; }

    }
}
