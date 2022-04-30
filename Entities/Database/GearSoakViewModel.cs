using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class GearSoakViewModel
    {

        private bool _editSuccess;
        public ObservableCollection<GearSoak> GearSoakCollection { get; set; }
        private GearSoakRepository GearSoaks { get; set; }

        public GearSoakViewModel(VesselUnload vu)
        {
            GearSoaks = new GearSoakRepository(vu);
            GearSoakCollection = new ObservableCollection<GearSoak>(GearSoaks.GearSoaks);
            GearSoakCollection.CollectionChanged += GearSoaks_CollectionChanged;
        }
        public GearSoakViewModel()
        {
            GearSoaks = new GearSoakRepository();
            GearSoakCollection = new ObservableCollection<GearSoak>(GearSoaks.GearSoaks);
            GearSoakCollection.CollectionChanged += GearSoaks_CollectionChanged;
        }

        public List<GearSoak> GetAllGearSoaks()
        {
            return GearSoakCollection.ToList();
        }

        public List<GearSoakFlattened> GetAllFlattenedItems(bool tracked = false)
        {
            List<GearSoakFlattened> thisList = new List<GearSoakFlattened>();
            if (tracked)
            {
                foreach (var item in GearSoakCollection
                    .Where(t => t.Parent.OperationIsTracked == tracked))
                {
                    thisList.Add(new GearSoakFlattened(item));
                }
            }
            else
            {
                foreach (var item in GearSoakCollection)
                {
                    thisList.Add(new GearSoakFlattened(item));
                }
            }
            return thisList;
        }
        public bool ClearRepository()
        {
            GearSoakCollection.Clear();
            return GearSoaks.ClearTable();
        }

        public GearSoak getGearSoak(int pk)
        {
            return GearSoakCollection.FirstOrDefault(n => n.PK == pk);
        }


        private void GearSoaks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        _editSuccess = GearSoaks.Add(GearSoakCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<GearSoak> tempListOfRemovedItems = e.OldItems.OfType<GearSoak>().ToList();
                        _editSuccess = GearSoaks.Delete(tempListOfRemovedItems[0].PK);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<GearSoak> tempList = e.NewItems.OfType<GearSoak>().ToList();
                        _editSuccess = GearSoaks.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return GearSoakCollection.Count; }
        }

        public bool AddRecordToRepo(GearSoak item)
        {
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            GearSoakCollection.Add(item);
            return _editSuccess;
        }

        public bool UpdateRecordInRepo(GearSoak item)
        {
            if (item.PK == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < GearSoakCollection.Count)
            {
                if (GearSoakCollection[index].PK == item.PK)
                {
                    GearSoakCollection[index] = item;
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
                if (GearSoakCollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    return GearSoaks.MaxRecordNumber() + 1;
                }
            }
        }

        public bool DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < GearSoakCollection.Count)
            {
                if (GearSoakCollection[index].PK == id)
                {
                    GearSoakCollection.RemoveAt(index);
                    break;
                }
                index++;
            }

            return _editSuccess;
        }
    }
}
