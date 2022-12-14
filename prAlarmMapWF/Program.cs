using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using prAlarmMapWF.Data;
using NLog;


namespace prAlarmMapWF
{
    public static class Program
    {
        //******************************************************************************
        public static Logger outLog = NLog.LogManager.GetLogger("commonLog");
        public static bool EndWork { get; set; }
        public static List<n03> n03s = new List<n03>();
        public static List<n04> n04s = new List<n04>();
        public static List<CGeoLocData> geoLocs = new List<CGeoLocData>();
        internal static int nRec { get; set; }


        //*******************************************************************************


        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Map());
        }
    }
}
