using ConfigLibrary;
using ConfigLibrary.control;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alita.Models
{
    public class ConfigData
    {
        private LibraryControler Controller = new LibraryControler();
        private ConfigProvider provider { get; }
        private readonly string CestaKSouboru = string.Join("/", (Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase.ToString()) + "\\")
                .Replace("file:\\", ""), "config.ini");

        public ConfigData()
        {
            try
            {
                provider = Controller.NahrajConfig(CestaKSouboru);
                Version = Version.Parse((string)GetAttribute(main, nameof(Version)).Value);
                Assembly = Version.Parse((string)GetAttribute(main, nameof(Assembly)).Value);
                Controller.UlozConfig(provider);
            }
            catch
            {
                throw;
            }
        }

        IConfigSection main => GetSection(nameof(main));
        IConfigSection database => GetSection(nameof(database));
        private IConfigSection GetSection(string Jmeno) => provider?.Config.Single(x => x.Name.ToLower() == Jmeno.ToLower());


        private IConfigAttribute GetAttribute(IConfigSection Sekce, string Jmeno) => Sekce.Attributes.Single(x => x.Keyword.ToLower() == Jmeno.ToLower());

        private Version version;
        public Version Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
                PorovnaniVerzi(ref version);
                GetAttribute(main, nameof(version)).Value = version;
            }
        }

        private Version assembly;
        public Version Assembly
        {
            get
            {
                return assembly;
            }
            set
            {
                assembly = value;
                PorovnaniVerzi(ref assembly);
                GetAttribute(main, nameof(assembly)).Value = assembly;
            }
        }

        private void PorovnaniVerzi(ref Version Verze)
        {
            if(Verze == null)
            {
                Verze = new Version();
            }

            var versionInfo = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Version aktualniVerze = new Version(versionInfo.ProductVersion);
            int result = aktualniVerze.CompareTo(Verze);
            if (result != 0) Verze = aktualniVerze;
        }

        public int Location
        {
            get
            {
                int.TryParse((string)GetAttribute(main, nameof(Location)).Value, out int result);
                return result;
            }
            set
            {
                GetAttribute(main, nameof(Location)).Value = value;
            }
        }

        public string DataSource
        {
            get
            {
                return (string)GetAttribute(database, nameof(DataSource)).Value;
            }
        }

        public string InitialCatalog
        {
            get
            {
                return (string)GetAttribute(database, nameof(InitialCatalog)).Value;
            }
        }

        public string UserId
        {
            get
            {
                return (string)GetAttribute(database, nameof(UserId)).Value;
            }
        }

        public bool SwitchLocation
        {
            get
            {
                return (bool)GetAttribute(main, nameof(SwitchLocation)).Value;
            }
        }
    }
}
