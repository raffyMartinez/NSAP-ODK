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

namespace NSAP_ODK.Entities.Database
{
    internal class CarrierLandingRepository
    {
        public List<CarrierLanding> CarrierLandings { get; set; }

        public CarrierLandingRepository(LandingSiteSampling lss)
        {
            CarrierLandings = getCarrierLandings(lss);
        }

        public static bool ClearTable()
        {
            bool success = false;

            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = "Delete * from dbo_carrier_landing";

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
        public static async Task<bool>AddFieldToTable(string colName)
        {
            bool success = false;
            string sql = "";
            if(Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        switch (colName)
                        {
                            case "sample_weight":
                                sql = $"ALTER TABLE dbo_carrier_landing ADD COLUMN {colName} DOUBLE";
                                break;
                        }

                        con.Open();
                        cmd.CommandText = sql;
                        try
                        {
                            await cmd.ExecuteNonQueryAsync();
                            success = true;
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
        private List<CarrierLanding> getCarrierLandings(LandingSiteSampling lss)
        {
            List<CarrierLanding> this_list = new List<CarrierLanding>();
            if(Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@lss_id", lss.PK);
                        cmd.CommandText = "Select * from dbo_carrier_landing where landingsitesampling_id=@lss_id";
                        try
                        {
                            con.Open();
                            OleDbDataReader dr = cmd.ExecuteReader();
                            while(dr.Read())
                            {
                                
                                CarrierLanding cl = new CarrierLanding
                                {
                                    CarrierName = dr["carrier_name"].ToString(),
                                    Parent = lss,
                                    RowID = (int)dr["row_id"],
                                    SamplingDate = (DateTime)dr["sampling_date"],
                                    CountCatchers=null,
                                    WeightOfCatch=null,
                                    CountSpeciesComposition = (int)dr["count_species_catch_composition"],
                                    RefNo = dr["ref_no"].ToString()
                                };

                                if (dr["count_catchers"]!=DBNull.Value)
                                {
                                    cl.CountCatchers = (int)dr["count_catchers"];
                                }
                                if (dr["weight_catch"]!=DBNull.Value)
                                {
                                    cl.WeightOfCatch = (double)dr["weight_catch"];
                                }

                                this_list.Add(cl);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return this_list;
        }
        public bool Add(CarrierLanding cl)
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
                        cmd.Parameters.AddWithValue("@id", cl.RowID);
                        cmd.Parameters.AddWithValue("@ref_no", cl.RefNo);
                        cmd.Parameters.AddWithValue("@name", cl.CarrierName);
                        cmd.Parameters.AddWithValue("@lss_id", cl.Parent.PK);
                        cmd.Parameters.Add("@sampling_date", OleDbType.Date).Value = cl.SamplingDate;
                        if (cl.CatcherBoatOperation_ViewModel?.Count == 0)
                        {
                            cmd.Parameters.AddWithValue("@ct_catchers", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ct_catchers", cl.CatcherBoatOperation_ViewModel.Count);
                        }
                        if (cl.VesselCatchViewModel?.Count == 0)
                        {
                            cmd.Parameters.AddWithValue("@ct_species", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ct_species", cl.VesselCatchViewModel.Count);
                        }
                        if (cl.WeightOfCatch == null)
                        {
                            cmd.Parameters.AddWithValue("@catch_wt", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@catch_wt", cl.WeightOfCatch);
                        }

                        cmd.CommandText = @"INSERT INTO dbo_carrier_landing (
                                            row_id,
                                            ref_no,
                                            carrier_name,
                                            landingsitesampling_id,
                                            sampling_date,
                                            count_catchers,
                                            count_species_catch_composition,
                                            weight_catch) 
                                            VALUES (?,?,?,?,?,?,?,?)";

                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                            success = true;
                        }
                        catch(Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return success;
        }

        public bool Update(CarrierLanding cl)
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
                        cmd.Parameters.AddWithValue("@ref_no", cl.RefNo);
                        cmd.Parameters.AddWithValue("@name", cl.CarrierName);
                        cmd.Parameters.AddWithValue("@lss_id", cl.Parent.PK);
                        cmd.Parameters.Add("@sampling_date", OleDbType.Date).Value = cl.SamplingDate;
                        if (cl.CatcherBoatOperation_ViewModel?.Count == 0)
                        {
                            cmd.Parameters.AddWithValue("@ct_catchers", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ct_catchers", cl.CatcherBoatOperation_ViewModel.Count);
                        }
                        if (cl.VesselCatchViewModel?.Count == 0)
                        {
                            cmd.Parameters.AddWithValue("@ct_species", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ct_species", cl.VesselCatchViewModel.Count);
                        }
                        if (cl.WeightOfCatch == null)
                        {
                            cmd.Parameters.AddWithValue("@catch_wt", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@catch_wt", cl.WeightOfCatch);
                        }
                        cmd.Parameters.AddWithValue("@id", cl.RowID);

                        cmd.CommandText = @"UPDATE dbo_carrier_landing SET
                                            ref_no = @ref_no,
                                            carrier_name = @name,
                                            landingsitesampling_id = @lss_id,
                                            sampling_time = @sampling_time,
                                            count_catchers = @ct_catchers,
                                            count_species_catch_composition = @ct_species,
                                            weight_catch = @catch_wt
                                            WHERE row_id = @id";

                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                            success = true;
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

        public bool Delete(int row_id)
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
                        cmd.Parameters.AddWithValue("@id", row_id);

                        cmd.CommandText = "DELETE * FROM dbo_carrier_landing WHERE row_id = @id";

                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                            success = true;
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
        public static bool CheckTableExist()
        {
            bool tableExists = false;
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var schema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                if (schema.Rows
                    .OfType<DataRow>()
                    .Any(r => r.ItemArray[2].ToString().ToLower() == "dbo_carrier_landing"))
                {
                    tableExists = true;
                }

                if (!tableExists)
                {
                    tableExists = CreateTable();
                }

                return tableExists;
            }
        }

        public static bool CreateTable()
        {
            bool success = false;
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"CREATE TABLE dbo_carrier_landing (
                                                row_id INTEGER, 
                                                carrier_name VARCHAR,
                                                landingsitesampling_id INTEGER,
                                                sampling_time DATETIME,
                                                count_catchers INTEGER,
                                                count_species_catch_composition INTEGER,
                                                weight_catch DOUBLE,
                                                ref_no VARCHAR,    
                                                CONSTRAINT PrimaryKey PRIMARY KEY (row_id),
                                                CONSTRAINT FK_cl_landingsitesampling
                                                    FOREIGN KEY (landingsitesampling_id) REFERENCES
                                                    dbo_LC_FG_sample_day (unload_day_id)
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
    }
}
