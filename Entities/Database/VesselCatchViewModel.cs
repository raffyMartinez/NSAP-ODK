using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class VesselCatchViewModel : IDisposable
    {
        public bool EditSuccess { get; set; }
        public ObservableCollection<VesselCatch> VesselCatchCollection { get; set; }
        private VesselCatchRepository VesselCatches { get; set; }

        private static StringBuilder _csv = new StringBuilder();
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int? MissingCatchInfoCount { get; set; }
        public static int CurrentIDNumber { get; set; }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                VesselCatchCollection.Clear();
                VesselCatchCollection = null;
                VesselCatches = null;

            }
            // free native resources if there are any.
        }
        public List<VesselCatchEdited> GetVesselCatchEditedList(VesselUnload unload)
        {
            List<VesselCatchEdited> vces = new List<VesselCatchEdited>();
            foreach (var vc in VesselCatchCollection.Where(t => t.Parent.PK == unload.PK))
            {
                vces.Add(new VesselCatchEdited(vc));
            }

            return vces;
        }

        public List<VesselUnloadWithMaturityFlattened> GetUnloadsWithMaturity(NSAPRegion rg, FishingGround fg)
        {
            List<VesselUnloadWithMaturityFlattened> list = new List<VesselUnloadWithMaturityFlattened>();
            HashSet<int> parentPKs = new HashSet<int>();
            var catchesWithMaturity = VesselCatchCollection
                .Where(t => t.ListCatchMaturity.Count > 0 &&
                    t.Parent.Parent.Parent.FishingGround.Code == fg.Code &&
                    t.Parent.Parent.Parent.NSAPRegionID == rg.Code)
                .OrderBy(t => t.Parent.PK)
                .ToList();

            VesselUnloadWithMaturityFlattened vumf = null;
            foreach (var c in catchesWithMaturity)
            {
                if (!parentPKs.Contains(c.Parent.PK))
                {
                    vumf = new VesselUnloadWithMaturityFlattened
                    {
                        SamplingDayID = c.Parent.Parent.Parent.PK,
                        Region = rg.ShortName,
                        FMA = c.Parent.Parent.Parent.FMA.Name,
                        FishingGround = fg.Name,
                        LandingSite = c.Parent.Parent.Parent.LandingSiteName,
                        SamplingDateTime = c.Parent.SamplingDate,
                        GearUnloadID = c.Parent.Parent.PK,
                        Gear = c.Parent.Parent.GearUsedName,
                        VesselUnloadID = c.Parent.PK,
                        Enumerator = c.Parent.EnumeratorName,
                        IsBoatUsed = c.Parent.IsBoatUsed,
                        Vessel = c.Parent.VesselName,
                        CatchTotalWt = c.Parent.WeightOfCatch,
                        IsTracked = c.Parent.OperationIsTracked,
                        GPS = c.Parent.GPSText,
                        Departure = c.Parent.DepartureFromLandingSite,
                        Arrival = c.Parent.ArrivalAtLandingSite,
                        RowID = c.Parent.ODKRowID,
                        XFormIdentifier = c.Parent.XFormIdentifier,
                        XFormDate = c.Parent.XFormDate,
                        UserName = c.Parent.UserName,
                        DeviceID = c.Parent.DeviceID,
                        Submitted = c.Parent.DateTimeSubmitted,
                        FormVersion = c.Parent.FormVersion,
                        Notes = c.Parent.Notes,
                        DateAddedToDatabase = c.Parent.DateAddedToDatabase,
                        Sector = c.Parent.Sector,
                    };

                    if (c.Parent.ListFishingGroundGrid.Count > 0)
                    {
                        vumf.FishingGroundGird = c.Parent.ListFishingGroundGrid[0].ToString();
                        vumf.Longitude = c.Parent.ListFishingGroundGrid[0].GridCell.Coordinate.Longitude;
                        vumf.Latitude = c.Parent.ListFishingGroundGrid[0].GridCell.Coordinate.Latitude;

                    }

                    list.Add(vumf);
                    parentPKs.Add(c.Parent.PK);

                }
                vumf.ListOfCatchWithMaturity.Add(c);

            }

            return list;
        }

        public static void ClearCSV()
        {
            _csv.Clear();
        }
        //private void CheckTableColumns()
        //{
        //    var tableColumns = CreateTablesInAccess.GetColumnNames("dbo_vessel_catch", makeLowerCase: true);
        //    if (tableColumns.Contains(""))
        //}
        public VesselCatchViewModel(VesselUnload vu)
        {
            if (vu != null)
            {
                VesselCatches = new VesselCatchRepository(vu);
                VesselCatchCollection = new ObservableCollection<VesselCatch>(VesselCatches.VesselCatches);
                VesselCatchCollection.CollectionChanged += VesselCatches_CollectionChanged;
            }
        }
        public VesselCatchViewModel(bool isNew = false)
        {
            VesselCatches = new VesselCatchRepository(isNew);
            if (isNew)
            {
                VesselCatchCollection = new ObservableCollection<VesselCatch>();
            }
            else
            {
                VesselCatchCollection = new ObservableCollection<VesselCatch>(VesselCatches.VesselCatches);
            }
            VesselCatchCollection.CollectionChanged += VesselCatches_CollectionChanged;
        }

        public static List<OrphanedSpeciesName> OrphanedSpeciesNamesStatic(bool getMultiLine = false)
        {
            List<OrphanedSpeciesName> list = new List<OrphanedSpeciesName>();

            List<OrphanedSpeciesNameRaw> osnr_list = new List<OrphanedSpeciesNameRaw>();

            osnr_list = VesselCatchRepository.GetOrphanedSpecies(getMultiLine);//.OrderBy(t=>t.OrphanedSpName).ToList();

            var gr_osn = osnr_list.GroupBy(t => new
            {
                region = t.RegionName,
                fma = t.FMAName,
                fishing_ground = t.FishingGroundName,
                species = t.OrphanedSpName,
                hash_code = t.HashCode,
                taxa = t.Taxa
            })
                .Select(osn => new
                {
                    region = osn.Key.region,
                    fma = osn.Key.fma,
                    fishing_ground = osn.Key.fishing_ground,
                    species = osn.Key.species,
                    hash_code = osn.Key.hash_code,
                    taxa = osn.Key.taxa
                }).ToList();

            foreach (var item in gr_osn)
            {
                OrphanedSpeciesName o = new OrphanedSpeciesName
                {
                    Name = item.species,
                    Region = item.region,
                    FMA = item.fma,
                    FishingGround = item.fishing_ground,
                    HashCode = item.hash_code,
                    Taxa = item.taxa,
                };

                o.SampledLandings = GetUnloadsfromOrphanedSpecies(o, osnr_list);
                list.Add(o);
            }


            return list;
        }
        public static int GetNumberOfLanddingsWithOrphanedSpecies()
        {
            return VesselCatchRepository.CountOfLandingsWithOrphanedSpName();
        }
        private static List<VesselUnload> GetUnloadsfromOrphanedSpecies(OrphanedSpeciesName os, List<OrphanedSpeciesNameRaw> osnr)
        {
            List<VesselUnload> vus = new List<VesselUnload>();

            foreach (OrphanedSpeciesNameRaw item in osnr.Where(t => t.HashCode == os.HashCode))
            {
                VesselUnload vu = new VesselUnload
                {
                    PK = item.VesselUnloadID,
                };
                //vu.VesselCatchViewModel = new VesselCatchViewModel(vu);
                vus.Add(vu);

            }
            return vus;
        }

        public List<OrphanedSpeciesName> OrphanedSpeciesNames(bool getMultiLine = false)
        {
            List<IGrouping<string, VesselCatch>> catches = new List<IGrouping<string, VesselCatch>>();
            if (!getMultiLine)
            {
                catches = VesselCatchCollection
                    .Where(t => t.SpeciesID == null && t.SpeciesText != null && t.SpeciesText.Length > 0 && !t.SpeciesText.Contains('\n'))
                    .OrderBy(t => t.SpeciesText)
                    .GroupBy(t => t.SpeciesText).ToList();
            }
            else
            {
                catches = VesselCatchCollection
                    .Where(t => t.SpeciesID == null && t.SpeciesText != null && t.SpeciesText.Length > 0 && t.SpeciesText.Contains('\n'))
                    .OrderBy(t => t.SpeciesText)
                    .GroupBy(t => t.SpeciesText).ToList();
            }

            var list = new List<OrphanedSpeciesName>();
            foreach (var ct in catches)
            {

                var orphan = new OrphanedSpeciesName
                {
                    Name = ct.Key
                };

                var landings = new List<VesselUnload>();
                foreach (VesselCatch vc in VesselCatchCollection.Where(t => t.SpeciesText == ct.Key && t.SpeciesID == null))
                {
                    landings.Add(vc.Parent);
                }
                orphan.SampledLandings = landings;


                list.Add(orphan);
            }

            return list;

        }

        public int ConvertToIindividualCatches(string multilineItem, List<SpeciesName_Weight> items)
        {
            bool isDone = false;
            int counter = 0;
            foreach (var item in VesselCatchCollection.Where(t => t.CatchName.Trim(new char[] { ' ', '\n' }) == multilineItem).ToList())
            {
                if (item.VesselUnloadID == 605)
                {

                }
                counter = 0;
                foreach (var speciesName in items)
                {
                    isDone = false;
                    if (counter == 0 && !isDone)
                    {
                        isDone = true;
                        item.SpeciesText = speciesName.SpeciesName;
                        item.Catch_kg = null;
                        if (speciesName.Weight != null)
                        {
                            item.Catch_kg = speciesName.Weight;
                        }
                        var fsp = NSAPEntities.FishSpeciesViewModel.GetSpecies(item.SpeciesText);
                        item.TaxaCode = NSAPEntities.TaxaViewModel.NotIdentified.Code;
                        if (fsp != null)
                        {
                            item.SpeciesID = fsp.SpeciesCode;
                            item.TaxaCode = NSAPEntities.TaxaViewModel.FishTaxa.Code;
                        }
                        else
                        {
                            var nfsp = NSAPEntities.NotFishSpeciesViewModel.GetSpecies(item.SpeciesText);
                            if (nfsp != null)
                            {
                                item.SpeciesID = nfsp.SpeciesID;
                                item.TaxaCode = nfsp.Taxa.Code;
                            }


                        }

                        if (item.SpeciesID != null)
                        {
                            item.SpeciesText = multilineItem;
                        }

                        if (UpdateRecordInRepo(item))
                        {
                            counter++;
                        }
                    }
                    else
                    {
                        VesselCatch vc = new VesselCatch
                        {
                            SpeciesText = speciesName.SpeciesName,
                            Parent = item.Parent,
                            VesselUnloadID = item.Parent.PK,
                            PK = NextRecordNumber,
                            TaxaCode = NSAPEntities.TaxaViewModel.NotIdentified.Code
                        };
                        if (speciesName.Weight != null)
                        {
                            vc.Catch_kg = speciesName.Weight;
                        }

                        var fsp = NSAPEntities.FishSpeciesViewModel.GetSpecies(vc.SpeciesText);
                        if (fsp != null)
                        {
                            vc.SpeciesID = fsp.SpeciesCode;
                            vc.TaxaCode = NSAPEntities.TaxaViewModel.FishTaxa.Code;
                        }
                        else
                        {
                            var nfsp = NSAPEntities.NotFishSpeciesViewModel.GetSpecies(vc.SpeciesText);
                            if (nfsp != null)
                            {
                                vc.SpeciesID = nfsp.SpeciesID;
                                vc.TaxaCode = nfsp.Taxa.Code;
                            }
                        }


                        if (AddRecordToRepo(vc))
                        {
                            counter++;
                        }
                    }

                }
            }
            return counter;
        }
        public int DeleteCatchFromUnload(VesselUnload unload)
        {
            int counter = 0;
            var catchCollection = VesselCatchCollection.Where(t => t.Parent != null && t.Parent.PK == unload.PK).ToList();
            if (catchCollection != null && catchCollection.Count > 0)
            {
                foreach (VesselCatch vc in catchCollection)
                {
                    var listMaturity = vc.ListCatchMaturity;
                    if (listMaturity != null && listMaturity.Count > 0)
                    {
                        foreach (var item in listMaturity)
                        {
                            if (vc.CatchMaturityViewModel.DeleteRecordFromRepo(item.PK))
                            {
                                counter++;
                            }
                        }
                    }

                    var listLF = vc.ListCatchLenFreq;
                    if (listLF != null && listLF.Count > 0)
                    {
                        foreach (var item in listLF)
                        {
                            if (vc.CatchLenFreqViewModel.DeleteRecordFromRepo(item.PK))
                            {
                                counter++;
                            }
                        }
                    }

                    var listLW = vc.ListCatchLengthWeight;
                    if (listLW != null && listLF.Count > 0)
                    {
                        foreach (var item in listLW)
                        {
                            if (vc.CatchLengthWeightViewModel.DeleteRecordFromRepo(item.PK))
                            {
                                counter++;
                            }
                        }
                    }

                    var listL = vc.ListCatchLength;
                    if (listL != null && listL.Count > 0)
                    {
                        foreach (var item in listL)
                        {
                            if (vc.CatchLengthViewModel.DeleteRecordFromRepo(item.PK))
                            {
                                counter++;
                            }
                        }
                    }

                    if (unload.VesselCatchViewModel.DeleteRecordFromRepo(vc.PK))
                    {
                        counter++;
                    }
                }
            }
            return counter;
        }
        public int DeleteCatchFromUnloads(List<VesselUnload> listOfUnloads)
        {
            int counter = 0;
            foreach (var unload in listOfUnloads)
            {
                var catchCollection = VesselCatchCollection.Where(t => t.Parent.PK == unload.PK).ToList();
                if (catchCollection != null && catchCollection.Count > 0)
                {
                    foreach (VesselCatch vc in catchCollection)
                    {
                        var listMaturity = vc.ListCatchMaturity;
                        if (listMaturity != null && listMaturity.Count > 0)
                        {
                            foreach (var item in listMaturity)
                            {
                                if (NSAPEntities.CatchMaturityViewModel.DeleteRecordFromRepo(item.PK))
                                {
                                    counter++;
                                }
                            }
                        }

                        var listLF = vc.ListCatchLenFreq;
                        if (listLF != null && listLF.Count > 0)
                        {
                            foreach (var item in listLF)
                            {
                                if (NSAPEntities.CatchLenFreqViewModel.DeleteRecordFromRepo(item.PK))
                                {
                                    counter++;
                                }
                            }
                        }

                        var listLW = vc.ListCatchLengthWeight;
                        if (listLW != null && listLF.Count > 0)
                        {
                            foreach (var item in listLW)
                            {
                                if (NSAPEntities.CatchLengthWeightViewModel.DeleteRecordFromRepo(item.PK))
                                {
                                    counter++;
                                }
                            }
                        }

                        var listL = vc.ListCatchLength;
                        if (listL != null && listL.Count > 0)
                        {
                            foreach (var item in listL)
                            {
                                if (NSAPEntities.CatchLengthViewModel.DeleteRecordFromRepo(item.PK))
                                {
                                    counter++;
                                }
                            }
                        }

                        if (NSAPEntities.VesselCatchViewModel.DeleteRecordFromRepo(vc.PK))
                        {
                            counter++;
                        }
                    }
                }


            }
            return counter;
        }
        public List<OrphanedSpeciesName> OrphanedFishSpeciesNames(bool getMultiLine = false)
        {
            List<IGrouping<string, VesselCatch>> catches = new List<IGrouping<string, VesselCatch>>();
            if (!getMultiLine)
            {
                catches = VesselCatchCollection
                    .Where(t => t.TaxaCode == "FIS" && t.SpeciesID == null && t.SpeciesText != null && t.SpeciesText.Length > 0 && !t.SpeciesText.Contains('\n'))
                    .OrderBy(t => t.SpeciesText)
                    .GroupBy(t => t.SpeciesText).ToList();
            }
            else
            {
                catches = VesselCatchCollection
                    .Where(t => t.TaxaCode == "FIS" && t.SpeciesID == null && t.SpeciesText != null && t.SpeciesText.Length > 0 && t.SpeciesText.Contains('\n'))
                    .OrderBy(t => t.SpeciesText)
                    .GroupBy(t => t.SpeciesText).ToList();
            }

            var list = new List<OrphanedSpeciesName>();
            foreach (var ct in catches)
            {

                var orphan = new OrphanedSpeciesName
                {
                    Name = ct.Key
                };

                var landings = new List<VesselUnload>();
                foreach (VesselCatch vc in VesselCatchCollection.Where(t => t.SpeciesText == ct.Key && t.SpeciesID == null))
                {
                    landings.Add(vc.Parent);
                }
                orphan.SampledLandings = landings;


                list.Add(orphan);
            }

            return list;

        }

        public List<VesselCatch> MulitpleCatchNamesText()
        {
            return VesselCatchCollection
                .Where(t => t.SpeciesID != null && t.SpeciesText != null && t.SpeciesText.Contains('\n'))
                .OrderBy(t => t.SpeciesText)
                .ToList();
        }

        public VesselCatch getVesselCatch(FromJson.VesselLanding parent, int? speciesID, string speciesText)
        {
            if (speciesID == null)
            {
                return VesselCatchCollection
                    .Where(t => t.Parent != null && t.Parent.PK == parent.PK)
                    .Where(t => t.SpeciesText == speciesText)
                    .FirstOrDefault();
            }
            else
            {
                return VesselCatchCollection
                    .Where(t => t.Parent != null && t.Parent.ODKRowID == parent._uuid)
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
        public List<VesselCatchFlattened> GetAllFlattenedItems(bool tracked = false)
        {
            List<VesselCatchFlattened> thisList = new List<VesselCatchFlattened>();
            if (tracked)
            {
                foreach (var item in VesselCatchCollection
                    .Where(t => t.Parent.OperationIsTracked == tracked))
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
            return VesselCatchRepository.ClearTable();
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

        private static bool SetCSV(VesselCatch item)
        {
            string sp_id = item.SpeciesID == null ? "" : ((int)item.SpeciesID).ToString();
            //string tws = item.TWS == null ? "" : ((double)item.TWS).ToString();
            string catch_kg = item.Catch_kg == null ? "" : ((double)item.Catch_kg).ToString();
            string sample_kg = item.Sample_kg == null ? "" : ((double)item.Sample_kg).ToString();
            string price_sp = item.PriceOfSpecies == null ? "" : ((double)item.PriceOfSpecies).ToString();

            if (Utilities.Global.Settings.UsemySQL)
            {
                if (item.Catch_kg == null)
                {
                    catch_kg = @"\N";
                }

                if (item.Sample_kg == null)
                {
                    sample_kg = @"\N";
                }

                if (item.SpeciesID == null)
                {
                    sp_id = @"\N";
                }

                if (item.PriceOfSpecies == null)
                {
                    price_sp = @"\N";
                }

                // if (item.TWS == null)
                //{
                //   tws = @"\N";
                //}

                //_csv.AppendLine($"{item.PK},{item.Parent.PK},{sp_id},{catch_kg},{tws},{sample_kg},\"{item.TaxaCode}\",\"{item.SpeciesText}\"");
                _csv.AppendLine($"{item.PK},{item.Parent.PK},{sp_id},{catch_kg},{sample_kg},\"{item.TaxaCode}\",\"{item.SpeciesText}\"");
            }
            else
            {
                //_csv.AppendLine($"{item.PK},{item.Parent.PK},{sp_id},{catch_kg},{sample_kg},\"{item.TaxaCode}\",\"{item.SpeciesText}\",{tws}");
                _csv.AppendLine($"{item.PK},{item.Parent.PK},{sp_id},{catch_kg},{sample_kg},\"{item.TaxaCode}\",\"{item.SpeciesText}\",\"{item.WeighingUnit}\",{Convert.ToInt32(item.FromTotalCatch)},{price_sp},\"{item.PriceUnit}\"");
            }
            return true;
        }

        public static string CSV
        {
            get
            {
                if (Utilities.Global.Settings.UsemySQL)
                {
                    return $"{NSAPMysql.MySQLConnect.GetColumnNamesCSV("dbo_vessel_catch")}\r\n{_csv}";
                }
                else
                {
                    return $"{CreateTablesInAccess.GetColumnNamesCSV("dbo_vessel_catch")}\r\n{_csv}";
                }
            }
        }
        private void VesselCatches_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            EditSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        VesselCatch newItem = VesselCatchCollection[e.NewStartingIndex];
                        if (newItem.DelayedSave)
                        {
                            EditSuccess = SetCSV(newItem);
                        }
                        else
                        {
                            EditSuccess = VesselCatches.Add(newItem);
                        }
                        //int newIndex = e.NewStartingIndex;
                        //EditSuccess = VesselCatches.Add(VesselCatchCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<VesselCatch> tempListOfRemovedItems = e.OldItems.OfType<VesselCatch>().ToList();
                        EditSuccess = VesselCatches.Delete(tempListOfRemovedItems[0].PK);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<VesselCatch> tempList = e.NewItems.OfType<VesselCatch>().ToList();
                        EditSuccess = VesselCatches.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get
            {
                if (VesselCatchCollection == null)
                {
                    return 0;
                }
                else
                {
                    return VesselCatchCollection.Count;
                }
            }
        }

        public bool AddRecordToRepo(VesselCatch item)
        {
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            VesselCatchCollection.Add(item);
            return EditSuccess;
        }

        public bool UpdateRecordInRepo(VesselCatch item)
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
            return EditSuccess;
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

        public bool DeleteRecordFromRepo(int id)
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

            return EditSuccess;
        }
    }
}
