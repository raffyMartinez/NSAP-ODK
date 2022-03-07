using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.InkML;
using Newtonsoft.Json;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace NSAP_ODK.Entities.Database.FromJson
{


    public class CatchCompGroupCatchCompositionRepeatLenWtRepeat
    {
        private static int _pk;
        private int _rowID;

        public static bool RowIDSet { get; private set; }
        public static void SetRowIDs()
        {
            if (NSAPEntities.CatchLengthWeightViewModel.CatchLengthWeightCollection.Count == 0)
            {
                _pk = 0;
            }
            else
            {
                _pk = NSAPEntities.CatchLengthWeightViewModel.NextRecordNumber - 1;
            }

            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }


            }
        }
        public CatchCompGroupCatchCompositionRepeat Parent { get; set; }

        public int ParentID { get { return Parent.PK; } }
        [JsonProperty("catch_comp_group/catch_composition_repeat/len_wt_repeat/len_wt_group/wt_lenwt")]
        public double Weight { get; set; }
        [JsonProperty("catch_comp_group/catch_composition_repeat/len_wt_repeat/len_wt_group/len_lenwt")]
        public double Length { get; set; }
    }

    public class CatchCompGroupCatchCompositionRepeatLengthFreqRepeat
    {
        private static int _pk;
        private int _rowID;
        public static bool RowIDSet { get; private set; }
        public static void SetRowIDs()
        {
            if (NSAPEntities.CatchLenFreqViewModel.CatchLenFreqCollection.Count == 0)
            {
                _pk = 0;
            }
            else
            {
                _pk = NSAPEntities.CatchLenFreqViewModel.NextRecordNumber - 1;
            }
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }


            }
        }
        public CatchCompGroupCatchCompositionRepeat Parent { get; set; }
        public int ParentID { get { return Parent.PK; } }

        [JsonProperty("catch_comp_group/catch_composition_repeat/length_freq_repeat/group_LF/length_class")]
        public double LengthClass { get; set; }
        [JsonProperty("catch_comp_group/catch_composition_repeat/length_freq_repeat/group_LF/freq")]
        public int Frequency { get; set; }
    }

    public class CatchCompGroupCatchCompositionRepeatLengthListRepeat
    {
        private static int _pk;
        private int _rowID;
        public static bool RowIDSet { get; private set; }
        public static void SetRowIDs()
        {
            if (NSAPEntities.CatchLengthViewModel.CatchLengthCollection.Count == 0)
            {
                _pk = 0;
            }
            else
            {
                _pk = NSAPEntities.CatchLengthViewModel.NextRecordNumber - 1;
            }
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }


            }
        }
        public CatchCompGroupCatchCompositionRepeat Parent { get; set; }
        public int ParentID { get { return Parent.PK; } }
        [JsonProperty("catch_comp_group/catch_composition_repeat/length_list_repeat/length")]
        public double Length { get; set; }
    }
    public class CatchCompGroupCatchCompositionRepeatGmsRepeatGroup
    {
        private static int _pk;
        private int _rowID;

        public static bool RowIDSet { get; private set; }
        public static void SetRowIDs()
        {
            if (NSAPEntities.CatchMaturityViewModel.CatchMaturityCollection.Count == 0)
            {
                _pk = 0;
            }
            else
            {
                _pk = NSAPEntities.CatchMaturityViewModel.NextRecordNumber - 1;
            }
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }


            }
        }

        public CatchCompGroupCatchCompositionRepeat Parent { get; set; }
        public int ParentID { get { return Parent.PK; } }
        [JsonProperty("catch_comp_group/catch_composition_repeat/gms_repeat_group/gms_group/individual_weight")]
        public double? Weight { get; set; }
        [JsonProperty("catch_comp_group/catch_composition_repeat/gms_repeat_group/gms_group/gms_repeat")]
        public string GMSCode { get; set; }
        public string GMS
        {
            get
            {
                return GMSCode == "pr" ? "Premature" :
                       GMSCode == "im" ? "Immature" :
                       GMSCode == "de" ? "Developing" :
                       GMSCode == "ma" ? "Maturing" :
                       GMSCode == "dt" ? "Mature" :
                       GMSCode == "ri" ? "Ripening" :
                       GMSCode == "gr" ? "Gravid" :
                       GMSCode == "spw" ? "Spawning" :
                       GMSCode == "sp" ? "Spent" : "";
            }
        }

        public string MaturityStage { get { return GMS; } }
        [JsonProperty("catch_comp_group/catch_composition_repeat/gms_repeat_group/gms_group/individual_length")]
        public double? Length { get; set; }


        [JsonProperty("catch_comp_group/catch_composition_repeat/gms_repeat_group/gms_group/gonad_wt")]
        public double? GonadWeight { get; set; }

        [JsonProperty("catch_comp_group/catch_composition_repeat/gms_repeat_group/gms_group/sex")]
        public string SexCode { get; set; }

        public string Sex
        {
            get
            {
                return SexCode == "f" ? "Female" :
                       SexCode == "m" ? "Male" :
                       SexCode == "j" ? "Juvenile" : "";
            }
        }
        [JsonProperty("catch_comp_group/catch_composition_repeat/gms_repeat_group/gms_group/gut_content_category")]
        public string GutContentCategoryCode { get; set; }
        public string GutContentCategory
        {
            get
            {
                return GutContentCategoryCode == "F" ? "Full" :
                       GutContentCategoryCode == "HF" ? "Half full" :
                       GutContentCategoryCode == "E" ? "Empty" : "";
            }
        }
        [JsonProperty("catch_comp_group/catch_composition_repeat/gms_repeat_group/gms_group/stomach_content_wt")]
        public double? StomachContentWt { get; set; }

        public double? GutContentWeight { get { return StomachContentWt; } }

    }

    public class CatchCompGroupCatchCompositionRepeat
    {
        private static int _pk;
        private int _rowID;
        private string _speciesNameSelected;

        private List<CatchCompGroupCatchCompositionRepeatLenWtRepeat> _lenWtRepeat;
        private List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat> _lenFreqRepeat;
        private List<CatchCompGroupCatchCompositionRepeatLengthListRepeat> _lengthListRepeat;
        private List<CatchCompGroupCatchCompositionRepeatGmsRepeatGroup> _gmsRepeat;

        public VesselLanding Parent { get; set; }

        public int ParentID { get { return Parent.PK; } }

        public List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat> GetDuplicatedLenFreq()
        {
            var thisList = new List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat>();

            if (_lenFreqRepeat != null && _lenFreqRepeat.Count > 1)
            {
                foreach (var item in _lenFreqRepeat
                    .GroupBy(t => t.LengthClass)
                    .Select
                        (
                            group => new
                            {
                                lenFreqObjects = group.ToList(),
                                lenClassCount = group.Count()
                            }
                        )
                    )
                {
                    if (item.lenClassCount > 1)
                    {
                        foreach (var lfItem in item.lenFreqObjects)
                        {
                            thisList.Add(lfItem);
                        }
                    }

                }
            }

            return thisList;
        }

        [JsonProperty("catch_comp_group/catch_composition_repeat/len_wt_repeat")]
        public List<CatchCompGroupCatchCompositionRepeatLenWtRepeat> LenWtRepeat
        {
            get { return _lenWtRepeat; }
            set
            {
                _lenWtRepeat = value;
                foreach (var item in _lenWtRepeat)
                {
                    item.Parent = this;
                }
            }
        }
        [JsonProperty("catch_comp_group/catch_composition_repeat/length_freq_repeat")]
        public List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat> LenFreqRepeat
        {
            get { return _lenFreqRepeat; }
            set
            {
                _lenFreqRepeat = value;
                foreach (var item in _lenFreqRepeat)
                {
                    item.Parent = this;
                }
            }
        }
        [JsonProperty("catch_comp_group/catch_composition_repeat/length_list_repeat")]
        public List<CatchCompGroupCatchCompositionRepeatLengthListRepeat> LengthListRepeat
        {
            get { return _lengthListRepeat; }
            set
            {
                _lengthListRepeat = value;
                foreach (var item in _lengthListRepeat)
                {
                    item.Parent = this;
                }
            }
        }
        [JsonProperty("catch_comp_group/catch_composition_repeat/gms_repeat_group")]
        public List<CatchCompGroupCatchCompositionRepeatGmsRepeatGroup> GMSRepeat
        {

            get { return _gmsRepeat; }
            set
            {
                _gmsRepeat = value;
                foreach (var item in _gmsRepeat)
                {
                    item.Parent = this;
                }
            }
        }

        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/taxa")]
        public string TaxaCode { get; set; }

        public Taxa Taxa { get { return NSAPEntities.TaxaViewModel.GetTaxa(TaxaCode); } }

        public string TaxaName { get { return Taxa.ToString(); } }


        //this is the combined name from fish species, invert species or other name
        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/species_name_selected")]
        public string SpeciesNameSelected
        {
            get
            {
                return _speciesNameSelected.Replace("0 <span style=\"color:blue\">(", "").Replace(")</span>", "");
                //return _speciesNameSelected; 
            }
            set { _speciesNameSelected = value; }
        }

        //alias of SpeciesNameSelected
        public string SpeciesName { get { return SpeciesNameSelected; } }


        //this is the invert species ID
        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/species_notfish")]
        public int? SpeciesNotFish { get; set; }

        //this is the combined species id from fish or invert
        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/sp_id")]
        public int? SpeciesID { get; set; }

        //this is fish species ID
        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/species")]
        public int? Species { get; set; }

        //this is species name other
        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/spName_other")]
        public string SpeciesNameOther { get; set; }

        //sampled weight of the species
        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/species_sample_wt")]
        public double? SpeciesSampleWt { get; set; }

        //weight of the species
        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/species_wt")]
        public double? SpeciesWt { get; set; }

        //alias of SpeciesWt
        public double? WeightOfCatch { get { return SpeciesWt; } }

        //alias of SpeciesSampleWt
        public double? SampleWeightOfCatch { get { return SpeciesSampleWt; } }
        public static bool RowIDSet { get; private set; }
        public static void SetRowIDs()
        {

            if (NSAPEntities.VesselCatchViewModel.VesselCatchCollection.Count == 0)
            {
                _pk = 0;
            }
            else
            {
                _pk = NSAPEntities.VesselCatchViewModel.NextRecordNumber - 1;
            }
            RowIDSet = true;
        }

        public int PK
        {
            get
            {
                if (Parent.SavedInLocalDatabase)
                {
                    var this_catch = NSAPEntities.VesselCatchViewModel.getVesselCatch(Parent, SpeciesID, SpeciesNameOther);
                    if (this_catch != null)
                    {
                        _rowID = NSAPEntities.VesselCatchViewModel.getVesselCatch(Parent, SpeciesID, SpeciesNameOther).PK;
                    }
                    else
                    {
                        Utilities.Logger.Log($"IMPORTANT: Catch composition item not found in repository" +
                            $"\r\n Vessel unload PK is {Parent.PK}" +
                            $"\r\n Vessel name is {Parent.FishingVesselName}" +
                            $"\r\n Catch name is {SpeciesNameSelected}" +
                            $"\r\n Vessel unload is saved is {Parent.SavedInLocalDatabase}");

                        Utilities.VesselLandingFixDownload.VesselLandingToRepair(Parent);

                    }
                }
                else
                {

                    if (RowIDSet && _rowID == 0)
                    {

                        _rowID = ++_pk;
                    }

                }
                return _rowID;

            }
        }
    }

    public class ValidationStatus
    {
    }
    public class SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat
    {
        private static int _pk;
        private int _rowID;
        public VesselLanding Parent { get; set; }

        public int ParentID { get { return Parent.PK; } }

        [JsonProperty("soak_time_group/soaktime_tracking_group/soak_time_repeat/wpt_set")]
        public string WaypointAtSet { get; set; }
        [JsonProperty("soak_time_group/soaktime_tracking_group/soak_time_repeat/wpt_haul")]
        public string WaypointAtHaul { get; set; }
        [JsonProperty("soak_time_group/soaktime_tracking_group/soak_time_repeat/haul_time")]
        public DateTime HaulTime { get; set; }
        [JsonProperty("soak_time_group/soaktime_tracking_group/soak_time_repeat/set_time")]
        public DateTime SetTime { get; set; }

        public static bool RowIDSet { get; private set; }


        public static void SetRowIDs()
        {
            if (NSAPEntities.GearSoakViewModel.GearSoakCollection.Count == 0)
            {
                _pk = 0;
            }
            else
            {
                _pk = NSAPEntities.GearSoakViewModel.NextRecordNumber - 1;
            }
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }

            }
        }


    }
    public class GridCoordGroupBingoRepeat
    {
        private static int _pk;
        private VesselLanding _parent;
        private UTMZone _utmZone;
        private int _rowID;
        public VesselLanding Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                if (_parent.UTMZone.Length > 0)
                {
                    _utmZone = new UTMZone(_parent.UTMZone);
                }
            }
        }
        public int ParentID { get { return Parent.PK; } }


        [JsonProperty("grid_coord_group/bingo_repeat/bingo_group/col_name")]
        public string Column { get; set; }
        [JsonProperty("grid_coord_group/bingo_repeat/bingo_group/bingo_complete")]
        public string CompleteGridName { get; set; }
        [JsonProperty("grid_coord_group/bingo_repeat/bingo_group/major_grid")]
        public int MajorGrid { get; set; }
        [JsonProperty("grid_coord_group/bingo_repeat/bingo_group/row_name")]
        public int Row { get; set; }
        public string LongLat { get { return $"{Grid25Cell.Coordinate.Longitude.ToString("N6")}, {Grid25Cell.Coordinate.Latitude.ToString("N6")}"; } }

        public string UTMCoordinate { get { return Grid25Cell.UTMCoordinate.ToString(); } }
        public Grid25GridCell Grid25Cell
        {
            get
            {
                return new Grid25GridCell(_utmZone, CompleteGridName);
            }
        }
        public static bool RowIDSet { get; private set; }
        public static void SetRowIDs()
        {
            if (NSAPEntities.FishingGroundGridViewModel.FishingGroundGridCollection.Count == 0)
            {
                _pk = 0;
            }
            else
            {
                _pk = NSAPEntities.FishingGroundGridViewModel.NextRecordNumber - 1;
            }
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }

            }
        }
    }

    public class EffortsGroupEffortRepeat
    {
        private static int _pk;
        private int _rowID;
        public VesselLanding Parent { get; set; }
        public int ParentID { get { return Parent.PK; } }
        [JsonProperty("efforts_group/effort_repeat/group_effort/selected_effort_measure")]
        public string SelectedEffortMeasure { get; set; }

        public string EffortSpecification { get { return SelectedEffortMeasure; } }
        [JsonProperty("efforts_group/effort_repeat/group_effort/response_type")]
        public string ResponseType { get; set; }
        [JsonProperty("efforts_group/effort_repeat/group_effort/effort_spec_name")]
        public string EffortSpecName { get; set; }
        [JsonProperty("efforts_group/effort_repeat/group_effort/effort_intensity")]
        public double? EffortIntensity { get; set; }
        [JsonProperty("efforts_group/effort_repeat/group_effort/effort_type")]
        public int EffortType { get; set; }
        [JsonProperty("efforts_group/effort_repeat/group_effort/effort_desc")]
        public string EffortDescription { get; set; }
        [JsonProperty("efforts_group/effort_repeat/group_effort/effort_bool")]
        public string EffortBoolYesNo { get; set; }

        public bool EffortBool { get { return EffortBoolYesNo == "yes"; } }

        public double? EffortNumericValue { get { return EffortIntensity; } }

        public string EffortTextValue { get { return EffortDescription; } }
        public bool? EffortBooleanValue
        {
            get
            {
                if (EffortBoolYesNo != null && EffortBoolYesNo.Length > 0)
                {
                    return EffortBool;
                }
                else
                {
                    return null;
                }
            }
        }

        public static bool RowIDSet { get; private set; }
        public static void SetRowIDs()
        {
            if (NSAPEntities.VesselEffortViewModel.VesselEffortCollection.Count == 0)
            {
                _pk = 0;
            }
            else
            {
                _pk = NSAPEntities.VesselEffortViewModel.NextRecordNumber - 1;
            }
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }

            }
        }
    }
    public class VesselLandingFlattened
    {
        public VesselLandingFlattened(VesselLanding vl)
        {
            ID = vl.PK;
            Start = vl.start;
            Device_id = vl.device_id;
            User_name = vl.user_name;
            //Email = vl.em;
            Version = vl.intronote;
            SamplingDate = vl.SamplingDate;
            NSAPRegion = vl.NSAPRegion.Name;
            Enumerator = vl.EnumeratorName;
            FMA = vl.FMA == null ? "" : vl.FMA.ToString();
            FishingGround = vl.FishingGround.Name;
            LandingSite = vl.LandingSiteName;
            Gear = vl.GearName;
            Sector = vl.Sector;
            IsBoatUsed = vl.IsBoatUsed;
            Vessel = vl.FishingVesselName;
            SuccessOperation = vl.TripIsSuccess;
            CatchTotalWt = vl.CatchTotalWt;
            CatchSampleWt = vl.CatchSampleWt;
            BoxesTotal = vl.BoxesTotal;
            BoxesSampled = vl.BoxesSampled;
            RasingFactor = vl.RaisingFactor;
            Remarks = vl.Remarks;
            IncludeTracking = vl.IncludeTracking;
            UTMZone = vl.UTMZone;
            DepartureLandingSite = vl.TimeDepartLandingSite;
            ArrivalLandingSite = vl.TimeArriveLandingSite;
            GPS = vl.GPS == null ? "" : vl.GPS.AssignedName;
            _version = vl.__version__;
            _metaID = vl.Meta_InstanceID;
            _id = vl._id;
            _uuid = vl._uuid;
            SubmissionTime = vl._submission_time;
            SavedToDatabase = vl.SavedInLocalDatabase;
        }
        public int ID { get; set; }
        public DateTime Start { get; set; }
        public string Device_id { get; set; }
        public string User_name { get; set; }
        public string Email { get; set; }
        public string Version { get; set; }
        public DateTime SamplingDate { get; set; }
        public string NSAPRegion { get; set; }
        public string Enumerator { get; set; }
        public string FMA { get; set; }
        public string FishingGround { get; set; }
        public string LandingSite { get; set; }
        public string Gear { get; set; }
        public string Sector { get; set; }

        public bool IsBoatUsed { get; set; }
        public string Vessel { get; set; }
        public bool SuccessOperation { get; set; }
        public double? CatchTotalWt { get; set; }
        public double? CatchSampleWt { get; set; }

        public int? BoxesTotal { get; set; }
        public int? BoxesSampled { get; set; }

        public double? RasingFactor { get; set; }
        public string Remarks { get; set; }
        public bool IncludeTracking { get; set; }
        public string UTMZone { get; set; }
        public DateTime? DepartureLandingSite { get; set; }
        public DateTime? ArrivalLandingSite { get; set; }
        public string GPS { get; set; }
        public string _version { get; set; }
        public string _metaID { get; set; }
        public int _id { get; set; }

        public string _uuid { get; set; }

        public DateTime SubmissionTime { get; set; }
        public bool SavedToDatabase { get; set; }
    }

    public class VesselLanding
    {
        private string _gps2;
        private static int _pk;
        private int _rowid;
        private bool? _isSaved;
        private string _includeTrackingYesNo;
        private string _tripIsSuccessYesNo;
        private List<GridCoordGroupBingoRepeat> _gridCoordinates;
        private List<EffortsGroupEffortRepeat> _effortSpecs;
        private List<SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat> _gearSoakTimes;
        private List<CatchCompGroupCatchCompositionRepeat> _catchComps;

        public List<CatchCompGroupCatchCompositionRepeat> GetDuplicatedCatchComposition()
        {
            var thisList = new List<CatchCompGroupCatchCompositionRepeat>();
            if (_catchComps != null && _catchComps.Count > 1)
            {
                foreach (var item in _catchComps
                    .GroupBy(t => t.SpeciesName)
                    .Select
                        (
                            group => new
                            {
                                catchCompObjects = group.ToList(),
                                catchCompCount = group.Count()
                            }
                        )
                    )
                {
                    if (item.catchCompCount > 1)
                    {
                        foreach (var catchItem in item.catchCompObjects)
                        {
                            thisList.Add(catchItem);
                        }
                    }

                }
            }
            return thisList;
        }

        public List<EffortsGroupEffortRepeat> GetDuplicatedEffortSpecs()
        {
            var thisList = new List<EffortsGroupEffortRepeat>();
            if (_effortSpecs != null && _effortSpecs.Count > 1)
            {
                foreach (var item in _effortSpecs
                    .GroupBy(t => t.EffortType)
                    .Select
                        (
                            group => new
                            {
                                effortObjects = group.ToList(),
                                effortSpecCount = group.Count()
                            }
                        )
                    )
                {
                    if (item.effortSpecCount > 1)
                    {
                        foreach (var effortItem in item.effortObjects)
                        {
                            thisList.Add(effortItem);
                        }
                    }

                }
            }
            return thisList;
        }

        [JsonProperty("catch_comp_group/catch_composition_repeat")]
        public List<CatchCompGroupCatchCompositionRepeat> CatchComposition
        {
            get { return _catchComps; }
            set
            {
                _catchComps = value;
                foreach (CatchCompGroupCatchCompositionRepeat item in _catchComps)
                {
                    item.Parent = this;
                }
            }
        }
        [JsonProperty("grid_coord_group/bingo_repeat")]
        public List<GridCoordGroupBingoRepeat> GridCoordinates
        {
            get { return _gridCoordinates; }
            set
            {
                _gridCoordinates = value;
                foreach (GridCoordGroupBingoRepeat item in _gridCoordinates)
                {
                    item.Parent = this;
                }
            }
        }
        [JsonProperty("efforts_group/effort_repeat")]
        public List<EffortsGroupEffortRepeat> GearEffortSpecs
        {
            get { return _effortSpecs; }
            set
            {
                _effortSpecs = value;
                foreach (EffortsGroupEffortRepeat effort in _effortSpecs)
                {
                    effort.Parent = this;
                }
            }
        }
        [JsonProperty("soak_time_group/soaktime_tracking_group/soak_time_repeat")]
        public List<SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat> GearSoakTimes
        {
            get { return _gearSoakTimes; }
            set
            {
                _gearSoakTimes = value;
                foreach (SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat item in _gearSoakTimes)
                {
                    item.Parent = this;
                }
            }
        }

        [JsonProperty("vessel_sampling/gps2")]
        public string GPS2
        {
            get { return _gps2; }
            set
            {
                _gps2 = value;
                if (_gps2.Length > 0)
                {
                    IncludeTracking = true;
                }
            }
        }

        [JsonProperty("vessel_sampling/time_depart_landingsite")]
        public DateTime? DateTimeDepartLandingSite { get; set; }

        [JsonProperty("vessel_sampling/time_arrive_landingsite")]
        public DateTime? DateTimeArriveLandingSite { get; set; }

        [JsonProperty("vessel_sampling/sampling_date")]
        public DateTime SamplingDate { get; set; }
        [JsonProperty("vessel_sampling/nsap_region")]
        public string NSAPRegionCode { get; set; }

        public NSAPRegion NSAPRegion
        {
            get { return NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(NSAPRegionCode); }
            set { NSAPRegionCode = value.Code; }
        }
        [JsonProperty("vessel_sampling/region_enumerator")]
        public int? RegionEnumeratorID { get; set; }
        [JsonProperty("vessel_sampling/region_enumerator_text")]
        public string EnumeratorText { get; set; }

        public NSAPEnumerator NSAPEnumerator
        {
            get
            {
                if (RegionEnumeratorID != null)
                {
                    var regionEnumerator = (NSAPEntities.NSAPEnumeratorViewModel.GetNSAPEnumerator((int)RegionEnumeratorID));
                    if (regionEnumerator != null && NSAPRegion.NSAPEnumerators.Count > 0 && RegionEnumeratorID != null)
                    {
                        var enumerator = NSAPRegion.NSAPEnumerators.FirstOrDefault(t => t.RowID == (int)RegionEnumeratorID).Enumerator;
                        return enumerator;
                    }
                    else
                    {
                        if (regionEnumerator == null)
                        {
                            NSAP_ODK.Utilities.Logger.Log($"Query for NSAPEnumerator with ID {RegionEnumeratorID} returned null");
                        }
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }


        public string EnumeratorName
        {
            get
            {
                if (RegionEnumeratorID != null)
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
                else
                {
                    return EnumeratorText;
                }
            }
        }
        [JsonProperty("vessel_sampling/fma_in_region")]
        public int FMAInRegionID { get; set; }

        public FMA FMA
        {
            get { return NSAPEntities.FMAViewModel.GetFMA(fma_number); }
        }

        public NSAPRegionFMA NSAPRegionFMA
        {
            get
            {
                return NSAPRegion.FMAs.Where(t => t.RowID == FMAInRegionID).FirstOrDefault();
            }
        }
        [JsonProperty("vessel_sampling/fishing_ground")]
        public int RegionFishingGroundID { get; set; }
        public FishingGround FishingGround { get { return RegionFishingGround.FishingGround; } }

        public NSAPRegionFMAFishingGround RegionFishingGround
        {
            get
            {
                return NSAPRegionFMA.FishingGrounds.FirstOrDefault(t => t.RowID == RegionFishingGroundID);
            }
        }

        [JsonProperty("vessel_sampling/landing_site")]
        public int? LandingSiteID { get; set; }
        [JsonProperty("vessel_sampling/landing_site_text")]
        public string LandingSiteText { get; set; }

        public LandingSite LandingSite
        {
            get
            {
                if (LandingSiteID != null)
                {
                    var ls = RegionFishingGround.LandingSites.FirstOrDefault(t => t.RowID == (int)LandingSiteID)?.LandingSite;
                    if (ls != null)
                    {
                        return ls;
                    }
                    else
                    {
                        return null;
                    }
                    //return RegionFishingGround.LandingSites.FirstOrDefault(t => t.RowID == (int)LandingSiteID).LandingSite;
                }
                else
                {
                    return null;
                }
            }
        }
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
                    return LandingSite.ToString();
                }
            }
        }
        [JsonProperty("vessel_sampling/gear_used")]
        public int? GearUsed { get; set; }

        public Gear Gear { get { return NSAPEntities.GearViewModel.GetGear(GearCode); } }
        [JsonProperty("vessel_sampling/gear_used_text")]
        public string GearUsedText { get; set; }



        public string length_type { get; set; }
        [JsonProperty("vessel_catch/trip_isSuccess")]
        public string TripIsSuccessYesNo
        {
            get { return _tripIsSuccessYesNo; }
            set
            {
                _tripIsSuccessYesNo = value;
                TripIsSuccess = _tripIsSuccessYesNo == "yes";
            }
        }
        public bool TripIsSuccess { get; set; }

        [JsonProperty("vessel_catch/catch_total")]
        public double? CatchTotalWt { get; set; }
        [JsonProperty("vessel_catch/catch_sampled")]
        public double? CatchSampleWt { get; set; }
        [JsonProperty("vessel_catch/remarks")]
        public string Remarks { get; set; }
        [JsonProperty("vessel_catch/boxes_sampled")]
        public int? BoxesSampled { get; set; }
        [JsonProperty("vessel_catch/boxes_total")]
        public int? BoxesTotal { get; set; }
        [JsonProperty("vessel_catch/raising_factor")]
        public double? RaisingFactor { get; set; }



        [JsonProperty("soak_time_group/include_tracking")]
        public string IncludeTrackingYesNo
        {
            get { return _includeTrackingYesNo; }
            set
            {
                _includeTrackingYesNo = value;
                IncludeTracking = _includeTrackingYesNo == "yes";
            }
        }

        public bool IncludeTracking { get; set; }

        [JsonProperty("soak_time_group/soaktime_tracking_group/gps")]
        public string GPSCode { get; set; }
        public GPS GPS
        {
            get
            {
                if (GPS2 != null)
                {
                    return NSAPEntities.GPSViewModel.GetGPS(GPS2);
                }
                else
                {
                    return NSAPEntities.GPSViewModel.GetGPS(GPSCode);
                }
            }
        }
        [JsonProperty("soak_time_group/soaktime_tracking_group/time_depart_landingsite")]
        public DateTime? TimeDepartLandingSite { get; set; }
        [JsonProperty("soak_time_group/soaktime_tracking_group/time_arrive_landingsite")]
        public DateTime? TimeArriveLandingSite { get; set; }





        [JsonProperty("fishing_vessel_group/fish_sector")]
        public string SectorCode { get; set; }

        public string Sector
        {
            get
            {
                return SectorCode == "m" ? "Municipal" :
                       SectorCode == "c" ? "Commercial" : "";
            }
        }
        [JsonProperty("fishing_vessel_group/boat_used")]
        public int? BoatUsed { get; set; }
        [JsonProperty("fishing_vessel_group/boat_used_text")]
        public string BoatUsedText { get; set; }
        [JsonProperty("fishing_vessel_group/gear_code")]
        public string GearCode { get; set; }
        [JsonProperty("fishing_vessel_group/gear_name")]
        public string GearName { get; set; }
        [JsonProperty("fishing_vessel_group/is_boat_used")]
        public string IsBoatUsedYesNo { get; set; }
        [JsonProperty("catch_comp_group/include_catchcomp")]
        public string IncludeCatchComposition { get; set; }
        public bool IsBoatUsed
        {
            get
            {
                return IsBoatUsedYesNo == null ? BoatUsed != null || (BoatUsedText != null && BoatUsedText.Length > 0) : IsBoatUsedYesNo == "yes";
            }
            set
            {
                IsBoatUsedYesNo = value ? "yes" : "no";
            }
        }
        public string FishingVesselName
        {
            get
            {
                if (BoatUsed == null)
                {
                    return BoatUsedText;
                }
                else
                {
                    var boatUsed = NSAPEntities.FishingVesselViewModel.GetFishingVessel((int)BoatUsed);
                    if (boatUsed == null)
                    {
                        return "";
                    }
                    else
                    {

                        return boatUsed.ToString();
                    }
                }
            }
        }




        [JsonProperty("grid_coord_group/utmZone")]
        public string UTMZone { get; set; }





        public ValidationStatus _validation_status { get; set; }
        public string __version__ { get; set; }
        public List<object> _attachments { get; set; }
        public string user_name { get; set; }
        public List<object> _geolocation { get; set; }
        public string _submitted_by { get; set; }
        public string _uuid { get; set; }

        public DateTime _submission_time { get; set; }
        [JsonProperty("formhub/uuid")]
        public string formhub_uuid { get; set; }
        public int _id { get; set; }
        public string device_id { get; set; }
        public List<object> _notes { get; set; }
        public DateTime today { get; set; }
        public string intronote { get; set; }
        public string _bamboo_dataset_id { get; set; }
        public List<object> _tags { get; set; }
        public string _xform_id_string { get; set; }
        [JsonProperty("meta/instanceID")]
        public string Meta_InstanceID { get; set; }
        public int fma_number { get; set; }
        public DateTime start { get; set; }
        public string _status { get; set; }


        public static bool RowIDSet { get; private set; }
        public static void SetRowIDs()
        {
            if (NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection.Count == 0)
            {
                _pk = 0;
            }
            else
            {
                _pk = NSAPEntities.VesselUnloadViewModel.NextRecordNumber - 1;
            }
            RowIDSet = true;
        }

        private VesselUnload _savedVesselUnloadObject;

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
                    if (_savedVesselUnloadObject == null)
                    {
                        _savedVesselUnloadObject = SavedVesselUnloadObject;
                    }


                    _rowid = _savedVesselUnloadObject.PK;

                }
                return _rowid;
            }
        }

        private VesselUnload SavedVesselUnloadObject
        {
            get
            {
                _savedVesselUnloadObject = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection.FirstOrDefault(t => t.ODKRowID == _uuid);
                return _savedVesselUnloadObject;
            }

        }
        public bool SavedInLocalDatabase
        {
            get
            {
                if (_isSaved == null)
                {
                    _isSaved = SavedVesselUnloadObject != null;
                }
                return (bool)_isSaved;
            }
            set { _isSaved = value; }

        }

    }



    public static class VesselUnloadServerRepository
    {
        private static List<GridCoordGroupBingoRepeat> _listGridBingoCoordinates;
        private static List<EffortsGroupEffortRepeat> _listGearEfforts;
        private static List<SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat> _listGearSoakTimes;
        private static List<CatchCompGroupCatchCompositionRepeat> _listCatchComps;
        private static List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat> _listLenFreqs;
        private static List<CatchCompGroupCatchCompositionRepeatGmsRepeatGroup> _listGMS;
        private static List<CatchCompGroupCatchCompositionRepeatLenWtRepeat> _listLenWts;
        private static List<CatchCompGroupCatchCompositionRepeatLengthListRepeat> _listLengths;

        public static string JSON { get; set; }
        public static void CreateLandingsFromJSON()
        {
            VesselLanding.SetRowIDs();
            VesselLandings = JsonConvert.DeserializeObject<List<VesselLanding>>(JSON);
        }
        public static DateTime DownloadedLandingsEarliestLandingDate()
        {
            return VesselLandings.OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate;
        }

        public static DateTime DownloadedLandingsLatestLandingDate()
        {
            return VesselLandings.OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate;
        }

        public static int DownloadedLandingsCount()
        {
            return VesselLandings.Count;
        }
        public static List<VesselLanding> VesselLandings { get; internal set; }

        public static event EventHandler<UploadToDbEventArg> UploadSubmissionToDB;

        public static void ResetLists()
        {
            _listGridBingoCoordinates = null;
            _listGearEfforts = null;
            _listGearSoakTimes = null;
            _listCatchComps = null;
            _listLenFreqs = null;
            _listGMS = null;
            _listLenWts = null;
            _listLengths = null;

            DuplicatedLenFreq = new List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat>();
            DuplicatedCatchComposition = new List<CatchCompGroupCatchCompositionRepeat>();
            DuplicatedEffortSpec = new List<EffortsGroupEffortRepeat>();

        }

        public static void FillDuplicatedLists()
        {
            foreach (var item in VesselLandings)
            {
                if (item.CatchComposition != null && item.CatchComposition.Count > 1)
                {
                    var ccd = item.GetDuplicatedCatchComposition();
                    if (ccd.Count > 1)
                    {
                        DuplicatedCatchComposition.AddRange(ccd);
                    }

                    foreach (var catchItem in item.CatchComposition)
                    {
                        var duplicatedLFItems = catchItem.GetDuplicatedLenFreq();
                        if (duplicatedLFItems.Count > 1)
                        {
                            DuplicatedLenFreq.AddRange(duplicatedLFItems);
                        }
                    }
                }

                if (item.GearEffortSpecs != null && item.GearEffortSpecs.Count > 1)
                {
                    var esd = item.GetDuplicatedEffortSpecs();
                    if (esd.Count > 1)
                    {
                        DuplicatedEffortSpec.AddRange(esd);
                    }
                }


            }
        }

        public static List<CatchCompGroupCatchCompositionRepeat> DuplicatedCatchComposition { get; private set; }
        public static List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat> DuplicatedLenFreq { get; private set; }
        public static List<EffortsGroupEffortRepeat> DuplicatedEffortSpec { get; private set; }


        public static List<CatchCompGroupCatchCompositionRepeatLengthListRepeat> GetLengthList()
        {
            List<CatchCompGroupCatchCompositionRepeatLengthListRepeat> thisList = new List<CatchCompGroupCatchCompositionRepeatLengthListRepeat>();
            if (_listLengths == null)
            {
                CatchCompGroupCatchCompositionRepeatLengthListRepeat.SetRowIDs();
                foreach (var landing in VesselLandings)
                {
                    if (landing.CatchComposition != null)
                    {
                        foreach (var cc in landing.CatchComposition)
                        {
                            if (cc.LengthListRepeat != null)
                            {
                                foreach (var cc_len in cc.LengthListRepeat)
                                {
                                    thisList.Add(cc_len);
                                }
                            }
                        }
                    }
                }
                _listLengths = thisList;
            }
            return _listLengths;
        }
        public static List<CatchCompGroupCatchCompositionRepeatLenWtRepeat> GetLenWtList()
        {
            List<CatchCompGroupCatchCompositionRepeatLenWtRepeat> thisList = new List<CatchCompGroupCatchCompositionRepeatLenWtRepeat>();
            if (_listLenWts == null)
            {
                CatchCompGroupCatchCompositionRepeatLenWtRepeat.SetRowIDs();
                foreach (var landing in VesselLandings)
                {
                    if (landing.CatchComposition != null)
                    {
                        foreach (var cc in landing.CatchComposition)
                        {
                            if (cc.LenWtRepeat != null)
                            {
                                foreach (var cc_lw in cc.LenWtRepeat)
                                {
                                    thisList.Add(cc_lw);
                                }
                            }
                        }
                    }
                }
                _listLenWts = thisList;
            }
            return _listLenWts;
        }
        public static List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat> GetLenFreqList()
        {
            List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat> thisList = new List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat>();
            if (_listLenFreqs == null)
            {
                CatchCompGroupCatchCompositionRepeatLengthFreqRepeat.SetRowIDs();
                foreach (var landing in VesselLandings)
                {
                    if (landing.CatchComposition != null)
                    {
                        foreach (var cc in landing.CatchComposition)
                        {
                            if (cc.LenFreqRepeat != null)
                            {
                                foreach (var cc_lf in cc.LenFreqRepeat)
                                {
                                    thisList.Add(cc_lf);
                                }
                            }
                        }
                    }
                }
                _listLenFreqs = thisList;
            }
            return _listLenFreqs;
        }
        public static List<CatchCompGroupCatchCompositionRepeatGmsRepeatGroup> GetGMSList()
        {
            List<CatchCompGroupCatchCompositionRepeatGmsRepeatGroup> thisList = new List<CatchCompGroupCatchCompositionRepeatGmsRepeatGroup>();
            if (_listGMS == null)
            {
                CatchCompGroupCatchCompositionRepeatGmsRepeatGroup.SetRowIDs();
                foreach (var landing in VesselLandings)
                {
                    if (landing.CatchComposition != null)
                    {
                        foreach (var cc in landing.CatchComposition)
                        {
                            if (cc.GMSRepeat != null)
                            {
                                foreach (var cc_gms in cc.GMSRepeat)
                                {
                                    thisList.Add(cc_gms);
                                }
                            }
                        }
                    }
                }
                _listGMS = thisList;
            }
            return _listGMS;
        }
        public static List<GridCoordGroupBingoRepeat> GetGridBingoCoordinates()
        {
            List<GridCoordGroupBingoRepeat> thisList = new List<GridCoordGroupBingoRepeat>();
            if (_listGridBingoCoordinates == null)
            {
                if (!GridCoordGroupBingoRepeat.RowIDSet)
                {
                    GridCoordGroupBingoRepeat.SetRowIDs();
                }
                foreach (var item in VesselLandings)
                {
                    if (item.GridCoordinates != null)
                    {
                        foreach (var bingoCoord in item.GridCoordinates)
                        {
                            thisList.Add(bingoCoord);
                        }
                    }
                }
                _listGridBingoCoordinates = thisList;
            }
            return _listGridBingoCoordinates;

        }

        public static List<EffortsGroupEffortRepeat> GetGearEfforts()
        {
            List<EffortsGroupEffortRepeat> thisList = new List<EffortsGroupEffortRepeat>();
            if (_listGearEfforts == null)
            {
                if (!EffortsGroupEffortRepeat.RowIDSet)
                {
                    EffortsGroupEffortRepeat.SetRowIDs();
                }
                foreach (var item in VesselLandings)
                {
                    if (item.GearEffortSpecs != null)
                    {
                        foreach (var effort in item.GearEffortSpecs)
                        {
                            thisList.Add(effort);
                        }
                    }
                }
                _listGearEfforts = thisList;
            }

            return _listGearEfforts;

        }

        public static List<SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat> GetGearSoakTimes()
        {
            List<SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat> thisList = new List<SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat>();
            if (_listGearSoakTimes == null)
            {
                if (!SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat.RowIDSet)
                {
                    SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat.SetRowIDs();
                }
                foreach (var item in VesselLandings)
                {
                    if (item.GearSoakTimes != null)
                    {
                        foreach (var soakTime in item.GearSoakTimes)
                        {
                            thisList.Add(soakTime);
                        }
                    }
                }
                _listGearSoakTimes = thisList;
            }

            return _listGearSoakTimes;

        }
        public static List<CatchCompGroupCatchCompositionRepeat> GetCatchCompositions()
        {
            List<CatchCompGroupCatchCompositionRepeat> thisList = new List<CatchCompGroupCatchCompositionRepeat>();
            if (_listCatchComps == null)
            {
                if (!CatchCompGroupCatchCompositionRepeat.RowIDSet)
                {
                    CatchCompGroupCatchCompositionRepeat.SetRowIDs();
                }
                foreach (var item in VesselLandings)
                {
                    if (item.CatchComposition != null)
                    {
                        foreach (var catchComp in item.CatchComposition)
                        {
                            thisList.Add(catchComp);
                        }
                    }
                }
                _listCatchComps = thisList;
            }

            return _listCatchComps;

        }




        public static DateTime? JSONFileCreationTime { get; set; }
        public static Task<bool> UploadToDBAsync()
        {
            return Task.Run(() => UploadToDatabase());
        }
        public static bool UploadToDatabase()
        {
            int savedCount = 0;
            var landings = VesselLandings.Where(t => t.SavedInLocalDatabase == false).ToList();
            if (landings.Count > 0)
            {

                UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { VesselUnloadToSaveCount = landings.Count, Intent = UploadToDBIntent.StartOfUpload });
                foreach (var landing in landings)
                {
                    var landingSiteSampling = NSAPEntities.LandingSiteSamplingViewModel.getLandingSiteSampling(landing);
                    if (landingSiteSampling == null)
                    {
                        landingSiteSampling = new LandingSiteSampling

                        {
                            PK = NSAPEntities.LandingSiteSamplingViewModel.NextRecordNumber,
                            LandingSiteID = landing.LandingSite == null ? null : (int?)landing.LandingSite.LandingSiteID,
                            FishingGroundID = landing.FishingGround.Code,
                            IsSamplingDay = true,
                            SamplingDate = landing.SamplingDate.Date,
                            NSAPRegionID = landing.NSAPRegion.Code,
                            LandingSiteText = landing.LandingSiteText == null ? "" : landing.LandingSiteText,
                            FMAID = landing.NSAPRegionFMA.FMA.FMAID
                        };

                        //if(landingSiteSampling.PK==40)
                        //{

                        //}
                        NSAPEntities.LandingSiteSamplingViewModel.AddRecordToRepo(landingSiteSampling);
                    }

                    GearUnload gu = NSAPEntities.GearUnloadViewModel.getGearUnload(landing);
                    if (gu == null)
                    {
                        if (gu != null && gu.GearUsedText != null && gu.GearUsedText.Length > 0)
                        {

                        }
                        gu = new GearUnload
                        {
                            PK = NSAPEntities.GearUnloadViewModel.NextRecordNumber,
                            LandingSiteSamplingID = landingSiteSampling.PK,
                            //GearID = item.Vessel_sampling__gear_used != null ? item.Gear.Code : null,
                            //GearID = landing.GearUsed != null ? landing.NSAPRegion.Gears.FirstOrDefault(t => t.RowID == (int)landing.GearUsed).Gear.Code : null,
                            GearUsedText = landing.GearUsedText == null ? "" : landing.GearUsedText,
                            Remarks = ""
                        };

                        if (landing.GearUsed == null)
                        {
                            gu.GearID = null;
                        }
                        else
                        {
                            var gear = landing.NSAPRegion.Gears.FirstOrDefault(t => t.RowID == (int)landing.GearUsed);
                            if (gear == null)
                            {
                                gu.GearID = null;
                            }
                            else
                            {
                                gu.GearID = gear.GearCode;
                            }
                        }
                        NSAPEntities.GearUnloadViewModel.AddRecordToRepo(gu);
                    }

                    var gpscode = "";
                    if (landing.GPS2 != null)
                    {
                        gpscode = landing.GPS2;
                    }
                    else
                    {
                        gpscode = landing.GPSCode;
                    }

                    bool withCatchComp;
                    if (landing.IncludeCatchComposition == null)
                    {
                        withCatchComp = false;
                    }
                    else
                    {
                        withCatchComp = landing.IncludeCatchComposition == "yes" ? true : false;
                    }

                    VesselUnload vu = new VesselUnload
                    {
                        PK = landing.PK,
                        GearUnloadID = gu.PK,
                        IsBoatUsed = landing.IsBoatUsed,
                        VesselID = landing.IsBoatUsed == false ? null :
                                    landing.BoatUsed == null ? null : landing.BoatUsed,
                        VesselText = landing.BoatUsedText,
                        SectorCode = landing.SectorCode,
                        WeightOfCatch = landing.CatchTotalWt,
                        WeightOfCatchSample = landing.CatchSampleWt,
                        Boxes = landing.BoxesTotal,
                        BoxesSampled = landing.BoxesSampled,
                        RaisingFactor = landing.RaisingFactor,
                        OperationIsSuccessful = landing.TripIsSuccess,
                        OperationIsTracked = landing.IncludeTracking,
                        DepartureFromLandingSite = landing.DateTimeDepartLandingSite,
                        ArrivalAtLandingSite = landing.DateTimeArriveLandingSite,
                        ODKRowID = landing._uuid,
                        UserName = landing.user_name,
                        DeviceID = landing.device_id,
                        DateTimeSubmitted = landing._submission_time,
                        FormVersion = landing.intronote,
                        GPSCode = gpscode,
                        SamplingDate = landing.SamplingDate,
                        Notes = landing.Remarks,
                        NSAPEnumeratorID = landing.NSAPEnumerator == null ? null : (int?)landing.NSAPEnumerator.ID,
                        EnumeratorText = landing.EnumeratorText,
                        DateAddedToDatabase = DateTime.Now,
                        FromExcelDownload = false,
                        TimeStart = landing.start,
                        HasCatchComposition = withCatchComp
                    };

                    if (JSONFileCreationTime != null)
                    {
                        vu.DateAddedToDatabase = (DateTime)JSONFileCreationTime;
                    }

                    if (NSAPEntities.VesselUnloadViewModel.AddRecordToRepo(vu))
                    {

                        if (landing.GearEffortSpecs != null)
                        {
                            vu.CountEffortIndicators = landing.GearEffortSpecs.Count;
                            if (!EffortsGroupEffortRepeat.RowIDSet)
                            {
                                EffortsGroupEffortRepeat.SetRowIDs();
                            }
                            foreach (var effort in landing.GearEffortSpecs
                                .Where(t => t.Parent.PK == landing.PK))
                            {
                                VesselEffort ve = new VesselEffort
                                {
                                    PK = (int)effort.PK,
                                    VesselUnloadID = vu.PK,
                                    EffortSpecID = effort.EffortType,
                                    EffortValueNumeric = effort.EffortIntensity,
                                    EffortValueText = effort.EffortDescription
                                };
                                NSAPEntities.VesselEffortViewModel.AddRecordToRepo(ve);



                            }
                        }

                        if (landing.GearSoakTimes != null)
                        {
                            vu.CountGearSoak = landing.GearSoakTimes.Count;
                            if (!SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat.RowIDSet)
                            {
                                SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat.SetRowIDs();
                            }
                            foreach (var soak in landing.GearSoakTimes
                                .Where(t => t.Parent.PK == landing.PK))
                            {
                                GearSoak gs = new GearSoak
                                {
                                    PK = (int)soak.PK,
                                    VesselUnloadID = vu.PK,
                                    TimeAtSet = soak.SetTime,
                                    TimeAtHaul = soak.HaulTime,
                                    WaypointAtSet = soak.WaypointAtSet,
                                    WaypointAtHaul = soak.WaypointAtHaul
                                };
                                NSAPEntities.GearSoakViewModel.AddRecordToRepo(gs);



                            }
                        }

                        if (landing.GridCoordinates != null)
                        {
                            vu.CountGrids = landing.GridCoordinates.Count;
                            if (!GridCoordGroupBingoRepeat.RowIDSet)
                            {
                                GridCoordGroupBingoRepeat.SetRowIDs();
                            }
                            foreach (var gr in landing.GridCoordinates
                                 .Where(t => t.Parent.PK == landing.PK))
                            {
                                FishingGroundGrid fgg = new FishingGroundGrid
                                {
                                    PK = (int)gr.PK,
                                    VesselUnloadID = vu.PK,
                                    UTMZoneText = gr.Parent.UTMZone,
                                    Grid = gr.CompleteGridName
                                };
                                NSAPEntities.FishingGroundGridViewModel.AddRecordToRepo(fgg);


                            }
                        }

                        if (landing.CatchComposition != null)
                        {
                            vu.CountCatchCompositionItems = landing.CatchComposition.Count;
                            if (!CatchCompGroupCatchCompositionRepeat.RowIDSet)
                            {
                                CatchCompGroupCatchCompositionRepeat.SetRowIDs();
                            }
                            foreach (var catchComp in landing.CatchComposition
                                .Where(t => t.Parent.PK == landing.PK))
                            {
                                VesselCatch vc = new VesselCatch
                                {
                                    PK = catchComp.PK,
                                    VesselUnloadID = vu.PK,
                                    SpeciesID = catchComp.SpeciesID,
                                    Catch_kg = catchComp.SpeciesWt,
                                    Sample_kg = catchComp.SpeciesSampleWt,
                                    TaxaCode = catchComp.TaxaCode,
                                    SpeciesText = catchComp.SpeciesNameOther
                                };


                                if (NSAPEntities.VesselCatchViewModel.AddRecordToRepo(vc))
                                {

                                    if (catchComp.LenFreqRepeat != null)
                                    {
                                        vu.CountLenFreqRows += catchComp.LenFreqRepeat.Count;
                                        if (!CatchCompGroupCatchCompositionRepeatLengthFreqRepeat.RowIDSet)
                                        {
                                            CatchCompGroupCatchCompositionRepeatLengthFreqRepeat.SetRowIDs();
                                        }
                                        foreach (var lf in catchComp.LenFreqRepeat
                                            .Where(t => t.Parent.PK == catchComp.PK))
                                        {
                                            CatchLenFreq clf = new CatchLenFreq
                                            {
                                                PK = (int)lf.PK,
                                                VesselCatchID = vc.PK,
                                                LengthClass = lf.LengthClass,
                                                Frequency = lf.Frequency
                                            };
                                            if (NSAPEntities.CatchLenFreqViewModel.AddRecordToRepo(clf))
                                            {
                                                vu.CountLenFreqRows++;
                                            }
                                        }
                                    }

                                    if (catchComp.LenWtRepeat != null)
                                    {
                                        vu.CountLenWtRows += catchComp.LenWtRepeat.Count;
                                        if (!CatchCompGroupCatchCompositionRepeatLenWtRepeat.RowIDSet)
                                        {
                                            CatchCompGroupCatchCompositionRepeatLenWtRepeat.SetRowIDs();
                                        }
                                        foreach (var lw in catchComp.LenWtRepeat
                                             .Where(t => t.Parent.PK == catchComp.PK))
                                        {
                                            CatchLengthWeight clw = new CatchLengthWeight
                                            {
                                                PK = (int)lw.PK,
                                                VesselCatchID = vc.PK,
                                                Length = lw.Length,
                                                Weight = lw.Weight
                                            };
                                            NSAPEntities.CatchLengthWeightViewModel.AddRecordToRepo(clw);
                                        }
                                    }

                                    if (catchComp.LengthListRepeat != null)
                                    {
                                        vu.CountLengthRows += catchComp.LengthListRepeat.Count;
                                        if (!CatchCompGroupCatchCompositionRepeatLengthListRepeat.RowIDSet)
                                        {
                                            CatchCompGroupCatchCompositionRepeatLengthListRepeat.SetRowIDs();
                                        }
                                        foreach (var l in catchComp.LengthListRepeat
                                             .Where(t => t.Parent.PK == catchComp.PK))
                                        {
                                            CatchLength cl = new CatchLength
                                            {
                                                PK = (int)l.PK,
                                                VesselCatchID = vc.PK,
                                                Length = l.Length

                                            };
                                            NSAPEntities.CatchLengthViewModel.AddRecordToRepo(cl);
                                        }
                                    }

                                    if (catchComp.GMSRepeat != null)
                                    {
                                        vu.CountMaturityRows += catchComp.GMSRepeat.Count;
                                        if (!CatchCompGroupCatchCompositionRepeatGmsRepeatGroup.RowIDSet)
                                        {
                                            CatchCompGroupCatchCompositionRepeatGmsRepeatGroup.SetRowIDs();
                                        }
                                        foreach (var m in catchComp.GMSRepeat
                                             .Where(t => t.Parent.PK == catchComp.PK))
                                        {
                                            CatchMaturity cm = new CatchMaturity
                                            {
                                                PK = (int)m.PK,
                                                VesselCatchID = vc.PK,
                                                Length = m.Length,
                                                Weight = m.Weight,
                                                SexCode = m.SexCode,
                                                MaturityCode = m.GMSCode,
                                                WeightGutContent = m.StomachContentWt,
                                                GutContentCode = m.GutContentCategoryCode,
                                                GonadWeight = m.GonadWeight
                                            };
                                            if (cm.GonadWeight != null)
                                            {

                                            }
                                            NSAPEntities.CatchMaturityViewModel.AddRecordToRepo(cm);
                                        }
                                    }
                                }
                            }
                        }
                        NSAPEntities.VesselUnloadViewModel.UpdateUnloadStats(vu);
                        savedCount++;
                        landing.SavedInLocalDatabase = true;
                        UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { VesselUnloadSavedCount = savedCount, Intent = UploadToDBIntent.Uploading });
                    }
                }
            }
            UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg
            {
                VesselUnloadTotalSavedCount = savedCount,
                Intent = UploadToDBIntent.EndOfUpload
            });
            return savedCount > 0;
        }

    }
}