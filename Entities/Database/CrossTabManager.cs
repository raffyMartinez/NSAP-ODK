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

            dc = new DataColumn { ColumnName = "Fishing ground" };
            _effortCrostabDataTable.Columns.Add(dc);


            dc = new DataColumn { ColumnName = "Year",DataType=typeof(int) };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Month" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Date",DataType=typeof(DateTime) };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Province" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing ground" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Municipality" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Landing site" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sector" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Grid location" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Gear" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessel" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessels landded", DataType=typeof(int)};
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessels monitored",DataType = typeof(int) };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sampling day", DataType = typeof(bool) };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Family" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Species" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Catch weight", DataType=typeof(double) };
            _effortCrostabDataTable.Columns.Add(dc);

            foreach(var spec in NSAPEntities.EffortSpecificationViewModel.EffortSpecCollection.OrderBy(t=>t.Name))
            {
                dc = new DataColumn { ColumnName = spec.Name };
                switch(spec.ValueType)
                {
                    case ODKValueType.isBoolean:
                        dc.DataType = typeof(bool);
                        break;
                    case ODKValueType.isDecimal:
                        dc.DataType = typeof(double);
                        break;
                    case ODKValueType.isInteger:
                        dc.DataType = typeof(int);
                        break;
                    case ODKValueType.isText:
                    case ODKValueType.isUndefined:
                        dc.DataType = typeof(string);
                        break;
                }
                _effortCrostabDataTable.Columns.Add(dc);
            }

            foreach(var item in _crossTabEfforts)
            {
                var row = _effortCrostabDataTable.NewRow();
                row["Data ID"] = item.CrossTabCommon.DataID;
                row["Fishing ground"] = item.CrossTabCommon.FishingGround.Name;
                row["Year"] = item.CrossTabCommon.MonthSampled.Year;
                row["Month"] = item.CrossTabCommon.MonthSampled.ToString("MMM");
                row["Date"] = item.CrossTabCommon.SamplingDate.ToString("dd-MMM-yyyy");

                if (item.CrossTabCommon.LandingSite != null)
                {
                    row["Province"] = item.CrossTabCommon.LandingSite.Municipality.Province.ProvinceName;
                    row["Municipality"] = item.CrossTabCommon.LandingSite.Municipality.MunicipalityName;
                }
                row["Landing site"] = item.CrossTabCommon.LandingSiteName;
                row["Sector"] = item.CrossTabCommon.Sector;
                row["Grid location"] = $"{item.CrossTabCommon.FishingGroundGrid.ToString()}";
                row["Gear"] = item.CrossTabCommon.GearName;
                row["Fishing vessel"] = item.CrossTabCommon.FBName;
                row["Fishing vessels landded"] = item.CrossTabCommon.FBL;
                row["Fishing vessels monitored"] = item.CrossTabCommon.FBM;




            }


        }
        public static List<CrossTabLength> CrossTabLengths { get { return _crossTabLengths; } }
        public static List<CrossTabMaturity> CrossTabMaturities { get { return _crossTabMaturities; } }

        public static List<CrossTabLenFreq> CrossTabLenFreqs { get { return _crossTabLenFreqs; } }

        public static DataTable CrossTabEfforts { get { return _effortCrostabDataTable; } }
    }
}
