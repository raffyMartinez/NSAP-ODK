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
using NSAP_ODK.Utilities;
namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for SelectionToReplaceOrpanWIndow.xaml
    /// </summary>
    public partial class SelectionToReplaceOrpanWIndow : Window
    {
        private RadioButton _selectedButton;
        public SelectionToReplaceOrpanWIndow()
        {
            InitializeComponent();
            Closing += OnWindowClosing;
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SavePlacement();
            Closing -= OnWindowClosing;
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }
        public Entities.NSAPEntity NSAPEntity { get; set; }
        public LandingSiteSampling LandingSiteSampling { get; set; }

        public void FillSelection()
        {
            int counter = 0;
            switch (NSAPEntity)
            {
                case Entities.NSAPEntity.LandingSite:
                    foreach (var item in LandingSiteSampling.NSAPRegion.FMAs
                     .FirstOrDefault(t => t.FMAID == LandingSiteSampling.FMAID).FishingGrounds
                     .FirstOrDefault(t => t.FishingGroundCode == LandingSiteSampling.FishingGroundID).LandingSites
                     .OrderBy(t => t.LandingSite.LandingSiteName))
                    {
                        var rb = new RadioButton { Content = item.LandingSite.ToString(), Tag = item.LandingSite };
                        rb.Checked += OnButtonChecked;
                        rb.Margin = new Thickness(10, 10, 0, 0);
                        panelButtons.Children.Add(rb);
                        counter++;
                    }



                    break;
                case Entities.NSAPEntity.Enumerator:
                    foreach(var regionEnumerator in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection
                        .Where(t=>t.Code==LandingSiteSampling.NSAPRegionID).FirstOrDefault().NSAPEnumerators )
                    {
                        var rb = new RadioButton { Content = regionEnumerator.Enumerator.Name, Tag = regionEnumerator.Enumerator};
                        rb.Checked += OnButtonChecked;
                        rb.Margin = new Thickness(10, 10, 0, 0);
                        panelButtons.Children.Add(rb);
                        counter++;
                    }

                    break;
                case Entities.NSAPEntity.FishingGear:
                    break;
            }
        }
        private void onButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonReplace":
                    if (_selectedButton != null)
                    {
                        switch (NSAPEntity)
                        {
                            case NSAPEntity.LandingSite:
                                ((OrphanItemsManagerWindow)Owner).ReplacementLandingSite = (LandingSite)_selectedButton.Tag;
                                break;
                            case NSAPEntity.Enumerator:
                                ((OrphanItemsManagerWindow)Owner).ReplacementEnumerator = (NSAPEnumerator)_selectedButton.Tag;
                                break;
                        }
                        Close();
                    }

                    break;
                case "buttonCancel":
                    Close();
                    break;
            }
        }

        private void OnButtonChecked(object sender, RoutedEventArgs e)
        {
            _selectedButton = (RadioButton)sender;
        }
    }
}
