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
            NSAPRegion = _nsapRegion;
            Name = _nsapRegion.Name;
            ShortName = _nsapRegion.ShortName;
            FMAs = _nsapRegion.FMAs.Count;
            Gears = _nsapRegion.Gears.Count;
            Vessels = _nsapRegion.FishingVessels.Count;
            Enumerators = _nsapRegion.NSAPEnumerators.Count;
            ID = NSAPRegion.Code;
        }

        public NSAPRegionEdit()
        {
            IsNew = true;
        }

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