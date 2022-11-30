using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    class XLSForm2
    {
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

    public class AdvancedFeatures
    {
    }

    public class AdvancedSubmissionSchema
    {
        public string type { get; set; }

        [JsonProperty("$description")]
        public string description { get; set; }
    }

    public class AnalysisFormJson
    {
        public Engines engines { get; set; }
        public List<object> additional_fields { get; set; }
    }

    public class ArriveDateString
    {
    }

    public class AssignablePermission
    {
        public string url { get; set; }
        public object label { get; set; }
    }


    public class BingoComplete
    {
    }

    public class BingoGroup
    {
    }

    public class BingoRepeat
    {
    }

    public class BoatName
    {
    }

    public class BoatUsed
    {
    }

    public class BoatUsedText
    {
    }

    public class BoxesSampled
    {
    }

    public class BoxesTotal
    {
    }


    public class CalculateHaulHint
    {
    }

    public class CalculateSetHint
    {
    }

    public class CatchCompGroup
    {
    }

    public class CatchCompositionRepeat
    {
    }

    public class CatchSampled
    {
    }

    public class CatchTotal
    {
    }

    public class Cd8dhwymm
    {
    }

    public class Children
    {
        public int count { get; set; }
    }

    public class Choice
    {
        public string name { get; set; }

        [JsonProperty("$kuid")]
        public string kuid { get; set; }
        public List<string> label { get; set; }
        public string list_name { get; set; }

        [JsonProperty("$autovalue")]
        public string autovalue { get; set; }

        [JsonProperty("media::image")]
        public List<string> mediaimage { get; set; }
    }

    public class ColName
    {
    }

    public class CombinedGmsFields
    {
    }

    public class Content
    {
        public string schema { get; set; }
        public List<Survey> survey { get; set; }
        public List<Choice> choices { get; set; }
        public Settings settings { get; set; }
        public List<string> translated { get; set; }
        public List<object> translations { get; set; }
    }

    public class CountCatchComp
    {
    }

    public class Country
    {
        public string label { get; set; }
        public string value { get; set; }
    }

    public class DataSharing
    {
    }

    public class DecimalArriveDate
    {
    }

    public class DecimalDepartDate
    {
    }

    public class DecimalSamplingDate
    {
    }

    public class DecimalSetTime
    {
    }

    public class Default
    {
    }

    public class DepartDateString
    {
    }

    public class DeployedVersions
    {
        public int count { get; set; }
        public object next { get; set; }
        public object previous { get; set; }
        public List<Result> results { get; set; }
    }

    public class DeploymentDataDownloadLinks
    {
        public string xls_legacy { get; set; }
        public string csv_legacy { get; set; }
        public string zip_legacy { get; set; }
        public string kml_legacy { get; set; }
        public string xls { get; set; }
        public string csv { get; set; }
    }

    public class DeploymentLinks
    {
        public string url { get; set; }
        public string single_url { get; set; }
        public string single_once_url { get; set; }
        public string offline_url { get; set; }
        public string preview_url { get; set; }
        public string iframe_url { get; set; }
        public string single_iframe_url { get; set; }
        public string single_once_iframe_url { get; set; }
    }

    public class DeviceId
    {
    }


    public class Download
    {
        public string format { get; set; }
        public string url { get; set; }
    }

    public class EffortBool
    {
    }

    public class EffortDesc
    {
    }

    public class EffortIntensity
    {
    }

    public class EffortRepeat
    {
    }

    public class EffortsGroup
    {
    }

    public class EffortSpecName
    {
    }

    public class EffortType
    {
    }

    public class Email
    {
    }

    public class Embed
    {
        public string format { get; set; }
        public string url { get; set; }
    }

    public class EnforceMaxlen
    {
    }

    public class Engines
    {
    }


    public class ExceedCatchWeight1
    {
    }

    public class ExceedLenwtTotal
    {
    }

    public class ExceedWtGmsTotal
    {
    }


    public class Firsts
    {
    }

    public class FishingGround
    {
    }

    public class FishingGroundName
    {
    }

    public class FishingVesselGroup
    {
    }

    public class FishSector
    {
    }

    public class FmaInRegion
    {
    }

    public class FmaNumber
    {
    }

    public class Freq
    {
    }

    public class FromSampleStatus
    {
    }

    public class FromTotalCatch
    {
    }

    public class FromTotalCatchCode
    {
    }


    public class GearCode
    {
    }

    public class GearName
    {
    }

    public class GearUsed
    {
    }

    public class GearUsedText
    {
    }


    public class GmsGroup
    {
    }

    public class GmsRepeat
    {
    }

    public class GmsRepeatGroup
    {
    }

    public class GonadWt
    {
    }

    public class Gps2
    {
    }

    public class GridCoordGroup
    {
    }

    public class GroupEffort
    {
    }

    public class GroupFinalTally
    {
    }

    public class GroupLabel
    {
    }

    public class GroupLF
    {
    }

    public class GutContentCategory
    {
    }


    public class HasGms
    {
    }

    public class HasGmsG
    {
    }

    public class HasMeasurement
    {
    }

    public class HaulTime
    {
    }


    public class IncludeBingo
    {
    }

    public class IncludeCatchcomp
    {
    }

    public class IncludeCatchcompG
    {
    }

    public class IncludeEffort
    {
    }

    public class IncludeSexForLength
    {
    }

    public class IncludeSoakTime
    {
    }

    public class IndividualLength
    {
    }

    public class IndividualWeight
    {
    }

    public class IndividualWeightKg
    {
    }

    public class IndividualWtUnit
    {
    }

    public class InlandgridCsv
    {
    }

    public class InlandGridNote
    {
    }

    public class InputTotalSpeciesFirst
    {
    }

    public class Intronote
    {
    }

    public class IsBoatUsed
    {
    }

    public class IsCatchSampled
    {
    }

    public class IsGpsUsed
    {
    }

    public class IsInland
    {
    }




    public class LandingSite
    {
    }

    public class LandingSiteName
    {
    }

    public class LandingSiteText
    {
    }


    public class Length
    {
    }

    public class LengthClass
    {
    }

    public class LengthFreqRepeat
    {
    }

    public class LengthListGroup
    {
    }

    public class LengthListRepeat
    {
    }

    public class LengthType
    {
    }

    public class LengthTypeG
    {
    }

    public class LenLenwt
    {
    }

    public class LenMax
    {
    }

    public class LenMax1
    {
    }

    public class LenMaxHint
    {
    }

    public class LenWtGroup
    {
    }

    public class LenWtRepeat
    {
    }

    public class LfGrpTitle
    {
    }

    public class LlGrpTitle
    {
    }


    public class LwGrpTitle
    {
    }

    public class MajorGrid
    {
    }

    public class MajorgridCsv
    {
    }

    public class MapCustom
    {
    }

    public class MapStyles
    {
    }

    public class MaxLenHint
    {
    }

    public class MaxSizeHint
    {
    }

    public class MeasureLenAndGms
    {
    }


    public class NameQuality
    {
        public int ok { get; set; }
        public int bad { get; set; }
        public int good { get; set; }
        public int total { get; set; }
        public Firsts firsts { get; set; }
    }

    public class NsapRegion
    {
    }


    public class NumberOfFishers
    {
    }


    public class OverweightWarningFinal
    {
    }

    public class OverwtPrompt
    {
    }

    public class Permission
    {
        public string url { get; set; }
        public string user { get; set; }
        public string permission { get; set; }
        public string label { get; set; }
    }

 
    public class RaisingFactor
    {
    }

    public class RaisingFactorText
    {
    }


    public class RefNo
    {
    }

    public class RefNoNote
    {
    }

    public class RegionEnumerator
    {
    }

    public class RegionEnumeratorText
    {
    }

    public class Remarks
    {
    }

    public class RemarksNormalOperation
    {
    }

    public class RemarksNotCompleted
    {
    }

    public class RepeatTitle
    {
    }

    public class ReportCustom
    {
    }

    public class ReportStyles
    {
        public Default @default { get; set; }
        public Specified specified { get; set; }
    }

    public class ResponseType
    {
    }

    public class Result
    {
        public string uid { get; set; }
        public string url { get; set; }
        public string content_hash { get; set; }
        public DateTime date_deployed { get; set; }
        public string date_modified { get; set; }
    }


    public class Root
    {
        public string url { get; set; }
        public string owner { get; set; }
        public string owner__username { get; set; }
        public object parent { get; set; }
        public Settings settings { get; set; }
        public string asset_type { get; set; }
        public DateTime date_created { get; set; }
        public Summary summary { get; set; }
        public DateTime date_modified { get; set; }
        public string version_id { get; set; }
        public string version__content_hash { get; set; }
        public int version_count { get; set; }
        public bool has_deployment { get; set; }
        public string deployed_version_id { get; set; }
        public DeployedVersions deployed_versions { get; set; }
        public string deployment__identifier { get; set; }
        public DeploymentLinks deployment__links { get; set; }
        public bool deployment__active { get; set; }
        public DeploymentDataDownloadLinks deployment__data_download_links { get; set; }
        public int deployment__submission_count { get; set; }
        //public ReportStyles report_styles { get; set; }
        public ReportCustom report_custom { get; set; }
        public AdvancedFeatures advanced_features { get; set; }
        public AdvancedSubmissionSchema advanced_submission_schema { get; set; }
        public AnalysisFormJson analysis_form_json { get; set; }
        public MapStyles map_styles { get; set; }
        public MapCustom map_custom { get; set; }
        public Content content { get; set; }
        public List<Download> downloads { get; set; }
        public List<Embed> embeds { get; set; }
        public string koboform_link { get; set; }
        public string xform_link { get; set; }
        public string hooks_link { get; set; }
        public string tag_string { get; set; }
        public string uid { get; set; }
        public string kind { get; set; }
        public string xls_link { get; set; }
        public string name { get; set; }
        public List<AssignablePermission> assignable_permissions { get; set; }
        public List<Permission> permissions { get; set; }
        public string exports { get; set; }
        public List<object> export_settings { get; set; }
        public string data { get; set; }
        public Children children { get; set; }
        public int subscribers_count { get; set; }
        public string status { get; set; }
        public object access_types { get; set; }
        public DataSharing data_sharing { get; set; }
        public string paired_data { get; set; }
    }

    public class RowName
    {
    }


    public class SampleWtText
    {
    }

    public class SamplingDate
    {
    }

    public class SamplingDateString
    {
    }

    public class SearchColumn
    {
    }

    public class SearchMode
    {
    }

    public class SearchOrShowall
    {
    }

    public class SearchSpecies
    {
    }

    public class SearchValue
    {
    }

    public class SearchVessel
    {
    }

    public class Sector
    {
        public string label { get; set; }
        public string value { get; set; }
    }

    public class SelectedEffortMeasure
    {
    }

    public class SelectEnumerator
    {
    }

    public class SelectGear
    {
    }

    public class SelectLandingsite
    {
    }

    public class SelectLengthGroup
    {
    }

    public class SelectSpName
    {
    }

    public class SelectVessel
    {
    }

    public class SetTime
    {
    }

    public class Settings
    {
        public Sector sector { get; set; }
        public List<Country> country { get; set; }
        public string style { get; set; }
        public string version { get; set; }
        public string id_string { get; set; }
        public string instance_name { get; set; }
        public string allow_choice_duplicates { get; set; }
    }

    public class Sex
    {
    }

    public class SexL
    {
    }

    public class SexLf
    {
    }

    public class SexLw
    {
    }

    public class ShowTaxaImage
    {
    }

    public class SizeTypeName
    {
    }

    public class SoakTimeGroup
    {
    }

    public class SoakTimeRepeat
    {
    }

    public class SoaktimeTrackingGroup
    {
    }

    public class Species
    {
    }

    public class SpeciesAndWt
    {
    }

    public class SpeciesAndWt1
    {
    }

    public class SpeciesCsvSource
    {
    }

    public class SpeciesDataGroup
    {
    }

    public class SpeciesnameGroup
    {
    }

    public class SpeciesNameSelected
    {
    }

    public class SpeciesNotfish
    {
    }

    public class SpeciesSampleWt
    {
    }

    public class SpeciesSampleWtFromSample
    {
    }

    public class SpeciesSampleWtSampled
    {
    }

    public class SpeciesSampleWtTotal
    {
    }

    public class SpeciesWt
    {
    }

    public class SpeciesWt1
    {
    }

    public class SpeciesWtCheck
    {
    }

    public class SpeciesWtRaised
    {
    }

    public class SpeciesWtRaisedNote
    {
    }

    public class SpeciesWtRounded
    {
    }

    public class SpeciesWtTotal
    {
    }

    public class Specified
    {
        public Sex sex { get; set; }
        public Freq freq { get; set; }
        public Gps2 gps2 { get; set; }
        public Taxa taxa { get; set; }
        public Email email { get; set; }
        public SexL sex_l { get; set; }
        public SpId sp_id { get; set; }
        public Start start { get; set; }
        public Today today { get; set; }
        public Length length { get; set; }
        public RefNo ref_no { get; set; }
        public SexLf sex_lf { get; set; }
        public SexLw sex_lw { get; set; }
        public HasGms has_gms { get; set; }
        public LenMax len_max { get; set; }
        public Remarks remarks { get; set; }
        public Species species { get; set; }
        public TaxaIm taxa_im { get; set; }
        public UtmZone utmZone { get; set; }
        public WptSet wpt_set { get; set; }
        public ColName col_name { get; set; }
        public GonadWt gonad_wt { get; set; }
        public GroupLF group_LF { get; set; }
        public RowName row_name { get; set; }
        public SetTime set_time { get; set; }
        public WptHaul wpt_haul { get; set; }
        public WtLenwt wt_lenwt { get; set; }

        
        public BoatName boat_name { get; set; }
        public BoatUsed boat_used { get; set; }
        public DeviceId device_id { get; set; }
        public GearCode gear_code { get; set; }
        public GearName gear_name { get; set; }
        public GearUsed gear_used { get; set; }
        public GmsGroup gms_group { get; set; }
        public HasGmsG has_gms_g { get; set; }
        public HaulTime haul_time { get; set; }
        public Intronote intronote { get; set; }
        public IsInland is_inland { get; set; }
        public LenLenwt len_lenwt { get; set; }
        public LenMax1 len_max_1 { get; set; }
        public SumTotal sum_total { get; set; }
        public UserName user_name { get; set; }
        public FmaNumber fma_number { get; set; }
        public GmsRepeat gms_repeat { get; set; }
        public MajorGrid major_grid { get; set; }
        public SpeciesWt species_wt { get; set; }
        public SumSample sum_sample { get; set; }
        public TaxaNoIm taxa_no_im { get; set; }
        public BingoGroup bingo_group { get; set; }
        public BoxesTotal boxes_total { get; set; }
        public CatchTotal catch_total { get; set; }
        public EffortBool effort_bool { get; set; }
        public EffortDesc effort_desc { get; set; }
        public EffortType effort_type { get; set; }
        public FishSector fish_sector { get; set; }
        public GroupLabel group_label { get; set; }
        public IsGpsUsed is_gps_used { get; set; }
        public LengthType length_type { get; set; }
        public NsapRegion nsap_region { get; set; }
        public RefNoNote ref_no_note { get; set; }
        public SearchMode search_mode { get; set; }
        public SelectGear select_gear { get; set; }
        public WtLenwtKg wt_lenwt_kg { get; set; }
        public BingoRepeat bingo_repeat { get; set; }
        public GroupEffort group_effort { get; set; }
        public IsBoatUsed is_boat_used { get; set; }
        public LandingSite landing_site { get; set; }
        public LenMaxHint len_max_hint { get; set; }
        public LenWtGroup len_wt_group { get; set; }
        public LengthClass length_class { get; set; }
        public LfGrpTitle lf_grp_title { get; set; }
        public LlGrpTitle ll_grp_title { get; set; }
        public LwGrpTitle lw_grp_title { get; set; }
        public MaxLenHint max_len_hint { get; set; }
        public RepeatTitle repeat_title { get; set; }
        public SearchValue search_value { get; set; }
        public SpNameOther spName_other { get; set; }
        public SpeciesWt1 species_wt_1 { get; set; }
        public VesselCatch vessel_catch { get; set; }
        public WtUnitName wt_unit_name { get; set; }
        public BoxesSampled boxes_sampled { get; set; }
        public CatchSampled catch_sampled { get; set; }
        public EffortRepeat effort_repeat { get; set; }
        public EffortsGroup efforts_group { get; set; }
        public FmaInRegion fma_in_region { get; set; }
        public IncludeBingo include_bingo { get; set; }
        public LenWtRepeat len_wt_repeat { get; set; }
        public LengthTypeG length_type_g { get; set; }
        public MajorgridCsv majorgrid_csv { get; set; }
        public MaxSizeHint max_size_hint { get; set; }
        public OverwtPrompt overwt_prompt { get; set; }
        public ResponseType response_type { get; set; }
        public SamplingDate sampling_date { get; set; }
        public SearchColumn search_column { get; set; }
        public SearchVessel search_vessel { get; set; }
        public SelectSpName select_spName { get; set; }
        public SelectVessel select_vessel { get; set; }
        public BingoComplete bingo_complete { get; set; }
        public BoatUsedText boat_used_text { get; set; }
        public EnforceMaxlen enforce_maxlen { get; set; }
        public FishingGround fishing_ground { get; set; }
        public GearUsedText gear_used_text { get; set; }
        public IncludeEffort include_effort { get; set; }
        public InlandgridCsv inlandgrid_csv { get; set; }
        public RaisingFactor raising_factor { get; set; }
        public SampleWtText sample_wt_text { get; set; }
        public SearchSpecies search_species { get; set; }
        public SizeTypeName size_type_name { get; set; }
        public SpeciesAndWt species_and_wt { get; set; }
        public TripIsSuccess trip_isSuccess { get; set; }
        public HasMeasurement has_measurement { get; set; }
        public ShowTaxaImage show_taxa_image { get; set; }
        public SoakTimeGroup soak_time_group { get; set; }
        public SpeciesAndWt1 species_and_wt1 { get; set; }
        public SpeciesNotfish species_notfish { get; set; }
        public SumWtFromGms sum_wt_from_gms { get; set; }
        public TimeSetString time_set_string { get; set; }
        public TypeOfMeasure type_of_measure { get; set; }
        public VesselSampling vessel_sampling { get; set; }
        public CatchCompGroup catch_comp_group { get; set; }
        public CountCatchComp count_catch_comp { get; set; }
        public DecimalSetTime decimal_set_time { get; set; }
        public EffortIntensity effort_intensity { get; set; }
        public EffortSpecName effort_spec_name { get; set; }
        public FromTotalCatch from_total_catch { get; set; }
        public GmsRepeatGroup gms_repeat_group { get; set; }
        public GridCoordGroup grid_coord_group { get; set; }
        public InlandGridNote inland_grid_note { get; set; }
        public IsCatchSampled is_catch_sampled { get; set; }
        public SoakTimeRepeat soak_time_repeat { get; set; }
        public SpeciesWtCheck species_wt_check { get; set; }
        public SpeciesWtTotal species_wt_total { get; set; }
        public TripIsCompleted trip_isCompleted { get; set; }
        public UnknownLenNote unknown_len_note { get; set; }
        public WhatIsMeasured what_is_measured { get; set; }
        public GroupFinalTally group_final_tally { get; set; }
        public IncludeCatchcomp include_catchcomp { get; set; }
        public IncludeSoakTime include_soak_time { get; set; }
        public IndividualLength individual_length { get; set; }
        public IndividualWeight individual_weight { get; set; }
        public LandingSiteName landing_site_name { get; set; }
        public LandingSiteText landing_site_text { get; set; }
        public LengthListGroup length_list_group { get; set; }
        public NumberOfFishers number_of_fishers { get; set; }
        public RegionEnumerator region_enumerator { get; set; }
        public SearchOrShowall search_or_showall { get; set; }
        public SelectEnumerator select_enumerator { get; set; }
        public SpeciesSampleWt species_sample_wt { get; set; }
        public SpeciesWtRaised species_wt_raised { get; set; }
        public SpeciesnameGroup speciesname_group { get; set; }
        public SumWtFromLenwt sum_wt_from_lenwt { get; set; }
        public VesselCsvSource vessel_csv_source { get; set; }
        public ArriveDateString arrive_date_string { get; set; }
        public CalculateSetHint calculate_set_hint { get; set; }
        public DepartDateString depart_date_string { get; set; }
        public ExceedLenwtTotal exceed_lenwt_total { get; set; }
        public FromSampleStatus from_sample_status { get; set; }
        public IndividualWtUnit individual_wt_unit { get; set; }
        public LengthFreqRepeat length_freq_repeat { get; set; }
        public LengthListRepeat length_list_repeat { get; set; }
        public SelectLandingsite select_landingsite { get; set; }
        public SpeciesCsvSource species_csv_source { get; set; }
        public SpeciesDataGroup species_data_group { get; set; }
        public SpeciesWtRounded species_wt_rounded { get; set; }
        public StomachContentWt stomach_content_wt { get; set; }
        public SumSpeciesWeight sum_species_weight { get; set; }
        public CalculateHaulHint calculate_haul_hint { get; set; }
        public CombinedGmsFields combined_gms_fields { get; set; }
        public DecimalArriveDate decimal_arrive_date { get; set; }
        public DecimalDepartDate decimal_depart_date { get; set; }
        public ExceedWtGmsTotal exceed_wt_gms_total { get; set; }
        public FishingGroundName fishing_ground_name { get; set; }
        public IncludeCatchcompG include_catchcomp_g { get; set; }
        public MeasureLenAndGms measure_len_and_gms { get; set; }
        public RaisingFactorText raising_factor_text { get; set; }
        public SelectLengthGroup select_length_group { get; set; }
        public ExceedCatchWeight1 exceed_catch_weight1 { get; set; }
        public FishingVesselGroup fishing_vessel_group { get; set; }
        public GutContentCategory gut_content_category { get; set; }
        public IndividualWeightKg individual_weight_kg { get; set; }
        public SamplingDateString sampling_date_string { get; set; }
        public SumWtFromGmsNote sum_wt_from_gms_note { get; set; }
        public DecimalSamplingDate decimal_sampling_date { get; set; }
        public FromTotalCatchCode from_total_catch_code { get; set; }
        public RemarksNotCompleted remarks_not_completed { get; set; }
        public SpeciesNameSelected species_name_selected { get; set; }
        public IncludeSexForLength include_sex_for_length { get; set; }
        public RegionEnumeratorText region_enumerator_text { get; set; }
        public SpeciesWtRaisedNote species_wt_raised_note { get; set; }
        public SumWtFromLenwtNote sum_wt_from_lenwt_note { get; set; }
        public SelectedEffortMeasure selected_effort_measure { get; set; }
        public SoaktimeTrackingGroup soaktime_tracking_group { get; set; }
        public SpeciesSampleWtTotal species_sample_wt_total { get; set; }
        public SumSpeciesWeightNote sum_species_weight_note { get; set; }
        public TimeArriveLandingsite time_arrive_landingsite { get; set; }
        public TimeDepartLandingsite time_depart_landingsite { get; set; }
        public CatchCompositionRepeat catch_composition_repeat { get; set; }
        public OverweightWarningFinal overweight_warning_final { get; set; }
        public RemarksNormalOperation remarks_normal_operation { get; set; }
        public SumSpeciesWeightNote1 sum_species_weight_note1 { get; set; }
        public SumWtFromGmsCoalesce sum_wt_from_gms_coalesce { get; set; }
        public InputTotalSpeciesFirst input_total_species_first { get; set; }
        public SpeciesSampleWtSampled species_sample_wt_sampled { get; set; }
        public SumWtFromLenwtCoalesce sum_wt_from_lenwt_coalesce { get; set; }
        public SumSpeciesWeightCoalesce sum_species_weight_coalesce { get; set; }
        public SpeciesSampleWtFromSample species_sample_wt_from_sample { get; set; }
    }

    public class SpId
    {
    }

    public class SpNameOther
    {
    }

    public class Start
    {
    }

    public class StomachContentWt
    {
    }

    public class Summary
    {
        public bool geo { get; set; }
        public List<string> labels { get; set; }
        public List<string> columns { get; set; }
        public bool lock_all { get; set; }
        public bool lock_any { get; set; }
        public List<object> languages { get; set; }
        public int row_count { get; set; }
        public NameQuality name_quality { get; set; }
        public object default_translation { get; set; }
    }

    public class SumSample
    {
    }

    public class SumSpeciesWeight
    {
    }

    public class SumSpeciesWeightCoalesce
    {
    }

    public class SumSpeciesWeightNote
    {
    }

    public class SumSpeciesWeightNote1
    {
    }

    public class SumTotal
    {
    }

    public class SumWtFromGms
    {
    }

    public class SumWtFromGmsCoalesce
    {
    }

    public class SumWtFromGmsNote
    {
    }

    public class SumWtFromLenwt
    {
    }

    public class SumWtFromLenwtCoalesce
    {
    }

    public class SumWtFromLenwtNote
    {
    }

    public class Survey
    {
        public string name { get; set; }
        public string type { get; set; }

        [JsonProperty("$kuid")]
        public string kuid { get; set; }

        [JsonProperty("$qpath")]
        public string qpath { get; set; }

        [JsonProperty("$xpath")]
        public string xpath { get; set; }

        [JsonProperty("$autoname")]
        public string autoname { get; set; }
        public string @default { get; set; }

        [JsonProperty("media::image")]
        public List<string> mediaimage { get; set; }
        public List<string> label { get; set; }
        public string appearance { get; set; }
        public bool? required { get; set; }
        public string constraint { get; set; }
        public string constraint_message { get; set; }
        public string select_from_list_name { get; set; }
        public string relevant { get; set; }
        public string choice_filter { get; set; }
        public List<string> hint { get; set; }
        public string calculation { get; set; }
    }

    public class Taxa
    {
    }

    public class TaxaIm
    {
    }

    public class TaxaNoIm
    {
    }

    public class TimeArriveLandingsite
    {
    }

    public class TimeDepartLandingsite
    {
    }

    public class TimeSetString
    {
    }


    public class Today
    {
    }

    public class TripIsCompleted
    {
    }

    public class TripIsSuccess
    {
    }


    public class TypeOfMeasure
    {
    }


    public class UnknownLenNote
    {
    }

    public class UserName
    {
    }

    public class UtmZone
    {
    }

    public class VesselCatch
    {
    }

    public class VesselCsvSource
    {
    }

    public class VesselSampling
    {
    }

  

    public class WhatIsMeasured
    {
    }

    public class WptHaul
    {
    }

    public class WptSet
    {
    }

    public class WtLenwt
    {
    }

    public class WtLenwtKg
    {
    }

    public class WtUnitName
    {
    }



    }
}
