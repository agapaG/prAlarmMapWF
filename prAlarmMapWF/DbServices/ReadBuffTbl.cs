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
        static string cnn0 = ConfigurationManager.ConnectionStrings["cnnStrno"].ConnectionString;
        static string TblName = ConfigurationManager.AppSettings["TblName"];

        private static List<DataPackage> _getBuff()
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
                    cmd.CommandText = $"select * from {TblName}";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        DataPackage tbl = new DataPackage();
                        tbl.Tcentral = reader.IsDBNull(3) ? null : reader.GetString(3);
                        tbl.Time = reader.IsDBNull(8) ? null : reader.GetDateTime(8).ToString();
                        tbl.Rec = reader.GetInt32(9);
                        tbl.Color = (byte)(reader.IsDBNull(11) ? 0x00 : reader.GetByte(11));
                                              
                        rez.Add(tbl);
                    }
                }
                catch (SqlException ex)
                {
                    dbLog.Error(ex.Message);
                    mySql.Close();
                    return null;
                }
                catch (Exception ex)
                {
                    dbLog.Error(ex.Message);
                    mySql.Close();
                    return null;
                }

            }

            return rez;
        }
        private static List<n03> _getn03(string nb)
        {
            List<n03> rez = new List<n03>();
            
            using (SqlConnection mySql = new SqlConnection(cnn0))
            {
                SqlCommand cmd = null;

                try
                {
                    mySql.Open();
                    cmd = mySql.CreateCommand();
                    cmd.Prepare();
                    cmd.CommandText = $"select * from n03 where nb = '{nb}'";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        n03 tbln03 = new n03();
                        tbln03.Id = reader.GetInt32(0);
                        tbln03.Adr = reader.IsDBNull(2) ? null : reader.GetString(2);
                        tbln03.Nb = reader.IsDBNull(13) ? null : reader.GetString(13);
                        tbln03.Status = reader.IsDBNull(14) ? null : reader.GetString(14);  

                        rez.Add(tbln03);
                    }
                }
                catch (SqlException ex)
                {
                    dbLog.Error(ex.Message + $" {ex.ErrorCode}");
                    mySql.Close();
                    return null;
                }
                catch (Exception ex)
                {
                    dbLog.Error(ex.Message + $" {ex.HResult}");
                    mySql.Close();
                    return null;
                }
            }      
            return rez;
        }      

        public static List<DataPackage> GetDataPackages()
        {
            List<DataPackage> fromBuff = _getBuff();
            if (fromBuff == null)
                return null;
                        
            
            for (int i = 0; i < fromBuff.Count; ++i)
            {
                List<n03> n03 = _getn03(fromBuff[i].Tcentral);
                if (n03 == null)
                    continue;
                                
                fromBuff[i].N03s = n03; 
            }
            return fromBuff;
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
