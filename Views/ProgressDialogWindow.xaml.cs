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

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for ProgressDialogWindow.xaml
    /// </summary>
    public partial class ProgressDialogWindow : Window
    {
        private bool _proceedAndClose = false;
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
            }
        }
        private void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            panelStatus.Visibility = Visibility.Collapsed;
            //if (_proceedAndClose)
            //{

            Close();
            //}

        }
        public string TaskToDo { get; private set; }

        private void OnCopyTextDialogWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _instance = null;
        }


        public async Task DoTask()
        {
            string labelSuccessFindingMismatch = "Mismatched items were found";
            string labelNoSuccessFindingMismatch = "There were no mismatched items found";

            switch (TaskToDo)
            {
                case "fix mismatch in calendar days":
                    SamplingCalendaryMismatchFixer.ProcessingItemsEvent += SamplingCalendaryMismatchFixer_FixCalendarItemCountMismatchEvent;



                    if (await SamplingCalendaryMismatchFixer.SearchMismatchAsync())
                    {
                        panelStatus.Visibility = Visibility.Collapsed;
                        _proceedAndClose = true;

                        switch (TaskToDo)
                        {
                            case "fix mismatch in calendar days":
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
                            switch (TaskToDo)
                            {
                                case "fix mismatch in calendar days":
                                    labelDescription.Content = labelNoSuccessFindingMismatch;
                                    break;
                            }

                        }
                    }
                    SamplingCalendaryMismatchFixer.ProcessingItemsEvent -= SamplingCalendaryMismatchFixer_FixCalendarItemCountMismatchEvent;
                    break;
            }
        }

        private void SamplingCalendaryMismatchFixer_FixCalendarItemCountMismatchEvent(object sender, ProcessingItemsEventArg e)
        {
            string processName = "";
            switch (e.Intent)
            {
                case "start sorting":
                case "start fixing":
                    progressBar.Dispatcher.BeginInvoke
    (
                      DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                      {


                          progressBar.IsIndeterminate = true;
                          if (e.Intent == "start fixing")
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
                    processName = "Found";
                    if (e.Intent == "item fixed")
                    {
                        processName = "Fixed";
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
                              if (e.Intent == "done fixing")
                              {
                                  processName = "fixing";
                              }
                              progressLabel.Content = $"Finished {processName} {e.CountProcessed} items";
                              if (e.Intent == "cancel")
                              {
                                  progressLabel.Content = "Operation was cancelled";
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
                    if(!_processStarted)
                    {
                        Close();
                    }
                    else if (SamplingCalendaryMismatchFixer.Cancel)
                    {
                        Close();
                    }
                    else
                    {
                        //Cancel = true;
                        SamplingCalendaryMismatchFixer.Cancel = true;
                        panelButtons.Visibility = Visibility.Collapsed;
                        panelStatus.Visibility = Visibility.Collapsed;
                        //Close();
                    }
                    break;
            }
        }


    }
}
