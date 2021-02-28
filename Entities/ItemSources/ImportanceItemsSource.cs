using System.Linq;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NSAP_ODK.Entities.ItemSources
{
    internal class ImportanceItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection importances = new ItemCollection();
            importances.Add("");
            var importanceItems = NSAPEntities.FishSpeciesViewModel.SpeciesCollection.ToList()
                .Where(t => t.Importance != null)
                .Where(t => t.Importance.Trim().Length > 0)
                .GroupBy(t => t.Importance)
                .Select(x => x.First())
                .OrderBy(t => t.Importance);
            foreach (var imp in importanceItems)
            {
                importances.Add(imp.Importance);
            }

            return importances;
        }
    }
}