﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NSAP_ODK.Entities.ItemSources
{
    class FMAInRegionItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection fmas = new ItemCollection();
            foreach(var region_fma in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection.Where(t=>t.Code==NSAPEntities.NSAPRegion.Code).FirstOrDefault().FMAs)
            {
                
                fmas.Add(region_fma.FMAID, region_fma.FMA.Name);
            }
            return fmas;
        }
    }
}
