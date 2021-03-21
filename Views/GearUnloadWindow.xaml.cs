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
    /// <summary>
    /// Interaction logic for GearUnloadWindow.xaml
    /// </summary>
    public partial class GearUnloadWindow : Window
    {
        private GearUnload _gearUnload;
        private TreeViewModelControl.AllSamplingEntitiesEventHandler _treeItemData;
        private VesselUnload _selectedVesselUnload;
        private VesselUnloadWIndow _vesselUnloadWindow;
        private MainWindow _parentWindow;
        private bool _changeFromGridClick;

        public GearUnloadWindow(GearUnload gearUnload, TreeViewModelControl.AllSamplingEntitiesEventHandler treeItemData, MainWindow parent)
        {
            InitializeComponent();
            _gearUnload = gearUnload;
            _treeItemData = treeItemData;
            textBoxBoats.Text = _gearUnload.Boats.ToString();
            textBoxCatch.Text = _gearUnload.Catch.ToString();
            _parentWindow = parent;
            Title = $"Gear unload for {gearUnload.GearUsedName}";
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
                        _selectedVesselUnload = ((UnloadChildrenSummary)dataGridUnloadSummary.SelectedItem).VesselUnload;
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

            //switch (((DataGrid)sender).Name)
            //{
            //    case "dataGridUnloadSummary":

            //        break;

            //    case "GridVesselUnload":


            //        if (_vesselUnloadWindow == null)
            //        {
            //            _vesselUnloadWindow = new VesselUnloadWIndow(_selectedVesselUnload, this);
            //            _vesselUnloadWindow.Owner = this;
            //            _vesselUnloadWindow.Show();
            //        }
            //        else
            //        {
            //            _vesselUnloadWindow.VesselUnload = _selectedVesselUnload;
            //        }
            //        break;
            //}
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
        public GearUnload GearUnload
        {
            set
            {

                _gearUnload = value;
                
                //if (_gearUnload.ListVesselUnload.Count > 0)
                if(NSAPEntities.VesselUnloadViewModel.GetAllVesselUnloads(_gearUnload, true).Count>0)
                {
                    GridVesselUnload.Visibility = Visibility.Visible;
                    dataGridUnloadSummary.Visibility = Visibility.Visible;
                    gridGearUnloadNumbers.Visibility = Visibility.Visible;
                    switch(((TabItem)this.TabControl.SelectedItem).Header)
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
                    catch(ArgumentOutOfRangeException )
                    {
                        //ignore
                    }
                    catch(Exception ex)
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
            LabelTitle.Content = $"Gear unload for {_gearUnload.GearUsedName} at {_treeItemData.LandingSiteText}, {_treeItemData.FishingGround}";
            GridVesselUnload.Columns.Clear();

            GridVesselUnload.DataContext = NSAPEntities.VesselUnloadViewModel.GetAllVesselUnloads(_gearUnload, true);
            
            GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("PK") });
            GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("UserName") });
            GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Vessel", Binding = new Binding("VesselName") });
            GridVesselUnload.Columns.Add(new DataGridCheckBoxColumn { Header = "Fishing trip success", Binding = new Binding("OperationIsSuccessful") });
            GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Weight of catch", Binding = new Binding("WeightOfCatchText") });
            GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Catch composition count", Binding = new Binding("CatchCompositionCountText") });
            GridVesselUnload.Columns.Add(new DataGridCheckBoxColumn { Header = "Tracking", Binding = new Binding("OperationIsTracked") });
            GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "GPS", Binding = new Binding("GPS.AssignedName") });
            GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Notes", Binding = new Binding("Notes") });
        }

        private void ShowUnloadSummaryGrid()
        {
            //dataGridUnloadSummary.Visibility = Visibility.Visible;
            labelUnloadSummary.Content = "Summary of vessel unloads of selected sampling day";
            List<UnloadChildrenSummary> list = new List<UnloadChildrenSummary>();
            foreach (var unload in NSAPEntities.VesselUnloadViewModel.GetAllVesselUnloads(_gearUnload, true))
            {
                list.Add(new UnloadChildrenSummary(unload));
            }
            dataGridUnloadSummary.Columns.Clear();
            dataGridUnloadSummary.AutoGenerateColumns = false;
            dataGridUnloadSummary.DataContext = list;

            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("PK") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Date sampled", Binding = new Binding("DateSampling") });
            //dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Region", Binding = new Binding("Region") });
            //dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("FMA") });
            //dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("FishingGround") });
            //dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("LandingSite") });
            //dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("Gear") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("Enumerator") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Fishing grid count", Binding = new Binding("CountGridLocations") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Soak time count", Binding = new Binding("CountSoakTimes") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Effort indicator count", Binding = new Binding("CountEffortIndicators") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Catch composition count", Binding = new Binding("CountCatchComposition") });

            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Length freq count", Binding = new Binding("CountCatchLengthFreqs") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Length count", Binding = new Binding("CountCatchLengths") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Length weight count", Binding = new Binding("CountCatchLengthWeights") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Maturity count", Binding = new Binding("CountCatchMaturities") });

        }
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            ShowVesselUnloadGrid();
            ShowUnloadSummaryGrid();
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            this.SavePlacement();
            _parentWindow.GearUnloadWindowClosed();
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


    }
}
