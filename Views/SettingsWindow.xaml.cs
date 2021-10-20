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
            if (Utilities.Global.Settings.CutOFFUndersizedCW == null)
            {
                textCutoffWidth.Text = Utilities.Settings.DefaultCutoffUndesizedCW.ToString(); ;
            }
            else
            {
                textCutoffWidth.Text = ((int)Utilities.Global.Settings.CutOFFUndersizedCW).ToString();
            }

        }
        private bool ValidateForm()
        {
            string allMessages = "";
            string msg1 = "Path to backend BD cannot be empty";
            string msg2 = "Name of folder for saving backup JSON files cannot be empyy";
            string msg = "";

            if (textBackenDB.Text.Length > 0)
            {
                msg1 = "";
            }

            if (textJsonFolder.Text.Length > 0)
            {
                msg2 = "";
            }

            if (msg1.Length == 0 && msg2.Length == 0)
            {
                msg = "Expected value cannot be empty and must be a whole number";
                string cutoff = textCutoffWidth.Text;
                if (!cutoff.Contains("."))
                {
                    if (int.TryParse(cutoff, out int v))
                    {
                        if (v >= 20)
                        {
                            msg = "Cut-off width cannot exceed max CW of crab";
                        }
                        else if (v <= 0)
                        {
                            msg = "Cut-off width must be greater than zero";
                        }
                        else
                        {
                            msg = "";
                        }
                    }
                }
            }
            if (msg.Length > 0 || msg1.Length > 0 || msg2.Length > 0)
            {

                if (msg1.Length > 0)
                {
                    allMessages = msg1;
                }
                if (msg2.Length > 0)
                {
                    if (allMessages.Length > 0)
                    {
                        allMessages += msg2 + "\r\n";
                    }
                    else
                    {
                        allMessages = msg2;
                    }

                }
                if (msg.Length > 0)
                {
                    if (allMessages.Length > 0)
                    {
                        allMessages += msg + "\r\n";
                    }
                    else
                    {
                        allMessages = msg;
                    }

                }
                MessageBox.Show(allMessages, "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            else
            {
                return true;
            }

        }
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonLocateBackendDB":
                    Visibility = Visibility.Hidden;
                    if (((MainWindow)Owner).LocateBackendDB(out string backend))
                    {
                        textBackenDB.Text = backend;
                    }
                    Visibility = Visibility.Visible;
                    break;
                case "buttonLocateJsonFolder":
                    VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();
                    fbd.UseDescriptionForTitle = true;
                    fbd.Description = "Locate folder for saving JSON files";

                    if (textJsonFolder.Text.Length > 0)
                    {
                        fbd.SelectedPath = textJsonFolder.Text;
                    }

                    if ((bool)fbd.ShowDialog() && fbd.SelectedPath.Length > 0)
                    {
                        textJsonFolder.Text = fbd.SelectedPath;
                    }
                    break;
                case "buttonOk":
                    if (ValidateForm())
                    {
                        Utilities.Global.Settings.MDBPath = textBackenDB.Text;
                        Utilities.Global.Settings.JSONFolder = textJsonFolder.Text;
                        Utilities.Global.Settings.CutOFFUndersizedCW = int.Parse(textCutoffWidth.Text);
                        Utilities.Global.SaveGlobalSettings();
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
