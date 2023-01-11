using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
namespace NSAP_ODK.Entities.ItemSources
{
    class RegionEnumeratorItemsSource : IItemsSource
    {

        public ItemCollection GetValues()
        {
            ItemCollection enumerators = new ItemCollection();
            if (NSAPEntities.NSAPRegion == null)
            {
                return null;
            }
            else
            {
                foreach (var en in NSAPEntities.NSAPRegion.NSAPEnumerators
                    .OrderBy(t => t.Enumerator.Name))
                {
                    enumerators.Add(en.Enumerator.ID, en.Enumerator.Name);
                }
                return enumerators;
            }

        }
    }
}
