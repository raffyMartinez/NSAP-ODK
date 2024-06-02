using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
namespace NSAP_ODK.Entities.ItemSources
{
    class WeightUnitItemSource : IItemsSource
    {
        public ItemCollection Units { get; internal set; } = new ItemCollection();
        public ItemCollection GetValues()
        {
            Units = new ItemCollection();
            Units.Add("kg", "Kilograms");
            Units.Add("g", "Grams");
            return Units;

        }
    }
}
