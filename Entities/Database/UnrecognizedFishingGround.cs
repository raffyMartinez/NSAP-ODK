using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities.Database.FromJson;

namespace NSAP_ODK.Entities.Database
{
    public class UnrecognizedFishingGround
    {
        public string RowID { get; set; }
        public string FishingGroundCode { get; set; }

        public string FishingGroundName { get; set; }

        public int RegionFishingGround { get; set; }

        public string FishingGear { get; set; }

        public string LandingSite { get; set; }

        public string Enumerator { get; set; }

        public string FishingVessel { get; set; }

        public DateTime SamplingDate { get; set; }

        public string Region { get; set; }

        public string FMA { get; set; }

        public VesselLanding VesselLanding { get; set; }

        public bool Selected { get; set; }

    }
}
