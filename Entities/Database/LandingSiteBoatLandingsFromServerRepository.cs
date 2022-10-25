using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace NSAP_ODK.Entities.Database
{
    public class SpeciesRepeat
    {
        public LandingSiteBoatLandingFromServer Parent { get; set; }
        [JsonProperty("species_repeat/species_group/taxa")]
        public string SpeciesRepeatSpeciesGroupTaxa { get; set; }

        [JsonProperty("species_repeat/species_group/select_spName")]
        public string SpeciesRepeatSpeciesGroupSelectSpName { get; set; }

        [JsonProperty("species_repeat/species_group/species_csv_source")]
        public string SpeciesRepeatSpeciesGroupSpeciesCsvSource { get; set; }

        [JsonProperty("species_repeat/species_group/search_species")]
        public string SpeciesRepeatSpeciesGroupSearchSpecies { get; set; }

        [JsonProperty("species_repeat/species_group/species")]
        public string SpeciesRepeatSpeciesGroupSpecies { get; set; }

        [JsonProperty("species_repeat/species_group/sp_id")]
        public int SpeciesRepeatSpeciesGroupSpId { get; set; }

        [JsonProperty("species_repeat/species_group/species_name_selected")]
        public string SpeciesRepeatSpeciesGroupSpeciesNameSelected { get; set; }

        [JsonProperty("species_repeat/species_group/twsp")]
        public string SpeciesRepeatSpeciesGroupTwsp { get; set; }

        [JsonProperty("species_repeat/species_group/repeat_title")]
        public string SpeciesRepeatSpeciesGroupRepeatTitle { get; set; }

        [JsonProperty("species_repeat/species_group/species_notfish")]
        public int SpeciesRepeatSpeciesGroupSpeciesNotfish { get; set; }
    }
    public class LandingsRepeat
    {
        private static int _pk;
        private int _rowID;
        private bool? _isSaved;
        private GearUnload _saveGearUnloadObject;
        [JsonProperty("landings_repeat/landing/select_gear")]
        public string SelectGear { get; set; }
        [JsonProperty("landings_repeat/landing/gear_used")]
        public string GearUsed { get; set; }
        [JsonProperty("landings_repeat/landing/gear_used_text")]
        public string GearUsedText { get; set; }
        [JsonProperty("landings_repeat/landing/gear_code")]
        public string GearCode { get; set; }
        [JsonProperty("landings_repeat/landing/gear_name")]
        public string GearName { get; set; }
        public Gear Gear { get { return NSAPEntities.GearViewModel.GetGear(GearCode); } }
        [JsonProperty("landings_repeat/landing/landings_count")]
        public int Count { get; set; }
        [JsonProperty("landings_repeat/landing/total_catch_wt")]
        public double? TotalCatchWt { get; set; }
        [JsonProperty("landings_repeat/landing/landings_note")]
        public string Note { get; set; }

        [JsonProperty("landings_repeat/landing/landing_catch")]
        public string LandingsRepeatLandingLandingCatch { get; set; }

        public LandingSiteBoatLandingFromServer Parent { get; set; }

        public static bool RowIDSet { get; set; }
        public static void SetRowIDs()
        {
            if (NSAPEntities.GearUnloadViewModel.GearUnloadCollection.Count == 0)
            {
                _pk = 0;
            }
            else
            {
                _pk = NSAPEntities.GearUnloadViewModel.NextRecordNumber - 1;
            }
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (SavedInLocalDatabase)
                {
                    _rowID = NSAPEntities.GearUnloadViewModel.GearUnloadCollection
                        .Where(t => t.GearUsedName == GearName &&
                         t.Parent.PK == Parent.PK).FirstOrDefault().PK;
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

        private GearUnload SavedGearUnloadObject
        {
            get
            {
                _saveGearUnloadObject = NSAPEntities.GearUnloadViewModel.GearUnloadCollection
                   .FirstOrDefault(t => t.Parent.PK == Parent.PK && t.GearUsedName == GearName);
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
    }

    public class ValidationStatus
    {

    }
    public class LandingSiteBoatLandingFromServer
    {
        private LandingSiteSampling _savedLandingObject;
        private bool? _isSaved;
        private static int _pk;
        private int _rowid;
        private List<LandingsRepeat> _landingsRepeat;
        private List<SpeciesRepeat> _speciesesRepeat;

        public string has_fishing_operation { get; set; }
        public string reason_no_operation { get; set; }
        public string include_twsp { get; set; }

        [JsonProperty("vessel_sampling/sampling_date")]
        public DateTime SamplingDate { get; set; }
        [JsonProperty("vessel_sampling/is_sampling_day")]
        public string IsSamplingDay { get; set; }
        public bool SamplingConducted
        {
            get { return IsSamplingDay == "yes"; }
            set
            {
                if (value)
                {
                    IsSamplingDay = "yes";
                }
                else
                {
                    IsSamplingDay = "no";
                }
            }
        }
        [JsonProperty("vessel_sampling/nsap_region")]
        public string NsapRegionCode { get; set; }
        public string NSAPRegionName { get { return Region.Name; } }
        public NSAPRegion Region { get { return NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(NsapRegionCode); } }
        [JsonProperty("vessel_sampling/fma_in_region")]
        public int FmaInRegion { get; set; }


        public FMA FMA { get { return NSAPEntities.NSAPRegionViewModel.GetFMAInRegion(NsapRegionCode, FmaInRegion); } }
        [JsonProperty("vessel_sampling/select_enumerator")]
        public string SelectEnumerator { get; set; }
        [JsonProperty("vessel_sampling/region_enumerator")]
        public int? RegionEnumerator { get; set; }

        public NSAPEnumerator NSAPEnumerator
        {
            get
            {
                if (RegionEnumerator != null)
                {
                    return NSAPEntities.NSAPRegionViewModel.GetEnumeratorInRegion(NsapRegionCode, (int)RegionEnumerator);
                }
                else
                {
                    return null;
                }
            }
        }
        [JsonProperty("vessel_sampling/region_enumerator_text")]
        public string RegionEnumeratorText { get; set; }

        public string NSAPEnumeratorName
        {
            get
            {
                if (RegionEnumerator == null)
                {
                    return RegionEnumeratorText;
                }
                else
                {
                    if (NSAPEnumerator != null)
                    {
                        return NSAPEnumerator.Name;
                    }
                    else
                    {
                        return "";
                    }
                }
            }
        }

        [JsonProperty("vessel_sampling/fishing_ground")]
        public int FishingGroundCode { get; set; }

        public FishingGround FishingGround { get { return NSAPEntities.NSAPRegionViewModel.GetFishingGroundInRegion(NsapRegionCode, FmaInRegion, FishingGroundCode); } }
        [JsonProperty("vessel_sampling/select_landingsite")]

        public string SelectLandingsite { get; set; }
        [JsonProperty("vessel_sampling/landing_site")]
        public int? LandingSiteCode { get; set; }
        [JsonProperty("vessel_sampling/landing_site_text")]
        public string LandingSiteText { get; set; }
        [JsonProperty("vessel_sampling/sampling_notes")]
        public string Notes { get; set; }
        public LandingSite LandingSite
        {
            get
            {
                if (LandingSiteCode == null)
                {
                    return null;
                }
                else
                {
                    return NSAPEntities.NSAPRegionViewModel.GetLandingSiteInRegion(NsapRegionCode, FmaInRegion, FishingGroundCode, (int)LandingSiteCode);
                }
            }
        }
        public string LandingSiteName
        {
            get
            {
                if (LandingSiteCode == null)
                {
                    return LandingSiteText;
                }
                else
                {
                    return LandingSite?.ToString();
                }
            }
        }





        public List<object> _notes { get; set; }
        public List<object> _tags { get; set; }
        public string _xform_id_string { get; set; }
        [JsonProperty("meta/instanceID")]
        public string MetaInstanceID { get; set; }
        public DateTime start { get; set; }

        public List<SpeciesRepeat> Species_repeat 
        {
            get { return _speciesesRepeat; } 
            set
            {
                _speciesesRepeat = value;
                foreach(SpeciesRepeat item in _speciesesRepeat)
                {
                    item.Parent = this;
                }
            }
        }
        public List<LandingsRepeat> Landings_repeat
        {
            get { return _landingsRepeat; }
            set
            {
                _landingsRepeat = value;
                foreach (LandingsRepeat item in _landingsRepeat)
                {
                    item.Parent = this;
                }
            }
        }
        public List<object> _geolocation { get; set; }
        public string _status { get; set; }
        [JsonProperty("formhub/uuid")]
        public string FormhubUuid { get; set; }
        public DateTime today { get; set; }
        public ValidationStatus _validation_status { get; set; }
        public string _uuid { get; set; }
        public object _submitted_by { get; set; }
        public string device_id { get; set; }
        public string __version__ { get; set; }
        public DateTime _submission_time { get; set; }
        public List<object> _attachments { get; set; }
        public string intronote { get; set; }
        public string user_name { get; set; }
        public int _id { get; set; }

        public LandingSiteSampling SavedLandingObject
        {
            get
            {
                //_savedLandingObject = NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection.FirstOrDefault(t => t.RowID == _uuid);
                _savedLandingObject = NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection
                    .FirstOrDefault(
                        t => t.NSAPRegionID == NsapRegionCode &&
                        t.FMAID == FMA.FMAID &&
                        t.FishingGround.Code == FishingGround.Code &&
                        t.LandingSiteName == LandingSiteName &&
                        t.SamplingDate == SamplingDate);
                return _savedLandingObject;
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
        public static bool RowIDSet { get; set; }
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


    }
    public static class LandingSiteBoatLandingsFromServerRepository
    {
        public static event EventHandler<UploadToDbEventArg> UploadSubmissionToDB;
        private static List<LandingsRepeat> _listLandingsRepeat;


        public static string JSON { get; set; }
        public static List<LandingsRepeat> GetLandings()
        {
            List<LandingsRepeat> thisList = new List<LandingsRepeat>();
            if (_listLandingsRepeat == null)
            {
                LandingsRepeat.SetRowIDs();
                foreach (var item in LandingSiteBoatLandings)
                {
                    if (item.Landings_repeat != null)
                    {
                        foreach (var landing in item.Landings_repeat)
                        {
                            thisList.Add(landing);
                        }
                    }
                }
                _listLandingsRepeat = thisList;
            }

            return _listLandingsRepeat;

        }
        public static void CreateLandingSiteBoatLandingsFromJson()
        {
            _listLandingsRepeat = null;
            LandingSiteBoatLandingFromServer.SetRowIDs();
            LandingSiteBoatLandings = JsonConvert.DeserializeObject<List<LandingSiteBoatLandingFromServer>>(JSON);
        }
        public static List<LandingSiteBoatLandingFromServer> LandingSiteBoatLandings { get; internal set; }

        public static Task<bool> UploadToDBAsync()
        {
            return Task.Run(() => UploadToLocalDatabase());
        }
        public static bool UploadToLocalDatabase()
        {
            int savedCount = 0;

            if (!LandingsRepeat.RowIDSet)
            {
                LandingsRepeat.SetRowIDs();
            }

            if (LandingSiteBoatLandings.Count > 0)
            {
                UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { LandingSiteBoatLandingsToSaveCount = LandingSiteBoatLandings.Count, Intent = UploadToDBIntent.StartOfUpload });
                foreach (var landing in LandingSiteBoatLandings)
                {
                    LandingSiteSampling ls = new LandingSiteSampling
                    {
                        PK = landing.PK,
                        NSAPRegionID = landing.Region.Code,
                        SamplingDate = landing.SamplingDate,
                        LandingSiteID = landing.LandingSite == null ? null : (int?)landing.LandingSite.LandingSiteID,
                        FishingGroundID = landing.FishingGround.Code,
                        Remarks = landing.Notes,
                        IsSamplingDay = landing.SamplingConducted,
                        LandingSiteText = landing.LandingSiteText,
                        FMAID = landing.FMA.FMAID,

                        DateSubmitted = landing._submission_time,
                        UserName = landing.user_name,
                        DeviceID = landing.device_id,
                        XFormIdentifier = landing._xform_id_string,
                        DateAdded = DateTime.Now,
                        FromExcelDownload = false,
                        FormVersion = landing.intronote,
                        RowID = landing._uuid,
                        EnumeratorID = landing.RegionEnumerator,
                        EnumeratorText = landing.RegionEnumeratorText
                    };
                    if (landing.SavedInLocalDatabase)
                    {
                        if (NSAPEntities.LandingSiteSamplingViewModel.UpdateRecordInRepo(ls))
                        {

                            SaveLandingRepeat(landing, landing.SavedLandingObject);
                        }
                    }
                    else
                    {
                        if (NSAPEntities.LandingSiteSamplingViewModel.AddRecordToRepo(ls))
                        {
                            savedCount++;
                            landing.SavedInLocalDatabase = true;
                            SaveLandingRepeat(landing, ls);
                        }
                        UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { LandingSiteBoatLandingsSavedCount = savedCount, Intent = UploadToDBIntent.Uploading });
                    }
                }
                UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { LandingSiteBoatLandingsTotalSavedCount = savedCount, Intent = UploadToDBIntent.EndOfUpload });
            }
            return savedCount > 0;
        }

        public static bool SaveLandingRepeat(LandingSiteBoatLandingFromServer lsbl, LandingSiteSampling lss)
        {
            bool success = false;
            if (lsbl.Landings_repeat != null && lsbl.Landings_repeat.Count > 0)
            {
                foreach (var landingRepeat in lsbl.Landings_repeat)
                {

                    GearUnload gu = new GearUnload
                    {
                        PK = (int)landingRepeat.PK,
                        LandingSiteSamplingID = lss.PK,
                        GearID = landingRepeat.Gear == null ? null : landingRepeat.Gear.Code,
                        Boats = landingRepeat.Count,
                        Catch = landingRepeat.TotalCatchWt,
                        GearUsedText = landingRepeat.GearUsedText,
                        Remarks = landingRepeat.Note
                    };

                    if (landingRepeat.SavedInLocalDatabase)
                    {
                        success = NSAPEntities.GearUnloadViewModel.UpdateRecordInRepo(gu);
                    }
                    else
                    {
                        success = NSAPEntities.GearUnloadViewModel.AddRecordToRepo(gu);
                    }
                }
            }

            return success;
        }

    }
}
