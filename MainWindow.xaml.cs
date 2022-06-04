using NSAP_ODK.Entities;
using NSAP_ODK.Utilities;
using NSAP_ODK.Views;
using Ookii.Dialogs.Wpf;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Collections.Generic;
using NSAP_ODK.Entities.Database;
using System.Threading;
using Xceed.Wpf.Toolkit.PropertyGrid;
using System.Data;
using NSAP_ODK.Entities.Database.GPXparser;
using Microsoft.Win32;
using System.IO;
using System.Windows.Threading;
using System.Text;

namespace NSAP_ODK
{
    public enum DataDisplayMode
    {
        Dashboard,
        ODKData,
        Species,
        DownloadHistory,
        Others,
        DBSummary
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NSAPEntity _nsapEntity;
        private string _csvSaveToFolder = "";
        private FishingCalendarViewModel _fishingCalendarViewModel;
        private int _gridCol;
        private int _gridRow;
        private string _gearCode;
        private string _gearName;
        private DateTime _monthYear;
        private TreeViewModelControl.AllSamplingEntitiesEventHandler _treeItemData;
        private GearUnload _gearUnload;
        private List<GearUnload> _gearUnloads;
        private GearUnloadWindow _gearUnloadWindow;
        private Dictionary<DateTime, List<SummaryItem>> _vesselDownloadHistory;
        private DataDisplayMode _currentDisplayMode;
        private VesselUnloadWIndow _vesselUnloadWindow;
        private List<GearUnload> _gearUnloadList;
        private bool _saveChangesToGearUnload;
        private PropertyItem _selectedPropertyItem;
        private TreeViewItem _selectedTreeNode;
        private TreeViewModelControl.AllSamplingEntitiesEventHandler _allSamplingEntitiesEventHandler;
        private bool _acceptDataGridCellClick;
        private DispatcherTimer _timer;
        private DBSummary _dbSummary;
        private SummaryLevelType _summaryLevelType;
        private FishingGround _selectFishingGroundInSummary;
        private NSAPRegion _selectedRegionInSummary;
        private DataGrid _dataGrid;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
            Closing += OnWindowClosing;
        }

        public DataDisplayMode DataDisplayMode { get { return _currentDisplayMode; } }
        private void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            ShowStatusRow(false);

        }

        public void CloseAppilication()
        {
            Close();
        }
        private void OnWindowClosing(object sender, CancelEventArgs e)
        {

            foreach (Window w in Application.Current.Windows)
            {

                if (w.GetType().Name != "MainWindow")
                {

                    //we will close all open child windows of MainWindow
                    if (w.GetType().Name == "EditWindowEx")
                    {
                        ((EditWindowEx)w).CloseCommandFromMainWindow = true;
                        w.Close();
                    }
                }
            }

            CrossTabManager.CrossTabEvent -= OnCrossTabEvent;
            NSAPEntities.SummaryItemViewModel.BuildingSummaryTable -= SummaryItemViewModel_BuildingSummaryTable;
            NSAPEntities.SummaryItemViewModel.BuildingOrphanedEntity -= SummaryItemViewModel_BuildingOrphanedEntity;
        }


        public bool ShowSplash()
        {
            SplashWindow sw = new SplashWindow();
            sw.Owner = this;
            if ((bool)sw.ShowDialog())
            {
                return (bool)sw.DialogResult;
            }
            return false;
        }

        private Style AlignRightStyle
        {
            get
            {
                Style alignRightCellStype = new Style(typeof(DataGridCell));

                // Create a Setter object to set (get it? Setter) horizontal alignment.
                Setter setAlign = new
                    Setter(HorizontalAlignmentProperty,
                    HorizontalAlignment.Right);

                // Bind the Setter object above to the Style object
                alignRightCellStype.Setters.Add(setAlign);
                return alignRightCellStype;
            }
        }
        public void ShowSummary(string level)
        {
            panelVersionStats.Visibility = Visibility.Collapsed;
            rowSummary.Height = new GridLength(1, GridUnitType.Star);
            propertyGridSummary.Visibility = Visibility.Collapsed;




            switch (level)
            {
                case "Overall":

                    NSAPEntities.DBSummary.Refresh();
                    labelSummary.Content = "Overall summary of database content";
                    propertyGridSummary.SelectedObject = NSAPEntities.DBSummary;
                    propertyGridSummary.NameColumnWidth = 350;
                    propertyGridSummary.AutoGenerateProperties = false;

                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Database", Name = "DBPath", Description = "Path to database. Double click to open folder containing the database.", DisplayOrder = 1, Category = "Database" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of NSAP Regions", Name = "NSAPRegionCount", Description = "Number of NSAP Regions", DisplayOrder = 2, Category = "Lookup choices" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of FMAS", Name = "FMACount", Description = "Number of FMAs", DisplayOrder = 3, Category = "Lookup choices" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of fishing grounds", Name = "FishingGroundCount", Description = "Number of fishing grounds", DisplayOrder = 4, Category = "Lookup choices" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of landing sites", Name = "LandingSiteCount", Description = "Number of landing sites", DisplayOrder = 5, Category = "Lookup choices" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of fish species", Name = "FishSpeciesCount", Description = "Number of fish species", DisplayOrder = 6, Category = "Lookup choices" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of non-fish species", Name = "NonFishSpeciesCount", Description = "Number of non-fish species", DisplayOrder = 7, Category = "Lookup choices" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number fishing gears", Name = "FishingGearCount", Description = "Number of fishing gears", DisplayOrder = 8, Category = "Lookup choices" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of effort specs", Name = "GearSpecificationCount", Description = "Number of effort specifications", DisplayOrder = 9, Category = "Lookup choices" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of GPS", Name = "GPSCount", Description = "Number of GPS", DisplayOrder = 10, Category = "Lookup choices" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of enumerators", Name = "EnumeratorCount", Description = "Number of NSAP enumerators", DisplayOrder = 11, Category = "Lookup choices" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of fishing vessels", Name = "FishingVesselCount", Description = "Number of fishing vessels", DisplayOrder = 12, Category = "Lookup choices" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of gear unload", Name = "GearUnloadCount", Description = "Number of gear unload", DisplayOrder = 13, Category = "Submitted fish landing data" });

                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of complete gear unload", Name = "CountCompleteGearUnload", Description = "Number of gear unload", DisplayOrder = 14, Category = "Submitted fish landing data" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of landings", Name = "VesselUnloadCount", Description = "Number of vessel unload", DisplayOrder = 15, Category = "Submitted fish landing data" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of tracked operations", Name = "TrackedOperationsCount", Description = "Number of tracked fishing operations", DisplayOrder = 16, Category = "Submitted fish landing data" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Date of first sampled landing", Name = "FirstSampledLandingDate", Description = "Date of first sampled operation", DisplayOrder = 17, Category = "Submitted fish landing data" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Date of last sampled landing", Name = "LastSampledLandingDate", Description = "Date of last sampled operation", DisplayOrder = 18, Category = "Submitted fish landing data" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Date of latest download", Name = "DateLastDownload", Description = "Date of latest download", DisplayOrder = 19, Category = "Submitted fish landing data" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of landings including catch composition", Name = "CountLandingsWithCatchComposition", Description = "Number of sampled landings with included catch composition data", DisplayOrder = 20, Category = "Submitted fish landing data" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of missing Form IDs", Name = "CountMissingFormIDs", Description = "Number of saved forms with missing identifier", DisplayOrder = 21, Category = "Submitted fish landing data" });


                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Saved JSON files folder", Name = "SavedJSONFolder", Description = "Folder containing saved JSON data. Double click to open folder", DisplayOrder = 1, Category = "Saved JSON files" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of saved catch and effort monitoring JSON files", Name = "SavedFishingEffortJSONCount", Description = "Number of saved JSON files containing catch and effort monitoring data", DisplayOrder = 2, Category = "Saved JSON files" });

                    propertyGridSummary.Visibility = Visibility.Visible;

                    break;
                case "Enumerators and form versions":
                    labelSummary.Content = "Enumerators and latest eForm versions";
                    panelVersionStats.Visibility = Visibility.Visible;
                    dataGridEFormVersionStats.AutoGenerateColumns = false;
                    dataGridEFormVersionStats.Columns.Clear();
                    dataGridEFormVersionStats.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("NSAPEnumerator") });
                    dataGridEFormVersionStats.Columns.Add(new DataGridTextColumn { Header = "Form version", Binding = new Binding("FormVersion"), CellStyle = AlignRightStyle });

                    DataGridTextColumn col = new DataGridTextColumn()
                    {
                        Binding = new Binding("LastSamplingDate"),
                        Header = "Date of last sampling",
                        CellStyle = AlignRightStyle
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy";
                    dataGridEFormVersionStats.Columns.Add(col);


                    dataGridEFormVersionStats.DataContext = NSAPEntities.SummaryItemViewModel.EnumeratorsAndLatestFormVersion();
                    break;
                case "e-Form versions":
                    labelSummary.Content = "eForm versions, number of submitted landings and date of first submission";
                    panelVersionStats.Visibility = Visibility.Visible;
                    dataGridEFormVersionStats.AutoGenerateColumns = false;
                    dataGridEFormVersionStats.Columns.Clear();
                    dataGridEFormVersionStats.Columns.Add(new DataGridTextColumn { Header = "Version", Binding = new Binding("Version") });
                    dataGridEFormVersionStats.Columns.Add(new DataGridTextColumn { Header = "Number submitted", Binding = new Binding("Count"), CellStyle = AlignRightStyle });


                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("FirstSubmission"),
                        Header = "Date first submitted",
                        CellStyle = AlignRightStyle
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy";
                    dataGridEFormVersionStats.Columns.Add(col);

                    NSAPEntities.ODKEformVersionViewModel.Refresh();
                    dataGridEFormVersionStats.DataContext = NSAPEntities.ODKEformVersionViewModel.ODKEformVersionCollection.ToList();
                    break;
                case "Enumerators":
                    ShowStatusRow(isVisible: false);
                    NSAPEntities.DatabaseEnumeratorSummary.Refresh();
                    //propertyGridSummary.Properties.Clear();
                    labelSummary.Content = "Summary of enumerators";
                    propertyGridSummary.SelectedObject = NSAPEntities.DatabaseEnumeratorSummary;
                    propertyGridSummary.NameColumnWidth = 350;
                    propertyGridSummary.AutoGenerateProperties = false;

                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Total in database", Name = "TotalInDatabase", Description = "Total number of enumerators saved in the database", DisplayOrder = 1, Category = "Overall" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of enumerators with catch and effort data", Name = "CountWithVesselLandingRecords", Description = "Count of enumerators having catch and effort sampling data", DisplayOrder = 2, Category = "Overall" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Ilocos Region", Name = "CountIlocos", Description = "Number of enumerators in Ilocos Region", DisplayOrder = 3, Category = "Number of enumerators by region" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Cagayan Valley", Name = "CountCagayanValley", Description = "Number of enumerators in Cagayan Valley", DisplayOrder = 4, Category = "Number of enumerators by region" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Central Luzon", Name = "CountCentralLuzon", Description = "Number of enumerators in Central Luzon", DisplayOrder = 5, Category = "Number of enumerators by region" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "CALABARZON", Name = "CountCalabarzon", Description = "Number of enumerators in CALABARZON", DisplayOrder = 6, Category = "Number of enumerators by region" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "MIMAROPA", Name = "CountMimaropa", Description = "Number of enumerators in MIMAROPA", DisplayOrder = 7, Category = "Number of enumerators by region" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Bicol Region", Name = "CountBicol", Description = "Number of enumerators in Bicol Region", DisplayOrder = 8, Category = "Number of enumerators by region" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Western Visayas", Name = "CountWesternVisayas", Description = "Number of enumerators in Western Visayas", DisplayOrder = 9, Category = "Number of enumerators by region" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Central Visayas", Name = "CountCentralVisayas", Description = "Number of enumerators in Central Visayas", DisplayOrder = 10, Category = "Number of enumerators by region" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Eastern Visayas", Name = "CountEasternVisayas", Description = "Number of enumerators in Eastern Visayas", DisplayOrder = 11, Category = "Number of enumerators by region" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Zamboanga Peninsula", Name = "CountZamboangaPeninsula", Description = "Number of enumerators in Zamboanga Peninsula", DisplayOrder = 12, Category = "Number of enumerators by region" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Northern Mindanao", Name = "CountNorthernMindanao", Description = "Number of enumerators in Northern Mindanao", DisplayOrder = 13, Category = "Number of enumerators by region" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Davao Region", Name = "CountDavao", Description = "Number of enumerators in Davao", DisplayOrder = 14, Category = "Number of enumerators by region" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Soccsksargen", Name = "CountSoccsksargen", Description = "Number of enumerators in Soccsksargen", DisplayOrder = 15, Category = "Number of enumerators by region" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Caraga Region", Name = "CountCaraga", Description = "Number of enumerators in Caraga Region", DisplayOrder = 16, Category = "Number of enumerators by region" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "BARMM", Name = "CountBARMM", Description = "Number of enumerators in BARMM", DisplayOrder = 17, Category = "Number of enumerators by region" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "NCR", Name = "CountNCR", Description = "Number of enumerators in NCR", DisplayOrder = 18, Category = "Number of enumerators by region" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "CAR", Name = "CountCar", Description = "Number of enumerators in CAR", DisplayOrder = 19, Category = "Number of enumerators by region" });


                    propertyGridSummary.Visibility = Visibility.Visible;
                    break;
                case "Databases":
                    panelVersionStats.Visibility = Visibility.Visible;
                    labelSummary.Content = "Summary of online databases (Kobotoolbox)";
                    dataGridEFormVersionStats.AutoGenerateColumns = false;
                    dataGridEFormVersionStats.Columns.Clear();

                    dataGridEFormVersionStats.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("ServerNumericID") });
                    dataGridEFormVersionStats.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("FormName") });
                    dataGridEFormVersionStats.Columns.Add(new DataGridTextColumn { Header = "ID2", Binding = new Binding("ServerID") });
                    dataGridEFormVersionStats.Columns.Add(new DataGridTextColumn { Header = "e-Form version", Binding = new Binding("eFormVersion") });

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("DateCreated"),
                        Header = "Date created",
                        CellStyle = AlignRightStyle
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    dataGridEFormVersionStats.Columns.Add(col);

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("DateModified"),
                        Header = "Date modified",
                        CellStyle = AlignRightStyle
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    dataGridEFormVersionStats.Columns.Add(col);

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("DateLastAccessed"),
                        Header = "Date last accessed",
                        CellStyle = AlignRightStyle
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    dataGridEFormVersionStats.Columns.Add(col);

                    dataGridEFormVersionStats.Columns.Add(new DataGridTextColumn { Header = "# submissions", Binding = new Binding("SubmissionCount"), CellStyle = AlignRightStyle });
                    dataGridEFormVersionStats.Columns.Add(new DataGridTextColumn { Header = "# saved", Binding = new Binding("SavedInDBCount"), CellStyle = AlignRightStyle });
                    dataGridEFormVersionStats.Columns.Add(new DataGridCheckBoxColumn { Header = "Fish landing form", Binding = new Binding("IsFishLandingSurveyForm") });
                    dataGridEFormVersionStats.Columns.Add(new DataGridTextColumn { Header = "Last uploaded JSON file", Binding = new Binding("LastUploadedJSON") });
                    dataGridEFormVersionStats.Columns.Add(new DataGridTextColumn { Header = "Last created JSON file", Binding = new Binding("LastCreatedJSON") });

                    if (NSAPEntities.KoboServerViewModel.Count() > 0)
                    {
                        NSAPEntities.KoboServerViewModel.RefreshSavedCount();
                        dataGridEFormVersionStats.DataContext = NSAPEntities.KoboServerViewModel.KoboserverCollection.ToList();
                    }
                    else
                    {
                        dataGridEFormVersionStats.DataContext = null;
                        MessageBox.Show("Please connect online to the Kobotoolbox server to get a list of databases",
                            "NSAP-ODK Database",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                            );
                    }
                    break;
            }



        }


        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            Global.RequestLogIn += OnMysQLRequestLogin;

            Global.DoAppProceed();
            if (Global.AppProceed)
            {
                _currentDisplayMode = DataDisplayMode.Dashboard;
                SetDataDisplayMode();

                if (!Global.Settings.UsemySQL || !Global.MySQLLogInCancelled)
                {
                    ShowSplash();
                    //CSVFIleManager.ReadCSVXML();
                    if (
                        NSAPEntities.NSAPRegionViewModel.Count > 0 &&
                        NSAPEntities.FishSpeciesViewModel.Count > 0 &&
                        NSAPEntities.NotFishSpeciesViewModel.Count > 0 &&
                        NSAPEntities.FMAViewModel.Count > 0
                        )
                    {

                        buttonDelete.IsEnabled = false;
                        buttonEdit.IsEnabled = false;
                        dbPathLabel.Content = Global.MDBPath;
                        menuDatabaseSummary.IsChecked = true;

                        CrossTabManager.CrossTabEvent += OnCrossTabEvent;
                        _timer = new DispatcherTimer();
                        _timer.Tick += OnTimerTick;
                    }
                    else
                    {
                        ShowDatabaseNotFoundView();
                    }
                    mainStatusLabel.Content = string.Empty;
                }
                else
                {
                    ShowDatabaseNotFoundView();
                    mainStatusLabel.Content = "Application database not found";
                }
                SetMenuAndOtherToolbarButtonsVisibility(Visibility.Visible);
                if (NSAPEntities.SummaryItemViewModel != null)
                {
                    NSAPEntities.SummaryItemViewModel.BuildingSummaryTable += SummaryItemViewModel_BuildingSummaryTable;
                    NSAPEntities.SummaryItemViewModel.BuildingOrphanedEntity += SummaryItemViewModel_BuildingOrphanedEntity;
                }
            }

            if (Global.Settings != null && Global.Settings.UsemySQL)
            {
                Title += " - MySQL";
            }
            else if (Global.Settings == null)
            {
                ResetDisplay();
                ShowDatabaseNotFoundView();
                //MessageBox.Show(
                //    "The application cannot find the MDB file for saving fish landing data\r\nClick on the Setting toolbar button",
                //    "NSAP-ODK Database",
                //    MessageBoxButton.OK,
                //    MessageBoxImage.Information
                //    );
            }


        }
        private void SummaryItemViewModel_BuildingOrphanedEntity(object sender, BuildOrphanedEntityEventArg e)
        {
            switch (e.BuildOrphanedEntityStatus)
            {
                case BuildOrphanedEntityStatus.StatusBuildStart:
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              mainStatusBar.IsIndeterminate = e.IsIndeterminate;
                              if (!mainStatusBar.IsIndeterminate)
                              {
                                  mainStatusBar.Maximum = e.TotalCount;
                              }
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = "Building orphaned items. Please wait...";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case BuildOrphanedEntityStatus.StatusBuildFirstRecordFound:
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              //mainStatusBar.IsIndeterminate = false;
                              mainStatusBar.Maximum = e.TotalCount;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = "First orphaned item found";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case BuildOrphanedEntityStatus.StatusBuildFetchedRow:
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusBar.IsIndeterminate = false;
                              mainStatusBar.Value = e.CurrentCount;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = $"Found orphaned item {e.CurrentCount} of {mainStatusBar.Maximum}";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case BuildOrphanedEntityStatus.StatusBuildEnd:
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusBar.Value = 0;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = $"Finished getting {e.TotalCountFetched} orphaned items.";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    _timer.Dispatcher.BeginInvoke
                        (
                        DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                        {
                            _timer.Interval = TimeSpan.FromSeconds(3);
                            _timer.Start();
                            return null;
                        }
                        ), null);
                    break;
            }
        }
        private void SummaryItemViewModel_BuildingSummaryTable(object sender, BuildSummaryReportEventArg e)
        {
            switch (e.BuildSummaryReportStatus)
            {
                case BuildSummaryReportStatus.StatusBuildStart:

                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              mainStatusBar.IsIndeterminate = e.IsIndeterminate;
                              if (!mainStatusBar.IsIndeterminate)
                              {
                                  mainStatusBar.Maximum = e.TotalRowCount;
                              }
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = $"Building summary table. Please wait...";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case BuildSummaryReportStatus.StatusBuildFetchedRow:
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              mainStatusBar.Value = e.CurrentRow;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = $"Fetched table row {e.CurrentRow} of {mainStatusBar.Value}";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case BuildSummaryReportStatus.StatusBuildEnd:
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusBar.Value = 0;
                              mainStatusBar.IsIndeterminate = false;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = $"Completed summary table with {e.TotalRowCount} rows";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    _timer.Dispatcher.BeginInvoke
                        (
                        DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                        {
                            _timer.Interval = TimeSpan.FromSeconds(3);
                            _timer.Start();
                            return null;
                        }
                        ), null);
                    break;
            }
        }

        private void HideMainWindowUI(bool isHidden = true)
        {
            if (isHidden)
            {
                GridMain.Visibility = Visibility.Collapsed;
            }
            else
            {
                GridMain.Visibility = Visibility.Visible;
            }
        }
        private void OnMysQLRequestLogin(object sender, EventArgs e)
        {
            ResetDisplay();
            LogInMySQLWindow logInWindow = new LogInMySQLWindow();
            if ((bool)logInWindow.ShowDialog())
            {
                HideMainWindowUI(false);
                menuBackupMySQL.Visibility = Visibility.Visible;
                if (NSAPMysql.MySQLConnect.UserCanCreateDatabase)
                {
                    menuSetupMySQLTables.Visibility = Visibility.Visible;
                    //menuUpdateUnloadStats.Visibility = Visibility.Visible;
                }
            }
            else
            {
                Global.MySQLLogInCancelled = true;
                if (NSAPMysql.MySQLConnect.LastError == null || NSAPMysql.MySQLConnect.LastError.Length == 0)
                {
                    Close();
                }
            }
            Global.RequestLogIn -= OnMysQLRequestLogin;
        }

        private void OnCrossTabEvent(object sender, CrossTabReportEventArg e)
        {
            //mainStatusBar.IsIndeterminate = false;
            switch (e.Context)
            {
                case "FilteringCatchData":

                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusBar.IsIndeterminate = true;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = $"Filtering catch data. Please wait...";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    break;

                case "Start":

                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              //mainStatusBar.IsIndeterminate = false;
                              mainStatusBar.Maximum = e.RowsToPrepare;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = $"Preparing to add rows";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "RowsPrepared":
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusBar.IsIndeterminate = false;
                              //mainStatusBar.Maximum = e.RowsToPrepare;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);


                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = $"Rows prepared. Please wait for rows to start adding...";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "AddingRows":
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                            {
                                mainStatusBar.Value = e.RowsPrepared;
                                //do what you need to do on UI Thread
                                return null;
                            }
                         ), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = $"Added {e.RowsPrepared} of {e.RowsToPrepare} vessel unloads";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    break;
                case "PreparingDisplayRows":
                    mainStatusBar.Dispatcher.BeginInvoke
                         (
                           DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                             {
                                 mainStatusBar.IsIndeterminate = true;
                                 //do what you need to do on UI Thread
                                 return null;
                             }
                          ), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = $"Preparing to show results. Please wait...";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    break;
                case "DoneAddingRows":
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                            {
                                mainStatusBar.Value = 0;
                                //do what you need to do on UI Thread
                                return null;
                            }
                         ), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = $"Finished adding vessel unloads";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    _timer.Dispatcher.BeginInvoke
                        (
                        DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                        {
                            _timer.Interval = TimeSpan.FromSeconds(3);
                            _timer.Start();
                            return null;
                        }
                        ), null);


                    break;
            }


        }

        private void ShowStatusRow(bool isVisible = true)
        {
            if (!isVisible)
            {
                rowStatus.Height = new GridLength(0);
            }
            else
            {
                rowStatus.Height = new GridLength(30, GridUnitType.Pixel);
            }

        }
        private void ShowTitleAndStatusRow(bool isVisible = true)
        {

            rowTopLabel.Height = new GridLength(30, GridUnitType.Pixel);
            ShowStatusRow(isVisible:false);
            PanelButtons.Visibility = Visibility.Visible;
        }
        private void ResetDisplay()
        {
            rowDashboard.Height = new GridLength(0);
            rowTopLabel.Height = new GridLength(0);
            rowSpecies.Height = new GridLength(0);
            rowODKData.Height = new GridLength(0);
            rowOthers.Height = new GridLength(0);
            rowSummary.Height = new GridLength(0);
            rowStatus.Height = new GridLength(0);

            StackPanelDashboard.Visibility = Visibility.Collapsed;
            PanelButtons.Visibility = Visibility.Collapsed;
            GridNSAPData.Visibility = Visibility.Collapsed;
            PropertyGrid.Visibility = Visibility.Collapsed;
            gridCalendarHeader.Visibility = Visibility.Collapsed;
            samplingTree.Visibility = Visibility.Collapsed;
            treeViewDownloadHistory.Visibility = Visibility.Collapsed;
            panelVersionStats.Visibility = Visibility.Collapsed;


        }


        public void SetDataDisplayMode()
        {
            ResetDisplay();
            switch (_currentDisplayMode)
            {
                case DataDisplayMode.DownloadHistory:
                    rowODKData.Height = new GridLength(1, GridUnitType.Star);
                    treeViewDownloadHistory.Visibility = Visibility.Visible;
                    RefreshDownloadHistory();

                    treeViewDownloadHistory.Items.Clear();
                    int n = 0;
                    TreeViewItem item_0 = null;
                    foreach (var item in _vesselDownloadHistory.Keys)
                    {
                        TreeViewItem tvItem = new TreeViewItem { Header = item.ToString("MMM-dd-yyyy") };
                        tvItem.Tag = "downloadDate";
                        treeViewDownloadHistory.Items.Add(tvItem);


                        tvItem.Items.Add(new TreeViewItem { Header = "All fishing effort", Tag = "effort" });
                        tvItem.Items.Add(new TreeViewItem { Header = "Tracked fishing effort", Tag = "tracked" });
                        tvItem.Items.Add(new TreeViewItem { Header = "Gear unload", Tag = "gearUnload" });
                        tvItem.Items.Add(new TreeViewItem { Header = "Unload summary", Tag = "unloadSummary" });
                        if (n == 0)
                        {
                            item_0 = tvItem;
                        }
                        n++;
                    }
                    if (treeViewDownloadHistory.Items.Count > 0)
                    {
                        item_0.IsSelected = true;
                        item_0.IsExpanded = true;

                        //((TreeViewItem)treeViewDownloadHistory.Items[0]).IsSelected = true;
                        //((TreeViewItem)treeViewDownloadHistory.Items[0]).IsExpanded = true;
                    }

                    GridNSAPData.Visibility = Visibility.Visible;
                    GridNSAPData.SelectionUnit = DataGridSelectionUnit.FullRow;

                    break;
                case DataDisplayMode.Dashboard:
                    break;
                case DataDisplayMode.ODKData:
                    GridNSAPData.SelectionUnit = DataGridSelectionUnit.Cell;
                    GridNSAPData.SetValue(Grid.ColumnSpanProperty, 2);
                    rowODKData.Height = new GridLength(1, GridUnitType.Star);
                    samplingTree.Visibility = Visibility.Visible;
                    break;
                case DataDisplayMode.Species:
                    rowSpecies.Height = new GridLength(1, GridUnitType.Star);
                    ShowTitleAndStatusRow();
                    break;
                case DataDisplayMode.Others:
                    rowOthers.Height = new GridLength(1, GridUnitType.Star);
                    ShowTitleAndStatusRow();
                    break;
                case DataDisplayMode.DBSummary:
                    //ShowSummary();
                    summaryTreeNodeRegion.Items.Clear();
                    summaryTreeNodeEnumerators.Items.Clear();


                    //TreeViewItem tvi = new TreeViewItem { Header = "All regions", Tag = "EnumeratorAllRegions" };
                    //tvi.Expanded += OnSuumaryTreeItemExpanded;
                    //summaryTreeNodeEnumerators.Items.Add(tvi);

                    foreach (var region in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection)
                    {
                        TreeViewItem item = new TreeViewItem { Header = region.ShortName, Tag = region };
                        item.Expanded += OnSuumaryTreeItemExpanded;
                        item.Items.Add(new TreeViewItem { Header = "**dummy" });
                        summaryTreeNodeRegion.Items.Add(item);

                        TreeViewItem item1 = new TreeViewItem { Header = region.ShortName, Tag = region };
                        item1.Expanded += OnSuumaryTreeItemExpanded;
                        item1.Items.Add(new TreeViewItem { Header = "**dummy" });
                        summaryTreeNodeEnumerators.Items.Add(item1);
                    }
                    summaryTreeNodeOverall.IsSelected = true;

                    break;
            }
        }

        private void OnSuumaryTreeItemExpanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem node = (TreeViewItem)e.OriginalSource;
            if (node.Items.Count > 0)
            {
                TreeViewItem firstChild = (TreeViewItem)node.Items[0];
                if (firstChild.Header.ToString() == "**dummy")
                {
                    node.Items.Clear();
                    switch (((TreeViewItem)node.Parent).Header)
                    {
                        case "Enumerators":

                            var enumerators = NSAPEntities.NSAPRegionViewModel.GetEnumeratorsInRegion((NSAPRegion)node.Tag);
                            if (enumerators.Count > 0)
                            {
                                var enumSummary = new NSAPEnumerator { ID = -1, Name = "Summary of enumerators" };
                                TreeViewItem enumeratorNode = new TreeViewItem { Header = enumSummary.Name, Tag = enumSummary };
                                node.Items.Add(enumeratorNode);
                            }
                            foreach (var enumerator in enumerators)
                            {
                                TreeViewItem enumeratorNode = new TreeViewItem { Header = enumerator.Name, Tag = enumerator };
                                enumeratorNode.Items.Add(new TreeViewItem { Header = "**dummy" });
                                node.Items.Add(enumeratorNode);
                            }
                            break;
                        case "Regions":
                            foreach (var fg in NSAPEntities.NSAPRegionViewModel.GetFishingGrounds((NSAPRegion)node.Tag))
                            {
                                TreeViewItem fgNode = new TreeViewItem { Header = fg.Name, Tag = fg };
                                node.Items.Add(fgNode);
                            }
                            break;
                        default:
                            NSAPEnumerator en = (NSAPEnumerator)node.Tag;
                            foreach (var month in NSAPEntities.SummaryItemViewModel.GetMonthsSampledByEnumerator(en))
                            {
                                node.Items.Add(new TreeViewItem { Header = month.ToString("MMM-yyyy"), Tag = month });
                            }
                            //foreach (var month in NSAPEntities.VesselUnloadViewModel.MonthsSampledByEnumerator(en))
                            //{
                            //    node.Items.Add(new TreeViewItem { Header = month.ToString("MMM-yyyy"), Tag = month });
                            //}
                            break;
                    }
                }
            }
        }

        public void SetMenuAndOtherToolbarButtonsVisibility(Visibility visibility)
        {
            buttonCalendar.Visibility = visibility;
            buttonDownloadHistory.Visibility = visibility;
            buttonSummary.Visibility = visibility;
            buttonGeneratecsv.Visibility = visibility;

            menuFile.Visibility = visibility;
            menuEdit.Visibility = visibility;
            menuNSAPData.Visibility = visibility;
            menuGenerateCSV.Visibility = visibility;



            if (visibility == Visibility.Collapsed)
            {
                menuFile2.Visibility = Visibility.Visible;
            }
            else if (visibility == Visibility.Visible)
            {
                menuFile2.Visibility = Visibility.Collapsed;
            }

        }


        private void ShowDatabaseNotFoundView()
        {
            rowTopLabel.Height = new GridLength(300);
            labelTitle.Content = "Backend database file not found\r\nMake sure that the correct database is found in the application folder\r\n" +
                                  "The application folder is the folder where you installed this software\r\n\r\n" +
                                  "Click on the Settings button in the toolbar to setup the database";
            //if (CSVFIleManager.XMLError.Length > 0 && Global.MDBPath.Length > 0)
            //{
            //    labelTitle.Content = $"{CSVFIleManager.XMLError }";
            //}
            //else
            //{
            //    labelTitle.Content += $"\r\n\r\n{CSVFIleManager.XMLError }";
            //}
            labelTitle.FontSize = 18;
            labelTitle.FontWeight = FontWeights.Bold;
            labelTitle.VerticalContentAlignment = VerticalAlignment.Center;
            labelTitle.HorizontalContentAlignment = HorizontalAlignment.Center;
            labelTitle.Visibility = Visibility.Visible;

            rowDashboard.Height = new GridLength(0);
            rowODKData.Height = new GridLength(0);
            rowSpecies.Height = new GridLength(0);
            rowOthers.Height = new GridLength(0);
            rowStatus.Height = new GridLength(0);

            dataGridSpecies.Visibility = Visibility.Collapsed;
            PanelButtons.Visibility = Visibility.Collapsed;

            SetMenuAndOtherToolbarButtonsVisibility(Visibility.Collapsed);
        }

        private void LoadDataGrid()
        {
            buttonAdd.IsEnabled = true;

            if (_nsapEntity == NSAPEntity.FishSpecies)
            {
                dataGridSpecies.ItemsSource = null;
                dataGridSpecies.Columns.Clear();
                dataGridEntities.Visibility = Visibility.Collapsed;
                dataGridSpecies.Visibility = Visibility.Visible;
                //dataGridSpecies.Items.Clear();
            }
            else if (_nsapEntity != NSAPEntity.FishSpecies)
            {
                dataGridEntities.ItemsSource = null;
                //dataGridEntities.Items.Clear();
                dataGridEntities.Columns.Clear();
                dataGridEntities.Visibility = Visibility.Visible;
                dataGridSpecies.Visibility = Visibility.Collapsed;
            }

            SetDataGridSource();

            switch (_nsapEntity)
            {
                case NSAPEntity.GPS:
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Code", Binding = new Binding("Code") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Assigned name", Binding = new Binding("AssignedName") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Brand", Binding = new Binding("Brand") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Model", Binding = new Binding("Model") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Device type", Binding = new Binding("DeviceTypeString") });
                    break;
                case NSAPEntity.Province:
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Province ID", Binding = new Binding("ProvinceID"), Visibility = Visibility.Hidden });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Province name", Binding = new Binding("ProvinceName") });
                    break;

                case NSAPEntity.FishSpecies:

                    dataGridSpecies.Columns.Add(new DataGridTextColumn { Header = "RowNumber", Binding = new Binding("RowNumber"), Visibility = Visibility.Hidden });
                    dataGridSpecies.Columns.Add(new DataGridTextColumn { Header = "Fishbase species ID", Binding = new Binding("SpeciesCode") });
                    dataGridSpecies.Columns.Add(new DataGridTextColumn { Header = "Genus", Binding = new Binding("GenericName") });
                    dataGridSpecies.Columns.Add(new DataGridTextColumn { Header = "Species", Binding = new Binding("SpecificName") });
                    dataGridSpecies.Columns.Add(new DataGridTextColumn { Header = "Family", Binding = new Binding("Family") });
                    dataGridSpecies.Columns.Add(new DataGridTextColumn { Header = "Importance", Binding = new Binding("Importance") });
                    dataGridSpecies.Columns.Add(new DataGridTextColumn { Header = "Main catching method", Binding = new Binding("MainCatchingMethod") });
                    dataGridSpecies.Columns.Add(new DataGridTextColumn { Header = "Max length", Binding = new Binding("LengthMax") });
                    dataGridSpecies.Columns.Add(new DataGridTextColumn { Header = "Common length", Binding = new Binding("LengthCommon") });
                    dataGridSpecies.Columns.Add(new DataGridTextColumn { Header = "Max length type", Binding = new Binding("LengthType.Code") });
                    dataGridSpecies.Columns.Add(new DataGridTextColumn { Header = "Synonym", Binding = new Binding("Synonym") });
                    break;

                case NSAPEntity.FMA:
                    buttonAdd.IsEnabled = false;
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("ID"), Visibility = Visibility.Hidden });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("Name") });
                    break;

                case NSAPEntity.Enumerator:

                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("ID"), Visibility = Visibility.Hidden });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("Name") });
                    break;

                case NSAPEntity.FishingGear:

                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Code", Binding = new Binding("Code") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("GearName") });
                    dataGridEntities.Columns.Add(new DataGridCheckBoxColumn { Header = "Is generic gear", Binding = new Binding("IsGenericGear") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Base gear", Binding = new Binding("BaseGear") });
                    break;

                case NSAPEntity.FishingGround:
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Code", Binding = new Binding("Code") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("Name") });
                    break;

                case NSAPEntity.FishingVessel:

                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("ID") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("Name") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Name of owner", Binding = new Binding("NameOfOwner") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("FisheriesSector") });

                    break;

                case NSAPEntity.LandingSite:

                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("LandingSiteID") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("LandingSiteName") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Provice", Binding = new Binding("Municipality.Province.ProvinceName") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Municipality", Binding = new Binding("Municipality.MunicipalityName") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Barangay", Binding = new Binding("Barangay") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Longitude", Binding = new Binding("Longitude") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Latitude", Binding = new Binding("Latitude") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("WhereUsed") });
                    break;

                case NSAPEntity.NSAPRegion:
                    buttonAdd.IsEnabled = false;
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Code", Binding = new Binding("Code") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Region name", Binding = new Binding("Name") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Short name", Binding = new Binding("ShortName") });

                    break;

                case NSAPEntity.EffortIndicator:
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("ID"), Visibility = Visibility.Hidden });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("Name") });
                    dataGridEntities.Columns.Add(new DataGridCheckBoxColumn { Header = "Universal effort indicator", Binding = new Binding("IsForAllTypesFishing") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Type of value", Binding = new Binding("ValueTypeString") });
                    break;

                case NSAPEntity.NSAPRegionWithEntities:
                    break;

                case NSAPEntity.NonFishSpecies:
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Species ID", Binding = new Binding("SpeciesID") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Taxa", Binding = new Binding("Taxa.Name") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Genus", Binding = new Binding("Genus") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Species", Binding = new Binding("Species") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Size measure", Binding = new Binding("SizeType.Code") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Maximum size", Binding = new Binding("MaxSize") });
                    break;
            }
            dataGridEntities.Visibility = Visibility.Visible;
        }

        private void OnDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((DataGrid)sender).Name)
            {

                case "dataGridSummary":
                    if (dataGridSummary.SelectedItem != null)
                    {
                        switch (_summaryLevelType)
                        {
                            case SummaryLevelType.AllRegions:
                                //_dbSummary = (DBSummary)dataGridSummary.SelectedItem;
                                break;
                            case SummaryLevelType.Region:
                                break;
                            case SummaryLevelType.FishingGround:
                                //_dbSummary = (DBSummary)dataGridSummary.SelectedItem;
                                break;
                            case SummaryLevelType.EnumeratorRegion:
                                break;
                            case SummaryLevelType.Enumerator:
                                break;
                            case SummaryLevelType.EnumeratedMonth:
                                break;
                        }
                    }

                    break;
                default:
                    buttonEdit.IsEnabled = true;

                    if (_nsapEntity != NSAPEntity.FMA && _nsapEntity != NSAPEntity.NSAPRegion)
                        buttonDelete.IsEnabled = true;
                    break;
            }
        }

        public void RefreshEntityGrid()
        {
            SetDataGridSource();
        }
        private void SetDataGridSource()
        {
            switch (_nsapEntity)
            {
                case NSAPEntity.GPS:
                    dataGridEntities.ItemsSource = NSAPEntities.GPSViewModel.GPSCollection.OrderBy(t => t.AssignedName);
                    break;
                case NSAPEntity.Province:
                    dataGridEntities.ItemsSource = NSAPEntities.ProvinceViewModel.ProvinceCollection.OrderBy(t => t.ProvinceName);
                    break;

                case NSAPEntity.EffortIndicator:
                    dataGridEntities.ItemsSource = NSAPEntities.EffortSpecificationViewModel.EffortSpecCollection.OrderBy(t => t.IsForAllTypesFishing).ThenBy(t => t.Name);
                    break;

                case NSAPEntity.FishingGear:
                    dataGridEntities.ItemsSource = NSAPEntities.GearViewModel.GearCollection.OrderBy(t => t.GearName);
                    break;

                case NSAPEntity.FishingGround:
                    dataGridEntities.ItemsSource = NSAPEntities.FishingGroundViewModel.FishingGroundCollection.OrderBy(t => t.Name);
                    break;

                case NSAPEntity.FMA:
                    dataGridEntities.ItemsSource = NSAPEntities.FMAViewModel.FMACollection.OrderBy(t => t.FMAID);
                    break;

                case NSAPEntity.Enumerator:
                    dataGridEntities.ItemsSource = NSAPEntities.NSAPEnumeratorViewModel.NSAPEnumeratorCollection.OrderBy(t => t.Name);
                    break;

                case NSAPEntity.FishingVessel:
                    dataGridEntities.ItemsSource = NSAPEntities.FishingVesselViewModel.FishingVesselCollection.OrderBy(t => t.Name);
                    break;

                case NSAPEntity.LandingSite:

                    dataGridEntities.ItemsSource = NSAPEntities.LandingSiteViewModel.GetAllLandingSitesShowUsed().OrderBy(t => t.Municipality.Province.ProvinceName).ThenBy(t => t.Municipality.MunicipalityName).ThenBy(t => t.LandingSiteName);
                    break;

                case NSAPEntity.NSAPRegion:
                    dataGridEntities.ItemsSource = NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection.OrderBy(t => t.Sequence);
                    break;

                case NSAPEntity.NonFishSpecies:
                    dataGridEntities.ItemsSource = NSAPEntities.NotFishSpeciesViewModel.NotFishSpeciesCollection.OrderBy(t => t.Taxa.Name).ThenBy(t => t.Genus).ThenBy(t => t.Species);
                    break;

                case NSAPEntity.FishSpecies:
                    dataGridSpecies.ItemsSource = NSAPEntities.FishSpeciesViewModel.SpeciesCollection.OrderBy(t => t.GenericName).ThenBy(t => t.SpecificName);
                    break;
            }
        }

        private void AddEntity()
        {
            EditWindowEx ew = new EditWindowEx(_nsapEntity);
            ew.Owner = this;
            if ((bool)ew.ShowDialog())
            {
                SetDataGridSource();
            }
        }

        private void DeleteEntity()
        {

            if ((_nsapEntity == NSAPEntity.FishSpecies || _nsapEntity == NSAPEntity.NonFishSpecies)
                && MessageBox.Show("Are you sure you want to delete this species?\r\nThe species you delete is being used in the ODK collect app.",
                "Confirm deletetion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (_nsapEntity == NSAPEntity.FishSpecies)
                {
                    NSAPEntities.FishSpeciesViewModel.DeleteRecordFromRepo(((FishSpecies)dataGridSpecies.SelectedItem).RowNumber);
                    SetDataGridSource();
                }
                else
                {
                    NSAPEntities.NotFishSpeciesViewModel.DeleteRecordFromRepo(((NotFishSpecies)dataGridEntities.SelectedItem).SpeciesID);
                    SetDataGridSource();
                }
            }
            else
            {
                string message = "";
                switch (_nsapEntity)
                {
                    case NSAPEntity.FishingGround:
                        var fishingGround = (FishingGround)dataGridEntities.SelectedItem;
                        foreach (var region in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection)
                        {
                            foreach (var fma in region.FMAs)
                            {
                                foreach (var fg in fma.FishingGrounds)
                                {
                                    if (fg.FishingGroundCode == fishingGround.Code)
                                    {
                                        message = "Selected fishing ground cannot be deleted because it is used in an FMA in a region\r\n" +
                                                "Delete the fishing ground first in the list of fishing grounds in an FMA";
                                        break;
                                    }
                                }

                                if (message.Length > 0)
                                    break;
                            }

                            if (message.Length > 0)
                                break;
                        }
                        break;

                    case NSAPEntity.Enumerator:
                        var enumerator = (NSAPEnumerator)dataGridEntities.SelectedItem;
                        foreach (var region in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection)
                        {
                            foreach (var e in region.NSAPEnumerators)
                            {
                                if (enumerator.ID == e.EnumeratorID)
                                {
                                    message = "Selected enumerator cannot be deleted because it is used in a region\r\n" +
                                            "Delete the enumerator first in the list of enumerators in a region";
                                    break;
                                }
                            }

                            if (message.Length > 0)
                                break;
                        }
                        break;

                    case NSAPEntity.FishingVessel:
                        var vessel = (FishingVessel)dataGridEntities.SelectedItem;
                        foreach (var region in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection)
                        {
                            foreach (var v in region.FishingVessels)
                            {
                                if (vessel.ID == v.FishingVesselID)
                                {
                                    message = "Selected fishing vessel cannot be deleted because it is used in a region\r\n" +
                                            "Delete the fishing vessel first in the list of vessels in a region";
                                    break;
                                }
                            }

                            if (message.Length > 0)
                                break;
                        }
                        break;

                    case NSAPEntity.FishingGear:
                        var gear = (Gear)dataGridEntities.SelectedItem;
                        foreach (var region in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection)
                        {
                            foreach (var g in region.Gears)
                            {
                                if (gear.Code == g.GearCode)
                                {
                                    message = "Selected gear cannot be deleted because it is used in a region\r\n" +
                                            "Delete the gear first in the list of gears in a region";
                                    break;
                                }
                            }

                            if (message.Length > 0)
                                break;
                        }
                        break;

                    case NSAPEntity.EffortIndicator:
                        var indicator = (EffortSpecification)dataGridEntities.SelectedItem;
                        if (!indicator.IsForAllTypesFishing)
                        {
                            foreach (var gearItem in NSAPEntities.GearViewModel.GearCollection)
                            {
                                foreach (var i in gearItem.GearEffortSpecificationViewModel.GearEffortSpecificationCollection)
                                {
                                    if (i.EffortSpecificationID == indicator.ID)
                                    {
                                        message = "Selected effort indicator cannot be deleted because it is used in a gear\r\n" +
                                                "Delete the indicator first in the list of effort indicators in a gear";
                                        break;
                                    }


                                }
                                if (message.Length > 0)
                                    break;
                            }
                        }
                        break;

                    case NSAPEntity.LandingSite:
                        var landingSite = (LandingSite)dataGridEntities.SelectedItem;
                        foreach (var region in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection)
                        {
                            foreach (var fma in region.FMAs)
                            {
                                foreach (var fg in fma.FishingGrounds)
                                {
                                    foreach (var ls in fg.LandingSites)
                                    {
                                        if (ls.LandingSite.LandingSiteID == landingSite.LandingSiteID)
                                        {
                                            message = "Selected landing site cannot be deleted because it is used in an fishing ground in a region\r\n" +
                                                    "Delete the landing site first in the list of landing sites in a fishing ground";
                                            break;
                                        }
                                    }

                                    if (message.Length > 0)
                                        break;
                                }

                                if (message.Length > 0)
                                    break;
                            }

                            if (message.Length > 0)
                                break;
                        }
                        break;
                }

                if (message.Length > 0)
                {
                    MessageBox.Show(message, "Cannot delete selected item", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    bool success = false;
                    switch (_nsapEntity)
                    {
                        case NSAPEntity.GPS:
                            success = NSAPEntities.GPSViewModel.DeleteRecordFromRepo(((GPS)dataGridEntities.SelectedItem).Code);
                            break;
                        case NSAPEntity.FishingGround:
                            success = NSAPEntities.FishingGroundViewModel.DeleteRecordFromRepo(((FishingGround)dataGridEntities.SelectedItem).Code);
                            break;

                        case NSAPEntity.FishingVessel:
                            success = NSAPEntities.FishingVesselViewModel.DeleteRecordFromRepo(((FishingVessel)dataGridEntities.SelectedItem).ID);
                            break;

                        case NSAPEntity.FishingGear:
                            success = NSAPEntities.GearViewModel.DeleteRecordFromRepo(((Gear)dataGridEntities.SelectedItem).Code);

                            break;

                        case NSAPEntity.LandingSite:
                            success = NSAPEntities.LandingSiteViewModel.DeleteRecordFromRepo(((LandingSite)dataGridEntities.SelectedItem).LandingSiteID);
                            break;

                        case NSAPEntity.Enumerator:
                            success = NSAPEntities.NSAPEnumeratorViewModel.DeleteRecordFromRepo(((NSAPEnumerator)dataGridEntities.SelectedItem).ID);
                            break;

                        case NSAPEntity.EffortIndicator:
                            var effortSpecToDelete = (EffortSpecification)dataGridEntities.SelectedItem;
                            NSAPEntities.GearViewModel.DeleteEffortSpec(effortSpecToDelete);
                            NSAPEntities.EffortSpecificationViewModel.DeleteRecordFromRepo(effortSpecToDelete.ID);

                            break;
                    }
                    if (success)
                    {
                        SetDataGridSource();
                    }
                }
            }
        }

        private void EditEntity()
        {
            if (_nsapEntity == NSAPEntity.FishSpecies)
            {
                OnGridDoubleClick(dataGridSpecies, null);
            }
            else
            {
                OnGridDoubleClick(dataGridEntities, null);
            }
        }

        public void RefreshSpeciesGrid(int ItemsRefreshed)
        {
            dataGridSpecies.Items.Refresh();
            MessageBox.Show($"Updated {ItemsRefreshed} species!");
        }

        private void ExportSelectedEntityData()
        {
            string inFileName = "";
            DataSet ds = null;
            string entityExported = "";
            switch (_nsapEntity)
            {
                case NSAPEntity.Enumerator:
                    inFileName = "NSAP-ODK enumerators";
                    entityExported = "Enumerators";
                    break;
                case NSAPEntity.LandingSite:
                    inFileName = "NSAP-ODK landing sites";
                    entityExported = "Landing sites";
                    break;
                case NSAPEntity.FishSpecies:
                    inFileName = "NSAP-ODK fish species";
                    entityExported = "Fish species";
                    break;
                case NSAPEntity.NonFishSpecies:
                    inFileName = "NSAP-ODK invertebrates";
                    entityExported = "Invertebrate species";
                    break;
                case NSAPEntity.FishingGround:
                    inFileName = "NSAP-ODK fishing grounds";
                    entityExported = "Fishing grounds";
                    break;
                case NSAPEntity.FishingGear:
                    inFileName = "NSAP-ODK fishing gears";
                    entityExported = "Fishing gears";
                    break;
                case NSAPEntity.GPS:
                    inFileName = "NSAP-ODK GPS";
                    entityExported = "GPS";
                    break;
                case NSAPEntity.EffortIndicator:
                    inFileName = "NSAP-ODK effort specifications";
                    entityExported = "Effort specifications";
                    break;
            }

            string fileName = ExportExcel.GetSaveAsExcelFileName(this, inFileName);

            if (fileName.Length > 0)
            {
                switch (_nsapEntity)
                {
                    case NSAPEntity.LandingSite:
                        ds = NSAPEntities.LandingSiteViewModel.Dataset();
                        break;
                    case NSAPEntity.FishSpecies:
                        ds = NSAPEntities.FishSpeciesViewModel.DataSet();
                        break;
                    case NSAPEntity.NonFishSpecies:
                        ds = NSAPEntities.NotFishSpeciesViewModel.DataSet();
                        break;
                    case NSAPEntity.Enumerator:
                        ds = NSAPEntities.NSAPEnumeratorViewModel.DataSet();
                        break;
                    case NSAPEntity.FishingGround:
                        ds = NSAPEntities.FishingGroundViewModel.DataSet();
                        break;
                    case NSAPEntity.FishingGear:
                        ds = NSAPEntities.GearViewModel.DataSet();
                        break;
                    case NSAPEntity.GPS:
                        ds = NSAPEntities.GPSViewModel.DataSet();
                        break;
                    case NSAPEntity.EffortIndicator:
                        ds = NSAPEntities.EffortSpecificationViewModel.DataSet();
                        break;
                }

                if (ExportExcel.ExportDatasetToExcel(ds, fileName))
                {
                    MessageBox.Show($"{entityExported} exported to Excel", "NSAP-ODK Database");
                }
                else
                {
                    if (ExportExcel.ErrorMessage.Length > 0)
                    {
                        MessageBox.Show(ExportExcel.ErrorMessage, "NSAP-ODK Database");
                    }
                    else
                    {
                        MessageBox.Show($"An error occurred when exporting {entityExported} to Excel\r\n" +
                                        "Please report this error", "NSAP-ODK Database");
                    }
                }
            }
        }


        private async void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                //case "buttonEntitySummary":
                //    break;
                case "buttonExport":
                    ExportSelectedEntityData();
                    break;
                case "buttonOrphan":
                    ShowStatusRow();
                    if (_nsapEntity == NSAPEntity.FishSpecies || _nsapEntity == NSAPEntity.NonFishSpecies)
                    {
                        _nsapEntity = NSAPEntity.SpeciesName;
                    }
                    await NSAPEntities.SummaryItemViewModel.SetOrphanedEntityAsync(_nsapEntity);
                    var oiw = new OrphanItemsManagerWindow();
                    oiw.Owner = this;
                    oiw.NSAPEntity = _nsapEntity;
                    oiw.ShowDialog();
                    break;
                case "buttonImport":
                    var iw = new ImportByPlainTextWindow();
                    iw.Owner = this;
                    iw.NSAPEntityType = _nsapEntity;
                    //iw.ParentWindow = this;
                    iw.ShowDialog();
                    break;
                case "buttonDetails":
                    break;

                case "buttonAdd":
                    AddEntity();
                    break;

                case "buttonEdit":
                    EditEntity();
                    break;

                case "buttonDelete":
                    DeleteEntity();
                    break;
                case "ButtonSaveGearUnload":
                    SaveChangesToGearUnload();
                    break;
                case "ButtonUndoGearUnload":
                    UndoChangesToGearUnload();
                    break;

            }
        }



        public void ShowDBSummary()
        {
            menuDatabaseSummary.IsChecked = true;
        }
        private void OnMenuItemChecked(object sender, RoutedEventArgs e)
        {
            buttonImport.Visibility = Visibility.Collapsed;
            buttonOrphan.Visibility = Visibility.Collapsed;
            buttonExport.Visibility = Visibility.Visible;
            if (dataGridEntities.ContextMenu != null)
            {
                dataGridEntities.ContextMenu.Items.Clear();
            }

            string textOfTitle = "";
            UncheckEditMenuItems((MenuItem)e.Source);

            ContextMenu contextMenu = new ContextMenu();
            switch (((MenuItem)sender).Name)
            {
                case "menuDatabaseSummary":
                    _nsapEntity = NSAPEntity.DBSummary;
                    if (_selectedTreeNode != null)
                    {
                        _selectedTreeNode.IsSelected = false;
                    }
                    buttonExport.Visibility = Visibility.Collapsed;
                    break;
                case "menuGPS":
                    _nsapEntity = NSAPEntity.GPS;
                    textOfTitle = "List of GPS units";
                    break;
                case "menuProvinces":
                    _nsapEntity = NSAPEntity.Province;
                    textOfTitle = "List of Provinces";
                    buttonExport.Visibility = Visibility.Collapsed;
                    break;

                case "menuEffortIndicators":
                    _nsapEntity = NSAPEntity.EffortIndicator;
                    textOfTitle = "List of fishing effort indicators";
                    contextMenu.Items.Add(new MenuItem { Header = "View gears using this indicator", Name = "menuViewGearsUsingIndicator", Tag = "nsapEntities" });
                    break;

                case "menuFMAs":
                    _nsapEntity = NSAPEntity.FMA;
                    textOfTitle = "List of FMAs";
                    buttonExport.Visibility = Visibility.Collapsed;
                    break;

                case "menuNSAPRegions":
                    _nsapEntity = NSAPEntity.NSAPRegion;
                    textOfTitle = "List of NSAP Regions";
                    buttonExport.Visibility = Visibility.Collapsed;
                    break;

                case "menuFishingGrouds":
                    _nsapEntity = NSAPEntity.FishingGround;
                    textOfTitle = "List of fishing grounds";
                    break;

                case "menuLandingSites":
                    buttonOrphan.Visibility = Visibility.Visible;
                    _nsapEntity = NSAPEntity.LandingSite;
                    textOfTitle = "List of landing sites";
                    break;

                case "menuFishingGears":
                    buttonOrphan.Visibility = Visibility.Visible;
                    _nsapEntity = NSAPEntity.FishingGear;
                    textOfTitle = "List of fishing gears";
                    break;

                case "menuEnumerators":
                    buttonOrphan.Visibility = Visibility.Visible;
                    buttonImport.Visibility = Visibility.Visible;
                    //buttonEntitySummary.Visibility = Visibility.Visible;
                    _nsapEntity = NSAPEntity.Enumerator;
                    textOfTitle = "List of enumerators";
                    break;

                case "menuVessels":
                    _nsapEntity = NSAPEntity.FishingVessel;
                    textOfTitle = "List of fishing vessels";
                    buttonOrphan.Visibility = Visibility.Visible;
                    buttonExport.Visibility = Visibility.Collapsed;
                    buttonImport.Visibility = Visibility.Visible;
                    _nsapEntity = NSAPEntity.FishingVessel;
                    break;

                case "menuFishSpecies":
                    _nsapEntity = NSAPEntity.FishSpecies;
                    buttonOrphan.Visibility = Visibility.Visible;
                    textOfTitle = "List of fish species names";
                    break;

                case "menuNonFishSpecies":
                    _nsapEntity = NSAPEntity.NonFishSpecies;
                    buttonOrphan.Visibility = Visibility.Visible;
                    textOfTitle = "List of non-fish species names";
                    break;
            }

            dataGridSpecies.ContextMenu = null;
            dataGridEntities.ContextMenu = null;

            if (contextMenu.Items.Count > 0)
            {
                for (int n = 0; n < contextMenu.Items.Count; n++)
                {
                    ((MenuItem)contextMenu.Items[n]).Click += OnDataGridContextMenu;
                }
                if (_nsapEntity == NSAPEntity.FishSpecies)
                {
                    dataGridSpecies.ContextMenu = contextMenu;
                }
                else
                {
                    dataGridEntities.ContextMenu = contextMenu;
                }
            }
            if (_nsapEntity == NSAPEntity.FishSpecies)
            {
                _currentDisplayMode = DataDisplayMode.Species;
                LoadDataGrid();
                //rowOthers.Height = new GridLength(0);
                //rowSpecies.Height = new GridLength(1, GridUnitType.Star);
            }
            else if (_nsapEntity == NSAPEntity.DBSummary)
            {
                _currentDisplayMode = DataDisplayMode.DBSummary;
            }
            else
            {
                _currentDisplayMode = DataDisplayMode.Others;
                LoadDataGrid();
                //rowSpecies.Height = new GridLength(0);
                //rowOthers.Height = new GridLength(1, GridUnitType.Star);
            }
            SetDataDisplayMode();
            labelTitle.Visibility = Visibility.Visible;
            labelTitle.Content = textOfTitle;
            buttonDelete.IsEnabled = false;
            buttonEdit.IsEnabled = false;
        }

        private void UncheckEditMenuItems(MenuItem source = null)
        {
            foreach (var mi in menuEdit.Items)
            {
                if (mi.GetType().Name != "Separator")
                {
                    var menu = (MenuItem)mi;
                    if (source != null)
                    {
                        if (menu.Name != source.Name)
                        {
                            menu.IsChecked = false;
                        }
                    }
                    else
                    {
                        menu.IsChecked = false;
                    }
                    //if (menu.Name != ((MenuItem)e.Source).Name)
                    //{
                    //menu.IsChecked = false;
                    //}
                }
            }
        }

        private void ShowCrossTabWIndow()
        {
            CrossTabReportWindow ctw = CrossTabReportWindow.GetInstance();
            if (ctw.IsVisible)
            {
                ctw.BringIntoView();
            }
            else
            {
                ctw.Show();
                ctw.Owner = this;
            }
            ctw.ShowEffort();
        }
        private async void OnDataGridContextMenu(object sender, RoutedEventArgs e)
        {

            switch (((MenuItem)sender).Tag.ToString())
            {
                case "samplingCalendar":
                    _allSamplingEntitiesEventHandler.GearUsed = _gearName;
                    _allSamplingEntitiesEventHandler.ContextMenuTopic = "contextMenuCrosstabGear";

                    ShowStatusRow();
                    await CrossTabManager.GearByMonthYearAsync(_allSamplingEntitiesEventHandler);
                    ShowCrossTabWIndow();
                    break;
                case "nsapEntities":
                    EntityPropertyEnableWindow epe = null;
                    switch (_nsapEntity)
                    {
                        case NSAPEntity.EffortIndicator:
                            EffortSpecification es = (EffortSpecification)dataGridEntities.SelectedItem;
                            epe = new EntityPropertyEnableWindow(es, _nsapEntity);
                            break;
                    }
                    epe.ShowDialog();
                    break;
            }

        }

        private void Grid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }


        private bool GetCSVSaveLocationFromSaveAsDialog()
        {
            VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();
            fbd.UseDescriptionForTitle = true;
            fbd.Description = "Locate folder for saving csv file";

            if (_csvSaveToFolder.Length > 0)
            {
                fbd.SelectedPath = _csvSaveToFolder;
            }

            if ((bool)fbd.ShowDialog() && fbd.SelectedPath.Length > 0)
            {
                _csvSaveToFolder = fbd.SelectedPath;
                GenerateCSV.FolderSaveLocation = _csvSaveToFolder;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool GetCSVSaveLocationFromSaveAsDialog(LogType logType)
        {
            VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();
            fbd.UseDescriptionForTitle = true;
            fbd.Description = "Locate folder for saving csv file";

            if (_csvSaveToFolder.Length > 0)
            {
                fbd.SelectedPath = _csvSaveToFolder;
            }

            if ((bool)fbd.ShowDialog() && fbd.SelectedPath.Length > 0)
            {
                GenerateCSV.LogType = logType;
                _csvSaveToFolder = fbd.SelectedPath;
                GenerateCSV.FolderSaveLocation = _csvSaveToFolder;
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool GetCSVSaveLocationFromSaveAsDialog(out string fileName, LogType csvType)
        {
            fileName = "";
            VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();
            fbd.UseDescriptionForTitle = true;
            fbd.Description = "Locate folder for saving csv file";

            if (_csvSaveToFolder.Length > 0)
            {
                fbd.SelectedPath = _csvSaveToFolder;
            }

            if ((bool)fbd.ShowDialog() && fbd.SelectedPath.Length > 0)
            {
                switch (csvType)
                {
                    case LogType.EffortSpec_csv:
                        fileName = $"{fbd.SelectedPath}\\effortspec.csv";
                        break;

                    case LogType.Gear_csv:
                        fileName = $"{fbd.SelectedPath}\\gear.csv";
                        break;

                    case LogType.ItemSets_csv:
                        fileName = $"{fbd.SelectedPath}\\itemsets.csv";
                        break;

                    case LogType.Species_csv:
                        fileName = $"{fbd.SelectedPath}\\sp.csv";
                        break;

                    case LogType.SizeMeasure_csv:
                        fileName = $"{fbd.SelectedPath}\\size_measure.csv";
                        break;

                    case LogType.VesselName_csv:
                        fileName = $"{fbd.SelectedPath}\\vessel_name_municipal.csv";
                        break;

                    case LogType.FMACode_csv:
                        fileName = $"{fbd.SelectedPath}\\fma_code.csv";
                        break;

                    case LogType.FishingGroundCode_csv:
                        fileName = $"{fbd.SelectedPath}\\fishing_ground_code.csv";
                        break;
                }
                _csvSaveToFolder = fbd.SelectedPath;
            }

            return fileName.Length > 0;
        }


        private bool SelectRegions(bool resetList = false)
        {

            if (resetList || NSAPEntities.Regions.Count == 0)
            {
                SelectRegionWindow srw = new SelectRegionWindow();
                srw.ShowDialog();
            }
            return NSAPEntities.Regions.Count > 0;
        }

        private void ExportNSAPToExcel(bool tracked = false)
        {
            string filePath;
            string exportResult;
            if (ExportExcel.GetSaveAsExcelFileName(this, out filePath))
            {

                if (ExportExcel.ExportDatasetToExcel(EntitiesToDataTables.GenerateDataset(tracked), filePath))
                {
                    exportResult = "Successfully exported database to excel";
                }
                else
                {
                    exportResult = $"Was not successfull in exporting database to excel\r\n{ExportExcel.ErrorMessage}";
                }

                MessageBox.Show(exportResult, "Database export", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public bool LocateBackendDB(out string backendPath)
        {
            bool success = false;
            backendPath = "";
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Locate backend database for NSAP data";
            ofd.Filter = "MDB file(*.mdb)|*.mdb|All file types (*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if ((bool)ofd.ShowDialog() && File.Exists(ofd.FileName))
            {

                Global.SetMDBPath(ofd.FileName);
                if (Global.AppProceed)
                {
                    SetDataDisplayMode();
                    ShowSplash();
                    samplingTree.ReadDatabase();
                    if (
                        NSAPEntities.NSAPRegionViewModel.Count > 0 &&
                        NSAPEntities.FishSpeciesViewModel.Count > 0 &&
                        NSAPEntities.NotFishSpeciesViewModel.Count > 0 &&
                        NSAPEntities.FMAViewModel.Count > 0
                        )
                    {
                        SetDataDisplayMode();
                        menuDatabaseSummary.IsChecked = true;
                        success = true;
                        backendPath = ofd.FileName;
                    }
                    else
                    {
                        ShowDatabaseNotFoundView();
                    }
                }
            }
            return success;
        }

        private void ExportNSAPWithMaturityToExcel()
        {
            //var list = NSAPEntities.VesselCatchViewModel.GetUnloadsWithMaturity(_selectedRegionInSummary, _selectFishingGroundInSummary);
            var ds = UnloadWithMaturityDatasetManager.MaturityDataSet(_selectedRegionInSummary, _selectFishingGroundInSummary);
            if (ds.Tables[0].Rows.Count > 0)
            {

                string fn = ExportExcel.GetSaveAsExcelFileName(this, UnloadWithMaturityDatasetManager.FileName);
                if (fn.Length > 0)
                {
                    if (ExportExcel.ExportDatasetToExcel(ds, fn))
                    {
                        MessageBox.Show("Successfully exported maturity data to Excel", "NSAP ODK Database");
                    }
                    else
                    {
                        MessageBox.Show(ExportExcel.ErrorMessage, "NSAP ODK Database");
                    }
                }
                else
                {
                    MessageBox.Show("Export was cancelled", "NSAP ODK Database");
                }
            }
            else
            {
                MessageBox.Show("Selected region and fishing ground does not contain maturity data", "NSAP ODK Database");
            }
        }
        private async void OnMenuClicked(object sender, RoutedEventArgs e)
        {
            string fileName = "";

            string itemName = ((MenuItem)sender).Name;

            if (itemName == "menuDownloadHistory" || itemName == "menuNSAPCalendar")
            {
                UncheckEditMenuItems();
            }

            switch (itemName)
            {
                case "menuFileSettings1":
                case "menuFileSettings":
                    ShowSettingsWindow();
                    break;
                case "menuEnumeratorFirstSampling":
                    var fsw = FirstSamplingOfEnumeratorsWindow.GetInstance();
                    fsw.FirstSamplings = NSAPEntities.NSAPEnumeratorViewModel.GetFirstSamplingOfEnumerators();
                    if (fsw.Visibility == Visibility.Visible)
                    {
                        fsw.BringIntoView();
                    }
                    else
                    {
                        fsw.Show();
                    }

                    break;
                case "menuExtractVesselNames":
                    ExtractVesselFromRegionWindow erw = new ExtractVesselFromRegionWindow();
                    erw.ShowDialog();
                    break;
                case "menuCopyTextPropertyGrid":
                    StringBuilder sb = new StringBuilder();
                    foreach (PropertyItem prp in _propertyGrid.Properties)
                    {
                        sb.Append(prp.DisplayName + "\t" + prp.Value.ToString() + "\r\n");
                    }
                    Clipboard.SetText(sb.ToString());
                    break;
                case "menuCopyText":
                    // var stateCAnSelectMultiple = _dataGrid.cans
                    if (_dataGrid != null)
                    {
                        var smode = _dataGrid.SelectionMode;
                        _dataGrid.SelectionMode = DataGridSelectionMode.Extended;
                        _dataGrid.SelectAllCells();
                        _dataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
                        ApplicationCommands.Copy.Execute(null, _dataGrid);
                        var resultat = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
                        var result = (string)Clipboard.GetData(DataFormats.Text);
                        _dataGrid.UnselectAllCells();
                        _dataGrid.SelectionMode = smode;
                    }
                    break;
                case "menuUpdateUnloadStats":
                    if (NSAPEntities.VesselUnloadViewModel.CountLandingWithCatchComposition() > 0)
                    {
                        if (MessageBox.Show("Updating landing statistics coould take a long time\r\nDo you want to proceed?",
                                            "NSAP-ODK Database",
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Information) == MessageBoxResult.Yes)
                        {
                            ShowStatusRow();
                            NSAPEntities.VesselUnloadViewModel.DatabaseUpdatedEvent += VesselUnloadViewModel_DatabaseUpdatedEvent;
                            var r = await NSAPEntities.VesselUnloadViewModel.UpdateUnloadStatsAsync();
                            NSAPEntities.VesselUnloadViewModel.DatabaseUpdatedEvent -= VesselUnloadViewModel_DatabaseUpdatedEvent;
                            ShowStatusRow(isVisible: false);
                            MessageBox.Show("Finished updating the database for responses to \"Is catch composition included\"",
                                            "NSAP-ODK Database",
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Update table column \"Has catch composition\" before updating landings statistics",
                                        "NSAP-ODK Database",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Information
                                        );
                    }
                    break;
                case "menuSetupMySQLTables":
                    int beforeCount = NSAPMysql.MySQLConnect.SchemaTableCount();
                    NSAPMysql.CreateMySQLTables.CreateTables();
                    int afterCount = NSAPMysql.MySQLConnect.SchemaTableCount();
                    string msg = "There are no changes to the NSAP-ODK Database";
                    if (afterCount > beforeCount)
                    {
                        msg = $"{afterCount - beforeCount} table(s) added to NSAP-ODK Database";

                    }

                    MessageBox.Show(msg,
                                    "NSAP-ODK Database",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information
                                    );
                    break;
                case "menuBackupMySQL":
                    if (Global.Settings.MySQLBackupFolder.Length > 0)
                    {
                        var backupWindow = backupMySQLWindow.GetInstance();
                        if (backupWindow.Visibility == Visibility.Visible)
                        {
                            backupWindow.BringIntoView();
                        }
                        else
                        {
                            backupWindow.Show();
                            backupWindow.Owner = this;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please provide backup folder for MySQL data using the Settings dialog",
                                        "NSAP-ODK Database",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Information);
                    }
                    break;

                case "menuExportExcelMaturity":
                    ExportNSAPWithMaturityToExcel();
                    break;
                case "menuAbout":
                    AboutWindow aw = new AboutWindow();
                    aw.ShowDialog();
                    break;
                case "menuUploadMedia":
                    break;
                case "menuSaveGear":
                    SaveFileDialog sfd = new SaveFileDialog
                    {
                        Title = "Save entities to XML",
                        Filter = "XML|*.xml|Default|*.*",
                        FilterIndex = 1
                    };
                    if ((bool)sfd.ShowDialog() && sfd.FileName.Length > 0)
                    {
                        if (itemName == "menuSaveGear")
                        {
                            NSAPEntities.GearViewModel.Serialize(sfd.FileName);
                        }
                    }
                    break;
                case "menuTrackedLandingSummay":

                    break;
                case "menuLocateDatabase":

                    LocateBackendDB(out string backendPath);
                    break;
                case "menuImportGPX":
                    OpenFileDialog opf = new OpenFileDialog();
                    opf.Title = "Open GPX file";
                    opf.Filter = "GPX file (*.gpx)|*.gpx|All files (*.*)|*.*";
                    opf.FilterIndex = 1;
                    opf.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    if ((bool)opf.ShowDialog() && opf.FileName.Length > 0)
                    {
                        var tracks = Track.ReadTracksFromFile(opf.FileName);
                        var wpts = Track.ReadNamedWaypointsFromFile(opf.FileName);

                    }
                    break;
                case "menuDownloadHistory":

                    _currentDisplayMode = DataDisplayMode.DownloadHistory;
                    ColumnForTreeView.Width = new GridLength(1, GridUnitType.Star);
                    SetDataDisplayMode();
                    break;
                case "menuExportExcelTracked":
                    //ExportNSAPToExcel(tracked: true);
                    TrackedSummariesForExportWindow tws = new TrackedSummariesForExportWindow();
                    tws.ShowDialog();
                    break;
                case "menuExportExcel":
                    ExportNSAPToExcel();
                    break;
                case "menuNSAPCalendar":
                    ShowNSAPCalendar();
                    break;
                case "menuImport":
                    ShowImportWindow();
                    break;
                case "menuOptionGenerateCSV":
                    CSVOptionsWindow optionsWindow = new CSVOptionsWindow();
                    optionsWindow.ShowDialog();
                    break;
                case "menuExit":
                case "menuExit2":
                    Close();
                    break;
                case "menuQueryAPI":
                    QueryAPIWIndow qaw = new QueryAPIWIndow(this);
                    qaw.ShowDialog();
                    break;

                case "menuSelectRegions":
                    SelectRegions(resetList: true);
                    break;
                case "menuGenerateAll":
                    await GenerateAllCSV();

                    break;


                case "menuGenerateItemSets":

                    //if (SelectRegions() && GetCSVSaveLocationFromSaveAsDialog(out fileName, LogType.ItemSets_csv))
                    //{
                    //    Logger.FilePath = fileName;
                    //    MessageBox.Show($"{await GenerateCSV.GenerateItemsetsCSV()} items in itemsets.csv generated\r maxrow is {NSAPEntities.GetMaxItemSetID()}", "CSV file created", MessageBoxButton.OK, MessageBoxImage.Information);
                    //}
                    break;


            }

        }

        private int _rowsForUpdating;
        private void VesselUnloadViewModel_DatabaseUpdatedEvent(object sender, EventArgs e)
        {

            UpdateDatabaseColumnEventArg ev = (UpdateDatabaseColumnEventArg)e;

            switch (ev.Intent)
            {
                case "start":
                    _rowsForUpdating = ev.RowsToUpdate;

                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusBar.Value = 0;
                              mainStatusBar.Maximum = _rowsForUpdating;
                              mainStatusBar.IsIndeterminate = true;
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = $"Starting to update database {_rowsForUpdating} rows for landing statistics";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "start updating":
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusBar.IsIndeterminate = false;
                              //do what you need to do on UI Thread
                              return null;
                          }
                        ), null);
                    break;
                case "row updated":
                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = $" Updated row {ev.RunningCount} of {_rowsForUpdating}";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusBar.Value++;
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "finished":
                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = $"Finished updating {_rowsForUpdating} rows";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    break;
            }
        }

        private async Task GenerateAllCSV()
        {
            if (SelectRegions() && GetCSVSaveLocationFromSaveAsDialog())
            {
                try
                {
                    int result = await GenerateCSV.GenerateAll();

                    MessageBox.Show($"Generated {GenerateCSV.FilesCount} csv files with a total of {result} lines", "CSV files created", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (System.IO.IOException)
                {
                    MessageBox.Show("Cannot complete this operation because some files are open.", "Exception", MessageBoxButton.OK, MessageBoxImage.Exclamation); ;
                }
                catch (Exception ex)
                {
                    Logger.LogType = LogType.Logfile;
                    Logger.Log(ex);
                }
            }
        }
        private void ShowNSAPCalendar()
        {
            //if (!_saveChangesToGearUnload &&
            //        NSAPEntities.GearUnloadViewModel.CopyOfGearUnloadList != null &&
            //        NSAPEntities.GearUnloadViewModel.CopyOfGearUnloadList.Count > 0
            //    )
            //{
            //    UndoChangesToGearUnload(refresh: false);
            //}

            _currentDisplayMode = DataDisplayMode.ODKData;
            ColumnForTreeView.Width = new GridLength(2, GridUnitType.Star);
            SetDataDisplayMode();
        }
        private void ShowImportWindow()
        {
            var window = ODKResultsWindow.GetInstance();
            if (window.IsVisible)
            {
                window.BringIntoView();
            }
            else
            {
                window.Show();
                window.Owner = this;
                window.ParentWindow = this;
            }
        }

        public void RefreshSummary()
        {
            menuDatabaseSummary.IsChecked = false;
            menuDatabaseSummary.IsChecked = true;
        }
        public void GearUnloadWindowClosed()
        {
            _gearUnloadWindow = null;
        }
        private void OnGridDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string summaryRegion;
            string summaryFishingGround;
            string summaryLandingSite;
            List<VesselUnload> unloads = new List<VesselUnload>();
            string id = "";
            string formTitle = "";
            switch (((DataGrid)sender).Name)
            {
                case "dataGridSummary":

                    if (dataGridSummary.SelectedItem != null)
                    {
                        switch (_summaryLevelType)
                        {
                            case SummaryLevelType.AllRegions:
                                //selected item is a region
                                var cellinfo = dataGridSummary.SelectedCells[0];
                                summaryRegion = ((TextBlock)cellinfo.Column.GetCellContent(cellinfo.Item)).Text;
                                unloads = NSAPEntities.VesselUnloadViewModel.GetAllVesselUnloads(summaryRegion);
                                formTitle = $"All vessel unload of {summaryRegion}";
                                break;
                            case SummaryLevelType.Region:
                                //selected item is a fishing ground
                                summaryRegion = ((TreeViewItem)treeViewSummary.SelectedItem).Header.ToString();
                                cellinfo = dataGridSummary.SelectedCells[0];
                                summaryFishingGround = ((TextBlock)cellinfo.Column.GetCellContent(cellinfo.Item)).Text;
                                //unloads = NSAPEntities.VesselUnloadViewModel.GetAllVesselUnloads(summaryRegion, summaryFishingGround);
                                unloads = NSAPEntities.SummaryItemViewModel.GetVesselUnloads(summaryRegion, summaryFishingGround);
                                formTitle = $"All vessel unload of {summaryFishingGround}, {summaryRegion} ";
                                break;
                            case SummaryLevelType.FishingGround:
                                summaryFishingGround = ((TreeViewItem)treeViewSummary.SelectedItem).Header.ToString();
                                summaryRegion = ((TreeViewItem)((TreeViewItem)treeViewSummary.SelectedItem).Parent).Header.ToString();
                                cellinfo = dataGridSummary.SelectedCells[0];
                                summaryLandingSite = ((TextBlock)cellinfo.Column.GetCellContent(cellinfo.Item)).Text;
                                unloads = NSAPEntities.SummaryItemViewModel.GetVesselUnloads(summaryRegion, summaryFishingGround, summaryLandingSite);
                                formTitle = $"All vessel unload of {summaryFishingGround}, {summaryRegion} ";
                                break;
                            case SummaryLevelType.EnumeratorRegion:
                                summaryRegion = ((TreeViewItem)treeViewSummary.SelectedItem).Header.ToString();
                                unloads = NSAPEntities.SummaryItemViewModel.GetVesselUnloads(
                                    (SummaryResults)dataGridSummary.SelectedItem,
                                    summaryRegion,
                                    _summaryLevelType);
                                break;
                            case SummaryLevelType.Enumerator:
                                //summaryRegion = ((TreeViewItem)((TreeViewItem)((TreeViewItem)treeViewSummary.SelectedItem).Parent).Parent).Header.ToString();
                                summaryRegion = ((TreeViewItem)((TreeViewItem)treeViewSummary.SelectedItem).Parent).Header.ToString();
                                unloads = NSAPEntities.SummaryItemViewModel.GetVesselUnloads(
                                    (SummaryResults)dataGridSummary.SelectedItem,
                                    summaryRegion,
                                    _summaryLevelType);
                                break;
                            case SummaryLevelType.EnumeratedMonth:
                                summaryRegion = ((TreeViewItem)((TreeViewItem)((TreeViewItem)treeViewSummary.SelectedItem).Parent).Parent).Header.ToString();
                                unloads = NSAPEntities.SummaryItemViewModel.GetVesselUnloads(
                                    (SummaryResults)dataGridSummary.SelectedItem,
                                    summaryRegion,
                                    _summaryLevelType,
                                    ((TreeViewItem)treeViewSummary.SelectedItem).Header.ToString());
                                break;
                        }

                        if (unloads?.Count > 0)
                        {
                            ShowVesselUnloadsInWindow(unloads, formTitle);
                            //GearUnloadWindow guw = GearUnloadWindow.GetInstance(unloads);
                            //guw.Owner = this;
                            //if (guw.Visibility == Visibility.Visible)
                            //{
                            //    guw.BringIntoView();
                            //}
                            //else
                            //{
                            //    guw.Show();
                            //}
                            //guw.Title = formTitle;
                        }
                    }

                    break;
                case "GridNSAPData":
                    if (_currentDisplayMode == DataDisplayMode.ODKData)
                    {

                        if (_gearUnloads != null && _gearUnloadWindow == null)
                        {
                            _gearUnloadWindow = new GearUnloadWindow(_gearUnloads, _treeItemData, this);
                            _gearUnloadWindow.Owner = this;
                            _gearUnloadWindow.Show();
                        }
                        else
                        {

                        }

                    }
                    else if (_currentDisplayMode == DataDisplayMode.DownloadHistory)
                    {
                        VesselUnload unload = null;
                        if (GridNSAPData.SelectedItem != null)
                        {
                            switch (((TreeViewItem)treeViewDownloadHistory.SelectedItem).Tag.ToString())
                            {
                                case "unloadSummary":

                                    unload = NSAPEntities.SummaryItemViewModel.GetVesselUnload((SummaryItem)GridNSAPData.SelectedItem);

                                    break;
                                case "downloadDate":
                                    ShowVesselUnloadsInWindow(
                                        unloads: NSAPEntities.SummaryItemViewModel.GetVesselUnloads((SummaryResults)GridNSAPData.SelectedItem),
                                        formTitle: "Vessel unloads from download history");
                                    return;
                                case "tracked":
                                case "effort":
                                    unload = NSAPEntities.SummaryItemViewModel.GetVesselUnload((SummaryItem)GridNSAPData.SelectedItem);
                                    break;
                                default:
                                    return;
                            }

                            var unloadEditWindow = VesselUnloadEditWindow.GetInstance();

                            if (unloadEditWindow.Visibility == Visibility.Visible)
                            {
                                unloadEditWindow.BringIntoView();
                            }
                            else
                            {
                                unloadEditWindow.Owner = this;
                                unloadEditWindow.Show();
                            }
                            unloadEditWindow.VesselUnload = unload;
                        }

                    }

                    break;
                case "dataGridSpecies":
                    var fs = (FishSpecies)dataGridSpecies.SelectedItem;

                    if (fs != null)
                    {
                        //id = fs.RowNumber.ToString();
                        id = ((int)fs.SpeciesCode).ToString();
                    }

                    break;

                case "dataGridEntities":
                    switch (_nsapEntity)
                    {
                        case NSAPEntity.GPS:
                            var gps = (GPS)dataGridEntities.SelectedItem;
                            if (gps != null)
                            {
                                id = gps.Code;
                            }
                            break;

                        case NSAPEntity.Province:
                            var prv = (Province)dataGridEntities.SelectedItem;

                            if (prv != null)
                                id = prv.ProvinceID.ToString();

                            break;

                        case NSAPEntity.NonFishSpecies:
                            var nfs = (NotFishSpecies)dataGridEntities.SelectedItem;

                            if (nfs != null)
                                id = nfs.SpeciesID.ToString();
                            break;

                        case NSAPEntity.EffortIndicator:
                            var ef = (EffortSpecification)dataGridEntities.SelectedItem;

                            if (ef != null)
                                id = ef.ID.ToString();
                            break;

                        case NSAPEntity.FMA:
                            var fma = (FMA)dataGridEntities.SelectedItem;

                            if (fma != null)
                                id = fma.FMAID.ToString();
                            break;

                        case NSAPEntity.Enumerator:
                            var enumerator = (NSAPEnumerator)dataGridEntities.SelectedItem;

                            if (enumerator != null)
                                id = enumerator.ID.ToString();
                            break;

                        case NSAPEntity.NSAPRegion:
                            var nsapRegion = (NSAPRegion)dataGridEntities.SelectedItem;

                            if (nsapRegion != null)
                                id = nsapRegion.Code;
                            break;

                        case NSAPEntity.LandingSite:
                            var landingSite = (LandingSite)dataGridEntities.SelectedItem;

                            if (landingSite != null)
                                id = landingSite.LandingSiteID.ToString();
                            break;

                        case NSAPEntity.FishingGear:
                            var gear = (Gear)dataGridEntities.SelectedItem;

                            if (gear != null)
                                id = gear.Code;
                            break;

                        case NSAPEntity.FishingGround:
                            var fishingGround = (FishingGround)dataGridEntities.SelectedItem;

                            if (fishingGround != null)
                                id = fishingGround.Code;
                            break;

                        case NSAPEntity.FishingVessel:
                            var fv = (FishingVessel)dataGridEntities.SelectedItem;

                            if (fv != null)
                                id = fv.ID.ToString();
                            break;
                    }

                    break;
            }
            if (id.Length > 0)
            {
                EditWindowEx ew = new EditWindowEx(_nsapEntity, id);
                ew.Owner = this;
                if ((bool)ew.ShowDialog())
                {
                    SetDataGridSource();
                }
            }

        }

        private void ShowVesselUnloadsInWindow(List<VesselUnload> unloads, string formTitle = null)
        {
            GearUnloadWindow guw = GearUnloadWindow.GetInstance(unloads);
            guw.Owner = this;
            if (guw.Visibility == Visibility.Visible)
            {
                guw.BringIntoView();
            }
            else
            {
                guw.Show();
            }
            if (formTitle != null)
            {
                guw.Title = formTitle;
            }
        }
        public void VesselWindowClosed()
        {
            _vesselUnloadWindow = null;
        }

        private void MakeCalendar(TreeViewModelControl.AllSamplingEntitiesEventHandler e)
        {
            //NSAPEntities.SummaryItemViewModel.ResetResults();
            //NSAPEntities.SummaryItemViewModel.TreeViewData = e;
            var listGearUnload = NSAPEntities.SummaryItemViewModel.GearUnloadsByMonth((DateTime)e.MonthSampled);

            if (listGearUnload.Count > 0)
            {
                _fishingCalendarViewModel = new FishingCalendarViewModel(listGearUnload.OrderBy(t => t.GearUsedName).ToList());
                //_fishingCalendarViewModel = new FishingCalendarViewModel(listGearUnload.OrderBy(t => t.Gear.GearName).ToList());
                GridNSAPData.Columns.Clear();
                GridNSAPData.AutoGenerateColumns = true;
                GridNSAPData.DataContext = _fishingCalendarViewModel.DataTable;
            }
        }

        private void OnTreeViewItemSelected(object sender, TreeViewModelControl.AllSamplingEntitiesEventHandler e)
        {
            gridCalendarHeader.Visibility = Visibility.Visible;
            NSAPEntities.SummaryItemViewModel.ResetResults();
            NSAPEntities.SummaryItemViewModel.TreeViewData = e;
            string labelContent = "";
            GridNSAPData.SelectionUnit = DataGridSelectionUnit.FullRow;
            switch (e.TreeViewEntity)
            {
                case "tv_NSAPRegionViewModel":
                    labelContent = $"Summary of database content for {e.NSAPRegion.Name}";
                    SetUpSummaryGrid(SummaryLevelType.Region, GridNSAPData, treeviewData: e);
                    break;
                case "tv_FMAViewModel":
                    labelContent = $"Summary of database content for {e.FMA.Name}, {e.NSAPRegion.Name}";
                    SetUpSummaryGrid(SummaryLevelType.FMA, GridNSAPData, treeviewData: e);
                    break;
                case "tv_FishingGroundViewModel":
                    labelContent = $"Summary of database content for {e.FishingGround.Name}, {e.FMA.Name}, {e.NSAPRegion.Name}";
                    SetUpSummaryGrid(SummaryLevelType.FishingGround, GridNSAPData, treeviewData: e);
                    break;
                case "tv_LandingSiteViewModel":
                    labelContent = $"Summary of database content for {e.LandingSiteText}, {e.FishingGround.Name}, {e.FMA.Name}, {e.NSAPRegion.Name}";
                    SetUpSummaryGrid(SummaryLevelType.LandingSite, GridNSAPData, treeviewData: e);
                    break;
                case "tv_MonthViewModel":
                    GridNSAPData.SelectionUnit = DataGridSelectionUnit.Cell;

                    MonthLabel.Content = $"Fisheries landing sampling calendar for {((DateTime)e.MonthSampled).ToString("MMMM-yyyy")}";
                    MonthSubLabel.Content = $"{e.LandingSiteText}, {e.FishingGround}, {e.FMA}, {e.NSAPRegion}";
                    GridNSAPData.Visibility = Visibility.Visible;
                    GridNSAPData.SelectionUnit = DataGridSelectionUnit.Cell;
                    PropertyGrid.Visibility = Visibility.Collapsed;
                    NSAPEntities.NSAPRegion = e.NSAPRegion;
                    MakeCalendar(e);
                    break;
            }

            if (e.TreeViewEntity != "tv_MonthViewModel")
            {
                MonthLabel.Content = labelContent;
                MonthSubLabel.Visibility = Visibility.Collapsed;
                labelSummary.Content = labelContent;
                dataGridSummary.Visibility = Visibility.Visible;
                GridNSAPData.Visibility = Visibility.Visible;
                panelOpening.Visibility = Visibility.Visible;
            }
            else
            {
                MonthSubLabel.Visibility = Visibility.Visible;
                _acceptDataGridCellClick = true;
            }
            labelRowCount.Content = $"Rows: {GridNSAPData.Items.Count}";
        }
        //private async void OnTreeViewItemSelected1(object sender, TreeViewModelControl.AllSamplingEntitiesEventHandler e)
        //{
        //    labelRowCount.Visibility = Visibility.Collapsed;
        //    _acceptDataGridCellClick = false;
        //    _allSamplingEntitiesEventHandler = e;
        //    gridCalendarHeader.Visibility = Visibility.Visible;
        //    GridNSAPData.Visibility = Visibility.Visible;
        //    _gearUnload = null;
        //    if (_gearUnloadWindow != null)
        //    {
        //        _gearUnloadWindow.TurnGridOff();
        //    }

        //    string labelContent = "";
        //    switch (e.TreeViewEntity)
        //    {
        //        case "tv_NSAPRegionViewModel":
        //            labelContent = $"Summary of database content for {e.NSAPRegion.Name}";
        //            SetUpSummaryGrid1(SummaryLevelType.Region, GridNSAPData, e.NSAPRegion);
        //            break;
        //        case "tv_FMAViewModel":
        //            labelContent = $"Summary of database content for {e.FMA.Name}, {e.NSAPRegion.Name}";
        //            SetUpSummaryGrid1(SummaryLevelType.FMA, GridNSAPData, e.NSAPRegion, e.FMA);
        //            break;
        //        case "tv_FishingGroundViewModel":
        //            labelContent = $"Summary of database content for {e.FishingGround.Name}, {e.FMA.Name}, {e.NSAPRegion.Name}";
        //            SetUpSummaryGrid1(SummaryLevelType.FishingGround, GridNSAPData, e.NSAPRegion, e.FMA, e.FishingGround, inSummaryView: false);
        //            break;
        //        case "tv_LandingSiteViewModel":
        //            labelContent = $"Summary of database content for {e.LandingSiteText}, {e.FishingGround.Name}, {e.FMA.Name}, {e.NSAPRegion.Name}";

        //            SummaryManager.SummaryLevelType = SummaryLevelType.LandingSite;
        //            SummaryManager.TreeEntities = e;



        //            SetUpSummaryGrid1(SummaryLevelType.LandingSite, GridNSAPData, e.NSAPRegion, e.FMA, e.FishingGround, landingSite: e.LandingSiteText);


        //            if (CrossTabReportWindow.Instance != null)
        //            {
        //                _allSamplingEntitiesEventHandler.ContextMenuTopic = "contextMenuCrosstabLandingSite";
        //                await CrossTabManager.GearByMonthYearAsync(_allSamplingEntitiesEventHandler);
        //                ShowCrossTabWIndow();
        //            }
        //            break;
        //        case "tv_MonthViewModel":
        //            gridCalendarHeader.Visibility = Visibility.Visible;
        //            MonthLabel.Content = $"Fisheries landing sampling calendar for {((DateTime)e.MonthSampled).ToString("MMMM-yyyy")}";
        //            MonthSubLabel.Content = $"{e.LandingSiteText}, {e.FishingGround}, {e.FMA}, {e.NSAPRegion}";
        //            GridNSAPData.Visibility = Visibility.Visible;
        //            GridNSAPData.SelectionUnit = DataGridSelectionUnit.Cell;
        //            PropertyGrid.Visibility = Visibility.Collapsed;
        //            NSAPEntities.NSAPRegion = e.NSAPRegion;
        //            MakeCalendar(e);

        //            if (CrossTabReportWindow.Instance != null)
        //            {
        //                _allSamplingEntitiesEventHandler.ContextMenuTopic = "contextMenuCrosstabMonth";
        //                await CrossTabManager.GearByMonthYearAsync(_allSamplingEntitiesEventHandler);
        //                ShowCrossTabWIndow();
        //            }
        //            break;
        //    }


        //    if (e.TreeViewEntity != "tv_MonthViewModel")
        //    {
        //        MonthLabel.Content = labelContent;
        //        MonthSubLabel.Visibility = Visibility.Collapsed;
        //        labelSummary.Content = labelContent;
        //        dataGridSummary.Visibility = Visibility.Visible;
        //        panelOpening.Visibility = Visibility.Visible;
        //    }
        //    else
        //    {
        //        MonthSubLabel.Visibility = Visibility.Visible;
        //        _acceptDataGridCellClick = true;
        //    }

        //}

        private async void OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var type = GridNSAPData.DataContext.GetType().ToString();
            switch (_currentDisplayMode)
            {
                case DataDisplayMode.ODKData:
                    if (GridNSAPData.SelectedCells.Count == 1 && _acceptDataGridCellClick)
                    {
                        DataGridCellInfo cell = GridNSAPData.SelectedCells[0];
                        _gridRow = GridNSAPData.Items.IndexOf(cell.Item);
                        _gridCol = cell.Column.DisplayIndex;
                        var item = GridNSAPData.Items[_gridRow] as DataRowView;
                        _gearName = (string)item.Row.ItemArray[0];
                        _gearCode = (string)item.Row.ItemArray[1];
                        _monthYear = DateTime.Parse(item.Row.ItemArray[2].ToString());

                        if (_gridCol == 0)
                        {
                            ContextMenu contextMenu = new ContextMenu();
                            contextMenu.Items.Add(new MenuItem { Header = "Cross tab report", Name = "menuGearCrossTabReport", Tag = "samplingCalendar" });
                            ((MenuItem)contextMenu.Items[0]).Click += OnDataGridContextMenu;
                            GridNSAPData.ContextMenu = contextMenu;

                            if (CrossTabReportWindow.Instance != null)
                            {
                                _allSamplingEntitiesEventHandler.GearUsed = _gearName;
                                _allSamplingEntitiesEventHandler.ContextMenuTopic = "contextMenuCrosstabGear";
                                await CrossTabManager.GearByMonthYearAsync(_allSamplingEntitiesEventHandler);
                                ShowCrossTabWIndow();
                            }
                        }
                        else
                        {
                            GridNSAPData.ContextMenu = null;
                        }


                        if (NSAPEntities.SummaryItemViewModel.SummaryResults.Count > 0)
                        {

                        }
                        _gearUnloads = NSAPEntities.SummaryItemViewModel.GetGearUnloads(_gearName, _gridCol - 3);

                    }

                    if (_gearUnloadWindow != null)
                    {
                        _gearUnloadWindow.TurnGridOff();
                        if (_gearUnloads != null)
                        {
                            //_gearUnloadWindow.GearUnload = _gearUnload;
                            _gearUnloadWindow.GearUnloads = _gearUnloads;

                        }
                    }
                    break;
            }
        }

        private void OnGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if ((string)e.Column.Header == "GearCode" || (string)e.Column.Header == "Month")
            {
                e.Column.Visibility = Visibility.Hidden;
            }
        }

        private async void OnTreeContextMenu(object sender, TreeViewModelControl.AllSamplingEntitiesEventHandler e)
        {
            _allSamplingEntitiesEventHandler = e;
            switch (e.ContextMenuTopic)
            {
                case "contextMenuGearUnloadMonth":
                case "contextMenuGearUnloadFishingGround":
                case "contextMenuGearUnloadLandingSite":
                    EditGearUnloadByRegionFMAWIndow editUnloadsWindow = new EditGearUnloadByRegionFMAWIndow(e);
                    editUnloadsWindow.Owner = this;
                    editUnloadsWindow.Show();

                    break;

                case "contextMenuCrosstabLandingSite":
                case "contextMenuCrosstabMonth":

                    ShowStatusRow();
                    await CrossTabManager.GearByMonthYearAsync(_allSamplingEntitiesEventHandler);
                    ShowCrossTabWIndow();
                    break;
            }
        }
        public void RefreshDownloadHistory()
        {
            _vesselDownloadHistory = DownloadToDatabaseHistory.DownloadToDatabaseHistoryDictionary;
            if (treeViewDownloadHistory.Items.Count > 0
                && ((TreeViewItem)treeViewDownloadHistory.SelectedItem).Tag.ToString() == "downloadDate")
            {
                RefreshDownloadedItemsGrid();
            }
        }
        private void UndoChangesToGearUnload(bool refresh = true)
        {
            NSAPEntities.GearUnloadViewModel.UndoChangesToGearUnloadBoatCatch(_gearUnloadList);
            if (refresh)
            {
                GridNSAPData.DataContext = _gearUnloadList;
                GridNSAPData.Items.Refresh();
            }

        }
        private void SaveChangesToGearUnload()
        {
            NSAPEntities.GearUnloadViewModel.SaveChangesToBoatAndCatch(_gearUnloadList);
            _saveChangesToGearUnload = true;
        }

        public void RefreshDownloadedSummaryItemsGrid()
        {
            var dt = DateTime.Parse(((TreeViewItem)((TreeViewItem)treeViewDownloadHistory.SelectedItem).Parent).Header.ToString());
            //var dt = DateTime.Parse(((TreeViewItem)treeViewDownloadHistory.SelectedItem).Header.ToString());
            var unloads = _vesselDownloadHistory[dt];
            List<UnloadChildrenSummary> list = new List<UnloadChildrenSummary>();

            foreach (var item in unloads)
            {
                list.Add(new UnloadChildrenSummary(item));
            }
            GridNSAPData.DataContext = list;
        }
        private void RefreshDownloadedItemsGrid(DateTime dt)
        {
            try
            {
                GridNSAPData.DataContext = _vesselDownloadHistory[dt];
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
        private void RefreshDownloadedItemsGrid()
        {

            var dt = DateTime.Parse(((TreeViewItem)treeViewDownloadHistory.SelectedItem).Header.ToString());
            try
            {
                GridNSAPData.DataContext = _vesselDownloadHistory[dt];
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private async void OnHistoryTreeItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ShowStatusRow();
            gridCalendarHeader.Visibility = Visibility.Visible;
            GridNSAPData.SelectionUnit = DataGridSelectionUnit.FullRow;
            GridNSAPData.IsReadOnly = true;
            labelRowCount.Visibility = Visibility.Visible;
            MonthLabel.Visibility = Visibility.Visible;
            MonthLabel.Content = $"Vessel unload by date of download";
            var dt = DateTime.Now;
            if (e.NewValue != null)
            {
                var tvItem = (TreeViewItem)e.NewValue;

                switch (tvItem.Tag.ToString())
                {
                    case "downloadDate":
                        dt = DateTime.Parse(tvItem.Header.ToString()).Date;
                        GridNSAPData.DataContext = await NSAPEntities.SummaryItemViewModel.GetDownloadSummaryByDateAsync(dt);
                        GridNSAPData.Columns.Clear();
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("DBSummary.EnumeratorName") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("DBSummary.GearName") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Date of first sampling", Binding = new Binding("DBSummary.FirstLandingFormattedDate") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Date of last sampling", Binding = new Binding("DBSummary.LastLandingFormattedDate") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Date downloaded", Binding = new Binding("DBSummary.LatestDownloadFormattedDate") });
                        //GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Landing site ", Binding = new Binding("Parent.Parent.LandingSiteName") });
                        //GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Gear ", Binding = new Binding("Parent.GearUsedName") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "# landings", Binding = new Binding("DBSummary.VesselUnloadCount") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "# landings with catch composition", Binding = new Binding("DBSummary.CountLandingsWithCatchComposition") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "# tracked fishing operations", Binding = new Binding("DBSummary.TrackedOperationsCount") });

                        //var unloads = _vesselDownloadHistory[dt];
                        //GridNSAPData.DataContext = NSAPEntities.VesselUnloadViewModel.GetDownlodaSummary(unloads, dt); ;
                        GridNSAPData.AutoGenerateColumns = false;

                        //Setter bold = new Setter(TextBlock.FontWeightProperty, FontWeights.Bold, null);
                        //GridNSAPData.Items.Refresh();
                        //DataGridRow row = (DataGridRow)GridNSAPData.ItemContainerGenerator.ContainerFromIndex(GridNSAPData.Items.Count - 1);
                        //Style newStyle = new Style(row.GetType());

                        //newStyle.Setters.Add(bold);
                        //row.Style = newStyle;
                        break;
                    case "effort":
                    case "tracked":
                        //if (!_saveChangesToGearUnload &&
                        //    NSAPEntities.GearUnloadViewModel.CopyOfGearUnloadList != null &&
                        //    NSAPEntities.GearUnloadViewModel.CopyOfGearUnloadList.Count > 0
                        //    )
                        //{
                        //    UndoChangesToGearUnload(refresh: false);
                        //}


                        if (tvItem.Tag.ToString() == "effort")
                        {
                            dt = DateTime.Parse(((TreeViewItem)tvItem.Parent).Header.ToString()).Date;
                            GridNSAPData.DataContext = await NSAPEntities.SummaryItemViewModel.GetDownloadDetailsByDateAsync(dt);
                            //RefreshDownloadedItemsGrid(dt);
                        }
                        else if (tvItem.Tag.ToString() == "tracked")
                        {
                            dt = DateTime.Parse(((TreeViewItem)tvItem.Parent).Header.ToString()).Date;
                            GridNSAPData.DataContext = await NSAPEntities.SummaryItemViewModel.GetDownloadDetailsByDateAsync(dt, isTracked: true);
                            //GridNSAPData.DataContext = _vesselDownloadHistory[dt].Where(t => t.IsTracked == true);
                        }
                        GridNSAPData.AutoGenerateColumns = false;
                        GridNSAPData.Columns.Clear();
                        GridNSAPData.SetValue(Grid.ColumnSpanProperty, 2);
                        GearUnload_ButtonsPanel.Visibility = Visibility.Collapsed;





                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("VesselUnloadID") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("EnumeratorNameToUse") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Region", Binding = new Binding("Region.Name") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("FMA.Name") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Fishing ground ", Binding = new Binding("FishingGround.Name") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Landing site ", Binding = new Binding("LandingSiteNameText") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Gear ", Binding = new Binding("GearUsedName") });

                        var col = new DataGridTextColumn()
                        {
                            Binding = new Binding("SamplingDate"),
                            Header = "Sampling date"
                        };
                        col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                        GridNSAPData.Columns.Add(col);

                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("Sector") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("VesselNameToUse") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Number of fishers", Binding = new Binding("NumberOfFishers") });

                        if (tvItem.Tag.ToString() == "tracked")
                        {
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "GPS ", Binding = new Binding("GPSNameToUse") });
                        }

                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Excel download ", Binding = new Binding("FromExcelDownload") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Form version ", Binding = new Binding("FormVersion") });
                        break;
                    case "gearUnload":
                        dt = DateTime.Parse(((TreeViewItem)tvItem.Parent).Header.ToString()).Date;
                        GridNSAPData.DataContext = await NSAPEntities.SummaryItemViewModel.GetGearUnloadsAsync(dt);
                        GridNSAPData.Columns.Clear();
                        GridNSAPData.AutoGenerateColumns = false;
                        GridNSAPData.SelectionUnit = DataGridSelectionUnit.Cell;
                        GridNSAPData.CanUserAddRows = false;
                        GridNSAPData.IsReadOnly = false;
                        GridNSAPData.SetValue(Grid.ColumnSpanProperty, 1);
                        GearUnload_ButtonsPanel.Visibility = Visibility.Visible;

                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "ID ", Binding = new Binding("PK"), IsReadOnly = true });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Region ", Binding = new Binding("Parent.NSAPRegion.Name"), IsReadOnly = true });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "FMA ", Binding = new Binding("Parent.FMA.Name"), IsReadOnly = true });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("Parent.FishingGround"), IsReadOnly = true });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("Parent.LandingSiteName"), IsReadOnly = true });

                        col = new DataGridTextColumn()
                        {
                            Binding = new Binding("Parent.SamplingDate"),
                            Header = "Sampling date"
                        };
                        col.Binding.StringFormat = "MMM-dd-yyyy";
                        col.IsReadOnly = true;
                        GridNSAPData.Columns.Add(col);


                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("GearUsedName"), IsReadOnly = true });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "# of sampled landings", Binding = new Binding("NumberOfSampledLandings"), IsReadOnly = true });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Boats", Binding = new Binding("Boats"), IsReadOnly = false });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Catch", Binding = new Binding("Catch"), IsReadOnly = false });

                        break;
                    case "unloadSummary":
                        dt = DateTime.Parse(((TreeViewItem)tvItem.Parent).Header.ToString()).Date;
                        GridNSAPData.DataContext = await NSAPEntities.SummaryItemViewModel.GetUnloadStatisticsByDateAsync(dt);
                        GridNSAPData.Columns.Clear();
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("VesselUnloadID") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Date sampled", Binding = new Binding("SamplingDateFormatted") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Region", Binding = new Binding("Region") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("FMA") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("FishingGround") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("LandingSiteNameText") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("GearUsedName") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("EnumeratorNameToUse") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Fishing grid count", Binding = new Binding("FishingGridRows") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Soak time count", Binding = new Binding("GearSoakRows") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Effort indicator count", Binding = new Binding("VesselEffortRows") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Catch composition count", Binding = new Binding("CatchCompositionRows") });

                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Length freq count", Binding = new Binding("LenFreqRows") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Length count", Binding = new Binding("LengthRows") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Length weight count", Binding = new Binding("LenWtRows") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Maturity count", Binding = new Binding("CatchMaturityRows") });

                        break;
                }


                MonthSubLabel.Content = $" All items listed were downloaded on {dt.ToString("MMM-dd-yyyy")}";
                labelRowCount.Content = $"Rows: {GridNSAPData.Items.Count}";
            }
        }

        private void OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            _saveChangesToGearUnload = false;
        }

        private void OnPropertyGridDblClick(object sender, MouseButtonEventArgs e)
        {
            switch (_selectedPropertyItem.PropertyName)
            {
                case "DBPath":
                    System.Diagnostics.Process.Start($"{Path.GetDirectoryName(_selectedPropertyItem.Value.ToString())}");
                    break;
                case "SavedJSONFolder":
                    System.Diagnostics.Process.Start($"{_selectedPropertyItem.Value.ToString()}");
                    break;
                default:
                    break;
            }
        }



        private void OnPropertyChanged(object sender, RoutedPropertyChangedEventArgs<PropertyItemBase> e)
        {
            _selectedPropertyItem = (PropertyItem)((PropertyGrid)e.Source).SelectedPropertyItem;
        }

        private void SetupEnumeratorSummaryGrid(DataGrid targetGrid, bool allRegions = true, NSAPEnumerator enumerator = null)
        {

            if (allRegions)
            {

            }
            else
            {

            }
        }
        private async void SetUpSummaryGrid(SummaryLevelType summaryType,
            DataGrid targetGrid,
            NSAPRegion region = null,
            FMA fma = null,
            FishingGround fg = null,
            string landingSite = null,
            bool inSummaryView = true,
            TreeViewModelControl.AllSamplingEntitiesEventHandler treeviewData = null,
            string enumeratorName = null,
            NSAPEnumerator en = null,
            DateTime? monthEnumerated = null)
        {
            targetGrid.AutoGenerateColumns = false;
            targetGrid.Columns.Clear();
            targetGrid.DataContext = null;

            if (treeviewData != null)
            {
                NSAPEntities.SummaryItemViewModel.TreeViewData = treeviewData;
            }
            //targetGrid.DataContext = NSAPEntities.SummaryItemViewModel.SummaryResults;
            switch (summaryType)
            {
                case SummaryLevelType.AllRegions:
                    targetGrid.DataContext = await NSAPEntities.SummaryItemViewModel.GetRegionOverallSummaryAsync();
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Region", Binding = new Binding("DBSummary.NSAPRegion") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of gear unload", Binding = new Binding("DBSummary.GearUnloadCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of complete gear unload", Binding = new Binding("DBSummary.CountCompleteGearUnload"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of vessel unload", Binding = new Binding("DBSummary.VesselUnloadCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# catch composition included", Binding = new Binding("DBSummary.CountLandingsWithCatchComposition"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of tracked  operations", Binding = new Binding("DBSummary.TrackedOperationsCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Earliest date of monitoring", Binding = new Binding("DBSummary.FirstLandingFormattedDate") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest date of monitoring", Binding = new Binding("DBSummary.LastLandingFormattedDate") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest date of downloaded e-forms ", Binding = new Binding("DBSummary.LatestDownloadFormattedDate") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of FMAs", Binding = new Binding("DBSummary.FMACount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of fishing grounds", Binding = new Binding("DBSummary.FishingGroundCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of landing sites", Binding = new Binding("DBSummary.LandingSiteCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of gear types", Binding = new Binding("DBSummary.FishingGearCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of enumerators", Binding = new Binding("DBSummary.EnumeratorCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of fishing vessels", Binding = new Binding("DBSummary.FishingVesselCount"), CellStyle = AlignRightStyle });
                    break;
                case SummaryLevelType.Region:
                    if (treeviewData != null)
                    {
                        targetGrid.DataContext = NSAPEntities.SummaryItemViewModel.SummaryResults;
                    }
                    else
                    {
                        targetGrid.DataContext = await NSAPEntities.SummaryItemViewModel.GetRegionSummaryAsync(region);
                    }
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("DBSummary.FMA") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of gear unload", Binding = new Binding("DBSummary.GearUnloadCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of complete gear unload", Binding = new Binding("DBSummary.CountCompleteGearUnload"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of vessel unload", Binding = new Binding("DBSummary.VesselUnloadCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# catch composition included", Binding = new Binding("DBSummary.CountLandingsWithCatchComposition"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of tracked  operations", Binding = new Binding("DBSummary.TrackedOperationsCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Earliest date of monitoring", Binding = new Binding("DBSummary.FirstLandingFormattedDate") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest date of monitoring", Binding = new Binding("DBSummary.LastLandingFormattedDate") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest date of downloaded e-forms ", Binding = new Binding("DBSummary.LatestDownloadFormattedDate") });
                    break;
                case SummaryLevelType.FMA:
                    targetGrid.DataContext = NSAPEntities.SummaryItemViewModel.SummaryResults;
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("DBSummary.FishingGround") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of gear unload", Binding = new Binding("DBSummary.GearUnloadCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of complete gear unload", Binding = new Binding("DBSummary.CountCompleteGearUnload"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of vessel unload", Binding = new Binding("DBSummary.VesselUnloadCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# catch composition included", Binding = new Binding("DBSummary.CountLandingsWithCatchComposition"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of tracked  operations", Binding = new Binding("DBSummary.TrackedOperationsCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Earliest date of monitoring", Binding = new Binding("DBSummary.FirstLandingFormattedDate") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest date of monitoring", Binding = new Binding("DBSummary.LastLandingFormattedDate") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest date of downloaded e-forms ", Binding = new Binding("DBSummary.LatestDownloadFormattedDate") });
                    break;
                case SummaryLevelType.FishingGround:
                    if (treeviewData != null)
                    {
                        targetGrid.DataContext = NSAPEntities.SummaryItemViewModel.SummaryResults;
                    }
                    else
                    {
                        targetGrid.DataContext = await NSAPEntities.SummaryItemViewModel.GetRegionFishingGroundSummaryAsync(region, fg);
                    }
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("DBSummary.LandingSiteName") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of gear unload", Binding = new Binding("DBSummary.GearUnloadCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of complete gear unload", Binding = new Binding("DBSummary.CountCompleteGearUnload"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of vessel unload", Binding = new Binding("DBSummary.VesselUnloadCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# catch composition included", Binding = new Binding("DBSummary.CountLandingsWithCatchComposition"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of tracked  operations", Binding = new Binding("DBSummary.TrackedOperationsCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Earliest date of monitoring", Binding = new Binding("DBSummary.FirstLandingFormattedDate") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest date of monitoring", Binding = new Binding("DBSummary.LastLandingFormattedDate") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest date of downloaded e-forms ", Binding = new Binding("DBSummary.LatestDownloadFormattedDate") });

                    checkLandingSiteWithLandings.Visibility = Visibility.Visible;

                    break;
                case SummaryLevelType.LandingSite:

                    targetGrid.DataContext = NSAPEntities.SummaryItemViewModel.SummaryResults;
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Month of sampling", Binding = new Binding("DBSummary.MonthSampled") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of gear unload", Binding = new Binding("DBSummary.GearUnloadCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of complete gear unload", Binding = new Binding("DBSummary.CountCompleteGearUnload"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of vessel unload", Binding = new Binding("DBSummary.VesselUnloadCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# catch composition included", Binding = new Binding("DBSummary.CountLandingsWithCatchComposition"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of tracked  operations", Binding = new Binding("DBSummary.TrackedOperationsCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Earliest date of monitoring", Binding = new Binding("DBSummary.FirstLandingFormattedDate") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest date of monitoring", Binding = new Binding("DBSummary.LastLandingFormattedDate") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest date of downloaded e-forms ", Binding = new Binding("DBSummary.LatestDownloadFormattedDate") });
                    break;
                case SummaryLevelType.EnumeratorRegion:
                    targetGrid.DataContext = await NSAPEntities.SummaryItemViewModel.GetEnumeratorSummaryLatestUploadAsync(region);
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("DBSummary.EnumeratorName") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("DBSummary.LandingSiteName") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("DBSummary.GearName") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Number of landings sampled", Binding = new Binding("DBSummary.VesselUnloadCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Last sampling", Binding = new Binding("DBSummary.LastLandingFormattedDate") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Last upload date", Binding = new Binding("DBSummary.LatestDownloadFormattedDate") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest e-Form version", Binding = new Binding("DBSummary.LatestEformVersion") });

                    break;
                case SummaryLevelType.SummaryOfEnumerators:
                    targetGrid.DataContext = null;
                    targetGrid.DataContext = await NSAPEntities.SummaryItemViewModel.GetEnumeratorSummaryAsync(region);
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("DBSummary.EnumeratorName") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Number of landings sampled", Binding = new Binding("DBSummary.VesselUnloadCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Catch composition included", Binding = new Binding("DBSummary.CountLandingsWithCatchComposition"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Tracked operations", Binding = new Binding("DBSummary.TrackedOperationsCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "First sampling", Binding = new Binding("DBSummary.FirstLandingFormattedDate") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Last sampling", Binding = new Binding("DBSummary.LastLandingFormattedDate") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Last upload date", Binding = new Binding("DBSummary.LatestDownloadFormattedDate") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest e-Form version", Binding = new Binding("DBSummary.LatestEformVersion") });
                    break;
                case SummaryLevelType.Enumerator:
                    targetGrid.DataContext = await NSAPEntities.SummaryItemViewModel.GetEnumeratorSummaryAsync(region, enumeratorName);
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("DBSummary.LandingSiteName") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("DBSummary.GearName") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Number of landings sampled", Binding = new Binding("DBSummary.VesselUnloadCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Catch composition included", Binding = new Binding("DBSummary.CountLandingsWithCatchComposition"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Tracked operations", Binding = new Binding("DBSummary.TrackedOperationsCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "First sampling", Binding = new Binding("DBSummary.FirstLandingFormattedDate") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Last sampling", Binding = new Binding("DBSummary.LastLandingFormattedDate") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Last upload date", Binding = new Binding("DBSummary.LatestDownloadFormattedDate") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest e-Form version", Binding = new Binding("DBSummary.LatestEformVersion") });
                    break;
                case SummaryLevelType.EnumeratedMonth:

                    targetGrid.DataContext = await NSAPEntities.SummaryItemViewModel.GetEnumeratorSummaryByMonthAsync(en, (DateTime)monthEnumerated);
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("DBSummary.LandingSiteName") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("DBSummary.GearName") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Number of landings sampled", Binding = new Binding("DBSummary.VesselUnloadCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Catch composition included", Binding = new Binding("DBSummary.CountLandingsWithCatchComposition"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Tracked operations", Binding = new Binding("DBSummary.TrackedOperationsCount"), CellStyle = AlignRightStyle });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "First sampling", Binding = new Binding("DBSummary.FirstLandingFormattedDate") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Last sampling", Binding = new Binding("DBSummary.LastLandingFormattedDate") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Last upload date", Binding = new Binding("DBSummary.LatestDownloadFormattedDate") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest e-Form version", Binding = new Binding("DBSummary.LatestEformVersion") });
                    break;
            }

        }

        public void ShowSummaryAtLevel(
            SummaryLevelType summaryType,
            NSAPRegion region = null,
            FMA fma = null,
            FishingGround fg = null,
            string enumeratorName = null,
            NSAPEnumerator en = null,
            DateTime? monthEnumerated = null
            )
        {

            string labelContent = "";
            dataGridSummary.Columns.Clear();
            dataGridSummary.AutoGenerateColumns = false;
            switch (summaryType)
            {
                case SummaryLevelType.AllRegions:
                    labelContent = "Summary by region";
                    SetUpSummaryGrid(SummaryLevelType.AllRegions, dataGridSummary);
                    break;
                case SummaryLevelType.FMA:
                    break;
                case SummaryLevelType.Region:
                    labelContent = $"Summary of selected region: {region.Name}";
                    SetUpSummaryGrid(SummaryLevelType.Region, dataGridSummary, region: region);
                    break;
                case SummaryLevelType.FishingGround:
                    labelContent = $"Summary of selected fishing ground: {fg.Name}, {region}";
                    SetUpSummaryGrid(SummaryLevelType.FishingGround, dataGridSummary, region: region, fg: fg);
                    break;
                case SummaryLevelType.SummaryOfEnumerators:
                    labelContent = "Summary of all enumerators in the region";
                    SetUpSummaryGrid(summaryType, dataGridSummary, region: region);
                    break;
                case SummaryLevelType.Enumerator:
                    labelContent = $"Summary for enumerator: {enumeratorName}";
                    SetUpSummaryGrid(summaryType, dataGridSummary, region: region, enumeratorName: enumeratorName);
                    break;
                case SummaryLevelType.EnumeratedMonth:
                    labelContent = $"Summary for enumerator: {en.Name} at {(DateTime)monthEnumerated:MMMM, yyyy}";
                    SetUpSummaryGrid(
                        summaryType,
                        dataGridSummary,
                        en: en,
                        enumeratorName: enumeratorName,
                        monthEnumerated: monthEnumerated);
                    break;
            }
            labelSummary.Content = labelContent;
            dataGridSummary.Visibility = Visibility.Visible;
            panelOpening.Visibility = Visibility.Visible;
        }

        private void ProcessSummaryTreeSelection(TreeViewItem tvItem)
        {
            ShowStatusRow();
            labelSummary2.Visibility = Visibility.Collapsed;
            rowSummaryDataGrid.Height = new GridLength(1, GridUnitType.Star);
            rowOverallSummary.Height = new GridLength(0);
            string header = tvItem.Header.ToString();
            switch (header)
            {
                case "Overall":
                    ShowStatusRow(isVisible: false);
                    ShowSummary(header);
                    rowSummaryDataGrid.Height = new GridLength(0);
                    rowOverallSummary.Height = new GridLength(1, GridUnitType.Star);
                    _summaryLevelType = SummaryLevelType.Overall;
                    break;
                case "Regions":
                    ShowSummaryAtLevel(SummaryLevelType.AllRegions);
                    _summaryLevelType = SummaryLevelType.AllRegions;
                    break;
                case "Enumerators":
                    ShowSummary(header);
                    rowSummaryDataGrid.Height = new GridLength(0);
                    rowOverallSummary.Height = new GridLength(1, GridUnitType.Star);
                    _summaryLevelType = SummaryLevelType.Enumerators;
                    break;
                case "e-Form versions":
                case "Enumerators and form versions":
                    ShowStatusRow(isVisible: false);
                    ShowSummary(header);
                    rowSummaryDataGrid.Height = new GridLength(0);
                    rowOverallSummary.Height = new GridLength(1, GridUnitType.Star);
                    break;
                case "Databases":
                    ShowStatusRow(isVisible: false);
                    ShowSummary(header);
                    rowSummaryDataGrid.Height = new GridLength(0);
                    rowOverallSummary.Height = new GridLength(1, GridUnitType.Star);
                    break;
                default:
                    switch (tvItem.Tag.GetType().Name)
                    {
                        case "NSAPRegion":
                            switch (((TreeViewItem)tvItem.Parent).Header)
                            {
                                case "Enumerators":
                                    ShowEnumeratorSummary((NSAPRegion)tvItem.Tag);
                                    _summaryLevelType = SummaryLevelType.EnumeratorRegion;
                                    break;
                                case "Regions":
                                    ShowSummaryAtLevel(SummaryLevelType.Region, (NSAPRegion)tvItem.Tag);
                                    _summaryLevelType = SummaryLevelType.Region;
                                    break;
                            }
                            break;
                        case "FishingGround":
                            ShowSummaryAtLevel(summaryType: SummaryLevelType.FishingGround, region: (NSAPRegion)((TreeViewItem)tvItem.Parent).Tag, fg: (FishingGround)tvItem.Tag);
                            _summaryLevelType = SummaryLevelType.FishingGround;
                            break;
                        case "NSAPEnumerator":
                            //ShowEnumeratorSummary((NSAPEnumerator)tvItem.Tag);
                            if (header == "Summary of enumerators")
                            {
                                ShowSummaryAtLevel(SummaryLevelType.SummaryOfEnumerators, region: (NSAPRegion)((TreeViewItem)tvItem.Parent).Tag);
                            }
                            else
                            {
                                ShowSummaryAtLevel(SummaryLevelType.Enumerator, region: (NSAPRegion)((TreeViewItem)tvItem.Parent).Tag, enumeratorName: header);
                            }
                            _summaryLevelType = SummaryLevelType.Enumerator;
                            break;
                        case "DateTime":
                            ShowSummaryAtLevel(
                                SummaryLevelType.EnumeratedMonth,
                                en: (NSAPEnumerator)((TreeViewItem)tvItem.Parent).Tag,
                                enumeratorName: ((TreeViewItem)tvItem.Parent).Header.ToString(),
                                monthEnumerated: (DateTime)tvItem.Tag);
                            //ShowEnumeratorSummary((NSAPEnumerator)((TreeViewItem)tvItem.Parent).Tag, (DateTime)tvItem.Tag);
                            _summaryLevelType = SummaryLevelType.EnumeratedMonth;
                            break;
                    }
                    break;
            }

        }

        private void ShowEnumeratorSummary(NSAPRegion region)
        {
            //var summaries = NSAPEntities.NSAPEnumeratorViewModel.GetSummary(region);
            SetUpSummaryGrid(SummaryLevelType.EnumeratorRegion, dataGridSummary, region: region);
            //dataGridSummary.DataContext = summaries;

            labelSummary.Content = $"Summary of enumerators for {region}";
            labelSummary2.Content = "Latest upload to server and number of landings sampled";
            labelSummary2.Visibility = Visibility.Visible;
            dataGridSummary.Visibility = Visibility.Visible;
            panelOpening.Visibility = Visibility.Visible;
        }
        private void ShowEnumeratorSummary(NSAPEnumerator enumerator, DateTime? monthSampled = null)
        {
            string titleLabel = "Enumerator summaries";
            List<EnumeratorSummary> summaries = new List<EnumeratorSummary>();
            if (enumerator.Name != "Summary of enumerators")
            {
                titleLabel = $"Summary for {enumerator.ToString()}";
                if (monthSampled != null)
                {
                    SetUpSummaryGrid(SummaryLevelType.EnumeratedMonth, dataGridSummary);
                    summaries = NSAPEntities.NSAPEnumeratorViewModel.GetSummary(enumerator, (DateTime)monthSampled);
                    titleLabel = $"Monthly summary for {enumerator.ToString()} on {((DateTime)monthSampled).ToString("MMMM, yyyy")}";
                }
                else
                {
                    SetUpSummaryGrid(SummaryLevelType.Enumerator, dataGridSummary);
                    summaries = NSAPEntities.NSAPEnumeratorViewModel.GetSummary(enumerator);


                }
            }
            else
            {
                SetUpSummaryGrid(SummaryLevelType.SummaryOfEnumerators, dataGridSummary);
                var region = ((TreeViewItem)_selectedTreeNode.Parent).Tag as NSAPRegion;
                summaries = NSAPEntities.NSAPEnumeratorViewModel.GetSummary(region, summaryForAll: true);

                titleLabel = $"Summary of enumerators for {region.Name}";
                labelSummary2.Content = "Total count of landings enumerated and number of landings with catch composition, by enumerator";
                labelSummary2.Visibility = Visibility.Visible;

            }

            dataGridSummary.DataContext = summaries;


            labelSummary.Content = titleLabel;
            dataGridSummary.Visibility = Visibility.Visible;
            panelOpening.Visibility = Visibility.Visible;

        }
        private void OnSummaryTreeItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            labelSummary.Content = "To be implemented";
            treeViewSummary.Visibility = Visibility.Visible;
            panelSummaryLabel.Visibility = Visibility.Visible;

            propertyGridSummary.Visibility = Visibility.Collapsed;
            dataGridSummary.Visibility = Visibility.Collapsed;
            checkLandingSiteWithLandings.Visibility = Visibility.Collapsed;
            if (e.NewValue != null)
            {
                _selectedTreeNode = (TreeViewItem)e.NewValue;
                ProcessSummaryTreeSelection(_selectedTreeNode);
            }
        }

        private void OnCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            switch (chk.Name)
            {
                case "checkLandingSiteWithLandings":
                    ProcessSummaryTreeSelection(_selectedTreeNode);
                    break;
            }
        }

        private void ShowToBeImplemented()
        {
            MessageBox.Show("This feature is not yet implemented", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
        }



        private void ShowSettingsWindow()
        {
            //SettingsWindow sw = new SettingsWindow(this);
            SettingsWindow sw = new SettingsWindow();
            sw.Owner = this;
            sw.ShowDialog();

        }
        private async void OnToolbarButtonClick(object sender, RoutedEventArgs e)
        {
            ShowStatusRow();
            menuDatabaseSummary.IsChecked = false;
            switch (((Button)sender).Name)
            {
                case "buttonGeneratecsv":
                    await GenerateAllCSV();
                    break;
                case "buttonSummary":
                    menuDatabaseSummary.IsChecked = true;
                    break;
                case "buttonAbout":
                    AboutWindow aw = new AboutWindow();
                    aw.ShowDialog();
                    break;
                case "buttonSettings":
                    ShowSettingsWindow();
                    break;
                case "buttonExit":
                    Close();
                    break;
                case "buttonCalendar":
                    ShowNSAPCalendar();
                    break;
                case "buttonDownloadHistory":
                    _currentDisplayMode = DataDisplayMode.DownloadHistory;
                    ColumnForTreeView.Width = new GridLength(1, GridUnitType.Star);
                    SetDataDisplayMode();
                    break;
            }
        }

        private void OnTreeMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _selectedRegionInSummary = null;
            _selectFishingGroundInSummary = null;
            ContextMenu cm = new ContextMenu();
            MenuItem m = null;
            switch (((TreeView)sender).Name)
            {
                case "treeViewSummary":
                    if (((TreeViewItem)((TreeView)sender).SelectedItem).Tag.GetType().Name == "FishingGround")
                    {
                        _selectFishingGroundInSummary = ((TreeViewItem)((TreeView)sender).SelectedItem).Tag as FishingGround;
                        _selectedRegionInSummary = ((TreeViewItem)((TreeViewItem)((TreeView)sender).SelectedItem).Parent).Tag as NSAPRegion;
                        m = new MenuItem { Header = "Export vessel sampling with catch maturity", Name = "menuExportExcelMaturity" };
                        m.Click += OnMenuClicked;
                        cm.Items.Add(m);
                    }
                    break;
            }

            if (cm.Items.Count > 0)
            {
                cm.IsOpen = true;
            }

        }

        private void OnMainMenuGotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void OnGridGotFocus(object sender, RoutedEventArgs e)
        {
            _dataGrid = (DataGrid)sender;
            Title = _dataGrid.Name;
        }

        private void OnMenuRightClick(object sender, MouseButtonEventArgs e)
        {
            _dataGrid = (DataGrid)sender;
            ContextMenu cm = new ContextMenu();
            MenuItem m = null;
            m = new MenuItem { Header = "Copy text", Name = "menuCopyText" };
            m.Click += OnMenuClicked;
            cm.Items.Add(m);
            cm.IsOpen = true;
        }

        private PropertyGrid _propertyGrid;
        private void OnPropertyGridFGotFocus(object sender, RoutedEventArgs e)
        {
            _propertyGrid = (PropertyGrid)sender;

        }

        private void OnPropertyGridContextMenu(object sender, MouseButtonEventArgs e)
        {
            _propertyGrid = (PropertyGrid)sender;
            ContextMenu cm = new ContextMenu();
            MenuItem m = null;
            m = new MenuItem { Header = "Copy text", Name = "menuCopyTextPropertyGrid" };
            m.Click += OnMenuClicked;
            cm.Items.Add(m);
            cm.IsOpen = true;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}