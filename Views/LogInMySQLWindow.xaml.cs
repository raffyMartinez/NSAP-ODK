﻿using System;
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
        private bool _databaseCreated;
        private Entities.MySQLConnectEventArgs _ev;

        public LogInMySQLWindow()
        {
            InitializeComponent();

        }
        public LogInMySQLWindow(Entities.MySQLConnectEventArgs ev)
        {
            InitializeComponent();
            _ev = ev;
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonStats":

                    MySQLDataStatisticsWindow msdw = new MySQLDataStatisticsWindow();
                    Visibility = Visibility.Collapsed;
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
                        MySQLConnect.Password = textPassword.Text;
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
                            labelMessage.Content = $"NSAP-ODK Database ready with {MySQLConnect.TableCount} tables";
                            TablesStats.GetStats();
                            buttonStats.IsEnabled = true;
                        }
                    }


                    break;
                case "buttonCancel":
                    DialogResult = false;
                    break;
                case "buttonCreate":
                    _databaseCreated = false;
                    MySQLConnect.CreateDatabase = true;
                    if (MySQLConnect.SetUP())
                    {
                        labelMessage.Content = $"NSAP-ODK Database created with {MySQLConnect.TableCount} tables";
                        _databaseCreated = true;
                    }
                    break;
            }
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            _databaseCreated = false;
        }


    }
}
