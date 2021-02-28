using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
namespace NSAP_ODK.Entities.ItemSources
{
    class SizeTypeItemsSource:IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection sizeTypes = new ItemCollection();
            foreach (var sizeType in NSAPEntities.SizeTypeViewModel.SizeTypeCollection.OrderBy(t => t.Name))
            {
                sizeTypes.Add(sizeType.Code, sizeType.Name);
            }
            return sizeTypes;
        }
    }
}
