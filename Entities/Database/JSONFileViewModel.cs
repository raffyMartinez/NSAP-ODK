using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;


namespace NSAP_ODK.Entities.Database
{
    public class JSONFileViewModel
    {
        public event EventHandler<ProcessingItemsEventArg> ProcessingItemsEvent;
        private bool _editSuccess;
        public ObservableCollection<JSONFile> JSONFileCollection { get; set; }
        private JSONFileRepository JSONFiles { get; set; }

        public bool ClearRepository()
        {
            JSONFileCollection.Clear();
            return JSONFileRepository.ClearTable();
        }
        public JSONFileViewModel()
        {
            JSONFiles = new JSONFileRepository();
            JSONFileCollection = new ObservableCollection<JSONFile>(JSONFiles.JSONFiles);
            JSONFileCollection.CollectionChanged += JSONFiles_CollectionChanged;
        }


        public int CountSavedEffortJsonFile()
        {
            return JSONFileCollection.Count(t => t.Description.Contains("Fisheries landing survey") || t.Description.Contains("NSAP Fish Catch Monitoring e-Form"));
        }

        public int CountSavedVesselCountsJsonFile()
        {
            return JSONFileCollection.Count(t => t.Description.Contains("Daily landings and catch estimate"));
        }
        public int Count()
        {
            return JSONFileCollection.Count;
        }
        public JSONFile getJSONFileFromFileName(string fileName)
        {
            return JSONFileCollection.Where(t => t.FileName == fileName).FirstOrDefault();
        }
        public JSONFile getJSONFIle(string md5)
        {
            return JSONFileCollection.Where(t => t.MD5 == md5).FirstOrDefault();
        }
        public string CreateFileName(JSONFile jsf)
        {
            return $"{jsf.FormID} {jsf.Earliest.ToString("MMM-dd-yyyy")} - {jsf.Latest.ToString("MMM-dd-yyyy")}.json";
        }

        public async Task<bool> Save(JSONFile jsf)
        {
            try
            {
                if (!File.Exists(jsf.FullFileName))
                {
                    using (var sw = new StreamWriter(jsf.FullFileName))
                    {
                        await sw.WriteAsync(jsf.JSONText);
                    }
                }
                return AddRecordToRepo(jsf);
            }
            catch (Exception ex)
            {
                Utilities.Logger.Log(ex);
                return false;
            }

        }
        private bool JsonFileIsSaved(JSONFile jsonFile)
        {
            return NSAPEntities.JSONFileViewModel.Count() > 0 && NSAPEntities.JSONFileViewModel.getJSONFIle(jsonFile.MD5) != null;
        }
        public async Task<JSONFile> CreateJsonFile(string json, string description = "", string fileName = "")
        {
            var jsonFile = new JSONFile();
            jsonFile.JSONText = json;
            jsonFile.MD5 = Utilities.MD5.CreateMD5(json);
            jsonFile.RowID = NSAPEntities.JSONFileViewModel.NextRecordNumber;
            jsonFile.Description = description;
            jsonFile.Earliest = VesselUnloadServerRepository.DownloadedLandingsEarliestLandingDate();
            jsonFile.Latest = VesselUnloadServerRepository.DownloadedLandingsLatestLandingDate();
            jsonFile.Count = VesselUnloadServerRepository.DownloadedLandingsCount();
            jsonFile.LandingIdentifiers = VesselUnloadServerRepository.GetLandingIdentifiers();
            jsonFile.DateAdded = DateTime.Now;
            if (fileName.Length > 0)
            {
                jsonFile.FullFileName = fileName;
            }
            else
            {
                jsonFile.FullFileName = $@"{Utilities.Global.Settings.JSONFolder}\{NSAPEntities.JSONFileViewModel.CreateFileName(jsonFile)}";
            }
            //if (FormID == null)
            //{
            //    FormID = Path.GetFileName(jsonFile.FullFileName).Split(' ')[0];
            //}
            jsonFile.FormID = Path.GetFileName(jsonFile.FullFileName).Split(' ')[0];

            if (!JsonFileIsSaved(jsonFile))
            {
                await NSAPEntities.JSONFileViewModel.Save(jsonFile);
            }
            return jsonFile;
        }
        private void JSONFiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        _editSuccess = JSONFiles.AddItem(JSONFileCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<JSONFile> tempListOfRemovedItems = e.OldItems.OfType<JSONFile>().ToList();
                        _editSuccess = JSONFiles.Delete(tempListOfRemovedItems[0].RowID);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<JSONFile> tempList = e.NewItems.OfType<JSONFile>().ToList();
                        _editSuccess = JSONFiles.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }


        public async Task<bool> AnalyzeJSONFiles(List<FileInfoJSONMetadata> fileInfoJSONMetadatas)
        {
            ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "start analyzing JSON files", TotalCountToProcess = fileInfoJSONMetadatas.Count });
            int loopCount = 0;
            foreach (var item in fileInfoJSONMetadatas)
            {
                if (item.JSONFile == null)
                {
                    item.JSONFile = NSAPEntities.JSONFileViewModel.getJSONFileFromFileName(item.JSONFileInfo.Name);
                    string json = File.ReadAllText(item.JSONFileInfo.FullName);
                    VesselUnloadServerRepository.JSON = json;
                    VesselUnloadServerRepository.CreateLandingsFromJSON();
                    if (item.JSONFile == null)
                    {
                        item.JSONFile = await CreateJsonFile(json, "NSAP Fish Catch Monitoring e-Form", item.JSONFileInfo.FullName);
                    }
                }
                AnalyzeJsonForMismatch.Analyze(VesselUnloadServerRepository.VesselLandings, item.JSONFile);
                VesselUnloadServerRepository.ResetLists(includeJSON:true);
                loopCount++;
                ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "JSON file analyzed", CountProcessed = loopCount });
            }
            ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "done analyzing JSON file" });
            return Count() > 0 || NSAPEntities.UnmatchedFieldsFromJSONFileViewModel.Count() > 0;
        }

        public bool AddRecordToRepo(JSONFile jsf)
        {
            if (jsf == null)
                throw new ArgumentNullException("Error: The argument is Null");
            JSONFileCollection.Add(jsf);

            return _editSuccess;
        }

        public bool UpdateRecordInRepo(JSONFile jsf)
        {
            if (jsf.RowID == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < JSONFileCollection.Count)
            {
                if (JSONFileCollection[index].RowID == jsf.RowID)
                {
                    JSONFileCollection[index] = jsf;
                    break;
                }
                index++;
            }

            return _editSuccess;
        }

        public bool DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < JSONFileCollection.Count)
            {
                if (JSONFileCollection[index].RowID == id)
                {
                    JSONFileCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return _editSuccess;
        }
        public int NextRecordNumber
        {
            get
            {
                if (JSONFileCollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    //return ProvinceCollection.Max(t => t.ProvinceID) + 1;
                    return JSONFiles.MaxRecordNumber() + 1;
                }
            }
        }
    }
}
