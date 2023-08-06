using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
namespace NSAP_ODK.Entities.ItemSources
{
    class GutContentItemsSource : IItemsSource
    {
        private ItemCollection _items = new ItemCollection();
        public ItemCollection GetValues()
        {
            _items.Add("", "");
            _items.Add("F", "Full");
            _items.Add("HF", "Half full");
            _items.Add("E", "Empty");

            return _items;
        }
    }
}
