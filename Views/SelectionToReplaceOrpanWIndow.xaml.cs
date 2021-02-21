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
using System.Windows.Threading;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for SelectionToReplaceOrpanWIndow.xaml
    /// </summary>
    public partial class SelectionToReplaceOrpanWIndow : Window
    {
        private RadioButton _selectedButton;
        private DispatcherTimer _timer;
        public SelectionToReplaceOrpanWIndow()
        {
            InitializeComponent();
            Closing += OnWindowClosing;
            _timer = new DispatcherTimer();
            _timer.Tick += OnTimerTick;
            _timer.Interval = TimeSpan.FromMilliseconds(500);
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            foreach (var sp in NSAPEntities.FishSpeciesViewModel.GetAllSpecies(textSearch.Text))
            {
                var rb = new RadioButton { Content = sp.ToString(), Tag = sp };
                rb.Checked += OnButtonChecked;
                rb.Margin = new Thickness(10, 10, 0, 0);
                panelButtons.Children.Add(rb);
            }
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SavePlacement();
            Closing -= OnWindowClosing;

        }

        public string ItemToReplace { get; set; }
        protected override void OnSourceInitialized(EventArgs e)
        {
            var h = Height;
            var w = Width;
            base.OnSourceInitialized(e);
            this.ApplyPlacement();

            if (Height < 40 && Width < 140)
            {
                Height = h;
                Width = w;
            }

           

        }
        public Entities.NSAPEntity NSAPEntity { get; set; }
        public LandingSiteSampling LandingSiteSampling { get; set; }

        public GearUnload GearUnload { get; set; }

        public void FillSelection()
        {
            rowSearch.Height = new GridLength(0);
            switch (NSAPEntity)
            {
                case Entities.NSAPEntity.FishSpecies:
                   
                    rowSearch.Height = new GridLength(40);
                    break;
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

                    }



                    break;
                case Entities.NSAPEntity.Enumerator:
                    foreach (var regionEnumerator in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection
                        .Where(t => t.Code == LandingSiteSampling.NSAPRegionID)
                        .FirstOrDefault().NSAPEnumerators
                        .OrderBy(t=>t.Enumerator.Name)
                        )
                    {
                        var rb = new RadioButton { Content = regionEnumerator.Enumerator.Name, Tag = regionEnumerator.Enumerator };
                        rb.Checked += OnButtonChecked;
                        rb.Margin = new Thickness(10, 10, 0, 0);
                        panelButtons.Children.Add(rb);

                    }

                    break;
                case Entities.NSAPEntity.FishingGear:
                    foreach (var regionGear in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection
                        .Where(t => t.Code == GearUnload.Parent.NSAPRegionID)
                        .FirstOrDefault().Gears
                        .OrderBy(t=>t.Gear.GearName)
                        )
                    {
                        var rb = new RadioButton { Content = regionGear.Gear.GearName, Tag = regionGear.Gear };
                        rb.Checked += OnButtonChecked;
                        rb.Margin = new Thickness(10, 10, 0, 0);
                        panelButtons.Children.Add(rb);

                    }
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
                            case NSAPEntity.FishingGear:
                                ((OrphanItemsManagerWindow)Owner).ReplacementGear = (Gear)_selectedButton.Tag;
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

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).Text.Length > 2)
            {
                panelButtons.Children.Clear();
                _timer.Stop();
                _timer.Start();
            }
            else
            {
                panelButtons.Children.Clear();
            }
        }
    }
}
