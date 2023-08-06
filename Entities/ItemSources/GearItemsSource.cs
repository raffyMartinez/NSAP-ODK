using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using NSAP_ODK.Entities.Database;

namespace NSAP_ODK.Entities.ItemSources
{


    class GearItemsSource : IItemsSource
    {
        private ItemCollection _gears = new ItemCollection();
        public ItemCollection GetValues()
        {
            if (UnloadGears == null)
            {
                foreach (var gear in NSAPEntities.GearViewModel.GearCollection.OrderBy(t => t.GearName))
                {
                    _gears.Add(gear.Code, gear.GearName);
                }
                if (AllowAddBlankGearName)
                {
                    _gears.Add("", "");
                }
            }
            else
            {
                foreach (var item in UnloadGears)
                {
                    _gears.Add( item.GearUsedName);
                }
            }
            return _gears;
        }

        public static bool AllowAddBlankGearName { get; set; }
        public static List<VesselUnload_FishingGear> UnloadGears { get; set; }

        public void AddItem(string code, string name)
        {
            _gears.Add(code, name);
        }

        public void AddItem(KeyValuePair<string, string> item)
        {
            _gears.Add(item.Key, item.Value);
        }



    }
}
