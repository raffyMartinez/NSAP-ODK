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
using NSAP_ODK.Entities;
using NSAP_ODK.Entities.Database;
using NSAP_ODK.Utilities;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for EnumeratorsAndLandingSitesWindow.xaml
    /// </summary>
    public partial class EnumeratorsAndLandingSitesWindow : Window
    {
        private OrphanedFishingGear _orphanedFishingGear;
        private OrphanedLandingSite _orphanedLandingSite;
        private static EnumeratorsAndLandingSitesWindow _instance;
        private NSAPEntity _nsapEntity;
        public EnumeratorsAndLandingSitesWindow()
        {
            InitializeComponent();
            dataGrid.AutoGenerateColumns = false;
            dataGrid.IsReadOnly = true;
            dataGrid.CanUserAddRows = false;




            Closing += EnumeratorsAndLandingSitesWindow_Closing;
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }
        private void EnumeratorsAndLandingSitesWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SavePlacement();
            _instance = null;
        }

        public NSAPEntity NSAPEntity
        {
            get { return _nsapEntity; }
            set
            {
                _nsapEntity = value;
                if (_nsapEntity == NSAPEntity.LandingSite)
                {
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("LandingSiteName") });


                }
                else if (NSAPEntity == NSAPEntity.FishingGear)
                {
                    //
                }
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Gears", Binding = new Binding("GearsListed") });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Number of landings", Binding = new Binding("CountSampledLandings") });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "First sampling", Binding = new Binding("FirstSamplingText") });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Last sampling", Binding = new Binding("LastSamplingText") });
            }
        }
        public string OrphanedFishingGearName { get; private set; }
        public string OrphanedLandingSiteName { get; private set; }
        public static EnumeratorsAndLandingSitesWindow GetInstance()
        {
            if (_instance == null)
            {
                _instance = new EnumeratorsAndLandingSitesWindow();
            }
            return _instance;
        }

        public OrphanedFishingGear OrphanedFishingGear
        {
            get { return _orphanedFishingGear; }
            set
            {
                _orphanedFishingGear = value;
                OrphanedFishingGearName = _orphanedFishingGear.Name;
                NSAPRegionEnumerator = _orphanedFishingGear.Region.NSAPEnumerators.FirstOrDefault(t => t.Enumerator.Name == _orphanedFishingGear.EnumeratorNames[0]);
                labelTitle.Content = $"Summary of fishing gears of {NSAPRegionEnumerator}";
                var l = NSAPEntities.SummaryItemViewModel.SummaryItemCollection
                    .Where(t => t.EnumeratorNameToUse == NSAPRegionEnumerator.Enumerator.Name && t.GearName != OrphanedFishingGearName)
                    .GroupBy(t => t.GearUsedName);

                List<EnumeratorLandingSiteSummary> elss = new List<EnumeratorLandingSiteSummary>();
                foreach (var item in l)
                {
                    var ll = item.Where(t => t.GearUsedName.Length > 0).Select(t => t.GearUsedName).ToHashSet();
                    EnumeratorLandingSiteSummary els = new EnumeratorLandingSiteSummary
                    {
                        CountSampledLandings = item.Count(),
                        FirstSampling = (DateTime)item.Min(t => t.SamplingDate),
                        LastSampling = (DateTime)item.Max(t => t.SamplingDate),
                        //LandingSiteName = item.First().LandingSiteNameText,
                        GearsUsed = ll.ToList()
                    };
                    elss.Add(els);
                }
                dataGrid.DataContext = elss;
            }
        }
        public OrphanedLandingSite OrphanedLandingSite
        {
            get { return _orphanedLandingSite; }
            set
            {
                _orphanedLandingSite = value;
                OrphanedLandingSiteName = _orphanedLandingSite.LandingSiteName;
                NSAPRegionEnumerator = _orphanedLandingSite.Region.NSAPEnumerators.FirstOrDefault(t => t.Enumerator.Name == _orphanedLandingSite.EnumeratorName);
                labelTitle.Content = $"Summary of landing sites of {NSAPRegionEnumerator}";
                var l = NSAPEntities.SummaryItemViewModel.SummaryItemCollection
                    .Where(t => t.EnumeratorNameToUse == NSAPRegionEnumerator.Enumerator.Name && t.LandingSiteNameText != OrphanedLandingSiteName)
                    .GroupBy(t => t.LandingSiteNameText);

                List<EnumeratorLandingSiteSummary> elss = new List<EnumeratorLandingSiteSummary>();
                foreach (var item in l)
                {
                    var ll = item.Where(t => t.GearUsedName.Length > 0).Select(t => t.GearUsedName).ToHashSet();
                    EnumeratorLandingSiteSummary els = new EnumeratorLandingSiteSummary
                    {
                        CountSampledLandings = item.Count(),
                        FirstSampling = (DateTime)item.Min(t => t.SamplingDate),
                        LastSampling = (DateTime)item.Max(t => t.SamplingDate),
                        LandingSiteName = item.First().LandingSiteNameText,
                        GearsUsed = ll.ToList()
                    };
                    elss.Add(els);
                }
                dataGrid.DataContext = elss;
            }
        }
        public NSAPRegionEnumerator NSAPRegionEnumerator { get; private set; }
        private void onButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonOk":
                    break;
            }
        }
    }
}
