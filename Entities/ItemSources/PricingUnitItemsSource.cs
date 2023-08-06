using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NSAP_ODK.Entities.ItemSources
{
    class PricingUnitItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection items = new ItemCollection();
            items.Add("kg", "Kilogram");
            items.Add("box", "Box");
            items.Add("other", "Other");
            return items;

        }
    }
}
