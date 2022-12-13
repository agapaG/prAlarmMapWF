using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prAlarmMapWF.Data
{
    public class CGeoLocData : IEquatable<CGeoLocData>
    {
        public string NCentral { get; set; }    
        public string Time { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string AddrC { get; set; }
        public string AddrRender { get; set; }
        public string AddrM { get; set; }
        public byte Color { get; set; }
        //public string State { get; set; }
        public bool Closed { get; set; }

        #region IEquatable method
        public bool Equals(CGeoLocData other)
        {
            if (this.AddrC == other.AddrC)
                return true;
            return false;
        }
        #endregion
        public override int GetHashCode()
        {
            return AddrC.GetHashCode();
        }

    }
}
