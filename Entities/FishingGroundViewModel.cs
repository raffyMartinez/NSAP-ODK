using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
namespace NSAP_ODK.Entities
{
    public class FishingGroundViewModel
    {
        private bool _editSuccess;
        public ObservableCollection<FishingGround> FishingGroundCollection { get; set; }
        private FishingGroundRepository FishingGrounds { get; set; }

        public DataSet DataSet()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("Fishing grounds");

            DataColumn dc = new DataColumn { ColumnName = "Name", DataType = typeof(string) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Code", DataType = typeof(string) };
            dt.Columns.Add(dc);

            foreach (var item in FishingGroundCollection.OrderBy(t=>t.Name))
            {
                var row = dt.NewRow();
                row["Name"] = item.Name;
                row["Code"] = item.Code;
                dt.Rows.Add(row);
            }

            ds.Tables.Add(dt);
            return ds;
        }
        public FishingGroundViewModel()
        {
            FishingGrounds = new FishingGroundRepository();
            FishingGroundCollection = new ObservableCollection<FishingGround>(FishingGrounds.FishingGrounds);
            FishingGroundCollection.CollectionChanged += FishingGroundCollection_CollectionChanged;
        }
        public List<FishingGround> GetAllFishingGrounds()
        {
            return FishingGroundCollection.ToList();
        }
        public bool FishingGroundNameExist(string name)
        {
            foreach (FishingGround fg in FishingGroundCollection)
            {
                if (fg.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public FishingGround CurrentEntity { get; set; }
        public bool FishingGroundCodeExists(string code)
        {
            foreach (FishingGround fg in FishingGroundCollection)
            {
                if (fg.Code == code)
                {
                    return true;
                }
            }
            return false;
        }
        public FishingGround GetFishingGround(string code)
        {
            CurrentEntity = FishingGroundCollection.FirstOrDefault(n => n.Code == code);
            return CurrentEntity;

        }
        private void FishingGroundCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        FishingGround newFG = FishingGroundCollection[newIndex];
                        if (FishingGrounds.Add(newFG))
                        {
                            CurrentEntity = newFG;
                            _editSuccess = true;
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        List<FishingGround> tempListOfRemovedItems = e.OldItems.OfType<FishingGround>().ToList();
                        _editSuccess= FishingGrounds.Delete(tempListOfRemovedItems[0].Code);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        List<FishingGround> tempList = e.NewItems.OfType<FishingGround>().ToList();
                        _editSuccess= FishingGrounds.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return FishingGroundCollection.Count; }
        }

        public bool AddRecordToRepo(FishingGround fg)
        {
            if (fg == null)
                throw new ArgumentNullException("Error: The argument is Null");
            FishingGroundCollection.Add(fg);
            return _editSuccess;
        }

        public bool UpdateRecordInRepo(FishingGround fg)
        {
            if (fg.Code == null)
                throw new Exception("Error: ID cannot be null");

            int index = 0;
            while (index < FishingGroundCollection.Count)
            {
                if (FishingGroundCollection[index].Code == fg.Code)
                {
                    FishingGroundCollection[index] = fg;
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
            while (index < FishingGroundCollection.Count)
            {
                if (FishingGroundCollection[index].Code == code)
                {
                    FishingGroundCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return _editSuccess; 
        }

        public bool EntityValidated(FishingGround fg, out List<EntityValidationMessage> entityMessages, bool isNew, string oldName = "", string oldCode = "")
        {
            entityMessages = new List<EntityValidationMessage>();
            if (fg.Code == null || fg.Code.Length > 5)
            {
                entityMessages.Add(new EntityValidationMessage("Fishing ground code must made up of 1 to 5 characters"));
            }

            if (FishingGroundCodeExists(fg.Code) && isNew)
            {
                entityMessages.Add(new EntityValidationMessage("Fishing ground code is already in use"));
            }

            if (FishingGroundNameExist(fg.Name) && isNew)
            {
                entityMessages.Add(new EntityValidationMessage("Fishing ground name is already in use"));
            }


            if (!isNew
                 && oldName != fg.Name
                && FishingGroundNameExist(fg.Name))
                entityMessages.Add(new EntityValidationMessage("Fishing ground name is already in use"));

            if (!isNew
                 && oldCode != fg.Code
                && FishingGroundCodeExists(fg.Code))
                entityMessages.Add(new EntityValidationMessage("Gear code already used"));

            return entityMessages.Count == 0;
        }
    }
}
