using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using NSAP_ODK.Utilities;
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
        private bool _isMultiVesselDelete;
        private ODKResultsWindow _resultsWindow;
        private string closing_message;
        private ServerLogInInformation _serverLogInInformation;
        private List<DeleteRegionEntityFail> _deleteRegionEntityFails;

        public bool ServerIsMultiVessel { get; set; }
        public string ServerID { get; set; }
        public string ServerUserName { get; set; }
        public string ServerPassword { get; set; }
        public string KoboFormID { get; set; }

        public string KoboFormNumericID { get; set; }
        public HttpClient HttpClient { get; set; }

        public void SavedUnmatchedJson(int savedCount)
        {
            progressBar.Value = savedCount;
            progressLabel.Content = $"Saved {savedCount} submission of {progressBar.Maximum}";
        }


        private async Task<string> JSONStringFromAPICall(string api_call)
        {
            StringBuilder the_response = new StringBuilder();
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), api_call))
            {
                var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{ServerUserName}:{ServerPassword}"));
                request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");
                try
                {
                    var response = await HttpClient.SendAsync(request);
                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    Encoding encoding = Encoding.GetEncoding("utf-8");
                    the_response = new StringBuilder(encoding.GetString(bytes, 0, bytes.Length));
                }
                catch (Exception ex)
                {

                }
            }
            return the_response.ToString();
        }

        public List<LandingSiteSamplingSummarized> SamplingDaysForDeletion { get; set; }


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
                case "delete sampling days":
                    labelBigTitle.Content = "Delete landing site sampling";
                    _serverLogInInformation = ServerLoginWindow.GetLogInInformation();
                    if (_serverLogInInformation.CanLogIn)
                    {
                        labelBigTitle.Content += " (server data included)";
                    }
                    break;
                case "upload unmatched landings JSON":
                    labelBigTitle.Content = "Update missing submissions using JSON";
                    break;
                case "update submission pairing":
                    labelBigTitle.Content = "Update submission identifiers in database";
                    break;
                case "delete landing data from selected server":
                    labelBigTitle.Content = "Delete fish landing data downloaded from seleceted server";
                    break;
                case "delete all landing data":
                    labelBigTitle.Content = "Delete all fish landing";
                    break;
                case "delete single vessel data":
                    _isMultiVesselDelete = false;
                    labelBigTitle.Content = "Delete fish landing data encoded using non multi-vesselgear eForm";
                    break;
                case "delete multivessel gear data":
                    _isMultiVesselDelete = true;
                    labelBigTitle.Content = "Delete fish landing data encoded using multi-vesselgear eForm";
                    break;
                case "get enumerators first sampling":
                    labelBigTitle.Content = "Get first sampling of enumeartors";
                    break;
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

        public Koboserver Koboserver { get; set; }
        public List<string> VesselNamesFromDB { get; private set; }
        public List<NSAPRegionEnumerator> FirstSamplingsOfEnumerators { get; private set; }
        public List<NSAPRegionFishingVessel> NSAPRegionFishingVessels { get; set; }
        public FormSummary FormSummary { get; set; }
        public List<System.IO.FileInfo> BatchJSONFiles { get; set; }

        public string JSON { get; set; }
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

        /// <summary>
        /// Uploads landing data saved in JSON files. 
        /// These landings are the ones that are not saved in the local database and cannot be downloaded using the option "Get all not downloaded"
        /// </summary>
        /// <returns></returns>
        private async Task Upload_unmatched_landings_JSON()
        {
            //var resultsWindow = (ODKResultsWindow)((DownloadFromServerWindow)Owner).Owner;
            _resultsWindow.FormID = KoboFormNumericID;
            //resultsWindow.BatchUpload = true;
            foreach (var json in SubmissionIdentifierPairing.UnmatchedLandingsJSON)
            {
                bool isMultiVessel = json.Contains("repeat_landings") || json.Contains("landing_site_sampling_group/are_there_landings");
                bool isOptimizedMultiVessel = json.Contains("G_lss/sampling_date");

                _resultsWindow.JSONFromServer(json, isMultiVessel, isOptimizedMultiVessel, updateMissingSubmission: true);

                if (isMultiVessel)
                {
                    MultiVesselGear_UnloadServerRepository.JSON = json;
                    MultiVesselGear_UnloadServerRepository.CreateLandingsFromSingleJson();
                    _resultsWindow.MultiVesselMainSheets = MultiVesselGear_UnloadServerRepository.SampledVesselLandings;
                }
                else if (isOptimizedMultiVessel)
                {
                    MultiVessel_Optimized_UnloadServerRepository.JSON = json;
                    MultiVessel_Optimized_UnloadServerRepository.CreateLandingsFromSingleJson();
                    _resultsWindow.MultiVesselOptimizedMainSheets = MultiVessel_Optimized_UnloadServerRepository.SampledVesselLandings;
                }
                else
                {
                    VesselUnloadServerRepository.JSON = json;
                    VesselUnloadServerRepository.CreateLandingsFromJSON();
                    _resultsWindow.MainSheets = VesselUnloadServerRepository.VesselLandings;
                }
                await _resultsWindow.UploadToDatabase();
            }

        }
        public async Task DoTask()
        {
            string labelSuccessFindingMismatch = "Mismatched items were found";
            string labelNoSuccessFindingMismatch = "There were no mismatched items found";
            var ner = NSAPEntities.NSAPRegionViewModel.GetNSAPRegionWithEntitiesRepository(NSAPEntities.NSAPRegionViewModel.CurrentEntity);

            switch (TaskToDo)
            {
                case "delete sampling days":

                    break;
                case "upload unmatched landings JSON":
                    //await  _resultsWindow.Upload_unmatched_landings_JSON(KoboFormNumericID);
                    break;
                case "update submission pairing":
                    _resultsWindow = (ODKResultsWindow)((DownloadFromServerWindow)Owner).Owner;
                    bool success = false;
                    SubmissionIdentifierPairing.UploadSubmissionToDB += SubmissionIdentifierPairing_UploadSubmissionToDB;
                    SubmissionIdentifierPairing.KoboForm = FormSummary.KoboForm;
                    SubmissionIdentifierPairing.ServerPassword = ServerPassword;
                    SubmissionIdentifierPairing.ServerUserName = ServerUserName;
                    SubmissionIdentifierPairing.HttpClient = HttpClient;

                    textBlockDescription.Text = "Updating missing submission IDs";


                    if (await SubmissionIdentifierPairing.UpDateDatabaseTaskAsync())
                    {
                        textBlockDescription.Text = "Downloading unmatched JSON";


                        if (SubmissionIdentifierPairing.UnmatchedLandingsJSON.Count > 0)
                        {
                            //Visibility = Visibility.Collapsed;
                            progressBar.Value = 0;
                            progressBar.Maximum = SubmissionIdentifierPairing.UnmatchedLandingsJSON.Count;
                            textBlockDescription.Text = $"Saving {SubmissionIdentifierPairing.UnmatchedLandingsJSON.Count} submissions to database";
                            //_resultsWindow.Focus();
                            await _resultsWindow.Upload_unmatched_landings_JSON(KoboFormNumericID, this);
                        }
                        else if (SubmissionIdentifierPairing.UnmatchedPairs.Count > 0)
                        {
                            closing_message = "Server refused to download JSON data\r\n\r\nTry another time";
                        }
                        else
                        {
                            closing_message = "Data is updated\r\n\r\nJSON was not downloaded";
                        }

                        success = true;
                    }
                    if (!string.IsNullOrEmpty(closing_message))
                    {
                        Visibility = Visibility.Visible;
                        panelButtons.Visibility = Visibility.Collapsed;
                        textBlockDescription.Text = closing_message;
                        buttonCancel.Content = "Close";

                    }
                    else
                    {
                        try
                        {
                            DialogResult = success;
                        }
                        catch (Exception ex)
                        {
                            //Logger.Log(ex);
                        }
                        _resultsWindow.Close();

                    }
                    SubmissionIdentifierPairing.UploadSubmissionToDB -= SubmissionIdentifierPairing_UploadSubmissionToDB;
                    break;
                case "delete landing data from selected server":
                    DeleteServerData.DeletingServerDataEvent += DeleteServerData_DeletingServerDataEvent;
                    progressLabel.Visibility = Visibility.Collapsed;
                    textBlockDescription.Text = "Deleting fish landing data";
                    DeleteServerData.ServerID = ServerID;
                    DeleteServerData.IsMultiVessel = ServerIsMultiVessel;
                    DialogResult = await DeleteServerData.DeleteServerDataByServerIDAsync();
                    DeleteServerData.DeletingServerDataEvent -= DeleteServerData_DeletingServerDataEvent;
                    break;
                case "delete all landing data":
                    DeleteServerData.DeletingServerDataEvent += DeleteServerData_DeletingServerDataEvent;
                    progressLabel.Visibility = Visibility.Collapsed;
                    textBlockDescription.Text = "Deleting fish landing data";
                    if (Utilities.Global.Settings.UsemySQL)
                    {
                        DialogResult = NSAPMysql.MySQLConnect.DeleteDataFromTables(useScript: true);
                    }
                    else
                    {
                        DialogResult = await DeleteServerData.ClearNSAPDatabaseTablesAsync();
                    }
                    //else if (NSAPEntities.ClearNSAPDatabaseTables())
                    //{
                    //    NSAPEntities.LandingSiteSamplingViewModel.Clear();
                    //    NSAPEntities.SummaryItemViewModel.Clear();
                    //    DialogResult = true;
                    //}
                    DeleteServerData.DeletingServerDataEvent -= DeleteServerData_DeletingServerDataEvent;
                    break;
                case "delete single vessel data":
                case "delete multivessel gear data":
                    DeleteServerData.DeletingServerDataEvent += DeleteServerData_DeletingServerDataEvent;
                    progressLabel.Visibility = Visibility.Collapsed;
                    textBlockDescription.Text = "Deleting fish landing data";
                    DialogResult = await DeleteServerData.DeleteServerDataByTypeAsync(_isMultiVesselDelete);
                    DeleteServerData.DeletingServerDataEvent -= DeleteServerData_DeletingServerDataEvent;
                    break;
                case "get enumerators first sampling":
                    NSAPEntities.SummaryItemViewModel.ProcessingItemsEvent += OnProcessingItemsEvent;
                    textBlockDescription.Text = "Getting first sampling of enumerators";
                    FirstSamplingsOfEnumerators = await NSAPEntities.SummaryItemViewModel.GetFirstSamplingOfEnumeratorsASync();
                    NSAPEntities.SummaryItemViewModel.ProcessingItemsEvent -= OnProcessingItemsEvent;
                    DialogResult = true;
                    break;
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



        private void SubmissionIdentifierPairing_UploadSubmissionToDB(object sender, UploadToDbEventArg e)
        {
            switch (e.Intent)
            {
                case UploadToDBIntent.UpdateUnmatchedJSON:
                    progressBar.Dispatcher.BeginInvoke
                    (
                      DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                      {
                          progressBar.Maximum = e.ItemsForUpdatingCount;
                          progressBar.IsIndeterminate = false;
                          progressBar.Value = 0;
                          //do what you need to do on UI Thread
                          return null;
                      }), null);
                    break;

                case UploadToDBIntent.UpdateUnmatchedJSONDownloadingFromServer:
                    progressLabel.Dispatcher.BeginInvoke
                    (
                      DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                      {
                          progressLabel.Content = $"Downloading JSON from server...";
                          //do what you need to do on UI Thread
                          return null;
                      }
                     ), null);
                    break;

                case UploadToDBIntent.UpdatedUnmatchedJSON:
                    progressBar.Dispatcher.BeginInvoke
                    (
                      DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                      {
                          progressBar.Value = e.ItemsUpdatedCount;
                          return null;
                      }), null);

                    progressLabel.Dispatcher.BeginInvoke
                    (
                      DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                      {
                          progressLabel.Content = $"Added unmatched JSON {e.ItemsUpdatedCount} of {progressBar.Maximum}";
                          //do what you need to do on UI Thread
                          return null;
                      }
                     ), null);

                    break;
                case UploadToDBIntent.UpdatedUnmatchedJSONDone:
                    progressBar.Dispatcher.BeginInvoke
                    (
                      DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                      {
                          progressBar.Value = 0;
                          return null;
                      }), null);

                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressLabel.Content = $"Finished adding {e.ItemsUpdatedCount} unmatched JSON";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    break;


                case UploadToDBIntent.UpdateTableFields:
                    progressBar.Dispatcher.BeginInvoke
                    (
                      DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                      {
                          progressBar.Maximum = e.ItemsForUpdatingCount;
                          progressBar.IsIndeterminate = false;
                          progressBar.Value = 0;
                          //do what you need to do on UI Thread
                          return null;
                      }), null);

                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressLabel.Content = $"Updating database table. Please wait...";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;


                case UploadToDBIntent.UpdatedTableField:
                    progressBar.Dispatcher.BeginInvoke
                    (
                      DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                      {
                          progressBar.Value = e.ItemsUpdatedCount;
                          return null;
                      }), null);

                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressLabel.Content = $"Updated database item {e.ItemsUpdatedCount} of {progressBar.Maximum}";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;

                case UploadToDBIntent.UpdatedTableDone:
                    progressBar.Dispatcher.BeginInvoke
                    (
                      DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                      {
                          progressBar.Value = 0;
                          return null;
                      }), null);

                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressLabel.Content = $"Finished updating {e.ItemsUpdatedCount} items in database";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
            }
        }

        private void DeleteServerData_DeletingServerDataEvent(object sender, DeleteFromServerEventArg e)
        {
            switch (e.Intent)
            {
                case "start deleting":
                    progressBar.Dispatcher.BeginInvoke
                    (
                      DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                      {


                          progressBar.Maximum = e.CountToProcess;
                          progressBar.Value = 0;
                          //do what you need to do on UI Thread
                          return null;
                      }), null);


                    break;
                case "deleted from table":
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
                              progressLabel.Content = $"Deleted data in {e.TableName}: {e.CountProcessed} of {progressBar.Maximum}";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "finished deleting":
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
                              progressLabel.Content = $"Finished all tables";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
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
                          if (e.Intent == "start fixing" || e.Intent == "start analyzing JSON files" || e.Intent == "start get first sampling of enumerators")
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
                case "enumerator first sampling found":

                    processName = "Found";
                    if (e.Intent == "item fixed")
                    {
                        processName = "Fixed";
                    }
                    else if (e.Intent == "enumerator first sampling found")
                    {
                        processName = "First sampling of enumerator";
                    }
                    else if (e.Intent == "added name to list")
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
                              if (e.DoNotShowRunningTotal)
                              {
                                  progressLabel.Content = $"{processName} item {e.CountProcessed} processed";
                              }
                              else
                              {
                                  progressLabel.Content = $"{processName} item {e.CountProcessed} of {_countToProcess}";
                              }
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    break;
                case "done deleting":
                case "done deleting multivessel data":
                case "done searching":
                case "done sorting":
                case "done fixing":
                case "import_done":
                case "done analyzing JSON file":
                case "done removing entity id":
                case "cancel":
                case "finished adding names to list":
                case "getting enumerators first sampling done":

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
                                  case "getting enumerators first sampling done":
                                      processName = "getting first sampling of enumerators";
                                      break;
                                  case "done deleting multivessel data":
                                      processName = "deleting";
                                      DialogResult = true;
                                      break;
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
                case "Close":
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
                                Close();
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
                            case "update submission pairing":
                                Close();
                                break;
                            default:
                                Close();
                                break;

                        }
                    }
                    break;
            }
        }


    }
}
