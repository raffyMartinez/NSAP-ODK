using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
namespace NSAP_ODK.Entities.ItemSources
{
    class SexItemsSource : IItemsSource
    {
        private ItemCollection _items = new ItemCollection();
        public ItemCollection GetValues()
        {
            _items.Add("", "");
            _items.Add("j", "Juvenile");
            _items.Add("f", "Female");
            _items.Add("m", "Male");

            return _items;
        }
    }
}
