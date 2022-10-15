using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Data.SqlClient;

using prAlarmMapWF.Data;
using NLog;

namespace prAlarmMapWF.DbServices
{
    internal class Readn04Tbl
    {
        static Logger dbLog = NLog.LogManager.GetLogger("dbLog");

        public static List<n04> _getn04()
        {
            List<n04> rez = new List<n04>();

            string cnn = ConfigurationManager.ConnectionStrings["cnnStrno"].ConnectionString;

            using (SqlConnection mySql = new SqlConnection(cnn))
            {
                SqlCommand cmd = null;

                try
                {
                    mySql.Open();
                    cmd = mySql.CreateCommand();
                    cmd.Prepare();
                    cmd.CommandText = $"select * from n04";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        n04 tbln04 = new n04();
                        tbln04.Id = reader.GetInt32(0);
                        tbln04.Status = reader.IsDBNull(7) ? null : reader.GetString(7);

                        rez.Add(tbln04);
                    }
                }
                catch (SqlException ex)
                {
                    dbLog.Error(ex.Message);
                    return null;
                }
                catch (Exception ex)
                {
                    dbLog.Error(ex.Message);
                    return null;
                }

            }


            return rez;
        }
    }
}
