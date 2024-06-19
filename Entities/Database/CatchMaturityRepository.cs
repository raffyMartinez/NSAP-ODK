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
using DocumentFormat.OpenXml.Wordprocessing;

namespace NSAP_ODK.Entities.Database
{
    public class CatchMaturityRepository
    {
        public List<CatchMaturity> CatchMaturities { get; set; }

        public CatchMaturityRepository(VesselCatch vc)
        {
            CatchMaturities = getCatchMaturites(vc);
        }

        public static List<CatchMaturityCrossTab> GetCatchMaturityForCrosstab(NSAP_ODK.TreeViewModelControl.AllSamplingEntitiesEventHandler e)
        {
            List<CatchMaturityCrossTab> cmcts = new List<CatchMaturityCrossTab>();
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

                        cmd.CommandText = @"
                    SELECT
                      dbo_catch_maturity.catch_maturity_id,
                      dbo_vessel_unload.v_unload_id,
                      dbo_catch_maturity.catch_id,
                      dbo_vessel_unload.catch_total AS wt_catch,
                      gear.GearCode,
                      gear.GearName,
                      dbo_vessel_unload.catch_total AS wt_gear,
                      dbo_vessel_unload.catch_samp AS sample_wt_gear,
                      [Genus] & ' ' & [Species] AS spName,
                      dbo_vessel_catch.taxa,
                      dbo_vessel_catch.catch_kg AS wt_sp,
                      dbo_catch_maturity.length,
                      dbo_catch_maturity.weight,
                      dbo_catch_maturity.sex,
                      dbo_catch_maturity.maturity,
                      dbo_catch_maturity.gut_content_wt,
                      dbo_catch_maturity.gut_content_code,
                      dbo_catch_maturity.gonadWt,
                      's' AS unload_type
                   FROM
                      gear INNER JOIN
                      (phFish INNER JOIN
                      (dbo_LC_FG_sample_day INNER JOIN
                      (dbo_gear_unload INNER JOIN
                      ((dbo_vessel_unload INNER JOIN
                      (dbo_vessel_catch INNER JOIN
                      dbo_catch_maturity ON
                      dbo_vessel_catch.catch_id = dbo_catch_maturity.catch_id) ON
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
                      dbo_catch_maturity.catch_maturity_id,
                      dbo_vessel_unload.v_unload_id,
                      dbo_catch_maturity.catch_id,
                      dbo_vessel_unload.catch_total AS wt_catch,
                      gear.GearCode,
                      gear.GearName,
                      dbo_vessel_unload.catch_total AS wt_gear,
                      dbo_vessel_unload.catch_samp AS sample_wt_gear,
                      [Genus] & ' ' & [Species] AS spName,
                      dbo_vessel_catch.taxa,
                      dbo_vessel_catch.catch_kg AS wt_sp,
                      dbo_catch_maturity.length,
                      dbo_catch_maturity.weight,
                      dbo_catch_maturity.sex,
                      dbo_catch_maturity.maturity,
                      dbo_catch_maturity.gut_content_wt,
                      dbo_catch_maturity.gut_content_code,
                      dbo_catch_maturity.gonadWt,
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
                      dbo_catch_maturity ON
                      dbo_vessel_catch.catch_id = dbo_catch_maturity.catch_id) ON
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
                      dbo_catch_maturity.catch_maturity_id,
                      dbo_vessel_unload.v_unload_id,
                      dbo_catch_maturity.catch_id,
                      dbo_vessel_unload.catch_total AS wt_catch,
                      gear.GearCode,
                      gear.GearName,
                      dbo_vesselunload_fishinggear.catch_weight AS wt_gear,
                      dbo_vesselunload_fishinggear.sample_weight AS sample_wt_gear,
                      [Genus] & ' ' & [Species] AS spName,
                      dbo_vessel_catch.taxa,
                      dbo_vessel_catch.catch_kg AS wt_sp,
                      dbo_catch_maturity.length,
                      dbo_catch_maturity.weight,
                      dbo_catch_maturity.sex,
                      dbo_catch_maturity.maturity,
                      dbo_catch_maturity.gut_content_wt,
                      dbo_catch_maturity.gut_content_code,
                      dbo_catch_maturity.gonadWt,
                      'm' AS unload_type
                   FROM
                      phFish INNER JOIN
                      (gear INNER JOIN
                      (dbo_LC_FG_sample_day INNER JOIN
                      (dbo_gear_unload INNER JOIN
                      (dbo_vessel_unload INNER JOIN
                      (dbo_vesselunload_fishinggear INNER JOIN
                      (dbo_vessel_catch INNER JOIN
                      dbo_catch_maturity ON
                      dbo_vessel_catch.catch_id = dbo_catch_maturity.catch_id) ON
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
                      dbo_catch_maturity.catch_maturity_id,
                      dbo_vessel_unload.v_unload_id,
                      dbo_catch_maturity.catch_id,
                      dbo_vessel_unload.catch_total AS wt_catch,
                      gear.GearCode,
                      gear.GearName,
                      dbo_vesselunload_fishinggear.catch_weight AS wt_gear,
                      dbo_vesselunload_fishinggear.sample_weight AS sample_wt_gear,
                      [Genus] & ' ' & [Species] AS spName,
                      dbo_vessel_catch.taxa,
                      dbo_vessel_catch.catch_kg AS wt_sp,
                      dbo_catch_maturity.length,
                      dbo_catch_maturity.weight,
                      dbo_catch_maturity.sex,
                      dbo_catch_maturity.maturity,
                      dbo_catch_maturity.gut_content_wt,
                      dbo_catch_maturity.gut_content_code,
                      dbo_catch_maturity.gonadWt,
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
                      dbo_catch_maturity ON
                      dbo_vessel_catch.catch_id = dbo_catch_maturity.catch_id) ON
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
                                CatchMaturityCrossTab cmct = new CatchMaturityCrossTab
                                {
                                    RowID = (int)dr["catch_maturity_id"],
                                    ParentCatchID = (int)dr["catch_id"],
                                    V_unload_id = (int)dr["v_unload_id"],
                                    WeightOfCatch = (double)dr["wt_catch"],
                                    GearCode = dr["GearCode"].ToString(),
                                    GearName = dr["GearName"].ToString(),
                                    WeightGearCatch = (double)dr["wt_gear"],
                                    SpeciesName = dr["spName"].ToString(),
                                    Taxa = dr["taxa"].ToString(),
                                    WeightSpecies = (double)dr["wt_sp"],
                                    SexCode = dr["sex"].ToString(),
                                    MaturityCode = dr["maturity"].ToString(),
                                    GutContentCode = dr["gut_content_code"].ToString()
                                };
                                if (dr["weight"] != DBNull.Value && (double)dr["weight"] > 0)
                                {
                                    cmct.Weight = (double)dr["weight"];
                                }
                                if (dr["length"] != DBNull.Value && (double)dr["length"] > 0)
                                {
                                    cmct.Length = (double)dr["length"];
                                }
                                if (dr["gut_content_wt"] != DBNull.Value && (double)dr["gut_content_wt"] > 0)
                                {
                                    cmct.GutContentWeight = (double)dr["gut_content_wt"];
                                }
                                if (dr["gonadWt"] != DBNull.Value && (double)dr["gonadWt"] > 0)
                                {
                                    cmct.GonadWeight = (double)dr["gonadWt"];
                                }
                                cmcts.Add(cmct);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return cmcts;
        }
        public CatchMaturityRepository(bool isNew = false)
        {
            if (!isNew)
            {
                CatchMaturities = getCatchMaturites();
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
                        cmd.CommandText = "SELECT Max(catch_maturity_id) AS max_id FROM dbo_catch_maturity";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(catch_maturity_id) AS max_id FROM dbo_catch_maturity";
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

        private List<CatchMaturity> getFromMySQL(VesselCatch vc = null)
        {
            List<CatchMaturity> thisList = new List<CatchMaturity>();
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = "Select * from dbo_catch_maturity";
                    if (vc != null)
                    {
                        cmd.Parameters.AddWithValue("@parentID", vc.PK);
                        cmd.CommandText = "Select * from dbo_catch_maturity where catch_id=@parentID";
                    }

                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        CatchMaturity item = new CatchMaturity();
                        item.Parent = vc;
                        item.PK = (int)dr["catch_maturity_id"];
                        item.VesselCatchID = (int)dr["catch_id"];
                        item.GonadWeight = dr["gonad_Wt"] == DBNull.Value ? null : (double?)dr["gonad_wt"];
                        item.Length = dr["length"] == DBNull.Value ? null : (double?)dr["length"];
                        item.Weight = dr["weight"] == DBNull.Value ? null : (double?)dr["weight"];
                        item.SexCode = dr["sex"].ToString();
                        item.MaturityCode = dr["maturity"].ToString();
                        item.WeightGutContent = dr["gut_content_wt"] == DBNull.Value ? null : (double?)dr["gut_content_wt"];
                        item.GutContentCode = dr["gut_content_class"].ToString();
                        thisList.Add(item);
                    }
                }
            }
            return thisList;
        }


        public static bool AddFieldToTable(string fieldName)
        {
            bool success = false;
            string sql = "";
            switch (fieldName)
            {
                case "gonadWt":
                    sql = $"ALTER TABLE dbo_catch_maturity ADD COLUMN {fieldName} double";
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
        private List<CatchMaturity> getCatchMaturites(VesselCatch vc = null)
        {
            List<CatchMaturity> thisList = new List<CatchMaturity>();
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
                            cmd.CommandText = "Select * from dbo_catch_maturity";
                            if (vc != null)
                            {
                                cmd.Parameters.AddWithValue("@parentID", vc.PK);
                                cmd.CommandText = "Select * from dbo_catch_maturity where catch_id=@parentID";
                            }

                            thisList.Clear();
                            OleDbDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                CatchMaturity item = new CatchMaturity();
                                item.Parent = vc;
                                item.PK = (int)dr["catch_maturity_id"];
                                item.VesselCatchID = (int)dr["catch_id"];
                                item.GonadWeight = dr["gonadWt"] == DBNull.Value ? null : (double?)dr["gonadWt"];
                                item.Length = dr["length"] == DBNull.Value ? null : (double?)dr["length"];
                                item.Weight = dr["weight"] == DBNull.Value ? null : (double?)dr["weight"];
                                item.SexCode = dr["sex"].ToString();
                                item.MaturityCode = dr["maturity"].ToString();
                                item.WeightGutContent = dr["gut_content_wt"] == DBNull.Value ? null : (double?)dr["gut_content_wt"];
                                item.GutContentCode = dr["gut_content_code"].ToString();
                                thisList.Add(item);
                            }

                        }
                        catch (OleDbException dbex)
                        {
                            Logger.Log(dbex);
                        }
                        catch (Exception ex)
                        {
                            if (ex.HResult == -2147024809)
                            {
                                conection.Close();
                                UpdateTable();
                            }
                            else
                            {
                                Logger.Log(ex);
                            }

                        }
                    }
                }
            }
            return thisList;
        }

        private void UpdateTable()
        {
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = @"ALTER TABLE dbo_catch_maturity ADD COLUMN gonadWt FLOAT";
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;

                try
                {
                    cmd.ExecuteNonQuery();

                }
                catch (OleDbException dbex)
                {
                    Logger.Log(dbex);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }

                cmd.Connection.Close();
                conn.Close();
            }
            getCatchMaturites();
        }

        private bool AddToMySQL(CatchMaturity item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@pk", MySqlDbType.Int32).Value = item.PK;
                    update.Parameters.Add("@parent_id", MySqlDbType.Int32).Value = item.VesselCatchID;
                    if (item.Length == null)
                    {
                        update.Parameters.Add("@len", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@len", MySqlDbType.Double).Value = item.Length;
                    }
                    if (item.Weight == null)
                    {
                        update.Parameters.Add("@wt", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@wt", MySqlDbType.Double).Value = item.Weight;
                    }
                    if (item.SexCode == null)
                    {
                        update.Parameters.Add("@sex_code", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@sex_code", MySqlDbType.VarChar).Value = item.SexCode.ToString();
                    }

                    if (item.MaturityCode == null)
                    {
                        update.Parameters.Add("@maturity_code", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@maturity_code", MySqlDbType.VarChar).Value = item.MaturityCode.ToString();
                    }
                    if (item.WeightGutContent == null)
                    {
                        update.Parameters.Add("@wt_gut_content", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@wt_gut_content", MySqlDbType.Double).Value = item.WeightGutContent;
                    }
                    if (item.GutContentCode == null)
                    {
                        update.Parameters.Add("@gut_content_code", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@gut_content_code", MySqlDbType.VarChar).Value = item.GutContentCode.ToString();
                    }
                    if (item.GonadWeight == null)
                    {
                        update.Parameters.Add("@wt_gonad", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@wt_gonad", MySqlDbType.Double).Value = item.GonadWeight;
                    }
                    update.CommandText = @"Insert into dbo_catch_maturity(catch_maturity_id, catch_id, length, weight, sex, maturity, gut_content_wt, gut_content_class, gonad_wt)
                                            Values (@pk, @parent_id, @len, @wt, @sex_code, @maturity_code, @wt_gut_content, @gut_content_code, @wt_gonad)";
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
        public bool Add(CatchMaturity item)
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

                    var sql = @"Insert into dbo_catch_maturity(catch_maturity_id, catch_id, length, weight, sex, maturity, gut_content_wt, gut_content_code, gonadWt)
                           Values (?, ?, ?, ?, ?, ?, ?, ?, ?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@pk", OleDbType.Integer).Value = item.PK;
                        update.Parameters.Add("@parent_id", OleDbType.Integer).Value = item.VesselCatchID;
                        if (item.Length == null)
                        {
                            update.Parameters.Add("@len", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@len", OleDbType.Double).Value = item.Length;
                        }
                        if (item.Weight == null)
                        {
                            update.Parameters.Add("@wt", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@wt", OleDbType.Double).Value = item.Weight;
                        }
                        if (item.SexCode == null)
                        {
                            update.Parameters.Add("@sex_code", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@sex_code", OleDbType.VarChar).Value = item.SexCode.ToString();
                        }

                        if (item.MaturityCode == null)
                        {
                            update.Parameters.Add("@maturity_code", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@maturity_code", OleDbType.VarChar).Value = item.MaturityCode.ToString();
                        }
                        if (item.WeightGutContent == null)
                        {
                            update.Parameters.Add("@wt_gut_content", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@wt_gut_content", OleDbType.Double).Value = item.WeightGutContent;
                        }
                        if (item.GutContentCode == null)
                        {
                            update.Parameters.Add("@gut_content_code", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@gut_content_code", OleDbType.VarChar).Value = item.GutContentCode.ToString();
                        }
                        if (item.GonadWeight == null)
                        {
                            update.Parameters.Add("@wt_gonad", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@wt_gonad", OleDbType.Double).Value = item.GonadWeight;
                        }

                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException dbex)
                        {
                            if (dbex.ErrorCode == -2147217900)
                            {
                                if (dbex.Message.Contains("does not belong to table"))
                                {
                                    var arr = dbex.Message.Split('\'');
                                    if (UpdateTableDefinition(arr[1]))
                                    {
                                        return Add(item);
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine(dbex.Message);
                                success = false;
                            }
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
        private bool UpdateTableDefinition(string colName)
        {
            bool success = false;
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = "";
                switch (colName)
                {

                    case "gonadWt":
                        sql = $@"ALTER TABLE dbo_catch_maturity ADD COLUMN {colName} DOUBLE DEFAULT NULL";
                        break;
                }

                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;

                try
                {
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

                cmd.Connection.Close();
                conn.Close();
            }
            return success;
        }
        private bool UpdateMySQL(CatchMaturity item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@pk", MySqlDbType.Int32).Value = item.PK;
                    update.Parameters.Add("@parent_id", MySqlDbType.Int32).Value = item.VesselCatchID;
                    if (item.Length == null)
                    {
                        update.Parameters.Add("@len", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@len", MySqlDbType.Double).Value = item.Length;
                    }
                    if (item.Weight == null)
                    {
                        update.Parameters.Add("@wt", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@wt", MySqlDbType.Double).Value = item.Weight;
                    }
                    if (item.SexCode == null)
                    {
                        update.Parameters.Add("@sex_code", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@sex_code", MySqlDbType.VarChar).Value = item.SexCode.ToString();
                    }

                    if (item.MaturityCode == null)
                    {
                        update.Parameters.Add("@maturity_code", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@maturity_code", MySqlDbType.VarChar).Value = item.MaturityCode.ToString();
                    }
                    if (item.WeightGutContent == null)
                    {
                        update.Parameters.Add("@wt_gut_content", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@wt_gut_content", MySqlDbType.Double).Value = item.WeightGutContent;
                    }
                    if (item.GutContentCode == null)
                    {
                        update.Parameters.Add("@gut_content_code", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@gut_content_code", MySqlDbType.VarChar).Value = item.GutContentCode.ToString();
                    }
                    if (item.GonadWeight == null)
                    {
                        update.Parameters.Add("@wt_gonad", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@wt_gonad", MySqlDbType.Double).Value = item.GonadWeight;
                    }

                    update.CommandText = @"Update dbo_catch_maturity set
                                catch_id=@parent_id,
                                length = @len,
                                weight = @wt,
                                sex = @sex_code,
                                maturity = @maturity_code,
                                gut_content_wt =  @wt_gut_content,
                                gut_content_class = @gut_content_code,
                                gonad_wt = @wt_gonad,    
                            WHERE catch_maturity_id = @pk";

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
        public bool Update(CatchMaturity item)
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

                        update.Parameters.Add("@parent_id", OleDbType.Integer).Value = item.VesselCatchID;
                        if (item.Length == null)
                        {
                            update.Parameters.Add("@len", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@len", OleDbType.Double).Value = item.Length;
                        }
                        if (item.Weight == null)
                        {
                            update.Parameters.Add("@wt", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@wt", OleDbType.Double).Value = item.Weight;
                        }
                        if (string.IsNullOrEmpty(item.SexCode))
                        {
                            update.Parameters.Add("@sex_code", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@sex_code", OleDbType.VarChar).Value = item.SexCode.ToString();
                        }

                        if (string.IsNullOrEmpty(item.MaturityCode))
                        {
                            update.Parameters.Add("@maturity_code", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@maturity_code", OleDbType.VarChar).Value = item.MaturityCode.ToString();
                        }
                        if (item.WeightGutContent == null)
                        {
                            update.Parameters.Add("@wt_gut_content", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@wt_gut_content", OleDbType.Double).Value = item.WeightGutContent;
                        }
                        if (string.IsNullOrEmpty(item.GutContentCode))
                        {
                            update.Parameters.Add("@gut_content_code", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@gut_content_code", OleDbType.VarChar).Value = item.GutContentCode.ToString();
                        }
                        if (item.GonadWeight == null)
                        {
                            update.Parameters.Add("@wt_gonad", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@wt_gonad", OleDbType.Double).Value = item.GonadWeight;
                        }
                        update.Parameters.Add("@pk", OleDbType.Integer).Value = item.PK;
                        update.CommandText = @"Update dbo_catch_maturity set
                                catch_id=@parent_id,
                                length = @len,
                                weight = @wt,
                                sex = @sex_code,
                                maturity = @maturity_code,
                                gut_content_wt =  @wt_gut_content,
                                gut_content_code = @gut_content_code,
                                gonadWt = @wt_gonad    
                            WHERE catch_maturity_id = @pk";

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
                var sql = $"Delete * from dbo_catch_maturity";
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
                    update.CommandText = "Delete  from dbo_catch_maturity where catch_maturity_id=@id";
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
                        update.CommandText = "Delete * from dbo_catch_maturity where catch_maturity_id=@id";
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
