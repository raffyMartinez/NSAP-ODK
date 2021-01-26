using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using NSAP_ODK.Utilities;
namespace NSAP_ODK.Entities.Database
{
    public class GearUnloadViewModel
    {
        public bool EditSuccess;
        public ObservableCollection<GearUnload> GearUnloadCollection { get; set; }
        private GearUnloadRepository GearUnloads { get; set; }

        public GearUnloadViewModel()
        {
            GearUnloads = new GearUnloadRepository();
            GearUnloadCollection = new ObservableCollection<GearUnload>(GearUnloads.GearUnloads);
            GearUnloadCollection.CollectionChanged += GearUnloadCollection_CollectionChanged;
        }

        public int CountCompletedGearUnload
        {
           //get { return GearUnloadCollection.Where(t => t.Boats != null).Where(t => t.Catch != null).Count(); }
           get { return GearUnloadCollection.Count(t => (t.Catch!=null && t.Boats!=null)); }
        }
        public List<GearUnload> GetAllGearUnloads()
        {
            return GearUnloadCollection.ToList();
        }


        public void UndoChangesToGearUnloadBoatCatch(List<GearUnload> gearUnloadList)
        {
            if (gearUnloadList != null && gearUnloadList.Count > 0)
            {
                foreach (var item in gearUnloadList)
                {
                    var originalItem = CopyOfGearUnloadList.FirstOrDefault(t => t.PK == item.PK);
                    item.Boats = originalItem.Boats;
                    item.Catch = originalItem.Catch;
                }
            }
        }


        public void SaveChangesToBoatAndCatch(List<GearUnload> listToSave)
        {
            foreach(var item in listToSave)
            {
                try
                {
                    var original = CopyOfGearUnloadList.FirstOrDefault(t => t.PK == item.PK);

                    if (item.Boats != original.Boats || item.Catch != original.Catch)
                    {
                        UpdateRecordInRepo(item);
                    }
                }
                catch(Exception ex)
                {
                    Logger.Log(ex);
                }
            }
            CopyOfGearUnloadList = null;
        }
        public List<GearUnload> CopyOfGearUnloadList { get; internal set; }

        public List<GearUnload> GetAllGearUnloads(TreeViewModelControl.AllSamplingEntitiesEventHandler parameters, bool createCopyOfList = true)
        {
            CopyOfGearUnloadList = new List<GearUnload>();
            var list = new List<GearUnload>();
            string ls = "";
            switch(parameters.ContextMenuTopic)
            {
                case "contextMenuGearUnloadMonth":
                    ls = parameters.LandingSite == null ? parameters.LandingSiteText : parameters.LandingSite.LandingSiteName;
                    list = GearUnloadCollection
                        .Where(t => t.Parent.NSAPRegionID == parameters.NSAPRegion.Code)
                        .Where(t => t.Parent.FMAID == parameters.FMA.FMAID)
                        .Where(t => t.Parent.FishingGroundID == parameters.FishingGround.Code)
                        .Where(t => t.Parent.LandingSiteName == ls)
                        .Where(t=>t.Parent.SamplingDate.Year==((DateTime)parameters.MonthSampled).Year)
                        .Where(t=>t.Parent.SamplingDate.Month==((DateTime)parameters.MonthSampled).Month)
                        .OrderBy(t => t.Parent.LandingSiteName)
                        .ThenBy(t => t.Parent.SamplingDate).ToList();
                    break;
                case "contextMenuGearUnloadLandingSite":
                    ls = parameters.LandingSite == null ? parameters.LandingSiteText : parameters.LandingSite.LandingSiteName;
                    list = GearUnloadCollection
                        .Where(t => t.Parent.NSAPRegionID == parameters.NSAPRegion.Code)
                        .Where(t => t.Parent.FMAID == parameters.FMA.FMAID)
                        .Where(t => t.Parent.FishingGroundID == parameters.FishingGround.Code)
                        .Where(t => t.Parent.LandingSiteName == ls)
                        .OrderBy(t => t.Parent.LandingSiteName)
                        .ThenBy(t => t.Parent.SamplingDate).ToList();
                    break;
                case "contextMenuGearUnloadFishingGround":
                    
                    list = GearUnloadCollection
                        .Where(t => t.Parent.NSAPRegionID == parameters.NSAPRegion.Code)
                        .Where(t => t.Parent.FMAID == parameters.FMA.FMAID)
                        .Where(t => t.Parent.FishingGroundID == parameters.FishingGround.Code)
                        .OrderBy(t => t.Parent.LandingSiteName)
                        .ThenBy(t => t.Parent.SamplingDate).ToList();
                    break;
            }

            if (createCopyOfList)
            {
                var newList = new List<GearUnload>();
                foreach (var item in list)
                {
                    var gu = new GearUnload
                    {
                        PK = item.PK,
                        Boats = item.Boats,
                        Catch = item.Catch
                    };
                    newList.Add(gu);
                }
                CopyOfGearUnloadList = newList;
            }

            return list;
        }
        public List<GearUnload> GetAllGearUnloads(NSAPRegion region, FMA fma, FishingGround fishingGround, bool createCopyOfList = true)
        {
            CopyOfGearUnloadList = new List<GearUnload>();

            var list = GearUnloadCollection
                .Where(t => t.Parent.NSAPRegionID == region.Code)
                .Where(t => t.Parent.FMAID == fma.FMAID)
                .Where(t => t.Parent.FishingGroundID == fishingGround.Code)
                .OrderBy(t => t.Parent.LandingSiteName)
                .ThenBy(t => t.Parent.SamplingDate).ToList();


            if (createCopyOfList)
            {
                var newList = new List<GearUnload>();
                foreach (var item in list)
                {
                    var gu = new GearUnload
                    {
                        PK = item.PK,
                        Boats = item.Boats,
                        Catch = item.Catch
                    };
                    newList.Add(gu);
                }
                CopyOfGearUnloadList = newList;
            }

            return list;
        }

        public List<GearUnload>GetAllGearUnloads(DateTime dateAddedToDatabase, bool createCopyOfList=true)
        {
            CopyOfGearUnloadList = new List<GearUnload>();

            var list = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection
                    .Where(t => t.DateAddedToDatabase.Value.Date == dateAddedToDatabase.Date)
                    .Select(t => t.Parent)
                    .GroupBy(t => t.PK)
                    .Select(t => t.First())
                    .OrderBy(t => t.Parent.NSAPRegion.Name)
                    .ThenBy(t => t.Parent.FMA.Name)
                    .ThenBy(t => t.Parent.FishingGround.Name)
                    .ThenBy(t => t.Parent.LandingSiteName)
                    .ThenBy(t => t.Parent.SamplingDate)
                    .ThenBy(t => t.GearUsedName)
                    .ToList();
            

            if(createCopyOfList)
            {
                var newList = new List<GearUnload>();
                foreach(var item in list)
                {
                    var gu = new GearUnload
                    {
                        PK = item.PK,
                        Boats = item.Boats,
                        Catch = item.Catch
                    };
                    newList.Add(gu);
                }
                CopyOfGearUnloadList = newList;
            }

            return list;

                
        }
        public List<GearUnloadFlattened>GetAllFlattenedItems()
        {
            List<GearUnloadFlattened> thisList = new List<GearUnloadFlattened>();
            foreach (var item in GearUnloadCollection)
            {
                thisList.Add(new GearUnloadFlattened(item));
            }
            return thisList;
        }
        public bool ClearRepository()
        {
            GearUnloadCollection.Clear();
            return GearUnloads.ClearTable();
        }


        public GearUnload getGearUnload(int pk)
        {
            return GearUnloadCollection.FirstOrDefault(n => n.PK == pk);
        }

        public GearUnload getGearUnload(ExcelMainSheet ex)
        {
            return GearUnloadCollection
                .Where(t => t.Parent.LandingSiteName == ex.LandingSiteName)
                .Where(t => t.Parent.FishingGroundID == ex.NSAPRegionFMAFishingGround.FishingGround.Code)
                .Where(t => t.Parent.SamplingDate.Date == ex.SamplingDate.Date)
                .Where(t => t.GearUsedName == ex.GearName).FirstOrDefault();
        }

        public GearUnload getGearUnload(FromJson.VesselLanding landing)
        {
            return GearUnloadCollection
                .Where(t => t.Parent.LandingSiteName == landing.LandingSiteName)
                .Where(t => t.Parent.FishingGroundID == landing.FishingGround.Code)
                .Where(t => t.Parent.SamplingDate.Date == landing.SamplingDate.Date)
                .Where(t => t.GearUsedName == landing.GearName).FirstOrDefault();
        }


        private void GearUnloadCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            EditSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        EditSuccess= GearUnloads.Add(GearUnloadCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<GearUnload> tempListOfRemovedItems = e.OldItems.OfType<GearUnload>().ToList();
                        GearUnloads.Delete(tempListOfRemovedItems[0].PK);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<GearUnload> tempList = e.NewItems.OfType<GearUnload>().ToList();
                        EditSuccess= GearUnloads.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return GearUnloadCollection.Count; }
        }

        public bool AddRecordToRepo(GearUnload item)
        {
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            GearUnloadCollection.Add(item);
            return EditSuccess;
        }

        public bool UpdateRecordInRepo(GearUnload item)
        {
            if (item.PK == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < GearUnloadCollection.Count)
            {
                if (GearUnloadCollection[index].PK == item.PK)
                {
                    GearUnloadCollection[index] = item;
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
                if (GearUnloadCollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    return GearUnloads.MaxRecordNumber() + 1;
                }
            }
        }

        public void DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < GearUnloadCollection.Count)
            {
                if (GearUnloadCollection[index].PK == id)
                {
                    GearUnloadCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
        }
    }
}
