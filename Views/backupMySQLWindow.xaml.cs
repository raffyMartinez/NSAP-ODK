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
namespace NSAP_ODK.Views

{
    /// <summary>
    /// Interaction logic for backupMySQLWindow.xaml
    /// </summary>
    public partial class backupMySQLWindow : Window
    {
        private static backupMySQLWindow _instance;
        public backupMySQLWindow()
        {
            InitializeComponent();
            Loaded += BackupMySQLWindow_Loaded;
            Closing += BackupMySQLWindow_Closing;
        }

        private void BackupMySQLWindow_Loaded(object sender, RoutedEventArgs e)
        {
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
        private Task<bool> BackupAsync()
        {
            return Task.Run(() => BackUp());
        }
        private bool BackUp()
        {
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
                            mb.ExportToFile(backupFileName);
                            if (File.Exists(backupFileName))
                            {
                                success = true;
                            }
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
        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            switch (((Button)sender).Name)
            {
                case "buttonOk":
                    if(await BackupAsync())
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
