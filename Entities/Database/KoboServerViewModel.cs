using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class KoboServerViewModel
    {
        private bool _editSuccess = false;
        private List<KoboForm> _koboForms = new List<KoboForm>();
        public ObservableCollection<Koboserver> KoboserverCollection { get; set; }
        private KoboServerRepository Koboservers { get; set; }

        private bool _updateSavedCount;
        private bool _updateUploadedJSON;
        private bool _updateCreatedJSON;
        private Koboserver _serverWithUploadedJSON;
        private Koboserver _serverWithCreatedJSON;
        public int Count()
        {
            return KoboserverCollection.Count;
        }

        public Koboserver GetKoboServer(string xformID)
        {
            return KoboserverCollection.FirstOrDefault(t => t.ServerID == xformID);
        }
        public bool RemoveAllKoboserversOfOwner(Koboserver ks)
        {
            var ksvs = KoboserverCollection.Where(t => t.Owner == ks.Owner).ToList();
            int serverCount = ksvs.Count;
            int deleteCount = 0;
            foreach (Koboserver ksv in ksvs)
            {
                DeleteRecordFromRepo(ksv.ServerNumericID);
                deleteCount++;
            }

            return serverCount == deleteCount;
        }

        public bool RemoveKoboserver(Koboserver ks)
        {
            return false;
        }
        public bool ClearTable()
        {
            return Koboservers.ClearTable();
        }
        public void RefreshSavedCount()
        {
            _updateSavedCount = true;
            var fl_servers = KoboserverCollection.Where(t => t.IsFishLandingSurveyForm == true).ToList();
            foreach (var item in fl_servers)
            {
                if (item.IsFishLandingMultiVesselSurveyForm)
                {
                    item.SavedInDBCount = NSAPEntities.LandingSiteSamplingSubmissionViewModel.CountRecordsByFormID(item.ServerID);
                }
                else
                {
                    item.SavedInDBCount = NSAPEntities.SummaryItemViewModel.CountByFormID(item.ServerID);
                }
                UpdateRecordInRepo(item);
            }
            _updateSavedCount = false;
        }

        public Koboserver GetKoboServer(int id)
        {
            return KoboserverCollection.FirstOrDefault(t => t.ServerNumericID == id);
        }



        public List<KoboForm> KoboForms
        {
            get { return _koboForms; }
            set
            {
                _koboForms = value;
                foreach (var kf in _koboForms)
                {
                    try
                    {

                        Koboserver ks = new Koboserver
                        {
                            ServerNumericID = kf.formid,
                            FormName = kf.title,
                            ServerID = kf.id_string,
                            Owner = kf.owner,
                            eFormVersion = kf.eForm_version,
                            FormVersion = kf.xlsform_version,
                            DateCreated = kf.date_created,
                            DateModified = kf.date_modified,
                            DateLastSubmission = kf.last_submission_time,
                            SubmissionCount = kf.num_of_submissions,
                            DateLastAccessed = DateTime.Now,
                            //UserCount = kf.users.Count,
                            //SavedInDBCount = NSAPEntities.SummaryItemViewModel.CountByFormID(kf.id_string),
                            LastUploadedJSON = KoboserverCollection.FirstOrDefault(t => t.ServerNumericID == kf.formid)?.LastUploadedJSON,
                            LastCreatedJSON = KoboserverCollection.FirstOrDefault(t => t.ServerNumericID == kf.formid)?.LastCreatedJSON,
                        };
                        if(kf.users!=null)
                        {
                            ks.UserCount = kf.users.Count;
                        }

                        if (ks.IsFishLandingMultiVesselSurveyForm)
                        {
                            ks.SavedInDBCount = NSAPEntities.LandingSiteSamplingSubmissionViewModel.CountRecordsByFormID(ks.ServerID);
                        }
                        else
                        {
                            ks.SavedInDBCount = NSAPEntities.SummaryItemViewModel.CountByFormID(kf.id_string);
                        }


                        if (GetKoboServer(ks.ServerNumericID) == null)
                        {
                            AddRecordToRepo(ks);
                        }
                        else
                        {
                            UpdateRecordInRepo(ks);
                        }
                    }
                    catch (Exception ex)
                    {

                    }


                }
            }
        }

        public string GetLastUploadedJSON()
        {
            var fl_server = KoboserverCollection.FirstOrDefault(t => t.LastUploadedJSON?.Length > 0);
            if (fl_server != null)
            {
                return fl_server.LastUploadedJSON;
            }
            else
            {
                return null;
            }
        }
        public List<Koboserver> GetFishLandingSurverServers()
        {
            return KoboserverCollection.Where(t => t.IsFishLandingSurveyForm == true).ToList();
        }
        public KoboServerViewModel()
        {
            Koboservers = new KoboServerRepository();
            KoboserverCollection = new ObservableCollection<Koboserver>(Koboservers.Koboservers);
            KoboserverCollection.CollectionChanged += KoboserverCollection_CollectionChanged;
        }
        private void KoboserverCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        _editSuccess = Koboservers.Add(KoboserverCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<Koboserver> tempListOfRemovedItems = e.OldItems.OfType<Koboserver>().ToList();
                        _editSuccess = Koboservers.Delete(tempListOfRemovedItems[0].ServerNumericID);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<Koboserver> tempList = e.NewItems.OfType<Koboserver>().ToList();
                        if (_updateSavedCount)
                        {
                            _editSuccess = Koboservers.UpdateServerCount(tempList[0]);
                        }
                        else if (_updateCreatedJSON)
                        {
                            _editSuccess = Koboservers.UpdateUploadedJSONs(tempList[0], false);
                        }
                        else if (_updateUploadedJSON)
                        {
                            _editSuccess = Koboservers.UpdateUploadedJSONs(tempList[0], true);
                        }
                        else
                        {
                            _editSuccess = Koboservers.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                        }
                    }
                    break;
            }
        }
        public void ResetJSONFields(bool resetLastUploaded = true, bool isMultiGearform = false, bool isMultiVesselform = false)
        {
            List<Koboserver> fl_servers;
            if (isMultiVesselform)
            {
                fl_servers = KoboserverCollection.Where(t => t.IsFishLandingMultiVesselSurveyForm == true).ToList();
            }
            else if (isMultiGearform)
            {
                fl_servers = KoboserverCollection.Where(t => t.IsFishLandingMultiGearSurveyForm == true).ToList();
            }
            else
            {
                fl_servers = KoboserverCollection.Where(t => t.IsFishLandingSurveyForm == true).ToList();
            }
            if (resetLastUploaded)
            {
                _updateUploadedJSON = true;
                foreach (var item in fl_servers)
                {
                    item.LastUploadedJSON = "";
                    UpdateRecordInRepo(item);
                }
                _updateUploadedJSON = false;
            }
            else
            {
                _updateCreatedJSON = true;
                foreach (var item in fl_servers)
                {
                    item.LastCreatedJSON = "";
                    UpdateRecordInRepo(item);
                }
                _updateCreatedJSON = false;
            }
        }

        public Koboserver ServerWithCreatedJSON
        {
            get { return _serverWithCreatedJSON; }
            set
            {
                _serverWithCreatedJSON = value;
                _updateCreatedJSON = true;
                UpdateRecordInRepo(_serverWithCreatedJSON);
                _updateCreatedJSON = false;
            }
        }
        public Koboserver ServerWithUploadedJSON
        {
            get { return _serverWithUploadedJSON; }
            set
            {
                _serverWithUploadedJSON = value;

                _updateUploadedJSON = true;
                UpdateRecordInRepo(_serverWithUploadedJSON);
                _updateUploadedJSON = false;
            }
        }
        public bool UpdateRecordInRepo(Koboserver item)
        {
            if (item.ServerNumericID == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < KoboserverCollection.Count)
            {
                if (KoboserverCollection[index].ServerNumericID == item.ServerNumericID)
                {
                    KoboserverCollection[index] = item;
                    break;
                }
                index++;
            }
            return _editSuccess;
        }

        public bool AddRecordToRepo(Koboserver item)
        {
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            KoboserverCollection.Add(item);
            return _editSuccess;
        }
        public bool DeleteRecordFromRepo(int serverID)
        {
            if (serverID == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < KoboserverCollection.Count)
            {
                if (KoboserverCollection[index].ServerNumericID == serverID)
                {
                    KoboserverCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return _editSuccess;
        }
    }
}
