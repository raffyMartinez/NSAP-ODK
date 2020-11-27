using System.ComponentModel;

namespace NSAP_ODK.Entities
{
    public class Province
    {
        public Province()
        {
        }

        [ReadOnly(true)]
        public int ProvinceID { get; set; }

        public string ProvinceName { get; set; }

        public int MunicipalityCount
        {
            get { return Municipalities.Count; }
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