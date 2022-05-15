using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using NSAP_ODK.Entities.Database;
using System.Data;

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
        public DataSet DataSet()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("Enumerators");

            DataColumn dc = new DataColumn { ColumnName = "Region", DataType = typeof(string) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Name", DataType = typeof(string) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Id", DataType = typeof(string) };
            dt.Columns.Add(dc);



            foreach (var rgn in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection.OrderBy(t => t.Name))
            {
                foreach (var item in rgn.NSAPEnumerators.OrderBy(t => t.Enumerator.Name))
                {
                    var row = dt.NewRow();
                    row["Region"] = item.NSAPRegion.ShortName;
                    row["Name"] = item.Enumerator.Name;
                    row["Id"] = item.EnumeratorID;

                    dt.Rows.Add(row);
                }
            }
            ds.Tables.Add(dt);
            return ds;
        }
        public List<NSAPRegionEnumerator> GetFirstSamplingOfEnumerators()
        {
            HashSet<NSAPRegionEnumerator> thisList = new HashSet<NSAPRegionEnumerator>();
            bool success = false;
            foreach (var region in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection)
            {
                var unloads = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection.Where(
                    t => t.Parent.Parent.NSAPRegionID == region.Code);

                if (unloads.Count() > 0)
                    foreach (var nre in region.NSAPEnumerators.Where(t => t.DateFirstSampling == null))
                    {


                        {
                            var enumerators_firstSampling = unloads
                                .Where(t => t.NSAPEnumeratorID == nre.EnumeratorID)
                                .OrderBy(t => t.SamplingDate).FirstOrDefault();

                            if (enumerators_firstSampling != null)
                            {
                                nre.DateFirstSampling = enumerators_firstSampling.SamplingDate;
                                nre.FirstSampling = enumerators_firstSampling;
                                thisList.Add(nre);
                            }
                        }
                    }
            }

            return thisList.ToList();
        }

        public List<NSAPRegionEnumerator> FirstSamplingOfEnumerators { get; set; } = new List<NSAPRegionEnumerator>();
        public List<EnumeratorSummary> GetSummary(NSAPRegion region, bool summaryForAll = false)
        {
            List<EnumeratorSummary> summaries = new List<EnumeratorSummary>();

            var unloads = NSAPEntities.VesselUnloadViewModel.GetAllVesselUnloads(region, sorted: summaryForAll == false);
            var enumerators = unloads.GroupBy(t => t.EnumeratorName).OrderBy(t => t.Key);

            if (summaryForAll)
            {
                try
                {
                    if (NSAPEntities.NSAPEnumeratorViewModel.FirstSamplingOfEnumerators.Count == 0)
                    {
                        NSAPEntities.NSAPEnumeratorViewModel.FirstSamplingOfEnumerators = NSAPEntities.NSAPEnumeratorViewModel
                                                                                                .GetFirstSamplingOfEnumerators()
                                                                                                .Where(t => t.NSAPRegion.Code == region.Code)
                                                                                                .ToList();
                    }

                    foreach (var en in enumerators)
                    {
                        if (en.Key != null && en.Key.Length > 0)
                        {
                            var ve = unloads.Where(t => t.EnumeratorName == en.Key).ToList();
                            var lastSampling = ve.OrderByDescending(t => t.SamplingDate).FirstOrDefault();
                            EnumeratorSummary es = new EnumeratorSummary
                            {
                                EnumeratorName = en.Key,
                                RegionName = region.ShortName,
                                NumberOfLandingsSampled = en.Count(),
                                NumberOfLandingsWithCatchComposition = ve.Count(t => t.HasCatchComposition == true),
                                NumberOfTrackedLandings = ve.Count(t => t.OperationIsTracked == true),
                                DateOfLatestSampling = lastSampling.SamplingDate,
                                UploadDate = ve.OrderByDescending(t => t.DateTimeSubmitted).FirstOrDefault().DateTimeSubmitted,
                                LatestEformVersion = lastSampling.FormVersion.Replace("Version ", ""),
                                DateOfFirstSampling = NSAPEntities.NSAPEnumeratorViewModel.FirstSamplingOfEnumerators
                                                      .FirstOrDefault(t => t.NSAPRegion.Code == region.Code && t.Enumerator.Name == en.Key)
                                                      ?.DateFirstSampling
                            };
                            summaries.Add(es);
                        }
                    }
                    summaries = summaries.OrderBy(t => t.EnumeratorName).ToList();

                    EnumeratorSummary ens = new EnumeratorSummary
                    {
                        EnumeratorName = "Grand total",
                        NumberOfLandingsSampled = unloads.Count,
                        NumberOfLandingsWithCatchComposition = unloads.Count(t => t.HasCatchComposition == true),
                        NumberOfTrackedLandings = unloads.Count(t => t.OperationIsTracked == true)
                    };
                    summaries.Add(ens);
                }
                catch (Exception ex)
                {
                    Utilities.Logger.Log(ex);
                }

                //return summaries.OrderBy(t => t.EnumeratorName).ToList();
            }
            else
            {
                foreach (var e in enumerators)
                {
                    if (e.Key != null && e.Key.Length > 0)
                    {
                        var enumerator_date = unloads
                            .Where(t => t.EnumeratorName == e.Key)
                            //.GroupBy(t => t.SamplingDate.Date)
                            .GroupBy(t => ((DateTime)t.DateAddedToDatabase).Date)
                            .OrderByDescending(t => t.Key).ToList();

                        var ve = unloads
                            .Where(t => t.EnumeratorName == e.Key && ((DateTime)t.DateAddedToDatabase).Date == enumerator_date[0].Key)
                            .OrderByDescending(t => t.DateAddedToDatabase)
                            .ToList();

                        var unloads_latest = ve.Where(t => t.Parent.GearUsedName == ve[0].Parent.GearUsedName).OrderByDescending(t => t.SamplingDate).ToList();

                        EnumeratorSummary es = new EnumeratorSummary
                        {
                            EnumeratorName = e.Key,
                            LandingSite = ve[0].Parent.Parent.LandingSiteName,
                            Gear = ve[0].Parent.GearUsedName,
                            VesselUnloads = unloads_latest,
                            NumberOfLandingsSampled = unloads_latest.Count,
                            NumberOfTrackedLandings = unloads_latest.Count(t => t.OperationIsTracked == true),
                            NumberOfLandingsWithCatchComposition = unloads_latest.Count(t => t.HasCatchComposition == true),
                            DateOfLatestSampling = ve[0].SamplingDate,
                            UploadDate = (DateTime)ve[0].DateAddedToDatabase,
                            LatestEformVersion = unloads_latest[0].FormVersion.Replace("Version ", ""),
                            RegionName = region.ShortName
                        };
                        summaries.Add(es);


                    }
                }
                summaries = summaries.OrderBy(t => t.UploadDate).ToList();
            }

            return summaries;
        }

        public List<EnumeratorSummary> GetSummary(NSAPRegion region, string fishingGroundName)
        {
            List<EnumeratorSummary> summaries = new List<EnumeratorSummary>();
            var unloads = NSAPEntities.VesselUnloadViewModel.GetAllVesselUnloads(region.ShortName, fishingGroundName).OrderBy(t => t.EnumeratorName);
            foreach (var item in unloads)
            {

            }
            return summaries;
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
                        NumberOfLandingsSampled = unloads_row.Count,
                        VesselUnloads = unloads_row,
                        //NumberOfLandingsWithCatchComposition = unloads.
                        //    Where(t => t.ListVesselCatch.Count > 0 &&
                        //    t.Parent.Parent.LandingSiteName == ls.Key &&
                        //    t.Parent.GearUsedName == g.Key).ToList().Count,
                        //NumberOfLandingsWithCatchComposition = unloads.
                        //    Where(t => t.HasCatchComposition == true &&
                        //    t.Parent.Parent.LandingSiteName == ls.Key &&
                        //    t.Parent.GearUsedName == g.Key).ToList().Count,
                        NumberOfLandingsWithCatchComposition = unloads_row.Count(t => t.HasCatchComposition == true),
                        NumberOfTrackedLandings = unloads_row.Count(t => t.OperationIsTracked == true),
                        DateOfFirstSampling = unloads.Where(t => t.Parent.GearUsedName == g.Key && t.Parent.Parent.LandingSiteName == ls.Key).OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate,
                        DateOfLatestSampling = unloads.Where(t => t.Parent.GearUsedName == g.Key && t.Parent.Parent.LandingSiteName == ls.Key).OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate,
                        UploadDate = unloads.Where(t => t.Parent.GearUsedName == g.Key && t.Parent.Parent.LandingSiteName == ls.Key).OrderByDescending(t => t.DateTimeSubmitted).FirstOrDefault().DateTimeSubmitted,
                    };
                    summaries.Add(es);
                }
            }

            summaries = summaries.OrderBy(t => t.LandingSite).ThenBy(t => t.Gear).ToList();

            EnumeratorSummary ens = new EnumeratorSummary
            {
                LandingSite = "Grand total",
                NumberOfLandingsSampled = unloads.Count,
                NumberOfLandingsWithCatchComposition = unloads.Count(t => t.HasCatchComposition == true),
                NumberOfTrackedLandings = unloads.Count(t => t.OperationIsTracked == true)
            };
            summaries.Add(ens);

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
                        //NumberOfLandingsWithCatchComposition = unloads.
                        //    Where(t => t.ListVesselCatch.Count > 0 &&
                        //    t.Parent.Parent.LandingSiteName == ls.Key &&
                        //    t.Parent.GearUsedName == g.Key).ToList().Count,
                        //NumberOfLandingsSampled = unloads.Count(t => t.Parent.GearUsedName == g.Key && t.Parent.Parent.LandingSiteName == ls.Key),
                        NumberOfLandingsWithCatchComposition = unloads.
                            Where(t => t.HasCatchComposition == true &&
                            t.Parent.Parent.LandingSiteName == ls.Key &&
                            t.Parent.GearUsedName == g.Key).ToList().Count,
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
            return NSAPEntities.SummaryItemViewModel.GetOrphanedEnumerators();
        }
        public List<OrphanedEnumerator> OrphanedEnumerators1()
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
                        SampledLandings = NSAPEntities.VesselUnloadViewModel.GetSampledLandings(sl.Name, sl.LandingSiteName),
                        LandingSiteSamplings = NSAPEntities.LandingSiteSamplingViewModel.GetSampledLandings(sl.Name, sl.LandingSiteName)
                    };

                    list.Add(orphan);
                }
            }
            List<OrphanedEnumerator> sortedList = list.OrderBy(t => t.Name).ToList();
            return sortedList;

        }

        public List<OrphanedEnumerator> OrphanedEnumerators11()
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
                        EditSuccess = NSAPEnumerators.Delete(editedEnumerator.ID);
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

        public bool DeleteRecordFromRepo(int id)
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
            return EditSuccess;
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