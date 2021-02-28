using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
namespace NSAP_ODK.Entities.ItemSources
{
    public class FishingGroundItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection fishingGrounds = new ItemCollection();
            foreach (var fg in NSAPEntities.FishingGroundViewModel.FishingGroundCollection.OrderBy(t => t.Name))
            {
                fishingGrounds.Add(fg.Code, fg.Name);
            }
            return fishingGrounds;

        }
    }
}