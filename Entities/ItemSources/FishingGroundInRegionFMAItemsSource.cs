using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
namespace NSAP_ODK.Entities.ItemSources
{
    public class FishingGroundInRegionFMAItemsSource: IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection fishingGrounds = new ItemCollection();
            foreach (var fg in NSAPEntities.NSAPRegionFMA.FishingGrounds.OrderBy(t=>t.FishingGround.Name))
            {
                fishingGrounds.Add(fg.FishingGround.Code, fg.FishingGround.Name);
            }
            return fishingGrounds;

        }
    }
}
