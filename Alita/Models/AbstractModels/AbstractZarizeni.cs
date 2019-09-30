using Alita.Models.Interfaces;
using Alita.Models.Objects;
using Alita.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows_MVVM.Framework_MVVM;

namespace Alita.Models.AbstractModels
{
    public abstract class AbstractZarizeni : ObservableObject, IZarizeni
    {
        protected AbstractZarizeni()
        { }

        private long deviceId;
        public long Device_ID
        {
            get
            {
                return deviceId;
            }
            set
            {
                deviceId = value;
            }
        }

        private string hostname = "fst_device";
        public string Hostname
        {
            get => hostname;
            set
            {
                hostname = value;
                OnPropertyChanged();
            }
        }

        private string ipAdress = "192.168.1.1";
        public string IpAdress
        {
            get => ipAdress;
            set
            {
                ipAdress = value;
                OnPropertyChanged();
            }
        }

        private int port = 7171;
        public int Port
        {
            get => port;
            set
            {
                port = value;
                OnPropertyChanged();
            }
        }

        private long ping = 0;
        public long Ping
        {
            get => ping;
            set
            {
                ping = value;
                OnPropertyChanged();
                Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(100);
                    PingniNaZarizeni();
                });
            }
        }

        protected bool ZarizeniJeDlePinguOnline = false;
        protected virtual void PingniNaZarizeni()
        {
            try
            {
                Ping = PingNaZarizeni.PingCas(this);
                ZarizeniJeDlePinguOnline = true;
                Status = Stav.Online;
            }
            catch (PingNaZarizeniException pingException)
            {
                ZarizeniJeDlePinguOnline = false;
                Status = Stav.Offline;
                Ping = 0;
                var zarizeniArgs = new ZarizeniArgs(Status, Ping, $"!CHYBA!", pingException);
                OnStatusChanged(zarizeniArgs);
            }
            catch (Exception exception)
            {
                ZarizeniJeDlePinguOnline = false;
                Status = Stav.Offline;
                Ping = 0;
                var zarizeniArgs = new ZarizeniArgs(Status, Ping, $"!CHYBA!", exception);
                OnStatusChanged(zarizeniArgs);                
            }
        }

        public abstract bool Connect();
        public abstract bool Disconnect();

        private Stav status = Stav.Offline;
        public Stav Status
        {
            get => status; set
            {
                if (status != value)
                    OnStavPripojeniChanged(status, value);
                status = value;
                OnPropertyChanged();                
            }
        }

        public event ZarizeniHandler StatusChanged;
        public virtual void OnStatusChanged(ZarizeniArgs args)
        {
            StatusChanged?.Invoke(this, args);
        }

        public event ZarizeniZmeniloStavStav StavChanged;
        protected virtual void OnStavPripojeniChanged(Stav PredchoziStav, Stav NovyStavStatus)
        {
            StavChanged?.Invoke(PredchoziStav, Status);
        }

        public static void DeviceToZarizeni(AbstractZarizeni zarizeni, Device zarizeniZdtb)
        {
            zarizeni.Hostname = zarizeniZdtb.Hostname;
            zarizeni.IpAdress = zarizeniZdtb.Ip;
            zarizeni.Device_ID = zarizeniZdtb.Id;
            zarizeni.Port = unchecked((int)zarizeniZdtb.Port);
        }
    }
}
