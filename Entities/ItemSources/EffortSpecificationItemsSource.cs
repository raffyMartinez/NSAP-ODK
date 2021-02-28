using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NSAP_ODK.Entities.ItemSources
{
    public class EffortSpecificationItemsSource:IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection effortSpecs = new ItemCollection();
            foreach (var effortSpec in NSAPEntities.EffortSpecificationViewModel.EffortSpecCollection .OrderBy(t => t.Name))
            {
                effortSpecs.Add(effortSpec.ID, effortSpec.Name);
            }
            return effortSpecs;

        }
    }
}
