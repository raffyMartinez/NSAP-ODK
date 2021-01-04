using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using NSAP_ODK.Entities.Database;

namespace NSAP_ODK.Entities
{
    public class NSAPRegionViewModel
    {
        public Dictionary<NSAPRegion, DBSummary> RegionSummaryDictionary { get; private set; }
        public ObservableCollection<NSAPRegion> NSAPRegionCollection { get; set; }
        private NSAPRegionRepository NSAPRegions { get; set; }

        public Dictionary<string, NSAPRegionWithEntitiesRepository> NSAPRegionsWithEntitiesRepositories { get; set; }

        public void SetupSummary()
        {
            RegionSummaryDictionary = new Dictionary<NSAPRegion, DBSummary>();
            foreach(var region in NSAPRegionCollection)
            {
                DBSummary smmry = new DBSummary();
                smmry.FMACount = region.FMAs.Count(t => t.NSAPRegion.Code == region.Code);
                foreach( var fma in region.FMAs.Where(t=>t.NSAPRegion.Code==region.Code))
                {
                    smmry.FishingGroundCount += fma.FishingGroundCount;
                    foreach(var fg in fma.FishingGrounds )
                    {
                        smmry.LandingSiteCount += fg.LandingSiteCount;
                    }
                }
                smmry.FishingGearCount = region.Gears.Count(t => t.NSAPRegion.Code == region.Code);
                smmry.EnumeratorCount = region.NSAPEnumerators.Count(t => t.NSAPRegion.Code == region.Code);
                smmry.FishingVesselCount = region.FishingVessels.Count(t => t.NSAPRegion.Code == region.Code);
                int landings = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection.Count(t => t.Parent.Parent.NSAPRegion.Code == region.Code);
                smmry.VesselUnloadCount = landings;
                if (landings > 0)
                {
                    smmry.LastLandingFormattedDate = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection.OrderByDescending(t => t.SamplingDate).Where(t => t.Parent.Parent.NSAPRegion.Code == region.Code).FirstOrDefault().SamplingDate.ToString("MMM-dd-yyyy");
                    smmry.FirstLandingFormattedDate = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection.OrderBy(t => t.SamplingDate).Where(t => t.Parent.Parent.NSAPRegion.Code == region.Code).FirstOrDefault().SamplingDate.ToString("MMM-dd-yyyy");
                    smmry.LatestDownloadFormattedDate = ((DateTime)NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection.OrderByDescending(t => t.DateAddedToDatabase).Where(t => t.Parent.Parent.NSAPRegion.Code == region.Code).FirstOrDefault().DateAddedToDatabase).ToString("MMM-dd-yyyy");
                    smmry.GearUnloadCount = NSAPEntities.GearUnloadViewModel.GearUnloadCollection.Count(t => t.Parent.NSAPRegion.Code == region.Code);
                    smmry.CountCompleteGearUnload = NSAPEntities.GearUnloadViewModel.GearUnloadCollection.Count(t => t.Parent.NSAPRegion.Code == region.Code && t.Boats != null && t.Catch != null);
                    smmry.TrackedOperationsCount = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection.Count(t => t.Parent.Parent.NSAPRegion.Code == region.Code && t.OperationIsTracked == true);
                }
                RegionSummaryDictionary.Add(region, smmry);
            }
        }
        public NSAPRegionWithEntitiesRepository GetNSAPRegionWithEntitiesRepository(NSAPRegion nsapRegion)
        {
            return NSAPRegionsWithEntitiesRepositories[nsapRegion.Code];
        }

        public void SetNSAPRegionsWithEntitiesRepositories()
        {
            NSAPRegionsWithEntitiesRepositories = new Dictionary<string, NSAPRegionWithEntitiesRepository>();
            foreach (NSAPRegion nsapRegion in NSAPRegionCollection)
            {
                NSAPRegionWithEntitiesRepository nswer = new NSAPRegionWithEntitiesRepository(nsapRegion);
                NSAPRegionsWithEntitiesRepositories.Add(nsapRegion.Code, nswer);
            }
        }
        public NSAPRegionViewModel()
        {
            NSAPRegions = new NSAPRegionRepository();
            NSAPRegionCollection = new ObservableCollection<NSAPRegion>(NSAPRegions.NSAPRegions);
            NSAPRegionCollection.CollectionChanged += NSAPRegionCOllection_CollectionChanged;

            //NSAPRegionsWithEntitiesRepositories = new Dictionary<string, NSAPRegionWithEntitiesRepository>();
            //foreach (NSAPRegion nsapRegion in NSAPRegionCollection)
            //{
            //    NSAPRegionWithEntitiesRepository nswer = new NSAPRegionWithEntitiesRepository(nsapRegion);
            //    NSAPRegionsWithEntitiesRepositories.Add(nsapRegion.Code, nswer);
            //}
        }


        public NSAPRegionFMAFishingGround GetRegionFMAFishingGround(string regionCode, int fmaCode, string fishingGroundCode)
        {
            var region = NSAPRegionCollection.Where(t => t.Code == regionCode).FirstOrDefault();
            var regionFMA = region.FMAs.Where(t => t.FMAID == fmaCode).FirstOrDefault();
            return regionFMA.FishingGrounds.Where(t => t.FishingGroundCode == fishingGroundCode).FirstOrDefault();

        }
        public int Count

        {
            get { return NSAPRegionCollection.Count; }
        }

        public bool NSAPRegionNameExists(string regionName)
        {
            foreach (NSAPRegion nsr in NSAPRegionCollection)
            {
                if (nsr.Name == regionName)
                {
                    return true;
                }
            }
            return false;
        }

        public NSAPRegion GetNSAPRegion(string code)
        {
            CurrentEntity = NSAPRegionCollection.FirstOrDefault(n => n.Code == code);
            return CurrentEntity;
        }

        public NSAPRegion CurrentEntity { get; private set; }

        public List<NSAPRegion> GetAllNSAPRegions()
        {
            return NSAPRegionCollection.ToList();
        }

        private void NSAPRegionCOllection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NSAPRegion editedNSAPRegion = new NSAPRegion();
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        editedNSAPRegion = NSAPRegionCollection[newIndex];
                        NSAPRegions.Add(editedNSAPRegion);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<NSAPRegion> tempListOfRemovedItems = e.OldItems.OfType<NSAPRegion>().ToList();
                        editedNSAPRegion = tempListOfRemovedItems[0];
                        NSAPRegions.Delete(editedNSAPRegion.Code);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<NSAPRegion> tempList = e.NewItems.OfType<NSAPRegion>().ToList();
                        editedNSAPRegion = tempList[0];
                        NSAPRegions.Update(editedNSAPRegion);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
            //EntityChangedEventArgs args = new EntityChangedEventArgs(editedLandingSite.GetType().Name,editedLandingSite);
            //EntityChanged?.Invoke(this, args);
        }

        public void AddRecordToRepo(NSAPRegion nsr)
        {
            if (nsr == null)
                throw new ArgumentNullException("Error: The argument is Null");
            NSAPRegionCollection.Add(nsr);
        }

        public void UpdateRecordInRepo(NSAPRegion nsr)
        {
            if (nsr.Code == null)
                throw new Exception("Error: ID cannot be null");

            int index = 0;
            while (index < NSAPRegionCollection.Count)
            {
                if (NSAPRegionCollection[index].Code == nsr.Code)
                {
                    NSAPRegionCollection[index] = nsr;
                    break;
                }
                index++;
            }
        }

        public void DeleteRecordFromRepo(string code)
        {
            if (code == null)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < NSAPRegionCollection.Count)
            {
                if (NSAPRegionCollection[index].Code == code)
                {
                    NSAPRegionCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
        }

        public bool EntityValidated(NSAPRegion nsapRegion, out List<string> messages, bool isNew = false)
        {
            messages = new List<string>();

            if (nsapRegion.Name.Length < 5)
                messages.Add("NSAP Region's name must be at least 5 characters long");

            if (nsapRegion.Code == null)
                messages.Add("NSAP Region codecannot be empty");
            else if (nsapRegion.Code.Length > 3)
                messages.Add("NSAP Region code must not exceed 3 letters in length");
            return messages.Count == 0;
        }
    }
}