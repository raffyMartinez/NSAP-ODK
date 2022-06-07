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
    public enum UpdateJSONHistoryMode
    {
        UpdateNoneSelected,
        UpdateAll,
        UpdateMissingData
    }
    /// <summary>
    /// Interaction logic for UploadJSONHistoryOptionsWindow.xaml
    /// </summary>
    public partial class UploadJSONHistoryOptionsWindow : Window
    {
        public UploadJSONHistoryOptionsWindow()
        {
            InitializeComponent();
        }


        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonOk":
                    bool proceed = true;
                    if ((bool)rbReplace.IsChecked)
                    {
                        ((ODKResultsWindow)Owner).UpdateJSONHistoryMode = UpdateJSONHistoryMode.UpdateAll;
                    }
                    else if ((bool)rbUpdateMissing.IsChecked)
                    {
                        ((ODKResultsWindow)Owner).UpdateJSONHistoryMode = UpdateJSONHistoryMode.UpdateMissingData;
                    }
                    else
                    {
                        proceed = false;
                    }
                    if (proceed)
                    {
                        ((ODKResultsWindow)Owner).StartAtBeginningOfJSONDownloadList = (bool)chkStartAtBeginning.IsChecked;
                        DialogResult = true;
                    }
                    else
                    {
                        MessageBox.Show("Please select an option", "NSAP-ODK Databaase", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    break;
                case "buttonCancel":
                    DialogResult = false;
                    break;
            }
        }
    }
}
