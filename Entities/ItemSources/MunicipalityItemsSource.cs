using System.Linq;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NSAP_ODK.Entities.ItemSources
{
    public class MunicipalityItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection municipalities = new ItemCollection();
            if (NSAPEntities.ProvinceID != null)
            {
                var province = NSAPEntities.ProvinceViewModel.GetProvince((int)NSAPEntities.ProvinceID);
                foreach (var mun in province.Municipalities.MunicipalityCollection
                    .Where(t => t.Province.ProvinceID == NSAPEntities.ProvinceID).OrderBy(t => t.MunicipalityName))
                {
                    municipalities.Add(mun.MunicipalityID, mun.MunicipalityName);
                }
            }
            return municipalities;
        }
    }
}