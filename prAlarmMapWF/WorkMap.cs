using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;

using prAlarmMapWF.DbServices;
using prAlarmMapWF.Data;


namespace prAlarmMapWF
{
    public partial class Map : Form
    {
        int glMarCount = 0;
        int glMarCountCurr = 0;
        List<DataPackage> dataPackages = null;



        private void MapBgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show(e.Error.Message);

            MessageBox.Show("Завершение приложения...");

        }
        private void MapBgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Map_Work(object sender, DoWorkEventArgs e)
        {
            while (!Program.EndWork)
            {
                Thread.Sleep(10);
                try
                {
                    int count = ReadBuff_WTbl._get_rowscount();
                    if (count < 0)
                        continue;
                    glMarCountCurr = count;

                    dataPackages = (List<DataPackage>)ReadBuff_WTbl._getbuff_work(Program.nRec);
                    if (dataPackages.Count == 0)
                        continue;

                    glMarCount = dataPackages.Count;

                    if (glMarCount != glMarCountCurr)
                    {
                    }

                    for (int i = 0; i < dataPackages.Count; i++)
                    {
                        //Україна, Харків, Академіка Проскури вулиця, 10А
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Україна, ");

                        for (int j = 0; j < dataPackages[i].N03s.Count; ++j)
                        {
                            //string[] pars = dataPackages[i].N03s[i].Adr.Split(new char[''])
                        }
                    }
                    
                    if (dataPackages.Count != 0)
                        Program.nRec = dataPackages[dataPackages.Count - 1].Rec;
                }
                catch (ArgumentNullException ex)
                {
                    Program.outLog.Error(ex.Message);
                }
                catch (Exception ex)
                {
                    Program.outLog.Error(ex.Message);
                }

            }
        }
    }
}
