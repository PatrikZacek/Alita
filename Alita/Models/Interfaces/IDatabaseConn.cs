using Alita.Models.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alita.Models.Interfaces
{
    public interface IDatabaseConn
    {
        string DataSource { get; set; }
        string InitialCatalog { get; set; }
        string UserId { get; set; }

        RobotKolekce FlotilaDatabaze(bool IsActive);
        RobotServerKolekce ServeryZDatabaze(bool IsActive);
        ExterniZarizeniKolekce ExterniZarizeniZDatabaze(bool IsActive);

        void ZapisJobInformaciDoDatabaze(Job JobData);
        void AktualizujJobDataVDatabazi(Job JobData);
    }
}
