using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NSAP_ODK.Entities
{
    public class GearFlattened
    {
        public string GearName { get; set; }
        public string Code { get; set; }
        public string GenericCode { get; set; }
        public bool IsGeneric { get; set; }
    }
    public class GearEdit
    {
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

            if (IsNew)
            {
                if (BaseGear != null)
                {
                    Gear.BaseGear = NSAPEntities.GearViewModel.GetGear(BaseGear);
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
                else if(BaseGear==null && !IsGeneric)
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

        public Gear Gear { get; set; }
        public string GearName { get; set; }

        //[ReadOnly(true)]
        public string Code { get; set; }

        [ItemsSource(typeof(GenericGearItemsSource))]
        public string BaseGear { get; set; }

        public bool IsGeneric { get; set; }

        public bool IsNew { get; set; }

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
        }
    }

    public class Gear : INotifyPropertyChanged
    {
        private Gear _baseGear;
        public event PropertyChangedEventHandler PropertyChanged;
        private string _code;
        public string GearName { get; set; }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public Gear()
        {
            //EffortSpecifications = new List<GearEffortSpecification>();
        }

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
                if(value==null)
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
    }
}