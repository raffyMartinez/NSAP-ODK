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
    class VesselCatchRepository
    {
        public static event EventHandler<ProcessingItemsEventArg> ProcessingItemsEvent;
        public List<VesselCatch> VesselCatches { get; set; }
        private bool _requireTableDefinitionUpdate = false;

        private string _newColumnName;

        public VesselCatchRepository(CarrierLanding cl)
        {
            VesselCatches = getVesselCatches(cl);
        }
        public VesselCatchRepository(VesselUnload_FishingGear vufg)
        {
            VesselCatches = getVesselCatches(vufg);
        }
        public VesselCatchRepository(VesselUnload vu)
        {
            VesselCatches = getVesselCatches(vu);
            if (VesselCatches == null && _requireTableDefinitionUpdate && UpdateTableDefinition(_newColumnName))
            {
                _requireTableDefinitionUpdate = false;
                VesselCatches = getVesselCatches(vu);
            }
        }
        public VesselCatchRepository(bool isNew = false)
        {
            if (!isNew)
            {
                VesselCatches = getVesselCatches();
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
                        cmd.CommandText = "SELECT Max(catch_id) AS max_id FROM dbo_vessel_catch";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(catch_id) AS max_id FROM dbo_vessel_catch";
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

        public static Task<List<CatchWithZeroWeight>> GetCatchesWithZeroWeightAsync()
        {
            return Task.Run(() => GetCatchesWithZeroWeight());
        }


        public static List<CatchWithZeroWeight> GetCatchesWithZeroWeight()
        {

            List<CatchWithZeroWeight> thisLlist = new List<CatchWithZeroWeight>();
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
                        cmd.CommandText = "SELECT Count(catch_id) AS n FROM dbo_vessel_catch WHERE catch_kg=0";
                        int result = (int)cmd.ExecuteScalar();

                        if (result > 0)
                        {
                            int loopCount = 0;
                            ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "start", TotalCountToProcess = result });
                            cmd.CommandText = "Select * from dbo_vessel_catch where catch_kg=0";
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                int? sp_id = null;
                                if (dr["species_id"] != DBNull.Value)
                                {
                                    sp_id = (int)dr["species_id"];
                                }
                                CatchWithZeroWeight cw = new CatchWithZeroWeight
                                {
                                    SpeciesID = sp_id,
                                    SpeciesName = dr["species_text"].ToString(),
                                    VesselUnloadID = (int)dr["v_unload_id"],
                                    Taxa = NSAPEntities.TaxaViewModel.GetTaxa(dr["taxa"].ToString()),
                                };
                                cw.Parent = NSAPEntities.SummaryItemViewModel.GetVesselUnload(cw.VesselUnloadID);
                                if (cw.SpeciesID != null)
                                {
                                    if (cw.Taxa.Code == "FIS")
                                    {
                                        cw.FishSpecies = NSAPEntities.FishSpeciesViewModel.GetSpecies((int)cw.SpeciesID);
                                    }
                                    else
                                    {
                                        cw.NotFishSpecies = NSAPEntities.NotFishSpeciesViewModel.GetSpecies((int)cw.SpeciesID);
                                    }
                                }
                                thisLlist.Add(cw);
                                loopCount++;
                                ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "item found", CountProcessed = loopCount });
                            }
                        }
                    }
                }
            }
            ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "done searching" });
            return thisLlist;
        }
        public static List<OrphanedSpeciesNameRaw> GetOrphanedSpecies(bool multiline = false)
        {
            List<OrphanedSpeciesNameRaw> this_list = new List<OrphanedSpeciesNameRaw>();
            if (Global.Settings.UsemySQL)
            {
                using (var con = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT
                                              r.short_name,
                                              f.fma_name,
                                              fg.fishing_ground_name,
                                              ne.enumerator_name,
                                              sd.land_ctr_id,
                                              sd.land_ctr_text,
                                              vu1.enumerator_text,
                                              t.taxa,
                                              vc.species_text,
                                              g.gear_name,
                                              gu.gr_text,
                                              vu1.v_unload_id
                                            FROM
                                              nsap_odk.gears as g
                                            RIGHT JOIN
                                              ((nsap_odk.fishing_grounds as fg 
                                              INNER JOIN (nsap_odk.fma as f 
                                              INNER JOIN (nsap_odk.nsap_region as r 
                                              INNER JOIN (nsap_odk.dbo_lc_fg_sample_day as sd 
                                              INNER JOIN (nsap_odk.dbo_gear_unload as gu
                                              INNER JOIN (nsap_odk.nsap_enumerators as ne 
                                              RIGHT JOIN ((nsap_odk.dbo_vessel_unload as vu 
                                              INNER JOIN nsap_odk.dbo_vessel_catch as vc
                                                ON vu.v_unload_id = vc.v_unload_id) 
                                              INNER JOIN nsap_odk.dbo_vessel_unload_1 as vu1
                                                ON vu.v_unload_id = vu1.v_unload_id) 
                                                ON ne.enumerator_id = vu1.enumerator_id) 
                                                ON gu.unload_gr_id = vu.unload_gr_id) 
                                                ON sd.unload_day_id = gu.unload_day_id) 
                                                ON r.code = sd.region_id) 
                                                ON f.fma_id = sd.fma) 
                                                ON fg.fishing_ground_code = sd.ground_id) 
                                                LEFT JOIN nsap_odk.taxa as t ON vc.taxa = t.taxa_code)
                                            ON
                                               g.gear_code = gu.gr_id
                                            WHERE
                                               vc.species_id Is Null
                                              AND
                                               char_length(vc.species_text)>0
                                            ORDER BY
                                               vc.species_text";
                        con.Open();
                        MySqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            string spName = dr["species_text"].ToString().Trim(new char[] { ' ', '\n' });
                            bool proceed = false;
                            if (multiline && spName.Contains("\n"))
                            {
                                proceed = true;
                            }
                            else if (!multiline && !spName.Contains("\n"))
                            {
                                proceed = true;
                            }
                            if (proceed)
                            {
                                int? lsid = null;
                                if (dr["land_ctr_id"] != DBNull.Value)
                                {
                                    lsid = (int)dr["land_ctr_id"];
                                }

                                OrphanedSpeciesNameRaw osnr = new OrphanedSpeciesNameRaw
                                {
                                    RegionName = dr["short_name"].ToString(),
                                    FMAName = dr["fma_name"].ToString(),
                                    FishingGroundName = dr["fishing_ground_name"].ToString(),
                                    LandingSiteID = lsid,
                                    LandingSiteName = dr["land_ctr_text"].ToString(),
                                    EnumeratorName = dr["enumerator_name"].ToString(),
                                    EnumeratorText = dr["enumerator_text"].ToString(),
                                    GearName = dr["gear_name"].ToString(),
                                    GearText = dr["gr_text"].ToString(),
                                    Taxa = dr["taxa"].ToString(),
                                    VesselUnloadID = (int)dr["v_unload_id"],
                                    OrphanedSpName = dr["species_text"].ToString().Trim(new char[] { ' ', '\n' })
                                };

                                StringBuilder sb = new StringBuilder(osnr.RegionName);
                                sb.Append(osnr.FMAName);
                                sb.Append(osnr.FishingGroundName);
                                sb.Append(osnr.OrphanedSpName);
                                sb.Append(osnr.Taxa);

                                osnr.HashCode = sb.ToString().GetHashCode();
                                this_list.Add(osnr);
                            }
                        }
                    }
                }
            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT
                                                    nsapRegion.ShortName                ,
                                                    fma.FMAName                         ,
                                                    fishingGround.FishingGroundName     ,
                                                    NSAPEnumerator.EnumeratorName       ,
                                                    dbo_LC_FG_sample_day.land_ctr_id    ,
                                                    dbo_LC_FG_sample_day.land_ctr_text  ,
                                                    dbo_vessel_unload_1.EnumeratorText  ,
                                                    taxa.taxa                           ,
                                                    dbo_vessel_catch.species_text       ,
                                                    gear.GearName                       ,
                                                    dbo_gear_unload.gr_text             ,
                                                    dbo_vessel_unload_1.v_unload_id
                                            FROM
                                                    gear
                                            RIGHT JOIN
                                                    ((fishingGround INNER JOIN (fma INNER JOIN (nsapRegion INNER JOIN (dbo_LC_FG_sample_day INNER JOIN (dbo_gear_unload 
                                                            INNER JOIN (NSAPEnumerator RIGHT JOIN ((dbo_vessel_unload INNER JOIN dbo_vessel_catch 
                                                            ON dbo_vessel_unload.v_unload_id = dbo_vessel_catch.v_unload_id) INNER JOIN dbo_vessel_unload_1 
                                                            ON dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id) ON NSAPEnumerator.EnumeratorID = dbo_vessel_unload_1.EnumeratorID) 
                                                            ON dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) 
                                                            ON nsapRegion.Code = dbo_LC_FG_sample_day.region_id) ON fma.FMAID = dbo_LC_FG_sample_day.fma) ON fishingGround.FishingGroundCode = dbo_LC_FG_sample_day.ground_id) 
                                                            LEFT JOIN taxa ON dbo_vessel_catch.taxa = taxa.TaxaCode)
                                            ON
                                                    gear.GearCode = dbo_gear_unload.gr_id
                                            WHERE
                                                    dbo_vessel_catch.species_id Is Null
                                                    AND
                                                    Len([species_text])>0
                                            ORDER BY
                                                    dbo_vessel_catch.species_text";


                        con.Open();
                        OleDbDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            string spName = dr["species_text"].ToString().Trim(new char[] { ' ', '\n' });
                            bool proceed = false;
                            if (multiline && spName.Contains("\n"))
                            {
                                proceed = true;
                            }
                            else if (!multiline && !spName.Contains("\n"))
                            {
                                proceed = true;
                            }

                            if (proceed)
                            {
                                int? lsid = null;
                                if (dr["land_ctr_id"] != DBNull.Value)
                                {
                                    lsid = (int)dr["land_ctr_id"];
                                }

                                OrphanedSpeciesNameRaw osnr = new OrphanedSpeciesNameRaw
                                {
                                    RegionName = dr["ShortName"].ToString(),
                                    FMAName = dr["FMAName"].ToString(),
                                    FishingGroundName = dr["FishingGroundName"].ToString(),
                                    LandingSiteID = lsid,
                                    LandingSiteName = dr["land_ctr_text"].ToString(),
                                    EnumeratorName = dr["EnumeratorName"].ToString(),
                                    EnumeratorText = dr["EnumeratorText"].ToString(),
                                    GearName = dr["GearName"].ToString(),
                                    GearText = dr["gr_text"].ToString(),
                                    Taxa = dr["taxa"].ToString(),
                                    VesselUnloadID = (int)dr["v_unload_id"],
                                    OrphanedSpName = spName
                                };

                                StringBuilder sb = new StringBuilder(osnr.RegionName);
                                sb.Append(osnr.FMAName);
                                sb.Append(osnr.FishingGroundName);
                                sb.Append(osnr.OrphanedSpName);
                                sb.Append(osnr.Taxa);

                                osnr.HashCode = sb.ToString().GetHashCode();
                                this_list.Add(osnr);
                            }
                        }
                    }
                }
            }
            return this_list;
        }

        public static int CountOfLandingsWithOrphanedSpName()
        {
            int rows = 0;
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT DISTINCT dbo_vessel_catch.v_unload_id
                                            FROM dbo_vessel_catch
                                            WHERE dbo_vessel_catch.species_id Is Null AND Len([species_text]) > 0";

                        con.Open();
                        OleDbDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            rows++;
                        }


                    }
                }
            }
            return rows;
        }
        private List<VesselCatch> getFromMySQL(VesselUnload vu = null)
        {
            List<VesselCatch> thisList = new List<VesselCatch>();
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = "Select * from dbo_vessel_catch";
                    if (vu != null)
                    {
                        cmd.Parameters.AddWithValue("@parentID", vu.PK);
                        cmd.CommandText = "Select * from dbo_vessel_catch where v_unload_id=@parentID";
                    }

                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        VesselCatch item = new VesselCatch();
                        item.Parent = vu;
                        item.PK = (int)dr["catch_id"];
                        item.VesselUnloadID = (int)dr["v_unload_id"];
                        item.SpeciesID = string.IsNullOrEmpty(dr["species_id"].ToString()) ? null : (int?)dr["species_id"];
                        item.Catch_kg = string.IsNullOrEmpty(dr["catch_kg"].ToString()) ? null : (double?)dr["catch_kg"];
                        item.Sample_kg = string.IsNullOrEmpty(dr["samp_kg"].ToString()) ? null : (double?)dr["samp_kg"];
                        //item.TWS = string.IsNullOrEmpty(dr["tws"].ToString()) ? null : (double?)dr["tws"];
                        item.Sample_kg = string.IsNullOrEmpty(dr["tws"].ToString()) ? null : (double?)dr["tws"];
                        item.TaxaCode = dr["taxa"].ToString();
                        item.SpeciesText = dr["species_text"].ToString();
                        item.CatchLenFreqViewModel = new CatchLenFreqViewModel(item);
                        item.CatchLengthViewModel = new CatchLengthViewModel(item);
                        item.CatchLengthWeightViewModel = new CatchLengthWeightViewModel(item);
                        item.CatchMaturityViewModel = new CatchMaturityViewModel(item);
                        thisList.Add(item);
                    }
                }
            }
            return thisList;
        }

        public static List<VesselCatchWV> GetVesselCatchForWV()
        {
            List<VesselCatchWV> thisList = new List<VesselCatchWV>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = conection.CreateCommand())
                    {
                        cmd.CommandText = "Select catch_id, v_unload_id, catch_kg, samp_kg, from_total_catch from dbo_vessel_catch order by v_unload_id";
                        try
                        {
                            conection.Open();
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                VesselCatchWV vcwv = new VesselCatchWV
                                {
                                    PK = (int)dr["catch_id"],
                                    VesselUnloadID = (int)dr["v_unload_id"],
                                    Species_kg = dr["catch_kg"] == DBNull.Value ? null : (double?)dr["catch_kg"],
                                    Species_sample_kg = dr["samp_kg"] == DBNull.Value ? null : (double?)dr["samp_kg"],
                                    FromTotalCatch = (bool)dr["from_total_catch"]
                                };
                                thisList.Add(vcwv);
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

        private List<VesselCatch> getVesselCatches(CarrierLanding cl)
        {
            List<VesselCatch> thisList = new List<VesselCatch>();
            if(Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@id", cl.RowID);
                        cmd.CommandText = "Select * from dbo_vessel_catch where carrierlanding_id=@id";
                        con.Open();
                        OleDbDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {

                            VesselCatch item = new VesselCatch();
                            item.ParentCarrierLanding = cl;
                            item.PK = (int)dr["catch_id"];
                            item.SpeciesID = string.IsNullOrEmpty(dr["species_id"].ToString()) ? null : (int?)dr["species_id"];
                            item.Catch_kg = string.IsNullOrEmpty(dr["catch_kg"].ToString()) ? null : (double?)dr["catch_kg"];
                            item.Sample_kg = string.IsNullOrEmpty(dr["samp_kg"].ToString()) ? null : (double?)dr["samp_kg"];
                            item.TaxaCode = dr["taxa"].ToString();
                            item.SpeciesText = dr["species_text"].ToString();
                            item.CatchLenFreqViewModel = new CatchLenFreqViewModel(item);
                            item.CatchLengthViewModel = new CatchLengthViewModel(item);
                            item.CatchLengthWeightViewModel = new CatchLengthWeightViewModel(item);
                            item.CatchMaturityViewModel = new CatchMaturityViewModel(item);
                            item.WeighingUnit = dr["weighing_unit"].ToString();
                            thisList.Add(item);

                        }
                    }
                }
            }
            return thisList;
        }
        private List<VesselCatch> getVesselCatches(VesselUnload_FishingGear vufg)
        {
            List<VesselCatch> thisList = new List<VesselCatch>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@id", vufg.RowID);
                        cmd.CommandText = "Select * from dbo_vessel_catch where vessel_unload_gear_id=@id";
                        con.Open();
                        OleDbDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {

                            VesselCatch item = new VesselCatch();
                            item.ParentFishingGear = vufg;
                            item.PK = (int)dr["catch_id"];
                            item.VesselUnloadID = vufg.Parent.PK;
                            item.SpeciesID = string.IsNullOrEmpty(dr["species_id"].ToString()) ? null : (int?)dr["species_id"];
                            item.Catch_kg = string.IsNullOrEmpty(dr["catch_kg"].ToString()) ? null : (double?)dr["catch_kg"];
                            item.Sample_kg = string.IsNullOrEmpty(dr["samp_kg"].ToString()) ? null : (double?)dr["samp_kg"];
                            //item.TWS = string.IsNullOrEmpty(dr["tws"].ToString()) ? null : (double?)dr["tws"];
                            item.TaxaCode = dr["taxa"].ToString();
                            item.SpeciesText = dr["species_text"].ToString();
                            item.CatchLenFreqViewModel = new CatchLenFreqViewModel(item);
                            item.CatchLengthViewModel = new CatchLengthViewModel(item);
                            item.CatchLengthWeightViewModel = new CatchLengthWeightViewModel(item);
                            item.CatchMaturityViewModel = new CatchMaturityViewModel(item);
                            item.WeighingUnit = dr["weighing_unit"].ToString();
                            item.FromTotalCatch = (bool)dr["from_total_catch"];
                            item.IsCatchSold = (bool)dr["is_catch_sold"];
                            item.PriceOfSpecies = string.IsNullOrEmpty(dr["price_of_species"].ToString()) ? null : (double?)dr["price_of_species"];
                            item.PriceUnit = dr["price_unit"].ToString();
                            item.OtherPriceUnit = dr["other_price_unit"].ToString();
                            item.GearCode = vufg.GearCode;
                            item.GearText = vufg.GearText;
                            thisList.Add(item);

                        }
                    }
                }
            }
            return thisList;
        }
        private List<VesselCatch> getVesselCatches(VesselUnload vu = null)
        {
            List<VesselCatch> thisList = new List<VesselCatch>();
            if (Global.Settings.UsemySQL)
            {
                thisList = getFromMySQL(vu);
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
                            cmd.CommandText = "Select * from dbo_vessel_catch";

                            if (vu != null)
                            {
                                cmd.Parameters.AddWithValue("@parentID", vu.PK);
                                cmd.CommandText = "Select * from dbo_vessel_catch where v_unload_id=@parentID";
                            }

                            thisList.Clear();
                            OleDbDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                VesselCatch item = new VesselCatch();
                                item.Parent = vu;
                                item.PK = (int)dr["catch_id"];
                                item.VesselUnloadID = (int)dr["v_unload_id"];
                                item.SpeciesID = string.IsNullOrEmpty(dr["species_id"].ToString()) ? null : (int?)dr["species_id"];
                                item.Catch_kg = string.IsNullOrEmpty(dr["catch_kg"].ToString()) ? null : (double?)dr["catch_kg"];
                                item.Sample_kg = string.IsNullOrEmpty(dr["samp_kg"].ToString()) ? null : (double?)dr["samp_kg"];
                                //item.TWS = string.IsNullOrEmpty(dr["tws"].ToString()) ? null : (double?)dr["tws"];
                                item.TaxaCode = dr["taxa"].ToString();
                                item.SpeciesText = dr["species_text"].ToString();
                                item.CatchLenFreqViewModel = new CatchLenFreqViewModel(item);
                                item.CatchLengthViewModel = new CatchLengthViewModel(item);
                                item.CatchLengthWeightViewModel = new CatchLengthWeightViewModel(item);
                                item.CatchMaturityViewModel = new CatchMaturityViewModel(item);
                                item.WeighingUnit = dr["weighing_unit"].ToString();
                                item.FromTotalCatch = (bool)dr["from_total_catch"];
                                item.IsCatchSold = (bool)dr["is_catch_sold"];
                                item.PriceOfSpecies = string.IsNullOrEmpty(dr["price_of_species"].ToString()) ? null : (double?)dr["price_of_species"];
                                item.PriceUnit = dr["price_unit"].ToString();
                                item.OtherPriceUnit = dr["other_price_unit"].ToString();
                                item.GearCode = string.IsNullOrEmpty(dr["gear_code"].ToString()) ? null : dr["gear_code"].ToString();
                                item.GearText = string.IsNullOrEmpty(dr["gear_text"].ToString()) ? null : dr["gear_text"].ToString();
                                thisList.Add(item);
                            }

                        }
                        catch (Exception ex)
                        {
                            if (ex.HResult == -2146233080)
                            {

                                _requireTableDefinitionUpdate = true;
                                _newColumnName = ex.Message;
                                return null;
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


        public static bool UpdateTableDefinition(string colName = "", bool removeIndex = false, string indexName = "")
        {
            bool success = false;
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = "";
                if (removeIndex)
                {
                    sql = $"DROP INDEX {indexName} on dbo_vessel_catch";
                }
                else
                {
                    switch (colName)
                    {
                        case "weighing_unit":
                            sql = $@"ALTER TABLE dbo_vessel_catch ADD COLUMN {colName} TEXT(2)";
                            break;
                        case "tws":

                            sql = $@"ALTER TABLE dbo_vessel_catch ADD COLUMN {colName} FLOAT";
                            break;
                        case "from_total_catch":
                            sql = $@"ALTER TABLE dbo_vessel_catch ADD COLUMN {colName} BIT";
                            break;
                        case "gear_code":
                            sql = $@"ALTER TABLE dbo_vessel_catch ADD COLUMN {colName} TEXT(8)";
                            break;
                        case "gear_text":
                            sql = $@"ALTER TABLE dbo_vessel_catch ADD COLUMN {colName} VarChar";
                            break;

                    }
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
                    if (dbex.Message.Contains("No such index"))
                    {
                        success = true;
                    }
                    else
                    {
                        Logger.Log(dbex);
                    }
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
        private bool AddToMySQL(VesselCatch item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@pk", MySqlDbType.Int32).Value = item.PK;

                    update.Parameters.Add("@parent_id", MySqlDbType.Int32).Value = item.VesselUnloadID;

                    if (item.SpeciesID == null)
                    {
                        update.Parameters.Add("@species_id", MySqlDbType.Int32).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@species_id", MySqlDbType.Int32).Value = item.SpeciesID;
                    }

                    if (item.Catch_kg == null)
                    {
                        update.Parameters.Add("@catch_kg", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@catch_kg", MySqlDbType.Double).Value = item.Catch_kg;
                    }

                    if (item.Sample_kg == null)
                    {
                        update.Parameters.Add("@sample_kg", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@sample_kg", MySqlDbType.Double).Value = item.Sample_kg;
                    }

                    update.Parameters.Add("@taxa", MySqlDbType.VarChar).Value = item.TaxaCode;

                    if (item.SpeciesText == null)
                    {
                        update.Parameters.Add("@species_text", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@species_text", MySqlDbType.VarChar).Value = item.SpeciesText;
                    }


                    //if (item.TWS == null)
                    //{
                    //    update.Parameters.Add("@tws", MySqlDbType.VarChar).Value = DBNull.Value;
                    //}
                    //else
                    //{
                    //    update.Parameters.Add("@tws", MySqlDbType.VarChar).Value = item.TWS;
                    //}

                    update.CommandText = @"Insert into dbo_vessel_catch(catch_id, v_unload_id, species_id, catch_kg, samp_kg, taxa, species_text)
                            Values (@pk, @parent_id, @species_id, @catch_kg, @sample_kg, @taxa, @species_text)";
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
        public bool Add(VesselCatch item)
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

                    var sql = @"Insert into dbo_vessel_catch (
                                catch_id, 
                                v_unload_id, 
                                species_id, 
                                catch_kg, 
                                samp_kg, 
                                taxa, 
                                species_text, 
                                weighing_unit,
                                from_total_catch,
                                price_of_species,
                                price_unit,
                                other_price_unit,
                                is_catch_sold,
                                gear_code,
                                gear_text,
                                vessel_unload_gear_id,
                                carrierlanding_id )
                            Values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@pk", OleDbType.Integer).Value = item.PK;
                        if (item.VesselUnloadID == null)
                        {
                            update.Parameters.Add("@parent_id", OleDbType.Integer).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@parent_id", OleDbType.Integer).Value = item.VesselUnloadID;
                        }
                        if (item.SpeciesID == null)
                        {
                            update.Parameters.Add("@species_id", OleDbType.Integer).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@species_id", OleDbType.Integer).Value = item.SpeciesID;
                        }

                        if (item.Catch_kg == null)
                        {
                            update.Parameters.Add("@catch_kg", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@catch_kg", OleDbType.Double).Value = item.Catch_kg;
                        }

                        if (item.Sample_kg == null)
                        {
                            update.Parameters.Add("@sample_kg", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@sample_kg", OleDbType.Double).Value = item.Sample_kg;
                        }
                        update.Parameters.Add("@taxa", OleDbType.VarChar).Value = item.TaxaCode;
                        if (item.SpeciesText == null)
                        {
                            update.Parameters.Add("@species_text", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@species_text", OleDbType.VarChar).Value = item.SpeciesText;
                        }
                        if (item.WeighingUnit == null)
                        {
                               update.Parameters.Add("@wt_unit", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@wt_unit", OleDbType.VarChar).Value = item.WeighingUnit;
                        }

                        update.Parameters.Add("@from_total", OleDbType.Boolean).Value = item.FromTotalCatch;

                        if (item.PriceOfSpecies == null)
                        {
                            update.Parameters.Add("@price", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@price", OleDbType.Double).Value = item.PriceOfSpecies;
                        }
                        if (item.PriceUnit == null)
                        {
                               update.Parameters.Add("@price_unit", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@price_unit", OleDbType.VarChar).Value = item.PriceUnit;
                        }
                        if (item.OtherPriceUnit == null)
                        {
                               update.Parameters.Add("@other_price_unit", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@other_price_unit", OleDbType.VarChar).Value = item.OtherPriceUnit;
                        }
                        update.Parameters.Add("@is_sold", OleDbType.Boolean).Value = item.IsCatchSold;
                        if(item.GearCode==null)
                        {
                               update.Parameters.Add("@gear_code", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                               update.Parameters.Add("@gear_code", OleDbType.VarChar).Value = item.GearCode;
                        }
                        if(item.GearText==null)
                        {
                            update.Parameters.Add("@gear_text", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@gear_text", OleDbType.VarChar).Value = item.GearText;
                        }
                        if(item.ParentFishingGear==null)
                        {
                            update.Parameters.Add("@parent_gear", OleDbType.Integer).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@parent_gear", OleDbType.Integer).Value = item.ParentFishingGear.RowID;
                        }
                        if(item.ParentCarrierLanding==null)
                        {
                            update.Parameters.Add("@parent_carrier", OleDbType.Integer).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@parent_carrier", OleDbType.Integer).Value = item.ParentCarrierLanding.RowID;
                        }

                        //if(item.TWS==null)
                        //{
                        //    update.Parameters.Add("@tws", OleDbType.VarChar).Value = DBNull.Value;
                        //}
                        //else
                        //{
                        //    update.Parameters.Add("@tws", OleDbType.VarChar).Value = item.TWS;
                        //}

                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException dbex)
                        {
                            //Console.WriteLine($"item pk is {item.PK}");
                            switch (dbex.ErrorCode)
                            {
                                case -2147467259:
                                    //error because of duplicated key or index
                                    break;
                                default:
                                    Logger.Log(dbex);
                                    break;
                            }
                            //Logger.Log(dbex);
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
        private bool UpdateMySQL(VesselCatch item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Parameters.Add("@v_unload_id", MySqlDbType.Int32).Value = item.Parent.PK;

                    if (item.SpeciesID == null)
                    {
                        cmd.Parameters.Add("@species_id", MySqlDbType.Int32).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@species_id", MySqlDbType.Int32).Value = item.SpeciesID;
                    }

                    if (item.Catch_kg == null)
                    {
                        cmd.Parameters.Add("@catch_kg", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@catch_kg", MySqlDbType.Double).Value = (double)item.Catch_kg;
                    }

                    if (item.Sample_kg == null)
                    {
                        cmd.Parameters.Add("@sample_kg", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@sample_kg", MySqlDbType.Double).Value = (double)item.Sample_kg;
                    }

                    //if (item.TWS == null)
                    //{
                    //    cmd.Parameters.Add("@tws", MySqlDbType.Double).Value = DBNull.Value;
                    //}
                    //else
                    //{
                    //    cmd.Parameters.Add("@tws", MySqlDbType.Double).Value = (double)item.TWS;
                    //}

                    cmd.Parameters.Add("@taxa", MySqlDbType.VarChar).Value = item.TaxaCode;
                    cmd.Parameters.Add("@species_text", MySqlDbType.VarChar).Value = item.SpeciesText;
                    cmd.Parameters.Add("@catch_id", MySqlDbType.Int32).Value = item.PK;

                    cmd.CommandText = @"Update dbo_vessel_catch set
                                v_unload_id=@v_unload_id,
                                species_id = @species_id,
                                catch_kg = @catch_kg,
                                samp_kg = @sample_kg,
                                taxa = @taxa,
                                species_text = @species_text
                            WHERE catch_id = @catch_id";

                    try
                    {
                        conn.Open();
                        success = cmd.ExecuteNonQuery() > 0;
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
        public bool Update(VesselCatch item)
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
                    using (OleDbCommand cmd = conn.CreateCommand())
                    {
                        if (item.Parent != null)
                        {
                            cmd.Parameters.Add("@v_unload_id", OleDbType.Integer).Value = item.Parent.PK;
                        }
                        else
                        {
                            cmd.Parameters.Add("@v_unload_id", OleDbType.Integer).Value = DBNull.Value;
                        }

                        if (item.SpeciesID == null)
                        {
                            cmd.Parameters.Add("@species_id", OleDbType.Integer).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@species_id", OleDbType.Integer).Value = item.SpeciesID;
                        }

                        if (item.Catch_kg == null)
                        {
                            cmd.Parameters.Add("@catch_kg", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@catch_kg", OleDbType.Double).Value = (double)item.Catch_kg;
                        }

                        if (item.Sample_kg == null)
                        {
                            cmd.Parameters.Add("@sample_kg", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@sample_kg", OleDbType.Double).Value = (double)item.Sample_kg;
                        }
                        //if (item.TWS == null)
                        //{
                        //    cmd.Parameters.Add("@tws", OleDbType.Double).Value = DBNull.Value;
                        //}
                        //else
                        //{
                        //    cmd.Parameters.Add("@tws", OleDbType.Double).Value = (double)item.TWS;
                        //}

                        cmd.Parameters.Add("@taxa", OleDbType.VarChar).Value = item.TaxaCode;
                        cmd.Parameters.Add("@species_text", OleDbType.VarChar).Value = item.SpeciesText;
                        if (item.WeighingUnit == null)
                        {
                               cmd.Parameters.Add("@wt_unit", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@wt_unit", OleDbType.VarChar).Value = item.WeighingUnit;
                        }
                        cmd.Parameters.Add("@from_total", OleDbType.Boolean).Value = item.FromTotalCatch;

                        if (item.PriceOfSpecies == null)
                        {
                            cmd.Parameters.Add("@price", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@price", OleDbType.Double).Value = item.PriceOfSpecies;
                        }

                        if (item.PriceUnit == null)
                        {
                               cmd.Parameters.Add("@price_unit", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@price_unit", OleDbType.VarChar).Value = item.PriceUnit;
                        }
                        if (item.OtherPriceUnit == null)
                        {
                            cmd.Parameters.Add("@other_price_unit", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@other_price_unit", OleDbType.VarChar).Value = item.OtherPriceUnit;
                        }
                        cmd.Parameters.Add("@is_sold", OleDbType.Boolean).Value = item.IsCatchSold;

                        if(item.ParentFishingGear==null)
                        {
                            cmd.Parameters.Add("@parent_gear", OleDbType.Integer).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@parent_gear", OleDbType.Integer).Value = item.ParentFishingGear.RowID;
                        }
                        if(item.ParentCarrierLanding==null)
                        {
                            cmd.Parameters.Add("@parent_carrier", OleDbType.Integer).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@parent_carrier", OleDbType.Integer).Value = item.ParentCarrierLanding.RowID;
                        }

                        cmd.Parameters.Add("@catch_id", OleDbType.Integer).Value = item.PK;
                        

                        cmd.CommandText = @"Update dbo_vessel_catch set
                                v_unload_id=@v_unload_id,
                                species_id = @species_id,
                                catch_kg = @catch_kg,
                                samp_kg = @sample_kg,
                                taxa = @taxa,
                                species_text = @species_text,
                                weighing_unit = @wt_unit,
                                from_total_catch = @from_total,
                                price_of_species = @price,
                                price_unit = @price_unit,
                                other_price_unit = @other_price_unit,
                                is_catch_sold = @is_sold,
                                vessel_unload_gear_id = @parent_gear,
                                carrierlanding_id = @parent_carrier
                            WHERE catch_id = @catch_id";
                        try
                        {
                            var ressultCount = cmd.ExecuteNonQuery();
                            success = ressultCount > 0;
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

        public static bool AddFieldToTable(string fieldName)
        {
            bool success = false;
            string sql = "";
            switch (fieldName)
            {
                case "carrierlanding_id":
                case "vessel_unload_gear_id":
                    sql = $"ALTER TABLE dbo_vessel_catch ADD COLUMN {fieldName} int";
                    break;
                case "is_catch_sold":
                    sql = "ALTER TABLE dbo_vessel_catch ADD COLUMN is_catch_sold bit";
                    break;
                case "other_price_unit":
                    sql = "ALTER TABLE dbo_vessel_catch ADD COLUMN other_price_unit varchar(100)";
                    break;
                case "price_of_species":
                    sql = "ALTER TABLE dbo_vessel_catch ADD COLUMN price_of_species double";
                    break;
                case "price_unit":
                    sql = "ALTER TABLE dbo_vessel_catch ADD COLUMN price_unit text(10)";
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
                        if (success)
                        {
                            if (fieldName == "vessel_unload_gear_id")
                            {
                                sql = @"ALTER TABLE dbo_vessel_catch 
                                    ADD CONSTRAINT unload_gear_FK FOREIGN KEY (vessel_unload_gear_id) 
                                    REFERENCES dbo_vesselunload_fishinggear (row_id)";
                            }
                            else if (fieldName == "carrierlanding_id")
                            {
                                sql = @"ALTER TABLE dbo_vessel_catch 
                                    ADD CONSTRAINT carrierlanding_FK FOREIGN KEY (carrierlanding_id) 
                                    REFERENCES dbo_carrier_landing (row_id)";
                            }
                            using (var cmd_2 = con.CreateCommand())
                            {
                                cmd_2.CommandText = sql;
                                try
                                {
                                    cmd_2.ExecuteNonQuery();
                                    success = true;
                                }
                                catch (Exception ex)
                                {
                                    Logger.Log(ex);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
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
                var sql = $"Delete * from dbo_vessel_catch";
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
                    update.CommandText = "Delete  from dbo_vessel_catch where catch_id=@id";
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
                        update.CommandText = "Delete * from dbo_vessel_catch where catch_id=@id";
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
