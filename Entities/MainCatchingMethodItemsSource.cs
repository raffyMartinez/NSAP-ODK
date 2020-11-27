using System.Linq;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NSAP_ODK.Entities
{
    internal class MainCatchingMethodItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection catchingMethods = new ItemCollection();
            var methods = NSAPEntities.FishSpeciesViewModel.SpeciesCollection.ToList()
                .Where(t => t.MainCatchingMethod != null)
                .Where(t => t.MainCatchingMethod.Trim().Length > 0)
                .GroupBy(t => t.MainCatchingMethod)
                .Select(x => x.First())
                .OrderBy(t => t.MainCatchingMethod);

            catchingMethods.Add("");
            foreach (var m in methods)
            {
                catchingMethods.Add(m.MainCatchingMethod);
            }

            return catchingMethods;
        }
    }
}