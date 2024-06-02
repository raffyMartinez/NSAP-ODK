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
        public ItemCollection PricingUnitItems { get; internal set; } = new ItemCollection();
        public ItemCollection GetValues()
        {
            PricingUnitItems = new ItemCollection();
            PricingUnitItems.Add("kg", "Kilogram");
            PricingUnitItems.Add("box", "Box");
            PricingUnitItems.Add("other", "Other");
            
            return PricingUnitItems;

        }
    }
}
