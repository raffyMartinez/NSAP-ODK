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

        public string Region { get; set; }

        public string FMA { get; set; }

        public string FishingGround { get; set; }

        public int NumberOfLandings { get { return SampledLandings.Count; } }

        public List<VesselUnload> SampledLandings { get; set; }

        public bool ForReplacement { get; set; }

        public string Enumerator { get; set; }

        public string LandingSite { get; set; }

        public string Gear { get; set; }

        public string Taxa { get; set; }

        public int HashCode { get; set; }

    }
}
