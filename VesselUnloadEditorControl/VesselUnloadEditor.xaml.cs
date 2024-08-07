﻿using System;
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
using System.Collections.ObjectModel;
using NSAP_ODK.Views;
using System.Diagnostics;
using System.Data.OleDb;

namespace NSAP_ODK.VesselUnloadEditorControl
{

    /// <summary>
    /// Interaction logic for VesselUnloadEdit.xaml
    /// </summary>
    public partial class VesselUnloadEditor : UserControl
    {
        //public event EventHandler<UnloadEditorEventArgs> TreeViewItemSelected;
        public event EventHandler<UnloadEditorEventArgs> UnloadChangesSaved;
        public event EventHandler<UnloadEditorEventArgs> ButtonClicked;
        public event EventHandler<UnloadEditorEventArgs> GridDoubleClicked;
        public event EventHandler<UnloadEditorEventArgs> DeleteProceed;
        public event EventHandler UndoEditVesselUnload;

        private CatchLenFreq _clf;
        private CatchLenFreqEdited _clf_edited;
        private CatchLengthWeight _clw;
        private CatchLengthWeightEdited _clw_edited;
        private CatchLength _cl;
        private CatchLengthEdited _cl_edited;
        private CatchMaturity _cm;
        private CatchMaturityEdited _cm_edited;
        private FishingGroundGrid _fgg;
        private FishingGroundGridEdited _fgg_edited;
        private GearSoak _gs;
        private GearSoakEdited _gs_edited;
        private VesselUnload_Gear_Spec _vu_gs;
        private VesselUnload_Gear_Spec_Edited _vu_gs_edited;
        private VesselUnload_FishingGear _vufg;
        //private VesselCatchEdited _vesselCatchEdited;
        private VesselUnload_FishingGear_Edited _vufg_edited;

        private Window _owner;
        private VesselUnload _vesselUnload;
        //private VesselUnloadEdit _vesselUnloadEdit;
        private VesselUnloadForDisplay _vesselUnloadForDisplay;
        private string _unloadView;
        private bool _editMode;
        private NSAPRegionFMA _nsapRegionFMA;
        private NSAPRegion _nsapRegion;
        private NSAPRegionFMAFishingGround _nsapRegionFMAFishingGround;
        private Dictionary<string, int> _dictProperties = new Dictionary<string, int>();
        private GridLength _rowButtonHeight;
        private bool _isDone = false;
        private bool _isEffortView;
        private VesselCatch _vesselCatch;

        //public VesselUnload VesselUnloadEdited { get; set; }

        public void RefreshView()
        {
            switch (_unloadView)
            {
                case "treeItemVesselUnload":
                    break;
                case "treeItemFishingGears":
                    effortDataGrid.DataContext = _vesselUnload.VesselUnload_FishingGearsViewModel.VesselUnload_FishingGearsCollection;
                    break;
                case "treeItemSoakTime":
                    effortDataGrid.DataContext = _vesselUnload.GearSoakViewModel.GearSoakCollection;
                    break;
                case "treeItemFishingGround":
                    effortDataGrid.DataContext = _vesselUnload.FishingGroundGridViewModel.FishingGroundGridCollection;
                    break;
                case "treeItemEffortDefinition":
                    //_vesselUnload.ListVesselEffort = null;
                    //_vesselUnload.ClearListVesselVesselGearSpecs();
                    effortDataGrid.DataContext = _vesselUnload.ListVesselGearSpec;
                    break;
                case "treeItemCatchComposition":
                    effortDataGrid.DataContext = _vesselUnload.ListVesselCatch;
                    break;
                case "treeItemLenFreq":
                    catchDataGrid.DataContext = VesselCatch.CatchLenFreqViewModel.CatchLenFreqCollection;
                    break;
                case "treeItemLenWeight":
                    catchDataGrid.DataContext = VesselCatch.CatchLengthWeightViewModel.CatchLengthWeightCollection;
                    break;
                case "treeItemLenList":
                    catchDataGrid.DataContext = VesselCatch.CatchLengthViewModel.CatchLengthCollection;
                    break;
                case "treeItemMaturity":
                    catchDataGrid.DataContext = VesselCatch.CatchMaturityViewModel.CatchMaturityCollection;
                    break;
            }

            rowPropertyGrid.Height = new GridLength(0);
            rowDataGrid.Height = new GridLength(1, GridUnitType.Star);
            if (effortDataGrid.SelectedItems.Count == 1)
            {

                if (effortDataGrid.Items[0].GetType().Name == "VesselCatch")
                {
                    effortDataGrid.SetValue(Grid.ColumnSpanProperty, 1);
                    catchDataGrid.Visibility = Visibility.Visible;


                    labelEffort.SetValue(Grid.ColumnSpanProperty, 1);
                    labelCatch.Visibility = Visibility.Visible;
                }
            }


        }
        private void ResetProperty(string propertyName)
        {
            foreach (PropertyItem prp in propertyGrid.Properties)
            {
                if (prp.PropertyName == propertyName)
                {
                    switch (propertyName)
                    {
                        case "WeightOfCatch":
                            VesselUnloadEdit.WeightOfCatch = VesselUnload.WeightOfCatch;
                            prp.Value = VesselUnload.WeightOfCatch;
                            return;
                        case "CountGearTypesUsed":
                            VesselUnloadEdit.CountGearTypesUsed = VesselUnload.CountGearTypesUsed;
                            prp.Value = VesselUnload.CountGearTypesUsed;
                            return;
                    }
                }
            }
        }
        private bool ValidateVesselUnload()
        {
            EditorMessage = "";
            bool success = false;

            if (VesselUnloadEdit.WeightOfCatch != null)
            {
                var wtCatchComp = VesselUnload.ListVesselCatch.Sum(t => t.Catch_kg);
                success = VesselUnloadEdit.WeightOfCatch >= wtCatchComp;

                

                if (!success)
                {
                    EditorMessage = $"Weight of catch ({VesselUnloadEdit.WeightOfCatch}) cannot be less than total weight of catch composition ({wtCatchComp})";
                    ResetProperty("WeightOfCatch");
                }

                if(success)
                {
                    var totalWtGearCatch = VesselUnload.VesselUnload_FishingGearsViewModel.VesselUnload_FishingGearsCollection.Sum(t => t.WeightOfCatch);
                    success = VesselUnloadEdit.WeightOfCatch >= totalWtGearCatch;

                    if(!success)
                    {
                        EditorMessage = "Weight of catch must not be less than sum of weight of catch of all gears";
                        ResetProperty("WeightOfCatch");
                    }
                }

                if (success && VesselUnload.IsMultiGear)
                {
                    var countGears = VesselUnloadEdit.ListUnloadFishingGears.Count;
                    success = VesselUnloadEdit.CountGearTypesUsed != null && VesselUnloadEdit.CountGearTypesUsed >= countGears;
                    if (!success)
                    {
                        EditorMessage = $"Number of gear types ({VesselUnloadEdit.CountGearTypesUsed}) cannot be less than number of gear types listed ({countGears})";
                        ResetProperty("CountGearTypesUsed");

                    }
                }
            }
            VesselUnloadHasChangedProperties = !success;
            return success;
        }
        public bool SaveChangesToVesselUnload()
        {
            bool success = false;


            if (ValidateVesselUnload())
            {
                if (VesselUnloadEdit.EnumeratorText?.Length > 0)
                {
                    VesselUnload.EnumeratorText = VesselUnloadEdit.EnumeratorText;
                    VesselUnload.NSAPEnumeratorID = null;
                }
                else
                {
                    VesselUnload.NSAPEnumeratorID = VesselUnloadEdit.NSAPEnumeratorID;
                }

                if (VesselUnloadEdit.VesselText?.Length > 0)
                {
                    VesselUnload.VesselText = VesselUnloadEdit.VesselText;
                    VesselUnload.VesselID = null;
                }
                else
                {
                    VesselUnload.VesselID = VesselUnloadEdit.VesselID;
                }

                VesselUnload.WeightOfCatch = VesselUnloadEdit.WeightOfCatch;
                VesselUnload.WeightOfCatchSample = VesselUnloadEdit.WeightOfCatchSample;
                VesselUnload.IsCatchSold = VesselUnloadEdit.IsCatchSold;
                VesselUnload.CountGearTypesUsed = VesselUnloadEdit.CountGearTypesUsed;
                VesselUnload.Boxes = VesselUnloadEdit.Boxes;
                VesselUnload.BoxesSampled = VesselUnloadEdit.BoxesSampled;
                VesselUnload.CountGearTypesUsed = VesselUnloadEdit.CountGearTypesUsed;
                VesselUnload.NumberOfFishers = VesselUnloadEdit.NumberOfFishers;


                success = VesselUnload.Parent.VesselUnloadViewModel.UpdateRecordInRepo(VesselUnload);
                if (success)
                {
                    if (NSAPEntities.SummaryItemViewModel.UpdateRecordInRepo(VesselUnload))
                    {
                        _vesselUnloadForDisplay = new VesselUnloadForDisplay(_vesselUnload);
                        VesselUnloadHasChangedProperties = false;
                        UnloadEditorEventArgs eventArgs = new UnloadEditorEventArgs();
                        eventArgs.VesselUnload = VesselUnload;
                        UnloadChangesSaved?.Invoke(this, eventArgs);

                    }
                }
            }
            else
            {
                MessageBox.Show(EditorMessage, Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            return success;
        }

        public bool IsUnique(object obj)
        {

            bool unique = true;
            switch (UnloadView)
            {
                case "treeItemEffortDefinition":
                    VesselUnload_Gear_Spec vugs = obj as VesselUnload_Gear_Spec;
                    foreach (VesselUnload_Gear_Spec item in effortDataGrid.Items)
                    {
                        if (item.Equals(vugs))
                        {
                            unique = false;
                            break;
                        }
                    }
                    break;
            }
            return unique;
        }

        public GearSoakEdited GearSoakEdited { get; set; }
        public FishingGroundGridEdited FishingGroundGridEdited { get; set; }
        public VesselUnload_Gear_Spec_Edited VesselUnload_Gear_Spec_Edited { get; set; }
        public VesselUnload_FishingGear_Edited VesselUnload_FishingGear_Edited { get; set; }
        public bool VesselUnloadHasChangedProperties { get; private set; }
        public VesselUnloadEdit VesselUnloadEdit { get; private set; }
        public VesselUnloadEditor()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
            {
                InitializeComponent();
            }
            labelEffort.Content = "";
            effortDataGrid.MouseRightButtonDown += EffortDataGrid_MouseRightButtonDown;


        }

        private void EffortDataGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = new ContextMenu();
            MenuItem m = null;

            string catchName = ((VesselCatch)effortDataGrid.SelectedItem).CatchName;
            CatchNameURLGenerator.CatchName = catchName;

            foreach (var url in CatchNameURLGenerator.URLS)
            {

                m = new MenuItem { Header = $"Read about {catchName} in {url.Key}", Tag = url.Value };
                m.Click += OnMenuClicked;
                cm.Items.Add(m);
            }

            if (cm.Items.Count > 0)
            {
                cm.IsOpen = true;
            }
        }

        private void OnMenuClicked(object sender, RoutedEventArgs e)
        {
            Process.Start(((MenuItem)sender).Tag.ToString());
        }

        //public VesselUnload_FishingGear_Edited VesselUnload_FishingGear_Edited { get; set; }
        public CatchLenFreqEdited CatchLenFreqEdited { get; set; }
        public CatchLengthWeightEdited CatchLengthWeightEdited { get; set; }
        public CatchMaturityEdited CatchMaturityEdited { get; set; }
        public CatchLengthEdited CatchLengthEdited { get; set; }
        public VesselCatchEdited VesselCatchEdited { get; set; }
        public VesselCatch VesselCatch
        {
            get { return _vesselCatch; }
            set
            {
                _vesselCatch = value;
            }

        }
        public VesselUnload VesselUnload
        {
            get { return _vesselUnload; }
            set
            {
                _vesselUnload = value;
                //var clone = _vesselUnload.Clone();

                _vesselUnloadForDisplay = new VesselUnloadForDisplay(_vesselUnload);
                labelCatch.Content = "No selected catch";

                VesselUnloadViewModel.SetUpFishingGearSubModel(_vesselUnload);
                VesselUnloadEdit = new VesselUnloadEdit(_vesselUnload);
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

        public Window Owner
        {
            get { return _owner; }
            set { _owner = value; }

        }
        private void ShowEditButtons()
        {
            if (_editMode)
            {
                rowButton.Height = _rowButtonHeight;
                //if (_unloadView == "treeItemCatchComposition")
                if (_unloadView != "treeItemVesselUnload")
                {
                    buttonAdd.Visibility = Visibility.Visible;
                    buttonEdit.Visibility = Visibility.Visible;
                }
            }
        }
        private void ResetView()
        {

            if (!_isDone)
            {
                _rowButtonHeight = rowButton.Height;
                _isDone = true;
            }
            rowPropertyGrid.Height = new GridLength(1, GridUnitType.Star);
            rowDataGrid.Height = new GridLength(0);
            rowButton.Height = new GridLength(0);
            effortDataGrid.SetValue(Grid.ColumnSpanProperty, 2);
            catchDataGrid.Visibility = Visibility.Collapsed;
            labelEffort.SetValue(Grid.ColumnSpanProperty, 2);
            labelCatch.Visibility = Visibility.Collapsed;
            buttonAdd.Visibility = Visibility.Collapsed;
            buttonEdit.Visibility = Visibility.Collapsed;

            buttonDelete.IsEnabled = false;
            buttonEdit.IsEnabled = false;

        }
        private void SetupView()
        {
            ResetView();
            switch (_unloadView)
            {
                case "treeItemVesselUnload":
                    break;
                case "treeItemFishingGears":
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
                    if (effortDataGrid.SelectedItems.Count == 1)
                    {
                        effortDataGrid.SetValue(Grid.ColumnSpanProperty, 1);
                        catchDataGrid.Visibility = Visibility.Visible;

                        labelEffort.SetValue(Grid.ColumnSpanProperty, 1);
                        labelCatch.Visibility = Visibility.Visible;
                    }
                    break;
            }

            ShowEditButtons();
        }

        private void SetupDataGridsForDisplay(bool forCatchGrid = false)
        {
            DataGridTextColumn col;
            if (!forCatchGrid)
            {
                effortDataGrid.DataContext = null;
                effortDataGrid.Columns.Clear();
            }

            catchDataGrid.DataContext = null;
            catchDataGrid.Columns.Clear();

            switch (_unloadView)
            {
                case "treeItemSoakTime":
                    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("PK") });
                    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Time at set", Binding = new Binding("TimeAtSet") });
                    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Time at haul", Binding = new Binding("TimeAtHaul") });
                    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Waypoint at set", Binding = new Binding("WaypointAtSet") });
                    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Waypoint at haul", Binding = new Binding("WaypointAtHaul") });

                    effortDataGrid.DataContext = _vesselUnload.ListGearSoak;
                    break;
                case "treeItemFishingGears":
                    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("GearUsedName") });
                    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Weight of catch", Binding = new Binding("WeightOfCatch") });
                    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Weight of sample", Binding = new Binding("WeightOfSample") });
                    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Number of species in catch", Binding = new Binding("CountItemsInCatchComposition") });
                    effortDataGrid.DataContext = _vesselUnload.ListUnloadFishingGears;
                    //Console.WriteLine(_vesselUnload.ListUnloadFishingGears[0].Catches[0].CatchNameEx);
                    break;
                case "treeItemFishingGround":
                    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("PK") });
                    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "UTM Zone", Binding = new Binding("UTMZoneText") });
                    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Grid", Binding = new Binding("Grid") });

                    effortDataGrid.DataContext = _vesselUnload.ListFishingGroundGrid;
                    break;
                case "treeItemEffortDefinition":
                    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("GearUsedName") });
                    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Specification", Binding = new Binding("EffortSpecification") });
                    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Value", Binding = new Binding("EffortValue") });
                    effortDataGrid.DataContext = _vesselUnload.ListVesselGearSpec;
                    //if (VesselUnload.IsMultiGear)
                    //{
                    //    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("GearUsedName") });
                    //    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Specification", Binding = new Binding("EffortSpecification") });
                    //    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Value", Binding = new Binding("EffortValue") });
                    //    effortDataGrid.DataContext = _vesselUnload.ListVesselGearSpec;
                    //}
                    //else
                    //{
                    //    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("PK") });
                    //    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Effort specification", Binding = new Binding("EffortSpecification") });
                    //    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Value", Binding = new Binding("EffortValue") });

                    //    effortDataGrid.DataContext = _vesselUnload.ListVesselEffort;
                    //}
                    break;
                case "treeItemCatchComposition":
                    effortDataGrid.Columns.Clear();
                    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("PK") });
                    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Taxa", Binding = new Binding("Taxa") });
                    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("CatchNameEx") });
                    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("GearNameUsedEx") });

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("Catch_kg"),
                        Header = "Weight",
                        CellStyle = AlignRightStyle
                    };
                    col.Binding.StringFormat = "0.00";
                    effortDataGrid.Columns.Add(col);

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("Sample_kg"),
                        Header = "Weight of sample (TWS)",
                        CellStyle = AlignRightStyle
                    };
                    col.Binding.StringFormat = "0.00";
                    effortDataGrid.Columns.Add(col);

                    effortDataGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Is catch sold", Binding = new Binding("IsCatchSold") });
                    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Price", Binding = new Binding("PriceOfSpecies") });
                    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Unit", Binding = new Binding("PriceUnit") });
                    effortDataGrid.Columns.Add(new DataGridTextColumn { Header = "Other unit", Binding = new Binding("OtherPriceUnit") });
                    effortDataGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "From total catch", Binding = new Binding("FromTotalCatch") });
                    effortDataGrid.DataContext = _vesselUnload.ListVesselCatch;
                    //}

                    break;
                case "treeItemLenFreq":
                    catchDataGrid.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("PK") });
                    catchDataGrid.Columns.Add(new DataGridTextColumn { Header = "Length class", Binding = new Binding("LengthClass") });
                    catchDataGrid.Columns.Add(new DataGridTextColumn { Header = "Frequency", Binding = new Binding("Frequency") });

                    catchDataGrid.DataContext = VesselCatch?.ListCatchLenFreq;
                    break;
                case "treeItemLenWeight":
                    catchDataGrid.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("PK") });
                    catchDataGrid.Columns.Add(new DataGridTextColumn { Header = "Length", Binding = new Binding("Length") });
                    catchDataGrid.Columns.Add(new DataGridTextColumn { Header = "Weight", Binding = new Binding("Weight") });
                    catchDataGrid.Columns.Add(new DataGridTextColumn { Header = "Unit", Binding = new Binding("Parent.WeighingUnit") });

                    catchDataGrid.DataContext = VesselCatch?.ListCatchLengthWeight;
                    break;
                case "treeItemLenList":
                    catchDataGrid.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("PK") });
                    catchDataGrid.Columns.Add(new DataGridTextColumn { Header = "Length", Binding = new Binding("Length") });

                    catchDataGrid.DataContext = VesselCatch?.ListCatchLength;
                    break;
                case "treeItemMaturity":
                    catchDataGrid.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("PK") });
                    catchDataGrid.Columns.Add(new DataGridTextColumn { Header = "Length", Binding = new Binding("Length") });
                    catchDataGrid.Columns.Add(new DataGridTextColumn { Header = "Weight", Binding = new Binding("Weight") });
                    catchDataGrid.Columns.Add(new DataGridTextColumn { Header = "Unit", Binding = new Binding("Parent.WeighingUnit") });
                    catchDataGrid.Columns.Add(new DataGridTextColumn { Header = "Sex", Binding = new Binding("Sex") });
                    catchDataGrid.Columns.Add(new DataGridTextColumn { Header = "Maturity", Binding = new Binding("Maturity") });
                    catchDataGrid.Columns.Add(new DataGridTextColumn { Header = "Gonad weight", Binding = new Binding("GonadWeight") });
                    catchDataGrid.Columns.Add(new DataGridTextColumn { Header = "Weight gut content", Binding = new Binding("WeightGutContent") });
                    catchDataGrid.Columns.Add(new DataGridTextColumn { Header = "Gut content classification", Binding = new Binding("GutContentClassification") });

                    catchDataGrid.DataContext = VesselCatch?.ListCatchMaturity;
                    break;
            }
            labelCatch.Content = $"{GetContextLabel()} {VesselCatch?.CatchName}";
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
        private void SetupPropertyGridForDisplay()//object unloadObject = null)
        {
            //if (unloadObject != null && propertyGrid.Properties.Count == 0)
            if (propertyGrid.Properties.Count == 0)
            {
                propertyGrid.SelectedObject = _vesselUnloadForDisplay;
                //propertyGrid.SelectedObject = unloadObject;
                propertyGrid.NameColumnWidth = 350;
                propertyGrid.AutoGenerateProperties = false;

                //if (propertyGrid.SelectedObject.GetType().Name == "VesselUnloadForDisplay")
                //{
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Region", DisplayName = "NSAP Region", DisplayOrder = 1, Description = "NSAP region", Category = "Header" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FMA", DisplayName = "FMA", DisplayOrder = 2, Description = "FMA", Category = "Header" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FishingGround", DisplayName = "Fishing ground", DisplayOrder = 3, Description = "Fishing ground", Category = "Header" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "LandingSite", DisplayName = "Landing site", DisplayOrder = 4, Description = "Landing site", Category = "Header" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FishingGear", DisplayName = "Fishing gear", DisplayOrder = 5, Description = "Fishing gear", Category = "Header" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "RefNo", DisplayName = "Ref #", DisplayOrder = 6, Description = "Reference number", Category = "Header" });
                //}
                //else if (propertyGrid.SelectedObject.GetType().Name == "VesselUnload")
                //{
                //    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Parent.Parent.NSAPRegion", DisplayName = "NSAP Region", DisplayOrder = 1, Description = "NSAP region", Category = "Header" });
                //    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Parent.Parent.FMA", DisplayName = "FMA", DisplayOrder = 2, Description = "FMA", Category = "Header" });
                //}

                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Identifier", DisplayName = "Database identifier", DisplayOrder = 1, Description = "Database identifier", Category = "Effort" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "SamplingDate", DisplayName = "Sampling date", DisplayOrder = 2, Description = "Date of sampling", Category = "Effort" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Enumerator", DisplayName = "Select name of enumerator", DisplayOrder = 3, Description = "Name of enumerator", Category = "Effort" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "EnumeratorText", DisplayName = "Name of enumerator if not found in selection", DisplayOrder = 4, Description = "Type in name of enumerator", Category = "Effort" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "IsBoatUsed", DisplayName = "This fishing operation is using a fishing vessel", DisplayOrder = 5, Description = "This fishing operation is using a fishing vessel", Category = "Effort" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FishingVessel", DisplayName = "Select name of vessel", DisplayOrder = 6, Description = "Select name of fishing vessel", Category = "Effort" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "SectorCode", DisplayName = "Fisheries sector", DisplayOrder = 8, Description = "Select fisheris sector", Category = "Effort" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "OperationIsSuccessful", DisplayName = "This fishing operation is a success", DisplayOrder = 9, Description = "Is this fishing operation a success\r\nThe catch is not zero", Category = "Effort" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "NumberOfFishers", DisplayName = "Number of fishers", DisplayOrder = 10, Description = "Is this fishing operation a success\r\nThe catch is not zero", Category = "Effort" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FishingTripIsCompleted", DisplayName = "This fishing trip is completed", DisplayOrder = 11, Description = "Is this fishing trip completed\r\nThe trip was not cancelled", Category = "Effort" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "WeightOfCatch", DisplayName = "Weight of catch", DisplayOrder = 12, Description = "Weight of catch", Category = "Effort" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "WeightOfCatchSample", DisplayName = "Weight of sample", DisplayOrder = 13, Description = "Weight of sample taken from catch", Category = "Effort" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Boxes", DisplayName = "Number of boxes", DisplayOrder = 15, Description = "Number of boxes", Category = "Effort" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "BoxesSampled", DisplayName = "Number of boxes sampled", DisplayOrder = 16, Description = "Number of boxes sampled", Category = "Effort" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "RaisingFactor", DisplayName = "Raising factor", DisplayOrder = 17, Description = "Raising factor", Category = "Effort" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "HasCatchComposition", DisplayName = "Catch composition is included", DisplayOrder = 18, Description = "Whether or not catch composition of the landing was included", Category = "Effort" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "IsCatchSold", DisplayName = "Was the catch sold at the landing site", DisplayOrder = 19, Description = "Catch sold by the fisher at the landing site", Category = "Effort" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "IsMultigear", DisplayName = "Multiple gears can be documented", DisplayOrder = 20, Description = "The sampling is able to document multiple fishing gears used", Category = "Effort" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "CountGearTypesUsed", DisplayName = "Number of gears used", DisplayOrder = 21, Description = "Number of gears used in the sampled landing", Category = "Effort" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "IncludeEffortIndicators", DisplayName = "Include effort indicators", DisplayOrder = 22, Description = "INclude fishing effort indicators of the sampled landing", Category = "Effort" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Notes", DisplayName = "Notes", DisplayOrder = 23, Description = "Notes", Category = "Effort" });


                if (VesselUnloadEdit.IsMultigear)
                {
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "CountGearTypesUsed", DisplayName = "Number of gears used", DisplayOrder = 22, Description = "Number of gears used in the operation", Category = "Effort" });
                }


                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "OperationIsTracked", DisplayName = "This operation is tracked", DisplayOrder = 11, Description = "Is this fishing operation tracked", Category = "Tracking" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DepartureFromLandingSite", DisplayName = "Date and time of departure from landing site", DisplayOrder = 12, Description = "Date and time of departure from landing site", Category = "Tracking" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "ArrivalAtLandingSite", DisplayName = "Date and time of arrival at landing site", DisplayOrder = 13, Description = "Date and time of arrival atlanding site", Category = "Tracking" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GPS", DisplayName = "GPS", DisplayOrder = 14, Description = "GPS used in tracking", Category = "Tracking" });

                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "UserName", DisplayName = "User name saved in the device", DisplayOrder = 15, Description = "User name that was inputted and saved in the device", Category = "Device metadata" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DeviceID", DisplayName = "Identifier of the device", DisplayOrder = 16, Description = "Identifier of the device", Category = "Device metadata" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "XFormIdentifier", DisplayName = "XForm identifier", DisplayOrder = 17, Description = "Name of the downloaded excel file containing the data", Category = "Device metadata" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "XFormDate", DisplayName = "XForm date", DisplayOrder = 18, Description = "Date when the downloaded excel file was created", Category = "Device metadata" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FormVersion", DisplayName = "Version of the electronic form", DisplayOrder = 19, Description = "Version of the electronic form used in encoding", Category = "Device metadata" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "IsMultigear", DisplayName = "eForm used can capture multiple gears", DisplayOrder = 20, Description = "eForm version used by enumerators is able to capture multiple fishing gears", Category = "Device metadata" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Submitted", DisplayName = "Date when the electronic form was submitted to the net", DisplayOrder = 21, Description = "Date and time of submission of the encoded data", Category = "Device metadata" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DateAddedToDatabase", DisplayName = "Date added to database", DisplayOrder = 22, Description = "Date and time when data was added to the database", Category = "Device metadata" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FromExcelDownload", DisplayName = "Excel download", DisplayOrder = 23, Description = "Data was donwloaded to an Excel file", Category = "Device metadata" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "ODKRowID", DisplayName = "ODK assigned ID", DisplayOrder = 24, Description = "Unique identifier assigned by ODK", Category = "Device metadata" });

                propertyGrid.IsReadOnly = true;


            }
        }
        private void SetupPropertyGridForEditing()
        {

            propertyGrid.SelectedObject = VesselUnloadEdit;
            propertyGrid.NameColumnWidth = 350;
            propertyGrid.AutoGenerateProperties = false;

            if (VesselUnload.VesselUnload_FishingGearsViewModel.VesselUnload_FishingGearsCollection == null)
            {
                VesselUnload.VesselUnload_FishingGearsViewModel.RefreshCollection();
            }
            Entities.ItemSources.GearsInNSAPRegionItemsSource.UnloadGears = VesselUnload.VesselUnload_FishingGearsViewModel.VesselUnload_FishingGearsCollection.ToList();

            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "RegionCode", DisplayName = "NSAP Region", DisplayOrder = 1, Description = "NSAP region", Category = "Header" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FMAID", DisplayName = "FMA", DisplayOrder = 2, Description = "FMA", Category = "Header" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FishingGroundCode", DisplayName = "Fishing ground", DisplayOrder = 3, Description = "Fishing ground", Category = "Header" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "LandingSiteID", DisplayName = "Landing site", DisplayOrder = 4, Description = "Landing site", Category = "Header" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "OtherLandingSite", DisplayName = "Name of landing site if not in selection", DisplayOrder = 5, Description = "Landing site", Category = "Header" });
            //propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GearCode", DisplayName = "Fishing gear", DisplayOrder = 6, Description = "Fishing gear", Category = "Header" });
            //propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "OtherFishingGear", DisplayName = "Name of fishing gear if not in selection", DisplayOrder = 7, Description = "Fishing gear", Category = "Header" });


            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "MainGearName", DisplayName = "Main gear used", DisplayOrder = 7, Description = "Main gear used in fishing operation", Category = "Header" });

            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Identifier", DisplayName = "Database identifier", DisplayOrder = 1, Description = "Database identifier", Category = "Effort" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "SamplingDateText", DisplayName = "Sampling date", DisplayOrder = 2, Description = "Date of sampling", Category = "Effort" });
            //propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "NSAPRegionEnumeratorID", DisplayName = "Select name of enumerator", DisplayOrder = 3, Description = "Name of enumerator", Category = "Effort" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "NSAPEnumeratorID", DisplayName = "Select name of enumerator", DisplayOrder = 3, Description = "Name of enumerator", Category = "Effort" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "EnumeratorText", DisplayName = "Name of enumerator if not found in selection", DisplayOrder = 4, Description = "Type in name of enumerator", Category = "Effort" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "IsBoatUsed", DisplayName = "This fishing operation is using a fishing vessel", DisplayOrder = 5, Description = "This fishing operation is using a fishing vessel", Category = "Effort" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "VesselID", DisplayName = "Select name of vessel", DisplayOrder = 6, Description = "Select name of fishing vessel", Category = "Effort" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "VesselText", DisplayName = "Name of vessel if not found in selection", DisplayOrder = 7, Description = "Type in the name of vessel", Category = "Effort" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "SectorCode", DisplayName = "Fisheries sector", DisplayOrder = 8, Description = "Select fisheris sector", Category = "Effort" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "OperationIsSuccessful", DisplayName = "This fishing operation is a success", DisplayOrder = 9, Description = "Is this fishing operation a success\r\nThe catch is not zero", Category = "Effort" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "NumberOfFishers", DisplayName = "Number of fishers", DisplayOrder = 10, Description = "Number of fishers on board", Category = "Effort" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FishingTripIsCompleted", DisplayName = "This fishing trip is completed", DisplayOrder = 11, Description = "Is this fishing trip a completed\r\nThe trip was not cancelled", Category = "Effort" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "HasCatchComposition", DisplayName = "Catch composition is included", DisplayOrder = 12, Description = "Catch composition of sampled landing is recorded", Category = "Effort" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "WeightOfCatch", DisplayName = "Weight of catch", DisplayOrder = 13, Description = "Weight of catch", Category = "Effort" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "WeightOfCatchSample", DisplayName = "Weight of sample", DisplayOrder = 14, Description = "Weight of sample taken from catch", Category = "Effort" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Boxes", DisplayName = "Number of boxes", DisplayOrder = 15, Description = "Number of boxes", Category = "Effort" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "BoxesSampled", DisplayName = "Number of boxes sampled", DisplayOrder = 16, Description = "Number of boxes sampled", Category = "Effort" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "IsCatchSold", DisplayName = "Is the catch sold", DisplayOrder = 17, Description = "Is the catch sold at the landing site", Category = "Effort" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "IsMultigear", DisplayName = "Multiple gears can be documented", DisplayOrder = 18, Description = "The sampling is able to document multiple fishing gears used", Category = "Effort" });

            if (VesselUnloadEdit.IsMultigear)
            {
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "CountGearTypesUsed", DisplayName = "Number of gears used", DisplayOrder = 17, Description = "Number of gears used in the sampled landing", Category = "Effort" });
            }

            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "RaisingFactor", DisplayName = "Raising factor", DisplayOrder = 18, Description = "Raising factor", Category = "Effort" });
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


            if (_dictProperties.Count == 0)
            {
                for (int x = 0; x < propertyGrid.Properties.Count; x++)
                {
                    _dictProperties.Add(((PropertyItem)propertyGrid.Properties[x]).PropertyName, x);
                }

            }
            _nsapRegion = VesselUnloadEdit.NSAPRegion;
            _nsapRegionFMA = _nsapRegion.FMAs.Where(t => t.FMAID == VesselUnloadEdit.FMAID).FirstOrDefault();
            _nsapRegionFMAFishingGround = _nsapRegionFMA.FishingGrounds.Where(t => t.FishingGroundCode == VesselUnloadEdit.FishingGroundCode).FirstOrDefault();
            propertyGrid.IsReadOnly = false;
            SetupReadOnlyProperties();

        }

        private void SetupReadOnlyProperties()
        {
            foreach (PropertyItem item in propertyGrid.Properties)
            {
                if (item.PropertyName == "HasCatchComposition" && VesselUnloadEdit.HasCatchComposition && VesselUnload.VesselCatchViewModel.Count > 0)
                {
                    item.Editor = new Label { Content = "True", IsEnabled = false };
                }
            }
        }

        private void ShowStatusCatchComposition()
        {
            statusBar.Items.Add(new Label { Content = VesselUnloadViewModel.StatusText(_vesselUnload) });
        }
        private void ShowStatusCatchCOmposition1()
        {
            Label lbl;
            statusBar.Items.Clear();
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
            }
            else
            {
                statusBar.Items.Add(new Label { Content = "Catch composition is empty" });
            }
        }
        public string UnloadView
        {
            get { return _unloadView; }
            set
            {
                _isEffortView = true;
                if (VesselUnloadHasChangedProperties)
                {
                    SaveChangesToVesselUnload();

                }
                statusBar.Items.Clear();
                _unloadView = value;
                SetupView();
                buttonAdd.IsEnabled = true;
                switch (_unloadView)
                {
                    case "treeItemVesselUnload":
                        VesselCatch = null;
                        labelEffort.Content = "Details of sampled fish landing";

                        propertyGrid.SelectedObject = null;
                        propertyGrid.PropertyDefinitions.Clear();

                        if (_editMode)
                        {
                            SetupPropertyGridForEditing();
                        }
                        else
                        {
                            SetupPropertyGridForDisplay();// _vesselUnloadForDisplay);
                        }
                        buttonDelete.IsEnabled = _editMode;
                        break;
                    case "treeItemFishingGears":
                        VesselCatch = null;
                        labelEffort.Content = "Fishing gears used in the sampled landing";
                        SetupDataGridsForDisplay();
                        buttonAdd.IsEnabled = VesselUnloadEdit.IsMultigear;
                        break;
                    case "treeItemSoakTime":
                        VesselCatch = null;
                        labelEffort.Content = "Soak time of gears deployed by sampled landing";
                        SetupDataGridsForDisplay();
                        break;
                    case "treeItemFishingGround":
                        VesselCatch = null;
                        labelEffort.Content = "Grid location of fishing grounds of sampled landing";
                        SetupDataGridsForDisplay();
                        break;
                    case "treeItemEffortDefinition":
                        VesselCatch = null;
                        labelEffort.Content = "Fishing effort specifications  of sampled landing";
                        SetupDataGridsForDisplay();
                        break;
                    case "treeItemCatchComposition":
                        labelEffort.Content = "Catch composition  of sampled landing";
                        SetupDataGridsForDisplay();
                        ShowStatusCatchComposition();
                        break;
                    case "treeItemLenFreq":
                        _isEffortView = false;
                        SetupDataGridsForDisplay(forCatchGrid: true);
                        break;
                    case "treeItemLenWeight":
                        _isEffortView = false;
                        SetupDataGridsForDisplay(forCatchGrid: true);
                        break;
                    case "treeItemLenList":
                        _isEffortView = false;
                        SetupDataGridsForDisplay(forCatchGrid: true);
                        break;
                    case "treeItemMaturity":
                        _isEffortView = false;
                        SetupDataGridsForDisplay(forCatchGrid: true);
                        break;
                }

                //if(catchDataGrid.Visibility==Visibility.Visible && effortDataGrid.Items.Count>0 && effortDataGrid.Items[0].GetType().Name=="VesselCatch")
                //{

                //}
            }
        }
        public string EditorMessage { get; set; }

        public bool CanAddGear()
        {
            EditorMessage = "";
            bool canAdd = false;
            if (VesselUnloadEdit.IsMultigear && VesselUnloadEdit.ListUnloadFishingGears.Count < VesselUnloadEdit.CountGearTypesUsed)
            {
                double totalCatchOfAllGears = VesselUnloadEdit.ListUnloadFishingGears.Sum(t => (double)t.WeightOfCatch);
                canAdd = totalCatchOfAllGears < VesselUnloadEdit.WeightOfCatch;
                if (!canAdd)
                {
                    EditorMessage = $"Total weight of catch of all gears ({VesselUnloadEdit.ListUnloadFishingGears.Sum(t => t.WeightOfCatch)}) should be less than declared weight of catch ({VesselUnloadEdit.WeightOfCatch})";
                }
            }
            else
            {
                EditorMessage = $"Number of gear types listed ({VesselUnloadEdit.ListUnloadFishingGears.Count}) should be less than number of gears declared ({VesselUnloadEdit.CountGearTypesUsed})";
            }
            return canAdd;

        }
        public bool CanAddToCatchComposition()
        {
            EditorMessage = "";
            bool canAdd = false;
            if (!VesselUnloadEdit.IsMultigear)
            {
                if ( VesselUnloadEdit.HasCatchComposition == true)
                {
                    var sum_catch_composition_weight = VesselUnloadEdit.VesselUnload.VesselCatchViewModel.VesselCatchCollection.Sum(t => t.Catch_kg);
                    canAdd = sum_catch_composition_weight < VesselUnloadEdit.VesselUnload.WeightOfCatch;
                    //if(!canAdd)
                    //{
                    //    EditorMessage = "Weight of total catch must be more than total weight of catch composition";
                    //}
                }
            }
            else if (VesselUnload.IsMultiGear && VesselUnloadEdit.HasCatchComposition)
            {
                double total_species_composition_weight = 0;
                double gear_catch_wt = 0;
                foreach (var gear in VesselUnload.ListUnloadFishingGears)
                {
                    gear_catch_wt += (double)gear.WeightOfCatch;
                    foreach (VesselCatch c in gear.VesselCatchViewModel.VesselCatchCollection)
                    {
                        total_species_composition_weight += (double)c.Catch_kg;
                    }
                }

                canAdd = total_species_composition_weight < gear_catch_wt;
            }
            if (!canAdd)
            {
                if (VesselUnloadEdit.HasCatchComposition)
                {
                    //double wtOfCatch = 0;
                    //foreach (var item in VesselUnloadEdit.VesselCatches)
                    //{
                    //    wtOfCatch += (double)item.Catch_kg;
                    //}

                    //canAdd = wtOfCatch < VesselUnloadEdit.WeightOfCatch;
                    //if (!canAdd)
                    //{
                    EditorMessage = "Weight of total catch is not greater than weight of catch composition.\n\nNew items cannot be added.";
                    //}
                }
                else
                {
                    EditorMessage = "Sampled landing does not include catch composition";

                }
            }
            return canAdd;
        }
        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            UnloadEditorEventArgs eventArgs = new UnloadEditorEventArgs();
            eventArgs.ButtonPressed = ((Button)sender).Name;
            eventArgs.Proceed = false;
            eventArgs.UnloadView = UnloadView;
            eventArgs.VesselCatch = VesselCatch;

            ButtonClicked?.Invoke(this, eventArgs);

            //only when delete is clicked
            if (eventArgs.ButtonPressed == "buttonDelete" && eventArgs.Proceed)
            {
                eventArgs = new UnloadEditorEventArgs();
                eventArgs.UnloadView = UnloadView;
                eventArgs.VesselUnload = VesselUnload;
                DeleteProceed?.Invoke(this, eventArgs);
            }
        }

        private void OnPropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {

            VesselUnloadHasChangedProperties = true;
            ComboBox cbo = new ComboBox();
            cbo.Items.Clear();
            cbo.SelectionChanged += OnComboSelectionChanged;
            cbo.DisplayMemberPath = "Value";
            var currentProperty = (PropertyItem)e.OriginalSource;

            switch (currentProperty.PropertyName)
            {
                case "WeightOfCatch":
                    //if(!ValidateVesselUnload(currentProperty.PropertyName))
                    //{
                    //    UndoEditVesselUnload?.Invoke(this, null);
                    //}
                    //double totalCatchCompWt = VesselUnload.ListVesselCatch.Sum(t => (double)t.Catch_kg);
                    //if ((double)e.NewValue < totalCatchCompWt)
                    //{
                    //    MessageBox.Show($"New value of weight of catch ({e.NewValue}) is less than sum of weight of catch composition({totalCatchCompWt}).\r\n\r\nNew value is not accepted");
                    //    e.NewValue = e.OldValue;
                    //}
                    break;
                case "RegionCode":
                    cbo.Tag = "fma";
                    foreach (var fma in NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(currentProperty.Value.ToString()).FMAs)
                    {
                        cbo.Items.Add(new KeyValuePair<int, string>(fma.FMAID, fma.FMA.Name));
                    }

                    _nsapRegion = NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(currentProperty.Value.ToString());
                    ((PropertyItem)propertyGrid.Properties[_dictProperties["FMAID"]]).Editor = cbo;
                    ((PropertyItem)propertyGrid.Properties[_dictProperties["FishingGroundCode"]]).Editor = new ComboBox();
                    ((PropertyItem)propertyGrid.Properties[_dictProperties["LandingSiteID"]]).Editor = new ComboBox();

                    ComboBox gearEditor = new ComboBox();
                    gearEditor.Tag = "gear";
                    gearEditor.SelectionChanged += OnComboSelectionChanged;
                    gearEditor.DisplayMemberPath = "Value";
                    foreach (var gear in _nsapRegion.Gears.OrderBy(t => t.Gear.GearName))
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
                    _nsapRegionFMA = _nsapRegion.FMAs.Where(t => t.FMAID == int.Parse(currentProperty.Value.ToString())).FirstOrDefault();
                    foreach (var fg in _nsapRegionFMA.FishingGrounds)
                    {
                        cbo.Items.Add(new KeyValuePair<string, string>(fg.FishingGroundCode, fg.FishingGround.Name));
                    }


                    ((PropertyItem)propertyGrid.Properties[_dictProperties["FishingGroundCode"]]).Editor = cbo;
                    ((PropertyItem)propertyGrid.Properties[_dictProperties["LandingSiteID"]]).Editor = new ComboBox();
                    break;
                case "FishingGroundCode":
                    cbo.Tag = "landing_site";
                    NSAPEntities.NSAPRegionFMAFishingGround = _nsapRegionFMA.FishingGrounds.Where(t => t.FishingGroundCode == currentProperty.Value.ToString()).FirstOrDefault();
                    foreach (var ls in NSAPEntities.NSAPRegionFMAFishingGround.LandingSites)
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
                    ((PropertyItem)propertyGrid.Properties[_dictProperties["FMAID"]]).Value = fmaID;
                    _nsapRegionFMA = _nsapRegion.FMAs.Where(t => t.FMAID == fmaID).FirstOrDefault();



                    foreach (var fg in _nsapRegionFMA.FishingGrounds)
                    {
                        editor.Items.Add(new KeyValuePair<string, string>(fg.FishingGroundCode, fg.FishingGround.Name));
                    }

                    ((PropertyItem)propertyGrid.Properties[_dictProperties["FishingGroundCode"]]).Editor = editor;
                    editor.Tag = "fishing_ground";

                    break;
                case "fishing_ground":
                    string fgCode = ((KeyValuePair<string, string>)cbo.SelectedItem).Key;
                    _nsapRegionFMAFishingGround = _nsapRegionFMA.FishingGrounds.Where(t => t.FishingGroundCode == fgCode).FirstOrDefault();
                    ((PropertyItem)propertyGrid.Properties[_dictProperties["FishingGroundCode"]]).Value = fgCode;

                    foreach (var ls in _nsapRegionFMAFishingGround.LandingSites)
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
                    ((PropertyItem)propertyGrid.Properties[_dictProperties["GearCode"]]).Value = ((KeyValuePair<string, string>)cbo.SelectedItem).Key;
                    break;
                case "enumerator":
                    ((PropertyItem)propertyGrid.Properties[_dictProperties["NSAPEnumeratorID"]]).Value = ((KeyValuePair<int, string>)cbo.SelectedItem).Key;
                    break;
            }
        }

        private string GetContextLabel()
        {
            string contextLabel = "";
            switch (UnloadView)
            {
                case "treeItemLenFreq":
                    contextLabel = "Length frequency table for ";
                    break;
                case "treeItemLenWeight":
                    contextLabel = "Length-weight table for ";
                    break;
                case "treeItemLenList":
                    contextLabel = "Length table for ";
                    break;
                case "treeItemMaturity":
                    contextLabel = "Length, weight, sex and maturity table for";
                    break;
                case "treeItemFishingGears":
                    contextLabel = "Fishing gears for";
                    break;
            }
            return contextLabel;
        }

        private void OnDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //VesselCatch = null;
            _cl = null;
            _clf = null;
            _clw = null;
            _cm = null;
            labelCatch.Content = "No selected catch";
            if (((DataGrid)sender).SelectedItem != null)
            {
                DataGrid dg = (DataGrid)sender;
                if (dg.Name == "effortDataGrid" && !_isEffortView && effortDataGrid.Visibility == Visibility.Visible)
                {
                    VesselCatch = (VesselCatch)((DataGrid)sender).SelectedItem;
                    switch (UnloadView)
                    {
                        case "treeItemLenFreq":
                            catchDataGrid.DataContext = VesselCatch?.ListCatchLenFreq;
                            break;
                        case "treeItemLenWeight":
                            catchDataGrid.DataContext = VesselCatch?.ListCatchLengthWeight;
                            break;
                        case "treeItemLenList":
                            catchDataGrid.DataContext = VesselCatch?.ListCatchLength;
                            break;
                        case "treeItemMaturity":
                            catchDataGrid.DataContext = VesselCatch?.ListCatchMaturity;
                            break;
                    }
                    labelCatch.Content = $"{GetContextLabel()} {VesselCatch?.CatchName}";
                }
                else
                {

                    switch (dg.Name)
                    {
                        case "effortDataGrid":
                            switch (UnloadView)
                            {
                                case "treeItemFishingGears":
                                    _vufg = (VesselUnload_FishingGear)((DataGrid)sender).SelectedItem;
                                    if (_editMode)
                                    {
                                        VesselUnload_FishingGear_Edited = new VesselUnload_FishingGear_Edited(_vufg);
                                    }

                                    break;
                                case "treeItemSoakTime":
                                    _gs = (GearSoak)((DataGrid)sender).SelectedItem;
                                    if (_editMode)
                                    {
                                        GearSoakEdited = new GearSoakEdited(_gs);
                                    }

                                    break;
                                case "treeItemFishingGround":
                                    _fgg = (FishingGroundGrid)((DataGrid)sender).SelectedItem;
                                    if (_editMode)
                                    {
                                        FishingGroundGridEdited = new FishingGroundGridEdited(_fgg);
                                    }

                                    break;
                                case "treeItemEffortDefinition":
                                    _vu_gs = (VesselUnload_Gear_Spec)((DataGrid)sender).SelectedItem;
                                    if (_editMode)
                                    {
                                        //_vu_gs_edited = new VesselUnload_Gear_Spec_Edited(_vu_gs);
                                        VesselUnload_Gear_Spec_Edited = new VesselUnload_Gear_Spec_Edited(_vu_gs);
                                    }

                                    break;
                                case "treeItemCatchComposition":
                                    VesselCatch = (VesselCatch)((DataGrid)sender).SelectedItem;
                                    SetupDataGridsForDisplay(forCatchGrid: true);
                                    labelCatch.Content = $"{GetContextLabel()} {VesselCatch?.CatchName}";

                                    if (_editMode && VesselCatch != null)
                                    {
                                        VesselCatchEdited = new VesselCatchEdited(VesselCatch);
                                    }
                                    break;
                            }

                            break;
                        case "catchDataGrid":
                            switch (UnloadView)
                            {
                                case "treeItemLenFreq":
                                    _clf = (CatchLenFreq)((DataGrid)sender).SelectedItem;
                                    if (_editMode)
                                    {
                                        CatchLenFreqEdited = new CatchLenFreqEdited(_clf);
                                        //_clf_edited = new CatchLenFreqEdited(_clf);
                                    }
                                    break;
                                case "treeItemLenWeight":
                                    _clw = (CatchLengthWeight)((DataGrid)sender).SelectedItem;
                                    if (_editMode)
                                    {
                                        CatchLengthWeightEdited = new CatchLengthWeightEdited(_clw);
                                        //_clw_edited = new CatchLengthWeightEdited(_clw);
                                    }
                                    break;
                                case "treeItemLenList":
                                    _cl = (CatchLength)((DataGrid)sender).SelectedItem;
                                    if (_editMode)
                                    {
                                        CatchLengthEdited = new CatchLengthEdited(_cl);
                                        //_cl_edited = new CatchLengthEdited(_cl);
                                    }
                                    break;
                                case "treeItemMaturity":
                                    _cm = (CatchMaturity)((DataGrid)sender).SelectedItem;
                                    if (_editMode)
                                    {
                                        CatchMaturityEdited = new CatchMaturityEdited(_cm);
                                        //_cm_edited = new CatchMaturityEdited(_cm);
                                    }
                                    break;
                            }
                            break;
                    }

                    if (effortDataGrid.DataContext != null || catchDataGrid.DataContext != null)
                    {
                        buttonDelete.IsEnabled = true;
                        buttonEdit.IsEnabled = true;
                        buttonAdd.IsEnabled = true;

                        if (UnloadView == "treeItemFishingGears")
                        {
                            buttonAdd.IsEnabled = VesselUnload.IsMultiGear;
                        }
                    }
                }
            }
        }

        private void OnGridDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_editMode)
            {
                UnloadEditorEventArgs eventArgs = new UnloadEditorEventArgs { UnloadView = UnloadView };//, VesselCatchEdited = VesselCatchEdited };
                switch (UnloadView)
                {
                    case "treeItemSoakTime":
                        eventArgs.GearSoakEdited = GearSoakEdited;
                        break;
                    case "treeItemFishingGround":
                        eventArgs.FishingGroundGridEdited = FishingGroundGridEdited;
                        break;
                    case "treeItemEffortDefinition":
                        eventArgs.VesselUnload_Gear_Spec_Edited = VesselUnload_Gear_Spec_Edited;
                        break;
                    case "treeItemCatchComposition":
                        eventArgs.VesselCatchEdited = VesselCatchEdited;
                        break;
                    case "treeItemFishingGears":
                        eventArgs.VesselUnload_FishingGear_Edited = VesselUnload_FishingGear_Edited;
                        break;
                    case "treeItemLenFreq":
                        eventArgs.CatchLenFreqEdited = CatchLenFreqEdited;
                        break;
                    case "treeItemLenWeight":
                        eventArgs.CatchLengthWeightEdited = CatchLengthWeightEdited;
                        break;
                    case "treeItemLenList":
                        eventArgs.CatchLengthEdited = CatchLengthEdited;
                        break;
                    case "treeItemMaturity":
                        eventArgs.CatchMaturityEdited = CatchMaturityEdited;
                        break;
                }

                GridDoubleClicked?.Invoke(this, eventArgs);
            }

        }

        private void OnGridGotFocus(object sender, RoutedEventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            if (dg.Name == "effortDataGrid" && !_isEffortView && effortDataGrid.Visibility == Visibility.Visible)
            {
                switch (UnloadView)
                {
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
            else if (dg.Name == "catchDataGrid")
            {

            }
        }

        private void OnDataGridLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}

