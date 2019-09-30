using Alita.Models.AbstractModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alita.Models.Interfaces;
using Alita.Models.Struct;
using EasyModbus;
using System.Net;
using Alita.Services;
using Alita.Models.Objects;
using System.Windows;
using System.Threading;
using System.Diagnostics;

namespace Alita.Models
{
    public class MoxaModel : AbstractExternalDevice
    {
        public MoxaModel() : base()
        {

        }

        private ModbusClient Modbus { get; set; }
        private CancellationTokenSource Cancellation { get; set; }
        private CancellationToken CancellationToken { get; set; }
        private TimeSpan timeoutProZaslaniHodnotyNaVystup = TimeSpan.FromMilliseconds(100);

        public override PrikazHandler<MoxaPrikaz> ZarizeniZasilaPrikaz { get; set; }

        public override bool Connect()
        {
            try
            {
                if (Modbus == null) NastavModbusKlienta();                
                if (ZarizeniJeDlePinguOnline) Status = Stav.Online;
                return true;
            }
            catch (Exception exception)
            {
                var zarizeniArgs = new ZarizeniArgs(Status, Ping, $"!CHYBA!", exception);
                OnStatusChanged(zarizeniArgs);
                return false;
            }
        }

        public void NastavModbusKlienta()
        {
            PingniNaZarizeni();
            Modbus = new ModbusClient(IpAdress, Port);
            ServicePointManager.Expect100Continue = false;
            Modbus.ConnectionTimeout = 100000;
        }

        protected override void PingniNaZarizeni()
        {
            base.PingniNaZarizeni();                  
        }

        public override bool Disconnect()
        {
            try
            {
                Modbus?.Disconnect();
                Status = Stav.Offline;
                return true;
            }
            catch (Exception exception)
            {
                var zarizeniArgs = new ZarizeniArgs(Status, Ping, $"!CHYBA!", exception);
                OnStatusChanged(zarizeniArgs);
                return false;
            }

        }

        public override void ProvedPrikaz(MoxaPrikaz prikaz)
        {
            NastavHodnotuNaPozadovanemPinu(prikaz.Pin, prikaz.Hodnota);
        }

        private void NastavHodnotuNaPozadovanemPinu(int Pin, bool Hodnota)
        {
            try
            {
                Modbus.Connect();
                Stopwatch casovac = new Stopwatch();
                casovac.Start();
                Task.Factory.StartNew(async () => {
                    while (PrectiHodnotuNaVystupu(Pin) != Hodnota || casovac.Elapsed < timeoutProZaslaniHodnotyNaVystup)
                    {
                        Modbus.WriteSingleCoil(Pin, Hodnota);
                        await Task.Delay(10);
                    }
                });                
            }
            catch (Exception e)
            {

            }
        }

        private bool PrectiHodnotuNaVystupu(int Pin)
        {
            return Modbus.ReadCoils(Pin,1).First();
        }
        public override Task<bool[]> PrectiVstupyNaZarizeni()
        {
            return Task.Factory.StartNew(() => {
                var returnValue = Modbus.ReadDiscreteInputs(0, 6);
                return returnValue;
            });            
        }           
    }
}
