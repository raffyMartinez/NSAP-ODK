using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities.ItemSources;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using System.ComponentModel;
namespace NSAP_ODK.Entities.Database
{
    public class VesselUnload_Gear_Spec_Edited
    {
        public VesselUnload_Gear_Spec_Edited()
        {

        }

        public VesselUnload_Gear_Spec_Edited(VesselUnload_Gear_Spec vu_gs)
        {
            if (vu_gs != null)
            {
                RowID = vu_gs.RowID;
                VesselUnload_Gear_Spec = vu_gs;
                EffortSpecID = vu_gs.EffortSpecID;
                EffortValueNumeric = vu_gs.EffortValueNumeric;
                EffortValueText = vu_gs.EffortValueText;
                GearUsedName = vu_gs.GearUsedName;
            }
        }
        [ReadOnly(true)]
        public int RowID { get; set; }
        public VesselUnload_Gear_Spec VesselUnload_Gear_Spec { get; set; }
        [ItemsSource(typeof(EffortSpecificationItemsSource))]
        public int EffortSpecID { get; set; }

        public double? EffortValueNumeric { get; set; }
        public string EffortValueText { get; set; }
        [ItemsSource(typeof(GearsInNSAPRegionItemsSource))]
        public string GearUsedName { get; set; }
    }
    public class VesselUnload_Gear_Spec
    {
        public string GearUsedName
        {
            get
            {
                if (string.IsNullOrEmpty(Parent.GearCode))
                {
                    return Parent.GearText;
                }
                else
                {
                    return NSAPEntities.GearViewModel.GetGear(Parent.GearCode).GearName;
                }
            }
        }
        public VesselUnload_FishingGear Parent { get; set; }
        private EffortSpecification _effortSpecification;

        public static int CurrentIDNumber { get; set; }
        public bool DelayedSave { get; set; }
        public int RowID { get; set; }
        public int VesselUnload_FishingGears_ID { get; set; }
        public int EffortSpecID { get; set; }

        public EffortSpecification EffortSpecification
        {
            set { _effortSpecification = value; }
            get
            {
                if (_effortSpecification == null)
                {
                    _effortSpecification = NSAPEntities.EffortSpecificationViewModel.GetEffortSpecification(EffortSpecID);
                }
                return _effortSpecification;
            }
        }
        public double? EffortValueNumeric { get; set; }
        public string EffortValueText { get; set; }
        //public string DisplayValue
        //{
        //    get
        //    {
        //        if (!string.IsNullOrEmpty(EffortValueText))
        //        {
        //            return EffortValueText;
        //        }
        //        else
        //        {
        //            return ((int)EffortValueNumeric).ToString();
        //        }
        //    }
        //}
        public string EffortValue
        {
            get
            {
                if (EffortValueNumeric == null)
                {
                    return EffortValueText;
                }
                else
                {
                    return ((double)EffortValueNumeric).ToString();
                }
            }
        }



    }
}
