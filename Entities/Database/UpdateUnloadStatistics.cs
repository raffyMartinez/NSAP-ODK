using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public static class UpdateUnloadStatistics
    {
        public static event EventHandler DatabaseUpdatedEvent;

        private static void ManageUpdateEvent(string intent, int? round = null, int? runningCount = null, int? rowsForUpdating = null)
        {
            EventHandler h = DatabaseUpdatedEvent;
            if (h != null)
            {
                switch (intent)
                {
                    case "start":
                        var ev = new UpdateDatabaseColumnEventArg
                        {
                            Intent = intent,
                            RowsToUpdate = (int)rowsForUpdating
                        };
                        if (round != null)
                        {
                            ev.Round = (int)round;
                        }

                        h(null, ev);
                        break;
                    case "start updating":
                        ev = new UpdateDatabaseColumnEventArg
                        {
                            Intent = intent
                        };
                        h(null, ev);
                        break;
                    case "row updated":
                        ev = new UpdateDatabaseColumnEventArg
                        {
                            Intent = intent,
                            RunningCount = (int)runningCount
                        };
                        h(null, ev);
                        break;
                    case "finished":
                        h(null, new UpdateDatabaseColumnEventArg { Intent = intent });
                        break;
                }

            }
        }

        public static Task<int> UpdateUnloadStatsAsync()
        {
            return Task.Run(() => UpdateUnloadStats());
        }
        public static int UpdateUnloadStats()
        {

            int counter = 0;
            ManageUpdateEvent(intent: "start", rowsForUpdating: NSAPEntities.SummaryItemViewModel.Count);
            foreach (LandingSiteSampling lss in NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection)
            {
                foreach (GearUnload gu in lss.GearUnloadViewModel.GearUnloadCollection)
                {
                    if (gu.VesselUnloadViewModel == null)
                    {
                        gu.VesselUnloadViewModel = new VesselUnloadViewModel(gu, true);
                    }
                    foreach (VesselUnload vu in gu.VesselUnloadViewModel.VesselUnloadCollection)
                    {
                        vu.CountGrids = vu.FishingGroundGridViewModel.Count;
                        vu.CountEffortIndicators = vu.VesselEffortViewModel.Count;
                        vu.CountGearSoak = vu.GearSoakViewModel.Count;
                        vu.CountCatchCompositionItems = vu.VesselCatchViewModel.Count;
                        if (vu.CountCatchCompositionItems > 0)
                        {
                            foreach (VesselCatch vc in vu.VesselCatchViewModel.VesselCatchCollection)
                            {
                                vu.CountLenFreqRows += vc.ListCatchLenFreq.Count;
                                vu.CountLengthRows += vc.ListCatchLength.Count;
                                vu.CountLenWtRows += vc.ListCatchLengthWeight.Count;
                                vu.CountMaturityRows += vc.ListCatchMaturity.Count;
                            }

                        }
                        if (counter == 1)
                        {
                            ManageUpdateEvent(intent: "start updating");
                        }
                        if (VesselUnloadRepository.UpdateUnloadStats(vu))
                        {
                            counter++;
                            ManageUpdateEvent(intent: "row updated", runningCount: counter);
                        }
                    }

                }
            }
            ManageUpdateEvent(intent: "finished");
            return counter;
        }
    }
}

