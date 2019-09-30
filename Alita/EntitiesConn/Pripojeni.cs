using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Alita
{
    public class Pripojeni
    {
        static AlitaDatabase db = null;

        public static AlitaDatabase VytvorInstanci(string DataSource, string InitialCatalog, string UserId, string Password)
        {
            if (db == null) db = sestavPripojeni(DataSource, InitialCatalog, UserId, Password);
            return db;
        }

        public static AlitaDatabase VytvorInstanci(bool vzdyNovaInstance=true, string DataSource = "", string InitialCatalog = "", string UserId = "", string Password="")
        {
            AlitaDatabase database = sestavPripojeni(DataSource, InitialCatalog, UserId, Password);
            return database;
        }

        protected static AlitaDatabase sestavPripojeni(string DataSource, string InitialCatalog, string UserId, string Password)
        {
            EntityConnectionStringBuilder entityConnectionStringBuilder = new EntityConnectionStringBuilder();
            string nazevAplikace = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

            entityConnectionStringBuilder.Metadata = $@"res://*/AlitaDatabase.csdl|res://*/AlitaDatabase.ssdl|res://*/AlitaDatabase.msl";
            entityConnectionStringBuilder.Provider = "System.Data.SqlClient";
            entityConnectionStringBuilder.ProviderConnectionString = SestavConnectionString(DataSource, InitialCatalog, UserId, Password);

            return new AlitaDatabase(entityConnectionStringBuilder.ConnectionString);
        }

        protected static string SestavConnectionString(string DataSource = "127.0.0.1", string InitialCatalog = "", string UserId ="", string Password="")
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
