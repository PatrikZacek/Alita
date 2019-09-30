using Alita.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows_MVVM.Framework_MVVM;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using System.Windows.Media;
using System.Windows;
using System.Threading;

namespace Alita.Models.AbstractModels
{
    public abstract class AbstractKolekce<T> : ObservableObject, IKolekce<T> where T : IZarizeni
    {
        private CancellationTokenSource Cancellation { get; set; }
        private CancellationToken CancellationToken { get; set; }

        protected AbstractKolekce()
        {
            Cancellation = new CancellationTokenSource();
            CancellationToken = Cancellation.Token;
        }

        protected AbstractKolekce(IEnumerable<T> vstupniKolekce) : this()
        {
            Collection = new ObservableCollection<T>(vstupniKolekce);
            foreach (T zarizeni in Collection)
            {
                zarizeni.StavChanged += (s, d) => UpdatujHodnotyGrafu(); 
            }
            NastavHodnotyProGraf();
        }

        private ObservableCollection<T> collection = new ObservableCollection<T>();
        public ObservableCollection<T> Collection { get => collection; set { collection = value; OnPropertyChanged(); } }

        private int pocetOnline = 0;
        public int PocetOnline
        {
            get => pocetOnline;
            set
            {
                pocetOnline = value;
                OnPropertyChanged();
            }
        }

        private int pocetOffline = 0;
        public int PocetOffline
        {
            get => pocetOffline;
            set
            {
                pocetOffline = value; OnPropertyChanged();
            }
        }

        private int pocetInError = 0;
        public int PocetInError
        {
            get => pocetInError;
            set
            {
                pocetInError = value;
                OnPropertyChanged();
            }
        }

        private SeriesCollection hodnotyProGraf = new SeriesCollection();
        public SeriesCollection HodnotyProGraf { get => hodnotyProGraf; set { hodnotyProGraf = value; OnPropertyChanged(); } }

        protected void NastavHodnotyProGraf()
        {
            PocetOnline = Collection.Where(x => x.Status == Stav.Online).Count();
            PocetOffline = Collection.Where(x => x.Status == Stav.Offline).Count();
            PocetInError = Collection.Where(x => x.Status == Stav.InError).Count();

            HodnotyProGraf = new SeriesCollection {
                new PieSeries{ Title = "Online", Values = new ChartValues<ObservableValue>
                { new ObservableValue(PocetOnline)}, DataLabels = false, Fill = new SolidColorBrush(Colors.Green) },
                new PieSeries{ Title = "InError", Values = new ChartValues<ObservableValue>
                { new ObservableValue(PocetInError)}, DataLabels = false, Fill = new SolidColorBrush(Colors.DarkRed) },
                new PieSeries{ Title = "Offline", Values = new ChartValues<ObservableValue>
                { new ObservableValue(PocetOffline)}, DataLabels = false, Fill = new SolidColorBrush(Colors.Orange) }
            };
        }

        protected async Task UpdatujHodnotyGrafu()
        {
            await Task.Delay(10);
            var aktualniPocetOffline = Collection.Where(x => x.Status == Stav.Offline).Count();
            var aktualniPocetOnline = Collection.Where(x => x.Status == Stav.Online).Count();
            var aktualniPocetInError = Collection.Where(x => x.Status == Stav.InError).Count();

            if (aktualniPocetOffline != PocetOffline)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    PocetOffline = aktualniPocetOffline;
                    HodnotyProGraf.Single(x => x.Title == "Offline").Values = new ChartValues<ObservableValue> { new ObservableValue(PocetOffline) };
                });

            }
            if (aktualniPocetOnline != PocetOnline)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    PocetOnline = aktualniPocetOnline;
                    HodnotyProGraf.Single(x => x.Title == "Online").Values = new ChartValues<ObservableValue> { new ObservableValue(PocetOnline) };
                });
            }

            if (aktualniPocetInError != PocetInError)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    PocetInError = aktualniPocetInError;
                    HodnotyProGraf.Single(x => x.Title == "InError").Values = new ChartValues<ObservableValue> { new ObservableValue(PocetInError) };
                });
            }
        }

        public void Add(T Zarizeni) => Collection.Add(Zarizeni);
        public void OdpojVse()
        {
            foreach (T zarizeni in Collection)
            {
                zarizeni.Disconnect();
            }
            UpdatujHodnotyGrafu();
        }
        public void OdpojZarizeni(string Hostname)
        {
            var zarizeniZcolekce = Collection.Single(x => x.Hostname.ToLower() == Hostname.ToLower());
            zarizeniZcolekce.Disconnect();
            UpdatujHodnotyGrafu();
        }
        public void PripojVse()
        {
            foreach (T zarizeni in Collection)
            {
                zarizeni.Connect();
            }
            UpdatujHodnotyGrafu();
        }
        public void PripojZarizeni(string Hostname)
        {
            var zarizeniZcolekce = Collection.Single(x => x.Hostname.ToLower() == Hostname.ToLower());
            zarizeniZcolekce.Connect();
            UpdatujHodnotyGrafu();
        }
    }
}
