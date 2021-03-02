using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using NSAP_ODK.Entities;
using NSAP_ODK.Utilities;
namespace NSAP_ODK.Entities.Database
{
    class LandingSiteSamplingRepository
    {
        private string _dateFormat = "MMM-dd-yyyy";
        public List<LandingSiteSampling> LandingSiteSamplings { get; set; }

        public LandingSiteSamplingRepository()
        {
            LandingSiteSamplings = getLandingSiteSamplings();
        }
        public int MaxRecordNumber()
        {
            int max_rec_no = 0;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                const string sql = "SELECT Max(unload_day_id) AS max_id FROM dbo_LC_FG_sample_day";
                using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                {
                    max_rec_no = (int)getMax.ExecuteScalar();
                }
            }
            return max_rec_no;
        }
        private List<LandingSiteSampling> getLandingSiteSamplings()
        {
            List<LandingSiteSampling> thisList = new List<LandingSiteSampling>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = @"SELECT dbo_LC_FG_sample_day.*, 
                                        dbo_LC_FG_sample_day_1.datetime_submitted, 
                                        dbo_LC_FG_sample_day_1.user_name, 
                                        dbo_LC_FG_sample_day_1.device_id, 
                                        dbo_LC_FG_sample_day_1.XFormIdentifier, 
                                        dbo_LC_FG_sample_day_1.DateAdded, 
                                        dbo_LC_FG_sample_day_1.FromExcelDownload, 
                                        dbo_LC_FG_sample_day_1.form_version, 
                                        dbo_LC_FG_sample_day_1.RowID, 
                                        dbo_LC_FG_sample_day_1.EnumeratorID, 
                                        dbo_LC_FG_sample_day_1.EnumeratorText
                                        FROM dbo_LC_FG_sample_day 
                                            LEFT JOIN dbo_LC_FG_sample_day_1 
                                            ON dbo_LC_FG_sample_day.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id";


                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        thisList.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            LandingSiteSampling item = new LandingSiteSampling();
                            item.PK = (int)dr["unload_day_id"];
                            item.NSAPRegionID = dr["region_id"].ToString();
                            item.SamplingDate = (DateTime)dr["sdate"];
                            //item.LandingSiteID = string.IsNullOrEmpty( dr["land_ctr_id"].ToString())?null:(int?)dr["land_ctr_id"];
                            item.LandingSiteID = dr["land_ctr_id"] == DBNull.Value ? null : (int?)dr["land_ctr_id"];
                            item.FishingGroundID = dr["ground_id"].ToString();
                            item.Remarks = dr["remarks"].ToString();
                            item.IsSamplingDay = (bool)dr["sampleday"];
                            item.LandingSiteText = dr["land_ctr_text"].ToString();
                            item.FMAID = (int)dr["fma"];
                            item.DateSubmitted = dr["datetime_submitted"] == DBNull.Value ? null : (DateTime?)dr["datetime_submitted"];
                            item.UserName = dr["user_name"].ToString();
                            item.DeviceID = dr["device_id"].ToString();
                            item.XFormIdentifier = dr["XFormIdentifier"].ToString();
                            item.DateAdded = dr["DateAdded"] == DBNull.Value ? null : (DateTime?)dr["DateAdded"];
                            item.FromExcelDownload = dr["FromExcelDownload"] == DBNull.Value ? false : (bool)dr["FromExcelDownload"];
                            item.FormVersion = dr["form_version"].ToString();
                            item.RowID = dr["RowID"].ToString();
                            item.EnumeratorID = dr["EnumeratorID"] == DBNull.Value ? null : (int?)int.Parse(dr["EnumeratorID"].ToString());
                            item.EnumeratorText = dr["EnumeratorText"].ToString();
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

        public bool Add(LandingSiteSampling item)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                //var sql = $@"Insert into dbo_LC_FG_sample_day(
                //                unload_day_id, region_id,sdate, land_ctr_id,ground_id,
                //                remarks,sampleday,land_ctr_text,fma)
                //           Values (
                //                {item.PK},
                //                '{item.NSAPRegionID}',
                //                '{item.SamplingDate.ToString(_dateFormat)}',
                //                {(item.LandingSiteID == null ? "null" : item.LandingSiteID.ToString())},
                //                '{item.FishingGroundID}', 
                //                '{item.Remarks}',
                //                {item.IsSamplingDay},
                //                '{item.LandingSiteText}',
                //                {item.FMAID}
                //            )";

                var sql = $@"Insert into dbo_LC_FG_sample_day(
                                unload_day_id, region_id,sdate, land_ctr_id,ground_id,
                                remarks,sampleday,land_ctr_text,fma)
                           Values (?,?,?,?,?,?,?,?,?,)";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    try
                    {
                        update.Parameters.Add("@pk", OleDbType.Integer).Value = item.PK;
                        update.Parameters.Add("@nsap_region", OleDbType.VarChar).Value = item.NSAPRegionID;
                        update.Parameters.Add("@sampling_date", OleDbType.Date).Value = item.SamplingDate;

                        if (item.LandingSiteID == null)
                        {
                            update.Parameters.Add("@landing_site_id", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@landing_site_id", OleDbType.VarChar).Value = item.LandingSiteID;
                        }

                        update.Parameters.Add("@fg", OleDbType.VarChar).Value = item.FishingGroundID;
                        update.Parameters.Add("@remarks", OleDbType.VarChar).Value = item.Remarks;
                        update.Parameters.Add("@is_sampling_day", OleDbType.Boolean).Value = item.IsSamplingDay;
                        update.Parameters.Add("@landing_site_text", OleDbType.Boolean).Value = item.LandingSiteText;
                        update.Parameters.Add("@fma_id", OleDbType.Integer).Value = item.FMAID;


                        success = update.ExecuteNonQuery() > 0;
                        if (success)
                        {
                            string dateSubmitted = item.DateSubmitted == null ? "null" : $@"'{item.DateSubmitted.ToString()}'";
                            string dateAdded = item.DateAdded == null ? "null" : $@"'{item.DateAdded.ToString()}'";
                            //sql = $@"Insert into dbo_LC_FG_sample_day_1  (
                            //            unload_day_id,
                            //            datetime_submitted,
                            //            user_name,
                            //            device_id,
                            //            XFormIdentifier,
                            //            DateAdded,
                            //            FromExcelDownload,
                            //            form_version,
                            //            RowID,
                            //            EnumeratorID,
                            //            EnumeratorText
                            //        ) Values (
                            //            {item.PK} ,
                            //            {(item.DateSubmitted == null ? "null" : dateSubmitted)} ,
                            //            '{item.UserName}' ,
                            //            '{item.DeviceID}' ,
                            //            '{item.XFormIdentifier}' ,
                            //            {(item.DateAdded == null ? "null" : dateAdded)} ,
                            //            {item.FromExcelDownload} ,
                            //            '{item.FormVersion}' ,
                            //            '{item.RowID}' ,
                            //            {(item.EnumeratorID == null ? "null" : item.EnumeratorID.ToString())} ,
                            //            '{item.EnumeratorText}' 
                            //        )";

                            sql = $@"Insert into dbo_LC_FG_sample_day_1  (
                                        unload_day_id,
                                        datetime_submitted,
                                        user_name,
                                        device_id,
                                        XFormIdentifier,
                                        DateAdded,
                                        FromExcelDownload,
                                        form_version,
                                        RowID,
                                        EnumeratorID,
                                        EnumeratorText
                                    ) Values (?,?,?,?,?,?,?,?,?,?,?,)";


                            using (OleDbCommand update1 = new OleDbCommand(sql, conn))
                            {
                                update1.Parameters.Add("@pk", OleDbType.Integer).Value = item.PK;

                                if (item.DateSubmitted == null)
                                {
                                    update1.Parameters.Add("@date_submitted", OleDbType.Date).Value = DBNull.Value;
                                }
                                else
                                {
                                    update1.Parameters.Add("@date_submitted", OleDbType.Date).Value = item.DateSubmitted;
                                }

                                update1.Parameters.Add("@user", OleDbType.VarChar).Value = item.UserName;
                                update1.Parameters.Add("@device_id", OleDbType.VarChar).Value = item.DeviceID;
                                update1.Parameters.Add("@xform_id", OleDbType.VarChar).Value = item.XFormIdentifier;

                                if (item.DateAdded == null)
                                {
                                    update1.Parameters.Add("@date_added", OleDbType.Date).Value = DBNull.Value;
                                }
                                else
                                {
                                    update1.Parameters.Add("@date_added", OleDbType.Date).Value = item.DateAdded;
                                }

                                update1.Parameters.Add("@fromExcel", OleDbType.Boolean).Value = item.FromExcelDownload;
                                update1.Parameters.Add("@form_version", OleDbType.VarChar).Value = item.FormVersion;
                                update1.Parameters.Add("@row_id", OleDbType.VarChar).Value = item.RowID;

                                if (item.EnumeratorID == null)
                                {
                                    update1.Parameters.Add("@enum_id", OleDbType.Integer).Value = DBNull.Value;
                                }
                                else
                                {
                                    update1.Parameters.Add("@enum_id", OleDbType.Integer).Value = item.EnumeratorID;
                                }

                                update1.Parameters.Add("@enum_text", OleDbType.VarChar).Value = item.EnumeratorText;

                                try
                                {
                                    success = update1.ExecuteNonQuery() > 0;
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
                    catch (OleDbException odbex)
                    {
                        Logger.Log(odbex);
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

        public bool Update(LandingSiteSampling item)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update dbo_LC_FG_sample_day set
                                region_id='{item.NSAPRegionID}',
                                sdate = '{item.SamplingDate.ToString(_dateFormat)}',
                                land_ctr_id = {(item.LandingSiteID == null ? item.LandingSite == null ? "null" : item.LandingSite.LandingSiteID.ToString() : item.LandingSiteID.ToString())},
                                ground_id = '{item.FishingGroundID}',
                                remarks = '{item.Remarks}',
                                sampleday = {item.IsSamplingDay},
                                land_ctr_text = '{item.LandingSiteText}',
                                fma = {item.FMAID}
                            WHERE unload_day_id = {item.PK}";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                    if (success && (item.XFormIdentifier != null && item.XFormIdentifier.Length > 0) || (item.Remarks!=null &&  item.Remarks.Contains("orphaned")))
                    {
                        string dateSubmitted = item.DateSubmitted == null ? "null" : $@"'{item.DateSubmitted.ToString()}'";
                        string dateAdded = item.DateAdded == null ? "null" : $@"'{item.DateAdded.ToString()}'";

                        sql = $@"Update dbo_LC_FG_sample_day_1 set
                                        datetime_submitted = {dateSubmitted},
                                        user_name = '{item.UserName}',
                                        device_id = '{item.DeviceID}',
                                        XFormIdentifier = '{item.XFormIdentifier}',
                                        DateAdded = {dateAdded},
                                        FromExcelDownload = {item.FromExcelDownload},
                                        form_version = '{item.FormVersion}',
                                        RowID = '{item.RowID}',
                                        EnumeratorID = {(item.EnumeratorID == null ? "null" : item.EnumeratorID.ToString())},
                                        EnumeratorText = '{item.EnumeratorText}'
                                 WHERE unload_day_id = {item.PK}";

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
                var sql = "Delete * from dbo_LC_FG_sample_day_1";

                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    try
                    {
                        update.ExecuteNonQuery();
                        success = true;
                        if (success)
                        {
                            sql = "Delete * from dbo_LC_FG_sample_day";
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

                var sql = $"Delete * from dbo_LC_FG_sample_day_1 where unload_day_id={id}";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    try
                    {
                        success = update.ExecuteNonQuery() > 0;

                        sql = $"Delete * from dbo_LC_FG_sample_day where unload_day_id={id}";
                        using (OleDbCommand update1 = new OleDbCommand(sql, conn))
                        {
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
    }
}
