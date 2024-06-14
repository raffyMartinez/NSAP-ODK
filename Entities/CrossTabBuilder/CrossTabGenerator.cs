using NSAP_ODK.Entities.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities.Database;
using NSAP_ODK.TreeViewModelControl;
using System.Windows;
using System.Data;
using NSAP_ODK.Utilities;

namespace NSAP_ODK.Entities.CrossTabBuilder
{
    public static class CrossTabGenerator
    {

        public static AllSamplingEntitiesEventHandler EntitiesOfMonth { get; set; }
        public static Dictionary<string, List<VesselUnload>> VesselUnloadsDictionary { get; private set; } = new Dictionary<string, List<VesselUnload>>();
        public static Dictionary<string, List<VesselEffortCrossTab>> VesselEffortDictionary { get; private set; } = new Dictionary<string, List<VesselEffortCrossTab>>();
        public static Dictionary<string, List<VesselUnload_FishingGear>> VesselUnloadGearDictionary { get; private set; } = new Dictionary<string, List<VesselUnload_FishingGear>>();
        public static Dictionary<string, List<VesselCatch>> VesselCatchDictionary { get; private set; } = new Dictionary<string, List<VesselCatch>>();

        public static Dictionary<string, List<CatchLengthWeightCrossTab>> CatchLengthWeightCrossTabDictionary = new Dictionary<string, List<CatchLengthWeightCrossTab>>();

        public static Dictionary<string, List<CatchMaturityCrossTab>> CatchMaturityCrossTabDictionary = new Dictionary<string, List<CatchMaturityCrossTab>>();
        public static Dictionary<string, List<CatchLengthCrossTab>> CatchLengthCrossTabDictionary = new Dictionary<string, List<CatchLengthCrossTab>>();

        public static Dictionary<string, List<CatchLengthFreqCrossTab>> CatchLengthFreqCrossTabDictionary = new Dictionary<string, List<CatchLengthFreqCrossTab>>();
        public static List<CatchMaturityCrossTab> CatchMaturityCrossTabs { get; set; }
        public static List<CatchLengthFreqCrossTab> CatchLengthFreqCrossTabs { get; set; }
        public static List<CatchLengthCrossTab> CatchLengthCrossTabs { get; set; }
        public static List<CatchLengthWeightCrossTab> CatchLengthWeightCrossTabs { get; set; }
        public static List<VesselUnload> VesselUnloads { get; set; }
        public static List<VesselUnload_FishingGear> VesselUnload_FishingGears { get; set; }
        public static List<VesselEffortCrossTab> VesselEffortCrossTabs { get; set; }
        public static List<VesselCatch> VesselCatches { get; set; }
        public static List<VesselEffortCrossTab> GetVesselEffortCrossTabsFromRepository()
        {
            VesselEffortCrossTabs = VesselEffortRepository.GetEffortForCrossTab(EntitiesOfMonth);
            return VesselEffortCrossTabs;
        }
        public static void GetFromRepository()
        {
            try
            {
                VesselUnloads = VesselUnloadRepository.GetVesselUnloads(EntitiesOfMonth);
                var gear_unloads = NSAPEntities.SummaryItemViewModel.GetGearUnloadsFromTree(EntitiesOfMonth);
                foreach (VesselUnload vu in VesselUnloads)
                {
                    vu.Parent = gear_unloads.Find(t => t.PK == vu.GearUnloadID);
                    //gear_unloads.Remove(vu.Parent);
                }
                VesselUnload_FishingGears = VesselUnload_FishingGearRepository.GetFishingGears(EntitiesOfMonth);
                foreach (VesselUnload_FishingGear vufg in VesselUnload_FishingGears)
                {
                    vufg.Parent = VesselUnloads.Find(t => t.PK == vufg.ParentID);
                    vufg.Parent.ListUnloadFishingGearsEx.Add(vufg);
                }
                VesselEffortCrossTabs = VesselEffortRepository.GetEffortForCrossTab(EntitiesOfMonth);
                foreach (VesselEffortCrossTab vect in VesselEffortCrossTabs)
                {
                    vect.VesselUnload = VesselUnloads.Find(t => t.PK == vect.VesselUnloadID);
                    var vufg = vect.VesselUnload.ListUnloadFishingGearsEx.Find(t => t.GearCode == vect.GearCode);
                    vufg.ListOfSpecsForCrossTab.Add(vect);
                }

                VesselCatches = VesselCatchRepository.GetVesselCatchForCrosstab(EntitiesOfMonth);
                foreach (VesselCatch vc in VesselCatches)
                {
                    vc.Parent = VesselUnloads.Find(t => t.PK == vc.VesselUnloadID);
                    var vufg = vc.Parent.ListUnloadFishingGearsEx.Find(t => t.GearCode == vc.GearCode);
                    vc.ParentFishingGear = vufg;
                    vc.ParentFishingGear.ListOfCatchForCrossTab.Add(vc);
                }
                CatchLengthWeightCrossTabs = CatchLenWeightRepository.GetLengthWeightForCrosstab(EntitiesOfMonth);
                foreach (CatchLengthWeightCrossTab clwct in CatchLengthWeightCrossTabs)
                {
                    clwct.VesselUnload = VesselUnloads.Find(t => t.PK == clwct.V_unload_id);
                    clwct.VesselUnload.ListUnloadFishingGearsEx.
                        Find(t=>t.GearCode==clwct.GearCode).ListOfCatchForCrossTab.
                        Find(t=>t.PK==clwct.ParentCatchID).ListCrossTabLengthWeight.Add(clwct);
                }
                CatchLengthCrossTabs = CatchLengthRepository.GetLengthForCrosstab(EntitiesOfMonth);
                foreach (CatchLengthCrossTab clct in CatchLengthCrossTabs)
                {
                    clct.VesselUnload = VesselUnloads.Find(t => t.PK == clct.V_unload_id);
                    clct.VesselUnload.ListUnloadFishingGearsEx.
                        Find(t=>t.GearCode==clct.GearCode).ListOfCatchForCrossTab.
                        Find(t=>t.PK==clct.ParentCatchID).ListCrossTabLength.Add(clct);
                }
                CatchLengthFreqCrossTabs = CatchLenFreqRepository.GetLengthFreqForCrosstab(EntitiesOfMonth);
                foreach (CatchLengthFreqCrossTab clfct in CatchLengthFreqCrossTabs)
                {
                    clfct.VesselUnload = VesselUnloads.Find(t => t.PK == clfct.V_unload_id);
                    clfct.VesselUnload.ListUnloadFishingGearsEx.
                        Find(t => t.GearCode == clfct.GearCode).ListOfCatchForCrossTab.
                        Find(t => t.PK == clfct.ParentCatchID).ListCrossTabLengthFreq.Add(clfct);
                }



                CatchMaturityCrossTabs = CatchMaturityRepository.GetCatchMaturityForCrosstab(EntitiesOfMonth);
                foreach (CatchMaturityCrossTab cmt in CatchMaturityCrossTabs)
                {
                    cmt.VesselUnload = VesselUnloads.Find(t => t.PK == cmt.V_unload_id);
                    cmt.VesselUnload.ListUnloadFishingGearsEx.
                        Find(t=>t.GearCode==cmt.GearCode).ListOfCatchForCrossTab.
                        Find(t=>t.PK==cmt.ParentCatchID).ListCrossTabMaturity.Add(cmt);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            AddToDictionary();
        }
        public static bool GenerateCrossTab(AllSamplingEntitiesEventHandler entitiesDefinition)
        {
            GetEntities(entitiesDefinition);
            return CrossTabDatasetsGenerator.GenerateDatasets();
        }
        public static void Clear()
        {
            VesselUnloadsDictionary.Clear();
            VesselEffortDictionary.Clear();
            VesselUnloadGearDictionary.Clear();
            CatchLengthWeightCrossTabDictionary.Clear();
            CatchLengthCrossTabDictionary.Clear();
            CatchLengthFreqCrossTabDictionary.Clear();
            CatchMaturityCrossTabDictionary.Clear();
            
            NewLists();
        }

        private static void NewLists()
        {
            VesselUnloads = new List<VesselUnload>();
            VesselEffortCrossTabs = new List<VesselEffortCrossTab>();
            VesselUnload_FishingGears = new List<VesselUnload_FishingGear>();
            VesselCatches = new List<VesselCatch>();
            CatchLengthWeightCrossTabs = new List<CatchLengthWeightCrossTab>();
            CatchLengthCrossTabs = new List<CatchLengthCrossTab>();
            CatchLengthFreqCrossTabs = new List<CatchLengthFreqCrossTab>();
            CatchMaturityCrossTabs = new List<CatchMaturityCrossTab>();
        }
        
        public static void GetEntities(AllSamplingEntitiesEventHandler entities)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            EntitiesOfMonth = entities;

            NewLists();

            try
            {
                VesselUnloads = VesselUnloadsDictionary[EntitiesOfMonth.GUID];
                VesselEffortCrossTabs = VesselEffortDictionary[EntitiesOfMonth.GUID];
                VesselUnload_FishingGears = VesselUnloadGearDictionary[EntitiesOfMonth.GUID];
                VesselCatches = VesselCatchDictionary[EntitiesOfMonth.GUID];
                CatchLengthWeightCrossTabs = CatchLengthWeightCrossTabDictionary[EntitiesOfMonth.GUID];
                CatchLengthCrossTabs = CatchLengthCrossTabDictionary[EntitiesOfMonth.GUID];
                CatchLengthFreqCrossTabs = CatchLengthFreqCrossTabDictionary[EntitiesOfMonth.GUID];
                CatchMaturityCrossTabs = CatchMaturityCrossTabDictionary[EntitiesOfMonth.GUID];
            }
            catch
            {
                GetFromRepository();
            }
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            MessageBox.Show($"execution time:{elapsedMs} ms");

        }
        public static void AddToDictionary()
        {

            if (VesselUnloadsDictionary.Keys.Count == 0 || !VesselUnloadsDictionary.Keys.Contains(EntitiesOfMonth.GUID))
            {
                VesselUnloadsDictionary.Add(EntitiesOfMonth.GUID, VesselUnloads);
            }
            if (VesselEffortDictionary.Keys.Count == 0 || !VesselEffortDictionary.Keys.Contains(EntitiesOfMonth.GUID))
            {
                VesselEffortDictionary.Add(EntitiesOfMonth.GUID, VesselEffortCrossTabs);
            }
            if (VesselUnloadGearDictionary.Keys.Count == 0 || !VesselUnloadGearDictionary.Keys.Contains(EntitiesOfMonth.GUID))
            {
                VesselUnloadGearDictionary.Add(EntitiesOfMonth.GUID, VesselUnload_FishingGears);
            }
            if (CatchLengthWeightCrossTabDictionary.Keys.Count == 0 || !CatchLengthWeightCrossTabDictionary.Keys.Contains(EntitiesOfMonth.GUID))
            {
                CatchLengthWeightCrossTabDictionary.Add(EntitiesOfMonth.GUID, CatchLengthWeightCrossTabs);
            }
            if (CatchLengthCrossTabDictionary.Keys.Count == 0 || !CatchLengthCrossTabDictionary.Keys.Contains(EntitiesOfMonth.GUID))
            {
                CatchLengthCrossTabDictionary.Add(EntitiesOfMonth.GUID, CatchLengthCrossTabs);
            }
            if (CatchLengthFreqCrossTabDictionary.Keys.Count == 0 || !CatchLengthFreqCrossTabDictionary.Keys.Contains(EntitiesOfMonth.GUID))
            {
                CatchLengthFreqCrossTabDictionary.Add(EntitiesOfMonth.GUID, CatchLengthFreqCrossTabs);
            }
            if (CatchMaturityCrossTabDictionary.Keys.Count == 0 || !CatchMaturityCrossTabDictionary.Keys.Contains(EntitiesOfMonth.GUID))
            {
                CatchMaturityCrossTabDictionary.Add(EntitiesOfMonth.GUID, CatchMaturityCrossTabs);
            }

        }
    }
}
