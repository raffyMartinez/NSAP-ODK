using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Specialized;
using System;

namespace NSAP_ODK.Entities
{
    public class FMAViewModel
    {
        bool _editSuccess;
        public ObservableCollection<FMA> FMACollection { get; set; }
        private FMARepository FMAs { get; set; }

        public int NextRecordNumber
        {
            get
            {
                if (FMACollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    return FMACollection.Max(t => t.FMAID) + 1;
                }
            }
        }
        public bool AddRecordToRepo(FMA fma)
        {
            if (fma == null)
                throw new ArgumentNullException("Error: The argument is Null");
            FMACollection.Add(fma);
            return _editSuccess;
        }
        public bool AddFMASToMySQl()
        {
            int count = 0;
            if (Utilities.Global.Settings.UsemySQL)
            {
                foreach (var fma in FMACollection)
                {
                    if (FMAs.Add(fma))
                    {
                        count++;
                    }
                }
            }
            return count == FMACollection.Count;
        }
        public FMAViewModel()
        {
            FMAs = new FMARepository();
            FMACollection = new ObservableCollection<FMA>(FMAs.FMAs);
            FMACollection.CollectionChanged += FMACollection_CollectionChanged;
        }

        private void FMACollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        _editSuccess = FMAs.Add(FMACollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        //List<FMA> tempListOfRemovedItems = e.OldItems.OfType<FMA>().ToList();
                        //_editSuccess = Specieses.Delete(tempListOfRemovedItems[0].RowNumber);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        //List<FMA> tempListOfFishers = e.NewItems.OfType<FMA>().ToList();
                        //_editSuccess = Specieses.Update(tempListOfFishers[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public List<FMA> GetAllFMAs()
        {
            return FMACollection.ToList();
        }

        public FMA GetFMA(int id)
        {
            return FMACollection.FirstOrDefault(n => n.FMAID == id);
        }

        public int Count
        {
            get { return FMACollection.Count; }
        }

    }
}