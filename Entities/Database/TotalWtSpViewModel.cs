using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class TotalWtSpViewModel
    {
        private bool _editSuccess = false;

        public int Count()
        {
            return TotalWtSpCollection.Count;
        }
        private TotalWtSpRepository twsps { get; set; }
        public ObservableCollection<TotalWtSp> TotalWtSpCollection { get; set; }
        public TotalWtSpViewModel(GearUnload parent, bool updateCollection = false)
        {
            if (updateCollection)
            {
                twsps = new TotalWtSpRepository(parent);
                TotalWtSpCollection = new ObservableCollection<TotalWtSp>(twsps.TotalWtSps);
            }
            else
            {
                twsps = new TotalWtSpRepository();
                TotalWtSpCollection = new ObservableCollection<TotalWtSp>();
            }

            //TotalWtSpCollection = new ObservableCollection<TotalWtSp>(twsps.TotalWtSps);
            TotalWtSpCollection.CollectionChanged += TotalWtSpCollection_CollectionChanged;

        }

        public bool UpdateRecordInRepo(TotalWtSp item)
        {
            if (item.RowID == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < TotalWtSpCollection.Count)
            {
                if (TotalWtSpCollection[index].RowID == item.RowID)
                {
                    TotalWtSpCollection[index] = item;
                    break;
                }
                index++;
            }
            return _editSuccess;
        }
        public bool AddRecordToRepo(TotalWtSp item)
        {
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            TotalWtSpCollection.Add(item);
            return _editSuccess;
        }
        public bool DeleteRecordFromRepo(int rowID)
        {
            if (rowID == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < TotalWtSpCollection.Count)
            {
                if (TotalWtSpCollection[index].RowID == rowID)
                {
                    TotalWtSpCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return _editSuccess;
        }
        private void TotalWtSpCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    int newIndex = e.NewStartingIndex;
                    _editSuccess = twsps.Add(TotalWtSpCollection[newIndex]);

                    break;

                case NotifyCollectionChangedAction.Remove:

                    List<TotalWtSp> tempListOfRemovedItems = e.OldItems.OfType<TotalWtSp>().ToList();
                    _editSuccess = twsps.Delete(tempListOfRemovedItems[0].RowID);

                    break;

                case NotifyCollectionChangedAction.Replace:
                    List<TotalWtSp> tempList = e.NewItems.OfType<TotalWtSp>().ToList();
                    _editSuccess = twsps.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    break;
            }
        }
    }
}
