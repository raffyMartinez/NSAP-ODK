using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NSAP_ODK.Entities.Database.ODKEntities
{
    public class LenFreqRepeat_6_3
    {
        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/length_freq_repeat/group_LF/length_class")]
        public string LengthClass { get; set; }

        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/length_freq_repeat/group_LF/freq")]
        public string Freq { get; set; }
    }

    public class LengthListRepeat_6_3
    {
        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/length_list_repeat/length_list_group/length")]
        public string Length { get; set; }
    }

    public class LenWtRepeat_6_3
    {
        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/len_wt_repeat/len_wt_group/len_lenwt")]
        public string Length { get; set; }

        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/len_wt_repeat/len_wt_group/wt_lenwt")]
        public string Weight { get; set; }
    }

    public class GMSRepeat_6_3
    {

        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/gms_repeat_group/gms_group/individual_length")]
        public double? IndividualLength { get; set; }

        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/gms_repeat_group/gms_group/individual_weight")]
        public double? IndividualWeight { get; set; }

        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/gms_repeat_group/gms_group/sex")]
        public string Sex { get; set; }

        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/gms_repeat_group/gms_group/gms_repeat")]
        public string MaturityStage { get; set; }

        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/gms_repeat_group/gms_group/gonad_wt")]
        public double? GonadWt { get; set; }

        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/gms_repeat_group/stomach_content_wt")]
        public double? GutContentWt { get; set; }

        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/gms_repeat_group/gut_content_category")]
        public string GutContentCategory { get; set; }
    }
    public class CatchCompositionRepeat_6_3
    {
        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/taxa")]
        public string Taxa { get; set; }


        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/spName_other")]
        public string OtherSpeciesName { get; set; }

        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/species")]
        public string Species { get; set; }

        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/sp_id")]

        public int? SpeciesID { get; set; }


        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/species_notfish")]
        public string NotFishSpecies { get; set; }

        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/species_name_selected")]
        public string SpeciesSelected { get; set; }

        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/species_wt")]
        public double SpeciesWt { get; set; }

        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/species_sample_wt")]
        public double? SpeciesSampleWt { get; set; }


        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/length_freq_repeat")]
        public List<LenFreqRepeat_6_3> LenFreqRepeat { get; set; }


        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/length_list_repeat")]
        public List<LengthListRepeat_6_3> LenListRepeat { get; set; }

        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/len_wt_repeat")]
        public List<LenWtRepeat_6_3> LenWtRepeat { get; set; }



        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/gms_repeat_group")]
        public List<GMSRepeat_6_3> GMSRepeat { get; set; }
    }

    public class BingoRepeat_6_3
    {
        [JsonProperty("grid_coord_group/bingo_repeat/bingo_group/major_grid")]
        public int? MajorGrid { get; set; }

        [JsonProperty("grid_coord_group/bingo_repeat/bingo_group/col_name")]
        public string ColumnName { get; set; }

        [JsonProperty("grid_coord_group/bingo_repeat/bingo_group/row_name")]
        public int? RowName { get; set; }

        [JsonProperty("grid_coord_group/bingo_repeat/bingo_group/bingo_complete")]
        public string BingoCompplete { get; set; }

    }

    public class SoakTimeRepeat_6_3
    {

        [JsonProperty("soak_time_group/soaktime_tracking_group/soak_time_repeat/set_time")]
        public string SetTime { get; set; }

        [JsonProperty("soak_time_group/soaktime_tracking_group/soak_time_repeat/haul_time")]
        public string HaulTime { get; set; }

        [JsonProperty("soak_time_group/soaktime_tracking_group/soak_time_repeat/wpt_set")]
        public string SetWaypoint { get; set; }

        [JsonProperty("soak_time_group/soaktime_tracking_group/soak_time_repeat/wpt_haul")]
        public string HaulWaypoint { get; set; }
    }

    public class EffortRepeat_6_3
    {
        [JsonProperty("efforts_group/effort_repeat/group_effort/effort_type")]
        public string EffortType { get; set; }

 
        [JsonProperty("efforts_group/effort_repeat/group_effort/effort_spec_name")]
        public string EffortSpecName { get; set; }

        [JsonProperty("efforts_group/effort_repeat/group_effort/effort_intensity")]
        public double? EffortIntensity { get; set; }


        [JsonProperty("efforts_group/effort_repeat/group_effort/effort_desc")]
        public string EffortDescription { get; set; }

        [JsonProperty("efforts_group/effort_repeat/group_effort/effort_bool")]
        public string EffortBool { get; set; }
    }
    class VesselLanding_6_3
    {
        [JsonProperty("_id")]
        public int Id { get; set; }

        [JsonProperty("formhub/uuid")]
        public string FormhubUuid { get; set; }

        [JsonProperty("start")]
        public string Start { get; set; }

        [JsonProperty("today")]
        public string Today { get; set; }

        [JsonProperty("device_id")]
        public string DeviceId { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }

        [JsonProperty("intronote")]
        public string Intronote { get; set; }

        [JsonProperty("vessel_sampling/sampling_date")]
        public DateTime SamplingDate { get; set; }

        [JsonProperty("vessel_sampling/nsap_region")]
        public string NsapRegion { get; set; }


        [JsonProperty("vessel_sampling/region_enumerator")]
        public string RegionEnumerator { get; set; }

        [JsonProperty("vessel_sampling/region_enumerator_text")]
        public string RegionEnumeratorText { get; set; }

        [JsonProperty("vessel_sampling/fma_in_region")]
        public string FmaInRegion { get; set; }

        [JsonProperty("fma_number")]
        public string FmaNumber { get; set; }

        [JsonProperty("vessel_sampling/fishing_ground")]
        public string FishingGround { get; set; }

        [JsonProperty("vessel_sampling/landing_site")]
        public string LandingSite { get; set; }

        [JsonProperty("vessel_sampling/landing_site_text")]
        public string LandingSiteText { get; set; }

        [JsonProperty("vessel_sampling/select_gear")]
         public string GearUsed { get; set; }

        [JsonProperty("vessel_sampling/gear_used_text")]
        public string GearUsedText { get; set; }

        
        //tracking
        [JsonProperty("vessel_sampling/is_gps_used")]
        public string IsGpsUsed { get; set; }

        [JsonProperty("vessel_sampling/gps2")]
        public string GPSUsed { get; set; }

        [JsonProperty("vessel_sampling/time_depart_landingsite")]
        public DateTime? TimeDepartLandingsite { get; set; }


        [JsonProperty("vessel_sampling/time_arrive_landingsite")]
        public DateTime? TimeArriveLandingsite { get; set; }


        //vessel

        [JsonProperty("fishing_vessel_group/is_boat_used")]
        public string IsBoatUsed { get; set; }

        [JsonProperty("fishing_vessel_group/fish_sector")]
        public string FishSector { get; set; }


        [JsonProperty("fishing_vessel_group/boat_used")]
        public string BoatUsed { get; set; }

        [JsonProperty("fishing_vessel_group/boat_used_text")]
        public string BoatUsedText { get; set; }


        //catch weight
        [JsonProperty("vessel_catch/trip_isSuccess")]
        public string TripIsSuccess { get; set; }

        [JsonProperty("vessel_catch/catch_total")]
        public double CatchTotalWeight { get; set; }

        [JsonProperty("vessel_catch/boxes_total")]
        public int? BoxesTotal { get; set; }

        [JsonProperty("vessel_catch/catch_sampled")]
        public double? WeightSampledCatch { get; set; }

        [JsonProperty("vessel_catch/boxes_sampled")]
        public int? BoxesSampled { get; set; }

        [JsonProperty("vessel_catch/raising_factor")]
        public double? RaisingFactor { get; set; }

        [JsonProperty("vessel_catch/remarks")]
        public string Remarks { get; set; }

        
        //BINGO grid

        [JsonProperty("grid_coord_group/include_bingo")]
        public string IncludeBingo { get; set; }

        [JsonProperty("grid_coord_group/utmZone")]
        public string UTMZone { get; set; }

        [JsonProperty("grid_coord_group/bingo_repeat")]
        public List<BingoRepeat_6_3> BingoRepeat { get; set; }


        //soak time
        [JsonProperty("soak_time_group/include_soak_time")]
        public string IncludeSoakTime { get; set; }

        [JsonProperty("soak_time_group/soaktime_tracking_group/soak_time_repeat")]
        public List<SoakTimeRepeat_6_3> SoakTimeRepeat { get; set; }

        
        //effort measurement
        [JsonProperty("efforts_group/include_effort")]
        public string IncludeEffortMeasurement { get; set; }

        [JsonProperty("efforts_group/effort_repeat")]
        public List<EffortRepeat_6_3> EffortRepeat { get; set; }

        //catch composition

        [JsonProperty("catch_comp_group/include_catchcomp")]
        public string CatchCompGroupIncludeCatchcomp { get; set; }

        [JsonProperty("catch_comp_group/length_type")]
        public string CatchCompGroupLengthType { get; set; }

        [JsonProperty("catch_comp_group/has_gms")]
        public string CatchCompGroupHasGms { get; set; }

        [JsonProperty("catch_comp_group/count_catch_comp")]
        public string CatchCompGroupCountCatchComp { get; set; }

        [JsonProperty("catch_comp_group/catch_composition_repeat")]
        public List<CatchCompositionRepeat_6_3> CatchCompositionRepeat { get; set; }

        [JsonProperty("catch_comp_group/sum_species_weight")]
        public string CatchCompGroupSumSpeciesWeight { get; set; }

        [JsonProperty("catch_comp_group/sum_species_weight_coalesce")]
        public string CatchCompGroupSumSpeciesWeightCoalesce { get; set; }

        [JsonProperty("__version__")]
        public string Version { get; set; }

        //[JsonProperty("_version_")]
        //public string Version { get; set; }

        //[JsonProperty("_version__001")]
        //public string Version001 { get; set; }

        //[JsonProperty("_version__002")]
        //public string Version002 { get; set; }

        //[JsonProperty("_version__003")]
        //public string Version003 { get; set; }

        [JsonProperty("meta/instanceID")]
        public string MetaInstanceID { get; set; }

        [JsonProperty("_xform_id_string")]
        public string XformIdString { get; set; }

        [JsonProperty("_uuid")]
        public string Uuid { get; set; }

        [JsonProperty("_attachments")]
        public List<object> Attachments { get; set; }

        [JsonProperty("_status")]
        public string Status { get; set; }

        [JsonProperty("_geolocation")]
        public List<object> Geolocation { get; set; }

        [JsonProperty("_submission_time")]
        public string SubmissionTime { get; set; }

        [JsonProperty("_tags")]
        public List<object> Tags { get; set; }

        [JsonProperty("_notes")]
        public List<object> Notes { get; set; }

        [JsonProperty("_validation_status")]
        public ValidationStatus ValidationStatus { get; set; }

        [JsonProperty("_submitted_by")]
        public object SubmittedBy { get; set; }










  

  


    }
}
