using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class OrphanedLandingSite
    {

        public string LandingSiteName { get; set; }
        public List<LandingSiteSampling> LandingSiteSamplings { get; set; }

        public NSAPRegion Region { get { return LandingSiteSamplings[0].NSAPRegion; } }

        public FMA FMA { get { return LandingSiteSamplings[0].FMA; } }

        public FishingGround FishingGround { get { return LandingSiteSamplings[0].FishingGround; } }

        public int NumberOfLandings { get { return LandingSiteSamplings.Count; } }
        
        public bool ForReplacement { get; set; }

    }
}
