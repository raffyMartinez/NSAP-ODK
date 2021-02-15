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

        public GearUnloadWindow(GearUnload gearUnload, TreeViewModelControl.AllSamplingEntitiesEventHandler treeItemData,MainWindow parent)
        {
            InitializeComponent();
            _gearUnload = gearUnload;
            _treeItemData = treeItemData;
            textBoxBoats.Text = _gearUnload.Boats.ToString();
            textBoxCatch.Text = _gearUnload.Catch.ToString();
            _parentWindow = parent;
            Title = $"Gear unload for {gearUnload.GearUsedName}";
        }

        private void OnGridDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_vesselUnloadWindow == null)
            {
                _vesselUnloadWindow = new VesselUnloadWIndow(_selectedVesselUnload,this);
                _vesselUnloadWindow.Owner = this;
                _vesselUnloadWindow.Show();
            }
            else
            {
                _vesselUnloadWindow.VesselUnload = _selectedVesselUnload;
            }
        }

        public GearUnload GearUnload
        {
            set
            {
                _gearUnload = value;
                ShowVesselUnloadGrid();
                object item = GridVesselUnload.Items[0];
                GridVesselUnload.SelectedItem = item;
                GridVesselUnload.ScrollIntoView(item);
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
            //GridVesselUnload.Items.Clear();

            GridVesselUnload.DataContext = _gearUnload.ListVesselUnload;
            GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("PK") });
            GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("UserName") });
            GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Vessel", Binding = new Binding("VesselName") });
            GridVesselUnload.Columns.Add(new DataGridCheckBoxColumn { Header = "Fishing trip success", Binding = new Binding("OperationIsSuccessful") });
            GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Weight of catch", Binding = new Binding("WeightOfCatchText") });
            GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Catch composition count", Binding = new Binding("CatchCompositionCountText") });
            GridVesselUnload.Columns.Add(new DataGridCheckBoxColumn { Header = "Tracking", Binding = new Binding("OperationIsTracked") });
            GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "GPS", Binding = new Binding("GPS.AssignedName") });
        }
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            ShowVesselUnloadGrid();
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
            switch(((Button)sender).Name)
            {
                case "ButtonSaveBoatsCatch":
                    if (textBoxCatch.Text.Length > 0 && textBoxBoats.Text.Length > 0 
                        && double.TryParse(textBoxCatch.Text,out double c)
                        && int.TryParse(textBoxBoats.Text,out int b))
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
                        if(textBoxCatch.Text.Length>0 || textBoxBoats.Text.Length>0)
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
        private void OnGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedVesselUnload = (VesselUnload)GridVesselUnload.SelectedItem;
            if(_vesselUnloadWindow!=null)
            {
                _vesselUnloadWindow.VesselUnload = _selectedVesselUnload;
            }
                
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            ((MainWindow)Owner).Focus();
        }
    }   
}
