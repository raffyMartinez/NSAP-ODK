using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel;

namespace NSAP_ODK.Entities.Database
{

    public static class UnloadWithMaturityDatasetManager
    {
        private static DataTable _unloadWithMaturityDataTable;
        private static DataTable _catchCompithMaturityDataTable;
        private static DataTable _maturityDataTable;

        private static string _region;
        private static string _fishingGround;
        private static string _startDate;
        private static string _endDate;

        public static string FileName
        {
            get
            {
                return $"Maturity data {_region} {_fishingGround} {_startDate} - {_endDate}";
            }
        }
        public static DataSet MaturityDataSet(NSAPRegion region, FishingGround fishingGround)
        {
            _region = region.ToString();
            _fishingGround = fishingGround.Name;
            BuildUnloadWithMaturityDatatable(region, fishingGround);

            DataSet ds = new DataSet();
            _unloadWithMaturityDataTable.TableName = "Samplings with maturity";
            _catchCompithMaturityDataTable.TableName = "Catch";
            _maturityDataTable.TableName = "Maturity data";
            ds.Tables.Add(_unloadWithMaturityDataTable);
            ds.Tables.Add(_catchCompithMaturityDataTable);
            ds.Tables.Add(_maturityDataTable);
            return ds;

        }
        private static void BuildUnloadWithMaturityDatatable(NSAPRegion region, FishingGround fishingGround)
        {
            _unloadWithMaturityDataTable = new DataTable();

            DataColumn dc = new DataColumn { ColumnName = "VesselUnload ID", DataType = typeof(int) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Region", DataType = typeof(string) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "FMA", DataType = typeof(string) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing ground", DataType = typeof(string) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Landing site", DataType = typeof(string) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Gear", DataType = typeof(string) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sampling date", DataType = typeof(DateTime) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "SamplingDay ID", DataType = typeof(int) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "GearUnload ID", DataType = typeof(int) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Enumerator", DataType = typeof(string) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Is boat used", DataType = typeof(bool) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Vessel", DataType = typeof(string) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sector", DataType = typeof(string) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Catch total wt", DataType = typeof(double) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Is tracked", DataType = typeof(bool) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "GPS", DataType = typeof(string) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Grid location", DataType = typeof(string) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Longitude", DataType = typeof(double) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Latitude", DataType = typeof(double) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Departure date", DataType = typeof(DateTime) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Arrival date", DataType = typeof(DateTime) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "# of catch with maturity", DataType = typeof(int) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Row ID", DataType = typeof(string) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "XForm ID", DataType = typeof(string) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "XForm date", DataType = typeof(DateTime) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "User name", DataType = typeof(string) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Device ID", DataType = typeof(string) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Submitted", DataType = typeof(DateTime) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Form version", DataType = typeof(string) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Notes", DataType = typeof(string) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Date added to database", DataType = typeof(DateTime) };
            _unloadWithMaturityDataTable.Columns.Add(dc);

            BuildCatchCompWithMaturityDataTable();
            BuildMaturityDataTable();

            var list = NSAPEntities.VesselCatchViewModel.GetUnloadsWithMaturity(region, fishingGround);
            if (list.Count > 0)
            {
                _startDate = list.Min(t => t.SamplingDateTime).ToString("MMM-dd-yyyy");
                _endDate = list.Max(t => t.SamplingDateTime).ToString("MMM-dd-yyyy");

                foreach (var item in list)
                {
                    var row = _unloadWithMaturityDataTable.NewRow();
                    row["VesselUnload ID"] = item.VesselUnloadID;
                    row["Region"] = item.Region;
                    row["FMA"] = item.FMA;
                    row["Fishing ground"] = item.FishingGround;
                    row["Landing site"] = item.LandingSite;
                    row["Gear"] = item.Gear;
                    row["Sampling date"] = item.SamplingDateTime;
                    row["SamplingDay ID"] = item.SamplingDayID;
                    row["GearUnload ID"] = item.GearUnloadID;
                    row["Enumerator"] = item.Enumerator;
                    row["Is boat used"] = item.IsBoatUsed;
                    row["Vessel"] = item.Vessel;
                    row["Sector"] = item.Sector;
                    row["Catch total wt"] = item.CatchTotalWt;
                    row["Is tracked"] = item.IsTracked;
                    row["GPS"] = item.GPS;
                    row["Grid location"] = item.FishingGroundGird;
                    if (item.Longitude == null)
                    {
                        row["Longitude"] = DBNull.Value;
                    }
                    else
                    {
                        row["Longitude"] = item.Longitude;
                    }
                    if (item.Latitude == null)
                    {
                        row["Latitude"] = DBNull.Value;
                    }
                    else
                    {
                        row["Latitude"] = item.Latitude;
                    }

                    if (item.Departure == null)
                    {
                        row["Departure date"] = DBNull.Value;
                    }
                    else
                    {
                        row["Departure date"] = item.Departure;
                    }
                    if (item.Arrival == null)
                    {
                        row["Arrival date"] = DBNull.Value;
                    }
                    else
                    {
                        row["Arrival date"] = item.Arrival;
                    }
                    row["# of catch with maturity"] = item.ListOfCatchWithMaturity.Count;
                    row["Row ID"] = item.RowID;
                    row["XForm ID"] = item.XFormIdentifier;
                    if (item.XFormDate == null)
                    {
                        row["XForm date"] = DBNull.Value;
                    }
                    else
                    {
                        row["XForm date"] = item.XFormDate;
                    }
                    row["User name"] = item.UserName;
                    row["Device ID"] = item.DeviceID;
                    row["Submitted"] = item.Submitted;
                    row["Form version"] = item.FormVersion;
                    row["Notes"] = item.Notes;
                    row["Date added to database"] = item.DateAddedToDatabase;



                    foreach (var c in item.ListOfCatchWithMaturity)
                    {
                        if (c.ListCatchMaturity.Count > 0)
                        {
                            var catchRow = _catchCompithMaturityDataTable.NewRow();
                            catchRow["ID"] = c.PK;
                            catchRow["Parent ID"] = c.Parent.PK;
                            catchRow["Species"] = c.CatchName;
                            catchRow["Taxa"] = c.TaxaCode;
                            if (c.Catch_kg == null)
                            {
                                catchRow["Weight"] = DBNull.Value;
                            }
                            else
                            {
                                catchRow["Weight"] = c.Catch_kg;
                            }
                            catchRow["# of maturity data"] = c.ListCatchMaturity.Count;


                            foreach (var gms in c.ListCatchMaturity)
                            {
                                var gmsRow = _maturityDataTable.NewRow();
                                gmsRow["ID"] = gms.PK;
                                gmsRow["Parent ID"] = gms.Parent.PK;
                                if (gms.Length == null)
                                {
                                    gmsRow["Length"] = DBNull.Value;
                                }
                                else
                                {
                                    gmsRow["Length"] = gms.Length;
                                }
                                if (gms.Weight == null)
                                {
                                    gmsRow["Weight"] = DBNull.Value;
                                }
                                else
                                {
                                    gmsRow["Weight"] = gms.Weight;
                                }
                                gmsRow["Sex"] = gms.Sex;
                                gmsRow["Maturity state"] = gms.Maturity;
                                if (gms.GonadWeight == null)
                                {
                                    gmsRow["Gonad wt"] = DBNull.Value;
                                }
                                else
                                {
                                    gmsRow["Gonad wt"] = gms.GonadWeight;
                                }
                                gmsRow["Gut content category"] = gms.GutContentClassification;

                                _maturityDataTable.Rows.Add(gmsRow);
                            }

                            _catchCompithMaturityDataTable.Rows.Add(catchRow);
                        }
                    }
                    _unloadWithMaturityDataTable.Rows.Add(row);
                }

            }
        }

        private static void BuildCatchCompWithMaturityDataTable()
        {
            _catchCompithMaturityDataTable = new DataTable();

            DataColumn dc = new DataColumn { ColumnName = "ID", DataType = typeof(int) };
            _catchCompithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Parent ID", DataType = typeof(int) };
            _catchCompithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Species", DataType = typeof(string) };
            _catchCompithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Taxa", DataType = typeof(string) };
            _catchCompithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Weight", DataType = typeof(double) };
            _catchCompithMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "# of maturity data", DataType = typeof(int) };
            _catchCompithMaturityDataTable.Columns.Add(dc);
        }

        private static void BuildMaturityDataTable()
        {
            _maturityDataTable = new DataTable();

            DataColumn dc = new DataColumn { ColumnName = "ID", DataType = typeof(int) };
            _maturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Parent ID", DataType = typeof(int) };
            _maturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Length", DataType = typeof(double) };
            _maturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Weight", DataType = typeof(double) };
            _maturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sex", DataType = typeof(string) };
            _maturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Maturity state", DataType = typeof(string) };
            _maturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Gonad wt", DataType = typeof(double) };
            _maturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Gut content category", DataType = typeof(string) };
            _maturityDataTable.Columns.Add(dc);
        }
    }
}
