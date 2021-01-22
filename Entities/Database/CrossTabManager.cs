using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace NSAP_ODK.Entities.Database
{
    public static class CrossTabManager
    {
        private static List<CrossTabEffort> _crossTabEfforts;
        private static List<CrossTabLenFreq> _crossTabLenFreqs;
        private static List<CrossTabMaturity> _crossTabMaturities;
        private static List<CrossTabLength> _crossTabLengths;
        private static DataTable _effortCrostabDataTable;
        public static void GearByMonthYear(TreeViewModelControl.AllSamplingEntitiesEventHandler sev)
        {
            _crossTabEfforts = new List<CrossTabEffort>();
            _crossTabLenFreqs = new List<CrossTabLenFreq>();
            _crossTabMaturities = new List<CrossTabMaturity>();
            _crossTabLengths = new List<CrossTabLength>();

            List<GearUnload> gearUnloads = new List<GearUnload>();
            if (sev.GearUsed==null || sev.GearUsed.Length == 0)
            {
                //when we select from the tree and want to process all gears landed for a month
                   gearUnloads = NSAPEntities.GearUnloadViewModel.GearUnloadCollection
                    .Where(t => t.Parent.NSAPRegion.Code == sev.NSAPRegion.Code &&
                              t.Parent.FMA.FMAID == sev.FMA.FMAID &&
                              t.Parent.FishingGround.Code == sev.FishingGround.Code &&
                              t.Parent.LandingSiteName == sev.LandingSiteText &&
                              t.Parent.SamplingDate.Date >= (DateTime)sev.MonthSampled &&
                              t.Parent.SamplingDate.Date < ((DateTime)sev.MonthSampled).AddMonths(1)).ToList();

            }
            else
            {
                //when we select a gear from the datagrid and want to process only a gear for a month
                gearUnloads = NSAPEntities.GearUnloadViewModel.GearUnloadCollection
                 .Where(t => t.Parent.NSAPRegion.Code == sev.NSAPRegion.Code &&
                           t.Parent.FMA.FMAID == sev.FMA.FMAID &&
                           t.Parent.FishingGround.Code == sev.FishingGround.Code &&
                           t.Parent.LandingSiteName == sev.LandingSiteText &&
                           t.GearUsedName == sev.GearUsed && 
                           t.Parent.SamplingDate.Date >= (DateTime)sev.MonthSampled &&
                           t.Parent.SamplingDate.Date < ((DateTime)sev.MonthSampled).AddMonths(1)).ToList();
            }


            foreach(var gu in gearUnloads)
            {

                
                foreach (var item in NSAPEntities.VesselCatchViewModel.VesselCatchCollection
                    .Where(t => t.Parent.Parent.PK  == gu.PK).ToList())
                {
                    CrossTabCommon ctc = new CrossTabCommon(item);
                    _crossTabEfforts.Add(new CrossTabEffort { CrossTabCommon = ctc });
                }


                
                foreach (var item in NSAPEntities.CatchLenFreqViewModel.CatchLenFreqCollection
                    .Where(t => t.Parent.Parent.Parent.PK == gu.PK).ToList())
                {
                    CrossTabCommon ctc = new CrossTabCommon(item);
                    _crossTabLenFreqs.Add(new CrossTabLenFreq { CrossTabCommon = ctc });
                }

                foreach (var item in NSAPEntities.CatchMaturityViewModel.CatchMaturityCollection
                    .Where(t => t.Parent.Parent.Parent.PK == gu.PK).ToList())
                {
                    CrossTabCommon ctc = new CrossTabCommon(item);
                    _crossTabMaturities.Add(new CrossTabMaturity { CrossTabCommon = ctc });
                }

                foreach (var item in NSAPEntities.CatchLengthViewModel.CatchLengthCollection
                    .Where(t => t.Parent.Parent.Parent.PK == gu.PK).ToList())
                {
                    CrossTabCommon ctc = new CrossTabCommon(item);
                    _crossTabLengths.Add(new CrossTabLength{ CrossTabCommon = ctc });
                }

            }

            BuildEffortCrossTabDataTable();
        }

        private static void BuildEffortCrossTabDataTable()
        {
            _effortCrostabDataTable = new DataTable();

            DataColumn dc = new DataColumn { ColumnName = "Data ID" };
            _effortCrostabDataTable.Columns.Add(dc);


        }
        public static List<CrossTabLength> CrossTabLengths { get { return _crossTabLengths; } }
        public static List<CrossTabMaturity> CrossTabMaturities { get { return _crossTabMaturities; } }

        public static List<CrossTabLenFreq> CrossTabLenFreqs { get { return _crossTabLenFreqs; } }

        public static DataTable CrossTabEfforts { get { return _effortCrostabDataTable; } }
    }
}
