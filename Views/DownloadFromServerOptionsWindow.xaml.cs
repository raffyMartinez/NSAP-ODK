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

                    ((DownloadFromServerWindow)Owner).SaveDownloadAsJSON = (bool)chkSaveToJSONFile.IsChecked;
                    ((DownloadFromServerWindow)Owner).DownloadOptionDownloadAll = (bool)rbDownloadAll.IsChecked;
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
                            MessageBox.Show(msg, "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    //DialogResult = true;
                    break;
            }
        }

        private void OnRBChecked(object sender, RoutedEventArgs e)
        {
            panelOptions.Visibility = Visibility.Collapsed;
            if (((RadioButton)sender).Name == "rbDownloadByBatch")
            {
                panelOptions.Visibility = Visibility.Visible;
            }

        }
    }
}
