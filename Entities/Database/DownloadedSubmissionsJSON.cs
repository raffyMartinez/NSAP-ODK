using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Utilities;
namespace NSAP_ODK.Entities.Database
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class CatchCompGroupCatchCompositionRepeatLengthListRepeat
    {
        public int ParentID { get { return (int)Parent.ID; } }

        public double Length { get { return Catch_comp_group__catch_composition_repeat__length_list_repeat__length; } }
        public double Catch_comp_group__catch_composition_repeat__length_list_repeat__length { get; set; }
        public CatchCompGroupCatchCompositionRepeat Parent { get; set; }
        public int? ID { get; set; }
    }

    public class CatchCompGroupCatchCompositionRepeatLengthFreqRepeat
    {
        public double Length { get { return Catch_comp_group__catch_composition_repeat__length_freq_repeat__group_LF__length_class; } }
        public int Frequency { get { return Catch_comp_group__catch_composition_repeat__length_freq_repeat__group_LF__freq; } }
        public CatchCompGroupCatchCompositionRepeat Parent { get; set; }
        public Double Catch_comp_group__catch_composition_repeat__length_freq_repeat__group_LF__length_class { get; set; }
        public int Catch_comp_group__catch_composition_repeat__length_freq_repeat__group_LF__freq { get; set; }
        public int? ID { get; set; }
        public int ParentID { get { return (int)Parent.ID; } }
    }
    public class CatchCompGroupCatchCompositionRepeatLenWtRepeat
    {
        public int ParentID { get { return (int)Parent.ID; } }
        public CatchCompGroupCatchCompositionRepeat Parent { get; set; }

        public double Length { get { return Catch_comp_group__catch_composition_repeat__len_wt_repeat__len_wt_group__len_lenwt; } }
        public double Weight { get { return Catch_comp_group__catch_composition_repeat__len_wt_repeat__len_wt_group__wt_lenwt; } }
        public double Catch_comp_group__catch_composition_repeat__len_wt_repeat__len_wt_group__wt_lenwt { get; set; }
        public double Catch_comp_group__catch_composition_repeat__len_wt_repeat__len_wt_group__len_lenwt { get; set; }
        public int? ID { get; set; }
    }

    public class CatchCompGroupCatchCompositionRepeatGmsRepeatGroup
    {
        public int ParentID { get { return (int)Parent.ID; } }
        public CatchCompGroupCatchCompositionRepeat Parent { get; set; }
        public double? Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__individual_weight { get; set; }
        public string Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gms_repeat { get; set; }
        public string Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__sex { get; set; }
        public double? Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__individual_length { get; set; }
        public int? ID { get; set; }

        public double? Length { get { return Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__individual_length; } }
        public double? Weight { get { return Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__individual_weight; } }

        public Double? GutContentWeight { get { return Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__stomach_content_wt; } }
        public string Sex
        {
            get
            {
                return Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__sex=="f"?"Female":
                       Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__sex=="m"?"Male":"Juvenile";
            }
        }

        public string MaturityStage
        {
            get
            {
                return Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gms_repeat == "pr" ? "Premature" :
                       Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gms_repeat == "im" ? "Immature" :
                       Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gms_repeat == "de" ? "Developing" :
                       Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gms_repeat == "ma" ? "Maturing" :
                       Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gms_repeat == "dt" ? "Mature" :
                       Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gms_repeat == "ri" ? "Ripening" :
                       Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gms_repeat == "gr" ? "Gravid" :
                       Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gms_repeat == "spw" ? "Spawning" :
                       Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gms_repeat == "sp" ? "Spent": "";

            }
        }

        public string GutContentCategory
        {
            get
            {
                return Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gut_content_category == "F" ? "Full" :
                       Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gut_content_category == "HF" ? "Half full":
                       Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gut_content_category == "E" ? "Empty" : "";

            }
        }
        public string Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gut_content_category { get; set; }
        public double? Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__stomach_content_wt { get; set; }
    }

    public class CatchCompGroupCatchCompositionRepeat
    {
        public JSONMainSheet Parent { get; set; }
        public int ParentID { get { return Parent.ID; } }
        public List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat> Catch_comp_group__catch_composition_repeat__length_freq_repeat { get; set; }
        public List<CatchCompGroupCatchCompositionRepeatGmsRepeatGroup> Catch_comp_group__catch_composition_repeat__gms_repeat_group { get; set; }
        public List<CatchCompGroupCatchCompositionRepeatLenWtRepeat> Catch_comp_group__catch_composition_repeat__len_wt_repeat { get; set; }
        public List<CatchCompGroupCatchCompositionRepeatLengthListRepeat> Catch_comp_group__catch_composition_repeat__length_list_repeat { get; set; }
        public string Catch_comp_group__catch_composition_repeat__speciesname_group__taxa { get; set; }
        public Taxa Taxa
        {
            get { return NSAPEntities.TaxaViewModel.GetTaxa(Catch_comp_group__catch_composition_repeat__speciesname_group__taxa); }
        }

        public string TaxaName
        {
            get { return Taxa.Name; }
        }
        public string SpeciesName
        {
            get
            {
                if(Catch_comp_group__catch_composition_repeat__speciesname_group__spName_other.Length>0)
                {
                    return Catch_comp_group__catch_composition_repeat__speciesname_group__spName_other;
                }
                else
                {
                    if(Catch_comp_group__catch_composition_repeat__speciesname_group__taxa=="FIS")
                    {
                        return NSAPEntities.FishSpeciesViewModel.GetSpecies((int)Catch_comp_group__catch_composition_repeat__speciesname_group__sp_id).ToString();
                    }
                    else
                    {
                        return NSAPEntities.NotFishSpeciesViewModel.GetSpecies((int)Catch_comp_group__catch_composition_repeat__speciesname_group__sp_id).ToString();
                    }

                }

            }
        }
        public double? WeightOfCatch { get { return Catch_comp_group__catch_composition_repeat__speciesname_group__species_wt; } }
        public double? SampleWeightOfCatch { get { return Catch_comp_group__catch_composition_repeat__speciesname_group__species_sample_wt; } }

        public int? ID { get; set; }
        public string Catch_comp_group__catch_composition_repeat__speciesname_group__spName_other { get; set; }
        public double? Catch_comp_group__catch_composition_repeat__speciesname_group__species_wt { get; set; }

        public string Catch_comp_group__catch_composition_repeat__speciesname_group__sum_species_weight_note { get; set; }

        public double? Catch_comp_group__catch_composition_repeat__speciesname_group__species_sample_wt { get; set; }

        public int? Catch_comp_group__catch_composition_repeat__speciesname_group__sp_id { get; set; }
    }

    public class ValidationStatus
    {
    }

    public class SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat
    {
        public int ParentID { get { return Parent.ID; } }
        public DateTime DateTimeSet{ get { return Soak_time_group__soaktime_tracking_group__soak_time_repeat__set_time; } }
        public DateTime DateTimeHaul { get { return Soak_time_group__soaktime_tracking_group__soak_time_repeat__haul_time; } }
        public string WaypointSet { get { return Soak_time_group__soaktime_tracking_group__soak_time_repeat__wpt_set; } }
        public string WaypointHaul { get { return Soak_time_group__soaktime_tracking_group__soak_time_repeat__wpt_haul; } }
        public JSONMainSheet Parent { get; set; }
        public string Soak_time_group__soaktime_tracking_group__soak_time_repeat__wpt_set { get; set; }

        public string Soak_time_group__soaktime_tracking_group__soak_time_repeat__wpt_haul { get; set; }

        public DateTime Soak_time_group__soaktime_tracking_group__soak_time_repeat__haul_time { get; set; }
        public DateTime Soak_time_group__soaktime_tracking_group__soak_time_repeat__set_time { get; set; }
        public int? ID { get; set; }

    }

    public class GridCoordGroupBingoRepeat
    {
        public string Grid_coord_group__bingo_repeat__bingo_group__bingo_complete { get; set; }
        public JSONMainSheet Parent { get; set; }
        public int? ID { get; set; }
        public int ParentID { get { return Parent.ID; } }
        public string FishingGrid { get { return Grid_coord_group__bingo_repeat__bingo_group__bingo_complete; } }

        public UTMZone UTMZone { get; set; }

        public Grid25GridCell GridCell { get; set; }

        public string LongLat 
        { 
            get 
            {
                float x = 0f; float y = 0f;
                GridCell.Coordinate.GetD(out y, out x);
                return $"{x},{y}";
            } 
        }
        public string UTMCoordinate { get { return GridCell.UTMCoordinate.ToString(); } }

    }

    public class EffortsGroupEffortRepeat
    {
        public JSONMainSheet Parent { get; set; }
        public int ParentID { get { return Parent.ID; } }
        public double? EffortNumericValue { get { return Efforts_group__effort_repeat__group_effort__effort_intensity; } }
        public string EffortTextValue { get { return Efforts_group__effort_repeat__group_effort__effort_desc; } }
        public bool? EffortBooleanValue { get { return Efforts_group__effort_repeat__group_effort__effort_bool; } }
        public double? Efforts_group__effort_repeat__group_effort__effort_intensity { get; set; }

        public bool? Efforts_group__effort_repeat__group_effort__effort_bool { get; set; }
        public string Efforts_group__effort_repeat__group_effort__effort_desc { get; set; }
        public int Efforts_group__effort_repeat__group_effort__effort_type { get; set; }

        public int? ID { get; set; }
        public string EffortValueToString
        {
            get
            {
                if (Efforts_group__effort_repeat__group_effort__effort_intensity != null)
                {
                    return ((int)Efforts_group__effort_repeat__group_effort__effort_intensity).ToString();
                }
                else if (Efforts_group__effort_repeat__group_effort__effort_desc.Length > 0)
                {
                    return Efforts_group__effort_repeat__group_effort__effort_desc;
                }
                else
                {
                    return ((bool)Efforts_group__effort_repeat__group_effort__effort_bool).ToString();
                }
            }
        }
        public string EffortSpecification
        {
            get { return NSAPEntities.EffortSpecificationViewModel.GetEffortSpecification(Efforts_group__effort_repeat__group_effort__effort_type).ToString(); }
        }
        //public string efforts_group__effort_repeat__group_effort__selected_effort_measure { get; set; }
        //public string efforts_group__effort_repeat__group_effort__response_type { get; set; }
        //public string efforts_group__effort_repeat__group_effort__effort_spec_name { get; set; }
    }

    public class MainSheetFlattened 
    {
        public MainSheetFlattened(JSONMainSheet mainSheet)
        {
            ID = mainSheet.ID;
            Start = mainSheet.Start;
            Device_id = mainSheet.Device_id;
            User_name = mainSheet.User_name;
            Email = mainSheet.Email;
            Version = mainSheet.Intronote;
            SamplingDate = mainSheet.Vessel_sampling__sampling_date;
            NSAPRegion = mainSheet.NSAPRegion.Name;
            Enumerator = mainSheet.EnumeratorName;
            FMA = mainSheet.FMA_number.ToString();
            FishingGround = mainSheet.FishingGround.Name;
            LandingSite = mainSheet.LandingSiteName;
            Gear = mainSheet.GearName;
            Sector = mainSheet.Sector;
            IsBoatUsed = mainSheet.Fishing_vessel_group__is_boat_used;
            Vessel = mainSheet.FishingVesselName;
            SuccessOperation = mainSheet.Vessel_catch__trip_isSuccess;
            CatchTotalWt = mainSheet.Vessel_catch__catch_total;
            CatchSampleWt = mainSheet.Vessel_catch__catch_sampled;
            BoxesTotal = mainSheet.Vessel_catch__boxes_total;
            BoxesSampled = mainSheet.Vessel_catch__boxes_sampled;
            RasingFactor = mainSheet.Vessel_catch__raising_factor;
            Remarks = mainSheet.Vessel_catch__remarks;
            IncludeTracking = mainSheet.Soak_time_group__include_tracking;
            UTMZone = mainSheet.Grid_coord_group__utmZone;
            DepartureLandingSite = mainSheet.Soak_time_group__soaktime_tracking_group__time_depart_landingsite;
            ArrivalLandingSite = mainSheet.Soak_time_group__soaktime_tracking_group__time_arrive_landingsite;
            GPS = mainSheet.GPS == null ? "" : mainSheet.GPS.AssignedName;
            _version = mainSheet.__version__;
            _metaID = mainSheet.Meta__instanceID;
            _id = mainSheet._id;
            _uuid = mainSheet._uuid;
            SubmissionTime = mainSheet._submission_time;
            SavedToDatabase = mainSheet.IsSavedToDatabase;
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
    public class JSONMainSheet
    {
        private bool _isSavedDummy;
        public int ID { get; set; }
        public bool IsSavedToDatabase
        {
            get
            {
                var item = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection
                    .Where(t => t.ODKRowID == _uuid).FirstOrDefault();
                return item != null;
            }
            set
            { _isSavedDummy = value; }
        }
        public DateTime Start { get; set; }
        public DateTime Today { get; set; }
        public string Device_id { get; set; }
        public string User_name { get; set; }
        public string Email { get; set; }
        public string Intronote { get; set; }
        public DateTime Vessel_sampling__sampling_date { get; set; }
        public string Vessel_sampling__nsap_region { get; set; }
        public NSAPRegion NSAPRegion
        {
            get { return NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(Vessel_sampling__nsap_region); }
        }

        public int? Vessel_sampling__region_enumerator { get; set; }
        public string Vessel_sampling__region_enumerator_text { get; set; }
        public string EnumeratorName
        {
            get
            {
                if (Vessel_sampling__region_enumerator == null)
                {
                    return Vessel_sampling__region_enumerator_text;
                }
                else
                {
                    var enumerator = NSAPRegion.NSAPEnumerators.Where(t => t.RowID == (int)Vessel_sampling__region_enumerator).FirstOrDefault();
                    if(enumerator!=null)
                    {
                        return enumerator.Enumerator.Name;
                    }
                    else
                    {
                        return "";
                    }
                    //return NSAPRegion.NSAPEnumerators.Where(t => t.RowID == (int)Vessel_sampling__region_enumerator).FirstOrDefault().Enumerator.Name;
                }
            }
        }
        public NSAPEnumerator NSAPEnumerator
        {
            get
            {
                if (Vessel_sampling__region_enumerator != null)
                {
                    if (NSAPRegion.NSAPEnumerators.Count > 0)
                    {
                        return NSAPRegion.NSAPEnumerators.FirstOrDefault(t => t.RowID == Vessel_sampling__region_enumerator).Enumerator;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
        public int FMA_number { get; set; }
        public int Vessel_sampling__fma_in_region { get; set; }
        public NSAPRegionFMA NSAPRegionFMA
        {
            get
            {
                return NSAPRegion.FMAs.Where(t => t.RowID == Vessel_sampling__fma_in_region).FirstOrDefault();
            }
        }
        public FishingGround FishingGround
        {
            get
            {
                return NSAPRegionFMA.FishingGrounds.Where(t => t.RowID == Vessel_sampling__fishing_ground).FirstOrDefault().FishingGround;
            }
        }
        public int Vessel_sampling__fishing_ground { get; set; }
        public NSAPRegionFMAFishingGround RegionFishingGround
        {
            get
            {
                return NSAPRegionFMA.FishingGrounds.FirstOrDefault(t => t.RowID == Vessel_sampling__fishing_ground);
            }
        }
        public string LandingSiteName
        {
            get
            {
                if (Vessel_sampling__landing_site == null)
                {
                    return Vessel_sampling__landing_site_text;
                }
                else
                {
                    return NSAPRegionFMA.FishingGrounds
                        .Where(t => t.RowID == Vessel_sampling__fishing_ground).FirstOrDefault().LandingSites
                        .Where(t => t.RowID == (int)Vessel_sampling__landing_site).FirstOrDefault().LandingSite.ToString();
                }
            }
        }
        public string Vessel_sampling__landing_site_text { get; set; }
        public int? Vessel_sampling__landing_site { get; set; }
        public LandingSite LandingSite
        {
            get
            {
                if (Vessel_sampling__landing_site != null)
                {
                    return RegionFishingGround.LandingSites.FirstOrDefault(t => t.RowID == (int)Vessel_sampling__landing_site).LandingSite;
                }
                else
                {
                    return null;
                }
            }
        }
        public int? Vessel_sampling__gear_used { get; set; }
        
        public string Fishing_vessel_group__gear_code { get; set; }
        public string Gear_code { get; set; }

        public string GearName
        {
            get
            {
                if (Gear_code.Length > 0 && Gear_code != "_OT")
                {
                    return NSAPEntities.GearViewModel.GetGear(Gear_code).GearName;
                }
                else if(Gear_code.Length==0 && Vessel_sampling__gear_used != null)
                {
                    return NSAPRegion.Gears.FirstOrDefault(t => t.RowID == Vessel_sampling__gear_used).Gear.GearName;  
                }
                else
                {
                    return Vessel_sampling__gear_used_text;
                }
            }
        }
        public Gear Gear
        {
            get { return NSAPEntities.GearViewModel.GetGear(Gear_code); }
            //get { return NSAPEntities.GearViewModel.GetGear(Fishing_vessel_group__gear_code); }
        }
        public string Fishing_vessel_group__gear_name { get; set; }
        public string Vessel_sampling__gear_used_text { get; set; }
        public string Fishing_vessel_group__fish_sector { get; set; }
        public string Sector
        {
            get 
            {
                return Fishing_vessel_group__fish_sector == "m" ? "Municipal" :
                       Fishing_vessel_group__fish_sector == "c" ? "Commercial" : "";
            }
        }

        public bool Fishing_vessel_group__is_boat_used
        {
            get
            {
                if (Fishing_vessel_group__is_boat_used_text == "yes")
                {
                    return true;
                }
                else if(Fishing_vessel_group__is_boat_used_text=="no")
                {
                    return false;
                }
                else
                {
                    return FishingVesselName.Length > 0;
                }
            }
            set
            {
                Fishing_vessel_group__is_boat_used = value;
            }
        }
        public string Fishing_vessel_group__is_boat_used_text { get; set; }
        public int? Fishing_vessel_group__boat_used { get; set; }
        public string Fishing_vessel_group__boat_used_text { get; set; }

        public string FishingVesselName
        {
            get
            {
                if (Fishing_vessel_group__boat_used == null)
                {
                    return Fishing_vessel_group__boat_used_text;
                }
                else
                {
                    //return NSAPRegion.FishingVessels.FirstOrDefault(t => t.RowID == Fishing_vessel_group__boat_used).FishingVessel.ToString();
                    return NSAPEntities.FishingVesselViewModel.GetFishingVessel((int)Fishing_vessel_group__boat_used).ToString();
                }
            }
        }
        public bool Vessel_catch__trip_isSuccess { get; set; }
        public double? Vessel_catch__catch_total { get; set; }
        public double? Vessel_catch__catch_sampled { get; set; }
        public int? Vessel_catch__boxes_total { get; set; }

        public int? Vessel_catch__boxes_sampled { get; set; }
        public string Vessel_catch__remarks { get; set; }
        public double? Vessel_catch__raising_factor { get; set; }
        public bool Soak_time_group__include_tracking { get; set; }
        public string Soak_time_group__soaktime_tracking_group__gps { get; set; }
        public GPS GPS
        {
            get
            {
                if (Soak_time_group__soaktime_tracking_group__gps.Length > 0)
                {
                    return NSAPEntities.GPSViewModel.GetGPS(Soak_time_group__soaktime_tracking_group__gps);
                }
                else
                {
                    return null;
                }
            }
        }

        public DateTime? Soak_time_group__soaktime_tracking_group__time_depart_landingsite { get; set; }

        public DateTime? Soak_time_group__soaktime_tracking_group__time_arrive_landingsite { get; set; }

        public UTMZone UTMZone { get; set; }
        public string Grid_coord_group__utmZone { get; set; }
        public List<CatchCompGroupCatchCompositionRepeat> Catch_comp_group__catch_composition_repeat { get; set; }
        public List<SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat> Soak_time_group__soaktime_tracking_group__soak_time_repeat { get; set; }
        public List<GridCoordGroupBingoRepeat> Grid_coord_group__bingo_repeat { get; set; }
        public List<EffortsGroupEffortRepeat> Efforts_group__effort_repeat { get; set; }

        public string __version__ { get; set; }
        public string Meta__instanceID { get; set; }
        public int _id { get; set; }
        public List<object> _notes { get; set; }
        public string Formhub__uuid { get; set; }
        public string _uuid { get; set; }
        public DateTime _submission_time { get; set; }
        public string _xform_id_string { get; set; }
        public string _bamboo_dataset_id { get; set; }
        public List<object> _tags { get; set; }

        public string _status { get; set; }

        public int _index { get; set; }
        public ValidationStatus _validation_status { get; set; }

        public List<object> _attachments { get; set; }

        // public string Fishing_vessel_group__search_mode { get; set; }
        //public object _submitted_by { get; set; }
        //public string vessel_sampling__select_enumerator { get; set; }
        //public string catch_comp_group__length_type { get; set; }
        //public string catch_comp_group__has_gms { get; set; }
        //public string fishing_vessel_group__vessel_csv_source { get; set; }
        //public string catch_comp_group__sum_species_weight_coalesce { get; set; }
        //public string efforts_group__include_effort { get; set; }
        //public string catch_comp_group__include_catchcomp { get; set; }
        //public string grid_coord_group__majorgrid_csv { get; set; }
        //public string catch_comp_group__count_catch_comp { get; set; }
        //public string decimal_sampling_date { get; set; }
        //public string catch_comp_group__sum_species_weight { get; set; }
        //public string fishing_vessel_group__select_vessel { get; set; }
        //public string sampling_date_string { get; set; }
        //public string grid_coord_group__include_bingo { get; set; }
        //public string vessel_sampling__select_landingsite { get; set; }
        //public string soak_time_group__include_soak_time { get; set; }
        //public string fishing_vessel_group__search_column { get; set; }
        //public string fishing_vessel_group__search_or_showall { get; set; }
        //public string soak_time_group__soaktime_tracking_group__depart_date_string { get; set; }
        //public string soak_time_group__soaktime_tracking_group__decimal_depart_date { get; set; }
        //public string soak_time_group__soaktime_tracking_group__arrive_date_string { get; set; }
        //public string soak_time_group__soaktime_tracking_group__decimal_arrive_date { get; set; }
        //public string grid_coord_group__inlandgrid_csv { get; set; }
        //public string vessel_sampling__select_gear { get; set; }
        //public List<object> _geolocation { get; set; }
        //public string fishing_vessel_group__search_value { get; set; }
        //public string fishing_vessel_group__gear_name { get; set; }
    }

    //public class Root
    //{
    //    public MyArray MyArray { get; set; }
    //}


    public static class KoboAPI
    {
        private static int _vessel_unload_ID;
        private static int _gear_soak_ID;
        private static int _fishing_grid_ID;
        private static int _effort_spec_ID;
        private static int _catch_comp_ID;
        private static int _length_list_id;
        private static int _len_freq_ID;
        private static int _len_wt_ID;
        private static int _maturity_ID;
        private static List<CatchCompGroupCatchCompositionRepeat> _catchCompositions;


        public static event EventHandler<UploadToDbEventArg> UploadSubmissionToDB;

        private static bool Vessel_unload_saved(string uuid, out int id)
        {
            id = 0;
            var unload = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection.FirstOrDefault(t => t.ODKRowID == uuid);
            if(unload!=null)
            {
                id = unload.PK;
            }
            return unload != null;
        }

        static KoboAPI()
        {
            //_vessel_unload_ID = NSAPEntities.VesselUnloadViewModel.NextRecordNumber-1;
            _catch_comp_ID = NSAPEntities.VesselCatchViewModel.NextRecordNumber;
            _gear_soak_ID = NSAPEntities.GearSoakViewModel.NextRecordNumber;
            _fishing_grid_ID = NSAPEntities.FishingGroundGridViewModel.NextRecordNumber;
            _effort_spec_ID = NSAPEntities.VesselEffortViewModel.NextRecordNumber;
            _length_list_id = NSAPEntities.CatchLengthViewModel.NextRecordNumber;
            _len_freq_ID = NSAPEntities.CatchLenFreqViewModel.NextRecordNumber;
            _len_wt_ID = NSAPEntities.CatchLengthWeightViewModel.NextRecordNumber;
            _maturity_ID = NSAPEntities.CatchMaturityViewModel.NextRecordNumber;
        }

        public static void GetUnloadNextRecord()
        {
            _vessel_unload_ID= NSAPEntities.VesselUnloadViewModel.NextRecordNumber - 1;
        }

        public static Task<bool> UploadToDBAsync(List<JSONMainSheet> mainSheets)
        {
            return Task.Run( ()=>UploadToDB(mainSheets) );  
        }

        private static bool UploadToDB(List<JSONMainSheet> mainSheets)
        {

            SetupEntityIDs(mainSheets);
            int savedCount = 0;
            var sheetsToUpload = mainSheets.Where(t => t.IsSavedToDatabase == false).ToList();
            if (sheetsToUpload.Count > 0)
            {
                UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { VesselUnloadToSaveCount = sheetsToUpload.Count, Intent = UploadToDBIntent.StartOfUpload });

                foreach (var item in sheetsToUpload)
                {
                    var landingSiteSampling = NSAPEntities.LandingSiteSamplingViewModel.getLandingSiteSampling(item);
                    if (landingSiteSampling == null)
                    {
                        landingSiteSampling = new LandingSiteSampling

                        {
                            PK = NSAPEntities.LandingSiteSamplingViewModel.NextRecordNumber,
                            LandingSiteID = item.LandingSite == null ? null : (int?)item.LandingSite.LandingSiteID,
                            FishingGroundID = item.FishingGround.Code,
                            IsSamplingDay = true,
                            SamplingDate = item.Vessel_sampling__sampling_date.Date,
                            NSAPRegionID = item.NSAPRegion.Code,
                            LandingSiteText = item.Vessel_sampling__landing_site_text,
                            FMAID = item.NSAPRegionFMA.FMA.FMAID
                        };
                        NSAPEntities.LandingSiteSamplingViewModel.AddRecordToRepo(landingSiteSampling);
                    }

                    GearUnload gu = NSAPEntities.GearUnloadViewModel.getGearUnload(item);
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
                            GearID = item.Vessel_sampling__gear_used != null ? item.NSAPRegion.Gears.FirstOrDefault(t => t.RowID == (int)item.Vessel_sampling__gear_used).Gear.Code : null,
                            GearUsedText = item.Vessel_sampling__gear_used_text
                        };
                        NSAPEntities.GearUnloadViewModel.AddRecordToRepo(gu);
                    }

                    VesselUnload vu = new VesselUnload
                    {
                        PK = item.ID,
                        GearUnloadID = gu.PK,
                        IsBoatUsed = item.Fishing_vessel_group__is_boat_used,
                        VesselID = item.Fishing_vessel_group__is_boat_used == false ? null :
                                    item.Fishing_vessel_group__boat_used == null ? null : (int?)item.Fishing_vessel_group__boat_used,
                        VesselText = item.Fishing_vessel_group__boat_used_text,
                        SectorCode = item.Fishing_vessel_group__fish_sector,
                        WeightOfCatch = item.Vessel_catch__catch_total,
                        WeightOfCatchSample = item.Vessel_catch__catch_sampled,
                        Boxes = item.Vessel_catch__boxes_total,
                        BoxesSampled = item.Vessel_catch__boxes_sampled,
                        OperationIsSuccessful = item.Vessel_catch__trip_isSuccess,
                        OperationIsTracked = item.Soak_time_group__include_tracking,
                        DepartureFromLandingSite = item.Soak_time_group__soaktime_tracking_group__time_depart_landingsite,
                        ArrivalAtLandingSite = item.Soak_time_group__soaktime_tracking_group__time_arrive_landingsite,
                        ODKRowID = item._uuid,
                        UserName = item.User_name,
                        DeviceID = item.Device_id,
                        DateTimeSubmitted = item._submission_time,
                        FormVersion = item.Intronote,
                        GPSCode = item.Soak_time_group__soaktime_tracking_group__gps,
                        SamplingDate = item.Vessel_sampling__sampling_date,
                        Notes = item.Vessel_catch__remarks,
                        //NSAPRegionEnumeratorID = item.NSAPERegionEnumeratorID == null ? null : (int?)item.NSAPERegionEnumeratorID,
                        NSAPEnumeratorID = item.NSAPEnumerator == null ? null : (int?)item.NSAPEnumerator.ID,
                        EnumeratorText = item.Vessel_sampling__region_enumerator_text,
                        DateAddedToDatabase = DateTime.Now,
                        FromExcelDownload = false,
                        XFormIdentifier = item._xform_id_string

                    };

                    if (NSAPEntities.VesselUnloadViewModel.AddRecordToRepo(vu))
                    {
                        savedCount++;
                        UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { VesselUnloadSavedCount = savedCount, Intent = UploadToDBIntent.Uploading });
                        if (item.Efforts_group__effort_repeat != null)
                        {
                            foreach (var effort in item.Efforts_group__effort_repeat
                                .Where(t => t.Parent.ID == item.ID))
                            {
                                VesselEffort ve = new VesselEffort
                                {
                                    PK = (int)effort.ID,
                                    VesselUnloadID = vu.PK,
                                    EffortSpecID = effort.Efforts_group__effort_repeat__group_effort__effort_type,
                                    EffortValueNumeric = effort.Efforts_group__effort_repeat__group_effort__effort_intensity,
                                    EffortValueText = effort.Efforts_group__effort_repeat__group_effort__effort_desc
                                };
                                NSAPEntities.VesselEffortViewModel.AddRecordToRepo(ve);
                            }
                        }

                        if (item.Soak_time_group__soaktime_tracking_group__soak_time_repeat != null)
                        {
                            foreach (var soak in item.Soak_time_group__soaktime_tracking_group__soak_time_repeat
                                .Where(t => t.Parent.ID == item.ID))
                            {
                                GearSoak gs = new GearSoak
                                {
                                    PK = (int)soak.ID,
                                    VesselUnloadID = vu.PK,
                                    TimeAtSet = soak.Soak_time_group__soaktime_tracking_group__soak_time_repeat__set_time,
                                    TimeAtHaul = soak.Soak_time_group__soaktime_tracking_group__soak_time_repeat__haul_time,
                                    WaypointAtSet = soak.Soak_time_group__soaktime_tracking_group__soak_time_repeat__wpt_set,
                                    WaypointAtHaul = soak.Soak_time_group__soaktime_tracking_group__soak_time_repeat__wpt_haul
                                };
                                NSAPEntities.GearSoakViewModel.AddRecordToRepo(gs);
                            }
                        }

                        if (item.Grid_coord_group__bingo_repeat != null)
                        {
                            foreach (var gr in item.Grid_coord_group__bingo_repeat
                                 .Where(t => t.Parent.ID == item.ID))
                            {
                                FishingGroundGrid fgg = new FishingGroundGrid
                                {
                                    PK = (int)gr.ID,
                                    VesselUnloadID = vu.PK,
                                    UTMZoneText = gr.Parent.Grid_coord_group__utmZone,
                                    Grid = gr.Grid_coord_group__bingo_repeat__bingo_group__bingo_complete
                                };
                                NSAPEntities.FishingGroundGridViewModel.AddRecordToRepo(fgg);
                            }
                        }

                        if (item.Catch_comp_group__catch_composition_repeat != null)
                        {
                            foreach (var catchComp in item.Catch_comp_group__catch_composition_repeat
                                .Where(t => t.Parent.ID == item.ID))
                            {
                                VesselCatch vc = new VesselCatch
                                {
                                    PK = (int)catchComp.ID,
                                    VesselUnloadID = vu.PK,
                                    SpeciesID = catchComp.Catch_comp_group__catch_composition_repeat__speciesname_group__sp_id,
                                    Catch_kg = catchComp.Catch_comp_group__catch_composition_repeat__speciesname_group__species_wt,
                                    Sample_kg = catchComp.Catch_comp_group__catch_composition_repeat__speciesname_group__species_sample_wt,
                                    TaxaCode = catchComp.Catch_comp_group__catch_composition_repeat__speciesname_group__taxa,
                                    SpeciesText = catchComp.Catch_comp_group__catch_composition_repeat__speciesname_group__spName_other
                                };


                                if (NSAPEntities.VesselCatchViewModel.AddRecordToRepo(vc))
                                {
                                    if (catchComp.Catch_comp_group__catch_composition_repeat__length_freq_repeat != null)
                                    {
                                        foreach (var lf in catchComp.Catch_comp_group__catch_composition_repeat__length_freq_repeat
                                            .Where(t => t.Parent.ID == catchComp.ID))
                                        {
                                            CatchLenFreq clf = new CatchLenFreq
                                            {
                                                PK = (int)lf.ID,
                                                VesselCatchID = vc.PK,
                                                LengthClass = lf.Catch_comp_group__catch_composition_repeat__length_freq_repeat__group_LF__length_class,
                                                Frequency = lf.Catch_comp_group__catch_composition_repeat__length_freq_repeat__group_LF__freq
                                            };
                                            NSAPEntities.CatchLenFreqViewModel.AddRecordToRepo(clf);
                                        }
                                    }

                                    if (catchComp.Catch_comp_group__catch_composition_repeat__len_wt_repeat != null)
                                    {
                                        foreach (var lw in catchComp.Catch_comp_group__catch_composition_repeat__len_wt_repeat
                                             .Where(t => t.Parent.ID == catchComp.ID))
                                        {
                                            CatchLengthWeight clw = new CatchLengthWeight
                                            {
                                                PK = (int)lw.ID,
                                                VesselCatchID = vc.PK,
                                                Length = lw.Catch_comp_group__catch_composition_repeat__len_wt_repeat__len_wt_group__len_lenwt,
                                                Weight = lw.Catch_comp_group__catch_composition_repeat__len_wt_repeat__len_wt_group__wt_lenwt
                                            };
                                            NSAPEntities.CatchLengthWeightViewModel.AddRecordToRepo(clw);
                                        }
                                    }
                                    if (catchComp.Catch_comp_group__catch_composition_repeat__length_list_repeat != null)
                                    {
                                        foreach (var l in catchComp.Catch_comp_group__catch_composition_repeat__length_list_repeat
                                             .Where(t => t.Parent.ID == catchComp.ID))
                                        {
                                            CatchLength cl = new CatchLength
                                            {
                                                PK = (int)l.ID,
                                                VesselCatchID = vc.PK,
                                                Length = l.Catch_comp_group__catch_composition_repeat__length_list_repeat__length

                                            };
                                            NSAPEntities.CatchLengthViewModel.AddRecordToRepo(cl);
                                        }
                                    }
                                    if (catchComp.Catch_comp_group__catch_composition_repeat__gms_repeat_group != null)
                                    {
                                        foreach (var m in catchComp.Catch_comp_group__catch_composition_repeat__gms_repeat_group
                                             .Where(t => t.Parent.ID == catchComp.ID))
                                        {
                                            CatchMaturity cm = new CatchMaturity
                                            {
                                                PK = (int)m.ID,
                                                VesselCatchID = vc.PK,
                                                Length = m.Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__individual_length,
                                                Weight = m.Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__stomach_content_wt,
                                                SexCode = m.Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__sex,
                                                MaturityCode = m.Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gms_repeat,
                                                WeightGutContent = m.Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__stomach_content_wt,
                                                GutContentCode = m.Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gut_content_category
                                            };
                                            NSAPEntities.CatchMaturityViewModel.AddRecordToRepo(cm);
                                        }
                                    }
                                }

                            }
                        }

                    }
                }
            }

            UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { VesselUnloadTotalSavedCount=savedCount,  Intent = UploadToDBIntent.EndOfUpload });
            return savedCount > 0;
        }
        private static List<CatchCompGroupCatchCompositionRepeat> MakeCatchCompObjects(JToken obj)
        {
            var thisList = new List<CatchCompGroupCatchCompositionRepeat>();
            foreach (var item in obj.Children())
            {
                string sp_note = "";
                double? sp_sample_wt = null;
                double? sp_wt = null;
                int? sp_id = null;
                string spName_other = "";
                string taxa = "";
                List<CatchCompGroupCatchCompositionRepeatGmsRepeatGroup> gmsItems = null;
                List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat> lfItems = null;
                List<CatchCompGroupCatchCompositionRepeatLenWtRepeat> lenWeightItems = null;
                List<CatchCompGroupCatchCompositionRepeatLengthListRepeat> lenListItems = null;
                foreach (JToken item_child in item.Children())
                {
                    string propertyName = ((JProperty)item_child).Name.Replace("/", "__");
                    JToken propertyValue = ((JProperty)item_child).Value;
                    switch (propertyName)
                    {
                        case "catch_comp_group__catch_composition_repeat__gms_repeat_group":
                            gmsItems = MakeGMSObjects(item_child);
                            break;
                        case "catch_comp_group__catch_composition_repeat__length_freq_repeat":
                            lfItems = MakeLenFreqObjects(item_child);
                            break;
                        case "catch_comp_group__catch_composition_repeat__length_list_repeat":
                            lenListItems = MakeLengthListObjects(item_child);
                            break;
                        case "catch_comp_group__catch_composition_repeat__len_wt_repeat":
                            lenWeightItems = MakeLenWtObjects(item_child);
                            break;
                        case "catch_comp_group__catch_composition_repeat__speciesname_group__taxa":
                            taxa = propertyValue.ToString();
                            break;
                        case "catch_comp_group__catch_composition_repeat__speciesname_group__sp_id":
                            sp_id = int.Parse(propertyValue.ToString());
                            break;
                        case "catch_comp_group__catch_composition_repeat__speciesname_group__species_wt":
                            sp_wt = double.Parse(propertyValue.ToString());
                            break;
                        case "catch_comp_group__catch_composition_repeat__speciesname_group__species_sample_wt":
                            sp_sample_wt = double.Parse(propertyValue.ToString());
                            break;
                        case "catch_comp_group__catch_composition_repeat__speciesname_group__sum_species_weight_note":
                            sp_note = propertyValue.ToString();
                            break;
                        case "catch_comp_group__catch_composition_repeat__speciesname_group__spName_other":
                            spName_other = propertyValue.ToString();
                            break;
                    }
                }
                CatchCompGroupCatchCompositionRepeat catchComp = new CatchCompGroupCatchCompositionRepeat
                {
                    Catch_comp_group__catch_composition_repeat__length_list_repeat = lenListItems,
                    Catch_comp_group__catch_composition_repeat__len_wt_repeat = lenWeightItems,
                    Catch_comp_group__catch_composition_repeat__length_freq_repeat = lfItems,
                    Catch_comp_group__catch_composition_repeat__gms_repeat_group = gmsItems,
                    Catch_comp_group__catch_composition_repeat__speciesname_group__taxa = taxa,
                    Catch_comp_group__catch_composition_repeat__speciesname_group__spName_other = spName_other,
                    Catch_comp_group__catch_composition_repeat__speciesname_group__sp_id = sp_id,
                    Catch_comp_group__catch_composition_repeat__speciesname_group__species_wt = sp_wt,
                    Catch_comp_group__catch_composition_repeat__speciesname_group__species_sample_wt = sp_sample_wt,
                    Catch_comp_group__catch_composition_repeat__speciesname_group__sum_species_weight_note = sp_note
                    
                   
                };
                thisList.Add(catchComp);
            }
            return thisList;
        }
        private static List<GridCoordGroupBingoRepeat> MakeBingoGridObjects(JToken obj, UTMZone utmZone)
        {
            var thisList = new List<GridCoordGroupBingoRepeat>();
            foreach (var item in obj.Children())
            {
                string grid_coord_group__bingo_repeat__bingo_group__bingo_complete = "";
                foreach (JToken item_child in item.Children())
                {
                    string propertyName = ((JProperty)item_child).Name.Replace("/", "__");
                    JToken propertyValue = ((JProperty)item_child).Value;
                    switch (propertyName)
                    {
                        case "grid_coord_group__bingo_repeat__bingo_group__bingo_complete":
                            grid_coord_group__bingo_repeat__bingo_group__bingo_complete = propertyValue.ToString();
                            break;
                    }
                }
                GridCoordGroupBingoRepeat grid = new GridCoordGroupBingoRepeat
                {
                    Grid_coord_group__bingo_repeat__bingo_group__bingo_complete = grid_coord_group__bingo_repeat__bingo_group__bingo_complete,
                    UTMZone = utmZone,
                    GridCell = new Grid25GridCell(utmZone, grid_coord_group__bingo_repeat__bingo_group__bingo_complete)
                };
                thisList.Add(grid);
            }

            return thisList;
        }
        private static List<CatchCompGroupCatchCompositionRepeatGmsRepeatGroup> MakeGMSObjects(JToken obj)
        {
            var thisList = new List<CatchCompGroupCatchCompositionRepeatGmsRepeatGroup>();
            foreach (var item in obj.Children())
            {
                double? catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__individual_length = null;
                double? catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__individual_weight = null;
                string catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__sex = "";
                string catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gms_repeat = "";
                double? catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__stomach_content_wt = null;
                string catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gut_content_category = "";
                foreach (JToken item_child in item.Children())
                {
                    foreach (var item_gchild in item_child.Children())
                    {
                        string propertyName = ((JProperty)item_gchild).Name.Replace("/", "__");
                        JToken propertyValue = ((JProperty)item_gchild).Value;
                        switch (propertyName)
                        {
                            case "catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__individual_length":
                                catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__individual_length = double.Parse(propertyValue.ToString());
                                break;
                            case "catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__individual_weight":
                                catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__individual_weight = double.Parse(propertyValue.ToString());
                                break;
                            case "catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__sex":
                                catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__sex = propertyValue.ToString();
                                break;
                            case "catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gms_repeat":
                                catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gms_repeat = propertyValue.ToString();
                                break;
                            case "catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__stomach_content_wt":
                                catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__stomach_content_wt = double.Parse(propertyValue.ToString());
                                break;
                            case "catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gut_content_category":
                                catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gut_content_category = propertyValue.ToString();
                                break;
                        }
                    }

                    CatchCompGroupCatchCompositionRepeatGmsRepeatGroup gmsItem = new CatchCompGroupCatchCompositionRepeatGmsRepeatGroup
                    {
                        Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__individual_length = catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__individual_length,
                        Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__individual_weight = catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__individual_weight,
                        Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__sex = catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__sex,
                        Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gms_repeat = catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gms_repeat,
                        Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__stomach_content_wt = catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__stomach_content_wt,
                        Catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gut_content_category = catch_comp_group__catch_composition_repeat__gms_repeat_group__gms_group__gut_content_category
                    };
                    thisList.Add(gmsItem);
                }
            }
            return thisList;
        }
        private static List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat> MakeLenFreqObjects(JToken obj)
        {
            var thisList = new List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat>();
            foreach (var item in obj.Children())
            {
                foreach (JToken item_child in item.Children())
                {
                    double length_class = 0;
                    int freq = 0;
                    foreach (var item_gchild in item_child.Children())
                    {

                        string propertyName = ((JProperty)item_gchild).Name.Replace("/", "__");
                        JToken propertyValue = ((JProperty)item_gchild).Value;
                        switch (propertyName)
                        {
                            case "catch_comp_group__catch_composition_repeat__length_freq_repeat__group_LF__length_class":
                                length_class = double.Parse(propertyValue.ToString());
                                break;
                            case "catch_comp_group__catch_composition_repeat__length_freq_repeat__group_LF__freq":
                                freq = int.Parse(propertyValue.ToString());
                                break;
                        }

                    }
                    CatchCompGroupCatchCompositionRepeatLengthFreqRepeat lfItem = new CatchCompGroupCatchCompositionRepeatLengthFreqRepeat
                    {
                        Catch_comp_group__catch_composition_repeat__length_freq_repeat__group_LF__length_class = length_class,
                        Catch_comp_group__catch_composition_repeat__length_freq_repeat__group_LF__freq = freq
                        
                    };
                    thisList.Add(lfItem);
                }
            }
            return thisList;
        }
        private static List<CatchCompGroupCatchCompositionRepeatLenWtRepeat> MakeLenWtObjects(JToken obj)
        {
            var thisList = new List<CatchCompGroupCatchCompositionRepeatLenWtRepeat>();
            foreach (var item in obj.Children())
            {
                foreach (JToken item_child in item.Children())
                {
                    double length = 0;
                    double weight = 0;
                    foreach (var item_gchild in item_child.Children())
                    {

                        string propertyName = ((JProperty)item_gchild).Name.Replace("/", "__");
                        JToken propertyValue = ((JProperty)item_gchild).Value;
                        switch (propertyName)
                        {
                            case "catch_comp_group__catch_composition_repeat__len_wt_repeat__len_wt_group__len_lenwt":
                                length = double.Parse(propertyValue.ToString());
                                break;
                            case "catch_comp_group__catch_composition_repeat__len_wt_repeat__len_wt_group__wt_lenwt":
                                weight = double.Parse(propertyValue.ToString());
                                break;
                        }

                    }
                    CatchCompGroupCatchCompositionRepeatLenWtRepeat lwItem = new CatchCompGroupCatchCompositionRepeatLenWtRepeat
                    {
                        Catch_comp_group__catch_composition_repeat__len_wt_repeat__len_wt_group__wt_lenwt = weight,
                        Catch_comp_group__catch_composition_repeat__len_wt_repeat__len_wt_group__len_lenwt = length
                    };
                    thisList.Add(lwItem);
                }
            }
            return thisList;
        }
        private static List<CatchCompGroupCatchCompositionRepeatLengthListRepeat> MakeLengthListObjects(JToken obj)
        {
            var thisList = new List<CatchCompGroupCatchCompositionRepeatLengthListRepeat>();
            foreach (var item in obj.Children())
            {
                foreach (JToken item_child in item.Children())
                {
                    double length = 0;
                    foreach (var item_gchild in item_child.Children())
                    {

                        string propertyName = ((JProperty)item_gchild).Name.Replace("/", "__");
                        JToken propertyValue = ((JProperty)item_gchild).Value;
                        switch (propertyName)
                        {
                            case "catch_comp_group__catch_composition_repeat__length_list_repeat__length":
                                length = double.Parse(propertyValue.ToString());
                                break;
                        }

                    }
                    CatchCompGroupCatchCompositionRepeatLengthListRepeat lwItem = new CatchCompGroupCatchCompositionRepeatLengthListRepeat
                    {
                        Catch_comp_group__catch_composition_repeat__length_list_repeat__length = length
                    };
                    thisList.Add(lwItem);
                }
            }
            return thisList;
        }
        private static List<SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat> MakeSoakTimeObjects(JToken obj)
        {
            var thisList = new List<SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat>();
            foreach (var item in obj.Children())
            {
                DateTime soak_time_group__soaktime_tracking_group__soak_time_repeat__set_time = DateTime.Now;
                DateTime soak_time_group__soaktime_tracking_group__soak_time_repeat__haul_time = DateTime.Now;
                string soak_time_group__soaktime_tracking_group__soak_time_repeat__wpt_set = "";
                string soak_time_group__soaktime_tracking_group__soak_time_repeat__wpt_haul = "";
                foreach (JToken item_child in item.Children())
                {


                    string propertyName = ((JProperty)item_child).Name.Replace("/", "__");
                    JToken propertyValue = ((JProperty)item_child).Value;
                    switch (propertyName)
                    {
                        case "soak_time_group__soaktime_tracking_group__soak_time_repeat__set_time":
                            soak_time_group__soaktime_tracking_group__soak_time_repeat__set_time = DateTime.Parse(propertyValue.ToString());
                            break;
                        case "soak_time_group__soaktime_tracking_group__soak_time_repeat__haul_time":
                            soak_time_group__soaktime_tracking_group__soak_time_repeat__haul_time = DateTime.Parse(propertyValue.ToString());
                            break;
                        case "soak_time_group__soaktime_tracking_group__soak_time_repeat__wpt_set":
                            soak_time_group__soaktime_tracking_group__soak_time_repeat__wpt_set = propertyValue.ToString();
                            break;
                        case "soak_time_group__soaktime_tracking_group__soak_time_repeat__wpt_haul":
                            soak_time_group__soaktime_tracking_group__soak_time_repeat__wpt_haul = propertyValue.ToString();
                            break;
                    }
                }
                SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat soakTime = new SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat
                {
                    Soak_time_group__soaktime_tracking_group__soak_time_repeat__set_time = soak_time_group__soaktime_tracking_group__soak_time_repeat__set_time,
                    Soak_time_group__soaktime_tracking_group__soak_time_repeat__haul_time = soak_time_group__soaktime_tracking_group__soak_time_repeat__haul_time,
                    Soak_time_group__soaktime_tracking_group__soak_time_repeat__wpt_set = soak_time_group__soaktime_tracking_group__soak_time_repeat__wpt_set,
                    Soak_time_group__soaktime_tracking_group__soak_time_repeat__wpt_haul = soak_time_group__soaktime_tracking_group__soak_time_repeat__wpt_haul

                };
                thisList.Add(soakTime);
            }
            return thisList;
        }
        private static List<EffortsGroupEffortRepeat> MakeEffortObjects(JToken obj)
        {
            var thisList = new List<EffortsGroupEffortRepeat>();
            foreach (var item in obj.Children())
            {
                int efforts_group__effort_repeat__group_effort__effort_type = 0;
                double? efforts_group__effort_repeat__group_effort__effort_intensity = null;
                string efforts_group__effort_repeat__group_effort__effort_desc = "";
                bool? efforts_group__effort_repeat__group_effort__effort_bool = false;

                foreach (JToken item_child in item.Children())
                {
                    string propertyName = ((JProperty)item_child).Name.Replace("/", "__");
                    JToken propertyValue = ((JProperty)item_child).Value;
                    switch (propertyName)
                    {
                        case "efforts_group__effort_repeat__group_effort__effort_type":
                            efforts_group__effort_repeat__group_effort__effort_type = int.Parse(propertyValue.ToString());
                            break;
                        case "efforts_group__effort_repeat__group_effort__effort_intensity":
                            efforts_group__effort_repeat__group_effort__effort_intensity = double.Parse(propertyValue.ToString());
                            break;
                        case "efforts_group__effort_repeat__group_effort__effort_desc":
                            efforts_group__effort_repeat__group_effort__effort_desc = propertyValue.ToString();
                            break;
                        case "efforts_group__effort_repeat__group_effort__effort_bool":
                            efforts_group__effort_repeat__group_effort__effort_bool = propertyValue.ToString() == "yes";
                            break;
                    }
                }
                EffortsGroupEffortRepeat effort = new EffortsGroupEffortRepeat
                {
                    Efforts_group__effort_repeat__group_effort__effort_type = efforts_group__effort_repeat__group_effort__effort_type,
                    Efforts_group__effort_repeat__group_effort__effort_intensity = efforts_group__effort_repeat__group_effort__effort_intensity,
                    Efforts_group__effort_repeat__group_effort__effort_desc = efforts_group__effort_repeat__group_effort__effort_desc,
                    Efforts_group__effort_repeat__group_effort__effort_bool = efforts_group__effort_repeat__group_effort__effort_bool
                };
                thisList.Add(effort);
            }
            return thisList;
        }


        private static void SetupEntityIDs(List<JSONMainSheet> parents)
        {
            BingoGrids(parents);
            SoakTimes(parents);
            Efforts(parents);
            CatchCompositions(parents);
            CatchLengthFreqs(parents);
            CatchLengthMaturities(parents);
            CatchLengthWeights(parents);
            CatchLengths(parents);
        }
        public static List<SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat>SoakTimes(List<JSONMainSheet> parents)
        {
            List<SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat> soakTimes = new List<SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat>();
            foreach (var item in parents)
            {
                if (item.Soak_time_group__soaktime_tracking_group__soak_time_repeat != null)
                {
                    foreach (var item_child in item.Soak_time_group__soaktime_tracking_group__soak_time_repeat)
                    {
                        item_child.Parent = item;
                        if (!item.IsSavedToDatabase && item_child.ID==null)
                        {
                            item_child.ID = _gear_soak_ID++;
                        }
                        soakTimes.Add(item_child);
                    }
                }
            }
            return soakTimes;
        }

        public static List<EffortsGroupEffortRepeat> Efforts(List<JSONMainSheet> parents)
        {
            List<EffortsGroupEffortRepeat> thisList = new List<EffortsGroupEffortRepeat>();
            foreach (var item in parents)
            {
                if (item.Efforts_group__effort_repeat != null)
                {
                    foreach (var item_child in item.Efforts_group__effort_repeat)
                    {
                        item_child.Parent = item;
                        if(!item.IsSavedToDatabase && item_child.ID == null)
                        {
                            item_child.ID = _effort_spec_ID++;
                        }
                        thisList.Add(item_child);
                    }
                }
            }
            return thisList;
        }

        public static List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat> CatchLengthFreqs(List<JSONMainSheet> parents)
        {
            if (_catchCompositions == null)
            {
                CatchCompositions(parents);
            }

            List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat> thisList = new List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat>();
            foreach (var item in parents)
            {
                if (item.Catch_comp_group__catch_composition_repeat != null)
                {
                    foreach (var itemChild in item.Catch_comp_group__catch_composition_repeat)
                    {
                        if (itemChild.Catch_comp_group__catch_composition_repeat__length_freq_repeat != null)
                        {
                            foreach (var catchItem in itemChild.Catch_comp_group__catch_composition_repeat__length_freq_repeat)
                            {
                                catchItem.Parent = itemChild;
                                if(!item.IsSavedToDatabase && catchItem.ID == null)
                                {
                                    catchItem.ID = _len_freq_ID++;
                                }
                                else
                                thisList.Add(catchItem);
                            }
                        }
                    }
                }
            }
            return thisList;
        }


        public static List<CatchCompGroupCatchCompositionRepeatGmsRepeatGroup> CatchLengthMaturities(List<JSONMainSheet> parents)
        {
            if (_catchCompositions == null)
            {
                CatchCompositions(parents);
            }

            List<CatchCompGroupCatchCompositionRepeatGmsRepeatGroup> thisList = new List<CatchCompGroupCatchCompositionRepeatGmsRepeatGroup>();
            foreach (var item in parents)
            {
                if (item.Catch_comp_group__catch_composition_repeat != null)
                {
                    foreach (var itemChild in item.Catch_comp_group__catch_composition_repeat)
                    {
                        if (itemChild.Catch_comp_group__catch_composition_repeat__gms_repeat_group != null)
                        {
                            foreach (var catchItem in itemChild.Catch_comp_group__catch_composition_repeat__gms_repeat_group)
                            {
                                catchItem.Parent = itemChild;
                                if(!item.IsSavedToDatabase && catchItem.ID==null)
                                {
                                    catchItem.ID = _maturity_ID++;
                                }
                                thisList.Add(catchItem);
                            }
                        }
                    }
                }
            }
            return thisList;
        }
        public static List<CatchCompGroupCatchCompositionRepeatLenWtRepeat> CatchLengthWeights(List<JSONMainSheet> parents)
        {
            if (_catchCompositions == null)
            {
                CatchCompositions(parents);
            }

            List<CatchCompGroupCatchCompositionRepeatLenWtRepeat> thisList = new List<CatchCompGroupCatchCompositionRepeatLenWtRepeat>();
            foreach (var item in parents)
            {
                if (item.Catch_comp_group__catch_composition_repeat != null)
                {
                    foreach (var itemChild in item.Catch_comp_group__catch_composition_repeat)
                    {
                        if (itemChild.Catch_comp_group__catch_composition_repeat__len_wt_repeat != null)
                        {
                            foreach (var catchItem in itemChild.Catch_comp_group__catch_composition_repeat__len_wt_repeat)
                            {
                                catchItem.Parent = itemChild;
                                if(!item.IsSavedToDatabase && catchItem.ID==null)
                                {
                                    catchItem.ID = _len_wt_ID++;
                                }
                                thisList.Add(catchItem);
                            }
                        }
                    }
                }
            }
            return thisList;
        }
        public static List<CatchCompGroupCatchCompositionRepeatLengthListRepeat>CatchLengths(List<JSONMainSheet> parents)
        {
            if(_catchCompositions==null)
            {
                CatchCompositions(parents);
            }
            List<CatchCompGroupCatchCompositionRepeatLengthListRepeat> thisList = new List<CatchCompGroupCatchCompositionRepeatLengthListRepeat>();
            foreach(var item in parents)
            {
                if(item.Catch_comp_group__catch_composition_repeat!=null)
                {
                    foreach(var itemChild in item.Catch_comp_group__catch_composition_repeat)
                    {
                        if (itemChild.Catch_comp_group__catch_composition_repeat__length_list_repeat != null)
                        {
                            foreach (var catchItem in itemChild.Catch_comp_group__catch_composition_repeat__length_list_repeat)
                            {
                                catchItem.Parent = itemChild;
                                if(!item.IsSavedToDatabase && catchItem.ID==null)
                                {
                                    catchItem.ID = _length_list_id++;
                                }
                                thisList.Add(catchItem);
                            }
                        }
                    }
                }
            }
            return thisList;
        }
        public static List<CatchCompGroupCatchCompositionRepeat> CatchCompositions(List<JSONMainSheet> parents)
        {
            if (_catchCompositions == null)
            {
                _catchCompositions = new List<CatchCompGroupCatchCompositionRepeat>();
                foreach (var item in parents)
                {
                    if (item.Catch_comp_group__catch_composition_repeat != null)
                    {
                        foreach (var item_child in item.Catch_comp_group__catch_composition_repeat)
                        {
                            item_child.Parent = item;
                            if (!item.IsSavedToDatabase && item_child.ID == null)
                            {
                                item_child.ID = _catch_comp_ID++;
                            }
                            else
                            {
                                var vesselUnload = NSAPEntities.VesselUnloadViewModel.getVesselUnload(item.ID);
                                item_child.ID = NSAPEntities.VesselCatchViewModel.getVesselCatch(vesselUnload, item_child.SpeciesName).PK;
                            }

                            _catchCompositions.Add(item_child);
                        }
                    }
                }
            }
            return _catchCompositions;
        }
        public static List<GridCoordGroupBingoRepeat>BingoGrids(List<JSONMainSheet> parents)
        {
            List<GridCoordGroupBingoRepeat> bingoGrids = new List<GridCoordGroupBingoRepeat>();
            foreach (var item in parents)
            {
                if (item.Grid_coord_group__bingo_repeat != null)
                {
                    foreach (var grid_item in item.Grid_coord_group__bingo_repeat)
                    {
                        grid_item.Parent = item;
                        if(!item.IsSavedToDatabase && grid_item.ID==null)
                        {
                            grid_item.ID = _fishing_grid_ID++;
                        }
                        bingoGrids.Add(grid_item);
                    }
                }
            }

            return bingoGrids;
        }

        public static List<JSONMainSheet> MakeMainSheetObjects(JArray root)
        {
            var thisList = new List<JSONMainSheet>();
            foreach (var item in root)
            {
                var formhub__uuid = "";
                var intronote = "";
                var _xform_id_string = "";
                var _bamboo_dataset_id = "";
                int fma_number = 0;
                var user_name = "";
                int? vessel_sampling__region_enumerator = null;
                int? vessel_sampling__gear_used = null;
                int? fishing_vessel_group__boat_used = null;
                var _status = "";
                var __version__ = "";
                string meta__instanceID = "";
                DateTime start = DateTime.Now;
                DateTime vessel_sampling__sampling_date = DateTime.Now;
                DateTime today = DateTime.Now;
                string device_id = "";
                string email = "";
                string vessel_sampling__nsap_region = "";
                string vessel_sampling__region_enumerator_text = "";
                int vessel_sampling__fma_in_region = 0;
                int vessel_sampling__fishing_ground = 0;
                int? vessel_sampling__landing_site = null;
                string vessel_sampling__landing_site_text = "";
                string vessel_sampling__gear_used_text = "";
                string fishing_vessel_group__is_boat_used ="";
                var fishing_vessel_group__boat_used_text = "";
                string fishing_vessel_group__fish_sector = "";
                string fishing_vessel_group__gear_code = "";
                string fishing_vessel_group__gear_name = "";
                bool vessel_catch__trip_isSuccess = false;
                double? vessel_catch__catch_total = null;
                int? vessel_catch__boxes_total = null;
                double? vessel_catch__catch_sampled = null;
                int? vessel_catch__boxes_sampled = null;
                double? vessel_catch__raising_factor = null;
                string remarks = "";
                string grid_coord_group__utmZone = "";
                bool soak_time_group__include_tracking = false;
                string soak_time_group__soaktime_tracking_group__gps = "";
                DateTime? soak_time_group__soaktime_tracking_group__time_depart_landingsite = null;
                DateTime? soak_time_group__soaktime_tracking_group__time_arrive_landingsite = null;
                int id = 0;
                string uuid = "";
                DateTime submission_time = DateTime.Now;
                List<CatchCompGroupCatchCompositionRepeat> catchCompItems = null;
                List<SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat> soakTimeItems = null;
                List<EffortsGroupEffortRepeat> effortItems = null;
                List<GridCoordGroupBingoRepeat> bingoGrids = null;
                int _index = 0;
                foreach (JToken child in item.Values())
                {

                    var path = GetPath(child.Path);
                    //var arr = child.Parent.ToString().Split(':', '\\', '\"');
                    switch (path)
                    {
                        case "catch_comp_group__catch_composition_repeat":
                            catchCompItems = MakeCatchCompObjects(child);
                            break;

                        case "efforts_group__effort_repeat":
                            effortItems = MakeEffortObjects(child);
                            break;

                        case "grid_coord_group__bingo_repeat":
                            if (grid_coord_group__utmZone.Length > 0)
                            {
                                bingoGrids = MakeBingoGridObjects(child, new UTMZone(grid_coord_group__utmZone));
                            }
                            break;
                        case "soak_time_group__soaktime_tracking_group__soak_time_repeat":
                            soakTimeItems = MakeSoakTimeObjects(child);
                            break;




                        case "start":
                            start = DateTime.Parse(child.ToString());
                            break;
                        case "today":
                            today = DateTime.Parse(child.ToString());
                            break;
                        case "device_id":
                            device_id = child.ToString();
                            break;
                        case "user_name":
                            user_name = child.ToString();
                            break;
                        case "email":
                            email = child.ToString();
                            break;
                        case "intronote":
                            intronote = child.ToString();
                            break;



                        case "vessel_sampling__sampling_date":
                            vessel_sampling__sampling_date = DateTime.Parse(child.ToString());
                            break;
                        case "vessel_sampling__nsap_region":
                            vessel_sampling__nsap_region = child.ToString();
                            break;
                        case "vessel_sampling__region_enumerator":
                            vessel_sampling__region_enumerator = child.ToString() == "" ? null : (int?)int.Parse(child.ToString());
                            break;
                        case "vessel_sampling__region_enumerator_text":
                            vessel_sampling__region_enumerator_text = child.ToString();
                            break;
                        case "vessel_sampling__fma_in_region":
                            vessel_sampling__fma_in_region = int.Parse(child.ToString());
                            break;
                        case "vessel_sampling__fishing_ground":
                            vessel_sampling__fishing_ground = int.Parse(child.ToString());
                            break;
                        case "vessel_sampling__landing_site":
                            vessel_sampling__landing_site = child.ToString() == "" ? null : (int?)int.Parse(child.ToString());
                            break;
                        case "vessel_sampling__landing_site_text":
                            vessel_sampling__landing_site_text = child.ToString();
                            break;
                        case "vessel_sampling__gear_used":
                            vessel_sampling__gear_used = child.ToString() == "" ? null : (int?)int.Parse(child.ToString());
                            break;
                        case "vessel_sampling__gear_used_text":
                            vessel_sampling__gear_used_text = child.ToString();
                            break;
                        case "fma_number":
                            fma_number = int.Parse(child.ToString());
                            break;



                        case "fishing_vessel_group__is_boat_used":
                            fishing_vessel_group__is_boat_used = child.ToString();
                            break;
                        case "fishing_vessel_group__fish_sector":
                            fishing_vessel_group__fish_sector = child.ToString();
                            break;
                        case "fishing_vessel_group__boat_used":
                            fishing_vessel_group__boat_used = child.ToString() == "" ? null : (int?)int.Parse(child.ToString());
                            break;
                        case "fishing_vessel_group__boat_used_text":
                            fishing_vessel_group__boat_used_text = child.ToString();
                            break;
                        case "fishing_vessel_group__gear_code":
                            fishing_vessel_group__gear_code = child.ToString();
                            break;
                        case "fishing_vessel_group__gear_name":
                            fishing_vessel_group__gear_name = child.ToString();
                            break;


                        case "vessel_catch__trip_isSuccess":
                            vessel_catch__trip_isSuccess = child.ToString() == "yes";
                            break;
                        case "vessel_catch__catch_total":
                            vessel_catch__catch_total = child.ToString() == "" ? null : (double?)double.Parse(child.ToString());
                            break;
                        case "vessel_catch__catch_sampled":
                            vessel_catch__catch_sampled = child.ToString() == "" ? null : (double?)double.Parse(child.ToString());
                            break;
                        case "vessel_catch__boxes_total":
                            vessel_catch__boxes_total = child.ToString() == "" ? null : (int?)int.Parse(child.ToString());
                            break;
                        case "vessel_catch__boxes_sampled":
                            vessel_catch__boxes_sampled = child.ToString() == "" ? null : (int?)int.Parse(child.ToString());
                            break;
                        case "vessel_catch__raising_factor":
                            vessel_catch__raising_factor = child.ToString() == "" ? null : (double?)double.Parse(child.ToString());
                            break;
                        case "vessel_catch__remarks":
                            remarks = child.ToString();
                            break;


                        case "grid_coord_group__utmZone":
                            grid_coord_group__utmZone = child.ToString();
                            break;


                        case "soak_time_group__include_tracking":
                            soak_time_group__include_tracking = child.ToString() == "yes";
                            break;
                        case "soak_time_group__soaktime_tracking_group__gps":
                            soak_time_group__soaktime_tracking_group__gps = child.ToString();
                            break;
                        case "soak_time_group__soaktime_tracking_group__time_depart_landingsite":
                            soak_time_group__soaktime_tracking_group__time_depart_landingsite = child.ToString() == "" ? null : (DateTime?)DateTime.Parse(child.ToString());
                            break;
                        case "soak_time_group__soaktime_tracking_group__time_arrive_landingsite":
                            soak_time_group__soaktime_tracking_group__time_arrive_landingsite = child.ToString() == "" ? null : (DateTime?)DateTime.Parse(child.ToString());
                            break;


                        case "_id":
                            id = int.Parse(child.ToString());
                            break;
                        case "notes":
                            break;
                        case "_status":
                            _status = child.ToString();
                            break;
                        case "__version__":
                            __version__ = child.ToString();
                            break;
                        case "_index":
                            _index = int.Parse(child.ToString());
                            break;
                        case "_notes":
                            break;
                        case "meta__instanceID":
                            meta__instanceID = child.ToString();
                            break;
                        case "formhub__uuid":
                            formhub__uuid = child.ToString();
                            break;
                        case "_bamboo_dataset_id":
                            _bamboo_dataset_id = child.ToString();
                            break;
                        case "_xform_id_string":
                            _xform_id_string = child.ToString();
                            break;
                        case "_uuid":
                            uuid = child.ToString();
                            break;

                        case "_submission_time":
                            submission_time = DateTime.Parse(child.ToString());
                            break;
                        case "_submitted_by":
                            break;
                        default:
                            break;
                    }
                }
                int unload_id;
                JSONMainSheet ms = new JSONMainSheet
                {
                    Start = start,
                    Today = today,
                    Device_id = device_id,
                    User_name = user_name,
                    Email = email,
                    Intronote = intronote,
                    Vessel_sampling__sampling_date = vessel_sampling__sampling_date,
                    Vessel_sampling__nsap_region = vessel_sampling__nsap_region,
                    Vessel_sampling__region_enumerator = vessel_sampling__region_enumerator,
                    Vessel_sampling__region_enumerator_text = vessel_sampling__region_enumerator_text,
                    Vessel_sampling__fma_in_region = vessel_sampling__fma_in_region,
                    Vessel_sampling__fishing_ground = vessel_sampling__fishing_ground,
                    Vessel_sampling__landing_site = vessel_sampling__landing_site,
                    Vessel_sampling__landing_site_text = vessel_sampling__landing_site_text,
                    Vessel_sampling__gear_used = vessel_sampling__gear_used,
                    Vessel_sampling__gear_used_text = vessel_sampling__gear_used_text,
                    FMA_number = fma_number,
                    Fishing_vessel_group__fish_sector = fishing_vessel_group__fish_sector,
                    Fishing_vessel_group__is_boat_used_text = fishing_vessel_group__is_boat_used,
                    Fishing_vessel_group__boat_used = fishing_vessel_group__boat_used,
                    Fishing_vessel_group__boat_used_text = fishing_vessel_group__boat_used_text,
                    //Fishing_vessel_group__gear_code = fishing_vessel_group__gear_code,
                    Gear_code = fishing_vessel_group__gear_code,
                    Fishing_vessel_group__gear_name = fishing_vessel_group__gear_name,
                    Vessel_catch__trip_isSuccess = vessel_catch__trip_isSuccess,
                    Vessel_catch__catch_total = vessel_catch__catch_total,
                    Vessel_catch__boxes_total = vessel_catch__boxes_total,
                    Vessel_catch__catch_sampled = vessel_catch__catch_sampled,
                    Vessel_catch__boxes_sampled = vessel_catch__boxes_sampled,
                    Vessel_catch__raising_factor = vessel_catch__raising_factor,
                    Vessel_catch__remarks = remarks,
                    Grid_coord_group__utmZone = grid_coord_group__utmZone,
                    Soak_time_group__include_tracking = soak_time_group__include_tracking,
                    Soak_time_group__soaktime_tracking_group__gps = soak_time_group__soaktime_tracking_group__gps,
                    Soak_time_group__soaktime_tracking_group__time_depart_landingsite = soak_time_group__soaktime_tracking_group__time_depart_landingsite,
                    Soak_time_group__soaktime_tracking_group__time_arrive_landingsite = soak_time_group__soaktime_tracking_group__time_arrive_landingsite,
                    __version__ = __version__,
                    Meta__instanceID = meta__instanceID,
                    _id = id,
                    _uuid = uuid,
                    _submission_time = submission_time,
                    Formhub__uuid = formhub__uuid,
                    _bamboo_dataset_id = _bamboo_dataset_id,
                    _xform_id_string = _xform_id_string,
                    _index = _index,
                    _status = _status,
                    Catch_comp_group__catch_composition_repeat = catchCompItems,
                    Soak_time_group__soaktime_tracking_group__soak_time_repeat = soakTimeItems,
                    Efforts_group__effort_repeat = effortItems,
                    Grid_coord_group__bingo_repeat = bingoGrids,
                    ID = Vessel_unload_saved(uuid, out  unload_id)?unload_id:++_vessel_unload_ID,

                };
                if (ms.Grid_coord_group__utmZone.Length > 0)
                {
                    ms.UTMZone = new UTMZone(ms.Grid_coord_group__utmZone);
                }
                thisList.Add(ms);

            }
            return thisList;
        }
        public static string GetPath(string path)
        {
            const string PunctuationChars = "[]'.";
            const string Numbers = "0123456789";
            try
            {
                foreach (var punctuationChar in PunctuationChars)
                {
                    if (path.IndexOf(punctuationChar) >= 0)
                    {
                        path = path.Replace("" + punctuationChar, "");
                        if (path.IndexOf(punctuationChar) > 0)
                        {
                            path = GetPath(path);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            try
            {
                while (Numbers.Contains(path.Substring(0, 1)))
                {
                    foreach (var number in Numbers)
                    {
                        if (path.IndexOf(number) == 0)
                        {
                            path = path.Replace("" + number, "");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return path.Replace("/", "__");
        }
    }


    class DownloadedSubmissionsJSON
    {
    }
}
