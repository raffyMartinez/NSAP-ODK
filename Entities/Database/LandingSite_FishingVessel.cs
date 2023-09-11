using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class LandingSite_FishingVessel
    {
        public FishingVessel FishingVessel { get; set; }
        public LandingSite LandingSite { get; set; }
        public int PK { get; set; }

        public DateTime DateAdded { get; set; }
        public DateTime? DateRemoved { get; set; }
    }
}
