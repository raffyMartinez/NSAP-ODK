using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
namespace NSAP_ODK.Entities.ItemSources
{
    class FishingVesselInRegionItemsSource: IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection vessels = new ItemCollection();
            foreach (var v in NSAPEntities.NSAPRegion.FishingVessels
                .OrderBy(t => t.FishingVessel.ToString()))
            {
                vessels.Add(v.FishingVessel.ID, v.FishingVessel.ToString());
            }
            return vessels;

        }
    }
}
