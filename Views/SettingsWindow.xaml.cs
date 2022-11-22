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
        private bool _chkMySQlClicked;
        public SettingsWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
        }

        public SettingsWindow(MainWindow owner)
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (Utilities.Global.Settings != null)
            {
                textBackenDB.Text = Utilities.Global.Settings.MDBPath;
                textJsonFolder.Text = Utilities.Global.Settings.JSONFolder;
                textmySQLBackupFolder.Text = Utilities.Global.Settings.MySQLBackupFolder;
                chkUsemySQL.IsChecked = Utilities.Global.Settings.UsemySQL;
                buttonLocateMySQlBackupFolder.IsEnabled = (bool)chkUsemySQL.IsChecked;
                chkUsemySQL.Click += ChkUsemySQL_Click;
                if (Utilities.Global.Settings.CutOFFUndersizedCW == null)
                {
                    textCutoffWidth.Text = Utilities.Settings.DefaultCutoffUndesizedCW.ToString();
                }
                else
                {
                    textCutoffWidth.Text = ((int)Utilities.Global.Settings.CutOFFUndersizedCW).ToString();
                }

                if(Utilities.Global.Settings.DownloadSizeForBatchMode==null)
                {
                    textDownloadSizeForBatchMode.Text = Utilities.Settings.DefaultDownloadSizeForBatchMode.ToString();
                }
                else
                {
                    textCutoffWidth.Text = ((int)Utilities.Global.Settings.DownloadSizeForBatchMode).ToString();
                }
            }
            else
            {
                textDownloadSizeForBatchMode.Text = "2000";
                textCutoffWidth.Text = "11";
            }

        }

        private void ChkUsemySQL_Click(object sender, RoutedEventArgs e)
        {
            _chkMySQlClicked = true;
            buttonLocateMySQlBackupFolder.IsEnabled = (bool)chkUsemySQL.IsChecked;
        }

        private bool ValidateForm()
        {
            string allMessages = "";
            string msg1 = "Path to backend BD cannot be empty";
            string msg2 = "Name of folder for saving backup JSON files cannot be empyy";
            string msg3 = "Name of folder of NSAP-ODK Databse for MySQL cannot be empyy";
            string msg = "";

            if (textBackenDB.Text.Length > 0)
            {
                msg1 = "";
            }

            if (textJsonFolder.Text.Length > 0)
            {
                msg2 = "";
            }

            if ((bool)chkUsemySQL.IsChecked && textmySQLBackupFolder.Text.Length > 0)
            {
                msg3 = "";
            }
            else if((bool)chkUsemySQL.IsChecked==false)
            {
                msg3 = "";
            }

            if (msg1.Length == 0 && msg2.Length == 0 && msg3.Length==0)
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
            if (msg.Length > 0 || msg1.Length > 0 || msg2.Length > 0 || msg3.Length > 0)
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
                if (msg3.Length > 0)
                {
                    if (allMessages.Length > 0)
                    {
                        allMessages += msg3 + "\r\n";
                    }
                    else
                    {
                        allMessages = msg3;
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
            VistaFolderBrowserDialog fbd;
            switch (((Button)sender).Name)
            {
                case "buttonLocateMySQlBackupFolder":
                    fbd = new VistaFolderBrowserDialog();
                    fbd.UseDescriptionForTitle = true;
                    fbd.Description = "Locate folder to save NSAP-ODK Database for MySQL";

                    if (textmySQLBackupFolder.Text.Length > 0)
                    {
                        fbd.SelectedPath = textmySQLBackupFolder.Text;
                    }

                    if ((bool)fbd.ShowDialog() && fbd.SelectedPath.Length > 0)
                    {
                        textmySQLBackupFolder.Text = fbd.SelectedPath;
                    }
                    break;
                case "buttonLocateBackendDB":
                    Visibility = Visibility.Hidden;
                    //double oldWidth = Width;
                    //double oldHeight = Height;
                    //Height = 1;
                    //Width = 1;
                    if (((MainWindow)Owner).LocateBackendDB(out string backend))
                    {
                        textBackenDB.Text = backend;
                    }
                    Visibility = Visibility.Visible;
                    //Height = oldHeight;
                    //Width = oldWidth;
                    break;
                case "buttonLocateJsonFolder":
                    fbd = new VistaFolderBrowserDialog();
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
                        Utilities.Global.Settings.UsemySQL = (bool)chkUsemySQL.IsChecked;
                        Utilities.Global.Settings.MySQLBackupFolder = textmySQLBackupFolder.Text;
                        Utilities.Global.SaveGlobalSettings();


                        if (_chkMySQlClicked)
                        {
                            MessageBox.Show("The application need to restart to switch to another database backend", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                            ((MainWindow)Owner).CloseAppilication();
                        }
                        if (Owner != null && Owner.GetType().Name.Contains("MainWindow"))
                        {
                            ((MainWindow)Owner).SetMenuAndOtherToolbarButtonsVisibility(Visibility.Visible);
                            ((MainWindow)Owner).Focus();
                        }
                    }
                    Close();
                    break;
                case "buttonCancel":
                    Close();
                    break;
            }
        }
    }
}
