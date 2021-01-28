using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace NSAP_ODK.Entities
{
    public class NSAPEnumeratorViewModel
    {
        public ObservableCollection<NSAPEnumerator> NSAPEnumeratorCollection { get; set; }
        private NSAPEnumeratorRepository NSAPEnumerators { get; set; }

        public bool EditSuccess { get; internal set; }

        //public event EventHandler<EntityChangedEventArgs> EntityChanged;

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
                        if(NSAPEnumerators.Add(editedEnumerator))
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
                        EditSuccess= NSAPEnumerators.Update(editedEnumerator);      // As the IDs are unique, only one row will be effected hence first index only
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

        public bool  UpdateRecordInRepo(NSAPEnumerator nse)
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

            if(NSAPEnumeratorCollection.Where(t=>t.Name==nse.Name).FirstOrDefault()!=null)
            {
                entityMessages.Add(new EntityValidationMessage("Name already exists"));
            }
            return entityMessages.Count == 0;
        }


    }
}