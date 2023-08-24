using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class CountLandings
    {
        [JsonProperty("_id")]

        public int LandingsId { get; set; }
        [JsonProperty("today")]
        public string Today { get; set; }

        public DateTime UploadDate { get { return DateTime.Parse(Today); } }

        [JsonProperty("landing_site_sampling_group/count_sampled_landings")]
        public int CountSampledLandings { get; set; }
    }


    public static class MultiVesselGear_UnloadServerRepository
    {
        //public static void ResetLists(bool includeJSON = false)
        //{
        //    _listGridBingoCoordinates = null;
        //    _listGearEfforts = null;
        //    _listGearSoakTimes = null;
        //    _listCatchComps = null;
        //    _listLenFreqs = null;
        //    _listGMS = null;
        //    _listLenWts = null;
        //    _listLengths = null;

        //    DuplicatedLenFreq = new List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat>();
        //    DuplicatedCatchComposition = new List<CatchCompGroupCatchCompositionRepeat>();
        //    DuplicatedEffortSpec = new List<EffortSpecSingleGear>();

        //    if (includeJSON)
        //    {
        //        JSON = "";
        //        VesselLandings = null;
        //    }

        //}

        public static event EventHandler<UploadToDbEventArg> UploadSubmissionToDB;
        public static Task<bool> UploadToDBAsync(string jsonFileName = "")
        {
            return Task.Run(() => UploadToDatabase(jsonFileName: jsonFileName));
        }

        public static List<MultiVesselGear_SampledLanding> SampledVesselLandings
        {
            get
            {
                List<MultiVesselGear_SampledLanding> sampledLandings = new List<MultiVesselGear_SampledLanding>();
                foreach (MultiVesselGear_Root root in MultiVesselLandings)
                {
                    if (root.SampledFishLandings != null)
                    {
                        sampledLandings.AddRange(root.SampledFishLandings);
                    }
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
            //SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat.SetRowIDs();
            //GridCoordGroupBingoRepeat.SetRowIDs();
            //EffortSpecSingleGear.SetRowIDs();
            //MultiGearEffortSpec.SetRowIDs();
            //VesselSamplingRepeatGear.SetRowIDs();
            //CatchCompGroupCatchCompositionRepeat.SetRowIDs();
            //CatchCompGroupCatchCompositionRepeatLenWtRepeat.SetRowIDs();
            //CatchCompGroupCatchCompositionRepeatLengthListRepeat.SetRowIDs();
            //CatchCompGroupCatchCompositionRepeatLengthFreqRepeat.SetRowIDs();
            //CatchCompGroupCatchCompositionRepeatGmsRepeatGroup.SetRowIDs();
            MultiVesselGear_Root.SetRowIDs();
            MultiVesselGear_Gear.SetRowIDs();
            MultiVesselGear_SampledLanding.SetRowIDs();
            MultiVesselGear_LandingGear.SetRowIDs();
            MultiVesselGear_GearEffortDetail.SetRowIDs();
            MultiVesselGear_FishingGrid.SetRowIDs();
            MultiVesselGear_SoakTime.SetRowIDs();
            MultiVesselGear_CatchCompositionItem.SetRowIDs();
            MultiVesselGear_CatchGMS.SetRowIDs();
            MultiVesselGear_CatchLenFreq.SetRowIDs();
            MultiVesselGear_CatchLength.SetRowIDs();
            MultiVesselGear_CatchLenWt.SetRowIDs();
        }

        public static void ResetTotalUploadCounter(bool uploadingDone = false)
        {
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

            DelayedSave = true;
            bool proceed = false;
            int savedCount = 0;
            int lss_loop_count = 0;

            UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { Intent = UploadToDBIntent.Searching });
            UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { VesselUnloadToSaveCount = SampledVesselLandings.Count, Intent = UploadToDBIntent.StartOfUpload });
            //if (LandingSiteSamplingViewModel.CurrentIDNumber == 0)
            //{
            LandingSiteSamplingViewModel.CurrentIDNumber = NSAPEntities.SummaryItemViewModel.GetLandingSiteSamplingMaxRecordNumber(fromCollection: true);
            int maxID = LandingSiteSamplingRepository.MaxRecordNumber_from_db();
            if (maxID > LandingSiteSamplingViewModel.CurrentIDNumber)
            {
                LandingSiteSamplingViewModel.CurrentIDNumber = maxID;
            }
            //LandingSiteSamplingViewModel.CurrentIDNumber = NSAPEntities.SummaryItemViewModel.GetLandingSiteSamplingMaxRecordNumber(fromCollection: true);
            //}
            foreach (MultiVesselGear_Root root in MultiVesselLandings)
            {
                lss_loop_count++;
                LandingSiteSampling lss = NSAPEntities.SummaryItemViewModel.GetLandingSiteSampling(root.SubmissionUUID);
                if (lss == null)
                {
                    if (lss == null && root.LandingSiteID != null)
                    {
                        lss = NSAPEntities.SummaryItemViewModel.GetLandingSiteSampling(ls_id: (int)root.LandingSite.LandingSiteID, fg_id: root.FishingGround.Code, sdate: root.SamplingDate);
                    }

                    if (lss == null)
                    {

                        //if we are here then it means that the current landing site sampling is not yet saved
                        lss = new LandingSiteSampling
                        {
                            PK = LandingSiteSamplingViewModel.CurrentIDNumber + 1,
                            LandingSiteID = root.LandingSite == null ? null : (int?)root.LandingSite.LandingSiteID,
                            FishingGroundID = root.RegionFishingGround.FishingGround.Code,
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
                            EnumeratorID = root.RegionEnumeratorID,
                            EnumeratorText = root.RegionEnumeratorText,
                            JSONFileName = jsonFileName,
                        };
                        if (NSAPEntities.LandingSiteSamplingViewModel.AddRecordToRepo(lss))
                        {
                            LandingSiteSamplingViewModel.CurrentIDNumber = lss.PK;
                            proceed = true;
                        }
                    }
                    else
                    {
                        proceed = true;
                    }

                    if (proceed)
                    {
                        proceed = false;
                        if (!lss.HasFishingOperation)
                        {

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
                        if (proceed)
                        {
                            GearUnload gu;
                            foreach (MultiVesselGear_Gear gear in root.GearsInLandingSite)
                            {
                                gu = lss.GearUnloadViewModel.GetGearUnloadEx(gear.GearName);
                                //if (lss.GearUnloadViewModel.GetGearUnload(gear.GearName, gear.NumberOfLandingsOfGear, gear.WeightOfCatchOfGear) == null)
                                if (gu == null)
                                {
                                    gu = new GearUnload
                                    {
                                        PK = GearUnloadViewModel.CurrentIDNumber + 1,
                                        Parent = lss,
                                        LandingSiteSamplingID = lss.PK,
                                        Boats = gear.NumberOfLandingsOfGear,
                                        Catch = gear.WeightOfCatchOfGear,
                                        Remarks = gear.GearRemarks,
                                        Sequence = gear.GearLoopCounter,
                                        GearID = gear.GearCode == "_OT" ? "" : gear.GearCode,
                                        GearUsedText = gear.GearUsedText,
                                        DelayedSave = DelayedSave
                                    };

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
                                            foreach (MultiVesselGear_SampledLanding sl in root.SampledFishLandings.Where(t => t.Main_gear_used == gu.Sequence).ToList())
                                            {
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
                                                    NumberOfSpeciesInCatchComposition = sl.NumberSpeciesInCatchComposition
                                                };
                                                if (gu.VesselUnloadViewModel.AddRecordToRepo(vu))
                                                {
                                                    VesselUnloadViewModel.CurrentIDNumber = vu.PK;



                                                    if (sl.FishingGrids?.Count > 0)
                                                    {
                                                        if (vu.FishingGroundGridViewModel == null)
                                                        {
                                                            vu.FishingGroundGridViewModel = new FishingGroundGridViewModel(isNew: true);
                                                        }
                                                        foreach (MultiVesselGear_FishingGrid mvg_fg in sl.FishingGrids)
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
                                                        foreach (MultiVesselGear_SoakTime mvg_st in sl.GearSoakTimes)
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
                                                    foreach (MultiVesselGear_LandingGear mvg_lg in sl.SampledLandingGears)
                                                    {
                                                        //MultiVesselGear_Gear mvgg = root.GearsInLandingSite.FirstOrDefault(t => t.GearLoopCounter == int.Parse(g));
                                                        VesselUnload_FishingGear vufg = new VesselUnload_FishingGear
                                                        {
                                                            GearCode = mvg_lg.GearCode == "_OT" ? "" : mvg_lg.GearCode,
                                                            GearText = mvg_lg.GearText,
                                                            Parent = vu,
                                                            RowID = VesselUnload_FishingGearViewModel.CurrentIDNumber + 1,
                                                            DelayedSave = true
                                                        };

                                                        if (vu.VesselUnload_FishingGearsViewModel.AddRecordToRepo(vufg))// && sl.CatchCompositionItems.Count > 0)
                                                        {
                                                            VesselUnload_FishingGearViewModel.CurrentIDNumber = vufg.RowID;
                                                            if (vufg.VesselUnload_Gear_Specs_ViewModel == null)
                                                            {
                                                                vufg.VesselUnload_Gear_Specs_ViewModel = new VesselUnload_Gear_Spec_ViewModel();
                                                            }
                                                            if (sl.IncludeEffort)
                                                            {
                                                                MultiVesselGear_GearEffort mvg_ge = sl.GearEfforts.FirstOrDefault(t => t.SelectedGearName == vufg.GearUsedName);
                                                                foreach (MultiVesselGear_GearEffortDetail mvg_ged in mvg_ge.GearEffortDetails)
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

                                                                    }
                                                                }
                                                            }
                                                            //VesselUnload_FishingGearViewModel.


                                                            //if (sl.IncludeCatchComp)
                                                            //{
                                                            //    vufg.VesselCatchViewModel = new VesselCatchViewModel(isNew: true);
                                                            //}
                                                            countSavedInLoop++;
                                                        }
                                                        vufg.VesselUnload_Gear_Specs_ViewModel.Dispose();
                                                    }



                                                    proceed = countSavedInLoop == gearList.Count;

                                                    if (sl.IncludeCatchComp && sl.GearCatchCompositionItems != null && sl.GearCatchCompositionItems.Count > 0)
                                                    {
                                                        foreach (var item in sl.GearCatchCompositionItems)
                                                        {
                                                            var vufg = vu.ListUnloadFishingGears.FirstOrDefault(t => t.GearUsedName == item.GearName);
                                                            if (vufg.VesselCatchViewModel == null)
                                                            {
                                                                vufg.VesselCatchViewModel = new VesselCatchViewModel(isNew: true);
                                                            }

                                                            vufg.CountItemsInCatchComposition = item.CatchCompositionItems.Count;
                                                            vufg.WeightOfCatch = item.WeightOfCatch;
                                                            vu.VesselUnload_FishingGearsViewModel.UpdateRecordInRepo(vufg, update_wt_ct: true);
                                                            //vu.CountCatchCompositionItems = sl.CatchCompositionItems.Count;
                                                            //if (vu.VesselCatchViewModel == null)
                                                            //{
                                                            //    vu.VesselCatchViewModel = new VesselCatchViewModel(isNew: true);
                                                            //}
                                                            foreach (MultiVesselGear_CatchCompositionItem mvg_cci in item.CatchCompositionItems)
                                                            {
                                                                VesselCatch vc = new VesselCatch
                                                                {
                                                                    PK = VesselCatchViewModel.CurrentIDNumber + 1,
                                                                    ParentFishingGear = vufg,
                                                                    GearCode = mvg_cci.CodeOfGearUsedForCatching == "_OT" ? "" : mvg_cci.CodeOfGearUsedForCatching,
                                                                    GearText = mvg_cci.GearUsedForCatching == null ? mvg_cci.NameOfGearUsedForCatching : "",
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
                                                                            vu.CountMaturityRows += mvg_cci.CatchGMSes.Count;
                                                                            if (vc.CatchMaturityViewModel == null)
                                                                            {
                                                                                vc.CatchMaturityViewModel = new CatchMaturityViewModel(isNew: true);
                                                                            }
                                                                            foreach (MultiVesselGear_CatchGMS mvg_cgms in mvg_cci.CatchGMSes)
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
                                                                            vu.CountLenFreqRows += mvg_cci.CatchLenFreqs.Count;
                                                                            if (vc.CatchLenFreqViewModel == null)
                                                                            {
                                                                                vc.CatchLenFreqViewModel = new CatchLenFreqViewModel(isNew: true);
                                                                            }
                                                                            foreach (MultiVesselGear_CatchLenFreq mvg_clf in mvg_cci.CatchLenFreqs)
                                                                            {
                                                                                CatchLenFreq lf = new CatchLenFreq
                                                                                {
                                                                                    Parent = vc,
                                                                                    PK = CatchLenFreqViewModel.CurrentIDNumber + 1,
                                                                                    LengthClass = mvg_clf.LengthClass,
                                                                                    Frequency = mvg_clf.Frequency,
                                                                                    DelayedSave = DelayedSave
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
                                                                            vu.CountLengthRows += mvg_cci.CatchLengths.Count;
                                                                            if (vc.CatchLengthViewModel == null)
                                                                            {
                                                                                vc.CatchLengthViewModel = new CatchLengthViewModel(isNew: true);
                                                                            }
                                                                            foreach (MultiVesselGear_CatchLength mv_cl in mvg_cci.CatchLengths)
                                                                            {
                                                                                CatchLength len = new CatchLength
                                                                                {
                                                                                    Parent = vc,
                                                                                    PK = CatchLengthViewModel.CurrentIDNumber + 1,
                                                                                    Length = mv_cl.Length,
                                                                                    DelayedSave = DelayedSave
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
                                                                            vu.CountLenWtRows += mvg_cci.CatchLengthWeights.Count;
                                                                            if (vc.CatchLengthWeightViewModel == null)
                                                                            {
                                                                                vc.CatchLengthWeightViewModel = new CatchLengthWeightViewModel(isNew: true);
                                                                            }
                                                                            foreach (MultiVesselGear_CatchLenWt mvg_clw in mvg_cci.CatchLengthWeights)
                                                                            {
                                                                                CatchLengthWeight lw = new CatchLengthWeight
                                                                                {
                                                                                    Parent = vc,
                                                                                    PK = CatchLengthWeightViewModel.CurrentIDNumber + 1,
                                                                                    Length = mvg_clw.Length,
                                                                                    Weight = mvg_clw.Weight,
                                                                                    DelayedSave = DelayedSave
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
                                                            vufg.VesselCatchViewModel.Dispose();
                                                            //vu.VesselCatchViewModel.Dispose();

                                                        }
                                                    }
                                                    //vu.VesselUnload_FishingGearsViewModel.Dispose();


                                                    if (gu.VesselUnloadViewModel.UpdateUnloadStats(vu) && NSAPEntities.SummaryItemViewModel.AddRecordToRepo(vu))
                                                    {
                                                        if (sl.IncludeCatchComp)
                                                        {
                                                            gu.VesselUnloadViewModel.UpdateWeightValidation(NSAPEntities.SummaryItemViewModel.CurrentEntity, vu);
                                                            //vu.VesselCatchViewModel.Dispose();
                                                            vu.VesselUnload_FishingGearsViewModel.Dispose();
                                                        }
                                                        savedCount++;
                                                        sl.SavedInLocalDatabase = true;
                                                        UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { VesselUnloadSavedCount = savedCount, Intent = UploadToDBIntent.Uploading });
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

                                    }
                                }


                            }


                        }
                        else if (lss.NumberOfGearTypesInLandingSite != lss.GearUnloadViewModel.Count)
                        {

                        }
                    }
                }
            }

            return savedCount > 0;
        }
        public static DateTime? JSONFileCreationTime { get; set; }
        public static bool CancelUpload { get; set; }
        public static bool DelayedSave { get; set; }
        public static string JSON { get; set; }
        public static List<MultiVesselGear_Root> MultiVesselLandings { get; internal set; }
        public static List<CountLandings> ListOfLandingsCount { get; internal set; }
        public static void CreateLandingCountsFromJSON()
        {
            try
            {
                ListOfLandingsCount = JsonConvert.DeserializeObject<List<CountLandings>>(JSON);
            }
            catch (Exception ex)
            {
                Utilities.Logger.Log(ex);
            }
        }
        public static void CreateLandingsFromJSON()
        {
            //How are PKs assigned to each landings contained in each incoming batch of JSON?
            //call VesselLanding.SetRowIDs()

            MultiVesselGear_SampledLanding.SetRowIDs();
            try
            {
                MultiVesselLandings = JsonConvert.DeserializeObject<List<MultiVesselGear_Root>>(JSON);
            }
            catch (Exception ex)
            {
                Utilities.Logger.Log(ex);
            }
        }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);

    public class MultiVesselGear_LandingGear
    {
        private MultiVesselGear_SampledLanding _parent;
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
        public MultiVesselGear_SampledLanding Parent
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
    public class MultiVesselGear_Gear
    {
        private MultiVesselGear_Root _parent;
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

            //if (NSAPEntities.FishingGroundGridViewModel.FishingGroundGridCollection.Count == 0)
            //{
            //    _pk = 0;
            //}
            //else
            //{
            //    _pk = NSAPEntities.FishingGroundGridViewModel.NextRecordNumber - 1;
            //}

            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastGearUnloadPK;
            GearUnloadViewModel.CurrentIDNumber = _pk;
            //}
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

        public MultiVesselGear_Root Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        [JsonProperty("repeat_gears/count_loop")]
        public int GearLoopCounter { get; set; }

        //[JsonProperty("repeat_gears/group_gear/position_indicator")]
        //public string repeat_gearsgroup_gearposition_indicator { get; set; }

        [JsonProperty("repeat_gears/group_gear/select_gear")]
        public string Will_select_gear { get; set; }

        public bool WillSelectGear
        {
            get
            {
                return Will_select_gear == "yes";
            }
        }

        [JsonProperty("repeat_gears/group_gear/gear_used")]
        public string GearSelected { get; set; }

        [JsonProperty("repeat_gears/group_gear/gear_used_text")]
        public string GearUsedText { get; set; }

        [JsonProperty("repeat_gears/group_gear/know_landing_count_gear")]
        public string CountOfLandingsOfGearKnown { get; set; }

        public bool IsCountOfLandingsOfGearKnown { get { return CountOfLandingsOfGearKnown == "yes"; } }

        [JsonProperty("repeat_gears/group_gear/count_landings_of_gear")]
        public int? NumberOfLandingsOfGear { get; set; }

        [JsonProperty("repeat_gears/group_gear/catch_of_gear")]
        public double? WeightOfCatchOfGear { get; set; }

        [JsonProperty("repeat_gears/group_gear/repeat_gear_code")]
        public string GearCode { get; set; }

        [JsonProperty("repeat_gears/group_gear/repeat_gear_name")]
        public string GearName { get; set; }

        [JsonProperty("repeat_gears/group_gear/gear_remarks")]
        public string GearRemarks { get; set; }

        //[JsonProperty("repeat_gears/group_gear/repeat_position")]
        //public string repeat_gearsgroup_gearrepeat_position { get; set; }

        //[JsonProperty("repeat_gears/group_gear/choices_so_far")]
        //public string repeat_gearsgroup_gearchoices_so_far { get; set; }
    }

    public class MultiVesselGear_SampledLanding
    {
        private bool? _isSaved;
        private static int _pk;
        private int _rowid;
        private MultiVesselGear_Root _parent;
        private List<MultiVesselGear_GearEffort> _gearEfforts;
        private List<MultiVesselGear_LandingGear> _landingGears;
        private List<MultiVesselGear_CatchCompositionItem> _catchCompositionItems;
        private List<MultiVesselGear_GearCatchComposition> _gearCatchCompositionItems;

        private List<MultiVesselGear_FishingGrid> _fishingGrids;

        private List<MultiVesselGear_SoakTime> _gearSoakTimes;
        private readonly object collectionLock = new object();
        private SummaryItem _savedVesselUnloadObject;
        private string _gearsUsedInSampledLanding;


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
                            //_savedVesselUnloadObject = SavedVesselUnloadObject;
                            //_rowid = _savedVesselUnloadObject.VesselUnloadID;
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
            if (NSAPEntities.SummaryItemViewModel.Count == 0)
            {
                _pk = 0;
            }
            else
            {
                //NSAPEntities.SummaryItemViewModel.RefreshLastPrimaryLeys(MultiVesselGear_UnloadServerRepository.DelayedSave);
                //_pk = NSAPEntities.SummaryItemViewModel.GetNextRecordNumber() - 1;
                //_pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastVesselUnloadPK;
                _pk = NSAPEntities.SummaryItemViewModel.GetVesselUnloadMaxRecordNumber();
                VesselUnloadViewModel.CurrentIDNumber = _pk;
            }
            RowIDSet = true;
        }
        public MultiVesselGear_Root Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        [JsonProperty("repeat_landings/group_sampled_landing/efforts_group/repeat_gear_effort")]
        public List<MultiVesselGear_GearEffort> GearEfforts
        {
            get { return _gearEfforts; }
            set
            {
                _gearEfforts = value;
                foreach (var item in _gearEfforts)
                {
                    item.Parent = this;
                }
            }
        }
        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition")]
        public List<MultiVesselGear_GearCatchComposition> GearCatchCompositionItems
        {
            get { return _gearCatchCompositionItems; }
            set
            {
                _gearCatchCompositionItems = value;
                foreach (var item in _gearCatchCompositionItems)
                {
                    item.Parent = this;
                }
            }
        }
        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat")]
        //public List<MultiVesselGear_CatchCompositionItem> CatchCompositionItems
        //{
        //    get { return _catchCompositionItems; }
        //    set
        //    {
        //        _catchCompositionItems = value;
        //        foreach (var item in _catchCompositionItems)
        //        {
        //            item.Parent = this;
        //        }
        //    }
        //}

        [JsonProperty("repeat_landings/group_sampled_landing/grid_coord_group/bingo_repeat")]
        public List<MultiVesselGear_FishingGrid> FishingGrids
        {
            get { return _fishingGrids; }
            set
            {
                _fishingGrids = value;
                foreach (var item in _fishingGrids)
                {
                    item.Parent = this;
                }
            }
        }

        [JsonProperty("repeat_landings/group_sampled_landing/soak_time_group/soaktime_tracking_group/soak_time_repeat")]
        public List<MultiVesselGear_SoakTime> GearSoakTimes
        {
            get { return _gearSoakTimes; }
            set
            {
                _gearSoakTimes = value;
                foreach (var item in _gearSoakTimes)
                {
                    item.Parent = this;
                }
            }
        }

        [JsonProperty("repeat_landings/landing_count_loop")]
        public int LandingSequence { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/group_landing/landing_position_indicator")]
        //public string repeat_landingsgroup_sampled_landinggroup_landinglanding_position_indicator { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/group_landing/sampling_time")]
        public DateTime TimeOfSampling { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/group_landing/is_boat_used")]
        public string BoatIsUsedInLanding { get; set; }
        public bool IsBoatUsedInLanding { get { return BoatIsUsedInLanding == "yes"; } set { BoatIsUsedInLanding = ""; } }

        [JsonProperty("repeat_landings/group_sampled_landing/group_landing/select_vessel")]
        public string VesselNameSelectedFromList { get; set; }

        public bool IsVesselNameSelectedFromList { get { return VesselNameSelectedFromList == "yes"; } set { VesselNameSelectedFromList = ""; } }

        [JsonProperty("repeat_landings/group_sampled_landing/group_landing/fish_sector")]
        public string SectorOfLanding { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/group_landing/vessel_csv_source")]
        //public string repeat_landingsgroup_sampled_landinggroup_landingvessel_csv_source { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/group_landing/search_mode")]
        //public string repeat_landingsgroup_sampled_landinggroup_landingsearch_mode { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/group_landing/search_column")]
        //public string repeat_landingsgroup_sampled_landinggroup_landingsearch_column { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/group_landing/search_value")]
        //public string repeat_landingsgroup_sampled_landinggroup_landingsearch_value { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/group_landing/boat_used")]
        public int? BoatUsedID { get; set; }

        public FishingVessel FishingVessel
        {
            get
            {
                if (BoatUsedID == null)
                {
                    return null;
                }
                else
                {
                    return NSAPEntities.FishingVesselViewModel.GetFishingVessel((int)BoatUsedID);
                }
            }
        }

        [JsonProperty("repeat_landings/group_sampled_landing/group_landing/boat_used_text")]
        public string BoatUsedText { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/group_landing/number_of_fishers")]
        public int? NumberOfFishers { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/group_landing/landing_gears_used")]
        public string GearsUsedInSampledLanding
        {
            get { return _gearsUsedInSampledLanding; }
            set
            {
                _gearsUsedInSampledLanding = value;
            }
        }
        public List<MultiVesselGear_LandingGear> SampledLandingGears
        {
            get
            {
                if (_landingGears == null)
                {
                    _landingGears = new List<MultiVesselGear_LandingGear>();
                    var gears = _gearsUsedInSampledLanding.Split(' ');
                    foreach (string g in gears)
                    {

                        var gg = _parent.GearsInLandingSite.FirstOrDefault(t => t.GearLoopCounter == int.Parse(g));
                        MultiVesselGear_LandingGear lg = new MultiVesselGear_LandingGear
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
        }

        public string NamesOfGearsUsed
        {
            get
            {
                var gears = "";
                foreach (MultiVesselGear_LandingGear lg in SampledLandingGears)
                {
                    gears += $"{lg.GearName}, ";
                }
                return gears.Trim(',', ' ');
            }
        }
        public int NumberOfGearsUsedInSampledLanding
        {
            get
            {
                return GearsUsedInSampledLanding.Split(' ').Length;
            }
        }

        [JsonProperty("repeat_landings/group_sampled_landing/group_landing/main_gear_used")]
        public int Main_gear_used { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/gear_name")]
        public string Main_gear_name { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/gear_code")]
        public string Main_gear_code { get; set; }

        public Gear MainGear
        {
            get
            {
                if (string.IsNullOrEmpty(Main_gear_code) || Main_gear_code == "_OT")
                {
                    return null;
                }
                else
                {
                    return NSAPEntities.GearViewModel.GetGear(Main_gear_code);
                }
            }
        }

        [JsonProperty("repeat_landings/group_sampled_landing/boat_name")]
        public string Boat_name { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/vessel_catch/trip_isSuccess")]
        public string Trip_isSuccess { get; set; }

        public bool TripIsSuccess { get { return Trip_isSuccess == "yes"; } set { Trip_isSuccess = ""; } }

        [JsonProperty("repeat_landings/group_sampled_landing/vessel_catch/trip_isCompleted")]
        public string Trip_isCompleted { get; set; }

        public bool TripIsCompleted { get { return Trip_isCompleted == "yes"; } set { Trip_isCompleted = ""; } }

        //[JsonProperty("repeat_landings/group_sampled_landing/vessel_catch/remarks_not_completed")]
        //public string Remarks_not_completed { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/vessel_catch/remarks")]
        public string Remarks { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/vessel_catch/remarks_normal_operation")]
        //public string Remarks_normal_operation { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/vessel_catch/catch_total")]
        public double? CatchTotal { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/vessel_catch/is_catch_sampled")]
        public string Is_Catch_Sampled { get; set; }

        public bool IsCatchSampled { get { return Is_Catch_Sampled == "yes"; } }

        [JsonProperty("repeat_landings/group_sampled_landing/vessel_catch/catch_sampled")]
        public double? CatchSampled { get; set; }
        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/select_length_group/catch_composition_items_count")]
        public int? NumberSpeciesInCatchComposition { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/vessel_catch/is_region_total_enumeration")]
        public string Is_region_total_enumeration { get; set; }

        public bool IsRegionTotalEnumeration { get { return Is_region_total_enumeration == "yes"; } }

        [JsonProperty("repeat_landings/group_sampled_landing/vessel_catch/is_catch_sold")]
        public string Is_catch_sold { get; set; }

        public bool IsCatchSold { get { return Is_catch_sold == "yes"; } }

        //[JsonProperty("repeat_landings/group_sampled_landing/vessel_catch/sample_wt_text")]
        //public string repeat_landingsgroup_sampled_landingvessel_catchsample_wt_text { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/grid_coord_group/include_bingo")]
        public string Include_bingo { get; set; }

        public bool IncludeBingo { get { return Include_bingo == "yes"; } }

        //[JsonProperty("repeat_landings/group_sampled_landing/grid_coord_group/majorgrid_csv")]
        //public string repeat_landingsgroup_sampled_landinggrid_coord_groupmajorgrid_csv { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/grid_coord_group/inlandgrid_csv")]
        //public string repeat_landingsgroup_sampled_landinggrid_coord_groupinlandgrid_csv { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/soak_time_group/include_soak_time")]
        public string Include_soak_time { get; set; }

        public bool IncludeSoakTime { get { return Include_soak_time == "yes"; } }

        [JsonProperty("repeat_landings/group_sampled_landing/efforts_group/include_effort")]
        public string Include_effort { get; set; }
        public bool IncludeEffort { get { return Include_effort == "yes"; } }

        //[JsonProperty("repeat_landings/group_sampled_landing/efforts_group/outside_effort_repeat")]
        //public string repeat_landingsgroup_sampled_landingefforts_groupoutside_effort_repeat { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/efforts_group/repeat_gear_effort_count")]
        //public string repeat_landingsgroup_sampled_landingefforts_grouprepeat_gear_effort_count { get; set; }



        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/select_length_group/include_catchcomp_g")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupselect_length_groupinclude_catchcomp_g { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/select_length_group/length_type_g")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupselect_length_grouplength_type_g { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/select_length_group/has_gms_g")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupselect_length_grouphas_gms_g { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/count_catch_comp")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcount_catch_comp { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/include_catchcomp")]
        public string Include_catchcomp { get; set; }

        public bool IncludeCatchComp { get { return Include_catchcomp == "yes"; } set { Include_catchcomp = ""; } }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/length_type")]
        public string Length_type { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/has_gms")]
        public string Has_gms { get; set; }

        public bool HasGMS { get { return Has_gms == "yes"; } }


        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/count_species_comp")]
        public int? Count_species_comp { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/sum_total")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupsum_total { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/raising_factor")]
        public double? RaisingFactor { get; set; }
        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/raising_factor_text")]

        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupraising_factor_text { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/sum_sample")]
        public double? Sum_sample { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/sum_species_weight")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupsum_species_weight { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/sum_species_weight_coalesce")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupsum_species_weight_coalesce { get; set; }


        [JsonProperty("repeat_landings/group_sampled_landing/group_final_tally/ref_no")] //version 7.12
        public string Reference_number_12 { get; set; }


        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/group_final_tally/ref_no")] //version 7.11
        public string Reference_number_11 { get; set; }

        public string Reference_number
        {
            get
            {
                if (Reference_number_11 == null)
                {
                    return Reference_number_12;
                }
                else
                {
                    return Reference_number_11;
                }
            }
        }

        [JsonProperty("repeat_landings/group_sampled_landing/vessel_catch/boxes_total")]
        public int? Boxes_total { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/vessel_catch/boxes_sampled")]
        public int? Boxes_sampled { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/grid_coord_group/utmZone")]
        public string UTMZone { get; set; }

    }
    public class MultiVesselGear_GearCatchComposition
    {
        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/selected_gear_catch_comp")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_grouprepeat_gear_catch_compositionselected_gear_catch_comp { get; set; }
        private MultiVesselGear_SampledLanding _parent;
        private List<MultiVesselGear_CatchCompositionItem> _catchCompositionItems;
        public MultiVesselGear_SampledLanding Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/selected_gear_catch_comp_name")]
        public string GearName { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/gear_code_catch_comp")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_grouprepeat_gear_catch_compositiongear_code_catch_comp { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/selected_gear__catch_comp_code")]
        public string GearCode { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/weight_catch_of_gear")]
        public double? WeightOfCatch { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_items_count")]
        public int? NumberOfSpeciesInCatchComposition { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat_count")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_grouprepeat_gear_catch_compositioncatch_composition_repeat_count { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat")]
        public List<MultiVesselGear_CatchCompositionItem> CatchCompositionItems
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

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/count_species_comp")]
        public int? CountOfSpeciesComposition { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/sum_total")]
        public double? SumTotalWeight { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/raising_factor_text")]
        public string RaisingFactorText { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/sum_sample")]
        public double? SumSampleWeight { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/sum_species_weight")]
        public double? SumSpeciesWeight { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/sum_species_weight_coalesce")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_grouprepeat_gear_catch_compositionsum_species_weight_coalesce { get; set; }
    }
    public class MultiVesselGear_CatchLenFreq
    {
        private bool? _isSaved;
        private static int _pk;
        private int _rowID;
        private MultiVesselGear_CatchCompositionItem _parent;
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

        public MultiVesselGear_CatchCompositionItem Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/length_freq_repeat/group_LF/length_class")]
        public double LengthClass { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/length_freq_repeat/group_LF/freq")]
        public int Frequency { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/length_freq_repeat/group_LF/lf_grp_title")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_grouplength_freq_repeatgroup_LFlf_grp_title { get; set; }
    }

    public class MultiVesselGear_CatchLenWt
    {
        private MultiVesselGear_CatchCompositionItem _parent;
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
        public MultiVesselGear_CatchCompositionItem Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/len_wt_repeat/len_wt_group/len_lenwt")]
        public double Length { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/len_wt_repeat/len_wt_group/wt_lenwt")]
        public double Weight { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/len_wt_repeat/len_wt_group/sex_lw")]
        public string Sex { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/len_wt_repeat/len_wt_group/lw_grp_title")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_grouplen_wt_repeatlen_wt_grouplw_grp_title { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/len_wt_repeat/len_wt_group/wt_lenwt_kg")]
        public double Weight_kg { get; set; }
    }

    public class MultiVesselGear_CatchCompositionItem
    {
        //private MultiVesselGear_SampledLanding _parent;
        private MultiVesselGear_GearCatchComposition _parent;
        private List<MultiVesselGear_CatchGMS> _catchGMSes;
        private List<MultiVesselGear_CatchLength> _catchLengths;
        private List<MultiVesselGear_CatchLenWt> _catchLengthWeights;
        private List<MultiVesselGear_CatchLenFreq> _catchLenFreqs;
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
        public MultiVesselGear_GearCatchComposition Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/gms_repeat_group")]
        public List<MultiVesselGear_CatchGMS> CatchGMSes
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
        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/length_list_repeat")]
        public List<MultiVesselGear_CatchLength> CatchLengths
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

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/len_wt_repeat")]
        public List<MultiVesselGear_CatchLenWt> CatchLengthWeights
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

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/length_freq_repeat")]
        public List<MultiVesselGear_CatchLenFreq> CatchLenFreqs
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

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/show_taxa_image")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_groupspecies_data_groupshow_taxa_image { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/taxa_no_im")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_groupspecies_data_grouptaxa_no_im { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/taxa")]
        public string SelectedTaxa { get; set; }

        public Taxa Taxa { get { return NSAPEntities.TaxaViewModel.GetTaxa(SelectedTaxa); } }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/select_spName")]

        public string SelectSpeciesNameFromList { get; set; }

        public bool IsSelectSpeciesNameFromList { get { return SelectSpeciesNameFromList == "yes"; } }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/species_csv_source")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_groupspecies_data_groupspecies_csv_source { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/search_species")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_groupspecies_data_groupsearch_species { get; set; }


        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/spName_other")]
        public string SpeciesNameOther { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/species")]
        public int? SelectedFishSpeciesID { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/species_notfish")]
        public int? SelectedNonFishSpeciesID { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/gear_species")]
        //public int gear_species { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/gear_species_name")]
        public string NameOfGearUsedForCatching { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/gear_species_code")]
        //public string gear_species_code { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/gear_species_code_final")]
        public string CodeOfGearUsedForCatching { get; set; }

        public Gear GearUsedForCatching
        {
            get
            {
                if (CodeOfGearUsedForCatching == "_OT")
                {
                    return null;
                }
                else
                {
                    return NSAPEntities.GearViewModel.GetGear(CodeOfGearUsedForCatching);
                }
            }
        }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/sp_id")]
        public int? SpeciesID { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/len_max_1")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_groupspecies_data_grouplen_max_1 { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/len_max")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_groupspecies_data_grouplen_max { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/species_name_selected")]
        //this will 
        public string SpeciesNameSelected { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/size_type_name")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_groupspecies_data_groupsize_type_name { get; set; }


        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/from_total_catch")]
        public string From_total_catch { get; set; }

        public bool FromTotalCatch { get { return From_total_catch == "yes"; } }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/species_sample_wt_sampled")]
        public double? WeightOfSpeciesSampled { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/species_wt_1")]
        public double? WeightOfSpecies { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/species_wt")]
        public double? Species_wt { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/species_wt_rounded")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_groupspecies_data_groupspecies_wt_rounded { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/measure_len_and_gms")]
        public string Measure_len_and_gms { get; set; }

        public bool MeasureLengthAndGMS { get { return Measure_len_and_gms == "yes"; } }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/has_measurement")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_groupspecies_data_grouphas_measurement { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/what_is_measured")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_groupspecies_data_groupwhat_is_measured { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/species_sample_wt_sampled")]
        //public double? WeightOfSampleOfSpeciesFromSample { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/species_sample_wt_total")]
        public double? WeightOfSampleOfSpeciesFromTotal { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/max_len_hint")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_groupspecies_data_groupmax_len_hint { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/enforce_maxlen")]
        public string Enforce_maxlen { get; set; }

        public bool EnforceMaxLen { get { return Enforce_maxlen == "yes"; } }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/species_sample_wt")]
        public double? Species_sample_wt { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/species_sample_wt_from_sample")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_groupspecies_data_groupspecies_sample_wt_from_sample { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/from_total_catch_code")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_groupspecies_data_groupfrom_total_catch_code { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/include_sex_for_length")]
        public string Include_sex_when_measuring_length { get; set; }



        public bool IncludeSexWhenMeasuringLength { get { return Include_sex_when_measuring_length == "yes"; } }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/len_max_hint")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_groupspecies_data_grouplen_max_hint { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/wt_unit_name")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_groupspecies_data_groupwt_unit_name { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/max_size_hint")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_groupspecies_data_groupmax_size_hint { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/species_wt_check")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_groupspecies_data_groupspecies_wt_check { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/overwt_prompt")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_groupspecies_data_groupoverwt_prompt { get; set; }



        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/sum_wt_from_lenwt")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_groupsum_wt_from_lenwt { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/sum_wt_from_lenwt_coalesce")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_groupsum_wt_from_lenwt_coalesce { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/sum_wt_from_gms")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_groupsum_wt_from_gms { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/sum_wt_from_gms_coalesce")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_groupsum_wt_from_gms_coalesce { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/repeat_title")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_grouprepeat_title { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/is_species_sold")]
        public string Is_species_sold { get; set; }

        public bool IsSpeciesSold { get { return Is_species_sold == "yes"; } }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/price_of_species")]
        public double? Price_of_species { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/pricing_unit")]
        public string Pricing_unit { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/other_pricing_unit")]
        public string OtherPricingUnit { get; set; }
        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/type_of_measure")]
        public string Type_of_measure { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/species_data_group/individual_wt_unit")]
        public string Individual_wt_unit { get; set; }


    }

    public class MultiVesselGear_CatchGMS
    {
        private MultiVesselGear_CatchCompositionItem _parent;
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
        public MultiVesselGear_CatchCompositionItem Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/gms_repeat_group/gms_group/individual_length")]
        public double? Individual_length { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/gms_repeat_group/gms_group/individual_weight")]
        public double? Individual_weight { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/gms_repeat_group/gms_group/stomach_content_wt")]
        public double? Stomach_content_weight { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/gms_repeat_group/gms_group/sex")]
        public string Sex { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/gms_repeat_group/gms_group/gms_repeat")]
        public string GMS { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/gms_repeat_group/gms_group/gonad_wt")]
        public double? GonadWt { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/gms_repeat_group/gms_group/gut_content_category")]
        public string GutContentCategory { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/gms_repeat_group/gms_group/combined_gms_fields")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_groupgms_repeat_groupgms_groupcombined_gms_fields { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/gms_repeat_group/gms_group/individual_weight_kg")]
        public double? Individual_weight_kg { get; set; }
    }

    public class MultiVesselGear_CatchLength
    {
        private MultiVesselGear_CatchCompositionItem _parent;
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
        public MultiVesselGear_CatchCompositionItem Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        [JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/length_list_repeat/length_list_group/length")]
        public double Length { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/catch_comp_group/repeat_gear_catch_composition/catch_composition_repeat/speciesname_group/length_list_repeat/length_list_group/ll_grp_title")]
        //public string repeat_landingsgroup_sampled_landingcatch_comp_groupcatch_composition_repeatspeciesname_grouplength_list_repeatlength_list_groupll_grp_title { get; set; }
    }

    public class MultiVesselGear_GearEffort
    {
        private MultiVesselGear_SampledLanding _parent;
        public MultiVesselGear_SampledLanding Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        //[JsonProperty("repeat_landings/group_sampled_landing/efforts_group/repeat_gear_effort/selected_gear_effort")]
        //public string repeat_landingsgroup_sampled_landingefforts_grouprepeat_gear_effortselected_gear_effort { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/efforts_group/repeat_gear_effort/selected_gear_name")]
        public string SelectedGearName { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/efforts_group/repeat_gear_effort/gear_code1")]
        //public string repeat_landingsgroup_sampled_landingefforts_grouprepeat_gear_effortgear_code1 { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/efforts_group/repeat_gear_effort/selected_gear_code")]
        public string SelectedGearCode { get; set; }

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

        //[JsonProperty("repeat_landings/group_sampled_landing/efforts_group/repeat_gear_effort/choices_effort")]
        //public string repeat_landingsgroup_sampled_landingefforts_grouprepeat_gear_effortchoices_effort { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/efforts_group/repeat_gear_effort/effort_group_inside/outside_gear_effort_repeat")]
        //public string repeat_landingsgroup_sampled_landingefforts_grouprepeat_gear_efforteffort_group_insideoutside_gear_effort_repeat { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/efforts_group/repeat_gear_effort/effort_group_inside/effort_repeat")]
        public List<MultiVesselGear_GearEffortDetail> GearEffortDetails { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/efforts_group/repeat_gear_effort/effort_group_inside/count_effort_specs")]
        //public string repeat_landingsgroup_sampled_landingefforts_grouprepeat_gear_efforteffort_group_insidecount_effort_specs { get; set; }
    }

    public class MultiVesselGear_GearEffortDetail
    {
        private MultiVesselGear_GearEffort _parent;
        private static int _pk;
        private int _rowID;
        //private MultiGearEffortSpecContainer _parent;
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
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastVesselUnloadGearSpecPK;
            VesselUnload_Gear_Spec_ViewModel.CurrentIDNumber = _pk;
            //VesselEffortViewModel.CurrentIDNumber = _pk;
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
        public MultiVesselGear_GearEffort Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        [JsonProperty("repeat_landings/group_sampled_landing/efforts_group/repeat_gear_effort/effort_group_inside/effort_repeat/group_effort/effort_type")]
        public int EffortType { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/efforts_group/repeat_gear_effort/effort_group_inside/effort_repeat/group_effort/response_type")]
        public string ResponseType { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/efforts_group/repeat_gear_effort/effort_group_inside/effort_repeat/group_effort/effort_spec_name")]
        public string EffortSpecName { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/efforts_group/repeat_gear_effort/effort_group_inside/effort_repeat/group_effort/effort_intensity")]
        public double? EffortIntensity { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/efforts_group/repeat_gear_effort/effort_group_inside/effort_repeat/group_effort/selected_effort_measure")]
        public string SelectedEffortMeasure { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/efforts_group/repeat_gear_effort/effort_group_inside/effort_repeat/group_effort/choices_gear_effort")]
        //public string repeat_landingsgroup_sampled_landingefforts_grouprepeat_gear_efforteffort_group_insideeffort_repeatgroup_effortchoices_gear_effort { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/efforts_group/repeat_gear_effort/effort_group_inside/effort_repeat/group_effort/effort_desc")]
        public string EffortText { get; set; }
    }

    public class MultiVesselGear_FishingGrid
    {
        private static int _pk;
        private int _rowID;
        private MultiVesselGear_SampledLanding _parent;
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

        public MultiVesselGear_SampledLanding Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        [JsonProperty("repeat_landings/group_sampled_landing/grid_coord_group/bingo_repeat/bingo_group/major_grid")]
        public string MajorGrid { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/grid_coord_group/bingo_repeat/bingo_group/col_name")]
        public string ColumnName { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/grid_coord_group/bingo_repeat/bingo_group/row_name")]
        public string RowName { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/grid_coord_group/bingo_repeat/bingo_group/bingo_complete")]
        public string GridNameComplete { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/grid_coord_group/bingo_repeat/bingo_group/is_inland")]
        public string IsInland { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/grid_coord_group/bingo_repeat/bingo_group/group_label")]
        //public string repeat_landingsgroup_sampled_landinggrid_coord_groupbingo_repeatbingo_groupgroup_label { get; set; }
    }

    public class MultiVesselGear_SoakTime
    {
        private MultiVesselGear_SampledLanding _parent;
        private static int _pk;
        private int _rowID;
        public MultiVesselGear_SampledLanding Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        //[JsonProperty("repeat_landings/group_sampled_landing/soak_time_group/soaktime_tracking_group/soak_time_repeat/calculate_set_hint")]
        //public string repeat_landingsgroup_sampled_landingsoak_time_groupsoaktime_tracking_groupsoak_time_repeatcalculate_set_hint { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/soak_time_group/soaktime_tracking_group/soak_time_repeat/set_time")]
        public DateTime? TimeOfSet { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/soak_time_group/soaktime_tracking_group/soak_time_repeat/decimal_set_time")]
        //public string repeat_landingsgroup_sampled_landingsoak_time_groupsoaktime_tracking_groupsoak_time_repeatdecimal_set_time { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/soak_time_group/soaktime_tracking_group/soak_time_repeat/time_set_string")]
        //public string repeat_landingsgroup_sampled_landingsoak_time_groupsoaktime_tracking_groupsoak_time_repeattime_set_string { get; set; }

        //[JsonProperty("repeat_landings/group_sampled_landing/soak_time_group/soaktime_tracking_group/soak_time_repeat/calculate_haul_hint")]
        //public string repeat_landingsgroup_sampled_landingsoak_time_groupsoaktime_tracking_groupsoak_time_repeatcalculate_haul_hint { get; set; }

        [JsonProperty("repeat_landings/group_sampled_landing/soak_time_group/soaktime_tracking_group/soak_time_repeat/haul_time")]
        public DateTime? TimeOfHaul { get; set; }
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
    }


    public class MultiVesselGear_Root
    {
        private List<MultiVesselGear_Gear> _gearsInLandingSite;
        private List<MultiVesselGear_SampledLanding> _sampledFishLandings;
        private static int _pk;
        private int _rowid;
        private bool? _isSaved;

        [JsonProperty("repeat_gears")]
        public List<MultiVesselGear_Gear> GearsInLandingSite
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
        [JsonProperty("repeat_landings")]
        public List<MultiVesselGear_SampledLanding> SampledFishLandings
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
        public int _id { get; set; }

        //[JsonProperty("formhub/uuid")]
        //public string formhubuuid { get; set; }
        public DateTime start { get; set; }
        public string today { get; set; }
        public string device_id { get; set; }
        public string user_name { get; set; }
        public string intronote { get; set; }

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

        [JsonProperty("landing_site_sampling_group/sampling_date")]
        public DateTime SamplingDate { get; set; }

        [JsonProperty("landing_site_sampling_group/nsap_region")]
        public string NSAP_Region { get; set; }

        public NSAPRegion NSAPRegion { get { return NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(NSAP_Region); } }

        [JsonProperty("landing_site_sampling_group/select_enumerator")]
        public string SelectEnumerator { get; set; }

        public bool IsSelectEnumerator { get { return SelectEnumerator == "yes"; } }

        [JsonProperty("landing_site_sampling_group/region_enumerator")]
        public int? RegionEnumeratorID { get; set; }

        [JsonProperty("landing_site_sampling_group/region_enumerator_text")]
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
                    return NSAPEntities.NSAPEnumeratorViewModel.GetNSAPEnumerator((int)RegionEnumeratorID);
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
                    return NSAPEnumerator.Name;
                }
            }
        }

        [JsonProperty("landing_site_sampling_group/fma_in_region")]
        public int FMAinRegionID { get; set; }
        public FMA FMA
        {
            get { return NSAPEntities.FMAViewModel.GetFMA(fma_number); }
        }

        public NSAPRegionFMA NSAPRegionFMA
        {
            get
            {
                return NSAPRegion.FMAs.Where(t => t.RowID == FMAinRegionID).FirstOrDefault();
            }
        }
        public FishingGround FishingGround
        {
            get
            {
                return RegionFishingGround?.FishingGround;
            }
        }
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
                            //_savedVesselUnloadObject = SavedVesselUnloadObject;
                            //_rowid = _savedVesselUnloadObject.VesselUnloadID;
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
            if (NSAPEntities.SummaryItemViewModel.Count == 0)
            {
                _pk = 0;
            }
            else
            {
                //NSAPEntities.SummaryItemViewModel.RefreshLastPrimaryLeys(MultiVesselGear_UnloadServerRepository.DelayedSave);
                //_pk = NSAPEntities.SummaryItemViewModel.GetNextRecordNumber() - 1;
                _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastLandingSiteSamplingPK;
                LandingSiteSamplingViewModel.CurrentIDNumber = _pk;

            }
            RowIDSet = true;
        }
        private SummaryItem _savedVesselUnloadObject;
        private readonly object collectionLock = new object();
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
        public NSAPRegionFMAFishingGround RegionFishingGround
        {
            get
            {
                return NSAPRegionFMA.FishingGrounds.FirstOrDefault(t => t.RowID == RegionFishingGroundID);
            }
        }
        [JsonProperty("landing_site_sampling_group/fishing_ground")]
        public int RegionFishingGroundID { get; set; }

        [JsonProperty("landing_site_sampling_group/select_landingsite")]
        public string SelectLandingSite { get; set; }
        public bool IsSelectLandingSite { get { return SelectLandingSite == "yes"; } }
        [JsonProperty("landing_site_sampling_group/landing_site")]
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

        [JsonProperty("landing_site_sampling_group/is_sampling_day")]
        public string Is_sampling_day { get; set; }

        public bool IsSamplingDay { get { return Is_sampling_day == "yes"; } }

        [JsonProperty("landing_site_sampling_group/are_there_landings")]
        public string Are_there_landings { get; set; }

        public bool AreThereLandings { get { return Are_there_landings == "yes"; } }

        [JsonProperty("landing_site_sampling_group/count_sampled_landings")]
        public int? CountSampledLandings { get; set; }

        [JsonProperty("landing_site_sampling_group/fishing_gear_type_count")]
        public int? FishingGearTypeCount { get; set; }

        [JsonProperty("landing_site_sampling_group/know_total_landing_count")]
        public string Know_total_landing_count { get; set; }

        public bool KnowTotalLandingCount { get { return Know_total_landing_count == "yes"; } }

        [JsonProperty("landing_site_sampling_group/count_total_landing")]
        public int? CountTotalLanding { get; set; }
        public int fma_number { get; set; }
        public string fishing_ground_name { get; set; }

        [JsonProperty("landing_site_sampling_group/landing_site_text")]
        public string LandingSiteText { get; set; }
        [JsonProperty("landing_site_name")]
        public string Landing_site_name { get; set; }
        public string LandingSiteName { get { return Landing_site_name.Replace("»", ","); } }
        //public string sampling_date_string { get; set; }
        //public string decimal_sampling_date { get; set; }
        //public string outside_gear_repeat { get; set; }
        public int? Repeat_gears_count { get; set; }

        public string Gear_used1 { get; set; }
        public string Gear_used2 { get; set; }
        public string Gear_used3 { get; set; }
        public string Gear_used4 { get; set; }
        public string Gear_used5 { get; set; }
        public string Gear_used6 { get; set; }
        public string Gear_used7 { get; set; }
        public string Gear_used8 { get; set; }
        public string Gear_used9 { get; set; }
        public string Gear_used10 { get; set; }
        //public string repeat_landings_count { get; set; }

        public string __version__ { get; set; }

        [JsonProperty("meta/instanceID")]
        public string metainstanceID { get; set; }

        [JsonProperty("meta/instanceName")]
        public string metainstanceName { get; set; }
        public string _xform_id_string { get; set; }

        [JsonProperty("_uuid")]
        public string SubmissionUUID { get; set; }
        [JsonProperty("formhub/uuid")]
        public string UUID { get; set; }
        public List<object> _attachments { get; set; }
        public string _status { get; set; }
        public List<object> _geolocation { get; set; }
        [JsonProperty("_submission_time")]
        public DateTime SubmissionTime { get; set; }
        public List<object> _tags { get; set; }
        public List<object> _notes { get; set; }

        public object _submitted_by { get; set; }

    }




}
