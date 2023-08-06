using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
namespace NSAP_ODK.Entities.ItemSources
{
    class SpeciesFromGenusItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            HashSet<string> species_list = new HashSet<string>();
            ItemCollection specieses = new ItemCollection();

            if (GenusFromTaxaItemsSource.TaxaCode == "FIS")
            {
                foreach (var item in NSAPEntities.FishSpeciesViewModel.SpeciesCollection.Where(t => t.GenericName == Genus).OrderBy(t => t.SpecificName))
                {
                    species_list.Add(item.SpecificName);
                }
            }
            else
            {
                foreach (var item in NSAPEntities.NotFishSpeciesViewModel.NotFishSpeciesCollection.Where(t => t.Genus == Genus).OrderBy(t => t.Species))
                {
                    species_list.Add(item.Species);
                }
            }

            foreach (var item in species_list)
            {
                specieses.Add(item);
            }
            return specieses;
        }
        public static string Genus { get; set; }

    }
}
