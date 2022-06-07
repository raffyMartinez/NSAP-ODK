using Ookii.Dialogs.Wpf;
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
using MySql.Data.MySqlClient;
using NSAP_ODK.NSAPMysql;
using NSAP_ODK.Utilities;
using System.IO;
using System.Windows.Threading;
using System.Diagnostics;
using swf = System.Windows.Forms;

namespace NSAP_ODK.Views

{
    /// <summary>
    /// Interaction logic for backupMySQLWindow.xaml
    /// </summary>
    public partial class backupMySQLWindow : Window
    {
        private static backupMySQLWindow _instance;
        private bool _backupDBOTablesOnly = false;
        public backupMySQLWindow()
        {
            InitializeComponent();
            Loaded += BackupMySQLWindow_Loaded;
            Closing += BackupMySQLWindow_Closing;
        }

        private void BackupMySQLWindow_Loaded(object sender, RoutedEventArgs e)
        {
            chkDBOTablesOOnly.Visibility = Visibility.Collapsed;
            if (Debugger.IsAttached)
            {
                chkDBOTablesOOnly.Visibility = Visibility.Visible;
            }
            progressLabel.Content = "";
            labelFolderPath.Content = Utilities.Global.Settings.MySQLBackupFolder;
        }

        private void BackupMySQLWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _instance = null;
        }

        public static backupMySQLWindow GetInstance()
        {
            if (_instance == null)
                _instance = new backupMySQLWindow();

            return _instance;

        }
        string[] _tablesToInclude = new string[13];
        private void AddToTables(int index, string tableName)
        {
            _tablesToInclude[index] = tableName;
        }
        public bool BackupDBOTablesOnly
        {
            get { return _backupDBOTablesOnly; }
            set
            {
                _backupDBOTablesOnly = value;
                if (_backupDBOTablesOnly)
                {
                    
                    TablesToInclude = new List<string>();
                    TablesStats.GetStats();
                    int index = 0;
                    foreach (var item in TablesStats.TablesStatistics)
                    {
                        if (item.TableName.Substring(0, 3).ToLower() == "dbo")
                        {
                            switch(item.TableName)
                            {
                                case "dbo_lc_fg_sample_day":
                                    index = 0;
                                    break;
                                case "dbo_lc_fg_sample_day_1":
                                    index = 1;
                                    break;
                                case "dbo_gear_unload":
                                    index = 2;
                                    break;
                                case "dbo_vessel_unload":
                                    index = 3;
                                    break;
                                case "dbo_vessel_unload_1":
                                    index = 4;
                                    break;
                                case "dbo_fg_grid":
                                    index = 5;
                                    break;
                                case "dbo_gear_soak":
                                    index = 6;
                                    break;
                                case "dbo_vessel_effort":
                                    index = 7;
                                    break;
                                case "dbo_vessel_catch":
                                    index = 8;
                                    break;
                                case "dbo_catch_maturity":
                                    index = 9;
                                    break;
                                case "dbo_catch_len_freq":
                                    index = 10;
                                    break;
                                case "dbo_catch_len_wt":
                                    index = 11;
                                    break;
                                case "dbo_catch_length":
                                    index = 12;
                                    break;
                                case "dbo_vessel_unload_stats":
                                    index = 13;
                                    break;

                            }
                            AddToTables(index, item.TableName);
                            
                        }
                        
                    }
                    foreach(var item1 in _tablesToInclude)
                    {
                        TablesToInclude.Add(item1);
                    }
                }
            }
        }

        public List<string> TablesToInclude { get; set; }
        private Task<bool> BackupAsync()
        {
            return Task.Run(() => BackUp());
        }
        private bool BackUp()
        {
            StartBackup();
            bool success = false;
            string backupFileName = $@"{Global.Settings.MySQLBackupFolder}\NSAP-ODK Database {DateTime.Now:yyyy-MMM-dd} {DateTime.Now:HH}h {DateTime.Now:mm}m backup.sql";
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        try
                        {
                            conn.Open();
                            mb.ExportInfo.ExportEvents = true;
                            mb.ExportProgressChanged += Mb_ExportProgressChanged;
                            if (TablesToInclude.Count > 0)
                            {
                                mb.ExportInfo.TablesToBeExportedList = TablesToInclude;
                            }
                            mb.ExportToFile(backupFileName);
                            if (File.Exists(backupFileName))
                            {
                                success = true;
                            }
                            mb.ExportProgressChanged -= Mb_ExportProgressChanged;
                        }
                        catch (MySqlException msex)
                        {
                            Logger.Log(msex);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                        conn.Close();
                    }
                }
            }
            return success;
        }

        private void Mb_ExportProgressChanged(object sender, ExportProgressArgs e)
        {

            progressBar.Dispatcher.BeginInvoke
            (
              DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
              {
                  progressBar.Maximum = e.TotalRowsInCurrentTable;
                  progressBar.Value = e.CurrentRowIndexInCurrentTable;
                  //do what you need to do on UI Thread
                  return null;
              }), null);

            progressLabel.Dispatcher.BeginInvoke
           (
             DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
             {

                 progressLabel.Content = $"Backing up {e.CurrentTableName}";

                 //do what you need to do on UI Thread
                 return null;
             }
            ), null);
        }

        private string _sqlFile;
        private void RestoreSQL()
        {
            string constring = MySQLConnect.ConnectionString();
            using (MySqlConnection conn = new MySqlConnection(constring))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        mb.ImportProgressChanged += Mb_ImportProgressChanged;
                        try
                        {
                            mb.ImportFromFile(_sqlFile);
                        }
                        catch (IOException iox)
                        {
                            MessageBox.Show(iox.Message, "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                        mb.ImportProgressChanged -= Mb_ImportProgressChanged;

                        conn.Close();
                    }
                }
            }
        }


        private void StartBackup()
        {
            progressBar.Dispatcher.BeginInvoke
            (
              DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
              {
                  progressBar.IsIndeterminate = true;
                  //do what you need to do on UI Thread
                  return null;
              }), null);

        }
        private void Mb_ImportProgressChanged(object sender, ImportProgressArgs e)
        {
            progressBar.Dispatcher.BeginInvoke
            (
              DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
              {
                  progressBar.IsIndeterminate = false;
                  progressBar.Maximum = 100;
                  progressBar.Value = e.PercentageCompleted;
                  //do what you need to do on UI Thread
                  return null;
              }), null);

            progressLabel.Dispatcher.BeginInvoke
           (
             DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
             {

                 progressLabel.Content = $"Restoring file {_sqlFile}";

                 //do what you need to do on UI Thread
                 return null;
             }
            ), null);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            switch (((Button)sender).Name)
            {
                case "buttonRestore":
                    swf.OpenFileDialog ofd = new swf.OpenFileDialog();
                    ofd.Title = "Open sql file to restore backup";
                    ofd.InitialDirectory = Global.Settings.MySQLBackupFolder;
                    ofd.DefaultExt = "*.sql";
                    ofd.Filter = "sql file (*.sql)|*.sql|All files (*.*)|*.*";
                    if (ofd.ShowDialog() == swf.DialogResult.OK && File.Exists((ofd.FileName)))
                    {
                        _sqlFile = ofd.FileName;
                        RestoreSQL();
                    }
                    break;
                case "buttonOk":
                    BackupDBOTablesOnly = (bool)chkDBOTablesOOnly.IsChecked;
                    if (await BackupAsync())
                    {
                        MessageBox.Show("Backup succeeded!",
                                         "NSAP-ODK Database",
                                         MessageBoxButton.OK,
                                         MessageBoxImage.Information
                                        );
                        Close();
                    }
                    break;
                case "buttonCancel":
                    Close();
                    break;
            }
        }
    }
}
