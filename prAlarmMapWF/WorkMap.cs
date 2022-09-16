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
using GMap.NET.ObjectModel;

using prAlarmMapWF.DbServices;
using prAlarmMapWF.Data;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace prAlarmMapWF
{
    internal class CPoint
    {
        public double X { get; set; }   
        public double Y { get; set; }   
        public CPoint(double y, double x)
        {
            X = x;
            Y = y;
        }
    }
    public partial class Map : Form
    {
        CPoint cPoint = new CPoint(49.989897385959935, 36.22941235773933);

        GMapOverlay AlarmmarkersOverlay = new GMapOverlay("Alarms");
        GMapOverlay AlarmmarkersOverlayp13 = new GMapOverlay("Alarmsp13");

        GMapOverlay AlarmmpolyOverlay = new GMapOverlay("polygons");
        List<PointLatLng> pointLatLngs = new List<PointLatLng>();
        ObservableCollectionThreadSafe<GMapPolygon> Polygones = null;


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

            //CPoint pW = new CPoint(50.11279341666608, 36.11084117131164);
            //double Lat = pW.Y - cPoint.Y;
            //double Lon = pW.X - cPoint.X;
            //double m = Math.Sqrt(Lat * Lat + Lon * Lon);
            //MessageBox.Show($"Top {AlarmMap.ViewArea.Top}");
            //MessageBox.Show($"Left {AlarmMap.ViewArea.Left}");
            GMapMarker markT = new GMarkerGoogle(
                       new PointLatLng(cPoint.Y + (AlarmMap.ViewArea.HeightLat/2 - 0.03), cPoint.X + (AlarmMap.ViewArea.WidthLng/2 - 0.06)), GMarkerGoogleType.red_pushpin);
            markT.ToolTip = new GMapBaloonToolTip(markT);
            markT.ToolTip.Fill = Brushes.LightGray;
            markT.ToolTip.Foreground = Brushes.Black;
            markT.ToolTip.Stroke = Pens.Black;
            markT.ToolTip.TextPadding = new Size(5, 5);
            markT.ToolTipMode = MarkerTooltipMode.Always;

            AlarmmarkersOverlay.Markers.Add(markT);

            for (int i = 0; i < workGeoLocs.Count; i++)
            {
                double x1 = workGeoLocs[i].Latitude - cPoint.Y;
                double y1 = workGeoLocs[i].Longitude - cPoint.X;
                double mod = Math.Sqrt(x1 * x1 + y1 * y1);

                PointLatLng currenyPoint = new PointLatLng(workGeoLocs[i].Latitude, workGeoLocs[i].Longitude);
                
                                
                for (int j = 0; j < Polygones.Count; j++)
                {
                     if (Polygones[j].IsInside(currenyPoint))
                    {
                        GMapMarker marker = new GMarkerGoogle(
                        new PointLatLng(workGeoLocs[i].Latitude, workGeoLocs[i].Longitude), GMarkerGoogleType.red_small);
                        marker.ToolTip = new GMapRoundedToolTip(marker);
                        //Brush ToolTipBackColor = new SolidBrush(Color.Transparent);
                        //marker.ToolTip.Fill = ToolTipBackColor;
                        marker.ToolTip.Fill = Brushes.LightGray;
                        marker.ToolTip.Foreground = Brushes.Black;
                        marker.ToolTip.Stroke = Pens.Black;
                        marker.ToolTip.TextPadding = new Size(5, 5);
                        marker.ToolTipMode = MarkerTooltipMode.Always;
                        marker.ToolTipText = workGeoLocs[i].AddrC;
                        //marker.Size = new Size(2, 2);
                        

                        AlarmmarkersOverlay.Markers.Add(marker);
                    }    
                }
                /*
                if (mod < (0.009 * 13.1))
                {
                    GMapMarker marker = new GMarkerGoogle(
                        new PointLatLng(workGeoLocs[i].Latitude, workGeoLocs[i].Longitude), GMarkerGoogleType.red_small);
                    marker.ToolTip = new GMapRoundedToolTip(marker);
                    //Brush ToolTipBackColor = new SolidBrush(Color.Transparent);
                    //marker.ToolTip.Fill = ToolTipBackColor;
                    marker.ToolTip.Fill = Brushes.LightGray;
                    marker.ToolTip.Foreground = Brushes.Black;
                    marker.ToolTip.Stroke = Pens.Black;
                    marker.ToolTip.TextPadding = new Size(5, 5);
                    marker.ToolTipMode = MarkerTooltipMode.Always;
                    marker.ToolTipText = workGeoLocs[i].AddrC;
                    //marker.Size = new Size(2, 2);
                    //marker.

                    AlarmmarkersOverlay.Markers.Add(marker);
                }
                else
                {

                    Bitmap bmp8 = _createLeftMarker(workGeoLocs[i].AddrC);
                    GMapMarker marker1 = new GMarkerGoogle(
                        new PointLatLng(cPoint.X + x1*2.7f/3 , cPoint.Y + y1*2.7f/3), bmp8);
                    //GMapMarker marker1 = new GMarkerGoogle(
                    //    new PointLatLng(cPoint.X + x1 , cPoint.Y + y1), bmp8);
                    //marker1.Offset = new Point(-bmp8.Width / 2, -bmp8.Height / 2);
                    //marker1.Offset = new Point(bmp8.Width, 0);
                    AlarmmarkersOverlayp13.Markers.Add(marker1);
                }
                
                if (x1 > 0 && y1 > 0)
                {
                    GMapMarker mark = new GMarkerGoogle(
                       new PointLatLng(cPoint.X + 1.0f/3, cPoint.Y + 1.0f/3), GMarkerGoogleType.red_pushpin);
                    mark.ToolTip = new GMapBaloonToolTip(mark);
                    mark.ToolTip.Fill = Brushes.LightGray;
                    mark.ToolTip.Foreground = Brushes.Black;
                    mark.ToolTip.Stroke = Pens.Black;
                    mark.ToolTip.TextPadding = new Size(5, 5);
                    mark.ToolTipMode = MarkerTooltipMode.Always;
                    mark.ToolTipText = "I четверть";

                    AlarmmarkersOverlay.Markers.Add(mark);
                }

                if (x1 < 0 && y1 > 0)
                {
                    GMapMarker mark = new GMarkerGoogle(
                       new PointLatLng(cPoint.X + 1.7f/3, cPoint.Y - 1.7f/3), GMarkerGoogleType.red_pushpin);
                    mark.ToolTip = new GMapBaloonToolTip(mark);
                    mark.ToolTip.Fill = Brushes.LightGray;
                    mark.ToolTip.Foreground = Brushes.Black;
                    mark.ToolTip.Stroke = Pens.Black;
                    mark.ToolTip.TextPadding = new Size(5, 5);
                    mark.ToolTipMode = MarkerTooltipMode.Always;
                    mark.ToolTipText = "II четверть";

                    AlarmmarkersOverlay.Markers.Add(mark);
                }

                if (x1 < 0 && y1 < 0)
                {
                    GMapMarker mark = new GMarkerGoogle(
                       new PointLatLng(cPoint.X - 1.7f/3, cPoint.Y - 1.7f/3), GMarkerGoogleType.red_pushpin);
                    mark.ToolTip = new GMapBaloonToolTip(mark);
                    mark.ToolTip.Fill = Brushes.LightGray;
                    mark.ToolTip.Foreground = Brushes.Black;
                    mark.ToolTip.Stroke = Pens.Black;
                    mark.ToolTip.TextPadding = new Size(5, 5);
                    mark.ToolTipMode = MarkerTooltipMode.Always;
                    mark.ToolTipText = "III четверть";

                    AlarmmarkersOverlay.Markers.Add(mark);
                }
                
                if (x1 > 0 && y1 < 0)
                {
                    GMapMarker mark = new GMarkerGoogle(
                       new PointLatLng(cPoint.X + mod/3.2, cPoint.Y - mod/3), GMarkerGoogleType.red_pushpin);
                    mark.ToolTip = new GMapBaloonToolTip(mark);
                    mark.ToolTip.Fill = Brushes.LightGray;
                    mark.ToolTip.Foreground = Brushes.Black;
                    mark.ToolTip.Stroke = Pens.Black;
                    mark.ToolTip.TextPadding = new Size(5, 5);
                    mark.ToolTipMode = MarkerTooltipMode.Always;
                    //mark.ToolTipText = "IV четверть";
                    //mark.ToolTipText = "";

                    AlarmmarkersOverlay.Markers.Add(mark);
                }
                */
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
                        if (cGeoLocData != null)    
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

        private Bitmap _createLeftMarker(string streetname)
        {
            Bitmap bmp8 = null;
            //***********************************
            var bmp32 = new Bitmap(350, 50, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(bmp32))
            {
                RectangleF rectangleF = new RectangleF(33.0F, 20.0f, bmp32.Width, bmp32.Height);
                g.Clear(Color.LightGray);
                PointF pt1 = new PointF(0.0f,0.0f);
                PointF pt2 = new PointF(0.0f, 20.0f);
                PointF pt3 = new PointF(0.0f, 0.0f);
                PointF pt4 = new PointF(20.0f, 0.0f);
                PointF pt5 = new PointF(0.0f, 0.0f);
                PointF pt6 = new PointF(30.0f, 30.0f);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                g.DrawLine(Pens.Black, pt1, pt2);
                g.DrawLine(Pens.Black, pt3, pt4);
                g.DrawLine(Pens.Black, pt5, pt6);

                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                StringFormat sf = new StringFormat()
                {
                    //Alignment = StringAlignment.Near,
                    //LineAlignment = StringAlignment.Near,
                };
                g.DrawString(streetname, new Font("arial", 10), Brushes.Black, rectangleF, sf);
                g.Flush();
            }
            // создаем битмап с палитрой
            bmp8 = new Bitmap(350, 50, PixelFormat.Format8bppIndexed);
            var palette = bmp8.Palette;
            for (int j = 0; j < 256; j++)
            {
                // заполняем палитру, для простоты это будут все оттенки серого
                palette.Entries[j] = Color.FromArgb(j, j, j);
            }
            // это не просто так, обязательно нужна эта строка
            bmp8.Palette = palette;

            var data = bmp8.LockBits(new Rectangle(0, 0,350, 50), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            var bytes = new byte[data.Height * data.Stride];
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            for (int x = 0; x < 350; x++)
                for (int y = 0; y < 50; y++)
                {
                    // берем пиксель с основного 32битного битмапа
                    var color = bmp32.GetPixel(x, y);
                    // ищем цвет в палитре 8битного битмапа
                    int index = -1;
                    for (int ii = 0; ii < palette.Entries.Length; ii++)
                        if (palette.Entries[ii] == color)
                        {
                            index = ii;
                            break;
                        }
                    if (index == -1)
                        throw new InvalidOperationException("Cannot find color in palette");
                    // записываем индекс цвета в палитре напрямую в изображение
                    bytes[y * data.Stride + x] = (byte)index;
                }

            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            bmp8.UnlockBits(data);

            //bmp8.Save("1.gif");

            return bmp8;

        }
    }
}
