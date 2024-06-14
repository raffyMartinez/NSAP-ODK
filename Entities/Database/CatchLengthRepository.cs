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
    public class CatchLengthRepository
    {
        public List<CatchLength> CatchLengths { get; set; }

        public CatchLengthRepository(VesselCatch vc)
        {
            CatchLengths = getCatchLengths(vc);
        }
        public static List<CatchLengthCrossTab> GetLengthForCrosstab(NSAP_ODK.TreeViewModelControl.AllSamplingEntitiesEventHandler e)
        {
            List<CatchLengthCrossTab> clcts = new List<CatchLengthCrossTab>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@reg", e.NSAPRegion.Code);
                        DateTime sDate = (DateTime)e.MonthSampled;
                        cmd.Parameters.AddWithValue("@start", sDate.ToString("MMM/dd/yyyy"));
                        cmd.Parameters.AddWithValue("@end", sDate.AddMonths(1).ToString("MMM/dd/yyyy"));
                        cmd.Parameters.AddWithValue("@ls", e.LandingSite.LandingSiteID);
                        cmd.Parameters.AddWithValue("@fg", e.FishingGround.Code);
                        cmd.Parameters.AddWithValue("@fma", e.FMA.FMAID);

                        cmd.CommandText = @"SELECT
                                     dbo_catch_len.catch_len_id,
                                     dbo_catch_len.catch_id,
                                     dbo_vessel_unload.v_unload_id,
                                     dbo_vessel_unload.catch_total AS wt_catch,
                                     gear.GearCode,
                                     gear.GearName,
                                     dbo_vessel_unload.catch_total AS wt_gear,
                                     dbo_vessel_unload.catch_samp AS sample_wt_gear,
                                     [Genus] & ' ' & [Species] AS spName,
                                     dbo_vessel_catch.taxa,
                                     dbo_vessel_catch.catch_kg AS wt_sp,
                                     dbo_catch_len.length,
                                     dbo_catch_len.sex,
                                     's' AS unload_type
                                  FROM
                                     gear INNER JOIN
                                     (phFish INNER JOIN
                                     (dbo_LC_FG_sample_day INNER JOIN
                                     (dbo_gear_unload INNER JOIN
                                     ((dbo_vessel_unload INNER JOIN
                                     (dbo_vessel_catch INNER JOIN
                                     dbo_catch_len ON
                                     dbo_vessel_catch.catch_id = dbo_catch_len.catch_id) ON
                                     dbo_vessel_unload.v_unload_id = dbo_vessel_catch.v_unload_id) INNER JOIN
                                     dbo_vessel_unload_1 ON
                                     dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id) ON
                                     dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON
                                     dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) ON
                                     phFish.SpeciesID = dbo_vessel_catch.species_id) ON
                                     gear.GearCode = dbo_gear_unload.gr_id
                                  WHERE
                                     dbo_LC_FG_sample_day.region_id=@reg AND
                                     dbo_LC_FG_sample_day.sdate>=@start AND
                                     dbo_LC_FG_sample_day.sdate<@end AND
                                     dbo_LC_FG_sample_day.land_ctr_id=@ls AND
                                     dbo_LC_FG_sample_day.ground_id=@fg AND
                                     dbo_LC_FG_sample_day.fma=@fma AND
                                     dbo_vessel_unload_1.is_multigear=False

                                 UNION ALL

                                  SELECT
                                     dbo_catch_len.catch_len_id,
                                     dbo_catch_len.catch_id,
                                     dbo_vessel_unload.v_unload_id,
                                     dbo_vessel_unload.catch_total AS wt_catch,
                                     gear.GearCode,
                                     gear.GearName,
                                     dbo_vessel_unload.catch_total AS wt_gear,
                                     dbo_vessel_unload.catch_samp AS sample_wt_gear,
                                     [Genus] & ' ' & [Species] AS spName,
                                     dbo_vessel_catch.taxa,
                                     dbo_vessel_catch.catch_kg AS wt_sp,
                                     dbo_catch_len.length,
                                     dbo_catch_len.sex,
                                    's' AS unload_type
                                  FROM
                                     notFishSpecies INNER JOIN
                                     (gear INNER JOIN
                                     (((dbo_LC_FG_sample_day INNER JOIN
                                     (dbo_gear_unload INNER JOIN
                                     dbo_vessel_unload ON
                                     dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON
                                     dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN
                                     (dbo_vessel_catch INNER JOIN
                                     dbo_catch_len ON
                                     dbo_vessel_catch.catch_id = dbo_catch_len.catch_id) ON
                                     dbo_vessel_unload.v_unload_id = dbo_vessel_catch.v_unload_id) INNER JOIN
                                     dbo_vessel_unload_1 ON
                                     dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id) ON
                                     gear.GearCode = dbo_gear_unload.gr_id) ON
                                     notFishSpecies.SpeciesID = dbo_vessel_catch.species_id
                                  WHERE
                                     dbo_LC_FG_sample_day.region_id=@reg AND
                                     dbo_LC_FG_sample_day.sdate>=@start AND
                                     dbo_LC_FG_sample_day.sdate<@end AND
                                     dbo_LC_FG_sample_day.land_ctr_id=@ls AND
                                     dbo_LC_FG_sample_day.ground_id=@fg AND
                                     dbo_LC_FG_sample_day.fma=@fma AND
                                     dbo_vessel_unload_1.is_multigear=False

                                  UNION ALL

                                  SELECT
                                     dbo_catch_len.catch_len_id,
                                     dbo_catch_len.catch_id,
                                     dbo_vessel_unload.v_unload_id,
                                     dbo_vessel_unload.catch_total AS wt_catch,
                                     gear.GearCode,
                                     gear.GearName,
                                     dbo_vesselunload_fishinggear.catch_weight AS wt_gear,
                                     dbo_vesselunload_fishinggear.sample_weight AS sample_wt_gear,
                                     [Genus] & ' ' & [Species] AS spName,
                                     dbo_vessel_catch.taxa,
                                     dbo_vessel_catch.catch_kg AS wt_sp,
                                     dbo_catch_len.length,
                                     dbo_catch_len.sex,
                                     'm' AS unload_type
                                  FROM
                                     phFish INNER JOIN
                                     (gear INNER JOIN
                                     (dbo_LC_FG_sample_day INNER JOIN
                                     (dbo_gear_unload INNER JOIN
                                     (dbo_vessel_unload INNER JOIN
                                     (dbo_vesselunload_fishinggear INNER JOIN
                                     (dbo_vessel_catch INNER JOIN
                                     dbo_catch_len ON
                                     dbo_vessel_catch.catch_id = dbo_catch_len.catch_id) ON
                                     dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id) ON
                                     dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) ON
                                     dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON
                                     dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) ON
                                     gear.GearCode = dbo_vesselunload_fishinggear.gear_code) ON
                                     phFish.SpeciesID = dbo_vessel_catch.species_id
                                  WHERE
                                     dbo_LC_FG_sample_day.region_id = @reg AND
                                     dbo_LC_FG_sample_day.sdate >= @start AND
                                     dbo_LC_FG_sample_day.sdate < @end AND
                                     dbo_LC_FG_sample_day.land_ctr_id = @ls AND
                                     dbo_LC_FG_sample_day.ground_id = @fg AND
                                     dbo_LC_FG_sample_day.fma = @fma

                                  UNION ALL

                                  SELECT
                                     dbo_catch_len.catch_len_id,
                                     dbo_catch_len.catch_id,
                                     dbo_vessel_unload.v_unload_id,
                                     dbo_vessel_unload.catch_total AS wt_catch,
                                     gear.GearCode,
                                     gear.GearName,
                                     dbo_vesselunload_fishinggear.catch_weight AS wt_gear,
                                     dbo_vesselunload_fishinggear.sample_weight AS sample_wt_gear,
                                     [Genus] & ' ' & [Species] AS spName,
                                     dbo_vessel_catch.taxa,
                                     dbo_vessel_catch.catch_kg AS wt_sp,
                                     dbo_catch_len.length,
                                     dbo_catch_len.sex,
                                     'm' AS unload_type
                                  FROM
                                     notFishSpecies INNER JOIN
                                     (((dbo_LC_FG_sample_day INNER JOIN
                                     (dbo_gear_unload INNER JOIN
                                     dbo_vessel_unload ON
                                     dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON
                                     dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN
                                     (gear INNER JOIN
                                     dbo_vesselunload_fishinggear ON
                                     gear.GearCode = dbo_vesselunload_fishinggear.gear_code) ON
                                     dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN
                                     (dbo_vessel_catch INNER JOIN
                                     dbo_catch_len ON
                                     dbo_vessel_catch.catch_id = dbo_catch_len.catch_id) ON
                                     dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id) ON
                                     notFishSpecies.SpeciesID = dbo_vessel_catch.species_id
                                  WHERE
                                     dbo_LC_FG_sample_day.region_id = @reg AND
                                     dbo_LC_FG_sample_day.sdate >= @start AND
                                     dbo_LC_FG_sample_day.sdate < @end AND
                                     dbo_LC_FG_sample_day.land_ctr_id = @ls AND
                                     dbo_LC_FG_sample_day.ground_id = @fg AND
                                     dbo_LC_FG_sample_day.fma = @fma";
                        con.Open();
                        try
                        {
                            OleDbDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                CatchLengthCrossTab clct = new CatchLengthCrossTab
                                {
                                    RowID = (int)dr["catch_len_id"],
                                    ParentCatchID = (int)dr["catch_id"],
                                    V_unload_id = (int)dr["v_unload_id"],
                                    WeightOfCatch = (double)dr["wt_catch"],
                                    GearCode = dr["GearCode"].ToString(),
                                    GearName = dr["GearName"].ToString(),
                                    WeightGearCatch = (double)dr["wt_gear"],
                                    SpeciesName = dr["spName"].ToString(),
                                    Taxa = dr["taxa"].ToString(),
                                    WeightSpecies = (double)dr["wt_sp"],
                                    Length = (double)dr["length"],
                                    Sex = dr["sex"].ToString()
                                };
                                clcts.Add(clct);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return clcts;
        }
        public static bool AddFieldToTable(string fieldName)
        {
            bool success = false;
            string sql = "";
            switch (fieldName)
            {
                case "sex":
                    sql = "ALTER TABLE dbo_catch_len ADD COLUMN sex varchar(2)";
                    break;
            }
            using (var con = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = con.CreateCommand())
                {
                    con.Open();
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
        public CatchLengthRepository(bool isNew = false)
        {
            if (!isNew)
            {
                CatchLengths = getCatchLengths();
            }
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
                        conn.Open();
                        cmd.CommandText = "SELECT Max(catch_len_id) AS max_id FROM dbo_catch_length";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(catch_len_id) AS max_id FROM dbo_catch_len";
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
        private List<CatchLength> getFromMySQL(VesselCatch vc = null)
        {
            List<CatchLength> thisList = new List<CatchLength>();
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = "Select * from dbo_catch_length";
                    if (vc != null)
                    {
                        cmd.Parameters.AddWithValue("parentID", vc.PK);
                        cmd.CommandText = "Select * from dbo_catch_length where catch_id=@parentID";
                    }

                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        CatchLength item = new CatchLength();
                        item.Parent = vc;
                        item.PK = (int)dr["catch_len_id"];
                        item.VesselCatchID = (int)dr["catch_id"];
                        item.Length = (double)dr["length"];
                        thisList.Add(item);
                    }
                }
            }
            return thisList;
        }
        private List<CatchLength> getCatchLengths(VesselCatch vc = null)
        {
            List<CatchLength> thisList = new List<CatchLength>();
            if (Global.Settings.UsemySQL)
            {
                thisList = getFromMySQL(vc);
            }
            else
            {
                var dt = new DataTable();
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = conection.CreateCommand())
                    {
                        try
                        {
                            conection.Open();
                            cmd.CommandText = "Select * from dbo_catch_len";

                            if (vc != null)
                            {
                                cmd.Parameters.AddWithValue("parentID", vc.PK);
                                cmd.CommandText = "Select * from dbo_catch_len where catch_id=@parentID";
                            }
                            thisList.Clear();
                            OleDbDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                CatchLength item = new CatchLength();
                                item.Parent = vc;
                                item.PK = (int)dr["catch_len_id"];
                                item.VesselCatchID = (int)dr["catch_id"];
                                item.Length = (double)dr["length"];
                                item.Sex = dr["sex"].ToString();
                                thisList.Add(item);
                            }

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
        private bool AddToMySQL(CatchLength item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = item.PK;
                    update.Parameters.Add("@catch_id", MySqlDbType.Int32).Value = item.Parent.PK;
                    update.Parameters.Add("@length", MySqlDbType.Double).Value = item.Length;
                    update.CommandText = @"Insert into dbo_catch_length(catch_len_id, catch_id, length) 
                                         Values (@id,@catch_id,@length)";
                    try
                    {
                        conn.Open();
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException msex)
                    {
                        Logger.Log(msex);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return success;
        }
        public bool Add(CatchLength item)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = AddToMySQL(item);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    var sql = "Insert into dbo_catch_len(catch_len_id, catch_id, length, sex) Values (?,?,?,?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@id", OleDbType.Integer).Value = item.PK;
                        update.Parameters.Add("@catch_id", OleDbType.Integer).Value = item.Parent.PK;
                        update.Parameters.Add("@length", OleDbType.Double).Value = item.Length;
                        if (item.Sex == null)
                        {
                            update.Parameters.Add("@sex", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@sex", OleDbType.VarChar).Value = item.Sex;
                        }
                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
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
        private bool UpdateMySQL(CatchLength item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@catch_id", MySqlDbType.Int32).Value = item.Parent.PK;
                    update.Parameters.Add("@length", MySqlDbType.Double).Value = item.Length;
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = item.PK;

                    update.CommandText = @"Update dbo_catch_length set
                                        catch_id=@catch_id,
                                        length = @length
                                        WHERE catch_len_id = @id"; ;

                    try
                    {
                        conn.Open();
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException msex)
                    {
                        Logger.Log(msex);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return success;
        }
        public bool Update(CatchLength item)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = UpdateMySQL(item);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    using (OleDbCommand update = conn.CreateCommand())
                    {

                        update.Parameters.Add("@catch_id", OleDbType.Integer).Value = item.Parent.PK;
                        update.Parameters.Add("@length", OleDbType.Double).Value = item.Length;
                        if (item.Sex == null)
                        {
                            update.Parameters.Add("@sex", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@sex", OleDbType.VarChar).Value = item.Sex;
                        }
                        update.Parameters.Add("@id", OleDbType.Integer).Value = item.PK;

                        update.CommandText = @"Update dbo_catch_len set
                                        catch_id=@catch_id,
                                        length = @length,
                                        sex=@sex
                                        WHERE catch_len_id = @id";

                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
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
                var sql = $"Delete * from dbo_catch_len";
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
        private bool DeleteMySQL(int id)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    update.CommandText = "Delete  from dbo_catch_len where catch_len_id=@id";
                    try
                    {
                        conn.Open();
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException msex)
                    {
                        Logger.Log(msex);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
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
                success = DeleteMySQL(id);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    using (OleDbCommand update = conn.CreateCommand())
                    {
                        update.Parameters.Add("@id", OleDbType.Integer).Value = id;
                        update.CommandText = "Delete * from dbo_catch_len where catch_len_id=@id";
                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
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
    }
}
