using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
namespace NSAP_ODK.Entities.ItemSources
{
    class FMAItemsSource:IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection fmas = new ItemCollection();
            foreach (var fma in NSAPEntities.FMAViewModel.FMACollection.OrderBy(t => t.Name))
            {
                fmas.Add(fma.FMAID, fma.Name);
            }
            return fmas;
        }
    }
}
