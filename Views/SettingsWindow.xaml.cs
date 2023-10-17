using Microsoft.Win32;
using NSAP_ODK.Entities.Database;
using NSAP_ODK.Utilities;
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
using SysIo = System.IO;

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
            if (Global.Settings != null)
            {
                textBackenDB.Text = Global.Settings.MDBPath;
                textJsonFolder.Text = Global.Settings.JSONFolder;
                textmySQLBackupFolder.Text = Global.Settings.MySQLBackupFolder;
                chkUsemySQL.IsChecked = Global.Settings.UsemySQL;
                buttonLocateMySQlBackupFolder.IsEnabled = (bool)chkUsemySQL.IsChecked;
                chkUsemySQL.Click += ChkUsemySQL_Click;


                textJsonFolder.MouseDoubleClick += OnTextBoxDoubleClick;
                textBackenDB.MouseDoubleClick += OnTextBoxDoubleClick;

                if (Global.Settings.CutOFFUndersizedCW == null)
                {
                    textCutoffWidth.Text = Utilities.Settings.DefaultCutoffUndesizedCW.ToString();
                }
                else
                {
                    textCutoffWidth.Text = ((int)Global.Settings.CutOFFUndersizedCW).ToString();
                }

                if (Global.Settings.DownloadSizeForBatchMode == null)
                {
                    textDownloadSizeForBatchMode.Text = Utilities.Settings.DefaultDownloadSizeForBatchMode.ToString();
                }
                else
                {
                    textDownloadSizeForBatchMode.Text = ((int)Utilities.Global.Settings.DownloadSizeForBatchMode).ToString();
                }

                if (Global.Settings.DownloadSizeForBatchModeMultiVessel == null)
                {
                    textDownloadSizeForBatchModeMultivessel.Text = Utilities.Settings.DefaultDownloadSizeForBatchModeMultiVessel.ToString();
                }
                else
                {
                    textDownloadSizeForBatchModeMultivessel.Text = ((int)Utilities.Global.Settings.DownloadSizeForBatchModeMultiVessel).ToString();
                }

                if (Global.Settings.DbFilter == null)
                {
                    textDBFilter.Text = "";
                }
                else
                {
                    textDBFilter.Text = Global.Settings.DbFilter.ToString();
                }

                textAcceptableDiff.Text = ((int)Utilities.Global.Settings.AcceptableWeightsDifferencePercent).ToString();
            }
            else
            {
                textDownloadSizeForBatchModeMultivessel.Text = "100";
                textDownloadSizeForBatchMode.Text = "2000";
                textCutoffWidth.Text = "11";
                textAcceptableDiff.Text = "10";
            }

        }

        private void OnTextBoxDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var txt = (TextBox)sender;
            if (txt.Text.Length > 0)
            {
                switch (txt.Name)
                {
                    case "textBackenDB":
                        System.Diagnostics.Process.Start(SysIo.Path.GetDirectoryName(txt.Text));
                        break;
                    case "textJsonFolder":
                        System.Diagnostics.Process.Start(txt.Text);
                        break;
                }
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
            string msg4 = "Values must be a positive, whole number greater than zero";
            string msg5 = "Filter must be a date";
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
            else if ((bool)chkUsemySQL.IsChecked == false)
            {
                msg3 = "";
            }

            if (textDBFilter.Text.Length > 0)
            {
                if (DateTime.TryParse(textDBFilter.Text, out DateTime v))
                {
                    msg5 = "";
                }
            }

            if (textCutoffWidth.Text.Length > 0 && textDownloadSizeForBatchMode.Text.Length > 0 && textAcceptableDiff.Text.Length > 0)
            {
                if (int.TryParse(textAcceptableDiff.Text, out int v))
                {
                    if (int.TryParse(textCutoffWidth.Text, out v))
                    {
                        if (int.TryParse(textDownloadSizeForBatchMode.Text, out v))
                        {
                            msg4 = "";
                        }
                    }
                }
            }




            if (msg1.Length > 0 && msg2.Length > 0 && msg3.Length > 0 && msg4.Length > 0 )
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
            if (msg.Length > 0 || msg1.Length > 0 || msg2.Length > 0 || msg3.Length > 0 || msg4.Length > 0 || msg5.Length > 0)
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

                if (msg4.Length > 0)
                {
                    if (allMessages.Length > 0)
                    {
                        allMessages += msg4 + "\r\n";
                    }
                    else
                    {
                        allMessages = msg4;
                    }

                }

                if (msg5.Length > 0)
                {
                    if (allMessages.Length > 0)
                    {
                        allMessages += msg5 + "\r\n";
                    }
                    else
                    {
                        allMessages = msg5;
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
                        CreateTablesInAccess.GetMDBColumnInfo(Global.ConnectionString);
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
                        Utilities.Global.Settings.AcceptableWeightsDifferencePercent = int.Parse(textAcceptableDiff.Text);
                        if (int.TryParse(textDownloadSizeForBatchMode.Text, out int v))
                        {
                            Utilities.Global.Settings.DownloadSizeForBatchMode = v;
                            Utilities.Settings.DefaultDownloadSizeForBatchMode = v;
                        }
                        if (int.TryParse(textDownloadSizeForBatchModeMultivessel.Text, out v))
                        {
                            Utilities.Global.Settings.DownloadSizeForBatchModeMultiVessel = v;
                            Utilities.Settings.DefaultDownloadSizeForBatchModeMultiVessel = v;
                        }
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
