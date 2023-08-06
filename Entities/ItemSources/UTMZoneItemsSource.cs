using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using NSAP_ODK.Entities.Database;
namespace NSAP_ODK.Entities.ItemSources
{
    class UTMZoneItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection items= new ItemCollection();
            items.Add("50N");
            items.Add("51N");
            return items;
        }
    }
}
