using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace NSAP_ODK.Entities
{
    public class MunicipalityViewModel
    {
        private bool _editSuccess;
        public ObservableCollection<Municipality> MunicipalityCollection { get; set; }
        private MunicipalityRepository Municipalities { get; set; }

        public MunicipalityViewModel(Province province)
        {
            Municipalities = new MunicipalityRepository(province);
            MunicipalityCollection = new ObservableCollection<Municipality>(Municipalities.Municipalities);
            MunicipalityCollection.CollectionChanged += Municipalities_CollectionChanged;
        }

        public List<Municipality> GetAllMunicipalities()
        {
            return MunicipalityCollection.ToList();
        }

        public List<Municipality> GetAllMunicipalities(Province p)
        {
            List<Municipality> tempMunicipalities = new List<Municipality>();
            foreach (Municipality m in MunicipalityCollection)
            {
                if (m.Province.ProvinceID == p.ProvinceID)
                {
                    tempMunicipalities.Add(m);
                }
            }
            return tempMunicipalities;
        }

        public bool CanDeleteEntity(Municipality m)
        {
            return false;
        }

        public bool MunicipalityNameExists(string municipalityName)
        {
            foreach (Municipality m in MunicipalityCollection)
            {
                if (m.MunicipalityName == municipalityName)
                {
                    return true;
                }
            }
            return false;
        }

        public Municipality GetMunicipality(int municipalityID)
        {
            return MunicipalityCollection.FirstOrDefault(n => n.MunicipalityID == municipalityID);
        }

        private void Municipalities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        _editSuccess= Municipalities.Add(MunicipalityCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<Municipality> tempListOfRemovedItems = e.OldItems.OfType<Municipality>().ToList();
                        _editSuccess= Municipalities.Delete(tempListOfRemovedItems[0].MunicipalityID);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<Municipality> tempList = e.NewItems.OfType<Municipality>().ToList();
                        _editSuccess= Municipalities.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return MunicipalityCollection.Count; }
        }

        public bool AddRecordToRepo(Municipality m)
        {
            if (m == null)
                throw new ArgumentNullException("Error: The argument is Null");
            MunicipalityCollection.Add(m);
            return _editSuccess;
        }

        public bool UpdateRecordInRepo(Municipality m)
        {
            if (m.MunicipalityID == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < MunicipalityCollection.Count)
            {
                if (MunicipalityCollection[index].MunicipalityID == m.MunicipalityID)
                {
                    MunicipalityCollection[index] = m;
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
            while (index < MunicipalityCollection.Count)
            {
                if (MunicipalityCollection[index].MunicipalityID == id)
                {
                    MunicipalityCollection.RemoveAt(index);
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
                if (MunicipalityCollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    //return ProvinceCollection.Max(t => t.ProvinceID) + 1;
                    return Municipalities.MaxRecordNumber() + 1;
                }
            }
        }

        public bool EntityValidated(Dictionary<string, string> formValues, out List<string> messages)
        {
            messages = new List<string>();

            string stringLongitude = formValues["txtLongitude"];
            string stringLatitude = formValues["txtLatitude"];

            if (stringLongitude.Length > 0)
            {
                if (double.TryParse(stringLongitude, out double v))
                {
                    //reserved for future use
                }
                else
                {
                    messages.Add("Longitude must be a numeric value");
                }
            }
            else
            {
                messages.Add("Longitude cannot be empty");
            }

            if (stringLatitude.Length > 0)
            {
                if (double.TryParse(stringLatitude, out double v))
                {
                    //reserved for future use
                }
                else
                {
                    messages.Add("Latitude must be a numeric value");
                }
            }
            else
            {
                messages.Add("Latitude cannot be empty");
            }

            return messages.Count == 0;
        }

        public EntityValidationResult ValidateMunicipality(Municipality municipality, bool isNew, string oldName)
        {
            EntityValidationResult evr = new EntityValidationResult();

            if (municipality.MunicipalityName.Length < 2)
            {
                evr.AddMessage("Municipality name is too short");
            }
            else if (isNew && MunicipalityNameExists(municipality.MunicipalityName))
            {
                evr.AddMessage("Municipality name already used");
            }

            if (!isNew && oldName != municipality.MunicipalityName && MunicipalityNameExists(municipality.MunicipalityName))
            {
                evr.AddMessage("Municipality name already used");
            }

            if (municipality.Latitude != null && ((double)municipality.Latitude > 21.1 || (double)municipality.Latitude < 4.5))
            {
                evr.AddMessage("Latitude is out of bounds");
            }

            if (municipality.Longitude != null && ((double)municipality.Longitude < 112 || (double)municipality.Longitude > 126.6))
            {
                evr.AddMessage("Longitude is out of bounds");
            }

            if (municipality.Longitude != null || municipality.Latitude != null)
            {
                if (municipality.Longitude == null || municipality.Latitude == null)
                    evr.AddMessage("Longitude and latitude must both be filled up or should be both empty");
            }
            return evr;
        }
    }
}