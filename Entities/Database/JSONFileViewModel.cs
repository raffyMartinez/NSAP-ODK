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

        public async Task<bool> AnalyzeBatchJSONFilesAsync(List<FileInfo> batchJSONFiles, Koboserver ks)
        {
            int loop = 0;
            int countJSONFileFound = 0;
            int countJSONAnalyzed = 0;
            StatusJSONAnalyze("start", batchJSONFiles.Count);
            foreach (var fi in batchJSONFiles)
            {

                var jf = JSONFileCollection.FirstOrDefault(t => t.FileName == fi.Name);
                if (jf == null)
                {
                    jf = await CreateJSONFileAsync(fi.FullName);

                    if (jf != null)
                    {
                        jf.FormID = ks.ServerNumericID.ToString();
                        jf.Description = ks.FormName;
                        if (AddRecordToRepo(jf))
                        {
                            countJSONFileFound++;
                        }

                        var umjf = NSAPEntities.UnmatchedFieldsFromJSONFileViewModel.GetItem(jf.FileName);
                        if (umjf == null && AnalyzeForMismatchAndSave(jf))
                        {
                            countJSONAnalyzed++;
                        }


                        jf.Dispose();
                    }
                    else
                    {

                    }
                }
                loop++;
                StatusJSONAnalyze("found", loop);

            }

            StatusJSONAnalyze("end");
            return countJSONFileFound > 0 || countJSONAnalyzed > 0;
        }
        public async Task<bool> AnalyzeJSONInListAsync(List<FileInfoJSONMetadata> fijms)
        {
            return await Task.Run(() => AnalyzeJSONInList(fijms));
        }
        private async Task<bool> AnalyzeJSONInList(List<FileInfoJSONMetadata> fijms)
        {
            int loop = 0;
            int countJSONFileFound = 0;
            int countJSONAnalyzed = 0;
            StatusJSONAnalyze("start", fijms.Count);
            foreach (FileInfoJSONMetadata fijm in fijms)
            {
                var jf = JSONFileCollection.FirstOrDefault(t => t.FileName == fijm.JSONFileInfo.Name);
                if (jf == null)
                {
                    jf = await CreateJSONFileAsync(fijm.JSONFileInfo.FullName);
                    jf.FormID = fijm.Koboserver.ServerNumericID.ToString();
                    jf.Description = fijm.Koboserver.FormName;
                    jf.DateAdded = fijm.JSONFileInfo.CreationTime;
                    if (AddRecordToRepo(jf))
                    {
                        countJSONFileFound++;
                    }

                }
                var umjf = NSAPEntities.UnmatchedFieldsFromJSONFileViewModel.GetItem(jf.FileName);
                if (umjf == null && AnalyzeForMismatchAndSave(jf))
                //if (NSAPEntities.UnmatchedFieldsFromJSONFileViewModel.GetItem(jf.FileName) == null && AnalyzeForMismatchAndSave(jf))
                {
                    countJSONAnalyzed++;
                }

                loop++;
                StatusJSONAnalyze("found", loop);
                jf.Dispose();
            }
            StatusJSONAnalyze("end");
            return countJSONFileFound > 0 || countJSONAnalyzed > 0;
        }

        public async Task<bool> CreateJSONFilesFromJSONFolder(string folderPath)
        {
            return await JSONFiles.GetJSONFilesFromFolderAsync(folderPath);

        }

        public void StatusJSONAnalyze(string status, int? count = null)
        {
            switch (status)
            {
                case "start":
                    ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "start analyzing JSON files", TotalCountToProcess = (int)count });
                    break;
                case "found":
                    ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "JSON file analyzed", CountProcessed = (int)count });
                    break;
                case "end":
                    ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "done analyzing JSON file" });
                    break;
            }
        }
        public bool ClearRepository()
        {
            JSONFileCollection.Clear();
            return JSONFileRepository.ClearTable();
        }
        public JSONFileViewModel()
        {
            JSONFiles = new JSONFileRepository(this);
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

        public Task<JSONFile> CreateJSONFileAsync(string fileName)
        {
            return Task.Run(() => CreateJSONFile(fileName));
        }

        public bool AnalyzeForMismatchAndSave(JSONFile jf)
        {
            if (AnalyzeJsonForMismatch.Analyze(jsonFile: jf))
            {
                return true;
            }
            return false;
        }
        public JSONFile CreateJSONFile(string fileName)
        {
            var jsonFile = new JSONFile();

            jsonFile.FullFileName = fileName;
            if (jsonFile.VesselLandings != null)
            {
                jsonFile.Earliest = jsonFile.VesselLandings.OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate;
                jsonFile.Latest = jsonFile.VesselLandings.OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate;
                jsonFile.Count = jsonFile.VesselLandings.Count();
                jsonFile.RowID = NSAPEntities.JSONFileViewModel.NextRecordNumber;
                FileInfo fi = new FileInfo(fileName);
                //jsonFile.DateAdded = DateTime.Now;
                jsonFile.DateAdded = fi.CreationTime;
                return jsonFile;
            }
            else
            {
                return null;
            }
        }

        public static string GetVersionString(string json)
        {
            string search = "\"intronote\":\"Version ";
            string r = "";
            int start = 0;
            string searchstring = json;
            if (json.IndexOf(Environment.NewLine) < json.Length)
            {
                search = "\"intronote\": \"Version ";
                var lines = json.Split('\n');
                foreach (var item in lines)
                {
                    if (item.Contains(search))
                    {
                        start = item.IndexOf(search) + search.Length;
                        searchstring = item;
                        break;
                    }
                }
            }

            else
            {

                start = json.IndexOf(search) + search.Length;
            }
            bool isnumeric = true;
            int x = 0;
            do
            {
                char c = searchstring[start + x];
                isnumeric = c >= '0' && c <= '9' || c == '.';
                if (isnumeric)
                {
                    r += c;
                    x++;

                }

            } while (isnumeric);

            return r;
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
            if (fileName.Length > 0)
            {
                jsonFile.FullFileName = fileName;
            }
            else
            {
                jsonFile.FullFileName = $@"{Utilities.Global.Settings.JSONFolder}\{NSAPEntities.JSONFileViewModel.CreateFileName(jsonFile)}";
            }
            FileInfo fi = new FileInfo(jsonFile.FullFileName);
            //jsonFile.DateAdded = DateTime.Now;
            jsonFile.DateAdded = fi.CreationTime;
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
                VesselUnloadServerRepository.ResetLists(includeJSON: true);
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
