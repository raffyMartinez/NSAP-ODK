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
using NSAP_ODK.NSAPMysql;
namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for MySQLDataStatistics.xaml
    /// </summary>
    public partial class MySQLDataStatisticsWindow : Window
    {
        public MySQLDataStatisticsWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            dgStats.ItemsSource = NSAP_ODK.NSAPMysql.TablesStats.TablesStatistics;
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch(((Button)sender).Name)
            {
                case "buttonOk":
                    DialogResult = true;
                    break;

                case "buttonCopy":
                    MigrateDataToMySQL.Migrate();
                    break;
            }
        }
    }
}
