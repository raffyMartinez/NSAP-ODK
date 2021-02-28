using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NSAP_ODK.Entities.ItemSources
{
    public class LandingSiteInFMAFishingGroundItemsSource : IItemsSource
    {

        public ItemCollection GetValues()
        {
            ItemCollection lss = new ItemCollection();
            foreach (var ls in NSAPEntities.NSAPRegionFMAFishingGround.LandingSites.OrderBy(t => t.LandingSite.LandingSiteName))
            {
                lss.Add(ls.LandingSite.LandingSiteID, ls.LandingSite.ToString()); ;
            }
            return lss;

        }
    }
}
