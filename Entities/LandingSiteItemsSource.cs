using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
namespace NSAP_ODK.Entities
{
    public class LandingSiteItemsSource:IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection landingSites = new ItemCollection();
            foreach (var ls in NSAPEntities.LandingSiteViewModel.LandingSiteCollection
                .Where(t=>t.Municipality.MunicipalityID==NSAPEntities.MunicipalityID)
                .OrderBy(t => t.LandingSiteName))
            {
                landingSites.Add(ls.LandingSiteID, ls.LandingSiteName);
            }
            return landingSites;

        }
    }
}
