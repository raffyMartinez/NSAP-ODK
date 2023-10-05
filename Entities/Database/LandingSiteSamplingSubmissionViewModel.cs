using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class LandingSiteSamplingSubmissionViewModel
    {
        private static StringBuilder _csv = new StringBuilder();
        private bool _editSuccess = false;
        public LandingSiteSamplingSubmissionViewModel()
        {

            LandingSiteSamplingSubmissions = new LandingSiteSamplingSubmissionRepository();
            LandingSiteSamplingSubmissionCollection = new ObservableCollection<LandingSiteSamplingSubmission>(LandingSiteSamplingSubmissions.LandingSiteSamplingSubmissions);
            LandingSiteSamplingSubmissionCollection.CollectionChanged += LandingSiteSamplingSubmissionCollection_CollectionChanged;

        }
        public static void ClearCSV()
        {
            _csv.Clear();
        }
        public bool ClearRepository()
        {
            LandingSiteSamplingSubmissionCollection.Clear();
            return LandingSiteSamplingSubmissionRepository.ClearTable();
        }
        public LandingSiteSampling GetLandingSiteSampling(string submission_id)
        {
            return LandingSiteSamplingSubmissionCollection.FirstOrDefault(t => t.SubmissionID == submission_id)?.LandingSiteSampling;
        }
        public bool Add(LandingSiteSamplingSubmission ls)
        {
            if (ls == null)
                throw new ArgumentNullException("Error: The argument is Null");
            LandingSiteSamplingSubmissionCollection.Add(ls);
            return _editSuccess;
        }
        public bool Update(LandingSiteSamplingSubmission ls)
        {
            if (string.IsNullOrEmpty(ls.SubmissionID))
                throw new Exception("Error: ID cannot be blank");

            int index = 0;
            while (index < LandingSiteSamplingSubmissionCollection.Count)
            {
                if (LandingSiteSamplingSubmissionCollection[index].SubmissionID == ls.SubmissionID)
                {
                    LandingSiteSamplingSubmissionCollection[index] = ls;
                    break;
                }
                index++;
            }
            return _editSuccess;
        }
        public bool Delete(string ls_id)
        {
            if (string.IsNullOrEmpty(ls_id))
                throw new Exception("Sampling cannot be null");

            int index = 0;
            while (index < LandingSiteSamplingSubmissionCollection.Count)
            {
                if (LandingSiteSamplingSubmissionCollection[index].SubmissionID == ls_id)
                {
                    LandingSiteSamplingSubmissionCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return _editSuccess;
        }
        public int CountRecordsByFormID(string xFormID)
        {
            return LandingSiteSamplingSubmissionCollection.Where(t=>t.XFormIdentifier==xFormID).Count();
        }
        public int Count()
        {
            return LandingSiteSamplingSubmissionCollection.Count;
        }
        public static string CSV
        {
            get
            {
                if (Utilities.Global.Settings.UsemySQL)
                {
                    return $"{NSAPMysql.MySQLConnect.GetColumnNamesCSV("dbo_lss_submissionIDs")}\r\n{_csv}";
                }
                else
                {
                    return $"{CreateTablesInAccess.GetColumnNamesCSV("dbo_lss_submissionIDs")}\r\n{_csv}";
                }
            }
        }
        private static bool SetCSV(LandingSiteSamplingSubmission item)
        {
            bool success = false;

            Dictionary<string, string> myDict = new Dictionary<string, string>();
            myDict.Add("submission_id", item.SubmissionID);
            myDict.Add("date_added", item.DateAdded.ToString());
            myDict.Add("json_File", item.JSONFile);
            myDict.Add("landing_site_sampling_id", item.LandingSiteSampling.PK.ToString());
            myDict.Add("xFormIdentifier", item.XFormIdentifier);

            _csv.AppendLine(CreateTablesInAccess.CSVFromObjectDataDictionary(myDict, "dbo_lss_submissionIDs"));
            success = true;
            return success;
        }
        private void LandingSiteSamplingSubmissionCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        LandingSiteSamplingSubmission newItem = LandingSiteSamplingSubmissionCollection[e.NewStartingIndex];
                        if (newItem.DelayedSave)
                        {
                            _editSuccess = SetCSV(newItem);
                        }
                        else
                        {
                            _editSuccess = LandingSiteSamplingSubmissions.Add(newItem);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<LandingSiteSamplingSubmission> tempListOfRemovedItems = e.OldItems.OfType<LandingSiteSamplingSubmission>().ToList();
                        _editSuccess = LandingSiteSamplingSubmissions.Delete(tempListOfRemovedItems[0].SubmissionID);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<LandingSiteSamplingSubmission> tempList = e.NewItems.OfType<LandingSiteSamplingSubmission>().ToList();
                        _editSuccess = LandingSiteSamplingSubmissions.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public ObservableCollection<LandingSiteSamplingSubmission> LandingSiteSamplingSubmissionCollection { get; set; }
        private LandingSiteSamplingSubmissionRepository LandingSiteSamplingSubmissions { get; set; }
    }
}
