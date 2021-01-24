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
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Data ID", Binding = new Binding("CrossTabCommon.DataID") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("CrossTabCommon.FishingGround") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Year", Binding = new Binding("CrossTabCommon.Year") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Month", Binding = new Binding("CrossTabCommon.Month")  });

            DataGridTextColumn col = new DataGridTextColumn { Header = "Date", Binding = new Binding("CrossTabCommon.SamplingDate") };
            col.Binding.StringFormat = "MMM-dd-yyyy";
            col.IsReadOnly = true;
            dataGrid.Columns.Add(col);

            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Province", Binding = new Binding("CrossTabCommon.ProvinceName")  });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Municipality", Binding = new Binding("CrossTabCommon.MunicipalityName")  });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("CrossTabCommon.LandingSite.LandingSiteName")  });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("CrossTabCommon.Sector")  });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Grid", Binding = new Binding("CrossTabCommon.FishingGroundGrid")  });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("CrossTabCommon.GearName")  });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("CrossTabCommon.FBName")  });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "FBL", Binding = new Binding("CrossTabCommon.FBL")  });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "FBM", Binding = new Binding("CrossTabCommon.FBM")  });
            dataGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Sampling day", Binding = new Binding("CrossTabCommon.SamplingDay")  });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Family", Binding = new Binding("CrossTabCommon.Family")  });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Species", Binding = new Binding("CrossTabCommon.SN")  });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Catch", Binding = new Binding("CrossTabCommon.Weight")  });

        }

        private void OnTreeItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                TreeViewItem tvItem = (TreeViewItem)e.NewValue;
                string topic = tvItem.Tag.ToString();
                dataGrid.Columns.Clear();

                dataGrid.AutoGenerateColumns = false;
                if (topic != "effort")
                {
                    SetupGridColumns();
                }
                subLabel.Content = $"Month of vessel landings: {((DateTime)CrossTabManager.AllSamplingEntitiesEventHandler.MonthSampled).ToString("MMM-yyyy")}";
                switch (topic)

                {
                    case "effort":
                        dataGrid.DataContext = CrossTabManager.CrossTabEfforts;
                        dataGrid.AutoGenerateColumns = true;
                        mainLabel.Content = "Crosstab between catch composition and effort indicators";

                        break;
                    case "lenfreq":
                        dataGrid.DataContext = CrossTabManager.CrossTabLenFreqs;
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Length", Binding = new Binding("Length")  });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Frequency", Binding = new Binding("Freq")  });
                        mainLabel.Content = "Crosstab between length frequency of catch and fishing effort";
                        break;
                    case "len":
                        dataGrid.DataContext = CrossTabManager.CrossTabLengths;
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Length", Binding = new Binding("Length")  });
                        mainLabel.Content = "Crosstab between length of catch and fishing effort";
                        break;
                    case "maturity":
                        dataGrid.DataContext = CrossTabManager.CrossTabMaturities;
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Length", Binding = new Binding("Length") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Weight", Binding = new Binding("Weight") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Sex", Binding = new Binding("Sex") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Maturity", Binding = new Binding("MaturityStage") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Gut content", Binding = new Binding("GutContent") });
                        mainLabel.Content = "Crosstab between maturity indicators of catch and fishing effort";
                        break;
                }
            }
        }

        private void ExportToExcel()
        {
            string filePath;
            string exportResult;
            if (ExportExcel.GetSaveAsExcelFileName(this, out filePath))
            {
                if (ExportExcel.ExportDatasetToExcel(CrossTabManager.CrossTabDataSet, filePath))
                {
                    exportResult = "Successfully exported to excel";
                }
                else
                {
                    exportResult = $"Was not successfull in exporting to excel\r\n{ExportExcel.ErrorMessage}";
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
    }
}
