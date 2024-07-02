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
    class RegionWatchedSpeciesRepository
    {
     
        public static event EventHandler<CrossTabReportEventArg> WatchedSpeciesEvent;
        public List<RegionWatchedSpecies> RegionWatchedSpecieses { get; set; }

        public RegionWatchedSpeciesRepository(NSAPRegion reg)
        {
            RegionWatchedSpecieses = getRegionWatchedSpecieses(reg);
        }

        public static Task<List<RegionWatchedSpecies>> GetFromExistingTask(NSAPRegion reg)
        {
            return Task.Run(() => GetFromExisting(reg));
        }
        public static List<RegionWatchedSpecies> GetFromExisting(NSAPRegion reg)
        {
            WatchedSpeciesEvent?.Invoke(null, new CrossTabReportEventArg { Context = "Reading database" });
            List<RegionWatchedSpecies> watchedSpecieses = new List<RegionWatchedSpecies>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@reg", reg.Code);
                        cmd.CommandText = @"SELECT DISTINCT
                                                dbo_vessel_catch.taxa,
                                                dbo_vessel_catch.species_id
                                            FROM
                                                dbo_LC_FG_sample_day INNER JOIN
                                                (dbo_gear_unload INNER JOIN
                                                (dbo_vessel_unload INNER JOIN
                                                (dbo_vessel_catch INNER JOIN
                                                dbo_catch_len ON
                                                dbo_vessel_catch.catch_id = dbo_catch_len.catch_id) ON
                                                dbo_vessel_unload.v_unload_id = dbo_vessel_catch.v_unload_id) ON
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id
                                            WHERE 
                                                dbo_LC_FG_sample_day.region_id=@reg AND 
                                                dbo_vessel_catch.species_id Is Not Null

                                            UNION

                                            SELECT DISTINCT
                                                dbo_vessel_catch.taxa,
                                                dbo_vessel_catch.species_id
                                            FROM
                                                ((dbo_LC_FG_sample_day INNER JOIN
                                                (dbo_gear_unload INNER JOIN
                                                dbo_vessel_unload ON
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN
                                                dbo_vesselunload_fishinggear ON
                                                dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN
                                                (dbo_vessel_catch INNER JOIN
                                                dbo_catch_len ON
                                                dbo_vessel_catch.catch_id = dbo_catch_len.catch_id) ON
                                                dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id
                                            WHERE 
                                                dbo_LC_FG_sample_day.region_id=@reg AND 
                                                dbo_vessel_catch.species_id Is Not Null                                            

                                            UNION

                                            SELECT DISTINCT
                                                dbo_vessel_catch.taxa,
                                                dbo_vessel_catch.species_id
                                            FROM
                                                dbo_LC_FG_sample_day INNER JOIN
                                                (dbo_gear_unload INNER JOIN
                                                (dbo_vessel_unload INNER JOIN
                                                (dbo_vessel_catch INNER JOIN
                                                dbo_catch_len_freq ON
                                                dbo_vessel_catch.catch_id = dbo_catch_len_freq.catch_id) ON
                                                dbo_vessel_unload.v_unload_id = dbo_vessel_catch.v_unload_id) ON
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id
                                            WHERE 
                                                dbo_LC_FG_sample_day.region_id=@reg AND 
                                                dbo_vessel_catch.species_id Is Not Null

                                            UNION

                                            SELECT DISTINCT
                                                dbo_vessel_catch.taxa,
                                                dbo_vessel_catch.species_id
                                            FROM
                                                ((dbo_LC_FG_sample_day INNER JOIN
                                                (dbo_gear_unload INNER JOIN
                                                dbo_vessel_unload ON
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN
                                                dbo_vesselunload_fishinggear ON
                                                dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN
                                                (dbo_vessel_catch INNER JOIN
                                                dbo_catch_len_freq ON
                                                dbo_vessel_catch.catch_id = dbo_catch_len_freq.catch_id) ON
                                                dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id
                                            WHERE 
                                                dbo_LC_FG_sample_day.region_id=@reg AND 
                                                dbo_vessel_catch.species_id Is Not Null

                                            UNION

                                            SELECT DISTINCT
                                                dbo_vessel_catch.taxa,
                                                dbo_vessel_catch.species_id
                                            FROM
                                                dbo_LC_FG_sample_day INNER JOIN
                                                (dbo_gear_unload INNER JOIN
                                                (dbo_vessel_unload INNER JOIN
                                                (dbo_vessel_catch INNER JOIN
                                                dbo_catch_len_wt ON
                                                dbo_vessel_catch.catch_id = dbo_catch_len_wt.catch_id) ON
                                                dbo_vessel_unload.v_unload_id = dbo_vessel_catch.v_unload_id) ON
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id
                                            WHERE 
                                                dbo_LC_FG_sample_day.region_id=@reg AND 
                                                dbo_vessel_catch.species_id Is Not Null

                                            UNION

                                            SELECT DISTINCT
                                                dbo_vessel_catch.taxa,
                                                dbo_vessel_catch.species_id
                                            FROM
                                                ((dbo_LC_FG_sample_day INNER JOIN
                                                (dbo_gear_unload INNER JOIN
                                                dbo_vessel_unload ON
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN
                                                dbo_vesselunload_fishinggear ON
                                                dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN
                                                (dbo_vessel_catch INNER JOIN
                                                dbo_catch_len_wt ON
                                                dbo_vessel_catch.catch_id = dbo_catch_len_wt.catch_id) ON
                                                dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id
                                            WHERE 
                                                dbo_LC_FG_sample_day.region_id=@reg AND 
                                                dbo_vessel_catch.species_id Is Not Null

                                            UNION

                                            SELECT DISTINCT
                                                dbo_vessel_catch.taxa,
                                                dbo_vessel_catch.species_id
                                            FROM
                                                dbo_LC_FG_sample_day INNER JOIN
                                                (dbo_gear_unload INNER JOIN
                                                (dbo_vessel_unload INNER JOIN
                                                (dbo_vessel_catch INNER JOIN
                                                dbo_catch_maturity ON
                                                dbo_vessel_catch.catch_id = dbo_catch_maturity.catch_id) ON
                                                dbo_vessel_unload.v_unload_id = dbo_vessel_catch.v_unload_id) ON
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id
                                            WHERE 
                                                dbo_LC_FG_sample_day.region_id=@reg AND 
                                                dbo_vessel_catch.species_id Is Not Null

                                            UNION

                                            SELECT DISTINCT
                                                dbo_vessel_catch.taxa,
                                                dbo_vessel_catch.species_id
                                            FROM
                                                ((dbo_LC_FG_sample_day INNER JOIN
                                                (dbo_gear_unload INNER JOIN
                                                dbo_vessel_unload ON
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN
                                                dbo_vesselunload_fishinggear ON
                                                dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN
                                                (dbo_vessel_catch INNER JOIN
                                                dbo_catch_maturity ON
                                                dbo_vessel_catch.catch_id = dbo_catch_maturity.catch_id) ON
                                                dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id
                                            WHERE 
                                                dbo_LC_FG_sample_day.region_id=@reg AND 
                                                dbo_vessel_catch.species_id Is Not Null";

                        con.Open();
                        try
                        {
                            var dr = cmd.ExecuteReader();
                            WatchedSpeciesEvent?.Invoke(null, new CrossTabReportEventArg { Context = "Getting entities"});
                            while (dr.Read())
                            {
                                RegionWatchedSpecies rws = new RegionWatchedSpecies
                                {
                                    SpeciesID = (int)dr["species_id"],
                                    TaxaCode = dr["taxa"].ToString()
                                };
                                if(rws.TaxaCode=="FIS")
                                {
                                    rws.FishSpecies = NSAPEntities.FishSpeciesViewModel.GetSpecies(rws.SpeciesID);
                                }
                                else
                                {
                                    rws.NotFishSpecies = NSAPEntities.NotFishSpeciesViewModel.GetSpecies(rws.SpeciesID);
                                }
                                watchedSpecieses.Add(rws);
                            }
                        }
                        catch (Exception ex)
                        {

                            Logger.Log(ex);
                        }
                    }
                }
            }

            WatchedSpeciesEvent?.Invoke(null, new CrossTabReportEventArg { Context = "Entities retrieved", RowsPrepared=watchedSpecieses.Count});
            return watchedSpecieses;
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
                    .Any(r => r.ItemArray[2].ToString().ToLower() == "dbo_region_watched_species"))
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
            using (var conn = new OleDbConnection(Utilities.Global.ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {

                    cmd.CommandText = @"CREATE TABLE dbo_region_watched_species (
                                                row_id Int NOT NULL PRIMARY KEY,
                                                sp_id Int,
                                                taxa_code VarChar,
                                                region_code VarChar,
                                                CONSTRAINT FK_region_species
                                                    FOREIGN KEY(region_code) REFERENCES nsapRegion(Code)
                                                )";
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        success = true;

                    }
                    catch (OleDbException odx)
                    {
                        Utilities.Logger.Log(odx);
                    }
                    catch (Exception ex)
                    {
                        Utilities.Logger.Log(ex);
                    }
                }
            }

            return success;
        }

        private List<RegionWatchedSpecies> getRegionWatchedSpecieses(NSAPRegion reg)
        {
            List<RegionWatchedSpecies> rwss = new List<RegionWatchedSpecies>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@reg", reg.Code);
                        cmd.CommandText = @"SELECT * 
                                            FROM 
                                                dbo_region_watched_species
                                            WHERE
                                                region_code=@reg";
                        con.Open();
                        try
                        {
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                RegionWatchedSpecies rws = new RegionWatchedSpecies
                                {
                                    PK = (int)dr["row_id"],
                                    TaxaCode = dr["taxa_code"].ToString(),
                                    SpeciesID = (int)dr["sp_id"]
                                };
                                rwss.Add(rws);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return rwss;
        }
        public bool Add(RegionWatchedSpecies rws)
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
                        cmd.Parameters.AddWithValue("@reg", rws.NSAPRegion.Code);
                        cmd.Parameters.AddWithValue("@sp_id", rws.SpeciesID);
                        cmd.Parameters.AddWithValue("@taxa", rws.TaxaCode);
                        cmd.Parameters.AddWithValue("@pk", rws.PK);

                        cmd.CommandText = @"INSERT INTO dbo_region_watched_species (
                                                region_code,
                                                sp_id,
                                                taxa_code,
                                                row_id)
                                            VALUES (
                                                @reg,
                                                @sp_id,
                                                @taxa,
                                                @pk)";
                        con.Open();
                        try
                        {
                            success = cmd.ExecuteNonQuery() >= 0;
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

        public bool Update(RegionWatchedSpecies rws)
        {
            return false;
        }

        public bool Delete(int rws_id)
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
                        cmd.Parameters.AddWithValue("@id", rws_id);
                        cmd.CommandText = "Delete * from dbo_region_watched_species where row_id=@id";
                        con.Open();
                        try
                        {
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

        public static int MaxRecordNumber()
        {
            int max_rec_no = 0;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();

                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(row_id) AS max_id FROM dbo_region_watched_species";
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
    }
}
