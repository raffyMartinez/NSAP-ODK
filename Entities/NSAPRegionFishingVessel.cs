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
    public class NSAPRegionFishingVesselEdit
    {
        public NSAPRegionFishingVesselEdit() { }

        public NSAPRegionFishingVessel NSAPRegionFishingVessel { get; set; }
        public NSAPRegionFishingVesselEdit(NSAPRegionFishingVessel regionVessel)
        {
            NSAPRegionFishingVessel = regionVessel;
            FishingVesselID = regionVessel.FishingVesselID;
            DateStart = regionVessel.DateStart;
            DateEnd = regionVessel.DateEnd;
            FisheriesSector = regionVessel.FishingVessel.FisheriesSector;
            RowID = regionVessel.RowID;
            NSAPRegion = regionVessel.NSAPRegion;

        }

        [ReadOnly(true)]
        public int RowID { get; set; }

        [ItemsSource(typeof(FishingVesselItemsSource))]
        public int FishingVesselID { get; set; }

        public FishingVessel FishingVessel { get; set; }

        [Editor(typeof(DateTimePickerEditor), typeof(DateTimePicker))]
        public DateTime DateStart { get; set; }

        public FisheriesSector FisheriesSector { get; set; }

        [Editor(typeof(DateTimePickerEditor), typeof(DateTimePicker))]
        public DateTime? DateEnd { get; set; }

        public NSAPRegion NSAPRegion { get; set; }
    }
    public class NSAPRegionFishingVessel
    {
        private FishingVessel _fishingVessel;


        public int RowID { get; set; }
        public string NSAPRegionCode { get; set; }
        public NSAPRegion NSAPRegion { get; set; }


        public int FishingVesselID { get; set; }
        public FishingVessel FishingVessel
        {
            get { return _fishingVessel; }
            set
            {
                _fishingVessel = value;
                FishingVesselID = _fishingVessel.ID;
            }
        }


        public DateTime DateStart { get; set; }



        public DateTime? DateEnd { get; set; }
    }
}
