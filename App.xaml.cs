﻿using NSAP_ODK.Entities;
using NSAP_ODK.Entities.Database;
using NSAP_ODK.Utilities;
using System.Windows;

namespace NSAP_ODK
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {

        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length >= 1)
            {
                Global.CommandArgs = e.Args;
            }
            //MainWindow window = new MainWindow();   
            //if(e.Args.Length==1)
            //{
            //    window.CommandArgs = e.Args[0];
            //}
            //window.Show();
        }
    }
}