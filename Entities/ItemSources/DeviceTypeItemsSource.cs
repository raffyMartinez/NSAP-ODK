using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NSAP_ODK.Entities.ItemSources
{
    class DeviceTypeItemsSource:IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection deviceTypes = new ItemCollection();
            deviceTypes.Add(DeviceType.DeviceTypeNone, "None");
            deviceTypes.Add(DeviceType.DeviceTypeGPS, "GPS");
            deviceTypes.Add(DeviceType.DeviceTypePhone, "Phone");
            deviceTypes.Add(DeviceType.DeviceTypeOther, "Other");
            return deviceTypes;

        }
    }
}
