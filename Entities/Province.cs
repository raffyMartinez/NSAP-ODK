using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
namespace NSAP_ODK.Entities
{

    public class ProvinceEdited
    {
        public ProvinceEdited(Province p)
        {

        }
        public int ProvinceID { get; set; }
        public string RegionCode { get; set; }
        public string ProvinceName { get; set; }

        public NSAPRegion NSAPRegion { get; set; }
    }

    public class Province
    {
        private NSAPRegion _nsapRegion;
        public Province()
        {
        }

        [ReadOnly(true)]
        public int ProvinceID { get; set; }

       [ItemsSource(typeof(NSAPRegionItemsSource))]
        public string RegionCode { get; set; }
        public string ProvinceName { get; set; }


        public NSAPRegion NSAPRegion {
            get
            {
                if (_nsapRegion == null)
                {
                    _nsapRegion = NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(RegionCode);
                }
                return _nsapRegion;
            }
            set 
            {
                _nsapRegion = value;
                RegionCode = _nsapRegion.Code;
            }
        }
        public int MunicipalityCount
        {
            get {
                if (Municipalities == null)
                {
                    return 0;
                }
                else
                {
                    return Municipalities.Count;
                }
            }
        }

        public void SetMunicipalities()
        {
            if (ProvinceID > 0 && ProvinceName != null)
                Municipalities = new MunicipalityViewModel(this);
        }

        public MunicipalityViewModel Municipalities { get; set; }

        public override string ToString()
        {
            return ProvinceName;
        }
    }
}