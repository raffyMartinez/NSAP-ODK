using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
namespace NSAP_ODK.Entities.ItemSources
{
    class RegionLandingSiteItemsSource : IItemsSource
    {

        public ItemCollection GetValues()
        {
            ItemCollection landingSites = new ItemCollection();
            foreach (var ls in NSAPEntities.NSAPRegionFMAFishingGround.LandingSites
                .OrderBy(t => t.LandingSite.LandingSiteName))
            {
                landingSites.Add(ls.LandingSite.LandingSiteID, ls.LandingSite.LandingSiteName);
            }
            return landingSites;

        }
    }
}
