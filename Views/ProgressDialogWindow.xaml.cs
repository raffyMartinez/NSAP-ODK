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
using NSAP_ODK.Entities;
using NSAP_ODK.Entities.Database;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for ProgressDialogWindow.xaml
    /// </summary>
    public partial class ProgressDialogWindow : Window
    {
        //private bool _proceedAndClose = false;
        private DispatcherTimer _timer;
        private int _countToProcess;
        private static ProgressDialogWindow _instance;
        private bool _processStarted = false;

        public ProgressDialogWindow(string taskToDo)
        {
            InitializeComponent();
            SamplingCalendaryMismatchFixer.Cancel = false;
            TaskToDo = taskToDo;
            labelDescription.Content = "This process might take a while\r\nDo you wish to continue?";
            Closing += OnCopyTextDialogWindow_Closing;
            panelStatus.Visibility = Visibility.Collapsed;
            _timer = new DispatcherTimer();
            _timer.Tick += OnTimerTick;
            switch (TaskToDo)
            {
                case "fix mismatch in calendar days":
                    labelBigTitle.Content = "Search and fix mismatch in sampling calendar";

                    break;
                case "import fishing vessels":
                    labelBigTitle.Content = "Importing fishing vessels";
                    break;
                case "analyze json history files":
                    labelBigTitle.Content = "Analyze content of JSON history files";
                    break;

            }
            Title = labelBigTitle.Content.ToString();
        }
        private void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            panelStatus.Visibility = Visibility.Collapsed;
            Owner.Focus();
            Close();
        }
        public string TaskToDo { get; private set; }

        private void OnCopyTextDialogWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _instance = null;
        }

        public NSAPRegion Region { get; set; }
        public string ListToImportFromTextBox { get; set; }
        public List<FileInfoJSONMetadata> FileInfoJSONMetadatas { get; set; }
        public FisheriesSector Sector { get; set; }

        public async Task DoTask()
        {
            string labelSuccessFindingMismatch = "Mismatched items were found";
            string labelNoSuccessFindingMismatch = "There were no mismatched items found";

            switch (TaskToDo)
            {
                case "analyze json history files":
                    NSAPEntities.JSONFileViewModel.ProcessingItemsEvent += OnProcessingItemsEvent;
                    await NSAPEntities.JSONFileViewModel.AnalyzeJSONFiles(FileInfoJSONMetadatas);
                    NSAPEntities.JSONFileViewModel.ProcessingItemsEvent -= OnProcessingItemsEvent;
                    break;
                case "import fishing vessels":
                    labelDescription.Content = "Importing fishing vessels";
                    NSAPEntities.FishingVesselViewModel.ProcessingItemsEvent += OnProcessingItemsEvent;
                    if (await NSAPEntities.FishingVesselViewModel.ImportVesselsAsync(ListToImportFromTextBox, Region, Sector))
                    {
                        panelStatus.Visibility = Visibility.Collapsed;
                        ((MainWindow)Owner).RefreshEntityGrid();
                        //_proceedAndClose = true;
                    }
                    NSAPEntities.FishingVesselViewModel.ProcessingItemsEvent -= OnProcessingItemsEvent;
                    break;
                case "fix mismatch in calendar days":
                    SamplingCalendaryMismatchFixer.ProcessingItemsEvent += OnProcessingItemsEvent;
                    if (await SamplingCalendaryMismatchFixer.SearchMismatchAsync())
                    {
                        panelStatus.Visibility = Visibility.Collapsed;
                        //_proceedAndClose = true;


                        labelDescription.Content = labelSuccessFindingMismatch;
                        FixCalendarVesselUnloadWindow fcmw = FixCalendarVesselUnloadWindow.GetInstance();
                        fcmw.Owner = this.Owner;
                        if (fcmw.Visibility == Visibility.Visible)
                        {
                            fcmw.BringIntoView();
                        }
                        else
                        {
                            fcmw.Show();
                        }
                        fcmw.ShowSearchResults();
                        break;
                    }
                    else
                    {
                        if (SamplingCalendaryMismatchFixer.Cancel)
                        {

                            labelDescription.Content = "Operation was cancelled";
                        }
                        else
                        {
                            _processStarted = false;
                            labelDescription.Content = labelNoSuccessFindingMismatch;
                        }
                    }
                    SamplingCalendaryMismatchFixer.ProcessingItemsEvent -= OnProcessingItemsEvent;
                    break;
            }



        }



        private void OnProcessingItemsEvent(object sender, ProcessingItemsEventArg e)
        {
            string processName = "";
            switch (e.Intent)
            {
                case "start sorting":
                case "start fixing":
                case "start analyzing JSON files":
                    progressBar.Dispatcher.BeginInvoke
                    (
                      DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                      {


                          progressBar.IsIndeterminate = true;
                          if (e.Intent == "start fixing" || e.Intent == "start analyzing JSON files")
                          {
                              progressBar.IsIndeterminate = false;
                              _countToProcess = e.TotalCountToProcess;
                          }
                          progressBar.Value = 0;
                          progressBar.Maximum = e.TotalCountToProcess;
                          //do what you need to do on UI Thread
                          return null;
                      }), null);

                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressLabel.Content = "Sorting data. Please wait...";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    break;
                case "processing start":
                case "start":
                    progressBar.Dispatcher.BeginInvoke
                    (
                      DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                      {

                          progressBar.IsIndeterminate = false;
                          _countToProcess = e.TotalCountToProcess;
                          progressBar.Maximum = _countToProcess;
                          progressBar.Value = 0;
                          //do what you need to do on UI Thread
                          return null;
                      }), null);
                    break;
                case "item sorted":
                case "item fixed":
                case "imported_entity":
                case "JSON file analyzed":
                    processName = "Found";
                    if (e.Intent == "item fixed")
                    {
                        processName = "Fixed";
                    }
                    else if (e.Intent == "imported_entity")
                    {
                        processName = "Imported";
                    }
                    else if (e.Intent == "JSON file analyzed")
                    {
                        processName = "Analyzed";
                    }
                    progressBar.Dispatcher.BeginInvoke
                    (
                      DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                      {

                          progressBar.Value = e.CountProcessed;
                          //do what you need to do on UI Thread
                          return null;
                      }), null);

                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressLabel.Content = $"{processName} item {e.CountProcessed} of {_countToProcess}";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    break;
                case "done sorting":
                case "done fixing":
                case "import_done":
                case "done analyzing JSON file":
                case "cancel":
                    progressBar.Dispatcher.BeginInvoke
                    (
                      DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                      {

                          progressBar.Value = 0;
                          //do what you need to do on UI Thread
                          return null;
                      }), null);

                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              processName = "sorting";
                              switch (e.Intent)
                              {
                                  case "done fixing":
                                      processName = "fixing";
                                      break;
                                  case "import_done":
                                      processName = "importing";
                                      break;
                                  case "done analyzing JSON file":
                                      processName = "analyzing";
                                      break;
                              }

                              progressLabel.Content = $"Finished {processName} {progressBar.Maximum} items";

                              if (e.Intent == "cancel")
                              {
                                  progressLabel.Content = "Operation was cancelled";
                              }

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);



                    labelDescription.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              switch (TaskToDo)
                              {
                                  case "analyze json history files":
                                      labelDescription.Content = $"Finishsed analyzing {progressBar.Maximum} JSON files";
                                      break;
                                  case "import fishing vessels":
                                      labelDescription.Content = $"Finishsed importing {progressBar.Maximum} vessels";
                                      break;
                                  case "fix mismatch in calendar days":
                                      labelDescription.Content = $"Found {progressBar.Maximum} gear unloads with mismatch";
                                      break;
                              }

                              if (e.Intent == "cancel")
                              {
                                  labelDescription.Content = $"Operation was cancelled";
                              }

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    _timer.Interval = TimeSpan.FromSeconds(3);
                    if (e.Intent == "cancel")
                    {
                        _timer.Interval = TimeSpan.FromSeconds(3);
                    }
                    _timer.Start();
                    break;
            }
        }

        public static ProgressDialogWindow GetInstance(string taskToDo)
        {
            if (_instance == null)
            {
                _instance = new ProgressDialogWindow(taskToDo);
            }
            return _instance;
        }

        private async void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Content)
            {
                case "Yes":
                    panelButtons.Visibility = Visibility.Collapsed;
                    panelStatus.Visibility = Visibility.Visible;
                    _processStarted = true;
                    await DoTask();
                    //panelButtons.Visibility = Visibility.Collapsed;
                    panelStatus.Visibility = Visibility.Collapsed;
                    break;
                case "No":
                    Close();
                    break;
                case "Cancel":
                    if (!_processStarted)
                    {
                        Close();
                    }
                    else
                    {
                        switch (TaskToDo)
                        {
                            case "analyze json history files":
                                break;
                            case "fix mismatch in calendar days":
                                if (SamplingCalendaryMismatchFixer.Cancel)
                                {
                                    Close();
                                }
                                else
                                {
                                    SamplingCalendaryMismatchFixer.Cancel = true;
                                    panelButtons.Visibility = Visibility.Collapsed;
                                    panelStatus.Visibility = Visibility.Collapsed;
                                }
                                break;
                            case "import fishing vessels":
                                if (NSAPEntities.FishingVesselViewModel.Cancel)
                                {
                                    Close();
                                }
                                else
                                {
                                    NSAPEntities.FishingVesselViewModel.Cancel = true;
                                    panelButtons.Visibility = Visibility.Collapsed;
                                    panelStatus.Visibility = Visibility.Collapsed;
                                }
                                break;

                        }
                    }
                    break;
                    //else if (SamplingCalendaryMismatchFixer.Cancel)
                    //{
                    //    Close();
                    //}
                    //else
                    //{
                    //    //Cancel = true;
                    //    SamplingCalendaryMismatchFixer.Cancel = true;
                    //    panelButtons.Visibility = Visibility.Collapsed;
                    //    panelStatus.Visibility = Visibility.Collapsed;
                    //    //Close();
                    //}
                    //break;
            }
        }


    }
}
