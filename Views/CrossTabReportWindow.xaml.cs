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

        public CrossTabReportWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
            Closed += OnWindowClosed;
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            this.SavePlacement();
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            ((TreeViewItem)treeView.Items[0]).IsSelected = true;
        }

        private void OnTreeItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            dataGrid.Columns.Clear();
            switch(((TreeViewItem)sender).Tag.ToString())
            {
                case "effort":
                    break;
                case "lenfreq":
                    dataGrid.DataContext = CrossTabManager.CrossTabLenFreqs;
                    break;
                case "len":
                    dataGrid.DataContext = CrossTabManager.CrossTabLengths;
                    break;
                case "maturity":
                    dataGrid.DataContext = CrossTabManager.CrossTabMaturities;
                    break;
            }
        }

        private void OnMenuClicked(object sender, RoutedEventArgs e)
        {

        }
    }
}
