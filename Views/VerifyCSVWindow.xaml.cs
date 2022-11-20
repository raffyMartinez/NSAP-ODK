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
        }

        private void VerifyCSVWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _instance = null;
            Closing -= VerifyCSVWindow_Closing;
            Loaded -= VerifyCSVWindow_Loaded;
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
                        Title = "Landing site";
                        labelHeader.Content = $"Landing sites in {_currentRegion.Name}";
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
                        Title = "Fishing ground";
                        labelHeader.Content = $"Fishing grounds in {_currentRegion.Name}";
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
                        Title = "Fishing gears";
                        labelHeader.Content = $"Fishing gears in {_currentRegion.Name}";
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
                        Title = "Municipal fishing vessels";
                        labelHeader.Content = $"Municipal fishing vessels in {_currentRegion.Name}";

                        NSAPEntities.VesselSelectViewModel = new VesselSelectViewModel($@"{Utilities.Global.CSVMediaSaveFolder}\vessel_name_municipal.csv", FisheriesSector.Municipal);

                        _vesselSelects = new List<VesselSelect>();
                        _vesselSelects = NSAPEntities.VesselSelectViewModel.VesselSelectCollection.ToList();

                        _vesselsInRegion = _currentRegion.FishingVessels.Where(t => t.FishingVessel.FisheriesSector == FisheriesSector.Municipal).ToList();
                        break;
                    case "VesselsCommercial":
                        Title = "Commercial fishing vessels";
                        labelHeader.Content = $"Commercial fishing vessels in {_currentRegion.Name}";

                        NSAPEntities.VesselSelectViewModel = new VesselSelectViewModel($@"{Utilities.Global.CSVMediaSaveFolder}\vessel_name_commercial.csv", FisheriesSector.Commercial);

                        _vesselSelects = new List<VesselSelect>();
                        _vesselSelects = NSAPEntities.VesselSelectViewModel.VesselSelectCollection.ToList();
                        _vesselsInRegion = _currentRegion.FishingVessels.Where(t => t.FishingVessel.FisheriesSector == FisheriesSector.Commercial).ToList();
                        break;
                    case "Enumerators":
                        Title = "Enumerators";
                        labelHeader.Content = $"Enumerators in {_currentRegion.Name}";
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


            dataGrid.DataContext = null;
            switch (selectedMenu)
            {
                case "CSV from server":
                    
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
                    dataGrid.Visibility = Visibility.Visible;
                    break;
                case "Database":

                    switch (_selectedProperty)
                    {
                        case "FMAs":
                            dataGrid.DataContext = _fmasInRegion.OrderBy(t => t.RowID);
                            break;
                        case "LandingSiteCount":
                            dataGrid.DataContext = _landingSitesInRegion.OrderBy(t => t.RowID);
                            break;
                        case "Enumerators":
                            dataGrid.DataContext = _enumeratorsInRegion.OrderBy(t => t.RowID);
                            break;
                        case "Gears":
                            dataGrid.DataContext = _gearsInRegion.OrderBy(t => t.RowID);
                            break;
                        case "FishingGroundCount":
                            dataGrid.DataContext = _fishingGroundsInRegion.OrderBy(t => t.RowID);
                            break;
                        case "Vessels":
                        case "VesselsCommercial":
                            dataGrid.DataContext = _vesselsInRegion.OrderBy(t => t.RowID);
                            break;
                    }
                    ConfigureDataGrid(fromCSV: false);
                    dataGrid.Visibility = Visibility.Visible;
                    break;
                case "Analysis":
                    propertyGrid.Visibility = Visibility.Visible;
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
