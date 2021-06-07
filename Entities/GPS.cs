using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using NSAP_ODK.Entities.ItemSources;

namespace NSAP_ODK.Entities
{
    public enum DeviceType
    {
        DeviceTypeNone,
        DeviceTypeGPS,
        DeviceTypePhone,
        DeviceTypeOther = 9

    }
    public class GPS
    {
        public string Code { get; set; }
        public string AssignedName { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        [ItemsSource(typeof(DeviceTypeItemsSource))]
        public DeviceType DeviceType { get; set; }
        public string DeviceTypeString
        {
            get
            {
                string deviceType = "None";
                switch (DeviceType)
                {
                    case DeviceType.DeviceTypeGPS:
                        deviceType = "GPS";
                        break;
                    case DeviceType.DeviceTypePhone:
                        deviceType = "Phone";
                        break;
                    case DeviceType.DeviceTypeOther:
                        deviceType = "Other";
                        break;
                }
                return deviceType;
            }
        }

        public override string ToString()
        {
            return AssignedName;
        }
    }
}
