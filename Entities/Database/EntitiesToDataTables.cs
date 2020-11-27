using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using FastMember;
using NSAP_ODK.Entities.Database.FromJson;

namespace NSAP_ODK.Entities.Database
{
    public static class EntitiesToDataTables
    {
        public static DataSet DataSet { get; internal set; }
        public static List<VesselLanding> VesselLandings { get; set; }
        public static DataSet GenerateDataSeFromImport()
        {
            DataSet = new System.Data.DataSet();


            DataTable dtVesselUnload = new DataTable();
            DataTable dtVesselCatch = new DataTable();
            DataTable dtGearSoak = new DataTable();
            DataTable dtFishingGrid = new DataTable();
            DataTable dtVesselEffort = new DataTable();
            DataTable dtLenFreq = new DataTable();
            DataTable dtLength = new DataTable();
            DataTable dtLenWt = new DataTable();
            DataTable dtMaturity = new DataTable();

            if (VesselLandings != null)
            {
                var mainSheetFlatteneds = new List<VesselLandingFlattened>();
                foreach (var item in VesselLandings)
                {
                    mainSheetFlatteneds.Add(new VesselLandingFlattened(item));
                }
                using (var reader = ObjectReader.Create(mainSheetFlatteneds,
                    "ID",
                    "Start",
                    "Device_id",
                    "User_name",
                    "Email",
                    "Version",
                    "SamplingDate",
                    "NSAPRegion",
                    "Enumerator",
                    "FMA",
                    "FishingGround",
                    "LandingSite",
                    "Gear",
                    "Sector",
                    "IsBoatUsed",
                    "Vessel",
                    "SuccessOperation",
                    "CatchTotalWt",
                    "CatchSampleWt",
                    "BoxesTotal",
                    "BoxesSampled",
                    "RasingFactor",
                    "Remarks",
                    "IncludeTracking",
                    "UTMZone",
                    "DepartureLandingSite",
                    "ArrivalLandingSite",
                    "GPS",
                    "_version",
                    "_metaID",
                    "_id",
                    "_uuid",
                    "SubmissionTime",
                    "SavedToDatabase"
                    ))
                { dtVesselUnload.Load(reader); }

                using (var reader = ObjectReader.Create(VesselUnloadServerRepository.GetGridBingoCoordinates(),
                        "PK",
                        "ParentID",
                        "CompleteGridName",
                        "LongLat",
                        "UTMCoordinate"))
                { dtFishingGrid.Load(reader); }

                using (var reader = ObjectReader.Create(VesselUnloadServerRepository.GetGearSoakTimes(),
                        "PK",
                        "ParentID",
                        "SetTime",
                        "HaulTime",
                        "WaypointAtSet",
                        "WaypointAtHaul"))
                { dtGearSoak.Load(reader); }

                using (var reader = ObjectReader.Create(VesselUnloadServerRepository.GetGearEfforts(),
                        "PK",
                        "ParentID",
                        "EffortSpecification",
                        "EffortNumericValue",
                        "EffortTextValue",
                        "EffortBooleanValue"
                        ))
                { dtVesselEffort.Load(reader); }

                using (var reader = ObjectReader.Create(VesselUnloadServerRepository.GetCatchCompositions(),
                        "PK",
                        "ParentID",
                        "TaxaName",
                        "SpeciesName",
                        "WeightOfCatch",
                        "SampleWeightOfCatch"))
                { dtVesselCatch.Load(reader); }

                using (var reader = ObjectReader.Create(VesselUnloadServerRepository.GetLenFreqList(),
                        "PK",
                        "ParentID",
                        "LengthClass",
                        "Frequency"))
                { dtLenFreq.Load(reader); }

                using (var reader = ObjectReader.Create(VesselUnloadServerRepository.GetLenWtList(),
                        "PK",
                        "ParentID",
                        "Length",
                        "Weight"))
                { dtLenWt.Load(reader); }

                using (var reader = ObjectReader.Create(VesselUnloadServerRepository.GetLengthList(),
                        "PK",
                        "ParentID",
                        "Length"))
                { dtLength.Load(reader); }

                using (var reader = ObjectReader.Create(VesselUnloadServerRepository.GetGMSList(),
                        "PK",
                        "ParentID",
                        "Length",
                        "Weight",
                        "Sex",
                        "MaturityStage",
                        "GutContentWeight",
                        "GutContentCategory"))
                { dtMaturity.Load(reader); }
            }
            else
            {

            }

            dtVesselUnload.TableName = "Sampling";
            dtFishingGrid.TableName = "Fishing ground grid";
            dtGearSoak.TableName = "Gear soak time";
            dtVesselEffort.TableName = "Fishing effort specification";
            dtVesselCatch.TableName = "Catch composition";
            dtLenFreq.TableName = "Length frequency";
            dtLenWt.TableName = "Length-Weight";
            dtLength.TableName = "Lenghts";
            dtMaturity.TableName = "Maturity";

            DataSet.Tables.Add(dtVesselUnload);
            DataSet.Tables.Add(dtFishingGrid);
            DataSet.Tables.Add(dtGearSoak);
            DataSet.Tables.Add(dtVesselEffort);
            DataSet.Tables.Add(dtVesselCatch);
            DataSet.Tables.Add(dtLenFreq);
            DataSet.Tables.Add(dtLenWt);
            DataSet.Tables.Add(dtLength);
            DataSet.Tables.Add(dtMaturity);

            return DataSet;
        }

        public static DataSet GenerateDataset(bool tracked=false)
        {
            DataSet = new System.Data.DataSet();

            DataTable dtSampling = new DataTable();
            DataTable dtGearUnload = new DataTable();
            DataTable dtVesselUnload = new DataTable();
            DataTable dtVesselCatch = new DataTable();
            DataTable dtGearSoak = new DataTable();
            DataTable dtFishingGrid = new DataTable();
            DataTable dtVesselEffort = new DataTable();
            DataTable dtLenFreq = new DataTable();
            DataTable dtLength = new DataTable();
            DataTable dtLenWt = new DataTable();
            DataTable dtMaturity = new DataTable();

            if (!tracked)
            {
                using (var reader = ObjectReader.Create
                    (
                    NSAPEntities.LandingSiteSamplingViewModel.GetAllFlattenedItems(),
                    "ID",
                    "SamplingDate",
                    "NSAPRegion",
                    "FMA",
                    "FishingGround",
                    "LandingSite",
                    "Remarks"
                    ))
                {
                    dtSampling.Load(reader);
                }

                using (var reader = ObjectReader.Create
                    (
                    NSAPEntities.GearUnloadViewModel.GetAllFlattenedItems(),
                     "ID",
                     "ParentID",
                    "SamplingDate",
                    "NSAPRegion",
                    "FMA",
                    "FishingGround",
                    "LandingSite",
                    "Gear",
                    "Boats",
                    "Catch"
                    ))
                {
                    dtGearUnload.Load(reader);
                }

                using (var reader = ObjectReader.Create
                (
                NSAPEntities.VesselUnloadViewModel.GetAllFlattenedItems(),
                    "ID",
                    "ParentID",
                    "SamplingDate",
                    "Gear",
                    "IsBoatUsed",
                    "FishingVessel",
                    "Sector",
                    "SamplingEnumerator",
                    "OperationIsSuccessful",
                    "WeightOfCatch",
                    "WeightOfCatchSample",
                    "Boxes",
                    "BoxesSampled",
                    "Notes",
                    "OperationIsTracked",
                    "GPS",
                    "DepartureFromLandingSite",
                    "ArrivalAtLandingSite",
                    "ODKRowID",
                    "FormVersion",
                    "UserName",
                    "DeviceID",
                    "Submitted",
                    "XFormIdentifier",
                    "XFormDate",
                    "DateAddedToDatabase"
                ))
                {
                    dtVesselUnload.Load(reader);
                }


                dtSampling.TableName = "dbo_LC_FG_sample_day";
                dtGearUnload.TableName = "dbo_gear_unload";
                dtVesselUnload.TableName = "dbo_vessel_unload";

                DataSet.Tables.Add(dtSampling);
                DataSet.Tables.Add(dtGearUnload);
                DataSet.Tables.Add(dtVesselUnload);

            }
            else
            {
                DataTable dtVesselUnloadTracked = new DataTable();
                try
                {
                    using (var reader = ObjectReader.Create
                    (
                    NSAPEntities.VesselUnloadViewModel.GetAllTrackedFlattenedItems(),
                        "SamplingDayID",
                        "Region",
                        "FMA",
                        "FishingGround",
                        "LandingSite",
                        "SamplingDate",
                        "GearUnloadID",
                        "Gear",
                        "BoatsLanded",
                        "CatchTotalLanded",
                        "VesselUnloadID",
                        "SamplingDateTime",
                        "Enumerator",
                        "Sector",
                        "IsBoatUsed",
                        "Vessel",
                        "CatchTotalWt",
                        "CatchSampleWt",
                        "Boxes",
                        "BoxesSampled",
                        "IsSuccess",
                        "IsTracked",
                        "GPS",
                        "Departure",
                        "Arrival",
                        "RowID",
                        "XFormIdentifier",
                        "XFormDate",
                        "UserName",
                        "DeviceID",
                        "Submitted",
                        "FormVersion",
                        "Notes",
                        "DateAddedToDatabase",
                        "FromExcelDownload"
                    ))
                    {
                        dtVesselUnloadTracked.Load(reader);
                    }
                }
                catch (Exception ex)
                {
                    Utilities.Logger.Log(ex);
                }

                dtVesselUnloadTracked.TableName = "dbo_vessel_unload_tracked";

                DataSet.Tables.Add(dtVesselUnloadTracked);
            }

            using (var reader = ObjectReader.Create
                 (
                 NSAPEntities.FishingGroundGridViewModel.GetAllFlattenedItems(tracked),
                  "ID",
                  "ParentID",
                 "UTMZone",
                 "Grid",
                 "LonLat",
                 "UTMCoordinate"
                 ))
            {
                dtFishingGrid.Load(reader);
            }

            using (var reader = ObjectReader.Create
                (
                NSAPEntities.GearSoakViewModel.GetAllFlattenedItems(tracked),
                "ID",
                "ParentID",
                "TimeAtSet",
                "TimeAtHaul",
                "WaypointAtSet",
                "WaypointAtHaul"
                ))
            {
                dtGearSoak.Load(reader);
            }

            using (var reader = ObjectReader.Create
            (
            NSAPEntities.VesselEffortViewModel.GetAllFlattenedItems(tracked),
            "ID",
            "ParentID",
            "EffortSpecification",
            "EffortValueNumeric",
            "EffortValueText"
            ))
            {
                dtVesselEffort.Load(reader);
            }

            using (var reader = ObjectReader.Create
            (
            NSAPEntities.VesselCatchViewModel.GetAllFlattenedItems(tracked),
             "ID",
             "ParentID",
            "CatchName",
            "Taxa",
            "Catch_kg",
            "Sample_kg"
            ))
            {
                dtVesselCatch.Load(reader);
            }

            using (var reader = ObjectReader.Create
            (
            NSAPEntities.CatchLenFreqViewModel.GetAllFlattenedItems(tracked),
            "ID",
            "ParentID",
            "CatchName",
            "Taxa",
            "Gear",
            "Length",
            "Frequency"
            ))
            {
                dtLenFreq.Load(reader);
            }

            using (var reader = ObjectReader.Create
            (
            NSAPEntities.CatchLengthViewModel.GetAllFlattenedItems(tracked),
            "ID",
            "ParentID",
            "CatchName",
            "Taxa",
            "Gear",
            "Length"
            ))
            {
                dtLength.Load(reader);
            }

            using (var reader = ObjectReader.Create
            (
            NSAPEntities.CatchLengthWeightViewModel.GetAllFlattenedItems(tracked),
            "ID",
            "ParentID",
            "CatchName",
            "Taxa",
            "Gear",
            "Length",
            "Weight"
            ))
            {
                dtLenWt.Load(reader);
            }

            using (var reader = ObjectReader.Create
            (
            NSAPEntities.CatchMaturityViewModel.GetAllFlattenedItems(tracked),
            "ID",
            "ParentID",
            "CatchName",
            "Taxa",
            "Gear",
            "Length",
            "Weight",
            "Sex",
            "Maturity",
            "WeightGutContent",
            "GutContentClassification"
            ))
            {
                dtMaturity.Load(reader);
            }

            dtVesselCatch.TableName = "dbo_vessel_catch";
            dtGearSoak.TableName = "dbo_gear_soak";
            dtFishingGrid.TableName = "dbo_fg_grid";
            dtVesselEffort.TableName = "dbo_vessel_effort";
            dtLenFreq.TableName = "dbo_catch_len_freq";
            dtLength.TableName = "dbo_catch_len";
            dtLenWt.TableName = "dbo_catch_len_wt";
            dtMaturity.TableName = "dbo_catch_maturity";


            DataSet.Tables.Add(dtFishingGrid);
            DataSet.Tables.Add(dtGearSoak);
            DataSet.Tables.Add(dtVesselEffort);
            DataSet.Tables.Add(dtVesselCatch);
            DataSet.Tables.Add(dtLenFreq);
            DataSet.Tables.Add(dtLength);
            DataSet.Tables.Add(dtLenWt);
            DataSet.Tables.Add(dtMaturity);

            return DataSet;

        }
        public static DataSet GenerateTrackedDataSet()
        {
            DataSet = new System.Data.DataSet();

            DataTable dtVesselUnloadTracked = new DataTable();
            try
            {
                using (var reader = ObjectReader.Create
                (
                NSAPEntities.VesselUnloadViewModel.GetAllTrackedFlattenedItems(),
                    "SamplingDayID",
                    "Region",
                    "FMA",
                    "FishingGround",
                    "LandingSite",
                    "SamplingDate",
                    "GearUnloadID",
                    "Gear",
                    "BoatsLanded",
                    "CatchTotalLanded",
                    "VesselUnloadID",
                    "SamplingDateTime",
                    "Enumerator",
                    "IsBoatUsed",
                    "Vessel",
                    "CatchTotalWt",
                    "CatchSampleWt",
                    "Boxes",
                    "BoxesSampled",
                    "IsSuccess",
                    "IsTracked",
                    "GPS",
                    "Departure",
                    "Arrival",
                    "RowID",
                    "ExcelName",
                    "ExcelDate",
                    "UserName",
                    "DeviceID",
                    "Submitted",
                    "FormVersion",
                    "Notes"
                ))
                {
                    dtVesselUnloadTracked.Load(reader);
                }
            }
            catch(Exception ex)
            {
                Utilities.Logger.Log(ex);
            }

            dtVesselUnloadTracked.TableName = "vessel_unload";
            DataSet.Tables.Add(dtVesselUnloadTracked);
            return DataSet;
        }
        public static DataSet  GenerateDataSet()
        {
            DataSet = new System.Data.DataSet();

            DataTable dtSampling = new DataTable();
            DataTable dtGearUnload = new DataTable();
            DataTable dtVesselUnload = new DataTable();
            DataTable dtVesselCatch = new DataTable();
            DataTable dtGearSoak = new DataTable();
            DataTable dtFishingGrid = new DataTable();
            DataTable dtVesselEffort = new DataTable();
            DataTable dtLenFreq = new DataTable();
            DataTable dtLength = new DataTable();
            DataTable dtLenWt = new DataTable();
            DataTable dtMaturity = new DataTable();

            using (var reader = ObjectReader.Create
                (
                NSAPEntities.LandingSiteSamplingViewModel.GetAllFlattenedItems(),
                "ID",
                "SamplingDate",
                "NSAPRegion",
                "FMA",
                "FishingGround",
                "Remarks"
                ))
            {
                dtSampling.Load(reader);
            }

            using (var reader = ObjectReader.Create
                (
                NSAPEntities.GearUnloadViewModel.GetAllFlattenedItems(),
                 "ID",
                 "ParentID",
                "SamplingDate",
                "NSAPRegion",
                "FMA",
                "FishingGround",
                "Gear",
                "Boats",
                "Catch"
                ))
            {
                dtGearUnload.Load(reader);
            }


            using (var reader = ObjectReader.Create
            (
            NSAPEntities.VesselUnloadViewModel.GetAllFlattenedItems(),
                "ID",
                "ParentID",
                "SamplingDate",
                "Gear",     
                "IsBoatUsed",
                "FishingVessel",
                "SamplingEnumerator",
                "OperationIsSuccessful",
                "WeightOfCatch",
                "WeightOfCatchSample",
                "Boxes",
                "BoxesSampled",
                "Notes",
                "OperationIsTracked",
                "GPS",
                "DepartureFromLandingSite",
                "ArrivalAtLandingSite",
                "ODKRowID",
                "FormVersion",
                "UserName",
                "DeviceID",
                "Submitted",
                "XFormIdentifier",
                "XFormDate"
            ))
            {
                dtVesselUnload.Load(reader);
            }



           using (var reader = ObjectReader.Create
            (
            NSAPEntities.FishingGroundGridViewModel.GetAllFlattenedItems(),
             "ID",
             "ParentID",
            "UTMZone",
            "Grid",
            "LonLat",
            "UTMCoordinate"
            ))
            {
                dtFishingGrid.Load(reader);
            }

            using (var reader = ObjectReader.Create
                (
                NSAPEntities.GearSoakViewModel.GetAllFlattenedItems(),
                "ID",
                "ParentID",
                "TimeAtSet",
                "TimeAtHaul",
                "WaypointAtSet",
                "WaypointAtHaul"
                ))
            {
                dtGearSoak.Load(reader);
            }

            using (var reader = ObjectReader.Create
            (
            NSAPEntities.VesselEffortViewModel.GetAllFlattenedItems(),
            "ID",
            "ParentID",
            "EffortSpecification",
            "EffortValueNumeric",
            "EffortValueText"
            ))
            {
                dtVesselEffort.Load(reader);
            }

            using (var reader = ObjectReader.Create
            (
            NSAPEntities.VesselCatchViewModel.GetAllFlattenedItems(),
             "ID",
             "ParentID",
            "CatchName",
            "Taxa",
            "Catch_kg",
            "Sample_kg"
            ))
            {
                dtVesselCatch.Load(reader);
            }

            using (var reader = ObjectReader.Create
            (
            NSAPEntities.CatchLenFreqViewModel.GetAllFlattenedItems(),
            "ID",
            "ParentID",
            "CatchName",
            "Taxa",
            "Gear",
            "Length",
            "Frequency"
            ))
            {
                dtLenFreq.Load(reader);
            }

            using (var reader = ObjectReader.Create
            (
            NSAPEntities.CatchLengthViewModel.GetAllFlattenedItems(),
            "ID",
            "ParentID",
            "CatchName",
            "Taxa",
            "Gear",
            "Length"
            ))
            {
                dtLength.Load(reader);
            }

            using (var reader = ObjectReader.Create
            (
            NSAPEntities.CatchLengthWeightViewModel.GetAllFlattenedItems(),
            "ID",
            "ParentID",
            "CatchName",
            "Taxa",
            "Gear",
            "Length",
            "Weight"
            ))
            {
                dtLenWt.Load(reader);
            }

            using (var reader = ObjectReader.Create
            (
            NSAPEntities.CatchMaturityViewModel.GetAllFlattenedItems(),
            "ID",
            "ParentID",
            "CatchName",
            "Taxa",
            "Gear",
            "Length",
            "Weight",
            "Sex",
            "Maturity",
            "WeightGutContent"
            ))
            {
                dtMaturity.Load(reader);
            }

            dtSampling.TableName = "dbo_LC_FG_sample_day";
            dtGearUnload.TableName = "dbo_gear_unload";
            dtVesselUnload.TableName = "dbo_vessel_unload";
            dtVesselCatch.TableName = "dbo_vessel_catch";
            dtGearSoak.TableName = "dbo_gear_soak";
            dtFishingGrid.TableName = "dbo_fg_grid";
            dtVesselEffort.TableName = "dbo_vessel_effort";
            dtLenFreq.TableName = "dbo_catch_len_freq";
            dtLength.TableName = "dbo_catch_len";
            dtLenWt.TableName = "dbo_catch_len_wt";
            dtMaturity.TableName = "dbo_catch_maturity";


            DataSet.Tables.Add(dtSampling);
            DataSet.Tables.Add(dtGearUnload);
            DataSet.Tables.Add(dtVesselUnload);
            DataSet.Tables.Add(dtFishingGrid);
            DataSet.Tables.Add(dtGearSoak);
            DataSet.Tables.Add(dtVesselEffort);
            DataSet.Tables.Add(dtVesselCatch);
            DataSet.Tables.Add(dtLenFreq);
            DataSet.Tables.Add(dtLength);
            DataSet.Tables.Add(dtLenWt);
            DataSet.Tables.Add(dtMaturity);
            return DataSet;
        }
    }
}
