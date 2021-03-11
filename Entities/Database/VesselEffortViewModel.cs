﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class VesselEffortViewModel
    {
        public bool AddSucceeded { get; set; }
        public ObservableCollection<VesselEffort> VesselEffortCollection { get; set; }
        private VesselEffortRepository VesselEfforts { get; set; }


        
        public VesselEffortViewModel()
        {
            VesselEfforts = new VesselEffortRepository();
            VesselEffortCollection = new ObservableCollection<VesselEffort>(VesselEfforts.VesselEfforts);
            VesselEffortCollection.CollectionChanged += LandingSiteSamplingCollection_CollectionChanged;
        }
        public bool ClearRepository()
        {
            VesselEffortCollection.Clear();
            return VesselEfforts.ClearTable();
        }

        public List<VesselEffortFlattened> GetAllFlattenedItems(bool tracked=false)
        {
            List<VesselEffortFlattened> thisList = new List<VesselEffortFlattened>();
            if (tracked)
            {
                foreach (var item in VesselEffortCollection
                    .Where(t=>t.Parent.OperationIsTracked==tracked))
                {
                    thisList.Add(new VesselEffortFlattened(item));
                }
            }
            else
            {
                foreach (var item in VesselEffortCollection)
                {
                    thisList.Add(new VesselEffortFlattened(item));
                }
            }
            return thisList;
        }
        public List<VesselEffort> GetAllVesselEfforts()
        {
            return VesselEffortCollection.ToList();
        }

        //public List<VesselEffort> getLandingSiteSamplings(LandingSite ls, FishingGround fg, DateTime samplingDate)
        //{
        //    return VesselEffortCollection
        //        .Where(t => t.LandingSiteID == ls.LandingSiteID)
        //        .Where(t => t.FishingGroundID == fg.Code)
        //        .Where(t => t.SamplingDate == samplingDate).ToList();
        //}

        public VesselEffort getVesselEffort(ExcelEffortRepeat sheet)
        {
            return VesselEffortCollection
                .Where(t => t.EffortSpecID == sheet.EffortTypeID)
                .Where(t => t.Parent.FishingVessel.ID == sheet.Parent.FishingVessel.ID).FirstOrDefault();
        }


        public VesselEffort getVesselEffort(int pk)
        {
            return VesselEffortCollection.FirstOrDefault(n => n.PK == pk);
        }


        private void LandingSiteSamplingCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        AddSucceeded = VesselEfforts.Add(VesselEffortCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<VesselEffort> tempListOfRemovedItems = e.OldItems.OfType<VesselEffort>().ToList();
                        VesselEfforts.Delete(tempListOfRemovedItems[0].PK);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<VesselEffort> tempList = e.NewItems.OfType<VesselEffort>().ToList();
                        VesselEfforts.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return VesselEffortCollection.Count; }
        }

        public bool AddRecordToRepo(VesselEffort item)
        {
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            VesselEffortCollection.Add(item);
            return AddSucceeded;
        }

        public void UpdateRecordInRepo(VesselEffort item)
        {
            if (item.PK == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < VesselEffortCollection.Count)
            {
                if (VesselEffortCollection[index].PK == item.PK)
                {
                    VesselEffortCollection[index] = item;
                    break;
                }
                index++;
            }
        }

        public int NextRecordNumber
        {
            get
            {
                if (VesselEffortCollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    return VesselEfforts.MaxRecordNumber() + 1;
                }
            }
        }

        public void DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < VesselEffortCollection.Count)
            {
                if (VesselEffortCollection[index].PK == id)
                {
                    VesselEffortCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
        }
    }
}
