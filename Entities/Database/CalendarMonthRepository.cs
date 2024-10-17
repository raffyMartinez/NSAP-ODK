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
using NSAP_ODK.TreeViewModelControl;

namespace NSAP_ODK.Entities.Database
{
    public class CalendarMonthRepository
    {
        public List<CalendarGearSector> CalendarGearSectors { get; private set; }
        public CalendarMonthRepository(AllSamplingEntitiesEventHandler e)
        {
            CalendarGearSectors = GetCalendarGearSectors(e);

        }

        private List<CalendarGearSector> GetCalendarGearSectors(AllSamplingEntitiesEventHandler e)
        {
            HashSet<CalendarGearSector> this_list = new HashSet<CalendarGearSector>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {

                        cmd.Parameters.AddWithValue("@landing_site", e.LandingSite.LandingSiteID);
                        cmd.Parameters.AddWithValue("@nsapRegion", e.NSAPRegion.Code);
                        cmd.Parameters.AddWithValue("@fma", e.FMA.FMAID);
                        cmd.Parameters.AddWithValue("@fishing_ground", e.FishingGround.Code);
                        cmd.Parameters.AddWithValue("@sampling_type", "rs");

                        DateTime month_start = (DateTime)e.MonthSampled;
                        cmd.Parameters.AddWithValue("@month_start", month_start.Date);
                        cmd.Parameters.AddWithValue("@month_end", month_start.AddMonths(1).Date);

                        cmd.CommandText = @"SELECT 
                                                dbo_vesselunload_fishinggear.gear_code,
                                                dbo_vessel_unload_1.sector_code,
                                                gear.GearName,
                                                dbo_LC_FG_sample_day.sdate,
                                                dbo_LC_FG_sample_day.sampleday,
                                                dbo_LC_FG_sample_day.has_fishing_operation,
                                                Count(dbo_vesselunload_fishinggear.gear_code) AS n,
                                                Sum(dbo_vesselunload_fishinggear.catch_weight) AS sum_wt
                                            FROM 
                                                ((dbo_LC_FG_sample_day LEFT JOIN
                                                (dbo_gear_unload LEFT JOIN
                                                dbo_vessel_unload ON
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) LEFT JOIN
                                                dbo_vessel_unload_1 ON
                                                dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id) LEFT JOIN
                                                (gear RIGHT JOIN
                                                dbo_vesselunload_fishinggear ON
                                                gear.GearCode = dbo_vesselunload_fishinggear.gear_code) ON
                                                dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id
                                            WHERE 
                                                dbo_LC_FG_sample_day.land_ctr_id=@landing_site AND
                                                dbo_gear_unload.gr_id Is Not Null AND
                                                dbo_LC_FG_sample_day.region_id=@nsapRegion AND
                                                dbo_LC_FG_sample_day.fma=@fma AND
                                                dbo_LC_FG_sample_day.ground_id=@fishing_ground AND
                                                dbo_LC_FG_sample_day.type_of_sampling=@sampling_type
                                            GROUP BY 
                                                dbo_vesselunload_fishinggear.gear_code,
                                                dbo_vessel_unload_1.sector_code,
                                                gear.GearName,
                                                dbo_LC_FG_sample_day.sdate,
                                                dbo_LC_FG_sample_day.sampleday,
                                                dbo_LC_FG_sample_day.has_fishing_operation
                                            HAVING 
                                                dbo_LC_FG_sample_day.sdate>@month_start AND
                                                dbo_LC_FG_sample_day.sdate<=@month_end
                                            ORDER BY 
                                                dbo_vesselunload_fishinggear.gear_code,
                                                dbo_vessel_unload_1.sector_code DESC,
                                                dbo_LC_FG_sample_day.sdate";



                        string qry = cmd.CommandText
                            .Replace("@nsapRegion", $"'{e.NSAPRegion.Code}'")
                            .Replace("@fma", e.FMA.FMAID.ToString())
                            .Replace("@fishing_ground", $"'{e.FishingGround.Code}'")
                            .Replace("@landing_site", e.LandingSite.LandingSiteID.ToString())
                            .Replace("@sampling_type", "'rs'")
                            .Replace("@month_start", $"#{month_start.ToString("MM/d/yyyy")}#")
                            .Replace("@month_end", $"#{month_start.AddMonths(1).ToString("MM/d/yyyy")}#");


                        try
                        {
                            con.Open();
                            var dr = cmd.ExecuteReader();
                            CalendarDay cd;
                            while (dr.Read())
                            {
                                bool isSamplingDay = (bool)dr["sampleday"];
                                bool hasOperation = (bool)dr["has_fishing_operation"];

                                CalendarGearSector cgs = new CalendarGearSector();
                                if (isSamplingDay && hasOperation && dr["gear_code"]!=DBNull.Value)
                                {
                                    cgs.Gear = NSAPEntities.GearViewModel.GetGear(dr["gear_code"].ToString());
                                    cgs.SectorCode = dr["sector_code"].ToString();


                                     cd = new CalendarDay
                                    {
                                        CountOfFishingOperations = (int)dr["n"],
                                        TotalWeightOfCatch = (double)dr["sum_wt"],
                                        Day = (DateTime)dr["sdate"],
                                        IsSamplingDay = isSamplingDay,
                                        HasFishingOperation = hasOperation
                                    };


                                    if (!this_list.Contains(cgs))
                                    {
                                        this_list.Add(cgs);
                                    }
                                    else
                                    {
                                        cgs = this_list.First(t => t.SectorCode == cgs.SectorCode && t.Gear.Code == cgs.Gear.Code);
                                    }

                                    cd.Parent = cgs;
                                    cgs.CalendarDays.Add(cd);

                                }
                                else
                                {
                                    cgs.NoFishing = true;
                                    if (!this_list.Contains(cgs))
                                    {
                                        this_list.Add(cgs);
                                    }
                                    else
                                    {
                                        cgs = this_list.First(t => t.NoFishing);
                                    }

                                    cd = new CalendarDay
                                    {
                                        Day = (DateTime)dr["sdate"],
                                        IsSamplingDay = isSamplingDay,
                                        HasFishingOperation = hasOperation,
                                        Parent=cgs
                                    };
                                    cgs.CalendarDays.Add(cd);

                                }

                                //CalendarDay cd = new CalendarDay
                                //{
                                //    CountOfFishingOperations = (int)dr["n"],
                                //    TotalWeightOfCatch = (double)dr["sum_wt"],
                                //    Day = (DateTime)dr["sdate"],
                                //    IsSamplingDay = isSamplingDay,
                                //    HasFishingOperation = hasOperation
                                //};


                                //if (!this_list.Contains(cgs))
                                //{
                                //    this_list.Add(cgs);
                                //}
                                //else
                                //{
                                //    cgs = this_list.First(t => t.SectorCode == cgs.SectorCode && t.Gear.Code == cgs.Gear.Code);
                                //}

                                //cd.Parent = cgs;
                                //cgs.CalendarDays.Add(cd);

                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }

                    }
                }
            }
            return this_list.ToList();
        }
    }
}
