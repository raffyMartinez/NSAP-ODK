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
        public ItemCollection Gears { get; internal set; } = new ItemCollection();
        public ItemCollection GetValues()
        {
            if (UnloadGears == null)
            {
                foreach (var gear in NSAPEntities.GearViewModel.GearCollection.OrderBy(t => t.GearName))
                {
                    Gears.Add(gear.Code, gear.GearName);
                }
            }
            else
            {
                foreach (var item in UnloadGears)
                {
                    Gears.Add( item.GearCode, item.GearUsedName);
                }
            }
            if (AllowAddBlankGearName)
            {
                Gears.Add("", "");
            }
            return Gears;
        }

        public static bool AllowAddBlankGearName { get; set; }
        public static List<VesselUnload_FishingGear> UnloadGears { get; set; }

        public void AddItem(string code, string name)
        {
            Gears.Add(code, name);
        }

        public void AddItem(KeyValuePair<string, string> item)
        {
            Gears.Add(item.Key, item.Value);
        }



    }
}
