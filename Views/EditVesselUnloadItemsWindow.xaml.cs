using DocumentFormat.OpenXml.Presentation;
using NSAP_ODK.Entities;
using NSAP_ODK.Entities.Database;
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
using Xceed.Wpf.Toolkit.PropertyGrid;
using NSAP_ODK.Entities.Database;
using Xceed.Wpf.Toolkit;
using NSAP_ODK.Utilities;
using NPOI.OpenXmlFormats.Dml.Chart;
using DocumentFormat.OpenXml.Drawing.Diagrams;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for EditVesselUnloadItems.xaml
    /// </summary>
    public partial class EditVesselUnloadItemsWindow : Window
    {
        public static EditVesselUnloadItemsWindow _instance;
        private string _unloadView;
        private bool _isNew;
        private VesselUnloadEdit _vesselUnloadEdit;
        private DateTimePickerEditor _dtHaul;
        private DateTimePickerEditor _dtSet;
        private string _currentGearSpecGearName;
        public VesselUnloadEditWindow VesselUnloadEditWindow { get; set; }

        public GearSoakEdited GearSoakEdited { get; set; }
        public VesselCatch VesselCatch { get; internal set; }
        public CatchLengthEdited CatchLengthEdited { get; set; }
        public CatchLengthWeightEdited CatchLengthWeightEdited { get; set; }
        public CatchLenFreqEdited CatchLenFreqEdited { get; set; }
        public CatchMaturityEdited CatchMaturityEdited { get; set; }
        public FishingGroundGridEdited FishingGroundGridEdited { get; set; }
        public VesselUnloadEdit VesselUnloadEdit { get; set; }
        public VesselUnload_Gear_Spec_Edited VesselUnload_Gear_Spec_Edited { get; set; }
        public VesselUnload_FishingGear_Edited VesselUnload_FishingGear_Edited { get; set; }

        public VesselUnload_FishingGear VesselUnload_FishingGear { get; set; }
        public EditVesselUnloadItemsWindow(bool isNew = false)
        {
            InitializeComponent();
            _isNew = isNew;
            Closing += OnEditVesselUnloadItems_Closing;
            propertyGrid.PropertyValueChanged += OnPropertyGrid_PropertyValueChanged;
        }

        private void OnPropertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            ComboBox cbo = new ComboBox();
            cbo.Items.Clear();
            cbo.SelectionChanged += OnComboSelectionChanged;
            cbo.DisplayMemberPath = "Value";
            var currentProperty = (PropertyItem)e.OriginalSource;
            switch (currentProperty.PropertyName)
            {
                case "GearCode":
                    if (UnloadView == "treeItemEffortDefinition")
                    {
                        cbo.Items.Clear();
                        cbo.Tag = "gear effort specs";
                        Gear gear = NSAPEntities.GearViewModel.GetGear(VesselUnload_Gear_Spec_Edited.GearCode);
                        if (_currentGearSpecGearName != gear.GearName)
                        {

                        }
                        gear.GearEffortSpecificationViewModel = new GearEffortSpecificationViewModel(gear);
                        foreach (PropertyItem prp in propertyGrid.Properties)
                        {
                            if (prp.PropertyName == "EffortSpecID")
                            {
                                foreach (GearEffortSpecification ges in gear.GearEffortSpecificationViewModel.GearEffortSpecificationCollection
                                                                        .Where(t => t.Gear.Code == gear.Code))
                                {
                                    cbo.Items.Add(new KeyValuePair<int, string>(ges.EffortSpecificationID, ges.EffortSpecification.Name));
                                }

                                if (gear.BaseGear.Code != gear.Code)
                                {
                                    foreach (GearEffortSpecification ges in gear.BaseGear.GearEffortSpecificationViewModel.GearEffortSpecificationCollection)
                                    {
                                        cbo.Items.Add(new KeyValuePair<int, string>(ges.EffortSpecificationID, ges.EffortSpecification.Name));
                                    }
                                }

                                foreach (var spec in NSAPEntities.EffortSpecificationViewModel.GetBaseGearEffortSpecification())
                                {
                                    cbo.Items.Add(new KeyValuePair<int, string>(spec.ID, spec.Name));
                                }
                                prp.Editor = cbo;
                                break;
                            }
                        }
                    }
                    break;
                case "SpecValue":
                    if (UnloadView == "treeItemEffortDefinition")
                    {
                        string val = VesselUnload_Gear_Spec_Edited.SpecValue;
                        switch (NSAPEntities.EffortSpecificationViewModel.GetEffortSpecification((int)VesselUnload_Gear_Spec_Edited.EffortSpecID).ValueType)
                        {
                            case ODKValueType.isInteger:
                                VesselUnload_Gear_Spec_Edited.EffortValueNumeric = int.Parse(val);
                                break;
                            case ODKValueType.isDecimal:
                                VesselUnload_Gear_Spec_Edited.EffortValueNumeric = double.Parse(val);
                                break;
                            case ODKValueType.isText:
                                VesselUnload_Gear_Spec_Edited.EffortValueText = val;
                                break;
                            case ODKValueType.isBoolean:
                                break;

                        }
                    }
                    break;
            }
        }

        private void OnComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;
            switch (cbo.Tag.ToString())
            {
                case "gear effort specs":
                    foreach (PropertyItem prp in propertyGrid.Properties)
                    {
                        if (prp.PropertyName == "EffortSpecID")
                        {
                            prp.Value = ((KeyValuePair<int, string>)cbo.SelectedItem).Key;
                            break;
                        }
                    }
                    break;
            }
        }

        public EditVesselUnloadItemsWindow(string unLoadView, bool isNew = false)
        {
            InitializeComponent();
            Closing += OnEditVesselUnloadItems_Closing;
            _unloadView = unLoadView;
            _isNew = isNew;

        }

        public string UnloadView
        {
            get { return _unloadView; }
            set
            {
                _unloadView = value;
                VesselCatch = VesselUnloadEditWindow.UnloadEditor.VesselCatch;
                SetupUI();
            }
        }

        private void SetupUI()
        {
            propertyGrid.AutoGenerateProperties = false;
            propertyGrid.IsReadOnly = false;
            propertyGrid.NameColumnWidth = 220;
            switch (UnloadView)
            {
                case "treeItemFishingGears":
                    if (_isNew)
                    {
                        VesselUnload_FishingGear_Edited = new VesselUnload_FishingGear_Edited
                        {
                            RowID = VesselUnloadEdit.VesselUnload.VesselUnload_FishingGearsViewModel.NextRecordNumber
                        };
                    }
                    Entities.ItemSources.GearItemsSource.UnloadGears = null;
                    propertyGrid.SelectedObject = VesselUnload_FishingGear_Edited;
                    //propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Region", DisplayName = "NSAP Region", DisplayOrder = 1, Description = "NSAP region", Category = "Header" });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "RowID", DisplayName = "Row ID", Description = "Database identifier", DisplayOrder = 1 });

                    if (!VesselUnloadEdit.VesselUnload.IsMultiGear)
                    {
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GearUsedName", DisplayName = "Gear name", Description = "Name of gear", DisplayOrder = 2 });
                    }
                    else
                    {
                        if (VesselUnloadEdit.VesselUnload.Parent.GearID == VesselUnload_FishingGear_Edited.GearCode)
                        {
                            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GearUsedName", DisplayName = "Gear name", Description = "Name of gear", DisplayOrder = 2 });
                        }
                        else
                        {
                            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GearCode", DisplayName = "Gear", Description = "Name of gear", DisplayOrder = 2 });
                            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GearText", DisplayName = "Other gear", Description = "Other name of gear", DisplayOrder = 3 });
                        }
                    }

                    //propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GearCode", DisplayName = "Gear", Description = "Name of gear", DisplayOrder = 2 });
                    //propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GearText", DisplayName = "Other gear", Description = "Other name of gear", DisplayOrder = 3 });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "WeightOfCatch", DisplayName = "Weight of catch", Description = "Weight of the catch in kilograms", DisplayOrder = 4 });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "WeightOfSample", DisplayName = "Weight of sample", Description = "Weight of the sample from the catch in kilograms", DisplayOrder = 5 });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "NumberOfSpeciesInCatch", DisplayName = "Number of species in the catch", Description = "Number of species in the catch composition", DisplayOrder = 6 });

                    labelTitle.Content = "Edit fishing gear";

                    break;
                case "treeItemSoakTime":
                    if (_isNew)
                    {
                        GearSoakEdited = new GearSoakEdited
                        {
                            PK = VesselUnloadEdit.VesselUnload.GearSoakViewModel.NextRecordNumber
                        };
                    }
                    propertyGrid.SelectedObject = GearSoakEdited;
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "PK", DisplayName = "Database identifier", Description = "Database identifier", DisplayOrder = 1 });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "TimeAtSet", DisplayName = "Time gear was set", Description = "Time of setting gear", DisplayOrder = 2 });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "TimeAtHaul", DisplayName = "Time gear was hauled", Description = "Time of hauling gear", DisplayOrder = 3 });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "WaypointAtSet", DisplayName = "Waypoint at gear set", Description = "Waypoint of setting gear", DisplayOrder = 4 });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "WaypointAtHaul", DisplayName = "Waypoint at gear haul", Description = "Waypoint of hauling gear", DisplayOrder = 5 });

                    foreach (PropertyItem prp in propertyGrid.Properties)
                    {
                        if (prp.PropertyName == "TimeAtSet")
                        {
                            _dtSet = new DateTimePickerEditor
                            {
                                Format = DateTimeFormat.Custom,
                                FormatString = "MMM, dd yyyy HH:mm",
                                Value = _isNew ? VesselUnloadEdit.VesselUnload.SamplingDate : GearSoakEdited.TimeAtSet,
                                TimePickerVisibility = Visibility.Visible

                            };
                            prp.Editor = _dtSet;

                        }
                        else if (prp.PropertyName == "TimeAtHaul")
                        {
                            _dtHaul = new DateTimePickerEditor
                            {
                                Format = DateTimeFormat.Custom,
                                FormatString = "MMM, dd yyyy HH:mm",
                                Value = _isNew ? VesselUnloadEdit.VesselUnload.SamplingDate : GearSoakEdited.TimeAtHaul,
                                TimePickerVisibility = Visibility.Visible
                            };
                            prp.Editor = _dtHaul;
                            break;
                        }
                    }
                    labelTitle.Content = "Edit soak time of fishing gear";
                    break;
                case "treeItemFishingGround":
                    if (_isNew)
                    {
                        FishingGroundGridEdited = new FishingGroundGridEdited
                        {
                            PK = VesselUnloadEdit.VesselUnload.FishingGroundGridViewModel.NextRecordNumber
                        };
                    }
                    propertyGrid.SelectedObject = FishingGroundGridEdited;
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "PK", DisplayName = "Database identifier", Description = "Database identifier", DisplayOrder = 1 });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "UTMZoneText", DisplayName = "UTM zone", Description = "UTM zone", DisplayOrder = 2 });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Grid", DisplayName = "Grid location", Description = "Grid location of fishing ground", DisplayOrder = 3 });
                    labelTitle.Content = "Edit fishing ground";
                    break;
                case "treeItemEffortDefinition":
                    if (_isNew)
                    {
                        VesselEffortRepository ver = new VesselEffortRepository();
                        VesselUnload_Gear_Spec_Edited = new VesselUnload_Gear_Spec_Edited
                        {
                            RowID = ver.MaxRecordNumber() + 1
                        };

                    }
                    Entities.ItemSources.GearItemsSource.UnloadGears = VesselUnloadEdit.VesselUnload.VesselUnload_FishingGearsViewModel.VesselUnload_FishingGearsCollection.ToList();
                    if (!VesselUnloadEdit.VesselUnload.IsMultiGear)
                    {
                        VesselUnload_Gear_Spec_Edited.GearCode = VesselUnloadEdit.VesselUnload.Parent.GearID;
                        VesselUnload_Gear_Spec_Edited.ParentVesselUnload = VesselUnloadEdit.VesselUnload;
                    }
                    propertyGrid.SelectedObject = VesselUnload_Gear_Spec_Edited;
                    _currentGearSpecGearName = VesselUnload_Gear_Spec_Edited.GearUsedName;
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "RowID", DisplayName = "Row ID", Description = "Database identifier", DisplayOrder = 1 });

                    if (!VesselUnloadEdit.VesselUnload.IsMultiGear)
                    {
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GearUsedName", DisplayName = "Gear name", Description = "Name of gear", DisplayOrder = 2 });
                    }
                    else
                    {
                        if (VesselUnloadEdit.VesselUnload.Parent.GearID == VesselUnload_Gear_Spec_Edited.GearCode)
                        {
                            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GearUsedName", DisplayName = "Gear name", Description = "Name of gear", DisplayOrder = 2 });
                        }
                        else
                        {
                            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GearCode", DisplayName = "Gear", Description = "Name of gear", DisplayOrder = 2 });
                        }
                    }
                    //propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GearText", DisplayName = "Other name of gear", Description = "Other name of gear", DisplayOrder = 2 });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "EffortSpecID", DisplayName = "Specification", Description = "Gear specification", DisplayOrder = 3 });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "SpecValue", DisplayName = "Value of spec", Description = "Value of gear specification", DisplayOrder = 4 });


                    labelTitle.Content = "Edit effort definitions";
                    break;
                case "treeItemLenFreq":
                    if (_isNew)
                    {
                        CatchLenFreqEdited = new CatchLenFreqEdited
                        {
                            PK = VesselCatch.CatchLenFreqViewModel.NextRecordNumber
                        };
                    }
                    propertyGrid.SelectedObject = CatchLenFreqEdited;
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "PK", DisplayName = "Database identifier", Description = "Database identifier", DisplayOrder = 1 });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "LengthClass", DisplayName = "Length", Description = "Length of individual", DisplayOrder = 2 });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Frequency", DisplayName = "Number of individuals", Description = "Number of individual", DisplayOrder = 3 });
                    labelTitle.Content = $"Edit length frequency of catch of {VesselCatch.CatchName}";
                    break;
                case "treeItemLenWeight":
                    if (_isNew)
                    {
                        CatchLengthWeightEdited = new CatchLengthWeightEdited
                        {
                            PK = VesselCatch.CatchLengthWeightViewModel.NextRecordNumber
                        };
                    }
                    propertyGrid.SelectedObject = CatchLengthWeightEdited;
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "PK", DisplayName = "Database identifier", Description = "Database identifier", DisplayOrder = 1 });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Length", DisplayName = "Individual length", Description = "Length of individual", DisplayOrder = 2 });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Weight", DisplayName = "Individual weight", Description = "Weight of individual", DisplayOrder = 3 });
                    labelTitle.Content = $"Edit length-weight of catch of {VesselCatch.CatchName}";
                    break;
                case "treeItemLenList":
                    if (_isNew)
                    {
                        CatchLengthEdited = new CatchLengthEdited
                        {
                            PK = VesselCatch.CatchLengthViewModel.NextRecordNumber
                        };
                    }
                    propertyGrid.SelectedObject = CatchLengthEdited;
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "PK", DisplayName = "Database identifier", Description = "Database identifier", DisplayOrder = 1 });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Length", DisplayName = "Length", Description = "Length of individual", DisplayOrder = 2 });

                    labelTitle.Content = $"Edit length of catch of {VesselCatch.CatchName}"; break;
                case "treeItemMaturity":
                    if (_isNew)
                    {
                        CatchMaturityEdited = new CatchMaturityEdited
                        {
                            PK = VesselCatch.CatchMaturityViewModel.NextRecordNumber
                        };
                    }
                    propertyGrid.SelectedObject = CatchMaturityEdited;
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "PK", DisplayName = "Database identifier", Description = "Database identifier", DisplayOrder = 1 });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Length", DisplayName = "Length", Description = "Length of individual", DisplayOrder = 2 });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Weight", DisplayName = "Weight", Description = "Weight of individual", DisplayOrder = 3 });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "SexCode", DisplayName = "Sex", Description = "Sex of individual", DisplayOrder = 4 });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "MaturityCode", DisplayName = "Maturity", Description = "Maturity of individual", DisplayOrder = 5 });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GonadWeight", DisplayName = "Weight of gonad", Description = "Weight of gonad", DisplayOrder = 6 });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GutContentCode", DisplayName = "Gut content", Description = "Gut content", DisplayOrder = 7 });
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "WeightGutContent", DisplayName = "Weight of gut content", Description = "Gut content weight", DisplayOrder = 8 });
                    labelTitle.Content = $"Edit maturity data of catch of {VesselCatch.CatchName}";
                    break;
            }
            if (_isNew)
            {
                labelTitle.Content = $"{(string)labelTitle.Content} (NEW)";
            }
            Title = (string)labelTitle.Content;
        }
        private void OnEditVesselUnloadItems_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SavePlacement();
            _instance = null;
            VesselUnloadEditWindow.Focus();
        }
        public static EditVesselUnloadItemsWindow GetInstance(bool isNew = false)
        {
            if (_instance == null)
            {
                _instance = new EditVesselUnloadItemsWindow(isNew);
            }
            return _instance;
        }
        public static EditVesselUnloadItemsWindow GetInstance(string unLoadView, bool isNew = false)
        {
            if (_instance == null)
            {
                _instance = new EditVesselUnloadItemsWindow(unLoadView, isNew);
            }
            return _instance;
        }
        public string EditVesselUnloadItemsMessage { get; set; }
        private bool Validate()
        {
            EditVesselUnloadItemsMessage = "";
            bool proceed = false;
            switch (UnloadView)
            {

                case "treeItemFishingGears":
                    double totalwtgears;
                    if (_isNew)
                    {
                        totalwtgears = VesselUnloadEdit.VesselUnload.ListUnloadFishingGears.Sum(t => (double)t.WeightOfCatch) + (double)VesselUnload_FishingGear_Edited?.WeightOfCatch;
                    }
                    else
                    {
                        totalwtgears = VesselUnloadEdit.VesselUnload.ListUnloadFishingGears
                                    .Where(t => t.RowID != VesselUnload_FishingGear_Edited.RowID)
                                    .Sum(t => (double)t.WeightOfCatch) + (double)VesselUnload_FishingGear_Edited.WeightOfCatch;
                    }
                    proceed = VesselUnload_FishingGear_Edited.GearCode != null ||
                              VesselUnload_FishingGear_Edited.GearText != null;
                    if (proceed)
                    {
                        proceed = VesselUnload_FishingGear_Edited.WeightOfCatch != null &&
                           VesselUnload_FishingGear_Edited.WeightOfCatch > 0 &&
                           VesselUnload_FishingGear_Edited.NumberOfSpeciesInCatch != null &&
                           VesselUnload_FishingGear_Edited.NumberOfSpeciesInCatch > 0;

                        if (!proceed)
                        {
                            EditVesselUnloadItemsMessage = "Weight of catch and number of species in the catch must be provided";
                        }
                    }
                    else
                    {
                        EditVesselUnloadItemsMessage = "Gear must be provided";
                    }

                    if (proceed)
                    {
                        if (VesselUnload_FishingGear_Edited.WeightOfSample != null)
                        {
                            proceed = VesselUnload_FishingGear_Edited.WeightOfSample > 0;

                            if (!proceed)
                            {
                                EditVesselUnloadItemsMessage = "Weight of sample must be greater than zero";
                            }
                        }
                    }

                    if (proceed)
                    {
                        proceed = totalwtgears <= (double)VesselUnloadEdit.WeightOfCatch;
                        if (!proceed)
                        {
                            EditVesselUnloadItemsMessage = $"Total weight of catch of all gears ({totalwtgears}) must not exceed weight of catch of vessel ({(double)VesselUnloadEdit.WeightOfCatch})";
                        }
                    }

                    if (proceed)
                    {
                        if(VesselUnloadEdit.VesselUnload.VesselUnload_FishingGearsViewModel.GearIsDuplicated(VesselUnload_FishingGear_Edited))
                        {
                            proceed = false;
                            EditVesselUnloadItemsMessage = "Gear already exist";
                        }

                        //if (VesselUnloadEdit.VesselUnload.VesselUnload_FishingGearsViewModel.GetGear(VesselUnload_FishingGear_Edited.GearCode) != null)
                        //{
                        //    proceed = false;
                        //    EditVesselUnloadItemsMessage = "Gear already exist";
                        //}
                    }

                    break;
                case "treeItemSoakTime":
                    var dateset = (DateTime)_dtSet.Value;
                    var dateHaul = (DateTime)_dtHaul.Value;

                    proceed = dateHaul > dateset && dateHaul < VesselUnloadEdit.VesselUnload.SamplingDate;
                    if (!proceed)
                    {
                        EditVesselUnloadItemsMessage = "Time of set must be before time of haul and time of haul should be before date and time of sampling";
                    }
                    break;
                case "treeItemFishingGround":
                    
                    if (FishingGroundGridEdited.UTMZoneText.Length > 0)
                    {
                        if (FishingGroundGridViewModel.IsFormatCorrect(FishingGroundGridEdited.Grid, out int? majorGrid))
                        {
                            if (!VesselUnloadEdit.VesselUnload.FishingGroundGridViewModel.CheckForDuplicate(FishingGroundGridEdited))
                            {
                                if (NSAPEntities.MajorGridFMAViewModel == null)
                                {
                                    NSAPEntities.MajorGridFMAViewModel = new MajorGridFMAViewModel();
                                }
                                NSAPEntities.MajorGridFMAViewModel.GetAllMajorGridFMA(VesselUnloadEdit.VesselUnload.Parent.Parent.FMA, FishingGroundGridEdited.UTMZone);
                                if (MajorGridFMAViewModel.MajorGridsInFMA.FirstOrDefault(t => t.MajorGridNumber == (int)majorGrid) == null)
                                {
                                    EditVesselUnloadItemsMessage = "Major grid is not in the vicinity of FMA";
                                }
                                else
                                {
                                    UTMZone zone = new UTMZone(FishingGroundGridEdited.UTMZoneText);
                                    Grid25GridCell grid = new Grid25GridCell(zone, FishingGroundGridEdited.Grid);
                                    if (NSAPEntities.Grid25InlandLocationViewModel.GridIsInland(grid))
                                    {
                                        EditVesselUnloadItemsMessage = "Grid is located inland";
                                    }
                                    else
                                    {
                                        proceed = true;
                                    }
                                }
                            }
                            else
                            {
                                EditVesselUnloadItemsMessage = "Grid is duplicated";
                            }
                        }
                        else
                        {
                            EditVesselUnloadItemsMessage = FishingGroundGridViewModel.FormattingErrorMessage;
                        }
                    }
                    else
                    {
                        EditVesselUnloadItemsMessage = "UTM zone is required";
                    }
                    break;
                case "treeItemEffortDefinition":

                    string val = VesselUnload_Gear_Spec_Edited.SpecValue;
                    string err_msg = "";
                    proceed = VesselUnload_Gear_Spec_Edited.EffortSpecID != null &&
                        !string.IsNullOrEmpty(VesselUnload_Gear_Spec_Edited.SpecValue) &&
                        !string.IsNullOrEmpty(VesselUnload_Gear_Spec_Edited.GearCode);
                    if (proceed)
                    {
                        EffortSpecification es = NSAPEntities.EffortSpecificationViewModel.GetEffortSpecification((int)VesselUnload_Gear_Spec_Edited.EffortSpecID);
                        switch (es.ValueType)
                        {
                            case ODKValueType.isDecimal:
                                proceed = double.TryParse(val, out double d);
                                err_msg = "Expected value of spec is a decimal number";
                                break;
                            case ODKValueType.isInteger:
                                proceed = int.TryParse(val, out int i);
                                err_msg = "Expected value of spec is a whole number";
                                break;
                            case ODKValueType.isBoolean:
                                proceed = val.ToLower() == "y" || val.ToLower() == "n" || val.ToLower() == "yes" || val.ToLower() == "no";
                                err_msg = "Expected value of spec is true or false";
                                break;
                            case ODKValueType.isText:
                                proceed = !string.IsNullOrEmpty(val);
                                err_msg = "Expected value must not be blank";
                                break;
                        }
                        if (proceed)
                        {
                            if (_isNew)
                            {
                                VesselUnload_Gear_Spec vugs = new VesselUnload_Gear_Spec
                                {
                                    EffortSpecID = (int)VesselUnload_Gear_Spec_Edited.EffortSpecID,
                                    EffortValueNumeric = VesselUnload_Gear_Spec_Edited.EffortValueNumeric,
                                    EffortValueText = VesselUnload_Gear_Spec_Edited.EffortValueText,
                                    GearCode = VesselUnload_Gear_Spec_Edited.GearCode

                                };
                                proceed = VesselUnloadEditWindow.UnloadEditor.IsUnique(vugs);
                                if (!proceed)
                                {
                                    EditVesselUnloadItemsMessage = "Gear and specification must be unique";
                                }
                            }
                        }
                        else
                        {
                            EditVesselUnloadItemsMessage = err_msg;
                        }



                    }
                    else
                    {
                        EditVesselUnloadItemsMessage = "All fields must have a response";
                    }
                    //check for duplicates

                    break;
                case "treeItemLenFreq":
                    if (CatchLenFreqEdited.LengthClass == null || CatchLenFreqEdited.Frequency == null)
                    {
                        EditVesselUnloadItemsMessage = "Length and frequency cannot be blank";
                    }
                    else if (CatchLenFreqEdited.LengthClass <= 0)
                    {
                        EditVesselUnloadItemsMessage = "Length must be greater than zero";
                    }
                    else if (CatchLenFreqEdited.Frequency <= 0)
                    {
                        EditVesselUnloadItemsMessage = "Frequency must be greater than zero";
                    }
                    else
                    {
                        proceed = true;
                    }


                    break;
                case "treeItemLenWeight":
                    if (CatchLengthWeightEdited.Length == null || CatchLengthWeightEdited.Weight == null)
                    {
                        EditVesselUnloadItemsMessage = "Length and weight cannot be blank";
                    }
                    else if (CatchLengthWeightEdited.Length <= 0)
                    {
                        EditVesselUnloadItemsMessage = "Length must be greater than zero";
                    }
                    else if (CatchLengthWeightEdited.Weight <= 0)
                    {
                        EditVesselUnloadItemsMessage = "Weight must be greater than zero";
                    }
                    else
                    {
                        proceed = true;
                    }
                    break;
                case "treeItemLenList":
                    if (CatchLengthEdited.Length == null)
                    {
                        EditVesselUnloadItemsMessage = "Length cannot be blank";
                    }
                    else if (CatchLengthEdited.Length <= 0)
                    {
                        EditVesselUnloadItemsMessage = "Length must be greater than zero";
                    }
                    else
                    {
                        proceed = true;
                    }
                    break;
                case "treeItemMaturity":
                    if (CatchMaturityEdited.Length == null || (double)CatchMaturityEdited.Length <= 0 || CatchMaturityEdited.Weight == null || (double)CatchMaturityEdited.Weight <= 0)
                    {
                        EditVesselUnloadItemsMessage = "Length and weight cannot be blank and must be numbers greater than zero";
                    }
                    //else if (CatchMaturityEdited.Weight == null || (double)CatchMaturityEdited.Weight <= 0)
                    //{
                    //    EditVesselUnloadItemsMessage = "Weight cannot be blank and must be a number greater than zero";
                    //}
                    else if (CatchMaturityEdited.GonadWeight <= 0)
                    {
                        EditVesselUnloadItemsMessage = "Gonad weight must be a number greater than zero";
                    }
                    else
                    {
                        proceed = true;
                    }
                    break;
            }
            return proceed;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }
        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonOk":
                    if (Validate())
                    {
                        bool save_success = false;
                        switch (UnloadView)
                        {

                            case "treeItemFishingGears":
                                VesselUnload_FishingGear vufg = new VesselUnload_FishingGear
                                {
                                    RowID = VesselUnload_FishingGear_Edited.RowID,
                                    WeightOfCatch = VesselUnload_FishingGear_Edited.WeightOfCatch,
                                    WeightOfSample = VesselUnload_FishingGear_Edited.WeightOfSample,
                                    Parent = VesselUnloadEdit.VesselUnload,
                                    CountItemsInCatchComposition = VesselUnload_FishingGear_Edited.NumberOfSpeciesInCatch,
                                    GearCode = VesselUnload_FishingGear_Edited.GearCode,
                                    GearText = VesselUnload_FishingGear_Edited.GearText
                                };
                                if (_isNew)
                                {
                                    if (VesselUnloadEdit.VesselUnload.VesselUnload_FishingGearsViewModel.AddRecordToRepo(vufg))
                                    {
                                        vufg.VesselCatchViewModel = new VesselCatchViewModel(isNew: true);
                                        vufg.VesselUnload_Gear_Specs_ViewModel = new VesselUnload_Gear_Spec_ViewModel();
                                        save_success = true;
                                    }
                                }
                                else
                                {
                                    vufg.VesselCatchViewModel = VesselUnload_FishingGear_Edited.VesselUnload_FishingGear.VesselCatchViewModel;
                                    vufg.VesselUnload_Gear_Specs_ViewModel = VesselUnload_FishingGear_Edited.VesselUnload_FishingGear.VesselUnload_Gear_Specs_ViewModel;
                                    save_success = VesselUnloadEdit.VesselUnload.VesselUnload_FishingGearsViewModel.UpdateRecordInRepo(vufg);
                                }
                                break;
                            case "treeItemSoakTime":
                                GearSoak gs = new GearSoak
                                {
                                    PK = GearSoakEdited.PK,
                                    TimeAtSet = (DateTime)_dtSet.Value,
                                    TimeAtHaul = (DateTime)_dtHaul.Value,
                                    WaypointAtSet = GearSoakEdited.WaypointAtSet,
                                    WaypointAtHaul = GearSoakEdited.WaypointAtHaul,
                                    Parent = VesselUnloadEdit.VesselUnload,
                                    VesselUnloadID = VesselUnloadEdit.VesselUnload.PK
                                };
                                if (_isNew)
                                {
                                    save_success = VesselUnloadEdit.VesselUnload.GearSoakViewModel.AddRecordToRepo(gs);
                                }
                                else
                                {
                                    save_success = VesselUnloadEdit.VesselUnload.GearSoakViewModel.UpdateRecordInRepo(gs);
                                }
                                break;
                            case "treeItemFishingGround":
                                FishingGroundGrid fgg = new FishingGroundGrid
                                {
                                    UTMZoneText = FishingGroundGridEdited.UTMZoneText,
                                    Grid = FishingGroundGridEdited.Grid,
                                    Parent = VesselUnloadEdit.VesselUnload,
                                    PK = FishingGroundGridEdited.PK
                                };

                                if(_isNew)
                                {
                                    save_success= VesselUnloadEdit.VesselUnload.FishingGroundGridViewModel.AddRecordToRepo(fgg);
                                }
                                else
                                {
                                    save_success= VesselUnloadEdit.VesselUnload.FishingGroundGridViewModel.UpdateRecordInRepo(fgg);
                                }
                                break;
                            case "treeItemEffortDefinition":
                                if (VesselUnloadEdit.VesselUnload.IsMultiGear)
                                {
                                    var parent = VesselUnloadEdit.VesselUnload.VesselUnload_FishingGearsViewModel.Get(VesselUnload_Gear_Spec_Edited.GearUsedName);
                                    if (parent.VesselUnload_Gear_Specs_ViewModel.Count == 0)
                                    {
                                        parent.VesselUnload_Gear_Specs_ViewModel = new VesselUnload_Gear_Spec_ViewModel(parent);
                                    }
                                    VesselUnload_Gear_Spec vugs = new VesselUnload_Gear_Spec
                                    {
                                        RowID = VesselUnload_Gear_Spec_Edited.RowID,
                                        EffortSpecID = (int)VesselUnload_Gear_Spec_Edited.EffortSpecID,
                                        EffortValueNumeric = VesselUnload_Gear_Spec_Edited.EffortValueNumeric,
                                        EffortValueText = VesselUnload_Gear_Spec_Edited.EffortValueText,
                                        Parent = parent,
                                        ParentVesselUnloadID = null
                                    };

                                    if (_isNew)
                                    {
                                        save_success = parent.VesselUnload_Gear_Specs_ViewModel.AddRecordToRepo(vugs);

                                    }
                                    else
                                    {
                                        save_success = parent.VesselUnload_Gear_Specs_ViewModel.UpdateRecordInRepo(vugs);
                                    }
                                }
                                else
                                {
                                    VesselEffort ve = new VesselEffort
                                    {
                                        Parent = VesselUnloadEdit.VesselUnload,
                                        EffortSpecID = (int)VesselUnload_Gear_Spec_Edited.EffortSpecID,
                                        EffortValueNumeric = VesselUnload_Gear_Spec_Edited.EffortValueNumeric,
                                        EffortValueText = VesselUnload_Gear_Spec_Edited.EffortValueText,

                                    };
                                    if (_isNew)
                                    {
                                        ve.PK = VesselUnloadEdit.VesselUnload.VesselEffortViewModel.NextRecordNumber;


                                        if (VesselUnloadEdit.VesselUnload.VesselEffortViewModel.AddRecordToRepo(ve))
                                        {
                                            save_success = VesselUnloadViewModel.AddGearSpecForSingleUnload(ve);
                                        }

                                    }
                                    else
                                    {
                                        ve.PK = VesselUnload_Gear_Spec_Edited.RowID;
                                        if (GearUnloadRepository.UpdateGearOfUnload(VesselUnload_Gear_Spec_Edited.GearCode, ve))
                                        {
                                            save_success = VesselUnloadViewModel.UpdateGearSpecsForSingleGearUnload(ve);
                                            if (VesselUnloadEdit.VesselUnload.VesselEffortViewModel.UpdateRecordInRepo(ve))
                                            {
                                                VesselUnloadViewModel.UpdateGearSpecsForSingleGearUnload(ve);
                                                save_success = true;
                                            }
                                        }
                                    }
                                }

                                break;
                            case "treeItemLenFreq":
                                CatchLenFreq clf = new CatchLenFreq
                                {
                                    Parent = VesselCatch,
                                    LengthClass = (double)CatchLenFreqEdited.LengthClass,
                                    Frequency = (int)CatchLenFreqEdited.Frequency
                                };
                                if (_isNew)
                                {
                                    clf.PK = VesselCatch.CatchLenFreqViewModel.NextRecordNumber;
                                    save_success = VesselCatch.CatchLenFreqViewModel.AddRecordToRepo(clf);
                                }
                                else
                                {
                                    clf.PK = CatchLenFreqEdited.PK;
                                    save_success = VesselCatch.CatchLenFreqViewModel.UpdateRecordInRepo(clf);
                                }

                                break;
                            case "treeItemLenWeight":
                                CatchLengthWeight clw = new CatchLengthWeight
                                {
                                    Parent = VesselCatch,
                                    Length = (double)CatchLengthWeightEdited.Length,
                                    Weight = (double)CatchLengthWeightEdited.Weight
                                };
                                if (_isNew)
                                {
                                    clw.PK = VesselCatch.CatchLengthWeightViewModel.NextRecordNumber;
                                    save_success = VesselCatch.CatchLengthWeightViewModel.AddRecordToRepo(clw);
                                }
                                else
                                {
                                    clw.PK = CatchLengthWeightEdited.PK;
                                    save_success = VesselCatch.CatchLengthWeightViewModel.UpdateRecordInRepo(clw);
                                }
                                break;
                            case "treeItemLenList":
                                CatchLength cl = new CatchLength
                                {
                                    Parent = VesselCatch,
                                    Length = (double)CatchLengthEdited.Length
                                };
                                if (_isNew)
                                {
                                    cl.PK = VesselCatch.CatchLengthViewModel.NextRecordNumber;
                                    save_success = VesselCatch.CatchLengthViewModel.AddRecordToRepo(cl);
                                }
                                else
                                {
                                    cl.PK = CatchLengthEdited.PK;
                                    save_success = VesselCatch.CatchLengthViewModel.UpdateRecordInRepo(cl);
                                }
                                break;
                            case "treeItemMaturity":
                                CatchMaturity cm = new CatchMaturity
                                {
                                    Length = CatchMaturityEdited.Length,
                                    Weight = CatchMaturityEdited.Weight,
                                    SexCode = CatchMaturityEdited.SexCode,
                                    MaturityCode = CatchMaturityEdited.MaturityCode,
                                    GutContentCode = CatchMaturityEdited.GutContentCode,
                                    WeightGutContent = CatchMaturityEdited.WeightGutContent,
                                    Parent = VesselCatch,
                                    VesselCatchID = VesselCatch.PK
                                };
                                if (_isNew)
                                {
                                    cm.PK = VesselCatch.CatchMaturityViewModel.NextRecordNumber;
                                    save_success = VesselCatch.CatchMaturityViewModel.AddRecordToRepo(cm);
                                }
                                else
                                {
                                    cm.PK = CatchMaturityEdited.PK;
                                    save_success = VesselCatch.CatchMaturityViewModel.UpdateRecordInRepo(cm);
                                }
                                break;
                        }
                        if (save_success)
                        {
                            VesselUnloadEditWindow.VesselUnloadEditItemsSaved();
                            Close();
                        }
                    }
                    else if (EditVesselUnloadItemsMessage.Length > 0)
                    {
                        System.Windows.MessageBox.Show(EditVesselUnloadItemsMessage, Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    break;
                case "buttonCancel":
                    Close();
                    break;
            }
        }


    }
}
