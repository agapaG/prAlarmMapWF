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

        List<CGeoLocData> cGeoLocDatas = new List<CGeoLocData>();

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
            AlarmMap.Overlays.Add(AlarmmarkersOverlayp13);
            AlarmMap.Overlays.Add(AlarmmpolyOverlay);
            //**********************************************************

            //**************************************************************
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
            #endregion

            //*****************************************************************
                        
        }

        private void Map_Load(object sender, EventArgs e)
        {
            //markLeft.BackColor = ToolTipBackColor;

            //Левый верх
            pointLatLngs.Add(new PointLatLng(AlarmMap.ViewArea.Top-0.012, AlarmMap.ViewArea.Left));
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

            Polygones = AlarmmpolyOverlay.Polygons;

            Height1per100 = AlarmMap.ViewArea.HeightLat/100;
            WWidth1per100 = AlarmMap.ViewArea.WidthLng/100;


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
            AlarmMap.MinZoom = 9; //12 -> 7

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
