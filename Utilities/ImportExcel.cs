using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.SS.UserModel;
using Npoi.Mapper;
using NSAP_ODK.Entities;
using NSAP_ODK.Entities.Database;

namespace NSAP_ODK.Utilities
{
    
    public static class ImportExcel
    {
        public static event EventHandler<UploadToDbEventArg> UploadSubmissionToDB;
        private static IWorkbook _wkBook;
        private static string _excelFileName;
        public static string ExcelFileName
        {
            get { return _excelFileName; }
            set
            {
                _excelFileName = value;
                if (File.Exists(_excelFileName))
                {
                    ReadExcel();
                }
            }
        }
        public static Dictionary<int, string> ExcelSheets { get; private set; }
        public static List<ExcelMainSheet> ExcelMainSheets { get; private set; }

        public static List<ExcelBingoGroup> ExcelBingoGroups { get; private set; }

        public static List<ExcelSoakTime> ExcelSoakTimes { get; private set; }

        public static List<ExcelEffortRepeat> ExcelEffortRepeats { get; private set; }

        public static List<ExcelCatchComposition> ExcelCatchCompositions { get; private set; }

        public static List<ExcelLengthList> ExcelLengthLists { get; private set; }

        public static List<ExcelLengthWeight> ExcelLengthWeights { get; private set; }

        public static List<ExcelLenFreq> ExcelLenFreqs { get; private set; }

        public static List<ExcelGMS> ExcelGMSes { get; private set; }

        public static bool ClearNSAPDatabaseTables()
        {
            bool success = false;
            NSAPEntities.CatchMaturityViewModel.ClearRepository();
            NSAPEntities.CatchLengthViewModel.ClearRepository();
            NSAPEntities.CatchLengthWeightViewModel.ClearRepository();
            NSAPEntities.CatchLenFreqViewModel.ClearRepository();
            NSAPEntities.FishingGroundGridViewModel.ClearRepository();
            NSAPEntities.GearSoakViewModel.ClearRepository();
            NSAPEntities.VesselCatchViewModel.ClearRepository();
            NSAPEntities.VesselEffortViewModel.ClearRepository();
            NSAPEntities.VesselUnloadViewModel.ClearRepository();
            NSAPEntities.GearUnloadViewModel.ClearRepository();
            NSAPEntities.CatchLengthViewModel.ClearRepository();
            if (NSAPEntities.LandingSiteSamplingViewModel.ClearRepository())
            {
                success = true;
            }
            return success;
        }

        public static Task<bool> UploadToDatabaseAsync()
        {
            return Task.Run(() => UploadToDatabase());
        }
        public static bool UploadToDatabase()
        {
            int savedCount = 0;
            var sheetsToUpload = ExcelMainSheets.Where(t=>t.IsSaved==false).ToList();
            if (sheetsToUpload.Count > 0)
            {
                UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { VesselUnloadToSaveCount = sheetsToUpload.Count, Intent = UploadToDBIntent.StartOfUpload });

                foreach (var item in sheetsToUpload)
                {
                    var landingSiteSampling = NSAPEntities.LandingSiteSamplingViewModel.getLandingSiteSampling(item);
                    if (landingSiteSampling == null)
                    {
                        landingSiteSampling = new LandingSiteSampling

                        {
                            PK = NSAPEntities.LandingSiteSamplingViewModel.NextRecordNumber,
                            LandingSiteID = item.NSAPRegionFMAFishingGroundLandingSite == null ? null : (int?)item.NSAPRegionFMAFishingGroundLandingSite.LandingSite.LandingSiteID,
                            FishingGroundID = item.NSAPRegionFMAFishingGround.FishingGround.Code,
                            IsSamplingDay = true,
                            SamplingDate = item.SamplingDate,
                            NSAPRegionID = item.NSAPRegion.Code,
                            LandingSiteText = item.LandingSiteText,
                            FMAID = item.NSAPRegionFMA.FMA.FMAID
                        };
                        NSAPEntities.LandingSiteSamplingViewModel.AddRecordToRepo(landingSiteSampling);
                    }

                    GearUnload gu = NSAPEntities.GearUnloadViewModel.getGearUnload(item);
                    if (gu == null)
                    {
                        if (gu != null && gu.GearUsedText != null && gu.GearUsedText.Length > 0)
                        {

                        }
                        gu = new GearUnload
                        {
                            PK = NSAPEntities.GearUnloadViewModel.NextRecordNumber,
                            LandingSiteSamplingID = landingSiteSampling.PK,
                            GearID = item.NSAPRegionGear != null ? item.NSAPRegionGear.Gear.Code : null,
                            GearUsedText = item.GearUsedText
                        };
                        NSAPEntities.GearUnloadViewModel.AddRecordToRepo(gu);
                    }



                    VesselUnload vu = new VesselUnload
                    {
                        PK = NSAPEntities.VesselUnloadViewModel.NextRecordNumber,
                        GearUnloadID = gu.PK,
                        IsBoatUsed = item.IsBoatUsed,
                        VesselID = item.VesselUsedID == null ? null : (int?)item.FishingVessel.ID,
                        VesselText = item.FishingVesselText,
                        SectorCode = item.SectorCode,
                        WeightOfCatch = item.CatchWeightTotal,
                        WeightOfCatchSample = item.CatchWeightSampled,
                        Boxes = item.BoxesTotal,
                        BoxesSampled = item.BoxesSampled,
                        RaisingFactor = item.RaisingFactor,
                        OperationIsSuccessful = item.TripIsSuccess,
                        OperationIsTracked = item.TripIsTracked,
                        DepartureFromLandingSite = item.DateTimeDepartLandingSite,
                        ArrivalAtLandingSite = item.DateTimeArriveLandingSite,
                        ODKRowID = item.RowUUID,
                        XFormIdentifier = item.XFormIdentifier,
                        XFormDate = item.XFormDate,
                        UserName = item.UserName,
                        DeviceID = item.DeviceId,
                        DateTimeSubmitted = item.DateTimeSubmitted,
                        FormVersion = item.FormVersion,
                        GPSCode = item.GPSCode,
                        SamplingDate = item.SamplingDate,
                        Notes = item.Remarks,
                        //NSAPRegionEnumeratorID = item.NSAPERegionEnumeratorID == null ? null : (int?)item.NSAPERegionEnumeratorID,
                        NSAPEnumeratorID = item.EnumeratorID,
                        EnumeratorText = item.RegionEnumeratorText,
                        DateAddedToDatabase = DateTime.Now,
                        FromExcelDownload = true
                    };

                    if (NSAPEntities.VesselUnloadViewModel.AddRecordToRepo(vu))
                    {
                        savedCount++;
                        UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { VesselUnloadSavedCount = savedCount, Intent = UploadToDBIntent.Uploading });
                        if (ImportExcel.ExcelEffortRepeats != null && ImportExcel.ExcelEffortRepeats.Count > 0)
                        {
                            foreach (var effort in ImportExcel.ExcelEffortRepeats
                                .Where(t => t.ParentIndex == item.RowIndex))
                            {
                                VesselEffort ve = new VesselEffort
                                {
                                    PK = NSAPEntities.VesselEffortViewModel.NextRecordNumber,
                                    VesselUnloadID = vu.PK,
                                    EffortSpecID = effort.EffortTypeID,
                                    EffortValueNumeric = effort.EffortIntensityNumericValue,
                                    EffortValueText = effort.EffortIntenstityTextValue
                                };
                                NSAPEntities.VesselEffortViewModel.AddRecordToRepo(ve);
                            }
                        }

                        if (ImportExcel.ExcelSoakTimes != null && ImportExcel.ExcelSoakTimes.Count > 0)
                        {
                            foreach (var soak in ImportExcel.ExcelSoakTimes
                            .Where(t => t.ParentIndex == item.RowIndex))
                            {
                                GearSoak gs = new GearSoak
                                {
                                    PK = NSAPEntities.GearSoakViewModel.NextRecordNumber,
                                    VesselUnloadID = vu.PK,
                                    TimeAtSet = soak.DateTimeSet,
                                    TimeAtHaul = soak.DateTimeHaul,
                                    WaypointAtSet = soak.GPSWaypointAtSet,
                                    WaypointAtHaul = soak.GPSWaypointAtHaul
                                };
                                NSAPEntities.GearSoakViewModel.AddRecordToRepo(gs);
                            }
                        }

                        if (ImportExcel.ExcelBingoGroups != null && ImportExcel.ExcelBingoGroups.Count > 0)
                        {
                            foreach (var gr in ImportExcel.ExcelBingoGroups
                            .Where(t => t.ParentIndex == item.RowIndex))
                            {
                                FishingGroundGrid fgg = new FishingGroundGrid
                                {
                                    PK = NSAPEntities.FishingGroundGridViewModel.NextRecordNumber,
                                    VesselUnloadID = vu.PK,
                                    UTMZoneText = gr.Parent.UTMZone,
                                    Grid = gr.BingoCoordinate
                                };
                                NSAPEntities.FishingGroundGridViewModel.AddRecordToRepo(fgg);
                            }
                        }

                        if (ImportExcel.ExcelCatchCompositions != null && ImportExcel.ExcelCatchCompositions.Count > 0)
                        {
                            foreach (var catchComp in ImportExcel.ExcelCatchCompositions
                            .Where(t => t.ParentIndex == item.RowIndex))
                            {
                                VesselCatch vc = new VesselCatch
                                {
                                    PK = NSAPEntities.VesselCatchViewModel.NextRecordNumber,
                                    VesselUnloadID = vu.PK,
                                    SpeciesID = catchComp.SpeciesCode(catchComp.TaxonomicCode),
                                    Catch_kg = catchComp.SpeciesWeight,
                                    Sample_kg = catchComp.SpeciesSampleWeight,
                                    TaxaCode = catchComp.TaxonomicCode,
                                    SpeciesText = catchComp.CatchCompositionNameText
                                };

                                if (NSAPEntities.VesselCatchViewModel.AddRecordToRepo(vc))
                                {
                                    if (ImportExcel.ExcelLenFreqs != null && ImportExcel.ExcelLenFreqs.Count > 0)
                                    {
                                        foreach (var lf in ImportExcel.ExcelLenFreqs
                                            .Where(t => t.ParentIndex == catchComp.RowIndex))
                                        {
                                            CatchLenFreq clf = new CatchLenFreq
                                            {
                                                PK = NSAPEntities.CatchLenFreqViewModel.NextRecordNumber,
                                                VesselCatchID = vc.PK,
                                                LengthClass = lf.LengthClass,
                                                Frequency = lf.Frequency
                                            };
                                            NSAPEntities.CatchLenFreqViewModel.AddRecordToRepo(clf);
                                        }
                                    }

                                    if (ImportExcel.ExcelLengthWeights != null && ImportExcel.ExcelLengthWeights.Count > 0)
                                    {
                                        foreach (var lw in ImportExcel.ExcelLengthWeights
                                         .Where(t => t.ParentIndex == catchComp.RowIndex))
                                        {
                                            CatchLengthWeight clw = new CatchLengthWeight
                                            {
                                                PK = NSAPEntities.CatchLengthWeightViewModel.NextRecordNumber,
                                                VesselCatchID = vc.PK,
                                                Length = lw.Length,
                                                Weight = lw.Weight
                                            };
                                            NSAPEntities.CatchLengthWeightViewModel.AddRecordToRepo(clw);
                                        }
                                    }


                                    if (ImportExcel.ExcelLengthLists != null && ImportExcel.ExcelLengthLists.Count > 0)
                                    {
                                        foreach (var l in ImportExcel.ExcelLengthLists
                                         .Where(t => t.ParentIndex == catchComp.RowIndex))
                                        {
                                            CatchLength cl = new CatchLength
                                            {
                                                PK = NSAPEntities.CatchLengthViewModel.NextRecordNumber,
                                                VesselCatchID = vc.PK,
                                                Length = l.Length

                                            };
                                            NSAPEntities.CatchLengthViewModel.AddRecordToRepo(cl);
                                        }
                                    }

                                    if (ImportExcel.ExcelGMSes != null && ImportExcel.ExcelGMSes.Count > 0)
                                    {
                                        foreach (var m in ImportExcel.ExcelGMSes
                                         .Where(t => t.ParentIndex == catchComp.RowIndex))
                                        {
                                            CatchMaturity cm = new CatchMaturity
                                            {
                                                PK = NSAPEntities.CatchMaturityViewModel.NextRecordNumber,
                                                VesselCatchID = vc.PK,
                                                Length = m.Length,
                                                Weight = m.Weight,
                                                SexCode = m.SexCode,
                                                MaturityCode = m.MaturityCode,
                                                WeightGutContent = m.StomachContentWeight,
                                                GutContentCode = m.GutContentCode
                                            };
                                            NSAPEntities.CatchMaturityViewModel.AddRecordToRepo(cm);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { VesselUnloadTotalSavedCount = savedCount, Intent = UploadToDBIntent.EndOfUpload });
            return savedCount>0;
        }

        private static void ReadExcel()
        {

            try
            {
                FileStream fs = new FileStream(_excelFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var nameParts = Path.GetFileName(_excelFileName).Split('_');
                string time = "";
                string workBookName = nameParts[0];
                DateTime? fileTime = null;
                if (nameParts.Length > 4)
                {
                    time = $"{nameParts[2]}/{nameParts[3]}/{nameParts[1]} {nameParts[4]}:{ nameParts[5]}";
                }
                else if(nameParts.Length==1)
                {

                }
                else
                {
                    var daySplit = nameParts[3].Split('.');
                    time = $"{nameParts[2]}/{daySplit[0]}/{nameParts[1]}";
                }
                if (time.Length > 0)
                {
                    fileTime = DateTime.Parse(time);
                }
                // Try to read workbook as XLSX:
                try
                {
                    _wkBook = new XSSFWorkbook(fs);
                }
                catch
                {
                    _wkBook = null;
                }

                // If reading fails, try to read workbook as XLS:
                if (_wkBook == null)
                {
                    _wkBook = new HSSFWorkbook(fs);
                }

                if(_wkBook.NumberOfSheets>1)
                {
                    ExcelSheets = new Dictionary<int, string>(_wkBook.NumberOfSheets);
                    var importer = new Mapper(_wkBook);
                    ExcelMainSheets = new List<ExcelMainSheet>();
                    for (int n = 0; n < _wkBook.NumberOfSheets; n++)
                    {
                        ExcelSheets.Add(n, _wkBook.GetSheetAt(n).SheetName);
                        var sheetName = ExcelSheets[n];



                        if (sheetName == workBookName)
                        {
                            var items = importer.Take<ExcelMainSheet>(n);
                            foreach (var item in items.OrderByDescending(t=>t.Value.DateTimeSubmitted))
                            {
                                var row = item.Value;
                                row.XFormIdentifier = sheetName;
                                if (fileTime != null)
                                {
                                    row.XFormDate = (DateTime)fileTime;
                                }
                                ExcelMainSheets.Add(row);
                            }
                        }
                        else
                        {
                            switch(sheetName)
                            {
                                case "grid_coord_group_bingo_repeat":
                                    ExcelBingoGroups = new List<ExcelBingoGroup>();
                                    foreach (var item in importer.Take<ExcelBingoGroup>(n))
                                    {
                                        var row = item.Value;
                                        ExcelBingoGroups.Add(row);
                                    }
                                    break;
                                case "soak_time_group_soaktime_tracki":
                                    ExcelSoakTimes = new List<ExcelSoakTime>();
                                    foreach (var item in importer.Take<ExcelSoakTime>(n))
                                    {
                                        var row = item.Value;
                                        ExcelSoakTimes.Add(row);
                                    }
                                    break;
                                case "efforts_group_effort_repeat":
                                    ExcelEffortRepeats = new List<ExcelEffortRepeat>();
                                    foreach (var item in importer.Take<ExcelEffortRepeat>(n))
                                    {
                                        var row = item.Value;
                                        ExcelEffortRepeats.Add(row);
                                    }
                                    break;
                                case "catch_comp_group_catch_composit":
                                    ExcelCatchCompositions = new List<ExcelCatchComposition>();
                                    foreach (var item in importer.Take<ExcelCatchComposition>(n))
                                    {
                                        var row = item.Value;
                                        ExcelCatchCompositions.Add(row);
                                    }
                                    break;
                                case  "catch_comp_group_catch_composi1":
                                    ExcelLengthLists = new List<ExcelLengthList>();
                                    foreach (var item in importer.Take<ExcelLengthList>(n))
                                    {
                                        var row = item.Value;
                                        ExcelLengthLists.Add(row);
                                    }
                                    break;
                                case "catch_comp_group_catch_composi2":
                                    ExcelLengthWeights = new List<ExcelLengthWeight>();
                                    foreach (var item in importer.Take<ExcelLengthWeight>(n))
                                    {
                                        var row = item.Value;
                                        ExcelLengthWeights.Add(row);
                                    }
                                    break;
                                case "catch_comp_group_catch_composi3":
                                    ExcelLenFreqs = new List<ExcelLenFreq>();
                                    foreach (var item in importer.Take<ExcelLenFreq>(n))
                                    {
                                        var row = item.Value;
                                        ExcelLenFreqs.Add(row);
                                    }
                                    break;
                                case "catch_comp_group_catch_composi4":
                                    ExcelGMSes = new List<ExcelGMS>();
                                    foreach (var item in importer.Take<ExcelGMS>(n))
                                    {
                                        var row = item.Value;
                                        ExcelGMSes.Add(row);
                                    }
                                    break;
                            }
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return;
            }
        }

    }
}
