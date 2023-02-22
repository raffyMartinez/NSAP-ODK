using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class UnmatchedFieldsFromJSONFileViewModel
    {
        private bool _editSuccess = false;
        public ObservableCollection<UnmatchedFieldsFromJSONFile> UnmatchedFieldsFromJSONFileCollection { get; set; }
        private UnmatchedFieldsFromJSONFileRepository UnmatchedFieldsFromJSONFiles { get; set; }
        public UnmatchedFieldsFromJSONFileViewModel()
        {
            UnmatchedFieldsFromJSONFiles = new UnmatchedFieldsFromJSONFileRepository();
            UnmatchedFieldsFromJSONFileCollection = new ObservableCollection<UnmatchedFieldsFromJSONFile>(UnmatchedFieldsFromJSONFiles.UnmatchedFieldsFromJSONFiles);
            UnmatchedFieldsFromJSONFileCollection.CollectionChanged += UnmatchedFieldsFromJSONFileCollection_collectionChanged;
        }

        public UnmatchedFieldsFromJSONFile GetItem(string fileName)
        {
            return UnmatchedFieldsFromJSONFileCollection.FirstOrDefault(t => t.FileName == fileName);
        }
        public int Count()
        {
            return UnmatchedFieldsFromJSONFileCollection.Count;
        }
        private void UnmatchedFieldsFromJSONFileCollection_collectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        _editSuccess = UnmatchedFieldsFromJSONFiles.AddItem(UnmatchedFieldsFromJSONFileCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<UnmatchedFieldsFromJSONFile> tempListOfRemovedItems = e.OldItems.OfType<UnmatchedFieldsFromJSONFile>().ToList();
                        _editSuccess = UnmatchedFieldsFromJSONFiles.Delete(tempListOfRemovedItems[0].RowID);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<UnmatchedFieldsFromJSONFile> tempList = e.NewItems.OfType<UnmatchedFieldsFromJSONFile>().ToList();
                        _editSuccess = UnmatchedFieldsFromJSONFiles.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public bool AddRecordToRepo(UnmatchedFieldsFromJSONFile item)
        {
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            UnmatchedFieldsFromJSONFileCollection.Add(item);

            return _editSuccess;
        }

        public bool UpdateRecordInRepo(UnmatchedFieldsFromJSONFile item)
        {
            if (item.RowID == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < UnmatchedFieldsFromJSONFileCollection.Count)
            {
                if (UnmatchedFieldsFromJSONFileCollection[index].RowID == item.RowID)
                {
                    UnmatchedFieldsFromJSONFileCollection[index] = item;
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
            while (index < UnmatchedFieldsFromJSONFileCollection.Count)
            {
                if (UnmatchedFieldsFromJSONFileCollection[index].RowID == id)
                {
                    UnmatchedFieldsFromJSONFileCollection.RemoveAt(index);
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
                if (UnmatchedFieldsFromJSONFileCollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    //return ProvinceCollection.Max(t => t.ProvinceID) + 1;
                    return UnmatchedFieldsFromJSONFiles.MaxRecordNumber() + 1;
                }
            }
        }
    }
}
