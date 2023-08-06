using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using NSAP_ODK.Entities.Database;
namespace NSAP_ODK.Entities.ItemSources

{
    public class GearsInNSAPRegionItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection gears = new ItemCollection();
            if (GearUnloads== null && UnloadGears==null)
            {
                foreach (var gear in NSAPEntities.NSAPRegion.Gears
                .OrderBy(t => t.Gear.GearName))
                {
                    gears.Add(gear.GearCode, gear.Gear.GearName);

                }

                if (AllowBlankGearName)
                {
                    gears.Add("", "");
                }
            }
            else if(GearUnloads!=null)
            {
                foreach (var item in GearUnloads)
                {
                    gears.Add(item.GearUsedName);
                }

            }
            else
            {
                foreach (var item in UnloadGears)
                {
                    gears.Add(item.GearUsedName);
                }
            }
            return gears;

        }

        public static VesselUnload VesselUnload { get; set; }

        public static bool AllowBlankGearName { get; set; }
        public static List<VesselUnload_FishingGear> UnloadGears { get; set; }

        public static List<GearUnload> GearUnloads { get; set; }
    }
}
