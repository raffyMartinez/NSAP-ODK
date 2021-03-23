using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using NSAP_ODK.Entities.Database;

namespace NSAP_ODK.Entities
{
    public class NSAPEnumeratorViewModel
    {
        public ObservableCollection<NSAPEnumerator> NSAPEnumeratorCollection { get; set; }
        private NSAPEnumeratorRepository NSAPEnumerators { get; set; }

        public bool EditSuccess { get; internal set; }



        public List<EnumeratorSummary> SummaryByRegion(NSAPRegion region)
        {
            List<EnumeratorSummary> summaries = new List<EnumeratorSummary>();

            return summaries;
        }

        public List<EnumeratorSummary> GetSummary(NSAPRegion region)
        {
            List<EnumeratorSummary> summaries = new List<EnumeratorSummary>();
            var unloads = NSAPEntities.VesselUnloadViewModel.GetAllVesselUnloads(region);
            var enumerators = unloads.GroupBy(t => t.EnumeratorName).OrderBy(t => t.Key);

            foreach (var e in enumerators)
            {
                if (e.Key.Length > 0)
                {
                    var enumerator_date = unloads
                        .Where(t => t.EnumeratorName == e.Key)
                        //.GroupBy(t => t.SamplingDate.Date)
                        .GroupBy(t=>((DateTime)t.DateAddedToDatabase).Date)
                        .OrderByDescending(t => t.Key).ToList();

                    var ve = unloads
                        .Where(t => t.EnumeratorName == e.Key && ((DateTime)t.DateAddedToDatabase).Date == enumerator_date[0].Key)
                        .OrderByDescending(t=>t.DateAddedToDatabase)
                        .ToList();

                    var unloads_latest = ve.Where(t => t.Parent.GearUsedName == ve[0].Parent.GearUsedName).ToList();

                    EnumeratorSummary es = new EnumeratorSummary
                    {
                        EnumeratorName = e.Key,
                        LandingSite = ve[0].Parent.Parent.LandingSiteName,
                        Gear = ve[0].Parent.GearUsedName,
                        VesselUnloads = unloads_latest,
                        NumberOfLandingsSampled = unloads_latest.Count,
                        DateOfLatestSampling = ve[0].SamplingDate,
                        UploadDate = (DateTime)ve[0].DateAddedToDatabase
                    };
                    summaries.Add(es);
                }
            }

            return summaries.OrderByDescending(t => t.UploadDate).ToList();
        }
        public List<EnumeratorSummary> GetSummary(NSAPEnumerator enumerator)
        {
            List<EnumeratorSummary> summaries = new List<EnumeratorSummary>();
            var unloads = NSAPEntities.VesselUnloadViewModel.GetAllVesselUnloads(enumerator);
            var landingSites = unloads.GroupBy(t => t.Parent.Parent.LandingSiteName).OrderBy(t => t.Key);
            foreach (var ls in landingSites)
            {
                var gear_landingSites = unloads
                    .Where(t => t.Parent.Parent.LandingSiteName == ls.Key)
                    .GroupBy(t => t.Parent.GearUsedName);

                foreach (var g in gear_landingSites)
                {
                    var unloads_row = unloads.Where(t => t.Parent.GearUsedName == g.Key && t.Parent.Parent.LandingSiteName == ls.Key).ToList();
                    EnumeratorSummary es = new EnumeratorSummary
                    {
                        LandingSite = ls.Key,
                        Gear = g.Key,
                        //NumberOfLandingsSampled = unloads.Count(t => t.Parent.GearUsedName == g.Key && t.Parent.Parent.LandingSiteName == ls.Key),
                        NumberOfLandingsSampled  = unloads_row.Count,
                        VesselUnloads = unloads_row,
                        NumberOfLandingsWithCatchComposition = unloads.
                            Where(t => t.ListVesselCatch.Count > 0 &&
                            t.Parent.Parent.LandingSiteName == ls.Key &&
                            t.Parent.GearUsedName == g.Key).ToList().Count,
                        DateOfFirstSampling = unloads.Where(t => t.Parent.GearUsedName == g.Key && t.Parent.Parent.LandingSiteName == ls.Key).OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate,
                        DateOfLatestSampling = unloads.Where(t => t.Parent.GearUsedName == g.Key && t.Parent.Parent.LandingSiteName == ls.Key).OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate,
                        UploadDate = unloads.Where(t => t.Parent.GearUsedName == g.Key && t.Parent.Parent.LandingSiteName == ls.Key).OrderByDescending(t => t.DateTimeSubmitted).FirstOrDefault().DateTimeSubmitted,
                    };
                    summaries.Add(es);
                }
            }
            return summaries;
        }
        public List<EnumeratorSummary> GetSummary(NSAPEnumerator enumerator, DateTime monthSampled)
        {
            List<EnumeratorSummary> summaries = new List<EnumeratorSummary>();
            var unloads = NSAPEntities.VesselUnloadViewModel.GetAllVesselUnloads(enumerator, monthSampled);
            var landingSites = unloads.GroupBy(t => t.Parent.Parent.LandingSiteName).OrderBy(t => t.Key);
            foreach (var ls in landingSites)
            {
                var gear_landingSites = unloads
                    .Where(t => t.Parent.Parent.LandingSiteName == ls.Key)
                    .GroupBy(t => t.Parent.GearUsedName);

                foreach (var g in gear_landingSites)
                {
                    var unloads_row = unloads.Where(t => t.Parent.GearUsedName == g.Key && t.Parent.Parent.LandingSiteName == ls.Key).ToList();
                    EnumeratorSummary es = new EnumeratorSummary
                    {
                        LandingSite = ls.Key,
                        Gear = g.Key,
                        NumberOfLandingsWithCatchComposition = unloads.
                            Where(t => t.ListVesselCatch.Count > 0 &&
                            t.Parent.Parent.LandingSiteName == ls.Key &&
                            t.Parent.GearUsedName == g.Key).ToList().Count,
                        //NumberOfLandingsSampled = unloads.Count(t => t.Parent.GearUsedName == g.Key && t.Parent.Parent.LandingSiteName == ls.Key),
                        NumberOfLandingsSampled = unloads_row.Count,
                        VesselUnloads = unloads_row,
                        MonthOfSampling = monthSampled,
                        DateOfFirstSampling = unloads.Where(t => t.Parent.GearUsedName == g.Key && t.Parent.Parent.LandingSiteName == ls.Key).OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate,
                        DateOfLatestSampling = unloads.Where(t => t.Parent.GearUsedName == g.Key && t.Parent.Parent.LandingSiteName == ls.Key).OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate,
                        UploadDate = unloads.Where(t => t.Parent.GearUsedName == g.Key && t.Parent.Parent.LandingSiteName == ls.Key).OrderByDescending(t => t.DateTimeSubmitted).FirstOrDefault().DateTimeSubmitted,
                    };
                    summaries.Add(es);
                }
            }


            return summaries;
        }


        //    var itemsVesselSamplings = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection
        //.Where(t => t.VesselID == null && t.VesselName != null && t.VesselText.Length > 0)
        //.GroupBy(t => new { Sector = t.Sector, LandingSite = t.Parent.Parent.LandingSiteName, Name = t.VesselName })
        //.Select(vessel => new
        //{
        //    NameVessel = vessel.Key.Name,
        //    SectorVessel = vessel.Key.Sector,
        //    LandingSiteVessel = vessel.Key.LandingSite
        //}
        //)
        //.ToList();

        //    var list = new List<OrphanedFishingVessel>();
        //        //var listNames = new List<string>();


        //        foreach (var item in itemsVesselSamplings)
        //        {
        //            //listNames.Add(item.NameVessel);
        //            var orphan = new OrphanedFishingVessel
        //            {
        //                Name = item.NameVessel,
        //                VesselUnloads = NSAPEntities.VesselUnloadViewModel.GetSampledLandingsOfVessel(item.NameVessel, item.SectorVessel, item.LandingSiteVessel),
        //            };

        //    list.Add(orphan);
        //        }

        //List<OrphanedFishingVessel> sortedList = list.OrderBy(t => t.Sector).ThenBy(t => t.Name).ThenBy(t => t.LandingSiteName).ToList();
        //        return sortedList;

        public List<OrphanedEnumerator> OrphanedEnumerators()
        {
            var itemsVesselSamplings = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection
                .Where(t => t.NSAPEnumeratorID == null && t.EnumeratorText != null && t.EnumeratorText.Length > 0)
                .GroupBy(t => new { LandingsSiteName = t.Parent.Parent.LandingSiteName, EnumeratorName = t.EnumeratorName })
                .Select(enumerator => new
                {
                    LandingSiteName = enumerator.Key.LandingsSiteName,
                    EnumeratorName = enumerator.Key.EnumeratorName
                }
                 )
                .ToList();

            var list = new List<OrphanedEnumerator>();
            var listNames = new List<string>();

            var itemsLandingSiteSampling = NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection
                .Where(t => t.EnumeratorID == null && t.EnumeratorText != null && t.EnumeratorText.Length > 0)
                .GroupBy(t => new { Name = t.EnumeratorText, LandingSite = t.LandingSiteName })
                .Select(enumerator => new
                {
                    LandingSiteName = enumerator.Key.LandingSite,
                    Name = enumerator.Key.Name

                }
                 )
                .ToList();

            foreach (var item in itemsVesselSamplings)
            {
                listNames.Add(item.EnumeratorName);
                var orphan = new OrphanedEnumerator
                {
                    Name = item.EnumeratorName,
                    SampledLandings = NSAPEntities.VesselUnloadViewModel.GetSampledLandings(item.EnumeratorName, item.LandingSiteName),
                    LandingSiteSamplings = NSAPEntities.LandingSiteSamplingViewModel.GetSampledLandings(item.EnumeratorName, item.LandingSiteName)
                };

                list.Add(orphan);
            }

            foreach (var sl in itemsLandingSiteSampling)
            {
                if (!listNames.Contains(sl.Name))
                {
                    var orphan = new OrphanedEnumerator
                    {
                        Name = sl.Name,
                        SampledLandings = NSAPEntities.VesselUnloadViewModel.GetSampledLandings(sl.Name,sl.LandingSiteName),
                        LandingSiteSamplings = NSAPEntities.LandingSiteSamplingViewModel.GetSampledLandings(sl.Name,sl.LandingSiteName)
                    };

                    list.Add(orphan);
                }
            }
            List<OrphanedEnumerator> sortedList = list.OrderBy(t => t.Name).ToList();
            return sortedList;

        }

        public List<OrphanedEnumerator> OrphanedEnumerators1()
        {
            var itemsVesselSamplings = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection
                .Where(t => t.NSAPEnumeratorID == null && t.EnumeratorText != null && t.EnumeratorText.Length > 0)
                .OrderBy(t => t.EnumeratorName)
                .GroupBy(t => t.EnumeratorText)
                .ToList();

            var list = new List<OrphanedEnumerator>();
            var listNames = new List<string>();

            var itemsLandingSiteSampling = NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection
                .Where(t => t.EnumeratorID == null && t.EnumeratorText != null && t.EnumeratorText.Length > 0)
                .OrderBy(t => t.EnumeratorText)
                .GroupBy(t => t.EnumeratorText).ToList();

            foreach (var item in itemsVesselSamplings)
            {
                listNames.Add(item.Key);
                var orphan = new OrphanedEnumerator
                {
                    Name = item.Key,
                    SampledLandings = NSAPEntities.VesselUnloadViewModel.GetSampledLandings(item.Key),
                    LandingSiteSamplings = NSAPEntities.LandingSiteSamplingViewModel.GetSampledLandings(item.Key)
                };

                list.Add(orphan);
            }

            foreach (var sl in itemsLandingSiteSampling)
            {
                if (!listNames.Contains(sl.Key))
                {
                    var orphan = new OrphanedEnumerator
                    {
                        Name = sl.Key,
                        SampledLandings = NSAPEntities.VesselUnloadViewModel.GetSampledLandings(sl.Key),
                        LandingSiteSamplings = NSAPEntities.LandingSiteSamplingViewModel.GetSampledLandings(sl.Key)
                    };

                    list.Add(orphan);
                }
            }
            List<OrphanedEnumerator> sortedList = list.OrderBy(t => t.Name).ToList();
            return sortedList;

        }
        public NSAPEnumeratorViewModel()
        {
            NSAPEnumerators = new NSAPEnumeratorRepository();
            NSAPEnumeratorCollection = new ObservableCollection<NSAPEnumerator>(NSAPEnumerators.NSAPEnumerators);
            NSAPEnumeratorCollection.CollectionChanged += NSAPEnumeratorCollection_CollectionChanged;
        }


        public int Count

        {
            get { return NSAPEnumeratorCollection.Count; }
        }
        public NSAPEnumerator GetLatestAdded()
        {
            return NSAPEnumeratorCollection.OrderByDescending(t => t.ID).FirstOrDefault();
        }
        public bool EnumeratorName(string name)
        {
            foreach (NSAPEnumerator nse in NSAPEnumeratorCollection)
            {
                if (nse.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public NSAPEnumerator GetNSAPEnumerator(int id)
        {
            CurrentEntity = NSAPEnumeratorCollection.FirstOrDefault(n => n.ID == id);
            return CurrentEntity;
        }

        public List<NSAPEnumerator> GetALlNSAPENumerators()
        {
            return NSAPEnumeratorCollection.ToList();
        }

        public string GearList(NSAPRegion nsapRegion)
        {
            string list = "";
            foreach (var g in nsapRegion.Gears)
            {
                list += $"{g.Gear.ToString()}, ";
            }
            return list.Trim(new char[] { ' ', ',' });
        }

        public string EnumeratorList(NSAPRegion nsapRegion)
        {
            string list = "";
            foreach (var en in nsapRegion.NSAPEnumerators)
            {
                list += $"{en.Enumerator.ToString()}, ";
            }
            return list.Trim(new char[] { ' ', ',' });
        }

        public string FishingVesselList(NSAPRegion nsapRegion)
        {
            string list = "";
            foreach (var fg in nsapRegion.FishingVessels)
            {
                list += $"{fg.FishingVessel.ToString()}, ";
            }
            return list.Trim(new char[] { ' ', ',' });
        }

        public int NextRecordNumber
        {
            get
            {
                if (NSAPEnumeratorCollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    //return NSAPEnumeratorCollection.Max(t => t.ID) + 1;
                    return NSAPEnumerators.MaxRecordNumber() + 1;
                }
            }
        }


        public NSAPEnumerator CurrentEntity { get; set; }
        private void NSAPEnumeratorCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            EditSuccess = false;
            NSAPEnumerator editedEnumerator = new NSAPEnumerator();
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        editedEnumerator = NSAPEnumeratorCollection[newIndex];
                        if (NSAPEnumerators.Add(editedEnumerator))
                        {
                            CurrentEntity = editedEnumerator;
                            EditSuccess = true;
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<NSAPEnumerator> tempListOfRemovedItems = e.OldItems.OfType<NSAPEnumerator>().ToList();
                        editedEnumerator = tempListOfRemovedItems[0];
                        NSAPEnumerators.Delete(editedEnumerator.ID);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<NSAPEnumerator> tempList = e.NewItems.OfType<NSAPEnumerator>().ToList();
                        editedEnumerator = tempList[0];
                        EditSuccess = NSAPEnumerators.Update(editedEnumerator);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
            //EntityChangedEventArgs args = new EntityChangedEventArgs(editedLandingSite.GetType().Name,editedLandingSite);
            //EntityChanged?.Invoke(this, args);
        }

        public bool AddRecordToRepo(NSAPEnumerator nse)
        {
            if (nse == null)
                throw new ArgumentNullException("Error: The argument is Null");
            NSAPEnumeratorCollection.Add(nse);
            return EditSuccess;
        }

        public bool UpdateRecordInRepo(NSAPEnumerator nse)
        {
            if (nse.ID == 0)
                throw new Exception("Error: ID cannot be null");

            int index = 0;
            while (index < NSAPEnumeratorCollection.Count)
            {
                if (NSAPEnumeratorCollection[index].ID == nse.ID)
                {
                    NSAPEnumeratorCollection[index] = nse;
                    break;
                }
                index++;
            }
            return EditSuccess;
        }

        public void DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < NSAPEnumeratorCollection.Count)
            {
                if (NSAPEnumeratorCollection[index].ID == id)
                {
                    NSAPEnumeratorCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
        }

        public bool EntityValidated(NSAPEnumerator nse, out List<EntityValidationMessage> entityMessages, bool isNew)
        {
            entityMessages = new List<EntityValidationMessage>();

            if (nse.Name.Length < 5)
            {
                entityMessages.Add(new EntityValidationMessage("Name is too short"));
            }

            if (NSAPEnumeratorCollection.Where(t => t.Name == nse.Name).FirstOrDefault() != null)
            {
                entityMessages.Add(new EntityValidationMessage("Name already exists"));
            }
            return entityMessages.Count == 0;
        }


    }
}