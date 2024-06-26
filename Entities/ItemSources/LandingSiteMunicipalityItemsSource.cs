﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
namespace NSAP_ODK.Entities.ItemSources
{
    class LandingSiteMunicipalityItemsSource:IItemsSource
    {
        public ItemCollection GetValues()
        {
            List<int> municipalityIDs = new List<int>();
            ItemCollection municipalities = new ItemCollection();
            if (Province == null)
            {
                foreach (var ls in NSAPEntities.LandingSiteViewModel.LandingSiteCollection.Where(t => t.Municipality.Province.ProvinceID == NSAPEntities.ProvinceID))
                {
                    var municipality = ls.Municipality;
                    if (!municipalityIDs.Contains(municipality.MunicipalityID))
                    {
                        municipalities.Add(municipality.MunicipalityID, municipality.MunicipalityName);
                        municipalityIDs.Add(municipality.MunicipalityID);
                    }
                }
            }
            else
            {
                foreach(var mun in Province.Municipalities.MunicipalityCollection)
                {
                    municipalities.Add(mun.MunicipalityID, mun.MunicipalityName);
                }
            }    
            return municipalities;

        }
        public static Province Province { get; set; }
    }
}
