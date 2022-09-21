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
    public class GearAtLandingSiteDaysPerMonthRepository
    {
        public List<GearAtLandingSiteDaysPerMonth> GearAtLandingSiteDaysPerMonths { get; set; }

        public static bool CheckForGearLandingSiteTable()
        {
            bool tableExists = false;
            using (var conn = new OleDbConnection(Utilities.Global.ConnectionString))
            {
                conn.Open();
                var schema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                if (schema.Rows
                    .OfType<DataRow>()
                    .Any(r => r.ItemArray[2].ToString().ToLower() == "gear_landing_site_fishingdays"))
                {
                    tableExists = true;
                }

                if (!tableExists)
                {
                    return CreateGearLadningSiteTable();
                }
                else
                {
                    return true;
                }
            }
        }
        public static bool CreateGearLadningSiteTable()
        {
            bool success = false;
            using (var conn = new OleDbConnection(Utilities.Global.ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"CREATE TABLE gear_landing_site_fishingdays (
                                                row_id Int NOT NULL PRIMARY KEY,
                                                year_lsg Int,
                                                month_lsg Int,
                                                gear_code VarChar,
                                                landing_site Int,
                                                days_operation Int,
                                                CONSTRAINT FK_gls_gear
                                                    FOREIGN KEY(gear_code) REFERENCES gear(GearCode),
                                                CONSTRAINT FK_gls_landing_site   
                                                    FOREIGN KEY (landing_site) REFERENCES landingSite(LandingSiteID),   
                                                CONSTRAINT unique_date_gear_ls UNIQUE (year_lsg, month_lsg, gear_code, landing_site)
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
        public GearAtLandingSiteDaysPerMonthRepository()
        {
            GearAtLandingSiteDaysPerMonths = getGearAtLandingSiteDaysPerMonths();
        }

        public bool Add(GearAtLandingSiteDaysPerMonth item)
        {
            bool success = false;
            if(Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con=new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd=con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@row_id", item.RowID);
                        cmd.Parameters.AddWithValue("@year", item.Year);
                        cmd.Parameters.AddWithValue("@month", item.Month);
                        cmd.Parameters.AddWithValue("@gear_code", item.Gear.Code);
                        cmd.Parameters.AddWithValue("@landing_side_id", item.LandingSite.LandingSiteID);
                        cmd.Parameters.AddWithValue("@days_operation", item.DaysInOperation);

                        cmd.CommandText = @"INSERT INTO gear_landing_site_fishingdays
                                                row_id,
                                                year_lsg,
                                                month_lsg,
                                                gear_code,
                                                landing_site,
                                                days_operation,
                                            VALUES (@row_id,@year,@month,@gear_code,@landing_site_id,@days_operation)";

                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch(OleDbException oex)
                        {

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
        public bool Delete(int id)
        {
            bool success = false;
            if(Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd=con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@row_id", id);
                        cmd.CommandText = "DELETE * FROM gear_landing_site_fishingdays WHERE row_id=@row_id";

                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;
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
        public bool Update(GearAtLandingSiteDaysPerMonth item)
        {
            bool success = false;
            if(Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {

                        cmd.Parameters.AddWithValue("@year", item.Year);
                        cmd.Parameters.AddWithValue("@month", item.Month);
                        cmd.Parameters.AddWithValue("@gear_code", item.Gear.Code);
                        cmd.Parameters.AddWithValue("@landing_side_id", item.LandingSite.LandingSiteID);
                        cmd.Parameters.AddWithValue("@days_operation", item.DaysInOperation);
                        cmd.Parameters.AddWithValue("@row_id", item.RowID);

                        cmd.CommandText = @"UPDATE gear_landing_site_fishingdays SET
                                                year_lsg=@year,
                                                month_lsg=@month,
                                                gear_code=@gear_code,
                                                landing_site=@landing_site,
                                                days_operation=@days_operation
                                            WHERE row_id=@row_id";
                         try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch(OleDbException oex)
                        {

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
        private List<GearAtLandingSiteDaysPerMonth> getGearAtLandingSiteDaysPerMonths()
        {
            List<GearAtLandingSiteDaysPerMonth> thisList = new List<GearAtLandingSiteDaysPerMonth>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = "Select * from gear_landing_site_fishingdays";
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            GearAtLandingSiteDaysPerMonth gls = new GearAtLandingSiteDaysPerMonth();
                            gls.RowID = (int)dr["row_id"];
                            gls.Year = (int)dr["year_lsg"];
                            gls.Month = (int)dr["month_lsg"];
                            gls.Gear = NSAPEntities.GearViewModel.GetGear(dr["gear_code"].ToString());
                            gls.LandingSite = NSAPEntities.LandingSiteViewModel.GetLandingSite((int)dr["landing_site"]);
                            gls.DaysInOperation = (int)dr["days_operation"];
                            thisList.Add(gls);
                        }
                    }
                }
            }
            return thisList;
        }
    }
}
