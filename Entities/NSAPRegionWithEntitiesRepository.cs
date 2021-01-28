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

        /// <summary>
        /// Get the FMAs that are in an NSAP Region
        /// </summary>
        private void GetFMAS()
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
                        if (isNew || fishingGround.FishingGroundCode != oldFishingGround.FishingGround.Code)
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
                    if (g.GearCode == gear.GearCode)
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

        private void GetEnumerators()
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
                            if (DateTime.TryParse(dr["DateEnd"].ToString(), out DateTime v))
                            {
                                nre.DateEnd = v;
                            }
                            NSAPRegion.NSAPEnumerators.Add(nre);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
        }

        private void GetGears()
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
                            nrg.Gear = NSAPEntities.GearViewModel.GetGear(dr["GearCode"].ToString());
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

        private void GetFishingVessels()
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

        /// <summary>
        /// Get the landing site that are in the FMAs of an NSAP region
        /// </summary>
        /// <param name="regionFMA"></param>
        private void GetLandingSitesInFMAFishingGrounds(NSAPRegionFMAFishingGround regionFMAFishingGround)
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



        public static int MaxRecordNumber_Enumerator()
        {
            int max_rec_no = 0;
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
                    catch(Exception ex)
                    {
                        switch(ex.Message)
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
            return max_rec_no;
        }
        public static int MaxRecordNumber_Gear()
        {
            int max_rec_no = 0;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                const string sql = "SELECT Max(RowId) AS max_record_no FROM NSAPRegionGear";
                using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                {
                    max_rec_no = (int)getMax.ExecuteScalar();
                }
            }
            return max_rec_no;
        }
        public static int MaxRecordNumber_FishingGround()
        {
            int max_rec_no = 0;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                const string sql = "SELECT Max(RowId) AS max_record_no FROM NSAPRegionFMAFishingGrounds";
                using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                {
                    max_rec_no = (int)getMax.ExecuteScalar();
                }
            }
            return max_rec_no;
        }
        public static int MaxRecordNumber_LandingSite()
        {
            int max_rec_no = 0;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                const string sql = "SELECT Max(RowId) AS max_record_no FROM NSAPRegionLandingSite";
                using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                {
                    max_rec_no = (int)getMax.ExecuteScalar();
                }
            }
            return max_rec_no;
        }
        public static int MaxRecordNumber_FishingVessel()
        {
            int max_rec_no = 0;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                const string sql = "SELECT Max(RowId) AS max_record_no FROM NSAPRegionVessel";
                using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                {
                    max_rec_no = (int)getMax.ExecuteScalar();
                }
            }
            return max_rec_no;
        }


        public static NSAPRegionEnumerator CreateRegionEnumerator (NSAPEnumerator enumerator, NSAPRegion region, DateTime added)
        {
            return new NSAPRegionEnumerator
            {
                Enumerator = enumerator,
                NSAPRegion = region,
                DateStart = added,
                RowID = MaxRecordNumber_Enumerator() + 1
            };
        }
        public bool AddEnumerator(NSAPRegionEnumerator regionEnumerator)
        {
            bool success = false;
            string dateEnd = regionEnumerator.DateEnd == null ? "null" : $"'{((DateTime)regionEnumerator.DateEnd).ToString(_dateFormat)}'";
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into NSAPRegionEnumerator(RowID, NSAPRegionCode, EnumeratorID, DateStart, DateEnd)
                           Values ({regionEnumerator.RowID}, '{NSAPRegion.Code}','{regionEnumerator.EnumeratorID}','{regionEnumerator.DateStart.ToString(_dateFormat)}',{dateEnd})";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                    if (success)
                    {
                        NSAPRegion.NSAPEnumerators.Add(regionEnumerator);
                    }
                }
            }
            return success;
        }
        public bool AddGear(NSAPRegionGear region_gear)
        {
            bool success = false;
            string dateEnd = region_gear.DateEnd == null ? "null" : $"'{((DateTime)region_gear.DateEnd).ToString(_dateFormat)}'";
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into NSAPRegionGear(RowID, NSAPRegionCode, GearCode, DateStart, DateEnd)
                           Values ({region_gear.RowID}, '{region_gear.NSAPRegion.Code}','{region_gear.GearCode}','{region_gear.DateStart.ToString(_dateFormat)}',{dateEnd})";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                    if (success)
                    {
                        NSAPRegion.Gears.Add(region_gear);
                    }
                }
            }
            return success;
        }


        public bool AddFMAFishingGround(NSAPRegionFMAFishingGround fishingGround)
        {
            bool success = false;
            string dateEnd = fishingGround.DateEnd == null ? "null" : $"'{((DateTime)fishingGround.DateEnd).ToString(_dateFormat)}'";
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into NSAPRegionFMAFishingGrounds (RowID, FishingGround, RegionFMA, DateStart, DateEnd)
                           Values ({fishingGround.RowID}, '{fishingGround.FishingGroundCode}',{fishingGround.RegionFMA.RowID},'{fishingGround.DateStart.ToString(_dateFormat)}',{dateEnd})";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                    if (success)
                    {
                        fishingGround.RegionFMA.FishingGrounds.Add(fishingGround);
                    }
                }
            }
            return success;
        }


        public bool AddFMAFishingGroundLandingSite(NSAPRegionFMAFishingGroundLandingSite fishingGroundLandingSite)
        {
            bool success = false;
            string dateEnd = fishingGroundLandingSite.DateEnd == null ? "null" : $"'{((DateTime)fishingGroundLandingSite.DateEnd).ToString(_dateFormat)}'";
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into NSAPRegionLandingSite (RowID, NSAPRegionFMAFishingGround, LandingSiteID, DateStart, DateEnd)
                           Values ({fishingGroundLandingSite.RowID}, {fishingGroundLandingSite.NSAPRegionFMAFishingGround.RowID},{fishingGroundLandingSite.LandingSite.LandingSiteID},'{fishingGroundLandingSite.DateStart.ToString(_dateFormat)}',{dateEnd})";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                    if (success)
                    {
                        fishingGroundLandingSite.NSAPRegionFMAFishingGround.LandingSites.Add(fishingGroundLandingSite);
                    }
                }
            }
            return success;
        }


        public bool AddFishingVessel(NSAPRegionFishingVessel region_vessel)
        {
            bool success = false;
            string dateEnd = region_vessel.DateEnd == null ? "null" : $"'{((DateTime)region_vessel.DateEnd).ToString(_dateFormat)}'";
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into NSAPRegionVessel(RowID, NSAPRegionCode, VesselID, DateStart, DateEnd)
                           Values ({region_vessel.RowID}, '{region_vessel.NSAPRegion.Code}',{region_vessel.FishingVesselID},'{region_vessel.DateStart.ToString(_dateFormat)}',{dateEnd})";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                    if (success)
                    {
                        NSAPRegion.FishingVessels.Add(region_vessel);
                    }
                }
            }
            return success;
        }

        public bool EditLandingSite(NSAPRegionFMAFishingGroundLandingSite fmaFishingGroundLandingSite)
        {
            bool success = false;
            string dateEnd = fmaFishingGroundLandingSite.DateEnd == null ? "null" : $"'{((DateTime)fmaFishingGroundLandingSite.DateEnd).ToString(_dateFormat)}'";
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update NSAPRegionLandingSite set
                                LandingSiteID = '{fmaFishingGroundLandingSite.LandingSite.LandingSiteID}',
                                DateStart = '{fmaFishingGroundLandingSite.DateStart}',
                                DateEnd = {dateEnd}
                            Where RowID={fmaFishingGroundLandingSite.RowID}";

                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
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
            return success;
        }

        public bool EditEnumerator(NSAPRegionEnumerator regionEnumerator)
        {
            bool success = false;
            string dateEnd = regionEnumerator.DateEnd == null ? "null" : $"'{((DateTime)regionEnumerator.DateEnd).ToString(_dateFormat)}'";
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update NSAPRegionEnumerator set
                                EnumeratorID = {regionEnumerator.Enumerator.ID},
                                DateStart = '{regionEnumerator.DateStart}',
                                DateEnd = {dateEnd}
                            Where RowID={regionEnumerator.RowID}";

                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
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
            return success;
        }

        public bool EditFMAFishingGround(NSAPRegionFMAFishingGround fmaFishngGround)
        {
            bool success = false;
            string dateEnd = fmaFishngGround.DateEnd == null ? "null" : $"'{((DateTime)fmaFishngGround.DateEnd).ToString(_dateFormat)}'";
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update NSAPRegionFMAFishingGrounds set
                                FishingGround = '{fmaFishngGround.FishingGround.Code}',
                                DateStart = '{fmaFishngGround.DateStart}',
                                DateEnd = {dateEnd}
                            Where RowID={fmaFishngGround.RowID}";

                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
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
            return success;
        }

        public bool EditFishingVessel(NSAPRegionFishingVessel regionFishingVessel)
        {
            bool success = false;
            string dateEnd = regionFishingVessel.DateEnd == null ? "null" : $"'{((DateTime)regionFishingVessel.DateEnd).ToString(_dateFormat)}'";
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update NSAPRegionVessel set
                                VesselID = {regionFishingVessel.FishingVessel.ID},
                                DateStart = '{regionFishingVessel.DateStart}',
                                DateEnd = {dateEnd}
                            Where RowID={regionFishingVessel.RowID}";

                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
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
            return success;
        }

        public bool EditGear(NSAPRegionGear regionGear)
        {
            bool success = false;
            string dateEnd = regionGear.DateEnd == null ? "null" : $"'{((DateTime)regionGear.DateEnd).ToString(_dateFormat)}'";
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update NSAPRegionGear set
                                GearCode = '{regionGear.Gear.Code}',
                                DateStart = '{regionGear.DateStart}',
                                DateEnd = {dateEnd}
                            Where RowID={regionGear.RowID}";

                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
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
            return success;
        }

        public bool DeleteLandingSite(int id)
        {
            DatabaseErrorMessage = "";
            bool success = false;
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
            return success;
        }

        public bool DeleteFishingGround(int id)
        {
            DatabaseErrorMessage = "";
            bool success = false;
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
            return success;
        }

        public bool DeleteFishingVessel(int id)
        {
            DatabaseErrorMessage = "";
            bool success = false;
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
            return success;
        }

        public bool DeleteEnumerator(int id)
        {
            DatabaseErrorMessage = "";
            bool success = false;
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
            return success;
        }

        public bool DeleteGear(int id)
        {
            DatabaseErrorMessage = "";
            bool success = false;
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
            return success;
        }
    }
}