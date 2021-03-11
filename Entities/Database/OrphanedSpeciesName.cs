using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class OrphanedSpeciesName
    {
        public string Name { get; set; }

        public NSAPRegion Region { get { return SampledLandings[0].Parent.Parent.NSAPRegion; } }

        public FMA FMA { get { return SampledLandings[0].Parent.Parent.FMA; } }

        public FishingGround FishingGround { get { return SampledLandings[0].Parent.Parent.FishingGround; } }

        public int NumberOfLandings { get { return SampledLandings.Count; } }

      public List<VesselUnload> SampledLandings { get; set; }

       public bool ForReplacement { get; set; }

    }
}
