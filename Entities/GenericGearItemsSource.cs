using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
namespace NSAP_ODK.Entities
{
    class GenericGearItemsSource:IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection gears = new ItemCollection();
            foreach (var gear in NSAPEntities.GearViewModel.GearCollection.Where(t=>t.IsGenericGear).OrderBy(t => t.GearName))
            {
                gears.Add(gear.Code, gear.GearName);
            }
            return gears;
        }
    }
}
