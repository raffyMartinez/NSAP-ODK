using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NSAP_ODK.Entities.ItemSources
{
    class TaxaItemSourceNotFish:IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection taxas = new ItemCollection();
            foreach (var taxa in NSAPEntities.TaxaViewModel.TaxaCollection
                .Where(t => t.Code != "FIS")
                .OrderBy(t => t.Name))
            {
                taxas.Add(taxa.Code, taxa.Name);
            }
            return taxas;
        }
    }
}
