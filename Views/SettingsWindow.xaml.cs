using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            textBackenDB.Text = Utilities.Global.Settings.MDBPath;
            textJsonFolder.Text = Utilities.Global.Settings.JSONFolder;

        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch(((Button)sender).Name)
            {
                case "buttonLocateBackendDB":
                    Visibility = Visibility.Hidden;
                    if(((MainWindow)Owner).LocateBackendDB(out string backend))
                    {
                        textBackenDB.Text = backend;
                    }
                    Visibility = Visibility.Visible;
                    break;
                case "buttonLocateJsonFolder":
                    VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();
                    fbd.UseDescriptionForTitle = true;
                    fbd.Description = "Locate folder for saving JSON files";

                    if(textJsonFolder.Text.Length>0)
                    {
                        fbd.SelectedPath = textJsonFolder.Text;
                    }

                    if ((bool)fbd.ShowDialog() && fbd.SelectedPath.Length > 0)
                    {
                        textJsonFolder.Text = fbd.SelectedPath;
                    }
                    break;
                case "buttonOk":
                    Utilities.Global.Settings.MDBPath = textBackenDB.Text;
                    Utilities.Global.Settings.JSONFolder = textJsonFolder.Text;
                    Utilities.Global.SaveGlobalSettings();
                    Close();
                    break;
                case "buttonCancel":
                    Close();
                    break;
            }
        }
    }
}
