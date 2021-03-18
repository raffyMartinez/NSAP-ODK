using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
   public  class OrphanedFishingVessel
    {
        public string Name { get; set; }
        public List<VesselUnload> VesselUnloads{ get; set; }

        public NSAPRegion Region { get { return VesselUnloads[0].Parent.Parent.NSAPRegion; } }

        public FMA FMA { get { return VesselUnloads[0].Parent.Parent.FMA; } }

        public FishingGround FishingGround { get { return VesselUnloads[0].Parent.Parent.FishingGround; } }

        public string LandingSiteName { get { return VesselUnloads[0].Parent.Parent.LandingSiteName; } }

        public string Sector { get { return VesselUnloads[0].Sector; } }

        public int NumberOfUnload { get { return VesselUnloads.Count; } }

        public bool ForReplacement { get; set; }
    }
}
