using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
namespace NSAP_ODK.Entities
{
    public class FishingVesselItemsSource:IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection vessels = new ItemCollection();
            foreach (var vessel in NSAPEntities.FishingVesselViewModel.FishingVesselCollection
                .Where(t=>t.FisheriesSector==NSAPEntities.FisheriesSector)
                .OrderBy(t => t.Name)
                .ThenBy(t=>t.NameOfOwner))
            {
                vessels.Add(vessel.ID , vessel.ToString());
            }
            return vessels;
        }
    }
}
