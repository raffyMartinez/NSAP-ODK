using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities.ItemSources;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using System.ComponentModel;
using System.Drawing.Text;
using System.Security.Policy;
using System.Runtime.InteropServices.WindowsRuntime;
using Xceed.Wpf.AvalonDock.Controls;
namespace NSAP_ODK.Entities.Database
{
    public class VesselUnload_Gear_Spec_Edited
    {
        private string _gearCode;
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
                GearCode = vu_gs.Gear!=null?vu_gs.Gear.Code: vu_gs.Parent.GearCode;
                GearText = vu_gs.Gear!=null?"": vu_gs.Parent.GearText;
            }
            if (vu_gs.EffortValueNumeric != null)
            {
                SpecValue = vu_gs.EffortValueNumeric.ToString();
            }
            else if (!string.IsNullOrEmpty(vu_gs.EffortValueText))
            {
                SpecValue = vu_gs.EffortValueText;
            }
        }
        public Gear Gear { get; set; }
        [ItemsSource(typeof(GearItemsSource))]
        public string GearCode
        {
            get { return _gearCode; }
            set
            {
                _gearCode = value;
                Gear = NSAPEntities.GearViewModel.GetGear(_gearCode);
            }
        }
        public string GearText { get; set; }
        [ReadOnly(true)]
        public int RowID { get; set; }
        public VesselUnload_Gear_Spec VesselUnload_Gear_Spec { get; set; }
        [ItemsSource(typeof(EffortSpecificationItemsSource))]
        public int? EffortSpecID { get; set; }
        public string SpecValue { get; set; }
        public double? EffortValueNumeric { get; set; }
        public string EffortValueText { get; set; }

        public VesselUnload ParentVesselUnload { get; set; }
        //[ItemsSource(typeof(GearsInNSAPRegionItemsSource))]
        //public string GearUsedName { get; set; }
        public string GearUsedName
        {
            get
            {
                if(ParentVesselUnload!=null)
                {
                    return ParentVesselUnload.Parent.GearUsedName;
                }
                else if (string.IsNullOrEmpty(GearCode))
                {
                    return GearText;
                }
                else
                {
                    return NSAPEntities.GearViewModel.GetGear(GearCode).GearName;
                }
            }
        }

    }
    public class VesselUnload_Gear_Spec
    {
        private string _gearCode;

        public int? ParentVesselUnloadID { get; set; }
        public VesselUnload_Gear_Spec() { }

        public Gear Gear { get; set; }
        public override int GetHashCode()
        {
            return RowID.GetHashCode();
        }
        public string GearCode
        {
            get
            {
                if(Gear!=null)
                {
                    return Gear.Code;
                }
                else if (string.IsNullOrEmpty(_gearCode))
                {
                    if (Parent.Gear == null)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        _gearCode = Parent.Gear.Code;
                        return _gearCode;
                    }
                }
                else
                {
                    return _gearCode;
                }

            }
            set { _gearCode = value; }
        }

        public string GearUsedName
        {
            get
            {
                if (Gear!= null)
                {
                    return Gear.GearName;
                }
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


        public override bool Equals(object obj)
        {
            VesselUnload_Gear_Spec vugs = obj as VesselUnload_Gear_Spec;
            if (vugs == null)
            {
                return false;
            }

            if (this.Parent!=null &&  this.Parent.Gear == null)
            {
                return false;
            }


            return
                this.GearCode == vugs.GearCode &&
                this.EffortSpecID == vugs.EffortSpecID;
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
