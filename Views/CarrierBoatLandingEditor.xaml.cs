using NSAP_ODK.Entities.Database;
using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
//using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for CarrierBoatLandingEditor.xaml
    /// </summary>
    public partial class CarrierBoatLandingEditor : Window
    {
        private CarrierLanding _carrierLanding;
        private TreeViewItem _catchCompositionItem;
        private bool _treeSelectionChanged;
        private bool _gridIsCatchData;
        private string _currentTreeNode;
        private VesselCatch _selectedCatch;
        public CarrierBoatLandingEditor(CarrierLanding cl)
        {
            InitializeComponent();

            buttonCancel.Visibility = Visibility.Collapsed;
            buttonOK.Visibility = Visibility.Collapsed;

            propertyGrid.AutoGenerateProperties = false;
            propertyGrid.ShowAdvancedOptions = false;
            propertyGrid.ShowSearchBox = false;
            propertyGrid.ShowSortOptions = false;
            propertyGrid.ShowSummary = false;

            gridRowCarrier.Height = new GridLength(1, GridUnitType.Star);
            gridRowCatch.Height = new GridLength(0);
            gridRowLengths.Height = new GridLength(0);


            dataGridLengths.AutoGenerateColumns = false;
            dataGridLengths.CanUserAddRows = false;
            dataGridLengths.IsReadOnly = true;

            dataGrid.AutoGenerateColumns = false;
            dataGrid.CanUserAddRows = false;
            dataGrid.IsReadOnly = true;



            treeView.SelectedItemChanged += OnTreeSelectionChanged;
            dataGrid.SelectionChanged += OnDataGridSelectionChanged;
            _carrierLanding = cl;
            labelTitle.Content = $"Sampling of {cl.CarrierName} landed on {cl.SamplingDate.ToString("MMMM dd, yyyy")}";


            treeView.Items.Add(new TreeViewItem { Header = "Carrier", IsSelected = true });
            treeView.Items.Add(new TreeViewItem { Header = "Catchers" });
            treeView.Items.Add(new TreeViewItem { Header = "Fishing grounds" });

            _catchCompositionItem = new TreeViewItem { Header = "Catch" };
            treeView.Items.Add(_catchCompositionItem);

        }

        private void OnDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_treeSelectionChanged && _catchCompositionItem.Items.Count == 0 && _gridIsCatchData)
            {
                AddCatchCompositionTreeNodes();
                _catchCompositionItem.IsExpanded = true;
                _selectedCatch = (VesselCatch)dataGrid.SelectedItem;
            }
            else if (_catchCompositionItem.Items.Count > 0)
            {
                if(gridRowLengths.Height.Value>0)
                {
                    dataGridLengths.ItemsSource = null;
                }
                _selectedCatch = (VesselCatch)dataGrid.SelectedItem;
            }
        }

        private void ClearCatchCompositionTreeNodes()
        {
            _catchCompositionItem.Items.Clear();
        }
        private void AddCatchCompositionTreeNodes()
        {
            _catchCompositionItem.Items.Add(new TreeViewItem { Header = "Catch length" });
            _catchCompositionItem.Items.Add(new TreeViewItem { Header = "Length frequency" });
            _catchCompositionItem.Items.Add(new TreeViewItem { Header = "Length-weight" });
            _catchCompositionItem.Items.Add(new TreeViewItem { Header = "Gonadal maturity" });
        }
        private void OnTreeSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _gridIsCatchData = false;

            gridRowCarrier.Height = new GridLength(0);
            gridRowCatch.Height = new GridLength(0);
            gridRowLengths.Height = new GridLength(0);


            TreeViewItem tvi = (TreeViewItem)((TreeView)sender).SelectedItem;
            switch (tvi.Header.ToString())
            {
                case "Carrier":
                    _treeSelectionChanged = true;
                    _selectedCatch = null;
                    ClearCatchCompositionTreeNodes();
                    propertyGrid.Visibility = Visibility.Visible;
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "CarrierName", DisplayName = "Name" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "WeightOfCatch", DisplayName = "Weight of catch" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "SamplingDate", DisplayName = "Sampling date" });
                    propertyGrid.SelectedObject = _carrierLanding;

                    gridRowCarrier.Height = new GridLength(1, GridUnitType.Star);
                    labelCarrierProperties.Content = $"Properties of landings for {_carrierLanding.CarrierName}";
                    break;
                case "Catchers":
                    _treeSelectionChanged = true;
                    _selectedCatch = null;
                    ClearCatchCompositionTreeNodes();
                    if (_carrierLanding.CatcherBoatOperation_ViewModel == null)
                    {
                        _carrierLanding.CatcherBoatOperation_ViewModel = new CatcherBoatOperation_ViewModel(_carrierLanding);
                    }
                    dataGrid.Columns.Clear();
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Catcher boat", Binding = new Binding("CatcherBoatName") });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Weight of catch", Binding = new Binding("WeightOfCatch") });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("Gear") });

                    DataGridTextColumn col = new DataGridTextColumn
                    {
                        Header = "Date of operation",
                        Binding = new Binding("StartOfOperation")
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy";
                    dataGrid.Columns.Add(col);

                    dataGrid.ItemsSource = _carrierLanding.CatcherBoatOperation_ViewModel.CatcherBoatOperationCollection;

                    gridRowCatch.Height = new GridLength(1,GridUnitType.Star);
                    labelGridData.Content = $"Catch boats of {_carrierLanding.CarrierName}";
                    break;
                case "Fishing grounds":
                    _selectedCatch = null;
                    ClearCatchCompositionTreeNodes();
                    _treeSelectionChanged = true;
                    if (_carrierLanding.CarrierBoatLanding_FishingGround_ViewModel == null)
                    {
                        _carrierLanding.CarrierBoatLanding_FishingGround_ViewModel = new CarrierBoatLanding_FishingGround_ViewModel(_carrierLanding);
                    }
                    dataGrid.Columns.Clear();
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("FishingGround") });
                    dataGrid.ItemsSource = _carrierLanding.CarrierBoatLanding_FishingGround_ViewModel.CarrierBoatLanding_FishingGroundCollection;

                    gridRowCatch.Height = new GridLength(1,GridUnitType.Star);
                    labelGridData.Content = $"Fishing grounds of {_carrierLanding.CarrierName}";
                    break;
                case "Catch":
                    //ClearCatchCompositionTreeNodes();

                    _treeSelectionChanged = true;
                    if (_carrierLanding.VesselCatchViewModel == null)
                    {
                        _carrierLanding.VesselCatchViewModel = new VesselCatchViewModel(_carrierLanding);
                    }
                    dataGrid.Columns.Clear();
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Taxa", Binding = new Binding("Taxa") });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Species name", Binding = new Binding("CatchName") });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Weight", Binding = new Binding("Catch_kg") });
                    dataGrid.ItemsSource = _carrierLanding.VesselCatchViewModel.VesselCatchCollection;
                    
                    gridRowCatch.Height = new GridLength(1,GridUnitType.Star);
                    _gridIsCatchData = true;
                    labelGridData.Content = $"Catch composition of {_carrierLanding.CarrierName}";
                    break;
                case "Catch length":
                case "Length frequency":
                case "Length-weight":
                case "Gonadal maturity":
                    _gridIsCatchData = true;
                    gridRowCatch.Height = new GridLength(1, GridUnitType.Auto);
                    gridRowLengths.Height = new GridLength(1, GridUnitType.Star);
                    dataGridLengths.Columns.Clear();
                    switch (tvi.Header.ToString())
                    {
                        case "Catch length":
                            if (_selectedCatch.CatchLengthViewModel == null)
                            {
                                _selectedCatch.CatchLengthViewModel = new CatchLengthViewModel(_selectedCatch);
                            }

                            dataGridLengths.Columns.Add(new DataGridTextColumn { Header = "Length", Binding = new Binding("Length") });
                            dataGridLengths.ItemsSource = _selectedCatch.CatchLengthViewModel.CatchLengthCollection;
                            labelGridLengths.Content = $"Length measurement of {_selectedCatch.CatchName}";
                            break;

                        case "Length frequency":
                            if (_selectedCatch.CatchLenFreqViewModel == null)
                            {
                                _selectedCatch.CatchLenFreqViewModel = new CatchLenFreqViewModel(_selectedCatch);
                            }

                            dataGridLengths.Columns.Add(new DataGridTextColumn { Header = "Length", Binding = new Binding("LengthClass") });
                            dataGridLengths.Columns.Add(new DataGridTextColumn { Header = "Frequency", Binding = new Binding("Frequency") });
                            dataGridLengths.ItemsSource = _selectedCatch.CatchLenFreqViewModel.CatchLenFreqCollection;
                            labelGridLengths.Content = $"Length frequency measurement of {_selectedCatch.CatchName}";
                            break;
                        case "Length-weight":
                            if (_selectedCatch.CatchLengthWeightViewModel == null)
                            {
                                _selectedCatch.CatchLengthWeightViewModel = new CatchLengthWeightViewModel(_selectedCatch);
                            }

                            dataGridLengths.Columns.Add(new DataGridTextColumn { Header = "Length", Binding = new Binding("Length") });
                            dataGridLengths.Columns.Add(new DataGridTextColumn { Header = "Weight", Binding = new Binding("Weight") });
                            dataGridLengths.ItemsSource = _selectedCatch.CatchLengthWeightViewModel.CatchLengthWeightCollection;
                            labelGridLengths.Content = $"Length-weight measurement of {_selectedCatch.CatchName}";
                            break;
                        case "Gonadal maturity":
                            if (_selectedCatch.CatchMaturityViewModel == null)
                            {
                                _selectedCatch.CatchMaturityViewModel = new CatchMaturityViewModel(_selectedCatch);
                            }
                            dataGridLengths.Columns.Add(new DataGridTextColumn { Header = "Length", Binding = new Binding("Length") });
                            dataGridLengths.Columns.Add(new DataGridTextColumn { Header = "Weight", Binding = new Binding("Weight") });
                            dataGridLengths.Columns.Add(new DataGridTextColumn { Header = "Sex", Binding = new Binding("Sex") });
                            dataGridLengths.Columns.Add(new DataGridTextColumn { Header = "Maturity", Binding = new Binding("Maturity") });
                            dataGridLengths.Columns.Add(new DataGridTextColumn { Header = "Gonad weight", Binding = new Binding("GonadWeight") });
                            dataGridLengths.Columns.Add(new DataGridTextColumn { Header = "Gut content", Binding = new Binding("GutContentClassification") });
                            dataGridLengths.Columns.Add(new DataGridTextColumn { Header = "Gut content weight", Binding = new Binding("WeightGutContent") });
                            dataGridLengths.ItemsSource = _selectedCatch.CatchMaturityViewModel.CatchMaturityCollection;
                            labelGridLengths.Content = $"Maturity measurement of {_selectedCatch.CatchName}";
                            break;
                    }
                    break;
            }
            _currentTreeNode = ((TreeViewItem)treeView.SelectedItem).Header.ToString();
            _treeSelectionChanged = false;
        }

        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonOk":
                    break;
                case "buttonCancel":
                    break;
                case "buttonClose":
                    Close();
                    break;
            }
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            //dataGrid.Visibility = Visibility.Collapsed;
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }


        private void ClosingTrigger(object sender, CancelEventArgs e)
        {
            this.SavePlacement();
        }
    }
}
