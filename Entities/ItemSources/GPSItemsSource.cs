using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
namespace NSAP_ODK.Entities.ItemSources
{
    class GPSItemsSource: IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection gpsList = new ItemCollection();
            foreach (var gps in NSAPEntities.GPSViewModel.GPSCollection.OrderBy(t => t.AssignedName))
            {
                gpsList.Add(gps.Code, gps.AssignedName);
            }
            return gpsList;

        }
    }
}
