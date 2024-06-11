using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using NSAP_ODK.Utilities;
using MySql.Data.MySqlClient;
using NSAP_ODK.NSAPMysql;
using NSAP_ODK.TreeViewModelControl;
namespace NSAP_ODK.Entities.Database
{
    class VesselUnload_FishingGearRepository
    {
        public List<VesselUnload_FishingGear> VesselUnload_FishingGears { get; set; }
        private VesselUnload _parent;

        public static bool AddFieldToTable(string fieldName)
        {
            bool success = false;
            string sql = "";
            switch (fieldName)
            {
                case "catch_weight":
                case "sample_weight":
                    sql = $"ALTER TABLE dbo_vesselunload_fishinggear ADD Column {fieldName} double";
                    break;
                case "species_comp_count":
                    sql = $"ALTER TABLE dbo_vesselunload_fishinggear ADD Column {fieldName} int";
                    break;
            }
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    try
                    {
                        cmd.ExecuteNonQuery();
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return success;
        }
        public static bool CheckTableExists()
        {
            bool tableExists = false;
            using (var conn = new OleDbConnection(Utilities.Global.ConnectionString))
            {
                conn.Open();
                var schema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                if (schema.Rows
                    .OfType<DataRow>()
                    .Any(r => r.ItemArray[2].ToString().ToLower() == "dbo_vesselunload_fishinggear"))
                {
                    tableExists = true;
                }

                if (!tableExists)
                {
                    return CreateTable();
                }
                else
                {
                    return true;
                }
            }
        }

        public static List<VesselUnload_FishingGear> GetFishingGears(AllSamplingEntitiesEventHandler entities)
        {
            List<VesselUnload_FishingGear> gears = new List<VesselUnload_FishingGear>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@reg", entities.NSAPRegion.Code);
                        cmd.Parameters.AddWithValue("@ls", entities.LandingSite.LandingSiteID);
                        cmd.Parameters.AddWithValue("@fma", entities.FMA.FMAID);
                        cmd.Parameters.AddWithValue("@fg", entities.FishingGround.Code);
                        DateTime sMonth = (DateTime)entities.MonthSampled;
                        cmd.Parameters.AddWithValue("@start", sMonth.ToString("MMM/dd/yyyy"));
                        cmd.Parameters.AddWithValue("@end", sMonth.AddMonths(1).ToString("MMM/dd/yyyy"));
                        cmd.CommandText = @"SELECT 
                                                dbo_vessel_unload.v_unload_id,
                                                dbo_gear_unload.gr_id AS gear_code,
                                                dbo_vessel_unload.catch_total AS catch_weight,
                                                dbo_vessel_unload.catch_samp AS sample_weight,
                                                dbo_vessel_unload_1.number_species_catch_composition,
                                                's' AS unload_type
                                            FROM 
                                                dbo_LC_FG_sample_day INNER JOIN 
                                                ((dbo_gear_unload INNER JOIN 
                                                dbo_vessel_unload ON 
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                dbo_vessel_unload_1 ON 
                                                dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id) ON 
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id
                                            WHERE 
                                                dbo_vessel_unload_1.is_multigear=False AND
                                                dbo_LC_FG_sample_day.region_id=@reg AND
                                                dbo_LC_FG_sample_day.land_ctr_id = @ls AND
                                                dbo_LC_FG_sample_day.fma = @fma AND
                                                dbo_LC_FG_sample_day.ground_id = @fg AND
                                                dbo_LC_FG_sample_day.sdate >=@start AND
                                                dbo_LC_FG_sample_day.sdate <@end
                                                
                                                
                                            ORDER BY 
                                                dbo_vessel_unload.v_unload_id,
                                                dbo_gear_unload.gr_id
                                            
                                            UNION ALL
                                            
                                            SELECT 
                                                dbo_vessel_unload.v_unload_id,
                                                dbo_vesselunload_fishinggear.gear_code,
                                                dbo_vesselunload_fishinggear.catch_weight,
                                                dbo_vesselunload_fishinggear.sample_weight,
                                                dbo_vesselunload_fishinggear.species_comp_count,
                                                'm' AS unload_type
                                            FROM
                                                (dbo_LC_FG_sample_day INNER JOIN
                                                (dbo_gear_unload INNER JOIN 
                                                dbo_vessel_unload ON 
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON 
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) LEFT JOIN 
                                                dbo_vesselunload_fishinggear ON 
                                                dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id
                                            WHERE
                                                dbo_LC_FG_sample_day.region_id = @reg AND
                                                dbo_LC_FG_sample_day.land_ctr_id = @ls AND
                                                dbo_LC_FG_sample_day.fma = @fma AND
                                                dbo_LC_FG_sample_day.ground_id = @fg AND
                                                dbo_LC_FG_sample_day.sdate >=@start AND
                                                dbo_LC_FG_sample_day.sdate <@end";
                                          
                        con.Open();
                        try
                        {
                            OleDbDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                VesselUnload_FishingGear vufg = new VesselUnload_FishingGear
                                {
                                    ParentID = (int)dr["v_unload_id"],
                                    GearCode = dr["gear_code"].ToString(),
                                    WeightOfCatch = (double)dr["catch_weight"]
                                };
                                if (dr["sample_weight"] != DBNull.Value && (double)dr["sample_weight"] > 0)
                                {
                                    vufg.WeightOfSample = (double)dr["sample_weight"];
                                }
                                if (dr["number_species_catch_composition"] != DBNull.Value)
                                {
                                    vufg.CountItemsInCatchComposition = (int)dr["number_species_catch_composition"];
                                }

                                gears.Add(vufg);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return gears;
        }

        public static bool ClearTable(string otherConnectionString = "")
        {
            bool success = false;
            string con_string = Global.ConnectionString;
            if (otherConnectionString.Length > 0)
            {
                con_string = otherConnectionString;
            }

            using (OleDbConnection conn = new OleDbConnection(con_string))
            {
                conn.Open();
                var sql = $"Delete * from dbo_vesselunload_fishinggear";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    try
                    {
                        update.ExecuteNonQuery();
                        success = true;
                    }
                    catch (OleDbException)
                    {
                        success = false;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                        success = false;
                    }
                }
            }
            return success;
        }
        public int MaxRecordNumber()
        {
            int max_rec_no = 0;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        //conn.Open();
                        //cmd.CommandText = "SELECT Max(effort_row_id) AS max_id FROM dbo_vessel_effort";
                        //max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(row_id) AS max_id FROM dbo_vesselunload_fishinggear";
                    using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                    {
                        var r = getMax.ExecuteScalar();
                        if (r != DBNull.Value)
                        {
                            max_rec_no = (int)r;
                        }
                    }
                }
            }
            return max_rec_no;
        }
        public static bool CreateTable()
        {
            bool success = false;
            using (var conn = new OleDbConnection(Utilities.Global.ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"CREATE TABLE dbo_vesselunload_fishinggear (
                                                row_id Int NOT NULL PRIMARY KEY,
                                                vessel_unload_id Int,
                                                gear_code VarChar,
                                                gear_text VarChar,
                                                CONSTRAINT FK_vufg_gear
                                                    FOREIGN KEY(gear_code) REFERENCES gear(GearCode),
                                                CONSTRAINT FK_vufg_vu   
                                                    FOREIGN KEY (vessel_unload_id) REFERENCES dbo_vessel_unload(v_unload_id)
                                                )";
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        success = true;
                    }
                    catch (OleDbException odx)
                    {
                        Logger.Log(odx);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }

            return success;
        }

        public bool Add(VesselUnload_FishingGear item)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                //success = AddToMySQL(item);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    var sql = @"Insert into dbo_vesselunload_fishinggear(
                                    row_id, 
                                    vessel_unload_id,
                                    gear_code,
                                    gear_text,
                                    catch_weight,
                                    sample_weight,
                                    species_comp_count
                                    ) 
                                Values (?,?,?,?,?,?,?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        try
                        {
                            update.Parameters.Add("@id", OleDbType.Integer).Value = item.RowID;
                            update.Parameters.Add("@unload_id", OleDbType.Integer).Value = item.Parent.PK;
                            if (item.Gear == null)
                            {
                                update.Parameters.Add("@gear_code", OleDbType.VarChar).Value = DBNull.Value;
                            }
                            else
                            {
                                update.Parameters.Add("@gear_code", OleDbType.VarChar).Value = item.Gear.Code;
                            }
                            if (string.IsNullOrEmpty(item.GearText))
                            {
                                update.Parameters.Add("@gear_text", OleDbType.VarChar).Value = DBNull.Value;
                            }
                            else
                            {
                                update.Parameters.Add("@gear_text", OleDbType.VarChar).Value = item.GearText;
                            }

                            if (item.WeightOfCatch != null)
                            {
                                update.Parameters.Add("@wt_catch", OleDbType.Double).Value = item.WeightOfCatch;
                            }
                            else
                            {
                                update.Parameters.Add("@wt_catch", OleDbType.Double).Value = DBNull.Value;
                            }

                            if (item.WeightOfSample != null)
                            {
                                update.Parameters.Add("@wt_sample", OleDbType.Double).Value = item.WeightOfSample;
                            }
                            else
                            {
                                update.Parameters.Add("@wt_sample", OleDbType.Double).Value = DBNull.Value;
                            }
                            if (item.CountItemsInCatchComposition == null)
                            {
                                update.Parameters.Add("@count_species", OleDbType.Integer).Value = DBNull.Value;
                            }
                            else
                            {
                                update.Parameters.Add("@count_species", OleDbType.Integer).Value = item.CountItemsInCatchComposition;
                            }

                            try
                            {
                                success = update.ExecuteNonQuery() > 0;
                            }
                            catch (OleDbException dbex)
                            {
                                switch (dbex.ErrorCode)
                                {
                                    case -2147467259:
                                        //error due to duplicated key or index
                                        break;
                                    default:
                                        Logger.Log(dbex);
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Log(ex);
                            }
                        }
                        catch (OleDbException odbex)
                        {
                            Logger.Log(odbex);
                            success = false;
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return success;
        }

        //internal static bool AddFieldToTable(string fieldName)
        //{
        //    bool success = false;
        //    string sql = "";
        //    switch (fieldName)
        //    {
        //        case "gear_sequence":
        //            sql = $"ALTER TABLE dbo_vesselunload_fishinggear ADD COLUMN {fieldName} int";
        //            break;
        //        case "count_landings":
        //            sql = $"ALTER TABLE dbo_vesselunload_fishinggear ADD COLUMN {fieldName} int";
        //            break;
        //        case "catch_of_gear":
        //            sql = $"ALTER TABLE dbo_vesselunload_fishinggear ADD COLUMN {fieldName} double";
        //            break;
        //    }
        //    using (var con = new OleDbConnection(Global.ConnectionString))
        //    {
        //        using (var cmd = con.CreateCommand())
        //        {
        //            con.Open();
        //            cmd.CommandText = sql;
        //            try
        //            {
        //                cmd.ExecuteNonQuery();
        //                success = true;
        //            }
        //            catch (Exception ex)
        //            {
        //                Logger.Log(ex);
        //            }
        //        }
        //    }
        //    return success;
        //}

        public bool Update(VesselUnload_FishingGear item)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                //success = UpdateMySQL(item);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    using (OleDbCommand update = conn.CreateCommand())
                    {

                        update.Parameters.Add("@unload_id", OleDbType.Integer).Value = item.Parent.PK;
                        if (item.Gear == null)
                        {
                            update.Parameters.Add("@gear_code", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@gear_code", OleDbType.VarChar).Value = item.Gear.Code;
                        }

                        if (string.IsNullOrEmpty(item.GearText))
                        {
                            update.Parameters.Add("@gear_text", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@gear_text", OleDbType.VarChar).Value = item.GearText;
                        }

                        if (item.WeightOfCatch != null)
                        {
                            update.Parameters.Add("@wt_catch", OleDbType.Double).Value = item.WeightOfCatch;
                        }
                        else
                        {
                            update.Parameters.Add("@wt_catch", OleDbType.Double).Value = DBNull.Value;
                        }

                        if (item.WeightOfSample != null)
                        {
                            update.Parameters.Add("@wt_sample", OleDbType.Double).Value = item.WeightOfSample;
                        }
                        else
                        {
                            update.Parameters.Add("@wt_sample", OleDbType.Double).Value = DBNull.Value;
                        }
                        if (item.CountItemsInCatchComposition == null)
                        {
                            update.Parameters.Add("@count_species", OleDbType.Integer).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@count_species", OleDbType.Integer).Value = item.CountItemsInCatchComposition;
                        }

                        update.Parameters.Add("@id", OleDbType.Integer).Value = item.RowID;

                        update.CommandText = @"Update dbo_vesselunload_fishinggear set
                                            vessel_unload_id=@unload_id,
                                            gear_code = @gear_code,
                                            gear_text = @gear_text,
                                            catch_weight = @wt_catch,
                                            sample_weight = @wt_sample,
                                            species_comp_count = @count_species
                                        WHERE row_id = @id";

                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
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
            }
            return success;
        }

        public bool Delete(int id)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                //success = DeleteMySQL(id);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    using (OleDbCommand update = conn.CreateCommand())
                    {
                        update.Parameters.Add("@id", OleDbType.Integer).Value = id;
                        update.CommandText = "Delete * from dbo_vesselunload_fishinggear where row_id=@id";
                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
                            int result = 0;
                            if (success)
                            {
                                success = false;

                                using (OleDbCommand upd1 = conn.CreateCommand())
                                {
                                    upd1.Parameters.Add("@unload_gear_id", OleDbType.Integer).Value = id;
                                    upd1.CommandText = "Delete * from dbo_vessel_unload_stats where unload_gear=@id";

                                    try
                                    {
                                        result = upd1.ExecuteNonQuery();
                                        success = true;
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Log(ex);
                                    }
                                }
                            }
                            if (success)
                            {
                                result = 0;
                                using (OleDbCommand upd2 = conn.CreateCommand())
                                {
                                    upd2.Parameters.Add("@id", OleDbType.Integer).Value = id;
                                    upd2.CommandText = "Delete * from dbo_vessel_unload_weight_validation where unload_gear=@id";
                                    try
                                    {
                                        result = upd2.ExecuteNonQuery();
                                        success = true;
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Log(ex);
                                    }
                                }
                            }
                        }
                        catch (OleDbException)
                        {
                            success = false;
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                            success = false;
                        }
                    }
                }
            }
            return success;
        }

        public VesselUnload_FishingGearRepository(bool isNew = false)
        {
            if (!isNew)
            {
                VesselUnload_FishingGears = getGears();
            }
        }
        public VesselUnload_FishingGearRepository(VesselUnload vu)
        {
            if (vu != null)
            {
                _parent = vu;
                VesselUnload_FishingGears = getGears();
            }
        }
        private List<VesselUnload_FishingGear> getGears()
        {
            List<VesselUnload_FishingGear> thisList = new List<VesselUnload_FishingGear>();
            if (Global.Settings.UsemySQL)
            {
                //thisList = getFromMySQL(vu);
            }
            else
            {
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = conection.CreateCommand())
                    {
                        try
                        {
                            conection.Open();
                            //cmd.CommandText = "Select * from dbo_vessel_catch";


                            cmd.Parameters.AddWithValue("@parentID", _parent.PK);
                            cmd.CommandText = "Select * from dbo_vesselunload_fishinggear where vessel_unload_id=@parentID";

                            thisList.Clear();
                            OleDbDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                double? catch_wt = null;
                                int? catch_sp_count = null;
                                double? sample_wt = null;
                                if (dr["catch_weight"] != DBNull.Value)
                                {
                                    catch_wt = (double)dr["catch_weight"];
                                }
                                if (dr["sample_weight"] != DBNull.Value)
                                {
                                    sample_wt = (double)dr["sample_weight"];
                                }

                                if (dr["species_comp_count"] != DBNull.Value)
                                {
                                    catch_sp_count = (int)dr["species_comp_count"];
                                }

                                VesselUnload_FishingGear vufg = new VesselUnload_FishingGear
                                {
                                    RowID = (int)dr["row_id"],
                                    Parent = _parent,
                                    GearText = dr["gear_text"].ToString(),
                                    GearCode = dr["gear_code"].ToString(),
                                    WeightOfCatch = catch_wt,
                                    WeightOfSample = sample_wt,
                                    CountItemsInCatchComposition = catch_sp_count
                                };
                                vufg.VesselUnload_Gear_Specs_ViewModel = new VesselUnload_Gear_Spec_ViewModel(vufg);
                                vufg.VesselCatchViewModel = new VesselCatchViewModel(vufg);
                                thisList.Add(vufg);
                            }

                        }
                        catch (OleDbException ole_ex)
                        {
                            //
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }


                    }
                }
            }
            return thisList;
        }
    }
}
