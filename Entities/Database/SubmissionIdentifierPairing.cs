﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        public static bool UpdateDatabase()
        {
            UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { ItemsForUpdatingCount = SubmissionIDPairs.Count, Intent = UploadToDBIntent.UpdateTableFields });
            int update_count = 0;
            foreach(var item in SubmissionIDPairs)
            {
                string uid = $"{{{item._uuid}}}";
                if(LandingSiteSamplingRepository.UpdateSubmissionID(KoboForm.id_string, uid, item._id))
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
            return update_count>0;
        }

        public static KoboForm KoboForm { get; set; }
        public static Task<bool> UpDateDatabaseTaskAsync()
        {
            return Task.Run(() => UpdateDatabase());
        }
        public static List<SubmissionIDPair> UnmatchedPairs { get; private set; }
        public static bool IsMultivessel { get; set; }


        public static List<SubmissionIDPair> SubmissionIDPairs { get; internal set; }
        public static void CreateSubmissionsPairsFromJSON()
        {
            //How are PKs assigned to each landings contained in each incoming batch of JSON?
            //call VesselLanding.SetRowIDs()

            //MultiVesselGear_SampledLanding.SetRowIDs();
            try
            {
                _unmatchedLandingsJSON = new List<string>();
                UnmatchedPairs= new List<SubmissionIDPair>();
                SubmissionIDPairs = JsonConvert.DeserializeObject<List<SubmissionIDPair>>(JSON);
            }
            catch (Exception ex)
            {
                Utilities.Logger.Log(ex);
            }
        }
    }
}
