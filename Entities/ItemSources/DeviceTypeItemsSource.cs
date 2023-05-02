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
            deviceTypes.Add("int", "Integers or whole numbers");
            deviceTypes.Add("dbl", "Doubles or numbers with decimal places");
            deviceTypes.Add("txt","Text values");
            return deviceTypes;

        }
    }
}
