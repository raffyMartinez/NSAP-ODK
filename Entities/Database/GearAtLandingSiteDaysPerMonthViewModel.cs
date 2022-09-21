using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class GearAtLandingSiteDaysPerMonthViewModel
    {
        private bool _editSuccess;
        public ObservableCollection<GearAtLandingSiteDaysPerMonth> GearAtLandingSiteDaysPerMonthCollection { get; set; }
        private GearAtLandingSiteDaysPerMonthRepository GearAtLandingSiteDaysPerMonths { get; set; }
        public GearAtLandingSiteDaysPerMonthViewModel()
        {
            GearAtLandingSiteDaysPerMonths = new GearAtLandingSiteDaysPerMonthRepository();
            GearAtLandingSiteDaysPerMonthCollection = new ObservableCollection<GearAtLandingSiteDaysPerMonth>(GearAtLandingSiteDaysPerMonths.GearAtLandingSiteDaysPerMonths);
            GearAtLandingSiteDaysPerMonthCollection.CollectionChanged += GearAtLandingSiteDaysPerMonthCollection_CollectionChanged;
        }

        public int Count
        {
            get { return GearAtLandingSiteDaysPerMonthCollection.Count; }
        }

        public bool Add(GearAtLandingSiteDaysPerMonth item)
        {
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            GearAtLandingSiteDaysPerMonthCollection.Add(item);
            return _editSuccess;
        }
        public bool Update(GearAtLandingSiteDaysPerMonth item)
        {
            if (item.RowID == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < GearAtLandingSiteDaysPerMonthCollection.Count)
            {
                if (GearAtLandingSiteDaysPerMonthCollection[index].RowID == item.RowID)
                {
                    GearAtLandingSiteDaysPerMonthCollection[index] = item;
                    break;
                }
                index++;
            }
            return _editSuccess;
        }
        public bool Delete(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < GearAtLandingSiteDaysPerMonthCollection.Count)
            {
                if (GearAtLandingSiteDaysPerMonthCollection[index].RowID == id)
                {
                    GearAtLandingSiteDaysPerMonthCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return _editSuccess;
        }
        private void GearAtLandingSiteDaysPerMonthCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        GearAtLandingSiteDaysPerMonth newItem = GearAtLandingSiteDaysPerMonthCollection[e.NewStartingIndex];
                        //if (newItem.DelayedSave)
                        //{
                        //    _editSuccess = SetCSV(newItem);
                        //}
                        //else
                        //{
                        //    _editSuccess = GearAtLandingSiteDaysPerMonths.Add(newItem);

                        //int newIndex = e.NewStartingIndex;
                        _editSuccess = GearAtLandingSiteDaysPerMonths.Add(newItem);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<GearAtLandingSiteDaysPerMonth> tempListOfRemovedItems = e.OldItems.OfType<GearAtLandingSiteDaysPerMonth>().ToList();
                        _editSuccess = GearAtLandingSiteDaysPerMonths.Delete(tempListOfRemovedItems[0].RowID);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<GearAtLandingSiteDaysPerMonth> tempList = e.NewItems.OfType<GearAtLandingSiteDaysPerMonth>().ToList();
                        _editSuccess = GearAtLandingSiteDaysPerMonths.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }
    }
}
