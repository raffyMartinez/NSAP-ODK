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
using Ookii.Dialogs.Wpf;

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
        private bool _timerEnable = true;

        private List<DeleteRegionEntityFail> _deleteRegionEntityFails;

        public ProgressDialogWindow(string taskToDo)
        {
            InitializeComponent();
            SamplingCalendaryMismatchFixer.Cancel = false;
            TaskToDo = taskToDo;
            textBlockDescription.Text = "This process might take a while\r\nDo you wish to continue?";

            Closing += OnCopyTextDialogWindow_Closing;
            panelStatus.Visibility = Visibility.Collapsed;
            _timer = new DispatcherTimer();
            _timer.Tick += OnTimerTick;
            switch (TaskToDo)
            {
                case "import fishing vessel names from DB":
                    labelBigTitle.Content = "Import names of fishing vessels saved in database";
                    break;
                case "delete region vessels":
                    labelBigTitle.Content = "Delete fishing vessels of region";
                    break;
                case "analyze batch json files":
                    labelBigTitle.Content = "Analyze content of JSON batch files";
                    break;
                case "fix mismatch in calendar days":
                    labelBigTitle.Content = "Search and fix mismatch in sampling calendar";

                    break;
                case "import fishing vessels":
                    labelBigTitle.Content = "Importing fishing vessels";
                    break;
                case "analyze json history files":
                    labelBigTitle.Content = "Analyze content of JSON history files";
                    break;
                case "identify zero weight catch composition":
                    labelBigTitle.Content = "Identify catch composition weight of zero";
                    break;

            }
            Title = labelBigTitle.Content.ToString();
        }
        private void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            if (_timerEnable)
            {
                panelStatus.Visibility = Visibility.Collapsed;
                if (Owner != null)
                {
                    Owner.Focus();
                }
                Close();
            }
            else
            {
                panelButtons.Visibility = Visibility.Visible;
            }
        }
        public string TaskToDo { get; private set; }

        private void OnCopyTextDialogWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _instance = null;
        }
        public List<string> VesselNamesFromDB { get; private set; }
        public List<NSAPRegionFishingVessel> NSAPRegionFishingVessels { get; set; }
        public Koboserver Koboserver { get; set; }
        public List<System.IO.FileInfo> BatchJSONFiles { get; set; }
        public NSAPRegion Region { get; set; }
        public LandingSite LandingSite { get; set; }
        public string ListToImportFromTextBox { get; set; }
        public List<FileInfoJSONMetadata> FileInfoJSONMetadatas { get; set; }
        public FisheriesSector Sector { get; set; }
        private string GetJSONFolder(bool savedHistory = true)
        {
            string description = "Locate folder containing saved JSON history files";
            if (!savedHistory)
            {
                description = "Locate folder containing downloaded JSON files";
            }
            VistaFolderBrowserDialog vfbd = new VistaFolderBrowserDialog();
            vfbd.Description = description;
            vfbd.UseDescriptionForTitle = true;
            vfbd.ShowNewFolderButton = true;
            if (Utilities.Global.Settings.JSONFolder != null && Utilities.Global.Settings.JSONFolder.Length > 0)
            {
                vfbd.SelectedPath = Utilities.Global.Settings.JSONFolder;
            }
            else
            {
                vfbd.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
            }
            if ((bool)vfbd.ShowDialog() && vfbd.SelectedPath.Length > 0)
            {
                Utilities.Global.Settings.JSONFolder = vfbd.SelectedPath;
                return vfbd.SelectedPath;
            }
            return "";
        }
        public async Task DoTask()
        {
            string labelSuccessFindingMismatch = "Mismatched items were found";
            string labelNoSuccessFindingMismatch = "There were no mismatched items found";
            var ner = NSAPEntities.NSAPRegionViewModel.GetNSAPRegionWithEntitiesRepository(NSAPEntities.NSAPRegionViewModel.CurrentEntity);

            switch (TaskToDo)
            {
                case "import fishing vessel names from DB":
                    NSAPEntities.SummaryItemViewModel.ProcessingItemsEvent += OnProcessingItemsEvent;
                    textBlockDescription.Text = "Importing names of fishing vessels";
                    VesselNamesFromDB = await NSAPEntities.SummaryItemViewModel.GetVesselTextFromLandingSiteAsync(LandingSite);
                    NSAPEntities.SummaryItemViewModel.ProcessingItemsEvent -= OnProcessingItemsEvent;
                    break;
                case "identify zero weight catch composition":
                    textBlockDescription.Text = "Identifying catch composition with weight of zero";
                    VesselCatchRepository.ProcessingItemsEvent += OnProcessingItemsEvent;
                    var listZeroCatchWeight = await VesselCatchRepository.GetCatchesWithZeroWeightAsync();
                    if (listZeroCatchWeight != null && listZeroCatchWeight.Count > 0)
                    {
                        SpeciesWithZeroWeightListingWindow slw = SpeciesWithZeroWeightListingWindow.GetInstance();
                        if (slw.Visibility == Visibility.Visible)
                        {
                            slw.BringIntoView();
                        }
                        else
                        {
                            slw.Show();
                        }
                        slw.CatchesWithZeroWeight = listZeroCatchWeight;
                        slw.Owner = Owner;
                    }
                    VesselCatchRepository.ProcessingItemsEvent += OnProcessingItemsEvent;
                    break;
                case "delete vessel from vessel unload":
                    textBlockDescription.Text = "Removing vessel IDs from unload table";
                    ner.ResetDeleteEntityFails();
                    NSAPEntities.SummaryItemViewModel.ProcessingItemsEvent += OnProcessingItemsEvent;
                    await NSAPEntities.SummaryItemViewModel.DeleteVesselIDFromVesselUnloadsAsync(_deleteRegionEntityFails);
                    NSAPEntities.SummaryItemViewModel.ProcessingItemsEvent -= OnProcessingItemsEvent;
                    break;
                case "delete region vessels":
                    textBlockDescription.Text = "Deleting fishing vessels of region";

                    ner.ProcessingItemsEvent += OnProcessingItemsEvent;
                    await ner.DeleteFishingVesselsAsync(NSAPRegionFishingVessels);
                    ner.ProcessingItemsEvent -= OnProcessingItemsEvent;
                    break;
                case "analyze batch json files":
                    textBlockDescription.Text = "Analyzing JSON files";
                    NSAPEntities.JSONFileViewModel.ProcessingItemsEvent += OnProcessingItemsEvent;
                    //await NSAPEntities.JSONFileViewModel.CreateJSONFilesFromJSONFolder(GetJSONFolder());
                    await NSAPEntities.JSONFileViewModel.AnalyzeBatchJSONFilesAsync(BatchJSONFiles, Koboserver);
                    NSAPEntities.JSONFileViewModel.ProcessingItemsEvent -= OnProcessingItemsEvent;
                    break;

                case "analyze json history files":
                    textBlockDescription.Text = "Analyzing JSON files";
                    NSAPEntities.JSONFileViewModel.ProcessingItemsEvent += OnProcessingItemsEvent;
                    //await NSAPEntities.JSONFileViewModel.CreateJSONFilesFromJSONFolder(GetJSONFolder());
                    await NSAPEntities.JSONFileViewModel.AnalyzeJSONInListAsync(FileInfoJSONMetadatas);
                    NSAPEntities.JSONFileViewModel.ProcessingItemsEvent -= OnProcessingItemsEvent;
                    break;
                case "import fishing vessels":
                    textBlockDescription.Text = "Importing fishing vessels";

                    if (Region != null)
                    {
                        NSAPEntities.FishingVesselViewModel.ProcessingItemsEvent += OnProcessingItemsEvent;
                        if (await NSAPEntities.FishingVesselViewModel.ImportVesselsAsync(ListToImportFromTextBox, Region, Sector))
                        {
                            panelStatus.Visibility = Visibility.Collapsed;
                            ((MainWindow)Owner).RefreshEntityGrid();
                        }
                        NSAPEntities.FishingVesselViewModel.ProcessingItemsEvent -= OnProcessingItemsEvent;

                    }
                    else
                    {
                        NSAPEntities.LandingSiteViewModel.CurrentEntity.LandingSite_FishingVesselViewModel.ProcessingItemsEvent += OnProcessingItemsEvent;
                        if (await NSAPEntities.LandingSiteViewModel.CurrentEntity.LandingSite_FishingVesselViewModel.ImportVesselsAsync(ListToImportFromTextBox, Sector))
                        {
                            panelStatus.Visibility = Visibility.Collapsed;
                            //DialogResult = true;
                        }
                        NSAPEntities.LandingSiteViewModel.CurrentEntity.LandingSite_FishingVesselViewModel.ProcessingItemsEvent -= OnProcessingItemsEvent;
                    }

                    break;
                case "fix mismatch in calendar days":
                    SamplingCalendaryMismatchFixer.ProcessingItemsEvent += OnProcessingItemsEvent;
                    if (await SamplingCalendaryMismatchFixer.SearchMismatchAsync())
                    {
                        panelStatus.Visibility = Visibility.Collapsed;
                        //_proceedAndClose = true;


                        textBlockDescription.Text = labelSuccessFindingMismatch;
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

                            textBlockDescription.Text = "Operation was cancelled";
                        }
                        else
                        {
                            _processStarted = false;
                            textBlockDescription.Text = labelNoSuccessFindingMismatch;
                        }
                    }
                    SamplingCalendaryMismatchFixer.ProcessingItemsEvent -= OnProcessingItemsEvent;
                    break;
            }



        }

        private void LandingSite_FishingVesselViewModel_ProcessingItemsEvent(object sender, ProcessingItemsEventArg e)
        {
            throw new NotImplementedException();
        }

        private void OnProcessingItemsEvent(object sender, ProcessingItemsEventArg e)
        {
            _timerEnable = true;
            string processName = "";
            switch (e.Intent)
            {
                case "start remove entity id":
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
                          if (e.Intent != "start remove entity id")
                          {
                              progressBar.Maximum = e.TotalCountToProcess;
                          }

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
                case "finished getting list of vessel text":

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
                case "item deleted":
                case "entity id removed":
                case "item sorted":
                case "item fixed":
                case "imported_entity":
                case "JSON file analyzed":
                case "item found":
                case "Added name to list":

                    processName = "Found";
                    if (e.Intent == "item fixed")
                    {
                        processName = "Fixed";
                    }
                    else if(e.Intent=="added name to list")
                    {
                        processName = $"Added ({e.ProcessedItemName})";
                    }
                    else if (e.Intent == "entity id removed")
                    {
                        processName = "Removed item ID of";
                    }
                    else if (e.Intent == "imported_entity")
                    {
                        processName = "Imported";
                    }
                    else if (e.Intent == "JSON file analyzed")
                    {
                        processName = "Analyzed";
                    }
                    else if (e.Intent == "item deleted")
                    {
                        processName = "Deleted";
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
                case "done deleting":
                case "done searching":
                case "done sorting":
                case "done fixing":
                case "import_done":
                case "done analyzing JSON file":
                case "done removing entity id":
                case "cancel":
                case "finished adding names to list":
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
                                  case "finished adding names to list":
                                      processName = "adding";
                                      DialogResult = true;
                                      break;
                                  case "done searching":
                                      processName = "searching";
                                      break;
                                  case "done removing entity id":
                                      processName = "removing removing entity IDs of";
                                      break;
                                  case "done deleting":
                                      processName = "deleting";
                                      ((EditWindowEx)Owner).RefreshSubForm(TaskToDo);
                                      break;
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



                    //labelDescription.Dispatcher.BeginInvoke
                    _timer.Interval = TimeSpan.FromSeconds(3);
                    textBlockDescription.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              switch (TaskToDo)
                              {
                                  case "identify zero weight catch composition":
                                      textBlockDescription.Text = $"Finishsed searching for species with weight of zero.";
                                      break;
                                  case "delete vessel from vessel unload":
                                      textBlockDescription.Text = $"Finishsed removing entity IDs of {progressBar.Maximum} vessels";
                                      break;
                                  case "analyze json history files":
                                      //textBlockDescription.Text = $"Finishsed analyzing {progressBar.Maximum} JSON files";
                                      textBlockDescription.Text = $"Finishsed analyzing {progressBar.Maximum} JSON files";
                                      break;
                                  case "import fishing vessels":
                                      //textBlockDescription.Text = $"Finishsed importing {progressBar.Maximum} vessels";
                                      textBlockDescription.Text = $"Finishsed importing {progressBar.Maximum} vessels";
                                      break;
                                  case "fix mismatch in calendar days":
                                      //textBlockDescription.Text = $"Found {progressBar.Maximum} gear unloads with mismatch";
                                      textBlockDescription.Text = $"Found {progressBar.Maximum} gear unloads with mismatch";
                                      break;
                                  case "delete region vessels":

                                      _deleteRegionEntityFails = NSAPEntities.NSAPRegionViewModel.GetNSAPRegionWithEntitiesRepository(NSAPEntities.NSAPRegionViewModel.CurrentEntity).DeleteRegionEntityFails;
                                      if (_deleteRegionEntityFails.Count > 0)
                                      {

                                          string msg = $"There are {_deleteRegionEntityFails.Count} vessels that were not deleted because they are used in the vessel unload table\n\r" +
                                          "Do you want to delete the fishing vessel IDs from that table?";


                                          _timerEnable = false;
                                          _timer.Interval = TimeSpan.FromMilliseconds(1);
                                          textBlockDescription.Text = msg;


                                      }
                                      else
                                      {
                                          textBlockDescription.Text = $"Finishsed deleting {progressBar.Maximum} vessels in the region";
                                      }
                                      break;
                              }

                              if (e.Intent == "cancel")
                              {
                                  textBlockDescription.Text = $"Operation was cancelled";
                              }

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);


                    //_timer.Interval = TimeSpan.FromSeconds(3);
                    //if (e.Intent == "cancel")
                    //{
                    //    _timer.Interval = TimeSpan.FromSeconds(3);
                    //}
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
                    if (_deleteRegionEntityFails != null && _deleteRegionEntityFails.Count > 0)
                    {
                        switch (TaskToDo)
                        {
                            case "delete region vessels":
                                TaskToDo = "delete vessel from vessel unload";
                                break;
                        }
                    }
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
                            case "delete region vessels":
                                Close();
                                break;
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
