using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using NSAP_ODK.Utilities;
namespace NSAP_ODK.Entities.Database
{
    class VesselUnloadRepository
    {
        private string _dateFormat = "MMM-dd-yyyy HH:mm";
        public List<VesselUnload> VesselUnloads { get; set; }

        public VesselUnloadRepository()
        {
            VesselUnloads = getVesselUnloads();
        }
        public int MaxRecordNumber()
        {
            int max_rec_no = 0;
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                const string sql = "SELECT Max(v_unload_id) AS max_id FROM dbo_vessel_unload";
                using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                {
                    max_rec_no = (int)getMax.ExecuteScalar();
                }
            }
            return max_rec_no;
        }
        public List<VesselUnloadTrackedFlattened> getTrackedFlattenedList()
        {
            List<VesselUnloadTrackedFlattened> thisList = new List<VesselUnloadTrackedFlattened>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $@"SELECT 
                                        dbo_LC_FG_sample_day.unload_day_id,
                                        nsapRegion.RegionName, 
                                        fma.FMAName, 
                                        fishingGround.FishingGroundName, 
                                        [LandingSiteName] & ', ' & [Municipalities.Municipality] & ', ' & [ProvinceName] AS LandingSite, 
                                        dbo_LC_FG_sample_day.land_ctr_text, 
                                        dbo_LC_FG_sample_day.sdate, 
                                        dbo_gear_unload.unload_gr_id, 
                                        gear.GearName, 
                                        dbo_gear_unload.gr_text, 
                                        dbo_gear_unload.boats, 
                                        dbo_gear_unload.catch, 
                                        dbo_vessel_unload.v_unload_id, 
                                        dbo_vessel_unload_1.SamplingDate AS SamplingDateTime, 
                                        fishingVessel.VesselName, 
                                        fishingVessel.NameOfOwner, 
                                        dbo_vessel_unload.is_boat_used, 
                                        dbo_vessel_unload.boat_text, 
                                        dbo_vessel_unload.catch_total, 
                                        dbo_vessel_unload.catch_samp, 
                                        dbo_vessel_unload.boxes_total, 
                                        dbo_vessel_unload.boxes_samp, 
                                        dbo_vessel_unload.raising_factor,
                                        dbo_vessel_unload_1.Success, 
                                        dbo_vessel_unload_1.Tracked, 
                                        dbo_vessel_unload_1.DepartureLandingSite, 
                                        dbo_vessel_unload_1.ArrivalLandingSite, 
                                        dbo_vessel_unload_1.RowID, 
                                        dbo_vessel_unload_1.XFormIdentifier, 
                                        dbo_vessel_unload_1.XFormDate, 
                                        dbo_vessel_unload_1.user_name, 
                                        dbo_vessel_unload_1.device_id, 
                                        dbo_vessel_unload_1.datetime_submitted, 
                                        dbo_vessel_unload_1.form_version, 
                                        dbo_vessel_unload_1.Notes, 
                                        gps.AssignedName AS GPS, 
                                        NSAPEnumerator.EnumeratorName, 
                                        dbo_vessel_unload_1.EnumeratorText, 
                                        dbo_vessel_unload_1.DateAdded, 
                                        dbo_vessel_unload_1.sector_code, 
                                        dbo_vessel_unload_1.FromExcelDownload
                                    FROM Provinces 
                                        RIGHT JOIN(Municipalities 
                                        RIGHT JOIN (landingSite 
                                        RIGHT JOIN (gear 
                                        RIGHT JOIN (fma 
                                        INNER JOIN (fishingVessel 
                                        RIGHT JOIN (fishingGround 
                                        INNER JOIN (((nsapRegion 
                                        INNER JOIN dbo_LC_FG_sample_day 
                                            ON nsapRegion.Code = dbo_LC_FG_sample_day.region_id) 
                                        INNER JOIN(dbo_gear_unload 
                                        INNER JOIN dbo_vessel_unload 
                                            ON dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) 
                                            ON dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) 
                                        INNER JOIN((dbo_vessel_unload_1 
                                        INNER JOIN gps 
                                            ON dbo_vessel_unload_1.GPS = gps.GPSCode) 
                                        LEFT JOIN NSAPEnumerator 
                                            ON dbo_vessel_unload_1.EnumeratorID = NSAPEnumerator.EnumeratorID) 
                                            ON dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id) 
                                            ON fishingGround.FishingGroundCode = dbo_LC_FG_sample_day.ground_id) 
                                            ON fishingVessel.VesselID = dbo_vessel_unload.boat_id) 
                                            ON fma.FMAID = dbo_LC_FG_sample_day.fma) 
                                            ON gear.GearCode = dbo_gear_unload.gr_id) 
                                            ON landingSite.LandingSiteID = dbo_LC_FG_sample_day.land_ctr_id) 
                                            ON Municipalities.MunNo = landingSite.Municipality) 
                                            ON Provinces.ProvNo = Municipalities.ProvNo
                                        WHERE dbo_vessel_unload_1.Tracked = True
                                        ORDER BY 
                                            nsapRegion.RegionName, 
                                            fma.FMAName, 
                                            fishingGround.FishingGroundName, 
                                            [LandingSiteName] & ', ' & [Municipalities.Municipality] & ', ' & [ProvinceName], 
                                            dbo_LC_FG_sample_day.land_ctr_text, 
                                            dbo_LC_FG_sample_day.sdate, 
                                            gear.GearName";


                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        thisList.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            VesselUnloadTrackedFlattened vuf = new VesselUnloadTrackedFlattened
                            {
                                SamplingDayID = (int)dr["unload_day_id"],
                                Region = dr["RegionName"].ToString(),
                                FMA = dr["FMAName"].ToString(),
                                FishingGround = dr["FishingGroundName"].ToString(),
                                LandingSite = string.IsNullOrEmpty(dr["LandingSite"].ToString()) ? dr["land_ctr_text"].ToString() : dr["LandingSite"].ToString(),
                                SamplingDate = (DateTime)dr["sDate"],
                                GearUnloadID = (int)dr["unload_gr_id"],
                                Gear = !string.IsNullOrEmpty(dr["GearName"].ToString()) ? dr["GearName"].ToString() : dr["gr_text"].ToString(),
                                BoatsLanded = string.IsNullOrEmpty(dr["boats"].ToString()) ? null : (int?)dr["boats"],
                                CatchTotalLanded = string.IsNullOrEmpty(dr["catch"].ToString()) ? null : (double?)dr["catch"],
                                VesselUnloadID = (int)dr["v_unload_id"],
                                SamplingDateTime = (DateTime)dr["SamplingDateTime"],
                                Enumerator = string.IsNullOrEmpty(dr["EnumeratorName"].ToString()) ? dr["EnumeratorText"].ToString() : dr["EnumeratorName"].ToString(),
                                IsBoatUsed = (bool)dr["is_boat_used"],
                                Vessel = !string.IsNullOrEmpty(dr["VesselName"].ToString()) ? "F/V " + dr["VesselName"].ToString() :
                                         !string.IsNullOrEmpty(dr["NameOfOwner"].ToString()) ? dr["NameOfOwner"].ToString() :
                                         dr["boat_text"].ToString(),
                                CatchTotalWt = string.IsNullOrEmpty(dr["catch_total"].ToString()) ? null : (double?)dr["catch_total"],
                                CatchSampleWt = string.IsNullOrEmpty(dr["catch_samp"].ToString()) ? null : (double?)dr["catch_samp"],
                                Boxes = string.IsNullOrEmpty(dr["boxes_total"].ToString()) ? null : (int?)dr["boxes_total"],
                                BoxesSampled = string.IsNullOrEmpty(dr["boxes_samp"].ToString()) ? null : (int?)dr["boxes_samp"],
                                RaisingFactor = dr["raising_factor"] == DBNull.Value ? null : (double?)dr["raising_factor"],
                                IsSuccess = (bool)dr["Success"],
                                IsTracked = (bool)dr["Tracked"],
                                GPS = dr["GPS"].ToString(),
                                Departure = string.IsNullOrEmpty(dr["DepartureLandingSite"].ToString()) ? null : (DateTime?)dr["DepartureLandingSite"],
                                Arrival = string.IsNullOrEmpty(dr["ArrivalLandingSite"].ToString()) ? null : (DateTime?)dr["ArrivalLandingSite"],
                                RowID = dr["RowID"].ToString(),
                                XFormIdentifier = dr["XFormIdentifier"].ToString(),
                                XFormDate = dr["XFormDate"] == DBNull.Value ? null : (DateTime?)dr["XFormDate"],
                                UserName = dr["user_name"].ToString(),
                                DeviceID = dr["device_id"].ToString(),
                                Submitted = (DateTime)dr["datetime_submitted"],
                                FormVersion = dr["form_version"].ToString(),
                                Notes = dr["Notes"].ToString(),
                                DateAddedToDatabase = dr["DateAdded"] == DBNull.Value ? null : (DateTime?)dr["DateAdded"],
                                Sector = NSAPEntities.VesselUnloadViewModel.GetSector(dr["sector_code"].ToString()),
                                FromExcelDownload = (bool)dr["FromExcelDownload"]
                            };
                            thisList.Add(vuf);
                        }
                        return thisList;
                    }
                }
                catch (OleDbException dbEx)
                {
                    Logger.Log(dbEx);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);

                }
            }
            return null;
        }

        private List<VesselUnload> getVesselUnloads()
        {
            List<VesselUnload> thisList = new List<VesselUnload>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = @"SELECT dbo_vessel_unload.*, dbo_vessel_unload_1.* FROM dbo_vessel_unload 
                        INNER JOIN dbo_vessel_unload_1 ON dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id;";


                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        thisList.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            VesselUnload item = new VesselUnload();
                            item.PK = (int)dr["dbo_vessel_unload.v_unload_id"];
                            item.GearUnloadID = (int)dr["unload_gr_id"];
                            item.IsBoatUsed = (bool)dr["is_boat_used"];
                            item.VesselID = string.IsNullOrEmpty(dr["boat_id"].ToString()) ? null : (int?)dr["boat_id"];
                            item.VesselText = dr["boat_text"].ToString();
                            item.SectorCode = dr["sector_code"].ToString();
                            item.WeightOfCatch = string.IsNullOrEmpty(dr["catch_total"].ToString()) ? null : (double?)dr["catch_total"];
                            item.WeightOfCatchSample = string.IsNullOrEmpty(dr["catch_samp"].ToString()) ? null : (double?)dr["catch_samp"];
                            item.Boxes = string.IsNullOrEmpty(dr["boxes_total"].ToString()) ? null : (int?)dr["boxes_total"];
                            item.BoxesSampled = string.IsNullOrEmpty(dr["boxes_samp"].ToString()) ? null : (int?)dr["boxes_samp"];
                            item.RaisingFactor = dr["raising_factor"] == DBNull.Value ? null : (double?)dr["raising_factor"];
                            item.NSAPEnumeratorID = string.IsNullOrEmpty(dr["EnumeratorID"].ToString()) ? null : (int?)dr["EnumeratorID"];
                            item.EnumeratorText = dr["EnumeratorText"].ToString();

                            item.OperationIsSuccessful = (bool)dr["Success"];
                            item.OperationIsTracked = (bool)dr["Tracked"];
                            item.ODKRowID = dr["RowID"].ToString();
                            item.DepartureFromLandingSite = string.IsNullOrEmpty(dr["DepartureLandingSite"].ToString()) ? null : (DateTime?)dr["DepartureLandingSite"];
                            item.ArrivalAtLandingSite = string.IsNullOrEmpty(dr["ArrivalLandingSite"].ToString()) ? null : (DateTime?)dr["ArrivalLandingSite"];
                            item.XFormIdentifier = dr["XFormIdentifier"].ToString();
                            item.XFormDate = dr["XFormDate"] == DBNull.Value ? null : (DateTime?)dr["XFormDate"];
                            item.UserName = dr["user_name"].ToString();
                            item.DeviceID = dr["device_id"].ToString();
                            item.DateTimeSubmitted = (DateTime)dr["datetime_submitted"];
                            item.FormVersion = dr["form_version"].ToString();
                            item.GPSCode = dr["GPS"].ToString();
                            item.SamplingDate = (DateTime)dr["SamplingDate"];
                            item.Notes = dr["Notes"].ToString();
                            item.DateAddedToDatabase = dr["DateAdded"] == DBNull.Value ? null : (DateTime?)dr["DateAdded"];
                            item.FromExcelDownload = (bool)dr["FromExcelDownload"];
                            thisList.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);

                }
                return thisList;
            }
        }

        public bool Add(VesselUnload item)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into dbo_vessel_unload(v_unload_id, unload_gr_id,boat_id, boat_text, raising_factor,
                                                        catch_total,catch_samp,boxes_total,boxes_samp,is_boat_used)
                           Values 
                              (
                                {item.PK},
                                {item.GearUnloadID},
                                {(item.VesselID == null ? "null" : item.VesselID.ToString())},
                                '{item.VesselText}',
                                {(item.RaisingFactor == null ? "null" : item.RaisingFactor.ToString())},
                                {(item.WeightOfCatch == null ? "null" : item.WeightOfCatch.ToString())},
                                {(item.WeightOfCatchSample == null ? "null" : item.WeightOfCatchSample.ToString())},
                                {(item.Boxes == null ? "null" : item.Boxes.ToString())},
                                {(item.BoxesSampled == null ? "null" : item.BoxesSampled.ToString())},
                                {item.IsBoatUsed}
                             )";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    try
                    {
                        string dateAdded = item.DateAddedToDatabase == null ? "null" : ((DateTime)item.DateAddedToDatabase).ToString("MMM-dd-yyy HH:mm");
                        if (update.ExecuteNonQuery() > 0)
                        {
                            string departure = item.DepartureFromLandingSite == null ? "null" : $"'{((DateTime)item.DepartureFromLandingSite).ToString(_dateFormat)}'";
                            string arrival = item.ArrivalAtLandingSite == null ? "null" : $"'{((DateTime)item.ArrivalAtLandingSite).ToString(_dateFormat)}'";
                            string xFormDate = item.XFormDate == null ? "null" : $"'{((DateTime)item.XFormDate).ToString("MMM-dd-yyy HH:mm:ss")}'";
                            sql = $@"Insert into dbo_vessel_unload_1 
                                                (v_unload_id, Success, Tracked, DepartureLandingSite, ArrivalLandingSite, 
                                                RowID, XFormIdentifier, XFormDate, SamplingDate,
                                                user_name,device_id,datetime_submitted,form_version,
                                                GPS,Notes,EnumeratorID,EnumeratorText,DateAdded,sector_code,FromExcelDownload)
                                    Values (
                                        {item.PK},
                                        {item.OperationIsSuccessful},
                                        {item.OperationIsTracked},
                                        {departure},
                                        {arrival},
                                        {{{item.ODKRowID}}},
                                        '{item.XFormIdentifier}',
                                         {xFormDate},
                                        '{item.SamplingDate}',
                                        '{item.UserName}',
                                        '{item.DeviceID}',
                                        '{item.DateTimeSubmitted}',
                                        '{item.FormVersion}',
                                        '{item.GPSCode}',
                                        '{item.Notes}',
                                         {(item.NSAPEnumeratorID == null ? "null" : item.NSAPEnumeratorID.ToString())},
                                        '{item.EnumeratorText}',
                                        '{item.DateAddedToDatabase}',
                                        '{item.SectorCode}',
                                         {item.FromExcelDownload}
                                    )";
                            using (OleDbCommand update1 = new OleDbCommand(sql, conn))
                            {
                                try
                                {
                                    success = update1.ExecuteNonQuery() > 0;
                                }
                                catch (OleDbException dbex)
                                {
                                    Console.WriteLine(dbex.Message);
                                    success = false;
                                }
                                catch (Exception ex)
                                {
                                    Logger.Log(ex);
                                }
                            }
                        }
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
            return success;
        }

        public bool Update(VesselUnload item)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"UPDATE dbo_vessel_unload set
                        unload_gr_id = {item.Parent.PK},
                        boat_id = {(item.VesselID == null ? "null" : item.VesselID.ToString())}, 
                        boat_text = '{item.VesselText}', 
                        raising_factor = {(item.RaisingFactor == null ? "null" : item.RaisingFactor.ToString())},
                        catch_total = {(item.WeightOfCatch == null ? "null" : item.WeightOfCatch.ToString())},
                        catch_samp = {(item.WeightOfCatchSample == null ? "null" : item.WeightOfCatchSample.ToString())},
                        boxes_total = {(item.Boxes == null ? "null" : item.Boxes.ToString())},
                        boxes_samp = {(item.BoxesSampled == null ? "null" : item.BoxesSampled.ToString())},
                        is_boat_used  = {item.IsBoatUsed}
                        WHERE v_unload_id = {item.PK}";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    try
                    {
                        success = update.ExecuteNonQuery() > 0;
                        string departure = item.DepartureFromLandingSite == null ? "null" : $"'{((DateTime)item.DepartureFromLandingSite).ToString(_dateFormat)}'";
                        string arrival = item.ArrivalAtLandingSite == null ? "null" : $"'{((DateTime)item.ArrivalAtLandingSite).ToString(_dateFormat)}'";
                        string xFormDate = item.XFormDate == null ? "null" : $"'{((DateTime)item.XFormDate).ToString("MMM-dd-yyy HH:mm:ss")}'";
                        sql = $@"UPDATE dbo_vessel_unload_1 SET
                            Success = {item.OperationIsSuccessful},
                            Tracked =  {item.OperationIsTracked},
                            DepartureLandingSite = {departure},
                            ArrivalLandingSite = {arrival}, 
                            RowID =  {{{item.ODKRowID}}},
                            XFormIdentifier = '{item.XFormIdentifier}',
                            XFormDate = {xFormDate}, 
                            SamplingDate = '{item.SamplingDate}',
                            user_name = '{item.UserName}',
                            device_id = '{item.DeviceID}',
                            datetime_submitted = '{item.DateTimeSubmitted}',
                            form_version = '{item.FormVersion}',
                            GPS = '{item.GPSCode}',
                            Notes = '{item.Notes}',
                            EnumeratorID = {(item.NSAPEnumeratorID == null ? "null" : item.NSAPEnumeratorID.ToString())},
                            EnumeratorText = '{item.EnumeratorText}',
                            DateAdded = '{item.DateAddedToDatabase}',
                            sector_code = '{item.SectorCode}',
                            FromExcelDownload =  {item.FromExcelDownload}
                            WHERE v_unload_id = {item.PK}";


                        using (OleDbCommand update1 = new OleDbCommand(sql, conn))
                        {
                            success = false;
                            try
                            {
                                success = update1.ExecuteNonQuery() > 0;
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
            return success;
        }

        public bool ClearTable()
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $"Delete * from dbo_vessel_unload_1";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    try
                    {
                        update.ExecuteNonQuery();
                        sql = $"Delete * from dbo_vessel_unload";
                        using (OleDbCommand update1 = new OleDbCommand(sql, conn))
                        {
                            try
                            {
                                update1.ExecuteNonQuery();
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
        public bool Delete(int id)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $"Delete * from dbo_LC_FG_sample_day where unload_day_id={id}";
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
    }
}
