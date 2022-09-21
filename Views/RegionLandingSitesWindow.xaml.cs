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

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for RegionLandingSitesWindow.xaml
    /// </summary>
    public partial class RegionLandingSitesWindow : Window
    {
        private List<NSAPRegionFMAFishingGroundLandingSite> _fma_landingSites = new List<NSAPRegionFMAFishingGroundLandingSite>();
        private List<LandingSite> _landingSites = new List<LandingSite>();
        public RegionLandingSitesWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {

            foreach (var fma in NSAPRegion.FMAs)
            {
                foreach (var fg in fma.FishingGrounds)
                {
                    foreach (var ls in fg.LandingSites)
                    {
                        _fma_landingSites.Add(ls);
                    }
                }
            }


            foreach (var ls in _fma_landingSites)
            {
                if (_landingSites.Count == 0)
                {
                    _landingSites.Add(ls.LandingSite);
                    _landingSites[0].FishingGrounds.Add(ls.NSAPRegionFMAFishingGround);
                }
                else
                {
                    if(_landingSites.Contains(ls.LandingSite))
                    {
                        _landingSites.Where(t => t.LandingSiteID == ls.LandingSite.LandingSiteID).FirstOrDefault().FishingGrounds.Add(ls.NSAPRegionFMAFishingGround);
                    }
                    else
                    {
                        _landingSites.Add(ls.LandingSite);
                        _landingSites.Where(t => t.LandingSiteID == ls.LandingSite.LandingSiteID).FirstOrDefault().FishingGrounds.Add(ls.NSAPRegionFMAFishingGround);
                    }
                }
            }
            dataGrid.ItemsSource = _landingSites;
        }

        public Entities.NSAPRegion NSAPRegion { get; set; }
        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {

        }
    }
}
