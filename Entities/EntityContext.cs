using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public class EntityContext:EventArgs
    {
        public NSAPRegion Region { get; set; }

        public FMA FMA { get; set; }

        public FishingGround FishingGround { get; set; }

        public NSAPEntity NSAPEntity { get; set; }

        public LandingSite LandingSite { get; set; }
    }
}
