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
using NSAP_ODK.Entities;
namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for FirstSamplingOfEnumeratorsWindow.xaml
    /// </summary>
    public partial class FirstSamplingOfEnumeratorsWindow : Window
    {
        private static FirstSamplingOfEnumeratorsWindow _instance;
        private NSAPRegionEnumerator _nre;
        public FirstSamplingOfEnumeratorsWindow()
        {
            InitializeComponent();
            Loaded += FirstSamplingOfEnumeratorsWindow_Loaded;
            Closing += FirstSamplingOfEnumeratorsWindow_Closing;
        }
        public List<NSAPRegionEnumerator> FirstSamplings { get; set; }
        public static FirstSamplingOfEnumeratorsWindow GetInstance()
        {
            if (_instance == null) _instance = new FirstSamplingOfEnumeratorsWindow();
            return _instance;
        }
        private void FirstSamplingOfEnumeratorsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _instance = null;
        }

        private void FirstSamplingOfEnumeratorsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(FirstSamplings.Count>0)
            {
                gridFirstSampling.ItemsSource = null;
                gridFirstSampling.Columns.Clear();
                gridFirstSampling.Items.Clear();
                gridFirstSampling.ItemsSource = FirstSamplings;


                gridFirstSampling.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("Enumerator.Name") });
                gridFirstSampling.Columns.Add(new DataGridTextColumn { Header = "Region", Binding = new Binding("NSAPRegion.ShortName") });
                gridFirstSampling.Columns.Add(new DataGridTextColumn { Header = "Date of first sampling", Binding = new Binding("FirstSamplingDate") });
            }
            else
            {

            }
        }

        private void onButtonClick(object sender, RoutedEventArgs e)
        {
            switch(((Button)sender).Name)
            {
                case "buttonOk":

                    break;
                case "buttonCancel":
                    Close();
                    break;
            }
        }

        private void onMenuClick(object sender, RoutedEventArgs e)
        {
            switch(((MenuItem)sender).Name)
            {
                case "menuListLandings":
                    break;


            }
        }

        private void onRowLoading(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void onGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _nre = (NSAPRegionEnumerator)gridFirstSampling.SelectedItem;
        }

        private void onGridMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(_nre!=null)
            {

            }
        }
    }
}
