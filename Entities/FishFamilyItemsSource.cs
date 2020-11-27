using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
namespace NSAP_ODK.Entities
{
    class FishFamilyItemsSource:IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection families = new ItemCollection();
            var fishFamilies = NSAPEntities.FishSpeciesViewModel.SpeciesCollection.GroupBy(t => t.Family).Select(t => t.First()).OrderBy(t => t.Family);
            foreach (var sp in fishFamilies)
            {

                {
                    families.Add(sp.Family);
                }
            }
            return families;
        }
    }
}
