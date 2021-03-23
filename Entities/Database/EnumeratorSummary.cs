using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class EnumeratorSummary
    {
        //private List<VesselUnload> _unloadsForMonth;
        public EnumeratorSummary() { }

        public string Gear { get; set; }

        public DateTime UploadDate { get; set; }
        public string UploadDateString { get { return UploadDate.ToString("MMM dd, yyyy HH:mm"); } }
        public string EnumeratorName { get; set; }
        public string LandingSite { get; set; }
        //public List<VesselUnload> VesselUnloads { get { return _unloadsForMonth; } }

        public int NumberOfLandingsWithCatchComposition { get; set; }
        public int NumberOfLandingsSampled { get; set; }
        public DateTime MonthOfSampling { get; set; }
        public DateTime DateOfFirstSampling { get; set; }

        public string FirstSamplingDate { get { return DateOfFirstSampling.ToString("MMM dd, yyyy HH:mm"); } }
        public string LastSamplingDate { get { return DateOfLatestSampling.ToString("MMM dd, yyyy HH:mm"); } }
        public DateTime DateOfLatestSampling { get;  set; }

        public List<VesselUnload> VesselUnloads { get; set; }


    }
}
