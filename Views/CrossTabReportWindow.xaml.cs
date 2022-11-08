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
using NSAP_ODK.Entities.Database;
namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for CrossTabReportWindow.xaml
    /// </summary>
    public partial class CrossTabReportWindow : Window
    {
        private static CrossTabReportWindow _instance;
        private string _filePath;
        public CrossTabReportWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
            Closing += OnWindowClosing;
        }

        public static CrossTabReportWindow Instance { get { return _instance; } }
        public static CrossTabReportWindow GetInstance()
        {
            if (_instance == null)
            {
                _instance = new CrossTabReportWindow();
            }
            return _instance;
        }
        private void OnWindowClosing(object sender, EventArgs e)
        {
            this.SavePlacement();
            _instance = null;
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            Title = "Cross-tab reports";
        }

        public void ShowEffort()
        {
            ((TreeViewItem)treeView.Items[0]).IsSelected = false;
            ((TreeViewItem)treeView.Items[0]).IsSelected = true;

            mainLabel.Visibility = Visibility.Visible;
            subLabel.Visibility = Visibility.Visible;
        }

        private void SetupGridColumns()
        {
            dataGrid.IsReadOnly = true;
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Data ID", Binding = new Binding("CrossTabCommon.CommonProperties.DataID") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Year", Binding = new Binding("CrossTabCommon.CommonProperties.Year") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Month", Binding = new Binding("CrossTabCommon.CommonProperties.Month") });

            DataGridTextColumn col = new DataGridTextColumn { Header = "Date", Binding = new Binding("CrossTabCommon.CommonProperties.SamplingDate") };
            col.Binding.StringFormat = "MMM-dd-yyyy";
            col.IsReadOnly = true;
            dataGrid.Columns.Add(col);

            dataGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Sampling day", Binding = new Binding("CrossTabCommon.CommonProperties.SamplingDay") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Province", Binding = new Binding("CrossTabCommon.CommonProperties.Province") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Municipality", Binding = new Binding("CrossTabCommon.CommonProperties.Municipality") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Region", Binding = new Binding("CrossTabCommon.CommonProperties.Region") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("CrossTabCommon.CommonProperties.FMA") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("CrossTabCommon.CommonProperties.FishingGround") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("CrossTabCommon.CommonProperties.LandingSite") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("CrossTabCommon.CommonProperties.Enumerator") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("CrossTabCommon.CommonProperties.Sector") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Grid", Binding = new Binding("CrossTabCommon.CommonProperties.FishingGroundGrid") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Longitude", Binding = new Binding("CrossTabCommon.CommonProperties.xCoordinate") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Latitude", Binding = new Binding("CrossTabCommon.CommonProperties.yCoordinate") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("CrossTabCommon.CommonProperties.Gear") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("CrossTabCommon.CommonProperties.FBName") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Number of fishers", Binding = new Binding("CrossTabCommon.CommonProperties.NumberOfFishers") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "FBL", Binding = new Binding("CrossTabCommon.CommonProperties.FBL") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "FBM", Binding = new Binding("CrossTabCommon.CommonProperties.FBM") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Total weight of catch", Binding = new Binding("CrossTabCommon.CommonProperties.Notes") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Notes", Binding = new Binding("CrossTabCommon.CommonProperties.TotalWeight") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Family", Binding = new Binding("CrossTabCommon.Family") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Species", Binding = new Binding("CrossTabCommon.SN") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Species weight", Binding = new Binding("CrossTabCommon.SpeciesWeight") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "TWS", Binding = new Binding("CrossTabCommon.TWS") });
            //dataGrid.Columns.Add(new DataGridTextColumn { Header = "Weight unit", Binding = new Binding("CrossTabCommon.WeightUnit") });


        }

        private void OnTreeItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                TreeViewItem tvItem = (TreeViewItem)e.NewValue;
                string topic = tvItem.Tag.ToString();
                dataGrid.Columns.Clear();

                dataGrid.AutoGenerateColumns = false;
                if (topic != "effort" && topic != "effort_all")
                {
                    SetupGridColumns();
                }

                var eh = CrossTabManager.AllSamplingEntitiesEventHandler;

                string ls = eh.LandingSiteText;
                string gear = eh.GearUsed == null ? "" : eh.GearUsed;
                string month = eh.MonthSampled == null ? "" : ((DateTime)eh.MonthSampled).ToString("MMMM, yyyy");
                string location = $"{eh.NSAPRegion.ShortName} {eh.FMA.Name} {eh.FishingGround.Name}";

                if (eh.GearUsed != null)
                {
                    subLabel.Content = $"Vessel landings in {ls} using {gear} - {month}";
                    _filePath = $"Crosstab {location} {gear} - {ls} - {month}";
                }
                else if (eh.MonthSampled != null)
                {
                    subLabel.Content = $"Vessel landings in {ls} on {month}";
                    _filePath = $"Crosstab {location} {ls} - {month}";
                }
                else
                {
                    subLabel.Content = $"Vessel landings in {ls}";
                    _filePath = $"Crosstab {location} {ls} - {month}";
                }
                switch (topic)

                {
                    case "effort":
                        dataGrid.DataContext = CrossTabManager.CrossTabEfforts;
                        dataGrid.AutoGenerateColumns = true;
                        mainLabel.Content = "Crosstab between catch composition and effort indicators";

                        break;
                    case "effort_all":
                        dataGrid.DataContext = CrossTabManager.CrossTabAllEfforts;
                        dataGrid.AutoGenerateColumns = true;
                        mainLabel.Content = "Crosstab of effort indicators";
                        break;
                    case "lenfreq":
                        dataGrid.DataContext = CrossTabManager.CrossTabLenFreqs;
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Length", Binding = new Binding("Length") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Frequency", Binding = new Binding("Freq") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Sex", Binding = new Binding("Sex") });
                        mainLabel.Content = "Crosstab between length frequency of catch and fishing effort";
                        break;
                    case "len":
                        dataGrid.DataContext = CrossTabManager.CrossTabLengths;
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Length", Binding = new Binding("Length") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Sex", Binding = new Binding("Sex") });
                        mainLabel.Content = "Crosstab between length of catch and fishing effort";
                        break;
                    case "len_wt":
                        dataGrid.DataContext = CrossTabManager.CrossTabLengthWeights;
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Length", Binding = new Binding("Length") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Weight", Binding = new Binding("Weight") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Weight unit", Binding = new Binding("CrossTabCommon.WeightUnit") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Sex", Binding = new Binding("Sex") });
                        mainLabel.Content = "Crosstab between length/weight of catch and fishing effort";
                        break;
                    case "maturity":
                        dataGrid.DataContext = CrossTabManager.CrossTabMaturities;
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Length", Binding = new Binding("Length") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Weight", Binding = new Binding("Weight") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Weight unit", Binding = new Binding("CrossTabCommon.WeightUnit") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Sex", Binding = new Binding("Sex") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Maturity", Binding = new Binding("MaturityStage") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Gonad weight", Binding = new Binding("GonadWeight") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Gut content", Binding = new Binding("GutContent") });
                        mainLabel.Content = "Crosstab between maturity indicators of catch and fishing effort";
                        break;
                }
            }
        }

        private void ExportToExcel()
        {
            //string filePath;
            string exportResult;
            string file = ExportExcel.GetSaveAsExcelFileName(this, _filePath);
            if (file.Length > 0)
            {
                if (ExportExcel.ExportDatasetToExcel(CrossTabManager.CrossTabDataSet, file))
                {
                    exportResult = "Successfully exported to excel";
                }
                else
                {
                    if (CrossTabManager.ErrorMessage.Length > 0)
                    {
                        exportResult = $"Was not successfull in exporting to excel\r\n{CrossTabManager.ErrorMessage}";
                    }
                    else
                    {
                        exportResult = $"Was not successfull in exporting to excel\r\n{ExportExcel.ErrorMessage}";
                    }
                }

                MessageBox.Show(exportResult, "Export", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void OnGridLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            ((MainWindow)Owner).Focus();
        }
    }
}
