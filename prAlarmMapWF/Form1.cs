using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Configuration;
using System.Globalization;

using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;

using prAlarmMapWF.DbServices;
using prAlarmMapWF.Config;
using prAlarmMapWF.Data;

using NLog;

namespace prAlarmMapWF
{
    public partial class Map : Form
    { 

        List<CGeoLocData> cGeoLocDatas = new List<CGeoLocData>();

        BackgroundWorker mapBgWorker = null;

        Logger cLog = NLog.LogManager.GetLogger("commonLog");


        public Map()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.LightGray;
            
            //**********************************************************
            Program.EndWork = false;
            //**********************************************************
            AlarmMap.Overlays.Add(AlarmmarkersOverlay);
            AlarmMap.Overlays.Add(AlarmmarkersOverlayp13);
            AlarmMap.Overlays.Add(AlarmmpolyOverlay);
            //**********************************************************

            //**************************************************************
            Program.n04s = Readn04Tbl._getn04();
            Program.n03s = Readn03Tbl._getn03();
            
            cGeoLocDatas = CGeoLocation._getGeoloc();
           
            //**************************************************************

            #region Закулисье
            mapBgWorker = new BackgroundWorker();
            mapBgWorker.WorkerReportsProgress = true;
            mapBgWorker.DoWork += Map_Work;
            mapBgWorker.RunWorkerCompleted += MapBgWorker_RunWorkerCompleted;
            mapBgWorker.ProgressChanged += MapBgWorker_ProgressChanged;
            #endregion


            //*****************************************************************
            #region Signal
            eventWait = new EventWaitHandle(true, EventResetMode.ManualReset);
            eventWaitProc = new EventWaitHandle(false, EventResetMode.AutoReset);
            #endregion

            //*****************************************************************

        }

        private void Map_Load(object sender, EventArgs e)
        {
            //markLeft.BackColor = ToolTipBackColor;

            #region MainRec
            List<PointLatLng> pointLatLngs = new List<PointLatLng>();
            //Левый верх
            pointLatLngs.Add(new PointLatLng(AlarmMap.ViewArea.Top, AlarmMap.ViewArea.Left));
            //Левый низ
            pointLatLngs.Add(new PointLatLng(AlarmMap.ViewArea.Top-AlarmMap.ViewArea.Size.HeightLat,
                AlarmMap.ViewArea.Left));
            //Правый низ
            pointLatLngs.Add(new PointLatLng(AlarmMap.ViewArea.Bottom, AlarmMap.ViewArea.Right));
            //Правый верх
            pointLatLngs.Add(new PointLatLng(AlarmMap.ViewArea.Bottom + AlarmMap.ViewArea.Size.HeightLat - 0.012,
                AlarmMap.ViewArea.Right));

            GMapPolygon mapPolygon = new GMapPolygon(pointLatLngs, "poly");
            Brush ToolTipBackColor = new SolidBrush(Color.Transparent);
            //mapPolygon.Fill = ToolTipBackColor;
            //mapPolygon.Stroke = new Pen(Color.Transparent,0);
            mapPolygon.IsVisible = false;

            AlarmmpolyOverlay.Polygons.Add(mapPolygon);
            #endregion

            #region SecondRec
            List<PointLatLng> pointLatLngsSecond = new List<PointLatLng>();
            //Левый верх
            pointLatLngsSecond.Add(new PointLatLng( AlarmMap.ViewArea.Top - 0.0070515349649, 
                AlarmMap.ViewArea.Right - AlarmMap.ViewArea.Size.WidthLng/4));
            //Левый низ
            pointLatLngsSecond.Add(new PointLatLng(AlarmMap.ViewArea.Top - AlarmMap.ViewArea.Size.HeightLat / 2,
                AlarmMap.ViewArea.Right - AlarmMap.ViewArea.Size.WidthLng/4));
            //Правый низ
            pointLatLngsSecond.Add(new PointLatLng(AlarmMap.ViewArea.Top - AlarmMap.ViewArea.Size.HeightLat / 2,
                AlarmMap.ViewArea.Right - 0.0281524658203));
            //Правый верх
            pointLatLngsSecond.Add(new PointLatLng(AlarmMap.ViewArea.Top - 0.0070515349649,
                AlarmMap.ViewArea.Right - 0.0281524658203));

            GMapPolygon mapPolygonSec = new GMapPolygon(pointLatLngsSecond, "polySecond");
            //Brush ToolTipBackColor = new SolidBrush(Color.Transparent);
            //mapPolygon.Fill = ToolTipBackColor;
            //mapPolygon.Stroke = new Pen(Color.Transparent,0);
            mapPolygonSec.IsVisible = false;

            AlarmmpolyOverlaySec.Polygons.Add(mapPolygonSec);
            #endregion

            #region ThirdRec
            List<PointLatLng> pointLatLngsThird = new List<PointLatLng>();
            //Левый верх
            pointLatLngsThird.Add(new PointLatLng(AlarmMap.ViewArea.Top - 0.0070515349649,
                AlarmMap.ViewArea.Left + AlarmMap.ViewArea.Size.WidthLng / 2));
            //Левый низ
            pointLatLngsThird.Add(new PointLatLng(AlarmMap.ViewArea.Top - AlarmMap.ViewArea.Size.HeightLat / 2,
                AlarmMap.ViewArea.Left + AlarmMap.ViewArea.Size.WidthLng / 2));
            //Правый низ
            pointLatLngsThird.Add(new PointLatLng(AlarmMap.ViewArea.Top - AlarmMap.ViewArea.Size.HeightLat / 2,
                AlarmMap.ViewArea.Right - AlarmMap.ViewArea.Size.WidthLng / 4));
            //Правый верх
            pointLatLngsThird.Add(new PointLatLng(AlarmMap.ViewArea.Top - 0.0070515349649,
                AlarmMap.ViewArea.Right - AlarmMap.ViewArea.Size.WidthLng / 4));

            GMapPolygon mapPolygonThd = new GMapPolygon(pointLatLngsThird, "polyThird");
            mapPolygonThd.IsVisible = false;

            AlarmmpolyOverlaySec.Polygons.Add(mapPolygonThd);
            #endregion

            #region FourthRec
            List<PointLatLng> pointLatLngsFourth = new List<PointLatLng>();
            //Левый верх
            pointLatLngsFourth.Add(new PointLatLng(AlarmMap.ViewArea.Top - 0.0070515349649,
                AlarmMap.ViewArea.Left + AlarmMap.ViewArea.Size.WidthLng / 4));
            //Левый низ
            pointLatLngsFourth.Add(new PointLatLng(AlarmMap.ViewArea.Top - AlarmMap.ViewArea.Size.HeightLat / 2,
                AlarmMap.ViewArea.Left + AlarmMap.ViewArea.Size.WidthLng / 4));
            //Правый низ
            pointLatLngsFourth.Add(new PointLatLng(AlarmMap.ViewArea.Top - AlarmMap.ViewArea.Size.HeightLat / 2,
                AlarmMap.ViewArea.Left + AlarmMap.ViewArea.Size.WidthLng / 2));
            //Правый верх
            pointLatLngsFourth.Add(new PointLatLng(AlarmMap.ViewArea.Top - 0.0070515349649,
                AlarmMap.ViewArea.Left + AlarmMap.ViewArea.Size.WidthLng / 2));

            GMapPolygon mapPolygonFth = new GMapPolygon(pointLatLngsFourth, "polyFourth");
            mapPolygonFth.IsVisible = false;

            AlarmmpolyOverlaySec.Polygons.Add(mapPolygonFth);
            #endregion

            #region FifthRec
            List<PointLatLng> pointLatLngsFifth = new List<PointLatLng>();
            //Левый верх
            pointLatLngsFifth.Add(new PointLatLng(AlarmMap.ViewArea.Top - 0.0070515349649,
                AlarmMap.ViewArea.Left + 0.0281524658203));
            //Левый низ
            pointLatLngsFifth.Add(new PointLatLng(AlarmMap.ViewArea.Top - AlarmMap.ViewArea.Size.HeightLat / 2,
                AlarmMap.ViewArea.Left + 0.0281524658203));
            //Правый низ
            pointLatLngsFifth.Add(new PointLatLng(AlarmMap.ViewArea.Top - AlarmMap.ViewArea.Size.HeightLat / 2,
                AlarmMap.ViewArea.Left + AlarmMap.ViewArea.Size.WidthLng / 4));
            //Правый верх
            pointLatLngsFifth.Add(new PointLatLng(AlarmMap.ViewArea.Top - 0.0070515349649,
                AlarmMap.ViewArea.Left + AlarmMap.ViewArea.Size.WidthLng / 4));

            GMapPolygon mapPolygonFith = new GMapPolygon(pointLatLngsFifth, "polyFifth");
            mapPolygonFith.IsVisible = false;

            AlarmmpolyOverlaySec.Polygons.Add(mapPolygonFith);
            #endregion

            #region SixthRec
            List<PointLatLng> pointLatLngsSixth = new List<PointLatLng>();
            //Левый верх
            pointLatLngsSixth.Add(new PointLatLng(AlarmMap.ViewArea.Top - AlarmMap.ViewArea.Size.HeightLat / 2,
                AlarmMap.ViewArea.Left + 0.0281524658203));
            //Левый низ
            pointLatLngsSixth.Add(new PointLatLng(AlarmMap.ViewArea.Bottom + 0.0070515349649,
                AlarmMap.ViewArea.Left + 0.0281524658203));
            //Правый низ
            pointLatLngsSixth.Add(new PointLatLng(AlarmMap.ViewArea.Bottom + 0.0070515349649,
                AlarmMap.ViewArea.Left + AlarmMap.ViewArea.Size.WidthLng / 4));
            //Правый верх
            pointLatLngsSixth.Add(new PointLatLng(AlarmMap.ViewArea.Top - AlarmMap.ViewArea.Size.HeightLat / 2,
                AlarmMap.ViewArea.Left + AlarmMap.ViewArea.Size.WidthLng / 4));
                        

            GMapPolygon mapPolygonSixth = new GMapPolygon(pointLatLngsSixth, "polySixth");
            mapPolygonSixth.IsVisible = false;

            AlarmmpolyOverlaySec.Polygons.Add(mapPolygonSixth);
            #endregion

            #region SeventhRec
            List<PointLatLng> pointLatLngsSeventh = new List<PointLatLng>();
            //Левый верх
            pointLatLngsSeventh.Add(new PointLatLng(AlarmMap.ViewArea.Top - AlarmMap.ViewArea.Size.HeightLat / 2,
                AlarmMap.ViewArea.Left + AlarmMap.ViewArea.Size.WidthLng / 4));
            //Левый низ
            pointLatLngsSeventh.Add(new PointLatLng(AlarmMap.ViewArea.Bottom + 0.0070515349649,
                AlarmMap.ViewArea.Left + AlarmMap.ViewArea.Size.WidthLng / 4));
            //Правый низ
            pointLatLngsSeventh.Add(new PointLatLng(AlarmMap.ViewArea.Bottom + 0.0070515349649,
                AlarmMap.ViewArea.Left + AlarmMap.ViewArea.Size.WidthLng / 2));
            //Правый верх
            pointLatLngsSeventh.Add(new PointLatLng(AlarmMap.ViewArea.Top - AlarmMap.ViewArea.Size.HeightLat / 2,
                AlarmMap.ViewArea.Left + AlarmMap.ViewArea.Size.WidthLng / 2));

            GMapPolygon mapPolygonSev = new GMapPolygon(pointLatLngsSeventh, "polySeventh");
            mapPolygonSev.IsVisible = false;

            AlarmmpolyOverlaySec.Polygons.Add(mapPolygonSev);
            #endregion

            #region EighthRec
            List<PointLatLng> pointLatLngsEighth = new List<PointLatLng>();
            //Левый верх
            pointLatLngsEighth.Add(new PointLatLng(AlarmMap.ViewArea.Top - AlarmMap.ViewArea.Size.HeightLat / 2,
                AlarmMap.ViewArea.Left + AlarmMap.ViewArea.Size.WidthLng / 2));
            //Левый низ
            pointLatLngsEighth.Add(new PointLatLng(AlarmMap.ViewArea.Bottom + 0.0070515349649,
                AlarmMap.ViewArea.Left + AlarmMap.ViewArea.Size.WidthLng / 2));
            //Правый низ
            pointLatLngsEighth.Add(new PointLatLng(AlarmMap.ViewArea.Bottom + 0.0070515349649,
                AlarmMap.ViewArea.Right - AlarmMap.ViewArea.Size.WidthLng / 4));
            //Правый верх
            pointLatLngsEighth.Add(new PointLatLng(AlarmMap.ViewArea.Top - AlarmMap.ViewArea.Size.HeightLat / 2,
                AlarmMap.ViewArea.Right - AlarmMap.ViewArea.Size.WidthLng / 4));

            GMapPolygon mapPolygonEight = new GMapPolygon(pointLatLngsEighth, "polyEighth");
            mapPolygonEight.IsVisible = false;

            AlarmmpolyOverlaySec.Polygons.Add(mapPolygonEight);
            #endregion

            #region NinethRec
            List<PointLatLng> pointLatLngsNineth = new List<PointLatLng>();
            //Левый верх
            pointLatLngsNineth.Add(new PointLatLng(AlarmMap.ViewArea.Top - AlarmMap.ViewArea.Size.HeightLat / 2,
                AlarmMap.ViewArea.Right - AlarmMap.ViewArea.Size.WidthLng / 4));
            //Левый низ
            pointLatLngsNineth.Add(new PointLatLng(AlarmMap.ViewArea.Bottom + 0.0070515349649,
                AlarmMap.ViewArea.Right - AlarmMap.ViewArea.Size.WidthLng / 4));
            //Правый низ
            pointLatLngsNineth.Add(new PointLatLng(AlarmMap.ViewArea.Bottom + 0.0070515349649,
                AlarmMap.ViewArea.Right - 0.0281524658203));
            //Правый верх
            pointLatLngsNineth.Add(new PointLatLng(AlarmMap.ViewArea.Top - AlarmMap.ViewArea.Size.HeightLat / 2,
                AlarmMap.ViewArea.Right - 0.0281524658203));

            GMapPolygon mapPolygonNine = new GMapPolygon(pointLatLngsNineth, "polyNineth");
            mapPolygonNine.IsVisible = false;

            AlarmmpolyOverlaySec.Polygons.Add(mapPolygonNine);
            #endregion

            #region TenthRec
            List<PointLatLng> pointLatLngsTenth = new List<PointLatLng>();
            //Левый верх
            pointLatLngsTenth.Add(new PointLatLng(AlarmMap.ViewArea.Top,
                AlarmMap.ViewArea.Left + AlarmMap.ViewArea.Size.WidthLng / 2));
            //Левый низ
            pointLatLngsTenth.Add(new PointLatLng(AlarmMap.ViewArea.Top - 0.0070515349649,
                AlarmMap.ViewArea.Left + AlarmMap.ViewArea.Size.WidthLng / 2));
            //Правый низ
            pointLatLngsTenth.Add(new PointLatLng(AlarmMap.ViewArea.Top - 0.0070515349649,
                AlarmMap.ViewArea.Right - 0.0281524658203));
            //Правый верх
            pointLatLngsTenth.Add(new PointLatLng(AlarmMap.ViewArea.Top,
                AlarmMap.ViewArea.Right - 0.0281524658203));

            GMapPolygon mapPolygonTen = new GMapPolygon(pointLatLngsTenth, "polyTenth");
            mapPolygonTen.IsVisible = false;

            AlarmmpolyOverlaySec.Polygons.Add(mapPolygonTen);
            #endregion

            #region EleventhRec
            List<PointLatLng> pointLatLngsEleventh = new List<PointLatLng>();
            //Левый верх
            pointLatLngsEleventh.Add(new PointLatLng(AlarmMap.ViewArea.Top,
                AlarmMap.ViewArea.Left + 0.0281524658203));
            //Левый низ
            pointLatLngsEleventh.Add(new PointLatLng(AlarmMap.ViewArea.Top - 0.0070515349649,
                AlarmMap.ViewArea.Left + 0.0281524658203));
            //Правый низ
            pointLatLngsEleventh.Add(new PointLatLng(AlarmMap.ViewArea.Top - 0.0070515349649,
                AlarmMap.ViewArea.Left + AlarmMap.ViewArea.Size.WidthLng / 2));
            //Правый верх
            pointLatLngsEleventh.Add(new PointLatLng(AlarmMap.ViewArea.Top,
                AlarmMap.ViewArea.Left + AlarmMap.ViewArea.Size.WidthLng / 2));

            GMapPolygon mapPolygonElev = new GMapPolygon(pointLatLngsEleventh, "polyEleventh");
            mapPolygonElev.IsVisible = false;

            AlarmmpolyOverlaySec.Polygons.Add(mapPolygonElev);
            #endregion

            #region TvelnthRec
            List<PointLatLng> pointLatLngsTvelvth = new List<PointLatLng>();
            //Левый верх
            pointLatLngsTvelvth.Add(new PointLatLng(AlarmMap.ViewArea.Top,
                AlarmMap.ViewArea.Left ));
            //Левый низ
            pointLatLngsTvelvth.Add(new PointLatLng(AlarmMap.ViewArea.Bottom,
                AlarmMap.ViewArea.Left));
            //Правый низ
            pointLatLngsTvelvth.Add(new PointLatLng(AlarmMap.ViewArea.Bottom,
                AlarmMap.ViewArea.Left + 0.0281524658203));
            //Правый верх
            pointLatLngsTvelvth.Add(new PointLatLng(AlarmMap.ViewArea.Top,
                AlarmMap.ViewArea.Left + 0.0281524658203));

            GMapPolygon mapPolygonTvel = new GMapPolygon(pointLatLngsTvelvth, "polyTvelth");
            mapPolygonTvel.IsVisible = false;

            AlarmmpolyOverlaySec.Polygons.Add(mapPolygonTvel);
            #endregion

            #region ThirteenthRec
            List<PointLatLng> pointLatLngsThirteenth = new List<PointLatLng>();
            //Левый верх
            pointLatLngsThirteenth.Add(new PointLatLng(AlarmMap.ViewArea.Bottom + 0.0070515349649,
                AlarmMap.ViewArea.Left + 0.0281524658203));
            //Левый низ
            pointLatLngsThirteenth.Add(new PointLatLng(AlarmMap.ViewArea.Bottom,
                AlarmMap.ViewArea.Left + 0.0281524658203));
            //Правый низ
            pointLatLngsThirteenth.Add(new PointLatLng(AlarmMap.ViewArea.Bottom,
                AlarmMap.ViewArea.Left + AlarmMap.ViewArea.Size.WidthLng / 2));
            //Правый верх
            pointLatLngsThirteenth.Add(new PointLatLng(AlarmMap.ViewArea.Bottom + 0.0070515349649,
                AlarmMap.ViewArea.Left + AlarmMap.ViewArea.Size.WidthLng / 2));

            GMapPolygon mapPolygonThirteen = new GMapPolygon(pointLatLngsThirteenth, "polyThirteenth");
            mapPolygonThirteen.IsVisible = false;

            AlarmmpolyOverlaySec.Polygons.Add(mapPolygonThirteen);
            #endregion

            #region FourteenthRec
            List<PointLatLng> pointLatLngsFourteenth = new List<PointLatLng>();
            //Левый верх
            pointLatLngsFourteenth.Add(new PointLatLng(AlarmMap.ViewArea.Bottom + 0.0070515349649,
                AlarmMap.ViewArea.Left + AlarmMap.ViewArea.Size.WidthLng / 2));
            //Левый низ
            pointLatLngsFourteenth.Add(new PointLatLng(AlarmMap.ViewArea.Bottom,
                AlarmMap.ViewArea.Left + AlarmMap.ViewArea.Size.WidthLng / 2));
            //Правый низ
            pointLatLngsFourteenth.Add(new PointLatLng(AlarmMap.ViewArea.Bottom,
                AlarmMap.ViewArea.Right - 0.0281524658203));
            //Правый верх
            pointLatLngsFourteenth.Add(new PointLatLng(AlarmMap.ViewArea.Bottom + 0.0070515349649,
                AlarmMap.ViewArea.Right - 0.0281524658203));

            GMapPolygon mapPolygonFourteen = new GMapPolygon(pointLatLngsFourteenth, "polyFourteenth");
            mapPolygonFourteen.IsVisible = false;

            AlarmmpolyOverlaySec.Polygons.Add(mapPolygonFourteen);
            #endregion

            #region FifteenthRec
            List<PointLatLng> pointLatLngsFifteenth = new List<PointLatLng>();
            //Левый верх
            pointLatLngsFifteenth.Add(new PointLatLng(AlarmMap.ViewArea.Top,
                AlarmMap.ViewArea.Right - 0.0281524658203));
            //Левый низ
            pointLatLngsFifteenth.Add(new PointLatLng(AlarmMap.ViewArea.Bottom,
                AlarmMap.ViewArea.Right - 0.0281524658203));
            //Правый низ
            pointLatLngsFifteenth.Add(new PointLatLng(AlarmMap.ViewArea.Bottom,
                AlarmMap.ViewArea.Right));
            //Правый верх
            pointLatLngsFifteenth.Add(new PointLatLng(AlarmMap.ViewArea.Top,
                AlarmMap.ViewArea.Right));

            GMapPolygon mapPolygonFifteen = new GMapPolygon(pointLatLngsFifteenth, "polyFifteenth");
            mapPolygonFifteen.IsVisible = false;

            AlarmmpolyOverlaySec.Polygons.Add(mapPolygonFifteen);
            #endregion



            Height1per100 = AlarmMap.ViewArea.HeightLat/100;
            WWidth1per100 = AlarmMap.ViewArea.WidthLng/100;

            AlarmMapViewAreaSizeHeightLat = AlarmMap.ViewArea.Size.HeightLat;
            AlarmMapViewAreaSizeWidthLng = AlarmMap.ViewArea.Size.WidthLng;
            //cLog.Info($"Map_Load HeightLat: {AlarmMap.ViewArea.HeightLat}  WidthLng: {AlarmMap.ViewArea.WidthLng}");

            //MessageBox.Show($"WidthLng {AlarmMap.ViewArea.WidthLng}");
            //MessageBox.Show($"HeightLat {AlarmMap.ViewArea.HeightLat}");

            mapBgWorker.RunWorkerAsync();
        }

        private void AlarmMap_Load(object sender, EventArgs e)
        {
            //Настройки для компонента GMap
            AlarmMap.Bearing = 0; //Горизонтальное положение карты
            //Возможность перетаскивать карту левой кнопкой мыши
            AlarmMap.CanDragMap = false;
            AlarmMap.DragButton = MouseButtons.Left;

            AlarmMap.GrayScaleMode = true;

            //Отображение всех маркеров
            AlarmMap.MarkersEnabled = true;

            //Максимальное/Минимальное приближения
            AlarmMap.MaxZoom = 18;
            AlarmMap.MinZoom = 12; //12 -> 7

            //Курсор мыши в центр карты
            AlarmMap.MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;

            //Отключение негативного режима
            AlarmMap.NegativeMode = false;

            //Разрешение полигонов
            AlarmMap.PolygonsEnabled = true;
            //Разрешение маршрутов
            AlarmMap.RoutesEnabled = true;

            //Скрытие внешней сетки карты
            AlarmMap.ShowTileGridLines = false;
                       
            //Убрать красный крестик по центру
            AlarmMap.ShowCenter = false;

            //Чья карта используется
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            AlarmMap.MapProvider = GMapProviders.CzechMap;
            //Необходимо подколючение к интернету
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            

            //Стартовый центр карты
            AlarmMap.Position = new PointLatLng(cPoint.Y, cPoint.X);

            //При загрузке 13ти кратное увеличение
            AlarmMap.Zoom = 12.99;


            /*
            //Инициализация маркера и его координат
            try
            {   
                
                 //* ~40074км (длина экватора) - 360гр
                 //* 111км - 1гр
                 //* 1км - 0.00900900....гр
                

                if (cPoint != null)
                {
                    //1) //max - + (0.009 * 9.6)
                         //max - + (0.009 * 26.0)
                    
                    double x = cPoint.X + 0.009 * 8.3; 
                    double y = cPoint.Y - 0.009 * 26.0; 

                    GMapMarker marker = new GMarkerGoogle(new PointLatLng(x, y), GMarkerGoogleType.red_pushpin);
                    //markerGoogle.ToolTip = new GMapRoundedToolTip(markerGoogle);
                    marker.ToolTip = new GMapBaloonToolTip(marker);

                    Brush ToolTipBackColor = new SolidBrush(Color.Transparent);
                    marker.ToolTip.Fill = ToolTipBackColor;
                    //Текст обображаемый с маркером
                    marker.ToolTipText = "Test";
                    marker.ToolTipMode = MarkerTooltipMode.Always;


                    //Добавляю маркер в список маркеров
                    markersOverlay.Markers.Add(marker);

                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error");
            }
            */
            //MessageBox.Show($"Lat - {AlarmMap.Position.Lat}  Long {AlarmMap.Position.Lng}");

            //MessageBox.Show($"CLat - {AlarmMap.ViewArea.LocationTopLeft.Lat}  CLong {AlarmMap.ViewArea.LocationTopLeft.Lng}");
            //MessageBox.Show($"CLat - {AlarmMap.ViewArea.LocationRightBottom.Lat}  CLong {AlarmMap.ViewArea.LocationRightBottom.Lng}");
            //MessageBox.Show($"CMLat - {AlarmMap.ViewArea.LocationMiddle.Lat}  CMLong {AlarmMap.ViewArea.LocationMiddle.Lng}");

            //MessageBox.Show($"WidthLng {AlarmMap.ViewArea.WidthLng}");
            //MessageBox.Show($"HeightLat {AlarmMap.ViewArea.HeightLat}");

            //MessageBox.Show($"Top {AlarmMap.ViewArea.Top}");
            //MessageBox.Show($"Left {AlarmMap.ViewArea.Left}");

            //MessageBox.Show($"Right {AlarmMap.ViewArea.Right}");
            //MessageBox.Show($"Bottom {AlarmMap.ViewArea.Bottom}");

            //MessageBox.Show($"Lat {AlarmMap.ViewArea.Lat}");
            //MessageBox.Show($"Lng {AlarmMap.ViewArea.Lng}");
            //MessageBox.Show($"Lng {AlarmMap.ViewArea.Size}");

            //textBox1.Text = AlarmMap.ViewArea.Top.ToString();
            //textBox2.Text = AlarmMap.ViewArea.Left.ToString();   

        }
        
        private void Map_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.EndWork = true;
            Thread.Sleep(5);
            if (mapBgWorker.IsBusy)
                mapBgWorker.Dispose();
        }

        private void AlarmMap_Scroll(object sender, ScrollEventArgs e)
        {
            AlarmMap.Position = new PointLatLng(cPoint.X, cPoint.Y);
        }

        private void AlarmMap_MouseClick(object sender, MouseEventArgs e)
        {
            tLan.Text = AlarmMap.FromLocalToLatLng(e.X,e.Y).Lat.ToString();
            tLng.Text = AlarmMap.FromLocalToLatLng(e.X, e.Y).Lng .ToString();   
        }

        private void bSize_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Top {AlarmMap.ViewArea.Top} Left {AlarmMap.ViewArea.Left}\n" +
                $"Right {AlarmMap.ViewArea.Right} Bottom {AlarmMap.ViewArea.Bottom}\n" +
                $"{AlarmMap.ViewArea.Size}"
                );
            
        }
    }
}
