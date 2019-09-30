using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alita.Models.Struct
{
    public struct RobotPrikaz : IEquatable<RobotPrikaz>
    {
        
        private const char Separator = '_';
        private const string VolaciZnak = "alita";

        public RobotPrikaz(TypPrikazu typ, string telo, string hodnota = "", int pin = 0)
        {
            this.typ = typ;
            this.telo = telo;
            this.hodnota = hodnota;
            this.pin = pin;
        }

        private readonly TypPrikazu typ;
        public TypPrikazu Typ => typ;

        private readonly string telo;
        public string Telo => telo;

        private readonly string hodnota;
        public string Hodnota => hodnota;

        private readonly int pin;
        public int Pin => pin;

        public static RobotPrikaz GetRobotPrikaz(string vstupniText)
        {
            string vstupniTextBezVolacihoZnaku = vstupniText.ToLower().Replace(VolaciZnak+Separator, "");
            var textArray = vstupniTextBezVolacihoZnaku.ToArray();
            if (!textArray.Contains(Separator)) throw new Exception($"{nameof(GetRobotPrikaz)}", new Exception($"Text doesn't contain any separator – {vstupniText}"));
            var vstupniTextRozdelenDleSeparatoru = vstupniTextBezVolacihoZnaku.Split(Separator);
            Enum.TryParse(vstupniTextRozdelenDleSeparatoru[0], true, out TypPrikazu vyslednyTyp);
            string vysledneTelo = vstupniTextRozdelenDleSeparatoru[1];
            string vyslednaHodnota = string.Empty;
            int vyslednyPin = 0;
            if (vstupniTextRozdelenDleSeparatoru.Count() > 2) vyslednaHodnota = vstupniTextRozdelenDleSeparatoru[2];
            if (vstupniTextRozdelenDleSeparatoru.Count() > 3) int.TryParse(vstupniTextRozdelenDleSeparatoru[3], out vyslednyPin);

            return new RobotPrikaz(vyslednyTyp, vysledneTelo, vyslednaHodnota, vyslednyPin);
        }

        public override string ToString()
        {
            if (Typ == TypPrikazu.Trigger) return $"{Telo}{Separator}{Pin}{Separator}{Hodnota}";
            if (Typ == TypPrikazu.Macro) return $"{Telo} {Hodnota}";
            if (Typ == TypPrikazu.Task) return $"{Telo}";
            if (Typ == TypPrikazu.DI) return $"{Typ}: {Telo} | {Hodnota}";
            if (Typ == TypPrikazu.Goal) return $"{Typ}: {Telo}, {Hodnota}";

            return string.Join(Separator.ToString(), new string[] { VolaciZnak, Typ.ToString(), Telo, Hodnota });
        }

        public override bool Equals(object obj)
        {
            return obj is RobotPrikaz && Equals((RobotPrikaz)obj);
        }

        public bool Equals(RobotPrikaz other)
        {
            return Typ == other.Typ &&
                   Telo == other.Telo &&
                   Hodnota == other.Hodnota;
        }

        public override int GetHashCode()
        {
            var hashCode = -218798723;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + Typ.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Telo);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Hodnota);
            return hashCode;
        }
    }

    public enum TypPrikazu
    {
        Trigger,
        Macro,
        Task,
        DI,
        Goal
    }
}
