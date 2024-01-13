using NPOI.HSSF.Record.Aggregates;
using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace NSAP_ODK.Entities.Database
{
    public class CatcherBoatOperationRepository
    {
        public List<CatcherBoatOperation> CatcherBoatOperations;
        public CatcherBoatOperationRepository()
        {
            CatcherBoatOperations = getCatcherBoatOperations();
        }

        public CatcherBoatOperationRepository(CarrierLanding parent)
        {
            CatcherBoatOperations = getCatcherBoatOperations(parent);
        }

        private List<CatcherBoatOperation> getCatcherBoatOperations(CarrierLanding parent = null)
        {
            List<CatcherBoatOperation> this_list = new List<CatcherBoatOperation>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "Select * from dbo_catcher_boat_operations";
                        if (parent != null)
                        {
                            cmd.Parameters.AddWithValue("@carrier_id", parent.RowID);
                            cmd.CommandText = "SELECT * FROM dbo_catcher_boat_operations WHERE carrierlanding_id=@carrier_id";
                        }
                        try
                        {
                            con.Open();
                            OleDbDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                CatcherBoatOperation item = new CatcherBoatOperation
                                {
                                    RowID = (int)dr["row_id"],
                                    CatcherBoatName = dr["boat_name"].ToString(),
                                    StartOfOperation = (DateTime)dr["date_start_operation"],
                                    GearCode = dr["gear_code"].ToString(),
                                    Parent = parent
                                };

                                if (dr["catch_weight"] != DBNull.Value)
                                {
                                    item.WeightOfCatch = (double)dr["catch_weight"];
                                }

                                this_list.Add(item);
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

        public bool Add(CatcherBoatOperation item)
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
                        cmd.Parameters.AddWithValue("@row_id", item.RowID);
                        cmd.Parameters.AddWithValue("@boat_name", item.CatcherBoatName);
                        cmd.Parameters.Add("@start", OleDbType.Date).Value = item.StartOfOperation;
                        cmd.Parameters.AddWithValue("@gear_code", item.GearCode);
                        cmd.Parameters.AddWithValue("@carrier_id", item.Parent.RowID);

                        if (item.WeightOfCatch == null)
                        {
                            cmd.Parameters.AddWithValue("@wt", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@wt", item.WeightOfCatch);
                        }

                        cmd.CommandText = @"INSERT INTO dbo_catcher_boat_operations 
                                                (row_id,boat_name,date_start_operation,gear_code,carrierlanding_id,catch_weight)
                                                VALUES(?,?,?,?,?,?)";

                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;
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

        public bool Update(CatcherBoatOperation item)
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

                        cmd.Parameters.AddWithValue("@boat_name", item.CatcherBoatName);
                        cmd.Parameters.Add("@start", OleDbType.Date).Value = item.StartOfOperation;
                        cmd.Parameters.AddWithValue("@gear_code", item.GearCode);
                        cmd.Parameters.AddWithValue("@carrier_id", item.Parent.RowID);

                        if (item.WeightOfCatch == null)
                        {
                            cmd.Parameters.AddWithValue("@wt", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@wt", item.WeightOfCatch);
                        }

                        cmd.Parameters.AddWithValue("@row_id", item.RowID);

                        cmd.CommandText = @"UPDATE dbo_catcher_boat_operations 
                                            SET boat_name=@boat_name,
                                                date_start_operation=@start,
                                                gear_code=@gear_code,
                                                carrierlanding_id=@carrier_id,
                                                catch_weight=@wt
                                            WHERE row_id=@row_id";


                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;
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

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@row_id", id);
                        cmd.CommandText = @"DELETE * from dbo_catcher_boat_operations WHERE row_id=@row_id";

                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;
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
                    .Any(r => r.ItemArray[2].ToString().ToLower() == "dbo_catcher_boat_operations"))
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
                    cmd.CommandText = @"CREATE TABLE dbo_catcher_boat_operations (
                                                row_id INTEGER, 
                                                boat_name VARCHAR,
                                                date_start_operation DATETIME,
                                                catch_weight DOUBLE,
                                                gear_code VARCHAR(6),
                                                carrierlanding_id INTEGER,
                                                CONSTRAINT PrimaryKey PRIMARY KEY (row_id),
                                                CONSTRAINT FK_cbo_gear
                                                    FOREIGN KEY (gear_code) REFERENCES
                                                    gear (GearCode),
                                                CONSTRAINT FK_cbo_carrierlanding
                                                    FOREIGN KEY (carrierlanding_id) REFERENCES
                                                    dbo_carrier_landing (row_id)
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
