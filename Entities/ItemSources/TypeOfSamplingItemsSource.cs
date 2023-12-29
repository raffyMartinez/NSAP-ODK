using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using NSAP_ODK.Entities.Database;
namespace NSAP_ODK.Entities.ItemSources
{
    public class TypeOfSamplingItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection typesList = new ItemCollection();
            foreach (var value in Enum.GetValues(typeof(LandingSiteTypeOfSampling)))
            {
                //typesList.Add((int)value, LandingSiteSampling.LandingSiteSamplingTypeToString((LandingSiteTypeOfSampling)((int)value)));
                typesList.Add(value, LandingSiteSampling.LandingSiteSamplingTypeToString((LandingSiteTypeOfSampling)((int)value)));
            }
            return typesList;

        }
    }
}
