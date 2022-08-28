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
    internal class CPoint
    {
        public double X { get; set; }   
        public double Y { get; set; }   
        public CPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
    public partial class Map : Form
    {
        CPoint cPoint = new CPoint(49.989897385959935, 36.22941235773933);

        GMapOverlay AlarmmarkersOverlay = new GMapOverlay("Alarms");

        int glMarCount = 0;
        int glMarCountCurr = 0;
        List<DataPackage> dataPackagesCurrent = null;
        List<CGeoLocData> workGeoLocs = new List<CGeoLocData>();


        private void MapBgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show(e.Error.Message);

            MessageBox.Show("Завершение приложения...");

        }
        private void MapBgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Очищаем список маркеров.
            AlarmmarkersOverlay.Markers.Clear();

            for (int i = 0; i < workGeoLocs.Count; i++)
            {
                GMapMarker marker = new GMarkerGoogle(
                    new PointLatLng(workGeoLocs[i].Latitude, workGeoLocs[i].Longitude), GMarkerGoogleType.red_small);
                marker.ToolTip = new GMapRoundedToolTip(marker);
                Brush ToolTipBackColor = new SolidBrush(Color.Transparent);
                marker.ToolTip.Fill = ToolTipBackColor;
                //marker.ToolTip.Fill = Brushes.Black;
                marker.ToolTip.Foreground = Brushes.Red;
                marker.ToolTip.Stroke = Pens.Black;
                marker.ToolTip.TextPadding = new Size(10, 10);
                marker.ToolTipMode = MarkerTooltipMode.Always;
                marker.ToolTipText = workGeoLocs[i].AddrC;
                //marker.

                AlarmmarkersOverlay.Markers.Add(marker);
            }


            eventWait.Set();

        }

        private void Map_Work(object sender, DoWorkEventArgs e)
        {
            while (!Program.EndWork)
            {
                Thread.Sleep(10);
                
                eventWait.WaitOne();

                try
                {
                    int count = ReadBuff_WTbl._get_rowscount();
                    if (count < 0)
                        continue;
                    glMarCountCurr = count;

                    dataPackagesCurrent = (List<DataPackage>)ReadBuff_WTbl._getbuff_work(Program.nRec);
                    //if (dataPackages.Count == 0)
                    //    continue;

                    //dataPackages = dataPackagesCurrent;
                    glMarCount = dataPackagesCurrent.Count;

                    if (glMarCount != glMarCountCurr)
                    {
                    }

                    for (int i = 0; i < dataPackagesCurrent.Count; i++)
                    {
                        CGeoLocData cGeoLocData = new CGeoLocData();
                        cGeoLocData = cGeoLocDatas.Find(item => item.AddrC.Equals(dataPackagesCurrent[i].N03s[0].Adr));
                        workGeoLocs.Add(cGeoLocData);
                    }
                    
                    if (dataPackagesCurrent.Count != 0)
                        Program.nRec = dataPackagesCurrent[dataPackagesCurrent.Count - 1].Rec;

                    mapBgWorker.ReportProgress(100);


                }
                catch (ArgumentNullException ex)
                {
                    Program.outLog.Error(ex.Message);
                }
                catch (Exception ex)
                {
                    Program.outLog.Error(ex.Message);
                }

                eventWait.Reset();

            }
        }
    }
}
