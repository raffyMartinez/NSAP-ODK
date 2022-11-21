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
                    if (NSAPEntities.FMASelectViewModel == null)
                    {
                        NSAPEntities.FMASelectViewModel = new FMASelectViewModel($@"{Utilities.Global.CSVMediaSaveFolder}\fma_select.csv");
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
                    if (NSAPEntities.LSSelectViewModel == null)
                    {
                        NSAPEntities.LSSelectViewModel = new LSSelectViewModel($@"{Utilities.Global.CSVMediaSaveFolder}\ls_select.csv");
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
                    if (NSAPEntities.FishingGroundSelectViewModel == null)
                    {
                        NSAPEntities.FishingGroundSelectViewModel = new FishingGroundSelectViewModel($@"{Utilities.Global.CSVMediaSaveFolder}\fg_select.csv");
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
                    if (NSAPEntities.GearSelectViewModel == null)
                    {
                        NSAPEntities.GearSelectViewModel = new GearSelectViewModel($@"{Utilities.Global.CSVMediaSaveFolder}\gear_select.csv");
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
                    NSAPEntities.VesselSelectViewModel = new VesselSelectViewModel($@"{Utilities.Global.CSVMediaSaveFolder}\vessel_name_municipal.csv", FisheriesSector.Municipal);

                    _vesselSelects = new List<VesselSelect>();
                    _vesselSelects = NSAPEntities.VesselSelectViewModel.VesselSelectCollection.ToList();

                    _vesselsInRegion = _currentRegion.FishingVessels.Where(t => t.FishingVessel.FisheriesSector == FisheriesSector.Municipal).ToList();
                    break;
                case "VesselsCommercial":

                    NSAPEntities.VesselSelectViewModel = new VesselSelectViewModel($@"{Utilities.Global.CSVMediaSaveFolder}\vessel_name_commercial.csv", FisheriesSector.Commercial);

                    _vesselSelects = new List<VesselSelect>();
                    _vesselSelects = NSAPEntities.VesselSelectViewModel.VesselSelectCollection.ToList();
                    _vesselsInRegion = _currentRegion.FishingVessels.Where(t => t.FishingVessel.FisheriesSector == FisheriesSector.Commercial).ToList();
                    break;
                case "Enumerators":
                    if (NSAPEntities.EnumeratorSelectViewModel == null)
                    {
                        NSAPEntities.EnumeratorSelectViewModel = new EnumeratorSelectViewModel($@"{Utilities.Global.CSVMediaSaveFolder}\enumerator_select.csv");
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
                        Title = "FMA";
                        labelHeader.Content = $"FMAs {_currentRegion.Name}";
                        break;
                    case "LandingSiteCount":
                        Title = "Landing site";
                        labelHeader.Content = $"Landing sites in {_currentRegion.Name}";
                        break;
                    case "FishingGroundCount":
                        Title = "Fishing ground";
                        labelHeader.Content = $"Fishing grounds in {_currentRegion.Name}";
                        break;
                    case "Gears":
                        Title = "Fishing gears";
                        labelHeader.Content = $"Fishing gears in {_currentRegion.Name}";
                        break;
                    case "Vessels":
                        Title = "Municipal fishing vessels";
                        labelHeader.Content = $"Municipal fishing vessels in {_currentRegion.Name}";
                        break;
                    case "VesselsCommercial":
                        Title = "Commercial fishing vessels";
                        labelHeader.Content = $"Commercial fishing vessels in {_currentRegion.Name}";
                        break;
                    case "Enumerators":
                        Title = "Enumerators";
                        labelHeader.Content = $"Enumerators in {_currentRegion.Name}";
                        break;
                }
                FillEntityLists();
                tviCSV.IsSelected = true;
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

                    List<int> databaseItemsNotInCSV = new List<int>();
                    List<int> csvItemsNotInDatabase = new List<int>();
                    int csvItemsCount = 0;
                    int dbItemsCount = 0;
                    string entityName = "";

                    switch (_selectedProperty)
                    {
                        case "FMAs":
                            entityName = "FMA";
                            csvItemsCount = _fmaSelects.Count;
                            dbItemsCount = _fmasInRegion.Count;
                            labelDataGrid.Content = $"Analysis of comparison of content of csv and database for FMAs in {currentRegionName}";

                            foreach (var fma in _fmaSelects)
                            {
                                if (_fmasInRegion.FirstOrDefault(t => t.RowID == fma.RowID && t.FMA.Name == fma.Name && t.NSAPRegion.Code == fma.NSAPRegion.Code) == null)
                                {
                                    csvItemsNotInDatabase.Add(fma.RowID);
                                }
                            }

                            foreach (var fma_db in _fmasInRegion)
                            {
                                if (_fmaSelects.FirstOrDefault(t => t.RowID == fma_db.RowID && t.Name == fma_db.FMA.Name && t.NSAPRegion.Code == fma_db.NSAPRegion.Code) == null)
                                {
                                    databaseItemsNotInCSV.Add(fma_db.RowID);
                                }
                            }

                            break;
                        case "LandingSiteCount":
                            entityName = "Landing site";
                            csvItemsCount = _lSSelects.Count;
                            dbItemsCount = _landingSitesInRegion.Count;
                            labelDataGrid.Content = $"Analysis of comparison of content of csv and database for landing sites in {currentRegionName}";

                            foreach (var ls in _lSSelects)
                            {
                                if (_landingSitesInRegion.FirstOrDefault(t => t.RowID == ls.RowID && t.LandingSite.ToString() == ls.Name && t.NSAPRegionFMAFishingGround.RowID == ls.NSAPRegionFMAFishingGround.RowID) == null)
                                {
                                    csvItemsNotInDatabase.Add(ls.RowID);
                                }
                            }

                            foreach (var ls_db in _landingSitesInRegion)
                            {
                                if (_lSSelects.FirstOrDefault(t => t.RowID == ls_db.RowID && t.Name == ls_db.LandingSite.ToString() && t.NSAPRegionFMAFishingGround.RowID == ls_db.NSAPRegionFMAFishingGround.RowID) == null)
                                {
                                    databaseItemsNotInCSV.Add(ls_db.RowID);
                                }
                            }
                            break;
                        case "Enumerators":
                            entityName = "Enumerator";
                            csvItemsCount = _enumeratorSelects.Count;
                            dbItemsCount = _enumeratorsInRegion.Count;
                            labelDataGrid.Content = $"Analysis of comparison of content of csv and database for enumerators in {currentRegionName}";

                            foreach (var en in _enumeratorSelects)
                            {
                                if (_enumeratorsInRegion.FirstOrDefault(t => t.RowID == en.RowID && t.Enumerator.ToString() == en.Name && t.NSAPRegion.Code == en.NSAPRegion.Code) == null)
                                {
                                    csvItemsNotInDatabase.Add(en.RowID);
                                }
                            }

                            foreach (var en_db in _enumeratorsInRegion)
                            {
                                if (_enumeratorSelects.FirstOrDefault(t => t.RowID == en_db.RowID && t.Name == en_db.Enumerator.ToString() && t.NSAPRegion.Code == en_db.NSAPRegion.Code) == null)
                                {
                                    databaseItemsNotInCSV.Add(en_db.RowID);
                                }
                            }
                            break;
                        case "Gears":
                            entityName = "Gears";
                            csvItemsCount = _gearSelects.Count;
                            dbItemsCount = _gearsInRegion.Count;
                            labelDataGrid.Content = $"Analysis of comparison of content of csv and database for fishing gears in {currentRegionName}";

                            foreach (var g in _gearSelects)
                            {
                                if (_gearsInRegion.FirstOrDefault(t => t.RowID == g.RowID && t.Gear.ToString() == g.Name && t.NSAPRegion.Code == g.NSAPRegion.Code) == null)
                                {
                                    csvItemsNotInDatabase.Add(g.RowID);
                                }
                            }

                            foreach (var g_db in _gearsInRegion)
                            {
                                if (_gearSelects.FirstOrDefault(t => t.RowID == g_db.RowID && t.Name == g_db.Gear.ToString() && t.NSAPRegion.Code == g_db.NSAPRegion.Code) == null)
                                {
                                    databaseItemsNotInCSV.Add(g_db.RowID);
                                }
                            }
                            break;
                        case "FishingGroundCount":
                            entityName = "Fishing ground";
                            csvItemsCount = _fishingGroundSelects.Count;
                            dbItemsCount = _fishingGroundsInRegion.Count;
                            labelDataGrid.Content = $"Analysis of comparison of content of csv and database for fishing grounds in {currentRegionName}";

                            foreach (var g in _fishingGroundSelects)
                            {
                                if (_fishingGroundsInRegion.FirstOrDefault(t => t.RowID == g.RowID && t.FishingGround.ToString() == g.Name && t.RegionFMA.RowID == g.NSAPRegionFMA.RowID) == null)
                                {
                                    csvItemsNotInDatabase.Add(g.RowID);
                                }
                            }

                            foreach (var g_db in _fishingGroundsInRegion)
                            {
                                if (_fishingGroundSelects.FirstOrDefault(t => t.RowID == g_db.RowID && t.Name == g_db.FishingGround.ToString() && t.NSAPRegionFMA.RowID == g_db.RegionFMA.RowID) == null)
                                {
                                    databaseItemsNotInCSV.Add(g_db.RowID);
                                }
                            }
                            break;
                        case "Vessels":
                        case "VesselsCommercial":
                            entityName = "Municipal fishing vessel";
                            if (_selectedProperty == "VesselsCommercial")
                            {
                                entityName = "Commercial fishing vessel";
                            }
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
                                    csvItemsNotInDatabase.Add(v.RowID);
                                }
                            }

                            foreach (var v_db in _vesselsInRegion)
                            {
                                if (_vesselSelects.FirstOrDefault(t => t.RowID == v_db.RowID && t.Name == v_db.FishingVessel.ToString() && t.NSAPRegion.Code == v_db.NSAPRegion.Code) == null)
                                {
                                    databaseItemsNotInCSV.Add(v_db.RowID);
                                }
                            }
                            break;
                    }

                    cma.IsSimilarData = csvItemsCount == dbItemsCount && csvItemsNotInDatabase.Count == 0 && databaseItemsNotInCSV.Count == 0;
                    cma.EntityName = entityName;
                    cma.CSVRecordCount = csvItemsCount;
                    cma.DatabaseRecordCount = dbItemsCount;
                    cma.CSVItemsNotInDatabase = csvItemsNotInDatabase;
                    cma.DatabaseItemsNotInCSV = databaseItemsNotInCSV;

                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "EntityName", DisplayName = "Entity", DisplayOrder = 1, Description = "Name of entity" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "IsSimilarData", DisplayName = "CSV and database are similar", DisplayOrder = 2, Description = "Content of csv and database are similar" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "CSVRecordCount", DisplayName = "Number of items in csv", DisplayOrder = 3, Description = "Number of items in csv" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DatabaseRecordCount", DisplayName = "Number of items in database", DisplayOrder = 4, Description = "Number of items in database" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "CSVItemsNotInDatabase", DisplayName = "Identifiers in csv not in database", DisplayOrder = 5, Description = "Identifiers (Row IDs) of items in csv that are not found in the database" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DatabaseItemsNotInCSV", DisplayName = "Identifiers in database not in csv", DisplayOrder = 6, Description = "Identifiers (Row IDs) of items in database that are not found in the csv" });

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
