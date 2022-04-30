using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class SampledLandingSite
    {
        public NSAPRegion NSAPRegion { get; set; }
        public FMA FMA { get; set; }
        public FishingGround FishingGround { get; set; }
        public string LandingSiteName { get; set; }

        public string LandingSiteText { get; set; }
        public string Barangay { get; set; }
        public Municipality Municipality { get; set; }
        public Province Province { get; set; }
        public int LandingSiteID { get; set; }
    }
}
