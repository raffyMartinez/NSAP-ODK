using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class LandingSiteSamplingFlattened
    {
        public LandingSiteSamplingFlattened(LandingSiteSampling lss)
        {
            ID = lss.PK;
            NSAPRegion = lss.NSAPRegion.ToString();
            FMA = lss.FMA.ToString();
            FishingGround = lss.FishingGround.ToString();
            LandingSite = lss.LandingSiteName;
            Remarks = lss.Remarks;
            SamplingDate = lss.SamplingDate;
        }
        public int ID { get; private set; }
        public string NSAPRegion { get; private set; }
        public string FMA { get; private set; }
        public string FishingGround { get; private set; }

        public string LandingSite { get; private set; }
        public string Remarks { get; private set; }
        public DateTime SamplingDate
        {
            get; private set;

        }
    }
    public class LandingSiteSampling
    {
        private LandingSite _landingSite;
        private NSAPRegion _nsapRegion;
        private FMA _fma;
        private FishingGround _fishingGround;
        private List<GearInLandingSite> _gearsInLandingSite;

        public List<GearInLandingSite> GearsInLandingSite {
            get
            {
                if (_gearsInLandingSite == null)
                {
                    _gearsInLandingSite = NSAPEntities.LandingSiteSamplingViewModel.GetGearsInLandingSiteSampling(this);
                }
                return _gearsInLandingSite;
            }
            set { _gearsInLandingSite = value; }
        }
        public bool HasFishingOperation { get; set; }
        public bool IsMultiVessel { get; set; }
        public bool DelayedSave { get; set; }
        public int PK { get; set; }
        public string NSAPRegionID { get; set; }
        public DateTime SamplingDate { get; set; }
        public int? LandingSiteID { get; set; }
        public string FishingGroundID { get; set; }
        public string Remarks { get; set; }

        public bool SamplingFromCatchCompositionIsAllowed { get; set; }

        public string JSONFileName { get; set; }
        public bool IsSamplingDay { get; set; }
        public int? NumberOfLandingsSampled { get; set; }
        public int? NumberOfGearTypesInLandingSite { get; set; }
        public int? NumberOfLandings { get; set; }
        public string LandingSiteName
        {
            get
            {
                if (LandingSiteID == null)
                {
                    return LandingSiteText;
                }
                else
                {
                    if (LandingSite == null)
                    {
                        return "UNRECOGNIZED LANDING SITE";
                    }
                    else
                    {
                        return LandingSite.ToString();
                    }
                }
            }
        }

        public GearUnloadViewModel GearUnloadViewModel { get; set; }
        public string LandingSiteText { get; set; }
        public int FMAID { get; set; }

        //public string Notes { get; set; }
        public FishingGround FishingGround
        {
            set { _fishingGround = value; }
            get
            {
                if (_fishingGround == null)
                {
                    _fishingGround = NSAPEntities.FishingGroundViewModel.GetFishingGround(FishingGroundID);
                }
                return _fishingGround;
            }
        }
        public FMA FMA
        {
            set { _fma = value; }
            get
            {
                if (_fma == null)
                {
                    _fma = NSAPEntities.FMAViewModel.GetFMA(FMAID);
                }
                return _fma;
            }
        }

        public DateTime MonthSampled
        {
            get
            {
                return new DateTime(SamplingDate.Year, SamplingDate.Month, 1);
            }
        }
        public LandingSite LandingSite
        {
            set { _landingSite = value; }
            get
            {
                if (LandingSiteID != null && _landingSite == null)
                {
                    _landingSite = NSAPEntities.LandingSiteViewModel.GetLandingSite((int)LandingSiteID);
                }
                return _landingSite;
            }
        }

        public NSAPRegion NSAPRegion
        {
            set { _nsapRegion = value; }
            get
            {
                if (_nsapRegion == null)
                {
                    _nsapRegion = NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(NSAPRegionID);
                }
                return _nsapRegion;
            }
        }

        public override string ToString()
        {
            if (LandingSiteText != null && LandingSiteText.Length > 0)
            {
                return $"{NSAPRegion} - {FMA} - {FishingGround} - {LandingSiteText} - {SamplingDate.ToString("MMM-dd-yyyy")}";
            }
            else
            {
                return $"{NSAPRegion} - {FMA} - {FishingGround} - {LandingSite} - {SamplingDate.ToString("MMM-dd-yyyy")}";
            }
        }
        public DateTime? DateSubmitted { get; set; }
        public string UserName { get; set; }
        public string DeviceID { get; set; }
        public string XFormIdentifier { get; set; }
        public DateTime? DateAdded { get; set; }
        public bool FromExcelDownload { get; set; }
        public string FormVersion { get; set; }
        public string RowID { get; set; }
        public int? EnumeratorID { get; set; }
        public string EnumeratorText { get; set; }

        public string EnumeratorName
        {
            get
            {
                if (EnumeratorID == null)
                {
                    return EnumeratorText;
                }
                else
                {
                    return NSAPEntities.NSAPEnumeratorViewModel.GetNSAPEnumerator((int)EnumeratorID).Name;
                }
            }
        }
    }

}
