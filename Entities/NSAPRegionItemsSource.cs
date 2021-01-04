using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NSAP_ODK.Entities
{
    class NSAPRegionItemsSource:IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection regions = new ItemCollection();
            foreach (var r in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection
                .OrderBy(t => t.Sequence))
            {
                regions.Add(r.Code, r.Name);
            }
            return regions;

        }
    }
}
