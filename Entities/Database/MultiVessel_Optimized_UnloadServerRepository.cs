using DocumentFormat.OpenXml.Presentation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public static class MultiVessel_Optimized_UnloadServerRepository
    {

        public static event EventHandler<UploadToDbEventArg> UploadSubmissionToDB;
        private static HashSet<string> _undetectedGears = new HashSet<string>();
        public static Task<bool> UploadToDBAsync(string jsonFileName = "")
        {
            return Task.Run(() => UploadToDatabase(jsonFileName: jsonFileName));
        }

        public static List<MultiVessel_Optimized_CBS_FishCarrier> FishCarriers
        {
            get
            {
                List<MultiVessel_Optimized_CBS_FishCarrier> carriers = new List<MultiVessel_Optimized_CBS_FishCarrier>();
                foreach (MultiVessel_Optimized_Root root in MultiVesselLandings)
                {

                }
                return carriers;
            }
        }
        public static List<MultiVessel_Optimized_SampledLanding> SampledVesselLandings
        {
            get
            {
                List<MultiVessel_Optimized_SampledLanding> sampledLandings = new List<MultiVessel_Optimized_SampledLanding>();
                foreach (MultiVessel_Optimized_Root root in MultiVesselLandings)
                {
                    if (root.SampledFishLandingsEx != null)
                    {
                        sampledLandings.AddRange(root.SampledFishLandingsEx);
                    }

                    //if (root.SampledFishLandings != null)
                    //{
                    //    sampledLandings.AddRange(root.SampledFishLandings);
                    //}
                }
                return sampledLandings;
            }
        }

        public static List<string> GetLandingIdentifiers()
        {
            List<string> ids = new List<string>();
            foreach (var item in MultiVesselLandings)
            {
                //ids.Add(item._uuid);
            }
            return ids;
        }
        public static int DownloadedLandingsCount()
        {
            return MultiVesselLandings.Count;
        }
        public static DateTime DownloadedLandingsLatestLandingDate()
        {
            return MultiVesselLandings.OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate;
        }
        public static DateTime DownloadedLandingsEarliestLandingDate()
        {
            return MultiVesselLandings.OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate;
        }
        public static void ResetGroupIDs(bool delayedSave = false)
        {
            NSAPEntities.SummaryItemViewModel.RefreshLastPrimaryLeys(delayedSave);
            MultiVessel_Optimized_Root.SetRowIDs();
            MultiVessel_Optimized_Gear.SetRowIDs();
            MultiVessel_Optimized_SampledLanding.SetRowIDs();
            MultiVessel_Optimized_LandingGear.SetRowIDs();
            MultiVessel_Optimized_GearEffortDetail.SetRowIDs();
            MultiVessel_Optimized_FishingGrid.SetRowIDs();
            MultiVessel_Optimized_SoakTime.SetRowIDs();
            MultiVessel_Optimized_CatchCompositionItem.SetRowIDs();
            MultiVessel_Optimized_CatchGMS.SetRowIDs();
            MultiVessel_Optimized_CatchLenFreq.SetRowIDs();
            MultiVessel_Optimized_CatchLenght.SetRowIDs();
            MultiVessel_Optimized_CatchLenWt.SetRowIDs();

            MultiVessel_Optimized_CBS_FishCarrier.SetRowIDs();
            MultiVessel_Optimized_CBS_CatcherBoat.SetRowIDs();
            MultiVessel_Optimized_CBS_CatchComposition.SetRowIDs();
            MultiVessel_Optimized_CBS_CarrierBoatFishingGround.SetRowIDs();
            MultiVessel_Optimized_CBS_CatcherBoat_FishingGroundGrid.SetRowIDs();

            MultiVessel_Optimized_CBS_GonadalMaturity.SetRowIDs();
            MultiVessel_Optimized_CBS_Length.SetRowIDs();
            MultiVessel_Optimized_CBS_LenWt.SetRowIDs();
            MultiVessel_Optimized_CBS_LenFreq.SetRowIDs();


        }

        public static int LandingSiteSamplingProcessedCount { get; set; }
        public static int LandingSiteSamplingUniqueCount { get; set; }
        public static void ResetTotalUploadCounter(bool uploadingDone = false)
        {
            _undetectedGears.Clear();
            LandingSiteSamplingProcessedCount = 0;
            LandingSiteSamplingUniqueCount = 0;
            TotalUploadCount = 0;
            if (uploadingDone)
            {
                UpdateInProgress = false;
                UploadInProgress = false;
                UnmatchedEnumeratorIDs.Clear();
            }
        }

        public static int TotalUploadCount { get; private set; }
        public static bool UpdateInProgress { get; set; }
        public static bool UploadInProgress { get; set; }
        public static HashSet<UnmatchedEnumeratorJSONFile> UnmatchedEnumeratorIDs { get; set; } = new HashSet<UnmatchedEnumeratorJSONFile>();
        public static bool UploadToDatabase(List<VesselLanding> resolvedLandings = null, string jsonFileName = "")
        {
            //Utilities.Logger.LogUploadJSONToLocalDB($"start uploading JSON to local db with {MultiVesselLandings.Count} landing days count");
            DelayedSave = true;
            bool proceed = false;
            int savedCount = 0;
            int lss_loop_count = 0;

            UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { Intent = UploadToDBIntent.Searching });
            UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { LandingSiteSamplingCount = MultiVesselLandings.Count, Intent = UploadToDBIntent.StartOfUpload });

            LandingSiteSamplingViewModel.CurrentIDNumber = NSAPEntities.SummaryItemViewModel.GetLandingSiteSamplingMaxRecordNumber(fromCollection: true);
            int maxID = LandingSiteSamplingRepository.MaxRecordNumber_from_db();
            if (maxID > LandingSiteSamplingViewModel.CurrentIDNumber)
            {
                LandingSiteSamplingViewModel.CurrentIDNumber = maxID;
            }

            foreach (MultiVessel_Optimized_Root root in MultiVesselLandings)
            {
                try
                {
                    bool lss_is_hidden = false;
                    lss_loop_count++;
                    //Console.WriteLine($"lss_loop_count is {lss_loop_count}");
                    LandingSiteSampling lss = NSAPEntities.LandingSiteSamplingSubmissionViewModel.GetLandingSiteSampling(root.SubmissionUUID);
                    if (lss == null)
                    {
                        LandingSite fls = null;
                        FishingGround fg = null;
                        if (root.LandingSite == null)
                        {
                            fls = NSAPEntities.LandingSiteViewModel.GetLandingSite(root.LandingSiteName);
                        }
                        if (root.FishingGround == null)
                        {
                            fg = NSAPEntities.FishingGroundViewModel.GetFishingGroundFromName(root.Fishing_ground_name);
                        }

                        if (root.LandingSiteID != null && root.FishingGround != null)
                        {
                            try
                            {
                                SamplingDaySubmission sds;
                                if (root.LandingSite == null)
                                {

                                    sds = NSAPEntities.SamplingDaySubmissionViewModel.GetSamplingDaySubmission(
                                        fls.LandingSiteID,
                                        root.FishingGround.Code,
                                        root.SamplingDate.Date);
                                }
                                else if (root.FishingGround == null)
                                {
                                    sds = NSAPEntities.SamplingDaySubmissionViewModel.GetSamplingDaySubmission(
                                        root.LandingSite.LandingSiteID,
                                        root.TypeOfSampling,
                                        root.SamplingDate.Date);
                                }
                                else
                                {
                                    sds = NSAPEntities.SamplingDaySubmissionViewModel.GetSamplingDaySubmission(
                                        root.LandingSite.LandingSiteID,
                                        root.FishingGround.Code,
                                        root.SamplingDate.Date);
                                }
                                if (sds != null)
                                {
                                    if (sds.LandingSiteSampling == null)
                                    {
                                        sds.LandingSiteSampling = NSAPEntities.LandingSiteSamplingViewModel.CreateInstance(sds.SamplingDayID);
                                    }
                                    lss = sds.LandingSiteSampling;


                                    var lss_fromm_summary = NSAPEntities.SummaryItemViewModel.GetLandingSiteSampling(sds.SamplingDayID);
                                    if (lss_fromm_summary == null)
                                    {
                                        lss_is_hidden = true;
                                        proceed = false;
                                    }
                                    //else
                                    //{
                                    //    lss = lss_fromm_summary;
                                    //}
                                }

                            }
                            catch (Exception ex)
                            {
                                Logger.Log(ex);
                            }
                        }


                        if (lss == null && !lss_is_hidden)
                        {

                            //if we are here then it means that the current landing site sampling is not yet saved
                            lss = new LandingSiteSampling
                            {
                                PK = LandingSiteSamplingViewModel.CurrentIDNumber + 1,
                                IsSamplingDay = root.IsSamplingDay,
                                SamplingDate = root.SamplingDate.Date,
                                NSAPRegionID = root.NSAPRegion.Code,
                                LandingSiteText = root.LandingSiteText,
                                FMAID = root.NSAPRegionFMA.FMA.FMAID,
                                DelayedSave = DelayedSave,
                                HasFishingOperation = root.AreThereLandings,
                                NumberOfLandings = root.CountTotalLanding,
                                NumberOfLandingsSampled = root.CountSampledLandings,
                                IsMultiVessel = true,
                                NumberOfGearTypesInLandingSite = root.FishingGearTypeCount,
                                RowID = root.SubmissionUUID,
                                DeviceID = root.device_id,
                                UserName = root.user_name,
                                XFormIdentifier = root._xform_id_string,
                                DateAdded = DateTime.Now,
                                FromExcelDownload = false,
                                FormVersion = root.FormVersion.ToString(),
                                JSONFileName = jsonFileName,
                                Remarks = root.ReasonNoLanding,
                                DateSubmitted = root.SubmissionTime,
                                SamplingFromCatchCompositionIsAllowed = root.SamplingFromCatchCompositionAllowed,
                                Submission_id = root._id,

                                LandingSiteTypeOfSampling = root.TypeOfSampling,
                            };

                            if (string.IsNullOrEmpty(lss.LandingSiteTypeOfSampling))
                            {
                                lss.LandingSiteTypeOfSampling = "rs";
                            }
                            try
                            {
                                if (root.RegionEnumeratorID != null && root.NSAPEnumerator != null)
                                {
                                    lss.EnumeratorID = root.NSAPEnumerator.ID;
                                }
                                else
                                {
                                    lss.EnumeratorText = root.RegionEnumeratorText;
                                }
                            }
                            catch
                            {
                                if (root.RegionEnumeratorText == null)

                                {
                                    lss.EnumeratorText = "";
                                }                                //ignore
                            }
                            if (root.LandingSite == null)
                            {
                                if (fls != null)
                                {
                                    lss.LandingSiteID = fls.LandingSiteID;
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(root.LandingSiteText))
                                    {
                                        lss.LandingSiteText = root.LandingSiteText;
                                    }
                                    else
                                    {
                                        lss.LandingSiteText = root.Landing_site_name;
                                    }
                                }
                            }
                            else
                            {
                                lss.LandingSiteID = root.LandingSite.LandingSiteID;
                            }

                            if (root.TypeOfSampling == "rs")
                            {
                                if (root.RegionFishingGround != null)
                                {
                                    lss.FishingGroundID = root.RegionFishingGround.FishingGroundCode;
                                }
                                else if (fg != null)
                                {
                                    lss.FishingGroundID = fg.Code;
                                }
                                else
                                {
                                    //what to do if there is a fishing ground that is not recognized
                                }
                            }
                            else
                            {
                                lss.CountCarrierLandings = root.Count_Carrier_Landing;
                                lss.CountCarrierSamplings = root.Count_Carrier_Sampling;
                            }

                            if (NSAPEntities.LandingSiteSamplingViewModel.AddRecordToRepo(lss))
                            {
                                LandingSiteSamplingUniqueCount++;
                                LandingSiteSamplingViewModel.CurrentIDNumber = lss.PK;
                                //proceed = true;
                                proceed = NSAPEntities.SamplingDaySubmissionViewModel.Add(lss);
                            }
                        }
                        else
                        {
                            if (!lss_is_hidden)
                            {
                                Utilities.Logger.LogUploadJSONToLocalDB($"Duplicated LSS: {lss.ToString()}\tPosition: {lss_loop_count}\tJSON:{jsonFileName}\tSubmission UUID:{root.SubmissionUUID}");
                                proceed = true;
                            }
                        }

                        LandingSiteSamplingSubmission lsss = null;
                        if (proceed && !lss_is_hidden)
                        {
                            if (!NSAPEntities.LandingSiteSamplingSubmissionViewModel.SubmissionIDExists(root.SubmissionUUID))
                            {
                                lsss = new LandingSiteSamplingSubmission
                                {
                                    SubmissionID = root.SubmissionUUID,
                                    DateAdded = DateTime.Now,
                                    JSONFile = jsonFileName,
                                    LandingSiteSampling = lss,
                                    XFormIdentifier = root._xform_id_string,
                                    DelayedSave = true
                                };
                                proceed = NSAPEntities.LandingSiteSamplingSubmissionViewModel.Add(lsss);

                            }
                        }

                        if (proceed)
                        {
                            if (root.TypeOfSampling == "rs")
                            {
                                proceed = false;
                                if (!lss.HasFishingOperation)
                                {
                                    lss.HasFishingOperation = root.AreThereLandings;
                                    if (lss.HasFishingOperation && lss.GearUnloadViewModel == null)
                                    {
                                        lss.GearUnloadViewModel = new GearUnloadViewModel(lss);
                                    }
                                    proceed = lss.HasFishingOperation;
                                }
                                else if (lss.GearUnloadViewModel == null)
                                {
                                    lss.GearUnloadViewModel = new GearUnloadViewModel(lss);
                                    proceed = true;
                                }
                                else
                                {
                                    proceed = true;
                                }
                                if (proceed && root.AreThereLandings)
                                {
                                    GearUnload gu;
                                    foreach (MultiVessel_Optimized_Gear gear in root.GearsInLandingSite)
                                    {
                                        gu = lss.GearUnloadViewModel.GetGearUnloadEx(gear.GearName);
                                        if (gu == null)
                                        {
                                            gu = new GearUnload
                                            {
                                                PK = GearUnloadViewModel.CurrentIDNumber + 1,
                                                Parent = lss,
                                                LandingSiteSamplingID = lss.PK,
                                                Boats = gear.NumberOfLandingsOfGear,
                                                Catch = gear.WeightOfCatchOfGear,
                                                NumberOfCommercialLandings = gear.NumberOfLandingsCommercial,
                                                NumberOfMunicipalLandings = gear.NumberOfLandingsMunicipal,
                                                WeightOfMunicipalLandings = gear.WeightOfCatchMunicipal,
                                                WeightOfCommercialLandings = gear.WeightOfCatchCommercial,
                                                Remarks = gear.GearRemarks,
                                                Sequence = gear.GearLoopCounter,
                                                DelayedSave = DelayedSave,
                                            };
                                            var gear2 = NSAPEntities.GearViewModel.GetGear(gear.GearCode);
                                            if (gear2 != null)
                                            {
                                                gu.GearID = gear2.Code;
                                            }
                                            else
                                            {
                                                gu.GearID = null;
                                                gu.GearUsedText = gear.GearName;
                                            }
                                            proceed = false;
                                            if (lss.GearUnloadViewModel.AddRecordToRepo(gu))
                                            {
                                                GearUnloadViewModel.CurrentIDNumber = gu.PK;
                                                proceed = true;
                                            }
                                        }
                                        else
                                        {
                                            gu.Boats += gear.NumberOfLandingsOfGear;
                                            gu.Catch += gear.WeightOfCatchOfGear;
                                            proceed = true;
                                        }
                                        if (proceed)
                                        {
                                            if (root.IsSamplingDay)
                                            {
                                                if (gu.VesselUnloadViewModel == null)
                                                {
                                                    gu.VesselUnloadViewModel = new VesselUnloadViewModel(isNew: true);
                                                }

                                                if (root.SampledFishLandings != null && root.SampledFishLandings.Count > 0)
                                                {

                                                    foreach (MultiVessel_Optimized_SampledLanding sl in root.SampledFishLandings.Where(t => t.Main_gear_used == gu.Sequence).ToList())
                                                    {
                                                        //SummaryItem si_vu = NSAPEntities.SummaryItemViewModel.GetVesselUnload(lss.PK, sl.Reference_number);
                                                        //if (si_vu == null)
                                                        //{
                                                        VesselUnload vu = new VesselUnload
                                                        {
                                                            PK = VesselUnloadViewModel.CurrentIDNumber + 1,
                                                            Parent = gu,
                                                            GearUnloadID = gu.PK,
                                                            SectorCode = sl.SectorOfLanding,
                                                            SamplingDate = root.SamplingDate.Add(sl.TimeOfSampling.TimeOfDay),
                                                            DelayedSave = DelayedSave,
                                                            RefNo = sl.Reference_number,
                                                            OperationIsSuccessful = sl.TripIsSuccess,
                                                            OperationIsTracked = false,
                                                            FishingTripIsCompleted = sl.TripIsSuccess ? true : sl.TripIsCompleted,
                                                            Notes = sl.Remarks,
                                                            HasCatchComposition = sl.IncludeCatchComp,
                                                            WeightOfCatch = sl.CatchTotal,
                                                            WeightOfCatchSample = sl.CatchSampled,
                                                            Boxes = sl.Boxes_total,
                                                            BoxesSampled = sl.Boxes_sampled,
                                                            IsBoatUsed = sl.IsBoatUsedInLanding,
                                                            VesselID = sl.BoatUsedID,
                                                            VesselText = sl.BoatUsedText,
                                                            NumberOfFishers = sl.NumberOfFishers,
                                                            IsMultiGear = true,
                                                            IsCatchSold = sl.IsCatchSold,
                                                            CountGearTypesUsed = sl.NumberOfGearsUsedInSampledLanding,
                                                            RaisingFactor = sl.RaisingFactor,
                                                            SequenceOfSampling = sl.LandingSequence,
                                                            NSAPEnumeratorID = gu.Parent.EnumeratorID,
                                                            EnumeratorText = gu.Parent.EnumeratorText,
                                                            DateAddedToDatabase = DateTime.Now,
                                                            FormVersion = sl.Parent.FormVersion == -1 ? "" : sl.Parent.FormVersion.ToString(),
                                                            XFormIdentifier = sl.Parent._xform_id_string,
                                                            //NumberOfSpeciesInCatchComposition = sl.NumberSpeciesInCatchComposition,
                                                            NumberOfSpeciesInCatchComposition = null,
                                                            IncludeEffortIndicators = sl.IncludeEffort,
                                                            LandingSiteSamplingSubmissionID = lsss.SubmissionID,
                                                            HasInteractionWithETPs = sl.GearHasInteractionWithETP,
                                                            //ETPsIntercatedWith = sl.ListETPsEncountered,
                                                            //TypesOfIntercationWithETPs = sl.ListInteractionTypesWithETPs,

                                                        };

                                                        if(vu.HasInteractionWithETPs)
                                                        {
                                                            vu.ETPsIntercatedWith = sl.ListETPsEncountered;
                                                            vu.TypesOfIntercationWithETPs = sl.ListInteractionTypesWithETPs;
                                                            vu.OtherInteractionTypeWithETPs = sl.OtherTypeOfInteractionWithETPs;
                                                        }

                                                        if (gu.VesselUnloadViewModel.AddRecordToRepo(vu))
                                                        {
                                                            VesselUnloadViewModel.CurrentIDNumber = vu.PK;



                                                            if (sl.FishingGrids?.Count > 0)
                                                            {
                                                                if (vu.FishingGroundGridViewModel == null)
                                                                {
                                                                    vu.FishingGroundGridViewModel = new FishingGroundGridViewModel(isNew: true);
                                                                }
                                                                foreach (MultiVessel_Optimized_FishingGrid mvg_fg in sl.FishingGrids)
                                                                {
                                                                    if (!string.IsNullOrEmpty(sl.UTMZone) && !string.IsNullOrEmpty(mvg_fg.GridNameComplete))
                                                                    {
                                                                        FishingGroundGrid fgg = new FishingGroundGrid
                                                                        {
                                                                            PK = FishingGroundGridViewModel.CurrentIDNumber + 1,
                                                                            Parent = vu,
                                                                            VesselUnloadID = vu.PK,
                                                                            UTMZoneText = sl.UTMZone,
                                                                            Grid = mvg_fg.GridNameComplete,
                                                                            DelayedSave = DelayedSave
                                                                        };
                                                                        if (vu.FishingGroundGridViewModel.AddRecordToRepo(fgg))
                                                                        {
                                                                            FishingGroundGridViewModel.CurrentIDNumber = fgg.PK;
                                                                            vu.CountGrids++;
                                                                        }
                                                                    }
                                                                }
                                                                vu.FishingGroundGridViewModel.Dispose();
                                                            }

                                                            if (sl.GearSoakTimes?.Count > 0)
                                                            {
                                                                if (vu.GearSoakViewModel == null)
                                                                {
                                                                    vu.GearSoakViewModel = new GearSoakViewModel(isNew: true);
                                                                }
                                                                foreach (MultiVessel_Optimized_SoakTime mvg_st in sl.GearSoakTimes)
                                                                {
                                                                    GearSoak gs = new GearSoak
                                                                    {
                                                                        PK = GearSoakViewModel.CurrentIDNumber + 1,
                                                                        Parent = vu,
                                                                        VesselUnloadID = vu.PK,
                                                                        DelayedSave = DelayedSave,
                                                                        TimeAtSet = (DateTime)mvg_st.TimeOfSet,
                                                                        TimeAtHaul = (DateTime)mvg_st.TimeOfHaul,
                                                                    };
                                                                    if (vu.GearSoakViewModel.AddRecordToRepo(gs))
                                                                    {
                                                                        GearSoakViewModel.CurrentIDNumber = gs.PK;
                                                                        vu.CountGearSoak++;
                                                                    }
                                                                }

                                                                vu.GearSoakViewModel.Dispose();
                                                            }

                                                            if (vu.VesselUnload_FishingGearsViewModel == null)
                                                            {
                                                                vu.VesselUnload_FishingGearsViewModel = new VesselUnload_FishingGearViewModel(isNew: true);
                                                            }
                                                            List<string> gearList = sl.GearsUsedInSampledLanding.Split(' ').ToList();
                                                            int countSavedInLoop = 0;
                                                            foreach (MultiVessel_Optimized_LandingGear mvg_lg in sl.SampledLandingGears)
                                                            {
                                                                VesselUnload_FishingGear vufg = new VesselUnload_FishingGear
                                                                {
                                                                    GearCode = mvg_lg.GearCode == "_OT" ? "" : mvg_lg.GearCode,
                                                                    GearText = mvg_lg.GearText,
                                                                    Parent = vu,
                                                                    RowID = VesselUnload_FishingGearViewModel.CurrentIDNumber + 1,
                                                                    DelayedSave = true,
                                                                };

                                                                if (vu.VesselUnload_FishingGearsViewModel.AddRecordToRepo(vufg))
                                                                {
                                                                    VesselUnload_FishingGearViewModel.CurrentIDNumber = vufg.RowID;
                                                                    if (vufg.VesselUnload_Gear_Specs_ViewModel == null)
                                                                    {
                                                                        vufg.VesselUnload_Gear_Specs_ViewModel = new VesselUnload_Gear_Spec_ViewModel();
                                                                    }
                                                                    if (vufg.GearUsedName != null)
                                                                    {
                                                                        if (sl.IncludeEffort)
                                                                        {
                                                                            MultiVessel_Optimized_GearEffort mvg_ge = sl.GearEfforts.FirstOrDefault(t => t.SelectedGearName == vufg.GearUsedName);
                                                                            if (mvg_ge != null && mvg_ge.GearEffortDetails != null)
                                                                            {
                                                                                foreach (MultiVessel_Optimized_GearEffortDetail mvg_ged in mvg_ge.GearEffortDetails)
                                                                                {
                                                                                    VesselUnload_Gear_Spec vugs = new VesselUnload_Gear_Spec
                                                                                    {
                                                                                        RowID = VesselUnload_Gear_Spec_ViewModel.CurrentIDNumber + 1,
                                                                                        Parent = vufg,
                                                                                        EffortSpecID = mvg_ged.EffortType,
                                                                                        EffortValueNumeric = mvg_ged.EffortIntensity,
                                                                                        EffortValueText = mvg_ged.EffortText,
                                                                                        DelayedSave = DelayedSave,
                                                                                        VesselUnload_FishingGears_ID = vufg.RowID
                                                                                    };

                                                                                    if (vufg.VesselUnload_Gear_Specs_ViewModel.AddRecordToRepo(vugs))
                                                                                    {
                                                                                        VesselUnload_Gear_Spec_ViewModel.CurrentIDNumber = vugs.RowID;
                                                                                        vufg.CountEffortIndicators++;

                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (!_undetectedGears.Contains(vufg.GearCode))
                                                                        {
                                                                            _undetectedGears.Add(vufg.GearCode);
                                                                            Utilities.Logger.LogUploadJSONToLocalDB($"Gear not found in local DB: Gear code is {vufg.GearCode} Name:{vufg.GearUsedName}");
                                                                        }
                                                                    }

                                                                    countSavedInLoop++;
                                                                }
                                                                //vufg.VesselUnload_Gear_Specs_ViewModel.Dispose();
                                                            }



                                                            proceed = countSavedInLoop == gearList.Count;

                                                            if (sl.IncludeCatchComp)
                                                            {
                                                                if (sl.GearCatchCompositionItems != null && sl.GearCatchCompositionItems.Count > 0)
                                                                {
                                                                    foreach (var item in sl.GearCatchCompositionItems)
                                                                    {
                                                                        var vufg = vu.ListUnloadFishingGears.FirstOrDefault(t => t.GearUsedName == item.GearName);
                                                                        if (vufg != null)
                                                                        {
                                                                            if (vufg.VesselCatchViewModel == null)
                                                                            {
                                                                                vufg.VesselCatchViewModel = new VesselCatchViewModel(isNew: true);
                                                                            }

                                                                            vufg.CountItemsInCatchComposition = item.CatchCompositionItems.Count;
                                                                            vufg.WeightOfCatch = item.WeightOfCatch;
                                                                            vufg.WeightOfSample = item.WeightOfSample;
                                                                            vu.VesselUnload_FishingGearsViewModel.UpdateRecordInRepo(vufg, update_wt_ct: true);

                                                                            foreach (MultiVessel_Optimized_CatchCompositionItem mvg_cci in item.CatchCompositionItems)
                                                                            {
                                                                                VesselCatch vc = new VesselCatch
                                                                                {
                                                                                    PK = VesselCatchViewModel.CurrentIDNumber + 1,
                                                                                    ParentFishingGear = vufg,
                                                                                    //GearCode = mvg_cci.CodeOfGearUsedForCatching == "_OT" ? "" : mvg_cci.CodeOfGearUsedForCatching,
                                                                                    //GearText = mvg_cci.GearUsedForCatching == null ? mvg_cci.NameOfGearUsedForCatching : "",
                                                                                    GearCode = "",
                                                                                    GearText = "",
                                                                                    DelayedSave = DelayedSave,

                                                                                    SpeciesID = mvg_cci.SpeciesID,
                                                                                    Catch_kg = mvg_cci.Species_wt,
                                                                                    Sample_kg = mvg_cci.Species_sample_wt,
                                                                                    TaxaCode = mvg_cci.SelectedTaxa,
                                                                                    SpeciesText = mvg_cci.SpeciesNameOther,
                                                                                    WeighingUnit = mvg_cci.Individual_wt_unit,
                                                                                    FromTotalCatch = mvg_cci.FromTotalCatch,
                                                                                    IsCatchSold = mvg_cci.IsSpeciesSold,
                                                                                    PriceOfSpecies = mvg_cci.Price_of_species,
                                                                                    PriceUnit = mvg_cci.Pricing_unit,
                                                                                    OtherPriceUnit = mvg_cci.OtherPricingUnit
                                                                                };



                                                                                if (vufg.VesselCatchViewModel.AddRecordToRepo(vc))
                                                                                {
                                                                                    VesselCatchViewModel.CurrentIDNumber = vc.PK;
                                                                                    if (mvg_cci.HasMeasurements)
                                                                                    {
                                                                                        if (mvg_cci.CatchGMSes != null && mvg_cci.CatchGMSes.Count > 0)
                                                                                        {
                                                                                            vufg.CountMaturityRows += mvg_cci.CatchGMSes.Count;
                                                                                            if (vc.CatchMaturityViewModel == null)
                                                                                            {
                                                                                                vc.CatchMaturityViewModel = new CatchMaturityViewModel(isNew: true);
                                                                                            }
                                                                                            foreach (MultiVessel_Optimized_CatchGMS mvg_cgms in mvg_cci.CatchGMSes)
                                                                                            {
                                                                                                CatchMaturity cm = new CatchMaturity
                                                                                                {
                                                                                                    Parent = vc,
                                                                                                    PK = CatchMaturityViewModel.CurrentIDNumber + 1,
                                                                                                    SexCode = mvg_cgms.Sex,
                                                                                                    Length = mvg_cgms.Individual_length,
                                                                                                    Weight = mvg_cgms.Individual_weight_kg,
                                                                                                    MaturityCode = mvg_cgms.GMS,
                                                                                                    GutContentCode = mvg_cgms.GutContentCategory,
                                                                                                    GonadWeight = mvg_cgms.GonadWt,
                                                                                                    WeightGutContent = mvg_cgms.Stomach_content_weight,
                                                                                                    DelayedSave = DelayedSave
                                                                                                };
                                                                                                if (vc.CatchMaturityViewModel.AddRecordToRepo(cm))
                                                                                                {
                                                                                                    CatchMaturityViewModel.CurrentIDNumber = cm.PK;
                                                                                                }
                                                                                            }
                                                                                            vc.CatchMaturityViewModel.Dispose();
                                                                                        }
                                                                                        if (mvg_cci.CatchLenFreqs != null && mvg_cci.CatchLenFreqs.Count > 0)
                                                                                        {
                                                                                            vufg.CountLenFreqRows += mvg_cci.CatchLenFreqs.Count;
                                                                                            if (vc.CatchLenFreqViewModel == null)
                                                                                            {
                                                                                                vc.CatchLenFreqViewModel = new CatchLenFreqViewModel(isNew: true);
                                                                                            }
                                                                                            foreach (MultiVessel_Optimized_CatchLenFreq mvg_clf in mvg_cci.CatchLenFreqs)
                                                                                            {
                                                                                                CatchLenFreq lf = new CatchLenFreq
                                                                                                {
                                                                                                    Parent = vc,
                                                                                                    PK = CatchLenFreqViewModel.CurrentIDNumber + 1,
                                                                                                    LengthClass = mvg_clf.LengthClass,
                                                                                                    Frequency = mvg_clf.Frequency,
                                                                                                    DelayedSave = DelayedSave,
                                                                                                    Sex = mvg_clf.Sex
                                                                                                };
                                                                                                if (vc.CatchLenFreqViewModel.AddRecordToRepo(lf))
                                                                                                {
                                                                                                    CatchLenFreqViewModel.CurrentIDNumber = lf.PK;
                                                                                                }

                                                                                            }
                                                                                            vc.CatchLenFreqViewModel.Dispose();
                                                                                        }
                                                                                        if (mvg_cci.CatchLengths != null && mvg_cci.CatchLengths.Count > 0)
                                                                                        {
                                                                                            vufg.CountLengthRows += mvg_cci.CatchLengths.Count;
                                                                                            if (vc.CatchLengthViewModel == null)
                                                                                            {
                                                                                                vc.CatchLengthViewModel = new CatchLengthViewModel(isNew: true);
                                                                                            }
                                                                                            foreach (MultiVessel_Optimized_CatchLenght mv_cl in mvg_cci.CatchLengths)
                                                                                            {
                                                                                                CatchLength len = new CatchLength
                                                                                                {
                                                                                                    Parent = vc,
                                                                                                    PK = CatchLengthViewModel.CurrentIDNumber + 1,
                                                                                                    Length = mv_cl.Length,
                                                                                                    DelayedSave = DelayedSave,
                                                                                                    Sex = mv_cl.Sex
                                                                                                };

                                                                                                if (vc.CatchLengthViewModel.AddRecordToRepo(len))
                                                                                                {
                                                                                                    CatchLengthViewModel.CurrentIDNumber = len.PK;
                                                                                                }
                                                                                            }
                                                                                            vc.CatchLengthViewModel.Dispose();
                                                                                        }

                                                                                        if (mvg_cci.CatchLengthWeights != null && mvg_cci.CatchLengthWeights.Count > 0)
                                                                                        {
                                                                                            vufg.CountLenWtRows += mvg_cci.CatchLengthWeights.Count;
                                                                                            if (vc.CatchLengthWeightViewModel == null)
                                                                                            {
                                                                                                vc.CatchLengthWeightViewModel = new CatchLengthWeightViewModel(isNew: true);
                                                                                            }
                                                                                            foreach (MultiVessel_Optimized_CatchLenWt mvg_clw in mvg_cci.CatchLengthWeights)
                                                                                            {
                                                                                                CatchLengthWeight lw = new CatchLengthWeight
                                                                                                {
                                                                                                    Parent = vc,
                                                                                                    PK = CatchLengthWeightViewModel.CurrentIDNumber + 1,
                                                                                                    Length = mvg_clw.Length,
                                                                                                    Weight = mvg_clw.Weight,
                                                                                                    DelayedSave = DelayedSave,
                                                                                                    Sex = mvg_clw.Sex
                                                                                                };

                                                                                                if (vc.CatchLengthWeightViewModel.AddRecordToRepo(lw))
                                                                                                {
                                                                                                    CatchLengthWeightViewModel.CurrentIDNumber = lw.PK;
                                                                                                }
                                                                                            }
                                                                                            vc.CatchLengthWeightViewModel.Dispose();
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                            //vufg.VesselCatchViewModel.Dispose();
                                                                        }
                                                                        else
                                                                        {
                                                                            if (!_undetectedGears.Contains(vufg.GearCode))
                                                                            {
                                                                                _undetectedGears.Add(vufg.GearCode);
                                                                                Utilities.Logger.LogUploadJSONToLocalDB($"Gear not found in local DB: Gear code is {vufg.GearCode} Name:{vufg.GearUsedName}");
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                foreach (VesselUnload_FishingGear vufg in vu.VesselUnload_FishingGearsViewModel.VesselUnload_FishingGearsCollection.ToList())
                                                                {
                                                                    if (sl.LandingGearsWithWeight == null)
                                                                    {

                                                                        vufg.WeightOfCatch = sl.CatchTotal == null ? 0 : sl.CatchTotal;
                                                                        vufg.WeightOfSample = sl.CatchSampled == null ? 0 : sl.CatchSampled;
                                                                    }
                                                                    else
                                                                    {
                                                                        vufg.WeightOfCatch = sl.LandingGearsWithWeight.FirstOrDefault(t => t.GearName == vufg.GearUsedName).WeightOfCatch;
                                                                        vufg.WeightOfSample = sl.LandingGearsWithWeight.FirstOrDefault(t => t.GearName == vufg.GearUsedName).WeightOfSample;
                                                                    }
                                                                    vufg.CountItemsInCatchComposition = 0;
                                                                    vu.VesselUnload_FishingGearsViewModel.UpdateRecordInRepo(vufg, update_wt_ct: true);
                                                                }
                                                            }

                                                            if (gu.VesselUnloadViewModel.UpdateUnloadStats(vu) && NSAPEntities.SummaryItemViewModel.AddRecordToRepo(vu))
                                                            {
                                                                if (sl.IncludeCatchComp)
                                                                {
                                                                    gu.VesselUnloadViewModel.UpdateWeightValidation(vu);
                                                                }
                                                                vu.VesselUnload_FishingGearsViewModel.Dispose();
                                                                savedCount++;
                                                                sl.SavedInLocalDatabase = true;
                                                                UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { VesselUnloadSavedCount = savedCount, Intent = UploadToDBIntent.VesselUnloadSaved });
                                                                TotalUploadCount++;

                                                            }
                                                            else
                                                            {

                                                            }

                                                        }
                                                    }
                                                }
                                                else
                                                {

                                                }
                                            }
                                            else
                                            {
                                                if (NSAPEntities.SummaryItemViewModel.AddRecordToRepo(gu))
                                                {
                                                    savedCount++;
                                                    TotalUploadCount++;
                                                }
                                            }
                                        }


                                    }


                                }
                                else if (!lss.HasFishingOperation)
                                {
                                    if (NSAPEntities.SummaryItemViewModel.AddRecordToRepo(lss))
                                    {
                                        savedCount++;
                                        TotalUploadCount++;
                                    }
                                }
                                else if (lss.NumberOfGearTypesInLandingSite != lss.GearUnloadViewModel.Count)
                                {

                                }
                            }
                            else if (root.TypeOfSampling == "cbl")
                            {
                                proceed = false;
                                if (root.FishCarriers?.Count > 0)
                                {
                                    lss.CarrierLandingViewModel = new CarrierLandingViewModel(lss);
                                    foreach (var fish_carrier in root.FishCarriers)
                                    {
                                        CarrierLanding cl = new CarrierLanding
                                        {
                                            Parent = lss,
                                            CarrierName = fish_carrier.CarrierName,
                                            SamplingDate = lss.SamplingDate.Add(fish_carrier.SamplingTime.TimeOfDay),
                                            WeightOfCatch = fish_carrier.WeightOfCarrierCatch,
                                            CountCatchers = fish_carrier.CountCatcherBoats,
                                            CountSpeciesComposition = fish_carrier.CountCarrierSpeciesComposition,
                                            RowID = CarrierLandingViewModel.CurrentIDNumber + 1,
                                            RefNo = fish_carrier.RefNo,
                                            WeightOfSample = fish_carrier.WeightOfSampleFromCarrier

                                        };

                                        if (lss.CarrierLandingViewModel.AddRecordToRepo(cl))
                                        {
                                            CarrierLandingViewModel.CurrentIDNumber = cl.RowID;

                                            if (fish_carrier?.CountCarrierFishingGrounds > 0)
                                            {
                                                cl.CarrierBoatLanding_FishingGround_ViewModel = new CarrierBoatLanding_FishingGround_ViewModel(cl);
                                                foreach (var cfg in fish_carrier.Carrier_FishingGrounds)
                                                {
                                                    CarrierBoatLanding_FishingGround cbl_fg = new CarrierBoatLanding_FishingGround
                                                    {
                                                        Parent = cl,
                                                        FishingGroundCode = cfg.FishingGroundCode,
                                                        RowID = CarrierBoatLanding_FishingGround_ViewModel.CurrentIDNumber + 1
                                                    };

                                                    if (cl.CarrierBoatLanding_FishingGround_ViewModel.AddToRepo(cbl_fg))
                                                    {
                                                        CarrierBoatLanding_FishingGround_ViewModel.CurrentIDNumber = cbl_fg.RowID;
                                                    }
                                                }
                                                cl.CarrierBoatLanding_FishingGround_ViewModel.Dispose();
                                            }

                                            if (fish_carrier?.CountCatcherBoats > 0)
                                            {
                                                cl.CatcherBoatOperation_ViewModel = new CatcherBoatOperation_ViewModel(cl);
                                                foreach (var cb in fish_carrier.Carrier_Catcherboats)
                                                {
                                                    CatcherBoatOperation cbo = new CatcherBoatOperation
                                                    {
                                                        Parent = cl,
                                                        RowID = CatcherBoatOperation_ViewModel.CurrentIDNumber + 1,
                                                        CatcherBoatName = cb.NameCatcherBoat,
                                                        StartOfOperation = cb.DateOfOperation,
                                                        GearCode = cb.GearCode,
                                                        WeightOfCatch = cb.WeightOfCatch
                                                    };


                                                    if (cl.CatcherBoatOperation_ViewModel.AddToRepo(cbo))
                                                    {
                                                        CatcherBoatOperation_ViewModel.CurrentIDNumber = cbo.RowID;

                                                        if (cb.IsCatcherFishingGroundKnown)
                                                        {
                                                            cbo.FishingGroundGridViewModel = new FishingGroundGridViewModel(isNew: true);
                                                            foreach (var db_fg in cb.Catcher_fishing_ground_grids)
                                                            {
                                                                FishingGroundGrid fgg_cb = new FishingGroundGrid
                                                                {
                                                                    Grid = $"{db_fg.CatcherBoatFishingGroundMajorGrid}-{db_fg.CatcherBoatFishingGroundColumn}{db_fg.CatcherBoatFishingGroundRow}",
                                                                    UTMZoneText = cb.CatcherFishngGroundUTMZone,
                                                                    ParentCBO = cbo,
                                                                    PK = (int)db_fg.PK,
                                                                    DelayedSave = true
                                                                };


                                                                if (cbo.FishingGroundGridViewModel.AddRecordToRepo(fgg_cb))
                                                                {
                                                                    FishingGroundGridViewModel.CurrentIDNumber = fgg_cb.PK;
                                                                }
                                                            }
                                                        }
                                                    }


                                                }
                                                cl.CatcherBoatOperation_ViewModel.Dispose();
                                            }

                                            if (fish_carrier?.CountCarrierSpeciesComposition > 0 && fish_carrier.CarrierCatchCompositions != null)
                                            {
                                                cl.VesselCatchViewModel = new VesselCatchViewModel(cl);
                                                foreach (var cvc in fish_carrier.CarrierCatchCompositions)
                                                {
                                                    VesselCatch vc = new VesselCatch
                                                    {
                                                        ParentCarrierLanding = cl,
                                                        PK = VesselCatchViewModel.CurrentIDNumber + 1,
                                                        SpeciesID = cvc.SpeciesID,
                                                        TaxaCode = cvc.Taxa.Code,
                                                        SpeciesText = cvc.SpeciesNameOther,
                                                        Catch_kg = cvc.SpeciesWeight,
                                                        DelayedSave = true,
                                                        WeighingUnit = cvc.WeightUnit
                                                    };

                                                    if (cl.VesselCatchViewModel.AddRecordToRepo(vc))
                                                    {


                                                        if (cvc.LenFreqs?.Count > 0)
                                                        {
                                                            vc.CatchLenFreqViewModel = new CatchLenFreqViewModel(vc);
                                                            foreach (var lf in cvc.LenFreqs)
                                                            {
                                                                CatchLenFreq clf = new CatchLenFreq
                                                                {
                                                                    PK = CatchLenFreqViewModel.CurrentIDNumber + 1,
                                                                    LengthClass = lf.Length,
                                                                    Frequency = lf.Freq,
                                                                    Parent = vc,
                                                                    DelayedSave = true
                                                                };

                                                                if (vc.CatchLenFreqViewModel.AddRecordToRepo(clf))
                                                                {
                                                                    CatchLenFreqViewModel.CurrentIDNumber = clf.PK;

                                                                }
                                                            }
                                                            vc.CatchLenFreqViewModel.Dispose();
                                                        }

                                                        if (cvc.LenWts?.Count > 0)
                                                        {
                                                            vc.CatchLengthWeightViewModel = new CatchLengthWeightViewModel(vc);
                                                            foreach (var lw in cvc.LenWts)
                                                            {
                                                                CatchLengthWeight clw = new CatchLengthWeight
                                                                {
                                                                    PK = CatchLengthWeightViewModel.CurrentIDNumber + 1,
                                                                    Parent = vc,
                                                                    Length = lw.Length,
                                                                    Weight = lw.Weight_kg,
                                                                    DelayedSave = true
                                                                };

                                                                if (vc.CatchLengthWeightViewModel.AddRecordToRepo(clw))
                                                                {
                                                                    CatchLengthWeightViewModel.CurrentIDNumber = clw.PK;
                                                                }
                                                            }
                                                            vc.CatchLengthWeightViewModel.Dispose();
                                                        }

                                                        if (cvc.Lengths?.Count > 0)
                                                        {
                                                            vc.CatchLengthViewModel = new CatchLengthViewModel(vc);
                                                            foreach (var lt in cvc.Lengths)
                                                            {
                                                                CatchLength clt = new CatchLength
                                                                {
                                                                    PK = CatchLengthViewModel.CurrentIDNumber + 1,
                                                                    Length = lt.Length,
                                                                    Parent = vc,
                                                                    DelayedSave = true
                                                                };

                                                                if (vc.CatchLengthViewModel.AddRecordToRepo(clt))
                                                                {
                                                                    CatchLengthViewModel.CurrentIDNumber = clt.PK;
                                                                }
                                                            }
                                                            vc.CatchLengthViewModel.Dispose();
                                                        }

                                                        if (cvc.GonadalMaturities?.Count > 0)
                                                        {
                                                            vc.CatchMaturityViewModel = new CatchMaturityViewModel(vc);
                                                            foreach (var cm in cvc.GonadalMaturities)
                                                            {
                                                                CatchMaturity c_mt = new CatchMaturity
                                                                {
                                                                    PK = CatchMaturityViewModel.CurrentIDNumber + 1,
                                                                    Parent = vc,
                                                                    Length = cm.Length,
                                                                    Weight = cm.Weight,
                                                                    SexCode = cm.Sex,
                                                                    MaturityCode = cm.GMS_Stage,
                                                                    GonadWeight = cm.GonadWt,
                                                                    WeightGutContent = cm.StomachContentWt,
                                                                    GutContentCode = cm.GutContentCategory,
                                                                    DelayedSave = true
                                                                };

                                                                if (vc.CatchMaturityViewModel.AddRecordToRepo(c_mt))
                                                                {
                                                                    CatchMaturityViewModel.CurrentIDNumber = c_mt.PK;
                                                                }
                                                            }
                                                            vc.CatchMaturityViewModel.Dispose();
                                                        }

                                                        VesselCatchViewModel.CurrentIDNumber = vc.PK;

                                                    }
                                                }
                                                cl.VesselCatchViewModel.Dispose();
                                                fish_carrier.SavedInLocalDatabase = true;
                                            }
                                        }

                                    }
                                    lss.CarrierLandingViewModel.Dispose();
                                    savedCount++;
                                    TotalUploadCount++;
                                }

                            }
                        }
                    }
                    UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { LandingSiteSamplingProcessedCount = lss_loop_count, Intent = UploadToDBIntent.LandingSiteSamplingProcessed });
                }
                catch (Exception ex)
                {
                    Utilities.Logger.Log(ex);
                }
            }
            UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { Intent = UploadToDBIntent.EndOfUpload });
            Utilities.Logger.LogUploadJSONToLocalDB($"End uploading JSON to local db with {savedCount} landings");
            LandingSiteSamplingProcessedCount += lss_loop_count;
            Utilities.Logger.LogUploadJSONToLocalDB($"Landing site sampling processed count: {LandingSiteSamplingProcessedCount}");
            Utilities.Logger.LogUploadJSONToLocalDB($"Landing site sampling unique count: {LandingSiteSamplingUniqueCount}");
            return savedCount > 0;
        }
        public static DateTime? JSONFileCreationTime { get; set; }
        public static bool CancelUpload { get; set; }
        public static bool DelayedSave { get; set; }
        public static string JSON { get; set; }
        public static List<MultiVessel_Optimized_Root> MultiVesselLandings { get; internal set; }
        public static List<CountLandings_Optimized> ListOfLandingsCount { get; internal set; }
        public static void CreateLandingCountsFromJSON()
        {
            try
            {
                ListOfLandingsCount = JsonConvert.DeserializeObject<List<CountLandings_Optimized>>(JSON);
            }
            catch (Exception ex)
            {
                Utilities.Logger.Log(ex);
            }
        }

        public static void CreateLandingsFromSingleJson()
        {
            try
            {
                MultiVesselLandings = new List<MultiVessel_Optimized_Root>();
                var j = JObject.Parse(JSON);

                MultiVesselLandings = JsonConvert.DeserializeObject<List<MultiVessel_Optimized_Root>>(j["results"].ToString());
                //MultiVesselGear_Root root = JsonConvert.DeserializeObject<MultiVesselGear_Root>(j["results"].ToString());
                //MultiVesselLandings.Add(root);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

        }
        public static void CreateLandingsFromJSON()
        {
            //How are PKs assigned to each landings contained in each incoming batch of JSON?
            //call VesselLanding.SetRowIDs()

            //MultiVesselGear_SampledLanding.SetRowIDs();
            try
            {
                MultiVesselLandings = JsonConvert.DeserializeObject<List<MultiVessel_Optimized_Root>>(JSON);
            }
            catch (Exception ex)
            {
                Utilities.Logger.Log(ex);
            }
        }
    }
    public class MultiVessel_Optimized_CBS_CarrierBoatFishingGround
    {
        //private List<FishingGround> _fishingGrounds;
        private static int _pk;
        private int _rowID;

        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public FishingGround FishingGround
        {
            get { return NSAPEntities.FishingGroundViewModel.GetFishingGround(FishingGroundCode); }
        }
        public string FishingGroundCode { get; set; }

        public int? PK
        {
            get
            {
                if (Parent.Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }

            }
        }
        public static void SetRowIDs()
        {
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastCarrierSamplingFishingGroundPK;
            CarrierBoatLanding_FishingGround_ViewModel.CurrentIDNumber = _pk;

            RowIDSet = true;
        }
        //public MultiVessel_Optimized_CBS_CarrierBoatFishingGround(MultiVessel_Optimized_Root parent)
        //{
        //    Parent = parent;
        //    _fishingGrounds = new List<FishingGround>();
        //    _fishingGrounds.Add(NSAPEntities.FishingGroundViewModel.GetFishingGround(Parent.FishingGroundCode));

        //    string[] fgs = Parent.OtherFishingGrounds.Split(' ');
        //    foreach (string fg in fgs)
        //    {
        //        _fishingGrounds.Add(NSAPEntities.FishingGroundViewModel.GetFishingGround(fg));
        //    }
        //}
        public MultiVessel_Optimized_CBS_FishCarrier Parent { get; set; }
        //public List<FishingGround> FishingGrounds { get { return _fishingGrounds; } }
    }
    public class MultiVessel_Optimized_LandingGear
    {
        private MultiVessel_Optimized_SampledLanding _parent;
        private static int _pk;
        private int _rowID;

        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public static void SetRowIDs()
        {


            //    _pk = NSAPEntities.FishingGroundGridViewModel.NextRecordNumber - 1;
            //}

            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastVesselUnloadGearPK;
            VesselUnload_FishingGearViewModel.CurrentIDNumber = _pk;

            RowIDSet = true;
        }
        public MultiVessel_Optimized_SampledLanding Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }


        public string GearName
        {
            get
            {
                if (Gear == null)
                {
                    return GearText;
                }
                else
                {
                    return Gear.GearName;
                }
            }
        }
        public Gear Gear
        {
            get
            {
                if (string.IsNullOrEmpty(GearCode))
                {
                    return null;
                }
                else
                {
                    return NSAPEntities.GearViewModel.GetGear(GearCode);
                }
            }
        }
        public string GearCode { get; set; }
        public string GearText { get; set; }

        public double? WeightOfCatch { get; set; }

        public int? NumberOfSpeciesCaught { get; set; }

        public int? PK
        {
            get
            {
                if (Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }

            }
        }
    }
    public class CountLandings_Optimized
    {
        [JsonProperty("_id")]
        public int LandingsId { get; set; }
        [JsonProperty("today")]
        public string Today { get; set; }

        public DateTime UploadDate { get { return DateTime.Parse(Today); } }

        [JsonProperty("G_lss/count_sampled_landings")]
        public int CountSampledLandings { get; set; }
    }
    public class MultiVessel_Optimized_Gear
    {
        private MultiVessel_Optimized_Root _parent;
        private static int _pk;
        private int _rowID;
        //private MultiVesselGear_SampledLanding _parent;
        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public static void SetRowIDs()
        {

            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastGearUnloadPK;
            GearUnloadViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }

            }
        }

        public MultiVessel_Optimized_Root Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        [JsonProperty("R_g/count_loop")]
        public int GearLoopCounter { get; set; }

        [JsonProperty("R_g/G_g/position_indicator")]
        public string position_indicator { get; set; }

        [JsonProperty("R_g/G_g/select_gear")]

        public string Will_select_gear { get; set; }

        [JsonProperty("R_g/G_g/gear_remarks")]
        public string GearRemarks { get; set; }
        public bool WillSelectGear
        {
            get
            {
                return Will_select_gear == "yes";
            }
        }

        [JsonProperty("R_g/G_g/gear_used")]
        public string GearSelected { get; set; }

        [JsonProperty("R_g/G_g/gear_used_text")]
        public string GearUsedText { get; set; }

        [JsonProperty("R_g/G_g/know_landing_count_gear")]
        public string CountOfLandingsOfGearKnown { get; set; }

        public bool IsCountOfLandingsOfGearKnown { get { return CountOfLandingsOfGearKnown == "yes"; } }


        [JsonProperty("R_g/G_g/know_count_flag")]
        public string know_count_flag { get; set; }

        [JsonProperty("R_g/G_g/count_landings_of_gear_m")]
        public int NumberOfLandingsMunicipal { get; set; }

        [JsonProperty("R_g/G_g/count_landings_of_gear_c")]
        public int NumberOfLandingsCommercial { get; set; }

        [JsonProperty("R_g/G_g/count_landings_of_gear")]
        public int? NumberOfLandingsOfGear { get; set; }

        [JsonProperty("R_g/G_g/gear_total_count_landings")]
        public string gear_total_count_landings { get; set; }

        [JsonProperty("R_g/G_g/catch_of_gear_m")]
        public double? WeightOfCatchMunicipal { get; set; }

        [JsonProperty("R_g/G_g/total_catch_of_gear")]
        public string total_catch_of_gear { get; set; }

        [JsonProperty("R_g/G_g/repeat_gear_code")]
        public string GearCode { get; set; }

        [JsonProperty("R_g/G_g/repeat_gear_name")]
        public string GearName { get; set; }

        [JsonProperty("R_g/G_g/choices_so_far")]
        public string choices_so_far { get; set; }

        [JsonProperty("R_g/G_g/group_title")]
        public string group_title { get; set; }

        [JsonProperty("R_g/G_g/catch_of_gear_c")]
        public double? WeightOfCatchCommercial { get; set; }

        [JsonProperty("R_g/G_g/catch_of_gear")]
        public double? WeightOfCatchOfGear { get; set; }

        //[JsonProperty("R_g/G_g/gear_remarks")]
        //public string gear_remarks { get; set; }
    }

    public class MultiVessel_Optimized_SampledLanding
    {
        private bool? _isSaved;
        private static int _pk;
        private int _rowid;
        private MultiVessel_Optimized_Root _parent;
        private List<MultiVessel_Optimized_GearEffort> _gearEfforts;
        private List<MultiVessel_Optimized_LandingGear> _landingGears;
        private List<MultiVessel_Optimized_CatchCompositionItem> _catchCompositionItems;
        private List<MultiVessel_Optimized_GearCatchComposition> _gearCatchCompositionItems;
        private List<MultiVessel_Optimized_GearAndWeight> _landingGearsWithWeight;

        private List<MultiVessel_Optimized_FishingGrid> _fishingGrids;

        private List<MultiVessel_Optimized_SoakTime> _gearSoakTimes;
        private readonly object collectionLock = new object();
        private SummaryItem _savedVesselUnloadObject;
        private string _gearsUsedInSampledLanding;

        public int NumberOfGearsUsedInSampledLanding
        {
            get
            {
                if (GearsUsedInSampledLanding == null)
                {
                    return 0;
                }
                else
                {
                    return GearsUsedInSampledLanding.Split(' ').Length;
                }
            }
        }
        private SummaryItem SavedVesselUnloadObject
        {
            get
            {
                lock (collectionLock)
                {
                    try
                    {

                        //_savedVesselUnloadObject = NSAPEntities.SummaryItemViewModel.SummaryItemCollection.FirstOrDefault(t => t.ODKRowID == _uuid);
                        _savedVesselUnloadObject = NSAPEntities.SummaryItemViewModel.GetItemEx(Parent.SubmissionUUID);
                        if (_savedVesselUnloadObject == null)
                        {
                            _savedVesselUnloadObject = NSAPEntities.SummaryItemViewModel.GetItem(enumeratorName: Parent.EnumeratorName, refNo: Reference_number);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (Debugger.IsAttached)
                        {
                            Utilities.Logger.Log("VesselUnloadServerRepository.SavedVesselUnloadObject SummaryItemCollection was modified while in debug mode when getting SavedVesselUnloadObject.");
                            try
                            {
                                _savedVesselUnloadObject = NSAPEntities.SummaryItemViewModel.GetItemEx(Parent.UUID);
                            }
                            catch
                            {
                                _savedVesselUnloadObject = null;
                            }
                        }
                        else
                        {
                            Utilities.Logger.Log("VesselUnloadServerRepository.SavedVesselUnloadObject Error when getting savedVeselUnloadObject when in release mode", ex);
                            _savedVesselUnloadObject = null;
                        }
                    }
                }
                return _savedVesselUnloadObject;
            }

        }
        public DateTime SamplingDate
        {
            get
            {
                return Parent.SamplingDate.Add(TimeOfSampling.TimeOfDay);
            }
        }


        public int PK
        {
            get
            {
                if (!SavedInLocalDatabase)
                {
                    if (_rowid == 0)
                    {
                        _rowid = ++_pk;
                    }
                }
                else
                {
                    if (_savedVesselUnloadObject == null)
                    {
                        _savedVesselUnloadObject = SavedVesselUnloadObject;
                    }

                    if (Debugger.IsAttached)
                    {
                        try
                        {
                            _rowid = (int)_savedVesselUnloadObject.VesselUnloadID;
                        }
                        catch
                        {
                            _rowid = (int)SavedVesselUnloadObject.VesselUnloadID;
                            _savedVesselUnloadObject = SavedVesselUnloadObject;
                        }
                    }
                    else
                    {
                        _rowid = (int)_savedVesselUnloadObject.VesselUnloadID;
                    }
                }
                return _rowid;
            }
        }
        public bool SavedInLocalDatabase
        {
            get
            {
                if (_isSaved == null)
                {
                    _isSaved = SavedVesselUnloadObject != null;
                }
                return (bool)_isSaved;
            }
            set { _isSaved = value; }
        }
        public static bool RowIDSet { get; private set; }


        public static void SetRowIDs()
        {
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastVesselUnloadPK;
            VesselUnloadViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }
        public MultiVessel_Optimized_Root Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        [JsonProperty("R_l/landing_count_loop")]
        public int LandingSequence { get; set; }

        [JsonProperty("R_l/G_sl/landing_position_indicator")]
        public string landing_position_indicator { get; set; }

        [JsonProperty("R_l/G_sl/G_l/sampling_time")]
        public DateTime TimeOfSampling { get; set; }

        [JsonProperty("R_l/G_sl/G_l/is_boat_used")]
        public string BoatIsUsedInLanding { get; set; }
        public bool IsBoatUsedInLanding { get { return BoatIsUsedInLanding == "yes"; } set { BoatIsUsedInLanding = ""; } }

        [JsonProperty("R_l/G_sl/G_l/boat_used")]
        public int? BoatUsedID { get; set; }

        //[JsonProperty("R_l/G_sl/G_l/select_vessel")]
        //public string select_vessel { get; set; }

        [JsonProperty("R_l/G_sl/G_l/fish_sector")]
        public string SectorOfLanding { get; set; }

        //[JsonProperty("R_l/G_sl/G_l/sector_name")]
        //public string sector_name { get; set; }

        //[JsonProperty("R_l/G_sl/G_l/vessel_csv_source")]
        //public string vessel_csv_source { get; set; }

        //[JsonProperty("R_l/G_sl/G_l/search_mode")]
        //public string search_mode { get; set; }

        //[JsonProperty("R_l/G_sl/G_l/search_column")]
        //public string search_column { get; set; }

        //[JsonProperty("R_l/G_sl/G_l/search_value")]
        //public string search_value { get; set; }

        [JsonProperty("R_l/G_sl/G_l/boat_used_text")]
        public string BoatUsedText { get; set; }

        [JsonProperty("R_l/G_sl/G_l/number_of_fishers")]
        public int? NumberOfFishers { get; set; }

        [JsonProperty("R_l/G_sl/G_l/landing_gears_used")]
        //public string landing_gears_used { get; set; }

        public string GearsUsedInSampledLanding
        {
            get { return _gearsUsedInSampledLanding; }
            set
            {
                _gearsUsedInSampledLanding = value;
            }
        }

        public List<MultiVessel_Optimized_LandingGear> SampledLandingGears
        {
            get
            {
                if (_gearsUsedInSampledLanding != null)
                {
                    if (_landingGears == null)
                    {
                        _landingGears = new List<MultiVessel_Optimized_LandingGear>();
                        var gears = _gearsUsedInSampledLanding.Split(' ');
                        foreach (string g in gears)
                        {

                            var gg = _parent.GearsInLandingSite.FirstOrDefault(t => t.GearLoopCounter == int.Parse(g));
                            MultiVessel_Optimized_LandingGear lg = new MultiVessel_Optimized_LandingGear
                            {
                                GearCode = gg.GearCode,
                                GearText = gg.GearUsedText,
                                Parent = this,
                            };
                            _landingGears.Add(lg);
                        }
                    }

                    return _landingGears;
                }
                else
                {
                    return null;
                }
            }
        }





        [JsonProperty("R_l/G_sl/G_l/has_ETP_gear_interaction")]
        public string HasETPGearInteraction { get; set; }
        public bool GearHasInteractionWithETP
        {
            get
            {
                return HasETPGearInteraction == "yes";
            }
        }


        [JsonProperty("R_l/G_sl/G_l/etps_encoutered")]
        public string ETPs_encountered { get; set; }


        public List<string> ListETPsEncountered
        {
            get
            {
                var ls = new List<string>();
                var arr = ETPs_encountered.Split(' ');
                foreach (var item in arr)
                {
                    switch (item)
                    {
                        case "mm":
                            ls.Add("Marine mammals");
                            break;
                        case "st":
                            ls.Add("Sea turtles");
                            break;
                        case "sh":
                            ls.Add("Sharks");
                            break;
                        case "ry":
                            ls.Add("Rays");
                            break;
                    }
                }
                return ls;
            }
        }


        [JsonProperty("R_l/G_sl/G_l/interaction_type")]
        public string InteractionTypesWithETPs { get; set; }


        public List<string> ListInteractionTypesWithETPs

        {
            get
            {
                var ls = new List<string>();
                var arr = InteractionTypesWithETPs.Split(' ');
                foreach (var item in arr)
                {
                    switch (item)
                    {
                        case "efg":
                            ls.Add("Escape from gear");
                            break;
                        case "rel":
                            ls.Add("Release");
                            break;
                        case "irl":
                            ls.Add("Injury and release");
                            break;
                        case "mor":
                            ls.Add("With Mortality");
                            break;
                        case "oin":
                            ls.Add("Other interaction");
                            break;
                    }
                }
                return ls;
            }
        }




        [JsonProperty("R_l/G_sl/G_l/other_interaction")]
        public string OtherTypeOfInteractionWithETPs { get; set; }


        //[JsonProperty("R_l/G_sl/G_l/count_gears_used")]
        //public string count_gears_used { get; set; }

        [JsonProperty("R_l/G_sl/G_l/main_gear_used")]
        public int Main_gear_used { get; set; }

        //[JsonProperty("R_l/G_sl/G_l/count_landing_of_gear")]
        //public string count_landing_of_gear { get; set; }

        [JsonProperty("R_l/G_sl/gear_name")]
        public string Main_gear_name { get; set; }

        [JsonProperty("R_l/G_sl/gear_code")]
        public string Main_gear_code { get; set; }

        [JsonProperty("R_l/G_sl/boat_name")]
        public string Boat_name { get; set; }

        //[JsonProperty("R_l/G_sl/decimal_sampling_date")]
        //public string decimal_sampling_date { get; set; }

        //[JsonProperty("R_l/G_sl/sampling_date_time_string")]
        //public string sampling_date_time_string { get; set; }

        [JsonProperty("R_l/G_sl/G_vc/trip_isSuccess")]
        public string Trip_isSuccess { get; set; }
        public bool TripIsSuccess { get { return Trip_isSuccess == "yes"; } set { Trip_isSuccess = ""; } }

        [JsonProperty("R_l/G_sl/G_vc/catch_total")]
        public double? CatchTotal { get; set; }

        [JsonProperty("R_l/G_sl/G_vc/catch_sampled")]
        public double? CatchSampled { get; set; }

        //[JsonProperty("R_l/G_sl/G_vc/running_sum_total_rounded")]
        //public string running_sum_total_rounded { get; set; }

        //[JsonProperty("R_l/G_sl/G_vc/running_sum_rounded")]
        //public string running_sum_rounded { get; set; }

        [JsonProperty("R_l/G_sl/G_vc/is_catch_sold")]
        public string Is_catch_sold { get; set; }

        public bool IsCatchSold { get { return Is_catch_sold == "yes"; } }

        //[JsonProperty("R_l/G_sl/G_vc/know_catch_wt_per_gear")]
        //public string know_catch_wt_per_gear { get; set; }

        //[JsonProperty("R_l/G_sl/G_vc/sample_wt_text")]
        //public string sample_wt_text { get; set; }

        //[JsonProperty("R_l/G_sl/R_cwg_count")]
        //public string R_cwg_count { get; set; }

        [JsonProperty("R_l/G_sl/R_cwg")]
        public List<MultiVessel_Optimized_GearAndWeight> LandingGearsWithWeight { get; set; }

        //[JsonProperty("R_l/G_sl/sum_weight_catch")]
        //public string sum_weight_catch { get; set; }

        //[JsonProperty("R_l/G_sl/sum_weight_sample")]
        //public string sum_weight_sample { get; set; }

        [JsonProperty("R_l/G_sl/G_cg/C_co/include_bingo")]
        public string Include_bingo { get; set; }

        public bool IncludeBingo { get { return Include_bingo == "yes"; } }

        [JsonProperty("R_l/G_sl/G_cg/C_co/utmZone")]
        public string UTMZone { get; set; }

        //[JsonProperty("R_l/G_sl/G_cg/majorgrid_csv")]
        //public string majorgrid_csv { get; set; }

        //[JsonProperty("R_l/G_sl/G_cg/inlandgrid_csv")]
        //public string inlandgrid_csv { get; set; }

        [JsonProperty("R_l/G_sl/G_cg/R_b")]
        public List<MultiVessel_Optimized_FishingGrid> FishingGrids { get; set; }

        //[JsonProperty("R_l/G_sl/G_cg/count_grids")]
        //public string count_grids { get; set; }

        [JsonProperty("R_l/G_sl/G_st/include_soak_time")]
        public string Include_soak_time { get; set; }

        public bool IncludeSoakTime { get { return Include_soak_time == "yes"; } }

        [JsonProperty("R_l/G_sl/G_st/R_st")]
        public List<MultiVessel_Optimized_SoakTime> GearSoakTimes { get; set; }

        [JsonProperty("R_l/G_sl/G_e/include_effort")]
        public string Include_effort { get; set; }
        public bool IncludeEffort { get { return Include_effort == "yes"; } }

        //[JsonProperty("R_l/G_sl/G_e/outside_effort_repeat")]
        //public string outside_effort_repeat { get; set; }

        //[JsonProperty("R_l/G_sl/G_e/R_ge_count")]
        //public string R_ge_count { get; set; }

        [JsonProperty("R_l/G_sl/G_e/R_ge")]
        public List<MultiVessel_Optimized_GearEffort> GearEfforts { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/G_cc_sl/include_catchcomp_g")]
        //public string include_catchcomp_g { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/G_cc_sl/length_type_g")]
        //public string length_type_g { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/G_cc_sl/has_gms_g")]
        //public string has_gms_g { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/count_catch_comp")]
        //public string count_catch_comp { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/include_catchcomp")]
        public string Include_catchcomp { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/raising_factor")]
        public double? RaisingFactor { get; set; }

        public bool IncludeCatchComp { get { return Include_catchcomp == "yes"; } set { Include_catchcomp = ""; } }

        [JsonProperty("R_l/G_sl/G_cc/length_type")]
        public string Length_type { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/has_gms")]
        public string Has_gms { get; set; }

        public bool HasGMS { get { return Has_gms == "yes"; } }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc_count")]
        //public string R_gcc_count { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc")]
        public List<MultiVessel_Optimized_GearCatchComposition> GearCatchCompositionItems { get; set; }

        //[JsonProperty("R_l/G_sl/G_ft/sum_species_wt_note")]
        //public string sum_species_wt_note { get; set; }

        //[JsonProperty("R_l/G_sl/G_ft/total_weight_gears")]
        //public string total_weight_gears { get; set; }

        //[JsonProperty("R_l/G_sl/G_ft/catch_composition_weight_text")]
        //public string catch_composition_weight_text { get; set; }

        [JsonProperty("R_l/G_sl/G_ft/ref_no")]
        public string Reference_number { get; set; }


        [JsonProperty("R_l/G_sl/G_vc/boxes_total")]
        public int? Boxes_total { get; set; }

        [JsonProperty("R_l/G_sl/G_vc/boxes_sampled")]
        public int? Boxes_sampled { get; set; }

        //[JsonProperty("R_l/G_sl/G_vc/remarks_normal_operation")]
        //public string remarks_normal_operation { get; set; }

        [JsonProperty("R_l/G_sl/G_vc/remarks")]
        public string Remarks { get; set; }

        [JsonProperty("R_l/G_sl/G_vc/trip_isCompleted")]
        public string Trip_isCompleted { get; set; }

        public bool TripIsCompleted { get { return Trip_isCompleted == "yes"; } set { Trip_isCompleted = ""; } }

        //[JsonProperty("R_l/G_sl/G_vc/remarks_not_completed")]
        //public string remarks_not_completed { get; set; }
    }

    public class MultiVessel_Optimized_GearCatchComposition
    {
        private MultiVessel_Optimized_SampledLanding _parent;
        private List<MultiVessel_Optimized_CatchCompositionItem> _catchCompositionItems;
        public MultiVessel_Optimized_SampledLanding Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/selected_gear_catch_comp")]
        //public string selected_gear_catch_comp { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/selected_gear_catch_comp_name")]
        public string GearName { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/gear_code_catch_comp")]
        //public string gear_code_catch_comp { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/selected_gear__catch_comp_code")]
        public string GearCode { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/weight_catch_of_gear")]
        public double? WeightOfCatch { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/sample_wt_of_gear")]
        public double? WeightOfSample { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/catch_composition_items_count")]
        public int? NumberOfSpeciesInCatchComposition { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc_count")]
        //public string R_cc_count { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc")]
        public List<MultiVessel_Optimized_CatchCompositionItem> CatchCompositionItems
        {
            get { return _catchCompositionItems; }
            set
            {
                _catchCompositionItems = value;
                foreach (var item in _catchCompositionItems)
                {
                    item.Parent = this;
                }
            }
        }
        //public List<MultiVessel_Optimized_CatchCompositionItem> CatchCompositionItems { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/count_species_comp")]
        public int? CountOfSpeciesComposition { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/sum_total")]
        public double? SumTotalWeight { get; set; }


        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/raising_factor_text")]
        public string RaisingFactorText { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/sum_sample")]
        public double? SumSampleWeight { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/sum_species_weight")]
        public double? SumSpeciesWeight { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/sum_species_weight_coalesce")]
        //public string sum_species_weight_coalesce { get; set; }
    }

    public class MultiVessel_Optimized_CatchCompositionItem
    {
        private MultiVessel_Optimized_GearCatchComposition _parent;
        private List<MultiVessel_Optimized_CatchGMS> _catchGMSes;
        private List<MultiVessel_Optimized_CatchLenght> _catchLengths;
        private List<MultiVessel_Optimized_CatchLenWt> _catchLengthWeights;
        private List<MultiVessel_Optimized_CatchLenFreq> _catchLenFreqs;
        private static int _pk;
        private int _rowID;
        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public static void SetRowIDs()
        {
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastVesselCatchPK;
            VesselCatchViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        public int PK
        {
            get
            {
                if (Parent.Parent.SavedInLocalDatabase)
                {
                    return 0;
                }
                else
                {

                    if (RowIDSet && _rowID == 0)
                    {

                        _rowID = ++_pk;
                    }

                }
                return _rowID;
            }
        }

        public bool HasMeasurements
        {
            get
            {
                return
                    (_catchGMSes != null && _catchGMSes.Count > 0) ||
                    (_catchLengths != null && _catchLengths.Count > 0) ||
                    (_catchLengthWeights != null && _catchLengthWeights.Count > 0) ||
                    (_catchLenFreqs != null && _catchLenFreqs.Count > 0);
            }
        }
        public MultiVessel_Optimized_GearCatchComposition Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/show_taxa_image")]
        //public string show_taxa_image { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/taxa_no_im")]
        //public string taxa_no_im { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/taxa")]
        public string SelectedTaxa { get; set; }
        public Taxa Taxa { get { return NSAPEntities.TaxaViewModel.GetTaxa(SelectedTaxa); } }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/select_spName")]
        public string SelectSpeciesNameFromList { get; set; }

        public bool IsSelectSpeciesNameFromList { get { return SelectSpeciesNameFromList == "yes"; } }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/species_csv_source")]
        //public string species_csv_source { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/search_species")]
        //public string search_species { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/species")]
        public int? SelectedFishSpeciesID { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/sp_id")]
        public int? SpeciesID { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/len_max_1")]
        //public string len_max_1 { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/len_max")]
        //public string len_max { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/species_name_selected")]
        public string SpeciesNameSelected { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/size_type_name")]
        //public string size_type_name { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/species_wt_1")]
        public double? WeightOfSpecies { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/species_wt")]
        public double? Species_wt { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/species_wt_rounded")]
        //public string species_wt_rounded { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/measure_len_and_gms")]
        public string Measure_len_and_gms { get; set; }

        public bool MeasureLengthAndGMS { get { return Measure_len_and_gms == "yes"; } }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/what_is_measured")]
        //public string what_is_measured { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/from_total_catch_code")]
        //public string from_total_catch_code { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/len_max_hint")]
        //public string len_max_hint { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/wt_unit_name")]
        //public string wt_unit_name { get; set; }



        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/from_total_catch")]
        public string From_total_catch { get; set; }
        public bool FromTotalCatch
        {
            get
            {
                if (Parent.SumSampleWeight == null)
                {
                    return true;
                }
                else
                {
                    return From_total_catch == "yes";
                }

            }
        }


        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/is_species_sold")]
        public string Is_species_sold { get; set; }

        public bool IsSpeciesSold { get { return Is_species_sold == "yes"; } }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/price_of_species")]
        public double? Price_of_species { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/pricing_unit")]
        public string Pricing_unit { get; set; }
        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/other_pricing_unit")]
        public string OtherPricingUnit { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/max_size_hint")]
        //public string max_size_hint { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/species_wt_check")]
        //public string species_wt_check { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/overwt_prompt")]
        //public string overwt_prompt { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/sample_wt_gear_text")]
        //public string sample_wt_gear_text { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/sum_wt_from_lenwt")]
        //public string sum_wt_from_lenwt { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/sum_wt_from_lenwt_coalesce")]
        //public string sum_wt_from_lenwt_coalesce { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/sum_wt_from_gms")]
        //public string sum_wt_from_gms { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/sum_wt_from_gms_coalesce")]
        //public string sum_wt_from_gms_coalesce { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/repeat_title")]
        //public string repeat_title { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/species_notfish")]
        public int? SelectedNonFishSpeciesID { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/has_measurement")]
        //public string has_measurement { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/species_sample_wt_total")]
        public double? WeightOfSampleOfSpeciesFromTotal { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/max_len_hint")]
        //public string max_len_hint { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/enforce_maxlen")]
        //public string enforce_maxlen { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/species_sample_wt")]
        public double? Species_sample_wt { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/species_sample_wt_from_sample")]
        //public string species_sample_wt_from_sample { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/include_sex_for_length")]
        public string Include_sex_when_measuring_length { get; set; }

        public bool IncludeSexWhenMeasuringLength { get { return Include_sex_when_measuring_length == "yes"; } }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/R_ll")]
        public List<MultiVessel_Optimized_CatchLenght> CatchLengths
        {
            get { return _catchLengths; }
            set
            {
                _catchLengths = value;
                foreach (var item in _catchLengths)
                {
                    item.Parent = this;
                }
            }
        }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/spName_other")]
        public string SpeciesNameOther { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/type_of_measure")]
        public string Type_of_measure { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/unknown_len_note")]
        //public string unknown_len_note { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/G_sd/individual_wt_unit")]
        public string Individual_wt_unit { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/R_lw")]
        public List<MultiVessel_Optimized_CatchLenWt> CatchLengthWeights
        {
            get { return _catchLengthWeights; }
            set
            {
                _catchLengthWeights = value;
                foreach (var item in _catchLengthWeights)
                {
                    item.Parent = this;
                }
            }
        }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/R_gms")]
        public List<MultiVessel_Optimized_CatchGMS> CatchGMSes
        {
            get { return _catchGMSes; }
            set
            {
                _catchGMSes = value;
                foreach (var item in _catchGMSes)
                {
                    item.Parent = this;
                }
            }
        }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/R_lf")]
        public List<MultiVessel_Optimized_CatchLenFreq> CatchLenFreqs
        {
            get { return _catchLenFreqs; }
            set
            {
                _catchLenFreqs = value;
                foreach (var item in _catchLenFreqs)
                {
                    item.Parent = this;
                }
            }
        }
    }

    public class MultiVessel_Optimized_CatchGMS
    {
        private MultiVessel_Optimized_CatchCompositionItem _parent;
        private static int _pk;
        private int _rowID;

        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public static void SetRowIDs()
        {
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastMaturityPK;
            CatchMaturityViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.Parent.Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }


            }
        }
        public MultiVessel_Optimized_CatchCompositionItem Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/R_gms/G_gms/individual_length")]
        public double? Individual_length { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/R_gms/G_gms/individual_weight")]
        public double? Individual_weight { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/R_gms/G_gms/sex")]
        public string Sex { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/R_gms/G_gms/gms_repeat")]
        public string GMS { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/R_gms/G_gms/gonad_wt")]
        public double? GonadWt { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/R_gms/G_gms/stomach_content_wt")]
        public double? Stomach_content_weight { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/R_gms/G_gms/gut_content_category")]
        public string GutContentCategory { get; set; }

        //[JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/R_gms/G_gms/combined_gms_fields")]
        //public string combined_gms_fields { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/R_gms/G_gms/individual_weight_kg")]
        public double? Individual_weight_kg { get; set; }
    }

    public class MultiVessel_Optimized_CatchLenFreq
    {
        private bool? _isSaved;
        private static int _pk;
        private int _rowID;
        private MultiVessel_Optimized_CatchCompositionItem _parent;
        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public static void SetRowIDs()
        {
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastLenFreqPK;
            CatchLenFreqViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        public MultiVessel_Optimized_CatchCompositionItem Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/R_lf/G_lf/length_class")]
        public double LengthClass { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/R_lf/G_lf/freq")]
        public int Frequency { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/R_lf/G_lf/sex_lf")]
        public string Sex { get; set; }

    }

    public class MultiVessel_Optimized_CatchLenght
    {
        private MultiVessel_Optimized_CatchCompositionItem _parent;
        private static int _pk;
        private int _rowID;
        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public static void SetRowIDs()
        {
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastLengthsPK;
            CatchLengthViewModel.CurrentIDNumber = _pk;
            //if (_pk == 0)
            //{

            //}
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.Parent.Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }


            }
        }
        public MultiVessel_Optimized_CatchCompositionItem Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/R_ll/G_ll/length")]
        public double Length { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/R_ll/G_ll/sex_l")]
        public string Sex { get; set; }

    }

    public class MultiVessel_Optimized_CatchLenWt
    {
        private MultiVessel_Optimized_CatchCompositionItem _parent;
        private static int _pk;
        private int _rowID;

        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }


        public static void SetRowIDs()
        {
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastLenWtPK;
            CatchLengthWeightViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.Parent.Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }
            }
        }
        public MultiVessel_Optimized_CatchCompositionItem Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/R_lw/G_lw/len_lenwt")]
        public double Length { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/R_lw/G_lw/wt_lenwt")]
        public double Weight { get; set; }


        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/R_lw/G_lw/wt_lenwt_kg")]
        public double Weight_kg { get; set; }

        [JsonProperty("R_l/G_sl/G_cc/R_gcc/G_irg/R_cc/G_sn/R_lw/G_lw/sex_lw")]
        public string Sex { get; set; }
    }

    public class MultiVessel_Optimized_FishingGrid
    {
        private static int _pk;
        private int _rowID;
        private MultiVessel_Optimized_SampledLanding _parent;
        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public static void SetRowIDs()
        {

            //if (NSAPEntities.FishingGroundGridViewModel.FishingGroundGridCollection.Count == 0)
            //{
            //    _pk = 0;
            //}
            //else
            //{
            //    _pk = NSAPEntities.FishingGroundGridViewModel.NextRecordNumber - 1;
            //}
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastFishingGridsPK;
            FishingGroundGridViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }

            }
        }

        public MultiVessel_Optimized_SampledLanding Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        [JsonProperty("R_l/G_sl/G_cg/R_b/G_b/major_grid")]
        public string MajorGrid { get; set; }

        [JsonProperty("R_l/G_sl/G_cg/R_b/G_b/col_name")]
        public string ColumnName { get; set; }

        [JsonProperty("R_l/G_sl/G_cg/R_b/G_b/row_name")]
        public string RowName { get; set; }

        [JsonProperty("R_l/G_sl/G_cg/R_b/G_b/bingo_complete")]
        public string GridNameComplete { get; set; }

        [JsonProperty("R_l/G_sl/G_cg/R_b/G_b/is_inland")]
        public string IsInland { get; set; }

        //[JsonProperty("R_l/G_sl/G_cg/R_b/G_b/group_label")]
        //public string group_label { get; set; }
    }

    public class MultiVessel_Optimized_GearEffort
    {
        private MultiVessel_Optimized_SampledLanding _parent;
        public MultiVessel_Optimized_SampledLanding Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public Gear Gear
        {
            get
            {
                if (SelectedGearCode == "_OT")
                {
                    return null;
                }
                else
                {
                    return NSAPEntities.GearViewModel.GetGear(SelectedGearCode);
                }
            }
        }
        [JsonProperty("R_l/G_sl/G_e/R_ge/selected_gear_name")]
        public string SelectedGearName { get; set; }


        [JsonProperty("R_l/G_sl/G_e/R_ge/selected_gear_code")]
        public string SelectedGearCode { get; set; }

        [JsonProperty("R_l/G_sl/G_e/R_ge/G_ei/R_e")]
        public List<MultiVessel_Optimized_GearEffortDetail> GearEffortDetails { get; set; }
    }

    public class MultiVessel_Optimized_GearEffortDetail
    {
        private MultiVessel_Optimized_GearEffort _parent;
        private static int _pk;
        private int _rowID;
        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public static void SetRowIDs()
        {
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastVesselUnloadGearSpecPK;
            VesselUnload_Gear_Spec_ViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }

            }
        }
        public MultiVessel_Optimized_GearEffort Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        [JsonProperty("R_l/G_sl/G_e/R_ge/G_ei/R_e/G_e_e/effort_type")]
        public string EffortTypePadded { get; set; }

        public int EffortType { get { return int.Parse(EffortTypePadded); } }

        [JsonProperty("R_l/G_sl/G_e/R_ge/G_ei/R_e/G_e_e/response_type")]
        public string ResponseType { get; set; }

        [JsonProperty("R_l/G_sl/G_e/R_ge/G_ei/R_e/G_e_e/effort_spec_name")]
        public string EffortSpecName { get; set; }

        [JsonProperty("R_l/G_sl/G_e/R_ge/G_ei/R_e/G_e_e/effort_intensity")]
        public double? EffortIntensity { get; set; }

        [JsonProperty("R_l/G_sl/G_e/R_ge/G_ei/R_e/G_e_e/selected_effort_measure")]
        public string SelectedEffortMeasure { get; set; }

        //[JsonProperty("R_l/G_sl/G_e/R_ge/G_ei/R_e/G_e_e/choices_gear_effort")]
        //public string choices_gear_effort { get; set; }

        [JsonProperty("R_l/G_sl/G_e/R_ge/G_ei/R_e/G_e_e/effort_desc")]
        public string EffortText { get; set; }
    }

    public class MultiVessel_Optimized_SoakTime
    {
        private MultiVessel_Optimized_SampledLanding _parent;
        private static int _pk;
        private int _rowID;

        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public static void SetRowIDs()
        {
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastGearSoaksPK;
            GearSoakViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }

            }
        }
        public MultiVessel_Optimized_SampledLanding Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }


        [JsonProperty("R_l/G_sl/G_st/R_st/G_stg/set_time")]
        public DateTime? TimeOfSet { get; set; }


        [JsonProperty("R_l/G_sl/G_st/R_st/G_stg/haul_time")]
        public DateTime? TimeOfHaul { get; set; }

    }

    public class MultiVessel_Optimized_GearAndWeight
    {

        //[JsonProperty("R_l/G_sl/R_cwg/selected_gear_wt")]
        //public string selected_gear_wt { get; set; }

        [JsonProperty("R_l/G_sl/R_cwg/selected_gear_wt_name")]
        public string GearName { get; set; }


        [JsonProperty("R_l/G_sl/R_cwg/G_gu/weight_catch_gear")]
        public double WeightOfCatch { get; set; }

        [JsonProperty("R_l/G_sl/R_cwg/G_gu/sample_wt_gr")]
        public double? WeightOfSample { get; set; }
    }


    public class MultiVessel_Optimized_CBS_CatcherBoat_FishingGroundGrid
    {
        private MultiVessel_Optimized_CBS_CatcherBoat _parent;
        private static int _pk;
        private int _rowID;


        public static void SetRowIDs()
        {
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastCatcherBoatFishingGroundGridPK;
            CatcherBoatOperation_ViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }
        public int? PK
        {
            get
            {
                if (Parent.Parent.Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }

            }
        }

        public static bool RowIDSet { get; private set; }
        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public MultiVessel_Optimized_CBS_CatcherBoat Parent { get { return _parent; } set { _parent = value; } }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_CB/G_ct_b_out/R_cb_fg/G_cb_fg_out/major_grid_cb")]
        public int CatcherBoatFishingGroundMajorGrid { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_CB/G_ct_b_out/R_cb_fg/G_cb_fg_out/col_name_cb")]
        public string CatcherBoatFishingGroundColumn { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_CB/G_ct_b_out/R_cb_fg/G_cb_fg_out/row_name_cb")]
        public int CatcherBoatFishingGroundRow { get; set; }
    }
    public class MultiVessel_Optimized_CBS_CatcherBoat
    {
        private List<MultiVessel_Optimized_CBS_CatcherBoat_FishingGroundGrid> _catcher_fishing_grounds;
        private MultiVessel_Optimized_CBS_FishCarrier _parent;
        private static int _pk;
        private int _rowID;


        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_CB/G_ct_b_out/R_cb_fg")]
        public List<MultiVessel_Optimized_CBS_CatcherBoat_FishingGroundGrid> Catcher_fishing_ground_grids

        {
            get
            {
                return _catcher_fishing_grounds;
            }
            set
            {
                _catcher_fishing_grounds = value;
                foreach (var item in _catcher_fishing_grounds)
                {
                    item.Parent = this; ;
                }

            }
        }

        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public static void SetRowIDs()
        {
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastCarrierSamplingCatcherBoatOperationPK;
            CatcherBoatOperation_ViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }

            }
        }
        public MultiVessel_Optimized_CBS_FishCarrier Parent { get { return _parent; } set { _parent = value; } }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_CB/G_ct_b_out/G_ct_b/fishing_operation_date")]
        public DateTime DateOfOperation { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_CB/G_ct_b_out/G_ct_b/gear_code_catcher")]
        public string GearCode { get; set; }



        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_CB/G_ct_b_out/G_fg_cb_main/fishing_ground_catcher_boat_known")]
        public string CatcherFishingGroundIsKnown { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_CB/G_ct_b_out/G_fg_cb_main/count_catcher_fg")]
        public int? CountFishingGroundsOfCatcher { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_CB/G_ct_b_out/G_fg_cb_main/cb_utmZone")]
        public string CatcherFishngGroundUTMZone { get; set; }

        public bool IsCatcherFishingGroundKnown
        {
            get
            {
                return CatcherFishingGroundIsKnown == "yes";
            }
        }

        public Gear Gear
        {
            get
            {
                return NSAPEntities.GearViewModel.GetGear(GearCode);
            }
        }


        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_CB/G_ct_b_out/G_ct_b/name_catcher")]
        public string NameCatcherBoat { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_CB/G_ct_b_out/G_ct_b/weight_catch_catcher")]
        public double? WeightOfCatch { get; set; }
    }

    public class MultiVessel_Optimized_CBS_GonadalMaturity
    {
        public MultiVessel_Optimized_CBS_CatchComposition Parent { get; set; }
        private static int _pk;
        private int _rowID;

        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public static void SetRowIDs()
        {
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastMaturityPK;
            CatchMaturityViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.Parent.Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }


            }
        }


        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/R_ct_cc_gms/G_ct_cc_gms/cbs_gms_gonad_wt")]
        public double? GonadWt { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/R_ct_cc_gms/G_ct_cc_gms/cbs_gms_gut_content_category")]
        public string GutContentCategory { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/R_ct_cc_gms/G_ct_cc_gms/cbs_gms_len")]
        public double? Length { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/R_ct_cc_gms/G_ct_cc_gms/cbs_gms_sex")]
        public string Sex { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/R_ct_cc_gms/G_ct_cc_gms/cbs_gms_stage")]
        public string GMS_Stage { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/R_ct_cc_gms/G_ct_cc_gms/cbs_gms_stomach_content_wt")]
        public double? StomachContentWt { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/R_ct_cc_gms/G_ct_cc_gms/cbs_gms_wt")]
        public double? Weight { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/R_ct_cc_gms/G_ct_cc_gms/cbs_gms_wt_kg")]
        public double? Weight_kg { get; set; }
    }

    public class MultiVessel_Optimized_CBS_Length
    {
        public MultiVessel_Optimized_CBS_CatchComposition Parent { get; set; }

        private static int _pk;
        private int _rowID;

        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public static void SetRowIDs()
        {
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastLengthsPK;
            CatchLengthViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.Parent.Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }


            }
        }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/R_ct_cc_l/G_ct_cc_l/cbs_individual_length")]
        public double Length { get; set; }
        //[JsonProperty("G_CBO/R_ct_cc/G_ct_cc_out/R_ct_cc_l/G_ct_cc_l/cbs_ll_grp_title")]
        //public string G_CBOR_ct_ccG_ct_cc_outR_ct_cc_lG_ct_cc_lcbs_ll_grp_title { get; set; }
    }

    public class MultiVessel_Optimized_CBS_LenFreq
    {
        public MultiVessel_Optimized_CBS_CatchComposition Parent { get; set; }

        private static int _pk;
        private int _rowID;

        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public static void SetRowIDs()
        {
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastLenFreqPK;
            CatchLenFreqViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.Parent.Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }


            }
        }
        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/R_ct_cc_lf/G_ct_cc_lf/cbs_lf_freq")]
        public int Freq { get; set; }


        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/R_ct_cc_lf/G_ct_cc_lf/cbs_lf_len")]
        public double Length { get; set; }
    }

    public class MultiVessel_Optimized_CBS_FishCarrier
    {
        private List<MultiVessel_Optimized_CBS_CatchComposition> _carrierCatchCompositions;
        private List<MultiVessel_Optimized_CBS_CatcherBoat> _carrier_catcherboats;
        private List<MultiVessel_Optimized_CBS_CarrierBoatFishingGround> _carrier_fishinggrounds;
        private string _fishingGrounds;
        private MultiVessel_Optimized_Root _parent;
        private static int _pk;
        private int _rowID;
        private bool? _isSaved = null;
        private readonly object collectionLock = new object();
        private SummaryItem _savedVesselUnloadObject;
        //private MultiVesselGear_SampledLanding _parent;
        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/G_cbd/carrier_fishing_grounds")]
        private string FishingGrounds
        {
            get { return _fishingGrounds; }
            set
            {
                _carrier_fishinggrounds = new List<MultiVessel_Optimized_CBS_CarrierBoatFishingGround>();
                _fishingGrounds = value;
                string[] fgs = _fishingGrounds.Split(' ');
                foreach (string fg in fgs)
                {
                    MultiVessel_Optimized_CBS_CarrierBoatFishingGround c_fg = new MultiVessel_Optimized_CBS_CarrierBoatFishingGround
                    {
                        FishingGroundCode = fg,
                        Parent = this
                    };
                    _carrier_fishinggrounds.Add(c_fg);
                }
            }
        }
        public static void SetRowIDs()
        {

            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastCarrierPK;
            CarrierLandingViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        private SummaryItem SavedVesselUnloadObject
        {
            get
            {
                lock (collectionLock)
                {
                    try
                    {

                        //_savedVesselUnloadObject = NSAPEntities.SummaryItemViewModel.SummaryItemCollection.FirstOrDefault(t => t.ODKRowID == _uuid);
                        _savedVesselUnloadObject = NSAPEntities.SummaryItemViewModel.GetItemEx(Parent.SubmissionUUID);
                        if (_savedVesselUnloadObject == null)
                        {
                            _savedVesselUnloadObject = NSAPEntities.SummaryItemViewModel.GetItem(enumeratorName: Parent.EnumeratorName, refNo: RefNo);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (Debugger.IsAttached)
                        {
                            Utilities.Logger.Log("VesselUnloadServerRepository.SavedVesselUnloadObject SummaryItemCollection was modified while in debug mode when getting SavedVesselUnloadObject.");
                            try
                            {
                                _savedVesselUnloadObject = NSAPEntities.SummaryItemViewModel.GetItemEx(Parent.UUID);
                            }
                            catch
                            {
                                _savedVesselUnloadObject = null;
                            }
                        }
                        else
                        {
                            Utilities.Logger.Log("VesselUnloadServerRepository.SavedVesselUnloadObject Error when getting savedVeselUnloadObject when in release mode", ex);
                            _savedVesselUnloadObject = null;
                        }
                    }
                }
                return _savedVesselUnloadObject;
            }

        }

        public int? PK
        {
            get
            {
                if (Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }

            }
        }

        public bool SavedInLocalDatabase
        {
            get
            {
                if (_isSaved == null)
                {
                    _isSaved = SavedVesselUnloadObject != null;
                }
                return (bool)_isSaved;
            }
            set { _isSaved = value; }
        }
        public int CountCarrierFishingGrounds
        {
            get
            {
                return _carrier_fishinggrounds.Count;
            }
        }
        public List<MultiVessel_Optimized_CBS_CarrierBoatFishingGround> Carrier_FishingGrounds

        {
            get { return _carrier_fishinggrounds; }
        }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_CB")]
        public List<MultiVessel_Optimized_CBS_CatcherBoat> Carrier_Catcherboats
        {
            get
            {
                return _carrier_catcherboats;
            }
            set
            {
                _carrier_catcherboats = value;
                foreach (var item in _carrier_catcherboats)
                {
                    item.Parent = this; ;
                }

            }
        }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc")]
        public List<MultiVessel_Optimized_CBS_CatchComposition> CarrierCatchCompositions
        {
            get { return _carrierCatchCompositions; }
            set
            {
                _carrierCatchCompositions = value;
                foreach (var item in _carrierCatchCompositions)
                {
                    item.Parent = this;
                }
            }
        }
        public MultiVessel_Optimized_Root Parent { get; set; }


        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/G_cbd/cbs_sampling_time")]
        public DateTime SamplingTime { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/G_cbd/carrier_boat_name")]
        public string CarrierName { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/G_cbd/know_catcher_boats")]
        public string KnowCatcherBoats { get; set; }

        public bool KnowCatcherBoatsOfCarrier { get { return KnowCatcherBoats == "yes"; } }


        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/G_cbd/count_catcher_boats")]
        public int? CountCatcherBoats { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/G_cbd/know_catch_wt_catchers")]
        public string KnowWeightOfCarrierCatch { get; set; }

        public bool KnowWeightOfCatchOfCarrier
        {
            get
            {
                return KnowWeightOfCarrierCatch == "yes";
            }
        }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/G_cbd/wt_catch_of_carrier")]
        public double? WeightOfCarrierCatch { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/G_cbd/count_species_of_carrier")]
        public int CountCarrierSpeciesComposition { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/G_cbd/wt_sample_from_carrrier")]
        public double? WeightOfSampleFromCarrier { get; set; }


        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/G_cbs_summary/cbs_ref_no")]
        public string RefNo { get; set; }
    }

    public class MultiVessel_Optimized_CBS_LenWt
    {

        public MultiVessel_Optimized_CBS_CatchComposition Parent { get; set; }
        private static int _pk;
        private int _rowID;

        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public static void SetRowIDs()
        {
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastLenWtPK;
            CatchLengthWeightViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.Parent.Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }


            }
        }


        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/R_ct_cc_lw/G_ct_cc_lw/cbs_lw_len")]
        public double Length { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/R_ct_cc_lw/G_ct_cc_lw/cbs_lw_wt")]
        public double Weight { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/R_ct_cc_lw/G_ct_cc_lw/cbs_lw_wt_kg")]
        public double Weight_kg { get; set; }
    }

    public class MultiVessel_Optimized_CBS_CatchComposition
    {
        private MultiVessel_Optimized_CBS_FishCarrier _parent;
        private static int _pk;
        private int _rowID;

        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public static void SetRowIDs()
        {
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastVesselCatchPK;
            VesselCatchViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }

            }
        }
        private List<MultiVessel_Optimized_CBS_GonadalMaturity> _gonadalMaturities { get; set; }


        private List<MultiVessel_Optimized_CBS_Length> _lengths { get; set; }


        private List<MultiVessel_Optimized_CBS_LenFreq> _lenFreqs { get; set; }


        private List<MultiVessel_Optimized_CBS_LenWt> _lenWts { get; set; }

        public MultiVessel_Optimized_CBS_FishCarrier Parent { get { return _parent; } set { _parent = value; } }
        //[JsonProperty("G_CBO/R_ct_cc/G_ct_cc_out/G_ct_cc/cbs_cc_grp_title")]
        //public string G_CBOR_ct_ccG_ct_cc_outG_ct_cccbs_cc_grp_title { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/G_ct_cc/cbs_count_individuals_measured")]
        public string CountIndividualsMeasured { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/G_ct_cc/cbs_count_length_class")]
        public string CountLengthClasses { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/G_ct_cc/cbs_determine_gms")]
        public string DetermineGMS { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/G_ct_cc/cbs_individual_wt_unit")]
        public string WeightUnit { get; set; }

        public bool HasGonadalMaturityMeasurement
        {
            get
            {
                return DetermineGMS == "yes";
            }
        }
        public bool HasLengthMeasurement
        {
            get
            {
                if (string.IsNullOrEmpty(ListOfLengthMeasurementsTypes))
                {
                    return false;
                }
                else
                {
                    return ListOfLengthMeasurementsTypes.Contains("length_list");
                }

            }
        }
        public bool HasLenFreqMeasurement
        {
            get
            {
                if (string.IsNullOrEmpty(ListOfLengthMeasurementsTypes))
                {
                    return false;
                }
                else
                {
                    return ListOfLengthMeasurementsTypes.Contains("length_freq");
                }
            }
        }
        public bool HasLenWtMeasurement
        {
            get
            {
                if (string.IsNullOrEmpty(ListOfLengthMeasurementsTypes))
                {
                    return false;
                }
                else
                {
                    return ListOfLengthMeasurementsTypes.Contains("length_wt");
                }
            }
        }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/G_ct_cc/cbs_len_type")]
        public string ListOfLengthMeasurementsTypes { get; set; }

        //[JsonProperty("G_CBO/R_ct_cc/G_ct_cc_out/G_ct_cc/cbs_search_species")]
        //public string G_CBOR_ct_ccG_ct_cc_outG_ct_cccbs_search_species { get; set; }

        //[JsonProperty("G_CBO/R_ct_cc/G_ct_cc_out/G_ct_cc/cbs_select_spName")]
        //public string G_CBOR_ct_ccG_ct_cc_outG_ct_cccbs_select_spName { get; set; }

        //[JsonProperty("G_CBO/R_ct_cc/G_ct_cc_out/G_ct_cc/cbs_show_taxa_image")]
        //public string G_CBOR_ct_ccG_ct_cc_outG_ct_cccbs_show_taxa_image { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/G_ct_cc/cbs_sp_id")]
        public int? SpeciesID { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/G_ct_cc/cbs_spName_other")]
        public string SpeciesNameOther { get; set; }

        //[JsonProperty("G_CBO/R_ct_cc/G_ct_cc_out/G_ct_cc/cbs_species")]
        //public string G_CBOR_ct_ccG_ct_cc_outG_ct_cccbs_species { get; set; }

        //[JsonProperty("G_CBO/R_ct_cc/G_ct_cc_out/G_ct_cc/cbs_species_csv_source")]
        //public string G_CBOR_ct_ccG_ct_cc_outG_ct_cccbs_species_csv_source { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/G_ct_cc/cbs_species_name_selected")]
        public string SpeciesName { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/G_ct_cc/cbs_species_wt")]
        public double? SpeciesWeight { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/G_ct_cc/cbs_taxa")]
        public string SelectedTaxa { get; set; }

        public Taxa Taxa { get { return NSAPEntities.TaxaViewModel.GetTaxa(SelectedTaxa); } }

        //[JsonProperty("G_CBO/R_ct_cc/G_ct_cc_out/G_ct_cc/cbs_taxa_no_im")]
        //public string G_CBOR_ct_ccG_ct_cc_outG_ct_cccbs_taxa_no_im { get; set; }

        //[JsonProperty("G_CBO/R_ct_cc/G_ct_cc_out/G_ct_cc/cbs_wt_unit_name")]
        //public string G_CBOR_ct_ccG_ct_cc_outG_ct_cccbs_wt_unit_name { get; set; }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/R_ct_cc_gms")]
        public List<MultiVessel_Optimized_CBS_GonadalMaturity> GonadalMaturities
        {
            get { return _gonadalMaturities; }
            set
            {
                _gonadalMaturities = value;
                foreach (var item in _gonadalMaturities)
                {
                    item.Parent = this;
                }
            }
        }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/R_ct_cc_l")]
        public List<MultiVessel_Optimized_CBS_Length> Lengths
        {
            get { return _lengths; }
            set
            {
                _lengths = value;
                foreach (var item in _lengths)
                {
                    item.Parent = this;
                }
            }
        }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/R_ct_cc_lf")]
        public List<MultiVessel_Optimized_CBS_LenFreq> LenFreqs
        {
            get { return _lenFreqs; }
            set
            {
                _lenFreqs = value;
                foreach (var item in _lenFreqs)
                {
                    item.Parent = this;
                }
            }
        }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/R_ct_cc_lw")]
        public List<MultiVessel_Optimized_CBS_LenWt> LenWts
        {
            get { return _lenWts; }
            set
            {
                _lenWts = value;
                foreach (var item in _lenWts)
                {
                    item.Parent = this;
                }
            }
        }

        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/R_ct_cc_gms_count")]
        public int? CountMaturityMeasurements { get; set; }



        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/R_ct_cc_l_count")]
        public int? CountLenghtMeasurements { get; set; }



        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/R_ct_cc_lf_count")]
        public string CountLenFreqMeasurements { get; set; }



        [JsonProperty("G_CBO/R_S_CB/G_cbd_o/R_ct_cc/G_ct_cc_out/R_ct_cc_lw_count")]
        public string CountLenWtMeasurements { get; set; }


    }
    public class MultiVessel_Optimized_Root
    {
        private List<MultiVessel_Optimized_Gear> _gearsInLandingSite;
        private List<MultiVessel_Optimized_SampledLanding> _sampledFishLandings;
        private List<MultiVessel_Optimized_CBS_FishCarrier> _fishCarriers;
        //private List<MultiVessel_Optimized_CBS_CatchComposition> _carrierBoatCatchCompositions;
        //private List<MultiVessel_Optimized_CatcherBoat> _catcherBoats;
        //private MultiVessel_Optimized_CBS_CarrierBoatFishingGround _carrierFishingGround;
        private static int _pk;
        private int _rowid;
        private bool? _isSaved;
        private SummaryItem _savedVesselUnloadObject;
        private readonly object collectionLock = new object();
        private string _typeOfSampling;


        public static bool RowIDSet { get; private set; }
        public bool SavedInLocalDatabase
        {
            get
            {
                if (_isSaved == null)
                {
                    _isSaved = SavedVesselUnloadObject != null;
                }
                return (bool)_isSaved;
            }
            set { _isSaved = value; }
        }

        public override string ToString()
        {
            if (LandingSiteID != null)
            {
                return $"{SamplingDate.ToString("MMM-dd-yyyy")}-{LandingSite.LandingSiteID}-{FishingGround.Code}";
            }
            else
            {
                return $"{SamplingDate.ToString("MMM-dd-yyyy")}-{LandingSiteName}-{FishingGround.Code}";
            }
        }
        public int PK
        {
            get
            {
                if (!SavedInLocalDatabase)
                {
                    if (_rowid == 0)
                    {
                        _rowid = ++_pk;
                    }
                }
                else
                {
                    if (_savedVesselUnloadObject == null)
                    {
                        _savedVesselUnloadObject = SavedVesselUnloadObject;
                    }

                    if (Debugger.IsAttached)
                    {
                        try
                        {
                            _rowid = (int)_savedVesselUnloadObject.VesselUnloadID;
                        }
                        catch
                        {
                            _rowid = (int)SavedVesselUnloadObject.VesselUnloadID;
                            _savedVesselUnloadObject = SavedVesselUnloadObject;
                        }
                    }
                    else
                    {
                        _rowid = (int)_savedVesselUnloadObject.VesselUnloadID;
                    }
                }
                return _rowid;
            }
        }
        public static void SetRowIDs()
        {
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastLandingSiteSamplingPK;
            LandingSiteSamplingViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        private SummaryItem SavedVesselUnloadObject
        {
            get
            {
                lock (collectionLock)
                {
                    try
                    {

                        //_savedVesselUnloadObject = NSAPEntities.SummaryItemViewModel.SummaryItemCollection.FirstOrDefault(t => t.ODKRowID == _uuid);
                        _savedVesselUnloadObject = NSAPEntities.SummaryItemViewModel.GetItemEx(UUID);
                    }
                    catch (Exception ex)
                    {
                        if (Debugger.IsAttached)
                        {
                            Utilities.Logger.Log("VesselUnloadServerRepository.SavedVesselUnloadObject SummaryItemCollection was modified while in debug mode when getting SavedVesselUnloadObject.");
                            try
                            {
                                _savedVesselUnloadObject = NSAPEntities.SummaryItemViewModel.GetItemEx(UUID);
                            }
                            catch
                            {
                                _savedVesselUnloadObject = null;
                            }
                        }
                        else
                        {
                            Utilities.Logger.Log("VesselUnloadServerRepository.SavedVesselUnloadObject Error when getting savedVeselUnloadObject when in release mode", ex);
                        }
                    }
                }
                return _savedVesselUnloadObject;
            }

        }

        public List<MultiVessel_Optimized_SampledLanding> SampledFishLandingsEx
        {
            get
            {
                var list = SampledFishLandings;
                if (list == null)
                {
                    list = new List<MultiVessel_Optimized_SampledLanding>();
                }
                if (GearsInLandingSite != null)
                {
                    foreach (var item in GearsInLandingSite.Where(t => t.Parent.IsSamplingDay == false).ToList())
                    {
                        MultiVessel_Optimized_SampledLanding sl = new MultiVessel_Optimized_SampledLanding
                        {
                            Parent = this,
                            Main_gear_code = item.GearCode,
                            Main_gear_name = item.GearName,
                            Remarks = ReasonNoLanding
                        };
                        list.Add(sl);
                    }
                }

                if (list.Count == 0)
                {
                    MultiVessel_Optimized_SampledLanding sl = new MultiVessel_Optimized_SampledLanding
                    {
                        Parent = this,
                        Remarks = ReasonNoLanding
                    };
                    list.Add(sl);
                }
                return list.OrderByDescending(t => t.SamplingDate).ToList();
            }
        }

        [JsonProperty("G_lss/count_carrier_sampling")]
        public int Count_Carrier_Sampling { get; set; }

        [JsonProperty("G_lss/count_carrier_landings")]
        public int? Count_Carrier_Landing { get; set; }


        [JsonProperty("G_CBO/R_S_CB")]
        public List<MultiVessel_Optimized_CBS_FishCarrier> FishCarriers
        {
            get
            {
                return _fishCarriers;
            }
            set
            {
                _fishCarriers = value;
                foreach (var item in _fishCarriers)
                {
                    item.Parent = this;
                }
            }
        }
        //[JsonProperty("G_CBO/G_cbd/carrier_boat_name")]
        //public string CarrierBoatName { get; set; }

        //[JsonProperty("G_CBO/G_cbd/cbs_sampling_time")]
        //public DateTime? SamplingTimeOfCarrierBoat { get; set; }

        //[JsonProperty("G_CBO/G_cbd/count_catcher_boats")]
        //public int? CountCatcherBoats { get; set; }

        //[JsonProperty("G_CBO/G_cbd/count_species_of_carrier")]
        //public int? CountSpeciesSampledFromCarrierBoat { get; set; }

        //[JsonProperty("G_CBO/G_cbd/know_catch_wt_catchers")]
        //public string KnowWeightOfCatchOfCatcherBoats { get; set; }

        //[JsonProperty("G_CBO/G_cbd/know_catcher_boats")]
        //public string KnowCatcherBoatDetails { get; set; }

        //[JsonProperty("G_CBO/R_CB")]
        //public List<MultiVessel_Optimized_CatcherBoat> CatcherBoats
        //{
        //    get { return _catcherBoats; }
        //    set
        //    {
        //        _catcherBoats = value;
        //        foreach (var item in _catcherBoats)
        //        {
        //            item.Parent = this;
        //        }
        //    }
        //}

        ////[JsonProperty("G_CBO/R_CB_count")]
        ////public int? CountCatcherBoats { get; set; }

        //[JsonProperty("G_CBO/R_ct_cc")]
        //public List<MultiVessel_Optimized_CBS_CatchComposition> CarrierBoatCatchComposition
        //{
        //    get { return _carrierBoatCatchCompositions; }
        //    set
        //    {
        //        _carrierBoatCatchCompositions = value;
        //        foreach (var item in _carrierBoatCatchCompositions)
        //        {
        //            item.Parent = this;
        //        }
        //    }
        //}

        //[JsonProperty("G_CBO/R_ct_cc_count")]
        //public string G_CBOR_ct_cc_count { get; set; }
        public int _id { get; set; }

        //[JsonProperty("formhub/uuid")]
        //public string formhubuuid { get; set; }
        public DateTime start { get; set; }
        public string today { get; set; }
        public string device_id { get; set; }
        public string user_name { get; set; }
        public string intronote { get; set; }

        [JsonProperty("G_lss/sampling_date")]
        public DateTime SamplingDate { get; set; }

        [JsonProperty("G_lss/nsap_region")]
        public string NSAP_Region { get; set; }

        public NSAPRegion NSAPRegion { get { return NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(NSAP_Region); } }

        [JsonProperty("G_lss/select_enumerator")]
        public string select_enumerator { get; set; }

        [JsonProperty("G_lss/region_enumerator")]
        public int? RegionEnumeratorID { get; set; }

        [JsonProperty("G_lss/region_enumerator_text")]
        public string RegionEnumeratorText { get; set; }


        public NSAPEnumerator NSAPEnumerator
        {
            get
            {
                if (RegionEnumeratorID == null)
                {
                    return null;
                }
                else
                {
                    //return NSAPEntities.NSAPEnumeratorViewModel.GetNSAPEnumerator((int)RegionEnumeratorID);
                    var nre = NSAPRegion.NSAPEnumerators.FirstOrDefault(t => t.RowID == (int)RegionEnumeratorID);
                    if (nre != null)
                    {
                        return nre.Enumerator;
                    }
                    else
                    {
                        return null;
                    }
                    //return NSAPRegion.NSAPEnumerators.FirstOrDefault(t => t.RowID == (int)RegionEnumeratorID).Enumerator;
                }
            }
        }
        public string EnumeratorName
        {
            get
            {
                if (RegionEnumeratorID == null)
                {
                    return RegionEnumeratorText;
                }
                else
                {
                    if (NSAPEnumerator != null)
                    {


                        return NSAPEnumerator.Name;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        [JsonProperty("G_lss/fma_in_region")]
        public int FMAinRegionID { get; set; }



        /// <summary>
        /// So far there are 2 types of sampling at a landing site
        /// rs = regular sampling - all the data in the catch and eform can be collected
        /// cbl = carrier based landing - some important data are not known at sampling because the carrier boats do not have that information
        /// </summary>
        [JsonProperty("G_lss/type_of_sampling")]
        public string TypeOfSampling
        {
            get
            {
                if (string.IsNullOrEmpty(_typeOfSampling))
                {
                    return "rs";
                }
                else
                {
                    return _typeOfSampling;
                }
            }
            set
            {
                _typeOfSampling = value;
            }
        }

        public string SamplingType
        {
            get
            {
                if (TypeOfSampling == "rs")
                {
                    return "Regular sampling";
                }
                else
                {
                    return "Carrier boat landings sampling";
                }
            }
        }

        public double FormVersion
        {
            get
            {
                string form_version = intronote.Replace("Version ", "");
                if (double.TryParse(form_version, out double v))
                {
                    return double.Parse(form_version);
                }
                else
                {
                    var s = form_version.Split('.');
                    if (s.Length > 0)
                    {
                        string ss = "";
                        int count = 0;
                        foreach (var i in s)
                        {
                            if (int.TryParse(i, out int ii))
                            {
                                if (count == 0)
                                {
                                    ss = $"{ii.ToString()}.";
                                }
                                else
                                {
                                    ss += ii.ToString();
                                }
                            }
                            count++;
                        }
                        if (ss.Length > 0)
                        {
                            return double.Parse(ss);
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
        }

        [JsonProperty("G_lss/reason_no_landings")]
        public string ReasonNoLanding { get; set; }

        public NSAPRegionFMA NSAPRegionFMA
        {
            get
            {
                return NSAPRegion.FMAs.Where(t => t.RowID == FMAinRegionID).FirstOrDefault();
            }
        }

        [JsonProperty("G_lss/fishing_ground")]
        public int RegionFishingGroundID { get; set; }

        [JsonProperty("G_lss/fishing_ground_code")]
        public string FishingGroundCode { get; set; }
        public FishingGround FishingGround
        {
            get
            {
                return RegionFishingGround?.FishingGround;
            }
        }
        public NSAPRegionFMAFishingGround RegionFishingGround
        {
            get
            {
                return NSAPRegionFMA.FishingGrounds.FirstOrDefault(t => t.RowID == RegionFishingGroundID);
            }
        }
        [JsonProperty("G_lss/select_landingsite")]
        public string select_landingsite { get; set; }

        [JsonProperty("G_lss/landing_site")]
        public int? LandingSiteID { get; set; }


        public LandingSite LandingSite
        {
            get
            {
                if (LandingSiteID != null)
                {
                    var ls = RegionFishingGround?.LandingSites.FirstOrDefault(t => t.RowID == (int)LandingSiteID)?.LandingSite;
                    if (ls != null)
                    {
                        return ls;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        [JsonProperty("G_lss/is_sampling_day")]
        public string Is_sampling_day { get; set; }

        public bool IsSamplingDay
        {
            get { return Is_sampling_day == "yes"; }
            set { Is_sampling_day = "yes"; }
        }

        [JsonProperty("G_lss/sampling_from_catch_allowed")]
        public string SamplingFromCatchAllowed { get; set; }

        public bool SamplingFromCatchCompositionAllowed { get { return SamplingFromCatchAllowed == "yes"; } }

        [JsonProperty("G_lss/are_there_landings")]
        public string Are_there_landings { get; set; }

        public bool AreThereLandings { get { return Are_there_landings == "yes"; } }

        [JsonProperty("G_lss/count_sampled_landings")]
        public int? CountSampledLandings { get; set; }
        [JsonProperty("G_lss/fishing_gear_type_count")]
        public int? FishingGearTypeCount { get; set; }

        [JsonProperty("G_lss/know_total_landing_count")]
        public string know_total_landing_count { get; set; }

        [JsonProperty("G_lss/fish_landing_count_constraint")]
        public string fish_landing_count_constraint { get; set; }

        [JsonProperty("G_lss/count_total_landing")]
        public int? CountTotalLanding { get; set; }
        public string is_sampling_day_text { get; set; }
        public string are_there_landings_text { get; set; }
        public string fma_number { get; set; }
        [JsonProperty("fishing_ground_name")]
        public string Fishing_ground_name { get; set; }
        [JsonProperty("landing_site_name")]
        public string Landing_site_name { get; set; }

        public string LandingSiteName { get { return Landing_site_name.Replace("»", ","); } }

        [JsonProperty("G_lss/landing_site_text")]
        public string LandingSiteText { get; set; }
        public string sampling_date_string { get; set; }
        public string count_total_landing_text { get; set; }
        public string count_sampled_landings_text { get; set; }
        public string count_number_gears_text { get; set; }
        public string outside_gear_repeat { get; set; }
        public string R_g_count { get; set; }

        [JsonProperty("R_g")]
        public List<MultiVessel_Optimized_Gear> GearsInLandingSite
        {
            get { return _gearsInLandingSite; }
            set
            {
                _gearsInLandingSite = value;
                foreach (var item in _gearsInLandingSite)
                {
                    item.Parent = this;
                }
            }
        }
        public string total_catch_weight_all_landings { get; set; }
        public string total_catch_weight_all_landings_text { get; set; }
        public string is_weight_catch_all_gears_known { get; set; }
        public string gear_used1 { get; set; }
        public string gear_used2 { get; set; }
        public string gear_used3 { get; set; }
        public string R_l_count { get; set; }
        [JsonProperty("R_l")]

        public List<MultiVessel_Optimized_SampledLanding> SampledFishLandings
        {
            get { return _sampledFishLandings; }
            set
            {
                _sampledFishLandings = value;
                foreach (var item in _sampledFishLandings)
                {
                    item.Parent = this;
                }
            }
        }
        public string sum_catch_wt_sampled_landings { get; set; }
        public string sum_catch_wt_sampled_landings_text { get; set; }
        public string gear_catch_weight_complete { get; set; }
        public string __version__ { get; set; }

        [JsonProperty("meta/instanceID")]
        public string metainstanceID { get; set; }

        [JsonProperty("meta/instanceName")]
        public string metainstanceName { get; set; }
        public string _xform_id_string { get; set; }
        //public string _uuid { get; set; }
        [JsonProperty("_uuid")]
        public string SubmissionUUID { get; set; }
        public List<object> _attachments { get; set; }
        public string _status { get; set; }
        public List<object> _geolocation { get; set; }
        [JsonProperty("_submission_time")]
        public DateTime SubmissionTime { get; set; }
        public List<object> _tags { get; set; }
        public List<object> _notes { get; set; }
        public ValidationStatus _validation_status { get; set; }
        public object _submitted_by { get; set; }

        [JsonProperty("formhub/uuid")]
        public string UUID { get; set; }
    }

    //public class ValidationStatus
    //{
    //}




}
