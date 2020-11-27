using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
namespace NSAP_ODK.Entities
{
    internal class SectorTypeItemsSource:IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection valueTypes = new ItemCollection();
            valueTypes.Add("c", "Commercial");
            valueTypes.Add("m", "Municipal");
            return valueTypes;
        }
    }
}
