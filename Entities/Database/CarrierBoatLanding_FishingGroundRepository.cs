using DocumentFormat.OpenXml.Presentation;
using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class CarrierBoatLanding_FishingGroundRepository
    {
        public List<CarrierBoatLanding_FishingGround> CarrierBoatLanding_FishingGrounds { get; set; }

        public CarrierBoatLanding_FishingGroundRepository(CarrierLanding parent)
        {
            CarrierBoatLanding_FishingGrounds = getCarrierBoatLanding_FishingGrounds(parent);
        }

        public static bool ClearTable()
        {
            bool success = false;

            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = "Delete * from dbo_carrier_landing_fishing_ground";

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
        public CarrierBoatLanding_FishingGroundRepository()
        {
            CarrierBoatLanding_FishingGrounds = getCarrierBoatLanding_FishingGrounds();
        }

        public bool Add(CarrierBoatLanding_FishingGround item)
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
                        cmd.Parameters.AddWithValue("@id", item.RowID);
                        cmd.Parameters.AddWithValue("@parent_id", item.Parent.RowID);
                        cmd.Parameters.AddWithValue("@fg_code", item.FishingGroundCode);

                        cmd.CommandText = @"INSERT INTO dbo_carrier_landing_fishing_ground (row_id,carrierlanding_id,fishing_ground) VALUES (?,?,?)";
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

        public bool Update(CarrierBoatLanding_FishingGround item)
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
                        cmd.Parameters.AddWithValue("@carrierlanding_id", item.Parent.RowID);
                        cmd.Parameters.AddWithValue("@fg_code", item.FishingGroundCode);
                        cmd.Parameters.AddWithValue("@id", item.RowID);

                        cmd.CommandText = @"UPDATE dbo_carrier_landing_fishing_ground SET
                                               carrierlanding_id = @carrierlanding_id,
                                               fishing_ground = @fg_code
                                            WHERE
                                                row_id = @id";

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
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.CommandText = "DELETE * from dbo_carrier_landing_fishing_ground WHERE row_id = @id";

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

        private List<CarrierBoatLanding_FishingGround> getCarrierBoatLanding_FishingGrounds(CarrierLanding parent = null)
        {
            List<CarrierBoatLanding_FishingGround> this_list = new List<CarrierBoatLanding_FishingGround>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "Select * from dbo_carrier_landing_fishing_ground";
                        if (parent != null)
                        {
                            cmd.Parameters.AddWithValue("@id", parent.RowID);
                            cmd.CommandText = "SELECT * FROM dbo_carrier_landing_fishing_ground WHERE carrierlanding_id=@id";

                        }
                        try
                        {
                            con.Open();
                            OleDbDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                CarrierBoatLanding_FishingGround item = new CarrierBoatLanding_FishingGround
                                {
                                    RowID = (int)reader["row_id"],
                                    Parent = parent,
                                    FishingGroundCode = reader["fishing_ground"].ToString()
                                };
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
        public static bool CheckTableExist()
        {
            bool tableExists = false;
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var schema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                if (schema.Rows
                    .OfType<DataRow>()
                    .Any(r => r.ItemArray[2].ToString().ToLower() == "dbo_carrier_landing_fishing_ground"))
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
                    cmd.CommandText = @"CREATE TABLE dbo_carrier_landing_fishing_ground (
                                                row_id INTEGER, 
                                                fishing_ground VARCHAR(6),
                                                carrierlanding_id INTEGER,
                                                CONSTRAINT PrimaryKey PRIMARY KEY (row_id),
                                                CONSTRAINT FK_clfg_fishingground
                                                    FOREIGN KEY (fishing_ground) REFERENCES
                                                    fishingGround (FishingGroundCode),
                                                CONSTRAINT FK_clfg_carrierlanding
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
