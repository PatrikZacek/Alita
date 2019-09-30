using Alita.Models.Struct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Alita.Services
{
    public static class GetHostNameAndIPAdress
    {
        public static DnsInfo DnsInfoFromHostname(string hostname)
        {
            var ipAdresyZDns = Dns.GetHostAddresses(hostname);
            return new DnsInfo(hostname, ipAdresyZDns.First());
        }
    }
}
