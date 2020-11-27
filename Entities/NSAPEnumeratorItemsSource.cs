using System.Linq;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NSAP_ODK.Entities
{
    public class NSAPEnumeratorItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection enumerators = new ItemCollection();
            foreach (var en in NSAPEntities.NSAPEnumeratorViewModel.NSAPEnumeratorCollection.OrderBy(t => t.Name))
            {
                enumerators.Add(en.ID, en.Name);
            }
            return enumerators;
        }
    }
}