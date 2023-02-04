using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class CalendarDayLineage
    {
        public VesselUnload VesselUnload { get; set; }
        public int RowID { get; set; }
        public int SamplingDayID { get; set; }
        public int GearUnloadID { get; set; }
        public int VesselUnloadID { get; set; }
        public DateTime SamplingDate { get; set; }
        public string GearName { get; set; }
        public string SamplingDateString { get { return SamplingDate.ToString("MMM-dd-yyyy"); } }

        public int? Grouping { get; set; }

        public int RowType{
        get
            {
                if(Grouping==null)
                {
                    return 1;
                }
                else
                {
                    if(((int)Grouping) % 2==0)
                    {
                        return 2;
                    }
                    else
                    {
                        return 3;
                    }
                }
            }
        }
        public override bool Equals(object obj)
        {
            CalendarDayLineage other = obj as CalendarDayLineage;
            return other != null && other.SamplingDate == this.SamplingDate && other.GearName==this.GearName;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = (int)2166136261;
                // Suitable nullity checks etc, of course :)
                hash = (hash * 16777619) ^ SamplingDate.GetHashCode();
                hash = (hash * 16777619) ^ GearName.GetHashCode();
                return hash;
            }
        }

        public string GroupingString
        {
            get
            {
                if (Grouping == null)
                {
                    return "";
                }
                else
                {
                    return $"[{((int)Grouping).ToString()}]";
                }
            }
        }

        public override string ToString()
        {
            return $"{GroupingString} {GearName} - {SamplingDateString} - {SamplingDayID} - {GearUnloadID} - {VesselUnloadID}";
        }
    }
}
