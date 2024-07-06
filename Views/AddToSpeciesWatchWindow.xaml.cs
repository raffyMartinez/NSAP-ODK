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
using NSAP_ODK.Entities.ItemSources;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for AddToSpeciesWatchWindow.xaml
    /// </summary>
    public partial class AddToSpeciesWatchWindow : Window
    {
        bool _getExisitingFromDB;
        int? _speciesID;
        public AddToSpeciesWatchWindow(bool getExistingFromDb = true)
        {
            InitializeComponent();
            _getExisitingFromDB = getExistingFromDb;
            Loaded += AddToSpeciesWatchWindow_Loaded;
            Closing += AddToSpeciesWatchWindow_Closing;
        }

        private void AddToSpeciesWatchWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Owner.Visibility = Visibility.Visible;
        }

        public List<RegionWatchedSpeciesForAdding> ListForAdding { get; set; }
        private void AddToSpeciesWatchWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid.Visibility = Visibility.Collapsed;
            stackPanelGenusSpecies.Visibility = Visibility.Collapsed;
            if (_getExisitingFromDB)
            {
                dataGrid.Visibility = Visibility.Visible;

                ListForAdding = new List<RegionWatchedSpeciesForAdding>();
                foreach (var item in WatchedSpeciesList)
                {
                    if (!NSAPRegion.RegionWatchedSpeciesViewModel.ExistsInList(item.SpeciesID))
                    {
                        ListForAdding.Add(new RegionWatchedSpeciesForAdding(item));
                    }
                }

                dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Family",
                    Binding = new Binding("RegionWatchedSpecies.Family")
                }
                );

                dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Species name",
                    Binding = new Binding("RegionWatchedSpecies.SpeciesName")
                });

                dataGrid.Columns.Add(new DataGridCheckBoxColumn
                {
                    Header = "Add to watch list",
                    Binding = new Binding("AddToWatchList")
                });


                //dataGrid.DataContext = ListForAdding;
                dataGrid.ItemsSource = ListForAdding;

                if (ListForAdding.Count == 0)
                {
                    MessageBox.Show("All species for watching are already included\r\n\r\nAdd to the list manually",
                        Utilities.Global.MessageBoxCaption,
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                        );

                    Close();
                }
            }
            else
            {
                labelTitle.Content = "Provide details for the species to add to watch list";
                buttonSelectAll.Visibility = Visibility.Collapsed;
                buttonOk.Content = "Add";

                cboTaxa.Items.Clear();
                cboGenus.Items.Clear();
                cboSpecies.Items.Clear();

                cboTaxa.DisplayMemberPath = "Value";
                cboTaxa.SelectedValuePath = "Key";

                cboGenus.DisplayMemberPath = "Value";
                cboGenus.SelectedValuePath = "Key";

                cboSpecies.DisplayMemberPath = "Value";
                cboSpecies.SelectedValuePath = "Key";

                txtIdentifier.Text = (RegionWatchedSpeciesRepository.MaxRecordNumber() + 1).ToString();
                txtIdentifier.IsEnabled = false;
                txtIdentifier.HorizontalContentAlignment = HorizontalAlignment.Right;

                stackPanelGenusSpecies.Visibility = Visibility.Visible;
                TaxaItemsSource tis = new TaxaItemsSource();
                tis.GetValues();


                foreach (var item in tis.TaxaItems)
                {
                    cboTaxa.Items.Add(new KeyValuePair<string, string>((string)item.Value, item.DisplayName));
                }
            }
        }
        public NSAP_ODK.Entities.NSAPRegion NSAPRegion { get; set; }
        public List<RegionWatchedSpecies> WatchedSpeciesList { get; set; }
        private bool ProcessSpeciesForAdding()
        {
            if (_getExisitingFromDB)
            {
                try
                {
                    int addedCount = 0;
                    foreach (var item in dataGrid.Items)
                    {
                        var ws = (RegionWatchedSpeciesForAdding)item;
                        if (ws.AddToWatchList)
                        {
                            ws.RegionWatchedSpecies.NSAPRegion = NSAPRegion;
                            if (ws.RegionWatchedSpecies.PK == 0)
                            {
                                ws.RegionWatchedSpecies.PK = RegionWatchedSpeciesRepository.MaxRecordNumber() + 1;
                            }
                            if (Entities.NSAPEntities.NSAPRegionViewModel.CurrentEntity.RegionWatchedSpeciesViewModel.AddRecordToRepo(ws.RegionWatchedSpecies))
                            {
                                addedCount++;
                            }
                        }
                    }

                    return addedCount > 0;
                }
                catch(Exception ex)
                {
                    Utilities.Logger.Log(ex);
                    return false;
                }
            }
            else
            {
                if (_speciesID != null)
                {
                    RegionWatchedSpecies rws = new RegionWatchedSpecies
                    {
                        NSAPRegion = NSAPRegion,
                        SpeciesID = (int)_speciesID,
                        TaxaCode = ((KeyValuePair<string, string>)cboTaxa.SelectedItem).Key,
                        PK = RegionWatchedSpeciesRepository.MaxRecordNumber() + 1
                    };
                    return NSAPRegion.RegionWatchedSpeciesViewModel.AddRecordToRepo(rws);
                }
                else
                {
                    return false;
                }

            }
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonSelectAll":
                    foreach (var item in dataGrid.Items)
                    {
                        var ws = (RegionWatchedSpeciesForAdding)item;
                        ws.AddToWatchList = true;
                    }
                    dataGrid.Items.Refresh();
                    //dataGrid.DataContext = ListForAdding;

                    break;
                case "buttonOk":
                    if (_getExisitingFromDB || _speciesID != null)
                    {
                        DialogResult = ProcessSpeciesForAdding();
                    }
                    else if (_speciesID == null)
                    {
                        MessageBox.Show("Provide species for adding to the watch list\r\n\r\nMake sure all fields are not blank",
                            Utilities.Global.MessageBoxCaption,
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                            );
                    }

                    break;
                case "buttonCancel":
                    DialogResult = false;
                    break;
            }
        }

        private void OnGridLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void OnComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((ComboBox)sender).Name)
            {
                case "cboTaxa":
                    cboGenus.Items.Clear();
                    GenusFromTaxaItemsSource.TaxaCode = ((KeyValuePair<string, string>)cboTaxa.SelectedItem).Key;
                    GenusFromTaxaItemsSource gftis = new GenusFromTaxaItemsSource();
                    gftis.GetValues();
                    foreach (var item in gftis.GeneraItemCollection)
                    {
                        cboGenus.Items.Add(new KeyValuePair<string, string>((string)item.Value, item.DisplayName));
                    }
                    break;
                case "cboGenus":
                    cboSpecies.Items.Clear();
                    SpeciesFromGenusItemsSource.Genus = ((KeyValuePair<string, string>)cboGenus.SelectedItem).Key;
                    SpeciesFromGenusItemsSource sfgti = new SpeciesFromGenusItemsSource();
                    sfgti.GetValues();
                    foreach (var item in sfgti.SpeciesItemCollection)
                    {
                        cboSpecies.Items.Add(new KeyValuePair<string, string>((string)item.Value, item.DisplayName));
                    }
                    break;
                case "cboSpecies":
                    if (cboSpecies.SelectedItem != null)
                    {
                        string species = $"{((KeyValuePair<string, string>)cboGenus.SelectedItem).Value} {((KeyValuePair<string, string>)cboSpecies.SelectedItem).Value}";
                        if (GenusFromTaxaItemsSource.TaxaCode == "FIS")
                        {
                            _speciesID = Entities.NSAPEntities.FishSpeciesViewModel.GetSpecies(species).SpeciesCode;
                        }
                        else
                        {
                            _speciesID = Entities.NSAPEntities.NotFishSpeciesViewModel.GetSpecies(species).SpeciesID;
                        }
                        if (NSAPRegion.RegionWatchedSpeciesViewModel.ExistsInList((int)_speciesID))
                        {
                            MessageBox.Show("Selected species already in watch list\r\n\r\nSelect another",
                                Utilities.Global.MessageBoxCaption,
                                MessageBoxButton.OK,
                                MessageBoxImage.Information
                                );
                            _speciesID = null;
                            //cboGenus.SelectedItem = null;
                            cboSpecies.SelectedItem = null;
                        }
                    }
                    break;
            }
        }
    }
}
