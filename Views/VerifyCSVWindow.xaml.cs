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
using NSAP_ODK.Entities;
using NSAP_ODK.Entities.Media_csv;
using NSAP_ODK.Utilities;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for VerifyCSVWindow.xaml
    /// </summary>
    public partial class VerifyCSVWindow : Window
    {
        private string _csvPath = "";
        private string _entityName = "";
        private static VerifyCSVWindow _instance;
        private string _selectedProperty;
        private NSAPRegion _currentRegion;
        private List<LSSelect> _lSSelects;
        private List<NSAPRegionFMAFishingGroundLandingSite> _landingSitesInRegion;
        private List<EnumeratorSelect> _enumeratorSelects;
        private List<NSAPRegionEnumerator> _enumeratorsInRegion;
        private List<GearSelect> _gearSelects;
        private List<NSAPRegionGear> _gearsInRegion;
        private List<FishingGroundSelect> _fishingGroundSelects;
        private List<NSAPRegionFMAFishingGround> _fishingGroundsInRegion;
        private List<VesselSelect> _vesselSelects;
        private List<NSAPRegionFishingVessel> _vesselsInRegion;
        private List<FMASelect> _fmaSelects;
        private List<NSAPRegionFMA> _fmasInRegion;
        public static VerifyCSVWindow GetInstance()
        {
            if (_instance == null)
            {
                _instance = new VerifyCSVWindow();
            }
            return _instance;
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }
        public static bool HasInstance()
        {
            return _instance != null;
        }
        public static void CloseInstance()
        {
            _instance.Close();
            _instance = null;

        }
        public VerifyCSVWindow()
        {
            InitializeComponent();
            Closing += VerifyCSVWindow_Closing;
            Loaded += VerifyCSVWindow_Loaded;
            _currentRegion = NSAPEntities.NSAPRegionViewModel.CurrentEntity;
        }

        private void VerifyCSVWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //tviCSV.IsSelected = true;
            dataGrid.Visibility = Visibility.Visible;
            propertyGrid.Visibility = Visibility.Collapsed;
            propertyGrid.NameColumnWidth = 250;
        }

        private void VerifyCSVWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _instance = null;
            Closing -= VerifyCSVWindow_Closing;
            Loaded -= VerifyCSVWindow_Loaded;
        }

        private void FillEntityLists()
        {

            switch (_selectedProperty)
            {
                case "FMAs":
                    _csvPath = $@"{Utilities.Global.CSVMediaSaveFolder}\fma_select.csv";
                    if (NSAPEntities.FMASelectViewModel == null)
                    {
                        NSAPEntities.FMASelectViewModel = new FMASelectViewModel(_csvPath);
                    }
                    _fmaSelects = new List<FMASelect>();
                    _fmaSelects = NSAPEntities.FMASelectViewModel.FMASelectCollection.ToList();

                    _fmasInRegion = new List<NSAPRegionFMA>();
                    foreach (var nsapRegionFMA in _currentRegion.FMAs)
                    {
                        _fmasInRegion.Add(nsapRegionFMA);
                    }
                    break;
                case "LandingSiteCount":
                    _csvPath = $@"{Utilities.Global.CSVMediaSaveFolder}\ls_select.csv";
                    if (NSAPEntities.LSSelectViewModel == null)
                    {
                        NSAPEntities.LSSelectViewModel = new LSSelectViewModel(_csvPath);
                    }
                    _lSSelects = new List<LSSelect>();
                    _lSSelects = NSAPEntities.LSSelectViewModel.LSSelectCollection.ToList();

                    _landingSitesInRegion = new List<NSAPRegionFMAFishingGroundLandingSite>();
                    foreach (var nsapRegionFMA in _currentRegion.FMAs)
                    {
                        foreach (var fma_fg in nsapRegionFMA.FishingGrounds)
                        {
                            foreach (var ls in fma_fg.LandingSites)
                            {
                                _landingSitesInRegion.Add(ls);
                            }
                        }
                    }

                    break;
                case "FishingGroundCount":
                    _csvPath = $@"{Utilities.Global.CSVMediaSaveFolder}\fg_select.csv";
                    if (NSAPEntities.FishingGroundSelectViewModel == null)
                    {
                        NSAPEntities.FishingGroundSelectViewModel = new FishingGroundSelectViewModel(_csvPath);
                    }
                    _fishingGroundSelects = new List<FishingGroundSelect>();
                    _fishingGroundSelects = NSAPEntities.FishingGroundSelectViewModel.FishingGroundSelectCollection.ToList();

                    _fishingGroundsInRegion = new List<NSAPRegionFMAFishingGround>();
                    foreach (var nsapRegionFMA in _currentRegion.FMAs)
                    {
                        foreach (var fma_fg in nsapRegionFMA.FishingGrounds)
                        {
                            _fishingGroundsInRegion.Add(fma_fg);
                        }
                    }
                    break;
                case "Gears":
                    _csvPath = $@"{Utilities.Global.CSVMediaSaveFolder}\gear_select.csv";
                    if (NSAPEntities.GearSelectViewModel == null)
                    {
                        NSAPEntities.GearSelectViewModel = new GearSelectViewModel(_csvPath);
                    }
                    _gearSelects = new List<GearSelect>();
                    _gearSelects = NSAPEntities.GearSelectViewModel.GearSelectCollection.ToList();

                    _gearsInRegion = new List<NSAPRegionGear>();
                    foreach (var gear in _currentRegion.Gears)
                    {
                        _gearsInRegion.Add(gear);
                    }
                    break;
                case "Vessels":
                    _csvPath = $@"{Utilities.Global.CSVMediaSaveFolder}\vessel_name_municipal.csv";
                    NSAPEntities.VesselSelectViewModel = new VesselSelectViewModel(_csvPath, FisheriesSector.Municipal);

                    _vesselSelects = new List<VesselSelect>();
                    _vesselSelects = NSAPEntities.VesselSelectViewModel.VesselSelectCollection.ToList();

                    _vesselsInRegion = _currentRegion.FishingVessels.Where(t => t.FishingVessel.FisheriesSector == FisheriesSector.Municipal).ToList();
                    break;
                case "VesselsCommercial":
                    _csvPath = $@"{Utilities.Global.CSVMediaSaveFolder}\vessel_name_commercial.csv";
                    NSAPEntities.VesselSelectViewModel = new VesselSelectViewModel(_csvPath, FisheriesSector.Commercial);

                    _vesselSelects = new List<VesselSelect>();
                    _vesselSelects = NSAPEntities.VesselSelectViewModel.VesselSelectCollection.ToList();
                    _vesselsInRegion = _currentRegion.FishingVessels.Where(t => t.FishingVessel.FisheriesSector == FisheriesSector.Commercial).ToList();
                    break;
                case "Enumerators":
                    _csvPath = $@"{Utilities.Global.CSVMediaSaveFolder}\enumerator_select.csv";
                    if (NSAPEntities.EnumeratorSelectViewModel == null)
                    {
                        NSAPEntities.EnumeratorSelectViewModel = new EnumeratorSelectViewModel(_csvPath);
                    }
                    _enumeratorSelects = new List<EnumeratorSelect>();
                    _enumeratorSelects = NSAPEntities.EnumeratorSelectViewModel.EnumeratorSelectCollection.ToList();

                    _enumeratorsInRegion = new List<NSAPRegionEnumerator>();
                    foreach (var regionEnumerator in _currentRegion.NSAPEnumerators)
                    {
                        _enumeratorsInRegion.Add(regionEnumerator);
                    }
                    break;
            }
        }

        public string SelectedProperty
        {
            get { return _selectedProperty; }
            set
            {
                _selectedProperty = value;
                switch (_selectedProperty)
                {
                    case "FMAs":
                        _entityName = "FMAs";
                        Title = "FMA";
                        labelHeader.Content = $"FMAs {_currentRegion.Name}";
                        break;
                    case "LandingSiteCount":
                        _entityName = "Landing sites";
                        Title = "Landing site";
                        labelHeader.Content = $"Landing sites in {_currentRegion.Name}";
                        break;
                    case "FishingGroundCount":
                        _entityName = "Fishing grounds";
                        Title = "Fishing ground";
                        labelHeader.Content = $"Fishing grounds in {_currentRegion.Name}";
                        break;
                    case "Gears":
                        _entityName = "Fishing gears";
                        Title = "Fishing gears";
                        labelHeader.Content = $"Fishing gears in {_currentRegion.Name}";
                        break;
                    case "Vessels":
                        _entityName = "Municipal fishing vessels";
                        Title = "Municipal fishing vessels";
                        labelHeader.Content = $"Municipal fishing vessels in {_currentRegion.Name}";
                        break;
                    case "VesselsCommercial":
                        _entityName = "Commercial fishing vessels";
                        Title = "Commercial fishing vessels";
                        labelHeader.Content = $"Commercial fishing vessels in {_currentRegion.Name}";
                        break;
                    case "Enumerators":
                        _entityName = "Enumerators";
                        Title = "Enumerators";
                        labelHeader.Content = $"Enumerators in {_currentRegion.Name}";
                        break;
                }
                FillEntityLists();
                tviCSV.IsSelected = true;
                statusbarlabelCurrentEntity.Content = _csvPath;
            }
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonClose":
                    Close();
                    break;
            }
        }

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            string selectedMenu = ((TreeViewItem)e.NewValue).Header.ToString();
            dataGrid.Visibility = Visibility.Collapsed;
            propertyGrid.Visibility = Visibility.Collapsed;
            string currentRegionName = NSAPEntities.NSAPRegionViewModel.CurrentEntity.ToString();

            dataGrid.DataContext = null;
            switch (selectedMenu)
            {
                case "CSV from server":

                    switch (_selectedProperty)
                    {
                        case "FMAs":
                            
                            dataGrid.DataContext = _fmaSelects.OrderBy(t => t.RowID);
                            labelDataGrid.Content = $"Content of csv file for FMAs in {currentRegionName}";
                            break;
                        case "LandingSiteCount":
                            
                            dataGrid.DataContext = _lSSelects.OrderBy(t => t.RowID);
                            labelDataGrid.Content = $"Content of csv file for landing sites in {currentRegionName}";
                            break;
                        case "Enumerators":
                            
                            dataGrid.DataContext = _enumeratorSelects.OrderBy(t => t.RowID);
                            labelDataGrid.Content = $"Content of csv file for enumerators in {currentRegionName}";
                            break;
                        case "Gears":
                            
                            dataGrid.DataContext = _gearSelects.OrderBy(t => t.RowID);
                            labelDataGrid.Content = $"Content of csv file for fishing gears in {currentRegionName}";
                            break;
                        case "FishingGroundCount":
                            
                            dataGrid.DataContext = _fishingGroundSelects.OrderBy(t => t.RowID);
                            labelDataGrid.Content = $"Content of csv file for fishing grounds in {currentRegionName}";
                            break;
                        case "Vessels":
                        case "VesselsCommercial":
                           
                            dataGrid.DataContext = _vesselSelects.OrderBy(t => t.RowID);
                            labelDataGrid.Content = $"Content of csv file for municipal fishing vessels in {currentRegionName}";
                            if (_selectedProperty == "VesselsCommercial")
                            {
                                
                                labelDataGrid.Content = $"Content of csv file for commercial fishing vessels in {currentRegionName}";
                            }
                            break;
                    }
                    ConfigureDataGrid(fromCSV: true);
                    dataGrid.Visibility = Visibility.Visible;

                    break;
                case "Database":
                    switch (_selectedProperty)
                    {
                        case "FMAs":
                            dataGrid.DataContext = _fmasInRegion.OrderBy(t => t.RowID);
                            labelDataGrid.Content = $"Content of database for FMAs in {currentRegionName}";
                            break;
                        case "LandingSiteCount":
                            dataGrid.DataContext = _landingSitesInRegion.OrderBy(t => t.RowID);
                            labelDataGrid.Content = $"Content of database for landing sites in {currentRegionName}";
                            break;
                        case "Enumerators":
                            dataGrid.DataContext = _enumeratorsInRegion.OrderBy(t => t.RowID);
                            labelDataGrid.Content = $"Content of database for enumerators in {currentRegionName}";
                            break;
                        case "Gears":
                            dataGrid.DataContext = _gearsInRegion.OrderBy(t => t.RowID);
                            labelDataGrid.Content = $"Content of database for fishing gears for FMAs in {currentRegionName}";
                            break;
                        case "FishingGroundCount":
                            dataGrid.DataContext = _fishingGroundsInRegion.OrderBy(t => t.RowID);
                            labelDataGrid.Content = $"Content of database for fishing grounds in {currentRegionName}";
                            break;
                        case "Vessels":
                        case "VesselsCommercial":
                            labelDataGrid.Content = $"Content of database for municipal fishing vessels in {currentRegionName}";
                            if (_selectedProperty == "VesselsCommercial")
                            {
                                labelDataGrid.Content = $"Content of database for commercial fishing vessels in {currentRegionName}";
                            }
                            dataGrid.DataContext = _vesselsInRegion.OrderBy(t => t.RowID);
                            break;
                    }
                    ConfigureDataGrid(fromCSV: false);
                    dataGrid.Visibility = Visibility.Visible;
                    break;
                case "Analysis":
                    propertyGrid.Visibility = Visibility.Visible;
                    CSVMediaAnalysis cma = new CSVMediaAnalysis();

                    //List<int> databaseItemsNotInCSV = new List<int>();
                    //List<int> csvItemsNotInDatabase = new List<int>();
                    List<string> databaseItemsNotInCSV = new List<string>();
                    List<string> csvItemsNotInDatabase = new List<string>();
                    int csvItemsCount = 0;
                    int dbItemsCount = 0;

                    switch (_selectedProperty)
                    {
                        case "FMAs":

                            csvItemsCount = _fmaSelects.Count;
                            dbItemsCount = _fmasInRegion.Count;
                            labelDataGrid.Content = $"Analysis of comparison of content of csv and database for FMAs in {currentRegionName}";

                            foreach (var fma in _fmaSelects)
                            {
                                if (_fmasInRegion.FirstOrDefault(t => t.RowID == fma.RowID && t.FMA.Name == fma.Name && t.NSAPRegion.Code == fma.NSAPRegion.Code) == null)
                                {
                                    //csvItemsNotInDatabase.Add(fma.RowID);
                                    csvItemsNotInDatabase.Add($"{fma.RowID}-{fma.Name}-{fma.NSAPRegion}");
                                }
                            }

                            foreach (var fma_db in _fmasInRegion)
                            {
                                if (_fmaSelects.FirstOrDefault(t => t.RowID == fma_db.RowID && t.Name == fma_db.FMA.Name && t.NSAPRegion.Code == fma_db.NSAPRegion.Code) == null)
                                {
                                    //databaseItemsNotInCSV.Add(fma_db.RowID);
                                    csvItemsNotInDatabase.Add($"{fma_db.RowID}-{fma_db.FMA}-{fma_db.NSAPRegion}");
                                }
                            }

                            break;
                        case "LandingSiteCount":

                            csvItemsCount = _lSSelects.Count;
                            dbItemsCount = _landingSitesInRegion.Count;
                            labelDataGrid.Content = $"Analysis of comparison of content of csv and database for landing sites in {currentRegionName}";

                            foreach (var ls in _lSSelects)
                            {
                                if (_landingSitesInRegion.FirstOrDefault(t => t.RowID == ls.RowID && t.LandingSite.ToString() == ls.Name && t.NSAPRegionFMAFishingGround.RowID == ls.NSAPRegionFMAFishingGround.RowID) == null)
                                {
                                    //csvItemsNotInDatabase.Add(ls.RowID);
                                    csvItemsNotInDatabase.Add($"{ls.RowID}-{ls.Name}-{ls.NSAPRegionFMAFishingGround}");
                                }
                            }

                            foreach (var ls_db in _landingSitesInRegion)
                            {
                                if (_lSSelects.FirstOrDefault(t => t.RowID == ls_db.RowID && t.Name == ls_db.LandingSite.ToString() && t.NSAPRegionFMAFishingGround.RowID == ls_db.NSAPRegionFMAFishingGround.RowID) == null)
                                {
                                    //databaseItemsNotInCSV.Add(ls_db.RowID);
                                    csvItemsNotInDatabase.Add($"{ls_db.RowID}-{ls_db.LandingSite}-{ls_db.NSAPRegionFMAFishingGround}");
                                }
                            }
                            break;
                        case "Enumerators":

                            csvItemsCount = _enumeratorSelects.Count;
                            dbItemsCount = _enumeratorsInRegion.Count;
                            labelDataGrid.Content = $"Analysis of comparison of content of csv and database for enumerators in {currentRegionName}";

                            foreach (var en in _enumeratorSelects)
                            {
                                if (_enumeratorsInRegion.FirstOrDefault(t => t.RowID == en.RowID && t.Enumerator.ToString() == en.Name && t.NSAPRegion.Code == en.NSAPRegion.Code) == null)
                                {
                                    //csvItemsNotInDatabase.Add(en.RowID);
                                    csvItemsNotInDatabase.Add($"{en.RowID}-{en.Name}-{en.NSAPRegion}");
                                }
                            }

                            foreach (var en_db in _enumeratorsInRegion)
                            {
                                if (_enumeratorSelects.FirstOrDefault(t => t.RowID == en_db.RowID && t.Name == en_db.Enumerator.ToString() && t.NSAPRegion.Code == en_db.NSAPRegion.Code) == null)
                                {
                                    databaseItemsNotInCSV.Add($"{en_db.RowID}-{en_db.Enumerator}-{en_db.NSAPRegion}");
                                }
                            }
                            break;
                        case "Gears":

                            csvItemsCount = _gearSelects.Count;
                            dbItemsCount = _gearsInRegion.Count;
                            labelDataGrid.Content = $"Analysis of comparison of content of csv and database for fishing gears in {currentRegionName}";

                            foreach (var g in _gearSelects)
                            {
                                if (_gearsInRegion.FirstOrDefault(t => t.RowID == g.RowID && t.Gear.ToString() == g.Name && t.NSAPRegion.Code == g.NSAPRegion.Code) == null)
                                {
                                    //csvItemsNotInDatabase.Add(g.RowID);
                                    csvItemsNotInDatabase.Add($"{g.RowID}-{g.Name}-{g.NSAPRegion}");
                                }
                            }

                            foreach (var g_db in _gearsInRegion)
                            {
                                if (_gearSelects.FirstOrDefault(t => t.RowID == g_db.RowID && t.Name == g_db.Gear.ToString() && t.NSAPRegion.Code == g_db.NSAPRegion.Code) == null)
                                {
                                    //databaseItemsNotInCSV.Add(g_db.RowID);
                                    csvItemsNotInDatabase.Add($"{g_db.RowID}-{g_db.Gear}-{g_db.NSAPRegion}");
                                }
                            }
                            break;
                        case "FishingGroundCount":

                            csvItemsCount = _fishingGroundSelects.Count;
                            dbItemsCount = _fishingGroundsInRegion.Count;
                            labelDataGrid.Content = $"Analysis of comparison of content of csv and database for fishing grounds in {currentRegionName}";

                            foreach (var g in _fishingGroundSelects)
                            {
                                if (_fishingGroundsInRegion.FirstOrDefault(t => t.RowID == g.RowID && t.FishingGround.ToString() == g.Name && t.RegionFMA.RowID == g.NSAPRegionFMA.RowID) == null)
                                {
                                    //csvItemsNotInDatabase.Add(g.RowID);
                                    csvItemsNotInDatabase.Add($"{g.RowID}-{g.Name}-{g.NSAPRegionFMA}");
                                }
                            }

                            foreach (var g_db in _fishingGroundsInRegion)
                            {
                                if (_fishingGroundSelects.FirstOrDefault(t => t.RowID == g_db.RowID && t.Name == g_db.FishingGround.ToString() && t.NSAPRegionFMA.RowID == g_db.RegionFMA.RowID) == null)
                                {
                                    //databaseItemsNotInCSV.Add(g_db.RowID);
                                    csvItemsNotInDatabase.Add($"{g_db.RowID}-{g_db.FishingGround}-{g_db.RegionFMA}");
                                }
                            }
                            break;
                        case "Vessels":
                        case "VesselsCommercial":
                            csvItemsCount = _vesselSelects.Count;
                            dbItemsCount = _vesselsInRegion.Count;
                            labelDataGrid.Content = $"Analysis of comparison of content of csv and database for municipal fishing vessels in {currentRegionName}";
                            if (_selectedProperty == "VesselsCommercial")
                            {
                                labelDataGrid.Content = $"Analysis of comparison of content of csv and database for commercial fishing vessels in {currentRegionName}";
                            }

                            foreach (var v in _vesselSelects)
                            {
                                if (_vesselsInRegion.FirstOrDefault(t => t.RowID == v.RowID && t.FishingVessel.ToString() == v.Name && t.NSAPRegion.Code == v.NSAPRegion.Code) == null)
                                {
                                    //csvItemsNotInDatabase.Add(v.RowID);
                                    csvItemsNotInDatabase.Add($"{v.RowID}-{v.Name}-{v.NSAPRegion}");
                                }
                            }

                            foreach (var v_db in _vesselsInRegion)
                            {
                                if (_vesselSelects.FirstOrDefault(t => t.RowID == v_db.RowID && t.Name == v_db.FishingVessel.ToString() && t.NSAPRegion.Code == v_db.NSAPRegion.Code) == null)
                                {
                                    //databaseItemsNotInCSV.Add(v_db.RowID);
                                    csvItemsNotInDatabase.Add($"{v_db.RowID}-{v_db.FishingVessel}-{v_db.NSAPRegion}");
                                }
                            }
                            break;
                    }



                    cma.IsSimilarData = csvItemsCount == dbItemsCount && csvItemsNotInDatabase.Count == 0 && databaseItemsNotInCSV.Count == 0;
                    cma.EntityName = _entityName;
                    cma.CSVRecordCount = csvItemsCount;
                    cma.DatabaseRecordCount = dbItemsCount;
                    cma.CSVItemsNotInDatabase = csvItemsNotInDatabase;
                    cma.DatabaseItemsNotInCSV = databaseItemsNotInCSV;

                    propertyGrid.PropertyDefinitions.Clear();

                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "EntityName", DisplayName = "Entity", DisplayOrder = 1, Description = "Name of entity" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "IsSimilarData", DisplayName = "CSV and database are similar", DisplayOrder = 2, Description = "Content of csv and database are similar" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "CSVRecordCount", DisplayName = $"Number of {(_entityName == "FMAs" ? _entityName : _entityName.ToLower())} in csv", DisplayOrder = 3, Description = "Number of items in csv" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DatabaseRecordCount", DisplayName = $"Number of {(_entityName == "FMAs" ? _entityName : _entityName.ToLower())} in database", DisplayOrder = 4, Description = "Number of items in database" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "CSVItemsNotInDatabase", DisplayName = $"{_entityName} in csv not in database", DisplayOrder = 5, Description = "Identifiers (Row IDs) of items in csv that are not found in the database" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DatabaseItemsNotInCSV", DisplayName = $"{_entityName} in database not in csv", DisplayOrder = 6, Description = "Identifiers (Row IDs) of items in database that are not found in the csv" });

                    propertyGrid.SelectedObject = cma;
                    break;
            }
        }

        private void Grid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void OnMenuClick(object sender, RoutedEventArgs e)
        {
            bool proceed = true;
            switch (((MenuItem)sender).Header.ToString())
            {

                case "FMAs":
                    SelectedProperty = "FMAs";
                    break;
                case "Landing sites":
                    SelectedProperty = "LandingSiteCount";
                    break;
                case "Fishing gears":
                    SelectedProperty = "Gears";
                    break;
                case "Enumerators":
                    SelectedProperty = "Enumerators";
                    break;
                case "Fishing grounds":
                    SelectedProperty = "FishingGroundCount";
                    break;
                case "Municipal fishing vessels":
                    SelectedProperty = "Vessels";
                    break;
                case "Commercial fishing vessels":
                    SelectedProperty = "VesselsCommercial";
                    break;
                case "CSV file":
                    proceed = false;
                    MessageBox.Show($"CSV file is\r\n\r\n{_csvPath}", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "Close":
                    proceed = false;
                    Close();
                    break;
            }

            if (proceed)
            {

                switch (_selectedProperty)
                {
                    case "FMAs":
                        dataGrid.DataContext = _fmaSelects.OrderBy(t => t.RowID);
                        break;
                    case "LandingSiteCount":
                        dataGrid.DataContext = _lSSelects.OrderBy(t => t.RowID);
                        break;
                    case "Enumerators":
                        dataGrid.DataContext = _enumeratorSelects.OrderBy(t => t.RowID);
                        break;
                    case "Gears":
                        dataGrid.DataContext = _gearSelects.OrderBy(t => t.RowID);
                        break;
                    case "FishingGroundCount":
                        dataGrid.DataContext = _fishingGroundSelects.OrderBy(t => t.RowID);
                        break;
                    case "Vessels":
                    case "VesselsCommercial":
                        dataGrid.DataContext = _vesselSelects.OrderBy(t => t.RowID);
                        break;
                }
                ConfigureDataGrid(fromCSV: true);
            }
        }
        private void ConfigureDataGrid(bool fromCSV)
        {
            dataGrid.Columns.Clear();
            if (fromCSV)
            {
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Row ID", Binding = new Binding("RowID") });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("Name") });
                switch (_selectedProperty)
                {
                    case "LandingSiteCount":
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("NSAPRegionFMAFishingGround") });
                        break;
                    case "FMAs":
                    case "Enumerators":
                    case "Gears":
                    case "Vessels":
                    case "VesselsCommercial":
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Region", Binding = new Binding("NSAPRegion") });
                        break;

                    case "FishingGroundCount":
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("NSAPRegionFMA") });
                        break;

                }
            }
            else
            {
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Row ID", Binding = new Binding("RowID") });
                switch (_selectedProperty)
                {
                    case "FMAs":
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("FMA") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Region", Binding = new Binding("NSAPRegion") });
                        break;
                    case "LandingSiteCount":
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("LandingSite") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("NSAPRegionFMAFishingGround") });
                        break;
                    case "Enumerators":
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("Enumerator") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Region", Binding = new Binding("NSAPRegion") });
                        break;
                    case "Gears":
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("Gear") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Region", Binding = new Binding("NSAPRegion") });
                        break;
                    case "FishingGroundCount":
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("FishingGround") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("RegionFMA") });
                        break;
                    case "Vessels":
                    case "VesselsCommercial":
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("FishingVessel") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Region", Binding = new Binding("NSAPRegion") });
                        break;

                }
            }

        }
        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SavePlacement();
        }
    }
}
