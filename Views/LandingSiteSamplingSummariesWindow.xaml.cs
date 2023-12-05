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
using NSAP_ODK.Entities;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for LandingSiteSamplingSummariesWindow.xaml
    /// </summary>
    public partial class LandingSiteSamplingSummariesWindow : Window
    {
        private List<LandingSiteSamplingSummarized> _list_landingSiteSampling_summarized;
        private LandingSiteSamplingSummarized _selectedItem;
        private VesselUnloadEditWindow _vesselUnloadWindow;
        public LandingSiteSamplingSummariesWindow(List<LandingSiteSamplingSummarized> list_landingSiteSampling_summarized)
        {
            InitializeComponent();
            Loaded += LandingSiteSamplingSummariesWindow_Loaded;
            Closing += LandingSiteSamplingSummariesWindow_Closing;
            dataGrid.SelectionChanged += DataGrid_SelectionChanged;
            dataGrid.MouseDoubleClick += DataGrid_MouseDoubleClick;
            _list_landingSiteSampling_summarized = list_landingSiteSampling_summarized;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (_selectedItem != null)
                {

                    LandingSiteSamplingWindow lssw = new LandingSiteSamplingWindow(_selectedItem.LandingSiteSampling);
                    lssw.Owner = this;
                    lssw.Show();

                }
            }
            catch(Exception ex)
            {

            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedItem = (LandingSiteSamplingSummarized)dataGrid.SelectedItems[0];
        }

        public LandingSiteSamplingSummariesWindow()
        {
            InitializeComponent();
            Loaded += LandingSiteSamplingSummariesWindow_Loaded;
            Closing += LandingSiteSamplingSummariesWindow_Closing;
        }

        private void LandingSiteSamplingSummariesWindow_Closing(object sender, EventArgs e)
        {
            this.SavePlacement();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }

        private void LandingSiteSamplingSummariesWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid.AutoGenerateColumns = false;
            //dataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
            dataGrid.SelectionMode = DataGridSelectionMode.Single;
            dataGrid.ItemsSource = _list_landingSiteSampling_summarized;

            DataGridTextColumn col = new DataGridTextColumn()
            {
                Binding = new Binding("SamplingDate"),
                Header = "Sampling date"
            };
            
            col.Binding.StringFormat = "MMM-dd-yyyy";
            col.IsReadOnly = true;
            dataGrid.Columns.Add(col);

            dataGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Sampling day", Binding = new Binding("IsSamplingDay") ,IsReadOnly=true});
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "# of landings", Binding = new Binding("NumberOfVesselUnloads"),IsReadOnly=true });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "# with catch composition", Binding = new Binding("NumberOfVesselUnloadsWithCatchComposition") , IsReadOnly = true });
            dataGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "For deletion", Binding = new Binding("ForDeletion") });

            labelTitle.Content = $"{_list_landingSiteSampling_summarized[0].LandingSiteSampling.LandingSiteName} - {_list_landingSiteSampling_summarized[0].MonthOfSampling.ToString("MMMM, yyyy")}";
            Title = _list_landingSiteSampling_summarized[0].LandingSiteSampling.LandingSiteName;

        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonOk":
                    break;
                case "buttonCancel":
                    Close();
                    break;
            }
        }



        private void OnGridLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
