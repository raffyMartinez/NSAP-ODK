using MySql.Data.MySqlClient;
using NSAP_ODK.NSAPMysql;
using NSAP_ODK.Utilities;
using System;
using System.Data;
using System.Data.OleDb;
using System.Linq;

namespace NSAP_ODK.Entities
{
    public class NSAPRegionWithEntitiesRepository
    {
        private string _dateFormat = "MMM-dd-yyyy";
        public NSAPRegion NSAPRegion { get; private set; }

        public string DatabaseErrorMessage { get; internal set; }
        public NSAPRegionWithEntitiesRepository() { }
        public NSAPRegionWithEntitiesRepository(NSAPRegion nsapRegion)
        {
            NSAPRegion = nsapRegion;

            GetFMAS();
            foreach (NSAPRegionFMA regionFMA in NSAPRegion.FMAs)
            {
                GetFishingGroundsInFMAs(regionFMA);
                foreach (var item in regionFMA.FishingGrounds)
                {
                    GetLandingSitesInFMAFishingGrounds(item);
                }
            }

            GetEnumerators();
            GetFishingVessels();
            GetGears();
        }

        public static FishingGround GetFishingGround(string region, string fma, string fishing_ground)
        {
            FishingGround fg = null;
            if (Global.Settings.UsemySQL)
            {
                using (var con = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@regionName", region);
                        cmd.Parameters.AddWithValue("@fmaName", fma);
                        cmd.Parameters.AddWithValue("@fgName", fishing_ground);

                        cmd.CommandText = @"SELECT fishing_grounds.fishing_ground_code
                                            FROM fma INNER JOIN 
                                                (fishing_grounds INNER JOIN 
                                                (nsap_region INNER JOIN 
                                                (nsap_region_fma INNER JOIN 
                                                nsap_region_fma_fishing_grounds ON 
                                                    nsap_region_fma.row_id = nsap_region_fma_fishing_grounds.region_fma) ON 
                                                    nsap_region.code = nsap_region_fma.nsap_region) ON 
                                                    fishing_grounds.fishing_ground_code = nsap_region_fma_fishing_grounds.fishing_ground) ON 
                                                    fma.fma_id = nsap_region_fma.fma
                                            WHERE nsap_region.short_name= @regionName AND
                                                  fma.fma_name=@fmaName AND 
                                                  fishing_grounds.fishing_ground_name=@fgName";
                        try
                        {
                            con.Open();
                            string fg_code = cmd.ExecuteScalar().ToString();
                            fg = NSAPEntities.FishingGroundViewModel.GetFishingGround(fg_code);
                        }
                        catch (OleDbException oex)
                        {

                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
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
                        cmd.Parameters.AddWithValue("@regionName", region);
                        cmd.Parameters.AddWithValue("@fmaName", fma);
                        cmd.Parameters.AddWithValue("@fgName", fishing_ground);

                        cmd.CommandText = @"SELECT fishingGround.FishingGroundCode
                                            FROM fma INNER JOIN 
                                                (fishingGround INNER JOIN 
                                                (nsapRegion INNER JOIN 
                                                (NSAPRegionFMA INNER JOIN 
                                                NSAPRegionFMAFishingGrounds ON 
                                                    NSAPRegionFMA.RowID = NSAPRegionFMAFishingGrounds.RegionFMA) ON 
                                                    nsapRegion.Code = NSAPRegionFMA.NSAPRegion) ON 
                                                    fishingGround.FishingGroundCode = NSAPRegionFMAFishingGrounds.FishingGround) ON 
                                                    fma.FMAID = NSAPRegionFMA.FMA
                                            WHERE nsapRegion.ShortName= @regionName AND
                                                  fma.FMAName=@fmaName AND 
                                                  fishingGround.FishingGroundName=@fgName";

                        try
                        {
                            con.Open();
                            fg = NSAPEntities.FishingGroundViewModel.GetFishingGround(cmd.ExecuteScalar().ToString());
                        }
                        catch (OleDbException oex)
                        {

                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return fg;
        }

        /// <summary>
        /// Get the FMAs that are in an NSAP Region
        /// </summary>
        private void GetFMAS()
        {
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $"Select * from nsap_region_fma  where nsap_region='{NSAPRegion.Code}'";
                        conn.Open();
                        MySqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            NSAPRegionFMA nrf = new NSAPRegionFMA();
                            nrf.RowID = Convert.ToInt32(dr["row_id"]);
                            nrf.NSAPRegion = NSAPRegion;
                            nrf.FMA = NSAPEntities.FMAViewModel.GetFMA(Convert.ToInt32(dr["fma"]));
                            NSAPRegion.FMAs.Add(nrf);
                        }
                    }
                }
            }
            else
            {
                var dt = new DataTable();
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    try
                    {
                        conection.Open();
                        string query = $"Select * from NSAPRegionFMA  where NSAPRegion='{NSAPRegion.Code}'";

                        var adapter = new OleDbDataAdapter(query, conection);
                        adapter.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                NSAPRegionFMA nrf = new NSAPRegionFMA();
                                nrf.RowID = Convert.ToInt32(dr["RowID"]);
                                nrf.NSAPRegion = NSAPRegion;
                                nrf.FMA = NSAPEntities.FMAViewModel.GetFMA(Convert.ToInt32(dr["FMA"]));
                                NSAPRegion.FMAs.Add(nrf);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
        }

        public EntityValidationResult ValidateFishingGroundLandingSite(NSAPRegionFMAFishingGroundLandingSite landingSite, bool isNew, NSAPRegionFMAFishingGroundLandingSite oldLandingSite = null)
        {
            EntityValidationResult evr = new EntityValidationResult();
            if (landingSite.NSAPRegionFMAFishingGround.LandingSites.Count > 0)
            {
                foreach (var ls in landingSite.NSAPRegionFMAFishingGround.LandingSites)
                {
                    if (ls.LandingSite.LandingSiteID == landingSite.LandingSite.LandingSiteID)
                    {
                        if (isNew || landingSite.LandingSite.LandingSiteID != oldLandingSite.LandingSite.LandingSiteID)
                        {
                            evr.AddMessage("Landing site already listed in the region");
                        }
                        break;
                    }
                }
            }
            if (landingSite.DateStart > DateTime.Now)
            {
                evr.AddMessage("Date added cannot be a future date");
            }
            return evr;
        }

        public EntityValidationResult ValidateNSAPEnumerator(NSAPRegionEnumerator enumerator, bool isNew, NSAPRegionEnumerator oldENumerator = null)
        {
            EntityValidationMessage msg = null;
            EntityValidationResult evr = new EntityValidationResult();
            if (enumerator.NSAPRegion.NSAPEnumerators.Count > 0)
            {
                foreach (var e in enumerator.NSAPRegion.NSAPEnumerators)
                {
                    if (e.EnumeratorID == enumerator.EnumeratorID)
                    {
                        if (isNew || enumerator.EnumeratorID != oldENumerator.EnumeratorID)
                        {
                            msg = new EntityValidationMessage("Enumerator already listed in the region");
                            evr.AddMessage(msg);
                        }
                        break;
                    }
                }
            }
            if (enumerator.DateStart > DateTime.Now)
            {
                msg = new EntityValidationMessage("Date added cannot be a future date");
                evr.AddMessage(msg);
            }
            return evr;
        }

        public EntityValidationResult ValidateNSAPRegionFMAFishingGround(NSAPRegionFMAFishingGround fishingGround, bool isNew, NSAPRegionFMAFishingGround oldFishingGround = null)
        {
            EntityValidationMessage msg = null;
            EntityValidationResult evr = new EntityValidationResult();
            if (fishingGround.RegionFMA.FishingGrounds.Count > 0)
            {
                foreach (var fg in fishingGround.RegionFMA.FishingGrounds)
                {
                    if (fg.FishingGroundCode == fishingGround.FishingGroundCode)
                    {
                        if (isNew || fishingGround.FishingGroundCode.ToUpper() != oldFishingGround.FishingGround.Code.ToUpper())
                        {
                            msg = new EntityValidationMessage("Fishing ground already listed in the FMA");
                            evr.AddMessage(msg);
                        }

                        break;
                    }
                }
            }
            if (fishingGround.DateStart > DateTime.Now)
            {
                msg = new EntityValidationMessage("Date added cannot be a future date");
                evr.AddMessage(msg);
            }
            return evr;
        }

        public EntityValidationResult ValidateNSAPRegionFishingVessel(NSAPRegionFishingVessel vessel, bool isNew, NSAPRegionFishingVessel oldVessel = null)
        {
            EntityValidationMessage msg = null;
            EntityValidationResult evr = new EntityValidationResult();
            if (vessel.NSAPRegion.FishingVessels.Count > 0)
            {
                foreach (var v in vessel.NSAPRegion.FishingVessels)
                {
                    if (v.FishingVesselID == vessel.FishingVesselID)
                    {
                        if (isNew || vessel.FishingVesselID != oldVessel.FishingVesselID)
                        {
                            msg = new EntityValidationMessage("Fishing vessel already listed in the region");
                            evr.AddMessage(msg);
                        }
                        break;
                    }
                }
            }
            if (vessel.DateStart > DateTime.Now)
            {
                msg = new EntityValidationMessage("Date added cannot be a future date");
                evr.AddMessage(msg);
            }
            return evr;
        }

        public EntityValidationResult ValidateNSAPRegionGear(NSAPRegionGear gear, bool isNew, NSAPRegionGear oldGear = null)
        {
            EntityValidationMessage msg = null;
            EntityValidationResult evr = new EntityValidationResult();
            if (gear.NSAPRegion.Gears.Count > 0)
            {
                foreach (var g in gear.NSAPRegion.Gears)
                {
                    if (g.GearCode.ToUpper() == gear.GearCode.ToUpper())
                    {
                        if (isNew || gear.GearCode != oldGear.GearCode)
                        {
                            msg = new EntityValidationMessage("Gear already listed in the region");
                            evr.AddMessage(msg);
                        }
                        break;
                    }
                }
            }
            if (gear.DateStart > DateTime.Now)
            {
                msg = new EntityValidationMessage("Date added cannot be a future date");
                evr.AddMessage(msg);
            }
            return evr;
        }

        private void GetFishingGroundsInFMAs(NSAPRegionFMA regionFMA)
        {
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $"Select * from nsap_region_fma_fishing_grounds where region_fma={regionFMA.RowID}";
                        conn.Open();
                        MySqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            NSAPRegionFMAFishingGround nrfg = new NSAPRegionFMAFishingGround();
                            nrfg.RowID = Convert.ToInt32(dr["row_id"]);
                            nrfg.RegionFMA = regionFMA;
                            nrfg.DateStart = (DateTime)dr["date_start"];
                            if (DateTime.TryParse(dr["date_end"].ToString(), out DateTime dte))
                            {
                                nrfg.DateEnd = dte;
                            }
                            nrfg.FishingGround = NSAPEntities.FishingGroundViewModel.GetFishingGround(dr["fishing_ground"].ToString());
                            regionFMA.FishingGrounds.Add(nrfg);
                        }
                    }
                }
            }
            else
            {
                var dt = new DataTable();
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    try
                    {
                        conection.Open();
                        string query = $"Select * from NSAPRegionFMAFishingGrounds where RegionFMA={regionFMA.RowID}";

                        var adapter = new OleDbDataAdapter(query, conection);
                        adapter.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                NSAPRegionFMAFishingGround nrfg = new NSAPRegionFMAFishingGround();
                                nrfg.RowID = Convert.ToInt32(dr["RowID"]);
                                nrfg.RegionFMA = regionFMA;
                                nrfg.DateStart = (DateTime)dr["DateStart"];
                                if (DateTime.TryParse(dr["DateEnd"].ToString(), out DateTime dte))
                                {
                                    nrfg.DateEnd = dte;
                                }
                                nrfg.FishingGround = NSAPEntities.FishingGroundViewModel.GetFishingGround(dr["FishingGround"].ToString());
                                regionFMA.FishingGrounds.Add(nrfg);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
        }

        private void GetEnumerators()
        {
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $"Select * from nsap_region_enumerator where region='{NSAPRegion.Code}'";
                        conn.Open();
                        MySqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            NSAPRegionEnumerator nre = new NSAPRegionEnumerator();
                            nre.RowID = Convert.ToInt32(dr["row_id"]);
                            nre.NSAPRegion = NSAPRegion;
                            nre.Enumerator = NSAPEntities.NSAPEnumeratorViewModel.GetNSAPEnumerator(Convert.ToInt32(dr["enumerator_id"]));
                            nre.DateStart = (DateTime)dr["date_start"];
                            if (DateTime.TryParse(dr["date_end"].ToString(), out DateTime v))
                            {
                                nre.DateEnd = v;
                            }
                            NSAPRegion.NSAPEnumerators.Add(nre);
                        }
                    }
                }
            }
            else
            {
                var dt = new DataTable();
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    try
                    {
                        conection.Open();
                        string query = $"Select * from NSAPRegionEnumerator where NSAPRegionCode='{NSAPRegion.Code}'";

                        var adapter = new OleDbDataAdapter(query, conection);
                        adapter.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                NSAPRegionEnumerator nre = new NSAPRegionEnumerator();
                                nre.RowID = Convert.ToInt32(dr["RowID"]);
                                nre.NSAPRegion = NSAPRegion;
                                nre.Enumerator = NSAPEntities.NSAPEnumeratorViewModel.GetNSAPEnumerator(Convert.ToInt32(dr["EnumeratorID"]));
                                nre.DateStart = (DateTime)dr["DateStart"];

                                if (dr["DateFirstSampling"] == DBNull.Value)
                                {
                                    nre.DateFirstSampling = null;
                                }
                                else
                                {
                                    nre.DateFirstSampling = (DateTime)dr["DateFirstUpload"];
                                }

                                if (DateTime.TryParse(dr["DateEnd"].ToString(), out DateTime v))
                                {
                                    nre.DateEnd = v;
                                }
                                NSAPRegion.NSAPEnumerators.Add(nre);
                            }
                        }
                    }
                    catch (OleDbException oex)
                    {

                    }
                    catch (Exception ex)
                    {
                        if (ex.HResult == -2147024809) //column does not exist
                        {
                            var arr = ex.Message.Split(' ');
                            if (AddColumnToTable(arr[1], "NSAPRegionEnumerator"))
                            {
                                //UpdateEnumeratorFirstUploadDate();
                                EnumeratorFirstSamplingDateRequired = true;
                                GetEnumerators();
                            }
                        }
                        else
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
        }

        public static bool EnumeratorFirstSamplingDateRequired { get; set; }

        public bool RefreshNSAPRegionEntities(NSAPRegion region)
        {
            NSAPRegion = region;
            GetGears();
            GetEnumerators();
            GetFMAS();
            return true;
        }
        private bool AddColumnToTable(string colName, string tableName)
        {
            bool success = false;
            string sql = "";
            colName = colName.Trim('\'');
            switch (colName)
            {
                case "DateFirstSampling":
                    sql = $"ALTER TABLE {tableName} ADD COLUMN {colName} DATETIME DEFAULT NULL";
                    break;
            }
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    try
                    {
                        conn.Open();
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

        private void GetGears()
        {
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $"Select * from nsap_region_gear where nsap_region='{NSAPRegion.Code}'";
                        conn.Open();
                        MySqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            NSAPRegionGear nrg = new NSAPRegionGear();
                            nrg.RowID = Convert.ToInt32(dr["row_id"]);
                            nrg.NSAPRegion = NSAPRegion;
                            nrg.Gear = NSAPEntities.GearViewModel.GetGear(dr["gear"].ToString());
                            nrg.DateStart = (DateTime)dr["date_start"];
                            if (DateTime.TryParse(dr["date_end"].ToString(), out DateTime v))
                            {
                                nrg.DateEnd = v;
                            }
                            //GetSpecificationForGear(nrg.Gear);
                            NSAPRegion.Gears.Add(nrg);
                        }
                    }
                }
            }
            else
            {

                var dt = new DataTable();
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    try
                    {
                        conection.Open();
                        string query = $"Select * from NSAPRegionGear where NSAPRegionCode='{NSAPRegion.Code}'";

                        var adapter = new OleDbDataAdapter(query, conection);
                        adapter.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                NSAPRegionGear nrg = new NSAPRegionGear();
                                nrg.RowID = Convert.ToInt32(dr["RowID"]);
                                nrg.NSAPRegion = NSAPRegion;
                                nrg.Gear = NSAPEntities.GearViewModel.GetGear(dr["GearCode"].ToString().ToUpper());
                                nrg.DateStart = (DateTime)dr["DateStart"];
                                if (DateTime.TryParse(dr["DateEnd"].ToString(), out DateTime v))
                                {
                                    nrg.DateEnd = v;
                                }
                                //GetSpecificationForGear(nrg.Gear);
                                NSAPRegion.Gears.Add(nrg);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }

            }
        }

        private void GetFishingVessels()
        {
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $"Select * from nsap_region_vessel where region='{NSAPRegion.Code}'";
                        conn.Open();
                        MySqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            NSAPRegionFishingVessel nrfv = new NSAPRegionFishingVessel();
                            nrfv.RowID = Convert.ToInt32(dr["row_id"]);
                            nrfv.NSAPRegion = NSAPRegion;
                            nrfv.FishingVessel = NSAPEntities.FishingVesselViewModel.GetFishingVessel(Convert.ToInt32(dr["vessel"]));
                            nrfv.DateStart = (DateTime)dr["date_start"];
                            if (DateTime.TryParse(dr["date_end"].ToString(), out DateTime v))
                            {
                                nrfv.DateEnd = v;
                            }
                            NSAPRegion.FishingVessels.Add(nrfv);
                        }
                    }
                }
            }
            else

            {
                var dt = new DataTable();
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    try
                    {
                        conection.Open();
                        string query = $"Select * from NSAPRegionVessel where NSAPRegionCode='{NSAPRegion.Code}'";

                        var adapter = new OleDbDataAdapter(query, conection);
                        adapter.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                NSAPRegionFishingVessel nrfv = new NSAPRegionFishingVessel();
                                nrfv.RowID = Convert.ToInt32(dr["RowID"]);
                                nrfv.NSAPRegion = NSAPRegion;
                                nrfv.FishingVessel = NSAPEntities.FishingVesselViewModel.GetFishingVessel(Convert.ToInt32(dr["VesselID"]));
                                nrfv.DateStart = (DateTime)dr["DateStart"];
                                if (DateTime.TryParse(dr["DateEnd"].ToString(), out DateTime v))
                                {
                                    nrfv.DateEnd = v;
                                }
                                NSAPRegion.FishingVessels.Add(nrfv);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Get the landing site that are in the FMAs of an NSAP region
        /// </summary>
        /// <param name="regionFMA"></param>
        private void GetLandingSitesInFMAFishingGrounds(NSAPRegionFMAFishingGround regionFMAFishingGround)
        {
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $"Select * from nsap_region_landing_site where nsap_region_fma_fg='{regionFMAFishingGround.RowID}'";
                        conn.Open();
                        MySqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            NSAPRegionFMAFishingGroundLandingSite nrls = new NSAPRegionFMAFishingGroundLandingSite();
                            nrls.RowID = Convert.ToInt32(dr["row_id"]);
                            nrls.NSAPRegionFMAFishingGround = regionFMAFishingGround;
                            nrls.LandingSite = NSAPEntities.LandingSiteViewModel.GetLandingSite(Convert.ToInt32(dr["landing_site"]));
                            nrls.DateStart = (DateTime)dr["date_start"];
                            if (DateTime.TryParse(dr["date_end"].ToString(), out DateTime v))
                            {
                                nrls.DateEnd = v;
                            }
                            regionFMAFishingGround.LandingSites.Add(nrls);
                        }
                    }
                }
            }
            else
            {
                var dt = new DataTable();
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    try
                    {
                        conection.Open();
                        string query = $"Select * from NSAPRegionLandingSite where NSAPRegionFMAFishingGround={regionFMAFishingGround.RowID}";

                        var adapter = new OleDbDataAdapter(query, conection);
                        adapter.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                NSAPRegionFMAFishingGroundLandingSite nrls = new NSAPRegionFMAFishingGroundLandingSite();
                                nrls.RowID = Convert.ToInt32(dr["RowID"]);
                                nrls.NSAPRegionFMAFishingGround = regionFMAFishingGround;
                                nrls.LandingSite = NSAPEntities.LandingSiteViewModel.GetLandingSite(Convert.ToInt32(dr["LandingSiteID"]));
                                nrls.DateStart = (DateTime)dr["DateStart"];
                                if (DateTime.TryParse(dr["DateEnd"].ToString(), out DateTime v))
                                {
                                    nrls.DateEnd = v;
                                }
                                regionFMAFishingGround.LandingSites.Add(nrls);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
        }

        public static int MaxRecordNumber_RegionFMA()
        {
            int max_rec_no = 0;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT Max(row_id) AS max_record_no FROM nsap_region_fma";
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    using (OleDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT Max(RowID) AS max_record_no FROM NSAPRegionFMA";
                    }
                }
            }
            return max_rec_no;
        }

        public static int MaxRecordNumber_Enumerator()
        {
            int max_rec_no = 0;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT Max(row_id) AS max_record_no FROM nsap_region_enumerator";
                        try
                        {
                            conn.Open();
                            max_rec_no = (int)cmd.ExecuteScalar();
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
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(RowId) AS max_record_no FROM NSAPRegionEnumerator";
                    using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                    {
                        try
                        {
                            max_rec_no = (int)getMax.ExecuteScalar();

                        }
                        catch (Exception ex)
                        {
                            switch (ex.Message)
                            {
                                case "Specified cast is not valid.":
                                case "No data exists for the row/column.":
                                    max_rec_no = 0;
                                    break;
                                default:
                                    Logger.Log(ex);
                                    break;
                            }
                        }
                    }
                }
            }
            return max_rec_no;
        }
        public static int MaxRecordNumber_Gear()
        {
            int max_rec_no = 0;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = "SELECT Max(row_id) AS max_record_no FROM nsap_region_gear";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(RowID) AS max_record_no FROM NSAPRegionGear";
                    using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                    {
                        max_rec_no = (int)getMax.ExecuteScalar();
                    }
                }
            }
            return max_rec_no;
        }
        public static int MaxRecordNumber_FishingGround()
        {
            int max_rec_no = 0;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = "SELECT Max(row_id) AS max_record_no FROM nsap_region_fma_fishing_grounds";
                        try
                        {
                            max_rec_no = (int)cmd.ExecuteScalar();
                        }
                        catch
                        {
                            //
                        }

                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(RowId) AS max_record_no FROM NSAPRegionFMAFishingGrounds";
                    using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                    {
                        try
                        {
                            max_rec_no = (int)getMax.ExecuteScalar();
                        }
                        catch
                        {
                            //
                        }
                    }
                }
            }
            return max_rec_no;
        }
        public static int MaxRecordNumber_LandingSite()
        {
            int max_rec_no = 0;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = "SELECT Max(row_id) AS max_record_no FROM nsap_region_landing_site";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(RowId) AS max_record_no FROM NSAPRegionLandingSite";
                    using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                    {
                        max_rec_no = (int)getMax.ExecuteScalar();
                    }
                }
            }
            return max_rec_no;
        }
        public static int MaxRecordNumber_FishingVessel()
        {
            int max_rec_no = 0;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = "SELECT Max(row_id) AS max_record_no FROM nsap_region_vessel";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(RowId) AS max_record_no FROM NSAPRegionVessel";
                    using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                    {
                        max_rec_no = (int)getMax.ExecuteScalar() + 1;
                    }
                }
            }
            return max_rec_no;
        }
        private static int GetLastRegionFishingVesselID()
        {
            int maxID = 0;
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "Select Max(RowID) from NSAPRegionVessel";
                        try
                        {
                            con.Open();
                            maxID = (int)cmd.ExecuteScalar();
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return maxID;
        }
        public static NSAPRegionFishingVessel CreateRegionFishingVessel(FishingVessel fv, NSAPRegion region, DateTime added)
        {
            NSAPRegionFishingVessel nrfv = new NSAPRegionFishingVessel
            {
                FishingVessel = fv,
                NSAPRegion = region,
                DateStart = added,

            };

            if (region.FishingVessels.Count == 0)
            {
                nrfv.RowID = GetLastRegionFishingVesselID() + 1;
            }
            else
            {
                //nrfv.RowID = MaxRecordNumber_FishingVessel() + 1;
                nrfv.RowID = region.FishingVessels.Max(t => t.RowID) + 1;
            }
            return nrfv;
        }

        public static NSAPRegionEnumerator CreateRegionEnumerator(NSAPEnumerator enumerator, NSAPRegion region, DateTime added)
        {
            return new NSAPRegionEnumerator
            {
                Enumerator = enumerator,
                NSAPRegion = region,
                DateStart = added,
                RowID = MaxRecordNumber_Enumerator() + 1
            };
        }

        private bool AddEnumeratorToMySQL(NSAPRegionEnumerator nre)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    conn.Open();
                    update.Parameters.Add("@row_id", MySqlDbType.VarChar).Value = nre.RowID;
                    update.Parameters.Add("@enumerator", MySqlDbType.VarChar).Value = nre.EnumeratorID;
                    update.Parameters.Add("@nsap_region", MySqlDbType.VarChar).Value = nre.NSAPRegion.Code;
                    update.Parameters.Add("@start", MySqlDbType.DateTime).Value = nre.DateStart;
                    update.Parameters.Add("@end", MySqlDbType.DateTime).Value = nre.DateEnd;
                    update.CommandText = @"Insert into nsap_region_enumerator (row_id, enumerator_id,region,date_start,date_end)
                                           Values (@row_id,@enumerator,@nsap_region,@start,@end)";
                    try
                    {
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException msex)
                    {
                        switch (msex.ErrorCode)
                        {
                            case -2147467259:
                                //duplicated unique index error
                                break;
                            default:
                                Logger.Log(msex);
                                break;
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
        public bool AddEnumerator(NSAPRegionEnumerator regionEnumerator)
        {
            bool success = false;

            if (Global.Settings.UsemySQL)
            {
                success = AddEnumeratorToMySQL(regionEnumerator);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    var sql = "Insert into NSAPRegionEnumerator(RowID, NSAPRegionCode, EnumeratorID, DateStart, DateEnd) Values (?,?,?,?,?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@id", OleDbType.Integer).Value = regionEnumerator.RowID;
                        update.Parameters.Add("@region_code", OleDbType.VarChar).Value = NSAPRegion.Code;
                        update.Parameters.Add("@enum_id", OleDbType.Integer).Value = regionEnumerator.EnumeratorID;
                        update.Parameters.Add("@start", OleDbType.Date).Value = regionEnumerator.DateStart;
                        if (regionEnumerator.DateEnd == null)
                        {
                            update.Parameters.Add("@end", OleDbType.Date).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@end", OleDbType.Date).Value = regionEnumerator.DateEnd;
                        }
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
            if (success)
            {
                NSAPRegion.NSAPEnumerators.Add(regionEnumerator);
            }
            return success;
        }
        private bool AddGeartoMySQL(NSAPRegionGear region_gear)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    conn.Open();
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = region_gear.RowID;
                    update.Parameters.Add("@region_code", MySqlDbType.VarChar).Value = region_gear.NSAPRegion.Code;
                    update.Parameters.Add("@gear_code", MySqlDbType.VarChar).Value = region_gear.GearCode;
                    update.Parameters.Add("@start", MySqlDbType.DateTime).Value = region_gear.DateStart;
                    if (region_gear.DateEnd == null)
                    {
                        update.Parameters.Add("@end", MySqlDbType.DateTime).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@end", MySqlDbType.DateTime).Value = region_gear.DateEnd;
                    }
                    update.CommandText = @"Insert into nsap_region_gear(row_id, nsap_region, gear, date_start, date_end)
                                        Values (@id,@region_code,@gear_code,@start,@end)";
                    try
                    {
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException msex)
                    {
                        switch (msex.ErrorCode)
                        {
                            case -2147467259:
                                //duplicated unique index error
                                break;
                            default:
                                Logger.Log(msex);
                                break;
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
        public bool AddGear(NSAPRegionGear region_gear)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = AddGeartoMySQL(region_gear);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();


                    var sql = "Insert into NSAPRegionGear(RowID, NSAPRegionCode, GearCode, DateStart, DateEnd) Values (?,?,?,?,?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@id", OleDbType.Integer).Value = region_gear.RowID;
                        update.Parameters.Add("@region_code", OleDbType.VarChar).Value = region_gear.NSAPRegion.Code;
                        update.Parameters.Add("@gear_code", OleDbType.VarChar).Value = region_gear.GearCode;
                        update.Parameters.Add("@start", OleDbType.Date).Value = region_gear.DateStart;
                        if (region_gear.DateEnd == null)
                        {
                            update.Parameters.Add("@end", OleDbType.Date).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@end", OleDbType.Date).Value = region_gear.DateEnd;
                        }
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
            if (success)
            {
                NSAPRegion.Gears.Add(region_gear);
            }
            return success;
        }

        private bool AddNSAPRegionFMAToMySQL(NSAPRegionFMA nf)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    conn.Open();
                    update.Parameters.Add("@nsap_region", MySqlDbType.VarChar).Value = nf.NSAPRegion.Code;
                    update.Parameters.Add("@fma", MySqlDbType.VarChar).Value = nf.FMA.FMAID;
                    update.Parameters.Add("@row_id", MySqlDbType.VarChar).Value = nf.RowID;
                    update.CommandText = @"Insert into nsap_region_fma (nsap_region, fma,row_id) Values (@nsap_region,@fma,@row_id)";
                    try
                    {
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException msex)
                    {
                        switch (msex.ErrorCode)
                        {
                            case -2147467259:
                                //duplicated unique index error
                                break;
                            default:
                                Logger.Log(msex);
                                break;
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
        public bool AddNSAPRegionFMA(NSAPRegionFMA nf)
        {
            bool success = false;
            if (!(nf?.RowID > 0))
            {
                nf.RowID = MaxRecordNumber_RegionFMA() + 1;
            }
            if (Global.Settings.UsemySQL)
            {
                success = AddNSAPRegionFMAToMySQL(nf);
            }
            else
            {
                using (var conn = new OleDbConnection(Global.ConnectionString))
                {
                    using (var update = conn.CreateCommand())
                    {
                        conn.Open();
                        update.Parameters.AddWithValue("@nsap_region", nf.NSAPRegion.Code);
                        update.Parameters.AddWithValue("@fma", nf.FMA.FMAID);
                        update.Parameters.AddWithValue("@row_id", nf.RowID);
                        update.CommandText = @"Insert into NSAPRegionFMA (NSAPRegion, FMA,RowID) Values (@nsap_region,@fma,@row_id)";
                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (MySqlException msex)
                        {
                            switch (msex.ErrorCode)
                            {
                                case -2147467259:
                                    //duplicated unique index error
                                    break;
                                default:
                                    Logger.Log(msex);
                                    break;
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

        private bool AddFMAFishingGroundToMySQL(NSAPRegionFMAFishingGround fg)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    conn.Open();
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = fg.RowID;
                    update.Parameters.Add("@fg_code", MySqlDbType.VarChar).Value = fg.FishingGroundCode;
                    update.Parameters.Add("@region_fma", MySqlDbType.Int32).Value = fg.RegionFMA.RowID;
                    update.Parameters.Add("@start", MySqlDbType.DateTime).Value = fg.DateStart;
                    if (fg.DateEnd == null)
                    {
                        update.Parameters.Add("@end", MySqlDbType.DateTime).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@end", MySqlDbType.DateTime).Value = fg.DateEnd;
                    }
                    update.CommandText = @"Insert into nsap_region_fma_fishing_grounds (row_id, fishing_ground, region_fma, date_start, date_end) 
                                         Values (@id,@fg_code,@region_fma,@start,@end)";

                    try
                    {
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException msex)
                    {
                        switch (msex.ErrorCode)
                        {
                            case -2147467259:
                                //duplicated unique index error
                                break;
                            default:
                                Logger.Log(msex);
                                break;
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
        public bool AddFMAFishingGround(NSAPRegionFMAFishingGround fg)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = AddFMAFishingGroundToMySQL(fg);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    var sql = "Insert into NSAPRegionFMAFishingGrounds (RowID, FishingGround, RegionFMA, DateStart, DateEnd) Values (?,?,?,?,?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@id", OleDbType.Integer).Value = fg.RowID;
                        update.Parameters.Add("@fg_code", OleDbType.VarChar).Value = fg.FishingGroundCode;
                        update.Parameters.Add("@region_fma", OleDbType.Integer).Value = fg.RegionFMA.RowID;
                        update.Parameters.Add("@start", OleDbType.Date).Value = fg.DateStart;
                        if (fg.DateEnd == null)
                        {
                            update.Parameters.Add("@end", OleDbType.Date).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@end", OleDbType.Date).Value = fg.DateEnd;
                        }
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
            if (success)
            {
                fg.RegionFMA.FishingGrounds.Add(fg);
            }
            return success;
        }

        private bool AddFMAFishingGroundLandingSiteToMySQL(NSAPRegionFMAFishingGroundLandingSite fgls)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    conn.Open();
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = fgls.RowID;
                    update.Parameters.Add("@fglS_id", MySqlDbType.Int32).Value = fgls.NSAPRegionFMAFishingGround.RowID;
                    update.Parameters.Add("@ls_id", MySqlDbType.Int32).Value = fgls.LandingSite.LandingSiteID;
                    update.Parameters.Add("@start", MySqlDbType.DateTime).Value = fgls.DateStart;

                    if (fgls.DateEnd == null)
                    {
                        update.Parameters.Add("@end", MySqlDbType.DateTime).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@end", MySqlDbType.DateTime).Value = fgls.RowID;
                    }

                    update.CommandText = @"Insert into nsap_region_landing_site (row_id, nsap_region_fma_fg, landing_site, date_start, date_end) 
                                        Values (@id,@fglS_id,@ls_id,@start,@end)";
                    try
                    {
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
                    //if (success)
                    //{
                    //    fgls.NSAPRegionFMAFishingGround.LandingSites.Add(fgls);
                    //}
                }
            }
            return success;
        }
        public bool AddFMAFishingGroundLandingSite(NSAPRegionFMAFishingGroundLandingSite fgls)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = AddFMAFishingGroundLandingSiteToMySQL(fgls);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    var sql = "Insert into NSAPRegionLandingSite (RowID, NSAPRegionFMAFishingGround, LandingSiteID, DateStart, DateEnd) Values (?,?,?,?,?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@id", OleDbType.Integer).Value = fgls.RowID;
                        update.Parameters.Add("@fglS_id", OleDbType.Integer).Value = fgls.NSAPRegionFMAFishingGround.RowID;
                        update.Parameters.Add("@ls_id", OleDbType.Integer).Value = fgls.LandingSite.LandingSiteID;
                        update.Parameters.Add("@start", OleDbType.Date).Value = fgls.DateStart;
                        if (fgls.DateEnd == null)
                        {
                            update.Parameters.Add("@end", OleDbType.Date).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@end", OleDbType.Date).Value = fgls.RowID;
                        }
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
            if (success)
            {
                fgls.NSAPRegionFMAFishingGround.LandingSites.Add(fgls);
            }
            return success;
        }

        private bool AddFishingVesselToMySQL(NSAPRegionFishingVessel rv)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    conn.Open();
                    update.Parameters.Add("@row_id", MySqlDbType.VarChar).Value = rv.RowID;
                    update.Parameters.Add("@vessel", MySqlDbType.VarChar).Value = rv.FishingVesselID;
                    update.Parameters.Add("@nsap_region", MySqlDbType.VarChar).Value = rv.NSAPRegion.Code;
                    update.Parameters.Add("@start", MySqlDbType.DateTime).Value = rv.DateStart;
                    update.Parameters.Add("@end", MySqlDbType.DateTime).Value = rv.DateEnd;
                    update.CommandText = @"Insert into NSAPRegionVessel(row_id, vessel,region, date_start, date_end)
                                          Values (@row_id, @vessel, @nsap_region,@start,@end)";
                    try
                    {
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException msex)
                    {
                        switch (msex.ErrorCode)
                        {
                            case -2147467259:
                                //duplicated unique index error
                                break;
                            default:
                                Logger.Log(msex);
                                break;
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
        public bool AddFishingVessel(NSAPRegionFishingVessel rv)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = AddFishingVesselToMySQL(rv);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    var sql = "Insert into NSAPRegionVessel(RowID, NSAPRegionCode, VesselID, DateStart, DateEnd) Values (?,?,?,?,?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@id", OleDbType.Integer).Value = rv.RowID;
                        update.Parameters.Add("@region_code", OleDbType.VarChar).Value = rv.NSAPRegion.Code;
                        update.Parameters.Add("@vessel_id", OleDbType.Integer).Value = rv.FishingVesselID;
                        update.Parameters.Add("@start", OleDbType.Date).Value = rv.DateStart;
                        if (rv.DateEnd == null)
                        {
                            update.Parameters.Add("@end", OleDbType.Date).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@end", OleDbType.Date).Value = rv.DateEnd;
                        }
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
            if (success)
            {
                NSAPRegion.FishingVessels.Add(rv);
            }
            return success;
        }

        private bool EditLandingSiteMySQL(NSAPRegionFMAFishingGroundLandingSite fmaFishingGroundLandingSite)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@lsid", MySqlDbType.Int32).Value = fmaFishingGroundLandingSite.LandingSite.LandingSiteID;
                    update.Parameters.Add("@start", MySqlDbType.DateTime).Value = fmaFishingGroundLandingSite.DateStart;
                    if (fmaFishingGroundLandingSite.DateEnd == null)
                    {
                        update.Parameters.Add("@end", MySqlDbType.DateTime).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("end", MySqlDbType.DateTime).Value = fmaFishingGroundLandingSite.DateEnd;
                    }
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = fmaFishingGroundLandingSite.RowID;

                    update.CommandText = @"Update nsap_region_landing_site set
                                            landing_site = @id',
                                            date_start = @start',
                                            date_emd = @end
                                        Where RowID=@id";
                }
            }
            return success;
        }
        public bool EditLandingSite(NSAPRegionFMAFishingGroundLandingSite fmaFishingGroundLandingSite)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = EditLandingSiteMySQL(fmaFishingGroundLandingSite);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();



                    using (OleDbCommand update = conn.CreateCommand())
                    {
                        update.Parameters.Add("@lsid", OleDbType.Integer).Value = fmaFishingGroundLandingSite.LandingSite.LandingSiteID;
                        update.Parameters.Add("@start", OleDbType.Date).Value = fmaFishingGroundLandingSite.DateStart;
                        if (fmaFishingGroundLandingSite.DateEnd == null)
                        {
                            update.Parameters.Add("@end", OleDbType.Date).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("end", OleDbType.Date).Value = fmaFishingGroundLandingSite.DateEnd;
                        }
                        update.Parameters.Add("@id", OleDbType.Integer).Value = fmaFishingGroundLandingSite.RowID;

                        update.CommandText = @"Update NSAPRegionLandingSite set
                                            LandingSiteID = @id',
                                            DateStart = @start',
                                            DateEnd = @end
                                        Where RowID=@id";

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
                        if (success)
                        {
                            foreach (var ls in fmaFishingGroundLandingSite.NSAPRegionFMAFishingGround.LandingSites)
                            {
                                if (ls.RowID == fmaFishingGroundLandingSite.RowID)
                                {
                                    fmaFishingGroundLandingSite.NSAPRegionFMAFishingGround.LandingSites.Remove(ls);
                                    fmaFishingGroundLandingSite.NSAPRegionFMAFishingGround.LandingSites.Add(fmaFishingGroundLandingSite);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return success;
        }

        public bool EditEnumerator(NSAPRegionEnumerator regionEnumerator)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var update = conn.CreateCommand())
                    {
                        update.Parameters.Add("@enum_id", MySqlDbType.Int32).Value = regionEnumerator.Enumerator.ID;
                        update.Parameters.Add("@start", MySqlDbType.Date).Value = regionEnumerator.DateStart;
                        if (regionEnumerator.DateEnd == null)
                        {
                            update.Parameters.Add("@end", MySqlDbType.Date).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@end", MySqlDbType.Date).Value = regionEnumerator.DateEnd;
                        }
                        update.Parameters.Add("@id", MySqlDbType.Int32).Value = regionEnumerator.RowID;

                        update.CommandText = @"Update nsap_region_enumerator set
                                            enumerator_id = @enum_id,
                                            date_start = @start,
                                            date_end = @end
                                        Where RowID=@row_id";
                        try
                        {
                            conn.Open();
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (MySqlException mex)
                        {
                            Logger.Log(mex);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();


                    using (OleDbCommand update = conn.CreateCommand())
                    {
                        update.Parameters.Add("@enum_id", OleDbType.Integer).Value = regionEnumerator.Enumerator.ID;
                        update.Parameters.Add("@start", OleDbType.Date).Value = regionEnumerator.DateStart;
                        if (regionEnumerator.DateEnd == null)
                        {
                            update.Parameters.Add("@end", OleDbType.Date).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@end", OleDbType.Date).Value = regionEnumerator.DateEnd;
                        }
                        update.Parameters.Add("@id", OleDbType.Integer).Value = regionEnumerator.RowID;

                        update.CommandText = @"Update NSAPRegionEnumerator set
                                            EnumeratorID = @enum_id,
                                            DateStart = @start,
                                            DateEnd = @end
                                        Where RowID=@id";

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
                        if (success)
                        {
                            foreach (var en in regionEnumerator.NSAPRegion.NSAPEnumerators)
                            {
                                if (en.RowID == regionEnumerator.RowID)
                                {
                                    regionEnumerator.NSAPRegion.NSAPEnumerators.Remove(en);
                                    regionEnumerator.NSAPRegion.NSAPEnumerators.Add(regionEnumerator);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return success;
        }
        private bool EditFMAFishingGroundMySQL(NSAPRegionFMAFishingGround fmaFishngGround)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@fg_code", MySqlDbType.VarChar).Value = fmaFishngGround.FishingGround.Code;
                    update.Parameters.Add("@start", MySqlDbType.DateTime).Value = fmaFishngGround.DateStart;
                    if (fmaFishngGround.DateEnd == null)
                    {
                        update.Parameters.Add("@end", MySqlDbType.DateTime).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@end", MySqlDbType.DateTime).Value = fmaFishngGround.DateEnd;
                    }
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = fmaFishngGround.RowID;

                    update.CommandText = @"Update nsap_region_fma_fishing_grounds set
                                            fishing_ground = @fg_code,
                                            date_start = @start,
                                            date_end = @end
                                            Where row_id=@id";
                    try
                    {
                        conn.Open();
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException mse)
                    {
                        Logger.Log(mse);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return success;
        }
        public bool EditFMAFishingGround(NSAPRegionFMAFishingGround fmaFishngGround)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = EditFMAFishingGroundMySQL(fmaFishngGround);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();


                    using (OleDbCommand update = conn.CreateCommand())
                    {
                        update.Parameters.Add("@fg_code", OleDbType.VarChar).Value = fmaFishngGround.FishingGround.Code;
                        update.Parameters.Add("@start", OleDbType.Date).Value = fmaFishngGround.DateStart;
                        if (fmaFishngGround.DateEnd == null)
                        {
                            update.Parameters.Add("@end", OleDbType.Date).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@end", OleDbType.Date).Value = fmaFishngGround.DateEnd;
                        }
                        update.Parameters.Add("@id", OleDbType.Integer).Value = fmaFishngGround.RowID;

                        update.CommandText = @"Update NSAPRegionFMAFishingGrounds set
                                            FishingGround = @fg_code,
                                            DateStart = @start,
                                            DateEnd = @end
                                        Where RowID=@id";

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
                        if (success)
                        {
                            foreach (var fg in fmaFishngGround.RegionFMA.FishingGrounds)
                            {
                                if (fg.RowID == fmaFishngGround.RowID)
                                {
                                    fmaFishngGround.RegionFMA.FishingGrounds.Remove(fg);
                                    fmaFishngGround.RegionFMA.FishingGrounds.Add(fmaFishngGround);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return success;
        }
        private bool EditFishingVesselMySQL(NSAPRegionFishingVessel regionFishingVessel)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@vessel_id", MySqlDbType.Int32).Value = regionFishingVessel.FishingVessel.ID;
                    update.Parameters.Add("@start", MySqlDbType.DateTime).Value = regionFishingVessel.DateStart;
                    if (regionFishingVessel.DateEnd == null)
                    {
                        update.Parameters.Add("@end", MySqlDbType.DateTime).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@end", MySqlDbType.DateTime).Value = regionFishingVessel.DateEnd;
                    }
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = regionFishingVessel.RowID;

                    update.CommandText = @"Update nsap_region_vessel set
                                            vessel = @vessel_id,
                                            date_start = @start,
                                            date_end = @end
                                        Where RowID=@row_id";
                    try
                    {
                        conn.Open();
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException msx)
                    {
                        Logger.Log(msx);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return success;
        }

        public bool EditFishingVessel(NSAPRegionFishingVessel regionFishingVessel)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = EditFishingVesselMySQL(regionFishingVessel);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();


                    using (OleDbCommand update = conn.CreateCommand())
                    {
                        update.Parameters.Add("@vessel_id", OleDbType.Integer).Value = regionFishingVessel.FishingVessel.ID;
                        update.Parameters.Add("@start", OleDbType.Date).Value = regionFishingVessel.DateStart;
                        if (regionFishingVessel.DateEnd == null)
                        {
                            update.Parameters.Add("@end", OleDbType.Date).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@end", OleDbType.Date).Value = regionFishingVessel.DateEnd;
                        }
                        update.Parameters.Add("@id", OleDbType.Integer).Value = regionFishingVessel.RowID;

                        update.CommandText = @"Update NSAPRegionVessel set
                                            VesselID = @vessel_id,
                                            DateStart = @start,
                                            DateEnd = @end
                                        Where RowID=@id";

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
                        if (success)
                        {
                            foreach (var vs in regionFishingVessel.NSAPRegion.FishingVessels)
                            {
                                if (vs.RowID == regionFishingVessel.RowID)
                                {
                                    regionFishingVessel.NSAPRegion.FishingVessels.Remove(vs);
                                    regionFishingVessel.NSAPRegion.FishingVessels.Add(regionFishingVessel);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return success;
        }
        private bool EditGearMySQL(NSAPRegionGear regionGear)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@gear_code", MySqlDbType.VarChar).Value = regionGear.Gear.Code;
                    update.Parameters.Add("@start", MySqlDbType.DateTime).Value = regionGear.DateStart;
                    if (regionGear.DateEnd == null)
                    {
                        update.Parameters.Add("@end", MySqlDbType.DateTime).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@end", MySqlDbType.DateTime).Value = regionGear.DateEnd;
                    }
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = regionGear.RowID;

                    update.CommandText = @"Update nsap_region_gear set
                                            gear = @gear_code,
                                            date_start = @start,
                                            date_end = @end
                                        Where row_id=@id";
                    try
                    {
                        conn.Open();
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException msx)
                    {
                        Logger.Log(msx);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return success;
        }
        public bool EditGear(NSAPRegionGear regionGear)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = EditGearMySQL(regionGear);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();


                    using (OleDbCommand update = conn.CreateCommand())
                    {
                        update.Parameters.Add("@gear_code", OleDbType.VarChar).Value = regionGear.Gear.Code;
                        update.Parameters.Add("@start", OleDbType.Date).Value = regionGear.DateStart;
                        if (regionGear.DateEnd == null)
                        {
                            update.Parameters.Add("@end", OleDbType.Date).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@end", OleDbType.Date).Value = regionGear.DateEnd;
                        }
                        update.Parameters.Add("@id", OleDbType.Integer).Value = regionGear.RowID;

                        update.CommandText = @"Update NSAPRegionGear set
                                            GearCode = @gear_code,
                                            DateStart = @start,
                                            DateEnd = @end
                                        Where RowID=@id";

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
                        if (success)
                        {
                            foreach (var gear in regionGear.NSAPRegion.Gears)
                            {
                                if (gear.RowID == regionGear.RowID)
                                {
                                    regionGear.NSAPRegion.Gears.Remove(gear);
                                    regionGear.NSAPRegion.Gears.Add(regionGear);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return success;
        }

        private bool DeleteGearEffortMySQL(int id)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.CommandText = "Delete  from gear_effort_specification where row_id=@id";
                    try
                    {
                        conn.Open();
                        success = cmd.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException msx)
                    {
                        Logger.Log(msx);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return success;
        }
        public bool DeleteGearEffort(int id)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = DeleteGearEffortMySQL(id);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    var sql = $"Delete * from GearEffortSpecification where RowID={id}";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
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

        private bool DeleteLandingSiteMySQL(int id)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.CommandText = "Delete from nsap_region_landing_site where row_id=@id;";
                    try
                    {
                        conn.Open();
                        success = cmd.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException msx)
                    {
                        Logger.Log(msx);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return success;
        }
        public bool DeleteLandingSite(int id)
        {
            bool success = false;
            DatabaseErrorMessage = "";
            if (Global.Settings.UsemySQL)
            {
                success = DeleteLandingSiteMySQL(id);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    var sql = $"Delete * from NSAPRegionLandingSite where RowID={id}";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
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
        private bool DeleteFishingGroundMySQL(int id)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.CommandText = "Delete  from nsap_region_fma_fishing_grounds where row_id=@id";
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
        public bool DeleteFishingGround(int id)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = DeleteFishingGroundMySQL(id);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    var sql = $"Delete * from NSAPRegionFMAFishingGrounds where RowID={id}";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException dbex)
                        {
                            DatabaseErrorMessage = dbex.Message;
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
        private bool DeleteFishingVesselMySQL(int id)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.CommandText = "Delete  from nsap_region_vessel where row_id={@id}";
                    try
                    {
                        conn.Open();
                        success = cmd.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException msx)
                    {
                        Logger.Log(msx);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return success;
        }
        public bool DeleteFishingVessel(int id)
        {
            DatabaseErrorMessage = "";
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = DeleteFishingVesselMySQL(id);
            }
            else
            {

                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    var sql = $"Delete * from NSAPRegionVessel where RowID={id}";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException dbex)
                        {
                            DatabaseErrorMessage = dbex.Message;
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
        private bool DeleteEnumeratorMySQL(int id)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.CommandText = $"Delete from nsap_region_enumerator where row_id=@id";
                    try
                    {
                        conn.Open();
                        success = cmd.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException mex)
                    {
                        Logger.Log(mex);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return success;
        }
        public bool DeleteEnumerator(int id)
        {
            DatabaseErrorMessage = "";
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = DeleteEnumeratorMySQL(id);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    var sql = $"Delete * from NSAPRegionEnumerator where RowID={id}";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException dbex)
                        {
                            DatabaseErrorMessage = dbex.Message;
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

        private bool DeleteGearMySQL(int id)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.CommandText = "Delete  from nsap_region_gear where row_id=@id";
                    try
                    {
                        conn.Open();
                        success = cmd.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException msx)
                    {
                        Logger.Log(msx);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return success;
        }
        public bool DeleteGear(int id)
        {
            DatabaseErrorMessage = "";
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = DeleteGearMySQL(id);
            }
            else
            {

                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    var sql = $"Delete * from NSAPRegionGear where RowID={id}";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException dbex)
                        {
                            DatabaseErrorMessage = dbex.Message;
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