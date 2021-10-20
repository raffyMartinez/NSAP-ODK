using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities.Database;
using NSAP_ODK.Entities;
using System.Data;

public class TrackedOperationSummaryViewModel
{
    public event EventHandler<TrackedOperationSummaryEventArgs> SummaryRead;
    private List<TrackedOperationSummary> _trackedOperationSummarylist = new List<TrackedOperationSummary>();
    private List<LandingWithMaturityData> _landingWithMaturityDataList = new List<LandingWithMaturityData>();
    private DataSet _dataset;
    private List<TrackedLandingCentroid> _landingCentroids;
    public TrackedOperationSummaryViewModel(List<TrackedLandingCentroid>landingCentroids=null)
    {
        //SetupSummaryList();
        _landingCentroids = landingCentroids;
    }
    public int UndersizedCutoffLength { get; set; }
    public Task<int> SetupSuammryLlistAsync(DateTime? start = null, DateTime? end = null)
    {
        return Task.Run(() => SetupSummaryList(start, end));
    }

    public DataSet TrackedLadningSummaryDataset()
    {
        _dataset = new DataSet();
        DataTable dt = new DataTable("Landings with maturity data");

        DataColumn dc = new DataColumn { ColumnName = "SamplingID", DataType = typeof(int) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Date", DataType = typeof(DateTime) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "NSAP region", DataType = typeof(string) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "FMA", DataType = typeof(string) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Fishing ground", DataType = typeof(string) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Landing site", DataType = typeof(string) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Gear", DataType = typeof(string) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Vessel name", DataType = typeof(string) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Has GPS", DataType = typeof(bool) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Has BSC maturity data", DataType = typeof(bool) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Has berried crab", DataType = typeof(bool) };
        dt.Columns.Add(dc);

        //dc = new DataColumn { ColumnName = "Undersized crab cutoff length", DataType = typeof(int) };
        //dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Has undersized crab", DataType = typeof(bool) };
        dt.Columns.Add(dc);

        foreach (LandingWithMaturityData lwmd in _landingWithMaturityDataList)
        {
            var row = dt.NewRow();
            row["SamplingID"] = lwmd.VesselUnload.PK;
            row["Date"] = lwmd.VesselUnload.SamplingDate;
            row["NSAP region"] = lwmd.VesselUnload.Parent.Parent.NSAPRegion.Name;
            row["FMA"] = lwmd.VesselUnload.Parent.Parent.FMA.Name;
            row["Fishing ground"] = lwmd.VesselUnload.Parent.Parent.FishingGround.ToString();
            row["Landing site"] = lwmd.VesselUnload.Parent.Parent.LandingSite.ToString();
            row["Gear"] = lwmd.VesselUnload.Parent.Gear?.GearName;
            row["Vessel name"] = lwmd.VesselUnload.VesselName;
            row["Has GPS"] = lwmd.HasGPS;
            row["Has BSC maturity data"] = lwmd.HasBSCMaturityData;
            row["Has berried crab"] = lwmd.HasBerriedCrabs;
            row["Has undersized crab"] = lwmd.HasUndersizedCrabs;
            dt.Rows.Add(row);
        }
        _dataset.Tables.Add(dt);
        TracedlLandingsDataset();
        return _dataset;
    }
    private void TracedlLandingsDataset()
    {
        //DataSet ds = new DataSet();
        DataTable dt = new DataTable("Tracked landing summary");


        DataColumn dc = new DataColumn { ColumnName = "SamplingID", DataType = typeof(int) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Date", DataType = typeof(DateTime) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "NSAP region", DataType = typeof(string) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "FMA", DataType = typeof(string) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Fishing ground", DataType = typeof(string) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Landing site", DataType = typeof(string) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Gear", DataType = typeof(string) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Vessel name", DataType = typeof(string) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "GPS", DataType = typeof(string) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "X", DataType = typeof(double) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Y", DataType = typeof(double) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "TimeToSampling", DataType = typeof(double) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Weight of catch", DataType = typeof(double) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Number of fishers", DataType = typeof(int) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Number of hours fishing", DataType = typeof(double) };
        dt.Columns.Add(dc);


        dc = new DataColumn { ColumnName = "Weight of crabs", DataType = typeof(double) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Has crab maturity data", DataType = typeof(bool) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Number of males", DataType = typeof(int) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Number of females", DataType = typeof(int) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Number of maturity measurements", DataType = typeof(int) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Weight of crabs from maturity data", DataType = typeof(double) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Berried crab weight", DataType = typeof(double) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Berried crab count", DataType = typeof(int) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Berried crab percent by weight", DataType = typeof(double) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Undersized crab cutoff length", DataType = typeof(int) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Undersized crab weight", DataType = typeof(double) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Undersized crab count", DataType = typeof(int) };
        dt.Columns.Add(dc);

        dc = new DataColumn { ColumnName = "Undersized crab percent by weight", DataType = typeof(double) };
        dt.Columns.Add(dc);

        foreach (var tr in _trackedOperationSummarylist)
        {
            var row = dt.NewRow();
            row["SamplingID"] = tr.SamplingID;
            row["Date"] = tr.SamplingDate;
            row["NSAP region"] = tr.VesselUnload.Parent.Parent.NSAPRegion.Name;
            row["FMA"] = tr.VesselUnload.Parent.Parent.FMA.Name;
            row["Fishing ground"] = tr.VesselUnload.Parent.Parent.FishingGround.Name;
            row["Landing site"] = tr.LandingSite.ToString();
            row["Gear"] = tr.Gear.ToString();
            row["Vessel name"] = tr.VesselName;
            row["GPS"] = tr.GPS.AssignedName;
            row["X"] = tr.X;
            row["Y"] = tr.Y;
            //row["TimeToSampling"] = $"{tr.EndHaulToSamplingDate.TotalHours}:{tr.EndHaulToSamplingDate.TotalMinutes}";
            row["TimeToSampling"] = tr.EndHaulToSamplingDate.TotalHours;
            if (tr.NumberOfFishers == null)
            {
                row["Number of fishers"] = DBNull.Value;
            }
            else
            {
                row["Number of fishers"] = tr.NumberOfFishers;
            }
            if (tr.NumberOHoursFishing == null)
            {
                row["Number of hours fishing"] = DBNull.Value;
            }
            else
            {
                row["Number of hours fishing"] = tr.NumberOHoursFishing;
            }
            row["Weight of catch"] = tr.TotalWeightOfCatch;
            row["Undersized crab cutoff length"] = tr.CutoffLengfthForUndersize;
            if (tr.WeightOfCrabs == null)
            {
                row["Weight of crabs"] = DBNull.Value;
            }
            else
            {
                row["Weight of crabs"] = tr.WeightOfCrabs;
            }
            if (tr.CountMaturityMeasurements == 0)
            {

                row["Has crab maturity data"] = false;
                row["Berried crab weight"] = DBNull.Value;
                row["Berried crab count"] = DBNull.Value;
                row["Berried crab percent by weight"] = DBNull.Value;
                row["Number of males"] = DBNull.Value;
                row["Number of females"] = DBNull.Value;
                row["Number of maturity measurements"] = DBNull.Value;
                row["Undersized crab weight"] = DBNull.Value;
                row["Undersized crab count"] = DBNull.Value;
                row["Undersized crab percent by weight"] = DBNull.Value;
            }
            else
            {

                row["Has crab maturity data"] = true;
                row["Weight of crabs from maturity data"] = tr.CrabMaturityTotalWeight;
                row["Berried crab weight"] = tr.BerriedCrabTotalWeight;
                row["Berried crab count"] = tr.BerriedCrabCount;
                row["Berried crab percent by weight"] = tr.BerriedCrabPercentByWeight;
                row["Number of males"] = tr.CountMale;
                row["Number of females"] = tr.CountFemale;
                row["Number of maturity measurements"] = tr.CountMaturityMeasurements;
                if (tr.UndersizedCrabWeight == null)
                {
                    row["Undersized crab weight"] = DBNull.Value;
                    row["Undersized crab count"] = DBNull.Value;
                    row["Undersized crab percent by weight"] = DBNull.Value;
                }
                else
                {
                    row["Undersized crab weight"] = tr.UndersizedCrabWeight;
                    row["Undersized crab count"] = tr.UndersizedCrabCount;
                    row["Undersized crab percent by weight"] = tr.UndersizedCrabPercentByWeight;
                }

            }
            dt.Rows.Add(row);
        }
        _dataset.Tables.Add(dt);
        //return _dataset;
    }
    public int SetupSummaryList(DateTime? start = null, DateTime? end = null)
    {
        if (start != null)
        {
            if (end != null)
            {

            }
            else
            {

            }
        }
        else
        {
            SummaryRead?.Invoke(this, new TrackedOperationSummaryEventArgs { Intent = "start" });


            var landingList = NSAPEntities.VesselUnloadViewModel.GetAllVesselUnloads();
            foreach (VesselUnload unload in landingList)
            {
                foreach (VesselCatch vc in unload.ListVesselCatch)
                {
                    if (vc.ListCatchMaturity?.Count > 0)
                    {
                        _landingWithMaturityDataList.Add(new LandingWithMaturityData(unload, UndersizedCutoffLength));
                        break;
                    }

                }
            }


            var summaryList = NSAPEntities.VesselUnloadViewModel.GetAllVesselUnloads().Where(t => t.OperationIsTracked && t.GPS != null).ToList();
            int numberOfItems = summaryList.Count;

            SummaryRead?.Invoke(this, new TrackedOperationSummaryEventArgs { TotalCountSummary = numberOfItems, Intent = "setup" });

            foreach (var item in summaryList)
            {
                _trackedOperationSummarylist.Add(new TrackedOperationSummary(item, UndersizedCutoffLength, _landingCentroids));
                SummaryRead?.Invoke(this, new TrackedOperationSummaryEventArgs { CountOfSummaryRead = _trackedOperationSummarylist.Count, Intent = "reading" });
            }

            SummaryRead?.Invoke(this, new TrackedOperationSummaryEventArgs { Intent = "done" });
        }
        return _trackedOperationSummarylist.Count;
    }

    public TrackedOperationSummaryViewModel(DateTime start, DateTime end)
    {

    }

    public List<TrackedOperationSummary> TrackedOperationSummaryList
    {
        get { return _trackedOperationSummarylist; }
    }
    public int Count
    {
        get { return _trackedOperationSummarylist.Count; }
    }
}

