using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NSAP_ODK.Entities
{
    public class MunicipalityEdit
    {
        public MunicipalityEdit(Municipality municipality)
        {
            Municipality = municipality;
            MunicipalityID = municipality.MunicipalityID;
            MunicipalityName = municipality.MunicipalityName;
            ProvinceID = municipality.Province.ProvinceID;
            Longitude = municipality.Longitude;
            Latitude = municipality.Latitude;
            IsCoastal = municipality.IsCoastal;
        }

        public MunicipalityEdit()
        {
        }

        public Municipality Municipality { get; set; }
        public int MunicipalityID { get; set; }
        public string MunicipalityName { get; set; }

        [ItemsSource(typeof(ProvinceItemsSource))]
        public int ProvinceID { get; set; }

        public double? Longitude { get; set; }
        public double? Latitude { get; set; }

        public bool IsCoastal { get; set; }
    }

    public class Municipality
    {
        public int MunicipalityID { get; set; }
        public string MunicipalityName { get; set; }
        public Province Province { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }

        public bool IsCoastal { get; set; }

        public override string ToString()
        {
            
           return $"{MunicipalityName}, {Province.ProvinceName}";
            
        }
    }
}