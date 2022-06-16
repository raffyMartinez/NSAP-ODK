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
        private bool _isFish;
        private bool _hasInternet;
        private OBIResponseRoot _obiResponse;
        private string _itemHit;
        private string _itemInOBIS;
        public SelectionToReplaceOrpanWIndow()
        {
            InitializeComponent();
            Closing += OnWindowClosing;
            _timer = new DispatcherTimer();
            _timer.Tick += OnTimerTick;
            _timer.Interval = TimeSpan.FromMilliseconds(500);
        }

        private void SearchReplacements1(string toSearch)
        {
            //_isFish = false;
            int count = 0;
            foreach (var sp in NSAPEntities.FishSpeciesViewModel.GetAllSpecies(toSearch))
            {
                var rb = new RadioButton { Content = sp.ToString(), Tag = sp };
                rb.Checked += OnButtonChecked;
                rb.MouseRightButtonDown += OnRadioButtonRightMouseButtonDown;
                _isFish = NSAPEntities.NotFishSpeciesViewModel.GetSpecies(rb.Content.ToString()) == null;
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
                else if (count == 0)
                {
                    TextBlock txtNotFound = null;
                    if (textSearch.Text.Length > 0)
                    {
                        txtNotFound = new TextBlock { Text = $"{textSearch.Text} is not found in the database" };
                    }
                    else
                    {
                        txtNotFound = new TextBlock
                        {
                            Text = $"The accepted name of {_itemHit} according to OBIS is {toSearch}." +
                                      "However, it is not listed as occuring in the Philippines\r\n \r\n" +
                                      "Be aware though that status of accepted species names is constantly changing.",

                        };
                    }
                    txtNotFound.TextWrapping = TextWrapping.Wrap;
                    txtNotFound.Margin = new Thickness(20);
                    txtNotFound.MouseRightButtonDown += OnRadioButtonRightMouseButtonDown;
                    panelButtons.Children.Add(txtNotFound);
                }



            }

            //speciesHyperLink.Inlines.Clear();
            //speciesHyperLink.Inlines.Add($"Search OBIS for {toSearch}");
        }
        private void SearchReplacements(string toSearch)
        {
            //_isFish = false;
            int count = 0;
            foreach (var sp in NSAPEntities.FishSpeciesViewModel.GetAllSpecies(toSearch))
            {
                var rb = new RadioButton { Content = sp.ToString(), Tag = sp };

                rb.Foreground = Brushes.Black;
                rb.Checked += OnButtonChecked;
                rb.MouseRightButtonDown += OnRadioButtonRightMouseButtonDown;
                _isFish = NSAPEntities.NotFishSpeciesViewModel.GetSpecies(rb.Content.ToString()) == null;
                rb.Margin = new Thickness(10, 10, 0, 0);
                panelButtons.Children.Add(rb);
                count++;
            }
            foreach (NotFishSpecies nf in NSAPEntities.NotFishSpeciesViewModel.GetAllSpecies(toSearch))
            {
                var rb = new RadioButton { Content = nf.ToString(), Tag = nf };
                rb.Foreground = Brushes.DarkOrange;
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
                TextBlock txtNotFound = null;
                if (textSearch.Text.Length > 0 && (_itemHit == null || _itemHit.Length == 0))
                {
                    txtNotFound = new TextBlock { Text = $"{textSearch.Text} is not found in the database" };
                }
                else
                {
                    txtNotFound = new TextBlock
                    {
                        Text = $"The accepted name of {_itemHit} according to OBIS is {toSearch}. " +
                                  "However, it is not listed as occuring in the Philippines.\r\n \r\n" +
                                  "Be aware though that status of accepted species names is constantly changing.",

                    };
                }
                txtNotFound.TextWrapping = TextWrapping.Wrap;
                txtNotFound.Margin = new Thickness(20);
                txtNotFound.MouseRightButtonDown += OnRadioButtonRightMouseButtonDown;
                panelButtons.Children.Add(txtNotFound);

            }

            //speciesHyperLink.Inlines.Clear();
            //speciesHyperLink.Inlines.Add($"Search OBIS for {toSearch}");
        }

        private Task<bool> CheckForInternet()
        {
            return Task.Run<bool>(() => Global.HasInternetConnection());
        }

        private void OnRadioButtonRightMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (NSAPEntity == NSAPEntity.FishSpecies || NSAPEntity == NSAPEntity.NonFishSpecies || NSAPEntity == NSAPEntity.SpeciesName)
            {
                string speciesToBrowse = "";
                if (sender.GetType().Name == "RadioButton")
                {
                    speciesToBrowse = ((RadioButton)sender).Content.ToString();
                }
                else if (sender.GetType().Name == "TextBlock")
                {
                    if (textSearch.Text.Length > 0)
                    {
                        speciesToBrowse = textSearch.Text;
                    }
                    else
                    {
                        speciesToBrowse = _itemInOBIS;
                    }
                    //_isFish = true;

                }
                //RadioButton rb = (RadioButton)sender;
                ContextMenu cm = new ContextMenu();
                MenuItem m = null;

                if (_isFish)
                {
                    m = new MenuItem { Header = $"Open {speciesToBrowse} in Fishbaase", Name = "menuFishBasePage" };
                    m.Tag = $"https://www.fishbase.de/summary/{speciesToBrowse.Replace(' ', '-')}.html";
                    m.Click += OnMenuClick;
                    cm.Items.Add(m);
                }

                m = new MenuItem { Header = $"Open {speciesToBrowse} in World Register of Marine Species (WORMS)", Name = "menuWORMS" };
                m.Tag = $"https://www.marinespecies.org/aphia.php?p=taxlist&tName={speciesToBrowse}";
                m.Click += OnMenuClick;
                cm.Items.Add(m);

                m = new MenuItem { Header = $"Open {speciesToBrowse} in Google image", Name = "menuGoogleImagePage" };
                m.Tag = $"https://www.google.com/images?q={speciesToBrowse.Replace(' ', '+')}";
                m.Click += OnMenuClick;
                cm.Items.Add(m);

                m = new MenuItem { Header = $"Open {speciesToBrowse} in Wikipaedia", Name = "menuWikipaedia" };
                m.Tag = $"https://en.wikipedia.org/wiki/{speciesToBrowse.Replace(' ', '_')}";
                m.Click += OnMenuClick;
                cm.Items.Add(m);

                if (_obiResponse != null && _obiResponse.total > 0)
                {
                    cm.Items.Add(new Separator());

                    m = new MenuItem { Header = $"Show details from OBIS", Name = "menuOBISDetails" };
                    m.Click += OnMenuClick;
                    cm.Items.Add(m);
                }

                cm.IsOpen = true;
            }
        }

        private void OnMenuClick(object sender, RoutedEventArgs e)
        {
            switch (((MenuItem)sender).Name)
            {
                case "menuOBISDetails":
                    OBISResultWindow orw = new OBISResultWindow(_obiResponse);
                    orw.ShowDialog();
                    break;
                default:
                    if (_hasInternet)
                    {
                        Process.Start(((MenuItem)sender).Tag.ToString());
                    }
                    else
                    {
                        MessageBox.Show("Not connected to the internet", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    break;
            }
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

        public NSAPRegion NSAPRegion { get; set; }
        public VesselUnload VesselUnload { get; set; }


        public GearUnload GearUnload { get; set; }

        public async void FillSelection()
        {
            if (NSAPEntity == NSAPEntity.FishSpecies || NSAPEntity == NSAPEntity.NonFishSpecies || NSAPEntity == NSAPEntity.SpeciesName)
            {
                _hasInternet = await CheckForInternet();
            }
            string title = "";
            rowSearch.Height = new GridLength(0);
            switch (NSAPEntity)
            {
                case Entities.NSAPEntity.FishingVessel:
                    break;
                case Entities.NSAPEntity.SpeciesName:
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
                    title = "Get replacement species name";
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
                    title = "Get replacement landing site";


                    break;
                case Entities.NSAPEntity.Enumerator:
                    foreach (var regionEnumerator in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection
                        .Where(t => t.Code == NSAPRegion.Code)
                        .FirstOrDefault().NSAPEnumerators.OrderBy(t => t.Enumerator.Name))
                    {
                        var rb = new RadioButton { Content = regionEnumerator.Enumerator.Name, Tag = regionEnumerator.Enumerator };
                        rb.Checked += OnButtonChecked;
                        rb.Margin = new Thickness(10, 10, 0, 0);
                        panelButtons.Children.Add(rb);

                    }
                    title = "Get replacement enumerator";
                    break;
                case Entities.NSAPEntity.FishingGear:
                    foreach(var g in GearUnload.Parent.NSAPRegion.Gears.OrderBy(t=>t.Gear.GearName))
                    {
                        var rb = new RadioButton { Content = g.Gear.GearName, Tag = g.Gear };
                        rb.Checked += OnButtonChecked;
                        rb.Margin = new Thickness(10, 10, 0, 0);
                        panelButtons.Children.Add(rb);
                    }

                    //foreach (var regionGear in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection
                    //    .Where(t => t.Code == GearUnload.Parent.NSAPRegionID)
                    //    .FirstOrDefault().Gears
                    //    .OrderBy(t => t.Gear.GearName))
                    //{
                    //    var rb = new RadioButton { Content = regionGear.Gear.GearName, Tag = regionGear.Gear };
                    //    rb.Checked += OnButtonChecked;
                    //    rb.Margin = new Thickness(10, 10, 0, 0);
                    //    panelButtons.Children.Add(rb);

                    //}
                    title = "Get replacement fishing gear";
                    break;
            }



            string buttonContent = "";
            if (NSAPEntity == NSAPEntity.LandingSite)
            {
                buttonContent = "Landing site not in list";
            }
            else if (NSAPEntity == NSAPEntity.FishingGear)
            {
                buttonContent = "Fishing gear not in list";
            }
            else if (NSAPEntity == NSAPEntity.Enumerator)
            {
                buttonContent = "Enumerator not in list";
            }

            if (buttonContent.Length > 0)
            {
                Button btn = new Button
                {
                    Content = buttonContent,
                    Height = 28,
                    Width = 150,
                    Name = "buttonNotInList",
                    Margin = new Thickness(3, 10, 3, 3),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                btn.Click += onButtonClick;
                panelButtons.Children.Add(btn);
            }

            Title = title;
        }

        private void RefreshList()
        {
            panelButtons.Children.Clear();
            FillSelection();
            ((MainWindow)Owner.Owner).RefreshEntityGrid();
        }
        public void NewFishingGearInSelection(Gear g)
        {
            RefreshList();
        }
        public void NewEnumeratorInSelection(NSAPEnumerator ns)
        {
            RefreshList();
        }
        public void NewLandingSiteInSelection(LandingSite ls)
        {
            RefreshList();
        }
        public EntityContext EntityContext { get; set; }
        private void onButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonNotInList":
                    //Visibility = Visibility.Collapsed;
                    IsEnabled = false;
                    EditWindowEx ewx = new EditWindowEx(NSAPEntity);
                    ewx.EntityContext = EntityContext;
                    ewx.Owner = this;
                    ewx.ShowDialog();
                    break;
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
                            case NSAPEntity.SpeciesName:
                                if (_isFish)
                                {
                                    try
                                    {
                                        ((OrphanItemsManagerWindow)Owner).ReplacementFishSpecies = (FishSpecies)_selectedButton.Tag;
                                    }
                                    catch (InvalidCastException)
                                    {
                                        ((OrphanItemsManagerWindow)Owner).ReplacementNotFishSpecies = (NotFishSpecies)_selectedButton.Tag;
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Log(ex);
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        ((OrphanItemsManagerWindow)Owner).ReplacementNotFishSpecies = (NotFishSpecies)_selectedButton.Tag;
                                    }
                                    catch (InvalidCastException)
                                    {
                                        ((OrphanItemsManagerWindow)Owner).ReplacementFishSpecies = (FishSpecies)_selectedButton.Tag;
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Log(ex);
                                    }
                                }
                                break;
                            case NSAPEntity.FishSpecies:
                                ((OrphanItemsManagerWindow)Owner).ReplacementFishSpecies = (FishSpecies)_selectedButton.Tag;
                                break;
                            case NSAPEntity.NonFishSpecies:
                                ((OrphanItemsManagerWindow)Owner).ReplacementNotFishSpecies = (NotFishSpecies)_selectedButton.Tag;
                                break;
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
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("You must select one item from the list", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    break;
                case "buttonCancel":
                    DialogResult = false;
                    Close();
                    break;
            }
        }

        private void On_EditWindowsEx_EntityUpdated(object sender, EntityContext e)
        {
            panelButtons.Children.Clear();
            FillSelection();
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
            _isFish = false;
            _itemInOBIS = "";
            _itemHit = "";
            _obiResponse = null;
            bool isConnected = true;
            if (_hasInternet)
            {
                try
                {

                    Hyperlink hyp = (Hyperlink)sender;
                    panelButtons.Children.Clear();
                    switch (hyp.Name)
                    {
                        case "speciesHyperLink":


                            //if (textSearch.Text.Length > 0)
                            //{
                            //    _obiResponse = await NSAPEntities.FishSpeciesViewModel.RequestDataFromOBI(textSearch.Text);
                            //}
                            //else
                            //{
                            _obiResponse = await NSAPEntities.FishSpeciesViewModel.RequestDataFromOBI(ItemToReplace);
                            //}
                            if (_obiResponse.total > 0)
                            {
                                _itemHit = ItemToReplace;
                                _itemInOBIS = _obiResponse.results[0].acceptedNameUsage;
                                _isFish = _obiResponse.results[0].@class == "Actinopteri" || _obiResponse.results[0].@class == "Elasmobranchii";
                                SearchReplacements(_itemInOBIS);
                            }
                            else
                            {
                                MessageBox.Show($"{ItemToReplace} is not found in OBIS", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                            }


                            break;
                        case "multi_speciesHyperLink":
                            bool isFound = false;
                            foreach (var name in ItemsToReplace)
                            {
                                _obiResponse = await NSAPEntities.FishSpeciesViewModel.RequestDataFromOBI(name);
                                if (_obiResponse.total > 0)
                                {
                                    _itemHit = name;
                                    _itemInOBIS = _obiResponse.results[0].acceptedNameUsage;
                                    SearchReplacements(_itemInOBIS);
                                    //SearchReplacements(_obiResponse.results[0].acceptedNameUsage);
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
                catch (System.Net.Http.HttpRequestException)
                {
                    isConnected = false;
                }

            }
            else
            {
                isConnected = false;
            }

            if (!isConnected)
            {
                MessageBox.Show("Not connected to the internet", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (NSAPEntity == NSAPEntity.SpeciesName)
            {
                textSearch.Focus();
            }
        }
    }
}
