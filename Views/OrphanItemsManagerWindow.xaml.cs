using NSAP_ODK.Entities;
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
using NSAP_ODK.Utilities;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for OrphanItemsManagerWindow.xaml
    /// </summary>
    public partial class OrphanItemsManagerWindow : Window
    {
        public OrphanItemsManagerWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
            Closing += OnWindowClosing;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            switch (NSAPEntity)
            {
                case NSAPEntity.LandingSite:
                    labelTitle.Content = "Manage orphaned landing sites";
                    dataGrid.DataContext = NSAPEntities.LandingSiteSamplingViewModel.OrphanedLandingSites();

                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site name", Binding = new Binding("LandingSiteName"), IsReadOnly = true });
                    dataGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Replace", Binding = new Binding("ForReplacement") });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Region", Binding = new Binding("Region.ShortName"), IsReadOnly = true });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("FMA"), IsReadOnly = true });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("FishingGround"), IsReadOnly = true });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Number of landings", Binding = new Binding("NumberOfLandings"), IsReadOnly = true });
                    break;
                case NSAPEntity.Enumerator:
                    labelTitle.Content = "Manage orphaned enumerators";
                    break;
                case NSAPEntity.FishingGear:
                    labelTitle.Content = "Manage orphaned fishing gears";
                    break;
            }

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
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            bool procced = false;
            switch (((Button)sender).Name)
            {
                case "buttonReplace":
                    var replacementWindow = new SelectionToReplaceOrpanWIndow();
                    replacementWindow.Owner = this;
                    replacementWindow.NSAPEntity = NSAPEntity;

                    foreach (var item in dataGrid.Items)
                    {
                        switch (NSAPEntity)
                        {
                            case NSAPEntity.LandingSite:
                                if (((OrphanedLandingSite)item).ForReplacement)
                                {
                                    procced = true;
                                    replacementWindow.LandingSiteSampling = ((OrphanedLandingSite)item).LandingSiteSamplings[0];

                                }
                                break;
                            case NSAPEntity.FishingGear:
                                break;
                            case NSAPEntity.NSAPRegionEnumerator:
                                break;

                        }

                        if (procced)
                        {
                            break;
                        }
                    }

                    if (procced)
                    {
                        replacementWindow.FillSelection();
                        replacementWindow.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Check at least one item in the table", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    break;
                case "buttonCancel":
                    Close();
                    break;
            }
        }

        public NSAPEntity NSAPEntity { get; set; }
        private void Grid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
