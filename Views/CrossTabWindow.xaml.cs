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

        }
        private Style RightAlignStyle()
        {
            Style s = new Style();
            s.Setters.Add(new Setter(DataGridCell.HorizontalAlignmentProperty, HorizontalAlignment.Right));
            return s;
        }

        private void OnTreeSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            dataGrid.AutoGenerateColumns = true;
            switch (((TreeViewItem)((TreeView)sender).SelectedItem).Name)
            {
                case "tviLandings":
                    dataGrid.DataContext = CrossTabDatasetsGenerator.DailyLandingsDataTable;
                    break;
                case "tviEffort":
                    dataGrid.DataContext = CrossTabDatasetsGenerator.EffortDataTable;
                    break;
                case "tviEffortAndCatch":
                    dataGrid.DataContext = CrossTabDatasetsGenerator.EffortSpeciesDataTable;
                    break;
                case "tviLength":
                    dataGrid.DataContext = CrossTabDatasetsGenerator.SpeciesLengthsDataTable;
                    break;
                case "tviLenWt":
                    dataGrid.DataContext = CrossTabDatasetsGenerator.SpeciesLengthWeightDataTable;
                    break;
                case "tviLenFreq":
                    dataGrid.DataContext = CrossTabDatasetsGenerator.SpeciesLengthFreqDataTable;
                    break;
                case "tviMaturity":
                    dataGrid.DataContext = CrossTabDatasetsGenerator.SpeciesMaturityDataTable;
                    break;
            }
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
