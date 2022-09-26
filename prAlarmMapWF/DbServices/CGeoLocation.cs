using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Windows.Forms;

using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;

using prAlarmMapWF.Data;

using NLog;

namespace prAlarmMapWF.DbServices
{
    public class CGeoLocation
    {
        static Logger dbLog = NLog.LogManager.GetLogger("dbLog");

        public static List<CGeoLocData> _getGeoloc()
        {
            List<CGeoLocData> geoloc = new List<CGeoLocData>(); 

            string ConnStr = ConfigurationManager.ConnectionStrings["cnnStr"].ConnectionString;
            SqlCommand cmd = null;

            using (SqlConnection sql = new SqlConnection(ConnStr))
            {
                try
                {
                    sql.Open();
                    cmd = sql.CreateCommand();
                    cmd.Prepare();
                    cmd.CommandText = "select * from GeoLoc";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        CGeoLocData cGeo = new CGeoLocData();
                        cGeo.Latitude = reader.GetDouble(0);
                        cGeo.Longitude = reader.GetDouble(1);
                        cGeo.AddrC = reader.GetString(2);
                        cGeo.AddrM = reader.GetString(3);

                        geoloc.Add(cGeo);   
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                    return null;
                }
            }
            return geoloc;
        }

    }
}
