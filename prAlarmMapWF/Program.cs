using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using prAlarmMapWF.Data;

namespace prAlarmMapWF
{
    public static class Program
    {
        public static List<n03> n03s = new List<n03>();

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
