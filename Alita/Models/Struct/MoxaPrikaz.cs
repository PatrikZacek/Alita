using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alita.Models.Struct
{
    public struct MoxaPrikaz
    {
        public MoxaPrikaz(string Hostname, int Pin, bool Hodnota)
        {
            hostname = Hostname;
            pin = Pin;
            hodnota = Hodnota;
        }

        private readonly string hostname;
        public string Hostname
        {
            get => hostname;
        }

        private readonly int pin;
        public int Pin
        {
            get => pin;
        }

        private readonly bool hodnota;
        public bool Hodnota
        {
            get => hodnota;
        }
    }
}
