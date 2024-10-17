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
        public int CountOfFishingOperations { get; set; }
        public double TotalWeightOfCatch { get; set; }
        public CalendarGearSector Parent { get; set; }

        public bool IsSamplingDay { get; set; }
        public bool HasFishingOperation { get; set; }
        public override string ToString()
        {
            if (IsSamplingDay && HasFishingOperation)
            {
                return $"{Parent.Gear}({Parent.Sector}) - {Day.ToString("MMM-dd-yyyy")} n:{CountOfFishingOperations} wt:{TotalWeightOfCatch})";
            }
            else
            {
                return $"{Day.ToString("MMM-dd-yyyy")} NO FISHING OPERATION";
            }
        }
    }
}
