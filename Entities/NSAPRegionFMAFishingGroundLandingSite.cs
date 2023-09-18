using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
//using System.ComponentModel;
namespace NSAP_ODK.Entities
{
    public class NSAPRegionFMAFishingGroundLandingSite
    {

        //[ReadOnly(true)]
        public int RowID { get; set; }
        public NSAPRegionFMAFishingGround NSAPRegionFMAFishingGround { get; set; }
        public LandingSite LandingSite { get; set; }
        public int? NumberOfLandings { get { return LandingSite.CountLandings; } }
        public int? NumberOfFishingVessels { get { return LandingSite.CountFishingVessels; } }
        //[ItemsSource(typeof(LandingSiteItemsSource))]
        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; }

        public override string ToString()
        {
            return $"{LandingSite} - {NSAPRegionFMAFishingGround.FishingGround} - {NSAPRegionFMAFishingGround.RegionFMA.FMA}";
        }
    }
}
