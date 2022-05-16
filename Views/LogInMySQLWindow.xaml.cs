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
using NSAP_ODK.NSAPMysql;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for LogInMySQLWindow.xaml
    /// </summary>
    public partial class LogInMySQLWindow : Window
    {
        private static LogInMySQLWindow _instance;
        public LogInMySQLWindow()
        {
            InitializeComponent();

        }

        public static LogInMySQLWindow GetInstance()
        {
            if (_instance == null) _instance = new LogInMySQLWindow();

            return _instance;
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }
        public LogInMySQLWindow(Entities.MySQLConnectEventArgs ev)
        {
            InitializeComponent();
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonStats":
                    
                    MySQLDataStatisticsWindow msdw = new MySQLDataStatisticsWindow();
                    msdw.ShowDialog();
                    DialogResult = true;
                    break;
                case "buttonOk":
                    if (MySQLConnect.DatabaseExists)
                    {
                        DialogResult = true;
                        //Close();
                    }
                    else
                    {
                        MySQLConnect.UserName = textUserName.Text;
                        MySQLConnect.Password = textPassword.Password;
                        if (MySQLConnect.SetUP())
                        {
                            DialogResult = true;
                        }
                        else if (!MySQLConnect.DatabaseExists && MySQLConnect.ValidUser)
                        {
                            labelMessage.Content = "NSAP-ODK Database not found";
                            buttonCreate.IsEnabled = MySQLConnect.UserCanCreateDatabase;
                        }
                        else if (MySQLConnect.LastError.Length > 0)
                        {
                            MessageBox.Show(MySQLConnect.LastError, "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else if (MySQLConnect.DatabaseExists)
                        {
                            //labelMessage.Content = $"NSAP-ODK Database ready with {MySQLConnect.TableCount} tables";
                            labelMessage.Content = "NSAP-ODK Database ready ";
                            //TablesStats.GetStats();
                            buttonStats.IsEnabled = true;
                        }
                    }


                    break;
                case "buttonCancel":
                    
                    DialogResult = false;
                    break;
                case "buttonCreate":
                    MySQLConnect.CreateDatabase = true;
                    if (MySQLConnect.SetUP())
                    {
                        labelMessage.Content = $"NSAP-ODK Database created with {MySQLConnect.TableCount} tables";
                        buttonStats.IsEnabled = true;
                    }
                    break;
            }
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            textUserName.Focus();
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _instance = null;
            this.SavePlacement();
        }
    }
}
