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
using Xceed.Wpf.Toolkit.PropertyGrid;
using NSAP_ODK.Entities.ItemSources;
using NSAP_ODK.Utilities;
namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for VessekUnloadEditor.xaml
    /// </summary>
    public partial class VesselUnloadEditorWindow : Window
    {
        private string _unloadView;
        private Dictionary<string, int> _dictProperties = new Dictionary<string, int>();
        public VesselUnloadEditorWindow()
        {
            InitializeComponent();
            Loaded += VesselUnloadEditorWindow_Loaded;
            Closing += VesselUnloadEditorWindow_Closing;
        }

        private void VesselUnloadEditorWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SavePlacement();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }
        private void VesselUnloadEditorWindow_Loaded(object sender, RoutedEventArgs e)
        {

            //
        }


        public string UnloadView
        {
            get { return _unloadView; }
            set
            {
                _dictProperties.Clear();
                _unloadView = value;
                propertyGrid.AutoGenerateProperties = false;
                propertyGrid.NameColumnWidth = 220;
                switch (_unloadView)
                {
                    case "treeItemFishingGears":
                        Title = "Edit fishing gears used in operation";

                        //GearItemsSource.UnloadGears = null;
                        var ls_sampling = VesselUnload_FishingGear_Edited.VesselUnload_FishingGear.Parent.Parent.Parent;
                        if(ls_sampling.GearUnloadViewModel==null)
                        {
                            ls_sampling.GearUnloadViewModel = new GearUnloadViewModel(ls_sampling);
                        }
                        //GearsInNSAPRegionItemsSource.GearUnloads = VesselUnload_FishingGear_Edited.VesselUnload_FishingGear.Parent.Parent.Parent.GearUnloadViewModel.GearUnloadCollection.ToList();
                        GearsInNSAPRegionItemsSource.GearUnloads = ls_sampling.GearUnloadViewModel.GearUnloadCollection.ToList();
                        GearsInNSAPRegionItemsSource.AllowBlankGearName = false;
                        GearsInNSAPRegionItemsSource.UnloadGears = null;
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GearUsedName", DisplayName = "Name of fishing gear", DisplayOrder = 1, Description = "Select gear from the list", Category = "Header" });
                        //propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GearText", DisplayName = "Name of fishing gear if not in list", DisplayOrder = 2, Description = "Provide name of gear", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "RowID", DisplayName = "Identifier", DisplayOrder = 3, Description = "Database identifier", Category = "Header" });

                        propertyGrid.SelectedObject = VesselUnload_FishingGear_Edited;
                        break;
                    case "treeItemSoakTime":
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "TimeAtSet", DisplayName = "Time gear was set", DisplayOrder = 1, Description = "Time gear was set", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "TimeAtHaul", DisplayName = "Time gear was hauled", DisplayOrder = 2, Description = "Time gear was hauled", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "WaypointAtSet", DisplayName = "Waypoint of gear setting", DisplayOrder = 3, Description = "Waypoint of gear setting", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "WaypointAtHaul", DisplayName = "Waypoint of gear hauling", DisplayOrder = 4, Description = "Waypoint of gear hauling", Category = "Header" });
                        propertyGrid.SelectedObject = GearSoakEdited;
                        break;
                    case "treeItemFishingGround":
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "UTMZoneText", DisplayName = "UTM zone of grid map", DisplayOrder = 1, Description = "UTM zone of grid map", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Grid", DisplayName = "Grid location", DisplayOrder = 2, Description = "Grid location", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "PK", DisplayName = "Identifier", DisplayOrder = 3, Description = "Database identifier", Category = "Header" });
                        propertyGrid.SelectedObject = FishingGroundGridEdited;
                        break;
                    case "treeItemEffortDefinition":
                        EffortSpecificationItemsSource.VesselUnload_Gear_Spec = VesselUnload_Gear_Spec_Edited.VesselUnload_Gear_Spec;
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GearUsedName", DisplayName = "Fishing gear", DisplayOrder = 1, Description = "Name of fishing gear", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "EffortSpecID", DisplayName = "Effort specification", DisplayOrder = 2, Description = "Effort specification", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "EffortValueNumeric", DisplayName = "Numeric value of effort", DisplayOrder = 3, Description = "Numeric value of effort", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "EffortValueText", DisplayName = "Text value of effort", DisplayOrder = 4, Description = "Text value of effort", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "RowID", DisplayName = "Identifier", DisplayOrder = 5, Description = "Database identifier", Category = "Header" });

                        propertyGrid.SelectedObject = VesselUnload_Gear_Spec_Edited;
                        break;
                    case "treeItemCatchComposition":
                        Title = "Edit catch composition item";
                        GenusFromTaxaItemsSource.TaxaCode = VesselCatchEdited.TaxaCode;
                        SpeciesFromGenusItemsSource.Genus = VesselCatchEdited.Genus;
                        GearItemsSource.UnloadGears = VesselCatchEdited.VesselCatch.Parent.VesselUnload_FishingGearsViewModel.VesselUnload_FishingGearsCollection.ToList();

                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "TaxaCode", DisplayName = "Taxa", DisplayOrder = 1, Description = "Taxonomic category of catch", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Genus", DisplayName = "Genus", DisplayOrder = 2, Description = "Generic name of catch", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Species", DisplayName = "Species", DisplayOrder = 3, Description = "Specific name of catch", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "OtherName", DisplayName = "Other name", DisplayOrder = 4, Description = "Other identifier", Category = "Header" });
                        //propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GearCode", DisplayName = "Name of fishing gear used", DisplayOrder = 5, Description = "Gear used in catching the catch", Category = "Header" });
                        //propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GearText", DisplayName = "Other name of fishing gear used", DisplayOrder = 6, Description = "Gear used in catching the catch", Category = "Header" });

                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GearUsed", DisplayName = "Gear used", DisplayOrder = 6, Description = "Gear used in capturing the catch", Category = "Header" });

                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FromTotalCatch", DisplayName = "From total catch", DisplayOrder = 7, Description = "Is the catch taken from a sample or from the total catch", Category = "Header" });

                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Catch_kg", DisplayName = "Weight of catch (kg)", DisplayOrder = 8, Description = "Weight of the species", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Sample_kg", DisplayName = "Sample weight of catch (kg)", DisplayOrder = 9, Description = "Sample weight of the species", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "IsCatchSold", DisplayName = "Is the species sold", DisplayOrder = 10, Description = "Is the species sold in the landing site", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "PriceOfSpecies", DisplayName = "Price of species", DisplayOrder = 11, Description = "Price of the species", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "PriceUnit", DisplayName = "Unit when sold", DisplayOrder = 12, Description = "Units of selling (per kg, per can, per group, etc", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "PK", DisplayName = "Database identifier", DisplayOrder = 13, Description = "Numeric identifier", Category = "Header" });


                        propertyGrid.SelectedObject = VesselCatchEdited;



                        //MakePropertyReadOnly("PK");
                        break;
                    case "treeItemLenFreq":
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "LengthClass", DisplayName = "Length class", DisplayOrder = 1, Description = "Length class of catch", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Frequency", DisplayName = "Frequency", DisplayOrder = 2, Description = "Frequency of length of catch", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "SexCode", DisplayName = "Sex", DisplayOrder = 3, Description = "Sex of catch", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "PK", DisplayName = "Database identifier", DisplayOrder = 4, Description = "Numeric identifier", Category = "Header" });
                        propertyGrid.SelectedObject = CatchLenFreqEdited;
                        break;
                    case "treeItemLenWeight":
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Length", DisplayName = "Length", DisplayOrder = 1, Description = "Length of catch", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Weight", DisplayName = "Weight", DisplayOrder = 2, Description = "Length of catch", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "SexCode", DisplayName = "Sex", DisplayOrder = 3, Description = "Sex of catch", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "PK", DisplayName = "Database identifier", DisplayOrder = 4, Description = "Numeric identifier", Category = "Header" });
                        propertyGrid.SelectedObject = CatchLengthWeightEdited;
                        break;
                    case "treeItemLenList":
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Length", DisplayName = "Length", DisplayOrder = 1, Description = "Length of catch", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "SexCode", DisplayName = "Sex", DisplayOrder = 2, Description = "Sex of catch", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "PK", DisplayName = "Database identifier", DisplayOrder = 3, Description = "Numeric identifier", Category = "Header" });
                        propertyGrid.SelectedObject = CatchLengthEdited;
                        break;
                    case "treeItemMaturity":
                        Title = "Edit catch maturity";


                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Length", DisplayName = "Length", DisplayOrder = 1, Description = "Length of catch", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Weight", DisplayName = "Weight", DisplayOrder = 2, Description = "Weight of catch", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "SexCode", DisplayName = "Sex", DisplayOrder = 3, Description = "Sex of catch", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "MaturityCode", DisplayName = "Maturity", DisplayOrder = 4, Description = "Maturity of catch", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GonadWeight", DisplayName = "Gonad weight", DisplayOrder = 5, Description = "Gonad weight of catch ", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "WeightGutContent", DisplayName = "Weight of gut content", DisplayOrder = 6, Description = "Weight of gut content of catch ", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GutContentCode", DisplayName = "Classification of gut content", DisplayOrder = 7, Description = "Classification of gut content of catch ", Category = "Header" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "PK", DisplayName = "Identifier", DisplayOrder = 8, Description = "Database identifier", Category = "Header" });
                        propertyGrid.SelectedObject = CatchMaturityEdited;
                        break;
                }

                if (_dictProperties.Count == 0)
                {
                    for (int x = 0; x < propertyGrid.Properties.Count; x++)
                    {
                        _dictProperties.Add(((PropertyItem)propertyGrid.Properties[x]).PropertyName, x);
                    }

                }
                if (_unloadView == "treeItemCatchComposition")
                {
                    if (!(bool)((PropertyItem)propertyGrid.Properties[_dictProperties["IsCatchSold"]]).Value)
                    {
                        ((PropertyItem)propertyGrid.Properties[_dictProperties["PriceOfSpecies"]]).Editor = new Label();
                        ((PropertyItem)propertyGrid.Properties[_dictProperties["PriceUnit"]]).Editor = new Label();
                    }
                }

                labelTitle.Content = Title;
            }
        }
        public VesselUnload_FishingGear VesselUnload_FishingGear { get; set; }

        public VesselUnload_FishingGear_Edited VesselUnload_FishingGear_Edited { get; set; }
        public GearSoak GearSoak { get; set; }
        public GearSoakEdited GearSoakEdited { get; set; }
        public VesselUnload_Gear_Spec VesselUnload_Gear_Spec { get; set; }
        public VesselUnload_Gear_Spec_Edited VesselUnload_Gear_Spec_Edited { get; set; }
        public VesselEffort VesselEffort { get; set; }
        public FishingGroundGrid FishingGroundGrid { get; set; }
        public FishingGroundGridEdited FishingGroundGridEdited { get; set; }
        public VesselCatchEdited VesselCatchEdited { get; set; }
        public VesselCatch VesselCatch { get; set; }
        public CatchLenFreq CatchLenFreq { get; set; }
        public CatchLenFreqEdited CatchLenFreqEdited { get; set; }
        public CatchMaturity CatchMaturity { get; set; }

        public CatchMaturityEdited CatchMaturityEdited { get; set; }
        public CatchLength CatchLength { get; set; }
        public CatchLengthEdited CatchLengthEdited { get; set; }
        public CatchLengthWeight CatchLengthWeight { get; set; }
        public CatchLengthWeightEdited CatchLengthWeightEdited { get; set; }


        private void OnButtonClick(object sender, RoutedEventArgs e)
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
        private void OnComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;
            ComboBox editor = new ComboBox();
            editor.SelectionChanged += OnComboSelectionChanged;
            editor.DisplayMemberPath = "Value";
            switch (cbo.Tag.ToString())
            {
                case "genus":
                    ((PropertyItem)propertyGrid.Properties[_dictProperties["Genus"]]).Value = cbo.SelectedItem.ToString();

                    SpeciesFromGenusItemsSource.Genus = cbo.SelectedItem.ToString();
                    var speciesItemsSource = new SpeciesFromGenusItemsSource();

                    foreach (var species in speciesItemsSource.GetValues().OrderBy(t => t.DisplayName))
                    {
                        editor.Items.Add(species.DisplayName);
                    }
                    ((PropertyItem)propertyGrid.Properties[_dictProperties["Species"]]).Editor = editor;
                    editor.DisplayMemberPath = "";
                    editor.Tag = "species";
                    break;
                case "species":
                    ((PropertyItem)propertyGrid.Properties[_dictProperties["Species"]]).Value = cbo.SelectedItem.ToString();

                    break;
            }
        }
        private void OnPropertyValueChanged(object sender, Xceed.Wpf.Toolkit.PropertyGrid.PropertyValueChangedEventArgs e)
        {
            ComboBox editor = new ComboBox();
            editor.Items.Clear();
            editor.SelectionChanged += OnComboSelectionChanged;
            editor.DisplayMemberPath = "Value";
            var currentProperty = (PropertyItem)e.OriginalSource;

            switch (currentProperty.PropertyName)
            {
                case "IsCatchSold":
                    if ((bool)currentProperty.Value)
                    {
                        ((PropertyItem)propertyGrid.Properties[_dictProperties["PriceOfSpecies"]]).Editor = new TextBox();
                        ((PropertyItem)propertyGrid.Properties[_dictProperties["PriceUnit"]]).Editor = new TextBox();
                        ((TextBox)((PropertyItem)propertyGrid.Properties[_dictProperties["PriceOfSpecies"]]).Editor).BorderThickness = new Thickness(0);
                        ((TextBox)((PropertyItem)propertyGrid.Properties[_dictProperties["PriceUnit"]]).Editor).BorderThickness = new Thickness(0);
                        ((TextBox)((PropertyItem)propertyGrid.Properties[_dictProperties["PriceOfSpecies"]]).Editor).Text = ((PropertyItem)propertyGrid.Properties[_dictProperties["PriceOfSpecies"]]).Value.ToString();
                        ((TextBox)((PropertyItem)propertyGrid.Properties[_dictProperties["PriceUnit"]]).Editor).Text = ((PropertyItem)propertyGrid.Properties[_dictProperties["PriceUnit"]]).Value.ToString();
                    }
                    else
                    {
                        ((PropertyItem)propertyGrid.Properties[_dictProperties["PriceOfSpecies"]]).Editor = new Label();
                        ((PropertyItem)propertyGrid.Properties[_dictProperties["PriceUnit"]]).Editor = new Label();
                    }
                    break;
                case "TaxaCode":
                    editor.Tag = "genus";
                    editor.DisplayMemberPath = "";
                    GenusFromTaxaItemsSource.TaxaCode = currentProperty.Value.ToString();
                    var genusSource = new GenusFromTaxaItemsSource();
                    foreach (var item in genusSource.GetValues().OrderBy(t => t.DisplayName))
                    {
                        editor.Items.Add(item.DisplayName);
                    }
                    ((PropertyItem)propertyGrid.Properties[_dictProperties["Genus"]]).Editor = editor;
                    ((PropertyItem)propertyGrid.Properties[_dictProperties["Species"]]).Editor = new ComboBox();

                    break;
                case "Genus":
                    editor.Tag = "species";
                    editor.DisplayMemberPath = "";
                    GenusFromTaxaItemsSource.TaxaCode = ((PropertyItem)propertyGrid.Properties[_dictProperties["TaxaCode"]]).Value.ToString();
                    SpeciesFromGenusItemsSource.Genus = currentProperty.Value.ToString();
                    var speciesNamesSource = new SpeciesFromGenusItemsSource();
                    foreach (var item in speciesNamesSource.GetValues().OrderBy(t => t.DisplayName))
                    {
                        editor.Items.Add(item.DisplayName);
                    }
                    ((PropertyItem)propertyGrid.Properties[_dictProperties["Species"]]).Editor = editor;
                    break;
                case "Species":
                    string sp_name = ((PropertyItem)propertyGrid.Properties[_dictProperties["Genus"]]).Value.ToString()
                            + " "
                            + currentProperty.Value.ToString();
                    if (GenusFromTaxaItemsSource.TaxaCode == "FIS")
                    {

                        VesselCatchEdited.SpeciesID = NSAPEntities.FishSpeciesViewModel.GetSpecies(sp_name).SpeciesCode;
                    }
                    else
                    {
                        VesselCatchEdited.SpeciesID = NSAPEntities.NotFishSpeciesViewModel.GetSpecies(sp_name).SpeciesID;
                    }
                    break;
            }

        }
    }
}
