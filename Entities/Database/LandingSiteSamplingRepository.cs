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
                            item.LandingSiteID=dr["land_ctr_id"]==DBNull.Value?null:(int?)dr["land_ctr_id"];
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
                var sql = $@"Insert into dbo_LC_FG_sample_day(
                                unload_day_id, region_id,sdate, land_ctr_id,ground_id,
                                remarks,sampleday,land_ctr_text,fma)
                           Values (
                                {item.PK},
                                '{item.NSAPRegionID}',
                                '{item.SamplingDate.ToString(_dateFormat)}',
                                {(item.LandingSiteID==null?"null":item.LandingSiteID.ToString())},
                                '{item.FishingGroundID}', 
                                '{item.Remarks}',
                                {item.IsSamplingDay},
                                '{item.LandingSiteText}',
                                {item.FMAID}
                            )";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    try
                    {
                        success = update.ExecuteNonQuery() > 0;
                        if(success)
                        {
                            string dateSubmitted = item.DateSubmitted == null ? "null" : $@"'{item.DateSubmitted.ToString()}'";
                            string dateAdded = item.DateAdded== null ? "null" : $@"'{item.DateAdded.ToString()}'";
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
                                    ) Values (
                                        {item.PK} ,
                                        {(item.DateSubmitted==null ? "null" : dateSubmitted)} ,
                                        '{item.UserName}' ,
                                        '{item.DeviceID}' ,
                                        '{item.XFormIdentifier}' ,
                                        {(item.DateAdded == null ? "null" : dateAdded)} ,
                                        {item.FromExcelDownload} ,
                                        '{item.FormVersion}' ,
                                        '{item.RowID}' ,
                                        {(item.EnumeratorID == null ? "null" : item.EnumeratorID.ToString() )} ,
                                        '{item.EnumeratorText}' 
                                    )";
                             using (OleDbCommand update1 = new OleDbCommand(sql, conn))
                            {
                                try
                                {
                                    success = update1.ExecuteNonQuery() > 0;
                                }
                                catch(OleDbException)
                                {
                                    success = false;
                                }
                                catch(Exception ex)
                                {
                                    Logger.Log(ex);
                                }
                            }
                        }
                    }
                    catch(OleDbException )
                    {
                        success = false;
                    }
                    catch(Exception ex)
                    {
                        Logger.Log(ex);
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
                                land_ctr_id = {(item.LandingSiteID==null? "null" : item.LandingSiteID.ToString())},
                                ground_id = '{item.FishingGroundID}',
                                remarks = '{item.Remarks}',
                                sampleday = {item.IsSamplingDay},
                                land_ctr_text = '{item.LandingSiteText}',
                                fma = {item.FMAID}
                            WHERE unload_day_id = {item.PK}";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                    if(success)
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
                                        EnumeratorID = {(item.EnumeratorID==null ? "null" : item.EnumeratorID.ToString())},
                                        EnumeratorText = '{item.EnumeratorText}'
                                 WHERE unload_day_id = {item.PK}";
                        
                        using (OleDbCommand update1 = new OleDbCommand(sql, conn))
                        {
                            try
                            {
                                success = update1.ExecuteNonQuery() > 0;
                            }
                            catch(OleDbException)
                            {
                                success = false;
                            }
                            catch(Exception ex)
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
                        if(success)
                        {
                            sql = "Delete * from dbo_LC_FG_sample_day";
                            using (OleDbCommand update1 = new OleDbCommand(sql, conn))
                            {
                                try
                                {
                                    update1.ExecuteNonQuery();
                                    success = true;
                                }
                                catch(OleDbException)
                                {
                                    success = false;
                                }
                                catch(Exception ex)
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
