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
        private GearUnloadWindow _gearUnloadWindow;
        private Dictionary<DateTime, List<VesselUnload>> _vesselDownloadHistory;
        private DataDisplayMode _currentDisplayMode;
        private VesselUnloadWIndow _vesselUnloadWindow;
        private List<GearUnload> _gearUnloadList;
        private bool _saveChangesToGearUnload;
        private PropertyItem _selectedPropertyItem;
        private TreeViewItem _selectedTreeNode;
        private TreeViewModelControl.AllSamplingEntitiesEventHandler _allSamplingEntitiesEventHandler;
        private bool _acceptDataGridCellClick;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
            Closing += OnWindowClosing;
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
        }


        private bool ShowSplash()
        {
            SplashWindow sw = new SplashWindow();
            sw.Owner = this;
            if ((bool)sw.ShowDialog())
            {
                return (bool)sw.DialogResult;
            }
            return false;
        }

        private void ShowSummary(string level)
        {
            rowOpening.Height = new GridLength(1, GridUnitType.Star);


            switch (level)
            {
                case "Overall":

                    NSAPEntities.DBSummary.Refresh();
                    labelSummary.Content = "Overall summary of database content";
                    propertyGridSummary.SelectedObject = NSAPEntities.DBSummary;
                    propertyGridSummary.NameColumnWidth = 350;
                    propertyGridSummary.AutoGenerateProperties = false;

                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Database", Name = "DBPath", Description = "Path to database", DisplayOrder = 1, Category = "Database" });
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
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of vessel unload", Name = "VesselUnloadCount", Description = "Number of vessel unload", DisplayOrder = 15, Category = "Submitted fish landing data" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Number of tracked operations", Name = "TrackedOperationsCount", Description = "Number of tracked fishing operations", DisplayOrder = 16, Category = "Submitted fish landing data" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Date of first sampled landing", Name = "FirstSampledLandingDate", Description = "Date of first sampled operation", DisplayOrder = 17, Category = "Submitted fish landing data" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Date of last sampled landing", Name = "LastSampledLandingDate", Description = "Date of last sampled operation", DisplayOrder = 18, Category = "Submitted fish landing data" });
                    propertyGridSummary.PropertyDefinitions.Add(new PropertyDefinition { DisplayName = "Date of latest download", Name = "DateLastDownload", Description = "Date of latet download", DisplayOrder = 19, Category = "Submitted fish landing data" });
                    break;
                case "Enumerators":
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

                    break;
            }
            propertyGridSummary.Visibility = Visibility.Visible;


        }


        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (Global.AppProceed)
            {
                _currentDisplayMode = DataDisplayMode.Dashboard;
                SetDataDisplayMode();

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
        }

        private void ShowStatusRow()
        {
            rowStatus.Height = new GridLength(30, GridUnitType.Pixel);
        }
        private void ShowTitleAndStatusRow()
        {
            rowTopLabel.Height = new GridLength(30, GridUnitType.Pixel);
            ShowStatusRow();
            PanelButtons.Visibility = Visibility.Visible;
        }
        private void ResetDisplay()
        {
            rowDashboard.Height = new GridLength(0);
            rowTopLabel.Height = new GridLength(0);
            rowSpecies.Height = new GridLength(0);
            rowODKData.Height = new GridLength(0);
            rowOthers.Height = new GridLength(0);
            rowOpening.Height = new GridLength(0);
            rowStatus.Height = new GridLength(0);

            StackPanelDashboard.Visibility = Visibility.Collapsed;
            PanelButtons.Visibility = Visibility.Collapsed;
            GridNSAPData.Visibility = Visibility.Collapsed;
            PropertyGrid.Visibility = Visibility.Collapsed;
            gridCalendarHeader.Visibility = Visibility.Collapsed;
            samplingTree.Visibility = Visibility.Collapsed;
            treeViewDownloadHistory.Visibility = Visibility.Collapsed;



        }


        private void SetDataDisplayMode()
        {
            ResetDisplay();
            switch (_currentDisplayMode)
            {
                case DataDisplayMode.DownloadHistory:
                    rowODKData.Height = new GridLength(1, GridUnitType.Star);
                    treeViewDownloadHistory.Visibility = Visibility.Visible;
                    RefreshDownloadHistory();

                    treeViewDownloadHistory.Items.Clear();
                    foreach (var item in _vesselDownloadHistory.Keys)
                    {
                        int itemNo = treeViewDownloadHistory.Items.Add(new TreeViewItem { Header = item.ToString("MMM-dd-yyyy") });
                        var tvItem = (TreeViewItem)treeViewDownloadHistory.Items[itemNo];
                        tvItem.Tag = "downloadDate";
                        tvItem.Items.Add(new TreeViewItem { Header = "Tracked operation", Tag = "tracked" });
                        tvItem.Items.Add(new TreeViewItem { Header = "Gear unload", Tag = "gearUnload" });
                    }
                    if (treeViewDownloadHistory.Items.Count > 0)
                    {
                        ((TreeViewItem)treeViewDownloadHistory.Items[0]).IsSelected = true;
                        ((TreeViewItem)treeViewDownloadHistory.Items[0]).IsExpanded = true;
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


                    TreeViewItem tvi = new TreeViewItem { Header = "All regions", Tag = "EnumeratorAllRegions" };
                    tvi.Expanded += OnSuumaryTreeItemExpanded;
                    summaryTreeNodeEnumerators.Items.Add(tvi);

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
                            foreach (var enumerator in NSAPEntities.NSAPRegionViewModel.GetEnumeratorsInRegion((NSAPRegion)node.Tag))
                            {
                                TreeViewItem enumeratorNode = new TreeViewItem { Header = enumerator.Name, Tag = enumerator };
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
                    }
                }
            }
        }

        private void ShowDatabaseNotFoundView()
        {
            rowTopLabel.Height = new GridLength(300);
            labelTitle.Content = "Backend database file not found\r\nMake sure that the correct database is found in the application folder\r\n" +
                                  "The application folder is the folder where you installed this software";
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
            buttonEdit.IsEnabled = true;

            if (_nsapEntity != NSAPEntity.FMA && _nsapEntity != NSAPEntity.NSAPRegion)
                buttonDelete.IsEnabled = true;
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
                    dataGridEntities.ItemsSource = NSAPEntities.LandingSiteViewModel.LandingSiteCollection.OrderBy(t => t.Municipality.Province.ProvinceName).ThenBy(t => t.Municipality.MunicipalityName).ThenBy(t => t.LandingSiteName);
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
                                                "Delete the fishing ground first in the list of fishing grounds in a FMA";
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
                    switch (_nsapEntity)
                    {
                        case NSAPEntity.GPS:
                            NSAPEntities.GPSViewModel.DeleteRecordFromRepo(((GPS)dataGridEntities.SelectedItem).Code);
                            break;
                        case NSAPEntity.FishingGround:
                            NSAPEntities.FishingGroundViewModel.DeleteRecordFromRepo(((FishingGround)dataGridEntities.SelectedItem).Code);
                            break;

                        case NSAPEntity.FishingVessel:
                            NSAPEntities.FishingVesselViewModel.DeleteRecordFromRepo(((FishingVessel)dataGridEntities.SelectedItem).ID);
                            break;

                        case NSAPEntity.FishingGear:
                            if (!NSAPEntities.GearViewModel.DeleteRecordFromRepo(((Gear)dataGridEntities.SelectedItem).Code))
                            {
                                return;
                            }
                            break;

                        case NSAPEntity.LandingSite:
                            NSAPEntities.LandingSiteViewModel.DeleteRecordFromRepo(((LandingSite)dataGridEntities.SelectedItem).LandingSiteID);
                            break;

                        case NSAPEntity.Enumerator:
                            NSAPEntities.NSAPEnumeratorViewModel.DeleteRecordFromRepo(((NSAPEnumerator)dataGridEntities.SelectedItem).ID);
                            break;

                        case NSAPEntity.EffortIndicator:
                            var effortSpecToDelete = (EffortSpecification)dataGridEntities.SelectedItem;
                            NSAPEntities.GearViewModel.DeleteEffortSpec(effortSpecToDelete);
                            NSAPEntities.EffortSpecificationViewModel.DeleteRecordFromRepo(effortSpecToDelete.ID);

                            break;
                    }
                    SetDataGridSource();
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



        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                //case "buttonEntitySummary":
                //    break;
                case "buttonOrphan":
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

        private void OnMenuItemChecked(object sender, RoutedEventArgs e)
        {
            buttonImport.Visibility = Visibility.Collapsed;
            buttonOrphan.Visibility = Visibility.Collapsed;
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
                    break;
                case "menuGPS":
                    _nsapEntity = NSAPEntity.GPS;
                    textOfTitle = "List of GPS units";
                    break;
                case "menuProvinces":
                    _nsapEntity = NSAPEntity.Province;
                    textOfTitle = "List of Provinces";
                    break;

                case "menuEffortIndicators":
                    _nsapEntity = NSAPEntity.EffortIndicator;
                    textOfTitle = "List of fishing effort indicators";
                    contextMenu.Items.Add(new MenuItem { Header = "View gears using this indicator", Name = "menuViewGearsUsingIndicator", Tag = "nsapEntities" });

                    break;

                case "menuFMAs":
                    _nsapEntity = NSAPEntity.FMA;
                    textOfTitle = "List of FMAs";
                    break;

                case "menuNSAPRegions":
                    _nsapEntity = NSAPEntity.NSAPRegion;
                    textOfTitle = "List of NSAP Regions";
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
        private void OnDataGridContextMenu(object sender, RoutedEventArgs e)
        {

            switch (((MenuItem)sender).Tag.ToString())
            {
                case "samplingCalendar":
                    _allSamplingEntitiesEventHandler.GearUsed = _gearName;
                    _allSamplingEntitiesEventHandler.ContextMenuTopic = "contextMenuCrosstabGear";
                    CrossTabManager.GearByMonthYear(_allSamplingEntitiesEventHandler);
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
        private async void OnMenuClicked(object sender, RoutedEventArgs e)
        {
            string fileName = "";
            //labelTitle.Visibility = Visibility.Collapsed;

            string itemName = ((MenuItem)sender).Name;

            if (itemName == "menuDownloadHistory" || itemName == "menuNSAPCalendar")
            {
                UncheckEditMenuItems();
            }

            switch (itemName)
            {
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
                case "menuLocateDatabase":
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
                            }
                            else
                            {
                                ShowDatabaseNotFoundView();
                            }
                        }
                    }
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
                    ExportNSAPToExcel(tracked: true);
                    break;
                case "menuExportExcel":
                    ExportNSAPToExcel();
                    break;
                case "menuNSAPCalendar":
                    ShowNSAPCaklendar();
                    break;
                case "menuImport":
                    ShowImportWindow();
                    break;
                case "menuOptionGenerateCSV":
                    CSVOptionsWindow optionsWindow = new CSVOptionsWindow();
                    optionsWindow.ShowDialog();
                    break;
                case "menuExit":
                    Close();
                    break;
                case "menuQueryAPI":
                    QueryAPIWIndow qaw = new QueryAPIWIndow(this);
                    qaw.ShowDialog();
                    break;

                //case "menuFishingGroundCode":
                //    if (SelectRegions() && GetCSVSaveLocationFromSaveAsDialog(out fileName, LogType.FishingGroundCode_csv))
                //    {
                //        Logger.FilePath = fileName;
                //        MessageBox.Show($"{ await GenerateCSV.GenerateFishingGroundCodeCSV()} items in fishing_ground_code.csv generated", "CSV file created", MessageBoxButton.OK, MessageBoxImage.Information);
                //    }
                //    break;

                //case "menuFMACode":
                //    if (SelectRegions() && GetCSVSaveLocationFromSaveAsDialog(out fileName, LogType.FMACode_csv))
                //    {
                //        Logger.FilePath = fileName;
                //        MessageBox.Show($"{ await GenerateCSV.GenerateFMACodeCSV()} items in fma_code.csv generated", "CSV file created", MessageBoxButton.OK, MessageBoxImage.Information);
                //    }
                //    break;
                case "menuSelectRegions":
                    SelectRegions(resetList: true);
                    break;
                case "menuGenerateAll":
                    await GenerateAllCSV();

                    break;


                case "menuGenerateItemSets":

                    if (SelectRegions() && GetCSVSaveLocationFromSaveAsDialog(out fileName, LogType.ItemSets_csv))
                    {
                        Logger.FilePath = fileName;
                        MessageBox.Show($"{await GenerateCSV.GenerateItemsetsCSV()} items in itemsets.csv generated\r maxrow is {NSAPEntities.GetMaxItemSetID()}", "CSV file created", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
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
        private void ShowNSAPCaklendar()
        {
            if (!_saveChangesToGearUnload &&
                    NSAPEntities.GearUnloadViewModel.CopyOfGearUnloadList != null &&
                    NSAPEntities.GearUnloadViewModel.CopyOfGearUnloadList.Count > 0
                )
            {
                UndoChangesToGearUnload(refresh: false);
            }

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
            string id = "";
            switch (((DataGrid)sender).Name)
            {
                case "GridNSAPData":
                    if (_currentDisplayMode == DataDisplayMode.ODKData)
                    {
                        if (_gearUnload != null && _gearUnloadWindow == null)
                        {
                            _gearUnloadWindow = new GearUnloadWindow(_gearUnload, _treeItemData, this);
                            _gearUnloadWindow.Owner = this;
                            _gearUnloadWindow.Show();
                        }
                        else
                        {

                        }
                    }
                    else if (_currentDisplayMode == DataDisplayMode.DownloadHistory)
                    {
                        if ((VesselUnload)GridNSAPData.SelectedItem != null)
                        {
                            var unload = (VesselUnload)GridNSAPData.SelectedItem;
                            if (_vesselUnloadWindow == null)
                            {

                                NSAPEntities.NSAPRegion = unload.Parent.Parent.NSAPRegion;
                                _vesselUnloadWindow = new VesselUnloadWIndow(unload, this);
                                _vesselUnloadWindow.Owner = this;
                                _vesselUnloadWindow.Show();
                            }
                            else
                            {
                                _vesselUnloadWindow.VesselUnload = unload;
                            }
                        }
                    }

                    break;
                case "dataGridSpecies":
                    var fs = (FishSpecies)dataGridSpecies.SelectedItem;

                    if (fs != null)
                        id = fs.RowNumber.ToString();
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

        public void VesselWindowClosed()
        {
            _vesselUnloadWindow = null;
        }
        private void MakeCalendar(TreeViewModelControl.AllSamplingEntitiesEventHandler e)
        {
            _treeItemData = e;
            var listGearUnload = NSAPEntities.GearUnloadViewModel.GearUnloadCollection
                .Where(t => t.Parent.NSAPRegionID == e.NSAPRegion.Code)
                .Where(t => t.Parent.FMAID == e.FMA.FMAID)
                .Where(t => t.Parent.FishingGroundID == e.FishingGround.Code)
                .Where(t => t.Parent.LandingSiteName == e.LandingSiteText)
                .Where(t => t.Parent.SamplingDate.Year == ((DateTime)e.MonthSampled).Year)
                .Where(t => t.Parent.SamplingDate.Month == ((DateTime)e.MonthSampled).Month)
                .OrderBy(t => t.GearUsedName).ToList();

            _fishingCalendarViewModel = new FishingCalendarViewModel(listGearUnload);
            GridNSAPData.Columns.Clear();
            GridNSAPData.AutoGenerateColumns = true;
            GridNSAPData.DataContext = _fishingCalendarViewModel.DataTable;
        }
        private void OnTreeViewItemSelected(object sender, TreeViewModelControl.AllSamplingEntitiesEventHandler e)
        {
            _acceptDataGridCellClick = false;
            _allSamplingEntitiesEventHandler = e;
            gridCalendarHeader.Visibility = Visibility.Visible;
            GridNSAPData.Visibility = Visibility.Visible;
            //PropertyGrid.Visibility = Visibility.Visible;

            string labelContent = "";
            switch (e.TreeViewEntity)
            {
                case "tv_NSAPRegionViewModel":
                    labelContent = $"Summary of database content for {e.NSAPRegion.Name}";
                    SetUpSummaryGrid(SummaryLevelType.Region, GridNSAPData, e.NSAPRegion);
                    break;
                case "tv_FMAViewModel":
                    labelContent = $"Summary of database content for {e.FMA.Name}, {e.NSAPRegion.Name}";
                    SetUpSummaryGrid(SummaryLevelType.FMA, GridNSAPData, e.NSAPRegion, e.FMA);
                    break;
                case "tv_FishingGroundViewModel":
                    labelContent = $"Summary of database content for {e.FishingGround.Name}, {e.FMA.Name}, {e.NSAPRegion.Name}";
                    SetUpSummaryGrid(SummaryLevelType.FishingGround, GridNSAPData, e.NSAPRegion, e.FMA, e.FishingGround, inSummaryView: false);
                    break;
                case "tv_LandingSiteViewModel":
                    labelContent = $"Summary of database content for {e.LandingSiteText}, {e.FishingGround.Name}, {e.FMA.Name}, {e.NSAPRegion.Name}";
                    SetUpSummaryGrid(SummaryLevelType.LandingSite, GridNSAPData, e.NSAPRegion, e.FMA, e.FishingGround, landingSite: e.LandingSiteText);


                    if (CrossTabReportWindow.Instance != null)
                    {
                        _allSamplingEntitiesEventHandler.ContextMenuTopic = "contextMenuCrosstabLandingSite";
                        CrossTabManager.GearByMonthYear(_allSamplingEntitiesEventHandler);
                        ShowCrossTabWIndow();
                    }
                    break;
                case "tv_MonthViewModel":
                    gridCalendarHeader.Visibility = Visibility.Visible;
                    MonthLabel.Content = $"Fisheries landing sampling calendar for {((DateTime)e.MonthSampled).ToString("MMMM-yyyy")}";
                    MonthSubLabel.Content = $"{e.LandingSiteText}, {e.FishingGround}, {e.FMA}, {e.NSAPRegion}";
                    GridNSAPData.Visibility = Visibility.Visible;
                    GridNSAPData.SelectionUnit = DataGridSelectionUnit.Cell;
                    PropertyGrid.Visibility = Visibility.Collapsed;
                    NSAPEntities.NSAPRegion = e.NSAPRegion;
                    MakeCalendar(e);

                    if (CrossTabReportWindow.Instance != null)
                    {
                        _allSamplingEntitiesEventHandler.ContextMenuTopic = "contextMenuCrosstabMonth";
                        CrossTabManager.GearByMonthYear(_allSamplingEntitiesEventHandler);
                        ShowCrossTabWIndow();
                    }
                    break;
            }


            if (e.TreeViewEntity != "tv_MonthViewModel")
            {
                MonthLabel.Content = labelContent;
                MonthSubLabel.Visibility = Visibility.Collapsed;
                labelSummary.Content = labelContent;
                dataGridSummary.Visibility = Visibility.Visible;
                panelOpening.Visibility = Visibility.Visible;
            }
            else
            {
                MonthSubLabel.Visibility = Visibility.Visible;
                _acceptDataGridCellClick = true;
            }

        }

        private void OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
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
                                CrossTabManager.GearByMonthYear(_allSamplingEntitiesEventHandler);
                                ShowCrossTabWIndow();
                            }
                        }
                        else
                        {
                            GridNSAPData.ContextMenu = null;
                        }

                        _gearUnload = NSAPEntities.GearUnloadViewModel.GearUnloadCollection
                            .Where(t => t.GearUsedName == _gearName)
                            .Where(t => t.Parent.NSAPRegionID == _treeItemData.NSAPRegion.Code)
                            .Where(t => t.Parent.FMAID == _treeItemData.FMA.FMAID)
                            .Where(t => t.Parent.FishingGroundID == _treeItemData.FishingGround.Code)
                            .Where(t => t.Parent.LandingSiteName == _treeItemData.LandingSiteText)
                            .Where(t => t.Parent.SamplingDate.Date == ((DateTime)_treeItemData.MonthSampled).AddDays(_gridCol - 3)).FirstOrDefault();
                    }

                    if (_gearUnloadWindow != null && _gearUnload != null)
                    {
                        _gearUnloadWindow.GearUnload = _gearUnload;
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

        private void OnTreeContextMenu(object sender, TreeViewModelControl.AllSamplingEntitiesEventHandler e)
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

                    CrossTabManager.GearByMonthYear(_allSamplingEntitiesEventHandler);
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

        private void RefreshDownloadedItemsGrid()
        {

            var dt = DateTime.Parse(((TreeViewItem)treeViewDownloadHistory.SelectedItem).Header.ToString());
            GridNSAPData.DataContext = _vesselDownloadHistory[dt];
        }

        private void OnHistoryTreeItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                var tvItem = (TreeViewItem)e.NewValue;

                switch (tvItem.Tag.ToString())
                {
                    case "downloadDate":
                    case "tracked":
                        if (!_saveChangesToGearUnload &&
                            NSAPEntities.GearUnloadViewModel.CopyOfGearUnloadList != null &&
                            NSAPEntities.GearUnloadViewModel.CopyOfGearUnloadList.Count > 0
                            )
                        {
                            UndoChangesToGearUnload(refresh: false);
                        }

                        var dt = DateTime.Now;
                        if (tvItem.Tag.ToString() == "downloadDate")
                        {
                            RefreshDownloadedItemsGrid();
                        }
                        else if (tvItem.Tag.ToString() == "tracked")
                        {
                            dt = DateTime.Parse(((TreeViewItem)tvItem.Parent).Header.ToString()).Date;
                            GridNSAPData.DataContext = _vesselDownloadHistory[dt].Where(t => t.OperationIsTracked == true);
                        }
                        GridNSAPData.AutoGenerateColumns = false;
                        GridNSAPData.Columns.Clear();
                        GridNSAPData.SelectionUnit = DataGridSelectionUnit.FullRow;
                        GridNSAPData.IsReadOnly = true;
                        GridNSAPData.SetValue(Grid.ColumnSpanProperty, 2);
                        GearUnload_ButtonsPanel.Visibility = Visibility.Collapsed;

                        gridCalendarHeader.Visibility = Visibility.Visible;
                        MonthLabel.Content = $"Vessel unload by date of download";
                        MonthSubLabel.Content = $" All items listed were downloaded on {dt.ToString("MMM-dd-yyyy")}";

                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("PK") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("UserName") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Region", Binding = new Binding("Parent.Parent.NSAPRegion.Name") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("Parent.Parent.FMA.Name") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Fishing ground ", Binding = new Binding("Parent.Parent.FishingGround.Name") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Landing site ", Binding = new Binding("Parent.Parent.LandingSiteName") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Gear ", Binding = new Binding("Parent.GearUsedName") });

                        var col = new DataGridTextColumn()
                        {
                            Binding = new Binding("SamplingDate"),
                            Header = "Sampling date"
                        };
                        col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                        GridNSAPData.Columns.Add(col);


                        if (tvItem.Tag.ToString() == "downloadDate")
                        {
                            GridNSAPData.Columns.Add(new DataGridCheckBoxColumn { Header = "Using fishing boat", Binding = new Binding("IsBoatUsed") });
                        }

                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Sector ", Binding = new Binding("Sector") });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel ", Binding = new Binding("VesselName") });

                        if (tvItem.Tag.ToString() == "downloadDate")
                        {

                            GridNSAPData.Columns.Add(new DataGridCheckBoxColumn { Header = "Successful operation ", Binding = new Binding("OperationIsSuccessful") });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Catch weight ", Binding = new Binding("WeightOfCatchText") });
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Catch composition count ", Binding = new Binding("CatchCompositionCountText") });
                        }
                        else if (tvItem.Tag.ToString() == "tracked")
                        {
                            GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "GPS ", Binding = new Binding("GPS.AssignedName") });
                        }

                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Excel download ", Binding = new Binding("FromExcelDownload") });
                        break;
                    case "gearUnload":
                        dt = DateTime.Parse(((TreeViewItem)tvItem.Parent).Header.ToString()).Date;
                        _gearUnloadList = NSAPEntities.GearUnloadViewModel.GetAllGearUnloads(dt);
                        GridNSAPData.DataContext = _gearUnloadList;
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
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Boats", Binding = new Binding("Boats"), IsReadOnly = false });
                        GridNSAPData.Columns.Add(new DataGridTextColumn { Header = "Catch", Binding = new Binding("Catch"), IsReadOnly = false });

                        break;
                }



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

        private void SetUpSummaryGrid(SummaryLevelType summaryType, DataGrid targetGrid, NSAPRegion region = null, FMA fma = null, FishingGround fg = null, string landingSite = null, bool inSummaryView = true)
        {
            targetGrid.AutoGenerateColumns = false;
            targetGrid.Columns.Clear();
            targetGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
            switch (summaryType)
            {
                case SummaryLevelType.AllRegions:
                    NSAPEntities.NSAPRegionViewModel.SetupSummary();
                    var dict = NSAPEntities.NSAPRegionViewModel.RegionSummaryDictionary;
                    var summarySource = from row in dict
                                        select new
                                        {
                                            Region = row.Key.ShortName,
                                            FMACount = row.Value.FMACount,
                                            FishingGroundCount = row.Value.FishingGroundCount,
                                            LandingSiteCount = row.Value.LandingSiteCount,
                                            GearCount = row.Value.FishingGearCount,
                                            EnumeratorCount = row.Value.EnumeratorCount,
                                            FishingVesselCount = row.Value.FishingVesselCount,
                                            GearUnloadCount = row.Value.GearUnloadCount,
                                            GearUnloadCompletedCount = row.Value.CountCompleteGearUnload,
                                            VesselUnloadCount = row.Value.VesselUnloadCount,
                                            TrackedOperationsCount = row.Value.TrackedOperationsCount,
                                            FirstSampling = row.Value.FirstLandingFormattedDate,
                                            LastSampling = row.Value.LastLandingFormattedDate,
                                            LastDownloadDate = row.Value.LatestDownloadFormattedDate
                                        };


                    targetGrid.DataContext = summarySource;

                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Region", Binding = new Binding("Region") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of gear unload", Binding = new Binding("GearUnloadCount") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of complete gear unload", Binding = new Binding("GearUnloadCompletedCount") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "#  of vessel unload", Binding = new Binding("VesselUnloadCount") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of tracked  operations", Binding = new Binding("TrackedOperationsCount") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Earliest date of monitoring", Binding = new Binding("FirstSampling") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest date of monitoring", Binding = new Binding("LastSampling") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest date of downloaded e-forms ", Binding = new Binding("LastDownloadDate") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of FMAs", Binding = new Binding("FMACount") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of fishing grounds", Binding = new Binding("FishingGroundCount") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of landing sites", Binding = new Binding("LandingSiteCount") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of gear types", Binding = new Binding("GearCount") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of enumerators", Binding = new Binding("EnumeratorCount") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of fishing vessels", Binding = new Binding("FishingVesselCount") });




                    break;

                case SummaryLevelType.FMA:
                    NSAPEntities.NSAPRegionViewModel.SetupSummaryForFMA(region, fma);
                    var dictFMA = NSAPEntities.NSAPRegionViewModel.RegionFMASummaryDictionary;
                    var summarySourceFMA = from row in dictFMA
                                           select new
                                           {
                                               FishingGroundName = row.Key.Name,
                                               FishingGround = row.Value.FishingGround,
                                               FMA = row.Value.FMA,
                                               GearUnloadCount = row.Value.GearUnloadCount,
                                               GearUnloadCompletedCount = row.Value.CountCompleteGearUnload,
                                               VesselUnloadCount = row.Value.VesselUnloadCount,
                                               TrackedOperationsCount = row.Value.TrackedOperationsCount,
                                               FirstSampling = row.Value.FirstLandingFormattedDate,
                                               LastSampling = row.Value.LastLandingFormattedDate,
                                               LastDownloadDate = row.Value.LatestDownloadFormattedDate
                                           };

                    targetGrid.DataContext = summarySourceFMA;
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("FMA") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("FishingGroundName") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of gear unload", Binding = new Binding("GearUnloadCount") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of complete gear unload", Binding = new Binding("GearUnloadCompletedCount") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "#  of vessel unload", Binding = new Binding("VesselUnloadCount") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of tracked  operations", Binding = new Binding("TrackedOperationsCount") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Earliest date of monitoring", Binding = new Binding("FirstSampling") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest date of monitoring", Binding = new Binding("LastSampling") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest date of downloaded e-forms ", Binding = new Binding("LastDownloadDate") });
                    break;
                case SummaryLevelType.Region:
                    NSAPEntities.NSAPRegionViewModel.SetupSummaryForRegion(region);

                    var dictFG = NSAPEntities.NSAPRegionViewModel.RegionFishingGroundSummaryDictionary;
                    var summarySourceFG = from row in dictFG
                                          select new
                                          {
                                              FishingGroundName = row.Key.Name,
                                              FishingGround = row.Value.FishingGround,
                                              FMA = row.Value.FMA.Name,
                                              GearUnloadCount = row.Value.GearUnloadCount,
                                              GearUnloadCompletedCount = row.Value.CountCompleteGearUnload,
                                              VesselUnloadCount = row.Value.VesselUnloadCount,
                                              TrackedOperationsCount = row.Value.TrackedOperationsCount,
                                              FirstSampling = row.Value.FirstLandingFormattedDate,
                                              LastSampling = row.Value.LastLandingFormattedDate,
                                              LastDownloadDate = row.Value.LatestDownloadFormattedDate
                                          };

                    targetGrid.DataContext = summarySourceFG;
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("FishingGroundName") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("FMA") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of gear unload", Binding = new Binding("GearUnloadCount") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of complete gear unload", Binding = new Binding("GearUnloadCompletedCount") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "#  of vessel unload", Binding = new Binding("VesselUnloadCount") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of tracked  operations", Binding = new Binding("TrackedOperationsCount") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Earliest date of monitoring", Binding = new Binding("FirstSampling") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest date of monitoring", Binding = new Binding("LastSampling") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest date of downloaded e-forms ", Binding = new Binding("LastDownloadDate") });
                    break;
                case SummaryLevelType.FishingGround:
                    if (inSummaryView)
                    {
                        NSAPEntities.NSAPRegionViewModel.SetupSummaryForFishingGroundAllSites(region, fg, (bool)checkLandingSiteWithLandings.IsChecked);
                    }
                    else
                    {
                        NSAPEntities.NSAPRegionViewModel.SetupSummaryForFishingGround(region, fg, (bool)checkLandingSiteWithLandings.IsChecked);
                    }

                    var dictLS = NSAPEntities.NSAPRegionViewModel.RegionLandingSiteSummaryDictionary;
                    var summarySourceLS = from row in dictLS
                                          select new
                                          {
                                              LandingSiteName = row.Key.ToString(),
                                              FishingGroundName = row.Value.FishingGround.Name,
                                              FMA = row.Value.FMA.Name,
                                              GearUnloadCount = row.Value.GearUnloadCount,
                                              GearUnloadCompletedCount = row.Value.CountCompleteGearUnload,
                                              VesselUnloadCount = row.Value.VesselUnloadCount,
                                              TrackedOperationsCount = row.Value.TrackedOperationsCount,
                                              FirstSampling = row.Value.FirstLandingFormattedDate,
                                              LastSampling = row.Value.LastLandingFormattedDate,
                                              LastDownloadDate = row.Value.LatestDownloadFormattedDate
                                          };

                    targetGrid.DataContext = summarySourceLS;
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("LandingSiteName") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of gear unload", Binding = new Binding("GearUnloadCount") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of complete gear unload", Binding = new Binding("GearUnloadCompletedCount") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "#  of vessel unload", Binding = new Binding("VesselUnloadCount") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of tracked  operations", Binding = new Binding("TrackedOperationsCount") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Earliest date of monitoring", Binding = new Binding("FirstSampling") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest date of monitoring", Binding = new Binding("LastSampling") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest date of downloaded e-forms ", Binding = new Binding("LastDownloadDate") });

                    checkLandingSiteWithLandings.Visibility = Visibility.Visible;

                    break;
                case SummaryLevelType.LandingSite:
                    NSAPEntities.NSAPRegionViewModel.SetUpSummaryForLandingSite(region, fma, fg, landingSite);
                    var dictLSMonth = NSAPEntities.NSAPRegionViewModel.RegionMonthSampledSummaryDictionary;

                    var summarySourceMonth = from row in dictLSMonth
                                             select new
                                             {
                                                 MonthSampled = row.Key.ToString("MMM-yyyy"),
                                                 GearUnloadCount = row.Value.GearUnloadCount,
                                                 GearUnloadCompletedCount = row.Value.CountCompleteGearUnload,
                                                 VesselUnloadCount = row.Value.VesselUnloadCount,
                                                 TrackedOperationsCount = row.Value.TrackedOperationsCount,
                                                 FirstSampling = row.Value.FirstLandingFormattedDate,
                                                 LastSampling = row.Value.LastLandingFormattedDate,
                                                 LastDownloadDate = row.Value.LatestDownloadFormattedDate
                                             };

                    targetGrid.DataContext = summarySourceMonth;
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Month of sampling", Binding = new Binding("MonthSampled") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of gear unload", Binding = new Binding("GearUnloadCount") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of complete gear unload", Binding = new Binding("GearUnloadCompletedCount") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "#  of vessel unload", Binding = new Binding("VesselUnloadCount") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "# of tracked  operations", Binding = new Binding("TrackedOperationsCount") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Earliest date of monitoring", Binding = new Binding("FirstSampling") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest date of monitoring", Binding = new Binding("LastSampling") });
                    targetGrid.Columns.Add(new DataGridTextColumn { Header = "Latest date of downloaded e-forms ", Binding = new Binding("LastDownloadDate") });
                    break;
            }
        }
        public void ShowSummaryAtLevel(SummaryLevelType summaryType, NSAPRegion region = null, FMA fma = null, FishingGround fg = null)
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
            }
            labelSummary.Content = labelContent;
            dataGridSummary.Visibility = Visibility.Visible;
            panelOpening.Visibility = Visibility.Visible;
        }

        private void ProcessSummaryTreeSelection(TreeViewItem tvItem)
        {
            rowSummaryDataGrid.Height = new GridLength(1, GridUnitType.Star);
            rowOverallSummary.Height = new GridLength(0);
            string header = tvItem.Header.ToString();
            switch (header)
            {
                case "Overall":
                    ShowSummary(header);
                    rowSummaryDataGrid.Height = new GridLength(0);
                    rowOverallSummary.Height = new GridLength(1, GridUnitType.Star);
                    break;
                case "Regions":
                    ShowSummaryAtLevel(SummaryLevelType.AllRegions);
                    break;
                case "Enumerators":
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
                                    break;
                                case "Regions":
                                    ShowSummaryAtLevel(SummaryLevelType.Region, (NSAPRegion)tvItem.Tag);
                                    break;
                            }

                            break;
                        case "FishingGround":
                            ShowSummaryAtLevel(summaryType: SummaryLevelType.FishingGround, region: (NSAPRegion)((TreeViewItem)tvItem.Parent).Tag, fg: (FishingGround)tvItem.Tag);
                            break;
                        case "NSAPEnumerator":
                            ShowEnumeratorSummary((NSAPEnumerator)tvItem.Tag);
                            break;
                    }
                    break;
            }
        }

        private void ShowEnumeratorSummary(NSAPEnumerator enumerator)
        {

        }
        private void OnSummaryTreeItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            labelSummary.Content = "To be implemented";

            //checkLandingSiteWithLandings.Visibility = Visibility.Visible;
            treeViewSummary.Visibility = Visibility.Visible;
            labelSummary.Visibility = Visibility.Visible;

            propertyGridSummary.Visibility = Visibility.Collapsed; ;
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
        private async void OnToolbarButtonClick(object sender, RoutedEventArgs e)
        {
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
                    ShowToBeImplemented();
                    break;
                case "buttonExit":
                    Close();
                    break;
                case "buttonCalendar":
                    ShowNSAPCaklendar();
                    break;
                case "buttonDownloadHistory":
                    _currentDisplayMode = DataDisplayMode.DownloadHistory;
                    ColumnForTreeView.Width = new GridLength(1, GridUnitType.Star);
                    SetDataDisplayMode();
                    break;
            }
        }
    }
}