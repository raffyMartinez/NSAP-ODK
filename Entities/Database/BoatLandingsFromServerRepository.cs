using Newtonsoft.Json;
using NSAP_ODK.Entities;
using NSAP_ODK.Entities.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public static class BoatLandingsFromServerRepository
    {
        private static List<GearRepeat> _gearRepeat;

        private static List<SpeciesTWSpRepeat> _speciesTWSPRepeat;
        public static string JSON { get; set; }
        public static void CreateBoatLandingsFromJson()
        {

            _gearRepeat = null;
            BoatLandingsFromServer.SetRowIDs();
            BoatLandings = JsonConvert.DeserializeObject<List<BoatLandingsFromServer>>(JSON);
            _gearRepeat = null;
            _speciesTWSPRepeat = null;
        }

        public static List<BoatLandingsFromServer> BoatLandings { get; internal set; }

        public static List<SpeciesTWSpRepeat> GetSpeciesTTWSpRepeats()
        {
            List<SpeciesTWSpRepeat> thisList = new List<SpeciesTWSpRepeat>();

            if (_gearRepeat == null)
            {
                GetGears();
            }

            if (_speciesTWSPRepeat == null)
            {
                SpeciesTWSpRepeat.SetRowIDs();
                foreach (var gr in _gearRepeat)
                {
                    if (gr.SpeciesTWSpRepeat != null)
                    {
                        foreach (var sp in gr?.SpeciesTWSpRepeat)
                        {
                            thisList.Add(sp);
                        }
                    }
                }
                _speciesTWSPRepeat = thisList;
            }
            return _speciesTWSPRepeat;
        }
        public static List<GearRepeat> GetGears()
        {
            List<GearRepeat> thisList = new List<GearRepeat>();
            if (_gearRepeat == null)
            {
                GearRepeat.SetRowIDs();
                foreach (var item in BoatLandings)
                {
                    if (item.gear_repeat != null)
                    {
                        foreach (var gear in item.gear_repeat)
                        {
                            thisList.Add(gear);
                        }
                    }
                }
                _gearRepeat = thisList;
            }

            return _gearRepeat;

        }
    }
}
// Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
public class GearRepeat
{
    private static bool _rowIDSet;
    private static int _pk;
    private int _rowID;
    private bool? _isSaved;
    private GearUnload _saveGearUnloadObject;
    private List<SpeciesTWSpRepeat> _speciesTWSpRepeat;

    public static bool RowIDSet
    {
        get { return _rowIDSet; }
        set
        {
            _rowIDSet = value;
            if (!_rowIDSet)
            {
                _pk = 0;
            }
        }
    }
    public static void SetRowIDs()
    {
        //if (NSAPEntities.GearUnloadViewModel.GearUnloadCollection.Count == 0)
        if (GearUnloadRepository.GearUnloadCount() == 0)
        {
            _pk = 0;
        }
        else
        {
            _pk = GearUnloadRepository.MaxRecordNumberFromDB();
        }
        RowIDSet = true;
    }
    public int? PK
    {
        get
        {
            if (SavedInLocalDatabase)
            {
                _rowID = Parent.SavedLandingObject.GearUnloadViewModel.GearUnloadCollection
                    .Where(t => t.GearUsedName == GearName &&
                            t.Parent.PK == Parent.PK).FirstOrDefault().PK;
                //_rowID = NSAPEntities.GearUnloadViewModel.GearUnloadCollection
                //    .Where(t => t.GearUsedName == GearName &&
                //     t.Parent.PK == Parent.PK).FirstOrDefault().PK;
            }
            else
            {
                if (_rowID == 0)
                {
                    _rowID = ++_pk;
                }
            }
            return _rowID;
        }
    }


    public GearUnload SavedGearUnloadObject
    {
        get
        {
            //_saveGearUnloadObject = NSAPEntities.GearUnloadViewModel.GearUnloadCollection
            //   .FirstOrDefault(t => t.Parent.PK == Parent.PK && t.GearUsedName == GearName);
            _saveGearUnloadObject = NSAPEntities.SummaryItemViewModel.GetGearUnload(Parent.PK, GearName);
            return _saveGearUnloadObject;
        }

    }
    public bool SavedInLocalDatabase
    {
        get
        {
            if (_isSaved == null)
            {
                _isSaved = SavedGearUnloadObject != null;
            }
            return (bool)_isSaved;
        }
        set { _isSaved = value; }

    }

    [JsonProperty("gear_repeat/gear_group_1/gear_group_2/include_counts")]
    public string include_counts { get; set; }
    
    public bool IncludeCounts
    {
        get
        {
            return include_counts == "yes";
        }
        set
        {
            if (value)
            {
                include_counts = "yes";
            }
            else
            {
                include_counts = "no";
            }
        }
    }
    public BoatLandingsFromServer Parent { get; set; }

    [JsonProperty("gear_repeat/gear_group_1/gear_group_2/gear_used_text")]
    public string GearUsedText { get; set; }

    public Gear Gear { get { return NSAPEntities.GearViewModel.GetGear(GearCode); } }
    [JsonProperty("gear_repeat/gear_group_1/gear_group_2/select_gear")]
    public string SelectGear { get; set; }

    [JsonProperty("gear_repeat/gear_group_1/gear_group_2/gear_used")]
    public string GearUsed { get; set; }

    [JsonProperty("gear_repeat/gear_group_1/gear_group_2/gear_code")]
    public string GearCode { get; set; }

    [JsonProperty("gear_repeat/gear_group_1/gear_group_2/gear_name")]
    public string GearName { get; set; }

    [JsonProperty("gear_repeat/gear_group_1/gear_group_2/landings_count")]
    public int LandingsCount { get; set; }

    [JsonProperty("gear_repeat/gear_group_1/gear_group_2/total_catch_wt")]
    public double? TotalCatchWt { get; set; }

    [JsonProperty("gear_repeat/gear_group_1/include_twsp")]
    public string include_twsp { get; set; }

    public bool IncludeTWSp
    {
        get { return include_twsp == "yes"; }
        set
        {
            if (value)
            {
                include_twsp = "yes";
            }
            else
            {
                include_twsp = "no";
            }
        }
    }

    [JsonProperty("gear_repeat/gear_group_1/species_twsp_repeat")]
    //public List<GearRepeatGearGroup1SpeciesTwspRepeat> GearRepeatGearGroup1SpeciesTwspRepeat { get; set; }
    public List<SpeciesTWSpRepeat> SpeciesTWSpRepeat
    {
        get { return _speciesTWSpRepeat; }
        set
        {
            _speciesTWSpRepeat = value;
            foreach (var item in _speciesTWSpRepeat)
            {
                item.Parent = this;
            }
        }
    }

    [JsonProperty("gear_repeat/gear_group_1/sum_twsp")]
    public double? SumTwsp { get; set; }

    [JsonProperty("gear_repeat/gear_group_1/sum_twsp_coalesce")]
    public string SumTwspCoalesce { get; set; }

    [JsonProperty("gear_repeat/gear_group_1/landing_catch")]
    public string LandingCatch { get; set; }
}

//public class GearRepeatGearGroup1SpeciesTwspRepeat
public class SpeciesTWSpRepeat
{
    private int _rowID;
    private bool? _isSaved;
    private static int _pk;
    private static bool _rowIDSet;


    public static bool RowIDSet
    {
        get { return _rowIDSet; }
        set
        {
            _rowIDSet = value;
            if (!_rowIDSet)
            {
                _pk = 0;
            }
        }
    }

    public bool SavedInLocalDatabase
    {
        get
        {
            if (_isSaved == null)
            {
                _isSaved = SavedTWSpObject != null;
            }
            return (bool)_isSaved;
        }
        set { _isSaved = value; }

    }
    public int? PK
    {
        get
        {
            if (SavedInLocalDatabase)
            {
                _rowID = Parent.SavedGearUnloadObject.TotalWtSpViewModel.TotalWtSpCollection
                    .Where(t => t.SpeciesNameUsed == SpeciesNameSelected &&
                            t.Parent.PK == Parent.PK).FirstOrDefault().RowID;
            }
            else
            {
                if (_rowID == 0)
                {
                    _rowID = ++_pk;
                }
            }
            return _rowID;

        }

    }
    private TotalWtSp SavedTWSpObject
    {
        get
        {
            if (Parent.SavedGearUnloadObject == null)
            {
                return null;
            }
            else if (Parent.SavedGearUnloadObject.SpeciesWithTWSpCount == null)
            {
                return null;
            }
            //if (Parent?.SavedGearUnloadObject?.SpeciesWithTWSpCount == null)
            //    return null;
            else
            {
                if (SpId != null)
                {
                    return Parent.SavedGearUnloadObject.TotalWtSpViewModel.TotalWtSpCollection
                    .Where(t => (int)t.SpeciesID == (int)SpId).FirstOrDefault();
                }
                else
                {
                    return Parent.SavedGearUnloadObject.TotalWtSpViewModel.TotalWtSpCollection
                    .Where(t => t.SpeciesNameUsed == SpeciesNameSelected).FirstOrDefault();
                }

            }
        }
    }
    public static void SetRowIDs()
    {
        //if (NSAPEntities.GearUnloadViewModel.GearUnloadCollection.Count == 0)
        if (TotalWtSpRepository.TWSpCount() == 0)
        {
            _pk = 0;
        }
        else
        {
            _pk = TotalWtSpRepository.MaxRecordNumberFromDB();
        }
        RowIDSet = true;
    }
    public FishSpecies FishSpecies
    {
        get
        {
            if (SpId == null)
            {
                return null;
            }
            else
            {
                return NSAPEntities.FishSpeciesViewModel.GetSpecies((int)SpId);
            }
        }
    }

    public NotFishSpecies NotFishSpecies
    {
        get
        {
            if (SpeciesNotfish == null)
            {
                return null;
            }
            else
            {
                return NSAPEntities.NotFishSpeciesViewModel.GetSpecies((int)SpeciesNotfish);
            }
        }
    }
    public GearRepeat Parent { get; set; }
    [JsonProperty("gear_repeat/gear_group_1/species_twsp_repeat/species_twsp_group/taxa")]
    public string Taxa { get; set; }

    [JsonProperty("gear_repeat/gear_group_1/species_twsp_repeat/species_twsp_group/select_spName")]
    public string SelectSpName { get; set; }

    [JsonProperty("gear_repeat/gear_group_1/species_twsp_repeat/species_twsp_group/species_csv_source")]
    public string SpeciesCsvSource { get; set; }

    [JsonProperty("gear_repeat/gear_group_1/species_twsp_repeat/species_twsp_group/search_species")]
    public string SearchSpecies { get; set; }

    [JsonProperty("gear_repeat/gear_group_1/species_twsp_repeat/species_twsp_group/species")]
    public string Species { get; set; }

    [JsonProperty("gear_repeat/gear_group_1/species_twsp_repeat/species_twsp_group/sp_id")]
    public int? SpId { get; set; }

    [JsonProperty("gear_repeat/gear_group_1/species_twsp_repeat/species_twsp_group/species_name_selected")]
    public string SpeciesNameSelected { get; set; }

    [JsonProperty("gear_repeat/gear_group_1/species_twsp_repeat/species_twsp_group/twsp")]
    public double Twsp { get; set; }

    [JsonProperty("gear_repeat/gear_group_1/species_twsp_repeat/species_twsp_group/repeat_title")]
    public string RepeatTitle { get; set; }

    [JsonProperty("gear_repeat/gear_group_1/species_twsp_repeat/species_twsp_group/species_notfish")]
    public int? SpeciesNotfish { get; set; }

    [JsonProperty("gear_repeat/gear_group_1/species_twsp_repeat/species_twsp_group/spName_other")]
    public string SpNameOther { get; set; }
}

public class BoatLandingsFromServer
{
    private static bool _rowIDSet;
    private static int _pk;
    private LandingSiteSampling _savedLandingObject;
    private bool? _isSaved;
    private int _rowid;
    private List<GearRepeat> _gearsRepeat;
    public LandingSiteSampling SavedLandingObject
    {
        get
        {
            //_savedLandingObject = NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection.FirstOrDefault(t => t.RowID == _uuid);
            _savedLandingObject = NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection
                .FirstOrDefault(
                    t => t.NSAPRegionID == NsapRegion &&
                    t.FMAID == FMA.FMAID &&
                    t.FishingGround.Code == FishingGround.Code &&
                    t.LandingSiteName == LandingSiteName &&
                    t.SamplingDate == SamplingDate);
            return _savedLandingObject;
        }

    }
    public NSAPRegion Region { get { return NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(NsapRegion); } }
    public FishingGround FishingGround { get { return NSAPEntities.NSAPRegionViewModel.GetFishingGroundInRegion(NsapRegion, FmaInRegion, FishingGroundCode); } }
    public FMA FMA { get { return NSAPEntities.NSAPRegionViewModel.GetFMAInRegion(NsapRegion, FmaInRegion); } }

    [JsonProperty("vessel_sampling/landing_site_text")]
    public string LandingSiteText { get; set; }
    public string LandingSiteName
    {
        get
        {
            if (LandingSite == null)
            {
                return LandingSiteText;
            }
            else
            {
                return LandingSite.ToString();
            }
        }

    }
    public string EnumeratorName
    {
        get
        {
            if (NSAPEnumerator == null)
            {
                return EnumeratorText;
            }
            else
            {
                return NSAPEnumerator.ToString();
            }
        }
    }
    public NSAPEnumerator NSAPEnumerator
    {
        get
        {
            if (RegionEnumerator != null)
            {
                return NSAPEntities.NSAPRegionViewModel.GetEnumeratorInRegion(NsapRegion, (int)RegionEnumerator);
            }
            else
            {
                return null;
            }
        }
    }
    public bool SavedInLocalDatabase
    {
        get
        {
            if (_isSaved == null)
            {
                _isSaved = SavedLandingObject != null;
            }
            return (bool)_isSaved;
        }
        set { _isSaved = value; }

    }
    public int PK
    {
        get
        {
            if (!SavedInLocalDatabase)
            {
                if (_rowid == 0)
                {
                    _rowid = ++_pk;
                }
            }
            else
            {
                if (_savedLandingObject == null)
                {
                    _savedLandingObject = SavedLandingObject;
                }


                _rowid = _savedLandingObject.PK;

            }
            return _rowid;
        }
    }
    public static bool RowIDSet
    {
        get { return _rowIDSet; }
        set
        {
            _rowIDSet = value;
            if (!_rowIDSet)
            {
                _pk = 0;
            }
        }
    }
    public static void SetRowIDs()
    {
        if (NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection.Count == 0)
        {
            _pk = 0;
        }
        else
        {
            _pk = NSAPEntities.LandingSiteSamplingViewModel.NextRecordNumber - 1;
        }
        RowIDSet = true;

    }
    public int _id { get; set; }

    [JsonProperty("formhub/uuid")]
    public string FormhubUuid { get; set; }
    public DateTime start { get; set; }
    public string today { get; set; }
    public string device_id { get; set; }
    public string user_name { get; set; }
    public string email { get; set; }

    public string eFormVersion { get { return intronote.Replace("Version ", ""); } }
    public string intronote { get; set; }

    [JsonProperty("vessel_sampling/sampling_date")]
    public DateTime SamplingDate { get; set; }

    [JsonProperty("vessel_sampling/is_sampling_day")]
    public string is_sampling_day { get; set; }

    public bool IsSamplingDay
    {
        get { return is_sampling_day == "yes"; }
        set
        {
            if (value)
            {
                is_sampling_day = "yes";
            }
            else
            {
                is_sampling_day = "no";
            }
        }
    }

    [JsonProperty("vessel_sampling/nsap_region")]
    public string NsapRegion { get; set; }

    [JsonProperty("vessel_sampling/select_enumerator")]
    public string SelectEnumerator { get; set; }

    [JsonProperty("vessel_sampling/region_enumerator_text")]
    public string EnumeratorText { get; set; }

    [JsonProperty("vessel_sampling/region_enumerator")]
    public int? RegionEnumerator { get; set; }

    [JsonProperty("vessel_sampling/fma_in_region")]
    public int FmaInRegion { get; set; }

    [JsonProperty("vessel_sampling/fishing_ground")]
    public int FishingGroundCode { get; set; }

    [JsonProperty("vessel_sampling/select_landingsite")]
    public string SelectLandingsite { get; set; }

    [JsonProperty("vessel_sampling/landing_site")]
    public int? LandingSiteId { get; set; }

    [JsonProperty("vessel_sampling/sampling_notes")]
    public string Notes { get; set; }

    public LandingSite LandingSite
    {
        get
        {
            if (LandingSiteId == null)
            {
                return null;
            }
            else
            {
                return NSAPEntities.NSAPRegionViewModel.GetLandingSiteInRegion(NsapRegion, FmaInRegion, FishingGroundCode, (int)LandingSiteId);
            }
        }
    }
    public string sampling_date_string { get; set; }
    public int fma_number { get; set; }
    public string fishing_ground_name { get; set; }
    public string landing_site_name { get; set; }

    public string has_fishing_operation { get; set; }
    public bool HasFishingOperation
    {
        get { return has_fishing_operation == "yes"; }
        set
        {
            if (value)
            {
                has_fishing_operation = "yes";
            }
            else
            {
                has_fishing_operation = "no";
            }
        }
    }
    public List<GearRepeat> gear_repeat
    {
        get { return _gearsRepeat; }
        set
        {
            _gearsRepeat = value;
            foreach (GearRepeat item in _gearsRepeat)
            {
                item.Parent = this;
            }
        }
    }
    public string __version__ { get; set; }

    [JsonProperty("meta/instanceID")]
    public string MetaInstanceID { get; set; }

    [JsonProperty("meta/instanceName")]
    public string MetaInstanceName { get; set; }
    public string _xform_id_string { get; set; }
    public string _uuid { get; set; }
    public List<object> _attachments { get; set; }
    public string _status { get; set; }
    public List<object> _geolocation { get; set; }
    public DateTime _submission_time { get; set; }
    public List<object> _tags { get; set; }
    public List<object> _notes { get; set; }
    //public ValidationStatus _validation_status { get; set; }
    public object _submitted_by { get; set; }
    public string reason_no_operation { get; set; }

    public string NotesRemarks
    {
        get
        {
            string notesRemarks = "";
            if (!string.IsNullOrEmpty(Notes))
            {
                notesRemarks = $"{Notes}; ";
            }

            if (!string.IsNullOrEmpty(reason_no_operation))
            {
                notesRemarks += reason_no_operation;
            }
            return notesRemarks.Trim(';');
        }
    }
}




