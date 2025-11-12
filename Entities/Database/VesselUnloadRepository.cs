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
    class VesselUnloadRepository
    {
        public static event EventHandler<UpdateTableRowsEventArg> TableRowsUpdatingEvent;
        public static event EventHandler<SetFishingGroundOfUnloadEventArg> ChangeFishingGroundOFUnloadEvent;
        private string _dateFormat = "MMM-dd-yyyy HH:mm";
        public List<VesselUnload> VesselUnloads { get; set; }
        public static int GetFieldSize(string fieldName)
        {
            int fieldSize;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {

                using (OleDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"Select {fieldName} from dbo_vessel_unload_1";
                    conn.Open();
                    cmd.Connection = conn;
                    var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly);
                    var schema = reader.GetSchemaTable();
                    fieldSize = Convert.ToInt32(schema.Rows[0]["ColumnSize"]);
                }
            }
            return fieldSize;
        }

        public static ParentIDs GetParentIDs(int v_unloadID)
        {
            ParentIDs pids = new ParentIDs();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@vuid", v_unloadID);
                        cmd.CommandText = @"SELECT 
                                                dbo_vessel_unload.v_unload_id, 
                                                dbo_gear_unload.unload_gr_id, 
                                                dbo_LC_FG_sample_day.unload_day_id
                                            FROM 
                                                dbo_LC_FG_sample_day INNER JOIN 
                                                (dbo_gear_unload INNER JOIN 
                                                dbo_vessel_unload ON 
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON 
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id
                                            WHERE 
                                                dbo_vessel_unload.v_unload_id=@vuid";
                        try
                        {
                            con.Open();
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                pids = new ParentIDs
                                {
                                    VesselUnloadID = (int)dr["v_unload_id"],
                                    GearUnloadID = (int)dr["unload_gr_id"],
                                    LandingSiteSamplingID = (int)dr["unload_day_id"]
                                };
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return pids;
        }
        public static List<VesselUnload> GetVesselUnloads(AllSamplingEntitiesEventHandler entities, bool includeETPInteraction=false)
        {
            List<VesselUnload> unloads = new List<VesselUnload>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@reg", entities.NSAPRegion.Code);
                        cmd.Parameters.AddWithValue("@ls", entities.LandingSite.LandingSiteID);
                        cmd.Parameters.AddWithValue("@fg", entities.FishingGround.Code);
                        cmd.Parameters.AddWithValue("@fma", entities.FMA.FMAID);
                        DateTime sMonth = (DateTime)entities.MonthSampled;
                        cmd.Parameters.AddWithValue("@start", sMonth.ToString("MMM/dd/yyyy"));
                        cmd.Parameters.AddWithValue("@end", sMonth.AddMonths(1).ToString("MMM/dd/yyyy"));
                        cmd.CommandText = @"SELECT
                                                dbo_vessel_unload_1.SamplingDate,
                                                dbo_gear_unload.unload_gr_id,
                                                dbo_vessel_unload_1.is_multigear,
                                                dbo_vessel_unload.v_unload_id,
                                                dbo_vessel_unload_1.Success,
                                                dbo_vessel_unload_1.trip_is_completed, 
                                                dbo_vessel_unload_1.form_version,
                                                Provinces.ProvinceName,
                                                Municipalities.Municipality,
                                                nsapRegion.ShortName AS Region,
                                                dbo_LC_FG_sample_day.fma,
                                                fishingGround.FishingGroundName,
                                                landingSite.LandingSiteName,
                                                dbo_vessel_unload_1.EnumeratorID,
                                                NSAPEnumerator.EnumeratorName,
                                                dbo_vessel_unload_1.sector_code,
                                                First(dbo_fg_grid.utm_zone) AS grid_utm_zone,
                                                First(dbo_fg_grid.grid25) AS grid,
                                                gear.GearName,
                                                First(dbo_gear_soak.time_set) AS setting,
                                                First(dbo_gear_soak.time_hauled) AS hauling,
                                                dbo_vessel_unload.catch_total AS gear_catch_wt,
                                                dbo_vessel_unload_1.number_species_catch_composition AS number_species_catch,
                                                dbo_vessel_unload_1.ref_no,
                                                dbo_vessel_unload_1.HasCatchComposition,
                                                dbo_vessel_unload.is_boat_used,
                                                dbo_vessel_unload.boat_text,
                                                fishingVessel.VesselName,
                                                dbo_vessel_unload_1.NumberOfFishers,
                                                dbo_vessel_unload_1.number_species_catch_composition,
                                                dbo_vessel_unload.catch_total AS total_wt_catch,
                                                dbo_vessel_unload.catch_samp AS sample_wt_catch,
                                                dbo_vessel_unload_1.is_catch_sold,
                                                dbo_vessel_unload_1.count_gear_types,
                                                dbo_vessel_unload_1.Notes,
                                                dbo_vessel_unload_1.has_etp_interaction
                                            FROM
                                                 (gear INNER JOIN
                                                 (NSAPEnumerator RIGHT JOIN
                                                 (fishingGround INNER JOIN
                                                 (nsapRegion INNER JOIN
                                                 (Provinces INNER JOIN
                                                 (Municipalities INNER JOIN
                                                 (landingSite INNER JOIN
                                                 ((((dbo_LC_FG_sample_day INNER JOIN
                                                 (dbo_gear_unload INNER JOIN
                                                 dbo_vessel_unload ON
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN
                                                 dbo_vessel_unload_1 ON
                                                dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id) LEFT JOIN dbo_fg_grid ON
                                                dbo_vessel_unload.v_unload_id = dbo_fg_grid.v_unload_id) LEFT JOIN dbo_gear_soak ON
                                                dbo_vessel_unload.v_unload_id = dbo_gear_soak.v_unload_id) ON
                                                landingSite.LandingSiteID = dbo_LC_FG_sample_day.land_ctr_id) ON
                                                Municipalities.MunNo = landingSite.Municipality) ON
                                                Provinces.ProvNo = Municipalities.ProvNo) ON
                                                nsapRegion.Code = dbo_LC_FG_sample_day.region_id) ON
                                                fishingGround.FishingGroundCode = dbo_LC_FG_sample_day.ground_id) ON
                                                NSAPEnumerator.EnumeratorID = dbo_vessel_unload_1.EnumeratorID) ON
                                                gear.GearCode = dbo_gear_unload.gr_id) LEFT JOIN fishingVessel ON
                                                dbo_vessel_unload.boat_id = fishingVessel.VesselID
                                            WHERE
                                                dbo_LC_FG_sample_day.region_id=@reg AND
                                                dbo_LC_FG_sample_day.land_ctr_id=@ls AND
                                                dbo_LC_FG_sample_day.ground_id=@fg AND
                                                dbo_LC_FG_sample_day.fma=@fma AND
                                                dbo_LC_FG_sample_day.sdate>=@start AND
                                                dbo_LC_FG_sample_day.sdate<@end
                                            GROUP BY
                                                dbo_vessel_unload_1.SamplingDate,
                                                dbo_gear_unload.unload_gr_id,
                                                dbo_vessel_unload_1.is_multigear,
                                                dbo_vessel_unload.v_unload_id,
                                                dbo_vessel_unload_1.Success,
                                                dbo_vessel_unload_1.trip_is_completed, 
                                                dbo_vessel_unload_1.form_version,
                                                Provinces.ProvinceName,
                                                Municipalities.Municipality,
                                                nsapRegion.ShortName,
                                                dbo_LC_FG_sample_day.fma,
                                                fishingGround.FishingGroundName,
                                                landingSite.LandingSiteName,
                                                dbo_vessel_unload_1.EnumeratorID,
                                                NSAPEnumerator.EnumeratorName,
                                                dbo_vessel_unload_1.sector_code,
                                                gear.GearName,
                                                dbo_vessel_unload.catch_total,
                                                dbo_vessel_unload_1.number_species_catch_composition,
                                                dbo_vessel_unload_1.ref_no,
                                                dbo_vessel_unload_1.HasCatchComposition,
                                                dbo_vessel_unload.is_boat_used,
                                                dbo_vessel_unload.boat_text,
                                                fishingVessel.VesselName,
                                                dbo_vessel_unload_1.NumberOfFishers,
                                                dbo_vessel_unload_1.number_species_catch_composition,
                                                dbo_vessel_unload.catch_total,
                                                dbo_vessel_unload.catch_samp,
                                                dbo_vessel_unload_1.is_catch_sold,
                                                dbo_vessel_unload_1.count_gear_types,
                                                dbo_vessel_unload_1.Notes,
                                                dbo_vessel_unload_1.has_etp_interaction
                                            HAVING
                                                dbo_vessel_unload_1.is_multigear=False 
                                            ORDER BY
                                                dbo_vessel_unload_1.SamplingDate


                                            UNION ALL

                                            SELECT
                                                dbo_vessel_unload_1.SamplingDate,
                                                dbo_gear_unload.unload_gr_id,
                                                dbo_vessel_unload_1.is_multigear,
                                                dbo_vessel_unload.v_unload_id,
                                                dbo_vessel_unload_1.Success,
                                                dbo_vessel_unload_1.trip_is_completed, 
                                                dbo_vessel_unload_1.form_version,
                                                Provinces.ProvinceName,
                                                Municipalities.Municipality,
                                                nsapRegion.ShortName AS Region,
                                                dbo_LC_FG_sample_day.fma,
                                                fishingGround.FishingGroundName,
                                                landingSite.LandingSiteName,
                                                dbo_vessel_unload_1.EnumeratorID,
                                                NSAPEnumerator.EnumeratorName,
                                                dbo_vessel_unload_1.sector_code,
                                                First(dbo_fg_grid.utm_zone) AS grid_utm_zone,
                                                First(dbo_fg_grid.grid25) AS grid,
                                                gear.GearName,
                                                First(dbo_gear_soak.time_set) AS setting,
                                                First(dbo_gear_soak.time_hauled) AS hauling,
                                                dbo_vesselunload_fishinggear.catch_weight AS gear_catch_wt,
                                                dbo_vessel_unload_1.number_species_catch_composition AS number_species_catch,
                                                dbo_vessel_unload_1.ref_no,
                                                dbo_vessel_unload_1.HasCatchComposition,
                                                dbo_vessel_unload.is_boat_used,
                                                dbo_vessel_unload.boat_text,
                                                fishingVessel.VesselName,
                                                dbo_vessel_unload_1.NumberOfFishers,
                                                dbo_vessel_unload_1.number_species_catch_composition,
                                                dbo_vessel_unload.catch_total AS total_wt_catch,
                                                dbo_vessel_unload.catch_samp AS sample_wt_catch,
                                                dbo_vessel_unload_1.is_catch_sold,
                                                dbo_vessel_unload_1.count_gear_types,
                                                dbo_vessel_unload_1.Notes,
                                                dbo_vessel_unload_1.has_etp_interaction
                                            FROM
                                                 (nsapRegion INNER JOIN
                                                 ((Provinces INNER JOIN
                                                 Municipalities ON
                                                Provinces.ProvNo = Municipalities.ProvNo) INNER JOIN
                                                 (landingSite INNER JOIN
                                                 (fishingGround INNER JOIN
                                                 ((((dbo_LC_FG_sample_day INNER JOIN
                                                 (dbo_gear_unload INNER JOIN
                                                 (dbo_vessel_unload LEFT JOIN fishingVessel ON
                                                dbo_vessel_unload.boat_id = fishingVessel.VesselID) ON
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) LEFT JOIN dbo_fg_grid ON
                                                dbo_vessel_unload.v_unload_id = dbo_fg_grid.v_unload_id) LEFT JOIN dbo_gear_soak ON
                                                dbo_vessel_unload.v_unload_id = dbo_gear_soak.v_unload_id) INNER JOIN
                                                 (NSAPEnumerator RIGHT JOIN
                                                 dbo_vessel_unload_1 ON
                                                NSAPEnumerator.EnumeratorID = dbo_vessel_unload_1.EnumeratorID) ON
                                                dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id) ON
                                                fishingGround.FishingGroundCode = dbo_LC_FG_sample_day.ground_id) ON
                                                landingSite.LandingSiteID = dbo_LC_FG_sample_day.land_ctr_id) ON
                                                Municipalities.MunNo = landingSite.Municipality) ON
                                                nsapRegion.Code = dbo_LC_FG_sample_day.region_id) INNER JOIN
                                                 (gear INNER JOIN
                                                 dbo_vesselunload_fishinggear ON
                                                gear.GearCode = dbo_vesselunload_fishinggear.gear_code) ON
                                                dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id
                                            WHERE
                                                dbo_LC_FG_sample_day.region_id=@reg AND
                                                dbo_LC_FG_sample_day.land_ctr_id=@ls AND
                                                dbo_LC_FG_sample_day.ground_id=@fg AND
                                                dbo_LC_FG_sample_day.fma=@fma AND
                                                dbo_LC_FG_sample_day.sdate>=@start AND
                                                dbo_LC_FG_sample_day.sdate<@end
                                            GROUP BY
                                                dbo_vessel_unload_1.SamplingDate,
                                                dbo_gear_unload.unload_gr_id,
                                                dbo_vessel_unload_1.is_multigear,
                                                dbo_vessel_unload.v_unload_id,
                                                dbo_vessel_unload_1.Success,
                                                dbo_vessel_unload_1.trip_is_completed, 
                                                dbo_vessel_unload_1.form_version,
                                                Provinces.ProvinceName,
                                                Municipalities.Municipality,
                                                nsapRegion.ShortName,
                                                dbo_LC_FG_sample_day.fma,
                                                fishingGround.FishingGroundName,
                                                landingSite.LandingSiteName,
                                                dbo_vessel_unload_1.EnumeratorID,
                                                NSAPEnumerator.EnumeratorName,
                                                dbo_vessel_unload_1.sector_code,
                                                gear.GearName,
                                                dbo_vesselunload_fishinggear.catch_weight,
                                                dbo_vessel_unload_1.number_species_catch_composition,
                                                dbo_vessel_unload_1.ref_no,
                                                dbo_vessel_unload_1.HasCatchComposition,
                                                dbo_vessel_unload.is_boat_used,
                                                dbo_vessel_unload.boat_text,
                                                fishingVessel.VesselName,
                                                dbo_vessel_unload_1.NumberOfFishers,
                                                dbo_vessel_unload_1.number_species_catch_composition,
                                                dbo_vessel_unload.catch_total,
                                                dbo_vessel_unload.catch_samp,
                                                dbo_vessel_unload_1.is_catch_sold,
                                                dbo_vessel_unload_1.count_gear_types,
                                                dbo_vessel_unload_1.Notes,
                                                dbo_vessel_unload_1.has_etp_interaction
                                            HAVING
                                                 dbo_vessel_unload_1.is_multigear=True";

                        con.Open();
                        VesselUnload vu = null;
                        try
                        {

                            OleDbDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                vu = new VesselUnload
                                {
                                    GearUnloadID = (int)dr["unload_gr_id"],
                                    PK = (int)dr["v_unload_id"],
                                    RefNo = dr["ref_no"].ToString(),
                                    //EnumeratorText = dr["EnumeratorText"].ToString(),
                                    IsCatchSold = (bool)dr["is_catch_sold"],
                                    IsBoatUsed = (bool)dr["is_boat_used"],
                                    SamplingDate = (DateTime)dr["SamplingDate"],
                                    Notes = dr["Notes"].ToString(),
                                    HasCatchComposition = (bool)dr["HasCatchComposition"],
                                    FormVersion = dr["form_version"].ToString(),
                                    OperationIsSuccessful = (bool)dr["Success"],
                                    IsMultiGear = (bool)dr["is_multigear"],
                                    SectorCode = dr["sector_code"].ToString(),
                                    FishingTripIsCompleted = (bool)dr["trip_is_completed"],
                                    FirstFishingGround = $"{dr["grid_utm_zone"].ToString()} - {dr["grid"].ToString()}",
                                    HasInteractionWithETPs = (bool)dr["has_etp_interaction"]
                                };
                                vu.ListUnloadFishingGearsEx = new List<VesselUnload_FishingGear>();
                                if (dr["grid"] != DBNull.Value && dr["grid_utm_zone"] != DBNull.Value)
                                {
                                    UTMZone z = new UTMZone(dr["grid_utm_zone"].ToString());
                                    var gridCell = new Grid25GridCell(z, dr["grid"].ToString());
                                    vu.FirstFishingGroundCoordinate = gridCell.Coordinate;
                                }
                                if (dr["setting"] != DBNull.Value)
                                {
                                    vu.GearSettingFirst = (DateTime)dr["setting"];
                                }
                                if (dr["hauling"] != DBNull.Value)
                                {
                                    vu.GearHaulingFirst = (DateTime)dr["hauling"];
                                }
                                if (dr["NumberOfFishers"] != DBNull.Value && (int)dr["NumberOfFishers"] > 0)
                                {
                                    vu.NumberOfFishers = (int)dr["NumberOfFishers"];
                                }
                                if (dr["EnumeratorID"] != DBNull.Value)
                                {
                                    vu.NSAPEnumeratorID = (int)dr["EnumeratorID"];
                                    vu.NSAPEnumerator = NSAPEntities.NSAPEnumeratorViewModel.GetNSAPEnumerator((int)vu.NSAPEnumeratorID);
                                }
                                if (dr["total_wt_catch"] != DBNull.Value)
                                {
                                    vu.WeightOfCatch = (double)dr["total_wt_catch"];
                                }
                                if (dr["sample_wt_catch"] != DBNull.Value && (double)dr["sample_wt_catch"] > 0)
                                {
                                    vu.WeightOfCatchSample = (double)dr["sample_wt_catch"];
                                }
                                //if (dr["DepartureLandingSite"] != DBNull.Value)
                                //{
                                //    vu.DepartureFromLandingSite = (DateTime)dr["DepartureLandingSite"];
                                //}
                                //if (dr["ArrivalLandingSite"] != DBNull.Value)
                                //{
                                //    vu.ArrivalAtLandingSite = (DateTime)dr["ArrivalLandingSite"];
                                //}
                                if (string.IsNullOrEmpty(dr["boat_text"].ToString()))
                                {
                                    vu.VesselText = dr["VesselName"].ToString();
                                }
                                else

                                {
                                    vu.VesselText = dr["boat_text"].ToString();
                                }
                                if (vu.IsMultiGear && dr["count_gear_types"] != DBNull.Value)
                                {
                                    vu.CountGearTypesUsed = (int)dr["count_gear_types"];
                                }
                                if(includeETPInteraction)
                                {
                                    vu.VesselUnload_ETP_Interaction  = new VesselUnload_ETP_Interaction
                                    {
                                        VesselUnloadID = vu.PK,
                                        VesselUnload = vu,
                                        HasETPAndGearInteraction = vu.HasInteractionWithETPs,
                                    };
                                    if (vu.HasInteractionWithETPs)
                                    {
                                        PopulateETPIntercationFields(vu.VesselUnload_ETP_Interaction);
                                    }
                                }
                                unloads.Add(vu);
                            }

                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return unloads;
        }

        private static void PopulateETPIntercationFields(VesselUnload_ETP_Interaction vei)
        {
           if(Global.Settings.UsemySQL)
            {

            }
           else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@vu_id", vei.VesselUnloadID);
                        cmd.CommandText = "Select * from dbo_vessel_unload_etp_interaction where v_unload_id=@vu_id";
                        con.Open();
                        try
                        {
                            OleDbDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                switch (dr["etp_name"].ToString())
                                {
                                    case "Sharks":
                                        vei.HasShark = true;
                                        break;
                                    case "Rays":
                                        vei.HasRay = true;
                                        break;
                                    case "Sea turtles":
                                        vei.HasSeaTurtle = true;
                                        break;
                                    case "Marine mammals":
                                        vei.HasMarineMammal = true;
                                        break;
                                }
                            }
                            dr.Close();
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "Select * from dbo_vessel_unload_etp_interaction_type where v_unload_id=@vu_id";
                        cmd.Parameters.AddWithValue("@vu_id", vei.VesselUnloadID);
                        try
                        {
                            OleDbDataReader dr1 = cmd.ExecuteReader();
                            while (dr1.Read())
                            {
                                switch (dr1["etp_interaction"].ToString())
                                {
                                    case "Escape from gear":
                                        vei.HasETPEscapeFromGear = true;
                                        break;
                                    case "Release":
                                        vei.HasETPReleaseFromGear = true;
                                        break;
                                    case "Injury and release":
                                        vei.HasETPInjuryAndReleaseFromGear = true;
                                        break;
                                    case "With mortality":
                                    case "With Mortality":
                                        vei.HasETPMortality = true;
                                        break;
                                    case "Other interaction":
                                        vei.HasETPOtherInteraction = true;
                                        vei.ETPOtherInteraction = dr1["other_interaction"].ToString();
                                        break;
                                }
                            }
                        }
                        catch (Exception ex1)
                        {
                            Logger.Log(ex1);
                        }
                    }
                }
            }
        }
        public static int RefNoFieldSize { get; set; }
        public static bool UpdateTableDefinitionEx(string colName = null, int? colWidth = null, string relationshipToRemove = "")
        {
            bool success = false;
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = "";
                if (colWidth != null)
                {
                    switch (colName)
                    {
                        case "json_filename":
                        case "ref_no":
                            sql = $"ALTER TABLE dbo_vessel_unload_1   ALTER {colName} varchar({colWidth})";
                            break;
                    }
                }
                else if (string.IsNullOrEmpty(colName) && relationshipToRemove.Length > 0)
                {

                    string tableName = "dbo_vessel_unload";
                    if (relationshipToRemove.Contains("gps"))
                    {
                        tableName = "dbo_vessel_unload_1";
                    }
                    sql = $"ALTER TABLE {tableName} DROP CONSTRAINT {relationshipToRemove}";


                }
                else
                {
                    switch (colName)
                    {
                        case "number_species_catch_composition":
                        case "submission_id":
                            sql = $@"ALTER TABLE dbo_vessel_unload_1 ADD COLUMN {colName}  int";
                            break;
                        case "sampling_sequence":
                            sql = $@"ALTER TABLE dbo_vessel_unload_1 ADD COLUMN {colName}  int";
                            break;
                        case "count_gear_types":
                            sql = $@"ALTER TABLE dbo_vessel_unload_1 ADD COLUMN {colName}  int";
                            break;
                        case "is_multigear":
                        case "include_effort_indicators":
                        case "has_etp_interaction":
                            sql = $@"ALTER TABLE dbo_vessel_unload_1 ADD COLUMN {colName}  BIT";
                            break;
                        case "ref_no":

                            if (colWidth != null)
                            {
                                sql = $"ALTER TABLE dbo_vessel_unload_1 ALTER Column {colName} varchar({colWidth})";
                            }
                            break;
                        case "json_filename":
                        case "lss_submisionID":
                            sql = $@"ALTER TABLE dbo_vessel_unload_1 ADD COLUMN {colName}  varchar(255)";
                            break;
                    }
                }


                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                if (!string.IsNullOrEmpty(sql))
                {
                    cmd.CommandText = sql;

                    try
                    {
                        if (sql.Length > 0)
                        {
                            cmd.ExecuteNonQuery();
                            success = true;
                        }


                    }
                    catch (OleDbException dbex)
                    {
                        if (dbex.Message.Contains($"CHECK constraint '{relationshipToRemove}' does not exist."))
                        {
                            success = true;
                        }
                        else if (!success && colName == "ref_no" && colWidth != null)
                        {
                            //success = true;
                            success = CreateNewRefNoFieldAndCopy();
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
                }

                cmd.Connection.Close();
                conn.Close();
            }
            return success;
        }

        public static int GetTotalSavedLandingsCount()
        {
            int totalCount = 0;
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "Select Count(*) from dbo_vessel_unload";
                        con.Open();
                        try
                        {
                            totalCount = (int)cmd.ExecuteScalar();
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return totalCount;
        }

        private static bool CreateNewRefNoFieldAndCopy()
        {
            bool success = false;
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                string sql = "ALTER TABLE dbo_vessel_unload_1 Add Column ref_no_1 varchar(150)";

                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;

                try
                {
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "Update dbo_vessel_unload_1 SET dbo_vessel_unload_1.ref_no_1 = dbo_vessel_unload_1.ref_no";
                    try
                    {
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "ALTER TABLE dbo_vessel_unload_1 DROP COLUMN ref_no";
                        try
                        {
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "ALTER TABLE dbo_vessel_unload_1 Add Column ref_no varchar(150)";
                            try
                            {
                                cmd.ExecuteNonQuery();
                                cmd.CommandText = "Update dbo_vessel_unload_1 SET dbo_vessel_unload_1.ref_no = dbo_vessel_unload_1.ref_no_1";
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                    cmd.CommandText = "ALTER TABLE dbo_vessel_unload_1 DROP COLUMN ref_no_1";
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
                                catch (Exception ex)
                                {
                                    Logger.Log(ex);
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Log(ex);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }

            }
            return success;
        }
        public static bool ChangeGearUnloadIDOfLanding(int vesselUnloadID, int newGearUnloadID)
        {
            bool success = false;
            using (var con = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.Parameters.AddWithValue("@gu", newGearUnloadID);
                    cmd.Parameters.AddWithValue("@vu", vesselUnloadID);
                    cmd.CommandText = "UPDATE dbo_vessel_unload SET unload_gr_id = @gu WHERE v_unload_id = @vu";
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
        public static Task<int> SetFishingGroundsOfVesselUnloadsAsync(List<DBSummary> dbSummaries, FishingGround fg)
        {
            return Task.Run(() => SetFishingGroundsOfVesselUnloads(dbSummaries, fg));
        }
        public static int SetFishingGroundsOfVesselUnloads(List<DBSummary> dbSummaries, FishingGround fg)
        {
            int samplingDayCount = 0;
            foreach (DBSummary item in dbSummaries)
            {
                samplingDayCount += item.SamplingDayIDs.Count;
            }

            ChangeFishingGroundOFUnloadEvent?.Invoke(null, new SetFishingGroundOfUnloadEventArg { Intent = "start", TotalVesselUnloads = samplingDayCount });

            int loopCount = 0;
            foreach (DBSummary s in dbSummaries)
            {
                foreach (int s_id in s.SamplingDayIDs)
                {
                    if (ChangeFishingGround(s_id, fg.Code) && NSAPEntities.SummaryItemViewModel.SetFishingGroundOfSamplingDay(s_id, fg.Code) > 0)
                    {
                        loopCount++;
                        ChangeFishingGroundOFUnloadEvent?.Invoke(null, new SetFishingGroundOfUnloadEventArg { Intent = "fg changed", CountFishingGroundChanged = loopCount });
                    }
                }
            }
            ChangeFishingGroundOFUnloadEvent?.Invoke(null, new SetFishingGroundOfUnloadEventArg { Intent = "finished" });
            return loopCount;
        }

        private static bool ChangeFishingGround(int samplingDayID, string fishingGroundCode)
        {
            bool success = false;
            using (var con = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.Parameters.AddWithValue("@fg_code", fishingGroundCode);
                    cmd.Parameters.AddWithValue("@lss_id", samplingDayID);
                    cmd.CommandText = "UPDATE dbo_LC_FG_sample_day set ground_id=@fg_code WHERE unload_day_id=@lss_id";
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

        private static bool FillUpStatsRowIDS2(int rowID, int vu_id)
        {
            bool success = false;
            using (var con = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.Parameters.AddWithValue("@id", rowID);
                    cmd.Parameters.AddWithValue("@vu_id", vu_id);

                    cmd.CommandText = "UPDATE dbo_vessel_unload_stats SET row_id=@id WHERE v_unload_id=@vu_id";
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

        private static Task<bool> FillUpWeightsRowIDsAsync()
        {
            return Task.Run(() => FillUpWeightsRowIDs());
        }
        private static bool FillUpWeightsRowIDs()
        {
            int loop_count = 0;
            int rows_count = 0;
            using (var con = new OleDbConnection(Global.ConnectionString))
            {

                con.Open();

                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT Count(*) from dbo_vessel_unload_weight_validation";
                    rows_count = (int)cmd.ExecuteScalar();

                    cmd.CommandText = "Select * from dbo_vessel_unload_weight_validation";
                    var dr = cmd.ExecuteReader();

                    TableRowsUpdatingEvent?.Invoke(null, new UpdateTableRowsEventArg { Intent = "start", TotalRowsToUpdate = rows_count });

                    while (dr.Read())
                    {
                        int vu_id = (int)dr["v_unload_id"];
                        if (FillUpWeightsRowIDS2(loop_count + 1, vu_id))
                        {
                            loop_count++;
                            TableRowsUpdatingEvent?.Invoke(null, new UpdateTableRowsEventArg { Intent = "row updated", TableName = "Weight validation", RowsUpdatedCount = loop_count });
                        }
                    }
                }


            }

            TableRowsUpdatingEvent?.Invoke(null, new UpdateTableRowsEventArg { Intent = "row updating done", TableName = "Weight validation", RowsUpdatedCount = loop_count });
            return loop_count == rows_count;
        }

        private static bool FillUpWeightsRowIDS2(int rowID, int vu_id)
        {
            bool success = false;
            using (var con = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.Parameters.AddWithValue("@id", rowID);
                    cmd.Parameters.AddWithValue("@vu_id", vu_id);

                    cmd.CommandText = "UPDATE dbo_vessel_unload_weight_validation SET row_id=@id WHERE v_unload_id=@vu_id";
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


        private static bool FillUpStatsRowIDs()
        {
            int loop_count = 0;
            int rows_count = 0;
            using (var con = new OleDbConnection(Global.ConnectionString))
            {

                con.Open();

                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT Count(*) from dbo_vessel_unload_stats";
                    rows_count = (int)cmd.ExecuteScalar();

                    cmd.CommandText = "Select * from dbo_vessel_unload_stats";
                    var dr = cmd.ExecuteReader();

                    TableRowsUpdatingEvent?.Invoke(null, new UpdateTableRowsEventArg { Intent = "start", TotalRowsToUpdate = rows_count });

                    while (dr.Read())
                    {
                        int vu_id = (int)dr["v_unload_id"];
                        if (FillUpStatsRowIDS2(loop_count + 1, vu_id))
                        {
                            loop_count++;
                            TableRowsUpdatingEvent?.Invoke(null, new UpdateTableRowsEventArg { Intent = "row updated", TableName = "Unload summary statistics", RowsUpdatedCount = loop_count });
                        }
                    }
                }


            }

            TableRowsUpdatingEvent?.Invoke(null, new UpdateTableRowsEventArg { Intent = "row updating done", TableName = "Unload summary statistics", RowsUpdatedCount = loop_count });
            return loop_count == rows_count;
        }

        public static Task<bool> AddFieldToStatsTableAsync(string fieldName)
        {
            return Task.Run(() => AddFieldToStatsTable(fieldName));
        }
        public static bool AddFieldToStatsTable(string fieldName)
        {
            bool success = false;
            string sql = "";
            switch (fieldName)
            {
                case "unload_gear":
                    sql = $"ALTER TABLE dbo_vessel_unload_stats ADD COLUMN {fieldName} int";
                    break;
                case "row_id":
                    sql = $"ALTER TABLE dbo_vessel_unload_stats ADD COLUMN {fieldName} int";
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
                        if (success && fieldName == "row_id")
                        {
                            ////fill up empty row_ids with incrementing ints
                            //if (FillUpStatsRowIDs())
                            //{
                            //remove v_unload_id as primary key
                            cmd.CommandText = $"DROP INDEX PrimaryKey on dbo_vessel_unload_stats";
                            cmd.ExecuteNonQuery();
                            success = true;

                            if (success)
                            {
                                cmd.CommandText = "UPDATE dbo_vessel_unload_stats SET row_id = [v_unload_id]";
                                try
                                {
                                    success = cmd.ExecuteNonQuery() > 0;
                                }
                                catch (Exception ex)
                                {
                                    Logger.Log(ex);
                                }

                                if (success)
                                {
                                    cmd.CommandText = "ALTER TABLE dbo_vessel_unload_stats ADD CONSTRAINT PrimaryKey PRIMARY KEY(row_id) ";
                                    cmd.ExecuteNonQuery();
                                    success = true;
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

        public static Task<bool> AddFieldToWeightValidationTableAsync(string fieldName)
        {
            return Task.Run(() => AddFieldToWeightValidationTable(fieldName));
        }


        public static bool AddFieldToWeightValidationTable(string fieldName)
        {
            bool success = false;
            string sql = "";
            switch (fieldName)
            {
                case "row_id":
                case "unload_gear":
                    sql = $"ALTER TABLE dbo_vessel_unload_weight_validation ADD COLUMN {fieldName} int";
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
                        if (success && fieldName == "row_id")
                        {
                            cmd.CommandText = $"DROP INDEX PrimaryKey on dbo_vessel_unload_weight_validation";
                            cmd.ExecuteNonQuery();
                            success = true;

                            if (success)
                            {

                                cmd.CommandText = "UPDATE dbo_vessel_unload_weight_validation SET row_id = [v_unload_id]";
                                try
                                {
                                    success = cmd.ExecuteNonQuery() > 0;
                                }
                                catch (Exception ex)
                                {
                                    Logger.Log(ex);
                                }


                                if (success)
                                {
                                    cmd.CommandText = "ALTER TABLE dbo_vessel_unload_weight_validation ADD CONSTRAINT PrimaryKey PRIMARY KEY(row_id) ";
                                    cmd.ExecuteNonQuery();
                                    success = true;
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

        public static bool AddFieldToTable1(string fieldName)
        {
            bool success = false;
            string sql = "";
            switch (fieldName)
            {
                case "ref_no":
                    sql = "ALTER TABLE dbo_vessel_unload_1 ADD COLUMN ref_no varchar(25)";
                    break;
                case "is_catch_sold":
                    sql = "ALTER TABLE dbo_vessel_unload_1 ADD COLUMN is_catch_sold YESNO";
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
        public VesselUnloadRepository(GearUnload gu)
        {
            VesselUnloads = getVesselUnloads(gu);
        }
        public VesselUnloadRepository(bool isNew = false)
        {
            if (!isNew)
            {
                VesselUnloads = getVesselUnloads();
            }
        }
        public static bool UpdateUnloadWeightValidation(VesselUnload vu)
        {
            bool success = false;
            bool in_try_update = false;
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var conn = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {

                        cmd.Parameters.AddWithValue("@sum_catch_comp_wt", vu.SumOfCatchCompositionWeights);
                        //cmd.Parameters.AddWithValue("@sum_catch_sample_wt", vu.SumOfSampleWeights);
                        if (vu.SumOfSampleWeights == null)
                        {
                            cmd.Parameters.Add("@sum_catch_sample_wt", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@sum_catch_sample_wt", OleDbType.Double).Value = vu.SumOfSampleWeights;
                        }
                        cmd.Parameters.AddWithValue("@validation_flag", (int)vu.WeightValidationFlag);
                        cmd.Parameters.AddWithValue("@sampling_type_flag", (int)vu.SamplingTypeFlag);
                        if (vu.RaisingFactor == null)
                        {
                            cmd.Parameters.Add("@rf", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@rf", (double)vu.RaisingFactor);
                        }
                        if (vu.DifferenceCatchWtAndSumCatchCompWt == null)
                        {
                            cmd.Parameters.Add("@wt_diff", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@wt_diff", (double)vu.DifferenceCatchWtAndSumCatchCompWt);
                        }
                        cmd.Parameters.AddWithValue("@fv", vu.FormVersionCleaned);
                        cmd.Parameters.AddWithValue("@vu_id", vu.PK);

                        cmd.CommandText = @"UPDATE dbo_vessel_unload_weight_validation SET
                                                total_wt_catch_composition = @sum_catch_comp_wt,
                                                total_wt_sampled_species = @sum_catch_sample_wt,
                                                validity_flag = @validation_flag,
                                                type_of_sampling_flag = @sampling_type_flag,
                                                raising_factor = @rf,
                                                weight_difference = @wt_diff,
                                                form_version = @fv
                                                WHERE v_unload_id = @vu_id";

                        try
                        {
                            conn.Open();
                            in_try_update = true;
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }

                        if (in_try_update && !success)
                        {
                            cmd.CommandText = @"INSERT INTO dbo_vessel_unload_weight_validation (
                                                    total_wt_catch_composition,
                                                    total_wt_sampled_species,
                                                    validity_flag,
                                                    type_of_sampling_flag,
                                                    raising_factor,
                                                    weight_difference,
                                                    form_version,
                                                    v_unload_id)
                                                VALUES(
                                                    @sum_catch_comp_wt,
                                                    @sum_catch_sample_wt,
                                                    @validation_flag,
                                                    @sampling_type_flag,
                                                    @rf,
                                                    @wt_diff,
                                                    @fv,
                                                    @vu_id )";
                            try
                            {
                                success = cmd.ExecuteNonQuery() > 0;
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
        public static int VesselUnloadCount(bool isTracked = false)
        {
            int count = 0;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select count(*) from dbo_vessel_unload";
                        if (isTracked)
                        {
                            cmd.CommandText = "Select count(*) from dbo_vessel_unload_1 where tracked=1";
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
                        cmd.CommandText = "Select count(*) from dbo_vessel_unload";
                        if (isTracked)
                        {
                            cmd.CommandText = "Select count(*) from dbo_vessel_unload_1 where Tracked=-1";
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

        //public static VesselUnloadSummary GetSummary()
        //{
        //    VesselUnloadSummary vs = new VesselUnloadSummary();
        //    if (Global.Settings.UsemySQL)
        //    {
        //        using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
        //        {
        //            using (var cmd = conn.CreateCommand())
        //            {
        //                cmd.CommandText = @"SELECT 
        //                                        Min(sampling_date) AS FirstSD, 
        //                                        Max(sampling_date) AS LastSD, 
        //                                        Max(date_added) AS LatestDL
        //                                    FROM dbo_vessel_unload_1";
        //                conn.Open();
        //                var dr = cmd.ExecuteReader();
        //                while (dr.Read())
        //                {
        //                    vs.FirstSamplingDate = (DateTime)dr["FirstSD"];
        //                    vs.LastSamplingDate = (DateTime)dr["LastSD"];
        //                    vs.LatestDownloadDate = (DateTime)dr["LatestDL"];
        //                }
        //                dr.Close();
        //                cmd.CommandText = @"SELECT Count(*) AS Expr1
        //                                    FROM dbo_vessel_unload_1
        //                                    WHERE has_catch_composition=1";
        //                vs.CountUnloadsWithCatchComposition = (int)(long)cmd.ExecuteScalar();

        //            }
        //        }
        //    }
        //    else
        //    {
        //        using (var conn = new OleDbConnection(Global.ConnectionString))
        //        {
        //            using (var cmd = conn.CreateCommand())
        //            {
        //                cmd.CommandText = @"SELECT 
        //                                        Min(SamplingDate) AS FirstSD, 
        //                                        Max(SamplingDate) AS LastSD, 
        //                                        Max(DateAdded) AS LatestDL
        //                                    FROM dbo_vessel_unload_1";
        //                conn.Open();
        //                var dr = cmd.ExecuteReader();
        //                while (dr.Read())
        //                {
        //                    vs.FirstSamplingDate = (DateTime)dr["FirstSD"];
        //                    vs.LastSamplingDate = (DateTime)dr["LastSD"];
        //                    vs.LatestDownloadDate = (DateTime)dr["LatestDL"];
        //                }
        //                dr.Close();
        //                cmd.CommandText = @"SELECT Count(*) AS Expr1
        //                                    FROM dbo_vessel_unload_1
        //                                    WHERE HasCatchComposition=-1";
        //                vs.CountUnloadsWithCatchComposition = (int)cmd.ExecuteScalar();

        //            }
        //        }
        //    }
        //    return vs;
        //}

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
                        cmd.CommandText = "SELECT Max(v_unload_id) AS max_id FROM dbo_vessel_unload";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (var conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(v_unload_id) AS max_id FROM dbo_vessel_unload";
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
                        nsapRegion.RegionName, 
                        dbo_LC_FG_sample_day.unload_day_id, 
                        fma.FMAName, fishingGround.FishingGroundName, 
                        [LandingSiteName] & ', ' & [Municipalities.Municipality] & ', ' & [ProvinceName] AS LandingSite, 
                        dbo_LC_FG_sample_day.land_ctr_text, 
                        dbo_LC_FG_sample_day.sdate, 
                        dbo_gear_unload.unload_gr_id, 
                        gear.GearName, dbo_gear_unload.gr_text, 
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
                        dbo_vessel_unload_1.GPS, 
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
                        NSAPEnumerator.EnumeratorName, 
                        dbo_vessel_unload_1.EnumeratorText, 
                        dbo_vessel_unload_1.DateAdded, 
                        dbo_vessel_unload_1.sector_code, 
                        dbo_vessel_unload_1.FromExcelDownload,
                        dbo_vessel_unload_1.has_etp_interaction,
                FROM nsapRegion RIGHT JOIN
                    ((Provinces RIGHT JOIN 
                    Municipalities ON 
                    Provinces.ProvNo = Municipalities.ProvNo) RIGHT JOIN 
                    (landingSite RIGHT JOIN 
                    (gear RIGHT JOIN 
                    (fma RIGHT JOIN 
                    (fishingVessel RIGHT JOIN 
                    (fishingGround RIGHT JOIN 
                    ((dbo_LC_FG_sample_day INNER JOIN 
                    (dbo_gear_unload INNER JOIN 
                    dbo_vessel_unload ON 
                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON
                    dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN
                    (dbo_vessel_unload_1 LEFT JOIN 
                    NSAPEnumerator ON 
                    dbo_vessel_unload_1.EnumeratorID = NSAPEnumerator.EnumeratorID) ON
                    dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id) ON
                    fishingGround.FishingGroundCode = dbo_LC_FG_sample_day.ground_id) ON
                    fishingVessel.VesselID = dbo_vessel_unload.boat_id) ON
                    fma.FMAID = dbo_LC_FG_sample_day.fma) ON
                    gear.GearCode = dbo_gear_unload.gr_id) ON
                    landingSite.LandingSiteID = dbo_LC_FG_sample_day.land_ctr_id) ON
                    Municipalities.MunNo = landingSite.Municipality) ON
                    nsapRegion.Code = Provinces.NSAPRegion
                WHERE (((dbo_vessel_unload_1.Tracked)=True))
                ORDER BY fma.FMAName, 
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
                                FromExcelDownload = (bool)dr["FromExcelDownload"],
                                HasETPInteraction = (bool)dr["has_etp_interaction"],
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

        public static bool UpdateUnloadStats(VesselUnload vu)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@efforts", vu.CountEffortIndicators);
                        cmd.Parameters.AddWithValue("@grids", vu.CountGrids);
                        cmd.Parameters.AddWithValue("@soaks", vu.CountGearSoak);
                        cmd.Parameters.AddWithValue("@catch", vu.CountCatchCompositionItems);

                        cmd.Parameters.AddWithValue("@lns", vu.CountLengthRows);
                        cmd.Parameters.AddWithValue("@lfs", vu.CountLenFreqRows);
                        cmd.Parameters.AddWithValue("@lws", vu.CountLenWtRows);
                        cmd.Parameters.AddWithValue("@ms", vu.CountMaturityRows);
                        cmd.Parameters.AddWithValue("@id", vu.PK);

                        cmd.CommandText = @"INSERT INTO dbo_vessel_unload_stats (count_effort,count_grid,count_soak,count_catch_composition,count_lengths,count_lenfreq,count_lenwt,count_maturity,v_unload_id) 
                                          values
                                          (@efforts,@grids,@soaks,@catch,@lns,@lfs,@lws,@ms,@id)";
                        try
                        {
                            conn.Open();
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch (MySqlException mx)
                        {
                            if (mx.Message.Contains("Duplicate entry"))
                            {

                            }
                            else
                            {
                                Logger.Log(mx);
                            }
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
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@efforts", vu.CountEffortIndicators);
                        cmd.Parameters.AddWithValue("@grids", vu.CountGrids);
                        cmd.Parameters.AddWithValue("@soaks", vu.CountGearSoak);
                        cmd.Parameters.AddWithValue("@catch", vu.CountCatchCompositionItems);

                        cmd.Parameters.AddWithValue("@lns", vu.CountLengthRows);
                        cmd.Parameters.AddWithValue("@lfs", vu.CountLenFreqRows);
                        cmd.Parameters.AddWithValue("@lws", vu.CountLenWtRows);
                        cmd.Parameters.AddWithValue("@ms", vu.CountMaturityRows);
                        cmd.Parameters.AddWithValue("@id", vu.PK);

                        cmd.CommandText = @"INSERT INTO dbo_vessel_unload_stats (count_effort,count_grid,count_soak,count_catch_composition,count_lengths,count_lenfreq,count_lenwt,count_maturity,v_unload_id) 
                                          values
                                          (@efforts,@grids,@soaks,@catch,@lns,@lfs,@lws,@ms,@id)";
                        try
                        {
                            conn.Open();
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch (MySqlException mx)
                        {
                            Logger.Log(mx);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("Could not find output table"))
                            {
                                var arr = ex.Message.Split('\'');
                                if (CreateTable1(arr[1]))
                                {
                                    return UpdateUnloadStats(vu);
                                }

                            }
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return success;
        }

        public static bool CreateTable1(string tableName)
        {
            bool success = false;
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    switch (tableName)
                    {
                        case "dbo_vessel_unload_stats":
                            cmd.CommandText = @"CREATE TABLE dbo_vessel_unload_stats (
                                                v_unload_id INTEGER,
                                                count_effort INTEGER,
                                                count_grid INTEGER,
                                                count_soak INTEGER,
                                                count_catch_composition INTEGER,
                                                count_lengths INTEGER,
                                                count_lenfreq INTEGER,
                                                count_lenwt INTEGER,
                                                count_maturity INTEGER,
                                                CONSTRAINT PrimaryKey PRIMARY KEY (v_unload_id),
                                                CONSTRAINT FK_unload_stats
                                                    FOREIGN KEY (v_unload_id) REFERENCES
                                                    dbo_vessel_unload (v_unload_id)
                                                )";
                            break;
                    }

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        success = true;

                    }
                    catch (OleDbException odx)
                    {
                        Logger.Log(odx);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }

            return success;
        }

        public bool AddWeightValidation(VesselUnload vu)
        {
            bool success = false;
            return success;
        }
        public bool AddUnloadStats(VesselUnload vu)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@efforts", vu.CountEffortIndicators);
                        cmd.Parameters.AddWithValue("@grids", vu.CountGrids);
                        cmd.Parameters.AddWithValue("@soaks", vu.CountGearSoak);
                        cmd.Parameters.AddWithValue("@catch", vu.CountCatchCompositionItems);

                        cmd.Parameters.AddWithValue("@lns", vu.CountLengthRows);
                        cmd.Parameters.AddWithValue("@lfs", vu.CountLenFreqRows);
                        cmd.Parameters.AddWithValue("@lws", vu.CountLenWtRows);
                        cmd.Parameters.AddWithValue("@ms", vu.CountMaturityRows);
                        cmd.Parameters.AddWithValue("@id", vu.PK);

                        cmd.CommandText = @"INSERT INTO dbo_vessel_unload_stats (count_effort,count_grid,count_soak,count_catch_composition,count_lengths,count_lenfreq,count_lenwt,count_maturity,v_unload_id) 
                                          values
                                          (@efforts,@grids,@soaks,@catch,@lns,@lfs,@lws,@ms,@id)";
                        try
                        {
                            conn.Open();
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch (MySqlException mx)
                        {
                            if (mx.Message.Contains("Duplicate entry"))
                            {

                            }
                            else
                            {
                                Logger.Log(mx);
                            }
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
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@efforts", vu.CountEffortIndicators);
                        cmd.Parameters.AddWithValue("@grids", vu.CountGrids);
                        cmd.Parameters.AddWithValue("@soaks", vu.CountGearSoak);
                        cmd.Parameters.AddWithValue("@catch", vu.CountCatchCompositionItems);

                        cmd.Parameters.AddWithValue("@lns", vu.CountLengthRows);
                        cmd.Parameters.AddWithValue("@lfs", vu.CountLenFreqRows);
                        cmd.Parameters.AddWithValue("@lws", vu.CountLenWtRows);
                        cmd.Parameters.AddWithValue("@ms", vu.CountMaturityRows);
                        cmd.Parameters.AddWithValue("@id", vu.PK);

                        cmd.CommandText = @"INSERT INTO dbo_vessel_unload_stats (count_effort,count_grid,count_soak,count_catch_composition,count_lengths,count_lenfreq,count_lenwt,count_maturity,v_unload_id) 
                                          values
                                          (@efforts,@grids,@soaks,@catch,@lns,@lfs,@lws,@ms,@id)";
                        try
                        {
                            conn.Open();
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch (MySqlException mx)
                        {
                            Logger.Log(mx);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("Could not find output table"))
                            {
                                var arr = ex.Message.Split('\'');
                                if (CreateTable(arr[1]))
                                {
                                    return AddUnloadStats(vu);
                                }

                            }
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return success;
        }

        public static bool CheckForLSS_SubmissionIDTable()
        {
            bool tableExists = false;
            using (var conn = new OleDbConnection(Utilities.Global.ConnectionString))
            {
                conn.Open();
                var schema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                if (schema.Rows
                    .OfType<DataRow>()
                    .Any(r => r.ItemArray[2].ToString() == "dbo_lss_submissionIDs"))
                {
                    tableExists = true;
                }

                if (!tableExists)
                {
                    tableExists = CreateTable("dbo_lss_submissionIDs");
                }
            }
            return tableExists;

        }

        public static bool CheckForGearInteractionTables()
        {
            int tableCount = 0;
            using (var conn = new OleDbConnection(Utilities.Global.ConnectionString))
            {
                conn.Open();
                var schema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                if (schema.Rows
                    .OfType<DataRow>()
                    .Any(r => r.ItemArray[2].ToString().ToLower() == "dbo_vessel_unload_etp_interaction"))
                {
                    tableCount++;
                }
                else
                {
                    if (CreateTable("dbo_vessel_unload_etp_interaction"))
                    {
                        tableCount++;
                    }
                }


                if (schema.Rows
                    .OfType<DataRow>()
                    .Any(r => r.ItemArray[2].ToString().ToLower() == "dbo_vessel_unload_etp_interaction_type"))
                {
                    tableCount++;
                }
                else
                {
                    if (CreateTable("dbo_vessel_unload_etp_interaction_type"))
                    {
                        tableCount++;
                    }
                }


            }
            return tableCount == 2;
        }
        public static bool CheckForWtValidationTable()
        {
            bool tableExists = false;
            using (var conn = new OleDbConnection(Utilities.Global.ConnectionString))
            {
                conn.Open();
                var schema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                if (schema.Rows
                    .OfType<DataRow>()
                    .Any(r => r.ItemArray[2].ToString().ToLower() == "dbo_vessel_unload_weight_validation"))
                {
                    tableExists = true;
                }

                if (!tableExists)
                {
                    tableExists = CreateTable("dbo_vessel_unload_weight_validation");
                }
            }
            return tableExists;

        }
        public static int? WeightValidationTableMaxID()
        {
            int? result = null;
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Max(v_unload_id) AS max_id FROM dbo_vessel_unload_weight_validation";
                    try
                    {
                        conn.Open();
                        result = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message != "Specified cast is not valid.")
                        {
                            Logger.Log(ex);
                        }
                        //ignore
                    }
                }
            }
            return result;
        }

        public static int? GetWeightValidationTableRecordCount()
        {
            int? result = null;
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Count(v_unload_id) AS max_id FROM dbo_vessel_unload_weight_validation";
                    try
                    {
                        conn.Open();
                        result = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message != "Specified cast is not valid.")
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return result;
        }
        public static bool BulkUpdateWeightValidationUsingCSV(StringBuilder csv)
        {
            bool success = false;
            string base_dir = AppDomain.CurrentDomain.BaseDirectory;
            string csv_file = $@"{base_dir}\temp.csv";

            System.IO.File.WriteAllText(csv_file, CreateTablesInAccess.GetColumnNamesCSV("dbo_vessel_unload_weight_validation") + "\r\n" + csv.ToString());

            using (OleDbConnection connection = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = $@"INSERT INTO dbo_vessel_unload_weight_validation SELECT * FROM [Text;FMT=Delimited;DATABASE={base_dir};HDR=yes].[temp.csv]";
                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
                return success;
            }
        }
        public static bool CreateTable(string tableName)
        {
            bool success = false;
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    switch (tableName)
                    {
                        case "dbo_vessel_unload_etp_interaction_type":
                            cmd.CommandText = @"CREATE TABLE dbo_vessel_unload_etp_interaction_type (
                                             v_unload_id INTEGER, 
                                             row_id INTEGER,
                                             etp_interaction  VARCHAR(20),
                                             other_interaction VARCHAR(150),
                                             CONSTRAINT PrimaryKey PRIMARY KEY (row_id),        
                                             CONSTRAINT FK_interaction
                                                FOREIGN KEY (v_unload_id) REFERENCES
                                                dbo_vessel_unload (v_unload_id)
                                             )";
                            break;
                        case "dbo_vessel_unload_etp_interaction":
                            cmd.CommandText = @"CREATE TABLE dbo_vessel_unload_etp_interaction (
                                             v_unload_id INTEGER, 
                                             row_id INTEGER,   
                                             etp_name  VARCHAR(20),
                                             CONSTRAINT PrimaryKey PRIMARY KEY (row_id),        
                                             CONSTRAINT FK_etp
                                                FOREIGN KEY (v_unload_id) REFERENCES
                                                dbo_vessel_unload (v_unload_id)
                                             )";
                            break;
                        case "dbo_vessel_unload_weight_validation":
                            cmd.CommandText = @"CREATE TABLE dbo_vessel_unload_weight_validation (
                                                v_unload_id INTEGER, 
                                                total_wt_catch_composition DOUBLE,
                                                total_wt_sampled_species DOUBLE,
                                                validity_flag INTEGER,
                                                type_of_sampling_flag INTEGER,
                                                weight_difference DOUBLE,
                                                form_version VARCHAR(10),
                                                raising_factor DOUBLE,
                                                CONSTRAINT PrimaryKey PRIMARY KEY (v_unload_id),
                                                CONSTRAINT FK_validation_stats
                                                    FOREIGN KEY (v_unload_id) REFERENCES
                                                    dbo_vessel_unload (v_unload_id)
                                                )";
                            break;
                        case "dbo_vessel_unload_stats":
                            cmd.CommandText = @"CREATE TABLE dbo_vessel_unload_stats (
                                                v_unload_id INTEGER,
                                                count_effort INTEGER,
                                                count_grid INTEGER,
                                                count_soak INTEGER,
                                                count_catch_composition INTEGER,
                                                count_lengths INTEGER,
                                                count_lenfreq INTEGER,
                                                count_lenwt INTEGER,
                                                count_maturity INTEGER,
                                                CONSTRAINT PrimaryKey PRIMARY KEY (v_unload_id),
                                                CONSTRAINT FK_unload_stats
                                                    FOREIGN KEY (v_unload_id) REFERENCES
                                                    dbo_vessel_unload (v_unload_id)
                                                )";
                            break;
                    }

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        success = true;

                    }
                    catch (OleDbException odx)
                    {
                        Logger.Log(odx);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }

            return success;
        }
        private List<VesselUnload> getFromMySQL(GearUnload gu = null)
        {
            List<VesselUnload> thisList = new List<VesselUnload>();
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    //cmd.CommandText = @"SELECT dvu.*, dvu1.* FROM dbo_vessel_unload As dvu
                    //    INNER JOIN dbo_vessel_unload_1 As dvu1 ON dvu.v_unload_id = dvu1.v_unload_id;";
                    cmd.CommandText = @"SELECT dvu.*, dvu1.*, dvu2.* FROM nsap_odk.dbo_vessel_unload As dvu 
                                        INNER JOIN nsap_odk.dbo_vessel_unload_1 As dvu1 ON dvu.v_unload_id = dvu1.v_unload_id 
                                        LEFT JOIN nsap_odk.dbo_vessel_unload_stats as dvu2 on dvu.v_unload_id=dvu2.v_unload_id";
                    if (gu != null)
                    {
                        cmd.Parameters.AddWithValue("@parentID", gu.PK);
                        cmd.CommandText = @"SELECT t1.*, t2.*, t3.*
                                    FROM(
                                        dbo_vessel_unload AS t1 
                                        INNER JOIN 
                                            dbo_vessel_unload_1 AS t2 ON t1.v_unload_id = t2.v_unload_id)
                                        LEFT JOIN 
                                            dbo_vessel_unload_stats AS t3 ON t1.v_unload_id = t3.v_unload_id
                                    WHERE
                                        t1.unload_gr_id = @parentID";
                    }

                    try
                    {
                        MySqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            VesselUnload item = new VesselUnload();
                            item.Parent = gu;
                            item.PK = (int)dr["v_unload_id"];
                            item.GearUnloadID = (int)dr["unload_gr_id"];
                            item.IsBoatUsed = (bool)dr["is_boat_used"];
                            item.VesselID = string.IsNullOrEmpty(dr["boat_id"].ToString()) ? null : (int?)dr["boat_id"];
                            item.VesselText = dr["boat_text"].ToString();
                            item.SectorCode = dr["sector_code"].ToString();
                            item.NumberOfFishers = string.IsNullOrEmpty(dr["number_of_fishers"].ToString()) ? null : (int?)dr["number_of_fishers"];
                            item.WeightOfCatch = string.IsNullOrEmpty(dr["catch_total"].ToString()) ? null : (double?)dr["catch_total"];
                            item.WeightOfCatchSample = string.IsNullOrEmpty(dr["catch_samp"].ToString()) ? null : (double?)dr["catch_samp"];
                            item.Boxes = string.IsNullOrEmpty(dr["boxes_total"].ToString()) ? null : (int?)dr["boxes_total"];
                            item.BoxesSampled = string.IsNullOrEmpty(dr["boxes_samp"].ToString()) ? null : (int?)dr["boxes_samp"];
                            //item.RaisingFactor = dr["raising_factor"] == DBNull.Value ? null : (double?)dr["raising_factor"];
                            item.NSAPEnumeratorID = string.IsNullOrEmpty(dr["enumerator_id"].ToString()) ? null : (int?)dr["enumerator_id"];
                            item.EnumeratorText = dr["enumerator_text"].ToString();
                            item.FishingTripIsCompleted = (bool)dr["trip_is_completed"];
                            item.OperationIsSuccessful = (bool)dr["success"];
                            item.OperationIsTracked = (bool)dr["tracked"];
                            item.ODKRowID = dr["row_id"].ToString();
                            item.DepartureFromLandingSite = string.IsNullOrEmpty(dr["departure_landing_site"].ToString()) ? null : (DateTime?)dr["departure_landing_site"];
                            item.ArrivalAtLandingSite = string.IsNullOrEmpty(dr["arrival_landing_site"].ToString()) ? null : (DateTime?)dr["arrival_landing_site"];
                            item.XFormIdentifier = dr["xform_identifier"].ToString();
                            item.XFormDate = dr["xform_date"] == DBNull.Value ? null : (DateTime?)dr["xform_date"];
                            item.UserName = dr["user_name"].ToString();
                            item.DeviceID = dr["device_id"].ToString();
                            item.DateTimeSubmitted = (DateTime)dr["datetime_submitted"];
                            item.FormVersion = dr["form_version"].ToString();
                            item.GPSCode = dr["gps"].ToString();
                            item.SamplingDate = (DateTime)dr["sampling_date"];
                            item.Notes = dr["notes"].ToString();
                            item.DateAddedToDatabase = dr["date_added"] == DBNull.Value ? null : (DateTime?)dr["date_added"];
                            item.FromExcelDownload = (bool)dr["from_excel_download"];


                            if (dr["count_catch_composition"] != DBNull.Value)
                            {
                                item.CountCatchCompositionItems = (int)dr["count_catch_composition"];
                                item.CountEffortIndicators = (int)dr["count_effort"];
                                item.CountGearSoak = (int)dr["count_soak"];
                                item.CountGrids = (int)dr["count_grid"];
                                item.CountLenFreqRows = (int)dr["count_lenfreq"];
                                item.CountLengthRows = (int)dr["count_lengths"];
                                item.CountLenWtRows = (int)dr["count_lenwt"];
                                item.CountMaturityRows = (int)dr["count_maturity"];
                            }


                            if (dr["has_catch_composition"] == DBNull.Value)
                            {
                                item.HasCatchComposition = false;
                            }
                            else
                            {
                                item.HasCatchComposition = (bool)dr["has_catch_composition"];
                            }
                            try
                            {
                                thisList.Add(item);
                            }
                            catch (MySqlException mx)
                            {
                                Logger.Log(mx);
                            }
                            catch (Exception ex)
                            {
                                Logger.Log(ex);
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return thisList;
        }


        public bool UpdateXFormIdentifierColumn(UpdateXFormIdentifierItem item)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        if (item._xform_id_string == null)
                        {
                            cmd.Parameters.AddWithValue("@xformID", "");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@xformID", item._xform_id_string);
                        }
                        cmd.Parameters.AddWithValue("@rowID", item._uuid);
                        cmd.CommandText = "UPDATE dbo_vessel_unload_1 set xform_identifier=@xformID WHERE row_id=@rowID";
                        try
                        {
                            conn.Open();
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch (MySqlException mx)
                        {
                            Logger.Log(mx);
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
                    using (var cmd = conn.CreateCommand())
                    {
                        if (item._xform_id_string == null)
                        {
                            cmd.Parameters.AddWithValue("@xformID", "");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@xformID", item._xform_id_string);
                        }
                        cmd.Parameters.AddWithValue("@rowID", $"{{{item._uuid}}}");
                        cmd.CommandText = "UPDATE dbo_vessel_unload_1 set XFormIdentifier=@xformID WHERE RowID=@rowID";
                        try
                        {
                            conn.Open();
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch (MySqlException mx)
                        {
                            Logger.Log(mx);
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
        public bool UpdateHasCatchCompositionColumn(UpdateHasCatchCompositionResultItem item)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        if (item.CatchCompGroupIncludeCatchcomp == null)
                        {
                            cmd.Parameters.AddWithValue("@hascc", 0);
                        }
                        else if (item.CatchCompGroupIncludeCatchcomp == "yes")
                        {
                            cmd.Parameters.AddWithValue("@hascc", 1);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@hascc", 0);
                        }
                        cmd.Parameters.AddWithValue("@rowID", item._uuid);
                        cmd.CommandText = "UPDATE dbo_vessel_unload_1 set has_catch_composition=@hascc WHERE row_id=@rowID";
                        try
                        {
                            conn.Open();
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch (MySqlException mx)
                        {
                            Logger.Log(mx);
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
                    using (var cmd = conn.CreateCommand())
                    {
                        if (item.CatchCompGroupIncludeCatchcomp == null)
                        {
                            cmd.Parameters.AddWithValue("@hascc", false);
                        }
                        else if (item.CatchCompGroupIncludeCatchcomp == "yes")
                        {
                            cmd.Parameters.AddWithValue("@hascc", true);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@hascc", false);
                        }
                        cmd.Parameters.AddWithValue("@rowID", $"{{{item._uuid}}}");
                        cmd.CommandText = "UPDATE dbo_vessel_unload_1 set HasCatchComposition=@hascc WHERE RowID=@rowID";
                        try
                        {
                            conn.Open();
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch (MySqlException mx)
                        {
                            Logger.Log(mx);
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
        public bool UpdateHasCatchCompositionColumn(bool hasCatchComposition, int rowID)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@hascc", hasCatchComposition);
                        cmd.Parameters.AddWithValue("@rowID", rowID);
                        cmd.CommandText = "UPDATE dbo_vessel_unload_1 set has_catch_composition=@hascc WHERE v_unload_id=@rowID";
                        try
                        {
                            conn.Open();
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch (MySqlException mx)
                        {
                            Logger.Log(mx);
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
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@hascc", hasCatchComposition);
                        cmd.Parameters.AddWithValue("@rowID", rowID);
                        cmd.CommandText = "UPDATE dbo_vessel_unload_1 set HasCatchComposition=@hascc WHERE v_unload_id=@rowID";
                        try
                        {
                            conn.Open();
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch (MySqlException mx)
                        {
                            Logger.Log(mx);
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
        private List<VesselUnload> getVesselUnloads(GearUnload gu = null)
        {
            List<VesselUnload> thisList = new List<VesselUnload>();
            if (Global.Settings.UsemySQL)
            {
                thisList = getFromMySQL(gu);
            }
            else
            {
                var dt = new DataTable();
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = conection.CreateCommand())
                    {
                        try
                        {
                            conection.Open();
                            cmd.CommandText = @"SELECT t1.*, t2.*, t3.*, t4.*
                                                FROM ((dbo_vessel_unload AS t1 INNER JOIN 
                                                       dbo_vessel_unload_1 AS t2 ON t1.v_unload_id = t2.v_unload_id) LEFT JOIN 
                                                       dbo_vessel_unload_stats AS t3 ON t1.v_unload_id = t3.v_unload_id) LEFT JOIN 
                                                       dbo_vessel_unload_weight_validation AS t4 ON t1.v_unload_id = t4.v_unload_id";

                            if (gu != null)
                            {
                                cmd.Parameters.AddWithValue("@parentID", gu.PK);
                                cmd.CommandText = @"SELECT t1.*, t2.*, t3.*, t4.*
                                                    FROM ((dbo_vessel_unload AS t1 INNER JOIN 
                                                           dbo_vessel_unload_1 AS t2 ON t1.v_unload_id = t2.v_unload_id) LEFT JOIN 
                                                           dbo_vessel_unload_stats AS t3 ON t1.v_unload_id = t3.v_unload_id) LEFT JOIN 
                                                           dbo_vessel_unload_weight_validation AS t4 ON t1.v_unload_id = t4.v_unload_id
                                                    WHERE t1.unload_gr_id=@parentID";
                            }

                            thisList.Clear();
                            OleDbDataReader dr = cmd.ExecuteReader();


                            while (dr.Read())
                            {

                                int? submission_id = null;
                                if (dr["submission_id"] != DBNull.Value)
                                {
                                    submission_id = (int)dr["submission_id"];
                                }

                                VesselUnload item = new VesselUnload();
                                item.Parent = gu;
                                item.PK = (int)dr["t1.v_unload_id"];
                                item.GearUnloadID = (int)dr["unload_gr_id"];
                                item.IsBoatUsed = (bool)dr["is_boat_used"];
                                item.VesselID = string.IsNullOrEmpty(dr["boat_id"].ToString()) ? null : (int?)dr["boat_id"];
                                item.VesselText = dr["boat_text"].ToString();
                                item.SectorCode = dr["sector_code"].ToString();
                                //item.NumberOfFishers = (int)dr["NumberOfFishers"];
                                item.NumberOfFishers = string.IsNullOrEmpty(dr["NumberOfFishers"].ToString()) ? null : (int?)dr["NumberOfFishers"];
                                item.WeightOfCatch = string.IsNullOrEmpty(dr["catch_total"].ToString()) ? null : (double?)dr["catch_total"];
                                item.WeightOfCatchSample = string.IsNullOrEmpty(dr["catch_samp"].ToString()) ? null : (double?)dr["catch_samp"];
                                item.Boxes = string.IsNullOrEmpty(dr["boxes_total"].ToString()) ? null : (int?)dr["boxes_total"];
                                item.BoxesSampled = string.IsNullOrEmpty(dr["boxes_samp"].ToString()) ? null : (int?)dr["boxes_samp"];
                                item.RaisingFactor = dr["t4.raising_factor"] == DBNull.Value ? null : (double?)dr["t4.raising_factor"];
                                item.NSAPEnumeratorID = string.IsNullOrEmpty(dr["EnumeratorID"].ToString()) ? null : (int?)dr["EnumeratorID"];
                                item.EnumeratorText = dr["EnumeratorText"].ToString();
                                item.SubmissionID = submission_id;

                                item.OperationIsSuccessful = (bool)dr["Success"];
                                item.OperationIsTracked = (bool)dr["Tracked"];
                                item.FishingTripIsCompleted = (bool)dr["trip_is_completed"];
                                item.ODKRowID = dr["RowID"].ToString();
                                item.DepartureFromLandingSite = string.IsNullOrEmpty(dr["DepartureLandingSite"].ToString()) ? null : (DateTime?)dr["DepartureLandingSite"];
                                item.ArrivalAtLandingSite = string.IsNullOrEmpty(dr["ArrivalLandingSite"].ToString()) ? null : (DateTime?)dr["ArrivalLandingSite"];
                                item.XFormIdentifier = dr["XFormIdentifier"].ToString();
                                item.XFormDate = dr["XFormDate"] == DBNull.Value ? null : (DateTime?)dr["XFormDate"];
                                item.UserName = dr["user_name"].ToString();
                                item.DeviceID = dr["device_id"].ToString();
                                item.DateTimeSubmitted = (DateTime)dr["datetime_submitted"];
                                item.FormVersion = dr["t2.form_version"].ToString();
                                item.GPSCode = dr["GPS"].ToString();
                                item.SamplingDate = (DateTime)dr["SamplingDate"];
                                item.Notes = dr["Notes"].ToString();
                                item.DateAddedToDatabase = dr["DateAdded"] == DBNull.Value ? null : (DateTime?)dr["DateAdded"];
                                item.FromExcelDownload = (bool)dr["FromExcelDownload"];
                                item.HasCatchComposition = (bool)dr["HasCatchComposition"];
                                item.RefNo = dr["ref_no"].ToString();
                                item.IsCatchSold = (bool)dr["is_catch_sold"];
                                item.IsMultiGear = (bool)dr["is_multigear"];
                                item.IncludeEffortIndicators = (bool)dr["include_effort_indicators"];
                                item.NumberOfSpeciesInCatchComposition = null;
                                item.LandingSiteSamplingSubmissionID = dr["lss_submisionID"].ToString();
                                if (dr["number_species_catch_composition"] != DBNull.Value)
                                {
                                    item.NumberOfSpeciesInCatchComposition = (int)dr["number_species_catch_composition"];
                                }

                                if (item.IsMultiGear)
                                {
                                    item.CountGearTypesUsed = (int)dr["count_gear_types"];
                                }
                                else
                                {
                                    item.CountGearTypesUsed = 1;
                                }

                                item.JSONFileName = dr["json_filename"].ToString();
                                if (dr["count_catch_composition"] != DBNull.Value)
                                {
                                    item.CountCatchCompositionItems = (int)dr["count_catch_composition"];
                                    item.CountEffortIndicators = (int)dr["count_effort"];
                                    item.CountGearSoak = (int)dr["count_soak"];
                                    item.CountGrids = (int)dr["count_grid"];
                                    item.CountLenFreqRows = (int)dr["count_lenfreq"];
                                    item.CountLengthRows = (int)dr["count_lengths"];
                                    item.CountLenWtRows = (int)dr["count_lenwt"];
                                    item.CountMaturityRows = (int)dr["count_maturity"];
                                }

                                if (dr["total_wt_catch_composition"] != DBNull.Value)
                                {
                                    item.SumOfSampleWeights = dr["total_wt_sampled_species"] == DBNull.Value ? 0 : (double)dr["total_wt_sampled_species"];
                                    item.SumOfCatchCompositionWeights = (double)dr["total_wt_catch_composition"];
                                    item.WeightValidationFlag = (WeightValidationFlag)(int)dr["validity_flag"];
                                    item.SamplingTypeFlag = (SamplingTypeFlag)(int)dr["type_of_sampling_flag"];
                                    if (dr["weight_difference"] == DBNull.Value)
                                    {
                                        item.DifferenceCatchWtAndSumCatchCompWt = null;
                                    }
                                    else
                                    {
                                        item.DifferenceCatchWtAndSumCatchCompWt = (double)dr["weight_difference"];
                                    }

                                }
                                if (item.Parent.Parent.IsMultiVessel && dr["sampling_sequence"] != DBNull.Value)
                                {
                                    item.SequenceOfSampling = (int)dr["sampling_sequence"];
                                }

                                //double? rf = null;
                                //if (dr["t4.raising_factor"] != DBNull.Value)
                                //{
                                //    rf = (double)dr["t4.raising_factor"];
                                //}
                                //item.RaisingFactor = rf;
                                //item.VesselCatchViewModel = new VesselCatchViewModel(null);
                                //item.FishingGroundGridViewModel = new FishingGroundGridViewModel(item);
                                //item.GearSoakViewModel = new GearSoakViewModel(item);
                                //item.VesselEffortViewModel = new VesselEffortViewModel(item);
                                item.HasInteractionWithETPs = (bool)dr["has_etp_interaction"];
                                thisList.Add(item);
                            }

                        }
                        catch (Exception ex)
                        {
                            switch (ex.HResult)
                            {
                                case -2147217865:
                                    if (ex.Message.Contains("The Microsoft Jet database engine cannot find the input table or query"))
                                    {
                                        var arr = ex.Message.Split('\'');
                                        if (CreateTable(arr[1]))
                                        {
                                            return thisList = getVesselUnloads();
                                        }
                                    }
                                    break;
                                case -2147024809:
                                    if (ex.Message.Contains("does not belong to table"))
                                    {
                                        var arr = ex.Message.Split('\'');
                                        if (UpdateTableDefinition(arr[1]))
                                        {
                                            UpdateAllTripCompleted();
                                            return thisList = getVesselUnloads();
                                        }
                                    }
                                    break;
                            }
                            Logger.Log(ex);
                        }

                    }
                }

            }
            return thisList;
        }

        private int UpdateAllTripCompleted()
        {
            var resultCount = 0;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                using (OleDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Update dbo_vessel_unload_1 SET trip_is_completed=true";
                    resultCount = cmd.ExecuteNonQuery();
                }
            }
            return resultCount;
        }
        private bool UpdateTableDefinition(string colName)
        {
            bool success = false;
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = "";
                switch (colName)
                {
                    case "HasCatchComposition":
                    case "trip_is_completed":

                        sql = $@"ALTER TABLE dbo_vessel_unload_1 ADD COLUMN {colName} YESNO";
                        break;
                    case "NumberOfFishers":
                        sql = $@"ALTER TABLE dbo_vessel_unload_1 ADD COLUMN {colName} INTEGER DEFAULT NULL";
                        break;
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
                    Logger.Log(dbex);
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
        private bool AddToMySQL(VesselUnload item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    string vesselText = item.VesselText == null ? "" : item.VesselText;
                    update.Parameters.Add("@unloadid", MySqlDbType.Int32).Value = item.PK;
                    update.Parameters.Add("@gearUnload", MySqlDbType.Int32).Value = item.GearUnloadID;

                    if (item.VesselID == null)
                    {
                        update.Parameters.Add("@vesselid", MySqlDbType.Int32).Value = DBNull.Value;
                    }
                    else
                    {
                        if (NSAPEntities.FishingVesselViewModel.GetFishingVessel((int)item.VesselID) != null)
                        {
                            update.Parameters.Add("@vesselid", MySqlDbType.Int32).Value = item.VesselID;
                        }
                        else
                        {
                            update.Parameters.Add("@vesselid", MySqlDbType.Int32).Value = DBNull.Value;
                            vesselText = ((int)item.VesselID).ToString();
                            if (item.Notes == null)
                            {
                                item.Notes = "(orphaned vessel ID)";
                            }
                            else
                            {
                                item.Notes += " (orphaned vessel ID)";
                            }
                        }
                    }

                    if (vesselText == null)
                    {
                        update.Parameters.Add("@vesseltext", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@vesseltext", MySqlDbType.VarChar).Value = vesselText;
                    }

                    if (item.RaisingFactor == null)
                    {
                        update.Parameters.Add("@raisingfactor", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@raisingfactor", MySqlDbType.Double).Value = item.RaisingFactor;
                    }

                    if (item.WeightOfCatch == null)
                    {
                        update.Parameters.Add("@wtcatch", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@wtcatch", MySqlDbType.Double).Value = item.WeightOfCatch;
                    }

                    if (item.WeightOfCatchSample == null)
                    {
                        update.Parameters.Add("@wtsample", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@wtsample", MySqlDbType.Double).Value = item.WeightOfCatchSample;
                    }

                    if (item.Boxes == null)
                    {
                        update.Parameters.Add("@boxes", MySqlDbType.Int32).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@boxes", MySqlDbType.Int32).Value = item.Boxes;
                    }

                    if (item.BoxesSampled == null)
                    {
                        update.Parameters.Add("@boxessampled", MySqlDbType.Int32).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@boxessampled", MySqlDbType.Int32).Value = item.Boxes;
                    }

                    update.Parameters.AddWithValue("@isboatused", item.IsBoatUsed);

                    update.CommandText = @"Insert into dbo_vessel_unload(v_unload_id, unload_gr_id,boat_id, boat_text, raising_factor,
                            catch_total,catch_samp,boxes_total,boxes_samp,is_boat_used)
                           Values (@unloadid,@gearUnload,@vesselid,@vesseltext,@raisingfactor,@wtcatch,@wtsample,@boxes,@boxessampled,@isboatused)";
                    try
                    {
                        conn.Open();
                        if (update.ExecuteNonQuery() > 0)
                        {
                            using (var update1 = conn.CreateCommand())
                            {

                                update1.Parameters.Add("@unloadid", MySqlDbType.Int32).Value = item.PK;
                                update1.Parameters.AddWithValue("@operation_success", item.OperationIsSuccessful);
                                update1.Parameters.AddWithValue("@operation_tracked", item.OperationIsTracked);
                                update1.Parameters.AddWithValue("@operation_completed", item.FishingTripIsCompleted);

                                if (item.DepartureFromLandingSite == null)
                                {
                                    update1.Parameters.Add("@departure_date", MySqlDbType.DateTime).Value = DBNull.Value;
                                }
                                else
                                {
                                    update1.Parameters.Add("@departure_date", MySqlDbType.DateTime).Value = item.DepartureFromLandingSite;
                                }

                                if (item.ArrivalAtLandingSite == null)
                                {
                                    update1.Parameters.Add("@arrival_date", MySqlDbType.DateTime).Value = DBNull.Value;
                                }
                                else
                                {
                                    update1.Parameters.Add("@arrival_date", MySqlDbType.DateTime).Value = item.ArrivalAtLandingSite;
                                }



                                update1.Parameters.Add("@row_id", MySqlDbType.Guid).Value = Guid.Parse(item.ODKRowID);

                                string xformID = item.XFormIdentifier == null ? "" : item.XFormIdentifier;
                                update1.Parameters.Add("@xform_id", MySqlDbType.VarChar).Value = xformID;


                                if (item.XFormDate == null)
                                {
                                    update1.Parameters.Add("@xform_date", MySqlDbType.DateTime).Value = DBNull.Value;
                                }
                                else
                                {
                                    update1.Parameters.Add("@xform_date", MySqlDbType.DateTime).Value = item.XFormDate;
                                }

                                update1.Parameters.Add("@sampling_date", MySqlDbType.DateTime).Value = item.SamplingDate;



                                update1.Parameters.Add("@user_name", MySqlDbType.VarChar).Value = item.UserName;
                                update1.Parameters.Add("@device_id", MySqlDbType.VarChar).Value = item.DeviceID;
                                update1.Parameters.Add("@date_submitted", MySqlDbType.DateTime).Value = item.DateTimeSubmitted;
                                update1.Parameters.Add("@form_version", MySqlDbType.VarChar).Value = item.FormVersion;


                                if (item.GPSCode == null || item.GPSCode.Length == 0 || NSAPEntities.GPSViewModel.GetGPS(item.GPSCode) == null)
                                {
                                    update1.Parameters.Add("@gps", MySqlDbType.VarChar).Value = DBNull.Value;
                                }
                                else
                                {
                                    update1.Parameters.Add("@gps", MySqlDbType.VarChar).Value = item.GPSCode;
                                }

                                string notes = item.Notes == null ? "" : item.Notes;
                                update1.Parameters.Add("@notes", MySqlDbType.VarChar).Value = notes;

                                if (item.NSAPEnumeratorID == null)
                                {
                                    update1.Parameters.Add("@enumerator", MySqlDbType.Int32).Value = DBNull.Value;
                                }
                                else
                                {
                                    update1.Parameters.Add("@enumerator", MySqlDbType.Int32).Value = item.NSAPEnumeratorID; ;
                                }

                                string en_text = item.EnumeratorText == null ? "" : item.EnumeratorText;
                                update1.Parameters.Add("@enumerator_text", MySqlDbType.VarChar).Value = en_text;

                                update1.Parameters.Add("@date_added", MySqlDbType.DateTime).Value = item.DateAddedToDatabase;

                                string sector = item.SectorCode == null ? "" : item.SectorCode;
                                update1.Parameters.Add("@sector_code", MySqlDbType.VarChar).Value = sector;

                                update1.Parameters.AddWithValue("@from_excel", item.FromExcelDownload);
                                update1.Parameters.AddWithValue("@has_catch_composition", item.HasCatchComposition);

                                if (item.NumberOfFishers == null)
                                {
                                    update1.Parameters.Add("@num_fisher", MySqlDbType.Int32).Value = DBNull.Value;
                                }
                                else
                                {
                                    update1.Parameters.Add("@num_fisher", MySqlDbType.Int32).Value = item.NumberOfFishers;
                                }

                                update1.CommandText = @"Insert into dbo_vessel_unload_1 
                                                (v_unload_id, success, tracked, trip_is_completed, departure_landing_site, arrival_landing_site, 
                                                row_id, xform_identifier, xform_date, sampling_date,
                                                user_name,device_id,datetime_submitted,form_version,
                                                gps,notes,enumerator_id,enumerator_text,date_added,sector_code,from_excel_download,has_catch_composition,number_of_fishers)
                                                Values (@unloadid,@operation_success,@operation_tracked,@operation_completed,@departure_date,@arrival_date,
                                                        @row_id,@xform_id,@xform_date,@sampling_date,@user_name,@device_id,@date_submitted,
                                                        @form_version,@gps,@notes,@enumerator,@enumerator_text,@date_added,@sector_code,@from_excel,@has_catch_composition,@num_fisher)";
                                try
                                {
                                    success = update1.ExecuteNonQuery() > 0;
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

        private bool _newFieldAdded = false;
        public bool Add(VesselUnload item)
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
                    string vesselText = item.VesselText == null ? "" : item.VesselText;
                    conn.Open();

                    var sql = @"Insert into dbo_vessel_unload(v_unload_id, unload_gr_id,boat_id, boat_text, raising_factor,
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
                            if (NSAPEntities.FishingVesselViewModel.GetFishingVessel((int)item.VesselID) != null)
                            {
                                update.Parameters.Add("@vesselid", OleDbType.Integer).Value = item.VesselID;
                            }
                            else
                            {
                                update.Parameters.Add("@vesselid", OleDbType.Integer).Value = DBNull.Value;
                                vesselText = ((int)item.VesselID).ToString();
                                if (item.Notes == null)
                                {
                                    item.Notes = "(orphaned vessel ID)";
                                }
                                else
                                {
                                    item.Notes += " (orphaned vessel ID)";
                                }
                            }
                        }

                        if (vesselText == null)
                        {
                            update.Parameters.Add("@vesseltext", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@vesseltext", OleDbType.VarChar).Value = vesselText;
                        }

                        if (item.RaisingFactor == null)
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

                            bool proceed = true;
                            if (!_newFieldAdded)
                            {
                                proceed = update.ExecuteNonQuery() > 0;
                            }
                            if (proceed)
                            {
                                sql = @"Insert into dbo_vessel_unload_1 
                                                (v_unload_id, Success, Tracked, trip_is_completed, DepartureLandingSite, ArrivalLandingSite, 
                                                RowID, XFormIdentifier, XFormDate, SamplingDate,
                                                user_name,device_id,datetime_submitted,form_version,
                                                GPS,Notes,EnumeratorID,EnumeratorText,DateAdded,sector_code,FromExcelDownload,HasCatchComposition,
                                                NumberOfFishers,ref_no,is_catch_sold,is_multigear,count_gear_types,number_species_catch_composition,
                                                include_effort,submission_id,lss_submisionID,has_etp_interaction)
                                    Values (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";

                                using (OleDbCommand update1 = new OleDbCommand(sql, conn))
                                {


                                    update1.Parameters.Add("@unloadid", OleDbType.Integer).Value = item.PK;
                                    update1.Parameters.Add("@operation_success", OleDbType.Boolean).Value = item.OperationIsSuccessful;
                                    update1.Parameters.Add("@operation_tracked", OleDbType.Boolean).Value = item.OperationIsTracked;
                                    update1.Parameters.Add("@operation_completed", OleDbType.Boolean).Value = item.FishingTripIsCompleted;

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


                                    if (item.GPSCode == null)
                                    {
                                        update1.Parameters.Add("@gps", OleDbType.VarChar).Value = "";
                                    }
                                    else
                                    {
                                        update1.Parameters.Add("@gps", OleDbType.VarChar).Value = item.GPSCode;
                                    }

                                    string notes = item.Notes == null ? "" : item.Notes;
                                    string refNo = item.RefNo;
                                    if (RefNoFieldSize < 150)
                                    {
                                        notes = $"RefNo:{refNo}\r\nNotes:{notes}";
                                        refNo = "";
                                    }
                                    update1.Parameters.Add("@notes", OleDbType.VarChar).Value = notes;

                                    if (item.NSAPEnumeratorID == null)
                                    {
                                        update1.Parameters.Add("@enumerator", OleDbType.Integer).Value = DBNull.Value;
                                    }
                                    else
                                    {
                                        update1.Parameters.Add("@enumerator", OleDbType.Integer).Value = item.NSAPEnumeratorID; ;
                                    }

                                    string en_text = item.EnumeratorText == null ? "" : item.EnumeratorText;
                                    update1.Parameters.Add("@enumerator_text", OleDbType.VarChar).Value = en_text;

                                    update1.Parameters.Add("@date_added", OleDbType.Date).Value = item.DateAddedToDatabase;

                                    string sector = item.SectorCode == null ? "" : item.SectorCode;
                                    update1.Parameters.Add("@sector_code", OleDbType.VarChar).Value = sector;

                                    update1.Parameters.Add("@from_excel", OleDbType.Boolean).Value = item.FromExcelDownload;
                                    update1.Parameters.Add("@hasCatchComposition", OleDbType.Boolean).Value = item.HasCatchComposition;
                                    if (item.NumberOfFishers == null)
                                    {
                                        update1.Parameters.Add("@num_fishers", OleDbType.Integer).Value = DBNull.Value;
                                    }
                                    else
                                    {
                                        update1.Parameters.Add("@num_fishers", OleDbType.Integer).Value = item.NumberOfFishers;
                                    }
                                    update1.Parameters.Add("@ref_no", OleDbType.VarChar).Value = refNo;
                                    update1.Parameters.Add("@is_catch_sold", OleDbType.Boolean).Value = item.IsCatchSold;
                                    update1.Parameters.Add("@is_multigear", OleDbType.Boolean).Value = item.IsMultiGear;

                                    if (item.IsMultiGear)
                                    {
                                        update1.Parameters.Add("@num_gear_type", OleDbType.Integer).Value = item.CountGearTypesUsed;
                                    }
                                    else
                                    {
                                        update1.Parameters.Add("@num_gear_type", OleDbType.Integer).Value = 1;
                                    }
                                    if (item.NumberOfSpeciesInCatchComposition == null)
                                    {
                                        update.Parameters.Add("@count_species_catch_comp", OleDbType.Integer).Value = DBNull.Value;
                                    }
                                    else
                                    {
                                        update.Parameters.Add("@count_species_catch_comp", OleDbType.Integer).Value = item.NumberOfSpeciesInCatchComposition;
                                    }
                                    update1.Parameters.Add("@include_effort", OleDbType.Boolean).Value = item.IncludeEffortIndicators;
                                    if (item.SubmissionID == null)
                                    {
                                        update1.Parameters.Add("@_id", OleDbType.Integer).Value = DBNull.Value;
                                    }
                                    else
                                    {
                                        update1.Parameters.Add("@_id", OleDbType.Integer).Value = item.SubmissionID;
                                    }
                                    update1.Parameters.Add("@lss_id", OleDbType.VarWChar).Value = item.LandingSiteSamplingSubmissionID;
                                    update1.Parameters.Add("@has_etp_interaction", OleDbType.Boolean).Value = item.HasInteractionWithETPs;
                                    try
                                    {
                                        success = update1.ExecuteNonQuery() > 0;
                                        _newFieldAdded = false;
                                    }
                                    catch (OleDbException dbex)
                                    {
                                        if (dbex.ErrorCode == -2147217900)
                                        {
                                            if (AddFieldToTable(dbex.Message))
                                            {
                                                _newFieldAdded = true;
                                                return Add(item);
                                            }
                                        }
                                        else
                                        {
                                            //Console.WriteLine(dbex.Message);
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
                            switch (odbex.ErrorCode)
                            {
                                case -2147467259:
                                    switch (odbex.Message)
                                    {
                                        case "You cannot add or change a record because a related record is required in table 'fishingVessel'.":
                                            break;
                                        default:
                                            if (odbex.Message.Contains("create duplicate values in the index, primary key, or relationship"))
                                            {

                                            }
                                            else
                                            {
                                                Logger.Log(odbex);
                                                success = false;
                                            }
                                            break;
                                    }
                                    break;
                                default:

                                    Logger.Log(odbex);
                                    success = false;
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

        public static bool AddETPs(VesselUnload vu)
        {
            int etp_list_inserted_count = 0;
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    var sql = @"Insert into dbo_vessel_unload_etp_interaction(v_unload_id, etp_name) Values (?,?)";
                    conn.Open();
                    foreach (var etp_item in vu.ETPsIntercatedWith)
                    {
                        using (OleDbCommand insert = new OleDbCommand(sql, conn))
                        {
                            insert.Parameters.Add("@id", OleDbType.Integer).Value = vu.PK;
                            insert.Parameters.Add("@etp", OleDbType.VarChar).Value = etp_item;
                            try
                            {
                                if (insert.ExecuteNonQuery() > 0)
                                {
                                    etp_list_inserted_count++;
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Log(ex);
                            }
                        }
                    }
                }
            }
            return etp_list_inserted_count == vu.ETPsIntercatedWith.Count;
        }

        public static bool AddInteractions(VesselUnload vu)
        {
            int interaction_inserted_count = 0;
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    var sql = @"Insert into dbo_vessel_unload_etp_interaction_type(v_unload_id, etp_interaction,other_interaction) Values (?,?,?)";
                    conn.Open();
                    foreach (var interaction_item in vu.TypesOfIntercationWithETPs)
                    {
                        using (OleDbCommand insert = new OleDbCommand(sql, conn))
                        {
                            insert.Parameters.Add("@id", OleDbType.Integer).Value = vu.PK;
                            insert.Parameters.Add("@etp", OleDbType.VarChar).Value = interaction_item;
                            insert.Parameters.Add("@other", OleDbType.VarChar).Value = vu.OtherInteractionTypeWithETPs;
                            try
                            {
                                if (insert.ExecuteNonQuery() > 0)
                                {
                                    interaction_inserted_count++;
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Log(ex);
                            }
                        }
                    }
                }
            }
            return interaction_inserted_count == vu.TypesOfIntercationWithETPs.Count;
        }
        private bool AddFieldToTable(string errorMessage)
        {
            bool success = false;
            int s1 = errorMessage.IndexOf("name: '");
            int s2 = errorMessage.IndexOf("'.  Make");
            string newField = errorMessage.Substring(s1 + 7, s2 - (s1 + 7));
            switch (newField)
            {
                case "NumberOfFishers":
                    success = UpdateTableDefinition(newField);
                    break;
                case "trip_is_completed":
                case "HasCatchComposition":
                    success = UpdateTableDefinition(newField);
                    break;

            }
            return success;
        }

        private bool UpdateMySQL(VesselUnload item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Parameters.Add("@Unload_Gear_id", MySqlDbType.Int32).Value = item.Parent.PK;
                    if (item.VesselID == null)
                    {
                        cmd.Parameters.Add("@Boat_ID", MySqlDbType.Int32).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@Boat_ID", MySqlDbType.Int32).Value = item.VesselID;
                    }
                    cmd.Parameters.Add("@Boat_text", MySqlDbType.VarChar).Value = item.VesselText;

                    if (item.RaisingFactor == null)
                    {
                        cmd.Parameters.Add("@Raising_factor", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@Raising_factor", MySqlDbType.Double).Value = item.RaisingFactor;
                    }

                    if (item.WeightOfCatch == null)
                    {
                        cmd.Parameters.Add("@Weight_catch", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@Weight_catch", MySqlDbType.Double).Value = item.WeightOfCatch;
                    }

                    if (item.WeightOfCatchSample == null)
                    {
                        cmd.Parameters.Add("@Weight_sample", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@Weight_sample", MySqlDbType.Double).Value = item.WeightOfCatchSample;
                    }

                    if (item.Boxes == null)
                    {
                        cmd.Parameters.Add("@Boxes_count", MySqlDbType.Int32).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@Boxes_count", MySqlDbType.Int32).Value = item.Boxes;
                    }

                    if (item.BoxesSampled == null)
                    {
                        cmd.Parameters.Add("@Boxes_sampled", MySqlDbType.Int32).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@Boxes_sampled", MySqlDbType.Int32).Value = item.BoxesSampled;
                    }

                    cmd.Parameters.AddWithValue("@Is_boat_used", item.IsBoatUsed);
                    cmd.Parameters.Add("@Unload_id", MySqlDbType.Int32).Value = item.PK;


                    cmd.CommandText = @"UPDATE dbo_vessel_unload set
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
                        conn.Open();
                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            using (var cmd_1 = conn.CreateCommand())
                            {
                                cmd_1.Parameters.AddWithValue("@Operation_success", item.OperationIsSuccessful);
                                cmd_1.Parameters.AddWithValue("@Operation_is_tracked", item.OperationIsTracked);
                                cmd_1.Parameters.AddWithValue("@Operation_is_completed", item.FishingTripIsCompleted);
                                if (item.DepartureFromLandingSite == null)
                                {
                                    cmd_1.Parameters.Add("@Departure_date", MySqlDbType.DateTime).Value = DBNull.Value;
                                }
                                else
                                {
                                    cmd_1.Parameters.Add("@Departure_date", MySqlDbType.DateTime).Value = item.DepartureFromLandingSite;
                                }
                                if (item.ArrivalAtLandingSite == null)
                                {
                                    cmd_1.Parameters.Add("@Arrival_date", MySqlDbType.DateTime).Value = DBNull.Value;
                                }
                                else
                                {
                                    cmd_1.Parameters.Add("@Arrival_date", MySqlDbType.DateTime).Value = item.ArrivalAtLandingSite;
                                }
                                cmd_1.Parameters.Add("@ODK_row_id", MySqlDbType.VarChar).Value = item.ODKRowID;
                                if (item.XFormIdentifier == null)
                                {
                                    cmd_1.Parameters.Add("@XForm_id", MySqlDbType.VarChar).Value = DBNull.Value;
                                }
                                else
                                {
                                    cmd_1.Parameters.Add("@XForm_id", MySqlDbType.VarChar).Value = item.XFormIdentifier;
                                }
                                if (item.XFormDate == null)
                                {
                                    cmd_1.Parameters.Add("@Xform_date", MySqlDbType.DateTime).Value = DBNull.Value;
                                }
                                else
                                {
                                    cmd_1.Parameters.Add("@Xform_date", MySqlDbType.DateTime).Value = item.XFormDate;
                                }
                                cmd_1.Parameters.Add("@Sampling_date", MySqlDbType.DateTime).Value = item.SamplingDate;
                                cmd_1.Parameters.Add("@User_name", MySqlDbType.VarChar).Value = item.UserName;
                                cmd_1.Parameters.Add("@Device_id", MySqlDbType.VarChar).Value = item.DeviceID;
                                cmd_1.Parameters.Add("@Date_submitted", MySqlDbType.DateTime).Value = item.DateTimeSubmitted;
                                cmd_1.Parameters.Add("@Form_version", MySqlDbType.VarChar).Value = item.FormVersion;
                                if (item.GPSCode == null || item.GPSCode.Length == 0)
                                {
                                    cmd_1.Parameters.Add("@GPS_code", MySqlDbType.VarChar).Value = DBNull.Value;
                                }
                                else
                                {
                                    cmd_1.Parameters.Add("@GPS_code", MySqlDbType.VarChar).Value = item.GPSCode;
                                }
                                if (item.Notes == null)
                                {
                                    cmd_1.Parameters.Add("@Notes", MySqlDbType.VarChar).Value = DBNull.Value;
                                }
                                else
                                {
                                    cmd_1.Parameters.Add("@Notes", MySqlDbType.VarChar).Value = item.Notes;
                                }
                                if (item.NSAPEnumeratorID == null)
                                {
                                    cmd_1.Parameters.Add("@Enumerator_id", MySqlDbType.Int32).Value = DBNull.Value;
                                }
                                else
                                {
                                    cmd_1.Parameters.Add("@Enumerator_id", MySqlDbType.Int32).Value = item.NSAPEnumeratorID;
                                }
                                cmd_1.Parameters.Add("@Enumerator_text", MySqlDbType.VarChar).Value = item.EnumeratorText;
                                cmd_1.Parameters.Add("@Date_added", MySqlDbType.Date).Value = item.DateAddedToDatabase;
                                cmd_1.Parameters.Add("@Sector_code", MySqlDbType.VarChar).Value = item.SectorCode;
                                cmd_1.Parameters.AddWithValue("@From_excel", item.FromExcelDownload);
                                cmd_1.Parameters.AddWithValue("@has_catch_composition", item.HasCatchComposition);
                                if (item.NumberOfFishers == null)
                                {
                                    cmd_1.Parameters.Add("@num_fisher", MySqlDbType.Int32).Value = DBNull.Value;
                                }
                                else
                                {
                                    cmd_1.Parameters.Add("@num_fisher", MySqlDbType.Int32).Value = item.NumberOfFishers;
                                }
                                cmd_1.Parameters.Add("@Vessel_unload_id", MySqlDbType.Int32).Value = item.PK;


                                cmd_1.CommandText = $@"UPDATE dbo_vessel_unload_1 SET
                                        success = @Operation_success,
                                        tracked =  @Operation_is_tracked,
                                        trip_is_completed = @Operation_is_completed,
                                        departure_landing_site = @Departure_date,
                                        arrival_landing_site = @Arrival_date, 
                                        row_id =  @ODK_row_id,
                                        xform_identifier = @XForm_id,
                                        xform_date = @Xform_date, 
                                        sampling_date = @Sampling_date,
                                        user_name = @User_name,
                                        device_id = @Device_id,
                                        datetime_submitted = @Date_submitted,
                                        form_version = @Form_version,
                                        gps = @GPS_code,
                                        notes = @Notes,
                                        enumerator_id = @Enumerator_id,
                                        enumerator_text = @Enumerator_text,
                                        date_added = @Date_added,
                                        sector_code = @Sector_code,
                                        from_excel_download =  @From_excel,
                                        has_catch_composition = @has_catch_composition,
                                        number_of_fishers = @num_fisher
                                        WHERE v_unload_id =@Vessel_unload_id";

                                try
                                {
                                    success = cmd_1.ExecuteNonQuery() > 0;
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

        public bool UpdateEx(VesselUnload item)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                using (MySqlConnection con = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {

                        cmd.Parameters.AddWithValue("@xform_id", item.XFormIdentifier);
                        cmd.Parameters.AddWithValue("@odk_row_id", item.ODKRowID);
                        cmd.CommandText = @"Update dbo_vessel_unload_1 SET
                                            xform_identifier = @xform_id
                                            Where row_id=@odk_row_id";
                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch (MySqlException msex)
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
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    using (OleDbCommand cmd = conn.CreateCommand())
                    {
                        if (item.XFormIdentifier == null)
                        {
                            cmd.Parameters.Add("@XForm_id", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@XForm_id", OleDbType.VarChar).Value = item.XFormIdentifier;
                        }

                        cmd.Parameters.Add("@ODK_row_id", OleDbType.VarChar).Value = $"{{{item.ODKRowID}}}";

                        cmd.CommandText = $@"UPDATE dbo_vessel_unload_1 SET
                                        XFormIdentifier = @XForm_id
                                        WHERE RowID =@ODK_row_id";


                        try
                        {
                            conn.Open();
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException odbx)
                        {

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

        public static bool UpdateMissingLandingSite(int rowID, string landingSiteText)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@row_id", rowID);
                        cmd.Parameters.AddWithValue("@landing_site", landingSiteText);
                        cmd.CommandText = "UPDATE dbo_lc_fg_sample_day SET land_ctr_text=@landing_site WHERE unload_day_id=@row_id";
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
            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@row_id", rowID);
                        cmd.Parameters.AddWithValue("@landing_site", landingSiteText);
                        cmd.CommandText = "UPDATE dbo_LC_FG_sample_day SET land_ctr_text=@landing_site WHERE unload_day_id=@row_id";
                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException oex)
                        {
                            Logger.Log(oex);
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

        public static bool UpdateEnumerator(VesselUnload vu)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {

                        cmd.Parameters.AddWithValue("@en_id", vu.NSAPEnumerator.ID);
                        cmd.Parameters.AddWithValue("@rowID", vu.ODKRowID);
                        cmd.CommandText = "UPDATE dbo_vessel_unload_1 set enumerator_id=@en_id WHERE row_id=@rowID";
                        try
                        {
                            conn.Open();
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch (MySqlException mx)
                        {
                            Logger.Log(mx);
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
                        cmd.Parameters.AddWithValue("@en_id", vu.NSAPEnumerator.ID);
                        //cmd.Parameters.AddWithValue("@rowID", rowID);
                        cmd.Parameters.AddWithValue("@rowID", $"{{{vu.ODKRowID}}}");
                        cmd.CommandText = "UPDATE dbo_vessel_unload_1 set EnumeratorID=@en_id WHERE RowID=@rowID";
                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException oex)
                        {
                            Logger.Log(oex);
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
        public static bool UpdateXFormID(string rowID, string xFormID)
        {
            bool success = false;
            if (Utilities.Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {

                        cmd.Parameters.AddWithValue("@xformID", xFormID);
                        cmd.Parameters.AddWithValue("@rowID", rowID);
                        cmd.CommandText = "UPDATE dbo_vessel_unload_1 set xform_identifier=@xformID WHERE row_id=@rowID";
                        try
                        {
                            conn.Open();
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch (MySqlException mx)
                        {
                            Logger.Log(mx);
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
                        cmd.Parameters.AddWithValue("@xformID", xFormID);
                        //cmd.Parameters.AddWithValue("@rowID", rowID);
                        cmd.Parameters.AddWithValue("@rowID", $"{{{rowID}}}");
                        cmd.CommandText = "UPDATE dbo_vessel_unload_1 set XFormIdentifier=@xformID WHERE RowID=@rowID";
                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException oex)
                        {
                            Logger.Log(oex);
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
        public bool Update(VesselUnload item)
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

                        if (item.RaisingFactor == null)
                        {
                            cmd.Parameters.Add("@Raising_factor", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@Raising_factor", OleDbType.Double).Value = item.RaisingFactor;
                        }

                        if (item.WeightOfCatch == null)
                        {
                            cmd.Parameters.Add("@Weight_catch", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@Weight_catch", OleDbType.Double).Value = item.WeightOfCatch;
                        }

                        if (item.WeightOfCatchSample == null)
                        {
                            cmd.Parameters.Add("@Weight_sample", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@Weight_sample", OleDbType.Double).Value = item.WeightOfCatchSample;
                        }

                        if (item.Boxes == null)
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


                        cmd.CommandText = @"UPDATE dbo_vessel_unload set
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
                                cmd_1.Parameters.Add("@Operation_is_completed", OleDbType.Boolean).Value = item.FishingTripIsCompleted;
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
                                cmd_1.Parameters.Add("@has_catch_composition", OleDbType.Boolean).Value = item.HasCatchComposition;
                                if (item.NumberOfFishers == null)
                                {
                                    cmd_1.Parameters.Add("@num_fisher", OleDbType.Integer).Value = DBNull.Value;
                                }
                                else
                                {
                                    cmd_1.Parameters.Add("@num_fisher", OleDbType.Integer).Value = item.NumberOfFishers;
                                }


                                cmd_1.Parameters.Add("@ref_no", OleDbType.VarChar).Value = item.RefNo;


                                cmd_1.Parameters.Add("@is_catch_sold", OleDbType.Boolean).Value = item.IsCatchSold;

                                cmd_1.Parameters.Add("@is_multigear", OleDbType.Boolean).Value = item.IsMultiGear;
                                if (item.IsMultiGear)
                                {
                                    cmd_1.Parameters.Add("@num_gear_type", OleDbType.Integer).Value = item.CountGearTypesUsed;
                                }
                                else
                                {
                                    cmd_1.Parameters.Add("@num_gear_type", OleDbType.Integer).Value = 1;
                                }
                                if (item.NumberOfSpeciesInCatchComposition == null)
                                {
                                    cmd_1.Parameters.Add("@count_species_catch_comp", OleDbType.Integer).Value = DBNull.Value;
                                }
                                else
                                {
                                    cmd_1.Parameters.Add("@count_species_catch_comp", OleDbType.Integer).Value = item.NumberOfSpeciesInCatchComposition;
                                }
                                cmd_1.Parameters.Add("@include_effort", OleDbType.Boolean).Value = item.IncludeEffortIndicators;
                                cmd_1.Parameters.Add("@has_interaction_etp", OleDbType.Boolean).Value = item.HasInteractionWithETPs;
                                cmd_1.Parameters.Add("@lsss_id", OleDbType.VarChar).Value = item.LandingSiteSamplingSubmissionID;

                                if (item.SubmissionID == null)
                                {
                                    cmd_1.Parameters.Add("@_id", OleDbType.Integer).Value = DBNull.Value; ;
                                }
                                else
                                {
                                    cmd_1.Parameters.Add("@_id", OleDbType.Integer).Value = item.SubmissionID;
                                }


                                cmd_1.Parameters.Add("@Vessel_unload_id", OleDbType.Integer).Value = item.PK;

                                cmd_1.CommandText = @"UPDATE dbo_vessel_unload_1 SET
                                        Success = @Operation_success,
                                        Tracked =  @Operation_is_tracked,
                                        trip_is_completed = @Operation_is_completed,
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
                                        FromExcelDownload =  @From_excel,
                                        HasCatchComposition = @has_catch_composition,
                                        NumberOfFishers = @num_fisher,
                                        ref_no = @ref_no,
                                        is_catch_sold = @is_catch_sold,
                                        is_multigear = @is_multigear,
                                        count_gear_types = @num_gear_type,
                                        number_species_catch_composition = @count_species_catch_comp,
                                        include_effort_indicators = @include_effort,
                                        has_etp_interaction= @has_interaction_etp,
                                        lss_submisionID = @lsss_id,
                                        submission_id = @_id
                                        WHERE v_unload_id = @Vessel_unload_id";


                                success = false;

                                try
                                {
                                    if (cmd_1.ExecuteNonQuery() > 0)
                                    {
                                        if (item.HasInteractionWithETPs)
                                        {
                                            UpdateETPs(item);
                                            UpdateETPInteraction(item);

                                        }

                                        success = true;

                                    }
                                }
                                catch (OleDbException dbex)
                                {
                                    Logger.Log(dbex);
                                    //ignore
                                }
                                catch (Exception ex)
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
            }
            return success;
        }
        private static bool UpdateETPs(VesselUnload vu)
        {
            int updateCount = 0;
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var conn = new OleDbConnection())
                {
                    using (var update = conn.CreateCommand())
                    {
                        update.CommandText = @"UPDATE dbo_vessel_unload_etp_interaction SET
                                              etp_name=@etp_name
                                              WHERE v_unload_id=@id";
                        foreach (var item in vu.ETPsIntercatedWith)
                        {
                            update.Parameters.Add("@etp_name", OleDbType.VarChar).Value = item;
                            update.Parameters.Add("@id", OleDbType.Numeric).Value = vu.PK;
                        }
                        try
                        {
                            if(update.ExecuteNonQuery() > 0)
                            {
                                updateCount++;
                            }
                        }
                        catch(Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return updateCount==vu.ETPsIntercatedWith.Count;
        }

        private static bool UpdateETPInteraction(VesselUnload vu)
        {
            int updateCount = 0;
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var conn = new OleDbConnection())
                {
                    using (var update = conn.CreateCommand())
                    {
                        update.CommandText = @"UPDATE dbo_vessel_unload_etp_interaction_type SET
                                              etp_interaction=@interaction,
                                              other_interaction=@other,
                                              WHERE v_unload_id=@id";
                        foreach (var item in vu.TypesOfIntercationWithETPs)
                        {
                            update.Parameters.Add("@interaction", OleDbType.VarChar).Value = item;
                            update.Parameters.Add("@other", OleDbType.VarChar).Value = vu.OtherInteractionTypeWithETPs;
                            update.Parameters.Add("@id", OleDbType.Numeric).Value = vu.PK;
                        }
                        try
                        {
                            if (update.ExecuteNonQuery() > 0)
                            {
                                updateCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return updateCount == vu.ETPsIntercatedWith.Count;
        }

        public static bool ClearWeightValidationTable(string otherConnectionString = "")
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
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"Delete * from dbo_vessel_unload_weight_validation";
                    try
                    {
                        cmd.ExecuteNonQuery();
                        success = true;
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
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"Delete * from dbo_vessel_unload_1";
                    try
                    {
                        cmd.ExecuteNonQuery();
                        success = true;
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

                if (success)
                {
                    success = false;
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $"Delete * from dbo_vessel_unload_stats";
                        try
                        {
                            cmd.ExecuteNonQuery();
                            success = true;
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

                if (success)
                {
                    success = false;
                    success = ClearWeightValidationTable(con_string);
                    //using (var cmd = conn.CreateCommand())
                    //{
                    //    cmd.CommandText = $"Delete * from dbo_vessel_unload_weight_validation";
                    //    try
                    //    {
                    //        cmd.ExecuteNonQuery();
                    //        success = true;
                    //    }
                    //    catch (OleDbException olx)
                    //    {
                    //        Logger.Log(olx);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Logger.Log(ex);
                    //    }
                    //}
                }

                if (success)
                {
                    success = false;
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $"Delete * from dbo_vessel_unload";
                        try
                        {

                            cmd.ExecuteNonQuery();
                            success = true;
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

            return success;
        }
        public static bool ClearTable1()
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
                        sql = $"Delete * from dbo_vessel_unload_stats";
                        using (OleDbCommand update1 = new OleDbCommand(sql, conn))
                        {
                            try
                            {
                                update1.ExecuteNonQuery();
                                success = true;
                            }
                            catch (OleDbException olx)
                            {
                                Logger.Log(olx);
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
        private bool DeleteMySQL(int id)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    update.CommandText = "Delete  from dbo_vessel_unload_1 where v_unload_id=@id";
                    try
                    {
                        conn.Open();
                        update.ExecuteNonQuery(); ;
                        using (var update1 = conn.CreateCommand())
                        {
                            update1.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                            update1.CommandText = "Delete  from dbo_vessel_unload where v_unload_id=@id";
                            try
                            {
                                success = update1.ExecuteNonQuery() > 0;

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
                        update.CommandText = "Delete * from dbo_vessel_unload_1 where v_unload_id=@id";
                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
                            if (success)
                            {
                                success = false;
                                using (OleDbCommand delStatsCommand = conn.CreateCommand())
                                {
                                    delStatsCommand.Parameters.AddWithValue("@delStatID", id);
                                    delStatsCommand.CommandText = "Delete * from dbo_vessel_unload_stats where v_unload_id=@delStatID";
                                    try
                                    {
                                        //success = delStatsCommand.ExecuteNonQuery() > 0;
                                        delStatsCommand.ExecuteNonQuery();
                                        success = true;

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
                                if (success)
                                {
                                    success = false;
                                    using (OleDbCommand update2 = conn.CreateCommand())
                                    {
                                        update2.Parameters.Add("@id", OleDbType.Integer).Value = id;
                                        update2.CommandText = "Delete * from dbo_vessel_unload_weight_validation where v_unload_id=@id";
                                        try
                                        {
                                            update2.ExecuteNonQuery();
                                            success = true;
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.Log(ex);
                                        }
                                    }
                                }
                                if (success)
                                {
                                    success = false;
                                    using (OleDbCommand update1 = conn.CreateCommand())
                                    {
                                        update1.Parameters.Add("@id", OleDbType.Integer).Value = id;
                                        update1.CommandText = "Delete * from dbo_vessel_unload where v_unload_id=@id";
                                        try
                                        {
                                            success = update1.ExecuteNonQuery() > 0;

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
            }
            return success;
        }
    }
}
