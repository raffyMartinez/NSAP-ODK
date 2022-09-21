using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class GearAtLandingSiteDaysPerMonth
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public Gear Gear { get; set; }
        public LandingSite LandingSite { get; set; }
        public int DaysInOperation { get; set; }

        public int RowID { get; set; }
    }
}
