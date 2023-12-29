using DocumentFormat.OpenXml.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using NSAP_ODK.Entities.ItemSources;
using Xceed.Wpf.Toolkit;
using System.ComponentModel;

namespace NSAP_ODK.Entities.Database
{
    public class LandingSiteFishingGround
    {
        private FishingGround _fishingGround;
        private LandingSite _landingSite;
        private string _fishingGroundCode;
        private int _LandingSiteID;

        public FishingGround FishingGround
        {
            get { return _fishingGround; }
            set
            {
                _fishingGround = value;
                _fishingGroundCode = _fishingGround.Code;
            }
        }

        [ItemsSource(typeof(FishingGroundItemsSource))]
        public string FishingGroundCode
        {
            get { return _fishingGroundCode; }
            set
            {
                _fishingGroundCode = value;
                _fishingGround = NSAPEntities.FishingGroundViewModel.GetFishingGround(_fishingGroundCode);
            }
        }
        public int RowID { get; set; }

        public int LandingSiteID
        {
            get { return _LandingSiteID; }
            set
            {
                _LandingSiteID = value;
                if (NSAPEntities.LandingSiteViewModel != null)
                {
                    _landingSite = NSAPEntities.LandingSiteViewModel.GetLandingSite(_LandingSiteID);
                }
            }
        }
        public LandingSite LandingSite { get { return _landingSite; } set { _landingSite = value; } }

        [Editor(typeof(DateTimePickerEditor), typeof(DateTimePicker))]
        public DateTime DateAdded { get; set; }
        [Editor(typeof(DateTimePickerEditor), typeof(DateTimePicker))]
        public DateTime? DateRemoved { get; set; }
    }
}
