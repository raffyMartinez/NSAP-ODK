using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class TrackedVesselUnloadSummary
    {
        public int ID { get; set; }
        public DateTime SamplingDateTime { get; set; }
        public int? NumberOfFishers { get; set; }
        public double NumberOfHoursFishing { get; set; }
        public string LandingSite { get; set; }
        public string Gear { get; set; }
        public string GPX { get; set; }
        public int CutOffForUndersizedLength { get; set; }
        public double CPUE { get; set; }
        public double PercentBerried { get; set; }
        public double PercentUndersized { get; set; }
    }
}
