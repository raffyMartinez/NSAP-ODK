using System.ComponentModel;

namespace NSAP_ODK.Entities
{
    public class FMA
    {
        public FMA()
        {
            //FishingGrounds = new List<NSAPRegionFMAFishingGround>();
        }

        [ReadOnly(true)]
        public int FMAID { get; set; }

        public string Name { get; set; }

        //public List<NSAPRegionFMAFishingGround> FishingGrounds {get;set;}
        public override string ToString()
        {
            return Name;
        }
    }
}