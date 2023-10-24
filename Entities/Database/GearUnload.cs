using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class GearUnloadFlattened
    {
        private LandingSiteSamplingFlattened _parent;
        public GearUnloadFlattened(GearUnload gu)
        {
            _parent = NSAPEntities.LandingSiteSamplingViewModel.GetFlattenedItem(gu.Parent.PK);
            Boats = gu.Boats;
            Catch = gu.Catch;
            NSAPRegion = _parent.NSAPRegion;
            FMA = _parent.FMA;
            FishingGround = _parent.FishingGround;
            LandingSite = _parent.LandingSite;
            SamplingDate = _parent.SamplingDate;
            Gear = gu.GearUsedName;
            ID = gu.PK;
            ParentID = _parent.ID;
        }
        public int ID { get; private set; }

        public int ParentID { get; private set; }
        public string NSAPRegion { get; private set; }
        public string FMA { get; private set; }
        public string FishingGround { get; private set; }

        public string LandingSite { get; private set; }
        public DateTime SamplingDate { get; private set; }

        public string Gear { get; private set; }
        public int? Boats { get; private set; }
        public double? Catch { get; private set; }

    }
    public class GearUnload:IDisposable
    {
        private Gear _gear;
        private LandingSiteSampling _parent;
        public bool DelayedSave { get; set; }
        public List<VesselUnload> AttachedVesselUnloads { get; set; }
        public string Remarks { get; set; }
        public int PK { get; set; }
        public int LandingSiteSamplingID { get; set; }
        public string GearID { get; set; }
        public int? Boats { get; set; }
        public double? Catch { get; set; }

        public int? SpeciesWithTWSpCount { get; set; }
        public VesselUnloadViewModel VesselUnloadViewModel { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                AttachedVesselUnloads.Clear();
                ListVesselUnload.Clear();
                ListVesselUnload = null;
                AttachedVesselUnloads = null;
                VesselUnloadViewModel = null;
                Parent = null;
            }
            // free native resources if there are any.
        }
        public int NumberOfSampledLandingsEx { get; set; }
        public string SectorCode { get; set; }

        public string Sector
        {
            get
            {
                switch (SectorCode)
                {
                    case "c":
                        return "Commercial";
                    case "m":
                        return "Municipal";
                    case "cm":
                        return "Mixed";
                    default:
                        return "";
                }
                //return SectorCode == "c" ? "Commercial" :
                //       SectorCode == "m" ? "Municipal" : "";
            }
        }

        public int? NumberOfMunicipalLandings { get; set; }
        public int? NumberOfCommercialLandings { get; set; }
        public double? WeightOfCommercialLandings { get; set; }
        public double? WeightOfMunicipalLandings { get; set; }
        public int? Sequence { get; set; }
        public TotalWtSpViewModel TotalWtSpViewModel { get; set; }
        public int NumberOfSampledLandings
        {
            get
            {
                return ListVesselUnload.Count;
            }
        }
        public List<VesselUnload> ListVesselUnload
        {
            get
            {
                if (VesselUnloadViewModel == null)
                {
                    VesselUnloadViewModel = new VesselUnloadViewModel(this);
                }
                return VesselUnloadViewModel.VesselUnloadCollection.ToList();

            }
            set
            {
                VesselUnloadViewModel.IgnoreCollectionChange = true;
                VesselUnloadViewModel.VesselUnloadCollection.Clear();
                foreach (var item in value)
                {
                    VesselUnloadViewModel.VesselUnloadCollection.Add(item);
                }
                VesselUnloadViewModel.IgnoreCollectionChange = false;
            }
        }

        public string GearUsedText { get; set; }
        public Gear Gear
        {
            set { _gear = value; }
            get
            {
                if (_gear == null)
                {
                    _gear = NSAPEntities.GearViewModel.GetGear(GearID);
                }
                return _gear;
            }
        }

        public string GearAndSector
        {
            get
            {
                if (string.IsNullOrEmpty(GearID))
                {
                    return $"{GearUsedText}_{SectorCode}";
                }
                else
                {
                    return $"{NSAPEntities.GearViewModel.GetGear(GearID).GearName}_{SectorCode}";
                }
            }
        }
        public string GearUsedName
        {
            get
            {
                if (string.IsNullOrEmpty(GearID))
                {
                    return GearUsedText;
                }
                else
                {
                    var gear = NSAPEntities.GearViewModel.GetGear(GearID);
                    if (gear == null)
                    {
                        return "";
                    }
                    else
                    {
                        return NSAPEntities.GearViewModel.GetGear(GearID).GearName;
                    }
                }
            }
        }
        public LandingSiteSampling Parent
        {
            set { _parent = value; }
            get
            {
                if (_parent == null)
                {
                    _parent = NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteSampling(LandingSiteSamplingID);
                }
                return _parent;
            }
        }

        public override string ToString()
        {
            if (Parent.LandingSite == null)
            {
                return "";
            }
            else
            {

                return $"{Parent.LandingSite.LandingSiteName} {GearUsedName} {Parent.SamplingDate.ToString("MMM-dd-yyyy")} - ({ListVesselUnload.Count} - sector:{SectorCode})";
            }
        }

    }
}
