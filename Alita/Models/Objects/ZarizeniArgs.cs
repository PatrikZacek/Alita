using Alita.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alita.Models.Objects
{
    public class ZarizeniArgs : EventArgs
    {
        public ZarizeniArgs()
        {
        }

        public ZarizeniArgs(Stav Online) => online = Online;

        public ZarizeniArgs(Stav Online, string Popis) : this(Online) => popis = Popis;

        public ZarizeniArgs(Stav Online, long Ping) : this(Online) => ping = Ping;

        public ZarizeniArgs(Stav Online, long Ping, string Popis) : this(Online, Ping) => popis = Popis;

        public ZarizeniArgs(Stav Online, long Ping, string Popis, Exception exception) : this(Online, Ping, Popis) => this.exception = exception;

        private readonly Stav online;
        /// <summary>
        /// True – Online
        /// False – Offline
        /// </summary>
        public Stav Online { get => online; }
        
        private readonly long ping;
        /// <summary>
        /// Latence v ms
        /// </summary>
        public long Ping { get => ping; }

        private readonly string popis;
        /// <summary>
        /// Kratky popis stavu, v pripade chyby pouze uvedeno !CHYBA!
        /// </summary>
        public string Popis { get => popis; }

        private readonly Exception exception;
        /// <summary>
        /// V pripade chyboveho stavu je zde uvedena Exception, ktera zpusobila chybu
        /// </summary>
        public Exception Exception { get => exception; }

    }
}
