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
                case "buttonOk":
                    break;
                case "buttonCancel":
                    Close();
                    break;
            }
        }


        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SavePlacement();
            if (_parentWindow != null)
            {
                _parentWindow.VesselWindowClosed();
            }
            else if(_parentMainWindow != null)
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
                    gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("CatchName") });
                    gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Taxa", Binding = new Binding("Taxa") });
                    gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Weight", Binding = new Binding("Catch_kg") });
                    gridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Weight of sample", Binding = new Binding("Sample_kg") });
                    break;
                case "treeItemLenFreq":
                    gridSelectedCatchProperty.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("PK")});
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
                    gridSelectedCatchProperty.Columns.Add(new DataGridTextColumn { Header = "Weight gut content", Binding = new Binding("WeightGutContent") });
                    gridSelectedCatchProperty.Columns.Add(new DataGridTextColumn { Header = "Gut content classification", Binding = new Binding("GutContentClassification") });
                    break;
            }
        }

        private void SetCatchGridContext()
        {
            switch(_selectedTreeItem)
            {
                case "treeItemLenFreq":
                    labelCatchComp.Content = $"Length frequency distribution of {_selectedCatch.CatchName}";
                    if (_selectedCatch != null)
                        gridSelectedCatchProperty.DataContext = _selectedCatch.ListCatchLenFreq;
                    break;
                case "treeItemLenWeight":
                    labelCatchComp.Content = $"Length weight data of {_selectedCatch.CatchName}";
                    if (_selectedCatch != null)
                        gridSelectedCatchProperty.DataContext = _selectedCatch.ListCatchLengthWeight;
                    break;
                case "treeItemLenList":
                    labelCatchComp.Content = $"Length data of {_selectedCatch.CatchName}";
                    if (_selectedCatch != null)
                        gridSelectedCatchProperty.DataContext = _selectedCatch.ListCatchLength;
                    break;
                case "treeItemMaturity":
                    labelCatchComp.Content = $"Maturity, sex, length-weight, and gut content data of {_selectedCatch.CatchName}";
                    if (_selectedCatch != null)
                        gridSelectedCatchProperty.DataContext = _selectedCatch.ListCatchMaturity;
                    break;
            }
        }

        private void SetCatchTreeItemsVisibility(Visibility visibility )
        {
            treeItemLenFreq.Visibility = visibility;
            treeItemLenWeight.Visibility = visibility;
            treeItemLenList.Visibility = visibility;
            treeItemMaturity.Visibility = visibility;
        }

        private void ShowSelectedItemDetails()
        {
            gridVesselUnload.SetValue(Grid.ColumnSpanProperty, 2);
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

                    gridVesselUnload.SetValue(Grid.ColumnSpanProperty, 1);
                    gridSelectedCatchProperty.Visibility = Visibility.Visible;
                }
                else
                {

                    labelCatchComp.Content = string.Empty;
                    _selectedCatch = null;
                    gridSelectedCatchProperty.DataContext = null;
                    SetCatchTreeItemsVisibility(Visibility.Hidden);
                    //gridSelectedCatchProperty.Items.Clear();
                }

                switch (_selectedTreeItem)
                {
                    case "treeItemSoakTime":
                        labelUnload.Content = "Soak time of gear";
                        gridVesselUnload.DataContext = _vesselUnload.ListGearSoak;
                        break;
                    case "treeItemFishingGround":
                        labelUnload.Content = "Grid location of fishing ground";
                        gridVesselUnload.DataContext = _vesselUnload.ListFishingGroundGrid;
                        break;
                    case "treeItemEffortDefinition":
                        labelUnload.Content = "Effort specifications of fishing operation";
                        gridVesselUnload.DataContext = _vesselUnload.ListVesselEffort;
                        break;
                    case "treeItemCatchComposition":
                        labelUnload.Content = "Catch composition";
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
                SetCatchTreeItemsVisibility(Visibility.Collapsed);
                labelUnload.Content = "Details of sampled fishing operation";
                _selectedCatch = null;
                gridVesselUnload.Visibility = Visibility.Collapsed;
                propertyGrid.Visibility = Visibility.Visible;
                propertyGrid.SetValue(Grid.ColumnSpanProperty, 2);
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

                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Identifier", DisplayName = "Database identifier", DisplayOrder = 1, Description = "Database identifier", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "SamplingDate", DisplayName = "Sampling date", DisplayOrder = 2, Description = "Date of sampling", Category = "Effort" });
                    //propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "NSAPRegionEnumeratorID", DisplayName = "Select name of enumerator", DisplayOrder = 3, Description = "Name of enumerator", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "NSAPEnumeratorID", DisplayName = "Select name of enumerator", DisplayOrder = 3, Description = "Name of enumerator", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "EnumeratorText", DisplayName = "Name of enumerator if not found in selection", DisplayOrder = 4, Description = "Type in name of enumerator", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "IsBoatUsed", DisplayName = "This fishing operation is using a fishing vessel", DisplayOrder = 5, Description = "This fishing operation is using a fishing vessel", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "VesselID", DisplayName = "Select name of vessel", DisplayOrder = 6, Description = "Select name of fishing vessel", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "VesselText", DisplayName = "Name of vessel if not found in selection", DisplayOrder = 7, Description = "Type in the name of vessel", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "SectorCode", DisplayName = "Fisheries sector", DisplayOrder = 8, Description = "Select fisheris sector", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "OperationIsSuccessful", DisplayName = "This fishing operation is a success", DisplayOrder = 9, Description = "Is this fishing operation a success\r\nThe catch is not zero", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "WeightOfCatch", DisplayName = "Weight of catch", DisplayOrder = 10, Description = "Weight of catch", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "WeightOfCatchSample", DisplayName = "Weight of sample", DisplayOrder = 11, Description = "Weight of sample taken from catch", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Boxes", DisplayName = "Number of boxes", DisplayOrder = 12, Description = "Number of boxes", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "BoxesSampled", DisplayName = "Number of boxes sampled", DisplayOrder = 13, Description = "Number of boxes sampled", Category = "Effort" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name="RaisingFactor", DisplayName="Raising factor", DisplayOrder=14, Description="Raising factor", Category="Effort"});
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Notes", DisplayName = "Notes", DisplayOrder = 15, Description = "Notes", Category = "Effort" });


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
            labelCatchComp.Visibility = gridSelectedCatchProperty.Visibility;
        }
        private void onTreeSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)

        {
            _selectedTreeItem = ((TreeViewItem)((TreeView)e.Source).SelectedValue).Name;
            ShowSelectedItemDetails();

        }

        private void OnGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((DataGrid)sender).Name)
            {
                case "gridVesselUnload":
                    if (gridVesselUnload.Items.CurrentItem.GetType().Name == "VesselCatch")
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
    }
}
