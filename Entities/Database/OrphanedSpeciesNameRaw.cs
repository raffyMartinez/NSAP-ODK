using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class OrphanedSpeciesNameRaw
    {
        public string RegionName { get; set; }
        public string FMAName { get; set; }
        public string FishingGroundName { get; set; }
        public string LandingSiteName { get; set; }
        public int? LandingSiteID { get; set; }

        public string EnumeratorName { get; set; }
        public string EnumeratorText { get; set; }
        public string OrphanedSpName { get; set; }
        public string Taxa { get; set; }

        public string GearText { get; set; }

        public string GearName { get; set; }

        public int VesselUnloadID { get; set; }

        public int HashCode { get; set; }

        public override string ToString()
        {
            return $"{RegionName}, {FMAName}, {FishingGroundName}, {Taxa}, {OrphanedSpName}"; 
        }
    }
}
