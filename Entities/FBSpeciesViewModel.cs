using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.IO;

namespace NSAP_ODK.Entities
{
    public class FBSpeciesViewModel
    {
        public event EventHandler<FBSpeciesUpdateEventArgs> FBSpeciesUpdateEvent;
        private bool _duplicateErrorInAddNew;
        private bool _editSuccess;
        public ObservableCollection<FBSpecies> FBSpeciesCollection { get; set; }
        private FBSpeciesRepository FBSpecieses { get; set; }
        private string _excelUpdateFileName;
        private int _editCount;
        public FBSpeciesViewModel()
        {
            FBSpecieses = new FBSpeciesRepository(this);
            FBSpeciesCollection = new ObservableCollection<FBSpecies>(FBSpecieses.FBSpecieses);
            FBSpeciesCollection.CollectionChanged += FBSpeciesCollection_CollectionChanged;
        }

        public void ObjectCreated()
        {
            if (FBSpeciesCollection != null)
            {
                FBSpeciesUpdateEvent?.Invoke(null, new FBSpeciesUpdateEventArgs { UpdateType = FBSpeciesUpdateType.UpdateTypeFetchedFbSpeciesList, FbSpeciesCount = FBSpeciesCollection.Count });
            }
        }


        public Task<bool> UpdateFBSpeciesTableAsync(FBSpeciesUpdateMode updateMode)
        {
            return Task.Run(() => UpdateFBSpeciesTable(updateMode));
        }
        public bool UpdateFBSpeciesTable(FBSpeciesUpdateMode updateMode)
        {
            return FBSpecieses.UpdateFBSpeciesTable(updateMode);

        }
        public bool AddRecordToRepo(FBSpecies sp)
        {
            if (sp == null)
                throw new ArgumentNullException("Error: The argument is Null");
            FBSpeciesCollection.Add(sp);
            _duplicateErrorInAddNew = FBSpecieses.DuplicateErrorWhenAddSpecies;
            if (_editSuccess)
            {
                _editCount++;
                FBSpeciesUpdateEvent?.Invoke(null, new FBSpeciesUpdateEventArgs
                {
                    UpdateType = FBSpeciesUpdateType.UpdateTypeAddingSpecies,
                    FBSpeciesUpdateCount = _editCount,
                    CurrentSpecies = sp
                });
            }
            return _editSuccess;
        }

        public bool DuplicateErrorInAddNew
        {
            get { return _duplicateErrorInAddNew; }
        }

        public bool UpdateRecordInRepo(FBSpecies sp)
        {
            if (sp.SpCode == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < FBSpeciesCollection.Count)
            {
                if (FBSpeciesCollection[index].SpCode == sp.SpCode)
                {
                    FBSpeciesCollection[index] = sp;
                    break;
                }
                index++;
            }
            if (_editSuccess)
            {
                _editCount++;
                FBSpeciesUpdateEvent?.Invoke(null, new FBSpeciesUpdateEventArgs
                {
                    UpdateType = FBSpeciesUpdateType.UpdateTypeUpdatingSpecies,
                    FBSpeciesUpdateCount = _editCount,
                    CurrentSpecies = sp
                });
            }
            return _editSuccess;
        }

        public FBSpeciesUpdateStatus FBSpeciesUpdateStatus
        {
            get
            {
                var settings = this.FBSpeciesUpdateSettings;
                if (settings == null)
                {
                    return FBSpeciesUpdateStatus.FBSpeciesStatus_SettingsNotFound;
                }
                else
                {
                    if (settings.UpdateFileRowCount == NSAPEntities.FBSpeciesViewModel.Count)
                    {
                        return FBSpeciesUpdateStatus.FBSpeciesStatus_UpdatedNoChanges;
                    }
                    else
                    {
                        return FBSpeciesUpdateStatus.FBSpeciesStatus_UpdatedWithChanges;
                    }
                }
            }
        }
        public FBSpeciesUpdateSettings FBSpeciesUpdateSettings
        {
            get
            {
                string fileName = $@"{AppDomain.CurrentDomain.BaseDirectory}\fbspecies_update_settings.xml";
                if (File.Exists(fileName))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(FBSpeciesUpdateSettings));

                    FBSpeciesUpdateSettings fbSpeciesUpdateSettings = null;

                    using (Stream reader = new FileStream(fileName, FileMode.Open))
                    {
                        // Call the Deserialize method to restore the object's state.
                        fbSpeciesUpdateSettings = (FBSpeciesUpdateSettings)serializer.Deserialize(reader);
                    }

                    return fbSpeciesUpdateSettings;
                }
                else
                {
                    return null;
                }

            }
        }
        public bool SaveFBSpeciesUpdateSettings(List<FBSpecies> updateList, string updateFileName, FBSpeciesUpdateMode updateMode)
        {
            bool success = true;
            DateTime createdOn = DateTime.Now;

            //string fileName = $@"{folderToSave}\{_formSummary.Owner}_{_formSummary.Title}_{createdOn:dd-MMM-yyyy_HH_mm}";
            string fileName = $@"{AppDomain.CurrentDomain.BaseDirectory}fbspecies_update_settings.xml";
            FileInfo fi = new FileInfo(updateFileName);
            try
            {
                using (FileStream fs = new FileStream($@"{fileName}", FileMode.Create))
                {
                    XmlSerializer xSer = new XmlSerializer(typeof(FBSpeciesUpdateSettings));
                    xSer.Serialize(
                        fs,
                        new FBSpeciesUpdateSettings
                        {
                            UpdateFileRowCount = updateList.Count,
                            FileName = updateFileName,
                            FileSize = fi.Length,
                            FBSpeciesCount = FBSpeciesCollection.Count,
                            UpdateMode = updateMode
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                Utilities.Logger.Log(ex);
                success = false;
            }
            return success;
        }
        public List<string> GetSpeciesNameFromGenus(string genus)
        {
            List<string> speciesList = new List<string>();
            foreach (var item in FBSpeciesCollection.Where(t => t.Genus == genus).ToList())
            {
                speciesList.Add(item.Species);
            }
            return speciesList;
        }
        public List<string> GetAllGenus()
        {
            List<string> listGenus = new List<string>();

            foreach (var g in FBSpeciesCollection.OrderBy(t => t.Genus).GroupBy(t => t.Genus).ToList())
            {
                listGenus.Add(g.Key);
            }
            return listGenus;
        }

        public FBSpecies GetFBSpecies(string genus, string species)
        {
            return FBSpeciesCollection.FirstOrDefault(t => t.Genus == genus && t.Species==species);
        }
        public FBSpecies GetFBSpecies(int spCode)
        {
            return FBSpeciesCollection.FirstOrDefault(t => t.SpCode == spCode);
        }
        public int Count
        {
            get { return FBSpeciesCollection.Count; }
        }
        public string ExcelUpdateFileName
        {
            get { return _excelUpdateFileName; }
            set
            {
                _excelUpdateFileName = value;
            }
        }

        public FBSpeciesUpdateStatus GetSettingsFileForUpdate()
        {
            string fileName = $@"{AppDomain.CurrentDomain.BaseDirectory}\fbspecies_update_settings.xml";
            return FBSpeciesUpdateStatus.FBSpeciesStatus_SettingsNotFound;

        }
        public async Task<List<FBSpecies>> GetUpdateFBSpecies()
        {
            if (!string.IsNullOrEmpty(_excelUpdateFileName))
            {
                FBSpeciesUpdateEvent?.Invoke(null, new FBSpeciesUpdateEventArgs { UpdateType = FBSpeciesUpdateType.UpdateTypeReadingUpdateFile });
                var updateList = await FBSpecieses.GetUpdateSpeciesListAsync(_excelUpdateFileName);
                FBSpeciesUpdateEvent?.Invoke(null, new FBSpeciesUpdateEventArgs { UpdateType = FBSpeciesUpdateType.UpdateTypeCreatingUpdateList, RowCountInUpdateFile = updateList.Count });
                return updateList;
            }
            else
            {
                return null;
            }
        }
        private void FBSpeciesCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        _editSuccess = FBSpecieses.Add(FBSpeciesCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<FBSpecies> tempListOfRemovedItems = e.OldItems.OfType<FBSpecies>().ToList();
                        _editSuccess = FBSpecieses.Delete(tempListOfRemovedItems[0].SpCode);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<FBSpecies> tempListOfFishers = e.NewItems.OfType<FBSpecies>().ToList();
                        _editSuccess = FBSpecieses.Update(tempListOfFishers[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }


        }
    }
}
