using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
namespace NSAP_ODK.Entities.ItemSources
{
    public class MaturityItemsSource : IItemsSource
    {
        private ItemCollection _items = new ItemCollection();
        public ItemCollection GetValues()
        {
            _items.Add("", "");
            _items.Add("pr", "Premature");
            _items.Add("im", "Immature(FI)");
            _items.Add("de", "Developing(FII)");
            _items.Add("ri", "Ripening(FIII)");
            _items.Add("spw", "Spawning(FIV)");
            _items.Add("sp", "Spent(FV)");

            return _items;
        }

        public string MaturityStageFromCode(string code)
        {

            foreach (var item in _items)
            {
                if ((string)item.Value == code)
                {
                    return item.DisplayName;
                }
            }
            return "";
        }
    }
}
