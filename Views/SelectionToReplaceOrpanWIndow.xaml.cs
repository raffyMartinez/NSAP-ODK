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
using System.Diagnostics;

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

        private void SearchReplacements(string toSearch)
        {
            int count = 0;
            foreach (var sp in NSAPEntities.FishSpeciesViewModel.GetAllSpecies(toSearch))
            {
                var rb = new RadioButton { Content = sp.ToString(), Tag = sp };
                rb.Checked += OnButtonChecked;
                rb.MouseRightButtonDown += OnRadioButtonRightMouseButtonDown;
                rb.Margin = new Thickness(10, 10, 0, 0);
                panelButtons.Children.Add(rb);
                count++;
            }

            if (count == 1)
            {
                ((RadioButton)panelButtons.Children[0]).IsChecked = true;
            }
            else if (count == 0)
            {
                foreach (NotFishSpecies nf in NSAPEntities.NotFishSpeciesViewModel.GetAllSpecies(toSearch))
                {
                    var rb = new RadioButton { Content = nf.ToString(), Tag = nf };
                    rb.Checked += OnButtonChecked;
                    rb.MouseRightButtonDown += OnRadioButtonRightMouseButtonDown;
                    rb.Margin = new Thickness(10, 10, 0, 0);
                    panelButtons.Children.Add(rb);
                    count++;

                }
                if (count == 1)
                {
                    ((RadioButton)panelButtons.Children[0]).IsChecked = true;
                }
            }
        }

        private void OnRadioButtonRightMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            ContextMenu cm = new ContextMenu();
            MenuItem m = null;

            m = new MenuItem { Header = $"Open {rb.Content} in Fishbaase", Name = "menuFishBasePage" };
            m.Tag =  $"https://www.fishbase.de/summary/{rb.Content.ToString().Replace(' ', '-')}.html";
            m.Click += OnMenuClick;
            cm.Items.Add(m);

            m = new MenuItem { Header = $"Open {rb.Content} in Google image", Name = "menuGoogleImagePage" };
            m.Tag = $"https://www.google.com/images?q={rb.Content.ToString().Replace(' ', '+')}";
            m.Click += OnMenuClick;
            cm.Items.Add(m);

            m = new MenuItem { Header = $"Open {rb.Content} in Wikipaedia", Name = "menuWikipaedia" };
            m.Tag = $"https://en.wikipedia.org/wiki/{rb.Content.ToString().Replace(' ', '_')}";
            m.Click += OnMenuClick;
            cm.Items.Add(m);

            cm.IsOpen = true;
        }

        private void OnMenuClick(object sender, RoutedEventArgs e)
        {
            Process.Start(((MenuItem)sender).Tag.ToString());
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            SearchReplacements(textSearch.Text);
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SavePlacement();
            Closing -= OnWindowClosing;

        }
        public List<string> ItemsToReplace { get; set; }
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
                    linkSingle.Visibility = Visibility.Collapsed;
                    panelMultiSpecieslink.Visibility = Visibility.Collapsed;
                    if (ItemToReplace != null)
                    {
                        linkSingle.Visibility = Visibility.Visible;
                    }
                    else if (ItemsToReplace != null && ItemsToReplace.Count > 1)
                    {
                        panelMultiSpecieslink.Visibility = Visibility.Visible;
                    }
                    speciesHyperLink.Inlines.Clear();
                    speciesHyperLink.Inlines.Add($"Search OBIS for {ItemToReplace}");
                    rowSearch.Height = new GridLength(70);
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
                        .OrderBy(t => t.Enumerator.Name)
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
                        .OrderBy(t => t.Gear.GearName)
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
                case "buttonViewList":
                    string fishList = ItemsToReplace[0];
                    for (int x = 1; x < ItemsToReplace.Count; x++)
                    {
                        fishList += "\r\n" + ItemsToReplace[x];
                    }
                    MessageBox.Show(fishList, "Fish name for replacement", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
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

        private async void OnRequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Hyperlink hyp = (Hyperlink)sender;
            panelButtons.Children.Clear();
            switch (hyp.Name)
            {
                case "speciesHyperLink":

                    var obi_results = await NSAPEntities.FishSpeciesViewModel.RequestDataFromOBI(ItemToReplace);
                    if (obi_results.total > 0)
                    {
                        SearchReplacements(obi_results.results[0].acceptedNameUsage);
                    }
                    else
                    {
                        MessageBox.Show($"{ItemToReplace} is not found in OBIS", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    break;
                case "multi_speciesHyperLink":
                    bool isFound = false;
                    foreach(var name in ItemsToReplace)
                    {
                        obi_results = await NSAPEntities.FishSpeciesViewModel.RequestDataFromOBI(name);
                        if(obi_results.total>0)
                        {
                            SearchReplacements(obi_results.results[0].acceptedNameUsage);
                            isFound = true;
                            return;
                        }
                    }

                    if (!isFound)
                    {
                        MessageBox.Show("Items to replace not found in OBIS", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    break;
            }
        }
    }
}
