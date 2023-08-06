using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using NSAP_ODK.Entities.Database;

namespace NSAP_ODK.Entities.ItemSources
{
    public class EffortSpecificationItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection effortSpecs = new ItemCollection();
            if (VesselUnload_Gear_Spec != null)
            {
                Gear gear = VesselUnload_Gear_Spec.Parent.Gear;


                if (gear != null)
                {
                    foreach (var baseSpec in gear.BaseGear.GearEffortSpecificationViewModel.GearEffortSpecificationCollection)
                    {
                        effortSpecs.Add(baseSpec.EffortSpecification.ID, baseSpec.EffortSpecification.Name);
                    }
                    if (gear.GearEffortSpecificationViewModel.Count > 0)
                    {
                        foreach (var spec in gear.GearEffortSpecificationViewModel.GearEffortSpecificationCollection)
                        {
                            effortSpecs.Add(spec.EffortSpecification.ID, spec.EffortSpecification.Name);
                        }
                    }
                }
                else
                {
                    var list = NSAPEntities.EffortSpecificationViewModel.GetBaseGearEffortSpecification();
                    foreach (var item in list)
                    {
                        effortSpecs.Add(item.ID, item.Name);
                    }
                }
            }
            else
            {
                foreach (var effortSpec in NSAPEntities.EffortSpecificationViewModel.EffortSpecCollection.OrderBy(t => t.Name))
                {
                    effortSpecs.Add(effortSpec.ID, effortSpec.Name);
                }
            }
            return effortSpecs;

        }

        public static VesselUnload_Gear_Spec VesselUnload_Gear_Spec { get; set; }
    }
}
