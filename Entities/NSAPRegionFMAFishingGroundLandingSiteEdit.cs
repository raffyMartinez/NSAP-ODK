using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using System.ComponentModel;
using Xceed.Wpf.Toolkit;
using NSAP_ODK.Entities.ItemSources;
namespace NSAP_ODK.Entities
{
    public class NSAPRegionFMAFishingGroundLandingSiteEdit
    {
        public bool IsNew { get; private set; }

        private int _province;
        private int _municipality;
        public LandingSite LandingSite { get; set; }

        public NSAPRegionFMAFishingGroundLandingSiteEdit()
        {
            IsNew = true;
        }
        public NSAPRegionFMAFishingGroundLandingSiteEdit(NSAPRegionFMAFishingGroundLandingSite landingSite)
        {
            LandingSite = landingSite.LandingSite;
            Province = landingSite.LandingSite.Municipality.Province.ProvinceID;
            Municipality = landingSite.LandingSite.Municipality.MunicipalityID;
            LandingSiteID = landingSite.LandingSite.LandingSiteID;
            DateStart = landingSite.DateStart;
            DateEnd = landingSite.DateEnd;
            RowID = landingSite.RowID;
            IsNew = false;
            Barangay = landingSite.LandingSite.Barangay;
        }

        [ReadOnly(true)]
        public int RowID { get; set; }

        public NSAPRegionFMAFishingGroundLandingSite NSAPRegionFMAFishingGroundLandingSite
        {
            get
            {
                return new NSAPRegionFMAFishingGroundLandingSite
                {
                    RowID = this.RowID,
                    DateEnd = this.DateEnd,
                    DateStart = this.DateStart,
                    LandingSite = this.LandingSite,
                    NSAPRegionFMAFishingGround = this.FMAFishingGround
                };
            }
        }
        public NSAPRegionFMAFishingGround FMAFishingGround { get; set; }

        [Editor(typeof(DateTimePickerEditor), typeof(DateTimePicker))]
        public DateTime DateStart { get; set; }


        [Editor(typeof(DateTimePickerEditor), typeof(DateTimePicker))]
        public DateTime? DateEnd { get; set; }

        [ItemsSource(typeof(LandingSiteItemsSource))]
        public int LandingSiteID { get; set; }

        [ItemsSource(typeof(LandingSiteProvinceItemsSource))]
        public int Province
        {
            get { return _province; }
            set
            {
                _province = value;
                NSAPEntities.ProvinceID = _province;
            }
        }

        [ItemsSource(typeof(LandingSiteMunicipalityItemsSource))]
        public int Municipality
        {
            get { return _municipality; }
            set
            {
                _municipality = value;
                NSAPEntities.MunicipalityID = _municipality;
            }
        }

        public string Barangay { get; set; }

    }
}
