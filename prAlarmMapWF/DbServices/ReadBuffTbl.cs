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
        static string TblName = ConfigurationManager.AppSettings["TblName"];

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
                    cmd.Prepare();
                    cmd.CommandText = $"select * from {TblName} where Rec > {Rec}";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        DataPackage tbl = new DataPackage();
                        tbl.Tcentral = reader.IsDBNull(3) ? null : reader.GetString(3);
                        tbl.Time = reader.IsDBNull(8) ? null : reader.GetDateTime(8).ToString();
                        tbl.Rec = reader.GetInt32(9);
                        tbl.Color = (byte)(reader.IsDBNull(11) ? 0x00 : reader.GetByte(11));


                        tbl.N03s = Program.n03s.FindAll(item => Equals(item.Nb, tbl.Tcentral));
                        
                        if (tbl.N03s.Count != 0)   
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

        public static int _get_rowscount()
        {
            int rowscount = 0;
            string cmdtxt = "select count(*) from buff_work";
            
            try
            {
                using (SqlConnection mySql = new SqlConnection(cnn))
                {
                    using (SqlCommand cmd = new SqlCommand(cmdtxt, mySql))
                    {
                        mySql.Open();
                        rowscount = (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (SqlException ex)
            {
                dbLog.Error(ex.Message);
                return -1;
            }
            catch (Exception ex)
            {
                dbLog.Error(ex.Message);
                return -1;
            }

            return rowscount;
        }

    }

}
