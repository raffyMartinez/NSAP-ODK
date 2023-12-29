using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using NSAP_ODK.Entities.ItemSources;
using NSAP_ODK.Entities.Database;
using System.Security.Cryptography.X509Certificates;

namespace NSAP_ODK.Entities
{
    public class LandingSiteEdit
    {
        public bool IsNew { get; private set; }

        private int _province;

        public LandingSiteEdit(LandingSite ls)
        {
            LandingSite = ls;
            Province = ls.Municipality.Province.ProvinceID;
            Municipality = ls.Municipality.MunicipalityID;
            Name = ls.LandingSiteName;
            ID = ls.LandingSiteID;
            IsNew = false;
            Latitude = ls.Latitude;
            Longitude = ls.Longitude;
            Barangay = ls.Barangay;
            CountFishingVessels = ls.LandingSite_FishingVesselViewModel.Count;
            //CountFishingGrounds = ls.LandingSiteFishingGroundViewModel.Count;
            LandingSiteTypeOfSampling = ls.LandingSiteTypeOfSampling;

            //CountFishingVessels = ls.CountFishingVessels;
        }
        public string Barangay { get; set; }
        public LandingSiteEdit()
        {
            IsNew = true;
        }

        [ItemsSource(typeof(TypeOfSamplingItemsSource))]
        public LandingSiteTypeOfSampling LandingSiteTypeOfSampling { get; set; }
        public int? CountFishingVessels { get;  }

        //public int? CountFishingGrounds { get; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        [ReadOnly(true)]
        public int ID { get; set; }

        public string Name { get; set; }

        [ItemsSource(typeof(MunicipalityItemsSource))]
        public int Municipality { get; set; }

        public LandingSite LandingSite { get; set; }

        [ItemsSource(typeof(ProvinceItemsSource))]
        public int Province
        {
            get { return _province; }
            set
            {
                _province = value;
                NSAPEntities.ProvinceID = _province;
            }
        }
    }
}