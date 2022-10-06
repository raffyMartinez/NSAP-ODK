using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class OrphanedFishingGear
    {
        public string Name { get; set; }
        public List<GearUnload> GearUnloads{ get; set; }

        public NSAPRegion Region { 
            get 
            {
                if (GearUnloads.Count > 0)
                {
                    return GearUnloads[0].Parent.NSAPRegion;
                }
                else
                {
                    return null;
                }
            } 
        }

        public FMA FMA { 
            get 
            {
                if (GearUnloads.Count == 0)
                {
                    return null;
                }
                else
                {
                    return GearUnloads[0].Parent.FMA;
                }
            } 
        }

        public FishingGround FishingGround { get { return GearUnloads[0].Parent.FishingGround; } }

        public int NumberOfUnload { get { return GearUnloads.Count; } }

        public bool ForReplacement { get; set; }
    }
}
