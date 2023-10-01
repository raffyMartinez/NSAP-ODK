using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class LastPrimaryKeys
    {
        public void Reset()
        {
            LastVesselUnloadPK = 0;
            LastFishingGridsPK = 0;
            LastGearSoaksPK = 0;
            LastVesselEffortsPK = 0;
            LastVesselCatchPK = 0;
            LastLengthsPK = 0;
            LastLenWtPK = 0;
            LastLenFreqPK = 0;
            LastMaturityPK = 0;
            LastVesselUnloadGearPK = 0;
            LastVesselUnloadGearSpecPK = 0;
            LastLandingSiteSamplingPK = 0;
            LastGearUnloadPK = 0;
            LastUnloadStatPK = 0;
        }
        public int LastGearUnloadPK { get; set; }
        public int LastUnloadStatPK { get; set; }
        public int LastLandingSiteSamplingPK { get; set; }
        public int LastVesselUnloadPK { get; set; }
        public int LastFishingGridsPK { get; set; }

        public int LastGearSoaksPK { get; set; }
        public int LastVesselEffortsPK { get; set; }

        public int LastVesselCatchPK { get; set; }

        public int LastLengthsPK { get; set; }

        public int LastLenWtPK { get; set; }

        public int LastLenFreqPK { get; set; }

        public int LastMaturityPK { get; set; }

        public int LastVesselUnloadGearPK { get; set; }
        public int LastVesselUnloadGearSpecPK { get; set; }
    }
}
