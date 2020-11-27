using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace NSAP_ODK.Entities
{
    public class GearEffortSpecificationViewModel
    {
        private bool _isBaseSpec = false;
        public ObservableCollection<GearEffortSpecification> GearEffortSpecificationCollection { get; set; }
        private GearEffortSpecificationRepository GearEffortSpecifications { get; set; }

        public GearEffortSpecificationViewModel(Gear gear)
        {
            GearEffortSpecifications = new GearEffortSpecificationRepository(gear);
            GearEffortSpecificationCollection = new ObservableCollection<GearEffortSpecification>(GearEffortSpecifications.GearEffortSpecifications);
            GearEffortSpecificationCollection.CollectionChanged += GearEffortSpecificationCollection_CollectionChanged;
        }

        public List<GearEffortSpecification> GetAllGearEffortSpecifications()
        {
            return GearEffortSpecificationCollection.ToList();
        }

        public GearEffortSpecification GetGearEffortSpecification(int rowID)
        {
            return GearEffortSpecificationCollection.FirstOrDefault(n => n.RowID == rowID);
        }

        public string GetEffortSpecChoices()
        {
            string effortSpecs = "";
            foreach (var item in GearEffortSpecificationCollection)
            {
                effortSpecs = $"\"effort_spec\", \"{item.RowID}\", \"{item.EffortSpecification.Name}\", \"{item.Gear.Code}\"\r\n";
            }
            return effortSpecs;
        }

        private void GearEffortSpecificationCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (!_isBaseSpec)
                        {
                            int newIndex = e.NewStartingIndex;
                            GearEffortSpecifications.Add(GearEffortSpecificationCollection[newIndex]);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        if (!_isBaseSpec)
                        {
                            List<GearEffortSpecification> tempListOfRemovedItems = e.OldItems.OfType<GearEffortSpecification>().ToList();
                            GearEffortSpecifications.Delete(tempListOfRemovedItems[0].RowID);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<GearEffortSpecification> tempList = e.NewItems.OfType<GearEffortSpecification>().ToList();
                        GearEffortSpecifications.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int NextRecordNumber
        {
            get
            {
                return GearEffortSpecificationRepository.AllGearEffortSpecificationMaxRecordNumber();
            }
        }

        public void RemoveBaseGearEffortSpecification(GearEffortSpecification ges)
        {
            if (ges.EffortSpecification.IsForAllTypesFishing)
            {
                _isBaseSpec = true;
                GearEffortSpecificationCollection.Remove(ges);
            }
        }
        public void AddBaseGearEffortSpecification(GearEffortSpecification ges)
        {
            if (ges.EffortSpecification.IsForAllTypesFishing)
            {
                _isBaseSpec = true;
                GearEffortSpecificationCollection.Add(ges);
            }
        }

        public void AddRecordToRepo(GearEffortSpecification ges)
        {
            if (ges == null)
                throw new ArgumentNullException("Error: The argument is Null");

            _isBaseSpec = false;
            GearEffortSpecificationCollection.Add(ges);
        }

        public void UpdateRecordInRepo(GearEffortSpecification ges)
        {
            if (ges.RowID == 0)
                throw new Exception("Error: ID cannot be null");

            int index = 0;
            while (index < GearEffortSpecificationCollection.Count)
            {
                if (GearEffortSpecificationCollection[index].RowID == ges.RowID)
                {
                    GearEffortSpecificationCollection[index] = ges;
                    break;
                }
                index++;
            }
        }

        public void DeleteRecordFromRepo(GearEffortSpecification gearEffortSpec)
        {

            if (GearEffortSpecificationCollection.Contains(gearEffortSpec))
            {
                _isBaseSpec = false;
                GearEffortSpecificationCollection.Remove(gearEffortSpec);
            }
        }
        public void DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < GearEffortSpecificationCollection.Count)
            {
                if (GearEffortSpecificationCollection[index].RowID == id)
                {
                    GearEffortSpecificationCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
        }

        public int Count
        {
            get { return GearEffortSpecificationCollection.Count; }
        }

        public EntityValidationResult ValidateGearEfforSpecifier(GearEffortSpecification gearEffortSpec, bool isNew)
        {
            EntityValidationResult evr = new EntityValidationResult();
            if (GearEffortSpecificationCollection.Count > 0)
            {
                foreach (var g in GearEffortSpecificationCollection)
                {
                    if (g.EffortSpecificationID == gearEffortSpec.EffortSpecificationID)
                    {
                        evr.AddMessage($"Effort specifier already added to {gearEffortSpec.Gear.GearName}");
                        break;
                    }
                }
            }
            return evr;
        }
    }
}