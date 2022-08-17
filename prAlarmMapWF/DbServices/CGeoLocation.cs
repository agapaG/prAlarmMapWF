using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;

using NLog;

namespace prAlarmMapWF.DbServices
{
    public class CGeoLocation
    {
        static Logger dbLog = NLog.LogManager.GetLogger("dbLog");

        public static void _create_geoloctbl(string tblname)
        {
            string ConnStr = ConfigurationManager.ConnectionStrings["cnnStr"].ConnectionString;
            SqlCommand cmd = null;
            using (SqlConnection sqlConnection = new SqlConnection(ConnStr))
            {
                string sqlcmd = string.Format(@"If not exists 
                (select name from sysobjects where name = '{0}') create table {0} (
                Prot smallint NULL,
                Ist tinyint NULL,
                Line tinyint NULL,
                Acount char(4) NULL,
                Code char(3) NULL,
                Q char(1) NULL,
                gru smallint NULL,
                NumZ smallint NULL,
                DT datetime NULL,
                Rec int NULL,
                gr char(2) NULL,
                so char(3) NULL)", tblname);


                try
                {
                    sqlConnection.Open();

                    cmd = sqlConnection.CreateCommand();
                    cmd.CommandText = sqlcmd;

                    cmd.ExecuteNonQuery();
                    dbLog.Info("Таблица создана");
                }
                catch (SqlException ex)
                {
                    dbLog.Error(ex.Message);
                    if (sqlConnection.State == System.Data.ConnectionState.Open)
                        sqlConnection.Close();
                }
            }

        }

    }
}
