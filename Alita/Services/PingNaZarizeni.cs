using Alita.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Alita.Services
{
    public static class PingNaZarizeni
    {
        public static long PingCas(IZarizeni zarizeni)
        {
            Ping pingClient = new Ping();
            IPAddress iPAddress = new IPAddress(1111);
            IPAddress.TryParse(zarizeni.IpAdress, out iPAddress);
            var respond = pingClient.Send(iPAddress);
            if (respond.Status != IPStatus.Success) throw new PingNaZarizeniException($"Cannot ping to device {zarizeni.Hostname}", respond.Status);
            return respond.RoundtripTime;
        }       
    }

    public class PingNaZarizeniException : Exception
    {
        public PingNaZarizeniException()
        {
        }

        public PingNaZarizeniException(string message) : base(message)
        {
        }

        public PingNaZarizeniException(string message, IPStatus status) : this(message)
        {
            Status = status;
        }

        public PingNaZarizeniException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PingNaZarizeniException(string message, Exception innerException, IPStatus status) : base(message,innerException)
        {
            Status = status;
        }

        protected PingNaZarizeniException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        private IPStatus ipStatus;
        public IPStatus Status
        {
            get { return ipStatus; }
            set { ipStatus = value; }
        }

    }
}
