using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Configuration;

using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;

using prAlarmMapWF.DbServices;
using prAlarmMapWF.Config;

namespace prAlarmMapWF
{
    public partial class Map : Form
    {
        GMarkerGoogle markerGoogle = null;
        //Создам список маркеров
        GMapOverlay AlarmmarkersOverlay = new GMapOverlay("Alarms");

        readonly BackgroundWorker mapBgWorker = null;

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
            //**********************************************************


            Program.n03s = Readn03Tbl._getn03();

            #region Закулисье
            mapBgWorker = new BackgroundWorker();
            mapBgWorker.WorkerReportsProgress = true;
            mapBgWorker.DoWork += Map_Work;
            mapBgWorker.RunWorkerCompleted += MapBgWorker_RunWorkerCompleted;
            mapBgWorker.ProgressChanged += MapBgWorker_ProgressChanged;
            #endregion

        }

        private void Map_Load(object sender, EventArgs e)
        {
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
            AlarmMap.MaxZoom = 20;
            AlarmMap.MinZoom = 2;

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

            //При загрузке 13ти кратное увеличение
            AlarmMap.Zoom = 12.95;

            //Убрать красный крестик по центру
            AlarmMap.ShowCenter = false;

            //Чья карта используется
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            AlarmMap.MapProvider = GMapProviders.CzechMap;
            //Необходимо подколючение к интернету
            GMaps.Instance.Mode = AccessMode.ServerOnly;

            //Стартовый центр карты
            AlarmMap.Position = new PointLatLng(49.989897385959935, 36.22941235773933);

            /*
            //Инициализация маркера и его координат
            try
            {
                GeoCoderStatusCode geoCoder;
                PointLatLng dd = (PointLatLng)GMapProviders.GoogleMap.GetPoint("Україна, Харків", out geoCoder);
                if (dd != null)
                {
                    markerGoogle = new GMarkerGoogle(new PointLatLng(dd.Lat, dd.Lng), GMarkerGoogleType.red);
                    markerGoogle.ToolTip = new GMapRoundedToolTip(markerGoogle);

                    //dd = {{Lat=50,0445987, Lng=36,2824705789611}}

                    //Текст обображаемый с маркером
                    markerGoogle.ToolTipText = "Мой дом";

                    //Добавляю маркер в список маркеров
                    AlarmmarkersOverlay.Markers.Add(markerGoogle);

                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error");
            }
            */
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
                AlarmmarkersOverlay.Clear();
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
