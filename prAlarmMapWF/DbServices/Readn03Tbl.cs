using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using prAlarmMapWF.Data;
using NLog;

namespace prAlarmMapWF.DbServices
{
    internal class Readn03Tbl
    {
        static Logger dbLog = NLog.LogManager.GetLogger("dbLog");

        public static List<n03> _getn03()
        {
            List<n03> rez = new List<n03>();

            string cnn = ConfigurationManager.ConnectionStrings["cnnStrno"].ConnectionString;

            using (SqlConnection mySql = new SqlConnection(cnn))
            {
                SqlCommand cmd = null;

                try
                {
                    mySql.Open();
                    cmd = mySql.CreateCommand();
                    cmd.Prepare();
                    cmd.CommandText = $"select * from n03";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        n03 tbln03 = new n03();
                        tbln03.Id = reader.GetInt32(0);
                        tbln03.Adr = reader.IsDBNull(2) ? null : reader.GetString(2);
                        tbln03.Nb = reader.IsDBNull(13) ? null : reader.GetString(13);
                        
                        rez.Add(tbln03);
                    }
                }
                catch (SqlException ex)
                {
                    dbLog.Error(ex.Message + $" {ex.ErrorCode}");
                    return null;
                }
                catch (Exception ex)
                {
                    dbLog.Error(ex.Message + $" {ex.HResult}");
                    return null;
                }

            }


            return rez;
        }
    }
}
