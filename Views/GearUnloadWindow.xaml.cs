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
using NSAP_ODK.Entities;
using NSAP_ODK.Utilities;
namespace NSAP_ODK.Views
{
    public enum GearUnloadWindowListSource
    {
        ListSourceGearUnload,
        listSourceVesselUnload
    }

    /// <summary>
    /// Interaction logic for GearUnloadWindow.xaml
    /// </summary>
    /// 
    public partial class GearUnloadWindow : Window
    {
        private GearUnload _gearUnload;
        private List<GearUnload> _gearUnloads;
        private TreeViewModelControl.AllSamplingEntitiesEventHandler _treeItemData;
        private VesselUnload _selectedVesselUnload;
        private VesselUnloadWIndow _vesselUnloadWindow;
        private MainWindow _parentWindow;
        private bool _changeFromGridClick;
        private List<VesselUnload> _vesselUnloads;
        private GearUnloadWindowListSource _listSource;
        private static GearUnloadWindow _instance;

        public static GearUnloadWindow GetInstance(List<VesselUnload> vesselUnloads)
        {
            NSAPEntities.NSAPRegion = vesselUnloads[0].Parent.Parent.NSAPRegion;
            if (_instance == null) _instance = new GearUnloadWindow(vesselUnloads);
            return _instance;
        }

        public static GearUnloadWindow GetInstance(GearUnload gearUnload, TreeViewModelControl.AllSamplingEntitiesEventHandler treeItemData, MainWindow parent)
        {
            if (_instance == null) _instance = new GearUnloadWindow(gearUnload, treeItemData, parent);
            return _instance;
        }
        public GearUnloadWindow(List<VesselUnload> vesselUnloads)
        {
            InitializeComponent();
            _vesselUnloads = vesselUnloads;
            _listSource = GearUnloadWindowListSource.listSourceVesselUnload;
            tabPageBoatCount.Visibility = Visibility.Collapsed;

        }

        public GearUnloadWindow(List<GearUnload> gearUnloads)
        {
            InitializeComponent();
            _gearUnloads = gearUnloads;
            _listSource = GearUnloadWindowListSource.listSourceVesselUnload;
            tabPageBoatCount.Visibility = Visibility.Collapsed;

        }

        public GearUnloadWindow(List<GearUnload>gearUnloads, TreeViewModelControl.AllSamplingEntitiesEventHandler treeItemData, MainWindow parent)
        {
            InitializeComponent();
            _gearUnloads = gearUnloads;
            _treeItemData = treeItemData;
            textBoxBoats.Text = _gearUnloads[0].Boats.ToString();
            textBoxCatch.Text = _gearUnloads[0].Catch.ToString();
            _parentWindow = parent;
            Title = $"Gear unload for {gearUnloads[0].GearUsedName}";
            _listSource = GearUnloadWindowListSource.ListSourceGearUnload;
            tabPageBoatCount.Visibility = Visibility.Visible;
        }
        public GearUnloadWindow(GearUnload gearUnload, TreeViewModelControl.AllSamplingEntitiesEventHandler treeItemData, MainWindow parent)
        {
            InitializeComponent();
            _gearUnload = gearUnload;
            _treeItemData = treeItemData;
            textBoxBoats.Text = _gearUnload.Boats.ToString();
            textBoxCatch.Text = _gearUnload.Catch.ToString();
            _parentWindow = parent;
            Title = $"Gear unload for {gearUnload.GearUsedName}";
            _listSource = GearUnloadWindowListSource.ListSourceGearUnload;
            tabPageBoatCount.Visibility = Visibility.Visible;
        }
        private void OnTabSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_changeFromGridClick)
            {
                _changeFromGridClick = false;
            }
            else
            {
                switch (((TabItem)((TabControl)sender).SelectedItem).Header)
                {
                    case "Unload entities summary":
                        ShowUnloadSummaryGrid();
                        break;
                    case "Vessel unload":
                        ShowVesselUnloadGrid();
                        break;
                    case "Number of boats and sum of catch":

                        //gridGearUnloadNumbers.Visibility = Visibility.Visible;
                        break;
                }
            }
        }
        private void OnGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _changeFromGridClick = true;
            switch (((DataGrid)sender).Name)
            {
                case "dataGridUnloadSummary":
                    if (dataGridUnloadSummary.SelectedItem != null)
                    {
                        _selectedVesselUnload = (VesselUnload)dataGridUnloadSummary.SelectedItem;
                    }
                    break;
                case "GridVesselUnload":
                    _selectedVesselUnload = (VesselUnload)GridVesselUnload.SelectedItem;
                    break;
            }

            if (_vesselUnloadWindow != null)
            {
                _vesselUnloadWindow.VesselUnload = _selectedVesselUnload;
            }

        }
        private void OnGridDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_vesselUnloadWindow == null)
            {
                _vesselUnloadWindow = new VesselUnloadWIndow(_selectedVesselUnload, this);
                _vesselUnloadWindow.Owner = this;
                _vesselUnloadWindow.Show();
            }
            else
            {
                _vesselUnloadWindow.VesselUnload = _selectedVesselUnload;
            }

        }

        public void TurnGridOff()
        {
            dataGridUnloadSummary.Visibility = Visibility.Hidden;
            GridVesselUnload.Visibility = Visibility.Hidden;
            gridGearUnloadNumbers.Visibility = Visibility.Hidden;

            //dataGridUnloadSummary.ItemsSource = null;
            //GridVesselUnload.ItemsSource = null;
            //dataGridUnloadSummary.Items.Clear();
            //GridVesselUnload.Items.Clear();
        }

        public List<VesselUnload> VesselUnloads
        {
            set
            {
                _vesselUnloads = value;
                NSAPEntities.NSAPRegion = _vesselUnloads[0].Parent.Parent.NSAPRegion;
            }
            get
            {
                return _vesselUnloads;
            }
        }

        public List<GearUnload> GearUnloads
        {
            set
            {
                _gearUnloads = value;
                VesselUnloads = VesselUnloadViewModel.GetVesselUnloads(_gearUnloads);
                if (VesselUnloads.Count > 0)
                {
                    GridVesselUnload.Visibility = Visibility.Visible;
                    dataGridUnloadSummary.Visibility = Visibility.Visible;
                    gridGearUnloadNumbers.Visibility = Visibility.Visible;
                    switch (((TabItem)this.TabControl.SelectedItem).Header)
                    {
                        case "Unload entities summary":
                            ShowUnloadSummaryGrid();
                            break;
                        case "Vessel unload":
                            ShowVesselUnloadGrid();
                            break;
                        case "Number of boats and sum of catch":
                            //gridGearUnloadNumbers.Visibility = Visibility.Visible;
                            break;
                    }



                    object item = GridVesselUnload.Items[0];
                    GridVesselUnload.SelectedItem = item;
                    try
                    {
                        GridVesselUnload.ScrollIntoView(item);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        //ignore
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            get
            {
                return _gearUnloads;
            }
        }
        public GearUnload GearUnload
        {
            set
            {

                _gearUnload = value;

                //if (_gearUnload.ListVesselUnload.Count > 0)
                if (NSAPEntities.VesselUnloadViewModel.GetAllVesselUnloads(_gearUnload, true).Count > 0)
                {
                    GridVesselUnload.Visibility = Visibility.Visible;
                    dataGridUnloadSummary.Visibility = Visibility.Visible;
                    gridGearUnloadNumbers.Visibility = Visibility.Visible;
                    switch (((TabItem)this.TabControl.SelectedItem).Header)
                    {
                        case "Unload entities summary":
                            ShowUnloadSummaryGrid();

                            break;
                        case "Vessel unload":
                            ShowVesselUnloadGrid();
                            break;
                        case "Number of boats and sum of catch":
                            //gridGearUnloadNumbers.Visibility = Visibility.Visible;
                            break;
                    }



                    object item = GridVesselUnload.Items[0];
                    GridVesselUnload.SelectedItem = item;
                    try
                    {
                        GridVesselUnload.ScrollIntoView(item);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        //ignore
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            get
            {
                return _gearUnload;
            }
        }
        public void VesselWindowClosed()
        {
            _vesselUnloadWindow = null;
        }

        private void ShowVesselUnloadGrid()
        {

            GridVesselUnload.Columns.Clear();

            switch (_listSource)
            {
                case GearUnloadWindowListSource.ListSourceGearUnload:
                    //LabelTitle.Content = $"Gear unload for {_gearUnload.GearUsedName} at {_treeItemData.LandingSiteText}, {_treeItemData.FishingGround}";
                    //GridVesselUnload.DataContext = NSAPEntities.VesselUnloadViewModel.GetAllVesselUnloads(_gearUnload, true);

                    //GridVesselUnload.DataContext = _gearUnload.VesselUnloadViewModel.VesselUnloadCollection;
                    //if (_gearUnload.VesselUnloadViewModel == null)
                    //{
                    //    _gearUnload.VesselUnloadViewModel = new VesselUnloadViewModel(_gearUnload);
                    //}
                    //GridVesselUnload.DataContext = NSAPEntities.LandingSiteSamplingViewModel.VesselUnloadsFromDummyGearUnload(_gearUnload);
                    _vesselUnloads = VesselUnloadViewModel.GetVesselUnloads(_gearUnloads);
                    GridVesselUnload.DataContext = _vesselUnloads;
                    break;
                case GearUnloadWindowListSource.listSourceVesselUnload:
                    LabelTitle.Content = "Vessel unloads from summary";
                    GridVesselUnload.DataContext = _vesselUnloads;
                    break;
            }


            GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("PK") });
            GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Date sampled", Binding = new Binding("DateTimeSampling") });
            GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("EnumeratorName") });
            GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Fishing gear", Binding = new Binding("GearUsed") });
            GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Vessel", Binding = new Binding("VesselName") });
            GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Number of fishers", Binding = new Binding("NumberOfFishers") });
            GridVesselUnload.Columns.Add(new DataGridCheckBoxColumn { Header = "Fishing trip success", Binding = new Binding("OperationIsSuccessful") });
            GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Weight of catch", Binding = new Binding("WeightOfCatchText") });
            GridVesselUnload.Columns.Add(new DataGridCheckBoxColumn { Header = "Includes catch composition", Binding = new Binding("HasCatchComposition") });
            GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Catch composition count", Binding = new Binding("CatchCompositionCountText") });
            GridVesselUnload.Columns.Add(new DataGridCheckBoxColumn { Header = "Tracking", Binding = new Binding("OperationIsTracked") });
            GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "GPS", Binding = new Binding("GPS.AssignedName") });
            GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Notes", Binding = new Binding("Notes") });
        }

        private void ShowUnloadSummaryGrid()
        {
            labelUnloadSummary.Content = "Summary of vessel unloads of selected sampling day";
            dataGridUnloadSummary.Columns.Clear();
            dataGridUnloadSummary.AutoGenerateColumns = false;
            //foreach(VesselUnload vu in _vesselUnloads)
            //{
            //    vu.SetSubModels();
            //}

            dataGridUnloadSummary.DataContext = _vesselUnloads;
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("PK") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Date sampled", Binding = new Binding("DateTimeSampling") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("EnumeratorName") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Fishing gear", Binding = new Binding("GearUsed") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Fishing grid count", Binding = new Binding("CountGrids") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Soak time count", Binding = new Binding("CountGearSoak") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Effort indicator count", Binding = new Binding("CountEffortIndicators") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Catch composition count", Binding = new Binding("CountCatchCompositionItems") });

            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Length freq count", Binding = new Binding("CountLenFreqRows") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Length count", Binding = new Binding("CountLengthRows") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Length weight count", Binding = new Binding("CountLenWtRows") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Maturity count", Binding = new Binding("CountMaturityRows") });

        }
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            ShowVesselUnloadGrid();
            ShowUnloadSummaryGrid();
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            this.SavePlacement();
            _instance = null;
            if (_parentWindow != null)
            {
                _parentWindow.GearUnloadWindowClosed();
            }
        }

        //This method is load the actual position of the window from the file
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }

        private void OnButtonCLick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "ButtonSaveBoatsCatch":
                    if (textBoxCatch.Text.Length > 0 && textBoxBoats.Text.Length > 0
                        && double.TryParse(textBoxCatch.Text, out double c)
                        && int.TryParse(textBoxBoats.Text, out int b))
                    {

                        _gearUnload.Boats = b;
                        _gearUnload.Catch = c;

                        GearUnload gu = new GearUnload
                        {
                            Boats = b,
                            Catch = c,
                            PK = _gearUnload.PK,
                            GearID = _gearUnload.GearID,
                            LandingSiteSamplingID = _gearUnload.Parent.PK
                        };
                        NSAPEntities.GearUnloadViewModel.UpdateRecordInRepo(gu);
                    }
                    else
                    {
                        if (textBoxCatch.Text.Length > 0 || textBoxBoats.Text.Length > 0)
                        {
                            MessageBox.Show("Number of boats and total catch are both required", "Validation error", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    break;
                case "ButtonClose":
                    Close();
                    break;
            }

        }


        private void OnWindowClosed(object sender, EventArgs e)
        {
            ((MainWindow)Owner).Focus();
        }

        private void Grid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
