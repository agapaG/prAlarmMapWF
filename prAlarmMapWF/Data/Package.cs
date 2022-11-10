using System;
using System.Collections.Generic;

namespace prAlarmMapWF.Data
{
    public class DataPackage : IEquatable<DataPackage>
    {
        public string Tcentral { get; set; }
        public int Rec { get; set; }
        public string Time { get; set; }    
        public byte Color { get; set; } 

        public List<n03> N03s { get; set; }


        #region IEquatable method
        public bool Equals(DataPackage other)
        {
            if (this.Rec == other.Rec)
                return true;
            return false;
        }
        #endregion

        public override int GetHashCode()
        {
            return Rec.GetHashCode();
        }

        public DataPackage()
        {
            N03s = new List<n03>();
        }
    }
}
