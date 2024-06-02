using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using NSAP_ODK.Entities.ItemSources;
using System.Collections.Generic;
namespace NSAP_ODK.Entities
{
    public class GearWithExtras
    {
        public GearWithExtras(Gear gear)
        {
            Gear = gear;
        }

        public int UseCount { get; set; }

        public List<FishingGround> FishingGrounds = new List<FishingGround>();
        public Gear Gear { get; private set; }

        public List<FishSpecies> CatchCompositionFish { get; set; }
    }


    public class GearFlattened
    {
        public string GearName { get; set; }
        public string Code { get; set; }
        public string GenericCode { get; set; }
        public bool IsGeneric { get; set; }
    }
    public class GearEdit
    {
        public bool IsUsedInLargeCommercial { get; set; }
        public Gear Save(bool isNew)
        {
            IsNew = isNew;
            if (Gear == null)
            {
                Gear = new Gear();
            }
            Gear.GearName = GearName;
            Gear.Code = Code;
            Gear.IsGenericGear = IsGeneric;
            Gear.GearIsNotUsed = GearIsNotUsed;
            Gear.IsUsedInLargeCommercial = IsUsedInLargeCommercial;

            if (IsNew)
            {
                if (BaseGear != null)
                {
                    Gear.BaseGear = NSAPEntities.GearViewModel.GetGear(BaseGear);
                }
                else if (IsGeneric)
                {
                    Gear.BaseGear = Gear;
                }
            }
            else
            {
                if (BaseGear != null && IsGeneric)
                {
                    Gear.BaseGear = NSAPEntities.GearViewModel.GetGear(BaseGear);
                }
                else if (BaseGear != null)
                {
                    Gear.BaseGear = NSAPEntities.GearViewModel.GetGear(BaseGear);
                }
                else if (BaseGear == null && !IsGeneric)
                {
                    Gear.BaseGear = null;
                }
                else
                {
                    Gear.BaseGear = Gear;
                }
                if (Gear.GearEffortSpecificationViewModel == null)
                {
                    Gear.GearEffortSpecificationViewModel = new GearEffortSpecificationViewModel(Gear);
                }
            }

            return Gear;
        }

        public bool GearIsNotUsed { get; set; }

        public Gear Gear { get; set; }
        public string GearName { get; set; }

        //[ReadOnly(true)]
        public string Code { get; set; }

        [ItemsSource(typeof(GenericGearItemsSource))]
        public string BaseGear { get; set; }

        public bool IsGeneric { get; set; }

        public bool IsNew { get; set; }

        [ReadOnly(true)]
        public int BaseGearEffortSpecifiers
        {
            get
            {
                return Gear.BaseGear.GearEffortSpecificationViewModel.Count;
            }
        }

        [ReadOnly(true)]
        public int EffortSpecifiers
        {
            get
            {
                if (Gear.GearEffortSpecificationViewModel == null)
                {
                    return 0;
                }
                else
                {
                    return Gear.GearEffortSpecificationViewModel.Count;
                }
            }
        }

        public GearEdit()
        {
            IsNew = true;
        }

        public GearEdit(Gear gear)
        {
            Gear = gear;
            GearName = gear.GearName;
            Code = gear.Code;
            BaseGear = gear.BaseGear.Code;
            IsGeneric = gear.IsGenericGear;
            IsNew = false;
            GearIsNotUsed = gear.GearIsNotUsed;
            IsUsedInLargeCommercial = gear.IsUsedInLargeCommercial;

            if (gear.GearEffortSpecificationViewModel == null)
            {
                gear.GearEffortSpecificationViewModel = new GearEffortSpecificationViewModel(gear);
                NSAPEntities.GearViewModel.AddBaseGearSpecsToGear(gear);
            }
        }
    }

    public class Gear : INotifyPropertyChanged
    {
        private Gear _baseGear;
        public event PropertyChangedEventHandler PropertyChanged;
        private string _code;

        public override bool Equals(object obj)
        {
            if (obj!=null && obj is Gear)
            {
                return ((Gear)obj).Code == this.Code;
            }
            else
            {
                return false;
            }

        }
        public string GearName { get; set; }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public Gear()
        {
            //EffortSpecifications = new List<GearEffortSpecification>();
        }

        public bool GearIsNotUsed { get; set; }

        public Gear(string gearName, string code, string gearID)
        {
            GearName = gearName;
            Code = code;
            //EffortSpecifications = new List<GearEffortSpecification>();
        }

        //public List<GearEffortSpecification> EffortSpecifications { get; set; }
        public GearEffortSpecificationViewModel GearEffortSpecificationViewModel { get; set; }

        public void GetEffortSpecificationsForGear()
        {
            GearEffortSpecificationViewModel = new GearEffortSpecificationViewModel(this);
        }

        public bool IsGenericGear { get; set; }
        public string CodeOfBaseGear { get; set; }

        public Gear BaseGear
        {
            get { return _baseGear; }
            set
            {
                if (value == null)
                {
                    _baseGear = null;
                }
                else if (_baseGear == null)
                {
                    _baseGear = value;
                }
                else if (_baseGear.Code != value.Code)
                {
                    _baseGear = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public override string ToString()
        {
            return GearName;
        }

        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
            }
        }

        public bool IsUsedInLargeCommercial { get; set; }
    }
}