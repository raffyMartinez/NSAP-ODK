using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using System.Threading;
using System.Threading.Tasks;

public enum KoboFormType
{
    FormTypeOther,
    FormTypeCatchAndEffort,
    FormTypeVesselCountAndCatchEstimate
}

/// <summary>
/// Handles query to server on forms that user has access to
///
/// Returns properties of forms, users and permsissions
/// </summary>
namespace NSAP_ODK.Entities
{
    public class Metadata
    {
        public bool replace { get; set; }
        public string url { get; set; }
        public int id { get; set; }
        public int xform { get; set; }
        public string data_value { get; set; }
        public string data_type { get; set; }
        public string data_file { get; set; }
        public string data_file_type { get; set; }
        public string file_hash { get; set; }

        public string Description
        {
            get
            {
                if (Database.CSVFIleManager.CSVFiles.Keys.Contains(data_value))
                {
                    return Database.CSVFIleManager.CSVFiles[data_value].Desccription;
                }
                else
                {
                    return "";
                }
            }
            set { }
        }
    }

    public class User
    {
        public string role { get; set; }
        public string user { get; set; }
        public List<string> permissions { get; set; }

        public string all_permissions
        {
            get
            {
                string perm = "";
                foreach (var item in permissions)
                {
                    perm += $"{item}, ";
                }
                return perm.Trim(',', ' ');
            }
        }
    }

    public class KoboForm
    {
        public Koboform_files koboform_files { get; set; }
        public string url { get; set; }
        public int formid { get; set; }

        public string Version_ID { get; set; }
        public string xlsForm_idstring { get; set; }
        public string xlsform_version { get; set; }

        public string eForm_version { get; set; }

        //public string version
        //{
        //    get
        //    {
        //        var arr = metadata.FirstOrDefault(t => t.data_value.Contains("version "))?.data_value?.Split(' ', '.');
        //        if (arr != null)
        //        {
        //            return arr[1];
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}

        public List<Metadata> metadata_active
        {
            get
            {
                return metadata.Where(t => t.data_file_type == @"text/csv").ToList();
                //return metadata.Where(t => t.file_hash.Length > 0 && t.data_value.Contains("csv")).ToList();
            }
        }

        public List<Metadata> metadata { get; set; }
        public string owner { get; set; }
        public bool is_public { get; set; }
        public bool public_data { get; set; }
        public bool require_auth { get; set; }
        public int submission_count_for_today { get; set; }
        public List<object> tags { get; set; }
        public string title { get; set; }
        public List<User> users { get; set; }
        public string hash { get; set; }
        public bool has_kpi_hooks { get; set; }
        public string description { get; set; }
        public bool downloadable { get; set; }
        public bool allows_sms { get; set; }
        public bool encrypted { get; set; }
        public string sms_id_string { get; set; }
        public string id_string { get; set; }
        public DateTime date_created { get; set; }
        public DateTime date_modified { get; set; }
        public DateTime? last_submission_time { get; set; }
        public string uuid { get; set; }
        public string bamboo_dataset { get; set; }
        public bool instances_with_geopoints { get; set; }
        public int num_of_submissions { get; set; }
        public string kpi_asset_uid { get; set; }
    }

    [CategoryOrder("Server data", 1)]
    [CategoryOrder("Saved in database", 2)]
    public class FormSummary
    {
        public FormSummary(KoboForm form)
        {
            Title = form.title;
            Description = form.description;
            DateCreated = form.date_created.ToString("MMM-dd-yyyy HH:mm");
            DateModified = form.date_modified.ToString("MMM-dd-yyyy HH:mm");
            DateLastSubmission = form.last_submission_time == null ? "" : ((DateTime)form.last_submission_time).ToString("MMM-dd-yyyy HH:mm:ss");
            NumberOfSubmissions = form.num_of_submissions;
            NumberOfUsers = form.users.Count;
            FormID = form.formid;
            SubmittedToday = form.submission_count_for_today;
            XLSForm_Version = form.xlsform_version;
            XLSForm_IDString = form.id_string;
            EFormVersion = form.eForm_version;
            KPI_id_uid = form.kpi_asset_uid;
            Owner = form.owner;
            if (Title.Contains("Fisheries landing survey") || Title.Contains("NSAP Fish Catch Monitoring e-Form"))
            {
                KoboFormType = KoboFormType.FormTypeCatchAndEffort;
            }
            else if (Title == "Daily landings and catch estimate")
            {
                KoboFormType = KoboFormType.FormTypeVesselCountAndCatchEstimate;
            }
            KoboForm = form;
        }

        public KoboFormType KoboFormType { get; set; }
        public string Title { get; internal set; }
        public string Description { get; internal set; }
        public string DateCreated { get; internal set; }
        public string DateModified { get; internal set; }
        public string DateLastSubmission { get; internal set; }
        public int NumberOfSubmissions { get; internal set; }
        public int NumberOfUsers { get; internal set; }
        public int SubmittedToday { get; internal set; }
        public int FormID { get; internal set; }
        public KoboForm KoboForm { get; internal set; }
        public string KPI_id_uid { get; internal set; }
        public string XLSForm_IDString { get; internal set; }
        public string XLSForm_Version { get; internal set; }
        public string Owner { get; internal set; }

        public string EFormVersion { get; internal set; }
        public int NumberSavedToDatabase
        {
            get
            {
                int v = 0;
                switch (KoboFormType)
                {
                    case KoboFormType.FormTypeCatchAndEffort:
                        try
                        {
                            //v = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection.Count(t => t.XFormIdentifier == XLSForm_IDString);
                            v = NSAPEntities.SummaryItemViewModel.CountRecordsByFormID(XLSForm_IDString);
                        }
                        catch
                        {
                            v = NSAPEntities.VesselUnloadViewModel.Count;
                        }
                        break;

                    case KoboFormType.FormTypeVesselCountAndCatchEstimate:
                        v = NSAPEntities.LandingSiteSamplingViewModel.CountEformSubmissions;
                        break;
                }
                return v;
            }
        }

        public string LastSubmittedDateInDatabase
        {
            get
            {
                string lastSaveDate = "";
                switch (KoboFormType)
                {
                    case KoboFormType.FormTypeCatchAndEffort:
                        if (NSAPEntities.SummaryItemViewModel.Count > 0)
                        {
                            DateTime? ld = NSAPEntities.SummaryItemViewModel.LastSubmittedDateInDatabase((XLSForm_IDString));
                            if (ld != null)
                            {
                                lastSaveDate = ((DateTime)ld).ToString("MMM-dd-yyyy HH:mm");
                            }
                        }
                        break;
                    case KoboFormType.FormTypeVesselCountAndCatchEstimate:
                        break;
                }

                return lastSaveDate;
            }
        }
        public string LastSaveDateInDatabase
        {
            get
            {
                string lastSaveDate = "";
                switch (KoboFormType)
                {
                    case KoboFormType.FormTypeCatchAndEffort:
                        //if (NSAPEntities.VesselUnloadViewModel.Count > 0)
                        //{
                        //    //put the form id(or project id guid) in the where clause
                        //    try
                        //    {
                        //        lastSaveDate = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection
                        //           .Where(t => t.XFormIdentifier == XLSForm_IDString)
                        //           .Max(t => t.DateTimeSubmitted).ToString("MMM-dd-yyyy HH:mm:ss");
                        //    }
                        //    catch
                        //    {
                        //        lastSaveDate = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection
                        //            .Max(t => t.DateTimeSubmitted).ToString("MMM-dd-yyyy HH:mm:ss");
                        //    }

                        //    //lastSaveDate = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection
                        //    //    .Max(t => t.DateTimeSubmitted).ToString("MMM-dd-yyyy HH:mm:ss");
                        //}
                        if (NSAPEntities.SummaryItemViewModel.Count > 0)
                        {
                            DateTime? ld = NSAPEntities.SummaryItemViewModel.LastSavedDateInDatabase((XLSForm_IDString));
                            if (ld != null)
                            {
                                lastSaveDate = ((DateTime)ld).ToString("MMM-dd-yyyy HH:mm");
                            }
                        }
                        break;

                    case KoboFormType.FormTypeVesselCountAndCatchEstimate:
                        if (NSAPEntities.LandingSiteSamplingViewModel.Count > 0)
                        {
                            var lastDate = NSAPEntities.LandingSiteSamplingViewModel.LatestEformSubmissionDate;
                            if (lastDate == null)
                            {
                                return "";
                            }
                            else
                            {
                                lastSaveDate = ((DateTime)lastDate).ToString("MMM-dd-yyyy HH:mm:ss");
                            }
                        }
                        break;
                }

                return lastSaveDate;
            }
        }
    }


    public static class KoboForms
    {


        public static List<KoboForm> FormsFromJSON { get; internal set; }

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



        public static async Task<bool> GetVersionFromXLSForm(KoboForm kf, string user_name, string password, HttpClient httpClient)
        {
            bool success = true;
            HttpResponseMessage response;
            byte[] bytes;
            Encoding encoding;
            string the_response = "";
            string base64authorization = "";
            HttpRequestMessage request;
            var api_call = $"https://kf.kobotoolbox.org/api/v2/assets/{kf.id_string}/?format=json";
            using (request = new HttpRequestMessage(new HttpMethod("GET"), api_call))
            {
                base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user_name}:{password}"));
                if (request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}"))
                {
                    try
                    {
                        response = await httpClient.SendAsync(request);
                        bytes = await response.Content.ReadAsByteArrayAsync();
                        encoding = Encoding.GetEncoding("utf-8");
                        the_response = encoding.GetString(bytes, 0, bytes.Length);
                        if (the_response.Contains("Invalid username/password"))
                        {

                        }
                        else
                        {

                            var x = MakeXLSForm(the_response);
                            kf.Version_ID = x.version_id;
                            //Version_ID = x.version_id;
                            XLSFormVersion = x.content.settings.version;
                            kf.xlsform_version = XLSFormVersion;
                            XLSForm_idString = x.content.settings.id_string;
                            if (x.name == "NSAP Fish Catch Monitoring e-Form" || x.name == "Fisheries landing survey")
                            {
                                kf.eForm_version = x.content.survey.Where(t => t.name == "intronote").FirstOrDefault().@default.Replace("Version ", "");
                            }

                        }
                    }
                    catch (HttpRequestException)
                    {
                        success = false;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }

            if (kf.koboform_files == null)
            {
                api_call = $"https://kf.kobotoolbox.org/api/v2/assets/{kf.id_string}/files/?format=json";
                using (request = new HttpRequestMessage(new HttpMethod("GET"), api_call))
                {
                    //base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user_name}:{password}"));
                    if (request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}"))
                    {
                        try
                        {
                            response = await httpClient.SendAsync(request);
                            bytes = await response.Content.ReadAsByteArrayAsync();
                            encoding = Encoding.GetEncoding("utf-8");
                            the_response = encoding.GetString(bytes, 0, bytes.Length);

                            kf.koboform_files = GetFilesFromKoboform(the_response);


                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                            success = false;
                        }
                    }
                }
            }
            //}
            return success;
        }

        public static void ResetVersion_and_ID()
        {
            XLSFormVersion = "";
            XLSForm_idString = "";
        }
        public static string Version_ID { get; internal set; }
        public static string XLSFormVersion { get; internal set; }
        public static string XLSForm_idString { get; internal set; }

        public static string EForm_version { get; internal set; }

        public static Database.XLSForm MakeXLSForm(string json)
        {
            return JsonConvert.DeserializeObject<Database.XLSForm>(json);
        }

        public static Koboform_files GetFilesFromKoboform(string json)
        {
            return JsonConvert.DeserializeObject<Koboform_files>(json);
        }


        public static List<KoboForm> MakeFormObjects(string json)
        {
            FormsFromJSON = JsonConvert.DeserializeObject<List<KoboForm>>(json);
            return FormsFromJSON;
        }

    }
}