using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
    public class EForm7LSS
    {
        public int PK { get; set; }
        [JsonProperty("_id")]
        public int? Id { get; set; }

        [JsonProperty("formhub/uuid")]
        public string FormhubUuid { get; set; }

        [JsonProperty("start")]
        public DateTime? Start { get; set; }

        [JsonProperty("today")]
        public string Today { get; set; }

        [JsonProperty("device_id")]
        public string DeviceId { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("intronote")]
        public string Intronote { get; set; }

        [JsonProperty("g_lss/landing_date")]
        public DateTime LandingDate { get; set; }

        //[JsonProperty("g_lss/landing_date_string")]
        //public string LandingDateString { get; set; }

        [JsonProperty("g_lss/is_sampling_day")]
        public string IsSamplingDay { get; set; }

        [JsonProperty("g_lss/are_fishing_boats_landing")]
        public string AreFishingBoatsLanding { get; set; }

        [JsonProperty("g_lss/nsap_region")]
        public string NsapRegion { get; set; }

        [JsonProperty("g_lss/select_enumerator")]
        public string SelectEnumerator { get; set; }

        [JsonProperty("g_lss/region_enumerator")]
        public int? RegionEnumerator { get; set; }

        [JsonProperty("g_lss/region_enumerator_text")]
        public string EnumeratorText { get; set; }


        [JsonProperty("g_lss/fma_in_region")]
        public int FmaInRegion { get; set; }

        [JsonProperty("g_lss/fishing_ground")]
        public string FishingGround { get; set; }

        [JsonProperty("g_lss/select_landingsite")]
        public string SelectLandingsite { get; set; }

        [JsonProperty("g_lss/landing_site")]
        public string LandingSite { get; set; }

        [JsonProperty("g_lss/select_gear")]
        public string SelectGear { get; set; }

        [JsonProperty("g_lss/gear_used")]
        public string GearUsed { get; set; }

        [JsonProperty("g_lss/gear_code")]
        public string GearCode { get; set; }

        [JsonProperty("g_lss/gear_name")]
        public string GearName { get; set; }

        [JsonProperty("fma_number")]
        public string FmaNumber { get; set; }

        [JsonProperty("fishing_ground_name")]
        public string FishingGroundName { get; set; }

        [JsonProperty("landing_site_name")]
        public string LandingSiteName { get; set; }

        [JsonProperty("g_counts/count_total_landing")]
        public int? CountTotalLanding { get; set; }

        [JsonProperty("g_counts/count_total_sampled")]
        public int? CountTotalSampled { get; set; }

        [JsonProperty("g_counts/total_weight_landing")]
        public string TotalWeightLanding { get; set; }

        [JsonProperty("r_vu_count")]
        public string RVuCount { get; set; }

        [JsonProperty("r_vu")]
        public List<EForm7VU> VU_Repeat { get; set; }

        [JsonProperty("__version__")]
        public string Version { get; set; }

        [JsonProperty("meta/instanceID")]
        public string MetaInstanceID { get; set; }

        [JsonProperty("meta/instanceName")]
        public string MetaInstanceName { get; set; }

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
        public DateTime? SubmissionTime { get; set; }

        [JsonProperty("_tags")]
        public List<object> Tags { get; set; }

        [JsonProperty("_notes")]
        public List<object> Notes { get; set; }

        [JsonProperty("_validation_status")]
        public ValidationStatus ValidationStatus { get; set; }

        [JsonProperty("_submitted_by")]
        public object SubmittedBy { get; set; }
    }

    public class EForm7VU
    {
        [JsonProperty("r_vu/g_fv/g_fv1/sampling_time")]
        public string SamplingTime { get; set; }

        [JsonProperty("r_vu/g_fv/g_fv1/sampling_date")]
        public DateTime? SamplingDate { get; set; }

        [JsonProperty("r_vu/g_fv/g_fv1/is_boat_used")]
        public string IsBoatUsed { get; set; }

        [JsonProperty("r_vu/g_fv/g_fv1/select_vessel")]
        public string SelectVessel { get; set; }

        [JsonProperty("r_vu/g_fv/g_fv1/fish_sector")]
        public string FishSector { get; set; }

        [JsonProperty("r_vu/g_fv/g_fv1/vessel_csv_source")]
        public string VesselCsvSource { get; set; }

        [JsonProperty("r_vu/g_fv/g_fv1/search_or_showall")]
        public string SearchOrShowall { get; set; }

        [JsonProperty("r_vu/g_fv/g_fv1/search_mode")]
        public string SearchMode { get; set; }

        [JsonProperty("r_vu/g_fv/g_fv1/search_column")]
        public string SearchColumn { get; set; }

        [JsonProperty("r_vu/g_fv/g_fv1/search_vessel")]
        public string SearchVessel { get; set; }

        [JsonProperty("r_vu/g_fv/g_fv1/search_value")]
        public string SearchValue { get; set; }

        [JsonProperty("r_vu/g_fv/g_fv1/boat_used")]
        public string BoatUsed { get; set; }

        [JsonProperty("r_vu/g_fv/g_fv1/number_of_fishers")]
        public string NumberOfFishers { get; set; }

        [JsonProperty("r_vu/g_fv/g_fv1/is_gps_used")]
        public string IsGpsUsed { get; set; }

        [JsonProperty("r_vu/g_fv/g_fv1/decimal_sampling_date")]
        public string DecimalSamplingDate { get; set; }

        [JsonProperty("r_vu/g_fv/g_fv1/sampling_date_string")]
        public string SamplingDateString { get; set; }

        [JsonProperty("r_vu/g_fv/g_fv1/boat_name")]
        public string BoatName { get; set; }

        [JsonProperty("r_vu/g_fv/g_catch/trip_isSuccess")]
        public string TripIsSuccess { get; set; }

        [JsonProperty("r_vu/g_fv/g_catch/catch_total")]
        public string CatchTotal { get; set; }

        [JsonProperty("r_vu/g_fv/g_catch/boxes_total")]
        public string BoxesTotal { get; set; }

        [JsonProperty("r_vu/g_fv/g_catch/is_catch_sampled")]
        public string IsCatchSampled { get; set; }

        [JsonProperty("r_vu/g_fv/g_catch/catch_sampled")]
        public string CatchSampled { get; set; }

        [JsonProperty("r_vu/g_fv/g_catch/boxes_sampled")]
        public string BoxesSampled { get; set; }

        [JsonProperty("r_vu/g_fv/g_catch/is_catch_sold")]
        public string IsCatchSold { get; set; }

        [JsonProperty("r_vu/g_fv/g_catch/remarks_normal_operation")]
        public string RemarksNormalOperation { get; set; }

        [JsonProperty("r_vu/g_fv/g_catch/remarks")]
        public string Remarks { get; set; }

        [JsonProperty("r_vu/g_fv/g_catch/sample_wt_text")]
        public string SampleWtText { get; set; }

        [JsonProperty("r_vu/g_fv/g_coord/include_bingo")]
        public string IncludeBingo { get; set; }

        [JsonProperty("r_vu/g_fv/g_coord/utmZone")]
        public string UtmZone { get; set; }

        [JsonProperty("r_vu/g_fv/g_coord/majorgrid_csv")]
        public string MajorgridCsv { get; set; }

        [JsonProperty("r_vu/g_fv/g_coord/inlandgrid_csv")]
        public string InlandgridCsv { get; set; }

        [JsonProperty("r_vu/g_fv/g_coord/r_coord")]
        public List<EForm7Coord> Coord_Repeat { get; set; }

        [JsonProperty("r_vu/g_fv/g_soaking/include_soak_time")]
        public string IncludeSoakTime { get; set; }

        [JsonProperty("r_vu/g_fv/g_soaking/r_soaking")]
        public List<EForm7Soak> Soaking_Repeat { get; set; }

        [JsonProperty("r_vu/g_fv/g_effort/include_effort")]
        public string IncludeEffort { get; set; }

        [JsonProperty("r_vu/g_fv/g_effort/r_effort")]
        public List<EForm7Effort> Effort_Repeat { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/g_measure/include_catchcomp")]
        public string IncludeCatchcomp { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/g_measure/species_count")]
        public string SpeciesCount { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/g_measure/length_type")]
        public string LengthType { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/g_measure/has_gms")]
        public string HasGms { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/count_catch_comp")]
        public string CountCatchComp { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc_count")]
        public string RCcCount { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc")]
        public List<EForm7CC> CC_Repeat { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/sum_total")]
        public string SumTotal { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/raising_factor")]
        public string RaisingFactor { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/raising_factor_text")]
        public string RaisingFactorText { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/sum_sample")]
        public string SumSample { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/sum_species_weight")]
        public string SumSpeciesWeight { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/sum_species_weight_coalesce")]
        public string SumSpeciesWeightCoalesce { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/g_summary/ref_no")]
        public string GSummaryRefNo { get; set; }

        [JsonProperty("r_vu/g_fv/g_fv1/boat_used_text")]
        public string BoatUsedText { get; set; }
    }

    public class EForm7CC
    {
        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/show_taxa_image")]
        public string ShowTaxaImage { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/taxa_no_im")]
        public string TaxaNoIm { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/taxa")]
        public string Taxa { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/select_spName")]
        public string SelectSpName { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/species_csv_source")]
        public string SpeciesCsvSource { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/search_species")]
        public string SearchSpecies { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/species")]
        public string Species { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/sp_id")]
        public string SpId { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/len_max_1")]
        public string LenMax1 { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/len_max")]
        public string LenMax { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/species_name_selected")]
        public string SpeciesNameSelected { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/size_type_name")]
        public string SizeTypeName { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/input_total_species_first")]
        public string InputTotalSpeciesFirst { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/from_total_catch")]
        public string FromTotalCatch { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/species_sample_wt_sampled")]
        public string SpeciesSampleWtSampled { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/species_wt_raised")]
        public string SpeciesWtRaised { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/species_wt")]
        public string SpeciesWt { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/species_wt_rounded")]
        public string SpeciesWtRounded { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/measure_len_and_gms")]
        public string MeasureLenAndGms { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/type_of_measure")]
        public string TypeOfMeasure { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/what_is_measured")]
        public string WhatIsMeasured { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/max_len_hint")]
        public string MaxLenHint { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/enforce_maxlen")]
        public string EnforceMaxlen { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/species_sample_wt")]
        public string SpeciesSampleWt { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/species_sample_wt_from_sample")]
        public string SpeciesSampleWtFromSample { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/from_total_catch_code")]
        public string FromTotalCatchCode { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/include_sex_for_length")]
        public string IncludeSexForLength { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/len_max_hint")]
        public string LenMaxHint { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/wt_unit_name")]
        public string WtUnitName { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/is_species_sold")]
        public string IsSpeciesSold { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/price_of_species")]
        public string PriceOfSpecies { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/pricing_unit")]
        public string PricingUnit { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/other_pricing_unit")]
        public string OtherPricingUnit { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/from_sample_status")]
        public string FromSampleStatus { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/max_size_hint")]
        public string MaxSizeHint { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/species_wt_check")]
        public string SpeciesWtCheck { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/overwt_prompt")]
        public string OverwtPrompt { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/r_ll")]
        public List<EForm7SpeciesLL> LL_Repeat { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/sum_wt_from_lenwt")]
        public string SumWtFromLenwt { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/sum_wt_from_lenwt_coalesce")]
        public string SumWtFromLenwtCoalesce { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/sum_wt_from_gms")]
        public string SumWtFromGms { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/sum_wt_from_gms_coalesce")]
        public string SumWtFromGmsCoalesce { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/repeat_title")]
        public string RepeatTitle { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/species_notfish")]
        public string SpeciesNotfish { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/species_wt_1")]
        public string SpeciesWt1 { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/species_wt_total")]
        public string SpeciesWtTotal { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/species_sample_wt_total")]
        public string SpeciesSampleWtTotal { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/individual_wt_unit")]
        public string IndividualWtUnit { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/r_gms")]
        public List<EForm7SpeciesGMS> GMS_Repeat { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/spName_other")]
        public string SpNameOther { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/g_species_id/has_measurement")]
        public string HasMeasurement { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/r_lw")]
        public List<EForm7SpeciesLW> LW_Repeat { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/r_lf")]
        public List<EForm7SpeciesLF> LF_Repeat { get; set; }
    }

    public class EForm7SpeciesGMS
    {
        public EForm7CC Parent { get; set; }
        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/r_gms/g_gms/individual_length")]
        public double? IndividualLength { get; set; }

        //[JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/r_gms/g_gms/individual_weight")]
        //public double? IndividualWeight { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/r_gms/g_gms/sex")]
        public string Sex { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/r_gms/g_gms/gms_repeat")]
        public string GMS { get; set; }

        //[JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/r_gms/g_gms/combined_gms_fields")]
        //public string CombinedGmsFields { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/r_gms/g_gms/individual_weight_kg")]
        public double? IndividualWeightKg { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/r_gms/g_gms/gonad_wt")]
        public double? GonadWt { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/r_gms/g_gms/stomach_content_wt")]
        public double? StomachContentWt { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/r_gms/g_gms/gut_content_category")]
        public string GutContentCategory { get; set; }
    }

    public class EForm7SpeciesLF
    {
        public int PK { get; set; }
        public EForm7CC Parent { get; set; }
        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/r_lf/g_lf/length_class")]
        public double LengthClass { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/r_lf/g_lf/freq")]
        public int Freq { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/r_lf/g_lf/sex_lf")]
        public string Sex { get; set; }

        //[JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/r_lf/g_lf/lf_grp_title")]
        //public string LfGrpTitle { get; set; }
    }

    public class EForm7SpeciesLL
    {
        public int PK { get; set; }
        public EForm7CC Parent { get; set; }
        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/r_ll/g_ll/length")]
        public double Length { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/r_ll/g_ll/sex_l")]
        public string Sex { get; set; }

        //[JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/r_ll/g_ll/ll_grp_title")]
        //public string LlGrpTitle { get; set; }
    }

    public class EForm7SpeciesLW
    {
        public int PK { get; set; }
        public EForm7CC Parent { get; set; }
        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/r_lw/g_lw/len_lenwt")]
        public double Length { get; set; }

        //[JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/r_lw/g_lw/wt_lenwt")]
        //public string Weight { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/r_lw/g_lw/sex_lw")]
        public string Sex { get; set; }

        //[JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/r_lw/g_lw/lw_grp_title")]
        //public string LwGrpTitle { get; set; }

        [JsonProperty("r_vu/g_fv/g_cc/r_cc/g_species/r_lw/g_lw/wt_lenwt_kg")]
        public double WeightKg { get; set; }
    }

    public class EForm7Coord
    {
        public int PK { get; set; }
        public EForm7VU Parent { get; set; }

        [JsonProperty("r_vu/g_fv/g_coord/r_coord/g_coord1/bingo_complete")]
        public string BingoComplete { get; set; }
        [JsonProperty("r_vu/g_fv/g_coord/r_coord/g_coord1/is_inland")]
        public string IsInland { get; set; }
        //[JsonProperty("r_vu/g_fv/g_coord/r_coord/g_coord1/major_grid")]
        //public string MajorGrid { get; set; }

        //[JsonProperty("r_vu/g_fv/g_coord/r_coord/g_coord1/col_name")]
        //public string ColName { get; set; }

        //[JsonProperty("r_vu/g_fv/g_coord/r_coord/g_coord1/row_name")]
        //public string RowName { get; set; }

        //[JsonProperty("r_vu/g_fv/g_coord/r_coord/g_coord1/group_label")]
        //public string GroupLabel { get; set; }
    }

    public class EForm7Effort
    {
        public int PK { get; set; }
        public EForm7VU Parent { get; set; }
        [JsonProperty("r_vu/g_fv/g_effort/r_effort/g_effort1/effort_type")]
        public int EffortType { get; set; }

        [JsonProperty("r_vu/g_fv/g_effort/r_effort/g_effort1/response_type")]
        public string ResponseType { get; set; }

        [JsonProperty("r_vu/g_fv/g_effort/r_effort/g_effort1/effort_spec_name")]
        public string EffortSpecName { get; set; }

        [JsonProperty("r_vu/g_fv/g_effort/r_effort/g_effort1/effort_intensity")]
        public double? EffortIntensity { get; set; }

        [JsonProperty("r_vu/g_fv/g_effort/r_effort/g_effort1/effort_desc")]
        public string EffortDescrription { get; set; }

        [JsonProperty("r_vu/g_fv/g_effort/r_effort/g_effort1/effort_bool")]
        public string EffortYesNo { get; set; }

        [JsonProperty("r_vu/g_fv/g_effort/r_effort/g_effort1/selected_effort_measure")]
        public string SelectedEffortMeasure { get; set; }
    }

    public class EForm7Soak
    {
        public int PK { get; set; }
        public EForm7VU Parent { get; set; }
        [JsonProperty("r_vu/g_fv/g_soaking/r_soaking/g_soaking1/set_time")]
        public DateTime? SetTime { get; set; }
        [JsonProperty("r_vu/g_fv/g_soaking/r_soaking/g_soaking1/haul_time")]
        public DateTime? HaulTime { get; set; }
        //[JsonProperty("r_vu/g_fv/g_soaking/r_soaking/g_soaking1/calculate_set_hint")]
        //public string RVuGFvGSoakingRSoakingGSoaking1CalculateSetHint { get; set; }
        //[JsonProperty("r_vu/g_fv/g_soaking/r_soaking/g_soaking1/decimal_set_time")]
        //public string RVuGFvGSoakingRSoakingGSoaking1DecimalSetTime { get; set; }

        //[JsonProperty("r_vu/g_fv/g_soaking/r_soaking/g_soaking1/time_set_string")]
        //public string RVuGFvGSoakingRSoakingGSoaking1TimeSetString { get; set; }

        //[JsonProperty("r_vu/g_fv/g_soaking/r_soaking/g_soaking1/calculate_haul_hint")]
        //public string RVuGFvGSoakingRSoakingGSoaking1CalculateHaulHint { get; set; }



        //[JsonProperty("r_vu/g_fv/g_soaking/r_soaking/g_soaking1/time_haul_string")]
        //public string RVuGFvGSoakingRSoakingGSoaking1TimeHaulString { get; set; }

        //[JsonProperty("r_vu/g_fv/g_soaking/r_soaking/g_soaking1/soaking_label")]
        //public string RVuGFvGSoakingRSoakingGSoaking1SoakingLabel { get; set; }
    }









    class VesselUnload7ServerRepository
    {
        public static string JSON { get; set; }
        public static List<EForm7LSS> LandingSiteSamplings { get; internal set; }

        public static void CreateLandingSiteSamplingsFromJSON()
        {
            //How are PKs assigned to each landings contained in each incoming batch of JSON?
            //call VesselLanding.SetRowIDs()

            //VesselLanding.SetRowIDs();
            try
            {
                LandingSiteSamplings = JsonConvert.DeserializeObject<List<EForm7LSS>>(JSON);
            }
            catch (Exception ex)
            {
                Utilities.Logger.Log(ex);
            }
        }
    }
}
