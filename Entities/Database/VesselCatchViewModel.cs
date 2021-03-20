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


        public int DeleteCatchFromUnload(VesselUnload unload)
        {
            int counter = 0;
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
                    .Where(t => t.Parent!=null &&  t.Parent.PK == parent.PK)
                    .Where(t => t.SpeciesText == speciesText)
                    .FirstOrDefault();
            }
            else
            {
                return VesselCatchCollection
                    .Where(t => t.Parent!=null &&  t.Parent.ODKRowID == parent._uuid)
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
