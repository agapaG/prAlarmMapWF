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

namespace prAlarmMapWF
{
    public partial class Map : Form
    { 
        DataTable geoData = null;   

        List<CGeoLocData> cGeoLocDatas = new List<CGeoLocData>();

        GMapOverlay markersOverlay = new GMapOverlay("Test");

        readonly BackgroundWorker mapBgWorker = null;
        EventWaitHandle eventWait;

        public Map()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.LightGray;
            //**********************************************************
            string extbl = ConfigurationManager.AppSettings["ExistTbl"];
            if (Equals(extbl, "0"))
            {
                CGeoLocation._create_geoloctbl("GeoLoc");
                ConfigHelper.AddUpdateAppSettings("ExistTbl", "1");
            }
            //**********************************************************
            Program.EndWork = false;
            //**********************************************************
            AlarmMap.Overlays.Add(AlarmmarkersOverlay);
            AlarmMap.Overlays.Add(markersOverlay);
            //**********************************************************

            //**************************************************************
            Program.n03s = Readn03Tbl._getn03();
            Program.geoLocs = CGeoLocation._getGeoloc();
           
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
            #endregion

            //*****************************************************************
                        
        }

        private void Map_Load(object sender, EventArgs e)
        {
            string[] split = null;
            string str;
            NumberFormatInfo provide = new NumberFormatInfo();
            provide.NumberDecimalSeparator = ",";
            try
            {
                using (StreamReader sr = new StreamReader("forshow"))
                {
                    while ((str = sr.ReadLine()) != null)
                    {
                        CGeoLocData cGeoLocData = new CGeoLocData();
                        split = str.Split('\t');
                        cGeoLocData.Latitude = Convert.ToDouble(split[0], provide);
                        cGeoLocData.Longitude = Convert.ToDouble(split[1], provide);
                        cGeoLocData.AddrC = split[2];
                        cGeoLocData.AddrM = split[3];

                        cGeoLocDatas.Add(cGeoLocData);
                    }
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }


            mapBgWorker.RunWorkerAsync();
        }


        private void AlarmMap_Load(object sender, EventArgs e)
        {
            //Настройки для компонента GMap
            AlarmMap.Bearing = 0; //Горизонтальное положение карты
            //Возможность перетаскивать карту левой кнопкой мыши
            AlarmMap.CanDragMap = true;
            AlarmMap.DragButton = MouseButtons.Left;

            AlarmMap.GrayScaleMode = true;

            //Отображение всех маркеров
            AlarmMap.MarkersEnabled = true;

            //Максимальное/Минимальное приближения
            AlarmMap.MaxZoom = 18;
            AlarmMap.MinZoom = 5; //12

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
            AlarmMap.Position = new PointLatLng(cPoint.X, cPoint.Y);

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
            
        }

        private void Map_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.EndWork = true;
            Thread.Sleep(5);
            if (mapBgWorker.IsBusy)
                mapBgWorker.Dispose();
        }
              
    }
}
