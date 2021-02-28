using System.Collections.Generic;

using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using NSAP_ODK.Entities.ItemSources;

namespace NSAP_ODK.Entities
{
    public class NSAPRegionFMA
    {
        private FMA _fma;

        public NSAPRegionFMA()
        {
            FishingGrounds = new List<NSAPRegionFMAFishingGround>();
        }

        [ReadOnly(true)]
        public int RowID { get; set; }

        public NSAPRegion NSAPRegion { get; set; }

        public FMA FMA
        {
            get { return _fma; }
            set
            {
                _fma = value;
                FMAID = _fma.FMAID;
            }
        }

        [ReadOnly(true)]
        [ItemsSource(typeof(FMAItemsSource))]
        public int FMAID { get; set; }

        public int FishingGroundCount
        {
            get { return FishingGrounds.Count; }
        }

        public string FishingGroundList
        {
            get
            {
                string items = "";
                foreach (var fg in FishingGrounds)
                {
                    items += $"{fg.FishingGround.Name}, ";
                }
                return items.Trim(new char[] { ' ', ',' });
            }
        }

        public List<NSAPRegionFMAFishingGround> FishingGrounds { get; set; }

        public override string ToString()
        {
            return $"{NSAPRegion.ShortName}-{FMA}";
        }
    }
}