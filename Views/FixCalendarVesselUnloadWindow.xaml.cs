using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NSAP_ODK.Entities.Database;
using NSAP_ODK.Utilities;
using NSAP_ODK.Entities;
using System.Windows.Threading;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for FixCalendarVesselUnloadWindow.xaml
    /// </summary>
    public partial class FixCalendarVesselUnloadWindow : Window
    {
        private HashSet<CalendarDayLineage> _hashSetCDL;
        private DispatcherTimer _timer;
        private static FixCalendarVesselUnloadWindow _instance;
        private int _searchResults;
        private int _countToProcess;
        public static FixCalendarVesselUnloadWindow GetInstance()
        {
            if (_instance == null)
            {
                _instance = new FixCalendarVesselUnloadWindow();
            }
            return _instance;
        }
        public FixCalendarVesselUnloadWindow()
        {
            InitializeComponent();
            Loaded += FixCalendarVesselUnloadWindow_Loaded;
            dataGrid.LoadingRow += DataGrid_LoadingRow;
            dataGrid.IsReadOnly = true;
            dataGrid.CanUserAddRows = false;
            dataGrid.AutoGenerateColumns = false;
            Closing += FixCalendarVesselUnloadWindow_Closing;
        }
        private void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            rowStatus.Height = new GridLength(0);

        }
        private void FixCalendarVesselUnloadWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SavePlacement();
            _instance = null;
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            int rowNumber = e.Row.GetIndex() + 1;
            e.Row.Header = rowNumber.ToString();
            int? rowType = null;
            if (FishingCalendarViewModel != null)
            {
                var cdl = ((List<CalendarDayLineage>)dataGrid.DataContext).FirstOrDefault(t => t.RowID == rowNumber);
                rowType = cdl.RowType;
            }
            else
            {
                var si = ((List<SummaryItem>)dataGrid.DataContext).FirstOrDefault(t => (int)t.RowId == rowNumber);
                rowType = si.RowType;
            }
            if (rowType != null)
            {
                switch (rowType)
                {
                    case 2:
                        e.Row.Background = new SolidColorBrush(Colors.Beige);
                        break;
                    case 3:
                        e.Row.Background = new SolidColorBrush(Colors.Azure);
                        break;
                    case 1:
                        e.Row.Background = new SolidColorBrush(Colors.White);
                        break;
                }
            }
            else
            {
                e.Row.Background = new SolidColorBrush(Colors.White);
            }


        }

        public List<CalendarDayLineage> CalendarDayLineages { get; private set; } = new List<CalendarDayLineage>();
        private async void FixCalendarVesselUnloadWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _timer = new DispatcherTimer();
            _timer.Tick += OnTimerTick;

            buttonFix.IsEnabled = false;
            Title = "Identifying mismatch in number of sampled fish landings";

            if (FishingCalendarViewModel != null)
            {
                _hashSetCDL = new HashSet<CalendarDayLineage>();
                CalendarDayLineages = new List<CalendarDayLineage>();
                foreach (GearUnload gu in FishingCalendarViewModel.UnloadList)
                {
                    foreach (VesselUnload vu in gu.VesselUnloadViewModel.VesselUnloadCollection)
                    {
                        CalendarDayLineage cdl = new CalendarDayLineage
                        {
                            GearName = gu.GearUsedName,
                            SamplingDate = gu.Parent.SamplingDate,
                            SamplingDayID = gu.Parent.PK,
                            GearUnloadID = gu.PK,
                            VesselUnloadID = vu.PK,
                            VesselUnload = vu
                        };
                        CalendarDayLineages.Add(cdl);
                    }
                }
                var ds = CalendarDayLineages.OrderBy(t => t.GearName).ThenBy(t => t.SamplingDate).ThenBy(t => t.VesselUnloadID).ToList();

                int loopCount = 0;
                int grouping = 0;
                foreach (var item in ds)
                {
                    item.RowID = loopCount + 1;
                    if (loopCount > 0)
                    {
                        bool matching = item.GearName == ds[loopCount - 1].GearName && item.SamplingDate == ds[loopCount - 1].SamplingDate && item.GearUnloadID != ds[loopCount - 1].GearUnloadID;


                        if (matching && !_hashSetCDL.Contains(item))
                        {

                            var item2 = new CalendarDayLineage { SamplingDate = item.SamplingDate, GearName = item.GearName, SamplingDayID = item.SamplingDayID, GearUnloadID = item.GearUnloadID, VesselUnloadID = item.VesselUnloadID };
                            var itemMatch = _hashSetCDL.FirstOrDefault(t => t.VesselUnloadID != item.VesselUnloadID && t.GearName == item.GearName && t.SamplingDate == item.SamplingDate);
                            if (itemMatch == null)
                            {
                                _hashSetCDL.Add(item2);
                                grouping++;
                                item2.Grouping = grouping;
                            }
                        }
                    }
                    loopCount++;
                }
                foreach (var cd in _hashSetCDL)
                {
                    foreach (var item in ds)
                    {
                        if (item.Equals(cd))
                        {
                            item.Grouping = cd.Grouping;
                        }
                    }
                }
                var samplingDay = FishingCalendarViewModel.UnloadList[0].Parent;
                labelTitle.Content = $"{samplingDay.LandingSite.ToString()}, {samplingDay.SamplingDate.ToString("MMM-dd-yyyy")} ({_hashSetCDL.Count})";
                dataGrid.DataContext = ds;

                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Grouping", Binding = new Binding("Grouping") });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("GearName") });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Date", Binding = new Binding("SamplingDateString") });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Sampling day ID", Binding = new Binding("SamplingDayID") });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Gear unload ID", Binding = new Binding("GearUnloadID") });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Vessel unload ID", Binding = new Binding("VesselUnloadID") });
                if (ds.Count > 0)
                {
                    buttonFix.IsEnabled = true;
                }
            }
            else
            {
                rowStatus.Height = new GridLength(30);
                dataGrid.Visibility = Visibility.Collapsed;

                labelTitle.Content = $"Mismatch between count of fish landings sampled per day and number shown in a calendar day";
                SamplingCalendaryMismatchFixer.FixCalendarItemCountMismatchEvent += SamplingCalendaryMismatchFixer_FixCalendarItemCountMismatchEvent;
                _searchResults = await SamplingCalendaryMismatchFixer.SearchMismatchAsync();
                SamplingCalendaryMismatchFixer.FixCalendarItemCountMismatchEvent -= SamplingCalendaryMismatchFixer_FixCalendarItemCountMismatchEvent;
                ShowSearchResults();
            }
        }

        private void ShowSearchResults()
        {
            if (_searchResults > 0)
            {
                dataGrid.Visibility = Visibility.Visible;
                var ds = SamplingCalendaryMismatchFixer.MismatchSortResults.OrderBy(t => t.Grouping).ToList();
                dataGrid.DataContext = ds;
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Grouping", Binding = new Binding("Grouping") });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("EnumeratorNameToUse") });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("FMA.Name") });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("FishingGround.Name") });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("LandingSiteNameText") });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("GearUsedName") });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Date", Binding = new Binding("SamplingDayDateString") });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Sampling day ID", Binding = new Binding("SamplingDayID") });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Gear unload ID", Binding = new Binding("GearUnloadID") });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Vessel unload ID", Binding = new Binding("VesselUnloadID") });
                buttonFix.IsEnabled = true;
            }
            else
            {
                Visibility = Visibility.Collapsed;
                MessageBox.Show("No mismatched sampling calendar items was found", Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
                if(Owner!=null)
                {
                    Owner.Focus();
                }
            }

            labelTitle.Content = $"Mismatch between count of fish landings sampled per day and number shown in a calendar day ({SamplingCalendaryMismatchFixer.MismatchSortResults.Count})";


        }
        private void SamplingCalendaryMismatchFixer_FixCalendarItemCountMismatchEvent(object sender, FixCalendarMismatchEventArg e)
        {
            string processName = "";
            switch (e.Intent)
            {
                case "start sorting":
                case "start fixing":
                    progressBar.Dispatcher.BeginInvoke
    (
                      DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                      {


                          progressBar.IsIndeterminate = true;
                          if (e.Intent == "start fixing")
                          {
                              progressBar.IsIndeterminate = false;
                              _countToProcess = e.TotalCountToProcess;
                          }
                          progressBar.Value = 0;
                          progressBar.Maximum = e.TotalCountToProcess;
                          //do what you need to do on UI Thread
                          return null;
                      }), null);

                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressLabel.Content = "Sorting data. Please wait...";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    break;
                case "processing start":
                    progressBar.Dispatcher.BeginInvoke
    (
                      DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                      {

                          progressBar.IsIndeterminate = false;
                          _countToProcess = e.TotalCountToProcess;
                          progressBar.Maximum = _countToProcess;
                          progressBar.Value = 0;
                          //do what you need to do on UI Thread
                          return null;
                      }), null);
                    break;
                case "item sorted":
                case "item fixed":
                    processName = "Sorted";
                    if (e.Intent == "item fixed")
                    {
                        processName = "Fixed";
                    }
                    progressBar.Dispatcher.BeginInvoke
    (
                      DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                      {

                          progressBar.Value = e.CountProcessed;
                          //do what you need to do on UI Thread
                          return null;
                      }), null);

                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressLabel.Content = $"{processName} item {e.CountProcessed} of {_countToProcess}";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "done sorting":
                case "done fixing":
                    progressBar.Dispatcher.BeginInvoke
    (
                      DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                      {

                          progressBar.Value = 0;
                          //do what you need to do on UI Thread
                          return null;
                      }), null);

                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              processName = "sorting";
                              if (e.Intent == "done fixing")
                              {
                                  processName = "fixing";
                              }
                              progressLabel.Content = $"Finished {processName} {e.CountProcessed} items";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    _timer.Interval = TimeSpan.FromSeconds(3);
                    _timer.Start();
                    break;
            }
        }

        public FishingCalendarViewModel FishingCalendarViewModel { get; set; }
        private async void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Content)
            {
                case "Fix mismatch":
                    rowStatus.Height = new GridLength(30);
                    if (FishingCalendarViewModel != null)
                    {
                        SamplingCalendaryMismatchFixer.SetCalendarDayMismatchResults((List<CalendarDayLineage>)dataGrid.DataContext);
                    }

                    SamplingCalendaryMismatchFixer.FixCalendarItemCountMismatchEvent += SamplingCalendaryMismatchFixer_FixCalendarItemCountMismatchEvent;
                    var success = await SamplingCalendaryMismatchFixer.FixMismatchesAsync();
                    SamplingCalendaryMismatchFixer.FixCalendarItemCountMismatchEvent += SamplingCalendaryMismatchFixer_FixCalendarItemCountMismatchEvent;

                    if (success)
                    {
                        MessageBox.Show("Finished fixing mismatch in number of landings sampled in sampling calendar", Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                        Close();

                    }
                    else
                    {
                        MessageBox.Show("Fixing mismatch in number of landings sampled in sampling calendar was not completed", Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                    }


                    break;
                case "Cancel":
                    Close();
                    break;
            }
        }
    }
}
