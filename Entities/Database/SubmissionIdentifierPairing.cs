using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using Newtonsoft.Json;

namespace NSAP_ODK.Entities.Database
{
    public class SubmissionIDPair
    {
        public int _id { get; set; }
        public string _uuid { get; set; }
    }
    public static class SubmissionIdentifierPairing
    {
        private static List<string> _unmatchedLandingsJSON = null;
        public static event EventHandler<UploadToDbEventArg> UploadSubmissionToDB;
        public static string JSON { get; set; }

        public static List<string> UnmatchedLandingsJSON { get { return _unmatchedLandingsJSON; } }
        public static void AddUnmatchedJSON(string json)
        {
            _unmatchedLandingsJSON.Add(json);
        }
        public static async Task<bool> UpdateDatabase()
        {
            UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { ItemsForUpdatingCount = SubmissionIDPairs.Count, Intent = UploadToDBIntent.UpdateTableFields });
            int update_count = 0;
            foreach (var item in SubmissionIDPairs)
            {
                string uid = $"{{{item._uuid}}}";
                if (LandingSiteSamplingRepository.UpdateSubmissionID(KoboForm.id_string, uid, item._id))
                {
                    update_count++;
                    UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { ItemsUpdatedCount = update_count, Intent = UploadToDBIntent.UpdatedTableField });
                }
                else
                {
                    if (NSAPEntities.LandingSiteSamplingSubmissionViewModel.GetLandingSiteSampling(item._uuid) == null)
                    {
                        UnmatchedPairs.Add(item);
                    }
                }
            }

            UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { ItemsUpdatedCount = update_count, Intent = UploadToDBIntent.UpdatedTableDone });

            
            if (UnmatchedPairs.Count > 0)
            {
                int loop_count = 0;
                UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { ItemsForUpdatingCount = UnmatchedPairs.Count, Intent = UploadToDBIntent.UpdateUnmatchedJSON });
                foreach (var item in UnmatchedPairs)
                {
                    UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { Intent = UploadToDBIntent.UpdateUnmatchedJSONDownloadingFromServer });
                    string api_call = $"https://kf.kobotoolbox.org/api/v2/assets/{KoboForm.id_string}/data/?format=json&query={{\"_id\":\"{item._id}\"}}";
                    var s = await JSONStringFromAPICall(api_call);
                    if (!s.Contains("\"detail\":\"Not found.\""))
                    {
                        AddUnmatchedJSON(s);
                        loop_count++;
                        
                    }
                    UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { ItemsUpdatedCount = loop_count, Intent = UploadToDBIntent.UpdatedUnmatchedJSON });
                }
                UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { ItemsUpdatedCount = loop_count, Intent = UploadToDBIntent.UpdatedUnmatchedJSONDone });
            }

          return update_count > 0;
        }


        private static async Task<string> JSONStringFromAPICall(string api_call)
        {
            StringBuilder the_response = new StringBuilder();
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), api_call))
            {
                var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{ServerUserName}:{ServerPassword}"));
                request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");
                try
                {
                    var response = await HttpClient.SendAsync(request);
                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    Encoding encoding = Encoding.GetEncoding("utf-8");
                    the_response = new StringBuilder(encoding.GetString(bytes, 0, bytes.Length));
                }
                catch (Exception ex)
                {

                }
            }
            return the_response.ToString();
        }

        public static HttpClient HttpClient { get; set; }
        public static KoboForm KoboForm { get; set; }
        public static Task<bool> UpDateDatabaseTaskAsync()
        {
            return Task.Run(() => UpdateDatabase());
        }
        public static List<SubmissionIDPair> UnmatchedPairs { get; private set; }
        public static bool IsMultivessel { get; set; }


        public static List<SubmissionIDPair> SubmissionIDPairs { get; internal set; }
        public static string ServerUserName { get; set; }
        public static string ServerPassword { get; set; }

        public static void CreateSubmissionsPairsFromJSON()
        {
            //How are PKs assigned to each landings contained in each incoming batch of JSON?
            //call VesselLanding.SetRowIDs()

            //MultiVesselGear_SampledLanding.SetRowIDs();
            try
            {
                _unmatchedLandingsJSON = new List<string>();
                UnmatchedPairs = new List<SubmissionIDPair>();
                SubmissionIDPairs = JsonConvert.DeserializeObject<List<SubmissionIDPair>>(JSON);
            }
            catch (Exception ex)
            {
                Utilities.Logger.Log(ex);
            }
        }
    }
}
