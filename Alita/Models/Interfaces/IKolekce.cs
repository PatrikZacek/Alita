using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows_MVVM.Framework_MVVM;

namespace Alita.Models.Interfaces
{
    public interface IKolekce<T> where T: IZarizeni
    {
        ObservableCollection<T> Collection { get; set; }

        int PocetOnline { get; set; }
        int PocetOffline { get; set; }
        int PocetInError { get; set; }

        void Add(T Zarizeni);
        void OdpojZarizeni(string Hostname);
        void OdpojVse();
        void PripojVse();
        void PripojZarizeni(string Hostname);
    }

}
