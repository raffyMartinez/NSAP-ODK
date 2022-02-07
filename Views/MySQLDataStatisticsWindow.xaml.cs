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
using System.Windows.Threading;
using NSAP_ODK.NSAPMysql;
using NSAP_ODK.Utilities;
namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for MySQLDataStatistics.xaml
    /// </summary>
    public partial class MySQLDataStatisticsWindow : Window
    {
        private static MySQLDataStatisticsWindow _instance;
        private DateTime _start;
        private DateTime _end;
        public MySQLDataStatisticsWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
        }

        public static MySQLDataStatisticsWindow GetInstance()
        {
            if (_instance == null) _instance = new MySQLDataStatisticsWindow();
            return _instance;
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            TablesStats.GetStats();
            dgStats.ItemsSource = TablesStats.TablesStatistics;
            labelCurrentTable.Visibility = Visibility.Collapsed;
            progressBarMigrate.Visibility = Visibility.Collapsed;
        }

        private async void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonOk":
                    DialogResult = true;
                    break;

                case "buttonCopy":
                    labelCurrentTable.Visibility = Visibility.Visible;
                    progressBarMigrate.Visibility = Visibility.Visible;
                    _start = DateTime.Now;
                    MigrateDataToMySQL.MigrateEvent += OnMigrateEvent;
                   if( await MigrateDataToMySQL.MigrateTablesAsync())
                    {
                        TablesStats.GetStats();
                        //dgStats.ItemsSource = TablesStats.TablesStatistics;
                        dgStats.Items.Refresh();

                        _end = DateTime.Now;
                        var timeDiff = _end - _start;
                        MessageBox.Show($"Finished copying data into mySQL in {timeDiff.Minutes} minutes and {timeDiff.Seconds} seconds", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                   MigrateDataToMySQL.MigrateEvent -= OnMigrateEvent;
                    break;
            }
        }

        private void OnMigrateEvent(object sender, EventArgs e)
        {
            var ev = (MigrateDataEventArg)e;
            switch (ev.Intent)
            {
                case "start":
                    labelCurrentTable.Dispatcher.BeginInvoke
                    (
                      DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                      {
                          labelCurrentTable.Content = $"Processing table: {ev.TableName}"; 
                          //do what you need to do on UI Thread
                          return null;
                      }), null);

                    progressBarMigrate.Dispatcher.BeginInvoke
                    (
                        DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                        {
                          progressBarMigrate.Maximum = ev.OriginalRowCount;
                          progressBarMigrate.Value = 0;
                          //do what you need to do on UI Thread
                          return null;
                        }),null);

                    break;

                case "copying":
                    progressBarMigrate.Dispatcher.BeginInvoke
                    (
                        DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                        {
                            progressBarMigrate.Value++;
                            //do what you need to do on UI Thread
                            return null;
                        }), null);

                    break;

                case "finished":
                    labelCurrentTable.Dispatcher.BeginInvoke
                    (
                      DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                      {
                          labelCurrentTable.Content = "Finished migrating data to MySQL";
                          //do what you need to do on UI Thread
                          return null;
                      }), null);

                    progressBarMigrate.Dispatcher.BeginInvoke
                    (
                        DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                        {
                            progressBarMigrate.Value=0;
                            //do what you need to do on UI Thread
                            return null;
                        }), null);
                    
                    break;
            }

        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SavePlacement();
            _instance = null;
        }
    }
}
