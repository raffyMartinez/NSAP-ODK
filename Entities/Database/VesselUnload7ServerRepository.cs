using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
    public class RepeatVesselUnload
    {
        public SamplingAtLandingSite Parent { get; set; }
        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/fishing_vessel_group/sampling_time")]
        public string sampling_time { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/fishing_vessel_group/sampling_date")]
        public DateTime sampling_date { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/fishing_vessel_group/is_boat_used")]
        public string is_boat_used { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/fishing_vessel_group/select_vessel")]
        public string select_vessel { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/fishing_vessel_group/fish_sector")]
        public string fish_sector { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/fishing_vessel_group/vessel_csv_source")]
        public string vessel_csv_source { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/fishing_vessel_group/search_or_showall")]
        public string search_or_showall { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/fishing_vessel_group/search_mode")]
        public string search_mode { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/fishing_vessel_group/search_column")]
        public string search_column { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/fishing_vessel_group/search_vessel")]
        public string search_vessel { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/fishing_vessel_group/search_value")]
        public string search_value { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/fishing_vessel_group/boat_used")]
        public string boat_used { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/fishing_vessel_group/number_of_fishers")]
        public string number_of_fishers { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/fishing_vessel_group/is_gps_used")]
        public string is_gps_used { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/fishing_vessel_group/decimal_sampling_date")]
        public string decimal_sampling_date { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/fishing_vessel_group/sampling_date_string")]
        public string sampling_date_string { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/fishing_vessel_group/boat_name")]
        public string boat_name { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/vessel_catch/trip_isSuccess")]
        public string trip_isSuccess { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/vessel_catch/catch_total")]
        public string catch_total { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/vessel_catch/is_catch_sampled")]
        public string is_catch_sampled { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/vessel_catch/catch_sampled")]
        public string catch_sampled { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/vessel_catch/is_catch_sold")]
        public string is_catch_sold { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/vessel_catch/sample_wt_text")]
        public string sample_wt_text { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/grid_coord_group/include_bingo")]
        public string include_bingo { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/grid_coord_group/utmZone")]
        public string utmZone { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/grid_coord_group/majorgrid_csv")]
        public string majorgrid_csv { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/grid_coord_group/inlandgrid_csv")]
        public string inlandgrid_csv { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/grid_coord_group/bingo_repeat")]
        public List<BingoRepeat> bingo_repeat { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/soak_time_group/include_soak_time")]
        public string include_soak_time { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/soak_time_group/soak_time_repeat")]
        public List<SoakTimeRepeat> soak_time_repeat { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/efforts_group/include_effort")]
        public string include_effort { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/efforts_group/effort_repeat")]
        public List<FishingEffortRepeat> effort_repeat { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/select_length_group/include_catchcomp")]
        public string include_catchcomp { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/select_length_group/length_type")]
        public string length_type { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/select_length_group/has_gms")]
        public string has_gms { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/count_catch_comp")]
        public string count_catch_comp { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat")]
        public List<CatchCompositionRepeat> catch_composition_repeat { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/sum_total")]
        public string sum_total { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/raising_factor")]
        public string raising_factor { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/raising_factor_text")]
        public string raising_factor_text { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/sum_sample")]
        public string sum_sample { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/sum_species_weight")]
        public string sum_species_weight { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/sum_species_weight_coalesce")]
        public string sum_species_weight_coalesce { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/group_final_tally/ref_no")]
        public string group_final_tallyref_no { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/fishing_vessel_group/boat_used_text")]
        public string boat_used_text { get; set; }
    }

    public class CatchCompositionRepeat
    {
        public RepeatVesselUnload Parent { get; set; }
        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/show_taxa_image")]
        public string show_taxa_image { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/taxa_no_im")]
        public string taxa_no_im { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/taxa")]
        public string taxa { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/select_spName")]
        public string select_spName { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/species_csv_source")]
        public string species_csv_source { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/species_notfish")]
        public string species_notfish { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/sp_id")]
        public string sp_id { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/len_max_1")]
        public string len_max_1 { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/len_max")]
        public string len_max { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/species_name_selected")]
        public string species_name_selected { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/size_type_name")]
        public string size_type_name { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/input_total_species_first")]
        public string input_total_species_first { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/from_total_catch")]
        public string from_total_catch { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/species_sample_wt_sampled")]
        public string species_sample_wt_sampled { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/species_wt_raised")]
        public string species_wt_raised { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/species_wt")]
        public string species_wt { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/species_wt_rounded")]
        public string species_wt_rounded { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/measure_len_and_gms")]
        public string measure_len_and_gms { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/type_of_measure")]
        public string type_of_measure { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/what_is_measured")]
        public string what_is_measured { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/max_len_hint")]
        public string max_len_hint { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/enforce_maxlen")]
        public string enforce_maxlen { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/species_sample_wt")]
        public string species_sample_wt { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/species_sample_wt_from_sample")]
        public string species_sample_wt_from_sample { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/from_total_catch_code")]
        public string from_total_catch_code { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/len_max_hint")]
        public string len_max_hint { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/individual_wt_unit")]
        public string individual_wt_unit { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/wt_unit_name")]
        public string wt_unit_name { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/is_species_sold")]
        public string is_species_sold { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/price_of_species")]
        public string price_of_species { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/pricing_unit")]
        public string pricing_unit { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/from_sample_status")]
        public string from_sample_status { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/max_size_hint")]
        public string max_size_hint { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/species_wt_check")]
        public string species_wt_check { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/overwt_prompt")]
        public string overwt_prompt { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/sum_wt_from_lenwt")]
        public string sum_wt_from_lenwt { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/sum_wt_from_lenwt_coalesce")]
        public string sum_wt_from_lenwt_coalesce { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/gms_repeat_group")]
        public List<GmsRepeatGroup> gms_repeat_group { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/sum_wt_from_gms")]
        public string sum_wt_from_gms { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/sum_wt_from_gms_coalesce")]
        public string sum_wt_from_gms_coalesce { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/repeat_title")]
        public string repeat_title { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/search_species")]
        public string search_species { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/species")]
        public string species { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/include_sex_for_length")]
        public string include_sex_for_length { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/length_list_repeat")]
        public List<LengthListRepeat> length_list_repeat { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/species_wt_1")]
        public string species_wt_1 { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/has_measurement")]
        public string has_measurement { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/species_sample_wt_total")]
        public string species_sample_wt_total { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/len_wt_repeat")]
        public List<LenWtRepeat> len_wt_repeat { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/length_freq_repeat")]
        public List<LenFreqRepeat> len_freq_repeat { get; set; }
    }

    public class GmsRepeatGroup
    {
        public CatchCompositionRepeat Parent { get; set; }
        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/gms_repeat_group/gms_group/individual_length")]
        public string individual_length { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/gms_repeat_group/gms_group/individual_weight")]
        public string individual_weight { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/gms_repeat_group/gms_group/sex")]
        public string sex { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/gms_repeat_group/gms_group/gms_repeat")]
        public string gms_repeat { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/gms_repeat_group/gms_group/combined_gms_fields")]
        public string combined_gms_fields { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/gms_repeat_group/gms_group/individual_weight_kg")]
        public string individual_weight_kg { get; set; }
    }

    public class LenFreqRepeat
    {
        public CatchCompositionRepeat Parent { get; set; }
        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/length_freq_repeat/group_LF/length_class")]
        public double length_class { get; set; }
        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/length_freq_repeat/group_LF/freq")]
        public int freq { get; set; }
        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/length_freq_repeat/group_LF/sex_lf")]
        public string sex_lf { get; set; }
    }
    public class LengthListRepeat
    {
        public CatchCompositionRepeat Parent { get; set; }
        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/length_list_repeat/length_list_group/length")]
        public string length { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/length_list_repeat/length_list_group/sex_l")]
        public string sex_l { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/length_list_repeat/length_list_group/ll_grp_title")]
        public string ll_grp_title { get; set; }
    }

    public class LenWtRepeat
    {
        public CatchCompositionRepeat Parent { get; set; }
        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/len_wt_repeat/len_wt_group/len_lenwt")]
        public string len_lenwt { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/len_wt_repeat/len_wt_group/wt_lenwt")]
        public string wt_lenwt { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/len_wt_repeat/len_wt_group/lw_grp_title")]
        public string lw_grp_title { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/catch_comp_group/catch_composition_repeat/speciesname_group/len_wt_repeat/len_wt_group/wt_lenwt_kg")]
        public string wt_lenwt_kg { get; set; }
    }

    public class FishingEffortRepeat
    {
        public RepeatVesselUnload Parent { get; set; }
        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/efforts_group/effort_repeat/group_effort/effort_type")]
        public string effort_type { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/efforts_group/effort_repeat/group_effort/response_type")]
        public string response_type { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/efforts_group/effort_repeat/group_effort/effort_spec_name")]
        public string effort_spec_name { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/efforts_group/effort_repeat/group_effort/effort_intensity")]
        public string effort_intensity { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/efforts_group/effort_repeat/group_effort/selected_effort_measure")]
        public string selected_effort_measure { get; set; }
    }

    public class BingoRepeat
    {
        public RepeatVesselUnload Parent { get; set; }
        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/grid_coord_group/bingo_repeat/bingo_group/major_grid")]
        public string major_grid { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/grid_coord_group/bingo_repeat/bingo_group/col_name")]
        public string col_name { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/grid_coord_group/bingo_repeat/bingo_group/row_name")]
        public string row_name { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/grid_coord_group/bingo_repeat/bingo_group/bingo_complete")]
        public string bingo_complete { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/grid_coord_group/bingo_repeat/bingo_group/is_inland")]
        public string is_inland { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/grid_coord_group/bingo_repeat/bingo_group/group_label")]
        public string group_label { get; set; }
    }

    public class SoakTimeRepeat
    {
        public RepeatVesselUnload Parent { get; set; }
        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/soak_time_group/soak_time_repeat/soaktime_tracking_group/calculate_set_hint")]
        public string calculate_set_hint { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/soak_time_group/soak_time_repeat/soaktime_tracking_group/set_time")]
        public DateTime set_time { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/soak_time_group/soak_time_repeat/soaktime_tracking_group/decimal_set_time")]
        public string decimal_set_time { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/soak_time_group/soak_time_repeat/soaktime_tracking_group/time_set_string")]
        public string time_set_string { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/soak_time_group/soak_time_repeat/soaktime_tracking_group/calculate_haul_hint")]
        public string calculate_haul_hint { get; set; }

        [JsonProperty("repeat_vessel_unload/fishing_vessel_data/soak_time_group/soak_time_repeat/soaktime_tracking_group/haul_time")]
        public DateTime haul_time { get; set; }
    }

    public class SamplingAtLandingSite
    {
        [JsonProperty("_id")]
        public int _id { get; set; }

        [JsonProperty("formhub/uuid")]
        public string formhubuuid { get; set; }

        [JsonProperty("start")]
        public DateTime start { get; set; }

        [JsonProperty("today")]
        public string today { get; set; }

        [JsonProperty("device_id")]
        public string device_id { get; set; }

        [JsonProperty("user_name")]
        public string user_name { get; set; }

        [JsonProperty("email")]
        public string email { get; set; }

        [JsonProperty("intronote")]
        public string intronote { get; set; }

        [JsonProperty("landing_site_sampling/landing_date")]
        public string landing_date { get; set; }

        [JsonProperty("landing_site_sampling/landing_date_string")]
        public string landing_date_string { get; set; }

        [JsonProperty("landing_site_sampling/is_sampling_day")]
        public string is_sampling_day { get; set; }

        [JsonProperty("landing_site_sampling/are_fishing_boats_landing")]
        public string are_fishing_boats_landing { get; set; }

        [JsonProperty("landing_site_sampling/nsap_region")]
        public string nsap_region { get; set; }

        [JsonProperty("landing_site_sampling/select_enumerator")]
        public string select_enumerator { get; set; }

        [JsonProperty("landing_site_sampling/region_enumerator")]
        public string region_enumerator { get; set; }

        [JsonProperty("landing_site_sampling/fma_in_region")]
        public string fma_in_region { get; set; }

        [JsonProperty("landing_site_sampling/fishing_ground")]
        public string fishing_ground { get; set; }

        [JsonProperty("landing_site_sampling/select_landingsite")]
        public string select_landingsite { get; set; }

        [JsonProperty("landing_site_sampling/landing_site")]
        public string landing_site { get; set; }

        [JsonProperty("landing_site_sampling/select_gear")]
        public string select_gear { get; set; }

        [JsonProperty("landing_site_sampling/gear_used")]
        public string gear_used { get; set; }

        [JsonProperty("landing_site_sampling/gear_code")]
        public string gear_code { get; set; }

        [JsonProperty("landing_site_sampling/gear_name")]
        public string gear_name { get; set; }

        [JsonProperty("fma_number")]
        public string fma_number { get; set; }

        [JsonProperty("fishing_ground_name")]
        public string fishing_ground_name { get; set; }

        [JsonProperty("landing_site_name")]
        public string landing_site_name { get; set; }

        [JsonProperty("group_vessel_landing_counts/count_total_landing")]
        public string count_total_landing { get; set; }

        [JsonProperty("group_vessel_landing_counts/count_total_sampled")]
        public string count_total_sampled { get; set; }

        [JsonProperty("group_vessel_landing_counts/total_weight_landing")]
        public string total_weight_landing { get; set; }

        [JsonProperty("repeat_vessel_unload")]
        public List<RepeatVesselUnload> repeat_vessel_unload { get; set; }

        [JsonProperty("__version__")]
        public string __version__ { get; set; }

        [JsonProperty("meta/instanceID")]
        public string metainstanceID { get; set; }

        [JsonProperty("meta/instanceName")]
        public string metainstanceName { get; set; }

        [JsonProperty("_xform_id_string")]
        public string _xform_id_string { get; set; }

        [JsonProperty("_uuid")]
        public string _uuid { get; set; }

        [JsonProperty("_attachments")]
        public List<object> _attachments { get; set; }

        [JsonProperty("_status")]
        public string _status { get; set; }

        [JsonProperty("_geolocation")]
        public List<object> _geolocation { get; set; }

        [JsonProperty("_submission_time")]
        public DateTime _submission_time { get; set; }

        [JsonProperty("_tags")]
        public List<object> _tags { get; set; }

        [JsonProperty("_notes")]
        public List<object> _notes { get; set; }

        [JsonProperty("_validation_status")]
        public ValidationStatus _validation_status { get; set; }

        [JsonProperty("_submitted_by")]
        public object _submitted_by { get; set; }
    }




    class VesselUnload7ServerRepository
    {
        public static string JSON { get; set; }
        public static List<SamplingAtLandingSite> LandingSiteSamplings { get; internal set; }

        public static void CreateLandingSiteSamplingsFromJSON()
        {
            //How are PKs assigned to each landings contained in each incoming batch of JSON?
            //call VesselLanding.SetRowIDs()

            //VesselLanding.SetRowIDs();
            try
            {
                LandingSiteSamplings = JsonConvert.DeserializeObject<List<SamplingAtLandingSite>>(JSON);
            }
            catch (Exception ex)
            {
                Utilities.Logger.Log(ex);
            }
        }
    }
}
