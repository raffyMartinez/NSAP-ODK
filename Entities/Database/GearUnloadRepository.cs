﻿using System;
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
    class GearUnloadRepository
    {
        public static event EventHandler<ProcessingItemsEventArg> ProcessingItemsEvent;
        public List<GearUnload> GearUnloads { get; set; }

        public GearUnloadRepository(LandingSiteSampling ls)
        {
            GearUnloads = getGearUnloads(ls);
        }

        public static List<GearUnload> GetAllGearUnloads()
        {
            List<GearUnload> allGearUnloads = new List<GearUnload>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "Select * from dbo_gear_unload order by unload_gr_id";
                        con.Open();
                        try
                        {
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                GearUnload gu = new GearUnload
                                {
                                    LandingSiteSamplingID = (int)dr["unload_day_id"],
                                    PK = (int)dr["unload_gr_id"],
                                    GearID = dr["gr_id"].ToString(),
                                    Boats = string.IsNullOrEmpty(dr["boats"].ToString()) ? null : (int?)dr["boats"],
                                    Catch = string.IsNullOrEmpty(dr["catch"].ToString()) ? null : (double?)dr["catch"],
                                    SpeciesWithTWSpCount = string.IsNullOrEmpty(dr["sp_twsp_count"].ToString()) ? null : (int?)dr["sp_twsp_count"],
                                    GearUsedText = dr["gr_text"].ToString(),
                                    Remarks = dr["remarks"].ToString(),
                                    SectorCode = dr["sector"].ToString()
                                };
                                //TotalWtSpViewModel = new TotalWtSpViewModel(item),
                                if (dr["gear_count_commercial"] != DBNull.Value)
                                {
                                    gu.NumberOfCommercialLandings = (int)dr["gear_count_commercial"];
                                }
                                if (dr["gear_count_municipal"] != DBNull.Value)
                                {
                                    gu.NumberOfMunicipalLandings = (int)dr["gear_count_municipal"];
                                }
                                if (dr["gear_catch_municipal"] != DBNull.Value)
                                {
                                    gu.WeightOfMunicipalLandings = (double)dr["gear_catch_municipal"];
                                }
                                if (dr["gear_catch_commercial"] != DBNull.Value)
                                {
                                    gu.WeightOfCommercialLandings = (double)dr["gear_catch_commercial"];
                                }
                                if (dr["gear_sequence"] != DBNull.Value)
                                {
                                    gu.Sequence = (int)dr["gear_sequence"];
                                }

                                allGearUnloads.Add(gu);
                            };
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return allGearUnloads;
        }
        public GearUnloadRepository(bool fetch = true)
        {
            if (fetch)
            {
                GearUnloads = getGearUnloads();
            }
            else
            {
                GearUnloads = new List<GearUnload>();
            }
        }

        public static List<GearUnload> GetGearUnloadsForCrosstab(NSAP_ODK.TreeViewModelControl.AllSamplingEntitiesEventHandler e)
        {
            List<GearUnload> gus = new List<GearUnload>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {

                        DateTime sDate = (DateTime)e.MonthSampled;
                        cmd.Parameters.AddWithValue("@start", sDate.ToString("MMM/dd/yyyy"));
                        cmd.Parameters.AddWithValue("@end", sDate.AddMonths(1).ToString("MMM/dd/yyyy"));
                        cmd.Parameters.AddWithValue("@reg", e.NSAPRegion.Code);
                        cmd.Parameters.AddWithValue("@fma", e.FMA.FMAID);
                        cmd.Parameters.AddWithValue("@fg", e.FishingGround.Code);
                        cmd.Parameters.AddWithValue("@ls", e.LandingSite.LandingSiteID);

                        //cmd.CommandText = @"SELECT 
                        //                        dbo_gear_unload.gr_id, 
                        //                        dbo_gear_unload.gear_count_municipal, 
                        //                        dbo_gear_unload.gear_count_commercial, 
                        //                        dbo_gear_unload.gear_catch_municipal, 
                        //                        dbo_gear_unload.gear_catch_commercial, 
                        //                        dbo_gear_unload.unload_gr_id
                        //                   FROM 
                        //                        dbo_LC_FG_sample_day INNER JOIN 
                        //                        dbo_gear_unload ON 
                        //                        dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id
                        //                   WHERE 
                        //                        dbo_LC_FG_sample_day.sdate>=@start AND 
                        //                        dbo_LC_FG_sample_day.sdate<@end AND 
                        //                        dbo_LC_FG_sample_day.region_id=@reg 
                        //                        AND dbo_LC_FG_sample_day.fma=@fma AND 
                        //                        dbo_LC_FG_sample_day.ground_id=@fg AND 
                        //                        dbo_LC_FG_sample_day.land_ctr_id=@ls";

                        cmd.CommandText = @"SELECT 
                                                dbo_gear_unload.gr_id, 
                                                dbo_gear_unload.gear_count_municipal, 
                                                dbo_gear_unload.gear_count_commercial, 
                                                dbo_gear_unload.gear_catch_municipal, 
                                                dbo_gear_unload.gear_catch_commercial, 
                                                dbo_gear_unload.unload_gr_id, 
                                                Count(dbo_vessel_unload.v_unload_id) AS count_sampled_landings
                                            FROM 
                                                (dbo_LC_FG_sample_day INNER JOIN 
                                                dbo_gear_unload ON 
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN 
                                                dbo_vessel_unload ON 
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id
                                            WHERE 
                                                dbo_LC_FG_sample_day.sdate>=@start AND 
                                                dbo_LC_FG_sample_day.sdate<@end AND 
                                                dbo_LC_FG_sample_day.region_id=@reg 
                                                AND dbo_LC_FG_sample_day.fma=@fma AND 
                                                dbo_LC_FG_sample_day.ground_id=@fg AND 
                                                dbo_LC_FG_sample_day.land_ctr_id=@ls
                                            GROUP BY
                                                dbo_gear_unload.gr_id, 
                                                dbo_gear_unload.gear_count_municipal, 
                                                dbo_gear_unload.gear_count_commercial, 
                                                dbo_gear_unload.gear_catch_municipal, 
                                                dbo_gear_unload.gear_catch_commercial, 
                                                dbo_gear_unload.unload_gr_id";
                        con.Open();
                        try
                        {
                            OleDbDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                GearUnload gu = new GearUnload
                                {
                                    PK = (int)dr["unload_gr_id"],
                                    GearID = dr["gr_id"].ToString(),

                                };
                                if (dr["count_sampled_landings"] != DBNull.Value)
                                {
                                    gu.NumberOfSampledLandingsEx = (int)dr["count_sampled_landings"];
                                }
                                if (dr["gear_count_municipal"] != DBNull.Value && (int)dr["gear_count_municipal"] > 0)
                                {
                                    gu.NumberOfMunicipalLandings = (int)dr["gear_count_municipal"];
                                }
                                if (dr["gear_count_commercial"] != DBNull.Value && (int)dr["gear_count_commercial"] > 0)
                                {
                                    gu.NumberOfCommercialLandings = (int)dr["gear_count_commercial"];
                                }
                                if (dr["gear_catch_municipal"] != DBNull.Value && (double)dr["gear_catch_municipal"] > 0)
                                {
                                    gu.WeightOfMunicipalLandings = (double)dr["gear_catch_municipal"];
                                }
                                if (dr["gear_catch_commercial"] != DBNull.Value && (double)dr["gear_catch_commercial"] > 0)
                                {
                                    gu.WeightOfCommercialLandings = (double)dr["gear_catch_commercial"];
                                }
                                gus.Add(gu);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return gus;
        }
        public static int MaxRecordNumberFromDB()
        {
            int maxRecNo = 0;
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
                        cmd.CommandText = "SELECT Max(unload_gr_id) AS max_id FROM dbo_gear_unload";
                        try
                        {
                            maxRecNo = (int)cmd.ExecuteScalar();
                        }
                        catch (Exception ex)
                        {
                            maxRecNo = 0;
                        }
                    }
                }
            }
            return maxRecNo;
        }

        public static bool EditRemarkOfGearUnload(int gearUnloadID, string remark)
        {
            bool success = false;
            using (var con = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = con.CreateCommand())
                {

                    cmd.Parameters.AddWithValue("@remark", remark);
                    //cmd.Parameters.Add("@remark", OleDbType.VarChar).Value = remark;
                    cmd.Parameters.AddWithValue("@id", gearUnloadID);
                    cmd.CommandText = "UPDATE dbo_gear_unload SET remarks=@remark WHERE unload_gr_id=@id";
                    try
                    {
                        con.Open();
                        success = cmd.ExecuteNonQuery() > 0;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return success;
        }
        public int MaxRecordNumber()
        {
            return NSAPEntities.SummaryItemViewModel.GetGearUnloadMaxRecordNumber();
        }
        public int MaxRecordNumber1()
        {
            int max_rec_no = 0;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = "SELECT Max(unload_gr_id) AS max_id FROM dbo_gear_unload";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(unload_gr_id) AS max_id FROM dbo_gear_unload";
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
                        catch (OleDbException olx)
                        {
                            Logger.Log(olx);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return max_rec_no;
        }

        private List<GearUnload> getFromMySQL(LandingSiteSampling ls = null)
        {
            List<GearUnload> thisList = new List<GearUnload>();
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = "Select * from dbo_gear_unload";
                    if (ls != null)
                    {
                        cmd.Parameters.AddWithValue("@parentID", ls.PK);
                        cmd.CommandText = $"Select * from dbo_gear_unload where unload_day_id=@parentID";
                    }
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        GearUnload item = new GearUnload();
                        item.Parent = ls;
                        item.PK = (int)dr["unload_gr_id"];
                        item.LandingSiteSamplingID = (int)dr["unload_day_id"];
                        item.GearID = dr["gr_id"].ToString();
                        item.Boats = string.IsNullOrEmpty(dr["boats"].ToString()) ? null : (int?)dr["boats"];
                        item.Catch = string.IsNullOrEmpty(dr["catch"].ToString()) ? null : (double?)dr["catch"];
                        item.GearUsedText = dr["gr_text"].ToString();
                        item.Remarks = dr["remarks"].ToString();
                        thisList.Add(item);
                    }
                }
            }
            return thisList;
        }


        public static bool UpdateGearOfUnload(string gear_code, VesselEffort ve)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var conn = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Parameters.Add("@gear_code", OleDbType.VarChar).Value = gear_code;
                        cmd.Parameters.Add("@gu_id", OleDbType.Integer).Value = ve.Parent.Parent.PK;
                        cmd.CommandText = "Update dbo_gear_unload set gr_id=@gear_code where unload_gr_id=@gu_id";
                        try
                        {
                            conn.Open();
                            success = cmd.ExecuteNonQuery() > 0;
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
        public static int GearUnloadCount(bool countCompleted = false)
        {
            int count = 0;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select count(*) from dbo_gear_unload";
                        if (countCompleted)
                        {
                            cmd.CommandText = "Select count(*) from dbo_gear_unload where boats Is Not null And catch Is Not Null";
                        }
                        try
                        {
                            conn.Open();
                            count = (int)(long)cmd.ExecuteScalar();
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
                using (var conn = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select count(*) from dbo_gear_unload";
                        if (countCompleted)
                        {
                            cmd.CommandText = "Select count(*) from dbo_gear_unload where boats Is Not null And catch Is Not Null";
                        }
                        if (Global.Filter1 != null)
                        {
                            cmd.Parameters.AddWithValue("@d1", Global.Filter1DateString());
                            if (countCompleted)
                            {
                                cmd.CommandText = @"SELECT count(*) as n
                                                    FROM dbo_LC_FG_sample_day INNER JOIN 
                                                        dbo_gear_unload ON dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id
                                                    WHERE boats Is Not null AND catch Is Not Null AND dbo_LC_FG_sample_day.sdate >= @d1";
                            }
                            else
                            {
                                cmd.CommandText = @"SELECT count(*) as n
                                                    FROM dbo_LC_FG_sample_day INNER JOIN 
                                                        dbo_gear_unload ON dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id
                                                    WHERE dbo_LC_FG_sample_day.sdate >= @d1";
                            }
                            if (Global.Filter2 != null)
                            {
                                cmd.Parameters.AddWithValue("@d2", Global.Filter2DateString());
                                if (countCompleted)
                                {
                                    cmd.CommandText = @"SELECT count(*) as n
                                                    FROM dbo_LC_FG_sample_day INNER JOIN 
                                                        dbo_gear_unload ON dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id
                                                    WHERE 
                                                        boats Is Not null AND catch Is Not Null AND 
                                                        dbo_LC_FG_sample_day.sdate >= @d1 AND dbo_LC_FG_sample_day.sdate < @d2";
                                }
                                else
                                {
                                    cmd.CommandText = @"SELECT count(*) as n
                                                    FROM dbo_LC_FG_sample_day INNER JOIN 
                                                        dbo_gear_unload ON dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id
                                                    WHERE dbo_LC_FG_sample_day.sdate >= @d1 AND dbo_LC_FG_sample_day.sdate < @d2";

                                    //cmd.CommandText = $@"SELECT count(*) as n
                                    //                FROM dbo_LC_FG_sample_day INNER JOIN 
                                    //                    dbo_gear_unload ON dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id
                                    //                WHERE dbo_LC_FG_sample_day.sdate >= #{Global.Filter1DateString()}# AND dbo_LC_FG_sample_day.sdate < #{Global.Filter2DateString()}#";
                                }
                            }
                        }
                        else if (!string.IsNullOrEmpty(Global.FilterServerID))
                        {
                            cmd.Parameters.AddWithValue("@srv", Global.FilterServerID);
                            if (countCompleted)
                            {
                                cmd.CommandText = @"SELECT Count(*) AS n
                                                FROM dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                                WHERE 
                                                    boats Is Not null AND catch Is Not Null AND 
                                                    dbo_LC_FG_sample_day_1.XFormIdentifier = @srv";
                            }
                            else
                            {
                                cmd.CommandText = @"SELECT Count(*) AS n
                                                FROM dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @srv";
                            }
                        }
                        try
                        {
                            conn.Open();
                            count = (int)cmd.ExecuteScalar();
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }

            }
            return count;
        }

        public static Task<List<GearUnload>> NumberOfDailyLandingsForCalendarTask(LandingSite ls, DateTime month_year)
        {
            return Task.Run(() => NumberOfDailyLandingsForCalendar(ls, month_year));
        }
        private static List<GearUnload> NumberOfDailyLandingsForCalendar(LandingSite ls, DateTime month_year)
        {
            ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "start build calendar" });
            List<GearUnload> thisList = new List<GearUnload>();
            int? boats = null;
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = conection.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@ls_id", ls.LandingSiteID);
                        cmd.Parameters.AddWithValue("@start_date", month_year.Date.ToString("M/d/yyyy"));
                        cmd.Parameters.AddWithValue("@end_date", month_year.AddMonths(1).ToString("M/d/yyyy"));

                        cmd.CommandText = @"SELECT 
                                                dbo_LC_FG_sample_day.unload_day_id, 
                                                dbo_LC_FG_sample_day.land_ctr_id, 
                                                dbo_LC_FG_sample_day.sdate, 
                                                dbo_LC_FG_sample_day_1.number_landings,
                                                dbo_LC_FG_sample_day.has_fishing_operation
                                            FROM 
                                                dbo_LC_FG_sample_day INNER JOIN 
                                                dbo_LC_FG_sample_day_1 ON 
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                            WHERE
                                                dbo_LC_FG_sample_day.land_ctr_id=@ls_id AND 
                                                dbo_LC_FG_sample_day.sdate >= @start_date AND 
                                                dbo_LC_FG_sample_day.sdate < @end_date
                                            ORDER BY
                                                dbo_LC_FG_sample_day.sdate";

                        try
                        {
                            conection.Open();
                            OleDbDataReader dr = cmd.ExecuteReader();
                            int loopCount = 0;
                            while (dr.Read())
                            {
                                boats = null;
                                if (dr["number_landings"] != DBNull.Value)
                                {
                                    boats = (int)dr["number_landings"];
                                }
                                GearUnload gu = new GearUnload
                                {
                                    Parent = NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteSampling((int)dr["unload_day_id"]),
                                    PK = ++loopCount,
                                    SectorCode = "",
                                    Boats = boats,
                                    Catch = 0,
                                    GearID = "",
                                    GearUsedText = "All gears",
                                    Remarks = "",
                                };
                                //if (gu.GearID.Length > 0)
                                //{
                                //    gu.Gear = NSAPEntities.GearViewModel.GetGear(gu.GearID);
                                //}
                                if (!(bool)dr["has_fishing_operation"])
                                {
                                    gu.Boats = 0;
                                }
                                thisList.Add(gu);
                            }

                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
            ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "end build calendar" });
            return thisList;
        }

        public static Task<List<GearUnload>> GearUnloadsForCalendarTask(LandingSite ls, DateTime month_year)
        {
            return Task.Run(() => GearUnloadsForCalendar(ls, month_year));
        }

        public static List<GearUnload> GearUnloadsForCalendar(LandingSite ls, DateTime month_year)
        {
            ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "start build calendar" });
            List<GearUnload> thisList = new List<GearUnload>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = conection.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@ls_id", ls.LandingSiteID);
                        cmd.Parameters.AddWithValue("@start_date", month_year.Date.ToString("M/d/yyyy"));
                        cmd.Parameters.AddWithValue("@end_date", month_year.AddMonths(1).ToString("M/d/yyyy"));

                        cmd.CommandText = @"SELECT temp_GearUnload.*
                                            FROM temp_GearUnload INNER JOIN dbo_LC_FG_sample_day ON temp_GearUnload.unload_day_id = dbo_LC_FG_sample_day.unload_day_id
                                            WHERE 
                                                dbo_LC_FG_sample_day.land_ctr_id=@ls_id AND 
                                                dbo_LC_FG_sample_day.sdate >= @start_date AND 
                                                dbo_LC_FG_sample_day.sdate < @end_date";

                        try
                        {
                            conection.Open();
                            OleDbDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                GearUnload gu = new GearUnload
                                {
                                    Parent = NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteSampling((int)dr["unload_day_id"]),
                                    PK = (int)dr["unload_gr_id"],
                                    SectorCode = dr["sector"].ToString(),
                                    Boats = (int)dr["boats"],
                                    Catch = (double)dr["catch"],
                                    GearID = dr["gr_id"].ToString(),
                                    GearUsedText = dr["gr_text"].ToString(),
                                    Remarks = dr["Remarks"].ToString(),
                                };
                                if (gu.GearID.Length > 0)
                                {
                                    gu.Gear = NSAPEntities.GearViewModel.GetGear(gu.GearID);
                                }
                                thisList.Add(gu);
                            }

                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }

            }
            ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "end build calendar" });
            return thisList;
        }
        private List<GearUnload> getGearUnloads(LandingSiteSampling ls = null)
        {
            List<GearUnload> thisList = new List<GearUnload>();
            if (Global.Settings.UsemySQL)
            {
                thisList = getFromMySQL(ls);
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
                            cmd.CommandText = $"Select * from dbo_gear_unload";

                            if (ls != null)
                            {
                                cmd.Parameters.AddWithValue("@parentID", ls.PK);
                                cmd.CommandText = $"Select * from dbo_gear_unload where unload_day_id=@parentID";
                            }


                            OleDbDataReader dr = cmd.ExecuteReader();
                            thisList.Clear();
                            while (dr.Read())
                            {
                                //double? wt_c = null;
                                //double? wt_m = null;

                                //if(dr["gear_catch_commercial"]!=DBNull.Value)
                                //{
                                //    wt_c = (double)dr["gear_catch_commercial"];
                                //}

                                GearUnload item = new GearUnload();
                                item.Parent = ls;
                                item.PK = (int)dr["unload_gr_id"];
                                item.LandingSiteSamplingID = (int)dr["unload_day_id"];
                                item.GearID = dr["gr_id"].ToString();
                                item.Boats = string.IsNullOrEmpty(dr["boats"].ToString()) ? null : (int?)dr["boats"];
                                item.Catch = string.IsNullOrEmpty(dr["catch"].ToString()) ? null : (double?)dr["catch"];
                                item.SpeciesWithTWSpCount = string.IsNullOrEmpty(dr["sp_twsp_count"].ToString()) ? null : (int?)dr["sp_twsp_count"];
                                item.GearUsedText = dr["gr_text"].ToString();
                                item.Remarks = dr["remarks"].ToString();
                                item.SectorCode = dr["sector"].ToString();
                                item.TotalWtSpViewModel = new TotalWtSpViewModel(item);
                                if (dr["gear_count_commercial"] != DBNull.Value)
                                {
                                    item.NumberOfCommercialLandings = (int)dr["gear_count_commercial"];
                                }
                                if (dr["gear_count_municipal"] != DBNull.Value)
                                {
                                    item.NumberOfMunicipalLandings = (int)dr["gear_count_municipal"];
                                }
                                if (dr["gear_catch_municipal"] != DBNull.Value)
                                {
                                    item.WeightOfMunicipalLandings = (double)dr["gear_catch_municipal"];
                                }
                                if (dr["gear_catch_commercial"] != DBNull.Value)
                                {
                                    item.WeightOfCommercialLandings = (double)dr["gear_catch_commercial"];
                                }
                                if (ls.IsMultiVessel && dr["gear_sequence"] != DBNull.Value)
                                {
                                    item.Sequence = (int)dr["gear_sequence"];
                                }

                                thisList.Add(item);

                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("sp_twsp_count"))
                            {
                                conection.Close();
                                if (AddFieldToTable("sp_twsp_count"))
                                {
                                    return getGearUnloads(ls);
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
            return thisList;
        }

        public static bool AddFieldToTable(string fieldName)
        {
            bool success = false;
            string sql = "";
            switch (fieldName)
            {
                case "gear_catch_municipal":
                case "gear_catch_commercial":
                    sql = $"ALTER TABLE dbo_gear_unload ADD COLUMN {fieldName} DOUBLE";
                    break;
                case "sector":
                    sql = $"ALTER TABLE dbo_gear_unload ADD COLUMN {fieldName} VARCHAR(1)";
                    break;
                case "sp_twsp_count":
                case "gear_sequence":
                case "gear_count_municipal":
                case "gear_count_commercial":
                    sql = $"ALTER TABLE dbo_gear_unload ADD COLUMN {fieldName} INT";
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
        private bool AddToMySQL(GearUnload item)
        {
            bool success = false;

            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@pk", MySqlDbType.Int32).Value = item.PK;
                    update.Parameters.Add("@parent", MySqlDbType.Int32).Value = item.LandingSiteSamplingID;
                    //success = false;
                    if (item.GearID == null || item.GearID.Length == 0)
                    {
                        update.Parameters.Add("@gear_id", MySqlDbType.Int32).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@gear_id", MySqlDbType.VarChar).Value = item.GearID;
                    }

                    if (item.Boats == null)
                    {
                        update.Parameters.Add("@boats", MySqlDbType.Int32).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@boats", MySqlDbType.Int32).Value = item.Boats;
                    }

                    if (item.Catch == null)
                    {
                        update.Parameters.Add("@catch", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@catch", MySqlDbType.Double).Value = item.Catch;
                    }

                    if (item.GearUsedText == null)
                    {
                        update.Parameters.Add("@gear_text", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@gear_text", MySqlDbType.VarChar).Value = item.GearUsedText;
                    }

                    if (item.Remarks == null)
                    {
                        update.Parameters.Add("@remarks", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@remarks", MySqlDbType.VarChar).Value = item.Remarks;
                    }
                    update.CommandText = @"Insert into dbo_gear_unload(unload_gr_id, unload_day_id, gr_id,boats,catch,gr_text,remarks) 
                                       Values (@pk,@parent,@gear_id,@boats,@catch,@gear_text,@remarks)";
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
        public bool Add(GearUnload item)
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


                    var sql = "Insert into dbo_gear_unload(unload_gr_id, unload_day_id, gr_id,boats,catch,gr_text,remarks,sector) Values (?,?,?,?,?,?,?,?)";

                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@pk", OleDbType.Integer).Value = item.PK;
                        update.Parameters.Add("@parent", OleDbType.Integer).Value = item.LandingSiteSamplingID;

                        if (item.GearID == null)
                        {
                            update.Parameters.Add("@gear_id", OleDbType.Integer).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@gear_id", OleDbType.VarChar).Value = item.GearID;
                        }
                        //var gearID = item.GearID == null ? DBNull.Value : item.GearID; 

                        //}

                        if (item.Boats == null)
                        {
                            update.Parameters.Add("@boats", OleDbType.Integer).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@boats", OleDbType.Integer).Value = item.Boats;
                        }

                        if (item.Catch == null)
                        {
                            update.Parameters.Add("@catch", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@catch", OleDbType.Double).Value = item.Catch;
                        }

                        if (item.GearUsedText == null)
                        {
                            update.Parameters.Add("@gear_text", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@gear_text", OleDbType.VarChar).Value = item.GearUsedText;
                        }

                        if (item.Remarks == null)
                        {
                            update.Parameters.Add("@remarks", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@remarks", OleDbType.VarChar).Value = item.Remarks;
                        }

                        if (string.IsNullOrEmpty(item.SectorCode))
                        {
                            update.Parameters.Add("@sector", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@sector", OleDbType.VarChar).Value = item.SectorCode;
                        }


                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException odbex)
                        {
                            switch (odbex.ErrorCode)
                            {
                                case -2147467259:
                                    //duplicate index or unique keys error
                                    break;
                                default:
                                    success = false;
                                    Logger.Log(odbex);
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
        private bool UpdateMySQL(GearUnload item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@parent", MySqlDbType.Int32).Value = item.LandingSiteSamplingID;

                    if (item.GearID == null)
                    {
                        update.Parameters.Add("@gear_id", MySqlDbType.Int32).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@gear_id", MySqlDbType.VarChar).Value = item.GearID;
                    }

                    if (item.Boats == null)
                    {
                        update.Parameters.Add("@boats", MySqlDbType.Int32).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@boats", MySqlDbType.Int32).Value = item.Boats;
                    }

                    if (item.Catch == null)
                    {
                        update.Parameters.Add("@catch", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@catch", MySqlDbType.Double).Value = item.Catch;
                    }

                    if (item.GearUsedText == null)
                    {
                        update.Parameters.Add("@gear_text", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@gear_text", MySqlDbType.VarChar).Value = item.GearUsedText;
                    }

                    if (item.Remarks == null)
                    {
                        update.Parameters.Add("@remarks", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@remarks", MySqlDbType.VarChar).Value = item.Remarks;
                    }
                    update.Parameters.Add("@pk", MySqlDbType.Int32).Value = item.PK;

                    update.CommandText = @"Update dbo_gear_unload set
                            unload_day_id=@parent,
                            gr_id = @gear_id,
                            boats = @boats,
                            catch = @catch,
                            gr_text = @gear_text,
                            remarks = @remarks
                        WHERE unload_gr_id = @pk";

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
        public bool Update(GearUnload item)
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

                        update.Parameters.Add("@parent", OleDbType.Integer).Value = item.LandingSiteSamplingID;

                        //if (item.GearID == null)
                        if (string.IsNullOrEmpty(item.GearID))
                        {
                            update.Parameters.Add("@gear_id", OleDbType.Integer).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@gear_id", OleDbType.VarChar).Value = item.GearID;
                        }

                        if (item.Boats == null)
                        {
                            update.Parameters.Add("@boats", OleDbType.Integer).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@boats", OleDbType.Integer).Value = item.Boats;
                        }

                        if (item.Catch == null)
                        {
                            update.Parameters.Add("@catch", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@catch", OleDbType.Double).Value = item.Catch;
                        }

                        if (item.GearUsedText == null)
                        {
                            update.Parameters.Add("@gear_text", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@gear_text", OleDbType.VarChar).Value = item.GearUsedText;
                        }

                        if (string.IsNullOrEmpty(item.SectorCode))
                        {
                            update.Parameters.Add("@sector", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@sector", OleDbType.VarChar).Value = item.SectorCode;
                        }

                        if (item.Remarks == null)
                        {
                            update.Parameters.Add("@remarks", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@remarks", OleDbType.VarChar).Value = item.Remarks;
                        }
                        update.Parameters.Add("@pk", OleDbType.Integer).Value = item.PK;

                        update.CommandText = @"Update dbo_gear_unload set
                            unload_day_id=@parent,
                            gr_id = @gear_id,
                            boats = @boats,
                            catch = @catch,
                            gr_text = @gear_text,
                            sector = @sector,
                            remarks = @remarks
                        WHERE unload_gr_id = @pk";

                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException olx)
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
                var sql = $"Delete * from dbo_gear_unload";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    try
                    {
                        update.ExecuteNonQuery();
                        success = DeleteTempGearUnload();
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

        private static bool DeleteTempGearUnload()
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
                        cmd.CommandText = "Delete * from temp_GearUnload";
                        try
                        {
                            con.Open();
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


        private bool DeleteMySQL(int id)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    update.CommandText = "Delete  from dbo_gear_unload where unload_gr_id=@id";
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
                        update.CommandText = "Delete * from dbo_gear_unload where unload_gr_id=@id";
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
