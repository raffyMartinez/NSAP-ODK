﻿using System;
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
                string vesselText = item.VesselText == null ? "" : item.VesselText;
                conn.Open();

                var sql = $@"Insert into dbo_vessel_unload(v_unload_id, unload_gr_id,boat_id, boat_text, raising_factor,
                            catch_total,catch_samp,boxes_total,boxes_samp,is_boat_used)
                           Values (?,?,?,?,?,?,?,?,?,?)";       
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {

                    update.Parameters.Add("@unloadid", OleDbType.Integer).Value = item.PK;
                    update.Parameters.Add("@gearUnload", OleDbType.Integer).Value = item.GearUnloadID;

                    if (item.VesselID == null)
                    {
                        update.Parameters.Add("@vesselid", OleDbType.Integer).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@vesselid", OleDbType.Integer).Value = item.VesselID;
                    }

                    update.Parameters.Add("@vesseltext", OleDbType.VarChar).Value = vesselText;
                    
                    if(item.RaisingFactor==null)
                    {
                        update.Parameters.Add("@raisingfactor", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@raisingfactor", OleDbType.Double).Value = item.RaisingFactor;
                    }

                    if (item.WeightOfCatch == null)
                    {
                        update.Parameters.Add("@wtcatch", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@wtcatch", OleDbType.Double).Value = item.WeightOfCatch;
                    }

                    if (item.WeightOfCatchSample == null)
                    {
                        update.Parameters.Add("@wtsample", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@wtsample", OleDbType.Double).Value = item.WeightOfCatchSample;
                    }

                    if (item.Boxes == null)
                    {
                        update.Parameters.Add("@boxes", OleDbType.Integer).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@boxes", OleDbType.Integer).Value = item.Boxes;
                    }

                    if (item.BoxesSampled == null)
                    {
                        update.Parameters.Add("@boxessampled", OleDbType.Integer).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@boxessampled", OleDbType.Integer).Value = item.Boxes;
                    }

                    update.Parameters.Add("@isboatused", OleDbType.Boolean).Value = item.IsBoatUsed;



                    try
                    {
                        string dateAdded = item.DateAddedToDatabase == null ? "null" : ((DateTime)item.DateAddedToDatabase).ToString("MMM-dd-yyy HH:mm");
                        if (update.ExecuteNonQuery() > 0)
                        {

                            sql = $@"Insert into dbo_vessel_unload_1 
                                                (v_unload_id, Success, Tracked, DepartureLandingSite, ArrivalLandingSite, 
                                                RowID, XFormIdentifier, XFormDate, SamplingDate,
                                                user_name,device_id,datetime_submitted,form_version,
                                                GPS,Notes,EnumeratorID,EnumeratorText,DateAdded,sector_code,FromExcelDownload)
                                    Values (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";

                            using (OleDbCommand update1 = new OleDbCommand(sql, conn))
                            {
                                update1.Parameters.Add("@unloadid", OleDbType.Integer).Value = item.PK;
                                update1.Parameters.Add("@operation_success", OleDbType.Boolean).Value = item.OperationIsSuccessful;
                                update1.Parameters.Add("@operation_tracked", OleDbType.Boolean).Value = item.OperationIsTracked;

                                if (item.DepartureFromLandingSite == null)
                                {
                                    update1.Parameters.Add("@departure_date", OleDbType.Date).Value = DBNull.Value;
                                }
                                else
                                {
                                    update1.Parameters.Add("@departure_date", OleDbType.Date).Value = item.DepartureFromLandingSite;
                                }

                                if (item.ArrivalAtLandingSite == null)
                                {
                                    update1.Parameters.Add("@arrival_date", OleDbType.Date).Value = DBNull.Value;
                                }
                                else
                                {
                                    update1.Parameters.Add("@arrival_date", OleDbType.Date).Value = item.ArrivalAtLandingSite;
                                }



                                update1.Parameters.Add("@row_id", OleDbType.Guid).Value = Guid.Parse(item.ODKRowID);

                                string xformID = item.XFormIdentifier == null ? "" : item.XFormIdentifier;
                                update1.Parameters.Add("@xform_id", OleDbType.VarChar).Value = xformID;


                                if (item.XFormDate == null)
                                {
                                    update1.Parameters.Add("@xform_date", OleDbType.Date).Value = DBNull.Value;
                                }
                                else
                                {
                                    update1.Parameters.Add("@xform_date", OleDbType.Date).Value = item.XFormDate;
                                }

                                update1.Parameters.Add("@sampling_date", OleDbType.Date).Value = item.SamplingDate;



                                update1.Parameters.Add("@user_name", OleDbType.VarChar).Value = item.UserName;
                                update1.Parameters.Add("@device_id", OleDbType.VarChar).Value = item.DeviceID;
                                update1.Parameters.Add("@date_submitted", OleDbType.Date).Value = item.DateTimeSubmitted;
                                update1.Parameters.Add("@form_version", OleDbType.VarChar).Value = item.FormVersion;


                                if (item.GPSCode == null )
                                {
                                    update1.Parameters.Add("@gps", OleDbType.VarChar).Value = "";
                                }
                                else
                                {
                                    update1.Parameters.Add("@gps", OleDbType.VarChar).Value = item.GPSCode;
                                }

                                string notes = item.Notes == null ? "" : item.Notes;
                                update1.Parameters.Add("@notes", OleDbType.VarChar).Value = notes;

                                if (item.NSAPEnumeratorID == null)
                                {
                                    update1.Parameters.Add("@enumerator", OleDbType.Integer).Value = DBNull.Value;
                                }
                                else
                                {
                                    update1.Parameters.Add("@enumerator", OleDbType.Integer).Value = item.NSAPEnumeratorID; ;
                                }

                                string en_text = item.EnumeratorText== null ? "" : item.EnumeratorText;
                                update1.Parameters.Add("@enumerator_text", OleDbType.VarChar).Value = en_text;
                                
                                update1.Parameters.Add("@date_added", OleDbType.Date).Value = item.DateAddedToDatabase;

                                string sector = item.SectorCode == null ? "" : item.SectorCode;
                                update1.Parameters.Add("@sector_code", OleDbType.VarChar).Value = sector;

                                update1.Parameters.Add("@from_excel", OleDbType.Boolean).Value = item.FromExcelDownload;

                                
                                try
                                {
                                    success = update1.ExecuteNonQuery() > 0;
                                }
                                catch (OleDbException dbex)
                                {
                                    if (dbex.ErrorCode == -2147217900)
                                    {
                                        AddFieldToTable(dbex.Message);
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
            return success;
        }

        private void AddFieldToTable(string errorMessage)
        {
            int s1 = errorMessage.IndexOf("name: '");
            int s2 = errorMessage.IndexOf("'.  Make");
            string newField = errorMessage.Substring(s1 + 7, s2-(s1+7));
            int y;
        }


        public bool Update(VesselUnload item)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                using (OleDbCommand cmd = conn.CreateCommand())
                {
                    cmd.Parameters.Add("@Unload_Gear_id", OleDbType.Integer).Value = item.Parent.PK;
                    if (item.VesselID == null)
                    {
                        cmd.Parameters.Add("@Boat_ID", OleDbType.Integer).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@Boat_ID", OleDbType.Integer).Value = item.VesselID;
                    }
                    cmd.Parameters.Add("@Boat_text", OleDbType.VarChar).Value = item.VesselText;
                    
                    if(item.RaisingFactor==null)
                    {
                        cmd.Parameters.Add("@Raising_factor", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@Raising_factor", OleDbType.Double).Value = item.RaisingFactor;
                    }

                    if(item.WeightOfCatch==null)
                    {
                        cmd.Parameters.Add("@Weight_catch", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@Weight_catch", OleDbType.Double).Value = item.WeightOfCatch;
                    }

                    if(item.WeightOfCatchSample==null)
                    {
                        cmd.Parameters.Add("@Weight_sample", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@Weight_sample", OleDbType.Double).Value = item.WeightOfCatchSample;
                    }

                    if(item.Boxes==null)
                    {
                        cmd.Parameters.Add("@Boxes_count", OleDbType.Integer).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@Boxes_count", OleDbType.Integer).Value = item.Boxes;
                    }

                    if (item.BoxesSampled == null)
                    {
                        cmd.Parameters.Add("@Boxes_sampled", OleDbType.Integer).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@Boxes_sampled", OleDbType.Integer).Value = item.BoxesSampled;
                    }

                    cmd.Parameters.Add("@Is_boat_used", OleDbType.Boolean).Value = item.IsBoatUsed;
                    cmd.Parameters.Add("@Unload_id", OleDbType.Integer).Value = item.PK;


                    cmd.CommandText = $@"UPDATE dbo_vessel_unload set
                        unload_gr_id = @Unload_Gear_id,
                        boat_id = @Boat_ID, 
                        boat_text = @Boat_text, 
                        raising_factor = @Raising_factor,
                        catch_total = @Weight_catch,
                        catch_samp = @Weight_sample,
                        boxes_total = @Boxes_count,
                        boxes_samp = @Boxes_sampled,
                        is_boat_used  = @Is_boat_used
                        WHERE v_unload_id = @Unload_id";
                    try
                    {
                        success = cmd.ExecuteNonQuery() > 0;
                        using (OleDbCommand cmd_1 = conn.CreateCommand())
                        {
                            cmd_1.Parameters.Add("@Operation_success", OleDbType.Boolean).Value = item.OperationIsSuccessful;
                            cmd_1.Parameters.Add("@Operation_is_tracked", OleDbType.Boolean).Value = item.OperationIsTracked;
                            if (item.DepartureFromLandingSite == null)
                            {
                                cmd_1.Parameters.Add("@Departure_date", OleDbType.Date).Value = DBNull.Value;
                            }
                            else
                            {
                                cmd_1.Parameters.Add("@Departure_date", OleDbType.Date).Value = item.DepartureFromLandingSite;
                            }
                            if (item.ArrivalAtLandingSite == null)
                            {
                                cmd_1.Parameters.Add("@Arrival_date", OleDbType.Date).Value = DBNull.Value;
                            }
                            else
                            {
                                cmd_1.Parameters.Add("@Arrival_date", OleDbType.Date).Value = item.ArrivalAtLandingSite;
                            }
                            cmd_1.Parameters.Add("@ODK_row_id", OleDbType.VarChar).Value = item.ODKRowID;
                            if (item.XFormIdentifier == null)
                            {
                                cmd_1.Parameters.Add("@XForm_id", OleDbType.VarChar).Value = DBNull.Value;
                            }
                            else
                            {
                                cmd_1.Parameters.Add("@XForm_id", OleDbType.VarChar).Value = item.XFormIdentifier;
                            }
                            if (item.XFormDate == null)
                            {
                                cmd_1.Parameters.Add("@Xform_date", OleDbType.Date).Value = DBNull.Value;
                            }
                            else
                            {
                                cmd_1.Parameters.Add("@Xform_date", OleDbType.Date).Value = item.XFormDate;
                            }
                            cmd_1.Parameters.Add("@Sampling_date", OleDbType.Date).Value = item.SamplingDate;
                            cmd_1.Parameters.Add("@User_name", OleDbType.VarChar).Value = item.UserName;
                            cmd_1.Parameters.Add("@Device_id", OleDbType.VarChar).Value = item.DeviceID;
                            cmd_1.Parameters.Add("@Date_submitted", OleDbType.Date).Value = item.DateTimeSubmitted;
                            cmd_1.Parameters.Add("@Form_version", OleDbType.VarChar).Value = item.FormVersion;
                            if (item.GPSCode == null)
                            {
                                 cmd_1.Parameters.Add("@GPS_code", OleDbType.VarChar).Value = DBNull.Value;
                            }
                            else
                            {
                                cmd_1.Parameters.Add("@GPS_code", OleDbType.VarChar).Value = item.GPSCode;
                            }
                            if (item.Notes == null)
                            {
                                cmd_1.Parameters.Add("@Notes", OleDbType.VarChar).Value = DBNull.Value;
                            }
                            else
                            {
                                cmd_1.Parameters.Add("@Notes", OleDbType.VarChar).Value = item.Notes;
                            }
                            if (item.NSAPEnumeratorID == null)
                            {
                                cmd_1.Parameters.Add("@Enumerator_id", OleDbType.Integer).Value = DBNull.Value;
                            }
                            else
                            {
                                cmd_1.Parameters.Add("@Enumerator_id", OleDbType.Integer).Value = item.NSAPEnumeratorID;
                            }
                            cmd_1.Parameters.Add("@Enumerator_text", OleDbType.VarChar).Value = item.EnumeratorText;
                            cmd_1.Parameters.Add("@Date_added", OleDbType.Date).Value = item.DateAddedToDatabase;
                            cmd_1.Parameters.Add("@Sector_code", OleDbType.VarChar).Value = item.SectorCode;
                            cmd_1.Parameters.Add("@From_excel", OleDbType.Boolean).Value = item.FromExcelDownload;
                            cmd_1.Parameters.Add("@Vessel_unload_id", OleDbType.Integer).Value = item.PK;

                            cmd_1.CommandText = $@"UPDATE dbo_vessel_unload_1 SET
                                        Success = @Operation_success,
                                        Tracked =  @Operation_is_tracked,
                                        DepartureLandingSite = @Departure_date,
                                        ArrivalLandingSite = @Arrival_date, 
                                        RowID =  @ODK_row_id,
                                        XFormIdentifier = @XForm_id,
                                        XFormDate = @Xform_date, 
                                        SamplingDate = @Sampling_date,
                                        user_name = @User_name,
                                        device_id = @Device_id,
                                        datetime_submitted = @Date_submitted,
                                        form_version = @Form_version,
                                        GPS = @GPS_code,
                                        Notes = @Notes,
                                        EnumeratorID = @Enumerator_id,
                                        EnumeratorText = @Enumerator_text,
                                        DateAdded = @Date_added,
                                        sector_code = @Sector_code,
                                        FromExcelDownload =  @From_excel
                                        WHERE v_unload_id =@Vessel_unload_id";


                            success = false;

                            try
                            {
                                success = cmd_1.ExecuteNonQuery() > 0;
                            }
                            catch(OleDbException dbex)
                            {
                                Logger.Log(dbex);
                                //ignore
                            }
                            catch(Exception ex)
                            {
                                Logger.Log(ex);
                            }
                        }

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
                var sql = $"Delete * from dbo_vessel_unload_1 where v_unload_id={id}";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    try
                    {
                        success = update.ExecuteNonQuery() > 0;
                        sql = $"Delete * from dbo_vessel_unload where v_unload_id={id}";
                        using (OleDbCommand update1 = new OleDbCommand(sql, conn))
                        {
                            try
                            {
                                success = update1.ExecuteNonQuery()>0;
                                
                            }
                            catch (OleDbException odbex)
                            {
                                Logger.Log(odbex);
                            }
                            catch (Exception ex)
                            {
                                Logger.Log(ex);
                            }
                        }
                    }
                    catch (OleDbException odbex)
                    {
                        Logger.Log(odbex);
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
