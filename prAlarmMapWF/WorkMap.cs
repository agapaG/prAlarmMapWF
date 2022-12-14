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

using NLog;

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
        Logger bloc = NLog.LogManager.GetLogger("LogBLoc");


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
        GMapOverlay AlarmmpolyOverlaySec = new GMapOverlay("polygonsSec");

              
        List<DataPackage> dataPackagesCurrent = null;
        List<CGeoLocData> workGeoLocs = new List<CGeoLocData>();

        List<string> geoLocBadNames = new List<string>();

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


            double leftout = AlarmMap.ViewArea.Top - 0.0070515349659;
            double rightout = AlarmMap.ViewArea.Top - 0.0070515349659;
            double delta = 0.006;
            for (int i = 0; i < Current.Count; ++i)
            {                
                double pLat = Current[i].Latitude - cPoint.Y;
                double pLong = Current[i].Longitude - cPoint.X;
                double Mod = Math.Sqrt(pLat*pLat + pLong*pLong);

                PointLatLng currenyPoint = new PointLatLng(Current[i].Latitude, Current[i].Longitude);
                
                                
                for (int j = 0; j < AlarmmpolyOverlay.Polygons.Count; j++)
                {                    
                    if (AlarmmpolyOverlay.Polygons[j].IsInside(currenyPoint))
                    {
                        for (int k = 0; k < AlarmmpolyOverlaySec.Polygons.Count; k++)
                        {
                            if (AlarmmpolyOverlaySec.Polygons[k].Name.Equals("polySecond"))
                            {
                                if (AlarmmpolyOverlaySec.Polygons[k].IsInside(currenyPoint))
                                {
                                    GMapMarker marker = new GMarkerGoogle(
                                        new PointLatLng(Current[i].Latitude, Current[i].Longitude), GMarkerGoogleType.red_small);
                                    var ToolTip = new GMapRoundedToolTip(marker)
                                    {       
                                        Foreground = new SolidBrush(Color.Black),
                                        TextPadding = new Size(5, 5),
                                        //Offset = new Point(-100, 0),
                                        Stroke = Pens.Black,
                                    };

                                    if (Current[i].Closed)
                                    {
                                        ToolTip.Fill = new SolidBrush(Color.LightGray);
                                    }
                                    else
                                    {
                                        if (Current[i].Color == 1)
                                            ToolTip.Fill = new SolidBrush(Color.OrangeRed);
                                        else if (Current[i].Color == 2)
                                            ToolTip.Fill = new SolidBrush(Color.Yellow);
                                        else if (Current[i].Color == 3)
                                            ToolTip.Fill = new SolidBrush(Color.LightGreen);
                                    }

                                    //ToolTip.Offset = new Point(-Current[i].AddrC.Length * 8, -30);
                                    ToolTip.Offset = new Point(-Current[i].AddrRender.Length * 8, -30);
                                    marker.ToolTip = ToolTip;
                                    marker.ToolTipMode = MarkerTooltipMode.Always;

                                    marker.ToolTipText = Current[i].AddrRender;
                                    AlarmmarkersOverlay.Markers.Add(marker);
                                    break;
                                }
                            }

                            if (AlarmmpolyOverlaySec.Polygons[k].Name.Equals("polyThird"))
                            {
                                if (AlarmmpolyOverlaySec.Polygons[k].IsInside(currenyPoint))
                                {
                                    GMapMarker marker = new GMarkerGoogle(
                                        new PointLatLng(Current[i].Latitude, Current[i].Longitude), GMarkerGoogleType.red_small);

                                    var ToolTip = new GMapRoundedToolTip(marker)
                                    {
                                        Foreground = new SolidBrush(Color.Black),
                                        TextPadding = new Size(5, 5),
                                        //Offset = new Point(-100, 0),
                                        Stroke = Pens.Black,
                                    };

                                    if (Current[i].Closed)
                                    {
                                        ToolTip.Fill = new SolidBrush(Color.LightGray);
                                    }
                                    else
                                    {
                                        if (Current[i].Color == 1)
                                            ToolTip.Fill = new SolidBrush(Color.OrangeRed);
                                        else if (Current[i].Color == 2)
                                            ToolTip.Fill = new SolidBrush(Color.Yellow);
                                        else if (Current[i].Color == 3)
                                            ToolTip.Fill = new SolidBrush(Color.LightGreen);
                                    }


                                    ToolTip.Offset = new Point(30, -30);
                                    marker.ToolTip = ToolTip;
                                    marker.ToolTipMode = MarkerTooltipMode.Always;

                                    marker.ToolTipText = Current[i].AddrRender;

                                    AlarmmarkersOverlay.Markers.Add(marker);
                                    break;
                                }
                            }

                            if (AlarmmpolyOverlaySec.Polygons[k].Name.Equals("polyFourth"))
                            {
                                if (AlarmmpolyOverlaySec.Polygons[k].IsInside(currenyPoint))
                                {
                                    GMapMarker marker = new GMarkerGoogle(
                                        new PointLatLng(Current[i].Latitude, Current[i].Longitude), GMarkerGoogleType.red_small);
                                    var ToolTip = new GMapRoundedToolTip(marker)
                                    {
                                        Foreground = new SolidBrush(Color.Black),
                                        TextPadding = new Size(5, 5),
                                        //Offset = new Point(-100, 0),
                                        Stroke = Pens.Black,
                                    };
                                    
                                    if (Current[i].Closed)
                                    {
                                        ToolTip.Fill = new SolidBrush(Color.LightGray);
                                    }
                                    else
                                    {
                                        if (Current[i].Color == 1)
                                            ToolTip.Fill = new SolidBrush(Color.OrangeRed);
                                        else if (Current[i].Color == 2)
                                            ToolTip.Fill = new SolidBrush(Color.Yellow);
                                        else if (Current[i].Color == 3)
                                            ToolTip.Fill = new SolidBrush(Color.LightGreen);
                                    }

                                    ToolTip.Offset = new Point(30, -30);
                                    marker.ToolTip = ToolTip;
                                    marker.ToolTipMode = MarkerTooltipMode.Always;

                                    marker.ToolTipText = Current[i].AddrRender;
                                    AlarmmarkersOverlay.Markers.Add(marker);
                                    break;
                                }
                            }

                            if (AlarmmpolyOverlaySec.Polygons[k].Name.Equals("polyFourth"))
                            {
                                if (AlarmmpolyOverlaySec.Polygons[k].IsInside(currenyPoint))
                                {
                                    GMapMarker marker = new GMarkerGoogle(
                                        new PointLatLng(Current[i].Latitude, Current[i].Longitude), GMarkerGoogleType.red_small);
                                    var ToolTip = new GMapRoundedToolTip(marker)
                                    {
                                        Foreground = new SolidBrush(Color.Black),
                                        TextPadding = new Size(5, 5),
                                        //Offset = new Point(-100, 0),
                                        Stroke = Pens.Black,
                                    };
                                    
                                    if (Current[i].Closed)
                                    {
                                        ToolTip.Fill = new SolidBrush(Color.LightGray);
                                    }
                                    else
                                    {
                                        if (Current[i].Color == 1)
                                            ToolTip.Fill = new SolidBrush(Color.OrangeRed);
                                        else if (Current[i].Color == 2)
                                            ToolTip.Fill = new SolidBrush(Color.Yellow);
                                        else if (Current[i].Color == 3)
                                            ToolTip.Fill = new SolidBrush(Color.LightGreen);
                                    }

                                    ToolTip.Offset = new Point(30, -30);
                                    marker.ToolTip = ToolTip;
                                    marker.ToolTipMode = MarkerTooltipMode.Always;

                                    marker.ToolTipText = Current[i].AddrRender;
                                    AlarmmarkersOverlay.Markers.Add(marker);
                                    break;
                                }
                            }

                            if (AlarmmpolyOverlaySec.Polygons[k].Name.Equals("polyFifth"))
                            {
                                if (AlarmmpolyOverlaySec.Polygons[k].IsInside(currenyPoint))
                                {
                                    GMapMarker marker = new GMarkerGoogle(
                                        new PointLatLng(Current[i].Latitude, Current[i].Longitude), GMarkerGoogleType.red_small);
                                    var ToolTip = new GMapRoundedToolTip(marker)
                                    {
                                        Foreground = new SolidBrush(Color.Black),
                                        TextPadding = new Size(5, 5),
                                        //Offset = new Point(-100, 0),
                                        Stroke = Pens.Black,
                                    };
                                    
                                    if (Current[i].Closed)
                                    {
                                        ToolTip.Fill = new SolidBrush(Color.LightGray);
                                    }
                                    else
                                    {
                                        if (Current[i].Color == 1)
                                            ToolTip.Fill = new SolidBrush(Color.OrangeRed);
                                        else if (Current[i].Color == 2)
                                            ToolTip.Fill = new SolidBrush(Color.Yellow);
                                        else if (Current[i].Color == 3)
                                            ToolTip.Fill = new SolidBrush(Color.LightGreen);
                                    }

                                    ToolTip.Offset = new Point(30, -30);
                                    marker.ToolTip = ToolTip;
                                    marker.ToolTipMode = MarkerTooltipMode.Always;

                                    marker.ToolTipText = Current[i].AddrRender;
                                    AlarmmarkersOverlay.Markers.Add(marker);
                                    break;
                                }
                            }

                            if (AlarmmpolyOverlaySec.Polygons[k].Name.Equals("polySixth"))
                            {
                                if (AlarmmpolyOverlaySec.Polygons[k].IsInside(currenyPoint))
                                {
                                    GMapMarker marker = new GMarkerGoogle(
                                        new PointLatLng(Current[i].Latitude, Current[i].Longitude), GMarkerGoogleType.red_small);
                                    var ToolTip = new GMapRoundedToolTip(marker)
                                    {
                                        Foreground = new SolidBrush(Color.Black),
                                        TextPadding = new Size(5, 5),
                                        //Offset = new Point(-100, 0),
                                        Stroke = Pens.Black,
                                    };

                                    if (Current[i].Closed)
                                    {
                                        ToolTip.Fill = new SolidBrush(Color.LightGray);
                                    }
                                    else
                                    {
                                        if (Current[i].Color == 1)
                                            ToolTip.Fill = new SolidBrush(Color.OrangeRed);
                                        else if (Current[i].Color == 2)
                                            ToolTip.Fill = new SolidBrush(Color.Yellow);
                                        else if (Current[i].Color == 3)
                                            ToolTip.Fill = new SolidBrush(Color.LightGreen);
                                    }

                                    ToolTip.Offset = new Point(30, -30);
                                    marker.ToolTip = ToolTip;
                                    marker.ToolTipMode = MarkerTooltipMode.Always;

                                    marker.ToolTipText = Current[i].AddrRender;
                                    AlarmmarkersOverlay.Markers.Add(marker);
                                    break;
                                }
                            }

                            if (AlarmmpolyOverlaySec.Polygons[k].Name.Equals("polySeventh"))
                            {
                                if (AlarmmpolyOverlaySec.Polygons[k].IsInside(currenyPoint))
                                {
                                    GMapMarker marker = new GMarkerGoogle(
                                        new PointLatLng(Current[i].Latitude, Current[i].Longitude), GMarkerGoogleType.red_small);
                                    var ToolTip = new GMapRoundedToolTip(marker)
                                    {
                                        Foreground = new SolidBrush(Color.Black),
                                        TextPadding = new Size(5, 5),
                                        //Offset = new Point(-100, 0),
                                        Stroke = Pens.Black,
                                    };

                                    if (Current[i].Closed)
                                    {
                                        ToolTip.Fill = new SolidBrush(Color.LightGray);
                                    }
                                    else
                                    {
                                        if (Current[i].Color == 1)
                                            ToolTip.Fill = new SolidBrush(Color.OrangeRed);
                                        else if (Current[i].Color == 2)
                                            ToolTip.Fill = new SolidBrush(Color.Yellow);
                                        else if (Current[i].Color == 3)
                                            ToolTip.Fill = new SolidBrush(Color.LightGreen);
                                    }

                                    ToolTip.Offset = new Point(30, -30);
                                    marker.ToolTip = ToolTip;
                                    marker.ToolTipMode = MarkerTooltipMode.Always;

                                    marker.ToolTipText = Current[i].AddrRender;
                                    AlarmmarkersOverlay.Markers.Add(marker);
                                    break;
                                }
                            }

                            if (AlarmmpolyOverlaySec.Polygons[k].Name.Equals("polyEighth"))
                            {
                                if (AlarmmpolyOverlaySec.Polygons[k].IsInside(currenyPoint))
                                {
                                    GMapMarker marker = new GMarkerGoogle(
                                        new PointLatLng(Current[i].Latitude, Current[i].Longitude), GMarkerGoogleType.red_small);
                                    var ToolTip = new GMapRoundedToolTip(marker)
                                    {
                                        Foreground = new SolidBrush(Color.Black),
                                        TextPadding = new Size(5, 5),
                                        //Offset = new Point(-100, 0),
                                        Stroke = Pens.Black,
                                    };

                                    if (Current[i].Closed)
                                    {
                                        ToolTip.Fill = new SolidBrush(Color.LightGray);
                                    }
                                    else
                                    {
                                        if (Current[i].Color == 1)
                                            ToolTip.Fill = new SolidBrush(Color.OrangeRed);
                                        else if (Current[i].Color == 2)
                                            ToolTip.Fill = new SolidBrush(Color.Yellow);
                                        else if (Current[i].Color == 3)
                                            ToolTip.Fill = new SolidBrush(Color.LightGreen);
                                    }

                                    ToolTip.Offset = new Point(30, -30);
                                    marker.ToolTip = ToolTip;
                                    marker.ToolTipMode = MarkerTooltipMode.Always;

                                    marker.ToolTipText = Current[i].AddrRender;
                                    AlarmmarkersOverlay.Markers.Add(marker);

                                    break;
                                }
                            }

                            if (AlarmmpolyOverlaySec.Polygons[k].Name.Equals("polyNineth"))
                            {
                                if (AlarmmpolyOverlaySec.Polygons[k].IsInside(currenyPoint))
                                {
                                    GMapMarker marker = new GMarkerGoogle(
                                        new PointLatLng(Current[i].Latitude, Current[i].Longitude), GMarkerGoogleType.red_small);
                                    var ToolTip = new GMapRoundedToolTip(marker)
                                    {
                                        Foreground = new SolidBrush(Color.Black),
                                        TextPadding = new Size(5, 5),
                                        //Offset = new Point(-100, 0),
                                        Stroke = Pens.Black,
                                    };

                                    if (Current[i].Closed)
                                    {
                                        ToolTip.Fill = new SolidBrush(Color.LightGray);
                                    }
                                    else
                                    {
                                        if (Current[i].Color == 1)
                                            ToolTip.Fill = new SolidBrush(Color.OrangeRed);
                                        else if (Current[i].Color == 2)
                                            ToolTip.Fill = new SolidBrush(Color.Yellow);
                                        else if (Current[i].Color == 3)
                                            ToolTip.Fill = new SolidBrush(Color.LightGreen);
                                    }

                                    //ToolTip.Offset = new Point(-Current[i].AddrC.Length * 8, -30);
                                    ToolTip.Offset = new Point(-Current[i].AddrRender.Length * 8, -30);
                                    marker.ToolTip = ToolTip;
                                    marker.ToolTipMode = MarkerTooltipMode.Always;

                                    marker.ToolTipText = Current[i].AddrRender;
                                    AlarmmarkersOverlay.Markers.Add(marker);

                                    break;
                                }
                            }

                            if (AlarmmpolyOverlaySec.Polygons[k].Name.Equals("polyTenth"))
                            {
                                if (AlarmmpolyOverlaySec.Polygons[k].IsInside(currenyPoint))
                                {
                                    GMapMarker marker = new GMarkerGoogle(
                                        new PointLatLng(Current[i].Latitude, Current[i].Longitude), GMarkerGoogleType.red_small);
                                    var ToolTip = new GMapRoundedToolTip(marker)
                                    {
                                        Foreground = new SolidBrush(Color.Black),
                                        TextPadding = new Size(5, 5),
                                        //Offset = new Point(-100, 0),
                                        Stroke = Pens.Black,
                                    };

                                    if (Current[i].Closed)
                                    {
                                        ToolTip.Fill = new SolidBrush(Color.LightGray);
                                    }
                                    else
                                    {
                                        if (Current[i].Color == 1)
                                            ToolTip.Fill = new SolidBrush(Color.OrangeRed);
                                        else if (Current[i].Color == 2)
                                            ToolTip.Fill = new SolidBrush(Color.Yellow);
                                        else if (Current[i].Color == 3)
                                            ToolTip.Fill = new SolidBrush(Color.LightGreen);
                                    }

                                    ToolTip.Offset = new Point(0, 10);
                                    marker.ToolTip = ToolTip;
                                    marker.ToolTipMode = MarkerTooltipMode.Always;

                                    marker.ToolTipText = Current[i].AddrRender;
                                    AlarmmarkersOverlay.Markers.Add(marker);

                                    break;
                                }
                            }

                            if (AlarmmpolyOverlaySec.Polygons[k].Name.Equals("polyEleventh"))
                            {
                                if (AlarmmpolyOverlaySec.Polygons[k].IsInside(currenyPoint))
                                {
                                    GMapMarker marker = new GMarkerGoogle(
                                        new PointLatLng(Current[i].Latitude, Current[i].Longitude), GMarkerGoogleType.red_small);
                                    var ToolTip = new GMapRoundedToolTip(marker)
                                    {
                                        Foreground = new SolidBrush(Color.Black),
                                        TextPadding = new Size(5, 5),
                                        //Offset = new Point(-100, 0),
                                        Stroke = Pens.Black,
                                    };

                                    if (Current[i].Closed)
                                    {
                                        ToolTip.Fill = new SolidBrush(Color.LightGray);
                                    }
                                    else
                                    {
                                        if (Current[i].Color == 1)
                                            ToolTip.Fill = new SolidBrush(Color.OrangeRed);
                                        else if (Current[i].Color == 2)
                                            ToolTip.Fill = new SolidBrush(Color.Yellow);
                                        else if (Current[i].Color == 3)
                                            ToolTip.Fill = new SolidBrush(Color.LightGreen);
                                    }

                                    ToolTip.Offset = new Point(0, 10);
                                    marker.ToolTip = ToolTip;
                                    marker.ToolTipMode = MarkerTooltipMode.Always;

                                    marker.ToolTipText = Current[i].AddrRender;
                                    AlarmmarkersOverlay.Markers.Add(marker);

                                    break;
                                }
                            }

                            if (AlarmmpolyOverlaySec.Polygons[k].Name.Equals("polyTvelth"))
                            {
                                if (AlarmmpolyOverlaySec.Polygons[k].IsInside(currenyPoint))
                                {
                                    GMapMarker marker = new GMarkerGoogle(
                                        new PointLatLng(Current[i].Latitude, Current[i].Longitude), GMarkerGoogleType.red_small);
                                    var ToolTip = new GMapRoundedToolTip(marker)
                                    {
                                        Foreground = new SolidBrush(Color.Black),
                                        TextPadding = new Size(5, 5),
                                        //Offset = new Point(-100, 0),
                                        Stroke = Pens.Black,
                                    };

                                    if (Current[i].Closed)
                                    {
                                        ToolTip.Fill = new SolidBrush(Color.LightGray);
                                    }
                                    else
                                    {
                                        if (Current[i].Color == 1)
                                            ToolTip.Fill = new SolidBrush(Color.OrangeRed);
                                        else if (Current[i].Color == 2)
                                            ToolTip.Fill = new SolidBrush(Color.Yellow);
                                        else if (Current[i].Color == 3)
                                            ToolTip.Fill = new SolidBrush(Color.LightGreen);
                                    }

                                    ToolTip.Offset = new Point(10, 0);
                                    marker.ToolTip = ToolTip;
                                    marker.ToolTipMode = MarkerTooltipMode.Always;

                                    marker.ToolTipText = Current[i].AddrRender;
                                    AlarmmarkersOverlay.Markers.Add(marker);

                                    break;
                                }
                            }

                            if (AlarmmpolyOverlaySec.Polygons[k].Name.Equals("polyThirteenth"))
                            {
                                if (AlarmmpolyOverlaySec.Polygons[k].IsInside(currenyPoint))
                                {
                                    GMapMarker marker = new GMarkerGoogle(
                                        new PointLatLng(Current[i].Latitude, Current[i].Longitude), GMarkerGoogleType.red_small);
                                    var ToolTip = new GMapRoundedToolTip(marker)
                                    {
                                        Foreground = new SolidBrush(Color.Black),
                                        TextPadding = new Size(5, 5),
                                        //Offset = new Point(-100, 0),
                                        Stroke = Pens.Black,
                                    };

                                    if (Current[i].Closed)
                                    {
                                        ToolTip.Fill = new SolidBrush(Color.LightGray);
                                    }
                                    else
                                    {
                                        if (Current[i].Color == 1)
                                            ToolTip.Fill = new SolidBrush(Color.OrangeRed);
                                        else if (Current[i].Color == 2)
                                            ToolTip.Fill = new SolidBrush(Color.Yellow);
                                        else if (Current[i].Color == 3)
                                            ToolTip.Fill = new SolidBrush(Color.LightGreen);
                                    }

                                    ToolTip.Offset = new Point(0, -10);
                                    marker.ToolTip = ToolTip;
                                    marker.ToolTipMode = MarkerTooltipMode.Always;

                                    marker.ToolTipText = Current[i].AddrRender;
                                    AlarmmarkersOverlay.Markers.Add(marker);

                                    break;
                                }
                            }

                            if (AlarmmpolyOverlaySec.Polygons[k].Name.Equals("polyFourteenth"))
                            {
                                if (AlarmmpolyOverlaySec.Polygons[k].IsInside(currenyPoint))
                                {
                                    GMapMarker marker = new GMarkerGoogle(
                                        new PointLatLng(Current[i].Latitude, Current[i].Longitude), GMarkerGoogleType.red_small);
                                    var ToolTip = new GMapRoundedToolTip(marker)
                                    {
                                        Foreground = new SolidBrush(Color.Black),
                                        TextPadding = new Size(5, 5),
                                        //Offset = new Point(-100, 0),
                                        Stroke = Pens.Black,
                                    };

                                    if (Current[i].Closed)
                                    {
                                        ToolTip.Fill = new SolidBrush(Color.LightGray);
                                    }
                                    else
                                    {
                                        if (Current[i].Color == 1)
                                            ToolTip.Fill = new SolidBrush(Color.OrangeRed);
                                        else if (Current[i].Color == 2)
                                            ToolTip.Fill = new SolidBrush(Color.Yellow);
                                        else if (Current[i].Color == 3)
                                            ToolTip.Fill = new SolidBrush(Color.LightGreen);
                                    }

                                    ToolTip.Offset = new Point(0, -10);
                                    marker.ToolTip = ToolTip;
                                    marker.ToolTipMode = MarkerTooltipMode.Always;

                                    marker.ToolTipText = Current[i].AddrRender;
                                    AlarmmarkersOverlay.Markers.Add(marker);

                                    break;
                                }
                            }

                            if (AlarmmpolyOverlaySec.Polygons[k].Name.Equals("polyFifteenth"))
                            {
                                if (AlarmmpolyOverlaySec.Polygons[k].IsInside(currenyPoint))
                                {
                                    GMapMarker marker = new GMarkerGoogle(
                                        new PointLatLng(Current[i].Latitude, Current[i].Longitude), GMarkerGoogleType.red_small);
                                    var ToolTip = new GMapRoundedToolTip(marker)
                                    {
                                        Foreground = new SolidBrush(Color.Black),
                                        TextPadding = new Size(5, 5),
                                        //Offset = new Point(-100, 0),
                                        Stroke = Pens.Black,
                                    };

                                    if (Current[i].Closed)
                                    {
                                        ToolTip.Fill = new SolidBrush(Color.LightGray);
                                    }
                                    else
                                    {
                                        if (Current[i].Color == 1)
                                            ToolTip.Fill = new SolidBrush(Color.OrangeRed);
                                        else if (Current[i].Color == 2)
                                            ToolTip.Fill = new SolidBrush(Color.Yellow);
                                        else if (Current[i].Color == 3)
                                            ToolTip.Fill = new SolidBrush(Color.LightGreen);
                                    }

                                    //ToolTip.Offset = new Point(-Current[i].AddrC.Length * 7, 0);
                                    ToolTip.Offset = new Point(-Current[i].AddrRender.Length * 7, 0);
                                    marker.ToolTip = ToolTip;
                                    marker.ToolTipMode = MarkerTooltipMode.Always;

                                    marker.ToolTipText = Current[i].AddrRender;
                                    AlarmmarkersOverlay.Markers.Add(marker);

                                    break;
                                }
                            }
                        }
                    }
                    else
                    {                                                
                        if (pLong < 0)
                        {
                            GMapMarker marker = new GMarkerGoogle(
                                        new PointLatLng(leftout, AlarmMap.ViewArea.Left), GMarkerGoogleType.red_small);
                            var ToolTip = new GMapRoundedToolTip(marker)
                            {
                                Foreground = new SolidBrush(Color.Black),
                                TextPadding = new Size(5, 5),
                                //Offset = new Point(-100, 0),
                                Stroke = Pens.Black,
                            };

                            if (Current[i].Closed)
                            {
                                ToolTip.Fill = new SolidBrush(Color.LightGray);
                            }
                            else
                            {
                                if (Current[i].Color == 1)
                                    ToolTip.Fill = new SolidBrush(Color.OrangeRed);
                                else if (Current[i].Color == 2)
                                    ToolTip.Fill = new SolidBrush(Color.Yellow);
                                else if (Current[i].Color == 3)
                                    ToolTip.Fill = new SolidBrush(Color.LightGreen);
                            }

                            ToolTip.Offset = new Point(10, 0);
                            marker.ToolTip = ToolTip;
                            marker.ToolTipMode = MarkerTooltipMode.Always;
                            marker.Size = new Size(2, 2);   

                            marker.ToolTipText = Current[i].AddrRender;
                            AlarmmarkersOverlay.Markers.Add(marker);

                            leftout -= delta;

                        }
                        if (pLong > 0)
                        {
                            GMapMarker marker = new GMarkerGoogle(
                                       new PointLatLng(rightout, AlarmMap.ViewArea.Right), GMarkerGoogleType.red_small);
                            var ToolTip = new GMapRoundedToolTip(marker)
                            {
                                Foreground = new SolidBrush(Color.Black),
                                TextPadding = new Size(5, 5),
                                //Offset = new Point(-100, 0),
                                Stroke = Pens.Black,
                            };

                            if (Current[i].Closed)
                            {
                                ToolTip.Fill = new SolidBrush(Color.LightGray);
                            }
                            else
                            {
                                if (Current[i].Color == 1)
                                    ToolTip.Fill = new SolidBrush(Color.OrangeRed);
                                else if (Current[i].Color == 2)
                                    ToolTip.Fill = new SolidBrush(Color.Yellow);
                                else if (Current[i].Color == 3)
                                    ToolTip.Fill = new SolidBrush(Color.LightGreen);
                            }

                            //ToolTip.Offset = new Point(-Current[i].AddrC.Length*15, 0);
                            ToolTip.Offset = new Point(-Current[i].AddrRender.Length * 15, 0);
                            marker.ToolTip = ToolTip;
                            marker.ToolTipMode = MarkerTooltipMode.Always;
                            marker.Size = new Size(2, 2);

                            marker.ToolTipText = Current[i].AddrRender;
                            AlarmmarkersOverlay.Markers.Add(marker);

                            rightout -= delta;
                        }
                    }
                }
               
            }
            //!!!!!!!!!!!!!!!!!!!!!!!!!!
            //workGeoLocs.RemoveAt(workGeoLocs.Count - 1);
            //Thread.Sleep(15);
            if (workGeoLocs.Count != 0)
                workGeoLocs.Clear();
            //onetime = true;
            
            eventWait.Set();

        }

        private void Map_Work(object sender, DoWorkEventArgs e)
        {
            string city_1 = "г. Харьков,";
            string city_2 = "г.Харьков,";
            
            bool bFirst = true;

            while (!Program.EndWork)
            {                
                eventWait.WaitOne();

                try
                {
                    //**********************************************************************
                    dataPackagesCurrent = ReadBuff_WTbl.GetDataPackages();

                    for (int i = 0; i < dataPackagesCurrent.Count; i++)
                    {                        
                        CGeoLocData cGeoLocData = new CGeoLocData();
                        cGeoLocData = cGeoLocDatas.Find(item => item.AddrC.Equals(dataPackagesCurrent[i].N03s[0].Adr));
                        if (cGeoLocData != null)
                        {
                            if (dataPackagesCurrent[i].N03s[0].Status.Trim().Equals("Закрыт"))
                                cGeoLocData.Closed = true;
                            else
                                cGeoLocData.Closed = false;

                            string tmp = dataPackagesCurrent[i].Tcentral + "  ";
                            tmp += dataPackagesCurrent[i].Time + "  ";

                            string stravoid = cGeoLocData.AddrC;

                            int index = stravoid.IndexOf(city_1);
                            if (index > -1)
                                stravoid = stravoid.Substring(index + city_1.Length);
                            else
                            {
                                index = stravoid.IndexOf(city_2);
                                if (index > -1)
                                    stravoid = stravoid.Substring(index + city_2.Length);
                            }
                            tmp += stravoid;
                            if (cGeoLocData.Closed)
                                cGeoLocData.AddrRender = tmp + "  Закрыт";
                            else
                                cGeoLocData.AddrRender = tmp;
                            cGeoLocData.NCentral = dataPackagesCurrent[i].Tcentral;
                            cGeoLocData.Time = dataPackagesCurrent[i].Time;
                            cGeoLocData.Color = dataPackagesCurrent[i].Color;
                            workGeoLocs.Add(cGeoLocData);
                        }
                        else
                        {
                            if (bFirst)
                            {
                                geoLocBadNames.Add(dataPackagesCurrent[i].N03s[0].Adr);
                                bloc.Info($"{dataPackagesCurrent[i].Tcentral} {dataPackagesCurrent[i].N03s[0].Adr}");
                                bFirst = false;
                            }
                            else
                            {
                                string fstr = geoLocBadNames.Find(s => Equals(s, dataPackagesCurrent[i].N03s[0].Adr));
                                if (fstr == null)
                                {
                                    geoLocBadNames.Add(dataPackagesCurrent[i].N03s[0].Adr);
                                    bloc.Info($"{dataPackagesCurrent[i].Tcentral} {dataPackagesCurrent[i].N03s[0].Adr}");
                                }
                            }
                        }
                    }
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

                Thread.Sleep(900);

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
