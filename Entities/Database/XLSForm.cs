using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Sector
    {
        public string label { get; set; }
        public string value { get; set; }
    }

    public class Country
    {
        public string label { get; set; }
        public string value { get; set; }
    }

    public class Settings
    {
        public Sector sector { get; set; }
        public Country country { get; set; }
        public string description { get; set; }

        [JsonProperty("share-metadata")]
        public bool ShareMetadata { get; set; }
        public string style { get; set; }
        public string version { get; set; }
        public string id_string { get; set; }
        public string allow_choice_duplicates { get; set; }
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
        public List<string> naming_conflicts { get; set; }
        public object default_translation { get; set; }
    }

    public class Result
    {
        public string uid { get; set; }
        public string url { get; set; }
        public string content_hash { get; set; }
        public DateTime date_deployed { get; set; }
        public string date_modified { get; set; }
    }

    public class DeployedVersions
    {
        public int count { get; set; }
        public object next { get; set; }
        public object previous { get; set; }
        public List<Result> results { get; set; }
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

    public class DeploymentDataDownloadLinks
    {
        public string xls_legacy { get; set; }
        public string csv_legacy { get; set; }
        public string zip_legacy { get; set; }
        public string kml_legacy { get; set; }
        public string xls { get; set; }
        public string csv { get; set; }
    }

    public class Default
    {
    }

   

   







    public class Survey
    {
        public string name { get; set; }
        public string type { get; set; }

        [JsonProperty("$kuid")]
        public string Kuid { get; set; }

        [JsonProperty("$autoname")]
        public string Autoname { get; set; }
        public List<string> label { get; set; }
        public string @default { get; set; }

        [JsonProperty("media::image")]
        public List<string> MediaImage { get; set; }
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

    public class Choice
    {
        public string name { get; set; }

        [JsonProperty("$kuid")]
        public string Kuid { get; set; }
        public List<string> label { get; set; }
        public string list_name { get; set; }

        [JsonProperty("$autovalue")]
        public string Autovalue { get; set; }
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

    public class Download
    {
        public string format { get; set; }
        public string url { get; set; }
    }

    public class Embed
    {
        public string format { get; set; }
        public string url { get; set; }
    }

    public class AssignablePermission
    {
        public string url { get; set; }
        public object label { get; set; }
    }

    public class Permission
    {
        public string url { get; set; }
        public string user { get; set; }
        public string permission { get; set; }
        public string label { get; set; }
    }

    public class ExportSettings
    {
        public string lang { get; set; }
        public string type { get; set; }
        public List<object> fields { get; set; }
        public string group_sep { get; set; }
        public string multiple_select { get; set; }
        public bool hierarchy_in_labels { get; set; }
        public bool fields_from_all_versions { get; set; }
        public string uid { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string date_modified { get; set; }
        public ExportSettings export_settings { get; set; }
    }

    public class Children
    {
        public int count { get; set; }
    }

    public class DataSharing
    {
    }

    public class XLSForm
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
        public string data { get; set; }
        public Children children { get; set; }
        public int subscribers_count { get; set; }
        public string status { get; set; }
        public object access_types { get; set; }
        public DataSharing data_sharing { get; set; }
        public string paired_data { get; set; }
    }



}
