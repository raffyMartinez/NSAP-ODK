using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NSAP_ODK.Entities
{

 
        class GearItemsSource : IItemsSource
        {
        private ItemCollection _gears = new ItemCollection();
            public ItemCollection GetValues()
            {
                
                foreach (var gear in NSAPEntities.GearViewModel.GearCollection.OrderBy(t => t.GearName))
                {
                    _gears.Add(gear.Code, gear.GearName);
                }
                return _gears;
            }

            public void AddItem(string code, string name)
            {
                _gears.Add(code, name);
            }

        public void AddItem(KeyValuePair<string,string>item)
        {
            _gears.Add(item.Key, item.Value);
        }

    }
    
}
