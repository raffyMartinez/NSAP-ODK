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
    public class CatchLenFreqRepository
    {
        public List<CatchLenFreq> CatchLenFreqs { get; set; }

        public CatchLenFreqRepository(VesselCatch vc)
        {
            CatchLenFreqs = getCatchLenFreqs(vc);
        }
        public static List<CatchLengthFreqCrossTab> GetLengthFreqForCrosstab(NSAP_ODK.TreeViewModelControl.AllSamplingEntitiesEventHandler e)
        {
            List<CatchLengthFreqCrossTab> clfcts = new List<CatchLengthFreqCrossTab>();
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
                             dbo_catch_len_freq.catch_len_freq_id,
                             dbo_vessel_unload.v_unload_id,
                             dbo_vessel_unload.catch_total AS wt_catch,
                             gear.GearName,
                             dbo_vessel_unload.catch_total AS wt_gear,
                             dbo_vessel_unload.catch_samp AS sample_wt_gear,
                             [Genus] & ' ' & [Species] AS spName,
                             dbo_vessel_catch.taxa,
                             dbo_vessel_catch.catch_kg AS wt_sp,
                             dbo_catch_len_freq.len_class,
                             dbo_catch_len_freq.freq,                             
                             dbo_catch_len_freq.sex,
                             's' AS unload_type
                          FROM
                             gear INNER JOIN
                             (phFish INNER JOIN
                             (dbo_LC_FG_sample_day INNER JOIN
                             (dbo_gear_unload INNER JOIN
                             ((dbo_vessel_unload INNER JOIN
                             (dbo_vessel_catch INNER JOIN
                             dbo_catch_len_freq ON
                             dbo_vessel_catch.catch_id = dbo_catch_len_freq.catch_id) ON
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
                             dbo_catch_len_freq.catch_len_freq_id,
                             dbo_vessel_unload.v_unload_id,
                             dbo_vessel_unload.catch_total AS wt_catch,
                             gear.GearName,
                             dbo_vessel_unload.catch_total AS wt_gear,
                             dbo_vessel_unload.catch_samp AS sample_wt_gear,
                             [Genus] & ' ' & [Species] AS spName,
                             dbo_vessel_catch.taxa,
                             dbo_vessel_catch.catch_kg AS wt_sp,
                             dbo_catch_len_freq.len_class,
                             dbo_catch_len_freq.freq,                             
                             dbo_catch_len_freq.sex,
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
                             dbo_catch_len_freq ON
                             dbo_vessel_catch.catch_id = dbo_catch_len_freq.catch_id) ON
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
                             dbo_catch_len_freq.catch_len_freq_id,
                             dbo_vessel_unload.v_unload_id,
                             dbo_vessel_unload.catch_total AS wt_catch,
                             gear.GearName,
                             dbo_vesselunload_fishinggear.catch_weight AS wt_gear,
                             dbo_vesselunload_fishinggear.sample_weight AS sample_wt_gear,
                             [Genus] & ' ' & [Species] AS spName,
                             dbo_vessel_catch.taxa,
                             dbo_vessel_catch.catch_kg AS wt_sp,
                             dbo_catch_len_freq.len_class,
                             dbo_catch_len_freq.freq,                             
                             dbo_catch_len_freq.sex,
                             'm' AS unload_type
                          FROM
                             phFish INNER JOIN
                             (gear INNER JOIN
                             (dbo_LC_FG_sample_day INNER JOIN
                             (dbo_gear_unload INNER JOIN
                             (dbo_vessel_unload INNER JOIN
                             (dbo_vesselunload_fishinggear INNER JOIN
                             (dbo_vessel_catch INNER JOIN
                             dbo_catch_len_freq ON
                             dbo_vessel_catch.catch_id = dbo_catch_len_freq.catch_id) ON
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
                             dbo_catch_len_freq.catch_len_freq_id,
                             dbo_vessel_unload.v_unload_id,
                             dbo_vessel_unload.catch_total AS wt_catch,
                             gear.GearName,
                             dbo_vesselunload_fishinggear.catch_weight AS wt_gear,
                             dbo_vesselunload_fishinggear.sample_weight AS sample_wt_gear,
                             [Genus] & ' ' & [Species] AS spName,
                             dbo_vessel_catch.taxa,
                             dbo_vessel_catch.catch_kg AS wt_sp,
                             dbo_catch_len_freq.len_class,
                             dbo_catch_len_freq.freq,                             
                             dbo_catch_len_freq.sex,
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
                             dbo_catch_len_freq ON
                             dbo_vessel_catch.catch_id = dbo_catch_len_freq.catch_id) ON
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
                                CatchLengthFreqCrossTab clfct = new CatchLengthFreqCrossTab
                                {
                                    RowID = (int)dr["catch_len_freq_id"],
                                    V_unload_id = (int)dr["v_unload_id"],
                                    WeightOfCatch = (double)dr["wt_catch"],
                                    GearName = dr["GearName"].ToString(),
                                    WeightGearCatch = (double)dr["wt_gear"],
                                    SpeciesName = dr["spName"].ToString(),
                                    Taxa = dr["taxa"].ToString(),
                                    WeightSpecies = (double)dr["wt_sp"],
                                    Length = (double)dr["len_class"],
                                    Frequency = (int)dr["freq"],
                                    Sex = dr["sex"].ToString()
                                };
                                clfcts.Add(clfct);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return clfcts;
        }
        public static bool AddFieldToTable(string fieldName)
        {
            bool success = false;
            string sql = "";
            switch (fieldName)
            {
                case "sex":
                    sql = "ALTER TABLE dbo_catch_len_freq ADD COLUMN sex varchar(2)";
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
        public CatchLenFreqRepository(bool isNew = false)
        {
            if (!isNew)
            {
                CatchLenFreqs = getCatchLenFreqs();
            }
        }
        private List<CatchLenFreq> getFromMySQL(VesselCatch vc = null)
        {
            List<CatchLenFreq> thisList = new List<CatchLenFreq>();
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = "Select * from dbo_catch_len_freq";
                    if (vc != null)
                    {
                        cmd.Parameters.AddWithValue("@parentID", vc.PK);
                        cmd.CommandText = $"Select * from dbo_catch_len_freq where catch_id=@parentID";
                    }

                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        CatchLenFreq item = new CatchLenFreq();
                        item.Parent = vc;
                        item.PK = (int)dr["catch_lf_id"];
                        item.VesselCatchID = (int)dr["catch_id"];
                        item.LengthClass = (double)dr["length"];
                        item.Frequency = (int)dr["freq"];
                        thisList.Add(item);
                    }
                }
            }
            return thisList;
        }
        private List<CatchLenFreq> getCatchLenFreqs(VesselCatch vc = null)
        {
            List<CatchLenFreq> thisList = new List<CatchLenFreq>();
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
                            cmd.CommandText = $"Select * from dbo_catch_len_freq";

                            if (vc != null)
                            {
                                cmd.Parameters.AddWithValue("@parentID", vc.PK);
                                cmd.CommandText = $"Select * from dbo_catch_len_freq where catch_id=@parentID";
                            }
                            thisList.Clear();
                            OleDbDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                CatchLenFreq item = new CatchLenFreq();
                                item.Parent = vc;
                                item.PK = (int)dr["catch_len_freq_id"];
                                item.VesselCatchID = (int)dr["catch_id"];
                                item.LengthClass = (double)dr["len_class"];
                                item.Frequency = (int)dr["freq"];
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

        private bool AddToMySQL(CatchLenFreq item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = item.PK;
                    update.Parameters.Add("@catch_id", MySqlDbType.Int32).Value = item.Parent.PK;
                    update.Parameters.Add("@len_class", MySqlDbType.Double).Value = item.LengthClass;
                    update.Parameters.Add("@freq", MySqlDbType.Int32).Value = item.Frequency;
                    update.Parameters.Add("@sex", MySqlDbType.VarChar).Value = item.Sex;
                    update.CommandText = @"Insert into dbo_catch_len_freq(catch_lf_id, catch_id, length,freq) 
                                        Values (@id,@catch_id,@len_class,@freq,@sex)";
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
        public bool Add(CatchLenFreq item)
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
                    var sql = "Insert into dbo_catch_len_freq(catch_len_freq_id, catch_id, len_class,freq,sex) Values (?,?,?,?,?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@id", OleDbType.Integer).Value = item.PK;
                        update.Parameters.Add("@catch_id", OleDbType.Integer).Value = item.Parent.PK;
                        update.Parameters.Add("@len_class", OleDbType.Double).Value = item.LengthClass;
                        update.Parameters.Add("@freq", OleDbType.Integer).Value = item.Frequency;
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
                        catch (OleDbException)
                        {
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
                        cmd.CommandText = "SELECT Max(catch_lf_id) AS max_id FROM dbo_catch_len_freq";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(catch_len_freq_id) AS max_id FROM dbo_catch_len_freq";
                    using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                    {
                        try
                        {
                            var r = getMax.ExecuteScalar();
                            if (r != DBNull.Value)
                            {
                                max_rec_no = (int)r;
                            }

                        }
                        catch (OleDbException oex)
                        {

                        }
                        catch (Exception ex)
                        {
                            //ignore
                            //Logger.Log(ex);
                        }
                    }
                }
            }
            return max_rec_no;
        }
        private bool UpdateMySQL(CatchLenFreq item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@catch_id", MySqlDbType.Int32).Value = item.Parent.PK;
                    update.Parameters.Add("@len_class", MySqlDbType.Double).Value = item.LengthClass;
                    update.Parameters.Add("@freq", MySqlDbType.Int32).Value = item.Frequency;
                    update.Parameters.Add("@sex", MySqlDbType.VarChar).Value = item.Sex;
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = item.PK;

                    update.CommandText = @"Update dbo_catch_len_freq set
                                        catch_id=@catch_id,
                                        length = @len_class,
                                        freq = @freq,
                                        sex=@sex
                                        WHERE catch_lf_id = @id";

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
        public bool Update(CatchLenFreq item)
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
                        update.Parameters.Add("@len_class", OleDbType.Double).Value = item.LengthClass;
                        update.Parameters.Add("@freq", OleDbType.Integer).Value = item.Frequency;
                        if (item.Sex == null)
                        {
                            update.Parameters.Add("@sex", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@sex", OleDbType.VarChar).Value = item.Sex;
                        }
                        update.Parameters.Add("@id", OleDbType.Integer).Value = item.PK;

                        update.CommandText = @"Update dbo_catch_len_freq set
                                        catch_id=@catch_id,
                                        len_class = @len_class,
                                        freq = @freq,
                                        sex = @sex
                                        WHERE catch_len_freq_id = @id";

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
                var sql = $"Delete * from dbo_catch_len_freq";
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
                    update.CommandText = "Delete  from dbo_catch_len_freq where catch_lf_id=@id";
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
                        update.CommandText = "Delete * from dbo_catch_len_freq where catch_len_freq_id=@id";
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
