using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class LandingsRepeat
    {
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
        [JsonProperty("landings_repeat/landing/landings_count")]
        public int Count { get; set; }
        [JsonProperty("landings_repeat/landing/total_catch_wt")]
        public double? TotalCatchWt { get; set; }
        [JsonProperty("landings_repeat/landing/landings_note")]
        public string Note { get; set; }

        [JsonProperty("landings_repeat/landing/landing_catch")]
        public string LandingsRepeatLandingLandingCatch { get; set; }



    }

    public class ValidationStatus
    {

    }
    public class GearUnloadFromServer
    {
        [JsonProperty("vessel_sampling/sampling_date")]
        public DateTime SamplingDate { get; set; }
        [JsonProperty("vessel_sampling/is_sampling_day")]
        public string IsSamplingDay { get; set; }
        public bool SamplingConducted { get { return IsSamplingDay == "yes"; } }
        [JsonProperty("vessel_sampling/nsap_region")]
        public string NsapRegionCode { get; set; }
        public string NSAPRegionName { get { return Region.Name; } }
        public NSAPRegion Region { get { return NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(NsapRegionCode); }  }
        [JsonProperty("vessel_sampling/fma_in_region")]
        public int FmaInRegion { get; set; }


        public FMA FMA { get { return NSAPEntities.NSAPRegionViewModel.GetFMAInRegion(NsapRegionCode, FmaInRegion); } }
        [JsonProperty("vessel_sampling/select_enumerator")]
        public string SelectEnumerator { get; set; }
        [JsonProperty("vessel_sampling/region_enumerator")]
        public int? RegionEnumerator { get; set; }

        public NSAPEnumerator NSAPEnumerator { get 
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

        public string RegionEnumeratorName
        {
            get
            {
                if(RegionEnumerator==null)
                {
                    return RegionEnumeratorText;
                }
                else
                {
                    return NSAPEnumerator.Name;
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

        public LandingSite LandingSite
        {
            get
            {
                if(LandingSiteCode==null)
                {
                    return null;
                }
                else
                {
                    return NSAPEntities.NSAPRegionViewModel.GetLandingSiteInRegion(NsapRegionCode, FmaInRegion,FishingGroundCode,(int)LandingSiteCode);
                }
            }
        }
        public string LandingSiteName
        {
            get
            {
                if(LandingSiteCode==null)
                {
                    return LandingSiteText;
                }
                else
                {
                    return LandingSite.ToString();
                }
            }
        }

        public string Notes { get; set; }



        public List<object> _notes { get; set; }
        public List<object> _tags { get; set; }
        public string _xform_id_string { get; set; }
        [JsonProperty("meta/instanceID")]
        public string MetaInstanceID { get; set; }
        public DateTime start { get; set; }
        public List<LandingsRepeat> landings_repeat { get; set; }
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



    }
    public static class GearUnloadServerRepository
    {
        
        public static void CreateGearUnloadsFromJSON(string json)
        {
            GearUnloadsFromServer = JsonConvert.DeserializeObject<List<GearUnloadFromServer>>(json);
        }
        public static List<GearUnloadFromServer> GearUnloadsFromServer { get; set; }
    }
}
