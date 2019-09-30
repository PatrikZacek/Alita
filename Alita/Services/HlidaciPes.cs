using Alita.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Alita.Services
{
    public class HlidaciPes<T> : IHlidaciPes<T>
    {
        public IPraceSDigitalInputsOutputs<T> DigitalInputsOutputs { get; }

        public HlidaciPes()
        { }

        public HlidaciPes(IPraceSDigitalInputsOutputs<T> praceSDigitalInputs) => DigitalInputsOutputs = praceSDigitalInputs;

        private CancellationTokenSource Cancellation { get; set; }
        private CancellationToken CancellationToken { get; set; }
        private bool Status = false;

        public void Vypni()
        {
            Status = false;
            Cancellation?.Dispose();
        }

        public void Zapni()
        {
            if (Status) return;
            Cancellation = new CancellationTokenSource();
            CancellationToken = Cancellation.Token;
            Status = true;
            TaskFactory taskFactory = new TaskFactory(CancellationToken);
            taskFactory.StartNew(HlidejSignalyNaVstupechZarizeni, CancellationToken);
        }

        private async Task HlidejSignalyNaVstupechZarizeni()
        {
            while (Status)
            {
                var referencniPoleHodnotNaVstupech = await DigitalInputsOutputs.PrectiVstupyNaZarizeni();
                await Task.Delay(30);
                var kontrolniPoleHodnotNaVstupech = await DigitalInputsOutputs.PrectiVstupyNaZarizeni();
                PorovnejVstupniHodnotyVycteneVcase(referencniPoleHodnotNaVstupech, kontrolniPoleHodnotNaVstupech);
            }            
        }

        private void PorovnejVstupniHodnotyVycteneVcase(T[] referencniPole, T[] kontrolniPole)
        {
            var poleJsouStejna = referencniPole.Equals(kontrolniPole);
            if (poleJsouStejna) return;            

            for (int index = 0; index < referencniPole.Count(); index++)
            {
                var referencniHodnota = referencniPole[index];
                var kontrolniHodnota = kontrolniPole[index];

                if (!referencniHodnota.Equals(kontrolniHodnota))
                {
                    HlidaciPesNecoZaznamenal?.Invoke(new Hlaseni(index.ToString(), kontrolniHodnota));
                }
            }
        }

        public event NahlasNalez HlidaciPesNecoZaznamenal;       
    }

    public class Hlaseni
    {
        public Hlaseni(string Predmet, object Zprava)
        {
            this.Predmet = Predmet;
            this.Zprava = Zprava;
        }

        public string Predmet { get; }
        public object Zprava { get; }
    }
}
