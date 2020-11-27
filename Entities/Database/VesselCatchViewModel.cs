using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class VesselCatchViewModel
    {
        public bool AddSucceeded { get; set; }
        public ObservableCollection<VesselCatch> VesselCatchCollection { get; set; }
        private VesselCatchRepository VesselCatches { get; set; }

        public VesselCatchViewModel()
        {
            VesselCatches = new VesselCatchRepository();
            VesselCatchCollection = new ObservableCollection<VesselCatch>(VesselCatches.VesselCatches);
            VesselCatchCollection.CollectionChanged += VesselCatches_CollectionChanged;
        }
        public VesselCatch getVesselCatch(FromJson.VesselLanding parent, int? speciesID, string speciesText)
        {
            if(speciesID==null)
            {
                return VesselCatchCollection
                    .Where(t => t.Parent.PK == parent.PK)
                    .Where(t => t.SpeciesText == speciesText)
                    .FirstOrDefault();
            }
            else
            {
                return VesselCatchCollection
                    .Where(t => t.Parent.ODKRowID == parent._uuid)
                    .Where(t => t.SpeciesID == speciesID)
                    .FirstOrDefault();
            }
        }
        public VesselCatch getVesselCatch(FromJson.VesselLanding parent, string nameOfCatch)
        {
            return VesselCatchCollection
                .Where(t => t.Parent.PK == parent.PK)
                .Where(t => t.CatchName == nameOfCatch)
                .FirstOrDefault();
        }
        public VesselCatch getVesselCatch(VesselUnload parent, string nameOfCatch)
        {
            return VesselCatchCollection
                .Where(t => t.Parent.PK == parent.PK)
                .Where(t => t.CatchName == nameOfCatch)
                .FirstOrDefault();
        }
        public List<VesselCatchFlattened> GetAllFlattenedItems(bool tracked=false)
        {
            List<VesselCatchFlattened> thisList = new List<VesselCatchFlattened>();
            if (tracked)
            {
                foreach (var item in VesselCatchCollection
                    .Where(t=>t.Parent.OperationIsTracked==tracked))
                {
                    thisList.Add(new VesselCatchFlattened(item));
                }
            }
            else
            {
                foreach (var item in VesselCatchCollection)
                {
                    thisList.Add(new VesselCatchFlattened(item));
                }
            }
            return thisList;
        }
        public bool ClearRepository()
        {
            VesselCatchCollection.Clear();
            return VesselCatches.ClearTable();
        }
        public List<VesselCatch> GetAllVesselCatches()
        {
            return VesselCatchCollection.ToList();
        }

        //public List<LandingSiteSampling> getLandingSiteSamplings(LandingSite ls, FishingGround fg, DateTime samplingDate)
        //{
        //    return LandingSiteSamplingCollection
        //        .Where(t => t.LandingSiteID == ls.LandingSiteID)
        //        .Where(t => t.FishingGroundID == fg.Code)
        //        .Where(t => t.SamplingDate == samplingDate).ToList();
        //}

        //public LandingSiteSampling getLandingSiteSampling(ExcelMainSheet sheet)
        //{
        //    return LandingSiteSamplingCollection
        //        .Where(t => t.LandingSiteID == sheet.NSAPRegionFMAFishingGroundLandingSite.LandingSite.LandingSiteID)
        //        .Where(t => t.FishingGroundID == sheet.NSAPRegionFMAFishingGround.FishingGround.Code)
        //        .Where(t => t.SamplingDate == sheet.SamplingDate).FirstOrDefault();
        //}


        public VesselCatch getVesselCatch(int pk)
        {
            return VesselCatchCollection.FirstOrDefault(n => n.PK == pk);
        }


        private void VesselCatches_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        AddSucceeded = VesselCatches.Add(VesselCatchCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<VesselCatch> tempListOfRemovedItems = e.OldItems.OfType<VesselCatch>().ToList();
                        VesselCatches.Delete(tempListOfRemovedItems[0].PK);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<VesselCatch> tempList = e.NewItems.OfType<VesselCatch>().ToList();
                        VesselCatches.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return VesselCatchCollection.Count; }
        }

        public bool AddRecordToRepo(VesselCatch item)
        {
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            VesselCatchCollection.Add(item);
            return AddSucceeded;
        }

        public void UpdateRecordInRepo(VesselCatch item)
        {
            if (item.PK == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < VesselCatchCollection.Count)
            {
                if (VesselCatchCollection[index].PK == item.PK)
                {
                    VesselCatchCollection[index] = item;
                    break;
                }
                index++;
            }
        }

        public int NextRecordNumber
        {
            get
            {
                if (VesselCatchCollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    return VesselCatches.MaxRecordNumber() + 1;
                }
            }
        }

        public void DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < VesselCatchCollection.Count)
            {
                if (VesselCatchCollection[index].PK == id)
                {
                    VesselCatchCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
        }
    }
}
