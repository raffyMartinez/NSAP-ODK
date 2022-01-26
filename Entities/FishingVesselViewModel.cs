using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using NSAP_ODK.Entities.Database;

namespace NSAP_ODK.Entities
{
    public class FishingVesselViewModel
    {
        private bool _editSuccess;
        public ObservableCollection<FishingVessel> FishingVesselCollection { get; set; }
        private FishingVesselRepository FishingVessels { get; set; }

        public FishingVesselViewModel()
        {
            FishingVessels = new FishingVesselRepository();
            FishingVesselCollection = new ObservableCollection<FishingVessel>(FishingVessels.FishingVessels);
            FishingVesselCollection.CollectionChanged += FishingVesselCollection_CollectionChanged;
        }

        public List<OrphanedFishingVessel> OrphanedFishingVesseks()
        {
            var itemsVesselSamplings = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection
                .Where(t => t.VesselID == null && t.VesselName!= null && t.VesselText.Length > 0)
                .GroupBy(t => new { Sector = t.Sector, LandingSite = t.Parent.Parent.LandingSiteName, Name = t.VesselName  })
                .Select(vessel => new
                    {
                        NameVessel = vessel.Key.Name,
                        SectorVessel = vessel.Key.Sector,
                        LandingSiteVessel = vessel.Key.LandingSite
                    }
                )
                .ToList();

            var list = new List<OrphanedFishingVessel>();
            //var listNames = new List<string>();


            foreach (var item in itemsVesselSamplings)
            {
                //listNames.Add(item.NameVessel);
                var orphan = new OrphanedFishingVessel
                {
                    Name = item.NameVessel,
                    VesselUnloads = NSAPEntities.VesselUnloadViewModel.GetSampledLandingsOfVessel(item.NameVessel,item.SectorVessel,item.LandingSiteVessel),
                };

                list.Add(orphan);
            }

            List<OrphanedFishingVessel> sortedList = list.OrderBy(t=>t.Sector).ThenBy(t =>  t.Name).ThenBy(t=>t.LandingSiteName).ToList() ;
            return sortedList;

        }
        public List<FishingVessel> GetAllGears()
        {
            return FishingVesselCollection.ToList();
        }

        public bool VesselNameExist(string vesselName)
        {
            foreach (FishingVessel fv in FishingVesselCollection)
            {
                if (fv.Name == vesselName)
                {
                    return true;
                }
            }
            return false;
        }

        public FishingVessel CurrentEntity { get; set; }
        public FishingVessel GetFishingVessel(int id)
        {
            CurrentEntity = FishingVesselCollection.FirstOrDefault(n => n.ID == id);
            return CurrentEntity;
        }

        private void FishingVesselCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        FishingVessel newVessel = FishingVesselCollection[newIndex];
                        if(FishingVessels.Add(newVessel))
                        {
                            CurrentEntity = newVessel;
                            _editSuccess = true;
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<FishingVessel> tempListOfRemovedItems = e.OldItems.OfType<FishingVessel>().ToList();
                       _editSuccess= FishingVessels.Delete(tempListOfRemovedItems[0].ID);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<FishingVessel> tempList = e.NewItems.OfType<FishingVessel>().ToList();
                       _editSuccess= FishingVessels.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return FishingVesselCollection.Count; }
        }

        public bool AddRecordToRepo(FishingVessel fv)
        {
            if (fv == null)
                throw new ArgumentNullException("Error: The argument is Null");
            FishingVesselCollection.Add(fv);
            return _editSuccess;
        }

        public bool UpdateRecordInRepo(FishingVessel fv)
        {
            if (fv.ID == 0)
                throw new Exception("Error: ID cannot be null");

            int index = 0;
            while (index < FishingVesselCollection.Count)
            {
                if (FishingVesselCollection[index].ID == fv.ID)
                {
                    FishingVesselCollection[index] = fv;
                    break;
                }
                index++;
            }
            return _editSuccess;
        }

        public bool DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < FishingVesselCollection.Count)
            {
                if (FishingVesselCollection[index].ID == id)
                {
                    FishingVesselCollection.RemoveAt(index);
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
                if (FishingVesselCollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    //return FishingVesselCollection.Max(t => t.ID) + 1;
                    return FishingVessels.MaxRecordNumber() + 1;
                }
            }
        }

        public bool EntityValidated(FishingVessel fishingVessel, out List<EntityValidationMessage> entityMessages, bool isNew)
        {
            bool dimensionMissing = false;
            bool dimensionNotNumeric = false;
            bool dimensionNotGreaterThanZero = false;
            entityMessages = new List<EntityValidationMessage>();
            if (string.IsNullOrEmpty(fishingVessel.Name) && string.IsNullOrEmpty(fishingVessel.NameOfOwner))
            {
                entityMessages.Add(new EntityValidationMessage("Vessel name and name of owner cannot be both blank./rAt least one should have a value"));
            }

            if (fishingVessel.Length != null || fishingVessel.Depth != null || fishingVessel.Breadth != null)
            {
                if (fishingVessel.Length == null)
                {
                    dimensionMissing = true;
                }
                else
                {
                    if (!double.TryParse(fishingVessel.Length.ToString(), out double len))
                    {
                        dimensionNotNumeric = true;
                    }
                    else if (len <= 0)
                    {
                        dimensionNotGreaterThanZero = true;
                    }
                }

                if (fishingVessel.Depth == null)
                {
                    dimensionMissing = true;
                }
                else
                {
                    if (!double.TryParse(fishingVessel.Depth.ToString(), out double dep))
                    {
                        dimensionNotNumeric = true;
                    }
                    else if (dep <= 0)
                    {
                        dimensionNotGreaterThanZero = true;
                    }
                }

                if (fishingVessel.Breadth == null)
                {
                    dimensionMissing = true;
                }
                else
                {
                    if (!double.TryParse(fishingVessel.Breadth.ToString(), out double bre))
                    {
                        dimensionNotNumeric = true;
                    }
                    else if (bre <= 0)
                    {
                        dimensionNotGreaterThanZero = true;
                    }
                }

                if (dimensionNotGreaterThanZero || dimensionNotNumeric)
                {
                    entityMessages.Add(new EntityValidationMessage("All vessel dimensions must be numeric values greater than zero"));
                }

                if (dimensionMissing)
                {
                    entityMessages.Add(new EntityValidationMessage("All vessel dimension must be provided or leave all blank"));
                }
            }

            return entityMessages.Count == 0;
        }
    }
}