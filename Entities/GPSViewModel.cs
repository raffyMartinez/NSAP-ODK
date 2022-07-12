using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using NSAP_ODK.Entities.Database;
namespace NSAP_ODK.Entities
{
    public class GPSViewModel
    {
        private bool _editSuccess;
        private bool _isGenericGPS;
        private string _importGPSCSV;
        private List<string> _csvHeaders = new List<string>();
        private int _importGPSCSVImportedCount;

        public ObservableCollection<GPS> GPSCollection { get; set; }
        private GPSRepository GPSes { get; set; }

        public List<KeyValuePair<int, string>> DeviceTypeSources()
        {
            var list = new List<KeyValuePair<int, string>>();
            list.Add(new KeyValuePair<int, string>(0, "None"));
            list.Add(new KeyValuePair<int, string>(1, "GPS"));
            list.Add(new KeyValuePair<int, string>(2, "Phone"));
            list.Add(new KeyValuePair<int, string>(9, "Other"));
            return list;
        }
        public DataSet DataSet()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("GPS");

            DataColumn dc = new DataColumn { ColumnName = "Name", DataType = typeof(string) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Code", DataType = typeof(string) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Brand", DataType = typeof(string) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Model", DataType = typeof(string) };
            dt.Columns.Add(dc);

            foreach (var item in GPSCollection.OrderBy(t => t.AssignedName))
            {
                var row = dt.NewRow();
                row["Name"] = item.AssignedName;
                row["Code"] = item.Code;
                row["Brand"] = item.Brand;
                row["Model"] = item.Model;
                dt.Rows.Add(row);
            }

            ds.Tables.Add(dt);
            return ds;
        }

        public int ImportGPSCSVImportedCount
        {
            get { return _importGPSCSVImportedCount; }
        }
        public string GPSImportErrorMessage { get; private set; }
        public string ImportGPSCSV
        {

            get { return _importGPSCSV; }
            set
            {
                int validationErrorCount = 0;
                string validationErrorMessage = "";
                _importGPSCSVImportedCount = 0;
                _csvHeaders.Clear();
                bool validHeader = false;

                _importGPSCSV = value;
                int loopCount = 0;
                int rowCount = 0;
                foreach (var item in _importGPSCSV.Split('\n').ToList())
                {
                    if (loopCount == 0)
                    {
                        foreach (var header in item.Split(',').ToList())
                        {
                            _csvHeaders.Add(header.Replace("_", "").ToLower());
                        }
                        validHeader = ISValidCSVHeaders();
                    }
                    else if (validHeader)
                    {
                        if (item.Length > 0)
                        {
                            var arrGPS = item.Split(',');
                            try
                            {
                                GPS gps = new GPS
                                {
                                    Code = arrGPS[0].Trim('\"'),
                                    AssignedName = arrGPS[1].Trim('\"'),
                                    Brand = arrGPS[2],
                                    Model = arrGPS[3],

                                };

                                if (int.TryParse(arrGPS[4], out int v))
                                {
                                    gps.DeviceType = (DeviceType)v;
                                    var result = NSAPEntities.GPSViewModel.ValidateGPS(gps, true, "", "");
                                    if (result.ErrorMessage.Length == 0)
                                    {
                                        if (NSAPEntities.GPSViewModel.AddRecordToRepo(gps))
                                        {
                                            _importGPSCSVImportedCount++;
                                        }
                                    }
                                    else
                                    {
                                        validationErrorCount++;
                                        //validationErrorMessage += $"{result.ErrorMessage}\n";
                                    }
                                    rowCount++;
                                }


                            }
                            catch (Exception ex)
                            {
                                Utilities.Logger.Log(ex);
                            }
                        }

                    }
                    else
                    {
                        GPSImportErrorMessage = "CSV headers does not match with the database";
                        GPSCSVImportSuccess = false;
                        break;
                    }
                    loopCount++;
                }

                var diff = rowCount - _importGPSCSVImportedCount;
                if (diff != 0)
                {
                    string item_items = "items were";
                    if (diff == 1)
                    {
                        item_items = "item was";
                    }
                    GPSImportErrorMessage = $"Not all GPS items in the CSV was imported\r\n\r\n{diff} GPS {item_items} not imported";
                    if (validationErrorCount > 0)
                    {
                        GPSImportErrorMessage += $"\r\n{validationErrorMessage.Trim('\n')}";
                    }
                }
                GPSCSVImportSuccess = _importGPSCSVImportedCount > 0;
            }

        }

        public bool GPSCSVImportSuccess { get; private set; }

        private bool ISValidCSVHeaders()
        {
            int headerCount = 0;
            List<string> headerList = null;
            if (Utilities.Global.Settings.UsemySQL)
            {

            }
            else
            {
                headerList = CreateTablesInAccess.GetColumnNames(tableName: "gps", makeLowerCase: true);
                foreach (var item in headerList)
                {
                    if (_csvHeaders.Contains(item))
                    {
                        headerCount++;
                    }
                }
            }
            return headerCount == headerList.Count;
        }
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
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        GPS newGPS = GPSCollection[newIndex];
                        if (!_isGenericGPS && GPSes.Add(newGPS))
                        {
                            _editSuccess = true;
                            CurrentEntity = newGPS;
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        List<GPS> tempListOfRemovedItems = e.OldItems.OfType<GPS>().ToList();
                        _editSuccess = GPSes.Delete(tempListOfRemovedItems[0].Code);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        List<GPS> tempList = e.NewItems.OfType<GPS>().ToList();
                        _editSuccess = GPSes.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return GPSCollection.Count; }
        }

        public bool AddGenericGPS(GPS gps)
        {
            _isGenericGPS = true;
            GPSCollection.Add(gps);
            CurrentEntity = gps;
            return true;
        }

        public bool Exists(string gpsCode)
        {
            return GPSCollection.FirstOrDefault(t => t.Code == gpsCode) != null;
        }
        public bool AddRecordToRepo(GPS gps)
        {
            if (gps == null)
                throw new ArgumentNullException("Error: The argument is Null");

            _isGenericGPS = false;
            GPSCollection.Add(gps);

            return _editSuccess;
        }

        public bool UpdateRecordInRepo(GPS gps)
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
            return _editSuccess;
        }

        public bool DeleteRecordFromRepo(string code)
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
            return _editSuccess;
        }
        public EntityValidationResult ValidateGPS(GPS gps, bool isNew, string oldAssignedName, string oldCode)
        {
            EntityValidationResult evr = new EntityValidationResult();

            if (isNew && (gps.Code.Length < 3 || gps.Code.Length > 6))
            {
                evr.AddMessage("Code must be at least 3 to 6 letters long");
            }

            if (gps.Code.ToLower() == "gps")
            {
                evr.AddMessage($"The code \"{gps.Code}\" is reserverd and cannot be used.");
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

            if (gps.AssignedName.Length > 0 && gps.Code.Length > 0)
            {
                if (isNew)
                {
                    if (GPSAssignedNameExist(gps.AssignedName))
                    {
                        evr.AddMessage("GPS name already used");
                    }

                    if (GPSCodeExist(gps.Code))
                    {
                        evr.AddMessage("GPS code already used");
                    }
                }
                else
                {

                    if (oldAssignedName != gps.AssignedName &&
                        GPSAssignedNameExist(gps.AssignedName))
                    {
                        evr.AddMessage("GPS name already used");
                    }

                    if (oldCode != gps.Code &&
                        GPSCodeExist(gps.Code))
                    {
                        evr.AddMessage("GPS code already used");
                    }
                }
            }

            return evr;
        }
    }
}
