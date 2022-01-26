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
        public bool EditSuccess { get; set; }
        public ObservableCollection<VesselCatch> VesselCatchCollection { get; set; }
        private VesselCatchRepository VesselCatches { get; set; }

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
                    t.Parent.Parent.Parent.NSAPRegionID==rg.Code)
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

                    if(c.Parent.ListFishingGroundGrid.Count>0)
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
        public VesselCatchViewModel()
        {
            VesselCatches = new VesselCatchRepository();
            VesselCatchCollection = new ObservableCollection<VesselCatch>(VesselCatches.VesselCatches);
            VesselCatchCollection.CollectionChanged += VesselCatches_CollectionChanged;
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
            foreach (var item in VesselCatchCollection.Where(t => t.CatchName == multilineItem).ToList())
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
            EditSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        EditSuccess = VesselCatches.Add(VesselCatchCollection[newIndex]);
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
            get { return VesselCatchCollection.Count; }
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
