﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Data;

namespace NSAP_ODK.Entities
{
    public class GearViewModel
    {
        private bool _editSuccess;
        public ObservableCollection<Gear> GearCollection { get; set; }
        private GearRepository Gears { get; set; }
        public void Serialize(string fileName)
        {
            List<GearFlattened> gears = new List<GearFlattened>();
            foreach (var item in GearCollection)
            {
                GearFlattened g = new GearFlattened
                {
                    GearName = item.GearName,
                    Code = item.Code,
                    GenericCode = item.BaseGear.Code,
                    IsGeneric = item.IsGenericGear
                };
                gears.Add(g);
            }
            SerializeGear serialize = new SerializeGear { Gears = gears };
            serialize.Save(fileName);
        }

        public DataSet DataSet()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("Fishing gears");

            DataColumn dc = new DataColumn { ColumnName = "Name", DataType = typeof(string) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Code", DataType = typeof(string) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Base gear", DataType = typeof(string) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Is generic gear", DataType = typeof(bool) };
            dt.Columns.Add(dc);


            foreach (var item in GearCollection.OrderBy(t => t.GearName))
            {
                var row = dt.NewRow();
                row["Name"] = item.GearName;
                row["Code"] = item.Code;
                row["Base gear"] = item.BaseGear.ToString();
                row["Is generic gear"] = item.IsGenericGear;
                dt.Rows.Add(row);
            }

            ds.Tables.Add(dt);
            return ds;
        }
        public Dictionary<string, GearEffortSpecification> GearEffortSpecifications { get; set; }

        public void DeleteEffortSpec(EffortSpecification effortSpec)
        {
            List<string> keysForDeletion = new List<string>();
            foreach (var gearEffort in GearEffortSpecifications.Values)
            {
                if (gearEffort.EffortSpecificationID == effortSpec.ID)
                {
                    keysForDeletion.Add($"{gearEffort.Gear.Code}-{gearEffort.EffortSpecification.ID}");
                }
            }
            foreach (var gear in GearCollection)
            {
                foreach (var gearEffort in gear.GearEffortSpecificationViewModel.GearEffortSpecificationCollection)
                {
                    if (gearEffort.EffortSpecification.ID == effortSpec.ID)
                    {
                        keysForDeletion.Add($"{gear.Code}-{effortSpec.ID}");
                        if (gearEffort.EffortSpecification.IsForAllTypesFishing)
                        {
                            gear.GearEffortSpecificationViewModel.RemoveBaseGearEffortSpecification(gearEffort);
                            if (gear.CodeOfBaseGear.Length > 0)
                            {
                                gear.BaseGear.GearEffortSpecificationViewModel.RemoveBaseGearEffortSpecification(gearEffort);
                            }
                        }
                        else
                        {
                            gear.GearEffortSpecificationViewModel.DeleteRecordFromRepo(gearEffort);
                        }
                        break;
                    }
                }
            }

            foreach (var key in keysForDeletion)
            {
                GearEffortSpecifications.Remove(key);
            }
        }

        public void FillGearEffortSpecifications(bool includeNumberFishers = false)
        {
            List<GearEffortSpecification> gear_specs = EffortSpecificationRepository.GetGearEffortSpecifications();

            List<Gear> gears = GearCollection.ToList();
            foreach (Gear gear in gears)
            {
                if (gear.GearEffortSpecificationViewModel == null)
                {
                    gear.GearEffortSpecificationViewModel = new GearEffortSpecificationViewModel(gear, readDatabase: false);
                    foreach (GearEffortSpecification g in gear_specs)
                    {
                        g.Gear = gears.Find(t => t.Code == g.GearCode);
                        gear.GearEffortSpecificationViewModel.AddRecordToRepo(g);
                    }
                    if (gear.Code == gear.BaseGear.Code && gear.BaseGear.IsGenericGear)
                    {
                        AddBaseGearSpecsToGear(gear);
                    }
                }
                foreach (var gearEffort in gear.GearEffortSpecificationViewModel.GearEffortSpecificationCollection)
                {
                    string key = $"{gear.Code}-{gearEffort.EffortSpecificationID}";
                    if (!GearEffortSpecifications.Keys.Contains(key))
                        GearEffortSpecifications.Add(key, gearEffort);
                }
                if (gear.BaseGear != null)
                {
                    if (gear.BaseGear.GearEffortSpecificationViewModel == null)
                    {
                        gear.BaseGear.GearEffortSpecificationViewModel = new GearEffortSpecificationViewModel(gear.BaseGear, readDatabase: false);
                    }
                    foreach (var gearEffort in gear.BaseGear.GearEffortSpecificationViewModel.GearEffortSpecificationCollection)
                    {
                        string key = $"{gear.Code}-{gearEffort.EffortSpecificationID}";
                        if (!GearEffortSpecifications.Keys.Contains(key))
                        {
                            GearEffortSpecification ges = new GearEffortSpecification { EffortSpecification = gearEffort.EffortSpecification, Gear = gear, RowID = gearEffort.EffortSpecification.ID };
                            GearEffortSpecifications.Add(key, ges);
                        }
                    }
                }
            }
        }
        public void FillGearEffortSpecificationsEx(bool includeNumberFishers = false)
        {
            //List<GearEffortSpecification> gear_specs = EffortSpecificationRepository.GetGearEffortSpecifications();
            foreach (Gear gear in GearCollection)
            {
                if (gear.GearEffortSpecificationViewModel == null)
                {
                    gear.GearEffortSpecificationViewModel = new GearEffortSpecificationViewModel(gear);
                    if (gear.Code == gear.BaseGear.Code && gear.BaseGear.IsGenericGear)
                    {
                        AddBaseGearSpecsToGear(gear);
                    }
                }
                foreach (var gearEffort in gear.GearEffortSpecificationViewModel.GearEffortSpecificationCollection)
                {
                    string key = $"{gear.Code}-{gearEffort.EffortSpecificationID}";
                    if (!GearEffortSpecifications.Keys.Contains(key))
                        GearEffortSpecifications.Add(key, gearEffort);
                }
                if (gear.BaseGear != null)
                {
                    if (gear.BaseGear.GearEffortSpecificationViewModel == null)
                    {
                        gear.BaseGear.GearEffortSpecificationViewModel = new GearEffortSpecificationViewModel(gear.BaseGear);
                    }
                    foreach (var gearEffort in gear.BaseGear.GearEffortSpecificationViewModel.GearEffortSpecificationCollection)
                    {
                        string key = $"{gear.Code}-{gearEffort.EffortSpecificationID}";
                        if (!GearEffortSpecifications.Keys.Contains(key))
                        {
                            GearEffortSpecification ges = new GearEffortSpecification { EffortSpecification = gearEffort.EffortSpecification, Gear = gear, RowID = gearEffort.EffortSpecification.ID };
                            GearEffortSpecifications.Add(key, ges);
                        }
                    }
                }
            }
            //foreach (var line in gear.Gear.GearEffortSpecificationViewModel.GearEffortSpecificationCollection)
            //{
            //    string key = $"{gear.Gear.Code}-{line.EffortSpecification.ID}";
            //    if (!NSAPEntities.GearViewModel.GearEffortSpecifications.Keys.Contains(key))
            //        NSAPEntities.GearViewModel.GearEffortSpecifications.Add(key, line);

            //}
        }

        public void AddBaseGearSpecsToGear(Gear g)
        {
            foreach (var specAllFishingType in NSAPEntities.EffortSpecificationViewModel.GetBaseGearEffortSpecification())
            {
                GearEffortSpecification ges = new GearEffortSpecification();
                ges.Gear = g;
                ges.EffortSpecification = specAllFishingType;
                g.GearEffortSpecificationViewModel.AddBaseGearEffortSpecification(ges,addToCollectionOnly:true);
            }
        }

        public GearViewModel()
        {
            Gears = new GearRepository();
            GearCollection = new ObservableCollection<Gear>(Gears.Gears);

            //List<EffortSpecification> baseGearEffortSpecs = NSAPEntities.EffortSpecificationViewModel.GetBaseGearEffortSpecification();
            List<EffortSpecification> efSpecs = NSAPEntities.EffortSpecificationViewModel.GetAllEffortSpecifications();
            List<GearEffortSpecification> specs = EffortSpecificationRepository.GetGearEffortSpecifications();
            List<Gear> gears = GearCollection.ToList();
            foreach (Gear g in gears)
            {
                g.GearEffortSpecificationViewModel = new GearEffortSpecificationViewModel(g, readDatabase: false);
                foreach (GearEffortSpecification spec in specs)
                {
                    spec.Gear = gears.Find(t => t.Code == spec.GearCode);
                    if (spec.Gear.Equals(g))
                    {
                        spec.EffortSpecification = efSpecs.Find(t => t.ID == spec.EffortSpecificationID);
                        spec.Gear.GearEffortSpecificationViewModel.AddRecordToRepo(spec, addToCollectionOnly: true);
                    }
                }
                //g.GetEffortSpecificationsForGear();

                if (g.CodeOfBaseGear.Length > 0)
                {
                    g.BaseGear = GetGear(g.CodeOfBaseGear);
                    if (g.Code == g.BaseGear.Code && g.BaseGear.IsGenericGear)
                    {
                        AddBaseGearSpecsToGear(g);
                        //foreach (var specAllFishingType in baseGearEffortSpecs)
                        //{
                        //    GearEffortSpecification ges = new GearEffortSpecification();
                        //    ges.Gear = g;
                        //    ges.EffortSpecification = specAllFishingType;
                        //    g.GearEffortSpecificationViewModel.AddBaseGearEffortSpecification(ges);
                        //}
                    }
                }
                else
                {
                    foreach (var specAllFishingType in NSAPEntities.EffortSpecificationViewModel.GetBaseGearEffortSpecification())
                    {
                        GearEffortSpecification ges = new GearEffortSpecification();
                        ges.Gear = g;
                        ges.EffortSpecification = specAllFishingType;
                        g.GearEffortSpecificationViewModel.AddBaseGearEffortSpecification(ges,addToCollectionOnly:true);
                    }
                }
            }
            GearEffortSpecifications = new Dictionary<string, GearEffortSpecification>();
            GearCollection.CollectionChanged += Gearss_CollectionChanged;
            FillGearEffortSpecifications();
        }
        //public GearViewModel()
        //{
        //    Gears = new GearRepository();
        //    GearCollection = new ObservableCollection<Gear>(Gears.Gears);

        //    //List<EffortSpecification> baseGearEffortSpecs = NSAPEntities.EffortSpecificationViewModel.GetBaseGearEffortSpecification();


        //    foreach (Gear g in GearCollection)
        //    {
        //        g.GetEffortSpecificationsForGear();

        //        if (g.CodeOfBaseGear.Length > 0)
        //        {
        //            g.BaseGear = GetGear(g.CodeOfBaseGear);
        //            if (g.Code == g.BaseGear.Code && g.BaseGear.IsGenericGear)
        //            {
        //                AddBaseGearSpecsToGear(g);
        //                //foreach (var specAllFishingType in baseGearEffortSpecs)
        //                //{
        //                //    GearEffortSpecification ges = new GearEffortSpecification();
        //                //    ges.Gear = g;
        //                //    ges.EffortSpecification = specAllFishingType;
        //                //    g.GearEffortSpecificationViewModel.AddBaseGearEffortSpecification(ges);
        //                //}
        //            }
        //        }
        //        else
        //        {
        //            foreach (var specAllFishingType in NSAPEntities.EffortSpecificationViewModel.GetBaseGearEffortSpecification())
        //            {
        //                GearEffortSpecification ges = new GearEffortSpecification();
        //                ges.Gear = g;
        //                ges.EffortSpecification = specAllFishingType;
        //                g.GearEffortSpecificationViewModel.AddBaseGearEffortSpecification(ges);
        //            }
        //        }
        //    }
        //    GearEffortSpecifications = new Dictionary<string, GearEffortSpecification>();
        //    GearCollection.CollectionChanged += Gearss_CollectionChanged;
        //    FillGearEffortSpecifications();
        //}

        public void AddUniversalSpec(EffortSpecification es)
        {
            foreach (Gear gear in GearCollection)
            {
                if (gear.Code == gear.BaseGear.Code && gear.BaseGear.IsGenericGear)
                {
                    GearEffortSpecification ges = new GearEffortSpecification();
                    ges.Gear = gear;
                    ges.EffortSpecification = es;
                    gear.GearEffortSpecificationViewModel.AddBaseGearEffortSpecification(ges);
                }
            }
        }
        public void UpdateAllFishingTypeSpecs(bool Update, EffortSpecification effortSpec, bool isNew)
        {

        }
        public List<Gear> GetAllGears()
        {
            return GearCollection.ToList();
        }

        public bool GearNameExist(string gearName, string code = "")
        {
            foreach (Gear g in GearCollection)
            {
                if (code.Length > 0)
                {
                    if (g.GearName == gearName && g.Code != code)
                    {
                        return true;
                    }

                    if (g.GearName == gearName)
                    {
                        break;
                    }
                }
                else
                {
                    if (g.GearName == gearName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool GearCodeExist(string gearCode)
        {
            foreach (Gear g in GearCollection)
            {
                if (g.Code == gearCode)
                {
                    return true;
                }
            }
            return false;
        }

        public Gear GetGear(string code)
        {
            CurrentEntity = GearCollection.FirstOrDefault(n => n.Code == code);
            //if(code==CurrentEntity.BaseGear.Code && CurrentEntity)
            //{

            //}
            //if (CurrentEntity.GearEffortSpecificationViewModel == null || CurrentEntity.GearEffortSpecificationViewModel.Count <CurrentEntity.BaseGear.GearEffortSpecificationViewModel.Count)
            //{
            //    if (CurrentEntity.GearEffortSpecificationViewModel == null)
            //    {
            //        CurrentEntity.GearEffortSpecificationViewModel = new GearEffortSpecificationViewModel(CurrentEntity);
            //    }
            //    if (CurrentEntity.BaseGear != null && CurrentEntity.GearEffortSpecificationViewModel.Count < CurrentEntity.BaseGear.GearEffortSpecificationViewModel.Count)
            //    {
            //        CurrentEntity.GearEffortSpecificationViewModel = CurrentEntity.BaseGear.GearEffortSpecificationViewModel;
            //    }
            //}
            return CurrentEntity;
        }

        public Gear CurrentEntity { get; private set; }

        private void Gearss_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        Gear newGear = GearCollection[newIndex];
                        if (Gears.Add(newGear))
                        {
                            CurrentEntity = newGear;
                            _editSuccess = true;
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<Gear> tempListOfRemovedItems = e.OldItems.OfType<Gear>().ToList();
                        _editSuccess = Gears.Delete(tempListOfRemovedItems[0].Code);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<Gear> tempList = e.NewItems.OfType<Gear>().ToList();
                        _editSuccess = Gears.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return GearCollection.Count; }
        }

        public bool AddRecordToRepo(Gear gear)
        {
            if (gear == null)
                throw new ArgumentNullException("Error: The argument is Null");

            GearCollection.Add(gear);

            return _editSuccess;
        }

        public bool UpdateRecordInRepo(Gear gear)
        {
            if (gear.Code == null)
                throw new Exception("Error: ID cannot be null");

            int index = 0;
            while (index < GearCollection.Count)
            {
                if (GearCollection[index].Code == gear.Code)
                {
                    GearCollection[index] = gear;
                    break;
                }
                index++;
            }

            return _editSuccess;
        }

        public bool DeleteRecordFromRepo(string code)
        {
            if (code == null)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < GearCollection.Count)
            {
                if (GearCollection[index].Code == code)
                {
                    GearCollection.RemoveAt(index);
                    break;
                }
                index++;
            }

            return _editSuccess;
        }

        public EntityValidationResult ValidateEntity(Gear gear, bool isNew, string oldName = "", string oldCode = "")
        {
            var result = new EntityValidationResult();

            if (gear.GearName == null || gear.GearName.Length < 3)
                result.AddMessage("Fishing gear's name must be at least 3 characters long");

            if (gear.Code == null || gear.Code.Length == 0)
            {
                result.AddMessage("Gear code cannot be empty");
            }
            else if (gear.Code.Length > 5)
            {
                result.AddMessage("Gear code cannot be more than 5 letters");
            }

            if (isNew && gear.GearName != null && gear.GearName.Length > 0 && GearNameExist(gear.GearName))
                result.AddMessage("Gear name already used");

            if (isNew && gear.Code != null && gear.Code.Length > 0 && GearCodeExist(gear.Code))
                result.AddMessage("Gear code already used");

            if (!isNew && gear.GearName.Length > 0
                 && oldName != gear.GearName
                && GearNameExist(gear.GearName, gear.Code))
                result.AddMessage("Gear name already used");

            if (!isNew && gear.Code.Length > 0
                 && oldCode != gear.Code
                && GearCodeExist(gear.Code))
                result.AddMessage("Gear code already used");

            if (!isNew && gear.BaseGear == null)
            {
                result.AddMessage("Base gear cannot be empty");
            }
            else if (isNew && !gear.IsGenericGear && gear.BaseGear == null)
            {
                result.AddMessage("Base gear cannot be empty\r\nIf gear is not generic then you must select a base gear");
            }


            return result;
        }

    }
}