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
        public CPoint() { }
        public CPoint(double y, double x)
        {
            X = x;
            Y = y;
        }
    }

    public static class ExtenClass
    {
        public static List<TSource> DistinctBy<TSource, TKey>(
                        this List<TSource> source,
                        Func<TSource, TKey> keySelector)
        {
            var knownKeys = new HashSet<TKey>();
            return source.Where(element => knownKeys.Add(keySelector(element))).ToList();
        }
    }

    public partial class Map : Form
    {
        EventWaitHandle eventWait;
        EventWaitHandle eventWaitProc;

        CPoint cPoint = new CPoint(49.989897385959935, 36.22941235773933);
        CPoint LeftCorner = new CPoint();

        double Height1per100;
        double WWidth1per100;

        double AlarmMapViewAreaSizeHeightLat;
        double AlarmMapViewAreaSizeWidthLng;

        GMapOverlay AlarmmarkersOverlay = new GMapOverlay("Alarms");
        GMapOverlay AlarmmarkersOverlayp13 = new GMapOverlay("Alarmsp13");

        GMapOverlay AlarmmpolyOverlay = new GMapOverlay("polygons");
        List<PointLatLng> pointLatLngs = new List<PointLatLng>();
        ObservableCollectionThreadSafe<GMapPolygon> Polygones = null;

              
        List<DataPackage> dataPackagesCurrent = null;
        List<CGeoLocData> workGeoLocs = new List<CGeoLocData>();
        bool onetime = true;

        List<string> geoLocNames = new List<string>();

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
            AlarmmarkersOverlayp13.Markers.Clear();

            //workGeoLocs = workGeoLocs.Distinct().ToList();
            List<CGeoLocData> Current = workGeoLocs.DistinctBy(x => x.AddrC);

            //cLog.Info($"MapBgWorker_ProgressChanged HeightLat: {AlarmMap.ViewArea.HeightLat}  WidthLng: {AlarmMap.ViewArea.WidthLng}");

            double deltHeight1per10 = AlarmMapViewAreaSizeHeightLat/10.0;
            double deltHeight1per5 = AlarmMapViewAreaSizeHeightLat/5.0;
            double deltHeight1per15 = AlarmMapViewAreaSizeHeightLat/15.0;
            double deltHeight1per20 = AlarmMapViewAreaSizeHeightLat/20.0;
            double deltHeight1per25 = AlarmMapViewAreaSizeHeightLat/25.0;
            double deltWidth1per10 = AlarmMapViewAreaSizeWidthLng/10.0;
            double deltWidth1per15 = AlarmMapViewAreaSizeWidthLng/15.0;
            double deltWidth1per20 = AlarmMapViewAreaSizeWidthLng/9.0;

            #region левая сторона
            double latLeft = cPoint.Y + AlarmMapViewAreaSizeHeightLat/2.0 - deltHeight1per15;
            double lngLeft = cPoint.X - AlarmMapViewAreaSizeWidthLng/2.0 + deltWidth1per20;
            #endregion
            #region правая сторона
            double latRight = cPoint.Y + AlarmMapViewAreaSizeHeightLat/2.0 - deltHeight1per15;
            double lngRight = cPoint.X + AlarmMapViewAreaSizeWidthLng/2.0 - deltWidth1per20;
            #endregion


            for (int i = 0; i < Current.Count; i++)
            {                
                double pLat = Current[i].Latitude - cPoint.Y;
                double pLong = Current[i].Longitude - cPoint.X;
                double Mod = Math.Sqrt(pLat*pLat + pLong*pLong);

                PointLatLng currenyPoint = new PointLatLng(Current[i].Latitude, Current[i].Longitude);
                
                                
                for (int j = 0; j < Polygones.Count; j++)
                {                    
                    if (Polygones[j].IsInside(currenyPoint))
                    {
                        GMapMarker marker = new GMarkerGoogle(
                        new PointLatLng(Current[i].Latitude, Current[i].Longitude), GMarkerGoogleType.red_small);
                        marker.ToolTip = new GMapRoundedToolTip(marker);
                        //Brush ToolTipBackColor = new SolidBrush(Color.Transparent);
                        //marker.ToolTip.Fill = ToolTipBackColor;
                        marker.ToolTip.Fill = Brushes.LightGray;
                        marker.ToolTip.Foreground = Brushes.Black;
                        marker.ToolTip.Stroke = Pens.Black;
                        marker.ToolTip.TextPadding = new Size(5, 5);
                        marker.ToolTipMode = MarkerTooltipMode.Always;
                        marker.ToolTipText = Current[i].AddrC;
                        //marker.Size = new Size(2, 2);


                        AlarmmarkersOverlay.Markers.Add(marker);
                    }
                    //else
                    //{

                    //    if (pLat > 0 && pLong > 0)
                    //    {
                    //        double cnAlpha = pLong / Mod;
                    //        double Alpha = Math.Acos(cnAlpha) * (180.0 / Math.PI);

                    //        if ((90.0 - Alpha) < 45.0)
                    //        {
                    //            Bitmap bmp8 = _createRightMarker(Current[i].AddrC);

                    //            GMapMarker marker1 = new GMarkerGoogle(
                    //                new PointLatLng(cPoint.Y, cPoint.X), bmp8);

                    //            latRight -= deltHeight1per15;
                    //            marker1.Position = new PointLatLng(latRight, lngRight);

                    //            AlarmmarkersOverlayp13.Markers.Add(marker1);
                    //        }
                    //        if ((90.0 - Alpha) > 55.0)
                    //        {
                    //            Bitmap bmp8 = _createRightUpMarker(Current[i].AddrC);

                    //            GMapMarker marker1 = new GMarkerGoogle(
                    //                new PointLatLng(cPoint.Y, cPoint.X), bmp8);

                    //            latRight -= deltHeight1per15;
                    //            marker1.Position = new PointLatLng(latRight, lngRight);

                    //            AlarmmarkersOverlayp13.Markers.Add(marker1);
                    //        }
                    //    }
                    //    if (pLat < 0 && pLong < 0)
                    //    {
                    //        double cnAlpha = pLong / Mod;
                    //        double Alpha = Math.Acos(cnAlpha) * (180.0 / Math.PI);

                    //        if ((180.0 - Alpha) < 35.0)
                    //        {
                    //            Bitmap bmp8 = _createLeftMarker(Current[i].AddrC);

                    //            GMapMarker marker1 = new GMarkerGoogle(
                    //                new PointLatLng(cPoint.Y, cPoint.X), bmp8);

                    //            marker1.Position = new PointLatLng(latLeft, lngLeft);

                    //            AlarmmarkersOverlayp13.Markers.Add(marker1);

                    //            //lngRight -= deltHeight1per25;

                    //            //Bitmap bmp81 = _createLestDownMarker(workGeoLocs[i].AddrC);
                    //            //GMapMarker marker11 = new GMarkerGoogle(
                    //            //    new PointLatLng(cPoint.Y, cPoint.X), bmp81);

                    //            //lngRight -= deltHeight1per15;
                    //            ////lngRight = cPoint.X - AlarmMap.ViewArea.Size.WidthLng / 2.0 + deltWidth1per20;

                    //            //marker11.Position = new PointLatLng(latLeft, lngRight);

                    //            //AlarmmarkersOverlayp13.Markers.Add(marker11);
                    //            //Bitmap bmp81 = _createLeftMarker(workGeoLocs[i].AddrC + "1");

                    //            //latLeft -= deltHeight1per15;
                    //            //lngRight = cPoint.X - AlarmMap.ViewArea.Size.WidthLng / 2.0
                    //            //    + deltWidth1per20;


                    //            //GMapMarker marker2 = new GMarkerGoogle(
                    //            //    new PointLatLng(cPoint.Y, cPoint.X), bmp81);

                    //            //marker2.Position = new PointLatLng(latLeft, lngRight);
                    //            //AlarmmarkersOverlayp13.Markers.Add(marker2);

                    //        }
                    //        if ((180.0 - Alpha) > 55.0)
                    //        {
                    //            Bitmap bmp8 = _createLeftDownMarker(Current[i].AddrC);
                    //            GMapMarker marker1 = new GMarkerGoogle(
                    //                new PointLatLng(cPoint.Y, cPoint.X), bmp8);

                    //            latLeft -= deltHeight1per15;
                    //            lngLeft = cPoint.X - AlarmMap.ViewArea.Size.WidthLng / 2.0 + deltWidth1per20;

                    //            marker1.Position = new PointLatLng(latLeft, lngLeft);

                    //            AlarmmarkersOverlayp13.Markers.Add(marker1);
                    //        }

                    //        if ((180.0 - Alpha) < 55.0 && (180.0 - Alpha) > 36.0)
                    //        {
                    //            Bitmap bmp8 = _createLeftDown45LMarker(Current[i].AddrC);
                    //            GMapMarker marker1 = new GMarkerGoogle(
                    //                new PointLatLng(cPoint.Y, cPoint.X), bmp8);

                    //            latLeft -= deltHeight1per15;
                    //            lngLeft = cPoint.X - AlarmMap.ViewArea.Size.WidthLng / 2.0 + deltWidth1per20;

                    //            marker1.Position = new PointLatLng(latLeft, lngLeft);

                    //            AlarmmarkersOverlayp13.Markers.Add(marker1);
                    //        }
                    //    }


                    //}
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
            //!!!!!!!!!!!!!!!!!!!!!!!!!!
            //workGeoLocs.RemoveAt(workGeoLocs.Count - 1);
            //Thread.Sleep(15);
            if (workGeoLocs.Count != 0)
                workGeoLocs.Clear();
            onetime = true;
            
            eventWait.Set();

        }

        private void Map_Work(object sender, DoWorkEventArgs e)
        {
            //int glMarCountCurr = 0;
            

            while (!Program.EndWork)
            {
                Thread.Sleep(900);

                eventWait.WaitOne();

                try
                {
                    //int count = ReadBuff_WTbl._get_rowscount();
                    //if (count < 0 || count < 1)
                    //    continue;
                    //if (glMarCountCurr != count)
                    //{
                    //    Program.nRec = 0;
                    //    workGeoLocs.Clear();
                    //    glMarCountCurr = count;
                    //}

                    dataPackagesCurrent = (List<DataPackage>)ReadBuff_WTbl._getbuff_work(Program.nRec);
                    //if (dataPackages.Count == 0)
                    //    continue;

                    //dataPackages = dataPackagesCurrent;
                    //glMarCount = dataPackagesCurrent.Count;

                    //if (glMarCount != glMarCountCurr)
                    //{
                    //}

                    for (int i = 0; i < dataPackagesCurrent.Count; i++)
                    {
                        string strtmp = _addrParse(dataPackagesCurrent[i].N03s[0].Adr);
                        //if (geoLocNames.Contains(dataPackagesCurrent[i].N03s[0].Adr))
                        //    continue;

                        CGeoLocData cGeoLocData = new CGeoLocData();
                        cGeoLocData = cGeoLocDatas.Find(item => item.AddrC.Equals(strtmp));
                        if (cGeoLocData != null)
                        {
                            var var1 = Program.n04s.Find(item => item.Id == dataPackagesCurrent[i].N03s[0].Id);
                            if (var1 != null)
                            {
                                if (var1.Status.Trim().Equals("Расторгнут"))
                                    continue;
                            }
                            else
                                continue;

                            CGeoLocData work = new CGeoLocData();
                            //tmp = cGeoLocDatas.Find
                            string tmp = dataPackagesCurrent[i].Tcentral + "  "; 
                            tmp += dataPackagesCurrent[i].Time + "  ";
                            tmp += cGeoLocData.AddrC;
                            work.AddrC = tmp;
                            work.Latitude = cGeoLocData.Latitude;
                            work.Longitude = cGeoLocData.Longitude;
                            work.AddrM = cGeoLocData.AddrM;
                            work.NCentral = dataPackagesCurrent[i].Tcentral;
                            work.Time = dataPackagesCurrent[i].Time;
                            workGeoLocs.Add(work);

                            //geoLocNames.Add(dataPackagesCurrent[i].N03s[0].Adr);
                        }
                        else
                        {
                            if (onetime)
                            {
                                CGeoLocData tmp = new CGeoLocData();
                                tmp.AddrC = "Улица Деревянко 3";
                                tmp.AddrM = "...";
                                tmp.Latitude = 50.03690493334075;
                                tmp.Longitude = 36.23892659172058;

                                workGeoLocs.Add(tmp);

                                onetime = false;
                            }
                        }
                    }
                    
                    //if (dataPackagesCurrent.Count != 0)
                    //    Program.nRec = dataPackagesCurrent[dataPackagesCurrent.Count - 1].Rec;

                    //**********************************************
                    //точка для анализа
                    //lat 49,899942109186
                    //long 35,809936523
                    //CGeoLocData cGD = new CGeoLocData();
                    //cGD.AddrC = "Test 1ч. < 35";
                    //cGD.AddrM = "...";
                    //cGD.Latitude = 50.0183289559089;
                    //cGD.Longitude = 36.6517639160156;
                    //workGeoLocs.Add(cGD);
                    //CGeoLocData cGD1 = new CGeoLocData();
                    //cGD1.AddrC = "Test 1ч. > 55";
                    //cGD1.AddrM = "...";
                    //cGD1.Latitude = 50.1377451310729;
                    //cGD1.Longitude = 36.6181182861328;
                    //workGeoLocs.Add(cGD1);
                    //CGeoLocData cGD2 = new CGeoLocData();
                    //cGD2.AddrC = "Test 1ч. ~ 45";
                    //cGD2.AddrM = "...";
                    //cGD2.Latitude = 50.3112694650712;
                    //cGD2.Longitude = 36.4540100097656;
                    //workGeoLocs.Add(cGD2);

                    //***********************************************
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
            var bmp32 = new Bitmap(300, 50, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(bmp32))
            {
                RectangleF rectangleF = new RectangleF(33.0F, 17.0f, bmp32.Width, bmp32.Height);
                g.Clear(Color.LightGray);
                PointF pt1 = new PointF(0.0f,25.0f);
                PointF pt2 = new PointF(10.0f, 10.0f);
                PointF pt3 = new PointF(0.0f, 25.0f);
                PointF pt4 = new PointF(10.0f, 35.0f);
                PointF pt5 = new PointF(0.0f, 25.0f);
                PointF pt6 = new PointF(30.0f, 25.0f);
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
            bmp8 = new Bitmap(300, 50, PixelFormat.Format8bppIndexed);
            var palette = bmp8.Palette;
            for (int j = 0; j < 256; j++)
            {
                // заполняем палитру, для простоты это будут все оттенки серого
                palette.Entries[j] = Color.FromArgb(j, j, j);
            }
            // это не просто так, обязательно нужна эта строка
            bmp8.Palette = palette;

            var data = bmp8.LockBits(new Rectangle(0, 0,300, 50), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            var bytes = new byte[data.Height * data.Stride];
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            for (int x = 0; x < 300; x++)
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

        private Bitmap _createLeftDownMarker(string streetname)
        {
            Bitmap bmp8 = null;
            //***********************************
            var bmp32 = new Bitmap(300, 50, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(bmp32))
            {
                RectangleF rectangleF = new RectangleF(33.0F, 17.0f, bmp32.Width, bmp32.Height);
                g.Clear(Color.LightGray);
                PointF pt1 = new PointF(20.0f, 50.0f);
                PointF pt2 = new PointF(20.0f, 20.0f);
                PointF pt3 = new PointF(20.0f, 50.0f);
                PointF pt4 = new PointF(10.0f, 40.0f);
                PointF pt5 = new PointF(20.0f, 50.0f);
                PointF pt6 = new PointF(30.0f, 40.0f);
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
                    //LineAlignment = StringAlignment.Center,
                };
                g.DrawString(streetname, new Font("arial", 10), Brushes.Black, rectangleF, sf);
                g.Flush();
            }
            // создаем битмап с палитрой
            bmp8 = new Bitmap(300, 50, PixelFormat.Format8bppIndexed);
            var palette = bmp8.Palette;
            for (int j = 0; j < 256; j++)
            {
                // заполняем палитру, для простоты это будут все оттенки серого
                palette.Entries[j] = Color.FromArgb(j, j, j);
            }
            // это не просто так, обязательно нужна эта строка
            bmp8.Palette = palette;

            var data = bmp8.LockBits(new Rectangle(0, 0, 300, 50), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            var bytes = new byte[data.Height * data.Stride];
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            for (int x = 0; x < 300; x++)
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
            //return bmp32;

        }

        private Bitmap _createLeftDown45LMarker(string streetname)
        {
            Bitmap bmp8 = null;
            //***********************************
            var bmp32 = new Bitmap(300, 50, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(bmp32))
            {
                RectangleF rectangleF = new RectangleF(33.0F, 17.0f, bmp32.Width, bmp32.Height);
                g.Clear(Color.LightGray);
                PointF pt1 = new PointF(0.0f, 50.0f);
                PointF pt2 = new PointF(0.0f, 40.0f);
                PointF pt3 = new PointF(0.0f, 50.0f);
                PointF pt4 = new PointF(0.0f, 10.0f);
                PointF pt5 = new PointF(0.0f, 50.0f);
                PointF pt6 = new PointF(30.0f, 20.0f);
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
                    //LineAlignment = StringAlignment.Center,
                };
                g.DrawString(streetname, new Font("arial", 10), Brushes.Black, rectangleF, sf);
                g.Flush();
            }
            // создаем битмап с палитрой
            bmp8 = new Bitmap(300, 50, PixelFormat.Format8bppIndexed);
            var palette = bmp8.Palette;
            for (int j = 0; j < 256; j++)
            {
                // заполняем палитру, для простоты это будут все оттенки серого
                palette.Entries[j] = Color.FromArgb(j, j, j);
            }
            // это не просто так, обязательно нужна эта строка
            bmp8.Palette = palette;

            var data = bmp8.LockBits(new Rectangle(0, 0, 300, 50), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            var bytes = new byte[data.Height * data.Stride];
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            for (int x = 0; x < 300; x++)
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
            //return bmp32;

        }

        private Bitmap _createRightMarker(string streetname)
        {
            Bitmap bmp8 = null;
            //***********************************
            var bmp32 = new Bitmap(300, 50, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(bmp32))
            {
                RectangleF rectangleF = new RectangleF(33.0F, 17.0f, bmp32.Width, bmp32.Height);
                g.Clear(Color.LightGray);
                PointF pt1 = new PointF(0.0f, 25.0f);
                PointF pt2 = new PointF(30.0f, 25.0f);
                PointF pt3 = new PointF(30.0f, 25.0f);
                PointF pt4 = new PointF(20.0f, 15.0f);
                PointF pt5 = new PointF(30.0f, 25.0f);
                PointF pt6 = new PointF(20.0f, 35.0f);
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
                    //LineAlignment = StringAlignment.Center,
                };
                g.DrawString(streetname, new Font("arial", 10), Brushes.Black, rectangleF, sf);
                g.Flush();
            }
            // создаем битмап с палитрой
            bmp8 = new Bitmap(300, 50, PixelFormat.Format8bppIndexed);
            var palette = bmp8.Palette;
            for (int j = 0; j < 256; j++)
            {
                // заполняем палитру, для простоты это будут все оттенки серого
                palette.Entries[j] = Color.FromArgb(j, j, j);
            }
            // это не просто так, обязательно нужна эта строка
            bmp8.Palette = palette;

            var data = bmp8.LockBits(new Rectangle(0, 0, 300, 50), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            var bytes = new byte[data.Height * data.Stride];
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            for (int x = 0; x < 300; x++)
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
            //return bmp32;

        }

        private Bitmap _createRightUpMarker(string streetname)
        {
            Bitmap bmp8 = null;
            //***********************************
            var bmp32 = new Bitmap(300, 50, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(bmp32))
            {
                RectangleF rectangleF = new RectangleF(33.0F, 17.0f, bmp32.Width, bmp32.Height);
                g.Clear(Color.LightGray);
                PointF pt1 = new PointF(20.0f, 0.0f);
                PointF pt2 = new PointF(20.0f, 30.0f);
                PointF pt3 = new PointF(20.0f, 0.0f);
                PointF pt4 = new PointF(10.0f, 20.0f);
                PointF pt5 = new PointF(20.0f, 0.0f);
                PointF pt6 = new PointF(30.0f, 20.0f);
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
                    //LineAlignment = StringAlignment.Center,
                };
                g.DrawString(streetname, new Font("arial", 10), Brushes.Black, rectangleF, sf);
                g.Flush();
            }
            // создаем битмап с палитрой
            bmp8 = new Bitmap(300, 50, PixelFormat.Format8bppIndexed);
            var palette = bmp8.Palette;
            for (int j = 0; j < 256; j++)
            {
                // заполняем палитру, для простоты это будут все оттенки серого
                palette.Entries[j] = Color.FromArgb(j, j, j);
            }
            // это не просто так, обязательно нужна эта строка
            bmp8.Palette = palette;

            var data = bmp8.LockBits(new Rectangle(0, 0, 300, 50), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            var bytes = new byte[data.Height * data.Stride];
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            for (int x = 0; x < 300; x++)
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
            //return bmp32;

        }


        private string _addrParse(string str)
        {
            string rez;
            int index = str.IndexOf(';');
            if (index > 0)
                str = str.Remove(index);
            int index1 = str.IndexOf('(');
            if (index1 > 0)
                str = str.Remove(index1);

            rez = str.Trim();

            return rez;
        }
    }
}
