using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alita
{
    public partial class AlitaDatabase : System.Data.Entity.DbContext
    {
        private static object locker = new object();
        public bool UlozZmenySvalidaci()
        {
            bool status = false;
            lock (locker)
            {
                try
                {
                    SaveChanges();
                    return status;
                }
                catch (DbEntityValidationException exception)
                {
                    var chyboveZpravy = exception.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);
                    var vyslednaChybovaZprava = string.Join("; ", chyboveZpravy);
                    var zpravaVyjimky = string.Concat(exception.Message, "Chyby validace jsou nasledujici: ", vyslednaChybovaZprava);
                    throw new DbEntityValidationException(zpravaVyjimky, exception.EntityValidationErrors);
                }
                catch (Exception obecnaException)
                {
                    while (obecnaException.InnerException != null)
                    {
                        obecnaException = obecnaException.InnerException;
                        SqlException sqlException = obecnaException as SqlException;
                        if (sqlException != null)
                        {
                            int[] chybyKvraceni =
                            {
                                2627 
                            };
                            if(sqlException.Errors.Cast<SqlError>().Any(x=> chybyKvraceni.Contains(x.Number)))
                            {
                                //ignorovani filtrovanych chybovych kodu
                                return true;
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                    throw;
                }
            }
        }

        public AlitaDatabase(string ConnectionString) : base(ConnectionString)
        {

        }
    }
}
