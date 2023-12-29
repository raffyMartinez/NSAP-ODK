using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class LandingSiteFishingGroundViewModel
    {
        private bool _editSuccess;
        public ObservableCollection<LandingSiteFishingGround> LandingSiteFishingGroundCollection { get; set; }
        private LandingSiteFishingGroundRepository LandingSiteFishingGrounds { get; set; }
        public LandingSiteFishingGroundViewModel(LandingSite ls)
        {
            LandingSiteFishingGrounds = new LandingSiteFishingGroundRepository(ls);
            LandingSiteFishingGroundCollection = new ObservableCollection<LandingSiteFishingGround>(LandingSiteFishingGrounds.LandingSiteFishingGrounds);
            LandingSiteFishingGroundCollection.CollectionChanged += LandingSiteFishingGroundCollection_CollectionChanged;
        }
        public int Count { get { return LandingSiteFishingGroundCollection.Count; } }

        public bool DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < LandingSiteFishingGroundCollection.Count)
            {
                if (LandingSiteFishingGroundCollection[index].RowID == id)
                {
                    LandingSiteFishingGroundCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return _editSuccess;
        }
        public bool UpdateRecordInRepo(LandingSiteFishingGround item)
        {
            if (item.RowID == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < LandingSiteFishingGroundCollection.Count)
            {
                if (LandingSiteFishingGroundCollection[index].RowID == item.RowID)
                {
                    LandingSiteFishingGroundCollection[index] = item;
                    break;
                }
                index++;
            }
            return _editSuccess;
        }
        public bool AddRecordToRepo(LandingSiteFishingGround item)
        {
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            LandingSiteFishingGroundCollection.Add(item);
            return _editSuccess;
        }
        private void LandingSiteFishingGroundCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        LandingSiteFishingGround newItem = LandingSiteFishingGroundCollection[e.NewStartingIndex];
                        _editSuccess = LandingSiteFishingGrounds.Add(newItem);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<LandingSiteFishingGround> tempListOfRemovedItems = e.OldItems.OfType<LandingSiteFishingGround>().ToList();
                        _editSuccess = LandingSiteFishingGrounds.Delete(tempListOfRemovedItems[0].RowID);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<LandingSiteFishingGround> tempList = e.NewItems.OfType<LandingSiteFishingGround>().ToList();
                        _editSuccess = LandingSiteFishingGrounds.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }
    }
}
