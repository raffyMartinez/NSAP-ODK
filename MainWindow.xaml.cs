﻿using NSAP_ODK.Entities;
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
using System.Net.Http;
using System.Windows.Media;
using System.Diagnostics;
using NSAP_ODK.Entities.Database.NSAPReports;
using NSAP_ODK.Entities.CrossTabBuilder;
using NSAP_ODK.TreeViewModelControl;
using NSAP_ODK.Mapping.views;
//using DocumentFormat.OpenXml.Wordprocessing;

namespace NSAP_ODK
{
    public enum DataDisplayMode
    {
        Dashboard,
        ODKData,
        Species,
        DownloadHistory,
        Others,
        DBSummary,
        CBLSampling
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        //private CalendarViewType _calendarViewType;
        private string _calendarGridColumnName;
        private bool _calendarDayHasValue = false;
        private string _calendarDayValue;
        private string _sector_code = "";
        private string _speciesTaxa;
        private string _speciesName;
        private bool _cancelBuildCalendar = false;
        private bool _hasNonSamplingDayColumns = false;
        private CalendarViewType _calendarOption;
        private bool _calendarFirstInvokeDone = false;
        WeightValidationTallyWindow _wvtw;
        private NSAPEntity _nsapEntity;
        private string _csvSaveToFolder = "";
        private FishingCalendarViewModel _fishingCalendarViewModel;
        private CarrierLanding _carrierLandingFromCalendar;
        private int _gridCol;
        private int _gridRow;
        private int? _calendarDay;
        private string _gearCode;
        private string _gearName;
        private string _fish_sector;
        private string _carrierBoatName;
        private DateTime? _monthYear;
        private TreeViewModelControl.AllSamplingEntitiesEventHandler _treeItemData;
        private string _calendarTreeSelectedEntity;
        private GearUnload _gearUnload;
        private List<GearUnload> _gearUnloads;
        private GearUnloadWindow _gearUnloadWindow;
        private Dictionary<DateTime, List<SummaryItem>> _vesselDownloadHistory;
        private DataDisplayMode _currentDisplayMode;
        //private VesselUnloadWIndow _vesselUnloadWindow;
        private VesselUnloadEditWindow _vesselUnloadEditWindow;
        private List<GearUnload> _gearUnloadList;
        private bool _saveChangesToGearUnload;
        private FishingGround _fishingGroundMoveDestination;
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
        private static HttpClient _httpClient = new HttpClient();
        private Koboserver _selectedKoboserver;
        private TreeViewItem _parentCBLNode;
        private LandingSite _landingSite;
        private bool _isWatchedSpeciesCalendar;
        private bool _isMeasuredWatchedSpeciesCalendar;
        DateTime _downloadHistorySelectedItem;
        private bool _getFemaleMaturity;
        private string _maturityStage;
        private string _species_measurement_type;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
            Closing += OnWindowClosing;
        }

        public string CommandArgs { get; set; }
        public DBView DBView { get; set; }
        public static HttpClient HttpClient
        {
            get
            {
                return _httpClient;
            }
        }

        public DataDisplayMode DataDisplayMode { get { return _currentDisplayMode; } }
        private void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            ShowStatusRow(false);

        }
        private bool DownloadCSVFromServer()
        {
            ODKResultsWindow window = new ODKResultsWindow();
            window.Owner = this;
            window.DownloadCSVFromServer = true;
            window.OpenLogInWindow(isOpen: true);
            return (bool)window.ShowDialog();
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
            if (NSAPEntities.SummaryItemViewModel != null)
            {
                NSAPEntities.SummaryItemViewModel.BuildingSummaryTable -= SummaryItemViewModel_BuildingSummaryTable;
                NSAPEntities.SummaryItemViewModel.BuildingOrphanedEntity -= SummaryItemViewModel_BuildingOrphanedEntity;
                DownloadFromServerWindow.RefreshDatabaseSummaryTable -= DownloadFromServerWindow_RefreshDatabaseSummaryTable;
                NSAPEntities.FishingVesselViewModel.ProcessingItemsEvent -= OnProcessingItemsEvent;
                NSAPEntities.SummaryItemViewModel.ProcessingItemsEvent -= OnProcessingItemsEvent;
                GearUnloadRepository.ProcessingItemsEvent -= OnProcessingItemsEvent;
                NSAPEntities.LandingSiteSamplingViewModel.ProcessingItemsEvent -= OnProcessingItemsEvent;

            }
            _httpClient.Dispose();

            if (Global.IsMapComponentRegistered)
            {
                Mapping.globalMapping.Cleanup();
            }

            //if (_vesselUnloadEditWindow?.UnloadEditor != null)
            //{
            //    _vesselUnloadEditWindow.UnloadEditor.UnloadChangesSaved += OnUnloadEditor_UnloadChangesSaved;
            //}
        }

        public Task<bool> ShowSplashAsync()
        {
            return Task.Run(() => ShowSplash());
        }
        public bool ShowSplash()
        {
            SplashWindow sw = new SplashWindow();
            sw.CommandArgs = Global.CommandArgs;
            sw.Owner = this;
            if ((bool)sw.ShowDialog())
            {
                return (bool)sw.DialogResult;
            }
            return false;
        }

        private System.Windows.Style AlignRightStyle
        {
            get
            {
                System.Windows.Style alignRightCellStype = new System.Windows.Style(typeof(DataGridCell));

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
            panelSummaryTableHint.Visibility = Visibility.Collapsed;




            switch (level)
            {
                case "Overall":

                    if (NSAPEntities.DBSummary != null)
                    {
                        NSAPEntities.DBSummary.Refresh();
                    }
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


                    if (!string.IsNullOrEmpty(NSAPEntities.DBSummary.FilterType))
                    {
                        propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Filter type", Name = "FilterType", Description = "Type of filter", DisplayOrder = 13, Category = "Filters" });
                        propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Filter", Name = "Filter", Description = "Filter", DisplayOrder = 14, Category = "Filters" });
                        propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of unfiltered landings", Name = "CountAllLandings", Description = "Number of landings when not filtered", DisplayOrder = 15, Category = "Filters" });
                    }
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of gear unload", Name = "GearUnloadCount", Description = "Number of gear unload", DisplayOrder = 15, Category = "Submitted fish landing data" });



                    //propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of complete gear unload", Name = "CountCompleteGearUnload", Description = "Number of gear unload", DisplayOrder = 14, Category = "Submitted fish landing data" });

                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of landings", Name = "VesselUnloadCount", Description = "Number of vessel unload", DisplayOrder = 15, Category = "Submitted fish landing data" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of tracked operations", Name = "TrackedOperationsCount", Description = "Number of tracked fishing operations", DisplayOrder = 16, Category = "Submitted fish landing data" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Date of first sampled landing", Name = "FirstSampledLandingDate", Description = "Date of first sampled operation", DisplayOrder = 17, Category = "Submitted fish landing data" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Date of last sampled landing", Name = "LastSampledLandingDate", Description = "Date of last sampled operation", DisplayOrder = 18, Category = "Submitted fish landing data" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Date of latest download", Name = "DateLastDownload", Description = "Date of latest download", DisplayOrder = 19, Category = "Submitted fish landing data" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of landings including catch composition", Name = "CountLandingsWithCatchComposition", Description = "Number of sampled landings with included catch composition data", DisplayOrder = 20, Category = "Submitted fish landing data" });

                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of landings with missing Form IDs", Name = "CountMissingFormIDs", Description = "Number of saved forms with missing identifier", DisplayOrder = 1, Category = "Data quality check" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of landings with missing landing site information", Name = "CountMissinsLandingSiteInformation", Description = "Number of saved forms with missing information on landing sites", DisplayOrder = 2, Category = "Data quality check" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of landings with missing fishing gear information", Name = "CountMissingFishingGearInformation", Description = "Number of saved forms with missing information on fishing gears", DisplayOrder = 3, Category = "Data quality check" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of landings with missing enumerator information", Name = "CountMissingEnumeratorInformation", Description = "Number of saved forms with missing information on enumerators", DisplayOrder = 4, Category = "Data quality check" });

                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of landings with orphaned enumerators", Name = "CountLandingsWithOrphanedEnumerators", Description = "Number of saved forms with orphaned enumerator names", DisplayOrder = 5, Category = "Data quality check" });

                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of landings with orphaned landing sites", Name = "CountLandingsWithOrphanedLandingSites", Description = "Number of saved forms with orphaned landing site names", DisplayOrder = 6, Category = "Data quality check" });

                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of gear unloads with orphaned fishing gears", Name = "CountLandingsWithOrphanedFishingGears", Description = "Number of gear unloads with orphaned fishing gear names", DisplayOrder = 7, Category = "Data quality check" });

                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of landings with orphaned fishing vessels", Name = "CountLandingsWithOrphanedFishingVessels", Description = "Number of saved forms with orphaned fishing vessel names", DisplayOrder = 8, Category = "Data quality check" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of landings with orphaned species names", Name = "CountLandingsWithOrphanedSpeciesNames", Description = "Number of saved forms with orphaned species names", DisplayOrder = 9, Category = "Data quality check" });

                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Saved JSON files folder", Name = "SavedJSONFolder", Description = "Folder containing saved JSON data. Double click to open folder", DisplayOrder = 1, Category = "Saved JSON files" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of saved catch and effort monitoring JSON files", Name = "SavedFishingEffortJSONCount", Description = "Number of saved JSON files containing catch and effort monitoring data", DisplayOrder = 2, Category = "Saved JSON files" });

                    propertyGridSummary.Visibility = Visibility.Visible;

                    break;
                case "Enumerators and form versions":
                    dataGridEFormVersionStats.ContextMenu.Visibility = Visibility.Collapsed;
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


                    //dataGridEFormVersionStats.DataContext = NSAPEntities.SummaryItemViewModel.EnumeratorsAndLatestFormVersion();
                    dataGridEFormVersionStats.ItemsSource = null;
                    dataGridEFormVersionStats.Items.Clear();
                    dataGridEFormVersionStats.ItemsSource = NSAPEntities.SummaryItemViewModel.EnumeratorsAndLatestFormVersion();
                    break;
                case "e-Form versions":
                    dataGridEFormVersionStats.ContextMenu.Visibility = Visibility.Collapsed;
                    labelSummary.Content = "eForm versions, number of submitted landings anded date of first submission";
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
                    //dataGridEFormVersionStats.DataContext = NSAPEntities.ODKEformVersionViewModel.ODKEformVersionCollection.ToList();
                    dataGridEFormVersionStats.ItemsSource = null;
                    dataGridEFormVersionStats.Items.Clear();
                    dataGridEFormVersionStats.ItemsSource = NSAPEntities.ODKEformVersionViewModel.ODKEformVersionCollection.ToList();
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
                    _selectedKoboserver = null;
                    dataGridEFormVersionStats.ContextMenu.IsOpen = false;
                    dataGridEFormVersionStats.ContextMenu.Visibility = Visibility.Visible;

                    panelVersionStats.Visibility = Visibility.Visible;
                    labelSummary.Content = "Summary of online databases (Kobotoolbox)";
                    dataGridEFormVersionStats.AutoGenerateColumns = false;
                    dataGridEFormVersionStats.Columns.Clear();

                    dataGridEFormVersionStats.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("ServerNumericID") });
                    dataGridEFormVersionStats.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("FormName") });
                    dataGridEFormVersionStats.Columns.Add(new DataGridTextColumn { Header = "Owner", Binding = new Binding("Owner") });
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
                    dataGridEFormVersionStats.Columns.Add(new DataGridCheckBoxColumn { Header = "MultiGear landing form", Binding = new Binding("IsFishLandingMultiGearSurveyForm") });
                    dataGridEFormVersionStats.Columns.Add(new DataGridCheckBoxColumn { Header = "Multivessel landing form", Binding = new Binding("IsFishLandingMultiVesselSurveyForm") });
                    dataGridEFormVersionStats.Columns.Add(new DataGridTextColumn { Header = "Last uploaded JSON file", Binding = new Binding("LastUploadedJSON") });
                    dataGridEFormVersionStats.Columns.Add(new DataGridTextColumn { Header = "Last created JSON file", Binding = new Binding("LastCreatedJSON") });



                    dataGridEFormVersionStats.ItemsSource = null;
                    dataGridEFormVersionStats.Items.Clear();
                    if (NSAPEntities.KoboServerViewModel.Count() > 0)
                    {
                        NSAPEntities.KoboServerViewModel.RefreshSavedCount();
                        //dataGridEFormVersionStats.DataContext = NSAPEntities.KoboServerViewModel.KoboserverCollection.ToList();


                        dataGridEFormVersionStats.ItemsSource = NSAPEntities.KoboServerViewModel.KoboserverCollection.ToList();
                    }
                    panelSummaryTableHint.Visibility = Visibility.Visible;
                    break;
            }



        }

        private async Task ShowServerMonthlySummary(Koboserver ks)
        {
            labelSummary.Content = $"Monthly summary statistics for server: {ks.FormName}";
            panelSummaryTableHint.Visibility = Visibility.Collapsed;
            panelVersionStats.Visibility = Visibility.Visible;
            dataGridEFormVersionStats.AutoGenerateColumns = false;
            dataGridEFormVersionStats.Columns.Clear();
            dataGridEFormVersionStats.ItemsSource = null;
            dataGridEFormVersionStats.Items.Clear();


            DataGridTextColumn col = new DataGridTextColumn()
            {
                Binding = new Binding("MonthOfSubmission"),
                Header = "Month submitted",
                CellStyle = AlignRightStyle
            };
            col.Binding.StringFormat = "MMM-yyyy";
            dataGridEFormVersionStats.Columns.Add(col);

            dataGridEFormVersionStats.Columns.Add(new DataGridTextColumn { Header = "Server ID", Binding = new Binding("Koboserver.ServerNumericID") });
            dataGridEFormVersionStats.Columns.Add(new DataGridTextColumn { Header = "Owner", Binding = new Binding("Koboserver.Owner") });
            dataGridEFormVersionStats.Columns.Add(new DataGridTextColumn { Header = "Number of submissions", Binding = new Binding("CountUploads"), CellStyle = AlignRightStyle });
            dataGridEFormVersionStats.Columns.Add(new DataGridTextColumn { Header = "Number of unique enumerators", Binding = new Binding("CountEnumerators"), CellStyle = AlignRightStyle });

            dataGridEFormVersionStats.ItemsSource = await NSAPEntities.SummaryItemViewModel.ListServerUploadsByMonthsAsync(ks);
        }

        private void SetUpCalendarMenu()
        {

            foreach (Control m in menuCalendar.Items)
            {
                if (m.Name == "menuSampledCalendar")
                {
                    ((MenuItem)m).IsChecked = true;
                }
                else if (m.GetType().Name != "Separator")
                {
                    ((MenuItem)m).IsChecked = false;
                }
            }
        }

        private async void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Global.RequestLogIn += OnMysQLRequestLogin;

                Global.DoAppProceed();
                if (Global.AppProceed)
                {
                    _currentDisplayMode = DataDisplayMode.Dashboard;
                    SetDataDisplayMode();

                    if (!Global.Settings.UsemySQL || !Global.MySQLLogInCancelled)
                    {
                        //ShowSplash();

                        //await ShowSplashAsync();
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


                            //SetUpCalendarMenu();
                            menuDatabaseSummary.IsChecked = true;

                            CrossTabManager.CrossTabEvent += OnCrossTabEvent;
                            _timer = new DispatcherTimer();
                            _timer.Tick += OnTimerTick;
                            if (NSAPEntities.SummaryItemViewModel != null)
                            {
                                NSAPEntities.SummaryItemViewModel.BuildingSummaryTable += SummaryItemViewModel_BuildingSummaryTable;
                                NSAPEntities.SummaryItemViewModel.BuildingOrphanedEntity += SummaryItemViewModel_BuildingOrphanedEntity;
                                DownloadFromServerWindow.RefreshDatabaseSummaryTable += DownloadFromServerWindow_RefreshDatabaseSummaryTable;
                                NSAPEntities.FishingVesselViewModel.ProcessingItemsEvent += OnProcessingItemsEvent;
                                NSAPEntities.SummaryItemViewModel.ProcessingItemsEvent += OnProcessingItemsEvent;
                                GearUnloadRepository.ProcessingItemsEvent += OnProcessingItemsEvent;
                                NSAPEntities.LandingSiteSamplingViewModel.ProcessingItemsEvent += OnProcessingItemsEvent;
                                treeViewDownloadHistory.MouseRightButtonUp += OnMenuRightClick;


                            }
                            CreateTablesInAccess.GetMDBColumnInfo(Global.ConnectionString);
                            _httpClient.Timeout = new TimeSpan(0, 10, 0);

                            if (Global.CommandArgs != null)
                            {
                                if (Global.CommandArgs.Count() >= 1 && (Global.Filter1 != null || Global.CommandArgs.Contains("server_id")))
                                {
                                    Title += " (Filtered)";
                                }

                                if (Global.CommandArgs.Contains("calendar_logging"))
                                {
                                    Title += " (Calendar debugging mode)";
                                }
                            }

                            if (Global.HasCarrierBoatLandings)
                            {
                                buttonCBL_calendar.Visibility = Visibility.Visible;
                            }



                            Global.OfficeIs64Bit(write_to_log: true);

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

                }

                if (Global.Settings != null)
                {
                    if (!File.Exists(Global.Settings.MDBPath))
                    {
                        ResetDisplay();
                        ShowDatabaseNotFoundView();
                    }
                    else if (Global.Settings.UsemySQL)
                    {
                        Title += " - MySQL";
                    }
                }
                else if (Global.Settings == null)
                {
                    ResetDisplay();
                    ShowDatabaseNotFoundView();
                }

                if (!string.IsNullOrEmpty(Global.FilterError))
                {
                    ShowDatabaseNotFoundView(isFilterError: true);
                    MessageBox.Show(Global.FilterError, Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("handler for things called in MainWindow.OnWindowLoaded", ex);
            }

        }



        private void OnProcessingItemsEvent(object sender, ProcessingItemsEventArg e)
        {
            switch (e.Intent)
            {
                case "start":
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
                              mainStatusLabel.Content = "Getting results from database...";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "end":
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                            DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                            {

                                mainStatusBar.IsIndeterminate = false;
                                mainStatusBar.Value = 0;
                                //do what you need to do on UI Thread
                                return null;
                            }), null);
                    break;
                case "start build calendar":
                    labelRowCount.Dispatcher.BeginInvoke
                        (
                        DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                        {
                            labelRowCount.Visibility = Visibility.Visible;
                            labelRowCount.Content = "Please wait while building calendar...";
                            return null;
                        }), null
                        );
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              mainStatusBar.IsIndeterminate = false;
                              mainStatusBar.Value = 0;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = "Building calendar. Please wait...";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "calendar build started":
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              mainStatusBar.Maximum = e.TotalCountToProcess;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = "Calendar is now building...";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "calendar item created":
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              mainStatusBar.Value = e.CountProcessed;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = $"Created calendar item {e.CountProcessed} of {mainStatusBar.Maximum}";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "end build calendar":
                    labelRowCount.Dispatcher.BeginInvoke
                        (
                        DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                        {
                            var totlaLandingsCount = "";
                            if (_fishingCalendarViewModel != null && _allSamplingEntitiesEventHandler.CalendarView != CalendarViewType.calendarViewTypeGearDailyLandings)
                            {
                                int totalLandingsCount = _fishingCalendarViewModel.CountVesselUnloads;
                                totlaLandingsCount = $", Total landings: {totalLandingsCount}";
                                int vuCountInGU = 0;
                                foreach (var gu in _fishingCalendarViewModel.UnloadList)
                                {
                                    vuCountInGU += gu.ListVesselUnload.Count;
                                }
                                if (vuCountInGU != totalLandingsCount)
                                {
                                    totlaLandingsCount += $" ({vuCountInGU})";
                                    buttonNote.Visibility = Visibility.Visible;
                                }
                            }
                            if (GridNSAPData.Items.Count > 0)
                            {
                                string columnStyle = " (Blue columns represent rest day)";
                                if (!_hasNonSamplingDayColumns)
                                {
                                    columnStyle = "";
                                }
                                //labelRowCount.Content = $"Rows: {GridNSAPData.Items.Count}{mainStatusBar.Value}{columnStyle}";
                                labelRowCount.Content = $"Rows: {GridNSAPData.Items.Count} {totlaLandingsCount} {columnStyle}";
                            }
                            else
                            {
                                labelRowCount.Content = "Older eforms cannot encode data. Use Catch and Effort eForm version 7.14";
                            }
                            return null;
                        }), null
                        );
                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = $"Finished building calendar";
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
                case "imported_entity":
                    if (e.CountProcessed % 100 == 0)
                    {
                        this.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              RefreshEntityGrid();
                              //do what you need to do on UI Thread
                              return null;
                          }), null);
                        //((MainWindow)Owner).RefreshEntityGrid();
                    }
                    break;
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

        private void ShowStatusRow(bool isVisible = true, bool resetIndicators = true)
        {
            if (!isVisible)
            {
                rowStatus.Height = new GridLength(0);
            }
            else
            {
                rowStatus.Height = new GridLength(30, GridUnitType.Pixel);
            }
            if (resetIndicators)
            {
                mainStatusBar.Value = 0;
                dbPathLabel.Content = string.Empty;
                mainStatusLabel.Content = string.Empty;
            }

        }
        private void ShowTitleAndStatusRow(bool isVisible = true)
        {

            rowTopLabel.Height = new GridLength(30, GridUnitType.Pixel);
            ShowStatusRow(isVisible: false);
            PanelButtons.Visibility = Visibility.Visible;
        }
        private void ResetDisplay()
        {
            rowCBL.Height = new GridLength(0);
            rowTopLabel.Height = new GridLength(0);
            rowSpecies.Height = new GridLength(0);
            rowODKData.Height = new GridLength(0);
            rowOthers.Height = new GridLength(0);
            rowSummary.Height = new GridLength(0);
            rowStatus.Height = new GridLength(0);

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
                case DataDisplayMode.CBLSampling:
                    rowCBL.Height = new GridLength(1, GridUnitType.Star);
                    //treeViewDownloadHistory.Visibility = Visibility.Visible;
                    treeCBL.Items.Clear();
                    _parentCBLNode = new TreeViewItem { Header = "Sampling of fish carriers", Tag = "cbl_landings" };
                    treeCBL.Items.Add(_parentCBLNode);
                    foreach (var ls in NSAPEntities.LandingSiteViewModel.GetCarrierLandingLandingSites())
                    {
                        TreeViewItem lsNode = new TreeViewItem { Header = ls.ToString(), Tag = ls };
                        _parentCBLNode.Items.Add(lsNode);
                        TreeViewItem dummyNode = new TreeViewItem { Header = "--dummy" };
                        lsNode.Items.Add(dummyNode);
                        lsNode.Expanded += OnTreeviewNodeExpanded;
                    }
                    if (_parentCBLNode.Items.Count > 0)
                    {
                        _parentCBLNode.IsExpanded = true;
                    }
                    _parentCBLNode.IsSelected = true;
                    break;
                case DataDisplayMode.DownloadHistory:
                    if (NSAPEntities.LandingSiteSamplingViewModel.Count > 0)
                    {
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
                            //tvItem.Items.Add(new TreeViewItem { Header = "Weights", Tag = "weights" });
                            tvItem.Items.Add(new TreeViewItem { Header = "Tracked fishing effort", Tag = "tracked" });
                            tvItem.Items.Add(new TreeViewItem { Header = "Gear unload", Tag = "gearUnload" });
                            tvItem.Items.Add(new TreeViewItem { Header = "Gear unload (Multiple vessel)", Tag = "gearUnload_mv" });
                            //tvItem.Items.Add(new TreeViewItem { Header = "Unload summary", Tag = "unloadSummary" });

                            TreeViewItem tv = new TreeViewItem { Header = "JSON analysis", Tag = "jsonAnalysis" };
                            tv.Expanded += onJsonDummyNode_Expanded;
                            tv.Items.Add(new TreeViewItem { Header = "json_dummy" });

                            tvItem.Items.Add(tv);
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
                    }
                    else
                    {
                        labelTitle.Content = "Database content is empty.";
                        labelTitle.Visibility = Visibility.Visible;
                        ShowDatabaseNotFoundView(isEmpty: true);
                    }

                    break;
                case DataDisplayMode.Dashboard:
                    break;
                case DataDisplayMode.ODKData:
                    if (NSAPEntities.LandingSiteSamplingViewModel.Count > 0)
                    {
                        GridNSAPData.SelectionUnit = DataGridSelectionUnit.Cell;
                        GridNSAPData.SetValue(Grid.ColumnSpanProperty, 2);
                        GridNSAPData.Visibility = Visibility.Visible;
                        rowODKData.Height = new GridLength(1, GridUnitType.Star);
                        samplingTree.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        ShowDatabaseNotFoundView(isEmpty: true);
                    }
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
                    if (NSAPEntities.SummaryItemViewModel != null && NSAPEntities.SummaryItemViewModel.Count > 0)
                    {
                        summaryTreeNodeDatabases.Items.Clear();
                        foreach (var item in NSAPEntities.KoboServerViewModel.KoboserverCollection)
                        {
                            TreeViewItem nodeKoboServer = new TreeViewItem { Header = item.FormName, Tag = item };
                            summaryTreeNodeDatabases.Items.Add(nodeKoboServer);
                        }
                    }

                    ShowTitleAndStatusRow();

                    break;
            }
        }

        private void OnTreeviewNodeExpanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = (TreeViewItem)sender;
            LandingSite ls = (LandingSite)tvi.Tag;

            if (((TreeViewItem)tvi.Items[0]).Header.ToString() == "--dummy")
            {
                tvi.Items.Clear();

                var months = NSAPEntities.LandingSiteSamplingViewModel.MonthsSampledForCBL(ls);

                foreach (var m in months)
                {
                    TreeViewItem monthNode = new TreeViewItem { Header = m.ToString("MMM-yyyy"), Tag = tvi.Tag };
                    tvi.Items.Add(monthNode);
                }
            }

        }

        private void onJsonDummyNode_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem node = e.Source as TreeViewItem;
            if (node.Tag.ToString() == "jsonAnalysis")
            {
                node.Items.Clear();
                DateTime dateDownload = DateTime.Parse(((TreeViewItem)node.Parent).Header.ToString());
                foreach (var item in NSAPEntities.UnmatchedFieldsFromJSONFileViewModel.UnmatchedFieldsFromJSONFileCollection.Where(t => t.DateOfParsing.Date == dateDownload.Date).OrderBy(t => t.DateOfParsing))
                {
                    TreeViewItem tv = new TreeViewItem { Header = $"{item.RowID} - {item.DateOfParsing.ToString("MMM-dd-yyyy HH:mm")}", Tag = item };
                    node.Items.Add(tv);
                }
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
                            foreach (var fma in ((NSAPRegion)node.Tag).FMAs)
                            {
                                TreeViewItem fmaNode = new TreeViewItem { Header = fma.FMA.Name, Tag = fma };
                                node.Items.Add(fmaNode);
                                foreach (var fg in fma.FishingGrounds)
                                {
                                    fmaNode.Items.Add(new TreeViewItem { Header = fg.FishingGround.Name, Tag = fg });
                                }

                            }
                            //foreach (var fg in NSAPEntities.NSAPRegionViewModel.GetFishingGrounds((NSAPRegion)node.Tag))
                            //{
                            //    TreeViewItem fgNode = new TreeViewItem { Header = fg.Name, Tag = fg };
                            //    node.Items.Add(fgNode);
                            //}
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

        public void SetMenuAndOtherToolbarButtonsVisibility(Visibility visibility, bool databaseIsEmtpy = false)
        {
            if (!databaseIsEmtpy)
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
        }


        private void ShowDatabaseNotFoundView(bool isFilterError = false, bool isEmpty = false)
        {
            rowTopLabel.Height = new GridLength(300);
            if (isEmpty)
            {
                labelTitle.Content = "Database is empty. Connect to Kobotoolbox server and download fish landing data to populate the local database.";
            }
            else if (isFilterError)
            {
                labelTitle.Content = "Cannot understand database filter\r\n\r\nIf there are two dates in the filter, the first date\r\n" +
                                      "must be before the second date";
            }
            else
            {
                labelTitle.Content = "Backend database file not found.\r\n\r\nMake sure that the correct database is found in the application folder\r\n" +
                                      "or in the folder used for saving NSAP data.\r\n" +
                                      "The application folder is the folder where you installed this software\r\n\r\n" +
                                      "Click on the Settings button in the toolbar to setup the database";
            }
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

            rowCBL.Height = new GridLength(0);
            rowODKData.Height = new GridLength(0);
            rowSpecies.Height = new GridLength(0);
            rowOthers.Height = new GridLength(0);
            rowStatus.Height = new GridLength(0);
            statusBar.Visibility = Visibility.Collapsed;

            dataGridSpecies.Visibility = Visibility.Collapsed;
            PanelButtons.Visibility = Visibility.Collapsed;

            SetMenuAndOtherToolbarButtonsVisibility(Visibility.Collapsed, databaseIsEmtpy: isEmpty);
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
                    dataGridEntities.Columns.Add(new DataGridCheckBoxColumn { Header = "Not used", Binding = new Binding("GearIsNotUsed") });
                    dataGridEntities.Columns.Add(new DataGridCheckBoxColumn { Header = "Gear is used in large commercial vessels", Binding = new Binding("IsUsedInLargeCommercial") });
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
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Type of sampling", Binding = new Binding("TypeOfSamplingInLandingSite") });

                    break;

                case NSAPEntity.NSAPRegion:
                    buttonAdd.IsEnabled = false;
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Code", Binding = new Binding("Code") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Region name", Binding = new Binding("Name") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Short name", Binding = new Binding("ShortName") });
                    dataGridEntities.Columns.Add(new DataGridCheckBoxColumn { Header = "Total enumeration only", Binding = new Binding("IsTotalEnumerationOnly") });
                    dataGridEntities.Columns.Add(new DataGridCheckBoxColumn { Header = "Regular sampling only", Binding = new Binding("IsRegularSamplingOnly") });

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
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("Name") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Size measure", Binding = new Binding("SizeType.Code") });
                    dataGridEntities.Columns.Add(new DataGridTextColumn { Header = "Maximum size", Binding = new Binding("MaxSize") });
                    break;
            }
            dataGridEntities.Visibility = Visibility.Visible;
        }

        private void OnDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _monthYear = null;
            switch (((DataGrid)sender).Name)
            {
                case "GridNSAPData":
                    if (_calendarTreeSelectedEntity == "tv_LandingSiteViewModel")
                    {
                        SummaryResults item = (SummaryResults)GridNSAPData.SelectedItem;

                        //var item = GridNSAPData.Items[_gridRow] as DataRowView;
                        if (item != null)
                        {
                            _monthYear = DateTime.Parse(item.DBSummary.MonthSampled);
                        }
                    }
                    else if (GridNSAPData.SelectedItem != null && GridNSAPData.SelectedItem.GetType().Name == "SummaryItem")
                    {
                        if (_vesselUnloadEditWindow != null)
                        {
                            _vesselUnloadEditWindow.VesselUnload = ((SummaryItem)GridNSAPData.SelectedItem).VesselUnload;
                        }
                    }
                    break;
                case "dataGridEFormVersionStats":
                    if (dataGridEFormVersionStats.SelectedItem != null)
                    {
                        switch (dataGridEFormVersionStats.SelectedItem.GetType().Name)
                        {
                            case "Koboserver":
                                _selectedKoboserver = (Koboserver)dataGridEFormVersionStats.SelectedItem;
                                break;
                            default:
                                break;
                        }
                    }
                    break;
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
                    if (Global.HasCarrierBoatLandings)
                    {
                        buttonCBL_calendar.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        buttonCBL_calendar.Visibility = Visibility.Collapsed;
                    }

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

        public static string GetPathToFBSpeciesMDB()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Locate Fishbase species database file (MDB)";
            ofd.DefaultExt = ".mdb";
            ofd.Filter = "Microsoft Access Database (*.mdb)|*.mdb|All files (*.*)|*.*";
            if (Global.Settings.PathToFBSpeciesMDB == null)
            {
                ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            }
            else
            {
                ofd.InitialDirectory = Path.GetDirectoryName(Global.Settings.PathToFBSpeciesMDB);
                ofd.FileName = Global.Settings.PathToFBSpeciesMDB;
            }
            if ((bool)ofd.ShowDialog() && File.Exists(ofd.FileName))
            {
                return ofd.FileName;
            }
            return "";
        }
        public void SetNSAPEntity(NSAPEntity en)
        {
            _nsapEntity = en;
        }
        public void NewSpeciesEditedSuccess()
        {
            SetDataGridSource();
        }
        public void AddEntity(string genus = "", string species = "")
        {
            //string pathToFbSpeciesMD = "";
            bool proceed = false;

            EditWindowEx ew = new EditWindowEx(_nsapEntity);
            ew.Genus = genus;
            ew.Species = species;
            if (_nsapEntity == NSAPEntity.FishSpecies)
            {
                //if (NSAPEntities.FBSpeciesViewModel == null || NSAPEntities.FBSpeciesViewModel.ErrorInGettingFishSpeciesFromExternalFile().Length > 0)
                if (string.IsNullOrEmpty(Global.Settings.PathToFBSpeciesMDB))
                {
                    //pathToFbSpeciesMD = ;
                    ew.PathToFBSpeciesMDB = GetPathToFBSpeciesMDB();
                }
                else
                {
                    ew.PathToFBSpeciesMDB = Global.Settings.PathToFBSpeciesMDB;
                }
                proceed = NSAPEntities.FBSpeciesViewModel != null && NSAPEntities.FBSpeciesViewModel.Count > 0 || ew.PathToFBSpeciesMDB.Length > 0;
                //if(proceed)
                //{
                //    //Global.Settings.PathToFBSpeciesMDB = ew.PathToFBSpeciesMDB;
                //    //Global.SaveGlobalSettings();
                //}
            }
            else
            {
                proceed = true;
            }

            if (proceed)
            {
                ew.Owner = this;
                if ((bool)ew.ShowDialog())
                {
                    SetDataGridSource();
                }
            }

        }

        private void DeleteEntity()
        {

            if ((_nsapEntity == NSAPEntity.FishSpecies || _nsapEntity == NSAPEntity.NonFishSpecies)
                && MessageBox.Show("Are you sure you want to delete this species?\r\nThe species you delete is being used in the ODK collect app.",
                Global.MessageBoxCaption, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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
                                            message = "Selected landing site cannot be deleted because it is used in a fishing ground in a region\r\n" +
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
                    MessageBox.Show(message, Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
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
            MessageBox.Show($"Updated {ItemsRefreshed} species!", Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
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
                    MessageBox.Show($"{entityExported} exported to Excel", Global.MessageBoxCaption);
                }
                else
                {
                    if (ExportExcel.ErrorMessage.Length > 0)
                    {
                        MessageBox.Show(ExportExcel.ErrorMessage, Global.MessageBoxCaption);
                    }
                    else
                    {
                        MessageBox.Show($"An error occurred when exporting {entityExported} to Excel\r\n" +
                                        "Please report this error", Global.MessageBoxCaption);
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
                case "buttonNote":

                    break;
                case "buttonFix":
                    ShowFixMismatchCalendarWindow();

                    //FixCalendarVesselUnloadWindow fcw = FixCalendarVesselUnloadWindow.GetInstance();
                    //fcw.FishingCalendarViewModel = _fishingCalendarViewModel;
                    //fcw.Owner = this;
                    //if (fcw.Visibility == Visibility.Visible)
                    //{
                    //    fcw.BringIntoView();
                    //}
                    //else
                    //{
                    //    fcw.Show();
                    //}
                    break;
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

        //public void SetupCalendar(CalendarViewType calendarView)
        public void SetupCalendarLabels()
        {
            //_calendarOption = _allSamplingEntitiesEventHandler.CalendarView;
            string monthOfSampling = ((DateTime)_allSamplingEntitiesEventHandler.MonthSampled).ToString("MMM-yyyy");
            switch (_calendarOption)
            {
                case CalendarViewType.calendarViewTypeSampledLandings:
                    MonthLabel.Content = $"Calendar of sampled/monitored landings per gear for {monthOfSampling}";
                    break;
                case CalendarViewType.calendarViewTypeCountAllLandingsByGear:
                    MonthLabel.Content = $"Calendar of number of boats landing per gear for {monthOfSampling}";
                    break;
                case CalendarViewType.calendarViewTypeWeightAllLandingsByGear:
                    MonthLabel.Content = $"Calendar of weight of catch per gear for {monthOfSampling}";
                    break;
                case CalendarViewType.calendarViewTypeCountAllLandings:
                    MonthLabel.Content = $"Calendar of total number of landings per day for {monthOfSampling}";
                    break;
                case CalendarViewType.calendarViewTypeGearDailyLandings:
                    MonthLabel.Content = $"Calendar of daily landings per gear per day for {monthOfSampling}";
                    break;
                case CalendarViewType.calendarViewTypeWeightAllLandings:
                    MonthLabel.Content = $"Calendar of total weight of landings per day for {monthOfSampling}";
                    break;
                case CalendarViewType.calendarViewTypeWatchedSpeciesLandings:
                    MonthLabel.Content = $"Calendar of number of landings of watched species per day for {monthOfSampling}";
                    break;
                case CalendarViewType.calendarViewTypeWatchedSpeciesLandedWeight:
                    MonthLabel.Content = $"Calendar of landed weight of watched species per day for {monthOfSampling}";
                    break;
                case CalendarViewType.calendarViewTypeLengthFrequencyMeasurement:
                    MonthLabel.Content = $"Calendar of number of length frequency measurements of watched species per day for {monthOfSampling}";
                    break;
                case CalendarViewType.calendarViewTypeLengthMeasurement:
                    MonthLabel.Content = $"Calendar of number of length measurements of watched species per day for {monthOfSampling}";
                    break;
                case CalendarViewType.calendarViewTypeLengthWeightMeasurement:
                    MonthLabel.Content = $"Calendar of number of length weight measurements of watched species per day for {monthOfSampling}";
                    break;
                case CalendarViewType.calendarViewTypeMaturityMeasurement:
                    if (_getFemaleMaturity)
                    {
                        MonthLabel.Content = $"Calendar of number of maturity measurements for females (length, weight, sex, maturity stage) of watched species per day for {monthOfSampling}";
                    }
                    else
                    {
                        MonthLabel.Content = $"Calendar of number of maturity measurements (length, weight, sex, maturity stage) of watched species per day for {monthOfSampling}";
                    }
                    break;
            }
            labelRowCount.Visibility = Visibility.Visible;
            MonthSubLabel.Visibility = Visibility.Visible;
            if (Global.Settings.UseAlternateCalendar)
            {
                labelRowCount.Content = NSAPEntities.CalendarMonthViewModel.SamplingCalendarTitle;
                MonthSubLabel.Content = NSAPEntities.CalendarMonthViewModel.LocationLabel;
            }
            else
            {
                labelRowCount.Content = NSAPEntities.FishingCalendarDayExViewModel.SamplingCalendarTitle;
                MonthSubLabel.Content = NSAPEntities.FishingCalendarDayExViewModel.LocationLabel;
            }

        }
        public async Task SetupCalendar()
        {
            ShowStatusRow();
            if (_allSamplingEntitiesEventHandler == null)
            {
                return;
            }
            else
            {
                //_allSamplingEntitiesEventHandler.CalendarView = calendarView;
                _allSamplingEntitiesEventHandler.CalendarView = _calendarOption;
                MonthSubLabel.Content = $"{_allSamplingEntitiesEventHandler.LandingSiteText}, {_allSamplingEntitiesEventHandler.FishingGround}, {_allSamplingEntitiesEventHandler.FMA}, {_allSamplingEntitiesEventHandler.NSAPRegion}";
                GridNSAPData.Visibility = Visibility.Visible;
                GridNSAPData.SelectionUnit = DataGridSelectionUnit.Cell;
                PropertyGrid.Visibility = Visibility.Collapsed;
                NSAPEntities.NSAPRegion = _allSamplingEntitiesEventHandler.NSAPRegion;

                //await MakeCalendar(_allSamplingEntitiesEventHandler);


                //_allSamplingEntitiesEventHandler.CalendarView = calendarView;
                //switch (calendarView)
                _calendarOption = _allSamplingEntitiesEventHandler.CalendarView;
                switch (_calendarOption)
                {
                    case CalendarViewType.calendarViewTypeSampledLandings:
                        MonthLabel.Content = $"Calendar of sampled/monitored landings per gear for {((DateTime)_allSamplingEntitiesEventHandler.MonthSampled).ToString("MMMM-yyyy")}";
                        break;
                    case CalendarViewType.calendarViewTypeCountAllLandingsByGear:
                        MonthLabel.Content = $"Calendar of number of boats landing per gear for {((DateTime)_allSamplingEntitiesEventHandler.MonthSampled).ToString("MMMM-yyyy")}";
                        break;
                    case CalendarViewType.calendarViewTypeWeightAllLandingsByGear:
                        MonthLabel.Content = $"Calendar of weight of catch per gear for {((DateTime)_allSamplingEntitiesEventHandler.MonthSampled).ToString("MMMM-yyyy")}";
                        break;
                    case CalendarViewType.calendarViewTypeCountAllLandings:
                        MonthLabel.Content = $"Calendar of total number of landings per day for {((DateTime)_allSamplingEntitiesEventHandler.MonthSampled).ToString("MMMM-yyyy")}";
                        break;
                    case CalendarViewType.calendarViewTypeGearDailyLandings:
                        MonthLabel.Content = $"Calendar of daily landings per gear per day for {((DateTime)_allSamplingEntitiesEventHandler.MonthSampled).ToString("MMMM-yyyy")}";
                        break;
                }

                //MonthSubLabel.Content = $"{_allSamplingEntitiesEventHandler.LandingSiteText}, {_allSamplingEntitiesEventHandler.FishingGround}, {_allSamplingEntitiesEventHandler.FMA}, {_allSamplingEntitiesEventHandler.NSAPRegion}";
                //GridNSAPData.Visibility = Visibility.Visible;
                //GridNSAPData.SelectionUnit = DataGridSelectionUnit.Cell;
                //PropertyGrid.Visibility = Visibility.Collapsed;
                //NSAPEntities.NSAPRegion = _allSamplingEntitiesEventHandler.NSAPRegion;
                //MakeCalendar(_allSamplingEntitiesEventHandler);

                string totlaLandingsCount = "";
                if (_allSamplingEntitiesEventHandler.CalendarView != CalendarViewType.calendarViewTypeGearDailyLandings)
                {
                    int totalLandingsCount = NSAPEntities.FishingCalendarDayExViewModel.TotalVesselUnloadCount;
                    totlaLandingsCount = $", Total sampled landings: {totalLandingsCount}";
                    int vuCountInGU = 0;
                    //foreach (var gu in _fishingCalendarViewModel.UnloadList)
                    //{
                    //    vuCountInGU += gu.ListVesselUnload.Count;
                    //}
                    //if (vuCountInGU != totalLandingsCount)
                    //{
                    //    totlaLandingsCount += $" ({vuCountInGU})";
                    //    buttonFix.Visibility = Visibility.Visible;
                    //}
                }
                if (GridNSAPData.Items.Count > 0)
                {
                    string columnStyle = " (Blue columns represent rest day)";
                    if (!_hasNonSamplingDayColumns)
                    {
                        columnStyle = "";
                    }
                    //labelRowCount.Content = $"Rows: {GridNSAPData.Items.Count}{mainStatusBar.Value}{columnStyle}";
                    //labelRowCount.Content = $"Rows: {GridNSAPData.Items.Count} {totlaLandingsCount} {columnStyle}";
                    labelRowCount.Content = NSAPEntities.FishingCalendarDayExViewModel.SamplingCalendarTitle;
                }
                else if (!NSAPEntities.FishingCalendarDayExViewModel.CalendarHasValue)
                {
                    labelRowCount.Content = "Older eforms cannot encode data. Use Catch and Effort eForm version 7.14";
                }
            }

        }
        public void ShowDBSummary()
        {
            menuDummy.IsChecked = true;
            menuDatabaseSummary.IsChecked = true;
        }

        private void UnCheckCalendarMenuItems()
        {
            foreach (Control mi in menuCalendar.Items)
            {
                if (mi.GetType().Name == "MenuItem")
                {
                    ((MenuItem)mi).IsChecked = false;
                }
            }
        }

        private async void OnMenuItemChecked(object sender, RoutedEventArgs e)
        {
            _getFemaleMaturity = false;
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
            string menuName = ((MenuItem)sender).Name;
            switch (menuName)
            {
                case "menuDatabaseSummary":
                    _nsapEntity = NSAPEntity.DBSummary;
                    if (_selectedTreeNode != null)
                    {
                        _selectedTreeNode.IsSelected = false;
                    }
                    buttonExport.Visibility = Visibility.Collapsed;
                    menuCalendar.Visibility = Visibility.Collapsed;
                    //textOfTitle = "Database summary";
                    break;
                case "menuGPS":
                    _nsapEntity = NSAPEntity.GPS;
                    buttonImport.Visibility = Visibility.Visible;
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

                case "menuSampledCalendar":
                case "menuAllLandingsCalendar":
                case "menuWeightLandingsCalendar":
                case "menuTotalLandingsCalendar":
                case "menuDailyGearLandingCalendar":
                case "menuTotalWeightsCalendar":
                case "menuWatchedSpeciesLandingCalendar":
                case "menuWeightWatchedSpeciesLandingCalendar":
                case "menuNumberLenMeas":
                case "menuNumberLenWtMeas":
                case "menuNumberLenFreqMeas":
                case "menuNumberMaturityMeas":
                case "menuNumberFemaleMaturityMeas":
                    if (NSAPEntities.FishingCalendarDayExViewModel.CanCreateCalendar || Global.Settings.UseAlternateCalendar)
                    {
                        foreach (Control mi in menuCalendar.Items)
                        {
                            var s = mi.GetType().Name;
                            if (mi.GetType().Name != "Separator" && ((MenuItem)mi).IsCheckable && mi.Name != menuName)
                            {
                                ((MenuItem)mi).IsChecked = false;
                            }
                        }
                        _isWatchedSpeciesCalendar = false;
                        _isMeasuredWatchedSpeciesCalendar = false;
                        _species_measurement_type = "";

                        switch (menuName)
                        {
                            case "menuSampledCalendar":
                                _calendarOption = CalendarViewType.calendarViewTypeSampledLandings;
                                break;

                            case "menuAllLandingsCalendar":
                                _calendarOption = CalendarViewType.calendarViewTypeCountAllLandingsByGear;
                                break;
                            case "menuWeightLandingsCalendar":
                                _calendarOption = CalendarViewType.calendarViewTypeWeightAllLandingsByGear;
                                break;
                            case "menuTotalLandingsCalendar":
                                _calendarOption = CalendarViewType.calendarViewTypeCountAllLandings;
                                break;
                            case "menuDailyGearLandingCalendar":
                                _calendarOption = CalendarViewType.calendarViewTypeGearDailyLandings;
                                break;
                            case "menuTotalWeightsCalendar":
                                _calendarOption = CalendarViewType.calendarViewTypeWeightAllLandings;
                                break;
                            case "menuWeightWatchedSpeciesLandingCalendar":
                                _calendarOption = CalendarViewType.calendarViewTypeWatchedSpeciesLandedWeight;
                                _isWatchedSpeciesCalendar = true;
                                break;
                            case "menuWatchedSpeciesLandingCalendar":
                                _calendarOption = CalendarViewType.calendarViewTypeWatchedSpeciesLandings;
                                _isWatchedSpeciesCalendar = true;
                                break;
                            case "menuNumberLenMeas":
                                _calendarOption = CalendarViewType.calendarViewTypeLengthMeasurement;
                                _isWatchedSpeciesCalendar = true;
                                _isMeasuredWatchedSpeciesCalendar = true;
                                _species_measurement_type = "length";
                                break;
                            case "menuNumberLenWtMeas":
                                _calendarOption = CalendarViewType.calendarViewTypeLengthWeightMeasurement;
                                _isWatchedSpeciesCalendar = true;
                                _isMeasuredWatchedSpeciesCalendar = true;
                                _species_measurement_type = "length-weight";
                                break;
                            case "menuNumberLenFreqMeas":
                                _calendarOption = CalendarViewType.calendarViewTypeLengthFrequencyMeasurement;
                                _isWatchedSpeciesCalendar = true;
                                _isMeasuredWatchedSpeciesCalendar = true;
                                _species_measurement_type = "length frequency";
                                break;
                            case "menuNumberMaturityMeas":
                                _calendarOption = CalendarViewType.calendarViewTypeMaturityMeasurement;
                                _isWatchedSpeciesCalendar = true;
                                _isMeasuredWatchedSpeciesCalendar = true;
                                _species_measurement_type = "maturity";
                                break;
                            case "menuNumberFemaleMaturityMeas":
                                _calendarOption = CalendarViewType.calendarViewTypeMaturityMeasurement;
                                _isWatchedSpeciesCalendar = true;
                                _isMeasuredWatchedSpeciesCalendar = true;
                                _getFemaleMaturity = true;
                                _species_measurement_type = "maturity";
                                break;
                        }

                        bool proceed = false;
                        if (_isWatchedSpeciesCalendar)
                        {
                            var reg = NSAPEntities.NSAPRegionViewModel.CurrentEntity;
                            if (reg.RegionWatchedSpeciesViewModel == null)
                            {
                                reg.RegionWatchedSpeciesViewModel = new RegionWatchedSpeciesViewModel(reg);
                            }
                            proceed = NSAPEntities.NSAPRegionViewModel.CurrentEntity.RegionWatchedSpeciesViewModel.Count > 0;
                        }
                        else
                        {
                            proceed = true;
                        }

                        if (proceed && !_cancelBuildCalendar)
                        {

                            if (Global.Settings.UseAlternateCalendar)
                            {

                                NSAPEntities.CalendarMonthViewModel = new CalendarMonthViewModel(
                                    e: _allSamplingEntitiesEventHandler,
                                    viewOption: _calendarOption,
                                    isWatchedSpeciesCalendar: _isWatchedSpeciesCalendar,
                                    measurementType: _species_measurement_type,
                                    isFemaleMaturity: _getFemaleMaturity
                                    );

                                ShowStatusRow();
                                NSAPEntities.CalendarMonthViewModel.CalendarMonthRepository.GettingCalendar += CalendarMonthRepository_GettingCalendar;
                                await NSAPEntities.CalendarMonthViewModel.GetCalendars();
                                NSAPEntities.CalendarMonthViewModel.CalendarMonthRepository.GettingCalendar -= CalendarMonthRepository_GettingCalendar;



                                GridNSAPData.SelectionUnit = DataGridSelectionUnit.Cell;
                                GridNSAPData.Columns.Clear();
                                GridNSAPData.AutoGenerateColumns = true;
                                GridNSAPData.DataContext = NSAPEntities.CalendarMonthViewModel.SamplingCalendarDataTable;
                                SetupCalendarLabels();


                                if (NSAPEntities.CalendarMonthViewModel.SamplingRestDays.Count > 0)
                                {
                                    foreach (DataGridColumn c in GridNSAPData.Columns)
                                    {
                                        if (int.TryParse(c.Header.ToString(), out int v))
                                        {
                                            if (NSAPEntities.CalendarMonthViewModel.SamplingRestDays.Contains(v))
                                            {
                                                _hasNonSamplingDayColumns = true;
                                                c.CellStyle = new Style(typeof(DataGridCell));
                                                c.CellStyle.Setters.Add(new Setter(DataGridCell.BackgroundProperty, new SolidColorBrush(Colors.LightBlue)));
                                            }
                                        }
                                    }
                                }
                                ShowStatusRow(isVisible: false);
                                if (!NSAPEntities.CalendarMonthViewModel.CalendarHasData)
                                {
                                    TimedMessageBox.Show(
                                        "There is no data for the calendar",
                                        Global.MessageBoxCaption,
                                        2500,
                                        System.Windows.Forms.MessageBoxButtons.OK);
                                }
                            }
                            else
                            {
                                NSAPEntities.FishingCalendarDayExViewModel.CalendarViewType = _calendarOption;
                                if (await NSAPEntities.FishingCalendarDayExViewModel.MakeCalendar(isWatchedSpeciesCalendar: _isWatchedSpeciesCalendar, getFemaleMaturity: _getFemaleMaturity))
                                {


                                    GridNSAPData.Columns.Clear();
                                    GridNSAPData.AutoGenerateColumns = true;
                                    GridNSAPData.DataContext = NSAPEntities.FishingCalendarDayExViewModel.DataTable;
                                    if (!NSAPEntities.FishingCalendarDayExViewModel.CalendarHasValue)
                                    {
                                        GridNSAPData.Visibility = Visibility.Collapsed;
                                    }
                                    else
                                    {
                                        GridNSAPData.Visibility = Visibility.Visible;
                                    }

                                    _hasNonSamplingDayColumns = false;
                                    foreach (DataGridColumn c in GridNSAPData.Columns)
                                    {
                                        if (int.TryParse(c.Header.ToString(), out int v))
                                        {
                                            if (NSAPEntities.FishingCalendarDayExViewModel.DayIsRestDay(v))
                                            {
                                                _hasNonSamplingDayColumns = true;
                                                c.CellStyle = new Style(typeof(DataGridCell));
                                                c.CellStyle.Setters.Add(new Setter(DataGridCell.BackgroundProperty, new SolidColorBrush(Colors.LightBlue)));
                                            }
                                        }
                                    }
                                    SetupCalendarLabels();
                                    if (!NSAPEntities.FishingCalendarDayExViewModel.CalendarHasValue)
                                    {

                                        TimedMessageBox.Show("No data for buidling calendar",
                                            Global.MessageBoxCaption,
                                            timeout: 1500,
                                            System.Windows.Forms.MessageBoxButtons.OK

                                            );
                                    }
                                }
                            }

                        }
                        else if (_isWatchedSpeciesCalendar && !proceed)
                        {
                            MessageBox.Show("There are no watched species for the selected NSAP Region",
                                Global.MessageBoxCaption,
                                MessageBoxButton.OK,
                                MessageBoxImage.Information
                                );
                        }
                    }
                    return;
                    //break;
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
            SetupCalendarView("");
            labelTitle.Visibility = Visibility.Visible;
            labelTitle.Content = textOfTitle;
            buttonDelete.IsEnabled = false;
            buttonEdit.IsEnabled = false;

        }

        private void CalendarMonthRepository_GettingCalendar(object sender, GettingCalendarEventArgs e)
        {

            switch (e.Context)
            {
                case "fetching":
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
                              mainStatusLabel.Content = "Getting calendar data";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break; ;
                case "fetching done":
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              mainStatusBar.IsIndeterminate = false;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = "Finished getting calendar data";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
            }
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

        private void ShowCrossTabWIndow(bool isCarrierBoatLanding = false)
        {
            CrossTabReportWindow ctw = CrossTabReportWindow.GetInstance(isCarrierBoatLanding);
            if (ctw.IsVisible)
            {
                ctw.BringIntoView();
            }
            else
            {
                ctw.Show();
                ctw.Owner = this;
            }
            //if (isCarrierBoatLanding)
            //{

            //}
            //else
            //{
            ctw.ShowEffort();
            //}
        }
        private async void OnDataGridContextMenu(object sender, RoutedEventArgs e)
        {

            switch (((MenuItem)sender).Tag.ToString())
            {
                case "samplingCalendar":
                    _allSamplingEntitiesEventHandler.GearUsed = _gearName;
                    _allSamplingEntitiesEventHandler.ContextMenuTopic = "contextMenuCrosstabGear";

                    ShowStatusRow();
                    CrossTabManager.IsCarrierLandding = false;
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

                MessageBox.Show(exportResult, Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public bool OpenRefreshedDatabase()
        {
            bool success = false;
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
            }
            return success;
        }
        public bool OpenDatabaseInApp(string filename, out string backendPath)
        {
            backendPath = "";
            bool success = false;
            Global.SetMDBPath(filename);
            if (Global.AppProceed)
            {
                SetDataDisplayMode();


                //show splash screen and load entities
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
                    backendPath = filename;

                }
                else
                {
                    ShowDatabaseNotFoundView();
                }
            }
            return success;
        }

        public bool LocateBackendDB(out string backendPath)
        {
            bool success = false;
            backendPath = "";
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = false;
            ofd.Title = "Locate backend database for NSAP data";
            ofd.Filter = "MDB file(*.mdb)|*.mdb|All file types (*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if ((bool)ofd.ShowDialog())
            {

                bool proceed = File.Exists(ofd.FileName);

                if (!proceed && MessageBox.Show(
                    $"{ofd.FileName} does not exist\r\n\r\nWould you like to create a new one?",
                    Global.MessageBoxCaption,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    File.Copy($@"{AppDomain.CurrentDomain.BaseDirectory}\nsap_odk.dat", ofd.FileName);
                    proceed = true;
                }


                if (proceed)
                {
                    success = OpenDatabaseInApp(ofd.FileName, out backendPath);
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
                        MessageBox.Show("Successfully exported maturity data to Excel", Global.MessageBoxCaption);
                    }
                    else
                    {
                        MessageBox.Show(ExportExcel.ErrorMessage, Global.MessageBoxCaption);
                    }
                }
                else
                {
                    MessageBox.Show("Export was cancelled", Global.MessageBoxCaption);
                }
            }
            else
            {
                MessageBox.Show("Selected region and fishing ground does not contain maturity data", Global.MessageBoxCaption);
            }
        }
        private void ShowFixMismatchCalendarWindow()
        {

            ProgressDialogWindow pdw = ProgressDialogWindow.GetInstance("fix mismatch in calendar days");
            pdw.Owner = this;
            if (pdw.Visibility == Visibility.Visible)
            {
                pdw.BringIntoView();
            }
            else
            {
                pdw.Show();
            }
        }

        private void ShowIdentifyCatchCompWithZeroWt()
        {
            ProgressDialogWindow pdw = ProgressDialogWindow.GetInstance("identify zero weight catch composition");
            pdw.Owner = this;
            if (pdw.Visibility == Visibility.Visible)
            {
                pdw.BringIntoView();
            }
            else
            {
                pdw.Show();
            }
        }

        private async void OnMenuClicked(object sender, RoutedEventArgs e)
        {
            //string fileName = "";
            MenuItem mi = (MenuItem)sender;
            string itemName = mi.Name;

            if (itemName == "menuDownloadHistory" || itemName == "menuNSAPCalendar")
            {
                UncheckEditMenuItems();
            }

            if (!itemName.Contains("menuWeightValidationTally"))
            {
                CloseTallyWindow();
            }
            ProgressDialogWindow pdw = null;
            switch (itemName)
            {

                case "menuSpeciesForInternet":
                    Process.Start(((MenuItem)sender).Tag.ToString());
                    break;
                case "menuCrossTabCBL_Month":
                    CrossTabManager.IsCarrierLandding = true;
                    await CrossTabManager.CarrierBoatLandingsByMonthAsync(_landingSite, (DateTime)_monthYear);
                    ShowCrossTabWIndow(isCarrierBoatLanding: true);
                    break;
                case "menuListSamplingAndCatchComposition":
                    if (_monthYear != null)
                    {
                        List<LandingSiteSamplingSummarized> list = NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteSamplingSummaries(ls: _treeItemData.LandingSite, (DateTime)_monthYear);
                        LandingSiteSamplingSummariesWindow lssw = new LandingSiteSamplingSummariesWindow(list);
                        lssw.Owner = this;
                        lssw.ShowDialog();
                    }
                    break;
                //Tree context menu ->Databases->Select server for server
                //database summary view tree view
                case "menuSelectServerForFilter":
                    var server = (Koboserver)dataGridEFormVersionStats.SelectedItem;
                    //Global.Settings.ServerFilter = server.ServerID;
                    Global.SaveGlobalSettings();
                    MessageBox.Show("Restart NSAP-ODK Database so that the filter will take effect", Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                    break;

                //NSAP data->Delete fish landing data
                case "menuDeleteLandingData":
                    DeleteOptionWindow dow = new DeleteOptionWindow();
                    if ((bool)dow.ShowDialog())
                    {
                        string action = "";
                        switch (dow.DeleteChoice)
                        {
                            case DeleteChoice.deleteChoiceMultiVesselGear:
                                action = "delete multivessel gear data";
                                break;
                            case DeleteChoice.deleteChoiceSingleVesselGear:
                                action = "delete single vessel data";
                                break;
                            case DeleteChoice.deleteChoiceBoth:
                                action = "delete all landing data";
                                break;
                        }
                        pdw = new ProgressDialogWindow(action);
                        if ((bool)pdw.ShowDialog())
                        {
                            OpenRefreshedDatabase();
                            Button b = new Button { Name = "buttonSummary" };
                            OnToolbarButtonClick(b, null);
                        }
                    }
                    break;
                case "menuCalendarRefresh":
                    break;

                //NSAP data -> Identify catch composition with zero weight
                case "menuIdentifyZeroWtCatchComp":
                    ShowIdentifyCatchCompWithZeroWt();
                    break;

                //NSAP data->Identify sampling calendar mismatch
                case "menuCalendarDayMismatch":
                    ShowFixMismatchCalendarWindow();
                    break;

                //Datagrid context menu->Regions->Fishing grounds->Move to fishing ground
                //data grid of landing sites beloning to a region
                case "menuMoveToFishingGround":

                    var nrf = (NSAPRegionFMAFishingGround)((TreeViewItem)treeViewSummary.SelectedItem).Tag;
                    MoveLandingSitesToFishingGroundWindow mlsw = new MoveLandingSitesToFishingGroundWindow();
                    mlsw.NSAPRegionFMAFishingGround = nrf;
                    mlsw.Owner = this;
                    if ((bool)mlsw.ShowDialog())
                    {
                        _fishingGroundMoveDestination = mlsw.FishingGround;
                        ShowStatusRow();
                        VesselUnloadRepository.ChangeFishingGroundOFUnloadEvent += VesselUnloadRepository_ChangeFishingGroundOFUnloadEvent;

                        List<DBSummary> sumarries = new List<DBSummary>();

                        foreach (SummaryResults item in dataGridSummary.SelectedItems)
                        {
                            sumarries.Add(item.DBSummary);
                        }
                        int result = await VesselUnloadRepository.SetFishingGroundsOfVesselUnloadsAsync(sumarries, _fishingGroundMoveDestination);
                        VesselUnloadRepository.ChangeFishingGroundOFUnloadEvent -= VesselUnloadRepository_ChangeFishingGroundOFUnloadEvent;
                        dataGridSummary.DataContext = await NSAPEntities.SummaryItemViewModel.GetRegionFishingGroundSummaryAsync(nrf.RegionFMA.NSAPRegion, nrf.FishingGround, nrf.RegionFMA.FMA);
                    }

                    break;

                case "menuCalendarGearMapping":
                case "menuCalendarDayGearMapping":
                case "menuCalendarDayFemaleMaturityMapping":
                case "menuCalendarGearSpeciesMapping":
                case "menuCalendarSpeciesMapping":
                case "menuCalendarDaySpeciesGearMapping":
                    string calendarDayFailMessage = "";

                    Mapping.MapWindowManager.OpenMapWindow(this);
                    Mapping.FishingGroundPointsFromCalendarMappingManager.MapInteractionHandler = Mapping.MapWindowManager.MapInterActionHandler;
                    Mapping.FishingGroundPointsFromCalendarMappingManager.EntitiesMonth = _allSamplingEntitiesEventHandler;
                    Mapping.FishingGroundPointsFromCalendarMappingManager.GearName = _gearName;
                    Mapping.FishingGroundPointsFromCalendarMappingManager.Sector = _fish_sector;
                    Mapping.FishingGroundPointsFromCalendarMappingManager.CalendarDay = _calendarDay;
                    Mapping.FishingGroundPointsFromCalendarMappingManager.SpeciesName = _speciesName;
                    Mapping.FishingGroundPointsFromCalendarMappingManager.MaturityStage = _maturityStage;
                    Mapping.FishingGroundPointsFromCalendarMappingManager.MappingContext = itemName;
                    Mapping.FishingGroundPointsFromCalendarMappingManager.Description = mi.Header.ToString();
                    if (mi.Tag != null && !string.IsNullOrEmpty(mi.Tag.ToString()))
                    {
                        Mapping.FishingGroundPointsFromCalendarMappingManager.MappingContext2 = mi.Tag.ToString();
                    }
                    Mapping.FishingGroundPointsFromCalendarMappingManager.SetSamplingDate(_calendarDay);
                    int? sp_id = null;

                    if (_speciesTaxa != null)
                    {
                        var tx = NSAPEntities.TaxaViewModel.TaxaCodeFromName(_speciesTaxa);

                        if (tx == "FIS")
                        {
                            sp_id = NSAPEntities.FishSpeciesViewModel.GetSpecies(_speciesName).SpeciesCode;
                        }
                        else
                        {
                            sp_id = NSAPEntities.NotFishSpeciesViewModel.GetSpecies(_speciesName).SpeciesID;
                        }
                    }

                    List<int> vu_ids = new List<int>();
                    switch (itemName)
                    {
                        case "menuCalendarDaySpeciesGearMapping":
                            if (!string.IsNullOrEmpty(_speciesTaxa))
                            {
                                string taxa_code = NSAPEntities.TaxaViewModel.TaxaCodeFromName(_speciesTaxa);
                                sp_id = null;
                                if (taxa_code == "FIS")
                                {
                                    sp_id = NSAPEntities.FishSpeciesViewModel.GetSpecies(_speciesName).SpeciesCode;
                                }
                                else
                                {
                                    sp_id = NSAPEntities.NotFishSpeciesViewModel.GetSpecies(_speciesName).SpeciesID;
                                }
                            }
                            vu_ids = VesselCatchRepository.GetVesselUnloadIDs(_treeItemData, mi.Tag.ToString(), (int)_calendarDay, sp_id, _gearCode, _sector_code);
                            if (await Mapping.FishingGroundPointsFromCalendarMappingManager.MapFishingGroundPoint(vesselUnloadIDs: vu_ids) == false)
                            {
                                calendarDayFailMessage = "Sampled landings do not contain fishing ground data";
                            }
                            break;
                        case "menuCalendarSpeciesMapping":
                            vu_ids = VesselCatchRepository.GetVesselUnloadIDs(_treeItemData, (int)sp_id, context: mi.Tag.ToString());
                            if (await Mapping.FishingGroundPointsFromCalendarMappingManager.MapFishingGroundPoint(vesselUnloadIDs: vu_ids) == false)
                            {
                                calendarDayFailMessage = "Sampled landings do not contain fishing ground data";
                            }
                            break;
                        case "menuCalendarGearSpeciesMapping":
                            vu_ids = VesselCatchRepository.GetVesselUnloadIDs(_treeItemData,
                                sp_id, context: mi.Tag == null ? "" : mi.Tag.ToString(), _gearCode, _sector_code
                                );
                            if (await Mapping.FishingGroundPointsFromCalendarMappingManager.MapFishingGroundPoint(vesselUnloadIDs: vu_ids) == false)
                            {
                                calendarDayFailMessage = "Sampled landings do not contain fishing ground data";
                            }
                            break;
                        case "menuCalendarDayFemaleMaturityMapping":
                            if (sp_id != null)
                            {
                                vu_ids = CatchMaturityRepository.GetVesselUnloadIDsForFemaleCatchMaturityStage(_treeItemData,
                                                            _gearUnloads.First().Parent.SamplingDate.Day,
                                                            _gearUnloads.First().GearID,
                                                            _sector_code,
                                                            CatchMaturity.CodeFromMaturityStage(_maturityStage),
                                                            (int)sp_id
                                                            );
                                if (await Mapping.FishingGroundPointsFromCalendarMappingManager.MapFishingGroundPoint(vesselUnloadIDs: vu_ids) == false)
                                {
                                    calendarDayFailMessage = "Sampled landings do not contain fishing ground data";
                                }

                            }
                            break;
                        case "menuCalendarGearMapping":
                            if (await Mapping.FishingGroundPointsFromCalendarMappingManager.MapFishingGroundPoint(_gearName, _fish_sector) == false)
                            {
                                calendarDayFailMessage = "Sampled landings do not contain fishing ground data";
                            }
                            break;

                        case "menuCalendarDayGearMapping":

                            if (await Mapping.FishingGroundPointsFromCalendarMappingManager.MapFishingGroundPoint(_gearName, _fish_sector, _calendarDay) == false)
                            {
                                calendarDayFailMessage = "Sampled landings do not contain fishing ground data";
                            }
                            break;
                    }

                    if (!string.IsNullOrEmpty(calendarDayFailMessage))
                    {
                        MessageBox.Show(calendarDayFailMessage,
                            Global.MessageBoxCaption,
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                            );
                    }
                    break;
                case "menuWeightValidationTally":
                case "menuWeightValidationTally_context":
                    _wvtw = WeightValidationTallyWindow.GetInstance((List<SummaryItem>)GridNSAPData.DataContext);
                    _wvtw.Owner = this;
                    _wvtw.DataGrid = GridNSAPData;
                    if (_wvtw.Visibility == Visibility.Visible)
                    {
                        _wvtw.BringIntoView();
                    }
                    else
                    {
                        _wvtw.Show();
                    }
                    break;

                //NSAP data ->Update weight validation summary
                case "menuUpdateWeightValidation":
                    UpdateWeightValidationTableWindow uwvw = UpdateWeightValidationTableWindow.GetInstance();
                    uwvw.Owner = this;
                    if (uwvw.Visibility == Visibility.Visible)
                    {
                        uwvw.BringIntoView();
                    }
                    else
                    {
                        uwvw.Show();
                    }
                    break;

                //NSAP data ->Download CSV from server
                case "menuDownloadCSV":
                    if (DownloadCSVFromServer())
                    {

                    }
                    break;
                //Tree context menu ->Databases->Remove landing data downloaded from server
                //database summary view tree view
                case "menuDeleteLandingDataFromServer":
                    //MessageBoxResult rs = MessageBoxResult.No;
                    //if (_selectedKoboserver.SavedInDBCount > 0)
                    //{
                    //    rs = MessageBox.Show(
                    //        "Delete fish landing data of selected server?",
                    //        Global.MessageBoxCaption,
                    //        MessageBoxButton.YesNo,
                    //        MessageBoxImage.Question);
                    //}
                    //if (rs == MessageBoxResult.Yes)
                    //{
                    pdw = new ProgressDialogWindow("delete landing data from selected server");
                    pdw.ServerID = _selectedKoboserver.ServerID;
                    pdw.ServerIsMultiVessel = _selectedKoboserver.IsFishLandingMultiVesselSurveyForm;
                    if ((bool)pdw.ShowDialog())
                    {
                        OpenRefreshedDatabase();
                        Button b = new Button { Name = "buttonSummary" };
                        OnToolbarButtonClick(b, null);
                    }
                    //}
                    break;

                //Tree context menu ->Databases->Remove server
                //database summary view tree view
                case "menuRemoveKoboserver":

                    if (NSAPEntities.KoboServerViewModel.DeleteRecordFromRepo(_selectedKoboserver.ServerNumericID))
                    {
                        dataGridEFormVersionStats.DataContext = NSAPEntities.KoboServerViewModel.KoboserverCollection.ToList();
                    }
                    break;

                //Tree context menu ->Databases->Remove all servers of owner
                //database summary view tree view
                case "menuRemoveAllKoboserversOfOwner":
                    if (NSAPEntities.KoboServerViewModel.RemoveAllKoboserversOfOwner(_selectedKoboserver))
                    {
                        dataGridEFormVersionStats.DataContext = NSAPEntities.KoboServerViewModel.KoboserverCollection.ToList();
                    }
                    break;

                //File->Settings
                case "menuFileSettings1":
                case "menuFileSettings":
                    ShowSettingsWindow();
                    break;

                //NSAP data->Enumerator's first sampling
                case "menuEnumeratorFirstSampling":

                    pdw = new ProgressDialogWindow("get enumerators first sampling");
                    if ((bool)pdw.ShowDialog())
                    {
                        var fsw = FirstSamplingOfEnumeratorsWindow.GetInstance();
                        //fsw.FirstSamplings = NSAPEntities.NSAPEnumeratorViewModel.GetFirstSamplingOfEnumerators();
                        fsw.FirstSamplings = pdw.FirstSamplingsOfEnumerators;
                        if (fsw.Visibility == Visibility.Visible)
                        {
                            fsw.BringIntoView();
                        }
                        else
                        {
                            fsw.Show();
                        }
                    }

                    break;
                //NSAP data->Export->Extract fishing vessels by region
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

                case "menuRegionLandingSites":
                    RegionLandingSitesWindow rlsw = new RegionLandingSitesWindow();
                    rlsw.NSAPRegion = (NSAPRegion)dataGridEntities.SelectedItem;
                    rlsw.ShowDialog();

                    break;

                //Grid context menu->Weights and weight validation

                case "menuWeights":
                    switch (_calendarTreeSelectedEntity)
                    {
                        case "tv_MonthViewModel":
                        case "tv_LandingSiteViewModel":
                            ShowStatusRow();
                            ((ContextMenu)((MenuItem)e.OriginalSource).Parent).IsOpen = false;
                            //await GearUnloadWindowWithWeightValidation();
                            _treeItemData.MonthSampled = _monthYear;
                            if (_calendarTreeSelectedEntity == "tv_MonthViewModel")
                            {
                                _treeItemData.GearUsed = _gearName;
                            }
                            //var items = await NSAPEntities.SummaryItemViewModel.GetValidateLandedCatchWeightsByCalendarTreeSelectionAsync(_treeItemData);

                            var items = await NSAPEntities.SummaryItemViewModel.GetDownloadDetailsByCalendarTreeSelectionTaskAsync(_treeItemData);
                            GearUnloadWindow guw = new GearUnloadWindow(items, _treeItemData);
                            guw.Owner = this;
                            guw.ShowDialog();

                            break;
                        default:
                            //ignore
                            break;
                    }
                    break;
                //Datagrid context menu->Copy text
                //copy the contents of data grid to the clipboard
                case "menuCopyText":
                    if (_dataGrid != null)
                    {
                        //CopyTextDialogWindow ctdw = CopyTextDialogWindow.GetInstance();
                        //ctdw.DataGridDataContext = _dataGrid.DataContext;
                        //ctdw.DataContextType = typeof(_dataGrid.DataContext);
                        //ctdw.DataGrid = _dataGrid;
                        //if(ctdw.Visibility==Visibility.Visible)
                        //{
                        //    ctdw.BringIntoView();
                        //}
                        //else
                        //{
                        //    ctdw.Show();
                        //    ctdw.Owner = this;
                        //}


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
                                            Global.MessageBoxCaption,
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Information) == MessageBoxResult.Yes)
                        {
                            ShowStatusRow();
                            NSAPEntities.VesselUnloadViewModel.DatabaseUpdatedEvent += VesselUnloadViewModel_DatabaseUpdatedEvent;
                            var r = await NSAPEntities.VesselUnloadViewModel.UpdateUnloadStatsAsync();
                            NSAPEntities.VesselUnloadViewModel.DatabaseUpdatedEvent -= VesselUnloadViewModel_DatabaseUpdatedEvent;
                            ShowStatusRow(isVisible: false);
                            MessageBox.Show("Finished updating the database for responses to \"Is catch composition included\"",
                                            Global.MessageBoxCaption,
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Update table column \"Has catch composition\" before updating landings statistics",
                                        Global.MessageBoxCaption,
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
                                    Global.MessageBoxCaption,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information
                                    );
                    break;
                case "menuBackupMySQL":
                    if (Global.Settings.MySQLBackupFolder.Length > 0)
                    {
                        var backupWindow = backupMySQLWindow.GetInstance();
                        //backupWindow.BackupDBOTablesOnly = true;
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
                                        Global.MessageBoxCaption,
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Information);
                    }
                    break;

                case "menuExportExcelMaturity":
                    ExportNSAPWithMaturityToExcel();
                    break;

                //Help->About this software
                case "menuAbout":
                    AboutWindow aw = new AboutWindow();
                    aw.ShowDialog();
                    break;

                case "menuUploadMedia":
                    break;

                //File->Save->Gears
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

                //File->Open database
                case "menuLocateDatabase":

                    if (LocateBackendDB(out string backendPath))
                    {
                        menuDummy.IsChecked = true;
                        menuDatabaseSummary.IsChecked = true;
                    }
                    break;
                //NSAP data -> Import GPX
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

                //NSAP data->Download history
                case "menuDownloadHistory":
                    menuCalendar.Visibility = Visibility.Collapsed;
                    _currentDisplayMode = DataDisplayMode.DownloadHistory;
                    ColumnForTreeView.Width = new GridLength(1, GridUnitType.Star);
                    SetDataDisplayMode();

                    break;

                //NSAP data->Export->Export tracked landing summaries to Excel
                case "menuExportExcelTracked":
                    //ExportNSAPToExcel(tracked: true);
                    TrackedSummariesForExportWindow tws = new TrackedSummariesForExportWindow();
                    tws.ShowDialog();
                    break;

                //NSAP data->Export->Export to Excel
                case "menuExportExcel":
                    ExportNSAPToExcel();
                    break;

                //NSAP data->Sampling calendar
                case "menuNSAPCalendar":
                    ShowNSAPCalendar();
                    break;

                //NSAP Data->Manage
                case "menuImport":
                    ShowImportWindow();
                    break;

                //Generate csv->OPtions for generating CSV
                case "menuOptionGenerateCSV":
                    CSVOptionsWindow optionsWindow = new CSVOptionsWindow();
                    optionsWindow.ShowDialog();
                    break;

                //File->Exit
                case "menuExit":
                case "menuExit2":
                    Close();
                    break;
                case "menuQueryAPI":
                    QueryAPIWIndow qaw = new QueryAPIWIndow(this);
                    qaw.ShowDialog();
                    break;

                //Generate CSV->Select regions
                case "menuSelectRegions":
                    SelectRegions(resetList: true);
                    break;

                //GEnerate CSV->Generate
                case "menuGenerateAll":
                    await GenerateAllCSV();

                    break;


                case "menuGenerateItemSets":
                    break;


            }

        }

        private void VesselUnloadRepository_ChangeFishingGroundOFUnloadEvent(object sender, SetFishingGroundOfUnloadEventArg e)
        {
            switch (e.Intent)
            {
                case "start":
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              mainStatusBar.IsIndeterminate = false;
                              mainStatusBar.Maximum = e.TotalVesselUnloads;
                              mainStatusBar.Value = 0;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = "Updating fishing ground of selected samplings. Please wait...";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    break;
                case "fg changed":
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              mainStatusBar.Value = e.CountFishingGroundChanged;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = $"Finished updating record {e.CountFishingGroundChanged} of {mainStatusBar.Maximum}";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "finished":

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
                              mainStatusLabel.Content = $"Finished updating records";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    _timer.Interval = TimeSpan.FromSeconds(3);
                    _timer.Start();
                    break;
            }
        }



        private void WeightValidationUpdater_UploadSubmissionToDB(object sender, UploadToDbEventArg e)
        {

            switch (e.Intent)
            {
                case UploadToDBIntent.StartOfUpdate:
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              mainStatusBar.IsIndeterminate = false;
                              mainStatusBar.Maximum = e.VesselUnloadToUpdateCount;
                              mainStatusBar.Value = 0;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = "Updating weight validation summary. Please wait...";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    break;
                case UploadToDBIntent.SummaryItemProcessed:
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              mainStatusBar.Value = e.SummaryItemProcessedCount;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = $"Finished updating record {e.SummaryItemProcessedCount} of {mainStatusBar.Maximum}";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case UploadToDBIntent.WeightValidationUpdated:
                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = $"Updated validation record #{e.VesselUnloadWeightValidationUpdateCount}";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case UploadToDBIntent.EndOfUpdate:
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              mainStatusBar.Value = 0;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    _timer.Interval = TimeSpan.FromSeconds(3);
                    _timer.Start();
                    break;
            }
        }


        //private async Task GearUnloadWindowWithWeightValidation()
        //{
        //    _treeItemData.MonthSampled = _monthYear;
        //    if (_calendarTreeSelectedEntity == "tv_MonthViewModel")
        //    {
        //        _treeItemData.GearUsed = _gearName;
        //    }
        //    var items = await NSAPEntities.SummaryItemViewModel.GetValidateLandedCatchWeightsByCalendarTreeSelectionAsync(_treeItemData);
        //    GearUnloadWindow guw = new GearUnloadWindow(items);
        //    guw.Owner = this;
        //    guw.ShowDialog();
        //}

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

                    MessageBox.Show($"Generated {GenerateCSV.FilesCount} csv files with a total of {result} lines", Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (System.IO.IOException)
                {
                    MessageBox.Show("Cannot complete this operation because some files are open.", Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Exclamation); ;
                }
                catch (Exception ex)
                {
                    Logger.LogType = LogType.Logfile;
                    Logger.Log(ex);
                }
            }
        }
        public string ImportIntoMDBFile { get; set; }
        public bool OpenImportedDatabaseInApplication { get; set; }

        private void ShowNSAPCalendar()
        {
            menuCalendar.Visibility = Visibility.Collapsed;
            _currentDisplayMode = DataDisplayMode.ODKData;
            ColumnForTreeView.Width = new GridLength(2, GridUnitType.Star);
            SetDataDisplayMode();
        }
        private void ShowImportWindow(bool openLogInWindow = false)
        {
            ODKResultsWindow window = new ODKResultsWindow();
            window.Owner = this;
            window.OpenLogInWindow(isOpen: openLogInWindow);
            window.ShowDialog();

            if (OpenImportedDatabaseInApplication)
            {
                OpenImportedDatabaseInApplication = false;
                if (OpenDatabaseInApp(ImportIntoMDBFile, out string dummy))
                {
                    menuDummy.IsChecked = true;
                    menuDatabaseSummary.IsChecked = true;
                }
            }
            //var window = ODKResultsWindow.GetInstance();
            //if (window.IsVisible)
            //{
            //    window.BringIntoView();
            //}
            //else
            //{
            //    window.Show();
            //    window.Owner = this;
            //    window.ParentWindow = this;
            //}

            //window.OpenLogInWindow(isOpen: openLogInWindow);
        }

        private void DownloadFromServerWindow_RefreshDatabaseSummaryTable()
        {
            dataGridEFormVersionStats.DataContext = null;
            dataGridEFormVersionStats.DataContext = NSAPEntities.KoboServerViewModel.KoboserverCollection.ToList();
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
        private async void OnGridDoubleClick(object sender, MouseButtonEventArgs e)
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
                    #region dataGridSummary
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
                #endregion
                case "GridNSAPData":
                case "gridCBL":
                    #region GridNSAPData
                    LandingSiteSampling lss = null;
                    //calendar view
                    if (_currentDisplayMode == DataDisplayMode.CBLSampling)
                    {
                        if (_carrierLandingFromCalendar != null)
                        {
                            CarrierBoatLandingEditor cbe = new CarrierBoatLandingEditor(_carrierLandingFromCalendar);
                            cbe.Owner = this;
                            cbe.ShowDialog();
                        }
                    }
                    else if (_currentDisplayMode == DataDisplayMode.ODKData)
                    {
                        if (_calendarDayHasValue)
                        {
                            switch (_calendarOption)
                            {
                                case CalendarViewType.calendarViewTypeCountAllLandingsByGear:
                                case CalendarViewType.calendarViewTypeWeightAllLandingsByGear:
                                case CalendarViewType.calendarViewTypeSampledLandings:
                                case CalendarViewType.calendarViewTypeWatchedSpeciesLandings:
                                case CalendarViewType.calendarViewTypeWatchedSpeciesLandedWeight:
                                case CalendarViewType.calendarViewTypeMaturityMeasurement:
                                case CalendarViewType.calendarViewTypeLengthFrequencyMeasurement:
                                case CalendarViewType.calendarViewTypeLengthMeasurement:
                                case CalendarViewType.calendarViewTypeLengthWeightMeasurement:
                                    if (_gearUnloads != null && _gearUnloads.Count > 0 && _gearUnloadWindow == null)
                                    {
                                        ShowStatusRow();
                                        GearUnloadViewModel.ProcessingItemsEvent += GearUnloadViewModel_ProcessingItemsEvent;
                                        var v_unloads = await GearUnloadViewModel.GetVesselUnloadsFromGearUnloadsAsync(_gearUnloads);
                                        GearUnloadViewModel.ProcessingItemsEvent += GearUnloadViewModel_ProcessingItemsEvent;
                                        ShowStatusRow(isVisible: false);




                                        _gearUnloadWindow = new GearUnloadWindow(_gearUnloads, _treeItemData, this, _sector_code);
                                        _gearUnloadWindow.CalendarViewType = _calendarOption;


                                        if (_isWatchedSpeciesCalendar)
                                        {
                                            var taxa_code = NSAPEntities.TaxaViewModel.TaxaCodeFromName(_speciesTaxa);
                                            _gearUnloadWindow.SpeciesID = VesselCatchViewModel.SpeciesIDFromSpeciesName(taxa_code, _speciesName);
                                            _gearUnloadWindow.WatchedSpecies = _speciesName;
                                        }
                                        else
                                        {
                                            _gearUnloadWindow.WatchedSpecies = null;
                                        }

                                        if (_calendarOption == CalendarViewType.calendarViewTypeMaturityMeasurement && _getFemaleMaturity)
                                        {
                                            _gearUnloadWindow.SectorCode = _sector_code;
                                            _gearUnloadWindow.MaturityCode = CatchMaturity.CodeFromMaturityStage(_maturityStage);
                                            //var taxa_code = NSAPEntities.TaxaViewModel.TaxaCodeFromName(_speciesTaxa);
                                            //_gearUnloadWindow.SpeciesID = VesselCatchViewModel.SpeciesIDFromSpeciesName(taxa_code, _speciesName);
                                        }
                                        else if (_calendarOption == CalendarViewType.calendarViewTypeLengthMeasurement ||
                                            _calendarOption == CalendarViewType.calendarViewTypeLengthFrequencyMeasurement ||
                                            _calendarOption == CalendarViewType.calendarViewTypeMaturityMeasurement ||
                                            _calendarOption == CalendarViewType.calendarViewTypeLengthWeightMeasurement
                                            )
                                        {
                                            //var taxa_code = NSAPEntities.TaxaViewModel.TaxaCodeFromName(_speciesTaxa);
                                            //_gearUnloadWindow.SpeciesID = VesselCatchViewModel.SpeciesIDFromSpeciesName(taxa_code, _speciesName);
                                        }
                                        else
                                        {

                                            //if (_isWatchedSpeciesCalendar)
                                            //{
                                            //    var taxa_code = NSAPEntities.TaxaViewModel.TaxaCodeFromName(_speciesTaxa);
                                            //    _gearUnloadWindow.SpeciesID = VesselCatchViewModel.SpeciesIDFromSpeciesName(taxa_code, _speciesName);
                                            //    _gearUnloadWindow.WatchedSpecies = _speciesName;
                                            //}
                                        }




                                        _gearUnloadWindow.VesselUnloads = v_unloads;
                                        _gearUnloadWindow.Owner = this;
                                        _gearUnloadWindow.Show();

                                    }
                                    else
                                    {
                                        if (_gearUnloadWindow != null && !_gearUnloadWindow.IsLoaded)
                                        {
                                            _gearUnloadWindow.Close();
                                            _gearUnloadWindow = null;
                                        }
                                    }

                                    break;

                                case CalendarViewType.calendarViewTypeCountAllLandings:
                                case CalendarViewType.calendarViewTypeWeightAllLandings:
                                    if (GridNSAPData.SelectedCells.Count > 0)
                                    {
                                        var cellinfo = GridNSAPData.SelectedCells[0];
                                        if (int.TryParse(cellinfo.Column.Header.ToString(), out int v))
                                        {
                                            //string landingsite_date = $"{_allSamplingEntitiesEventHandler.LandingSite.LandingSiteName}, {((DateTime)_allSamplingEntitiesEventHandler.MonthSampled).ToString("yyyy-MMM")}-{v}";
                                            //string msg = "No data for this date";
                                            if (_gearUnloads.Count > 0)
                                            {
                                                lss = _gearUnloads[0].Parent;
                                                //if (lss.HasFishingOperation)
                                                //{
                                                //    msg = "There are fish landings on the selected date";
                                                //}
                                                //else if (!lss.HasFishingOperation)
                                                //{
                                                //    msg = $"There are no fish landings on the selected date\r\nReason: {lss.Remarks}";
                                                //}
                                            }
                                            else
                                            {

                                                lss = NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteSampling(fma: _allSamplingEntitiesEventHandler.FMA, fg: _allSamplingEntitiesEventHandler.FishingGround, ls: _allSamplingEntitiesEventHandler.LandingSite, samplingDate: ((DateTime)_allSamplingEntitiesEventHandler.MonthSampled).AddDays(v - 1)).FirstOrDefault();
                                                //if (lss != null)
                                                //{
                                                //    if (lss?.GearUnloadViewModel.Count > 0 || lss?.GearsInLandingSite.Count > 0)
                                                //    {
                                                //        msg = "There are fish landings on the selected date";
                                                //    }
                                                //    else
                                                //    {
                                                //        msg = $"There are no fish landings on the selected date\r\nReason: {lss.Remarks}";
                                                //    }
                                                //}

                                            }
                                            //MessageBox.Show($"{landingsite_date}\r\n\r\n{msg}", Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                                            if (lss != null)// && LSSMessageBox.ShowAsDialog(landingsite_date, msg))

                                            {
                                                LandingSiteSamplingWindow lssw = new LandingSiteSamplingWindow(lss);
                                                lssw.Owner = this;
                                                lssw.Show();
                                            }
                                            else
                                            {
                                                MessageBox.Show("No data for this date", Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                                            }
                                        }
                                    }
                                    break;
                            }

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
                                case "weights":
                                    unload = NSAPEntities.SummaryItemViewModel.GetVesselUnload((SummaryItem)GridNSAPData.SelectedItem);
                                    break;
                                default:
                                    return;
                            }

                            _vesselUnloadEditWindow = VesselUnloadEditWindow.GetInstance(this);
                            _vesselUnloadEditWindow.UnloadEditor.UnloadChangesSaved += OnUnloadEditor_UnloadChangesSaved;

                            if (_vesselUnloadEditWindow.Visibility == Visibility.Visible)
                            {
                                _vesselUnloadEditWindow.BringIntoView();
                            }
                            else
                            {
                                _vesselUnloadEditWindow.Show();
                            }
                            _vesselUnloadEditWindow.VesselUnload = unload;
                            _vesselUnloadEditWindow.Owner = this;
                            //unloadEditWindow.OwnerSet();
                        }

                    }

                    break;
                #endregion
                case "dataGridSpecies":
                    #region dataGridSpecies
                    var fs = (FishSpecies)dataGridSpecies.SelectedItem;

                    if (fs != null)
                    {
                        //id = fs.RowNumber.ToString();
                        id = ((int)fs.SpeciesCode).ToString();
                    }

                    break;
                #endregion
                case "dataGridEntities":
                    #region dataGridEntities
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
                    #endregion
            }

            if (id.Length > 0)
            {
                EditWindowEx ew = new EditWindowEx(_nsapEntity, id);
                ew.Owner = this;

                if ((bool)ew.ShowDialog())
                {
                    SetDataGridSource();
                    // Global.CheckForCarrierBasedLanding();

                }

            }
        }

        private void GearUnloadViewModel_ProcessingItemsEvent(object sender, ProcessingItemsEventArg e)
        {
            switch (e.Intent)
            {
                case "start":
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              mainStatusBar.IsIndeterminate = true;
                              mainStatusBar.Value = 0;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = "Getting landing data from the database...";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    break;
                case "end":
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              mainStatusBar.IsIndeterminate = false;
                              mainStatusBar.Value = 0;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = "Finished getting landing data from the database";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
            }
        }



        private async void OnUnloadEditor_UnloadChangesSaved(object sender, VesselUnloadEditorControl.UnloadEditorEventArgs e)
        {
            //throw new NotImplementedException();
            GridNSAPData.DataContext = await NSAPEntities.SummaryItemViewModel.GetDownloadDetailsByDateAsync(_downloadHistorySelectedItem);
            //GridNSAPData.Items.Refresh();
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
            _vesselUnloadEditWindow = null;
        }



        private async Task MakeCalendar(TreeViewModelControl.AllSamplingEntitiesEventHandler e)
        {
            List<GearUnload> listGearUnload = new List<GearUnload>();
            List<Day_GearLanded> listDay_GearLanded = new List<Day_GearLanded>();
            switch (e.CalendarView)
            {
                case CalendarViewType.calendarViewTypeSampledLandings:
                    //NSAPEntities.SummaryItemViewModel.RefreshMonthCalendarSource();
                    listGearUnload = await NSAPEntities.SummaryItemViewModel.GearUnloadsByMonthTask((DateTime)e.MonthSampled, bySector: true);
                    if (NSAPEntities.SummaryItemViewModel.VesselUnloadHit == 0)
                    {
                        listGearUnload = await GearUnloadViewModel.GetTotalNumberLandingsPerDayCalendar(e.LandingSite, (DateTime)e.MonthSampled);
                        e.CalendarView = CalendarViewType.calendarViewTypeCountAllLandings;
                    }
                    break;
                case CalendarViewType.calendarViewTypeCountAllLandingsByGear:
                case CalendarViewType.calendarViewTypeWeightAllLandingsByGear:
                    listGearUnload = await GearUnloadViewModel.GetGearUnloadsForCalendar(e.LandingSite, (DateTime)e.MonthSampled);
                    break;
                case CalendarViewType.calendarViewTypeCountAllLandings:
                    listGearUnload = await GearUnloadViewModel.GetTotalNumberLandingsPerDayCalendar(e.LandingSite, (DateTime)e.MonthSampled);
                    break;
                case CalendarViewType.calendarViewTypeGearDailyLandings:
                    listDay_GearLanded = await NSAPEntities.LandingSiteSamplingViewModel.GetGearLandingsForDayTask(e.LandingSite, (DateTime)e.MonthSampled);
                    break;

            }


            //var listGearUnload = NSAPEntities.SummaryItemViewModel.GearUnloadsByMonth(e,bySector:true);


            GridNSAPData.Columns.Clear();
            GridNSAPData.AutoGenerateColumns = true;
            if (e.CalendarView == CalendarViewType.calendarViewTypeGearDailyLandings)
            {
                _fishingCalendarViewModel = new FishingCalendarViewModel(listDay_GearLanded, (DateTime)e.MonthSampled, e);
            }
            else
            {
                _fishingCalendarViewModel = new FishingCalendarViewModel(listGearUnload.OrderBy(t => t.GearUsedName).ToList(), e.CalendarView, (DateTime)e.MonthSampled, e);
            }

            GridNSAPData.DataContext = _fishingCalendarViewModel.DataTable;
            _hasNonSamplingDayColumns = false;
            foreach (DataGridColumn c in GridNSAPData.Columns)
            {
                if (int.TryParse(c.Header.ToString(), out int v))
                {
                    if (_fishingCalendarViewModel.ListDayIsSamplingDay[v - 1] != null && (bool)_fishingCalendarViewModel.ListDayIsSamplingDay[v - 1] == false)
                    {
                        _hasNonSamplingDayColumns = true;
                        c.CellStyle = new Style(typeof(DataGridCell));
                        c.CellStyle.Setters.Add(new Setter(DataGridCell.BackgroundProperty, new SolidColorBrush(Colors.LightBlue)));
                    }
                }
            }
        }

        public bool CalendarDayIsSamplingDay(TreeViewModelControl.AllSamplingEntitiesEventHandler e)
        {
            return true;
        }
        private async void OnTreeViewItemSelected(object sender, TreeViewModelControl.AllSamplingEntitiesEventHandler e)
        {
            try
            {
                _monthYear = null;
                _cancelBuildCalendar = false;
                menuCalendar.Visibility = Visibility.Collapsed;
                buttonNote.Visibility = Visibility.Collapsed;
                _fishingCalendarViewModel = null;
                gridCalendarHeader.Visibility = Visibility.Visible;
                _calendarTreeSelectedEntity = e.TreeViewEntity;
                NSAPEntities.SummaryItemViewModel.TreeViewData = e;
                string labelContent = "";
                GridNSAPData.SelectionUnit = DataGridSelectionUnit.FullRow;

                _treeItemData = e;
                var tvi = ((TreeViewModelControl.TreeControl)sender)._selectedItem;

                //switch (e.TreeViewEntity)
                switch (_calendarTreeSelectedEntity)
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
                        try
                        {
                            if (e.TreeViewItem.Children.Count != NSAPEntities.SummaryItemViewModel.GetLandingSitesSampledInFishingGround(e.FishingGround, e.FMA, e.NSAPRegion).Count())
                            {
                                ((TreeViewModelControl.tv_FishingGroundViewModel)e.TreeViewItem).Refresh();
                            }
                        }
                        catch
                        {
                            //
                        }
                        break;
                    case "tv_LandingSiteViewModel":
                        labelContent = $"Summary of database content for {e.LandingSiteText}, {e.FishingGround.Name}, {e.FMA.Name}, {e.NSAPRegion.Name}";
                        SetUpSummaryGrid(SummaryLevelType.LandingSite, GridNSAPData, treeviewData: e);
                        //var months_sampled = NSAPEntities.SummaryItemViewModel.GetMonthsSampledInLandingSite(e.LandingSite);
                        //;if (e.TreeViewItem.Children.Count != months_sampled.Count)
                        if (e.TreeViewItem.Children.Count != NSAPEntities.SummaryItemViewModel.GetMonthsSampledInLandingSite(e.LandingSite, e.FishingGround).Count())
                        {
                            ((TreeViewModelControl.tv_LandingSiteViewModel)e.TreeViewItem).Refresh();
                        }


                        //if (e.TreeViewItem.Children.Count != LandingSiteSamplingRepository.GetMonthsSampledInLandingSite(e.LandingSite, e.FishingGround,e.FMA,e.NSAPRegion).Count())
                        //{
                        //    ((TreeViewModelControl.tv_LandingSiteViewModel)e.TreeViewItem).Refresh();
                        //}
                        break;
                    case "tv_MonthViewModel":
                        UnCheckCalendarMenuItems();
                        _allSamplingEntitiesEventHandler = e;
                        menuCalendar.Visibility = Visibility.Visible;
                        SetUpCalendarMenu();
                        _calendarOption = e.CalendarView;

                        if (Global.Settings.UseAlternateCalendar)
                        {
                            //NSAPEntities.CalendarMonthViewModel = new CalendarMonthViewModel(e);
                            //GridNSAPData.SelectionUnit = DataGridSelectionUnit.Cell;
                            //GridNSAPData.Columns.Clear();
                            //GridNSAPData.AutoGenerateColumns = true;
                            //GridNSAPData.DataContext = NSAPEntities.CalendarMonthViewModel.LandingsCountDataTable;
                            //GridNSAPData.Visibility = Visibility.Visible;

                            //foreach (DataGridColumn c in GridNSAPData.Columns)
                            //{
                            //    if (int.TryParse(c.Header.ToString(), out int v))
                            //    {
                            //        if (NSAPEntities.CalendarMonthViewModel.DayIsRestDay(v))
                            //        {
                            //            _hasNonSamplingDayColumns = true;
                            //            c.CellStyle = new Style(typeof(DataGridCell));
                            //            c.CellStyle.Setters.Add(new Setter(DataGridCell.BackgroundProperty, new SolidColorBrush(Colors.LightBlue)));
                            //        }
                            //    }
                            //}
                        }
                        else
                        {
                            Logger.LogCalendar("start");
                            if (e.NSAPRegion.RegionWatchedSpeciesViewModel == null)
                            {
                                e.NSAPRegion.RegionWatchedSpeciesViewModel = new RegionWatchedSpeciesViewModel(e.NSAPRegion);
                            }


                            //CrossTabGenerator.GetVesselUnloads(_allSamplingEntitiesEventHandler);
                            GridNSAPData.SelectionUnit = DataGridSelectionUnit.Cell;

                            NSAPEntities.FishingCalendarDayExViewModel.CalendarViewType = _calendarOption;

                            NSAPEntities.FishingCalendarDayExViewModel.CalendarEvent += FishingCalendarDayExViewModel_CalendarEvent;
                            ShowStatusRow();
                            await NSAPEntities.FishingCalendarDayExViewModel.GetCalendarDaysForMonth(e);
                            await NSAPEntities.FishingCalendarDayExViewModel.MakeCalendarTask();
                            //NSAPEntities.FishingCalendarDayExViewModel.MakeCalendar();
                            GridNSAPData.Columns.Clear();
                            GridNSAPData.AutoGenerateColumns = true;
                            Logger.LogCalendar($"datatable from NSAPEntities.FishingCalendarDayExViewModel.DataTable created with {NSAPEntities.FishingCalendarDayExViewModel.DataTable.Rows.Count} rows");
                            GridNSAPData.DataContext = NSAPEntities.FishingCalendarDayExViewModel.DataTable;
                            Logger.LogCalendar($"NSAPEntities.FishingCalendarDayExViewModel.CalendarHasValue?: {NSAPEntities.FishingCalendarDayExViewModel.CalendarHasValue}");
                            if (NSAPEntities.FishingCalendarDayExViewModel.CalendarHasValue)
                            {
                                GridNSAPData.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                GridNSAPData.Visibility = Visibility.Collapsed;
                            }

                            _hasNonSamplingDayColumns = false;
                            foreach (DataGridColumn c in GridNSAPData.Columns)
                            {
                                if (int.TryParse(c.Header.ToString(), out int v))
                                {
                                    if (NSAPEntities.FishingCalendarDayExViewModel.DayIsRestDay(v))
                                    {
                                        _hasNonSamplingDayColumns = true;
                                        c.CellStyle = new Style(typeof(DataGridCell));
                                        c.CellStyle.Setters.Add(new Setter(DataGridCell.BackgroundProperty, new SolidColorBrush(Colors.LightBlue)));
                                    }
                                }
                            }
                            //SetupCalendar();
                            SetupCalendarLabels();
                            NSAPEntities.FishingCalendarDayExViewModel.CalendarEvent += FishingCalendarDayExViewModel_CalendarEvent;
                            ShowStatusRow(isVisible: false);
                            Logger.LogCalendar("end");
                        }
                        break;
                    case "tv_MonthViewModelx":
                        _allSamplingEntitiesEventHandler = e;
                        GridNSAPData.SelectionUnit = DataGridSelectionUnit.Cell;
                        menuCalendar.Visibility = Visibility.Visible;

                        SetUpCalendarMenu();
                        _calendarOption = e.CalendarView;
                        if (_calendarFirstInvokeDone)
                        {
                            //SetupCalendar(e.CalendarView);d

                            await SetupCalendar();//_calendarViewType);
                        }

                        if (!_calendarFirstInvokeDone)
                        {
                            _calendarFirstInvokeDone = true;
                        }
                        //SetupCalendarView(labelContent);
                        if (e.CalendarView == CalendarViewType.calendarViewTypeCountAllLandings)
                        {
                            _cancelBuildCalendar = true;
                            menuTotalLandingsCalendar.IsChecked = true;
                            _cancelBuildCalendar = false;
                        }
                        break;
                }

                SetupCalendarView(labelContent);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private void FishingCalendarDayExViewModel_CalendarEvent(object sender, MakeCalendarEventArg e)
        {
            switch (e.Context)
            {
                case "Fetching landing data from database":
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              mainStatusBar.IsIndeterminate = true;
                              mainStatusBar.Value = 0;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = "Fetching landing data. Please wait...";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "Fetched landing data from database":
                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = "Landing data retrieved";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "Preparing calendar data":
                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = "Creating calendar";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "Calendar data created":
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              mainStatusBar.IsIndeterminate = false;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = "Calendar created";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;

            }
        }

        private void SetupCalendarView(string labelContent)
        {
            if (_calendarTreeSelectedEntity != "tv_MonthViewModel")
            {
                MonthLabel.Content = labelContent;
                MonthSubLabel.Visibility = Visibility.Collapsed;
                labelSummary.Content = labelContent;
                dataGridSummary.Visibility = Visibility.Visible;
                GridNSAPData.Visibility = Visibility.Visible;
                panelOpening.Visibility = Visibility.Visible;
                labelRowCount.Visibility = Visibility.Collapsed;
            }
            else
            {
                labelRowCount.Visibility = Visibility.Visible;
                MonthSubLabel.Visibility = Visibility.Visible;
                _acceptDataGridCellClick = true;

                //var totlaLandingsCount = "";
                //if (_fishingCalendarViewModel != null && _allSamplingEntitiesEventHandler.CalendarView != CalendarViewType.calendarViewTypeGearDailyLandings)
                //{
                //    int totalLandingsCount = _fishingCalendarViewModel.CountVesselUnloads;
                //    totlaLandingsCount = $", Total landings: {totalLandingsCount}";
                //    int vuCountInGU = 0;
                //    foreach (var gu in _fishingCalendarViewModel.UnloadList)
                //    {
                //        vuCountInGU += gu.ListVesselUnload.Count;
                //    }
                //    if (vuCountInGU != totalLandingsCount)
                //    {
                //        totlaLandingsCount += $" ({vuCountInGU})";
                //        buttonFix.Visibility = Visibility.Visible;
                //    }
                //}
                //if (GridNSAPData.Items.Count > 0)
                //{
                //    string columnStyle = " (Blue columns represent rest day)";
                //    if (!_hasNonSamplingDayColumns)
                //    {
                //        columnStyle = "";
                //    }
                //    labelRowCount.Content = $"Rows: {GridNSAPData.Items.Count}{totlaLandingsCount}{columnStyle}";
                //}
                //else
                //{
                //    labelRowCount.Content = "Older eforms cannot encode data. Use Catch and Effort eForm version 7.14";
                //}
            }
        }

        private async void OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            _calendarDay = null;
            DataGridCellInfo cell;
            //_monthYear = null;
            if (_monthYear == null && _treeItemData != null)
            {
                _monthYear = _treeItemData.MonthSampled;
            }
            var type = GridNSAPData?.DataContext?.GetType().ToString();
            switch (_currentDisplayMode)
            {
                case DataDisplayMode.CBLSampling:
                    _carrierLandingFromCalendar = null;
                    try
                    {

                        cell = gridCBL.SelectedCells[0];
                        _gridRow = gridCBL.Items.IndexOf(cell.Item);
                        _gridCol = cell.Column.DisplayIndex;
                        var cbl = gridCBL.Items[_gridRow] as DataRowView;
                        if (cbl != null)
                        {
                            _carrierBoatName = (string)cbl.Row.ItemArray[0];
                            if (_gridCol > 0)
                            {
                                _carrierLandingFromCalendar = _fishingCalendarViewModel.FishingCalendarList.FirstOrDefault(t => t.CarrierBoatName == _carrierBoatName).CarrierLandings[_gridCol - 1];
                            }
                            //_monthYear = DateTime.Parse(cbl.Row.ItemArray[3].ToString());
                        }
                    }
                    catch { }
                    break;
                case DataDisplayMode.ODKData:
                    _maturityStage = null;
                    _speciesTaxa = null;
                    _speciesName = null;
                    _calendarDay = null;
                    _calendarDayHasValue = false;
                    if (GridNSAPData.SelectedCells.Count == 1 && _acceptDataGridCellClick)
                    {
                        int gridColumnForDay1 = 0;
                        cell = GridNSAPData.SelectedCells[0];
                        _gridRow = GridNSAPData.Items.IndexOf(cell.Item);
                        _gridCol = cell.Column.DisplayIndex;

                        var item = GridNSAPData.Items[_gridRow] as DataRowView;
                        _calendarDayHasValue = item.Row.ItemArray[_gridCol] != DBNull.Value;
                        _calendarDayValue = item.Row.ItemArray[_gridCol].ToString();
                        if (_calendarDayHasValue)
                        {
                            if (item != null)
                            {

                                if (_isWatchedSpeciesCalendar)
                                {
                                    if (Global.Settings.UseAlternateCalendar)
                                    {
                                        if (_getFemaleMaturity)
                                        {
                                            _speciesTaxa = (string)item.Row.ItemArray[0];
                                            _speciesName = (string)item.Row.ItemArray[1];
                                            _maturityStage = (string)item.Row.ItemArray[3];
                                            _gearName = (string)item.Row.ItemArray[4];
                                            _gearCode = (string)item.Row.ItemArray[5];
                                            _fish_sector = (string)item.Row.ItemArray[6];
                                            _monthYear = DateTime.Parse(item.Row.ItemArray[7].ToString());
                                            gridColumnForDay1 = 8;

                                        }
                                        else if (!_isMeasuredWatchedSpeciesCalendar)
                                        {
                                            _speciesTaxa = (string)item.Row.ItemArray[0];
                                            _speciesName = (string)item.Row.ItemArray[1];
                                            _gearName = (string)item.Row.ItemArray[2];
                                            _gearCode = (string)item.Row.ItemArray[3];
                                            _fish_sector = (string)item.Row.ItemArray[4];
                                            _monthYear = DateTime.Parse(item.Row.ItemArray[5].ToString());
                                            gridColumnForDay1 = 6;
                                        }
                                        else
                                        {
                                            _speciesTaxa = (string)item.Row.ItemArray[0];
                                            _speciesName = (string)item.Row.ItemArray[1];
                                            _gearName = (string)item.Row.ItemArray[3];
                                            _gearCode = (string)item.Row.ItemArray[4];
                                            _fish_sector = (string)item.Row.ItemArray[5];
                                            _monthYear = DateTime.Parse(item.Row.ItemArray[6].ToString());
                                            gridColumnForDay1 = 7;
                                        }
                                    }
                                    else
                                    {
                                        if (_getFemaleMaturity)
                                        {
                                            _speciesTaxa = (string)item.Row.ItemArray[0];
                                            _speciesName = (string)item.Row.ItemArray[1];
                                            _maturityStage = (string)item.Row.ItemArray[2];
                                            _gearName = (string)item.Row.ItemArray[3];
                                            _gearCode = (string)item.Row.ItemArray[4];
                                            _fish_sector = (string)item.Row.ItemArray[5];
                                            _monthYear = DateTime.Parse(item.Row.ItemArray[6].ToString());
                                            gridColumnForDay1 = 7;

                                        }
                                        else
                                        {
                                            _speciesTaxa = (string)item.Row.ItemArray[0];
                                            _speciesName = (string)item.Row.ItemArray[1];
                                            _gearName = (string)item.Row.ItemArray[2];
                                            _gearCode = (string)item.Row.ItemArray[3];
                                            _fish_sector = (string)item.Row.ItemArray[4];
                                            _monthYear = DateTime.Parse(item.Row.ItemArray[5].ToString());
                                            gridColumnForDay1 = 6;
                                        }
                                    }
                                    // _calendarDay = _gridCol - (gridColumnForDay1 - 1);
                                }
                                else
                                {
                                    _gearName = (string)item.Row.ItemArray[0];
                                    _gearCode = (string)item.Row.ItemArray[1];
                                    _fish_sector = (string)item.Row.ItemArray[2];
                                    _monthYear = DateTime.Parse(item.Row.ItemArray[3].ToString());
                                    gridColumnForDay1 = 4;
                                    //_calendarDay = _gridCol - (gridColumnForDay1 - 1);

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
                                            CrossTabManager.IsCarrierLandding = false;
                                            await CrossTabManager.GearByMonthYearAsync(_allSamplingEntitiesEventHandler);
                                            ShowCrossTabWIndow();
                                        }
                                    }
                                    else
                                    {
                                        GridNSAPData.ContextMenu = null;
                                        //if (_gridCol > 3)
                                        //{
                                        //    _calendarDay = _gridCol - 3;
                                        //}
                                    }
                                }
                                _calendarGridColumnName = GridNSAPData.Columns[_gridCol].Header.ToString();
                                if (int.TryParse(_calendarGridColumnName, out int v))
                                {
                                    _calendarDay = v;
                                }

                                if (NSAPEntities.SummaryItemViewModel.SummaryResults.Count > 0)
                                {

                                }

                                if (!string.IsNullOrEmpty(_fish_sector))
                                {
                                    switch (_fish_sector)
                                    {
                                        case "Commercial":
                                            _sector_code = "c";
                                            break;
                                        case "Municipal":
                                            _sector_code = "m";
                                            break;
                                    }
                                }
                                _gearUnloads = new List<GearUnload>();



                                if (_gridCol == 0)
                                {

                                }
                                else if (_gridCol >= gridColumnForDay1)
                                {
                                    NSAPEntities.FishingCalendarDayExViewModel.MonthSampled = _allSamplingEntitiesEventHandler;
                                    GearUnload gear_unload_from_day = NSAPEntities.FishingCalendarDayExViewModel.GetGearUnload(_gearName, _fish_sector, _gridCol - (gridColumnForDay1 - 1), _calendarOption, isAlternateCalendar: Global.Settings.UseAlternateCalendar, gearCode: _gearCode);
                                    //GearUnload gear_unload_from_day = _fishingCalendarViewModel.FishingCalendarList.FirstOrDefault(t => t.GearName == _gearName && t.Sector == _fish_sector).GearUnloads[_gridCol - 4];

                                    //sectorCode = gear_unload_from_day.SectorCode;

                                    if (gear_unload_from_day != null)
                                    {

                                        var lss = gear_unload_from_day.Parent;
                                        lss.GearUnloadViewModel = new GearUnloadViewModel(lss);
                                        List<GearUnload> list_gu = lss.GearUnloadViewModel.GearUnloadCollection
                                            .Where(t => t.GearID == gear_unload_from_day.GearID).ToList();

                                        _gearUnloads = list_gu;

                                    }


                                }
                                else
                                {

                                    switch (_calendarGridColumnName)
                                    {
                                        case "Maturity stage":
                                            break;
                                        case "Species":
                                            break;
                                        case "GearName":
                                            break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            _gearUnloads = null;
                        }
                        //_gearUnloads.Add(gear_unload_from_day);

                        //_gearUnloads = NSAPEntities.SummaryItemViewModel.GetGearUnloads(_gearName, _gridCol - 4, sector_code);
                    }

                    if ( _gearUnloadWindow != null  )
                    {
                        if ( _gearUnloads != null && _gearUnloads.Count > 0)
                        {
                            await _gearUnloadWindow.GetVesselUnloadsFromGearUnloadsTask(
                                treeviewData:_allSamplingEntitiesEventHandler,
                                gus: _gearUnloads,
                                viewType: _calendarOption,
                                sector_code: _sector_code,
                                taxa: _speciesTaxa,
                                speciesName: _speciesName,
                                isFemaleMaturity: _getFemaleMaturity,
                                maturityStage: _maturityStage
                                );
                        }
                        else
                        {
                            _gearUnloadWindow.TurnGridOff();
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
        private async Task GenerateCrossTabFirstVersion()
        {
            ShowStatusRow();
            CrossTabManager.IsCarrierLandding = false;
            await CrossTabManager.GearByMonthYearAsync(_allSamplingEntitiesEventHandler);
            ShowCrossTabWIndow();
        }
        private async void OnTreeContextMenu(object sender, TreeViewModelControl.AllSamplingEntitiesEventHandler e)
        {
            if (e.Equals(_allSamplingEntitiesEventHandler) && string.IsNullOrEmpty(e.GUID))
            {
                e.GUID = _allSamplingEntitiesEventHandler.GUID;
            }
            _allSamplingEntitiesEventHandler = e;
            switch (e.ContextMenuTopic)
            {
                case "contextMenuFemaleMaturitySummary":
                    SetUpSummaryGrid(SummaryLevelType.Region, GridNSAPData, region: e.NSAPRegion, summaryName: "femaleMaturityStages");
                    break;
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
                    CrossTabGenerator.CrossTabEvent += CrossTabGenerator_CrossTabEvent;
                    CrossTabDatasetsGenerator.CrossTabDatasetEvent += CrossTabDatasetsGenerator_CrossTabDatasetEvent;
                    if (await CrossTabGenerator.GenerateCrossTabTask(_allSamplingEntitiesEventHandler))
                    {
                        CrossTabWindow ctw = new CrossTabWindow();
                        ctw.ShowDialog();
                    }
                    CrossTabDatasetsGenerator.CrossTabDatasetEvent -= CrossTabDatasetsGenerator_CrossTabDatasetEvent;
                    CrossTabGenerator.CrossTabEvent -= CrossTabGenerator_CrossTabEvent;


                    break;
                case "contextMenuWeightValidation":
                    ShowStatusRow();
                    var items = await NSAPEntities.SummaryItemViewModel.GetDownloadDetailsByCalendarTreeSelectionTaskAsync(_treeItemData, monthInTreeView: true);
                    GearUnloadWindow guw = new GearUnloadWindow(items, _treeItemData);
                    guw.Owner = this;
                    guw.ShowDialog();
                    break;
                case "contextMenuNSAPForm1":
                    NSAPForm1 nsapForm1 = new NSAPForm1();
                    nsapForm1.FMA = _allSamplingEntitiesEventHandler.FMA;
                    nsapForm1.MonthSampled = (DateTime)_allSamplingEntitiesEventHandler.MonthSampled;
                    nsapForm1.FishingGround = _allSamplingEntitiesEventHandler.FishingGround;
                    nsapForm1.LandingSite = _allSamplingEntitiesEventHandler.LandingSite;
                    nsapForm1.Region = _allSamplingEntitiesEventHandler.NSAPRegion;
                    nsapForm1.GetData();
                    break;
                case "contextMenuNSAPForm2":
                case "contextMenuNSAPForm2a":
                case "contextMenuNSAPForm2b":
                case "contextMenuNSAPForm3":
                case "contextMenuNSAPForm4":
                case "contextMenuNSAPForm5":
                    break;
                case "contextMenuMapMonth":
                    Mapping.MapWindowManager.OpenMapWindow(this);
                    Mapping.FishingGroundPointsFromCalendarMappingManager.MapInteractionHandler = Mapping.MapWindowManager.MapInterActionHandler;
                    Mapping.FishingGroundPointsFromCalendarMappingManager.EntitiesMonth = _allSamplingEntitiesEventHandler;
                    Mapping.FishingGroundPointsFromCalendarMappingManager.MappingContext = e.ContextMenuTopic;

                    DateTime monthSampled = (DateTime)e.MonthSampled;
                    int no_days = DateTime.DaysInMonth(monthSampled.Year, monthSampled.Month);
                    DateTime endOfMonth = monthSampled.AddDays(no_days - 1);

                    Mapping.FishingGroundPointsFromCalendarMappingManager.Description = $"Map fishing grounds of landings at {e.LandingSite} from {monthSampled.ToString("MMM dd, yyyy")} to {endOfMonth.ToString("MMM dd, yyyy")}";
                    if (await Mapping.FishingGroundPointsFromCalendarMappingManager.MapFishingGroundPoint() == false)
                    {
                        MessageBox.Show("Sampled landings do not contain fishing ground data",
                            Global.MessageBoxCaption,
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                            );
                    }
                    break;
                case "contextMenuMeasurementCountsMonth":
                    VesselCatchRepository.GetSpeciesMeasurementCounts(e);
                    SetUpSummaryGrid(SummaryLevelType.LandingSiteMonth, GridNSAPData, treeviewData: _allSamplingEntitiesEventHandler, summaryName: "landingSiteMonthMeasurementCounts");
                    break;
                case "contextMenuMeasurementCountsLandingSite":

                    SetUpSummaryGrid(SummaryLevelType.LandingSite, GridNSAPData, treeviewData: _allSamplingEntitiesEventHandler, summaryName: "landingSiteMeasurementCounts");
                    break;
                case "contextMenuFemaleMeasurementCountsLandingSite":
                    SetUpSummaryGrid(SummaryLevelType.LandingSite, GridNSAPData, treeviewData: _allSamplingEntitiesEventHandler, summaryName: "landingSiteFemaleMeasurementCounts");
                    break;
            }
        }

        private void CrossTabDatasetsGenerator_CrossTabDatasetEvent(object sender, CrossTabReportEventArg e)
        {
            switch (e.Context)
            {
                case "Creating datasets":
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              mainStatusBar.IsIndeterminate = false;
                              mainStatusBar.Maximum = e.DataSetsToProcessCount;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = "Creating datasets...";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "Created datasets":
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              mainStatusBar.Value = e.DataSetsProcessedCount;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);
                    break;
                case "Done creating datasets":
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              mainStatusBar.IsIndeterminate = false;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    rowStatus.Dispatcher.BeginInvoke(
                        DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                            {

                                rowStatus.Height = new GridLength(0);
                                //do what you need to do on UI Thread
                                return null;
                            }), null);
                    //mainStatusLabel.Dispatcher.BeginInvoke
                    //    (
                    //      DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                    //      {
                    //          mainStatusLabel.Content = "Finished creating datasets...";
                    //          //do what you need to do on UI Thread
                    //          return null;
                    //      }
                    //     ), null);
                    break;
            }
        }

        private void CrossTabGenerator_CrossTabEvent(object sender, CrossTabReportEventArg e)
        {
            switch (e.Context)
            {
                case "Getting entities":
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              mainStatusBar.IsIndeterminate = true;
                              mainStatusBar.Value = 0;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = "Fetching landing data. Please wait...";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "Finished getting entities":
                    //mainStatusBar.Dispatcher.BeginInvoke
                    //    (
                    //      DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                    //      {

                    //          mainStatusBar.IsIndeterminate = false;
                    //          //do what you need to do on UI Thread
                    //          return null;
                    //      }), null);

                    //rowStatus.Dispatcher.BeginInvoke(
                    //    DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                    //        {

                    //            rowStatus.Height = new GridLength(0);
                    //            //do what you need to do on UI Thread
                    //            return null;
                    //        }), null);
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

        private void CloseTallyWindow()
        {
            if (_wvtw != null)
            {
                _wvtw.Close();
                _wvtw = null;
            }
        }

        public async void RefreshAfterDeleteVesselUnload()
        {
            string itemTag = ((TreeViewItem)treeViewDownloadHistory.SelectedItem).Tag.ToString();
            switch (itemTag)
            {
                case "downloadDate":
                    break;
                case "effort":
                    GridNSAPData.DataContext = await NSAPEntities.SummaryItemViewModel.GetDownloadDetailsByDateAsync(_downloadHistorySelectedItem);
                    break;
            }
        }
        private async void OnHistoryTreeItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ShowStatusRow();
            GridNSAPData.Visibility = Visibility.Visible;
            carrierSummaryPropertyGrid.Visibility = Visibility.Collapsed;
            gridCalendarHeader.Visibility = Visibility.Visible;
            GridNSAPData.SelectionUnit = DataGridSelectionUnit.FullRow;
            GridNSAPData.IsReadOnly = true;
            labelRowCount.Visibility = Visibility.Visible;
            MonthLabel.Visibility = Visibility.Visible;
            MonthLabel.Content = $"Vessel unload by date of download";


            var col = new DataGridTextColumn();

            var dt = DateTime.Now;
            if (e.NewValue != null)
            {
                TreeViewItem tvItem = null;
                tvItem = (TreeViewItem)e.NewValue;

                string itemTag = tvItem.Tag.ToString();
                if (itemTag != "weights")
                {
                    CloseTallyWindow();
                }

                switch (itemTag)
                {
                    case "cbl_landings":
                        MonthLabel.Content = "Summary of samplings of carrier boats";
                        var dbs = NSAPEntities.LandingSiteSamplingViewModel.GetCarrierLandingsOverallSummary();
                        carrierSummaryPropertyGrid.SelectedObject = dbs;
                        carrierSummaryPropertyGrid.Visibility = Visibility.Visible;
                        carrierSummaryPropertyGrid.AutoGenerateProperties = false;
                        carrierSummaryPropertyGrid.ShowAdvancedOptions = false;
                        carrierSummaryPropertyGrid.ShowSearchBox = false;
                        carrierSummaryPropertyGrid.ShowSortOptions = false;
                        carrierSummaryPropertyGrid.ShowSummary = false;
                        carrierSummaryPropertyGrid.ShowTitle = false;
                        carrierSummaryPropertyGrid.IsReadOnly = true;

                        carrierSummaryPropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of months", Name = "CountMonths", DisplayOrder = 1 });
                        carrierSummaryPropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of landing sites", Name = "CountLandingSites", DisplayOrder = 2 });
                        carrierSummaryPropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of samplings", Name = "CountCarrierSamplings", DisplayOrder = 3 });
                        carrierSummaryPropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of boats", Name = "CountCarrierBoats", DisplayOrder = 4 });
                        if (dbs.CountAllLandings > 0)
                        {
                            carrierSummaryPropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "First sampling date", Name = "FirstSampledLandingDate", DisplayOrder = 5 });
                            carrierSummaryPropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Latest sampling date", Name = "LastSampledLandingDate", DisplayOrder = 6 });
                        }
                        gridCBL.Visibility = Visibility.Collapsed;
                        labelCBL.Content = "Summary of sampling of fish carrier landings";
                        break;
                    case "downloadDate":
                        //dt = DateTime.Parse(tvItem.Header.ToString()).Date;
                        _downloadHistorySelectedItem = DateTime.Parse(tvItem.Header.ToString()).Date;
                        GridNSAPData.DataContext = await NSAPEntities.SummaryItemViewModel.GetDownloadSummaryByDateAsync(_downloadHistorySelectedItem);
                        GridNSAPData.Columns.Clear();
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("DBSummary.EnumeratorName") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("DBSummary.GearName") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Date of first sampling", Binding = new Binding("DBSummary.FirstLandingFormattedDate") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Date of last sampling", Binding = new Binding("DBSummary.LastLandingFormattedDate") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Date downloaded", Binding = new Binding("DBSummary.LatestDownloadFormattedDate") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "# landings", Binding = new Binding("DBSummary.VesselUnloadCount"), CellStyle = AlignRightStyle });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "# landings with catch composition", Binding = new Binding("DBSummary.CountLandingsWithCatchComposition"), CellStyle = AlignRightStyle });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "# tracked fishing operations", Binding = new Binding("DBSummary.TrackedOperationsCount"), CellStyle = AlignRightStyle });

                        GridNSAPData.AutoGenerateColumns = false;

                        break;
                    case "weights":
                    case "effort":
                    case "tracked":

                        //dt = DateTime.Parse(((TreeViewItem)tvItem.Parent).Header.ToString()).Date;
                        _downloadHistorySelectedItem = DateTime.Parse(((TreeViewItem)tvItem.Parent).Header.ToString()).Date;
                        if (tvItem.Tag.ToString() == "tracked")
                        {
                            GridNSAPData.DataContext = await NSAPEntities.SummaryItemViewModel.GetDownloadDetailsByDateAsync(_downloadHistorySelectedItem, isTracked: true);
                        }
                        else
                        {
                            GridNSAPData.DataContext = await NSAPEntities.SummaryItemViewModel.GetDownloadDetailsByDateAsync(_downloadHistorySelectedItem);
                        }
                        GridNSAPData.AutoGenerateColumns = false;
                        GridNSAPData.Columns.Clear();
                        GridNSAPData.SetValue(Grid.ColumnSpanProperty, 2);
                        GearUnload_ButtonsPanel.Visibility = Visibility.Collapsed;


                        if (tvItem.Tag.ToString() == "weights")
                        {
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("VesselUnloadID") });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("EnumeratorNameToUse") });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Region", Binding = new Binding("Region.Name") });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("FMA.Name") });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Fishing ground ", Binding = new Binding("FishingGround.Name") });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Landing site ", Binding = new Binding("LandingSiteNameText") });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Gear ", Binding = new Binding("GearUsedName") });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Ref # ", Binding = new Binding("RefNo") });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Form version ", Binding = new Binding("FormVersion") });

                            col = new DataGridTextColumn()
                            {
                                Binding = new Binding("SamplingDate"),
                                Header = "Sampling date"
                            };

                            col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                            GridNSAPData.Columns.Add(col);

                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Weight of catch ", Binding = new Binding("VesselUnload.VesselUnloadWeights.CatchWeight"), CellStyle = AlignRightStyle });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Weight of sample\r\nfrom catch", Binding = new Binding("VesselUnload.VesselUnloadWeights.CatchSampleWeight"), CellStyle = AlignRightStyle });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Number of species\r\nin catch", Binding = new Binding("VesselUnload.CountCatchCompositionItems"), CellStyle = AlignRightStyle });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Raising factor ", Binding = new Binding("VesselUnload.VesselUnloadWeights.RaisingFactor"), CellStyle = AlignRightStyle });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Weight of catch\r\ncomposition", Binding = new Binding("VesselUnload.VesselUnloadWeights.SumOfCatchCompositionWeights"), CellStyle = AlignRightStyle });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Weight of catch\r\ncomposition from sample ", Binding = new Binding("VesselUnload.VesselUnloadWeights.SumOfCatchCompositionFromSampleWeight"), CellStyle = AlignRightStyle });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Type of sampling", Binding = new Binding("VesselUnload.SamplingTypeFlagText") });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Validity of weights ", Binding = new Binding("VesselUnload.WeightValidationFlagText") });
                        }
                        else
                        {

                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("VesselUnloadID") });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("EnumeratorNameToUse") });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Region", Binding = new Binding("Region.Name") });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("FMA.Name") });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Fishing ground ", Binding = new Binding("FishingGround.Name") });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Landing site ", Binding = new Binding("LandingSiteNameText") });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Gear ", Binding = new Binding("GearUsedName") });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Ref # ", Binding = new Binding("RefNo") });

                            col = new DataGridTextColumn()
                            {
                                Binding = new Binding("SamplingDate"),
                                Header = "Sampling date"
                            };
                            col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                            GridNSAPData.Columns.Add(col);

                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("Sector") });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("VesselNameToUse") });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Number of fishers", Binding = new Binding("NumberOfFishers"), CellStyle = AlignRightStyle });

                            if (tvItem.Tag.ToString() == "tracked")
                            {
                                GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "GPS ", Binding = new Binding("GPSNameToUse") });
                            }
                            else if (tvItem.Tag.ToString() == "weights")
                            {

                            }
                            else
                            {
                                GridNSAPData.Columns.Add(new DataGridCheckBoxColumn { Header = "Excel download ", Binding = new Binding("FromExcelDownload") });
                                GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Form version ", Binding = new Binding("FormVersion") });
                                GridNSAPData.Columns.Add(new DataGridCheckBoxColumn { Header = "Multi-gear", Binding = new Binding("IsMultiGear") });
                                GridNSAPData.Columns.Add(new DataGridCheckBoxColumn { Header = "Is catch sold", Binding = new Binding("IsCatchSold") });
                                GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "# of gears used ", Binding = new Binding("CountFishingGearTypesUsed") });
                            }

                        }
                        break;
                    case "gearUnload_mv":
                        //dt = DateTime.Parse(((TreeViewItem)tvItem.Parent).Header.ToString()).Date;
                        _downloadHistorySelectedItem = DateTime.Parse(((TreeViewItem)tvItem.Parent).Header.ToString()).Date;
                        GridNSAPData.DataContext = await NSAPEntities.SummaryItemViewModel.GetGearUnloadsMultiVesselAsync(_downloadHistorySelectedItem);
                        GridNSAPData.Columns.Clear();
                        GridNSAPData.AutoGenerateColumns = false;

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

                        GridNSAPData.Columns.Add(new DataGridCheckBoxColumn { Header = "Sampling day", Binding = new Binding("Parent.IsSamplingDay"), IsReadOnly = true });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("GearUsedName"), IsReadOnly = true });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "# of landings", Binding = new Binding("Parent.NumberOfLandings"), IsReadOnly = true });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "# of sampled landings", Binding = new Binding("Parent.NumberOfLandingsSampled"), IsReadOnly = true });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "# of commercial landings", Binding = new Binding("NumberOfCommercialLandings"), IsReadOnly = true });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "# of municipal landings", Binding = new Binding("NumberOfMunicipalLandings"), IsReadOnly = true });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Weight of catch of commercial landings", Binding = new Binding("WeightOfCommercialLandings"), IsReadOnly = true });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Weight of catch of municipal landings", Binding = new Binding("WeightOfMunicipalLandings"), IsReadOnly = true });
                        break;
                    case "gearUnload":
                        //dt = DateTime.Parse(((TreeViewItem)tvItem.Parent).Header.ToString()).Date;
                        _downloadHistorySelectedItem = DateTime.Parse(((TreeViewItem)tvItem.Parent).Header.ToString()).Date;
                        GridNSAPData.DataContext = await NSAPEntities.SummaryItemViewModel.GetGearUnloadsAsync(_downloadHistorySelectedItem);
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

                        GridNSAPData.Columns.Add(new DataGridCheckBoxColumn { Header = "Sampling day", Binding = new Binding("Parent.IsSamplingDay"), IsReadOnly = true });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("GearUsedName"), IsReadOnly = true });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "# of sampled landings", Binding = new Binding("NumberOfSampledLandingsEx"), IsReadOnly = true });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Boats", Binding = new Binding("Boats"), IsReadOnly = false, CellStyle = AlignRightStyle });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Catch", Binding = new Binding("Catch"), IsReadOnly = false, CellStyle = AlignRightStyle });

                        break;
                    case "unloadSummary":
                        //dt = DateTime.Parse(((TreeViewItem)tvItem.Parent).Header.ToString()).Date;
                        _downloadHistorySelectedItem = DateTime.Parse(((TreeViewItem)tvItem.Parent).Header.ToString()).Date;
                        GridNSAPData.DataContext = await NSAPEntities.SummaryItemViewModel.GetUnloadStatisticsByDateAsync(_downloadHistorySelectedItem);
                        GridNSAPData.Columns.Clear();
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("VesselUnloadID") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Date sampled", Binding = new Binding("SamplingDateFormatted") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Region", Binding = new Binding("Region") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("FMA") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("FishingGround") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("LandingSiteNameText") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("GearUsedName") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Ref #", Binding = new Binding("RefNo") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("EnumeratorNameToUse") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Fishing grid count", Binding = new Binding("FishingGridRows"), CellStyle = AlignRightStyle });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Soak time count", Binding = new Binding("GearSoakRows"), CellStyle = AlignRightStyle });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Effort indicator count", Binding = new Binding("VesselEffortRows"), CellStyle = AlignRightStyle });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Catch composition count", Binding = new Binding("CatchCompositionRows"), CellStyle = AlignRightStyle });

                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Length freq count", Binding = new Binding("LenFreqRows"), CellStyle = AlignRightStyle });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Length count", Binding = new Binding("LengthRows"), CellStyle = AlignRightStyle });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Length weight count", Binding = new Binding("LenWtRows"), CellStyle = AlignRightStyle });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Maturity count", Binding = new Binding("CatchMaturityRows"), CellStyle = AlignRightStyle });

                        break;
                    default:
                        if (tvItem.Tag.GetType().Name == "UnmatchedFieldsFromJSONFile")
                        {
                            UnmatchedJSONAnalysisResultWindow uw = UnmatchedJSONAnalysisResultWindow.GetInstance();
                            uw.UnmatchedFieldsFromJSONFile = tvItem.Tag as UnmatchedFieldsFromJSONFile;
                            uw.Owner = this;
                            if (uw.Visibility == Visibility.Visible)
                            {
                                uw.BringIntoView();
                                uw.ShowAnalysis();
                            }
                            else
                            {
                                uw.Show();
                            }
                            tvItem.Focus();
                        }
                        else if (tvItem.Tag.GetType().Name == "LandingSite")
                        {
                            carrierSummaryPropertyGrid.Visibility = Visibility.Collapsed;
                            gridCBL.Visibility = Visibility.Visible;
                            if (((TreeViewItem)tvItem.Parent).Tag.ToString() == "cbl_landings")
                            {
                                _landingSite = (LandingSite)tvItem.Tag;
                                //MessageBox.Show("Summary of carrier is shown here");
                                SetUpSummaryGrid(summaryType: SummaryLevelType.LandingSiteFishCarrier, gridCBL, ls: _landingSite);
                                labelCBL.Content = $"Summary of sampling of fish carrier landings at {_landingSite.ToString()}";

                            }
                            else if (((TreeViewItem)tvItem.Parent).Tag.GetType().Name == "LandingSite")
                            {
                                _landingSite = (LandingSite)((TreeViewItem)tvItem.Parent).Tag;
                                _monthYear = DateTime.Parse(tvItem.Header.ToString());
                                var landings = NSAPEntities.LandingSiteSamplingViewModel.CarrierBoatLandings(_landingSite, (DateTime)_monthYear);
                                _fishingCalendarViewModel = new FishingCalendarViewModel(landings, (DateTime)_monthYear, _landingSite);

                                gridCBL.Columns.Clear();
                                gridCBL.AutoGenerateColumns = true;
                                gridCBL.SelectionUnit = DataGridSelectionUnit.Cell;
                                gridCBL.DataContext = _fishingCalendarViewModel.DataTable;
                                labelCBL.Content = $"Calendar of sampling of fish carrier landings at {_landingSite.ToString()} on {((DateTime)_monthYear).ToString("MMM, yyyy")}";
                            }
                        }
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
            if (_selectedPropertyItem != null)
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
            DateTime? monthEnumerated = null,
            LandingSite ls = null,
            string summaryName = "")
        {
            targetGrid.AutoGenerateColumns = false;
            targetGrid.Columns.Clear();
            targetGrid.DataContext = null;
            targetGrid.SelectionMode = DataGridSelectionMode.Single;
            bool summaryWithSpeciesNoData = false;

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
                    if (string.IsNullOrEmpty(summaryName))
                    {
                        if (treeviewData != null)
                        {
                            targetGrid.DataContext = NSAPEntities.SummaryItemViewModel.SummaryResults;
                        }
                        else
                        {
                            targetGrid.DataContext = await NSAPEntities.SummaryItemViewModel.GetRegionSummaryAsync(region);
                        }
                        targetGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("DBSummary.FMA") });
                        targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("DBSummary.FishingGround.Name") });
                        targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of gear unload", Binding = new Binding("DBSummary.GearUnloadCount"), CellStyle = AlignRightStyle });
                        targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of complete gear unload", Binding = new Binding("DBSummary.CountCompleteGearUnload"), CellStyle = AlignRightStyle });
                        targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of vessel unload", Binding = new Binding("DBSummary.VesselUnloadCount"), CellStyle = AlignRightStyle });
                        targetGrid.Columns.Add(new DataGridTextColumn { Header = "# catch composition included", Binding = new Binding("DBSummary.CountLandingsWithCatchComposition"), CellStyle = AlignRightStyle });
                        targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of tracked  operations", Binding = new Binding("DBSummary.TrackedOperationsCount"), CellStyle = AlignRightStyle });
                        targetGrid.Columns.Add(new DataGridTextColumn { Header = "Earliest date of monitoring", Binding = new Binding("DBSummary.FirstLandingFormattedDate") });
                        targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest date of monitoring", Binding = new Binding("DBSummary.LastLandingFormattedDate") });
                        targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest date of downloaded e-forms ", Binding = new Binding("DBSummary.LatestDownloadFormattedDate") });
                    }
                    else
                    {
                        if (summaryName == "femaleMaturityStages")
                        {
                            targetGrid.Tag = summaryName;
                            targetGrid.SelectionMode = DataGridSelectionMode.Extended;
                            targetGrid.DataContext = CatchMaturityRepository.GetSummaryOfFemaleMaturity(region)
                                .OrderBy(t => t.FMA.Name)
                                .ThenBy(t => t.FishingGround.Name)
                                .ThenBy(t => t.LandingSite.LandingSiteName)
                                .ThenBy(t => t.Gear.GearName)
                                .ThenBy(t => t.SampledMonth)
                                .ThenBy(t => t.Taxa)
                                .ThenBy(t => t.SpeciesName)
                                .ThenBy(t => t.Stage)
                                .ToList();
                            targetGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("FMA") });
                            targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("FishingGround") });
                            targetGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("LandingSite") });
                            targetGrid.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("Gear") });
                            //targetGrid.Columns.Add(new DataGridTextColumn { Header = "Month-Year", Binding = new Binding("SampledMonth") });

                            var col = new DataGridTextColumn()
                            {
                                Binding = new Binding("SampledMonth"),
                                Header = "Month-Year",
                                CellStyle = AlignRightStyle
                            };
                            col.Binding.StringFormat = "MMM-yyyy";
                            targetGrid.Columns.Add(col);

                            targetGrid.Columns.Add(new DataGridTextColumn { Header = "Taxa", Binding = new Binding("Taxa") });
                            targetGrid.Columns.Add(new DataGridTextColumn { Header = "Species name", Binding = new Binding("SpeciesName") });
                            targetGrid.Columns.Add(new DataGridTextColumn { Header = "Maturity stage", Binding = new Binding("MaturityStage") });
                            targetGrid.Columns.Add(new DataGridTextColumn { Header = "Number of individuals", Binding = new Binding("Count") });

                            MonthSubLabel.Visibility = Visibility.Visible;
                            MonthSubLabel.Content = $"Summary table of maturity stages of females from measured individuals taken from sampled landings in {region}";
                        }
                    }
                    break;
                case SummaryLevelType.FMA:
                    if (treeviewData != null)
                    {
                        targetGrid.DataContext = NSAPEntities.SummaryItemViewModel.SummaryResults;
                    }
                    else
                    {

                        targetGrid.DataContext = await NSAPEntities.SummaryItemViewModel.GetFMASummaryAsync(region, fma);
                    }

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
                        targetGrid.DataContext = await NSAPEntities.SummaryItemViewModel.GetRegionFishingGroundSummaryAsync(region, fg, fma);
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
                    if (summaryName == "landingSiteFemaleMeasurementCounts")
                    {
                        List<LandingSiteMeasurementFemaleMaturity> lsmfms = NSAPEntities.LandingSiteSamplingViewModel.GetLandingSitetFemaleMaturityStageCounts(treeviewData);
                        targetGrid.DataContext = lsmfms;

                        var col = new DataGridTextColumn()
                        {
                            Binding = new Binding("MonthSampled"),
                            Header = "Month-Year",
                            CellStyle = AlignRightStyle
                        };
                        col.Binding.StringFormat = "MMM-yyyy";
                        targetGrid.Columns.Add(col);

                        targetGrid.Columns.Add(new DataGridTextColumn { Header = "Taxa", Binding = new Binding("TaxaName") });
                        targetGrid.Columns.Add(new DataGridTextColumn { Header = "Species", Binding = new Binding("SpeciesName") });
                        targetGrid.Columns.Add(new DataGridTextColumn { Header = "# premature", Binding = new Binding("CountStagePremature") });
                        targetGrid.Columns.Add(new DataGridTextColumn { Header = "# immature", Binding = new Binding("CountStageImmature") });
                        targetGrid.Columns.Add(new DataGridTextColumn { Header = "# developing", Binding = new Binding("CountStageDeveloping") });
                        targetGrid.Columns.Add(new DataGridTextColumn { Header = "# ripening", Binding = new Binding("CountStageRipenening") });
                        targetGrid.Columns.Add(new DataGridTextColumn { Header = "# spawning", Binding = new Binding("CountStageSpawning") });
                        targetGrid.Columns.Add(new DataGridTextColumn { Header = "# spent", Binding = new Binding("CountStageSpent") });

                        labelRowCount.Visibility = Visibility.Collapsed;
                        MonthLabel.Content = "Summary for landing site";
                        MonthSubLabel.Visibility = Visibility.Visible;
                        MonthSubLabel.Content = $"Summary table of number of females that were measured for maturity stage from sampled landings in {treeviewData.LandingSite}";

                        if (lsmfms.Count == 0)
                        {
                            summaryWithSpeciesNoData = true;
                            //TimedMessageBox.Show("There is no data for the summary", Global.MessageBoxCaption, 2500, System.Windows.Forms.MessageBoxButtons.OK);
                        }

                    }
                    else if (summaryName == "landingSiteMeasurementCounts")
                    {
                        List<LandingSiteMeasurements> lsms = NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteMeasurements(treeviewData);
                        targetGrid.DataContext = lsms;
                        var col = new DataGridTextColumn()
                        {
                            Binding = new Binding("MonthSampled"),
                            Header = "Month-Year",
                            CellStyle = AlignRightStyle
                        };
                        col.Binding.StringFormat = "MMM-yyyy";
                        targetGrid.Columns.Add(col);
                        targetGrid.Columns.Add(new DataGridTextColumn { Header = "Taxa", Binding = new Binding("TaxaName") });
                        targetGrid.Columns.Add(new DataGridTextColumn { Header = "Species", Binding = new Binding("Species") });
                        targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of length measurements", Binding = new Binding("CountLength"), CellStyle = AlignRightStyle });
                        targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of length-freq measurements", Binding = new Binding("CountLenFreq"), CellStyle = AlignRightStyle });
                        targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of length-weight measurements", Binding = new Binding("CountLenWt"), CellStyle = AlignRightStyle });
                        targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of maturity measurements", Binding = new Binding("CountMat"), CellStyle = AlignRightStyle });


                        labelRowCount.Visibility = Visibility.Collapsed;
                        MonthLabel.Content = "Summary for landing site";
                        MonthSubLabel.Visibility = Visibility.Visible;
                        MonthSubLabel.Content = $"Summary table of number of individuals that were measured from sampled landings in {treeviewData.LandingSite}";


                        if (lsms.Count == 0)
                        {
                            summaryWithSpeciesNoData = true;
                            //TimedMessageBox.Show("There is no data for the summary", Global.MessageBoxCaption, 2500, System.Windows.Forms.MessageBoxButtons.OK);
                        }

                    }
                    else
                    {
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

                    }
                    break;
                case SummaryLevelType.LandingSiteMonth:
                    if (!string.IsNullOrEmpty(summaryName))
                    {
                        if (summaryName == "landingSiteMonthMeasurementCounts")
                        {
                            targetGrid.SelectionMode = DataGridSelectionMode.Extended;
                            List<SpeciesMeasurementCounts> smcs = VesselCatchRepository.GetSpeciesMeasurementCounts(treeviewData);
                            targetGrid.DataContext = smcs;
                            targetGrid.Columns.Add(new DataGridTextColumn { Header = "Taxa", Binding = new Binding("Taxa") });
                            targetGrid.Columns.Add(new DataGridTextColumn { Header = "Species", Binding = new Binding("SpeciesName") });
                            targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of length measurements", Binding = new Binding("CountLenMeasurements"), CellStyle = AlignRightStyle });
                            targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of length-weight measurements", Binding = new Binding("CountLWMeasurements"), CellStyle = AlignRightStyle });
                            targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of length frequency measurements", Binding = new Binding("CountLFMeasurements"), CellStyle = AlignRightStyle });
                            targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of maturity measurements", Binding = new Binding("CountMatMeasurements"), CellStyle = AlignRightStyle });

                            labelRowCount.Visibility = Visibility.Collapsed;
                            MonthLabel.Content = "Summary for landing site";
                            MonthSubLabel.Visibility = Visibility.Visible;
                            MonthSubLabel.Content = $"Summary table of number of individuals that were measured from sampled landings in {treeviewData.LandingSite} on {((DateTime)treeviewData.MonthSampled).ToString("MMM, yyyy")}";



                            if (smcs.Count == 0)
                            {
                                summaryWithSpeciesNoData = true;
                                //TimedMessageBox.Show("There is no data for the summary", Global.MessageBoxCaption, 2500, System.Windows.Forms.MessageBoxButtons.OK);
                            }

                        }
                    }
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
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("DBSummary.FMA.Name") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("DBSummary.FishingGround.Name") });
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

                    targetGrid.DataContext = await NSAPEntities.SummaryItemViewModel.GetEnumeratorSummaryByMonthAsync(region, en, (DateTime)monthEnumerated);
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("DBSummary.FMA.Name") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("DBSummary.FishingGround.Name") });
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
                case SummaryLevelType.LandingSiteFishCarrier:
                    targetGrid.DataContext = NSAPEntities.LandingSiteSamplingViewModel.GetCarrierLandingsSummary(ls: ls);
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Month", Binding = new Binding("DBSummary.SampledMonthString") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Number of carriers", Binding = new Binding("DBSummary.CountCarrierBoats") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Number of vessels sampled", Binding = new Binding("DBSummary.CountCarrierSamplings") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Date first sampling", Binding = new Binding("DBSummary.FirstLandingFormattedDate") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Date last sampling", Binding = new Binding("DBSummary.LastLandingFormattedDate") });
                    break;
            }

            if (summaryWithSpeciesNoData)
            {
                TimedMessageBox.Show(text: "There is no data for the summary\r\nCheck if NSAP Region has watched species list",
                                    caption: Global.MessageBoxCaption,
                                    timeout: 5000,
                                    buttons: System.Windows.Forms.MessageBoxButtons.OK
                                    );
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
                    labelContent = $"Summary of selected FMA: {region.Name}, {fma.Name}";
                    SetUpSummaryGrid(SummaryLevelType.FMA, dataGridSummary, region: region, fma: fma);
                    break;
                case SummaryLevelType.Region:
                    labelContent = $"Summary of selected region: {region.Name}";
                    SetUpSummaryGrid(SummaryLevelType.Region, dataGridSummary, region: region);
                    break;
                case SummaryLevelType.FishingGround:
                    labelContent = $"Summary of selected fishing ground: {fg.Name}, {region}";
                    SetUpSummaryGrid(SummaryLevelType.FishingGround, dataGridSummary, region: region, fma: fma, fg: fg);
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
                        region: region,
                        en: en,
                        enumeratorName: enumeratorName,
                        monthEnumerated: monthEnumerated);
                    break;
            }
            labelSummary.Content = labelContent;
            dataGridSummary.Visibility = Visibility.Visible;
            panelOpening.Visibility = Visibility.Visible;
        }

        private async void ProcessSummaryTreeSelection(TreeViewItem tvItem)
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
                        case "Koboserver":
                            rowSummaryDataGrid.Height = new GridLength(0);
                            rowOverallSummary.Height = new GridLength(1, GridUnitType.Star);
                            await ShowServerMonthlySummary((Koboserver)tvItem.Tag);
                            break;
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
                        case "NSAPRegionFMA":
                            //var nrf = (NSAPRegionFMA)((TreeViewItem)tvItem.Parent).Tag;
                            var nrf = (NSAPRegionFMA)tvItem.Tag;
                            ShowSummaryAtLevel(summaryType: SummaryLevelType.FMA, region: nrf.NSAPRegion, fma: nrf.FMA);
                            break;
                        case "FishingGround":
                        case "NSAPRegionFMAFishingGround":
                            //ShowSummaryAtLevel(summaryType: SummaryLevelType.FishingGround, region: (NSAPRegion)((TreeViewItem)tvItem.Parent).Tag, fg: (FishingGround)tvItem.Tag);
                            nrf = (NSAPRegionFMA)((TreeViewItem)tvItem.Parent).Tag;
                            ShowSummaryAtLevel(summaryType: SummaryLevelType.FishingGround, region: nrf.NSAPRegion, fma: nrf.FMA, fg: ((NSAPRegionFMAFishingGround)tvItem.Tag).FishingGround);
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
                                region: (NSAPRegion)((TreeViewItem)((TreeViewItem)tvItem.Parent).Parent).Tag,
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
            MessageBox.Show("This feature is not yet implemented", Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
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
            menuCalendar.Visibility = Visibility.Collapsed;
            CloseTallyWindow();
            bool showStatus = false;
            menuDatabaseSummary.IsChecked = false;
            switch (((Button)sender).Name)
            {
                case "buttonCBL_calendar":
                    DBView = DBView.dbviewCBLCalendar;
                    //ShowCBLCalendar();
                    _currentDisplayMode = DataDisplayMode.CBLSampling;
                    SetDataDisplayMode();
                    break;
                case "buttonGeneratecsv":
                    await GenerateAllCSV();
                    break;
                case "buttonSummary":
                    DBView = DBView.dbviewSummary;
                    //showStatus = true;
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
                    //MapColorsViewerWindow mcvw = new MapColorsViewerWindow();
                    //mcvw.Show();
                    Close();
                    break;
                case "buttonCalendar":
                    DBView = DBView.dbviewCalendar;
                    //showStatus = true;
                    ShowNSAPCalendar();
                    break;
                case "buttonDownloadHistory":
                    DBView = DBView.dbviewDownloadHistory;
                    showStatus = true;
                    _currentDisplayMode = DataDisplayMode.DownloadHistory;
                    ColumnForTreeView.Width = new GridLength(1, GridUnitType.Star);
                    SetDataDisplayMode();
                    break;
            }
            ShowStatusRow(showStatus);
        }

        private void OnTreeMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _selectedRegionInSummary = null;
            _selectFishingGroundInSummary = null;
            ContextMenu cm = new ContextMenu();
            MenuItem m = null;
            switch (((TreeView)sender).Name)
            {
                case "treeViewDownloadHistory":
                    if (((TreeViewItem)((TreeView)sender).SelectedItem).Tag.ToString() == "weights")
                    {
                        m = new MenuItem { Header = "Tally weight validation flags", Name = "menuWeightValidationTally_context" };
                        m.Click += OnMenuClicked;
                        cm.Items.Add(m);
                    }
                    break;
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

        private void GenerateSpeciesContextMenuForInternet(string catchName, string gearName = "")
        {
            ContextMenu cm = new ContextMenu();
            MenuItem m = null;

            CatchNameURLGenerator.CatchName = catchName;

            foreach (var url in CatchNameURLGenerator.URLS)
            {

                m = new MenuItem { Header = $"Read about {catchName} in {url.Key}", Tag = url.Value };
                m.Click += OnMenuClicked;
                cm.Items.Add(m);
                m.Name = "menuSpeciesForInternet";
            }

            if (!string.IsNullOrEmpty(gearName))
            {
                cm.Items.Add(new Separator());
                if (_isWatchedSpeciesCalendar)
                {
                    if (_isMeasuredWatchedSpeciesCalendar)
                    {
                        if (_getFemaleMaturity)
                        {
                            m = new MenuItem { Header = $"Map fishing ground of {catchName} with {_maturityStage.ToLower()} maturity stage measurements landed at {_allSamplingEntitiesEventHandler.LandingSite} on {((DateTime)_monthYear).ToString("MMM, yyyy")}", Name = "menuCalendarSpeciesMapping", Tag = $"maturity:{_maturityStage}" };
                        }
                        else
                        {
                            m = new MenuItem { Header = $"Map fishing ground of {catchName} with {_species_measurement_type} measurements landed at {_allSamplingEntitiesEventHandler.LandingSite} on {((DateTime)_monthYear).ToString("MMM, yyyy")}", Name = "menuCalendarSpeciesMapping", Tag = $"measured:{_species_measurement_type}" };
                        }
                    }
                    else
                    {
                        m = new MenuItem { Header = $"Map fishing ground of {catchName} landed at {_allSamplingEntitiesEventHandler.LandingSite} on {((DateTime)_monthYear).ToString("MMM, yyyy")}", Name = "menuCalendarSpeciesMapping", Tag = "occurence" };
                    }
                }
                m.Click += OnMenuClicked;
                cm.Items.Add(m);

            }


            if (cm.Items.Count > 0)
            {
                cm.IsOpen = true;
            }
        }
        private void OnGridGotFocus(object sender, RoutedEventArgs e)
        {
            _dataGrid = (DataGrid)sender;
            //Title = _dataGrid.Name;
        }

        private void OnMenuRightClick(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = new ContextMenu();
            MenuItem m = null;

            if (sender.GetType().Name == "TreeView")
            {
                TreeView tv = (TreeView)sender;
                switch (tv.Name)
                {

                    case "treeCBL":
                        TreeViewItem tvi = (TreeViewItem)tv.SelectedItem;
                        DateTime d;
                        if (DateTime.TryParse(tvi.Header.ToString(), out d) && tvi.Tag.GetType().Name == "LandingSite")
                        {
                            m = new MenuItem { Header = $"Crosstab carrier landings for {tvi.Header.ToString()}", Name = "menuCrossTabCBL_Month" };
                            m.Click += OnMenuClicked;
                            cm.Items.Add(m);
                        }
                        else
                        {
                            return;
                        }
                        break;

                }
            }
            else
            {
                _dataGrid = (DataGrid)sender;

                m = new MenuItem { Header = "Copy text", Name = "menuCopyText" };
                m.Click += OnMenuClicked;
                cm.Items.Add(m);

                switch (_dataGrid.Name)
                {
                    case "GridNSAPData":
                        switch (_calendarTreeSelectedEntity)
                        {
                            case "tv_LandingSiteViewModel":
                            case "tv_MonthViewModel":
                                if (_monthYear != null)
                                {

                                    DateTime monthSampled = (DateTime)_monthYear;
                                    int no_days = DateTime.DaysInMonth(monthSampled.Year, monthSampled.Month);
                                    DateTime endOfMonth = monthSampled.AddDays(no_days - 1);

                                    m = new MenuItem { Header = "Weights and weight validation", Name = "menuWeights" };
                                    m.Click += OnMenuClicked;
                                    cm.Items.Add(m);

                                    if (_calendarTreeSelectedEntity == "tv_MonthViewModel")
                                    {
                                        m.IsEnabled = false;
                                        bool proceed = false;

                                        if (Global.IsMapComponentRegistered)
                                        {
                                            switch (_calendarGridColumnName)
                                            {
                                                case "Sector":
                                                    if (_isWatchedSpeciesCalendar)
                                                    {
                                                        if (!_isMeasuredWatchedSpeciesCalendar)
                                                        {
                                                            m = new MenuItem { Header = $"Map fishing ground of {_fish_sector.ToLower()} fishing operations catching {_speciesName} landed at {_allSamplingEntitiesEventHandler.LandingSite} at {((DateTime)monthSampled).ToString("MMM, yyyy")}", Name = "menuCalendarGearSpeciesMapping", Tag = "sector_month" };
                                                            m.Click += OnMenuClicked;
                                                            cm.Items.Add(m);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        m = new MenuItem { Header = $"Map fishing ground of {_fish_sector.ToLower()} fishing operations landed at {_allSamplingEntitiesEventHandler.LandingSite} on {((DateTime)_monthYear).ToString("MMM, yyyy")}", Name = "menuCalendarGearSpeciesMapping", Tag = "sector_month_allspecies" };
                                                        m.Click += OnMenuClicked;
                                                        cm.Items.Add(m);
                                                    }
                                                    break;
                                                case "Species":
                                                    GenerateSpeciesContextMenuForInternet(_speciesName, _gearName);
                                                    return;

                                                case "Maturity stage":
                                                    //m.IsEnabled = true;
                                                    //m = new MenuItem { Header = $"Map fishing ground of {_speciesName} with {_maturityStage.ToLower()} maturity stage", Name = "menuCalendarGearSpeciesMaturityMapping" };
                                                    //m.Click += OnMenuClicked;
                                                    //cm.Items.Add(m);
                                                    break;
                                                case "Gear name":
                                                    if (_gearName != "All gears")
                                                    {
                                                        if (_isWatchedSpeciesCalendar)
                                                        {
                                                            m.IsEnabled = true;
                                                            m = new MenuItem { Header = $"Map fishing ground of {_gearName} ({_fish_sector}) catching {_speciesName} landed at {_allSamplingEntitiesEventHandler.LandingSite} on {((DateTime)monthSampled).ToString("MMMM, yyyy")}", Name = "menuCalendarGearSpeciesMapping", Tag = "occurence" };

                                                            if (_isMeasuredWatchedSpeciesCalendar)
                                                            {
                                                                if (_getFemaleMaturity)
                                                                {
                                                                    m = new MenuItem { Header = $"Map fishing ground of {_gearName} ({_fish_sector}) catching {_speciesName} with {_maturityStage.ToLower()} maturity stage landed at {_allSamplingEntitiesEventHandler.LandingSite} on {((DateTime)_monthYear).ToString("MMM, yyyy")}", Name = "menuCalendarGearSpeciesMapping", Tag = $"maturity:{_maturityStage}" };
                                                                }
                                                                else
                                                                {
                                                                    m = new MenuItem { Header = $"Map fishing ground of {_gearName} ({_fish_sector}) catching {_speciesName} with {_species_measurement_type} measurement landed at {_allSamplingEntitiesEventHandler.LandingSite} on {((DateTime)monthSampled).ToString("MMM, yyyy")}", Name = "menuCalendarGearSpeciesMapping", Tag = $"measured:{_species_measurement_type}" };
                                                                }
                                                            }
                                                            m.Click += OnMenuClicked;
                                                            cm.Items.Add(m);
                                                        }
                                                        else
                                                        {
                                                            m.IsEnabled = true;
                                                            m.Header += $" for {_gearName} ({_fish_sector})";


                                                            m = new MenuItem { Header = $"Map fishing ground of {_gearName} ({_fish_sector}) landed at {_allSamplingEntitiesEventHandler.LandingSite} on {((DateTime)_monthYear).ToString("MMM, yyyy")}", Name = "menuCalendarGearMapping" };
                                                            m.Click += OnMenuClicked;
                                                            cm.Items.Add(m);
                                                        }
                                                    }
                                                    break;
                                                default:
                                                    if (_calendarDayHasValue)
                                                    {
                                                        proceed = int.TryParse(_calendarDayValue, out int v);
                                                        if (!proceed)
                                                        {
                                                            proceed = double.TryParse(_calendarDayValue, out double d);
                                                        }
                                                    }
                                                    if (proceed && _gearName != "All gears")
                                                    {
                                                        m = null;
                                                        string samplingDate = $"{((DateTime)_monthYear).ToString("MMMM")} {_calendarDay}, {((DateTime)_monthYear).ToString("yyyy")}";
                                                        if (_isWatchedSpeciesCalendar)
                                                        {
                                                            if (_isMeasuredWatchedSpeciesCalendar)
                                                            {
                                                                if (_getFemaleMaturity)
                                                                {
                                                                    m = new MenuItem { Header = $"Map fishing ground of {_gearName} ({_fish_sector}) catching {_speciesName} with {_maturityStage.ToLower()} maturity stage landed at {_allSamplingEntitiesEventHandler.LandingSite} on {samplingDate}", Name = "menuCalendarDaySpeciesGearMapping", Tag = $"maturity:{_maturityStage}:by_day_species" };
                                                                }
                                                                else
                                                                {
                                                                    m = new MenuItem { Header = $"Map fishing ground of {_gearName} ({_fish_sector}) catching {_speciesName} measured for {_species_measurement_type} landed at {_allSamplingEntitiesEventHandler.LandingSite} on {samplingDate}", Name = "menuCalendarDaySpeciesGearMapping", Tag = $"measured:{ _species_measurement_type}:by_day_species" };

                                                                }
                                                            }
                                                            else
                                                            {
                                                                m = new MenuItem { Header = $"Map fishing ground of {_gearName} ({_fish_sector}) catching {_speciesName} landed at {_allSamplingEntitiesEventHandler.LandingSite} on {samplingDate}", Name = "menuCalendarDaySpeciesGearMapping", Tag = "gear_day_species" };
                                                            }

                                                        }
                                                        else
                                                        {
                                                            m = new MenuItem { Header = $"Map fishing ground of {_gearName} ({_fish_sector}) landed at {_allSamplingEntitiesEventHandler.LandingSite} on {samplingDate}", Name = "menuCalendarDaySpeciesGearMapping", Tag = "gear_day" };
                                                        }
                                                        if (m != null)
                                                        {
                                                            m.Click += OnMenuClicked;
                                                            cm.Items.Add(m);
                                                        }




                                                        if (_isWatchedSpeciesCalendar)
                                                        {
                                                            if (_isMeasuredWatchedSpeciesCalendar)
                                                            {
                                                                if (_getFemaleMaturity)
                                                                {
                                                                    m = new MenuItem { Header = $"Map fishing ground of {_speciesName} with {_maturityStage.ToLower()} maturity stage landed at {_allSamplingEntitiesEventHandler.LandingSite} on {samplingDate}", Name = "menuCalendarDaySpeciesGearMapping", Tag = $"maturity:{_maturityStage}:by_day_species" };
                                                                }
                                                                else
                                                                {
                                                                    m = new MenuItem { Header = $"Map fishing ground of {_speciesName} measured for {_species_measurement_type} landed at {_allSamplingEntitiesEventHandler.LandingSite} on {samplingDate}", Name = "menuCalendarDaySpeciesGearMapping", Tag = $"measured:{_species_measurement_type}:by_day_species" };
                                                                }
                                                            }
                                                            else
                                                            {
                                                                m = new MenuItem { Header = $"Map fishing ground of {_speciesName} landed at {_allSamplingEntitiesEventHandler.LandingSite} on {samplingDate}", Name = "menuCalendarDaySpeciesGearMapping", Tag = "by_day_species" };
                                                            }
                                                        }
                                                        else
                                                        {
                                                            m = new MenuItem { Header = $"Map fishing ground of operations landing at {_allSamplingEntitiesEventHandler.LandingSite} on {samplingDate}", Name = "menuCalendarDaySpeciesGearMapping", Tag = "by_day" };
                                                        }

                                                        m.Click += OnMenuClicked;
                                                        cm.Items.Add(m);


                                                        if (_isWatchedSpeciesCalendar)
                                                        {
                                                            if (_isMeasuredWatchedSpeciesCalendar)
                                                            {
                                                                if (_getFemaleMaturity)
                                                                {
                                                                    m = new MenuItem { Header = $"Map fishing ground of {_fish_sector.ToLower()} fishing operations catching {_speciesName} with {_maturityStage.ToLower()} maturity stage landed at {_allSamplingEntitiesEventHandler.LandingSite} on {samplingDate}", Name = "menuCalendarDaySpeciesGearMapping", Tag = $"maturity:{_maturityStage}:by_sector" };
                                                                }
                                                                else
                                                                {
                                                                    m = new MenuItem { Header = $"Map fishing ground of {_fish_sector.ToLower()} fishing operations catching {_speciesName} measured for {_species_measurement_type} landed at {_allSamplingEntitiesEventHandler.LandingSite} on {samplingDate}", Name = "menuCalendarDaySpeciesGearMapping", Tag = $"measured:{_species_measurement_type}:by_sector" };
                                                                }
                                                            }
                                                            else
                                                            {
                                                                m = new MenuItem { Header = $"Map fishing ground of {_fish_sector.ToLower()} fishing operations catching {_speciesName} landed at {_allSamplingEntitiesEventHandler.LandingSite} on {samplingDate}", Name = "menuCalendarDaySpeciesGearMapping", Tag = "by_day_sector" };

                                                            }
                                                        }
                                                        else
                                                        {
                                                            m = new MenuItem { Header = $"Map fishing ground of {_fish_sector.ToLower()} fishing operations landing at {_allSamplingEntitiesEventHandler.LandingSite} on {samplingDate}", Name = "menuCalendarDaySpeciesGearMapping", Tag = "by_Sector" };
                                                        }
                                                        m.Click += OnMenuClicked;
                                                        cm.Items.Add(m);
                                                    }
                                                    break;
                                            }
                                        }


                                        //if (_gridCol == 1 && _isWatchedSpeciesCalendar)
                                        //{
                                        //    GenerateSpeciesContextMenuForInternet(_speciesName);
                                        //    return;
                                        //}


                                    }
                                    else
                                    {
                                        m.Header += $" for landings sampled on {((DateTime)_monthYear).ToString("MMMM, yyyy")}";
                                    }



                                    if (_calendarTreeSelectedEntity == "tv_LandingSiteViewModel")
                                    {
                                        m = new MenuItem { Header = "List samplings and catch composition count", Name = "menuListSamplingAndCatchComposition" };
                                        m.Click += OnMenuClicked;
                                        cm.Items.Add(m);

                                        //if (_calendarTreeSelectedEntity == "tv_MonthViewModel")
                                        //{
                                        //    m.IsEnabled = false;
                                        //    if (_gridCol == 0)
                                        //    {
                                        //        m.IsEnabled = true;
                                        //        m.Header += $" for {_gearName} ({_fish_sector})";
                                        //    }

                                        //}
                                        //else
                                        //{
                                        m.Header += $" for landings sampled on {((DateTime)_monthYear).ToString("MMMM, yyyy")}";
                                        //}
                                    }
                                }
                                break;
                            default:
                                //ignore for now
                                break;
                        }
                        break;
                }


                if (_nsapEntity == NSAPEntity.NSAPRegion)
                {
                    m = new MenuItem { Header = "Landing sites", Name = "menuRegionLandingSites" };
                    m.Click += OnMenuClicked;
                    cm.Items.Add(m);
                }
                else if (DBView == DBView.dbviewSummary && _nsapEntity == NSAPEntity.DBSummary)
                {
                    if (_summaryLevelType == SummaryLevelType.FishingGround)
                    {
                        m = new MenuItem { Header = "Move to another fishing ground", Name = "menuMoveToFishingGround" };
                        m.Click += OnMenuClicked;
                        cm.Items.Add(m);
                    }
                }
            }

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

        private void OnTextBlockMouseUp(object sender, MouseButtonEventArgs e)
        {
            ShowImportWindow(openLogInWindow: true);
        }


    }
}