using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace NSAP_ODK.Entities.Database
{
    public class RegionWatchedSpeciesViewModel
    {
        private bool _editSuccess;
        private NSAPRegion _region;
        public ObservableCollection<RegionWatchedSpecies> RegionWatchedSpeciesCollection { get; set; }
        private RegionWatchedSpeciesRepository RegionWatchedSpecieses { get; set; }

        public List<RegionWatchedSpecies> GetAllFish()
        {

            return RegionWatchedSpeciesCollection
                .Where(t => t.TaxaCode == "FIS")
                .ToList();

        }

        public List<RegionWatchedSpecies> GetAllNotFish()
        {

            return RegionWatchedSpeciesCollection
                .Where(t => t.TaxaCode != "FIS")
                .ToList();

        }
        public List<RegionWatchedSpecies> GetAll(string taxaCode = "")
        {
            if (taxaCode.Length > 0)
            {
                return RegionWatchedSpeciesCollection
                    .Where(t => t.TaxaCode == taxaCode)
                    .ToList();
            }
            else
            {
                return RegionWatchedSpeciesCollection.ToList();
            }
        }

        public RegionWatchedSpecies GetRegionWatchedSpecies(int id)
        {
            return RegionWatchedSpeciesCollection.FirstOrDefault(t => t.PK == id);
        }
        public RegionWatchedSpeciesViewModel(NSAPRegion reg)
        {
            _region = reg;
            RegionWatchedSpecieses = new RegionWatchedSpeciesRepository(reg);
            RegionWatchedSpeciesCollection = new ObservableCollection<RegionWatchedSpecies>(RegionWatchedSpecieses.RegionWatchedSpecieses);
            RegionWatchedSpeciesCollection.CollectionChanged += RegionWatchedSpeciesCollection_CollectionChanged;
        }

        private void RegionWatchedSpeciesCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        RegionWatchedSpecies newItem = RegionWatchedSpeciesCollection[e.NewStartingIndex];

                        _editSuccess = RegionWatchedSpecieses.Add(newItem);

                        //int newIndex = e.NewStartingIndex;
                        //_editSuccess = CatchLenFreqs.Add(CatchLenFreqCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<RegionWatchedSpecies> tempListOfRemovedItems = e.OldItems.OfType<RegionWatchedSpecies>().ToList();
                        _editSuccess = RegionWatchedSpecieses.Delete(tempListOfRemovedItems[0].PK);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<RegionWatchedSpecies> tempList = e.NewItems.OfType<RegionWatchedSpecies>().ToList();
                        _editSuccess = RegionWatchedSpecieses.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public bool ExistsInList(int speciesID)
        {

            return RegionWatchedSpeciesCollection.ToList().Find(t => t.SpeciesID == speciesID) != null;
        }
        public int Count
        {
            get { return RegionWatchedSpeciesCollection.Count; }
        }

        public bool AddRecordToRepo(RegionWatchedSpecies item)
        {
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            RegionWatchedSpeciesCollection.Add(item);
            return _editSuccess;
        }

        public bool UpdateRecordInRepo(RegionWatchedSpecies item)
        {
            if (item.PK == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < RegionWatchedSpeciesCollection.Count)
            {
                if (RegionWatchedSpeciesCollection[index].PK == item.PK)
                {
                    RegionWatchedSpeciesCollection[index] = item;
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
                //if (CatchLenFreqCollection.Count == 0)
                //{
                //    return 1;
                //}
                //else
                //{
                return RegionWatchedSpeciesRepository.MaxRecordNumber() + 1;
                //}
            }
        }

        public bool DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < RegionWatchedSpeciesCollection.Count)
            {
                if (RegionWatchedSpeciesCollection[index].PK == id)
                {
                    RegionWatchedSpeciesCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return _editSuccess;
        }
    }

}
