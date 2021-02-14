using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using NSAP_ODK.Utilities;

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
        public string url { get; set; }
        public int id { get; set; }
        public int xform { get; set; }
        public string data_value { get; set; }
        public string data_type { get; set; }
        public string data_file { get; set; }
        public string data_file_type { get; set; }
        public string file_hash { get; set; }
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
        public string url { get; set; }
        public int formid { get; set; }
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
            DateLastSubmission = form.last_submission_time==null?"":((DateTime)form.last_submission_time).ToString("MMM-dd-yyyy HH:mm:ss");
            NumberOfSubmissions = form.num_of_submissions;
            NumberOfUsers = form.users.Count;
            FormID = form.formid;
            SubmittedToday = form.submission_count_for_today;

            if(Title.Contains("Fisheries landing survey"))
            {
                KoboFormType = KoboFormType.FormTypeCatchAndEffort;
            }
            else if(Title=="Daily landings and catch estimate")
            {
                KoboFormType = KoboFormType.FormTypeVesselCountAndCatchEstimate;
            }
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

        public int NumberSavedToDatabase
        {
            get
            {
                int v = 0;
                switch(KoboFormType)
                {
                    case KoboFormType.FormTypeCatchAndEffort:
                        v=NSAPEntities.VesselUnloadViewModel.Count;
                        break;
                    case KoboFormType.FormTypeVesselCountAndCatchEstimate:
                        v = NSAPEntities.LandingSiteSamplingViewModel.CountEformSubmissions;
                        break;
                }
                return v;
                
            }
        }

        public string LastSaveDateInDatabase
        {
            get
            {
                string lastSaveDate = "";
                switch(KoboFormType)
                {
                    case KoboFormType.FormTypeCatchAndEffort:
                        if (NSAPEntities.VesselUnloadViewModel.Count > 0)
                        {
                            lastSaveDate = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection.Max(t => t.DateTimeSubmitted).ToString("MMM-dd-yyyy HH:mm:ss");
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
    public class Root
    {
        public List<KoboForm> Forms { get; set; }
    }
    public static class KoboForms
    {
        private static List<string> GetPermissions(JToken obj)
        {
            List<string> thisList = new List<string>();
            foreach (var item in obj.Children())
            {
                foreach (var child_item in item.Children())
                {
                    thisList.Add(child_item.ToString());
                }
            }
            return thisList;
        }
        private static List<User> MakeUsers(JToken obj)
        {
            var thisList = new List<User>();
            foreach (var item in obj.Children())
            {
                string role = "";
                string user = "";
                List<string> permissions = null;
                foreach (JToken item_child in item.Children())
                {
                    string propertyName = ((JProperty)item_child).Name.Replace("/", "__");
                    JToken propertyValue = ((JProperty)item_child).Value;
                    switch (propertyName)
                    {
                        case "role":
                            role = propertyValue.ToString();
                            break;
                        case "user":
                            user = propertyValue.ToString();
                            break;
                        case "permissions":
                            permissions = GetPermissions(item_child);
                            break;
                    }
                }
                User newUser = new User
                {
                    role = role,
                    user = user,
                    permissions = permissions
                };
                thisList.Add(newUser);
            }
            return thisList;
        }
        private static List<Metadata> MakeMetadatas(JToken obj)
        {
            var thisList = new List<Metadata>();
            foreach (var item in obj.Children())
            {
                string url = "";
                int id = 0;
                int xform = 0;
                string data_value = "";
                string data_type = "";
                string data_file = "";
                string data_file_type = "";
                string file_hash = "";
                foreach (JToken item_child in item.Children())
                {
                    string propertyName = ((JProperty)item_child).Name.Replace("/", "__");
                    JToken propertyValue = ((JProperty)item_child).Value;
                    switch (propertyName)
                    {
                        case "url":
                            url = propertyValue.ToString();
                            break;
                        case "id":
                            id = int.Parse(propertyValue.ToString());
                            break;
                        case "xform":
                            xform = int.Parse(propertyValue.ToString());
                            break;
                        case "data_value":
                            data_value = propertyValue.ToString();
                            break;
                        case "data_type":
                            data_type = propertyValue.ToString();
                            break;
                        case "data_file":
                            data_file = propertyValue.ToString();
                            break;
                        case "data_file_type":
                            data_file_type = propertyValue.ToString();
                            break;
                        case "file_hash":
                            file_hash = propertyValue.ToString();
                            break;
                    }
                    if (data_file.Length > 0 || data_value.Length > 0)
                    {
                        Metadata md = new Metadata
                        {
                            url = url,
                            id = id,
                            xform = xform,
                            data_value = data_value,
                            data_type = data_type,
                            data_file = data_file,
                            data_file_type = data_file_type,
                            file_hash = file_hash
                        };

                        thisList.Add(md);
                    }
                }
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
        public static List<KoboForm> MakeFormObjects(JArray root)
        {
            var thisList = new List<KoboForm>();

            string url = "";
            int formid = 0;
            string owner = "";
            bool is_public = false;
            bool public_data = false;
            bool require_auth = false;
            int submission_count_for_today = 0;
            string title = "";
            bool has_kpi_hooks = false;
            string description = "";
            bool downloadable = false;
            bool allows_sms = false;
            bool encrypted = false;
            string sms_id_string = "";
            string id_string = "";
            DateTime date_created = DateTime.Now;
            DateTime date_modified = DateTime.Now;
            DateTime? last_submission_time = null;
            string uuid = "";
            string bamboo_dataset = "";
            bool instances_with_geopoints = false;
            int num_of_submissions = 0;
            string kpi_asset_uid = "";
            string hash = "";
            List<User> users = null;
            List<Metadata> metadatas = null;

            foreach (var item in root)
            {
                foreach (JToken child in item.Values())
                {


                    var path = GetPath(child.Path);
                    switch (path)
                    {
                        case "url":
                            url = child.ToString();
                            break;
                        case "formid":
                            formid = int.Parse(child.ToString());
                            break;
                        case "metadata":
                            metadatas = MakeMetadatas(child);
                            break;
                        case "owner":
                            owner = child.ToString();
                            break;
                        case "public":
                            is_public = bool.Parse(child.ToString());
                            break;
                        case "public_data":
                            public_data = bool.Parse(child.ToString());
                            break;
                        case "require_auth":
                            require_auth = bool.Parse(child.ToString());
                            break;
                        case "submission_count_for_today":
                            submission_count_for_today = int.Parse(child.ToString());
                            break;
                        case "tags":
                            break;
                        case "title":
                            title = child.ToString();
                            break;
                        case "users":
                            users = MakeUsers(child);
                            break;
                        case "hash":
                            hash = child.ToString();
                            break;
                        case "has_kpi_hooks":
                            has_kpi_hooks = bool.Parse(child.ToString());
                            break;
                        case "description":
                            description = child.ToString();
                            break;
                        case "downloadable":
                            downloadable = bool.Parse(child.ToString());
                            break;
                        case "allows_sms":
                            allows_sms = bool.Parse(child.ToString());
                            break;
                        case "encrypted":
                            encrypted = bool.Parse(child.ToString());
                            break;
                        case "sms_id_string":
                            sms_id_string = child.ToString();
                            break;
                        case "id_string":
                            id_string = child.ToString();
                            break;
                        case "date_created":
                            date_created = DateTime.Parse(child.ToString());
                            break;
                        case "date_modified":
                            date_modified = DateTime.Parse(child.ToString());
                            break;
                        case "last_submission_time":
                            //if (chil)
                            //{
                            //    last_submission_time = DateTime.Parse(child.ToString());
                            //}
                            if(DateTime.TryParse(child.ToString(), out DateTime dt))
                            {
                                last_submission_time = dt;
                            }
                            
                            break;
                        case "uuid":
                            uuid = child.ToString();
                            break;
                        case "bamboo_dataset":
                            break;
                        case "instances_with_geopoints":
                            instances_with_geopoints = bool.Parse(child.ToString());
                            break;
                        case "num_of_submissions":
                            num_of_submissions = int.Parse(child.ToString());
                            break;
                        case "kpi_asset_uid":
                            kpi_asset_uid = child.ToString();
                            break;

                    }


                }
                KoboForm form = new KoboForm
                {

                    url = url,
                    formid = formid,
                    owner = owner,
                    is_public = is_public,
                    public_data = public_data,
                    require_auth = require_auth,
                    submission_count_for_today = submission_count_for_today,
                    title = title,
                    has_kpi_hooks = has_kpi_hooks,
                    description = description,
                    downloadable = downloadable,
                    allows_sms = allows_sms,
                    encrypted = encrypted,
                    sms_id_string = sms_id_string,
                    id_string = id_string,
                    date_created = date_created,
                    date_modified = date_modified,
                    last_submission_time = last_submission_time,
                    uuid = uuid,
                    bamboo_dataset = bamboo_dataset,
                    instances_with_geopoints = instances_with_geopoints,
                    num_of_submissions = num_of_submissions,
                    kpi_asset_uid = kpi_asset_uid,
                    hash = hash,
                    users = users,
                    metadata = metadatas
                };
                thisList.Add(form);
            }
            return thisList;
        }
    }
}
