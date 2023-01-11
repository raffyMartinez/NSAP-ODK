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
using NSAP_ODK.Utilities;
using Xceed.Wpf.Toolkit.PropertyGrid;
using NSAP_ODK.Entities;
namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for VesselUnloadWIndow.xaml
    /// </summary>
    public partial class VesselUnloadWIndow : Window
    {
        private VesselUnload _vesselUnload;
        private VesselUnloadEdit _vesselUnloadEdit;
        private string _selectedTreeItem;
        private VesselCatch _selectedCatch;
        private GearUnloadWindow _parentWindow;
        private MainWindow _parentMainWindow;
        private bool _editMode;
        private bool _unloadGridIsDirty;
        private bool _catchGridIsDirty;

        public VesselUnloadWIndow(VesselUnload vesselUnload, MainWindow parent)
        {
            InitializeComponent();
            _vesselUnload = vesselUnload;
            _vesselUnloadEdit = new VesselUnloadEdit(_vesselUnload);
            _parentMainWindow = parent;
        }
        public VesselUnloadWIndow(VesselUnload vesselUnload, GearUnloadWindow parent)
        {
            InitializeComponent();
            _vesselUnload = vesselUnload;
            _vesselUnloadEdit = new VesselUnloadEdit(_vesselUnload);
            _parentWindow = parent;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            treeItemVesselUnload.IsSelected = true;
        }



        public VesselUnload VesselUnload
        {
            set
            {
                _vesselUnload = value;
                _vesselUnloadEdit = new VesselUnloadEdit(_vesselUnload);
                _selectedTreeItem = "treeItemVesselUnload";
                treeItemVesselUnload.IsSelected = true;
                propertyGrid.SelectedObject = null;
                ShowSelectedItemDetails();


            }
            get
            {
                return _vesselUnload;
            }
        }
        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonEdit":
                    _editMode = !_editMode;
                    if (_editMode)
                    {
                        buttonEdit.Background = Brushes.Yellow;
                        buttonEdit.Content = "Stop edits";
                        if (_selectedTreeItem != "treeItemVesselUnload")
                        {
                            panelLowerButtons.Visibility = Visibility.Visible;
                        }

                    }
                    else
                    {
                        buttonEdit.Background = Brushes.SkyBlue;
                        buttonEdit.Content = "Edit";
                        panelLowerButtons.Visibility = Visibility.Collapsed;
                    }

                    gridSelectedCatchProperty.CanUserAddRows = _editMode;
                    gridVesselUnload.CanUserAddRows = _editMode;
                    SetGridForEdit();
                    break;
                case "buttonOk":
                    break;
                case "buttonCancel":
                    Close();
                    break;
            }
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
        private void SetGridForEdit()
        {
            DataGridTextColumn col;
            _unloadGridIsDirty = false;
            _catchGridIsDirty = false;

            switch (_selectedTreeItem)
            {
                case "treeItemSoakTime":
                    gridVesselUnload.Columns.Clear();
                    if (_vesselUnloadEdit.OperationIsTracked || !_editMode)
                    {
                        gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("PK") });
                        gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Time at set", Binding = new Binding("TimeAtSet") });
                        gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Time at haul", Binding = new Binding("TimeAtHaul") });
                        gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Waypoint at set", Binding = new Binding("WaypointAtSet") });
                        gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Waypoint at haul", Binding = new Binding("WaypointAtHaul") });
                    }
                    else
                    {
                        gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("PK") });
                        gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Time at set", Binding = new Binding("TimeAtSet") });
                        gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Time at haul", Binding = new Binding("TimeAtHaul") });
                    }
                    break;
                case "treeItemFishingGround":
                    break;
                case "treeItemEffortDefinition":
                    break;
                case "treeItemCatchComposition":
                    gridVesselUnload.Columns.Clear();
                    if (!_editMode)
                    {
                        ConfigureGridColumns();
                        gridVesselUnload.DataContext = _vesselUnload.ListVesselCatch;
                    }
                    else
                    {
                        var editedCatchItems = NSAPEntities.VesselCatchViewModel.GetVesselCatchEditedList(_vesselUnload);
                        gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("PK") });

                        DataGridComboBoxColumn cboColumnTaxa = new DataGridComboBoxColumn();
                        cboColumnTaxa.Header = "Taxa";
                        cboColumnTaxa.ItemsSource = NSAPEntities.TaxaViewModel.AllTaxaTerms();
                        cboColumnTaxa.SelectedItemBinding = new Binding("TaxaName");
                        gridVesselUnload.Columns.Add(cboColumnTaxa);


                        //DataGridComboBoxColumn cboColumnGenus = new DataGridComboBoxColumn();
                        //cboColumnGenus.Header = "Genus";
                        //cboColumnGenus.ItemsSource = GenericNameViewModel.();
                        //cboColumnGenus.SelectedItemBinding = new Binding("Genus");
                        //gridVesselUnload.Columns.Add(cboColumnGenus);

                        col = new DataGridTextColumn()
                        {
                            Binding = new Binding("Catch_kg"),
                            Header = "Weight",
                            CellStyle = AlignRightStyle
                        };
                        col.Binding.StringFormat = "0.00";
                        gridVesselUnload.Columns.Add(col);

                        col = new DataGridTextColumn()
                        {
                            Binding = new Binding("Sample_kg"),
                            Header = "Weight of sample (TWS)",
                            CellStyle = AlignRightStyle
                        };
                        col.Binding.StringFormat = "0.00";
                        gridVesselUnload.Columns.Add(col);

                        //gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Other", Binding = new Binding("Catch_kg") });
                        //gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Weight", Binding = new Binding("Catch_kg") });
                        gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Weight of sample", Binding = new Binding("Sample_kg") });
                        gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "TWS", Binding = new Binding("TWS") });

                        gridVesselUnload.DataContext = editedCatchItems;

                    }
                    break;
                case "treeItemLenFreq":
                    break;
                case "treeItemLenWeight":
                    break;
                case "treeItemLenList":
                    break;
                case "treeItemMaturity":
                    break;
            }


            gridVesselUnload.CanUserAddRows = _editMode; ;
            gridVesselUnload.IsReadOnly = !_editMode;

        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SavePlacement();
            if (_parentWindow != null)
            {
                _parentWindow.VesselWindowClosed();
            }
            else if (_parentMainWindow != null)
            {
                _parentMainWindow.VesselWindowClosed();
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }

        private void OnSelectedPropertyItemChanged(object sender, RoutedPropertyChangedEventArgs<Xceed.Wpf.Toolkit.PropertyGrid.PropertyItemBase> e)
        {

        }

        private void OnPropertyValueChanged(object sender, Xceed.Wpf.Toolkit.PropertyGrid.PropertyValueChangedEventArgs e)
        {

        }

        private void ConfigureGridColumns()
        {
            DataGridTextColumn col;
            if (_selectedTreeItem == "treeItemLenFreq"
                || _selectedTreeItem == "treeItemLenWeight"
                || _selectedTreeItem == "treeItemLenList"
                || _selectedTreeItem == "treeItemMaturity"
                )
            {
                gridSelectedCatchProperty.Columns.Clear();
                gridSelectedCatchProperty.AutoGenerateColumns = false;
            }
            else
            {
                gridVesselUnload.AutoGenerateColumns = false;
                gridVesselUnload.Columns.Clear();
            }



            switch (_selectedTreeItem)
            {
                case "treeItemSoakTime":
                    gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("PK") });
                    gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Time at set", Binding = new Binding("TimeAtSet") });
                    gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Time at haul", Binding = new Binding("TimeAtHaul") });
                    gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Waypoint at set", Binding = new Binding("WaypointAtSet") });
                    gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Waypoint at haul", Binding = new Binding("WaypointAtHaul") });
                    break;
                case "treeItemFishingGround":
                    gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("PK") });
                    gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "UTM Zone", Binding = new Binding("UTMZoneText") });
                    gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Grid", Binding = new Binding("Grid") });
                    break;
                case "treeItemEffortDefinition":
                    gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("PK") });
                    gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Effort specification", Binding = new Binding("EffortSpecification") });
                    gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Value", Binding = new Binding("EffortValue") });
                    break;
                case "treeItemCatchComposition":
                    gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("PK") });
                    gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Taxa", Binding = new Binding("Taxa") });
                    gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("CatchName") });

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("Catch_kg"),
                        Header = "Weight",
                        CellStyle = AlignRightStyle
                    };
                    col.Binding.StringFormat = "0.00";
                    gridVesselUnload.Columns.Add(col);

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("Sample_kg"),
                        Header = "Weight of sample (TWS)",
                        CellStyle = AlignRightStyle
                    };
                    col.Binding.StringFormat = "0.00";
                    gridVesselUnload.Columns.Add(col);

                    gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Price", Binding = new Binding("PriceOfSpecies") });
                    gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Unit", Binding = new Binding("PriceUnit") });

                    //gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Weight", Binding = new Binding("Catch_kg") });
                    //gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Weight of sample", Binding = new Binding("Sample_kg") });
                    gridVesselUnload.Columns.Add(new DataGridCheckBoxColumn { Header = "From total catch", Binding = new Binding("FromTotalCatch ") });
                    break;
                case "treeItemLenFreq":
                    gridSelectedCatchProperty.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("PK") });
                    gridSelectedCatchProperty.Columns.Add(new DataGridTextColumn { Header = "Length class", Binding = new Binding("LengthClass") });
                    gridSelectedCatchProperty.Columns.Add(new DataGridTextColumn { Header = "Frequency", Binding = new Binding("Frequency") });
                    break;
                case "treeItemLenWeight":
                    gridSelectedCatchProperty.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("PK") });
                    gridSelectedCatchProperty.Columns.Add(new DataGridTextColumn { Header = "Length", Binding = new Binding("Length") });
                    gridSelectedCatchProperty.Columns.Add(new DataGridTextColumn { Header = "Weight", Binding = new Binding("Weight") });
                    break;
                case "treeItemLenList":
                    gridSelectedCatchProperty.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("PK") });
                    gridSelectedCatchProperty.Columns.Add(new DataGridTextColumn { Header = "Length", Binding = new Binding("Length") });
                    break;
                case "treeItemMaturity":
                    gridSelectedCatchProperty.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("PK") });
                    gridSelectedCatchProperty.Columns.Add(new DataGridTextColumn { Header = "Length", Binding = new Binding("Length") });
                    gridSelectedCatchProperty.Columns.Add(new DataGridTextColumn { Header = "Weight", Binding = new Binding("Weight") });
                    gridSelectedCatchProperty.Columns.Add(new DataGridTextColumn { Header = "Sex", Binding = new Binding("Sex") });
                    gridSelectedCatchProperty.Columns.Add(new DataGridTextColumn { Header = "Maturity", Binding = new Binding("Maturity") });
                    gridSelectedCatchProperty.Columns.Add(new DataGridTextColumn { Header = "Gonad weight", Binding = new Binding("GonadWeight") });
                    gridSelectedCatchProperty.Columns.Add(new DataGridTextColumn { Header = "Weight gut content", Binding = new Binding("WeightGutContent") });
                    gridSelectedCatchProperty.Columns.Add(new DataGridTextColumn { Header = "Gut content classification", Binding = new Binding("GutContentClassification") });
                    break;
            }
        }

        private void SetCatchGridContext()
        {
            switch (_selectedTreeItem)
            {
                case "treeItemLenFreq":
                    labelCurrentView.Content = $"Length frequency distribution of {_selectedCatch.CatchName}";
                    if (_selectedCatch != null)
                        gridSelectedCatchProperty.DataContext = _selectedCatch.ListCatchLenFreq;
                    break;
                case "treeItemLenWeight":
                    labelCurrentView.Content = $"Length weight data of {_selectedCatch.CatchName}";
                    if (_selectedCatch != null)
                        gridSelectedCatchProperty.DataContext = _selectedCatch.ListCatchLengthWeight;
                    break;
                case "treeItemLenList":
                    labelCurrentView.Content = $"Length data of {_selectedCatch.CatchName}";
                    if (_selectedCatch != null)
                        gridSelectedCatchProperty.DataContext = _selectedCatch.ListCatchLength;
                    break;
                case "treeItemMaturity":
                    labelCurrentView.Content = $"Maturity, sex, length-weight, and gut content data of {_selectedCatch.CatchName}";
                    if (_selectedCatch != null)
                        gridSelectedCatchProperty.DataContext = _selectedCatch.ListCatchMaturity;
                    break;
            }
        }

        private void SetCatchTreeItemsVisibility(Visibility visibility)
        {
            treeItemLenFreq.Visibility = visibility;
            treeItemLenWeight.Visibility = visibility;
            treeItemLenList.Visibility = visibility;
            treeItemMaturity.Visibility = visibility;
        }

        private void ShowStatusCatchCOmposition1()
        {

        }
        private void ShowStatusCatchCOmposition()
        {
            Label lbl;
            statusBar.Items.Clear();
           if(_vesselUnload.VesselCatchViewModel==null)
            {
                _vesselUnload.VesselCatchViewModel = new VesselCatchViewModel(_vesselUnload);
            }
            if (_vesselUnload.VesselCatchViewModel?.Count > 0)
            {
                lbl = new Label { Content = $"Weight of catch: {((double)_vesselUnload.WeightOfCatch).ToString("0.00")}" };
                statusBar.Items.Add(lbl);

                string sample_wt = "0";
                if (_vesselUnload.WeightOfCatchSample != null)
                {
                    sample_wt = ((double)_vesselUnload.WeightOfCatchSample).ToString("0.00");
                }
                lbl = new Label { Content = $"Weight of sample: {sample_wt}" };
                lbl.Margin = new Thickness(5, 0, 0, 0);
                statusBar.Items.Add(lbl);

                string total_wt_catch_comp = "";
                total_wt_catch_comp = _vesselUnload.VesselCatchViewModel.VesselCatchCollection.Sum(t => (double)t.Catch_kg).ToString("0.00");

                lbl = new Label { Content = $"Total weight of catch composition: {total_wt_catch_comp}" };
                lbl.Margin = new Thickness(5, 0, 0, 0);
                statusBar.Items.Add(lbl);

                string total_sample_wt_catch_comp = "";
                total_sample_wt_catch_comp = _vesselUnload.VesselCatchViewModel.VesselCatchCollection
                    .Where(t => t.FromTotalCatch == false && t.Sample_kg != null)
                    .Sum(t => (double)t.Sample_kg)
                    .ToString("0.00");

                lbl = new Label { Content = $"Total weight of sampled species: {total_sample_wt_catch_comp}" };
                lbl.Margin = new Thickness(5, 0, 0, 0);
                statusBar.Items.Add(lbl);

                lbl = new Label { Content = $"Raising factor: {((double)_vesselUnload.RaisingFactor).ToString("N2")}" };
                lbl.Margin = new Thickness(5, 0, 0, 0);
                statusBar.Items.Add(lbl);
            }
            else
            {
                statusBar.Items.Add(new Label { Content = "Catch composition is empty" });
            }
        }
        private void ShowSelectedItemDetails()
        {
            panelLowerButtons.Visibility = Visibility.Collapsed;

            gridSelectedCatchProperty.Visibility = Visibility.Collapsed;

            if (_selectedTreeItem != "treeItemVesselUnload")
            {
                ConfigureGridColumns();
                if (_selectedTreeItem == "treeItemLenFreq"
                    || _selectedTreeItem == "treeItemLenWeight"
                    || _selectedTreeItem == "treeItemLenList"
                    || _selectedTreeItem == "treeItemMaturity"
                    )
                {
                    if (_selectedCatch != null)
                    {
                        SetCatchTreeItemsVisibility(Visibility.Visible);
                    }


                    gridSelectedCatchProperty.Visibility = Visibility.Visible;
                }
                else
                {

                    labelCurrentView.Content = string.Empty;
                    _selectedCatch = null;
                    gridSelectedCatchProperty.DataContext = null;
                    SetCatchTreeItemsVisibility(Visibility.Hidden);
                    //gridSelectedCatchProperty.Items.Clear();
                }

                if (_editMode)
                {
                    panelLowerButtons.Visibility = Visibility.Visible;
                }

                switch (_selectedTreeItem)
                {
                    case "treeItemSoakTime":
                        labelCurrentView.Content = "Soak time of gear";
                        gridVesselUnload.DataContext = _vesselUnload.ListGearSoak;
                        break;
                    case "treeItemFishingGround":
                        labelCurrentView.Content = "Grid location of fishing ground";
                        gridVesselUnload.DataContext = _vesselUnload.ListFishingGroundGrid;
                        break;
                    case "treeItemEffortDefinition":
                        labelCurrentView.Content = "Effort specifications of fishing operation";
                        gridVesselUnload.DataContext = _vesselUnload.ListVesselEffort;
                        break;
                    case "treeItemCatchComposition":
                        ShowStatusCatchCOmposition();
                        labelCurrentView.Content = "Catch composition";
                        gridVesselUnload.DataContext = _vesselUnload.ListVesselCatch;
                        break;
                    default:
                        SetCatchGridContext();
                        break;
                }

                propertyGrid.Visibility = Visibility.Collapsed;
                gridVesselUnload.Visibility = Visibility.Visible;

            }
            else
            {
                panelLowerButtons.Visibility = Visibility.Collapsed;
                SetCatchTreeItemsVisibility(Visibility.Collapsed);
                labelCurrentView.Content = "Details of sampled fishing operation";
                _selectedCatch = null;
                gridVesselUnload.Visibility = Visibility.Collapsed;
                propertyGrid.IsReadOnly = true;
                propertyGrid.Visibility = Visibility.Visible;
                //propertyGrid.SetValue(Grid.ColumnSpanProperty, 2);
                if (propertyGrid.SelectedObject == null)
                {
                    propertyGrid.SelectedObject = _vesselUnloadEdit;
                    propertyGrid.NameColumnWidth = 350;
                    propertyGrid.AutoGenerateProperties = false;

                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Region", DisplayName = "NSAP Region", DisplayOrder = 1, Description = "NSAP region", Category = "Header" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FMA", DisplayName = "FMA", DisplayOrder = 2, Description = "FMA", Category = "Header" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FishingGround", DisplayName = "Fishing ground", DisplayOrder = 3, Description = "Fishing ground", Category = "Header" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "LandingSite", DisplayName = "Landing site", DisplayOrder = 4, Description = "Landing site", Category = "Header" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Gear", DisplayName = "Fishing gear", DisplayOrder = 5, Description = "Fishing gear", Category = "Header" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "RefNo", DisplayName = "Ref #", DisplayOrder = 6, Description = "Reference number", Category = "Header" });

                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Identifier", DisplayName = "Database identifier", DisplayOrder = 1, Description = "Database identifier", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "SamplingDate", DisplayName = "Sampling date", DisplayOrder = 2, Description = "Date of sampling", Category = "Effort" });
                    //propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "NSAPRegionEnumeratorID", DisplayName = "Select name of enumerator", DisplayOrder = 3, Description = "Name of enumerator", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "NSAPEnumeratorID", DisplayName = "Select name of enumerator", DisplayOrder = 3, Description = "Name of enumerator", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "EnumeratorText", DisplayName = "Name of enumerator if not found in selection", DisplayOrder = 4, Description = "Type in name of enumerator", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "IsBoatUsed", DisplayName = "This fishing operation is using a fishing vessel", DisplayOrder = 5, Description = "This fishing operation is using a fishing vessel", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "VesselID", DisplayName = "Select name of vessel", DisplayOrder = 6, Description = "Select name of fishing vessel", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "VesselText", DisplayName = "Name of vessel if not found in selection", DisplayOrder = 7, Description = "Type in the name of vessel", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "SectorCode", DisplayName = "Fisheries sector", DisplayOrder = 8, Description = "Select fisheris sector", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "NumberOfFishers", DisplayName = "Number of fishers", DisplayOrder = 9, Description = "Number of fishers in this operation", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "OperationIsSuccessful", DisplayName = "This fishing operation is a success", DisplayOrder = 10, Description = "Is this fishing operation a success\r\nThe catch is not zero", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FishingTripIsCompleted", DisplayName = "This fishing trip is completed", DisplayOrder = 11, Description = "Is this fishing trip completed\r\nThe trip is not cancelled", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "WeightOfCatch", DisplayName = "Weight of catch", DisplayOrder = 12, Description = "Weight of catch", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "WeightOfCatchSample", DisplayName = "Weight of sample", DisplayOrder = 13, Description = "Weight of sample taken from catch", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Boxes", DisplayName = "Number of boxes", DisplayOrder = 14, Description = "Number of boxes", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "BoxesSampled", DisplayName = "Number of boxes sampled", DisplayOrder = 15, Description = "Number of boxes sampled", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "RaisingFactor", DisplayName = "Raising factor", DisplayOrder = 16, Description = "Raising factor", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "HasCatchComposition", DisplayName = "Catch composition included", DisplayOrder = 17, Description = "Whether or not catch composition is included in the sampled landing", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "IsCatchSold", DisplayName = "Is the catch sold at landing site", DisplayOrder = 18, Description = "Notes", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Notes", DisplayName = "Notes", DisplayOrder = 19, Description = "Notes", Category = "Effort" });


                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "OperationIsTracked", DisplayName = "This operation is tracked", DisplayOrder = 11, Description = "Is this fishing operation tracked", Category = "Tracking" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DepartureFromLandingSite", DisplayName = "Date and time of departure from landing site", DisplayOrder = 12, Description = "Date and time of departure from landing site", Category = "Tracking" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "ArrivalAtLandingSite", DisplayName = "Date and time of arrival at landing site", DisplayOrder = 13, Description = "Date and time of arrival atlanding site", Category = "Tracking" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GPSCode", DisplayName = "GPS", DisplayOrder = 14, Description = "GPS used in tracking", Category = "Tracking" });

                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "UserName", DisplayName = "User name saved in the device", DisplayOrder = 15, Description = "User name that was inputted and saved in the device", Category = "Device metadata" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DeviceID", DisplayName = "Identifier of the device", DisplayOrder = 16, Description = "Identifier of the device", Category = "Device metadata" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "XFormIdentifier", DisplayName = "XForm identifier", DisplayOrder = 17, Description = "Name of the downloaded excel file containing the data", Category = "Device metadata" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "XFormDate", DisplayName = "XForm date", DisplayOrder = 18, Description = "Date when the downloaded excel file was created", Category = "Device metadata" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FormVersion", DisplayName = "Version of the electronic form", DisplayOrder = 19, Description = "Version of the electronic form used in encoding", Category = "Device metadata" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Submitted", DisplayName = "Date when the electronic form was submitted to the net", DisplayOrder = 20, Description = "Date and time of submission of the encoded data", Category = "Device metadata" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DateAddedToDatabase", DisplayName = "Date added to database", DisplayOrder = 21, Description = "Date and time when data was added to the database", Category = "Device metadata" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FromExcelDownload", DisplayName = "Excel download", DisplayOrder = 22, Description = "Data was donwloaded to an Excel file", Category = "Device metadata" });
                }
            }
            //labelCurrentView.Visibility = gridSelectedCatchProperty.Visibility;
        }
        private void onTreeSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)

        {
            statusBar.Items.Clear();
            _selectedTreeItem = ((TreeViewItem)((TreeView)e.Source).SelectedValue).Name;
            ShowSelectedItemDetails();

        }

        private void OnGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((DataGrid)sender).Name)
            {
                case "gridVesselUnload":
                    if (gridVesselUnload.Items.CurrentItem != null && gridVesselUnload.Items.CurrentItem.GetType().Name == "VesselCatch")
                    {
                        _selectedCatch = (VesselCatch)gridVesselUnload.SelectedItem;
                        if (_selectedCatch != null)
                        {

                            SetCatchGridContext();
                            SetCatchTreeItemsVisibility(Visibility.Visible);
                        }
                    }
                    break;
                case "gridSelectedCatchProperty":
                    break;
            }
        }

        private void OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

        }
    }
}
