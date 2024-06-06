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
using NSAP_ODK.Utilities;
using Xceed.Wpf.Toolkit.PropertyGrid;
using NSAP_ODK.Entities;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for LandingSiteSamplingWindow.xaml
    /// </summary>
    public partial class LandingSiteSamplingWindow : Window
    {
        private LandingSiteSampling _lss;
        private VesselUnload _selectedUnload;
        private VesselUnloadEditWindow _vesselUnloadWindow;
        public LandingSiteSamplingWindow(LandingSiteSampling lss)
        {
            _lss = lss;
            Loaded += LandingSiteSamplingWindow_Loaded;

            InitializeComponent();
        }

        private void GridLandings_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_selectedUnload != null)
            {
                if (NSAPEntities.NSAPRegion == null)
                {
                    NSAPEntities.NSAPRegion = _selectedUnload.Parent.Parent.NSAPRegion;
                }
                _vesselUnloadWindow = new VesselUnloadEditWindow(this);
                _vesselUnloadWindow.Owner = this;
                _vesselUnloadWindow.Show();
                _vesselUnloadWindow.VesselUnload = _selectedUnload;
            }
        }

        private void GridLandings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedUnload = gridLandings.SelectedItem as VesselUnload;
            if (_vesselUnloadWindow != null)
            {
                _vesselUnloadWindow.VesselUnload = _selectedUnload;
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }
        private void LandingSiteSamplingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            gridLandings.SelectionChanged += GridLandings_SelectionChanged;
            gridLandings.MouseDoubleClick += GridLandings_MouseDoubleClick;

            propertyGrid.AutoGenerateProperties = false;
            propertyGrid.ShowSearchBox = false;
            propertyGrid.ShowSortOptions = false;
            propertyGrid.ShowTitle = false;
            propertyGrid.IsReadOnly = true;
            propertyGrid.NameColumnWidth = 250;

            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "SamplingDate_MDY", DisplayName = "Sampling date", DisplayOrder = 1, Description = "Sampling date" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Enumerator", DisplayName = "Enumerator", DisplayOrder = 2, Description = "Name of enumerator" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "NSAPRegion", DisplayName = "NSAP Region", DisplayOrder = 3, Description = "NSAP region" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FMA", DisplayName = "FMA", DisplayOrder = 4, Description = "Fisheries management area" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FishingGround", DisplayName = "Fishing ground", DisplayOrder = 5, Description = "Fishing ground" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "LandingSite", DisplayName = "Landing site", DisplayOrder = 6, Description = "Landing site" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Remarks", DisplayName = "Remarks", DisplayOrder = 6, Description = "Remarks" });

            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "IsSamplingDay", DisplayName = "Sampling day", DisplayOrder = 7, Description = "Is this day a sampling day" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "HasFishingOperation", DisplayName = "Has fishing operation", DisplayOrder = 8, Description = "Are there operations on this day" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "NumberOfLandings", DisplayName = "Number of landings", DisplayOrder = 9, Description = "Total number of landings" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "NumberOfLandingsSampled", DisplayName = "Number of landings sampled", DisplayOrder = 10, Description = "Number of landings monitored" });
            propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "NumberOfGearTypesInLandingSite", DisplayName = "Number of gears", DisplayOrder = 11, Description = "Number of gears used by all landings" });



            LandingSiteSamplingFlattened lssf = new LandingSiteSamplingFlattened(_lss);
            propertyGrid.SelectedObject = lssf;




            gridGears.ItemsSource = lssf.GearUnloads;
            gridGears.AutoGenerateColumns = false;
            gridGears.CanUserAddRows = false;
            gridGears.IsReadOnly = true;

            gridGears.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("GearUsedName") });
            gridGears.Columns.Add(new DataGridTextColumn { Header = "# sampled landings", Binding = new Binding("NumberOfSampledLandings") });
            gridGears.Columns.Add(new DataGridTextColumn { Header = "# municipal", Binding = new Binding("NumberOfMunicipalLandings") });
            gridGears.Columns.Add(new DataGridTextColumn { Header = "# commercial", Binding = new Binding("NumberOfCommercialLandings") });
            gridGears.Columns.Add(new DataGridTextColumn { Header = "Weight of municipal catch", Binding = new Binding("WeightOfMunicipalLandings") });
            gridGears.Columns.Add(new DataGridTextColumn { Header = "Weight of commercial catch", Binding = new Binding("WeightOfCommercialLandings") });


            List<VesselUnload> unloads = new List<VesselUnload>();
            if (lssf.GearUnloads != null)
            {
                foreach (var gu in lssf.GearUnloads)
                {
                    unloads.AddRange(gu.ListVesselUnload);
                }
                gridLandings.ItemsSource = unloads;
                gridLandings.AutoGenerateColumns = false;
                gridLandings.CanUserAddRows = false;
                gridLandings.IsReadOnly = true;

                gridLandings.Columns.Add(new DataGridTextColumn { Header = "Vessel", Binding = new Binding("VesselName") });
                gridLandings.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("Sector") });
                gridLandings.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("GearUsed") });
                gridLandings.Columns.Add(new DataGridCheckBoxColumn { Header = "Success", Binding = new Binding("OperationIsSuccessful") });
                gridLandings.Columns.Add(new DataGridTextColumn { Header = "Weight of catch", Binding = new Binding("WeightOfCatch") });
                gridLandings.Columns.Add(new DataGridCheckBoxColumn { Header = "Has catch composition", Binding = new Binding("HasCatchComposition") });
                gridLandings.Columns.Add(new DataGridTextColumn { Header = "Ref #", Binding = new Binding("RefNo") });
                gridLandings.Columns.Add(new DataGridTextColumn { Header = "Notes", Binding = new Binding("Notes") });
                gridLandings.Columns.Add(new DataGridTextColumn { Header = "Form version", Binding = new Binding("Parent.Parent.FormVersion") });
            }


        }

        public LandingSiteSamplingWindow()
        {
            InitializeComponent();
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void ClosingTrigger(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Cancelled = !_saveButtonClicked;
            this.SavePlacement();
        }
    }
}
