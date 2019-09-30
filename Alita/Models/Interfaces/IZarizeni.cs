using Alita.Models.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alita.Models.Interfaces
{
    public interface IZarizeni
    {
        string Hostname { get; set; }
        string IpAdress { get; set; }
        int Port { get; set; }
        long Ping { get; set; }
        Stav Status { get; set; }          
        event ZarizeniHandler StatusChanged;
        event ZarizeniZmeniloStavStav StavChanged;

        bool Connect();
        bool Disconnect();
        
    }

    public delegate void ZarizeniHandler(object sender, ZarizeniArgs e);
    public delegate void ZarizeniZmeniloStavStav(Stav PredchoziStav, Stav NovyStavStatus);
    public enum Stav
    {
        Offline = 0x0,
        Online = 0x1,
        InError = -0x1
    }
}
