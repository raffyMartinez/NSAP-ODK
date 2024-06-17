using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class LandingSiteSampingIdentifier
    {
        public int PK { get; set; }
        public string XFormIdentifier { get; set; }
        public string RowID { get; set; }
        public int? SubmissionID { get; set; }
        public override string ToString()
        {
            return $"RowID:{RowID} - SubmissionID:{SubmissionID} - XFormID:{XFormIdentifier}";
        }
    }
    public class LandingSiteSamplingSummarized
    {
        public LandingSiteSamplingSummarized(LandingSiteSampling lss, bool fromSummaryItem = false)
        {
            if (lss.LandingSiteID != null)
            {
                LandingSite = lss.LandingSite.ToString();
            }
            else
            {
                LandingSite = lss.LandingSiteText;
            }
            LandingSiteSampling = lss;

            if (fromSummaryItem)
            {

                foreach (SummaryItem si in NSAPEntities.SummaryItemViewModel.SummaryItemCollection.Where(t => t.SamplingDayID == lss.PK))
                {

                    if (si.IsSamplingDay)
                    {
                        NumberOfVesselUnloads++;
                        if (si.HasCatchComposition)
                        {
                            NumberOfVesselUnloadsWithCatchComposition++;
                        }
                    }
                }
            }
            else
            {
                foreach (GearUnload gu in lss.GearUnloadViewModel.GearUnloadCollection)
                {
                    if (gu.VesselUnloadViewModel == null)
                    {
                        gu.VesselUnloadViewModel = new VesselUnloadViewModel(gu, updatesubViewModels: true);
                    }
                    foreach (VesselUnload vu in gu.VesselUnloadViewModel.VesselUnloadCollection)
                    {
                        NumberOfVesselUnloads++;
                        foreach (VesselUnload_FishingGear vufg in vu.VesselUnload_FishingGearsViewModel.VesselUnload_FishingGearsCollection)
                        {
                            if (vufg.CountItemsInCatchComposition > 0)
                            {
                                NumberOfVesselUnloadsWithCatchComposition++;
                                break;
                            }
                        }
                    }
                }
            }
            IsSamplingDay = lss.IsSamplingDay;
            SamplingDate = lss.SamplingDate;
            MonthOfSampling = new DateTime(lss.SamplingDate.Year, lss.SamplingDate.Month, 1);
            ForDeletion = false;
        }

        public DateTime MonthOfSampling { get; set; }

        public LandingSiteSampling LandingSiteSampling { get; set; }

        public string LandingSite { get; set; }
        public int NumberOfVesselUnloads { get; set; }
        public int NumberOfVesselUnloadsWithCatchComposition { get; set; }
        public bool IsSamplingDay { get; set; }
        public bool ForDeletion { get; set; }

        public DateTime SamplingDate { get; private set; }
    }
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
            Enumerator = lss.EnumeratorName;
            IsSamplingDay = lss.IsSamplingDay;
            NumberOfLandingsSampled = lss.NumberOfLandingsSampled;
            NumberOfGearTypesInLandingSite = lss.NumberOfGearTypesInLandingSite;
            HasFishingOperation = lss.HasFishingOperation;
            NumberOfLandings = lss.NumberOfLandings;
            if (lss.GearUnloadViewModel != null)
            {
                GearUnloads = lss.GearUnloadViewModel.GearUnloadCollection.ToList();
            }
            SamplingDate_MDY = SamplingDate.ToString("MMM-dd-yyyy");
        }

        public string SamplingDate_MDY { get; private set; }
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

        public bool IsSamplingDay { get; private set; }
        public int? NumberOfLandingsSampled { get; private set; }
        public int? NumberOfGearTypesInLandingSite { get; private set; }
        public string Enumerator { get; private set; }

        public bool HasFishingOperation { get; private set; }

        public int? NumberOfLandings { get; private set; }

        public List<GearUnload> GearUnloads { get; private set; }
    }
    public class LandingSiteSamplingForCrosstab
    {
        public int RowID { get; set; }
        public DateTime SamplingDate { get; set; }
        public DateTime SampledMonth { get; set; }
        public string Region { get; set; }
        public string FMA { get; set; }
        public string FishingGround { get; set; }
        public string LandingSite { get; set; }
        public string Enumerator { get; set; }
        public bool HasFishingOperation { get; set; }
        public bool IsSamplingDay { get; set; }
        public string Notes { get; set; }
        public int? NumberOfLandings { get; set; }

        public int? NumberLandingsMonitored { get; set; }
        public int? NumberGearTypes { get; set; }
        public string Gear { get; set; }

        public string SectorCode { get; set; }
        public string Sector
        {
            get
            {
                if (string.IsNullOrEmpty(SectorCode))
                {
                    return string.Empty;
                }
                else if (SectorCode == "c")
                {
                    return "Commercial";
                }
                else if (SectorCode == "m")
                {
                    return "Municipal";
                }
                else
                {
                    return "Other";
                }
            }
        }

        public int? NumberLandingsOfGear { get; set; }
        public double? WeightCatchOfGear { get; set; }
        public static bool PropertyColumnVisible(string pn)
        {
            bool isVisible = true;
            if (pn == "SampledMonth" || pn == "SectorCode")
            {
                isVisible = false;
            }
            return isVisible;
        }
        public static bool PropertyIsNumeric(string pn)
        {
            bool isNumeric = false;
            if (pn == "NumberLandingsOfGear" ||
                pn == "WeightCatchOfGear" ||
                pn == "NumberGearTypes" ||
                pn == "NumberLandingsMonitored" ||
                pn == "NumberOfLandings" ||
                pn == "RowID")
            {
                isNumeric = true;
            }
            return isNumeric;
        }
        public static string GetPropertyAlias(string pn)
        {
            string rv = pn;
            if (pn == "RowID")
            {
                rv = "Row ID";
            }
            else if (pn == "SamplingDate")
            {
                rv = "Sampling date";
            }
            else if (pn == "SampledMonth")
            {
                rv = "Sampling month";
            }
            else if (pn == "FishingGround")
            {
                rv = "Fishing ground";
            }
            else if (pn == "LandingSite")
            {
                rv = "Landing site";
            }
            else if (pn == "HasFishingOperation")
            {
                rv = "Has landings";
            }
            else if (pn == "NumberOfLandings")
            {
                rv = "# of landings";
            }
            else if (pn == "IsSamplingDay")
            {
                rv = "Is sampling day";
            }
            else if (pn == "NumberLandingsMonitored")
            {
                rv = "# of landings monitored";
            }
            else if (pn == "NumberGearTypes")
            {
                rv = "# of gear types";
            }
            else if (pn == "NumberLandingsOfGear")
            {
                rv = "Number of landings of gear";
            }
            else if (pn == "WeightCatchOfGear")
            {
                rv = "Total weight of catch of gear";
            }

            return rv;
        }

    }
    public class LandingSiteSampling
    {
        private LandingSite _landingSite;
        private NSAPRegion _nsapRegion;
        private FMA _fma;
        private FishingGround _fishingGround;
        private List<GearInLandingSite> _gearsInLandingSite;
        private string _fishingGroundID;

        public override bool Equals(object obj)
        {
            if (obj != null && obj is LandingSiteSampling)
            {
                return PK == ((LandingSiteSampling)obj).PK;
            }
            else
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            return PK.GetHashCode();
        }
        public CarrierLandingViewModel CarrierLandingViewModel { get; set; }
        //public VesselCatchViewModel VesselCatchViewModel { get; set; }
        //public CatcherBoatOperation_ViewModel CatcherBoatOperation_ViewModel { get; set; }
        //public CarrierBoatLanding_FishingGround_ViewModel CarrierBoatLanding_FishingGround_ViewModel { get; set; }

        public string LandingSiteTypeOfSampling { get; set; }

        //public string CarrierBoatName { get; set; }
        //public int? CountCatcherBoats { get; set; }
        //public int? CountCarrierCatchSpeciesComposition { get; set; }
        public static string LandingSiteSamplingType(string typeOfSampling)
        {
            string samplingType = "";
            switch (typeOfSampling)
            {
                case "rs":
                    samplingType = "Regular sampling";
                    break;
                case "cbl":
                    samplingType = "Carrier boats only";
                    break;
            }
            return samplingType;
        }
        public List<GearInLandingSite> GearsInLandingSite
        {
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

        public int? Submission_id { get; set; }
        public bool HasFishingOperation { get; set; }
        public bool IsMultiVessel { get; set; }
        public bool DelayedSave { get; set; }
        public int PK { get; set; }
        public string NSAPRegionID { get; set; }
        public DateTime SamplingDate { get; set; }


        public int? LandingSiteID { get; set; }
        public string FishingGroundID
        {
            get { return _fishingGroundID; }
            set
            {
                _fishingGroundID = value;
                _fishingGround = NSAPEntities.FishingGroundViewModel.GetFishingGround(_fishingGroundID);
            }
        }
        public string Remarks { get; set; }

        public bool SamplingFromCatchCompositionIsAllowed { get; set; }

        public DateTime? DateDeletedFromServer { get; set; }

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

        public bool FoundInServer { get; set; }

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
        public int? CountCarrierLandings { get; set; }
        public int? CountCarrierSamplings { get; set; }
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

