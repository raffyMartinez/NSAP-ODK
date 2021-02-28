using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
namespace NSAP_ODK.Entities.ItemSources
{
    public class GearsInNSAPRegionItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection gears = new ItemCollection();
            foreach (var gear in NSAPEntities.NSAPRegion.Gears
                .OrderBy(t => t.Gear.GearName))
            {
                gears.Add(gear.GearCode, gear.Gear.GearName);

            }
            return gears;

        }
    }
}
