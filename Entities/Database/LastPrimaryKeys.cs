using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class LastPrimaryKeys
    {
        public int LastVesselUnloadPK { get; set; }
        public int LastFishingGridsPK { get; set; }

        public int LastGearSoaksPK { get; set; }
        public int LastVesselEffortsPK { get; set; }

        public int LastVesselCatchPK { get; set; }

        public int LastLengthsPK { get; set; }

        public int LastLenWtPK { get; set; }

        public int LastLenFreqPK { get; set; }

        public int LastMaturityPK { get; set; }
    }
}
