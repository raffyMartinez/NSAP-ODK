using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace NSAP_ODK.Entities
{
    public class GPSViewModel
    {
        public ObservableCollection<GPS> GPSCollection { get; set; }
        private GPSRepository GPSes { get; set; }



        public GPSViewModel()
        {
            GPSes = new GPSRepository();
            GPSCollection = new ObservableCollection<GPS>(GPSes.GPSes);
            GPSCollection.CollectionChanged += GPSCollection_CollectionChanged;
        }
        public List<GPS> GetGPS()
        {
            return GPSCollection.ToList();
        }
        public bool GPSAssignedNameExist(string name)
        {
            foreach (GPS gps in GPSCollection)
            {
                if (gps.AssignedName == name)
                {
                    return true;
                }
            }
            return false;
        }

        public GPS CurrentEntity { get; set; }
        public bool GPSCodeExist(string code)
        {
            foreach (GPS gps in GPSCollection)
            {
                if (gps.Code == code)
                {
                    return true;
                }
            }
            return false;
        }
        public GPS GetGPS(string code)
        {
            CurrentEntity = GPSCollection.FirstOrDefault(n => n.Code == code);
            return CurrentEntity;

        }
        private void GPSCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        GPS newGPS = GPSCollection[newIndex];
                        if (GPSes.Add(newGPS))
                        {
                            CurrentEntity = newGPS;
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        List<GPS> tempListOfRemovedItems = e.OldItems.OfType<GPS>().ToList();
                        GPSes.Delete(tempListOfRemovedItems[0].Code);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        List<GPS> tempList = e.NewItems.OfType<GPS>().ToList();
                        GPSes.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return GPSCollection.Count; }
        }

        public void AddRecordToRepo(GPS gps)
        {
            if (gps == null)
                throw new ArgumentNullException("Error: The argument is Null");
            GPSCollection.Add(gps);
        }

        public void UpdateRecordInRepo(GPS gps)
        {
            if (gps.Code == null)
                throw new Exception("Error: ID cannot be null");

            int index = 0;
            while (index < GPSCollection.Count)
            {
                if (GPSCollection[index].Code == gps.Code)
                {
                    GPSCollection[index] = gps;
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
            while (index < GPSCollection.Count)
            {
                if (GPSCollection[index].Code == code)
                {
                    GPSCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
        }
        public EntityValidationResult ValidateGPS(GPS gps, bool isNew, string oldAssignedName, string oldCode)
        {
            EntityValidationResult evr = new EntityValidationResult();

            if (isNew && gps.Code.Length != 3)
            {
                evr.AddMessage("Code must be 3 letters long");
            }

            if (isNew && gps.AssignedName.Length < 5)
            {
                evr.AddMessage("Assigned name must be at least 5 letters long");
            }

            if (gps.Model.Length == 0)
            {
                evr.AddMessage("Model must not be empty");
            }

            if (gps.Brand.Length == 0)
            {
                evr.AddMessage("Brand must not be empty");
            }

            if (!isNew && gps.AssignedName.Length > 0
                && oldAssignedName != gps.AssignedName
                && GPSAssignedNameExist(gps.AssignedName))
                evr.AddMessage("Gear name already used");

            if (!isNew && gps.Code.Length > 0
                && oldCode != gps.Code
                && GPSCodeExist(gps.Code))
                evr.AddMessage("Gear code already used");

            return evr;
        }
    }
}
