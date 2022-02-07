using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Data;

namespace NSAP_ODK.Entities
{
    public class EffortSpecificationViewModel
    {
        private bool _editSuccess;
        public ObservableCollection<EffortSpecification> EffortSpecCollection { get; set; }
        private EffortSpecificationRepository EffortSpecs { get; set; }

        public EffortSpecificationViewModel()
        {
            EffortSpecs = new EffortSpecificationRepository();
            EffortSpecCollection = new ObservableCollection<EffortSpecification>(EffortSpecs.EffortSpecifications);
            EffortSpecCollection.CollectionChanged += EffortSpecCollection_CollectionChanged;
        }

        public DataSet DataSet()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("Fishing effort specifications");

            DataColumn dc = new DataColumn { ColumnName = "Name", DataType = typeof(string) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Id", DataType = typeof(string) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Expected value", DataType = typeof(string) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Universal specification", DataType = typeof(bool) };
            dt.Columns.Add(dc);



            foreach (var item in EffortSpecCollection.OrderBy(t => t.Name))
            {
                var row = dt.NewRow();
                row["Name"] = item.Name;
                row["Id"] = item.ID;
                row["Expected value"] = item.ValueTypeString;
                row["Universal specification"] = item.IsForAllTypesFishing;
                dt.Rows.Add(row);
            }

            ds.Tables.Add(dt);
            return ds;
        }
        public List<EffortSpecification> GetAllEffortSpecifications()
        {
            return EffortSpecCollection.ToList();
        }

        public Dictionary<Gear, bool> GearsHasEffortSpec(EffortSpecification spec)
        {
            Dictionary<Gear, bool> gearHasEffortSpec = new Dictionary<Gear, bool>();
            foreach(Gear gear in NSAPEntities.GearViewModel.GearCollection
                .OrderBy(t=>t.GearName))
            {
                bool isFound = false;
                foreach(GearEffortSpecification ges in gear.GearEffortSpecificationViewModel.GearEffortSpecificationCollection)
                {
                    if(ges.EffortSpecification.ID==spec.ID)
                    {
                        gearHasEffortSpec.Add(gear, true);
                        isFound = true;
                        break;
                    }
                }
                if(!isFound)
                {
                    if(gear.BaseGear!=null)
                    {
                        foreach(GearEffortSpecification ges in gear.BaseGear.GearEffortSpecificationViewModel.GearEffortSpecificationCollection)
                        {
                            if(ges.EffortSpecification.ID==spec.ID)
                            {
                                gearHasEffortSpec.Add(gear, true);
                                isFound = true;
                                break;
                            }
                        }
                    }
                    if (!isFound)
                    {
                        gearHasEffortSpec.Add(gear, false);
                    }
                }
                else
                {
                    break;
                }

            }
            return gearHasEffortSpec;
        }
        public List<EffortSpecification> GetBaseGearEffortSpecification()
        {
            return EffortSpecCollection.Where(t => t.IsForAllTypesFishing == true).ToList();
        }

        public bool CanDeleteEntity(EffortSpecification es)
        {
            return false;
        }

        public bool EffortSpeficiationNameExist(string name)
        {
            if (name.Length == 0)
                throw new ArgumentNullException("Error: Effort specification cannot be a zero length string");

            foreach (var es in EffortSpecCollection)
            {
                if (es.Name.ToLower() == name.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        public EffortSpecification GetEffortSpecification(int effortID)
        {
            return EffortSpecCollection.FirstOrDefault(n => n.ID == effortID);
        }

        public EffortSpecification GetEffortSpecification(string name)
        {
            return EffortSpecCollection.FirstOrDefault(n => n.Name == name);
        }

        private void EffortSpecCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        _editSuccess= EffortSpecs.Add(EffortSpecCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<EffortSpecification> tempListOfRemovedItems = e.OldItems.OfType<EffortSpecification>().ToList();
                        _editSuccess= EffortSpecs.Delete(tempListOfRemovedItems[0].ID);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<EffortSpecification> tempList = e.NewItems.OfType<EffortSpecification>().ToList();
                        _editSuccess= EffortSpecs.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return EffortSpecCollection.Count; }
        }

        public bool AddRecordToRepo(EffortSpecification e)
        {
            if (e == null)
                throw new ArgumentNullException("Error: The argument is Null");
            EffortSpecCollection.Add(e);
            return _editSuccess;
        }

        public bool UpdateRecordInRepo(EffortSpecification e)
        {
            if (e.ID == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < EffortSpecCollection.Count)
            {
                if (EffortSpecCollection[index].ID == e.ID)
                {
                    EffortSpecCollection[index] = e;
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
                if (EffortSpecCollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    //return EffortSpecCollection.Max(t => t.ID) + 1;
                    return EffortSpecs.MaxRecordNumber() + 1;
                }
            }
        }

        public bool DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < EffortSpecCollection.Count)
            {
                if (EffortSpecCollection[index].ID == id)
                {
                    EffortSpecCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return _editSuccess;
        }

        public EntityValidationResult EntityValidated(EffortSpecification effortSpec, bool isNew, string oldName, bool oldIsForAllTypesFishing)
        {
            EntityValidationResult evr = new EntityValidationResult();
            if (string.IsNullOrEmpty(effortSpec.Name))
            {
                evr.AddMessage(new EntityValidationMessage("Specification cannot be empty"));
            }
            else if (effortSpec.Name != null && effortSpec.Name.Length < 4)
            {
                evr.AddMessage(new EntityValidationMessage("Specification is too short"));
            }
            else if (effortSpec.ValueType == ODKValueType.isUndefined)
            {
                evr.AddMessage(new EntityValidationMessage("Type of value is not defined. The spec will be saved but will not be used in the ODK app.", MessageType.Warning));
            }

            if (isNew && effortSpec.Name != null && EffortSpeficiationNameExist(effortSpec.Name))
            {
                evr.AddMessage(new EntityValidationMessage("Specification already exists"));
            }

           evr.NSAPRegionGearRequireUpdate = effortSpec.IsForAllTypesFishing!= oldIsForAllTypesFishing;
            

            if (!isNew && effortSpec.Name.Length > 0
                 && oldName != effortSpec.Name
                && EffortSpeficiationNameExist(oldName))
                evr.AddMessage(new EntityValidationMessage("Specification already exists"));

            return evr;
        }
    }
}