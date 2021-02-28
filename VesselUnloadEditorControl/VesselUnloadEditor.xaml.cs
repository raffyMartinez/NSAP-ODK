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
using System.Windows.Navigation;
using System.Windows.Shapes;
using NSAP_ODK.Entities;
using NSAP_ODK.Entities.Database;
using NSAP_ODK.Utilities;
using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace NSAP_ODK.VesselUnloadEditorControl
{

    /// <summary>
    /// Interaction logic for VesselUnloadEdit.xaml
    /// </summary>
    public partial class VesselUnloadEditor : UserControl
    {

        private VesselUnload _vesselUnload;
        private VesselUnloadEdit _vesselUnloadEdit;
        private string _unloadView;
        private bool _editMode;
        private NSAPRegionFMA _nsapRegionFMA;
        private NSAPRegion _nsapRegion;
        private NSAPRegionFMAFishingGround _nsapRegionFMAFishingGround;
        private Dictionary<string, int> _dictProperties = new Dictionary<string, int>();

        public VesselUnloadEditor()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
            {
                InitializeComponent();
            }
        }
        public VesselUnload VesselUnload
        {
            get { return _vesselUnload; }
            set
            {
                _vesselUnload = value;
                _vesselUnloadEdit = new VesselUnloadEdit(_vesselUnload);
            }
        }

        public bool EditMode
        {
            get { return _editMode; }
            set
            {
                _editMode = value;

            }
        }
        private void ResetView()
        {
            rowPropertyGrid.Height = new GridLength(1, GridUnitType.Star);
            rowDataGrid.Height = new GridLength(0);
            effortDataGrid.SetValue(Grid.ColumnSpanProperty, 2);
            catchDataGrid.Visibility = Visibility.Collapsed;
            labelEffort.SetValue(Grid.ColumnSpanProperty, 2);
            labelCatch.Visibility = Visibility.Collapsed;
        }
        private void SetupView()
        {
            ResetView();
            switch (_unloadView)
            {
                case "treeItemVesselUnload":
                    break;
                case "treeItemSoakTime":
                case "treeItemFishingGround":
                case "treeItemEffortDefinition":
                case "treeItemCatchComposition":
                    rowPropertyGrid.Height = new GridLength(0);
                    rowDataGrid.Height = new GridLength(1, GridUnitType.Star);
                    break;
                case "treeItemLenFreq":
                case "treeItemLenWeight":
                case "treeItemLenList":
                case "treeItemMaturity":
                    rowPropertyGrid.Height = new GridLength(0);
                    rowDataGrid.Height = new GridLength(1, GridUnitType.Star);
                    effortDataGrid.SetValue(Grid.ColumnSpanProperty, 1);
                    catchDataGrid.Visibility = Visibility.Visible;

                    labelEffort.SetValue(Grid.ColumnSpanProperty, 1);
                    labelCatch.Visibility = Visibility.Visible;

                    break;
            }
        }

        private void SetupPropertyGrid()
        {
            propertyGrid.SelectedObject = _vesselUnloadEdit;
            propertyGrid.NameColumnWidth = 350;
            propertyGrid.AutoGenerateProperties = false;

            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "RegionCode", DisplayName = "NSAP Region", DisplayOrder = 1, Description = "NSAP region", Category = "Header" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FMAID", DisplayName = "FMA", DisplayOrder = 2, Description = "FMA", Category = "Header" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FishingGroundCode", DisplayName = "Fishing ground", DisplayOrder = 3, Description = "Fishing ground", Category = "Header" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "LandingSiteID", DisplayName = "Landing site", DisplayOrder = 4, Description = "Landing site", Category = "Header" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GearCode", DisplayName = "Fishing gear", DisplayOrder = 5, Description = "Fishing gear", Category = "Header" });

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
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "RaisingFactor", DisplayName = "Raising factor", DisplayOrder = 14, Description = "Raising factor", Category = "Effort" });
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


            for(int x=0;x<propertyGrid.Properties.Count;x++)
            {
                _dictProperties.Add(((PropertyItem)propertyGrid.Properties[x]).PropertyName, x);
            }

            _nsapRegion = _vesselUnloadEdit.NSAPRegion;
            _nsapRegionFMA = _nsapRegion.FMAs.Where(t => t.FMAID == _vesselUnloadEdit.FMAID).FirstOrDefault();
            _nsapRegionFMAFishingGround = _nsapRegionFMA.FishingGrounds.Where(t => t.FishingGroundCode == _vesselUnloadEdit.FishingGroundCode).FirstOrDefault();
        }

        public string UnloadView
        {
            get { return _unloadView; }
            set
            {
                _unloadView = value;
                SetupView();
                switch (_unloadView)
                {
                    case "treeItemVesselUnload":
                        SetupPropertyGrid();
                        break;
                    case "treeItemSoakTime":
                        break;
                    case "treeItemFishingGround":
                        break;
                    case "treeItemEffortDefinition":
                        break;
                    case "treeItemCatchComposition":
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
            }
        }
        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {

        }

        private void OnPropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            ComboBox cbo = new ComboBox();
            cbo.Items.Clear();
            cbo.SelectionChanged += OnComboSelectionChanged;
            cbo.DisplayMemberPath = "Value";
            var currentProperty = (PropertyItem)e.OriginalSource;

            switch (currentProperty.PropertyName)
            {
                case "RegionCode":
                    cbo.Tag = "fma";
                    foreach(var fma in NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(currentProperty.Value.ToString()).FMAs)
                    {
                        cbo.Items.Add(new KeyValuePair<int,string>(fma.FMAID,fma.FMA.Name));
                    }

                    _nsapRegion= NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(currentProperty.Value.ToString());
                    ((PropertyItem)propertyGrid.Properties[_dictProperties["FMAID"]]).Editor = cbo;
                    ((PropertyItem)propertyGrid.Properties[_dictProperties["FishingGroundCode"]]).Editor = new ComboBox();
                    ((PropertyItem)propertyGrid.Properties[_dictProperties["LandingSiteID"]]).Editor = new ComboBox();

                    ComboBox gearEditor = new ComboBox();
                    gearEditor.Tag = "gear";
                    gearEditor.SelectionChanged += OnComboSelectionChanged;
                    gearEditor.DisplayMemberPath = "Value";
                    foreach(var gear in _nsapRegion.Gears.OrderBy(t=>t.Gear.GearName))
                    {
                        gearEditor.Items.Add(new KeyValuePair<string, string>(gear.GearCode, gear.Gear.GearName));
                    }
                    ((PropertyItem)propertyGrid.Properties[_dictProperties["GearCode"]]).Editor = gearEditor;


                    ComboBox enumeratorEditor = new ComboBox();
                    enumeratorEditor.Tag = "enumerator";
                    enumeratorEditor.SelectionChanged += OnComboSelectionChanged;
                    enumeratorEditor.DisplayMemberPath = "Value";
                    foreach (var enumer in _nsapRegion.NSAPEnumerators.OrderBy(t => t.Enumerator.Name))
                    {
                        enumeratorEditor.Items.Add(new KeyValuePair<int, string>(enumer.EnumeratorID, enumer.Enumerator.Name));
                    }
                    ((PropertyItem)propertyGrid.Properties[_dictProperties["NSAPEnumeratorID"]]).Editor = enumeratorEditor;


                    break;
                case "FMAID":
                    cbo.Tag = "fishing_ground";
                   _nsapRegionFMA= _nsapRegion.FMAs.Where(t => t.FMAID == int.Parse(currentProperty.Value.ToString())).FirstOrDefault();
                    foreach(var fg in _nsapRegionFMA.FishingGrounds)
                    {
                        cbo.Items.Add(new KeyValuePair<string, string>(fg.FishingGroundCode, fg.FishingGround.Name));
                    }


                    ((PropertyItem)propertyGrid.Properties[_dictProperties["FishingGroundCode"]]).Editor = cbo;
                    ((PropertyItem)propertyGrid.Properties[_dictProperties["LandingSiteID"]]).Editor = new ComboBox();
                    break;
                case "FishingGroundCode":
                    cbo.Tag = "landing_site";
                    NSAPEntities.NSAPRegionFMAFishingGround = _nsapRegionFMA.FishingGrounds.Where(t => t.FishingGroundCode == currentProperty.Value.ToString()).FirstOrDefault();
                    foreach(var ls in NSAPEntities.NSAPRegionFMAFishingGround.LandingSites)
                    {
                        cbo.Items.Add(new KeyValuePair<int, string>(ls.LandingSite.LandingSiteID, ls.LandingSite.ToString()));
                    }


                    ((PropertyItem)propertyGrid.Properties[_dictProperties["LandingSiteID"]]).Editor = cbo;
                    break;
                case "LandingSiteID":
                    break;
                case "GearCode":
                    break;
            }

        }

        private void OnComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;
            ComboBox editor = new ComboBox();
            editor.SelectionChanged += OnComboSelectionChanged;
            editor.DisplayMemberPath = "Value";
            switch (cbo.Tag.ToString())
            {
                case "fma":
                    int fmaID = ((KeyValuePair<int, string>)cbo.SelectedItem).Key;
                    ((PropertyItem)propertyGrid.Properties[_dictProperties["FMAID"]]).Value=fmaID;
                    _nsapRegionFMA = _nsapRegion.FMAs.Where(t => t.FMAID == fmaID).FirstOrDefault();

                    
                    
                    foreach(var fg in _nsapRegionFMA.FishingGrounds)
                    {
                        editor.Items.Add(new KeyValuePair<string, string>(fg.FishingGroundCode, fg.FishingGround.Name));
                    }

                    ((PropertyItem)propertyGrid.Properties[_dictProperties["FishingGroundCode"]]).Editor = editor;
                    editor.Tag = "fishing_ground";

                    break;
                case "fishing_ground":
                    string fgCode = ((KeyValuePair<string, string>)cbo.SelectedItem).Key;
                    _nsapRegionFMAFishingGround = _nsapRegionFMA.FishingGrounds.Where(t => t.FishingGroundCode == fgCode).FirstOrDefault();
                    ((PropertyItem)propertyGrid.Properties[_dictProperties["FishingGroundCode"]]).Value=fgCode;

                    foreach(var ls in _nsapRegionFMAFishingGround.LandingSites)
                    {
                        editor.Items.Add(new KeyValuePair<int, string>(ls.LandingSite.LandingSiteID, ls.LandingSite.ToString()));
                    }

                    ((PropertyItem)propertyGrid.Properties[_dictProperties["LandingSiteID"]]).Editor = editor;
                    editor.Tag = "landing_site";
                    break;
                case "landing_site":
                    foreach (PropertyItem prp in propertyGrid.Properties)
                    {
                        if (prp.PropertyName == "LandingSiteID")
                        {
                            prp.Value = ((KeyValuePair<int, string>)cbo.SelectedItem).Key;
                            return;
                        }
                    }
                    break;
                case "gear":
                    ((PropertyItem)propertyGrid.Properties[_dictProperties["GearCode"]]).Value= ((KeyValuePair<string,string>)cbo.SelectedItem).Key;
                    break;
                case "enumerator":
                    ((PropertyItem)propertyGrid.Properties[_dictProperties["NSAPEnumeratorID"]]).Value= ((KeyValuePair<int,string>)cbo.SelectedItem).Key;                    
                    break;
            }
        }
    }
}
