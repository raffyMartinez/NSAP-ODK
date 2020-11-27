using System.Collections.Generic;

namespace NSAP_ODK.Entities
{
    public class NSAPRegion
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public int Sequence { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public List<NSAPRegionFMA> FMAs { get; set; }

        public List<NSAPRegionGear> Gears { get; set; }

        public List<NSAPRegionEnumerator> NSAPEnumerators { get; set; }

        public List<NSAPRegionFishingVessel> FishingVessels { get; set; }

        public NSAPRegion()
        {
            //FMAFishingGrounds = new List<NSAPRegionFMAFishingGround>();
            Gears = new List<NSAPRegionGear>();
            NSAPEnumerators = new List<NSAPRegionEnumerator>();
            FishingVessels = new List<NSAPRegionFishingVessel>();
            FMAs = new List<NSAPRegionFMA>();
        }
    }
}