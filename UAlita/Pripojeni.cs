using Alita;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAlita
{
    internal class Pripojeni : Alita.Pripojeni
    {

        public Pripojeni()
        { }

        internal string sestavPripojeni(string DataSource, string InitialCatalog, string UserId, string Password)
        {
            EntityConnectionStringBuilder entityConnectionStringBuilder = new EntityConnectionStringBuilder();
            string nazevAplikace = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

            entityConnectionStringBuilder.Metadata = $@"res://*/AlitaDb.csdl|res://*/AlitaDb.ssdl|res://*/AlitaDb.msl";
            entityConnectionStringBuilder.Provider = "System.Data.SqlClient";
            entityConnectionStringBuilder.ProviderConnectionString = SestavConnectionString(DataSource, InitialCatalog, UserId, Password);

            return entityConnectionStringBuilder.ConnectionString;
        }

        protected static string SestavConnectionString(string DataSource = "127.0.0.1", string InitialCatalog = "", string UserId = "", string Password = "")
        {
            SqlConnectionStringBuilder sqlConnectionString = new SqlConnectionStringBuilder();
            sqlConnectionString.DataSource = DataSource;
            sqlConnectionString.InitialCatalog = InitialCatalog;
            sqlConnectionString.UserID = UserId;
            sqlConnectionString.Password = Password;
            sqlConnectionString.MultipleActiveResultSets = true;
            sqlConnectionString.ApplicationName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

            return sqlConnectionString.ConnectionString;
        }
    }
}
