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
using System.Windows.Threading;
using NSAP_ODK.Entities.Database;
using NSAP_ODK.Entities.Database.FromJson;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for UpdateWeightValidationTableWindow.xaml
    /// </summary>
    public partial class UpdateWeightValidationTableWindow : Window
    {
        private DispatcherTimer _timer;
        private bool _cancel = false;
        bool updateSucceeded = false;
        private static UpdateWeightValidationTableWindow _instance;

        public static UpdateWeightValidationTableWindow GetInstance()
        {
            if (_instance == null)
            {
                _instance = new UpdateWeightValidationTableWindow();
            }
            return _instance;
        }
        public UpdateWeightValidationTableWindow()
        {
            InitializeComponent();
            Loaded += UpdateWeightValidationTableWindow_Loaded;
            Closed += UpdateWeightValidationTableWindow_Closed;
        }

        private void UpdateWeightValidationTableWindow_Closed(object sender, EventArgs e)
        {
            _instance = null;
        }

        private void UpdateWeightValidationTableWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ShowStatusRow(isVisible: false);
            mainStatusLabel.Content = "";
            _timer = new DispatcherTimer();
            _timer.Tick += OnTimerTick;

        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            ShowStatusRow(false);

        }
        private async Task StartUpdate()
        {
            buttonYes.IsEnabled = false;
            buttonNo.IsEnabled = false;
            WeightValidationUpdater.UploadSubmissionToDB += WeightValidationUpdater_UploadSubmissionToDB;
            if(await WeightValidationUpdater.UpdateDatabaseAsync())
            {
                labelTitle.Content = "Updating weight validation table succeeded!";
                updateSucceeded = true;
                buttonCancel.Content = "Close";
            }
            WeightValidationUpdater.UploadSubmissionToDB -= WeightValidationUpdater_UploadSubmissionToDB;
        }
        private void ShowStatusRow(bool isVisible = true, bool resetIndicators = true)
        {
            if (!isVisible)
            {
                rowStatus.Height = new GridLength(0);
            }
            else
            {
                rowStatus.Height = new GridLength(40, GridUnitType.Pixel);
            }
            if (resetIndicators)
            {
                mainStatusBar.Value = 0;
                mainStatusLabel.Content = string.Empty;
            }

        }
        private void WeightValidationUpdater_UploadSubmissionToDB(object sender, UploadToDbEventArg e)
        {
            switch (e.Intent)
            {
                case UploadToDBIntent.StartOfUpdate:
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              mainStatusBar.IsIndeterminate = false;
                              mainStatusBar.Maximum = e.VesselUnloadToUpdateCount;
                              mainStatusBar.Value = 0;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = "Updating weight validation summary. Please wait...";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    break;
                case UploadToDBIntent.SummaryItemProcessed:
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              mainStatusBar.Value = e.SummaryItemProcessedCount;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = $"Finished updating record {e.SummaryItemProcessedCount} of {mainStatusBar.Maximum}";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case UploadToDBIntent.WeightValidationUpdated:
                    mainStatusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              mainStatusLabel.Content = $"Updated validation record #{e.VesselUnloadWeightValidationUpdateCount}";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case UploadToDBIntent.EndOfUpdate:
                    mainStatusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              mainStatusBar.Value = 0;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    _timer.Interval = TimeSpan.FromSeconds(3);
                    _timer.Start();
                    break;
            }
        }

        private async void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch(((Button)sender).Name)
            {
                case "buttonYes":
                    ShowStatusRow();
                    await StartUpdate();
                    break;
                case "buttonNo":
                    Close();
                    break;
                case "buttonCancel":
                    _cancel = true;
                    if(!updateSucceeded)
                    {
                        WeightValidationUpdater.Cancel = true;
                    }
                    Close();
                    break;
            }
        }
    }
}
