using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class CalendarDay
    {
        public DateTime Day { get; set; }
        public List<CalendarDaySpecies> CalendarDaySpecieses { get; set; }

        public List<CalendarDaySpeciesMeasured> CalendarDaySpeciesesMeasured { get; set; }
        public List<CalendarDaySpeciesMeasured> CalendarDaySpeciesesMeasuredFemale { get; set; }
        public List<VesselUnload> VesselUnloads { get; set; }
        public int CountNumberOfLandings { get; set; }
        public int CountOfFishingOperations { get; set; }
        public double TotalWeightOfCatch { get; set; }
        public CalendarGearSector CalendarGearSector_Parent { get; set; }

        public bool IsSamplingDay { get; set; }
        public bool HasFishingOperation { get; set; }
        public override string ToString()
        {
            if (IsSamplingDay && HasFishingOperation)
            {
                return $"{CalendarGearSector_Parent.Gear}({CalendarGearSector_Parent.Sector}) - {Day.ToString("MMM-dd-yyyy")} n:{CountOfFishingOperations} wt:{TotalWeightOfCatch})";
            }
            else
            {
                return $"{Day.ToString("MMM-dd-yyyy")} NO FISHING OPERATION";
            }
        }
    }
}
