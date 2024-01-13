using System.ComponentModel;

namespace NSAP_ODK.Entities
{
    public class NSAPRegionEdit
    {
        private NSAPRegion _nsapRegion;

        [ReadOnly(true)]
        public string ID { get; set; }

        public NSAPRegionEdit(NSAPRegion nsapRegion)
        {
            _nsapRegion = nsapRegion;
            Refresh();
            IsNew = false;
        }

        public void Refresh()
        {
            
            NSAPRegion= NSAPEntities.NSAPRegionViewModel.GetNSAPRegionWithEntitiesRepository(_nsapRegion).NSAPRegion;
            Name = NSAPRegion.Name;
            ShortName = NSAPRegion.ShortName;
            FMAs = NSAPRegion.FMAs.Count;
            Gears = NSAPRegion.Gears.Count;
            Vessels = NSAPRegion.FishingVessels.Count;
            Enumerators = NSAPRegion.NSAPEnumerators.Count;
            IsTotalEnumerationOnly = NSAPRegion.IsTotalEnumerationOnly;
            IsRegularSamplingOnly = NSAPRegion.IsRegularSamplingOnly;
            ID = NSAPRegion.Code;
        }

        public NSAPRegionEdit()
        {
            IsNew = true;
        }

        public bool IsRegularSamplingOnly { get; set; }

        public bool IsTotalEnumerationOnly { get; set; }

        public bool IsNew { get; private set; }

        public NSAPRegion NSAPRegion { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }

        [ReadOnly(true)]
        public int FMAs { get; set; }

        [ReadOnly(true)]
        public int Gears { get; set; }

        [ReadOnly(true)]
        public int Vessels { get; set; }

        [ReadOnly(true)]
        public int Enumerators { get; set; }
    }
}