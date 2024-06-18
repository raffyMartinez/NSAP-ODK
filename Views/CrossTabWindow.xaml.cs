using ClosedXML;
using NSAP_ODK.Entities.CrossTabBuilder;
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

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for CrossTabWindow.xaml
    /// </summary>
    public partial class CrossTabWindow : Window
    {
        private string _filePath;
        public CrossTabWindow()
        {
            InitializeComponent();
            Loaded += CrossTabWindow_Loaded;
            ContentRendered += CrossTabWindow_ContentRendered;
            Closing += CrossTabWindow_Closing;


        }

        private void CrossTabWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SavePlacement();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }
        private void CrossTabWindow_ContentRendered(object sender, EventArgs e)
        {
            tviLandings.IsSelected = true;
        }

        private void CrossTabWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //tviLandings.IsSelected = true; ;
            dataGrid.IsReadOnly = true;
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private Style RightAlignStyle()
        {
            Style s = new Style();
            s.Setters.Add(new Setter(DataGridCell.HorizontalAlignmentProperty, HorizontalAlignment.Right));
            return s;
        }

        private void OnTreeSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var e_m = CrossTabGenerator.EntitiesOfMonth;
            string location_date = $"{e_m.LandingSite}, {e_m.NSAPRegion}, {e_m.FMA}, {e_m.FishingGround} on {((DateTime)e_m.MonthSampled).ToString("MMM, yyyy")}";
            dataGrid.AutoGenerateColumns = true;
            switch (((TreeViewItem)((TreeView)sender).SelectedItem).Name)
            {
                case "tviLandings":
                    dataGrid.DataContext = CrossTabDatasetsGenerator.DailyLandingsDataTable;
                    labelTitle.Content = $"Landings per day at {location_date}";
                    break;
                case "tviEffort":
                    dataGrid.DataContext = CrossTabDatasetsGenerator.EffortDataTable;
                    labelTitle.Content = $"Sampled landings and gear effort indicators at {location_date}";
                    break;
                case "tviEffortAndCatch":
                    dataGrid.DataContext = CrossTabDatasetsGenerator.EffortSpeciesDataTable;
                    labelTitle.Content = $"Catch composition of sampled landings and gear effort indicators at {location_date}";
                    break;
                case "tviLength":
                    dataGrid.DataContext = CrossTabDatasetsGenerator.SpeciesLengthsDataTable;
                    labelTitle.Content = $"Length of catch from sampled landings at {location_date}";
                    break;
                case "tviLenWt":
                    dataGrid.DataContext = CrossTabDatasetsGenerator.SpeciesLengthWeightDataTable;
                    labelTitle.Content = $"Length and weight of catch from sampled landings at {location_date}";
                    break;
                case "tviLenFreq":
                    dataGrid.DataContext = CrossTabDatasetsGenerator.SpeciesLengthFreqDataTable;
                    labelTitle.Content = $"Length frequency of catch from sampled landings at {location_date}";
                    break;
                case "tviMaturity":
                    dataGrid.DataContext = CrossTabDatasetsGenerator.SpeciesMaturityDataTable;
                    labelTitle.Content = $"Length, weight, and maturity data of catch from sampled landings at {location_date}";
                    break;
            }
            //labelTitle.Visibility = Visibility.Visible;
        }
        private void ExportToExcel()
        {
            //string filePath;
            string exportResult;
            string file = Utilities.ExportExcel.GetSaveAsExcelFileName(this, _filePath);
            if (file.Length > 0)
            {
                if (Utilities.ExportExcel.ExportDatasetToExcel(CrossTabManager.CrossTabDataSet, file))
                {
                    exportResult = "Successfully exported to Excel";
                }
                else
                {
                    if (CrossTabManager.ErrorMessage.Length > 0)
                    {
                        exportResult = $"Was not successfull in exporting to Excel\r\n{CrossTabManager.ErrorMessage}";
                    }
                    else
                    {
                        exportResult = $"Was not successfull in exporting to Excel\r\n{Utilities.ExportExcel.ErrorMessage}";
                    }
                }

                MessageBox.Show(exportResult, Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void OnMenuClicked(object sender, RoutedEventArgs e)
        {
            switch (((MenuItem)sender).Name)
            {
                case "menuExportExcel":
                    ExportToExcel();
                    break;
                case "menuClose":
                    Close();
                    break;
            }
        }
    }
}
