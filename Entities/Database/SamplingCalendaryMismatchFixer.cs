﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public static class SamplingCalendaryMismatchFixer
    {
        public static event EventHandler<ProcessingItemsEventArg> ProcessingItemsEvent;
        private static HashSet<SummaryItem> _hashSetSummaryItem = new HashSet<SummaryItem>();
        public static List<CalendarDayLineage> CalendarDayLineages { get; set; }
        public static Task<bool> SearchMismatchAsync()
        {
            return Task.Run(() => SearchMismatch());
        }

        public static Task<bool> FixMismatchesAsync()
        {
            return Task.Run(() => FixMismatches());
        }

        public static bool Cancel { get; set; }
        public static bool FixMismatches()
        {
            ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "start fixing", TotalCountToProcess = MismatchSortResults.Count });
            int loopCount = 0;
            int maxGroup = (int)MismatchSortResults.Max(t => t.Grouping);
            for (int x = 1; x <= maxGroup; x++)
            {
                var batch = MismatchSortResults.Where(t => t.Grouping == x).ToList();
                var minGearUnloadID = batch.Min(t => t.GearUnloadID);
                var ids = batch.Select(t => t.GearUnloadID).Distinct();
                var lss = NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteSampling((int)batch[0].SamplingDayID);

                int editRemarkCount = 0;
                foreach (var id in ids)
                {
                    if (id != minGearUnloadID)
                    {
                        if(GearUnloadRepository.EditRemarkOfGearUnload((int)id, "delete after fixing mismatch on calendar"))
                        {
                            editRemarkCount++;
                        }
                        //var gu = lss.GearUnloadViewModel.GetGearUnload((int)id);
                        //gu.Remarks = "delete after fixing mismatch on calendar";
                        //if (lss.GearUnloadViewModel.UpdateRecordInRepo(gu))
                        //{
                        //    editRemarkCount++;
                        //}
                    }
                }

                if (editRemarkCount == ids.Count() - 1)
                {
                    foreach (var unload in batch)
                    {
                        if (VesselUnloadRepository.ChangeGearUnloadIDOfLanding((int)unload.VesselUnloadID, (int)minGearUnloadID) &&
                            unload.ChangeGearUnloadID((int)minGearUnloadID))
                        {
                            loopCount++;
                            ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "item fixed", CountProcessed = loopCount });
                        }
                    }
                }
            }
            ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "done fixing" });
            return loopCount == MismatchSortResults.Count;
        }
        public static void SetCalendarDayMismatchResults(List<CalendarDayLineage> mismatchedItems)
        {
            List<SummaryItem> items = new List<SummaryItem>();
            foreach (var item in mismatchedItems.Where(t => t.Grouping != null))
            {
                var i = NSAPEntities.SummaryItemViewModel.SummaryItemCollection.FirstOrDefault(t => t.VesselUnloadID == item.VesselUnloadID);
                i.Grouping = item.Grouping;
                items.Add(NSAPEntities.SummaryItemViewModel.SummaryItemCollection.FirstOrDefault(t => t.VesselUnloadID == item.VesselUnloadID));
            }
            MismatchSortResults = items;
        }
        public static List<SummaryItem> MismatchSortResults { get; private set; }
        public static bool SearchMismatch()
        {
            _hashSetSummaryItem.Clear();
            if (MismatchSortResults != null)
            {
                MismatchSortResults.Clear();
            }
            ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "start sorting" });
            int loopCount = 0;
            int grouping = 0;
            var items = NSAPEntities.SummaryItemViewModel.SummaryItemCollection
                .OrderBy(t => t.EnumeratorNameToUse)
                .ThenBy(t => t.FishingGround.Code)
                .ThenBy(t => t.LandingSiteNameText)
                .ThenBy(t => t.GearUsedName)
                .ThenBy(t => t.SamplingDayDate)
                .ThenBy(t => t.GearUnloadID).ToList();


            foreach (var item in items)
            {
                if (loopCount > 0)
                {
                    var previous = items[loopCount - 1];
                    bool matching = item.SamplingDayDate == previous.SamplingDayDate &&
                                     item.RegionID == previous.RegionID &&
                                     item.FMAId == previous.FMAId &&
                                     item.FishingGroundID == previous.FishingGroundID &&
                                     item.LandingSiteNameText == previous.LandingSiteNameText &&
                                     item.GearUsedName == previous.GearUsedName &&
                                     item.EnumeratorNameToUse == previous.EnumeratorNameToUse &&
                                     item.GearUnloadID != previous.GearUnloadID;

                    if (matching && !_hashSetSummaryItem.Contains(item))
                    {
                        //var item_match = _hashSetSummaryItem.FirstOrDefault(t => t.VesselUnloadID != item.VesselUnloadID &&
                        //                t.GearUsedName == item.GearUsedName &&
                        //                t.SamplingDayDate == item.SamplingDayDate &&
                        //                t.RegionID == item.RegionID &&
                        //                t.FMAId == item.FMAId &&
                        //                t.FishingGroundID == item.FishingGroundID &&
                        //                t.LandingSiteNameText == item.LandingSiteNameText &&
                        //                t.EnumeratorNameToUse == item.EnumeratorNameToUse);
                        //if (item_match == null)
                        //{
                        _hashSetSummaryItem.Add(item);
                        grouping++;
                        item.Grouping = grouping;
                        //}
                    }


                }
                loopCount++;
            }

            ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "processing start", TotalCountToProcess = _hashSetSummaryItem.Count });
            int outerloopCount = 0;
            loopCount = 1;
            foreach (var hsi in _hashSetSummaryItem)
            {
                foreach (var item in items)

                {
                    if (Cancel)
                    {
                        ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "cancel" });
                        return false;
                    }
                    else if (hsi.Equals(item))
                    {
                        item.Grouping = hsi.Grouping;
                        item.RowId = loopCount;
                        loopCount++;
                    }
                }
                outerloopCount++;
                ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "item sorted", CountProcessed = outerloopCount });
            }

            if (_hashSetSummaryItem != null && _hashSetSummaryItem.Count > 0)
            {
                MismatchSortResults = items.Where(t => t.Grouping != null).ToList();
            }
            ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "done sorting" });

            return _hashSetSummaryItem.Count > 0;
        }
    }
}
