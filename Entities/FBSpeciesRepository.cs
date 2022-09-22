using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using MySql.Data.MySqlClient;
using NSAP_ODK.NSAPMysql;
using NSAP_ODK.Utilities;
using ClosedXML.Excel;

namespace NSAP_ODK.Entities
{
    public class FBSpeciesRepository
    {


        public List<FBSpecies> FBSpecieses { get; set; }
        public FBSpeciesViewModel _parent;
        private List<FBSpecies> _updateFBSpeciesList;
        public FBSpeciesRepository(FBSpeciesViewModel parent)
        {
            _parent = parent;
            FBSpecieses = getFBSpecies();
        }

        public bool DuplicateErrorWhenAddSpecies { get; set; }

        public bool Add(FBSpecies sp)
        {
            DuplicateErrorWhenAddSpecies = false;
            bool success = false;
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@genus", sp.Genus);
                        cmd.Parameters.AddWithValue("@species", sp.Species);
                        cmd.Parameters.AddWithValue("@sp_id", sp.SpCode);
                        cmd.Parameters.AddWithValue("@family", sp.Family);
                        cmd.Parameters.AddWithValue("@importance", sp.Importance);
                        cmd.Parameters.AddWithValue("@main_method", sp.MainCatchingMethod);


                        if (sp.LengthMax == null)
                        {
                            cmd.Parameters.AddWithValue("@len_max", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@len_max", (double)sp.LengthMax);
                        }

                        if (sp.LengthCommon == null)
                        {
                            cmd.Parameters.AddWithValue("@len_common", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@len_common", (double)sp.LengthCommon);
                        }

                        if (string.IsNullOrEmpty(sp.LengthType))
                        {
                            cmd.Parameters.AddWithValue("@len_type", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@len_type", sp.LengthType);
                        }
                        cmd.CommandText = @"INSERT INTO FBSpecies (
                                                Genus,
                                                Species,
                                                SpecCode,
                                                Family,
                                                Importance,
                                                MainCatchingMethod,
                                                LengthCommon,
                                                LengthMax,
                                                LengthType )
                                            VALUES (
                                                @genus,
                                                @species,
                                                @sp_id,
                                                @family,
                                                @importance,
                                                @main_method,
                                                @len_common,
                                                @len_max,
                                                @len_type )";

                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("The changes you requested to the table were not successful because they would create duplicate values "))
                            {
                                DuplicateErrorWhenAddSpecies = true;
                            }
                            else
                            {
                                Utilities.Logger.Log(ex);
                            }

                        }
                    }
                }
            }
            return success;
        }
        public bool Delete(int sp_id)
        {
            bool success = false;
            return success;
        }

        public bool UpdateFBSpeciesTable(FBSpeciesUpdateMode updateMode)
        {
            bool updateSuccess = false;
            int fbSpeciesCountBeforeUpdate = _parent.FBSpeciesCollection.Count;
            var addNewDuplicationErrorCount = 0;
            var editCount = 0;
            FBSpeciesUpdateType updateType = FBSpeciesUpdateType.UpdateTypeNone;
            if (updateMode == FBSpeciesUpdateMode.UpdateModeUpdateDoNotAdd)
            {
                foreach (FBSpecies fbSpecies in NSAPEntities.FBSpeciesViewModel.FBSpeciesCollection.ToList())
                {
                    var spToUpdate = _updateFBSpeciesList.FirstOrDefault(t => t.SpCode == fbSpecies.SpCode);
                    if (spToUpdate != null)
                    {
                        if (spToUpdate.LengthMax != null)
                        {
                            spToUpdate.LengthMax = Math.Round((double)spToUpdate.LengthMax, 1);
                        }
                        if (spToUpdate.LengthCommon != null)
                        {
                            spToUpdate.LengthCommon = Math.Round((double)spToUpdate.LengthCommon, 1);
                        }
                        if (_parent.UpdateRecordInRepo(spToUpdate))
                        {
                            //updateType = FBSpeciesUpdateType.UpdateTypeUpdatingSpecies;
                            editCount++;
                        }
                    }
                }
                updateSuccess = editCount > 0;
            }
            else
            {
                foreach (FBSpecies updateSpecies in _updateFBSpeciesList)
                {
                    bool newSpeciesFound = false;
                    var spToUpdate = _parent.GetFBSpecies(updateSpecies.SpCode);
                    if (spToUpdate == null)
                    {
                        newSpeciesFound = true;
                        spToUpdate = new FBSpecies
                        {
                            SpCode = updateSpecies.SpCode,
                            Genus = updateSpecies.Genus,
                            Species = updateSpecies.Species,
                        };
                    }

                    spToUpdate.Family = updateSpecies.Family;
                    spToUpdate.Importance = updateSpecies.Importance;
                    spToUpdate.MainCatchingMethod = updateSpecies.MainCatchingMethod;

                    if (updateSpecies.LengthMax != null)
                    {
                        spToUpdate.LengthMax = Math.Round((double)updateSpecies.LengthMax, 1);
                    }

                    if (updateSpecies.LengthCommon != null)
                    {
                        spToUpdate.LengthCommon = Math.Round((double)updateSpecies.LengthCommon, 1);
                    }

                    spToUpdate.LengthCommon = updateSpecies.LengthCommon;
                    spToUpdate.LengthType = updateSpecies.LengthType;

                    if (newSpeciesFound)
                    {
                        if (_parent.AddRecordToRepo(spToUpdate))
                        {
                            editCount++;
                        }
                        else if (_parent.DuplicateErrorInAddNew)
                        {

                            addNewDuplicationErrorCount++;
                        }
                    }
                    else if (!newSpeciesFound && _parent.UpdateRecordInRepo(spToUpdate))
                    {
                        editCount++;
                    }

                }

                updateSuccess = _parent.FBSpeciesCollection.Count > fbSpeciesCountBeforeUpdate;
            }

            return updateSuccess;
        }

        public bool Update(FBSpecies sp)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@genus", sp.Genus);
                        cmd.Parameters.AddWithValue("@species", sp.Species);
                        cmd.Parameters.AddWithValue("@family", sp.Family);
                        cmd.Parameters.AddWithValue("@importance", sp.Importance);
                        cmd.Parameters.AddWithValue("@main_method", sp.MainCatchingMethod);


                        if (sp.LengthMax == null)
                        {
                            cmd.Parameters.AddWithValue("@len_max", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@len_max", (double)sp.LengthMax);
                        }

                        if (sp.LengthCommon == null)
                        {
                            cmd.Parameters.AddWithValue("@len_common", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@len_common", (double)sp.LengthCommon);
                        }

                        if (string.IsNullOrEmpty(sp.LengthType))
                        {
                            cmd.Parameters.AddWithValue("@len_type", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@len_type", sp.LengthType);
                        }
                        cmd.Parameters.AddWithValue("@sp_id", sp.SpCode);

                        cmd.CommandText = @"UPDATE FBSpecies SET
                                                Genus = @genus,
                                                Species = @species,
                                                Family = @family,
                                                Importance = @importance,
                                                MainCatchingMethod = @main_method,
                                                LengthMax = @len_max,
                                                LengthCommon = @len_common,
                                                LengthType = @len_type
                                                WHERE SpecCode = @sp_id";

                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch (Exception ex)
                        {
                            Utilities.Logger.Log(ex);
                        }
                    }
                }
            }
            return success;
        }
        private bool UpdateFBExcelIsValid(List<string> columns)
        {
            int validColumnCount = 0;
            if (columns.Count != 17)
            {
                return false;
            }
            else
            {


                if (columns.Contains("SpecCode"))
                {
                    validColumnCount++;
                }

                if (columns.Contains("Genus"))
                {
                    validColumnCount++;
                }

                if (columns.Contains("Species"))
                {
                    validColumnCount++;
                }

                if (columns.Contains("FamCode"))
                {
                    validColumnCount++;
                }

                if (columns.Contains("Family"))
                {
                    validColumnCount++;
                }

                if (columns.Contains("DemersPelag"))
                {
                    validColumnCount++;
                }

                if (columns.Contains("Length"))
                {
                    validColumnCount++;
                }

                if (columns.Contains("LTypeMaxM"))
                {
                    validColumnCount++;
                }
                if (columns.Contains("LengthFemale"))
                {
                    validColumnCount++;
                }
                if (columns.Contains("LTypeMaxF"))
                {
                    validColumnCount++;
                }
                if (columns.Contains("CommonLength"))
                {
                    validColumnCount++;
                }
                if (columns.Contains("LTypeComM"))
                {
                    validColumnCount++;
                }
                if (columns.Contains("CommonLengthF"))
                {
                    validColumnCount++;
                }
                if (columns.Contains("LTypeComF"))
                {
                    validColumnCount++;
                }
                if (columns.Contains("Importance"))
                {
                    validColumnCount++;
                }
                if (columns.Contains("MainCatchingMethod"))
                {
                    validColumnCount++;
                }
            }
            return validColumnCount == 16;
        }

        public static bool HasUpdateSpeciesList { get; set; }

        public Task<List<FBSpecies>> GetUpdateSpeciesListAsync(string excelUpdateFileName)
        {
            return Task.Run(() => GetUpdateSpeciesList(excelUpdateFileName));
        }
        public List<FBSpecies> GetUpdateSpeciesList(string excelUpdateFileName)
        {
            ErrorMessage = "";
            _updateFBSpeciesList = new List<FBSpecies>();
            if (excelUpdateFileName.Length > 0)
            {
                try
                {

                    using (var xls = new XLWorkbook(excelUpdateFileName))
                    {
                        FBSpecies newFBs = new FBSpecies();
                        var rowCount = xls.Worksheet(1).RangeUsed().RowCount();
                        var columnCount = 17;
                        int column = 1;
                        int row = 1;
                        List<string> columnContents = new List<string>();
                        List<string> rowContent = new List<string>();
                        bool isvalidUpdateFile = true;
                        while (isvalidUpdateFile && row <= rowCount)
                        {


                            column = 1;
                            if (row == 1)
                            {
                                columnContents.Clear();
                                while (column <= columnCount)
                                {
                                    string title = xls.Worksheets.Worksheet(1).Cell(row, column).GetString();
                                    columnContents.Add(title);
                                    column++;
                                }
                                isvalidUpdateFile = UpdateFBExcelIsValid(columnContents);
                            }
                            else
                            {
                                bool is_newFBs = false;
                                newFBs = new FBSpecies();
                                FBSpecies existingFBs;
                                while (column <= columnCount)
                                {
                                    var col = columnContents[column - 1];
                                    var cellValue = xls.Worksheets.Worksheet(1).Cell(row, column).GetString();
                                    switch (columnContents[column - 1])
                                    {
                                        case "SpecCode":
                                            newFBs.SpCode = int.Parse(cellValue);
                                            break;
                                        case "Genus":
                                            newFBs.Genus = cellValue;
                                            break;
                                        case "Species":
                                            newFBs.Species = cellValue;
                                            break;
                                        case "Family":
                                            newFBs.Family = cellValue;
                                            break;
                                        case "DemersPelag":
                                            newFBs.DemersalPelagic = cellValue;
                                            break;
                                        case "Length":
                                            if (double.TryParse(cellValue, out double v))
                                            {
                                                newFBs.LengthMax = v;
                                            }
                                            break;
                                        case "LTypeMaxM":
                                            newFBs.LengthType = cellValue;
                                            break;
                                        case "CommonLength":
                                            if (double.TryParse(cellValue, out double x))
                                            {
                                                newFBs.LengthCommon = x;
                                            }
                                            break;
                                        case "Importance":
                                            newFBs.Importance = cellValue;
                                            break;
                                        case "MainCatchingMethod":
                                            newFBs.MainCatchingMethod = cellValue;
                                            break;
                                    }
                                    column++;
                                }
                            }
                            if (isvalidUpdateFile && row > 1)
                            {
                                _updateFBSpeciesList.Add(newFBs);
                            }
                            row++;
                        }

                        if (!isvalidUpdateFile)
                        {
                            ErrorMessage = "The selected excel file is not valid for updating FishBase species list";
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("because it is being used by another process"))
                    {
                        ErrorMessage = $"The excel file '{System.IO.Path.GetFileName(excelUpdateFileName)}' is open cannot be read by NSAP-ODK Database\r\n\r\n" +
                                        "Please close that file and try again";
                    }
                    else
                    {
                        Logger.Log(ex);
                    }
                }
            }
            if (_updateFBSpeciesList.Count > 0)
            {
                HasUpdateSpeciesList = true;

            }

            return _updateFBSpeciesList;
        }


        public static string ErrorMessage { get; set; }
        private static bool UpdateFBTableSQl(string colName, string type, int? length = null)
        {
            bool success = false;
            string sql = $"ALTER TABLE FbSpecies ADD COLUMN {colName} {type}";
            if (length != null)
            {
                sql += $" ({length})";
            }
            using (var con = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = sql;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        success = true;
                    }
                    catch (OleDbException dbex)
                    {
                        Logger.Log(dbex);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }

            }
            return success;
        }
        public static bool UpdateFBTableFields()
        {
            var cols = Database.CreateTablesInAccess.GetColumnNames("FBSpecies");
            int updateCount = 0;

            if (cols.Contains("Family"))
            {
                updateCount++;
            }
            else if (UpdateFBTableSQl("Family", "VARCHAR"))
            {
                updateCount++;

            }


            if (cols.Contains("Importance"))
            {
                updateCount++;
            }
            else if (UpdateFBTableSQl("Importance", "VARCHAR"))
            {
                updateCount++;
            }


            if (cols.Contains("MainCatchingMethod"))
            {
                updateCount++;
            }
            else if (UpdateFBTableSQl("MainCatchingMethod", "VARCHAR"))
            {
                updateCount++;
            }

            if (cols.Contains("LengthCommon"))
            {
                updateCount++;
            }
            else if (UpdateFBTableSQl("LengthCommon", "DOUBLE"))
            {
                updateCount++;
            }

            if (cols.Contains("LengthMax"))
            {
                updateCount++;
            }
            else if (UpdateFBTableSQl("LengthMax", "DOUBLE"))
            {
                updateCount++;
            }

            if (cols.Contains("LengthMaxLengthType"))
            {
                updateCount++;
            }
            else if (UpdateFBTableSQl("LengthMaxLengthType", "CHAR", 2))
            {
                updateCount++;
            }

            if (cols.Contains("LengthType"))
                updateCount++;
            else if (UpdateFBTableSQl("LengthType", "CHAR", 2))
            {
                updateCount++;
            }

            return updateCount == 7;
        }
        private List<FBSpecies> getFBSpecies()
        {
            List<FBSpecies> thisList = new List<FBSpecies>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "Select * from FBSpecies ORDER BY Genus, Species";
                        con.Open();
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            FBSpecies fb = new FBSpecies
                            {
                                Genus = dr["Genus"].ToString(),
                                Species = dr["Species"].ToString(),
                                SpCode = (int)dr["SpecCode"],
                                Family = dr["Family"].ToString(),
                                Importance = dr["Importance"].ToString(),
                                MainCatchingMethod = dr["MainCatchingMethod"].ToString(),
                                LengthType = dr["LengthType"].ToString(),
                            };
                            if (fb.Genus == "Lepidocybium")
                            {

                            }
                            fb.LengthMax = null;
                            fb.LengthCommon = null;
                            if (dr["LengthMax"] != DBNull.Value)
                            {
                                fb.LengthMax = (double)dr["LengthMax"];
                            }
                            if (dr["LengthCommon"] != DBNull.Value)
                            {
                                fb.LengthCommon = (double)dr["LengthCommon"];
                            }
                            thisList.Add(fb);
                        }
                    }
                }
            }
            return thisList;
        }
    }

}

