using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prAlarmMapWF.Services
{
    public class CPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public CPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
