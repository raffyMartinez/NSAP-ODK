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

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for DownloadFromServerOptionsWindow.xaml
    /// </summary>
    public partial class DownloadFromServerOptionsWindow : Window
    {
        public DownloadFromServerOptionsWindow()
        {
            InitializeComponent();
            Loaded += DownloadFromServerOptionsWindow_Loaded;
        }


        private void DownloadFromServerOptionsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            labelNumberOfItemsToDownload.Content += CountItemsToDownload.ToString();
            chkSaveToJSONFile.IsChecked = false;
            chkSaveToJSONFile.IsEnabled = false;
            if (Utilities.Global.Settings.DownloadSizeForBatchMode == null)
            {
                txtNumberToDownload.Text = Utilities.Settings.DefaultDownloadSizeForBatchMode.ToString();
            }
            else
            {
                txtNumberToDownload.Text = ((int)Utilities.Global.Settings.DownloadSizeForBatchMode).ToString();
            }

            if(KoboForm.title.Contains("Multi-VesselGear"))
            {
                if (Utilities.Global.Settings.DownloadSizeForBatchModeMultiVessel == null)
                {
                    txtNumberToDownload.Text = Utilities.Settings.DefaultDownloadSizeForBatchModeMultiVessel.ToString();
                }
                else
                {
                    txtNumberToDownload.Text = ((int)Utilities.Global.Settings.DownloadSizeForBatchModeMultiVessel).ToString();
                }
            }
        }

        private bool FormValidated()
        {
            return (bool)rbDownloadAll.IsChecked || (bool)rbDownloadByBatch.IsChecked;
        }
        public int CountItemsToDownload { get; set; }
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonCancel":
                    DialogResult = false;
                    break;
                case "buttonOk":
                    if (FormValidated())
                    {
                        ((DownloadFromServerWindow)Owner).SaveDownloadAsJSON = (bool)chkSaveToJSONFile.IsChecked;
                        ((DownloadFromServerWindow)Owner).DownloadOptionDownloadAll = (bool)rbDownloadAll.IsChecked;
                        ((DownloadFromServerWindow)Owner).DownloadAsJSONNotes = txtNotes.Text;
                        if ((bool)rbDownloadByBatch.IsChecked)
                        {
                            string msg = "Pls provide number of items to download per batch\r\n(Must be a positive, whole number)";
                            if (int.TryParse(txtNumberToDownload.Text, out int v))
                            {
                                if (v > 0)
                                {
                                    ((DownloadFromServerWindow)Owner).NumberToDownloadPerBatch = int.Parse(txtNumberToDownload.Text);
                                    DialogResult = true;
                                    msg = "";
                                }
                            }

                            if (msg.Length > 0)
                            {
                                MessageBox.Show(msg, Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        else
                        {
                            DialogResult = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show(
                            "Please select a download option", 
                            Utilities.Global.MessageBoxCaption, 
                            MessageBoxButton.OK, 
                            MessageBoxImage.Information);
                    }
                    //DialogResult = true;
                    break;
            }
        }

        public Entities.KoboForm KoboForm { get; set; }

        private void OnRBChecked(object sender, RoutedEventArgs e)
        {
            panelOptions.Visibility = Visibility.Collapsed;
            if (((RadioButton)sender).Name == "rbDownloadByBatch")
            {
                panelOptions.Visibility = Visibility.Visible;

            }
            chkSaveToJSONFile.IsChecked = (bool)rbDownloadByBatch.IsChecked;
            chkSaveToJSONFile.IsEnabled = (bool)rbDownloadByBatch.IsChecked;

        }
    }
}
