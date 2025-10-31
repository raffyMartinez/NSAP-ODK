using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace NSAP_ODK.Entities.Database
{
    public class ETPInteractionViewModel:IDisposable
    {
        private bool _editSuccess;

        public ETPInteractionViewModel(VesselUnload vu)
        {
            ETP_Interactions = new ETPInteractionRepository(vu);
            ETP_InteractionCollection = new ObservableCollection<ETP_Interaction>(ETP_Interactions.ETP_Interactions);
            ETP_InteractionCollection.CollectionChanged += ETP_InteractionCollection_CollectionChanged;
        }

        private static bool SetCSV(ETP_Interaction etp)
        {
            bool success = false;
            return success;
        }
        private void ETP_InteractionCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        ETP_Interaction newItem = ETP_InteractionCollection[e.NewStartingIndex];
                        if (newItem.DelayedSave)
                        {
                            _editSuccess = SetCSV(newItem);
                        }
                        else
                        {
                            _editSuccess = ETP_Interactions.Add(newItem);
                        }
                        int newIndex = e.NewStartingIndex;
                        _editSuccess = ETP_Interactions.Add(ETP_InteractionCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<ETP_Interaction> tempListOfRemovedItems = e.OldItems.OfType<ETP_Interaction>().ToList();
                        _editSuccess = ETP_Interactions.Delete(tempListOfRemovedItems[0].RowID);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<ETP_Interaction> tempList = e.NewItems.OfType<ETP_Interaction>().ToList();
                        _editSuccess = ETP_Interactions.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public ObservableCollection<ETP_Interaction> ETP_InteractionCollection { get; set; }
        private ETPInteractionRepository ETP_Interactions { get; set; }

        private static StringBuilder _csv = new StringBuilder();
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                ETP_InteractionCollection.Clear();
                ETP_InteractionCollection = null;
                ETP_Interactions = null;

            }
            // free native resources if there are any.
        }
        public int Count
        {
            get { return ETP_InteractionCollection.Count; }
        }

        public bool AddRecordToRepo(ETP_Interaction item)
        {
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            ETP_InteractionCollection.Add(item);
            return _editSuccess;
        }

        public bool UpdateRecordInRepo(ETP_Interaction item)
        {
            if (item.RowID == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < ETP_InteractionCollection.Count)
            {
                if (ETP_InteractionCollection[index].RowID == item.RowID)
                {
                    ETP_InteractionCollection[index] = item;
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
                if (ETP_InteractionCollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    return ETP_Interactions.MaxRecordNumber() + 1;
                    
                }
            }
        }

        public bool DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < ETP_InteractionCollection.Count)
            {
                if (ETP_InteractionCollection[index].RowID == id)
                {
                    ETP_InteractionCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return _editSuccess;
        }

    }
}
