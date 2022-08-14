using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;


namespace prAlarmMapWF
{
    public partial class Map : Form
    {
        GMarkerGoogle markerGoogle = null;
        //Создам список маркеров
        GMapOverlay markersOverlay = new GMapOverlay("markers");

        public Map()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.LightGray;
            
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
            AlarmMap.Zoom = 13.135;

            //Убрать красный крестик по центру
            AlarmMap.ShowCenter = false;

            //Чья карта используется
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            AlarmMap.MapProvider = GMapProviders.CzechMap;
            //Необходимо подколючение к интернету
            GMaps.Instance.Mode = AccessMode.ServerOnly;

            //Стартовый центр карты
            AlarmMap.Position = new PointLatLng(49.989897385959935, 36.22941235773933);


            //Инициализация маркера и его координат
            GeoCoderStatusCode geoCoder;
            PointLatLng dd = (PointLatLng)GMapProviders.OpenStreetMap.GetPoint("Україна, Харків, Академіка Проскури вулиця, 10А", out geoCoder);
            if (dd != null )
            {
                markerGoogle = new GMarkerGoogle(new PointLatLng(dd.Lat, dd.Lng), GMarkerGoogleType.red);
                markerGoogle.ToolTip = new GMapRoundedToolTip(markerGoogle);

                //dd = {{Lat=50,0445987, Lng=36,2824705789611}}

                //Текст обображаемый с маркером
                markerGoogle.ToolTipText = "Мой дом";

                //Добавляю маркер в список маркеров
                markersOverlay.Markers.Add(markerGoogle);
                AlarmMap.Overlays.Add(markersOverlay);
            }

        }
                
    }
}
