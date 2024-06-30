using NPOI.OpenXmlFormats.Wordprocessing;
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
                if (FishSpeciesViewModel.FishGenusList == null || FishSpeciesViewModel.FishGenusList.Count == 0)
                {
                    NSAPEntities.FishSpeciesViewModel.GetAllGenus();
                }

                if (!string.IsNullOrEmpty(SearchFishGenus))
                {
                    var c = SearchFishGenus.ToLower().First();
                    if (c == SearchFishGenus.First())
                    {
                        foreach (string s in FishSpeciesViewModel.FishGenusList)
                        {
                            if (s.ToLower().Contains(SearchFishGenus.ToLower()))
                            {
                                genus_list.Add(s);
                            }
                        }
                    }
                    else
                    {
                        foreach (string s in FishSpeciesViewModel.FishGenusList)
                        {
                            if (s.Contains(SearchFishGenus))
                            {
                                genus_list.Add(s);
                            }
                        }
                    }
                }
                else
                {
                    foreach (string s in FishSpeciesViewModel.FishGenusList)
                    {
                        genus_list.Add(s);
                    }
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
            GeneraItemCollection = generas;
            return generas;
        }

        public ItemCollection GeneraItemCollection { get; internal set; }

        public static string TaxaCode { get; set; }

        public static string SearchFishGenus { get; set; }
    }
}
