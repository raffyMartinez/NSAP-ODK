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
using NSAP_ODK.Utilities;
using NSAP_ODK.Entities;
using NSAP_ODK.Entities.Database;
using Xceed.Wpf.Toolkit.PropertyGrid;
using NSAP_ODK.Entities.ItemSources;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for EditVesselUnloadWindow.xaml
    /// </summary>
    public partial class EditVesselCatchCompWindow : Window
    {
        private static EditVesselCatchCompWindow _instance;
        private VesselCatchEdited _vesselCatchEdited;
        private bool _isNew = false;
        private VesselUnload _vesselUnload;
        private VesselUnloadEdit _vesselUnloadEdit;

        public static EditVesselCatchCompWindow GetInstance(bool isNew)
        {
            if (_instance == null)
            {
                _instance = new EditVesselCatchCompWindow(isNew);
            }
            return _instance;
        }

        public static EditVesselCatchCompWindow Instance
        {
            get { return _instance; }
        }
        public EditVesselCatchCompWindow(bool isNew)
        {
            InitializeComponent();
            Closing += OnWindowClosing;
            _isNew = isNew;
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SavePlacement();
            _instance = null;
        }
        public VesselUnloadEdit VesselUnloadEdit
        {
            get { return _vesselUnloadEdit; }
            set
            {
                _vesselUnloadEdit = value;
                SetUpPropertyGrid();
            }
        }
        public VesselUnload VesselUnload
        {
            get { return _vesselUnload; }
            set
            {
                _vesselUnload = value;
                SetUpPropertyGrid();
            }
        }
        public VesselCatchEdited VesselCatchEdited
        {
            get { return _vesselCatchEdited; }
            set
            {
                _vesselCatchEdited = value;
                SetUpPropertyGrid();
            }
        }

        private void SetUpPropertyGrid()
        {

            propertyGrid.AutoGenerateProperties = false;

            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "TaxaCode", DisplayName = "Taxa", DisplayOrder = 1, Description = "Taxonomic category of catch" });

            if (_isNew)
            {
                GearItemsSource.UnloadGears = _vesselUnloadEdit.ListUnloadFishingGears;
                _vesselCatchEdited = new VesselCatchEdited
                {
                    TaxaCode = "FIS",
                    GearUsed = _vesselUnloadEdit.Gear.GearName
                };
            }
            GenusFromTaxaItemsSource.TaxaCode = _vesselCatchEdited.TaxaCode;

            if (!_isNew)
            {
                SpeciesFromGenusItemsSource.Genus = _vesselCatchEdited.Genus;
                GearItemsSource.UnloadGears = _vesselCatchEdited.VesselCatch.Parent.ListUnloadFishingGears;
            }

            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Genus", DisplayName = "Genus", DisplayOrder = 2, Description = "Generic name" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Species", DisplayName = "Species", DisplayOrder = 3, Description = "Specific name" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "OtherName", DisplayName = "Other name", DisplayOrder = 4, Description = "Other name" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GearUsed", DisplayName = "Fishing gear", DisplayOrder = 5, Description = "Name of fishing gear" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Catch_kg", DisplayName = "Weight in kilograms", DisplayOrder = 6, Description = "Weight of the species in kilograms" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "IsCatchSold", DisplayName = "Is this catch sold", DisplayOrder = 7, Description = "Is this species sold" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "PriceOfSpecies", DisplayName = "Price of species", DisplayOrder = 8, Description = "Price"});
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "PriceUnit", DisplayName = "Unit of sale", DisplayOrder = 9, Description = "Unit of sale"});
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "OtherPriceUnit", DisplayName = "Other unit of sale", DisplayOrder = 10, Description = "Other unit of sale"});



            propertyGrid.SelectedObject = _vesselCatchEdited;
            propertyGrid.IsReadOnly = false;

            propertyGrid.CollapseProperty("PriceOfSpecies");
            propertyGrid.CollapseProperty("PriceUnit");
            propertyGrid.CollapseProperty("OtherPriceUnit");
            
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }

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

        private void OnPropertyValueChanged(object sender, Xceed.Wpf.Toolkit.PropertyGrid.PropertyValueChangedEventArgs e)
        {
            var currentProperty = (PropertyItem)e.OriginalSource;
            ComboBox cbo = new ComboBox();
            cbo.Items.Clear();
            cbo.SelectionChanged += OnComboSelectionChanged;
            cbo.DisplayMemberPath = "Value";

            switch (currentProperty.PropertyName)
            {
                case "IsCatchSold":
                    bool isBrowsable = (bool)currentProperty.Value;
                     foreach (PropertyItem prp in propertyGrid.Properties)
                    {
                        if(prp.Name=="PriceOfSpecies" || prp.Name=="PriceUnit" || prp.Name=="OtherPriceUnit")
                        {
                            
                            if (isBrowsable)
                            {
                                prp.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                prp.Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                    break;
                case "TaxaCode":
                    GenusFromTaxaItemsSource.TaxaCode = NSAPEntities.TaxaViewModel.GetTaxa((string)currentProperty.Value).Code;
                    GenusFromTaxaItemsSource taxaItemsSource = new GenusFromTaxaItemsSource();
                    taxaItemsSource.GetValues();
                    foreach (var item in taxaItemsSource.GeneraItemCollection)
                    {
                        cbo.Items.Add(item);
                    }

                    foreach (PropertyItem prp in propertyGrid.Properties)
                    {
                        if (prp.PropertyName == "Genus")
                        {
                            prp.Editor = cbo;
                            cbo.Tag = "Genus";

                        }
                        else if (prp.PropertyName == "Species" && prp.Editor.GetType().Name == "ComboBox")
                        {
                            ((ComboBox)prp.Editor).Items.Clear();
                        }
                    }
                    break;
                case "Genus":
                    SpeciesFromGenusItemsSource.Genus = _vesselCatchEdited.Genus;
                    SpeciesFromGenusItemsSource speciesItemSource = new SpeciesFromGenusItemsSource();
                    speciesItemSource.GetValues();

                    foreach (var item in speciesItemSource.SpeciesItemCollection)
                    {
                        cbo.Items.Add(item);
                    }

                    foreach (PropertyItem prp in propertyGrid.Properties)
                    {
                        if (prp.PropertyName == "Species")
                        {
                            prp.Editor = cbo;
                            cbo.Tag = "Species";

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
                case "Genus":
                    foreach (PropertyItem prp in propertyGrid.Properties)
                    {
                        if (prp.PropertyName == "Genus")
                        {
                            prp.Value = (string)((Item)cbo.SelectedItem).Value;
                            return;
                        }
                    }
                    break;
                case "Species":
                    foreach (PropertyItem prp in propertyGrid.Properties)
                    {
                        if (prp.PropertyName == "Species" && cbo.Items.Count > 0)
                        {
                            prp.Value = (string)((Item)cbo.SelectedItem).Value;
                            string spName = $"{_vesselCatchEdited.Genus} {_vesselCatchEdited.Species}";
                            if (_vesselCatchEdited.TaxaCode == "FIS")
                            {
                                _vesselCatchEdited.SpeciesID = NSAPEntities.FishSpeciesViewModel.GetSpecies(spName).SpeciesCode;
                            }
                            else
                            {
                                _vesselCatchEdited.SpeciesID = NSAPEntities.NotFishSpeciesViewModel.GetSpecies(spName).SpeciesID;
                            }
                            return;
                        }
                    }
                    break;
            }
        }
    }
}
