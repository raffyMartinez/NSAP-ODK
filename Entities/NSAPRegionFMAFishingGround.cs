using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NSAP_ODK.Entities
{
    public class NSAPRegionFMAFishingGround
    {
        private FishingGround _fishingGround;

        public NSAPRegionFMAFishingGround()
        {
            LandingSites = new List<NSAPRegionFMAFishingGroundLandingSite>();
        }

        [ReadOnly(true)]
        public int RowID { get; set; }

        public NSAPRegionFMA RegionFMA { get; set; }

        public FishingGround FishingGround
        {
            get { return _fishingGround; }
            set
            {
                _fishingGround = value;
                FishingGroundCode = _fishingGround.Code;
            }
        }

        [ItemsSource(typeof(FishingGroundItemsSource))]
        public string FishingGroundCode { get; set; }

        [Editor(typeof(DateTimePickerEditor), typeof(DateTimePicker))]
        public DateTime DateStart { get; set; }

        public string LandingSiteList
        {
            get
            {
                string list = "";
                foreach (var ls in LandingSites)
                {
                    list += $"{ls.LandingSite.LandingSiteName}, ";
                }
                return list.Trim(new char[] { ' ', ',' });
            }
        }

        [Editor(typeof(DateTimePickerEditor), typeof(DateTimePicker))]
        public DateTime? DateEnd { get; set; }

        public int LandingSiteCount { get { return LandingSites.Count; } }
        public List<NSAPRegionFMAFishingGroundLandingSite> LandingSites { get; set; }

        public override string ToString()
        {
            return $"{RegionFMA.NSAPRegion.ShortName}-{RegionFMA.FMA.Name}-{FishingGround.Name}";
        }
    }
}