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
    /// Interaction logic for CrossTabReportWindow.xaml
    /// </summary>
    public partial class CrossTabReportWindow : Window
    {
        public CrossTabReportWindow()
        {
            InitializeComponent();
        }

        private void OnTreeItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            switch(((TreeViewItem)sender).Tag.ToString())
            {
                case "effort":
                    break;
                case "lenfreq":
                    break;
                case "len":
                    break;
                case "maturity":
                    break;
            }
        }

        private void OnMenuClicked(object sender, RoutedEventArgs e)
        {

        }
    }
}
