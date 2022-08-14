using System;
using System.Collections.Generic;

using System.Configuration;
using System.Data.SqlClient;

using NLog;

using prAlarmMapWF.Data;

namespace prAlarmMapWF.DbServices
{
    internal class ReadBuff_WTbl
    {
        static Logger dbLog = NLog.LogManager.GetLogger("dbLog");

        static string cnn = ConfigurationManager.ConnectionStrings["cnnStr"].ConnectionString;

        public static ICollection<DataPackage> _getbuff_work(int Rec)
        {
            List<DataPackage> rez = new List<DataPackage>();

            using (SqlConnection mySql = new SqlConnection(cnn))
            {
                SqlCommand cmd = null;

                try
                {
                    mySql.Open();
                    cmd = mySql.CreateCommand();
                    cmd.   Prepare();
                    cmd.CommandText = $"select * from buff_work where Rec > {Rec}";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        DataPackage tbl = new DataPackage();
                        tbl.Tcentral = reader.IsDBNull(3) ? null : reader.GetString(3);
                        
                        tbl.Rec = reader.GetInt32(9);
                                                
                        tbl.N03s = Program.n03s.FindAll(item => Equals(item.Nb, tbl.Tcentral));

                        rez.Add(tbl);
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
