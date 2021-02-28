using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
namespace NSAP_ODK.Entities.ItemSources
{
    class LandingSiteProvinceItemsSource:IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection provinces = new ItemCollection();
            foreach (var ls in NSAPEntities.LandingSiteViewModel.LandingSiteCollection
                .GroupBy(t=>t.Municipality.Province.ProvinceName)
                .Select(t=>t.First())
                .OrderBy(t=>t.Municipality.Province.ProvinceName))
            {
                var province = ls.Municipality.Province;
                provinces.Add(province.ProvinceID, province.ProvinceName);
                
            }
            return provinces;

        }
    }
}
