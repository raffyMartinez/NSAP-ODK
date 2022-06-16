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

    public enum JSONFilesToUploadType
    {
        UploadTypeDownloadedJsonDownloadAll,
        UploadTypeDownloadedJsonNotDownloaded,
        UploadTypeJSONHistoryFiles,
        UploadTypeJSONHistoryUpdateXFormID,
        UploadTypeJSONHistoryUpdateLandingSites
    }
    public enum UpdateJSONHistoryMode
    {
        UpdateNoneSelected,
        UpdateReplaceExistingData,
        UpdateMissingData
    }
    /// <summary>
    /// Interaction logic for UploadJSONHistoryOptionsWindow.xaml
    /// </summary>
    public partial class UploadJSONHistoryOptionsWindow : Window
    {
        private bool _ignoreControls = false;
        public UploadJSONHistoryOptionsWindow()
        {
            InitializeComponent();
            Loaded += UploadJSONHistoryOptionsWindow_Loaded;
        }

        public JSONFilesToUploadType JSONFilesToUploadType { get; set; }

        private void UploadJSONHistoryOptionsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            panelControls.Visibility = Visibility.Visible;

            labelTitle.Content = "Select what to do with JSON files";
            switch (JSONFilesToUploadType)
            {

                case JSONFilesToUploadType.UploadTypeDownloadedJsonDownloadAll:
                    chkStartAtBeginning.Visibility = Visibility.Collapsed;
                    labelPrompt.Visibility = Visibility.Collapsed;
                    break;
                case JSONFilesToUploadType.UploadTypeDownloadedJsonNotDownloaded:
                    chkStartAtBeginning.Visibility = Visibility.Collapsed;
                    panelControls.Visibility = Visibility.Collapsed;
                    labelPrompt.Text = "Select OK to update missing sampled vessel landings in the database";
                    _ignoreControls = true;
                    break;
                case JSONFilesToUploadType.UploadTypeJSONHistoryFiles:
                    labelTitle.Content = "Select what to do with JSON history files";
                    chkStartAtBeginning.Visibility = Visibility.Visible;
                    labelPrompt.Visibility = Visibility.Collapsed;
                    break;

                case JSONFilesToUploadType.UploadTypeJSONHistoryUpdateLandingSites:
                case JSONFilesToUploadType.UploadTypeJSONHistoryUpdateXFormID:
                    chkStartAtBeginning.Visibility = Visibility.Collapsed;
                    panelControls.Visibility = Visibility.Collapsed;
                    labelPrompt.Text = "Select OK to update missing XFormIdentifiers in the saved sampled landings in the database";
                    if(JSONFilesToUploadType==JSONFilesToUploadType.UploadTypeJSONHistoryUpdateLandingSites)
                    {
                        labelPrompt.Text = "Select OK to update missing landing sites in the saved sampled landings in the database";
                    }
                    _ignoreControls = true;
                    break;
            }

            if(string.IsNullOrEmpty(Entities.NSAPEntities.KoboServerViewModel.GetLastUploadedJSON()))
            {
                chkStartAtBeginning.IsEnabled = false;
            }
        }

        public bool IsHistoryJSON { get; set; }
        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonOk":
                    bool proceed = true;
                    if ((bool)rbReplace.IsChecked)
                    {
                        ((ODKResultsWindow)Owner).UpdateJSONHistoryMode = UpdateJSONHistoryMode.UpdateReplaceExistingData;
                    }
                    else if ((bool)rbUpdateMissing.IsChecked)
                    {
                        ((ODKResultsWindow)Owner).UpdateJSONHistoryMode = UpdateJSONHistoryMode.UpdateMissingData;
                    }
                    else
                    {

                        proceed = _ignoreControls;
                    }

                    if (proceed)
                    {
                        ((ODKResultsWindow)Owner).StartAtBeginningOfJSONDownloadList = _ignoreControls && (bool)chkStartAtBeginning.IsChecked;
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
