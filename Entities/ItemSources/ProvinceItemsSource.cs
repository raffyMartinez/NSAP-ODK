using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NSAP_ODK.Entities.ItemSources
{
    public class ProvinceItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection provinces = new ItemCollection();
            foreach(var province in NSAPEntities.ProvinceViewModel.ProvinceCollection.OrderBy(t=>t.ProvinceName))
            {
                provinces.Add(province.ProvinceID, province.ProvinceName);
            }
            return provinces;
        }
    }
}
