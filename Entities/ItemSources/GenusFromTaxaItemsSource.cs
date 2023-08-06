using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NSAP_ODK.Entities.ItemSources
{
    class GenusFromTaxaItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            HashSet<string> genus_list = new HashSet<string>();
            ItemCollection generas = new ItemCollection();
            if (TaxaCode == "FIS")
            {
                foreach (var item in NSAPEntities.FishSpeciesViewModel.SpeciesCollection.OrderBy(t => t.GenericName))
                {
                    genus_list.Add(item.GenericName);
                }
            }
            else
            {
                foreach (var item in NSAPEntities.NotFishSpeciesViewModel.NotFishSpeciesCollection.OrderBy(t => t.Genus).Where(t => t.Taxa.Code == TaxaCode))
                {
                    if (!string.IsNullOrEmpty(item.Genus))
                    {
                        genus_list.Add(item.Genus);
                    }
                }
            }
            foreach (var item in genus_list)
            {
                generas.Add(item);
            }
            return generas;
        }

        public static string TaxaCode { get; set; }
    }
}
