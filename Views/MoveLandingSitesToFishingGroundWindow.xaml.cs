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

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for MoveLandingSitesToFishingGroundWindow.xaml
    /// </summary>
    public partial class MoveLandingSitesToFishingGroundWindow : Window
    {
        public MoveLandingSitesToFishingGroundWindow()
        {
            InitializeComponent();
            Loaded += MoveLandingSitesToFishingGroundWindow_Loaded;
        }

        private void MoveLandingSitesToFishingGroundWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (NSAPRegionFMAFishingGround != null)
            {
                //foreach(var fg in Region.FMAs.Where(t=>t.FMAID==NSAPRegionFMAFishingGround.RegionFMA.FMA.FMAID).FirstOrDefault().FishingGrounds.OrderBy(t=>t.FishingGround.Name.ToList()))
                foreach (var fg in NSAPRegionFMAFishingGround.RegionFMA.FishingGrounds.OrderBy(t => t.FishingGround.Name))
                {
                    if (fg.FishingGroundCode != NSAPRegionFMAFishingGround.FishingGround.Code)
                    {
                        RadioButton rb = new RadioButton
                        {
                            Content = fg.FishingGround.Name,
                            Tag = fg.FishingGround,
                            Margin = new Thickness(10, 5, 0, 0)
                        };
                        panelCheckBoxes.Children.Add(rb);
                    }
                }
            }
        }
        public NSAPRegionFMAFishingGround NSAPRegionFMAFishingGround { get; set; }
        public FishingGround FishingGround { get; set; }
        private void onButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Content)
            {
                case "Ok":
                    bool hasSelected = false;
                    foreach (RadioButton rb in panelCheckBoxes.Children)
                    {
                        if ((bool)rb.IsChecked)
                        {
                            hasSelected = true;
                            FishingGround = (FishingGround)rb.Tag;
                            break;
                        }
                    }
                    if (hasSelected)
                    {
                        DialogResult = hasSelected;
                    }
                    else
                    {
                        MessageBox.Show("Please select a fishing ground", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    break;
                case "Cancel":
                    DialogResult = false;
                    break;
            }
        }
    }
}
