using Microsoft.Win32;
using NSAP_ODK.Entities;
using NSAP_ODK.Entities.Database;
using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Ookii.Dialogs.Wpf;
using System.Linq;
using System.Xml.Serialization;
using System.Threading;
using System.Text;
//using System.Runtime.InteropServices;
//using System.Runtime.InteropServices.WindowsRuntime;
//using NPOI.OpenXmlFormats.Dml;
//using System.Windows.Forms;

namespace NSAP_ODK.Views
{
    public enum ODKServerDownload
    {
        ServerDownloadVesselUnload,
        ServerDownloadLandings
    }

    /// <summary>
    /// Interaction logic for ODKResultsWindow.xaml
    /// </summary>
    public partial class ODKResultsWindow : Window
    {
        private bool _lowMemoryReadingJSON = false;
        private List<FileInfoJSONMetadata> _listJSONNotUploaded;
        private TreeViewItem _formNameNode;
        private DispatcherTimer _timer;
        private TreeViewItem _jsonDateDownloadnode;
        private static ODKResultsWindow _instance;
        private List<MultiVesselGear_SampledLanding> _multiVesselMainSheets;
        private List<MultiVessel_Optimized_SampledLanding> _multiVesselOptimizedMainSheets;
        private List<VesselLanding> _mainSheets;
        private List<BoatLandings> _mainSheetsLanding;
        private string _excelDownloaded;
        private bool _isJSONData;
        private int _savedCount;
        private ODKServerDownload _odkServerDownload;
        private bool _uploadToDBSuccess;
        private JSONFile _jsonFile;
        private DateTime? _jsonFileUseCreationDateForHistory;
        private List<FileInfo> _jsonfiles;
        private int _countJSONFiles;
        DataGrid _targetGrid;
        private int _updateXFormIDCount;
        private TreeViewItem _firstJSONFileNode;
        private int? _jsonFileForUploadCount = null;
        private string _historyJSONFileForUploading = "";
        private List<UnrecognizedFishingGround> _unrecognizedFishingGrounds = new List<UnrecognizedFishingGround>();
        private bool _unrecognizedFGAlredyViewed = false;
        private DateTime? _unrecognizedFGDateCreated;
        private int _ufg_count = 0;
        private List<VesselLanding> _resolvedFishingGroundLandings = new List<VesselLanding>();
        private List<VesselLanding> _formsWithMissingLandingSiteInfo = new List<VesselLanding>();
        private int? _downloadedJSONBatchSize;
        private DownloadedJsonMetadata _downloadedJsonMetadata;
        private FileInfoJSONMetadata _selectedJSONMetaData;
        private string _json;
        private bool _jsonFromServer = false;
        private bool _updateMissingSubmission = false;
        public int CountJSONFilesForProcessingLandings { get; set; }
        public int CountJSONWithoutLandingData { get; set; }
        public bool JSONContainsLandingData { get; set; }
        public bool IsOptimizedMultiVessel { get; set; }
        public void JSONFromServer(string json, bool isMultivessel, bool is_optimized = false, bool updateMissingSubmission = false)
        {
            JSON = json;
            _jsonFromServer = true;
            IsMultiVessel = isMultivessel;
            IsOptimizedMultiVessel = is_optimized;
            _updateMissingSubmission = updateMissingSubmission;
        }
        public string JSON
        {
            get { return _json; }
            set
            {
                _json = value;
                IsMultiGear = JSON.Contains("fishing_gear_type_count");
                IsMultiVessel = JSON.Contains("repeat_landings_count");
                IsOptimizedMultiVessel = JSON.Contains("G_lss/sampling_date");
            }
        }
        public string FormID { get; set; }
        private bool _openLogInWindow;

        private void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            if (_openLogInWindow)
            {
                _openLogInWindow = false;
                OpenServerWindow(refreshDBSummary: true);
            }
        }

        public bool DownloadCSVFromServer { get; set; }
        public void OpenLogInWindow(bool isOpen = false)
        {
            _openLogInWindow = isOpen;
        }

        public string JSONFileName { get; set; }
        public string Version { get; set; }

        public string Description { get; set; }

        public int Count { get; set; }

        public KoboForm Koboform { get; set; }

        public ODKServerDownload ODKServerDownload
        {
            get { return _odkServerDownload; }
            set
            {
                _odkServerDownload = value;
            }
        }

        public MainWindow ParentWindow { get; set; }

        public static ODKResultsWindow GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ODKResultsWindow();
            }
            return _instance;
        }

        public ODKResultsWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
            Closed += OnWindowClosed;
        }

        public bool BatchUpload { get; set; }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            if (_timer != null)
            {
                _timer.Tick -= OnTimerTick;
            }
            _instance = null;

            try
            {
                if (_uploadToDBSuccess || _savedCount > 0)
                {
                    ((MainWindow)Owner).RefreshSummary();
                }
                ((MainWindow)Owner).Focus();
            }
            catch
            {
                //ignore
            }

            CreateTablesInAccess.AccessTableEvent -= CreateTablesInAccess_AccessTableEvent;
            NSAPMysql.MySQLConnect.AccessTableEvent -= CreateTablesInAccess_AccessTableEvent;
            VesselUnloadServerRepository.ResetGroupIDState();
            VesselUnloadServerRepository.ResetUnmatchedEnumeratorIDList();


        }

        private string GetJsonTextFileFromFileOpenDialog()
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Title = "Open json file generated by ODK Collect";
            opf.Filter = "Json (*.json)|*.json|Text (*.txt)|*.txt";
            opf.FilterIndex = 1;
            if ((bool)opf.ShowDialog(this))
            {
                return opf.FileName;
            }
            else
            {
                return "";
            }
        }

        private void GetExcelFromFileOpen()
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Title = "Open Excel file generated by ODK Collect";
            opf.Filter = "Excel (*.xlsx)|*.xlsx|Excel (*.xls)|*.xls";
            opf.FilterIndex = 1;
            if ((bool)opf.ShowDialog(this))
            {
                _isJSONData = false;
                ImportExcel.ExcelFileName = opf.FileName;
                if (ImportExcel.ExcelMainSheets.Count > 0)
                {
                    //we uncheck and check to force menu check to proceed
                    menuViewEffort.IsChecked = false;
                    menuViewEffort.IsChecked = true;
                }
                else
                {
                    dataGridExcel.ItemsSource = null;
                    dataGridExcel.Items.Clear();
                    MessageBox.Show("Imported excel file does not contain ODK data\r\n" +
                                     "or uses a different format for saving ODK data",
                                     "Excel file import error",
                                     MessageBoxButton.OK,
                                     MessageBoxImage.Information); ;
                }
            }
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {

            Title = "Manage NSAP-ODK data";
            labelProgress.Content = "";
            ImportExcel.UploadSubmissionToDB += OnUploadSubmissionToDB;
            VesselUnloadServerRepository.UploadSubmissionToDB += OnUploadSubmissionToDB;
            MultiVesselGear_UnloadServerRepository.UploadSubmissionToDB += OnUploadSubmissionToDBMultiVessel;
            VesselUnloadServerRepository.ClearUnrecgnizedFishingGroundsList();
            labelDuplicated.Content = string.Empty;
            menuDeleteFromServer.Visibility = Visibility.Collapsed;

            if (Debugger.IsAttached)
            {
                menuClearTables.Visibility = Visibility.Visible;
                menuVesselCountJSON.Visibility = Visibility.Visible;
                menuVesselUnloadJSON.Visibility = Visibility.Visible;
                menuDeletePastDate.Visibility = Visibility.Visible;
                menuDeleteFromServer.Visibility = Visibility.Visible;
            }

            menuImportSQLDump.Visibility = Visibility.Visible;
            if (Global.Settings.UsemySQL)
            {
                menuImportSQLDump.Visibility = Visibility.Collapsed;
            }

            menuSaveJson.IsEnabled = true;
            menuView.Visibility = Visibility.Collapsed;

            ResetView();
            rowGrid.Height = new GridLength(1, GridUnitType.Star);

            //if (NSAPEntities.KoboServerViewModel.Count() == 0)
            //{
            //    menuReUploadJsonHistory.Visibility = Visibility.Collapsed;
            //}

            if (DownloadFromServerWindow.HasLoggedInToServer)
            {
                menuDownloadFromServerOtherUser.Visibility = Visibility.Visible;
            }

            if (_openLogInWindow)
            {
                _timer = new DispatcherTimer();
                _timer.Tick += OnTimerTick;
                _timer.Interval = TimeSpan.FromSeconds(1);
                _timer.Start();
            }
            CreateTablesInAccess.AccessTableEvent += CreateTablesInAccess_AccessTableEvent;
            NSAPMysql.MySQLConnect.AccessTableEvent += CreateTablesInAccess_AccessTableEvent;
        }



        private void CreateTablesInAccess_AccessTableEvent(object sender, CreateTablesInAccessEventArgs e)
        {
            switch (e.Intent)
            {
                case "start importing csv":
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressBar.Value = 0;
                              progressBar.Maximum = e.TotalTableCount;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    labelProgress.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              labelProgress.Content = "Starting to save JSON data";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "done imported csv":
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              progressBar.Value = e.CurrentTableCount;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    labelProgress.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              labelProgress.Content = $"Finished saving JSON to {e.CurrentTableName} (Table {e.CurrentTableCount} of {progressBar.Maximum})";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
            }
        }

        private void SetMenuViewVisibility()
        {
            menuView.Visibility = Visibility.Visible;
            foreach (var item in menuView.Items)
            {
                if (item.GetType().Name == "MenuItem")
                {
                    var menu = (MenuItem)item;
                    menu.Visibility = Visibility.Collapsed;
                    switch (_odkServerDownload)
                    {
                        case ODKServerDownload.ServerDownloadVesselUnload:
                            if (!menu.Name.Contains("menuViewLandingSite"))
                            {
                                menu.Visibility = Visibility.Visible;
                            }
                            break;

                        case ODKServerDownload.ServerDownloadLandings:
                            if (menu.Name.Contains("menuViewLandingSite"))
                            {
                                menu.Visibility = Visibility.Visible;
                            }
                            break;
                    }
                }
            }
        }

        public List<BoatLandings> MainSheetsLanding
        {
            get { return _mainSheetsLanding; }
            set
            {
                menuSaveToExcel.Visibility = Visibility.Visible;
                _mainSheetsLanding = value;
                _isJSONData = true;

                if (menuViewLandingSiteSampling.IsChecked)
                {
                    ShowResultFromAPI("landingSiteSampling");
                }
                else
                {
                    menuViewLandingSiteSampling.IsChecked = true;
                }
                SetMenuViewVisibility();
            }
        }


        private void SetMenus()
        {
            menuSaveToExcel.Visibility = Visibility.Visible;
            _isJSONData = true;



            if (menuViewEffort.IsChecked)
            {
                if (IsMultiVessel)
                {
                    ShowResultFromAPI("effort_multiVessel");
                }
                else
                {
                    menuDuplicatedEffortSpecs.IsEnabled = VesselUnloadServerRepository.DuplicatedEffortSpec.Count > 0;
                    menuDuplicatedCatchComp.IsEnabled = VesselUnloadServerRepository.DuplicatedCatchComposition.Count > 0;
                    menuDuplicatedLF.IsEnabled = VesselUnloadServerRepository.DuplicatedLenFreq.Count > 0;

                    ShowResultFromAPI("effort");
                }
                //if (_jsonfiles != null && _jsonfiles.Count > 0)
                //{
                //    ShowResultFromAPI("effort", gridJSONContent);
                //}
                //else
                //{
                //    ShowResultFromAPI("effort", dataGridExcel);
                //}
            }
            else
            {
                menuViewEffort.IsChecked = true;
            }
            SetMenuViewVisibility();
        }

        public List<MultiVessel_Optimized_SampledLanding> MultiVesselOptimizedMainSheets
        {
            get { return _multiVesselOptimizedMainSheets; }
            set
            {
                _multiVesselOptimizedMainSheets = value;
                SetMenus();
            }
        }


        public List<MultiVesselGear_SampledLanding> MultiVesselMainSheets
        {
            get { return _multiVesselMainSheets; }
            set
            {
                _multiVesselMainSheets = value;
                SetMenus();
            }
        }
        public List<VesselLanding> MainSheets
        {
            get { return _mainSheets; }
            set
            {

                _mainSheets = value;
                SetMenus();

            }
        }
        public bool IsMultiGear { get; set; }
        public string ExcelFileDownloaded
        {
            get { return _excelDownloaded; }
            set
            {
                menuSaveToExcel.Visibility = Visibility.Collapsed;
                _isJSONData = false;
                _excelDownloaded = value;
                ImportExcel.ExcelFileName = _excelDownloaded;
                if (menuViewEffort.IsChecked)
                {
                    //ShowResultFromAPI("effort");
                    ShowResultFromExcel("effort");
                }
                else
                {
                    menuViewEffort.IsChecked = true;
                }
            }
        }

        public Task<bool> SaveJSONTextTask(bool verbose = true)
        {
            return Task.Run(() => SaveJSONText(verbose));
        }

        private async Task<JSONFile> CreateJsonFile(string description = "", string fileName = "", bool fromHistoryFiles = false, int? countVesselLandings = null, bool delaySave = false, bool fromServer = false)
        {
            var jsonFile = new JSONFile();
            if (fromServer)
            {
                jsonFile.JSONText = JSON;
            }

            if (JSON.Length <= 2)
            {
                return null;
            }

            jsonFile.VersionString = JSONFileViewModel.GetVersionString(JSON);
            jsonFile.MD5 = MD5.CreateMD5(JSON);
            jsonFile.RowID = NSAPEntities.JSONFileViewModel.NextRecordNumber;
            jsonFile.IsMultivessel = IsMultiVessel;
            if (IsMultiVessel || IsOptimizedMultiVessel)
            {
                if (IsOptimizedMultiVessel)
                {
                    MultiVessel_Optimized_UnloadServerRepository.JSON = JSON;
                    jsonFile.Earliest = MultiVessel_Optimized_UnloadServerRepository.DownloadedLandingsEarliestLandingDate();
                    jsonFile.Latest = MultiVessel_Optimized_UnloadServerRepository.DownloadedLandingsLatestLandingDate();
                    if (countVesselLandings == null)
                    {
                        countVesselLandings = MultiVessel_Optimized_UnloadServerRepository.SampledVesselLandings.Count;
                    }
                    jsonFile.Count = (int)countVesselLandings;
                }
                else
                {

                    MultiVesselGear_UnloadServerRepository.JSON = JSON;
                    jsonFile.Earliest = MultiVesselGear_UnloadServerRepository.DownloadedLandingsEarliestLandingDate();
                    jsonFile.Latest = MultiVesselGear_UnloadServerRepository.DownloadedLandingsLatestLandingDate();
                    if (countVesselLandings == null)
                    {
                        countVesselLandings = MultiVesselGear_UnloadServerRepository.SampledVesselLandings.Count;
                    }
                    jsonFile.Count = (int)countVesselLandings;

                }

            }
            else
            {
                VesselUnloadServerRepository.JSON = JSON;
                jsonFile.Earliest = VesselUnloadServerRepository.DownloadedLandingsEarliestLandingDate();
                jsonFile.Latest = VesselUnloadServerRepository.DownloadedLandingsLatestLandingDate();
                jsonFile.Count = VesselUnloadServerRepository.DownloadedLandingsCount();
                //jsonFile.LandingIdentifiers = VesselUnloadServerRepository.GetLandingIdentifiers();
                //jsonFile.CountVesselLandings = VesselUnloadServerRepository.VesselLandings.Count;
            }

            //jsonFile.DateAdded = DateTime.Now;


            if (FormID == null)
            {
                if (jsonFile.FullFileName != null)
                {
                    FormID = Path.GetFileName(jsonFile.FullFileName).Split(' ')[0];
                }
                if (_selectedJSONMetaData == null)
                {

                    FormID = Path.GetFileName(JSONFileName).Split(' ')[0];
                }
                else
                {
                    FormID = _selectedJSONMetaData.Koboserver.ServerNumericID.ToString();
                }
            }
            jsonFile.FormID = FormID;
            if (description.Length == 0)
            {
                var ks = NSAPEntities.KoboServerViewModel.KoboserverCollection.FirstOrDefault(t => t.ServerNumericID == int.Parse(FormID));
                if (ks != null)
                {
                    description = ks.FormName;
                }
            }
            jsonFile.Description = description;
            if (fileName.Length > 0)
            {
                jsonFile.FullFileName = fileName;
                jsonFile.DateAdded = (DateTime)_jsonFileUseCreationDateForHistory;
            }
            else
            {
                if (JSONFileName != null && JSONFileName.Length > 0)
                {
                    jsonFile.FullFileName = JSONFileName;
                }
                else
                {
                    jsonFile.FullFileName = $@"{Global.Settings.JSONFolder}\{NSAPEntities.JSONFileViewModel.CreateFileName(jsonFile)}";
                    jsonFile.DateAdded = DateTime.Now;
                }
            }

            //Console.WriteLine($"jsonfile filename is {jsonFile.FileName}");
            if (!delaySave && !JsonFileIsSaved(jsonFile))
            {
                await NSAPEntities.JSONFileViewModel.Save(jsonFile, _jsonFromServer);
            }
            else if (File.Exists(jsonFile.FullFileName))
            {
                //file exists
                //do nothing
            }
            else
            {
                var f = File.CreateText(jsonFile.FullFileName);
                f.Close();
            }
            //_jsonFromServer = false;
            return jsonFile;
        }

        private bool JsonFileIsSaved(JSONFile jsonFile)
        {
            return NSAPEntities.JSONFileViewModel.Count() > 0 && NSAPEntities.JSONFileViewModel.getJSONFIle(jsonFile.MD5) != null;
        }
        private async Task<bool> SaveJSONText(bool verbose = true)
        //private async Task<bool> SaveJSONText(bool verbose = true)
        {
            bool success = false;
            if (_jsonFile == null)
            {
                _jsonFile = await CreateJsonFile(fromServer: _jsonFromServer);
            }
            if (_jsonFile != null)
            {
                success = true;
                NSAPEntities.KoboServerViewModel.ResetJSONFields(resetLastUploaded: false, isMultiGearform: IsMultiGear, isMultiVesselform: IsMultiVessel);
                var ks = NSAPEntities.KoboServerViewModel.GetKoboServer(int.Parse(_jsonFile.FormID));
                if (ks != null)
                {
                    ks.LastCreatedJSON = new FileInfo(_jsonFile.FullFileName).Name;
                    NSAPEntities.KoboServerViewModel.ServerWithCreatedJSON = ks;
                }
                else
                {
                    verbose = true;
                    success = false;
                }
                if (verbose)
                {
                    if (ks == null)
                    {
                        MessageBox.Show("Please log-in to the Kobotoolbox server to refresh server information", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("JSON file saved successfully", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                success = false;
            }
            //_jsonFile = new JSONFile();
            //_jsonFile.JSONText = JSON;
            //_jsonFile.MD5 = MD5.CreateMD5(_jsonFile.JSONText);
            //_jsonFile.RowID = NSAPEntities.JSONFileViewModel.NextRecordNumber;
            //_jsonFile.FormID = FormID;
            //_jsonFile.Description = Description;
            //_jsonFile.Earliest = VesselUnloadServerRepository.DownloadedLandingsEarliestLandingDate();
            //_jsonFile.Latest = VesselUnloadServerRepository.DownloadedLandingsLatestLandingDate();
            //_jsonFile.Count = VesselUnloadServerRepository.DownloadedLandingsCount();
            //_jsonFile.LandingIdentifiers = VesselUnloadServerRepository.GetLandingIdentifiers();
            //_jsonFile.DateAdded = DateTime.Now;
            //_jsonFile.FileName = $@"{Global.Settings.JSONFolder}\{NSAPEntities.JSONFileViewModel.CreateFileName(_jsonFile)}";


            //if (NSAPEntities.JSONFileViewModel.Count() == 0 || NSAPEntities.JSONFileViewModel.getJSONFIle(_jsonFile.MD5) == null)
            //if (!JsonFileIsSaved(_jsonFile))
            //{
            //    success = await NSAPEntities.JSONFileViewModel.Save(_jsonFile);
            //    if (success)
            //    {
            //        NSAPEntities.KoboServerViewModel.ResetJSONFields(resetLastUploaded: false);
            //        var ks = NSAPEntities.KoboServerViewModel.GetKoboServer(int.Parse(_jsonFile.FormID));
            //        ks.LastCreatedJSON = new FileInfo(_jsonFile.FullFileName).Name;
            //        NSAPEntities.KoboServerViewModel.ServerWithCreatedJSON = ks;

            //        if (verbose)
            //        {
            //            MessageBox.Show("JSON file saved successfully", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
            //        }
            //    }
            //}
            //else
            //{
            //    success = true;
            //    if (verbose)
            //    {
            //        MessageBox.Show("JSON file already saved in database", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
            //    }
            //}
            return success;
        }

        private void ResetView()
        {
            rowJsonFiles.Height = new GridLength(0);
            rowGrid.Height = new GridLength(0);
            labelJSONFile.Content = "";
            _targetGrid = dataGridExcel;
            //rowGrid.Height = new GridLength(1, GridUnitType.Star);
        }


        private void AddFilesToDateNode(TreeViewItem dateNode, DownloadedJsonMetadata djmd, string dateDownloaded)
        {
            int counter = 0;
            foreach (FileInfo f in _jsonfiles.OrderBy(t => t.Name))
            {
                if (f.Extension == ".json" && f.Name.Contains(djmd.FileName.Replace("_info.xml", "")))
                {

                    TreeViewItem fileNode = new TreeViewItem
                    {
                        Header = $"{dateDownloaded} {++counter}",
                        Tag = new FileInfoJSONMetadata { JSONFileInfo = f, DownloadedJsonMetadata = djmd, ItemNumber = counter },

                    };

                    dateNode.Items.Add(fileNode);
                    _countJSONFiles++;
                }
            }

        }
        public bool StartAtBeginningOfJSONDownloadList { get; set; }

        public bool OnlyUploadJSONHistoryFromMultiVesselForm { get; set; }
        public UpdateJSONHistoryMode UpdateJSONHistoryMode { get; set; }
        private HashSet<string> _rootChildrenHeadersHashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private void AddMetadataToTreeView(DownloadedJsonMetadata djmd, TreeViewItem root)
        {


            string dateDownloaded = djmd.DateDownloaded.ToString("MMM-dd-yyyy HH:mm");
            TreeViewItem formNameNode = new TreeViewItem { Header = djmd.FormName };
            TreeViewItem formOwnerNode = new TreeViewItem { Header = djmd.DBOwner };
            MenuItem mi = new MenuItem { Header = "Analyze all JSON files", Name = "menuAnalyzeJSONOfOwner" };
            mi.Click += OnMenuClick;
            ContextMenu cm = new ContextMenu();
            cm.Items.Add(mi);
            formOwnerNode.ContextMenu = cm;

            TreeViewItem dateDownloadNode = new TreeViewItem { Header = dateDownloaded, Tag = "date_download" };
            dateDownloadNode.Tag = djmd;

            if (root.Items.Count == 0)
            {
                _rootChildrenHeadersHashSet.Add(formNameNode.Header.ToString());
                root.Items.Add(formNameNode);
                formNameNode.Items.Add(formOwnerNode);
                formOwnerNode.Items.Add(dateDownloadNode);
                AddFilesToDateNode(dateDownloadNode, djmd, dateDownloaded);
                formNameNode.ExpandSubtree();
                _firstJSONFileNode = dateDownloadNode.Items[0] as TreeViewItem;
                if (formOwnerNode.ContextMenu != null)
                {

                }
            }
            else if (_rootChildrenHeadersHashSet.Add(formNameNode.Header.ToString()))
            {
                root.Items.Add(formNameNode);
                formNameNode.Items.Add(formOwnerNode);
                formOwnerNode.Items.Add(dateDownloadNode);
                AddFilesToDateNode(dateDownloadNode, djmd, dateDownloaded);
            }
            else
            {

                bool found = true;
                foreach (TreeViewItem tvi in root.Items)
                {
                    if (tvi.Header.ToString() == djmd.FormName)
                    {
                        found = true;
                        formNameNode = tvi;
                    }
                    if (!found)
                    {
                        root.Items.Add(formNameNode);
                    }
                }

                found = false;
                foreach (TreeViewItem tvi1 in formNameNode.Items)
                {
                    if (tvi1.Header.ToString() == djmd.DBOwner)
                    {
                        formOwnerNode = tvi1;
                        found = true;
                    }
                }
                if (!found)
                {
                    formNameNode.Items.Add(formOwnerNode);
                }

                found = false;
                foreach (TreeViewItem tvi2 in formOwnerNode.Items)
                {
                    if (tvi2.Header.ToString() == dateDownloaded)
                    {
                        found = true;
                        dateDownloadNode = tvi2;
                    }
                }
                if (!found)
                {
                    dateDownloadNode.Tag = djmd;
                    formOwnerNode.Items.Add(dateDownloadNode);
                    AddFilesToDateNode(dateDownloadNode, djmd, dateDownloaded);

                }


            }
        }


        private async Task ProcessHistoryJsonNode(TreeViewItem historyJSONNode, int loopCount = 0)
        {
            bool proceed = true;
            var jm = (FileInfoJSONMetadata)historyJSONNode.Tag;
            if (OnlyUploadJSONHistoryFromMultiVesselForm && !jm.Koboserver.IsFishLandingMultiVesselSurveyForm)
            {
                proceed = false;
            }

            if (proceed)
            {
                _jsonFileUseCreationDateForHistory = jm.JSONFileInfo.CreationTime;
                historyJSONNode.IsSelected = true;
                _isJSONData = true;
                //if (jm.JSONFile != null)
                //{
                await Upload(verbose: !VesselUnloadServerRepository.DelayedSave, loopCount: loopCount, jsonFullFileName: jm.JSONFileInfo.FullName, fromHistoryFiles: true);
                NSAPEntities.KoboServerViewModel.ResetJSONFields();
                jm.Koboserver.LastUploadedJSON = jm.JSONFileInfo.Name;
                NSAPEntities.KoboServerViewModel.ServerWithUploadedJSON = jm.Koboserver;
                jm.JSONFile?.Dispose();
                //}
                //GC.Collect();
                //GC.WaitForPendingFinalizers();
            }
        }


        private async Task AnalyzeJSONHistoryNodes(TreeViewItem root)
        {
            foreach (TreeViewItem jsonNode in root.Items)
            {

                _selectedJSONMetaData = (FileInfoJSONMetadata)jsonNode.Tag;
                ProcessJsonFileForDisplay(_selectedJSONMetaData);
                //gridJSONContent.ItemsSource = null;
                //gridJSONContent.ItemsSource = VesselUnloadServerRepository.VesselLandings;
                if (_selectedJSONMetaData.JSONFile == null)
                {
                    _selectedJSONMetaData.JSONFile = NSAPEntities.JSONFileViewModel.getJSONFileFromFileName(_selectedJSONMetaData.JSONFileInfo.Name);
                    if (_selectedJSONMetaData.JSONFile == null)
                    {
                        _selectedJSONMetaData.JSONFile = await CreateJsonFile("NSAP Fish Catch Monitoring e-Form", _selectedJSONMetaData.JSONFileInfo.FullName);
                    }
                    AnalyzeJsonForMismatch.Analyze(VesselUnloadServerRepository.VesselLandings, _selectedJSONMetaData.JSONFile);
                }
            }


        }
        private bool IsValidJSONHistoryFile(string fileName)
        {
            var arr = fileName.Split(new char[] { ' ', '.' });
            DateTime? d1 = null;
            DateTime? d2 = null;
            bool proceed = false;
            if (int.TryParse(arr[0], out int i) && NSAPEntities.KoboServerViewModel.GetKoboServer(i) != null)
            {
                proceed = true;
                for (int x = 1; x < arr.Length; x++)
                {
                    if (DateTime.TryParse(arr[x], out DateTime d))
                    {
                        if (d1 == null)
                        {
                            d1 = d;
                        }
                        else if (d2 == null)
                        {
                            d2 = d;
                            break;
                        }
                    }
                }
            }
            return proceed && d1 != null && d2 != null && (DateTime)d2 > (DateTime)d1;
        }
        private async Task ProcessJSONHistoryNodes(TreeViewItem root)
        {
            int loopCount = 0;
            bool firstLoopDone = false;
            bool lastJSONUploadFound = false;
            string lastUploadedJSON = NSAPEntities.KoboServerViewModel.GetLastUploadedJSON();
            NSAPEntities.ClearCSVData();

            bool proceed = true;
            foreach (TreeViewItem jsonNode in root.Items)
            {
                if (!VesselUnloadServerRepository.CancelUpload)
                {
                    var jm = (FileInfoJSONMetadata)jsonNode.Tag;

                    if (!firstLoopDone)
                    {
                        MultiVessel_Optimized_UnloadServerRepository.ResetGroupIDs();
                        MultiVesselGear_UnloadServerRepository.ResetGroupIDs();
                        VesselUnloadServerRepository.ResetGroupIDs();//delayedSave: true);
                        firstLoopDone = true;
                    }

                    if (string.IsNullOrEmpty(lastUploadedJSON) || UpdateJSONHistoryMode == UpdateJSONHistoryMode.UpdateReplaceExistingData || StartAtBeginningOfJSONDownloadList)
                    {
                        //if (!firstLoopDone)
                        //{
                        //    MultiVesselGear_UnloadServerRepository.ResetGroupIDs();
                        //    VesselUnloadServerRepository.ResetGroupIDs();//delayedSave: true);
                        //    firstLoopDone = true;
                        //}
                        await ProcessHistoryJsonNode(jsonNode, loopCount);
                        loopCount++;
                    }
                    else
                    {
                        if (jm.JSONFileInfo.Name == lastUploadedJSON || lastJSONUploadFound)
                        {
                            await ProcessHistoryJsonNode(jsonNode, loopCount);
                            lastJSONUploadFound = true;
                            loopCount++;
                        }
                    }

                    if (VesselUnloadServerRepository.TotalUploadCount >= Global.Settings.DownloadSizeForBatchMode)
                    {
                        proceed = await SaveUploadedJsonInLoop();

                    }
                    else if (MultiVesselGear_UnloadServerRepository.TotalUploadCount > 0)
                    {
                        proceed = await SaveUploadedJsonInLoop();
                    }
                }
                else
                {

                    VesselUnloadServerRepository.CancelUpload = false;
                    break;
                }
                VesselUnloadServerRepository.VesselLandings?.Clear();

                if (!proceed)
                {
                    break;
                }
            }

            if (proceed)
            {
                MessageBox.Show($"Finished processing {loopCount} history items",
                    Global.MessageBoxCaption,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private async Task<bool> ProcessResolvedLandings(List<VesselLanding> resolvedLandings)
        {
            return await VesselUnloadServerRepository.UploadToDBResolvedLandingsAsync(resolvedLandings);
        }

        private async Task ProcessLandingsWithMissingLandingSites()
        {
            VesselUnloadServerRepository.LandingWithUpdatedLandingSite += VesselUnloadServerRepository_LandingWithUpdatedLandingSite;
            await VesselUnloadServerRepository.UpdateMissingLandingSitesAsync();
            VesselUnloadServerRepository.LandingWithUpdatedLandingSite -= VesselUnloadServerRepository_LandingWithUpdatedLandingSite;
        }

        private void VesselUnloadServerRepository_LandingWithUpdatedLandingSite(int obj)
        {
            //
        }

        private async Task<bool> ProcessJSONSNodes(bool updateXFormID = false, bool locateUnsavedFromServerDownload = false, bool updateLandingSites = false, bool locateMissingLSInfo = false)
        {



            int nodeProcessedCount = 0;
            bool firstLoopDone = false;
            _jsonDateDownloadnode.IsExpanded = true;
            VesselUnloadServerRepository.CancelUpload = false;
            NSAPEntities.ClearCSVData();
            bool success = true;
            CountJSONWithoutLandingData = 0;
            foreach (TreeViewItem tvi in _jsonDateDownloadnode.Items)
            {

                //when a treeview item is selected, it will read the JSON associated with the item
                //and loads it to the JSON parser and creates landings which are then processed individually
                tvi.IsSelected = true;

                
                if (!VesselUnloadServerRepository.CancelUpload)
                {
                    if (updateXFormID)
                    {
                        await ProcessXFormIDs();
                    }
                    else if (locateUnsavedFromServerDownload)
                    {
                        await ProcessXFormIDs(locateUnsavedFromServer: true);
                    }
                    if (updateLandingSites)
                    {
                        await ProcessLandingsWithMissingLandingSites();
                    }
                    if (locateMissingLSInfo)
                    {
                        _formsWithMissingLandingSiteInfo.AddRange(await VesselUnloadServerRepository.GetFormWithMissingLandingSiteInfoAsync());
                    }
                    else
                    {
                        FileInfoJSONMetadata jm = (FileInfoJSONMetadata)tvi.Tag;
                        CountJSONFilesForProcessingLandings = jm.DownloadedJsonMetadata.NumberOfFilesDownloaded;
                        if (JSONContainsLandingData)
                        {
                            if (MultiVesselGear_UnloadServerRepository.JSON == null && VesselUnloadServerRepository.JSON == null && MultiVessel_Optimized_UnloadServerRepository.JSON == null)
                            {
                                Logger.Log($"Possible error in JSON in file {jm.JSONFileInfo.FullName}. JSON was not extracted");
                            }
                            else
                            {
                                if (!firstLoopDone)
                                {
                                    if (IsOptimizedMultiVessel)
                                    {
                                        MultiVessel_Optimized_UnloadServerRepository.ResetGroupIDs();
                                    }
                                    else if (IsMultiVessel)
                                    {
                                        MultiVesselGear_UnloadServerRepository.ResetGroupIDs();// VesselUnloadServerRepository.DelayedSave);
                                    }
                                    else
                                    {
                                        VesselUnloadServerRepository.ResetGroupIDs();
                                    }

                                    //if (NSAPEntities.SummaryItemViewModel.Count == 0)
                                    //{
                                    //    VesselUnloadServerRepository.ResetGroupIDs();
                                    //}
                                    //else if (IsMultiVessel)
                                    //{
                                    //    MultiVesselGear_UnloadServerRepository.ResetGroupIDs();// VesselUnloadServerRepository.DelayedSave);
                                    //}

                                    firstLoopDone = true;
                                }
                                //FileInfoJSONMetadata jm = (FileInfoJSONMetadata)tvi.Tag;
                                VesselUnloadServerRepository.DelayedSave = !Global.Settings.UsemySQL;
                                _isJSONData = true;
                                success = await Upload(verbose: !VesselUnloadServerRepository.DelayedSave, fromJSONBatchFiles: true, loopCount: nodeProcessedCount, jsonFullFileName: jm.JSONFileInfo.FullName); ;
                                if (success)
                                {
                                    success = await SaveUploadedJsonInLoop(isHistoryJson: false);
                                    try
                                    {
                                        jm.JSONFile?.Dispose();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Log(ex);
                                    }
                                    if (!success)
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    //all data already saved 
                                }
                            }
                            nodeProcessedCount++;
                        }
                        else
                        {
                            Logger.Log($"JSON file does not contain landing data \r\n{jm.JSONFileInfo.FullName} ");
                            CountJSONWithoutLandingData++;
                        }
                        
                    }
                }
                else
                {
                    break;
                }
            }
            VesselUnloadServerRepository.UpdateInProgress = false;
            VesselUnloadServerRepository.UploadInProgress = false;
            return success;
        }

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
            if (Global.Settings.JSONFolder != null && Global.Settings.JSONFolder.Length > 0)
            {
                vfbd.SelectedPath = Global.Settings.JSONFolder;
            }
            else
            {
                vfbd.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
            }
            if ((bool)vfbd.ShowDialog() && vfbd.SelectedPath.Length > 0)
            {
                Global.Settings.JSONFolder = vfbd.SelectedPath;
                return vfbd.SelectedPath;
            }
            return "";
        }
        public bool OpenImportedDatabaseInApplication { get; set; }
        public string ImportIntoMDBFile { get; set; }
        public SelectImportActionOption ImportActionOption { get; set; }


        private async void OnMenuClick(object sender, RoutedEventArgs e)
        {
            BatchUpload = false;
            UploadJSONHistoryOptionsWindow ujhw;
            int counter = 0;
            bool proceed = false;
            string jsonFolder = "";
            string menuName = ((MenuItem)sender).Name;
            DownloadFromServerWindow serverForm = null;
            switch (menuName)
            {
                //File->Delete landing data from server
                case "menuDeleteFromServer":
                    serverForm = new DownloadFromServerWindow(this);
                    VesselUnloadServerRepository.ResetLists();
                    serverForm.ServerIntent = ServerIntent.IntentDeleteLandingDataFromServer;
                    serverForm.Owner = this;
                    serverForm.ShowDialog();
                    break;
                //tree context menu -> Analyze all JSON files
                //when header (name of owner of server where JSON files were downloaded) is rightt clicked
                case "menuAnalyzeJSONOfOwner":
                    #region menuAnalyzeJSONOfOwner
                    TreeViewItem selectedNode = treeViewJSONNavigator.SelectedItem as TreeViewItem;
                    var formName = ((TreeViewItem)selectedNode.Parent).Header.ToString();
                    string owner = selectedNode.Header.ToString();
                    var fileList = _jsonfiles
                        .Where(t => t.Extension == ".json" &&
                        t.Name.Contains(formName) &&
                        t.Name.Contains(selectedNode.Header.ToString())).ToList();
                    ProgressDialogWindow pdw = ProgressDialogWindow.GetInstance("analyze batch json files");
                    pdw.BatchJSONFiles = fileList;
                    pdw.Koboserver = NSAPEntities.KoboServerViewModel.KoboserverCollection.Where(t => t.FormName == formName && t.Owner == owner).FirstOrDefault();
                    pdw.Owner = Owner;
                    if (pdw.Visibility == Visibility.Visible)
                    {
                        pdw.BringIntoView();
                    }
                    else
                    {
                        pdw.Show();
                    }
                    break;
                #endregion


                //tree context menu ->Analyze all json
                //when header of a set of JSON history is right clicked
                case "menuAnalyzeAllJSON":
                    #region menuAnalyzeAllJSON
                    if (NSAPEntities.KoboServerViewModel.Count() > 0)
                    {
                        List<FileInfoJSONMetadata> metadatas = new List<FileInfoJSONMetadata>();
                        foreach (TreeViewItem jsonNode in ((TreeViewItem)treeViewJSONNavigator.SelectedItem).Items)
                        {
                            metadatas.Add((FileInfoJSONMetadata)jsonNode.Tag);
                        }
                        pdw = ProgressDialogWindow.GetInstance("analyze json history files");
                        pdw.FileInfoJSONMetadatas = metadatas;
                        pdw.Owner = Owner;
                        if (pdw.Visibility == Visibility.Visible)
                        {
                            pdw.BringIntoView();
                        }
                        else
                        {
                            pdw.Show();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please login to the server to save the Kobotoolbox database identifiers into the database",
                            Global.MessageBoxCaption,
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                            );
                    }

                    break;
                #endregion


                //tree context menu -> Analyze JSON
                //analyze json content of a file that belongs to a batch of JSON files or to set of JSON history files
                case "menuAnalyzeJSON":
                    #region menuAnalyzeJSON
                    var unMatched = NSAPEntities.UnmatchedFieldsFromJSONFileViewModel.GetItem(Path.GetFileName(_selectedJSONMetaData.JSONFileInfo.FullName));
                    if (unMatched == null)
                    {
                        if (_selectedJSONMetaData.JSONFile == null)
                        {
                            _selectedJSONMetaData.JSONFile = NSAPEntities.JSONFileViewModel.getJSONFileFromFileName(_selectedJSONMetaData.JSONFileInfo.Name);
                            if (_selectedJSONMetaData.JSONFile == null)
                            {
                                _selectedJSONMetaData.JSONFile = await CreateJsonFile("NSAP Fish Catch Monitoring e-Form", _selectedJSONMetaData.JSONFileInfo.FullName);
                            }
                        }
                        if (AnalyzeJsonForMismatch.Analyze(VesselUnloadServerRepository.VesselLandings, _selectedJSONMetaData.JSONFile))
                        {
                            unMatched = NSAPEntities.UnmatchedFieldsFromJSONFileViewModel.GetItem(Path.GetFileName(_selectedJSONMetaData.JSONFileInfo.FullName));
                        }
                        else
                        {
                            return;
                        }
                    }
                    UnmatchedJSONAnalysisResultWindow urw = UnmatchedJSONAnalysisResultWindow.GetInstance();
                    urw.UnmatchedFieldsFromJSONFile = unMatched;
                    urw.Owner = this;
                    if (urw.Visibility == Visibility.Visible)
                    {
                        urw.BringIntoView();
                    }
                    else
                    {
                        urw.Show();
                    }

                    break;
                #endregion


                //tree context menu ->Locate missing landing site info
                //applicable to batch downloaded json files of type download all
                case "menuLocateMissingLSInfo":
                    #region menuLocateMissingLSInfo
                    _formsWithMissingLandingSiteInfo.Clear();
                    await ProcessJSONSNodes(locateMissingLSInfo: true);
                    break;
                #endregion


                //Tree context menu ->Locate unsaved landings uploaded to server
                //applicable to batch downloaded json files of type download all
                case "menuLocateUnsaved":
                    #region menuLocateUnsaved
                    await ProcessJSONSNodes(locateUnsavedFromServerDownload: true);
                    break;
                #endregion


                //File->import sql dump
                case "menuImportSQLDump":
                    #region menuImportSQLDump
                    SelectImportActionWindow siaw = new SelectImportActionWindow();
                    siaw.Owner = this;
                    if ((bool)siaw.ShowDialog())
                    {
                        ImportSQLDumpWindow isqlw = new ImportSQLDumpWindow();
                        isqlw.ImportActionOption = ImportActionOption;
                        isqlw.ImportIntoMDBFile = ImportIntoMDBFile;

                        isqlw.Owner = this;
                        if ((bool)isqlw.ShowDialog())
                        {
                            if (OpenImportedDatabaseInApplication)
                            {
                                ((MainWindow)Owner).OpenImportedDatabaseInApplication = true;
                                ((MainWindow)Owner).ImportIntoMDBFile = ImportIntoMDBFile;

                            }
                        }
                    }
                    break;
                #endregion


                //opens a form that shows a table of sampled fish landings with fishing grounds not yet entered into the database
                case "menuUnrecognizedFG":
                    #region menuUnrecognizedFG
                    if (_unrecognizedFishingGrounds.Count > 0)
                    {
                        ViewUnrecognizedFishingGrounds();
                    }
                    break;
                #endregion


                //Tree context menu ->upload all
                //uploads all the json history files in the treeview.
                //when header of a set of history files is right clicked
                case "menuUploadAllJsonHistoryFiles":
                    #region menuUploadAllJsonHistoryFiles



                    proceed = true;
                    ujhw = new UploadJSONHistoryOptionsWindow();
                    ujhw.Owner = this;
                    ujhw.JSONFilesToUploadType = JSONFilesToUploadType.UploadTypeJSONHistoryFiles;
                    if ((bool)ujhw.ShowDialog())
                    {
                        //VesselUnloadServerRepository.DelayedSave = true;
                        VesselUnloadServerRepository.ResetTotalUploadCounter();
                        VesselUnloadServerRepository.ResetGroupIDs();// VesselUnloadServerRepository.DelayedSave);
                        VesselUnloadServerRepository.CancelUpload = false;

                        MultiVesselGear_UnloadServerRepository.ResetTotalUploadCounter();
                        MultiVesselGear_UnloadServerRepository.ResetGroupIDs();
                        MultiVesselGear_UnloadServerRepository.CancelUpload = false;

                        MultiVessel_Optimized_UnloadServerRepository.ResetTotalUploadCounter();
                        MultiVessel_Optimized_UnloadServerRepository.ResetGroupIDs();
                        MultiVessel_Optimized_UnloadServerRepository.CancelUpload = false;

                        //if we need to replace existing data and then update all
                        if (UpdateJSONHistoryMode == UpdateJSONHistoryMode.UpdateReplaceExistingData)
                        {
                            proceed = ClearTables(verboseMode: false);
                        }


                        if (proceed)
                        {
                            //call function which uploads all the json files that are listed in the treeview
                            try
                            {
                                BatchUpload = true;
                                await ProcessJSONHistoryNodes((TreeViewItem)treeViewJSONNavigator.SelectedItem);
                                await SaveUploadedJsonInLoop(closeWindow: true, verbose: true);
                            }
                            catch (Exception ex)
                            {
                                Logger.Log(ex);
                            }
                        }


                    }

                    break;
                #endregion


                //File->reupload JSON history files
                //locate folder of JSON history files and lists them in a treeview
                case "menuReUploadJsonHistory":
                    #region menuReUploadJsonHistory
                    //Console.Clear();
                    if (NSAPEntities.KoboServerViewModel.Count() == 0)
                    {
                        var result = MessageBox.Show(
                            "Log-in to the kobotoolbox server is required\r\n\r\nDo you want to log-in now?",
                            "NSAP-ODK Database",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Information);

                        if (result == MessageBoxResult.Yes)
                        {
                            OpenServerWindow();
                        }
                    }
                    else
                    {
                        ClearJSONTreeRootNodes();
                        counter = 0;
                        jsonFolder = GetJSONFolder(savedHistory: true);
                        if (jsonFolder.Length > 0)
                        {
                            _jsonfiles = Directory.GetFiles(jsonFolder).Select(s => new FileInfo(s)).ToList();
                            if (_jsonfiles.Any())
                            {
                                TreeViewItem root = new TreeViewItem { Header = "Upload history JSON files" };
                                treeViewJSONNavigator.Items.Add(root);
                                foreach (var f in _jsonfiles.OrderBy(t => t.CreationTime))
                                {
                                    if (f.Extension == ".json")
                                    {
                                        var parts = f.Name.Split(new char[] { ' ' });
                                        //if (int.TryParse(parts[0], out int v) && (DateTime.TryParse(parts[1], out DateTime d1) &&
                                        //    DateTime.TryParse(parts[1], out DateTime d3)))
                                        if (IsValidJSONHistoryFile(f.Name))
                                        {
                                            var ks = NSAPEntities.KoboServerViewModel.GetKoboServer(int.Parse(parts[0]));
                                            if (ks != null && ks.IsFishLandingSurveyForm)
                                            {
                                                counter++;
                                                FileInfoJSONMetadata fm = new FileInfoJSONMetadata
                                                {
                                                    JSONFileInfo = new FileInfo(f.FullName),
                                                    ItemNumber = counter,
                                                    Koboserver = ks
                                                };
                                                //TreeViewItem fileNode = new TreeViewItem { Header = f.Name, Tag = f.FullName };
                                                TreeViewItem fileNode = new TreeViewItem { Header = f.Name, Tag = fm };
                                                root.Items.Add(fileNode);
                                            }
                                        }
                                    }
                                }
                                root.IsExpanded = true;
                            }
                        }
                        ResetView();
                        if (counter > 0)
                        {
                            _targetGrid = gridJSONContent;
                            //ProcessJsonFileForDisplay((FileInfoJSONMetadata)_firstJSONFileNode.Tag);
                            //treeViewJSONNavigator.Focus();
                            _jsonFileForUploadCount = counter;
                        }
                        else
                        {
                            treeViewJSONNavigator.DataContext = null;
                            treeViewJSONNavigator.Items.Clear();
                            rowGrid.Height = new GridLength(1, GridUnitType.Star);
                            MessageBox.Show("No JSON files from Kobotoolbox server was found", "NSAP-ODK Database");
                            return;
                        }
                        rowJsonFiles.Height = new GridLength(1, GridUnitType.Star);
                    }

                    break;
                #endregion


                //tree context menu ->reupload all
                //when batch downloaded JSON is of type download_all is selected
                case "menuReuploadAll":
                    #region menuReuploadAll
                    proceed = true;
                    ujhw = new UploadJSONHistoryOptionsWindow();
                    ujhw.JSONFilesToUploadType = JSONFilesToUploadType.UploadTypeDownloadedJsonDownloadAll;
                    ujhw.Owner = this;
                    if ((bool)ujhw.ShowDialog())
                    {
                        if (!Global.Settings.UsemySQL)
                        {
                            //NSAPEntities.ClearCSVData();
                            VesselUnloadServerRepository.ResetTotalUploadCounter();
                            MultiVesselGear_UnloadServerRepository.ResetTotalUploadCounter();
                            MultiVessel_Optimized_UnloadServerRepository.ResetTotalUploadCounter();
                            VesselUnloadServerRepository.DelayedSave = true;
                            VesselUnloadServerRepository.ResetGroupIDs();//VesselUnloadServerRepository.DelayedSave);
                        }
                        _jsonFileUseCreationDateForHistory = null;
                        if (UpdateJSONHistoryMode == UpdateJSONHistoryMode.UpdateReplaceExistingData)
                        {
                            proceed = ClearTables(verboseMode: false);
                        }

                        if (proceed)
                        {
                            BatchUpload = true;
                            if (await ProcessJSONSNodes())
                            {
                                await SaveUploadedJsonInLoop(closeWindow: true, verbose: true, isHistoryJson: false, allowDownloadAgain: true);
                            }
                        }

                    }

                    break;
                #endregion


                //tree context menu ->update landing sites
                //applicable to batch downloaded json files of type download all
                case "menuUpdateLandingSites":
                    #region menuUpdateLandingSites
                    ujhw = new UploadJSONHistoryOptionsWindow();
                    ujhw.JSONFilesToUploadType = JSONFilesToUploadType.UploadTypeJSONHistoryUpdateLandingSites;
                    ujhw.Owner = this;
                    if ((bool)ujhw.ShowDialog())
                    {
                        await ProcessJSONSNodes(updateLandingSites: true);
                    }
                    break;
                #endregion


                // tree context menu -> update xFormIdentifier
                //applicable to batch downloaded json files of type download all
                case "menuUpdateXformIdentifier":
                    #region menuUpdateXformIdentifier
                    ujhw = new UploadJSONHistoryOptionsWindow();
                    ujhw.JSONFilesToUploadType = JSONFilesToUploadType.UploadTypeJSONHistoryUpdateXFormID;
                    ujhw.Owner = this;
                    if ((bool)ujhw.ShowDialog())
                    {
                        await ProcessJSONSNodes(updateXFormID: true);
                    }


                    break;
                #endregion


                //Tree context menu->upload ALL
                //upload all files belonging to a batch of JSON files
                //invoked after selecting context menu on xml header of batch json files
                case "menuUploadAllJsonFiles":
                    #region menuUploadAllJsonFiles
                    //invoked when right clicking on xml file describing a set of downloaded json files
                    //this is the actual call to process json files represented by tree node
                    ujhw = new UploadJSONHistoryOptionsWindow();
                    ujhw.JSONFilesToUploadType = JSONFilesToUploadType.UploadTypeDownloadedJsonNotDownloaded;
                    ujhw.Owner = this;
                    if ((bool)ujhw.ShowDialog())
                    {
                        MultiVesselGear_UnloadServerRepository.ResetTotalUploadCounter();
                        VesselUnloadServerRepository.ResetTotalUploadCounter();
                        BatchUpload = true;
                        if (await ProcessJSONSNodes())
                        {
                            await SaveUploadedJsonInLoop(closeWindow: true, verbose: true, isHistoryJson: false, allowDownloadAgain: true);
                            if(CountJSONWithoutLandingData>0)
                            {
                                MessageBox.Show(
                                    "Not all JSON data from the server was downloaded. Pls refer to error log for details",
                                    Global.MessageBoxCaption,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information
                                    );
                            }
                            BatchUpload = false;
                        }
                    }
                    break;
                #endregion


                //File->upload downloaded JSON files
                //when you want to upload json created in batch mode
                case "menuUploadJson":
                    #region menuUploadJson
                    ClearJSONTreeRootNodes();
                    _formNameNode = null;
                    _rootChildrenHeadersHashSet.Clear();
                    jsonFolder = GetJSONFolder(savedHistory: false);
                    if (jsonFolder.Length > 0)
                    {
                        _jsonfiles = Directory.GetFiles(jsonFolder).Select(s => new FileInfo(s)).ToList();
                        if (_jsonfiles.Any())
                        {

                            TreeViewItem root = new TreeViewItem { Header = "Downloaded e-form data" };
                            treeViewJSONNavigator.Items.Add(root);
                            counter = 0;
                            foreach (var f in _jsonfiles.OrderByDescending(t => t.CreationTime))
                            {
                                if (f.Extension == ".xml")
                                {
                                    using (FileStream fs = new FileStream(f.FullName, FileMode.Open)) //double check that...
                                    {
                                        XmlSerializer _xSer = new XmlSerializer(typeof(DownloadedJsonMetadata));

                                        try
                                        {
                                            DownloadedJsonMetadata djmd = (DownloadedJsonMetadata)_xSer.Deserialize(fs);
                                            djmd.FileName = f.Name;

                                            AddMetadataToTreeView(djmd, root);
                                            counter++;
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.Log(ex);
                                        }
                                    }

                                }
                            }
                        }
                    }
                    ResetView();
                    if (_countJSONFiles > 0)
                    {
                        _targetGrid = gridJSONContent;
                    }
                    else
                    {
                        treeViewJSONNavigator.DataContext = null;
                        treeViewJSONNavigator.Items.Clear();
                        rowGrid.Height = new GridLength(1, GridUnitType.Star);
                        MessageBox.Show("No JSON files from Kobotoolbox server was found", "NSAP-ODK Database");
                        //hide UI elements related to displaying JSON
                        return;
                    }
                    rowJsonFiles.Height = new GridLength(1, GridUnitType.Star);
                    break;
                #endregion


                //File->delete past date
                case "menuDeletePastDate":
                    #region menuDeletePastDate
                    DeleteUnloadPastDateWindow dpw = new DeleteUnloadPastDateWindow();
                    dpw.ShowDialog();
                    break;
                #endregion


                //File->save JSON
                case "menuSaveJson":
                    #region menuSaveJson
                    //for saving the downloadd JSON text into a file.
                    if (await SaveJSONTextTask())
                    {
                        menuSaveJson.IsEnabled = false;
                        ((MainWindow)Owner).ShowSummary("Overall");
                    }

                    break;
                #endregion


                //File->Upload media
                case "menuUploadMedia":
                    #region menuUploadMedia
                    serverForm = new DownloadFromServerWindow(this);
                    VesselUnloadServerRepository.ResetLists();
                    serverForm.ServerIntent = ServerIntent.IntentUploadMedia;
                    serverForm.Owner = this;
                    serverForm.ShowDialog();
                    break;
                #endregion


                //File-use vessel counts json file
                //only available in debugger
                case "menuVesselCountJSON":
                    #region menuVesselCountJSON
                    try
                    {
                        var json = System.IO.File.ReadAllText(GetJsonTextFileFromFileOpenDialog());
                        if (json.Length > 0)
                        {
                            //LandingSiteBoatLandingsFromServerRepository.JSON = json;
                            //LandingSiteBoatLandingsFromServerRepository.CreateLandingSiteBoatLandingsFromJson();
                            BoatLandingsFromServerRepository.JSON = json;
                            BoatLandingsFromServerRepository.CreateBoatLandingsFromJson();
                            ODKServerDownload = ODKServerDownload.ServerDownloadLandings;
                            MainSheetsLanding = BoatLandingsFromServerRepository.BoatLandings;
                        }
                    }
                    catch
                    {
                        //ignore
                    }
                    break;
                #endregion

                //File->use vessel unload JSON file
                //get vessel unloads from json text file
                //only available in debugger

                case "menuVesselUnloadJSON":
                    #region menuVesselUnloadJSON
                    _jsonFileUseCreationDateForHistory = null;
                    try
                    {
                        FileInfo fi = new FileInfo(GetJsonTextFileFromFileOpenDialog());
                        string[] arr = Path.GetFileName(fi.FullName).Split(' ');

                        if (int.TryParse(arr[0], out int v))
                        {

                            var result = MessageBox.Show($"File was created on {fi.CreationTime.ToString("MMM-dd-yyyy HH:mm")}\r\n" +
                                                "Would you like to use this date on the download history?",
                                                "NSAP-ODK Database",
                                                MessageBoxButton.YesNoCancel,
                                                MessageBoxImage.Question);

                            if (result == MessageBoxResult.Cancel)
                            {
                                return;
                            }
                            else if (result == MessageBoxResult.Yes)
                            {
                                _jsonFileUseCreationDateForHistory = fi.CreationTime;
                            }

                            var json = System.IO.File.ReadAllText(fi.FullName);

                            if (json.Length > 0)
                            {
                                IsOptimizedMultiVessel = json.Contains("G_lss/sampling_date");
                                IsMultiVessel = json.Contains("repeat_landings_count");
                                if (IsMultiVessel)
                                {
                                    MultiVesselGear_UnloadServerRepository.JSON = json;
                                    MultiVesselGear_UnloadServerRepository.CreateLandingsFromJSON();
                                    MultiVesselMainSheets = MultiVesselGear_UnloadServerRepository.SampledVesselLandings;
                                    if (JSON == null)
                                    {
                                        JSONFileName = fi.FullName;
                                        JSON = json;
                                    }
                                }
                                else if (IsOptimizedMultiVessel)
                                {
                                    IsMultiVessel = true;
                                    MultiVessel_Optimized_UnloadServerRepository.JSON = json;
                                    MultiVessel_Optimized_UnloadServerRepository.CreateLandingsFromJSON();
                                    MultiVesselOptimizedMainSheets = MultiVessel_Optimized_UnloadServerRepository.SampledVesselLandings;
                                    if (JSON == null)
                                    {
                                        JSONFileName = fi.FullName;
                                        JSON = json;
                                    }
                                }
                                else
                                {
                                    VesselUnloadServerRepository.ResetLists();
                                    if (json.Contains("species_data_group"))
                                    {
                                        VesselUnloadServerRepository.JSON = VesselLandingFixDownload.JsonNewToOldVersion(json);
                                        //VesselUnloadServerRepository.JSON = JsonNewToOldVersion(json);
                                    }
                                    else
                                    {
                                        VesselUnloadServerRepository.JSON = json;
                                    }
                                    VesselUnloadServerRepository.CreateLandingsFromJSON();
                                    VesselUnloadServerRepository.FillDuplicatedLists();
                                    ODKServerDownload = ODKServerDownload.ServerDownloadVesselUnload;
                                    MainSheets = VesselUnloadServerRepository.VesselLandings;
                                    if (string.IsNullOrEmpty(JSON))
                                    {
                                        JSONFileName = fi.FullName;
                                        JSON = json;
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Selected file is not valid",
                                Global.MessageBoxCaption,
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }

                    break;
                #endregion


                case "menuSaveToExcel":
                    #region menuSaveToExcel
                    if (dataGridExcel.ItemsSource != null)
                    {
                        string filePath;
                        string exportResult;
                        if (ExportExcel.GetSaveAsExcelFileName(this, out filePath))
                        {
                            EntitiesToDataTables.VesselLandings = VesselUnloadServerRepository.VesselLandings;
                            if (ExportExcel.ExportDatasetToExcel(EntitiesToDataTables.GenerateDataSeFromImport(), filePath))
                            {
                                exportResult = "Successfully exported to excel";
                            }
                            else
                            {
                                exportResult = $"Was not successfull in exporting to excel\r\n{ExportExcel.ErrorMessage}";
                            }

                            MessageBox.Show(exportResult, "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    break;
                #endregion

                //File->Download from server
                case "menuDownloadFromServer":
                case "menuDownloadFromServerOtherUser":
                    #region menuDownloadFromServer
                    OpenServerWindow(logInAsOtherUser: menuName == "menuDownloadFromServerOtherUser");
                    break;
                #endregion

                //File clear tables
                //only in debugger
                case "menuClearTables":
                    #region menuClearTables
                    ClearTables();
                    break;
                #endregion

                //File->upload to database
                //Tree context menu ->upload (upload one json file belonging to a batch)
                //Tree context menu ->upload (upload one json file belonging to set of history files)
                case "menuUpload":
                case "menuUploadJsonFile":
                    #region menuUpload
                    await UploadToDatabase();
                    //if (MainSheets != null || MultiVesselMainSheets != null || MultiVesselOptimizedMainSheets != null)
                    //{
                    //    VesselUnloadServerRepository.CancelUpload = false;
                    //    MultiVesselGear_UnloadServerRepository.CancelUpload = false;
                    //    MultiVessel_Optimized_UnloadServerRepository.CancelUpload = false;
                    //    if (!Global.Settings.UsemySQL)
                    //    {
                    //        NSAPEntities.ClearCSVData();
                    //    }

                    //}
                    //string fileName = "";
                    //if (!string.IsNullOrEmpty(JSONFileName))
                    //{
                    //    fileName = JSONFileName;
                    //}
                    ////else if(_selectedJSONMetaData!=null && !BatchUpload)
                    ////{
                    ////    fileName = _selectedJSONMetaData.JSONFileInfo.FullName;
                    ////}

                    //if (IsOptimizedMultiVessel)
                    //{
                    //    MultiVessel_Optimized_UnloadServerRepository.ResetTotalUploadCounter();
                    //    MultiVessel_Optimized_UnloadServerRepository.ResetGroupIDs();
                    //}
                    //else if (IsMultiVessel)
                    //{
                    //    MultiVesselGear_UnloadServerRepository.ResetTotalUploadCounter();
                    //    MultiVesselGear_UnloadServerRepository.ResetGroupIDs();
                    //}
                    //else
                    //{
                    //    VesselUnloadServerRepository.ResetTotalUploadCounter();
                    //    VesselUnloadServerRepository.ResetGroupIDs();// VesselUnloadServerRepository.DelayedSave);
                    //}







                    //if (await Upload(jsonFullFileName: fileName))
                    //{
                    //    //the actual call to save the data contained in csv files is called in the call below
                    //    //await SaveUploadedJsonInLoop(verbose: true, allowDownloadAgain: true, isHistoryJson: menuName == "menuUpload");
                    //    await SaveUploadedJsonInLoop(verbose: true, allowDownloadAgain: true, isHistoryJson: false);
                    //    if (_listJSONNotUploaded?.Count > 0)
                    //    {

                    //    }
                    //    JSON = string.Empty;
                    //    _jsonFromServer = false;
                    //}


                    break;
                #endregion

                //File->import ODK excel
                case "menuImport":
                    #region menuImport
                    GetExcelFromFileOpen();
                    break;
                #endregion

                //File->close
                case "menuClose":
                    #region menuClose
                    Close();
                    break;
                    #endregion
            }
        }

        public async Task<bool> UploadToDatabase(bool fromUnmatchedJSON = false, int? loopCount = null)
        {
            bool success = false;
            if (MainSheets != null || MultiVesselMainSheets != null || MultiVesselOptimizedMainSheets != null)
            {
                VesselUnloadServerRepository.CancelUpload = false;
                MultiVesselGear_UnloadServerRepository.CancelUpload = false;
                MultiVessel_Optimized_UnloadServerRepository.CancelUpload = false;
                if (!Global.Settings.UsemySQL && !fromUnmatchedJSON)
                {
                    NSAPEntities.ClearCSVData();
                }

            }
            string fileName = "";
            if (!string.IsNullOrEmpty(JSONFileName))
            {
                fileName = JSONFileName;
            }
            //else if(_selectedJSONMetaData!=null && !BatchUpload)
            //{
            //    fileName = _selectedJSONMetaData.JSONFileInfo.FullName;
            //}
            if (!fromUnmatchedJSON || loopCount==0)
            {
                if (IsOptimizedMultiVessel)
                {
                    MultiVessel_Optimized_UnloadServerRepository.ResetTotalUploadCounter();
                    MultiVessel_Optimized_UnloadServerRepository.ResetGroupIDs();
                }
                else if (IsMultiVessel)
                {
                    MultiVesselGear_UnloadServerRepository.ResetTotalUploadCounter();
                    MultiVesselGear_UnloadServerRepository.ResetGroupIDs();
                }
                else
                {
                    VesselUnloadServerRepository.ResetTotalUploadCounter();
                    VesselUnloadServerRepository.ResetGroupIDs();// VesselUnloadServerRepository.DelayedSave);
                }
            }







            if (await Upload(jsonFullFileName: fileName))
            {
                //the actual call to save the data contained in csv files is called in the call below
                //await SaveUploadedJsonInLoop(verbose: true, allowDownloadAgain: true, isHistoryJson: menuName == "menuUpload");
                if (!fromUnmatchedJSON)
                {
                    success = await SaveUploadedJsonInLoop(verbose: true, allowDownloadAgain: true, isHistoryJson: false);
                }
                else
                {
                    success = true;
                }
                //if (_listJSONNotUploaded?.Count > 0)
                //{

                //}
                JSON = string.Empty;
                _jsonFromServer = false;
            }
            return success;
        }
        public bool IsMultiVessel { get; set; }
        public async Task<bool> SaveUploadedJsonInLoop(bool closeWindow = false, bool verbose = false, bool isHistoryJson = true, bool allowDownloadAgain = false)
        {
            bool success = false;
            //await ProcessJSONHistoryNodes((TreeViewItem)treeViewJSONNavigator.SelectedItem);
            if (VesselUnloadServerRepository.DelayedSave || MultiVesselGear_UnloadServerRepository.DelayedSave || MultiVessel_Optimized_UnloadServerRepository.DelayedSave)
            {
                bool proceed = false;
                if (VesselUnloadServerRepository.TotalUploadCount > 0 ||
                    MultiVesselGear_UnloadServerRepository.TotalUploadCount > 0 ||
                    MultiVessel_Optimized_UnloadServerRepository.TotalUploadCount > 0)
                {
                    if (Global.Settings.UsemySQL && await NSAPMysql.MySQLConnect.BulkUpdateMySQLTablesWithLandingSurveyDataAsync())
                    {
                        proceed = true;
                    }
                    else
                    {
                        if (await CreateTablesInAccess.UploadImportJsonResultAsync())
                        {
                            proceed = true;

                        }
                        else if (CreateTablesInAccess.UploadErrorMessage.Length > 0)
                        {
                            string uploadError = $"{CreateTablesInAccess.TableWithUploadError}\r\n\r\n{CreateTablesInAccess.UploadErrorMessage}";
                            MessageBox.Show(uploadError, "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                    }
                    if (proceed)
                    {
                        NSAPEntities.ClearCSVData();
                        VesselUnloadServerRepository.ResetTotalUploadCounter(uploadingDone: closeWindow);
                    }
                }

                //success if true if we have made this far wityout exceptions called earlier
                success = true;

                if (_updateMissingSubmission)
                {
                    verbose = false;
                }

                if (verbose)
                {
                    string msg = "Finished uploading downloaded JSON files to the database";
                    if (isHistoryJson)
                    {
                        msg = "Finished uploading JSON history files to the database";

                    }
                    if (allowDownloadAgain)
                    {
                        msg += "\r\n\r\nDo you want to upload again?";
                        MessageBoxResult r = MessageBox.Show(msg, "NSAP-ODK Database", MessageBoxButton.YesNo, MessageBoxImage.Information);
                        if (r == MessageBoxResult.No)
                        {
                            closeWindow = true;
                        }
                        else
                        {
                            closeWindow = false;
                        }
                    }
                    else
                    {
                        MessageBox.Show(msg, "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                if (closeWindow)
                {
                    Close();
                }

            }
            return success;
        }

        public void EnableLoginFromADifferentUser(bool enable = true)
        {
            if (enable)
            {
                menuDownloadFromServerOtherUser.Visibility = Visibility.Visible;
            }
            else
            {
                menuDownloadFromServerOtherUser.Visibility = Visibility.Collapsed;
            }
        }

        public void CSVFileDownloaded()
        {
            if (DownloadCSVFromServer && Owner.GetType().Name == "MainWindow")
            {
                //((EditWindowEx)Owner).Focus();
                DialogResult = true;
                Close();

            }
        }
        private void OpenServerWindow(bool refreshDBSummary = false, bool logInAsOtherUser = false)
        {
            var serverForm = new DownloadFromServerWindow(this);
            ResetView();

            rowGrid.Height = new GridLength(1, GridUnitType.Star);
            VesselUnloadServerRepository.ResetLists();
            serverForm.Owner = this;

            serverForm.RefreshDatabaseSummry = refreshDBSummary;
            serverForm.DownloadCSVFromServer = DownloadCSVFromServer;
            serverForm.LogInAsAnotherUser = logInAsOtherUser;
            serverForm.ShowDialog();
        }
        public async Task<bool> SetResolvedFishingGroundLandings(List<VesselLanding> resolvedLandings)
        {
            VesselUnloadServerRepository.UploadSubmissionToDB -= OnUploadSubmissionToDB;
            _resolvedFishingGroundLandings = resolvedLandings;
            return await ProcessResolvedLandings(_resolvedFishingGroundLandings);

        }
        private void ViewUnrecognizedFishingGrounds()
        {
            UnrecognizedFGsWindows urgw = new UnrecognizedFGsWindows();
            urgw.Owner = this;

            urgw.UnrecognizedFishingGrounds = new CollectedUnrecognizedFG
            {
                DateCreated = (DateTime)_unrecognizedFGDateCreated,
                UnrecognizedFishingGrounds = _unrecognizedFishingGrounds
            };

            urgw.ShowDialog();
            _unrecognizedFGAlredyViewed = true;
        }
        private bool ClearTables(bool verboseMode = true)
        {
            bool proceed = false;
            if (Global.Settings.UsemySQL)
            {
                proceed = NSAPMysql.MySQLConnect.DeleteDataFromTables(useScript: true);
            }
            else if (NSAPEntities.ClearNSAPDatabaseTables())
            {
                proceed = true;
                NSAPEntities.LandingSiteSamplingViewModel.Clear();
                NSAPEntities.SummaryItemViewModel.Clear();
            }


            if (proceed)
            {
                ((MainWindow)Owner).ShowDBSummary();
                if (verboseMode)
                {
                    MessageBox.Show("All repo cleared");
                }
            }
            else
            {
                MessageBox.Show("Clearting tables not successful. Operation will not proceed", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            return proceed;
        }
        private async Task ProcessXFormIDs(bool locateUnsavedFromServer = false)
        {
            if (!VesselUnloadServerRepository.CancelUpload)
            {
                int updateCount = await VesselUnloadServerRepository.ProcessXFormIDsAsync(locateUnsavedFromServer);
                _updateXFormIDCount += updateCount;
                if (updateCount > 0)
                {
                    TimedMessageBox.Show(
                        "Finished updating XFormIdentifier to database\r\n" +
                        $"from source having {VesselUnloadServerRepository.VesselLandings.Count} records with {updateCount} saved\r\n",
                        "NSAP-ODK Database",
                        5000,
                        System.Windows.Forms.MessageBoxButtons.OK);
                }
            }
        }

        public async Task Upload_unmatched_landings_JSON(string formID, ProgressDialogWindow pdw)
        {
            //var resultsWindow = (ODKResultsWindow)((DownloadFromServerWindow)Owner).Owner;
            FormID = formID;
            int savedCount = 0;
            //resultsWindow.BatchUpload = true;
            
            foreach (var json in SubmissionIdentifierPairing.UnmatchedLandingsJSON)
            {
                if(savedCount==0)
                {
                    NSAPEntities.ClearCSVData();
                }

                bool isMultiVessel = json.Contains("repeat_landings") || json.Contains("landing_site_sampling_group/are_there_landings");
                bool isOptimizedMultiVessel = json.Contains("G_lss/sampling_date");

                JSONFromServer(json, isMultiVessel, isOptimizedMultiVessel, updateMissingSubmission: true);

                if (isMultiVessel)
                {
                    MultiVesselGear_UnloadServerRepository.JSON = json;
                    MultiVesselGear_UnloadServerRepository.CreateLandingsFromSingleJson();
                    MultiVesselMainSheets = MultiVesselGear_UnloadServerRepository.SampledVesselLandings;
                }
                else if (isOptimizedMultiVessel)
                {
                    MultiVessel_Optimized_UnloadServerRepository.JSON = json;
                    MultiVessel_Optimized_UnloadServerRepository.CreateLandingsFromSingleJson();
                    MultiVesselOptimizedMainSheets = MultiVessel_Optimized_UnloadServerRepository.SampledVesselLandings;
                }
                else
                {
                    VesselUnloadServerRepository.JSON = json;
                    VesselUnloadServerRepository.CreateLandingsFromJSON();
                    MainSheets = VesselUnloadServerRepository.VesselLandings;
                }
                if (await UploadToDatabase(fromUnmatchedJSON: true, savedCount))
                {
                    savedCount++;
                    pdw.SavedUnmatchedJson(savedCount);
                }

            }
            await SaveUploadedJsonInLoop(verbose: true, allowDownloadAgain: true, isHistoryJson: false);


        }
        private async Task<bool> Upload(bool verbose = true, bool fromJSONBatchFiles = false, int loopCount = 0, string jsonFullFileName = "", bool fromHistoryFiles = false)
        {
            int? countVesselLandings = null;
            bool proceed = false;
            string msg = "";
            int sourceCount = 0;
            int savedCount = 0;
            _ufg_count = 0;
            _uploadToDBSuccess = false;
            bool success = false;
            //bool fromServer = false;
            labelProgress.Content = "";


            if (ODKServerDownload == ODKServerDownload.ServerDownloadVesselUnload)
            {
                if (IsMultiVessel || IsOptimizedMultiVessel)
                {
                    proceed = !MultiVesselGear_UnloadServerRepository.CancelUpload && !MultiVessel_Optimized_UnloadServerRepository.CancelUpload;
                }
                else
                {
                    proceed = !VesselUnloadServerRepository.CancelUpload;
                }

                if (proceed)
                {
                    if (_isJSONData)
                    {
                        if (_targetGrid.Items.Count > 0 || BatchUpload || _updateMissingSubmission)
                        {
                            _jsonFile = null;
                            if (fromJSONBatchFiles)
                            //if set of json files are from a batch download and not individiual history files
                            {
                                proceed = false;
                                if (IsOptimizedMultiVessel)
                                {
                                    MultiVessel_Optimized_UnloadServerRepository.JSONFileCreationTime = _jsonFileUseCreationDateForHistory;
                                    proceed = await MultiVessel_Optimized_UnloadServerRepository.UploadToDBAsync(jsonFileName: jsonFullFileName);
                                    savedCount = MultiVessel_Optimized_UnloadServerRepository.SampledVesselLandings.Count(t => t.SavedInLocalDatabase == true);
                                    countVesselLandings = MultiVessel_Optimized_UnloadServerRepository.SampledVesselLandings.Count;
                                }
                                else if (IsMultiVessel)
                                {

                                    MultiVesselGear_UnloadServerRepository.JSONFileCreationTime = _jsonFileUseCreationDateForHistory;
                                    proceed = await MultiVesselGear_UnloadServerRepository.UploadToDBAsync(jsonFileName: jsonFullFileName);
                                    savedCount = MultiVesselGear_UnloadServerRepository.SampledVesselLandings.Count(t => t.SavedInLocalDatabase == true);
                                    countVesselLandings = MultiVesselGear_UnloadServerRepository.SampledVesselLandings.Count;

                                }
                                else
                                {
                                    VesselUnloadServerRepository.JSONFileCreationTime = _jsonFileUseCreationDateForHistory;
                                    proceed = await VesselUnloadServerRepository.UploadToDBAsync(jsonFullFileName: jsonFullFileName, loopCount: loopCount);
                                    savedCount = VesselUnloadServerRepository.VesselLandings.Count(t => t.SavedInLocalDatabase == true);
                                    if (fromHistoryFiles)
                                    {
                                        countVesselLandings = VesselUnloadServerRepository.VesselLandings.Count;
                                    }
                                }
                                _uploadToDBSuccess = proceed;
                                if (!NSAPEntities.JSONFileViewModel.Exists(jsonFullFileName))
                                {
                                    _jsonFile = await CreateJsonFile(fileName: jsonFullFileName, fromHistoryFiles: fromHistoryFiles, countVesselLandings: countVesselLandings, delaySave: true);
                                }
                                success = _uploadToDBSuccess;

                            }
                            else if (BatchUpload)
                            {
                                if (!NSAPEntities.JSONFileViewModel.Exists(jsonFullFileName))
                                {
                                    if (IsOptimizedMultiVessel)
                                    {
                                        countVesselLandings = MultiVessel_Optimized_UnloadServerRepository.SampledVesselLandings.Count;
                                    }
                                    else if (IsMultiVessel)
                                    {
                                        countVesselLandings = MultiVesselGear_UnloadServerRepository.SampledVesselLandings.Count;
                                    }
                                    _jsonFile = await CreateJsonFile(fileName: jsonFullFileName, fromHistoryFiles: fromHistoryFiles, countVesselLandings: countVesselLandings, delaySave: true);
                                }
                            }
                            else
                            {
                                //if (string.IsNullOrEmpty(jsonFullFileName))
                                //{
                                //    fromServer = true;
                                //}
                                _jsonFile = await CreateJsonFile(fileName: jsonFullFileName, fromHistoryFiles: fromHistoryFiles, countVesselLandings: countVesselLandings, delaySave: true, fromServer: _jsonFromServer);
                            }

                            if (_jsonFile?.FileName.Length > 0)
                            {
                                var ks = NSAPEntities.KoboServerViewModel.GetKoboServer(int.Parse(_jsonFile.FormID));
                                if (ks != null)
                                {
                                    if (_jsonFile != null)
                                    {
                                        string jsonDescription = "NSAP Fish Catch Monitoring e-Form";
                                        _targetGrid.ItemsSource = null;

                                        if (!success)
                                        {
                                            if (IsMultiVessel || IsOptimizedMultiVessel)
                                            {
                                                if (IsOptimizedMultiVessel)
                                                {
                                                    if (await MultiVessel_Optimized_UnloadServerRepository.UploadToDBAsync(_jsonFile.FileName))
                                                    {
                                                        //_targetGrid.ItemsSource = MultiVesselGear_UnloadServerRepository.SampledVesselLandings;
                                                        sourceCount = MultiVessel_Optimized_UnloadServerRepository.SampledVesselLandings.Count;
                                                        savedCount = MultiVessel_Optimized_UnloadServerRepository.SampledVesselLandings.Count(t => t.SavedInLocalDatabase == true);
                                                        savedCount += MultiVessel_Optimized_UnloadServerRepository.FishCarriers.Count(t => t.SavedInLocalDatabase == true);
                                                    }
                                                }
                                                else
                                                {
                                                    if (await MultiVesselGear_UnloadServerRepository.UploadToDBAsync(_jsonFile.FileName))
                                                    {
                                                        //_targetGrid.ItemsSource = MultiVesselGear_UnloadServerRepository.SampledVesselLandings;
                                                        sourceCount = MultiVesselGear_UnloadServerRepository.SampledVesselLandings.Count;
                                                        savedCount = MultiVesselGear_UnloadServerRepository.SampledVesselLandings.Count(t => t.SavedInLocalDatabase == true);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (await VesselUnloadServerRepository.UploadToDBAsync(_jsonFile.FileName, loopCount: loopCount))
                                                {
                                                    //_targetGrid.ItemsSource = VesselUnloadServerRepository.VesselLandings;
                                                    sourceCount = VesselUnloadServerRepository.VesselLandings.Count;
                                                    savedCount = VesselUnloadServerRepository.VesselLandings.Count(t => t.SavedInLocalDatabase == true);
                                                }
                                            }
                                        }

                                        if (savedCount > 0)
                                        {
                                            await NSAPEntities.JSONFileViewModel.Save(_jsonFile, _jsonFromServer);
                                        }


                                        success = true;
                                        _uploadToDBSuccess = true;

                                        if (JSON != null && (_jsonfiles == null || _jsonfiles.Count == 0) && await SaveJSONTextTask(verbose: false))
                                        {
                                            if (IsMultiVessel)
                                            {

                                            }
                                            else if (IsOptimizedMultiVessel)
                                            {

                                            }
                                            else
                                            {
                                                if (AnalyzeJsonForMismatch.Analyze(VesselUnloadServerRepository.VesselLandings, _jsonFile))
                                                {

                                                }
                                            }
                                            msg = "Finished uploading to database\r\n" +
                                                   $"from source having {sourceCount} records with {savedCount} saved\r\n" +
                                                   $"and saving JSON file to {Global.Settings.JSONFolder}";


                                        }
                                        else if (JSON != null && _selectedJSONMetaData != null)
                                        {
                                            if (_selectedJSONMetaData.JSONFile == null)
                                            {
                                                if (_jsonFile != null)
                                                {
                                                    _selectedJSONMetaData.JSONFile = _jsonFile;
                                                }
                                                else
                                                {
                                                    _selectedJSONMetaData.JSONFile = await CreateJsonFile(jsonDescription, _selectedJSONMetaData.JSONFileInfo.FullName);
                                                    if (_selectedJSONMetaData.JSONFile != null && AnalyzeJsonForMismatch.Analyze(VesselUnloadServerRepository.VesselLandings, _selectedJSONMetaData.JSONFile))
                                                    {

                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {

                                            msg = "Finished uploading JSON file to database\r\n" +
                                                $"from source having {sourceCount} records with {savedCount} saved\r\n";

                                        }
                                    }
                                    else if (_savedCount == 0 && VesselUnloadServerRepository.VesselLandings.Count > 0)
                                    {
                                        msg = "All records already saved to the database";
                                    }


                                    if (_ufg_count > 0)
                                    {
                                        msg += $"\r\n\r\nThere were {_ufg_count} landings with unrecognized fishing grounds";
                                    }

                                    if (!VesselUnloadServerRepository.DelayedSave && !MultiVesselGear_UnloadServerRepository.DelayedSave && !MultiVessel_Optimized_UnloadServerRepository.DelayedSave)
                                    {
                                        TimedMessageBox.Show(msg, "NSAP-ODK Database", 5000);
                                    }

                                    if (success)
                                    {
                                        ((MainWindow)Owner).SetDataDisplayMode();
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("You need to log-in to the Kobotoolbox to refresh server information before you can upload the data", Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        else
                        {
                            if (_lowMemoryReadingJSON)
                            {
                                _lowMemoryReadingJSON = false;
                            }
                            else
                            {
                                msg = "You do not have any downloaded data";
                                TimedMessageBox.Show(
                                    msg,
                                    "NSAP-ODK Database",
                                    5000);
                            }
                        }

                    }
                    else
                    {

                        if (dataGridExcel.Items.Count == 0)
                        {
                            if (_lowMemoryReadingJSON)
                            {
                                _lowMemoryReadingJSON = false;
                            }
                            else
                            {
                                MessageBox.Show("You do not have any downloaded data", "No data", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        else
                        {
                            if (await ImportExcel.UploadToDatabaseAsync())
                            {
                                dataGridExcel.ItemsSource = null;
                                dataGridExcel.ItemsSource = ImportExcel.ExcelMainSheets;
                                success = true;
                                _uploadToDBSuccess = true;
                                MessageBox.Show("Finished uploading to database", "Upload done", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else if (_savedCount == 0)
                            {
                                MessageBox.Show("Zero records were saved because all have been saved earlier");
                            }
                        }
                    }
                }

            }
            else if (ODKServerDownload == ODKServerDownload.ServerDownloadLandings)
            {
                if (_isJSONData)
                {
                    if (await BoatLandingsFromServerRepository.UploadToDBAsync())
                    {
                        _targetGrid.ItemsSource = null;
                        _targetGrid.ItemsSource = BoatLandingsFromServerRepository.BoatLandings;
                        _uploadToDBSuccess = true;
                        success = true;

                    }
                }
            }
            if (success && !BatchUpload)
            {
                ParentWindow?.RefreshDownloadHistory();
            }

            return success;
        }
        private string VersionFromJSON(string json)
        {
            string versionNumber = "";
            if (json.Contains("Version "))
            {
                int index = json.IndexOf("Version ");
                versionNumber = json.Substring(index + 8, 4).Trim();
            }
            return versionNumber;
        }

        public List<UnrecognizedFishingGround> UnrecognizedFishingGrounds
        {
            get { return _unrecognizedFishingGrounds.OrderBy(t => t.SamplingDate).ToList(); }
        }

        private void OnUploadSubmissionToDBMultiVessel(object sender, UploadToDbEventArg e)
        {
            switch (e.Intent)
            {
                case UploadToDBIntent.EndOfUpload:
                    labelProgress.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              labelProgress.Content = $"Uploading done. Number of landing days processed: {e.LandingSiteSamplingProcessedCount}";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case UploadToDBIntent.LandingSiteSamplingProcessed:
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              progressBar.Value = e.LandingSiteSamplingProcessedCount;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);
                    break;
                case UploadToDBIntent.VesselUnloadSaved:
                    labelProgress.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              labelProgress.Content = $"Number of landings processed: {e.VesselUnloadSavedCount}";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case UploadToDBIntent.StartOfUpload:
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressBar.IsIndeterminate = false;
                              progressBar.Maximum = e.LandingSiteSamplingCount;

                              //do what you need to do on UI Thread
                              return null;
                          }), null);
                    break;
                case UploadToDBIntent.UpdateFound:
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                            {
                                progressBar.IsIndeterminate = false;
                                //do what you need to do on UI Thread
                                return null;
                            }
                         ), null);
                    break;
                case UploadToDBIntent.Searching:

                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              progressBar.IsIndeterminate = true;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    labelProgress.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              labelProgress.Content = "Searching...";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
            }
        }
        private void OnUploadSubmissionToDB(object sender, UploadToDbEventArg e)
        {
            switch (e.Intent)
            {
                case UploadToDBIntent.Searching:

                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              progressBar.IsIndeterminate = true;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    labelProgress.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              labelProgress.Content = "Searching...";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case UploadToDBIntent.UnloadFound:
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              progressBar.Value = e.VesselUnloadFoundCount;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    labelProgress.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              labelProgress.Content = "Please wait...";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case UploadToDBIntent.SearchingUpdates:
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              progressBar.IsIndeterminate = true;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    labelProgress.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              labelProgress.Content = "Please wait...";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case UploadToDBIntent.Cancelled:
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressBar.Value = e.VesselUnloadTotalSavedCount;
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    labelProgress.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              labelProgress.Content = $"Uploading was cancelled with {e.VesselUnloadTotalSavedCount} submissions";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    _savedCount = e.VesselUnloadTotalSavedCount;
                    break;
                case UploadToDBIntent.EndOfUpload:
                case UploadToDBIntent.EndOfUpdate:
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressBar.IsIndeterminate = false;
                              progressBar.Value = 0;
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    labelProgress.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              if (e.Intent == UploadToDBIntent.EndOfUpload)
                              {
                                  labelProgress.Content = $"Finished uploading {e.VesselUnloadTotalSavedCount} submissions";
                              }
                              else
                              {
                                  labelProgress.Content = $"Finished updating {e.VesselUnloadUpdatedCount} submissions";
                              }
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    _savedCount = e.VesselUnloadTotalSavedCount;


                    if (VesselUnloadServerRepository.UnrecognizedFishingGrounds?.Count > 0)
                    {
                        _ufg_count = VesselUnloadServerRepository.UnrecognizedFishingGrounds.Count;
                        _unrecognizedFishingGrounds.AddRange(VesselUnloadServerRepository.UnrecognizedFishingGrounds);
                        //SetUpHashSet(VesselUnloadServerRepository.UnrecognizedFishingGrounds);

                        if (_unrecognizedFGDateCreated == null)
                        {
                            _unrecognizedFGDateCreated = DateTime.Now;
                            menuUnrecognizedFG.Dispatcher.BeginInvoke
                                (
                                DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                                {
                                    menuUnrecognizedFG.Visibility = Visibility.Visible;
                                    return null;
                                }
                                ), null);
                        }
                        VesselUnloadServerRepository.ClearUnrecgnizedFishingGroundsList();
                    }
                    break;

                case UploadToDBIntent.StartOfUpload:
                case UploadToDBIntent.StartOfUpdate:
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressBar.IsIndeterminate = false;
                              if (e.Intent == UploadToDBIntent.StartOfUpload)
                              {
                                  progressBar.Maximum = e.VesselUnloadToSaveCount;
                              }
                              else
                              {
                                  progressBar.Maximum = e.VesselUnloadToUpdateCount;
                              }
                              //do what you need to do on UI Thread
                              return null;
                          }), null);
                    break;
                case UploadToDBIntent.UpdateFound:
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                            {
                                progressBar.IsIndeterminate = false;
                                //do what you need to do on UI Thread
                                return null;
                            }
                         ), null);
                    break;
                case UploadToDBIntent.Updating:
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                            {

                                progressBar.Value = e.VesselUnloadFoundCount;

                                //do what you need to do on UI Thread
                                return null;
                            }
                         ), null);

                    labelProgress.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              labelProgress.Content = $"Updating {(int)progressBar.Value} of {(int)progressBar.Maximum} submissions";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    break;
                case UploadToDBIntent.Uploading:
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                            {

                                progressBar.Value = e.VesselUnloadSavedCount;

                                //do what you need to do on UI Thread
                                return null;
                            }
                         ), null);

                    labelProgress.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              labelProgress.Content = $"Uploading {(int)progressBar.Value} of {(int)progressBar.Maximum} submissions";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    break;

            }
        }
        public HashSet<UnrecognizedFishingGround> UnrecognizedFishingGroundsHashSet { get; private set; }
        private void SetUpHashSet(List<UnrecognizedFishingGround> ufgs)
        {
            if (UnrecognizedFishingGroundsHashSet == null)
            {
                UnrecognizedFishingGroundsHashSet = new HashSet<UnrecognizedFishingGround>(new UnrecognizedFishingGroundComparer());
            }
            foreach (var item in ufgs.OrderBy(t => t.RowID))
            {
                UnrecognizedFishingGroundsHashSet.Add(item);
            }
        }

        private Style AlignRightStyle
        {
            get
            {
                Style alignRightCellStype = new Style(typeof(DataGridCell));

                // Create a Setter object to set (get it? Setter) horizontal alignment.
                Setter setAlign = new
                    Setter(HorizontalAlignmentProperty,
                    HorizontalAlignment.Right);

                // Bind the Setter object above to the Style object
                alignRightCellStype.Setters.Add(setAlign);
                return alignRightCellStype;
            }
        }

        /// <summary>
        /// Shows the result of the downloaded JSON from the server as a datagrid
        /// </summary>
        /// <param name="result"></param>
        private void ShowResultFromAPI(string result)
        {
            DataGridTextColumn col;
            _targetGrid.ItemsSource = null;
            _targetGrid.Columns.Clear();
            _targetGrid.IsReadOnly = true;
            _targetGrid.AutoGenerateColumns = false;
            switch (result)
            {
                case "landingSiteSampling":
                    _targetGrid.IsReadOnly = true;
                    _targetGrid.ItemsSource = BoatLandingsFromServerRepository.BoatLandings;
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "RowUUID", Binding = new Binding("_uuid"), Visibility = Visibility.Hidden });

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("_submission_time"),
                        Header = "Date and time submitted",
                        CellStyle = AlignRightStyle
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    _targetGrid.Columns.Add(col);

                    _targetGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Saved to database", Binding = new Binding("SavedInLocalDatabase") });
                    _targetGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Saved boat counts and TWSP", Binding = new Binding("IsUpdatedForBoatLandings") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Device ID", Binding = new Binding("device_id") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("user_name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Form version", Binding = new Binding("eFormVersion") });

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("SamplingDate"),
                        Header = "Sampling date",
                        CellStyle = AlignRightStyle
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy";
                    _targetGrid.Columns.Add(col);

                    _targetGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Sampling day", Binding = new Binding("IsSamplingDay") });
                    _targetGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Has fishing operation", Binding = new Binding("HasFishingOperation") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "NSAP Region", Binding = new Binding("Region") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("EnumeratorName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("FMA") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("FishingGround") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site ID", Binding = new Binding("LandingSiteId") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("LandingSiteName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Notes", Binding = new Binding("NotesRemarks") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("PK") });

                    break;

                case "landingSiteCounts":
                    _targetGrid.ItemsSource = BoatLandingsFromServerRepository.GetGears();
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("Parent.user_name") });

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("Parent.SamplingDate"),
                        Header = "Sampling date",
                        CellStyle = AlignRightStyle
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy";
                    _targetGrid.Columns.Add(col);

                    _targetGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Sampling day", Binding = new Binding("Parent.IsSamplingDay") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "NSAP Region", Binding = new Binding("Parent.NsapRegion") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("Parent.EnumeratorName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("Parent.FMA") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("Parent.FishingGround") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("Parent.LandingSiteName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("GearName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("SectorFull") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Vessels landed", Binding = new Binding("LandingsCount"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Total catch weight", Binding = new Binding("TotalCatchWt"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Notes", Binding = new Binding("Note") });
                    //_targetGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Saved to database", Binding = new Binding("SavedInLocalDatabase") });
                    //_targetGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("LandingSiteName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Parent", Binding = new Binding("Parent.PK"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("PK"), CellStyle = AlignRightStyle });
                    break;
                case "twsp":
                    _targetGrid.ItemsSource = BoatLandingsFromServerRepository.GetSpeciesTTWSpRepeats();
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("Parent.Parent.user_name") });

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("Parent.Parent.SamplingDate"),
                        Header = "Sampling date",
                        CellStyle = AlignRightStyle
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy";
                    _targetGrid.Columns.Add(col);

                    _targetGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Sampling day", Binding = new Binding("Parent.Parent.IsSamplingDay") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "NSAP Region", Binding = new Binding("Parent.Parent.NsapRegion") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("Parent.Parent.EnumeratorName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("Parent.Parent.FMA") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("Parent.Parent.FishingGround") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("Parent.Parent.LandingSiteName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("Parent.GearName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Vessels landed", Binding = new Binding("Parent.LandingsCount"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Total catch weight", Binding = new Binding("Parent.TotalCatchWt"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Name of catch", Binding = new Binding("SpeciesNameSelected") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "TWSp", Binding = new Binding("Twsp"), CellStyle = AlignRightStyle });
                    //_targetGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Saved to database", Binding = new Binding("SavedInLocalDatabase") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Parent", Binding = new Binding("Parent.PK"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("PK"), CellStyle = AlignRightStyle });
                    break;
                case "effort_multiVessel":
                    _targetGrid.IsReadOnly = true;

                    //_targetGrid.AutoGenerateColumns = true;
                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("Parent.SubmissionTime"),
                        Header = "Date and time submitted",
                        CellStyle = AlignRightStyle
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    _targetGrid.Columns.Add(col);

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("SamplingDate"),
                        Header = "Sampling date",
                        CellStyle = AlignRightStyle
                    };

                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    _targetGrid.Columns.Add(col);
                    _targetGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Saved to database", Binding = new Binding("SavedInLocalDatabase") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("Parent.user_name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Form version", Binding = new Binding("Parent.FormVersion") });

                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "NSAP Region", Binding = new Binding("Parent.NSAPRegion.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Enumerator ID", Binding = new Binding("Parent.RegionEnumeratorID") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("Parent.EnumeratorName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("Parent.FMA.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("Parent.FishingGround.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site ID", Binding = new Binding("Parent.LandingSiteID") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("Parent.LandingSiteName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Number of gears used", Binding = new Binding("NumberOfGearsUsedInSampledLanding") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Names of gears used", Binding = new Binding("NamesOfGearsUsed") });
                    _targetGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Is sampling day", Binding = new Binding("Parent.IsSamplingDay") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing gear code", Binding = new Binding("Main_gear_code") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing gear", Binding = new Binding("Main_gear_name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Ref#", Binding = new Binding("Reference_number") });
                    _targetGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Fishing boat is used", Binding = new Binding("IsBoatUsedInLanding") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel ID", Binding = new Binding("BoatUsedID") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("Boat_name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("SectorOfLanding") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Number of fishers", Binding = new Binding("NumberOfFishers"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Successful trip", Binding = new Binding("TripIsSuccess") });
                    _targetGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Trip is completed", Binding = new Binding("TripIsCompleted") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Notes", Binding = new Binding("Remarks"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Weight of catch", Binding = new Binding("CatchTotal"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Weight of sample", Binding = new Binding("CatchSampled"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Number of boxes", Binding = new Binding("Boxes_total"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Number of boxes sampled", Binding = new Binding("Boxes_sampled"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Raising factor", Binding = new Binding("RaisingFactor"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Includes catch composition", Binding = new Binding("IncludeCatchComp") });

                    if (MultiVesselMainSheets != null)
                    {
                        _targetGrid.ItemsSource = MultiVesselMainSheets;
                    }
                    else if (MultiVesselOptimizedMainSheets != null)
                    {
                        _targetGrid.ItemsSource = MultiVesselOptimizedMainSheets;
                    }
                    else
                    {
                        _targetGrid.ItemsSource = MultiVesselGear_UnloadServerRepository.SampledVesselLandings;

                    }
                    break;
                case "effort":
                    _targetGrid.ItemsSource = VesselUnloadServerRepository.VesselLandings;
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "RowUUID", Binding = new Binding("_uuid"), Visibility = Visibility.Hidden });

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("_submission_time"),
                        Header = "Date and time submitted",
                        CellStyle = AlignRightStyle
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    _targetGrid.Columns.Add(col);

                    _targetGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Saved to database", Binding = new Binding("SavedInLocalDatabase") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Device ID", Binding = new Binding("device_id") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("user_name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Form version", Binding = new Binding("intronote") });

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("SamplingDate"),
                        Header = "Sampling date",
                        CellStyle = AlignRightStyle
                    };

                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    _targetGrid.Columns.Add(col);

                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "NSAP Region", Binding = new Binding("NSAPRegion.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Enumerator ID", Binding = new Binding("RegionEnumeratorID") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("EnumeratorName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("NSAPRegionFMA.FMA.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("FishingGround.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site ID", Binding = new Binding("LandingSiteID") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("LandingSiteName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing gear code", Binding = new Binding("GearCode") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing gear", Binding = new Binding("GearNameToUse") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Ref#", Binding = new Binding("ref_no") });
                    _targetGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Fishing boat is used", Binding = new Binding("IsBoatUsed") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel ID", Binding = new Binding("BoatUsed") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("FishingVesselName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("Sector") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Number of fishers", Binding = new Binding("NumberOfFishers"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Successful trip", Binding = new Binding("TripIsSuccess") });
                    _targetGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Trip is completed", Binding = new Binding("TripIsCompleted") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Weight of catch", Binding = new Binding("CatchTotalWt"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Weight of sample", Binding = new Binding("CatchSampleWt"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Number of boxes", Binding = new Binding("BoxesTotal"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Number of boxes sampled", Binding = new Binding("BoxesSampled"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Raising factor", Binding = new Binding("RaisingFactor"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Includes catch composition", Binding = new Binding("IncludeCatchComposition") });

                    _targetGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Vessel tracking", Binding = new Binding("IncludeTracking") });

                    col = new DataGridTextColumn()
                    {
                        //Binding = new Binding("TimeDepartLandingSite"),
                        Binding = new Binding("DateTimeDepartLandingSite"),
                        Header = "Departure from landing site",
                        CellStyle = AlignRightStyle
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    _targetGrid.Columns.Add(col);

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("DateTimeArriveLandingSite"),
                        Header = "Arrival at landing site",
                        CellStyle = AlignRightStyle
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    _targetGrid.Columns.Add(col);

                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "GPS", Binding = new Binding("GPS.AssignedName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Remarks", Binding = new Binding("Remarks") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("PK") });
                    break;

                case "grid":
                    _targetGrid.ItemsSource = VesselUnloadServerRepository.GetGridBingoCoordinates();

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("Parent.SamplingDate"),
                        Header = "Date and time sampled",
                        CellStyle = AlignRightStyle
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    _targetGrid.Columns.Add(col);

                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("Parent.user_name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "NSAP region", Binding = new Binding("Parent.NSAPRegion.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("Parent.NSAPRegionFMA.FMA.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("Parent.FishingGround.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("Parent.LandingSiteName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("Parent.FishingVesselName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("Parent.Sector") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing gear", Binding = new Binding("Parent.GearName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "UTM zone", Binding = new Binding("Parent.UTMZone") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Bingo coordinate", Binding = new Binding("CompleteGridName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Coordinates", Binding = new Binding("Grid25Cell.Coordinate") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "UTM Coordinates", Binding = new Binding("Grid25Cell.UTMCoordinate") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("PK") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Parent ID", Binding = new Binding("Parent.PK") });
                    break;

                case "soakTime":
                    _targetGrid.ItemsSource = VesselUnloadServerRepository.GetGearSoakTimes();

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("Parent.SamplingDate"),
                        Header = "Date and time sampled",
                        CellStyle = AlignRightStyle
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    _targetGrid.Columns.Add(col);

                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("Parent.user_name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "NSAP region", Binding = new Binding("Parent.NSAPRegion.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("Parent.NSAPRegionFMA.FMA.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("Parent.FishingGround.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("Parent.LandingSiteName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("Parent.FishingVesselName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("Parent.Sector") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing gear", Binding = new Binding("Parent.GearName") });

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("SetTime"),
                        Header = "Date and time of gear set".Length,
                        CellStyle = AlignRightStyle
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    _targetGrid.Columns.Add(col);

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("HaulTime"),
                        Header = "Date and time of gear haul"
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    _targetGrid.Columns.Add(col);

                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Waypoint at set", Binding = new Binding("WaypointAtSet") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Waypoint at haul", Binding = new Binding("WaypointAtHaul") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("PK") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Parent ID", Binding = new Binding("Parent.PK") });

                    break;

                case "effortSpecs":
                case "duplicatedEffort":
                    if (result == "effortSpecs")
                    {
                        _targetGrid.ItemsSource = VesselUnloadServerRepository.GetGearEfforts();
                    }
                    else
                    {
                        _targetGrid.ItemsSource = VesselUnloadServerRepository.DuplicatedEffortSpec;
                    }

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("Parent.SamplingDate"),
                        Header = "Date and time sampled"
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    _targetGrid.Columns.Add(col);

                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("Parent.user_name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "NSAP region", Binding = new Binding("Parent.NSAPRegion.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("Parent.NSAPRegionFMA.FMA.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("Parent.FishingGround.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("Parent.LandingSiteName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("Parent.FishingVesselName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("Parent.Sector") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing gear", Binding = new Binding("Parent.GearName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Effort specification", Binding = new Binding("EffortSpecName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Value", Binding = new Binding("SelectedEffortMeasure") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("PK") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Parent ID", Binding = new Binding("Parent.PK") });

                    break;

                case "catchComposition":
                case "duplicatedCatchComp":
                    if (result == "catchComposition")
                    {
                        _targetGrid.ItemsSource = VesselUnloadServerRepository.GetCatchCompositions();
                    }
                    else
                    {
                        _targetGrid.ItemsSource = VesselUnloadServerRepository.DuplicatedCatchComposition;
                    }

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("Parent.SamplingDate"),
                        Header = "Date and time sampled"
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    _targetGrid.Columns.Add(col);

                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("Parent.user_name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "NSAP region", Binding = new Binding("Parent.NSAPRegion.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("Parent.NSAPRegionFMA.FMA.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("Parent.FishingGround.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("Parent.LandingSiteName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("Parent.FishingVesselName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("Parent.Sector") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing gear", Binding = new Binding("Parent.GearName") });

                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Taxa", Binding = new Binding("Taxa.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Species ID", Binding = new Binding("SpeciesID") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Name of catch", Binding = new Binding("SpeciesNameSelected") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Weight", Binding = new Binding("SpeciesWt"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Sample weight", Binding = new Binding("SpeciesSampleWt"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("PK") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Parent ID", Binding = new Binding("Parent.PK") });
                    break;

                case "lengths":
                    _targetGrid.ItemsSource = VesselUnloadServerRepository.GetLengthList();

                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("Parent.Parent.user_name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "NSAP region", Binding = new Binding("Parent.Parent.NSAPRegion.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("Parent.Parent.NSAPRegionFMA.FMA.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("Parent.Parent.FishingGround.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("Parent.Parent.LandingSiteName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("Parent.Parent.FishingVesselName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("Parent.Parent.Sector") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing gear", Binding = new Binding("Parent.Parent.GearName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Taxa", Binding = new Binding("Parent.Taxa.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Name of catch", Binding = new Binding("Parent.SpeciesNameSelected") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Length", Binding = new Binding("Length"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("PK") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Parent ID", Binding = new Binding("Parent.PK") });
                    break;

                case "lengthWeight":
                    _targetGrid.ItemsSource = VesselUnloadServerRepository.GetLenWtList();

                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("Parent.Parent.user_name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "NSAP region", Binding = new Binding("Parent.Parent.NSAPRegion.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("Parent.Parent.NSAPRegionFMA.FMA.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("Parent.Parent.FishingGround.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("Parent.Parent.LandingSiteName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("Parent.Parent.FishingVesselName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("Parent.Parent.Sector") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing gear", Binding = new Binding("Parent.Parent.GearName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Taxa", Binding = new Binding("Parent.Taxa.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Name of catch", Binding = new Binding("Parent.SpeciesNameSelected") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Length", Binding = new Binding("Length"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Weight", Binding = new Binding("Weight"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("PK") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Parent ID", Binding = new Binding("Parent.PK") });
                    break;

                case "lengthFreq":
                case "duplicatedLenFreq":
                    if (result == "lengthFreq")
                    {
                        _targetGrid.ItemsSource = VesselUnloadServerRepository.GetLenFreqList();
                    }
                    else
                    {
                        _targetGrid.ItemsSource = VesselUnloadServerRepository.DuplicatedLenFreq;
                    }

                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("Parent.Parent.user_name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "NSAP region", Binding = new Binding("Parent.Parent.NSAPRegion.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("Parent.Parent.NSAPRegionFMA.FMA.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("Parent.Parent.FishingGround.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("Parent.Parent.LandingSiteName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("Parent.Parent.FishingVesselName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("Parent.Parent.Sector") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing gear", Binding = new Binding("Parent.Parent.GearName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Taxa", Binding = new Binding("Parent.Taxa.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Name of catch", Binding = new Binding("Parent.SpeciesNameSelected") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Length class", Binding = new Binding("LengthClass"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Frequency", Binding = new Binding("Frequency"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("PK") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Parent ID", Binding = new Binding("Parent.PK") });
                    break;

                case "gms":
                    _targetGrid.ItemsSource = VesselUnloadServerRepository.GetGMSList();

                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("Parent.Parent.user_name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "NSAP region", Binding = new Binding("Parent.Parent.NSAPRegion.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("Parent.Parent.NSAPRegionFMA.FMA.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("Parent.Parent.FishingGround.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("Parent.Parent.LandingSiteName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("Parent.Parent.FishingVesselName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("Parent.Parent.Sector") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing gear", Binding = new Binding("Parent.Parent.GearName") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Taxa", Binding = new Binding("Parent.Taxa.Name") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Name of catch", Binding = new Binding("Parent.SpeciesNameSelected") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Length", Binding = new Binding("Length"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Weight", Binding = new Binding("Weight"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Sex", Binding = new Binding("Sex") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Gonad maturity", Binding = new Binding("GMS") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Gonad weight", Binding = new Binding("GonadWeight"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Stomach content weight", Binding = new Binding("StomachContentWt"), CellStyle = AlignRightStyle });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Stomach content category", Binding = new Binding("GutContentCategory") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("PK") });
                    _targetGrid.Columns.Add(new DataGridTextColumn { Header = "Parent ID", Binding = new Binding("Parent.PK") });
                    break;
            }
        }

        private void ShowResultFromExcel(string result)
        {
            DataGridTextColumn col;
            dataGridExcel.ItemsSource = null;
            dataGridExcel.Columns.Clear();
            switch (result)
            {
                case "effort":
                    dataGridExcel.ItemsSource = ImportExcel.ExcelMainSheets;

                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "RowUUID", Binding = new Binding("RowUUID"), Visibility = Visibility.Hidden });

                    //dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Date and time submitted", Binding = new Binding("DateTimeSubmitted") });
                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("DateTimeSubmitted"),
                        Header = "Date and time submitted"
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    dataGridExcel.Columns.Add(col);

                    dataGridExcel.Columns.Add(new DataGridCheckBoxColumn { Header = "Saved to database", Binding = new Binding("IsSaved") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Device ID", Binding = new Binding("DeviceId") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("UserName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Form version", Binding = new Binding("FormVersion") });

                    //dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Sampling date", Binding = new Binding("SamplingDate") });
                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("SamplingDate"),
                        Header = "Sampling date"
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    dataGridExcel.Columns.Add(col);

                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "NSAP Region", Binding = new Binding("NSAPRegion.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("EnumeratorName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("NSAPRegionFMA.FMA.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("NSAPRegionFMAFishingGround.FishingGround.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("LandingSiteName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing gear", Binding = new Binding("GearName") });
                    dataGridExcel.Columns.Add(new DataGridCheckBoxColumn { Header = "Fishing boat is used", Binding = new Binding("IsBoatUsed") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("VesselName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("Sector") });
                    dataGridExcel.Columns.Add(new DataGridCheckBoxColumn { Header = "Successful trip", Binding = new Binding("TripIsSuccess") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Weight of catch", Binding = new Binding("CatchWeightTotal") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Weight of sample", Binding = new Binding("CatchWeightSampled") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Number of boxes", Binding = new Binding("BoxesTotal") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Number of boxes sampled", Binding = new Binding("BoxesSampled") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Raising factor", Binding = new Binding("RaisingFactor") });
                    dataGridExcel.Columns.Add(new DataGridCheckBoxColumn { Header = "Vessel tracking", Binding = new Binding("TripIsTracked") });

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("DateTimeDepartLandingSite"),
                        Header = "Departure from landing site"
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    dataGridExcel.Columns.Add(col);

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("DateTimeArriveLandingSite"),
                        Header = "Arrival at landing site"
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    dataGridExcel.Columns.Add(col);

                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "GPS", Binding = new Binding("GPS.AssignedName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Remarks", Binding = new Binding("Remarks") });

                    break;

                case "grid":
                    dataGridExcel.ItemsSource = ImportExcel.ExcelBingoGroups;

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("Parent.SamplingDate"),
                        Header = "Date and time sampled"
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    dataGridExcel.Columns.Add(col);

                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("Parent.UserName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "NSAP region", Binding = new Binding("Parent.NSAPRegion.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("Parent.NSAPRegionFMA.FMA.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("Parent.NSAPRegionFMAFishingGround.FishingGround.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("Parent.LandingSiteName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("Parent.VesselName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("Parent.Sector") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing gear", Binding = new Binding("Parent.GearName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "UTM zone", Binding = new Binding("Parent.UTMZone") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Bingo coordinate", Binding = new Binding("BingoCoordinate") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Coordinates", Binding = new Binding("Grid25Grid.Coordinate") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "UTM Coordinates", Binding = new Binding("Grid25Grid.UTMCoordinate") });

                    break;

                case "soakTime":
                    dataGridExcel.ItemsSource = ImportExcel.ExcelSoakTimes;

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("Parent.SamplingDate"),
                        Header = "Date and time sampled"
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    dataGridExcel.Columns.Add(col);

                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("Parent.UserName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "NSAP region", Binding = new Binding("Parent.NSAPRegion.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("Parent.NSAPRegionFMA.FMA.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("Parent.NSAPRegionFMAFishingGround.FishingGround.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("Parent.LandingSiteName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("Parent.VesselName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("Parent.Sector") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing gear", Binding = new Binding("Parent.GearName") });

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("DateTimeSet"),
                        Header = "Date and time of gear set"
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    dataGridExcel.Columns.Add(col);

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("DateTimeHaul"),
                        Header = "Date and time of gear haul"
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    dataGridExcel.Columns.Add(col);

                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Waypoint at set", Binding = new Binding("GPSWaypointAtSet") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Waypoint at haul", Binding = new Binding("GPSWaypointAtHaul") });

                    break;

                case "effortSpecs":
                case "duplicatedEffort":
                    if (result == "effortSpecs")
                    {
                        dataGridExcel.ItemsSource = ImportExcel.ExcelEffortRepeats;
                    }
                    else
                    {
                    }

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("Parent.SamplingDate"),
                        Header = "Date and time sampled"
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    dataGridExcel.Columns.Add(col);

                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("Parent.UserName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "NSAP region", Binding = new Binding("Parent.NSAPRegion.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("Parent.NSAPRegionFMA.FMA.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("Parent.NSAPRegionFMAFishingGround.FishingGround.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("Parent.LandingSiteName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("Parent.VesselName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("Parent.Sector") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing gear", Binding = new Binding("Parent.GearName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Effort specification", Binding = new Binding("EffortSpecification") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Value", Binding = new Binding("EfforValueText") });

                    break;

                case "catchComposition":
                case "duplicatedCatchComp":
                    dataGridExcel.ItemsSource = ImportExcel.ExcelCatchCompositions;

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("Parent.SamplingDate"),
                        Header = "Date and time sampled"
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    dataGridExcel.Columns.Add(col);

                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("Parent.UserName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "NSAP region", Binding = new Binding("Parent.NSAPRegion.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("Parent.NSAPRegionFMA.FMA.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("Parent.NSAPRegionFMAFishingGround.FishingGround.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("Parent.LandingSiteName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("Parent.VesselName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("Parent.Sector") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing gear", Binding = new Binding("Parent.GearName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Taxa", Binding = new Binding("Taxa") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Name of catch", Binding = new Binding("SpeciesName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Weight", Binding = new Binding("SpeciesWeight") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Sample weight", Binding = new Binding("SpeciesSampleWeight") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Notes", Binding = new Binding("Notes") });
                    break;

                case "lengths":
                    dataGridExcel.ItemsSource = ImportExcel.ExcelLengthLists;

                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("Parent.Parent.UserName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "NSAP region", Binding = new Binding("Parent.Parent.NSAPRegion.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("Parent.Parent.NSAPRegionFMA.FMA.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("Parent.Parent.NSAPRegionFMAFishingGround.FishingGround.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("Parent.Parent.LandingSiteName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("Parent.Parent.VesselName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("Parent.Parent.Sector") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing gear", Binding = new Binding("Parent.Parent.GearName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Taxa", Binding = new Binding("Parent.Taxa") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Name of catch", Binding = new Binding("Parent.SpeciesName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Length", Binding = new Binding("Length") });
                    break;

                case "lengthWeight":
                    dataGridExcel.ItemsSource = ImportExcel.ExcelLengthWeights;

                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("Parent.Parent.UserName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "NSAP region", Binding = new Binding("Parent.Parent.NSAPRegion.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("Parent.Parent.NSAPRegionFMA.FMA.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("Parent.Parent.NSAPRegionFMAFishingGround.FishingGround.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("Parent.Parent.LandingSiteName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("Parent.Parent.VesselName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("Parent.Parent.Sector") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing gear", Binding = new Binding("Parent.Parent.GearName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Taxa", Binding = new Binding("Parent.Taxa") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Name of catch", Binding = new Binding("Parent.SpeciesName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Length", Binding = new Binding("Length") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Weight", Binding = new Binding("Weight") });
                    break;

                case "lengthFreq":
                case "duplicatedLenFreq":
                    dataGridExcel.ItemsSource = ImportExcel.ExcelLenFreqs;

                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("Parent.Parent.UserName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "NSAP region", Binding = new Binding("Parent.Parent.NSAPRegion.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("Parent.NSAPRegionFMA.FMA.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("Parent.Parent.NSAPRegionFMAFishingGround.FishingGround.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("Parent.Parent.LandingSiteName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("Parent.Parent.VesselName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("Parent.Parent.Sector") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing gear", Binding = new Binding("Parent.Parent.GearName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Taxa", Binding = new Binding("Parent.Taxa") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Name of catch", Binding = new Binding("Parent.SpeciesName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Length class", Binding = new Binding("LengthClass") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Frequency", Binding = new Binding("Frequency") });
                    break;

                case "gms":
                    dataGridExcel.ItemsSource = ImportExcel.ExcelGMSes;

                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "User name", Binding = new Binding("Parent.Parent.UserName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "NSAP region", Binding = new Binding("Parent.Parent.NSAPRegion.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("Parent.Parent.NSAPRegionFMA.FMA.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("Parent.Parent.NSAPRegionFMAFishingGround.FishingGround.Name") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("Parent.Parent.LandingSiteName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("Parent.Parent.VesselName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("Parent.Parent.Sector") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Fishing gear", Binding = new Binding("Parent.Parent.GearName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Taxa", Binding = new Binding("Parent.Taxa") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Name of catch", Binding = new Binding("Parent.SpeciesName") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Length", Binding = new Binding("Length") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Weight", Binding = new Binding("Weight") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Sex", Binding = new Binding("Sex") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Gonad maturity", Binding = new Binding("GonadMaturity") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Stomach content weight", Binding = new Binding("StomachContentWeight") });
                    dataGridExcel.Columns.Add(new DataGridTextColumn { Header = "Stomach content category", Binding = new Binding("GutContentClassification") });
                    break;
            }
        }

        private void OnMenuItemChecked(object sender, RoutedEventArgs e)
        {
            var menuTag = ((MenuItem)sender).Tag.ToString();
            if (IsMultiVessel)
            {
                switch (menuTag)
                {
                    case "effort":
                        menuTag = "effort_multiVessel";
                        break;
                }
            }
            menuUpload.IsEnabled = menuTag == "effort" || menuTag == "landingSiteSampling" || menuTag == "effort_multiVessel";
            if (_isJSONData)
            {
                if (_jsonfiles != null && _jsonfiles.Count > 0)
                {
                    ShowResultFromAPI(menuTag);
                }
                else
                {
                    ShowResultFromAPI(menuTag);
                }
            }
            else
            {
                ShowResultFromExcel(menuTag);
            }

            labelDuplicated.Content = string.Empty;
            switch (menuTag)
            {
                case "effortSpecs":

                    if (VesselUnloadServerRepository.DuplicatedEffortSpec.Count > 0)
                    {
                        labelDuplicated.Content = "Effort specs are duplicated";
                    }
                    break;

                case "catchComposition":

                    if (VesselUnloadServerRepository.DuplicatedCatchComposition.Count > 0)
                    {
                        labelDuplicated.Content = "Catch composition items are duplicated";
                    }
                    break;

                case "lengthFreq":

                    if (VesselUnloadServerRepository.DuplicatedCatchComposition.Count > 0)
                    {
                        labelDuplicated.Content = "Length classes are duplicated";
                    }
                    break;
            }

            foreach (var mi in menuView.Items)
            {
                if (mi.GetType().Name != "Separator")
                {
                    var menu = (MenuItem)mi;
                    if (menu.Name != ((MenuItem)e.Source).Name)
                    {
                        menu.IsChecked = false;
                    }
                }
            }
        }

        private void OnDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (VesselUnloadServerRepository.UploadInProgress || VesselUnloadServerRepository.UpdateInProgress)
            {
                string mode = "Updating";
                if (VesselUnloadServerRepository.UpdateInProgress)
                {
                    mode = "Uploading";
                }
                if (MessageBox.Show(
                    $"{mode} is in progress\n\rDo you want to stop the operation",
                    "NSAP-ODK Database",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                    ) == MessageBoxResult.Yes)
                {
                    VesselUnloadServerRepository.CancelUpload = true;
                }
                else
                {
                    e.Cancel = true;
                    VesselUnloadServerRepository.UpdateInProgress = false;
                }
            }

            if (!e.Cancel)
            {
                this.SavePlacement();
                ImportExcel.UploadSubmissionToDB -= OnUploadSubmissionToDB;
                VesselUnloadServerRepository.UploadSubmissionToDB -= OnUploadSubmissionToDB;
                MultiVesselGear_UnloadServerRepository.UploadSubmissionToDB -= OnUploadSubmissionToDBMultiVessel;
                if (!_unrecognizedFGAlredyViewed && _unrecognizedFishingGrounds.Count > 0)
                {
                    if (MessageBox.Show(
                        "There were unrecognized fishing grounds in the data that was uploaded\r\n\r\n" +
                        "Select 'Yes' to see them.",
                        "NSAP-ODK Database",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.Yes
                        )
                    {
                        e.Cancel = true;
                        ViewUnrecognizedFishingGrounds();
                    }
                }
            }
        }

        private bool ProcessJsonFileForDisplay(FileInfoJSONMetadata fm, bool includeJSONWhenReset = false)
        {
            bool success = true;
            rowJSONGrid.Height = new GridLength(1, GridUnitType.Star);
            //Logger.Log($"Reading JSON file {fm.JSONFileInfo.Name}");
            //if (!BatchUpload)
            //{
            //    JSON = File.ReadAllText(fm.JSONFileInfo.FullName);
            //}
            //else
            //{
            JSON = "";
            JSONContainsLandingData = true;
            //}

            StringBuilder sb = new StringBuilder();
            try
            {
                sb = new StringBuilder(File.ReadAllText(fm.JSONFileInfo.FullName));
                JSON = sb.ToString();

                if (JSON.Contains("!DOCTYPE html"))
                {
                    success = false;
                    JSONContainsLandingData = false;
                }
                else
                {
                    if (JSON.Contains("repeat_landings_count"))
                    {
                        IsMultiVessel = true;
                        MultiVesselGear_UnloadServerRepository.JSON = JSON;
                        MultiVesselGear_UnloadServerRepository.CreateLandingsFromJSON();
                        NSAPEntities.NSAPRegionViewModel.GetNSAPRegionFromMultiVesselLanding(MultiVesselGear_UnloadServerRepository.MultiVesselLandings[0]);
                    }
                    else
                    {
                        VesselUnloadServerRepository.JSON = JSON;
                        VesselUnloadServerRepository.CreateLandingsFromJSON();
                        VesselUnloadServerRepository.ResetLists(includeJSON: includeJSONWhenReset);
                        NSAPEntities.NSAPRegionViewModel.GetNSAPRegionFromSingleVesselLanding(VesselUnloadServerRepository.VesselLandings[0]);
                    }
                    _historyJSONFileForUploading = fm.JSONFileInfo.Name;


                    SetMenus();
                    gridJSONContent.Visibility = Visibility.Visible;

                    if (fm.DownloadedJsonMetadata != null)
                    {
                        DownloadedJsonMetadata djmd = fm.DownloadedJsonMetadata;

                        if (_jsonFileForUploadCount != null)
                        {
                            labelJSONFile.Content = $"JSON file from {djmd.DBOwner} {djmd.FormName} {djmd.DateDownloaded} # {fm.ItemNumber} of {_jsonFileForUploadCount}";
                        }
                        else
                        {
                            labelJSONFile.Content = $"JSON file from {djmd.DBOwner} {djmd.FormName} {djmd.DateDownloaded}";
                        }
                    }
                    else
                    {
                        if (IsMultiVessel)
                        {

                        }
                        else
                        {
                            labelJSONFile.Content = $"({fm.ItemNumber} of {_jsonFileForUploadCount}) JSON file from {fm.JSONFileInfo.Name}: # of items - {VesselUnloadServerRepository.VesselLandings.Count} ";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                Logger.Log($"Reading contents from {fm.JSONFileInfo}");
                if (_listJSONNotUploaded == null)
                {
                    _listJSONNotUploaded = new List<FileInfoJSONMetadata>();
                }
                _listJSONNotUploaded.Add(fm);
                _lowMemoryReadingJSON = true;
                success = false;
            }
            return success;
        }

        private void OnDataGridLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }


        private void OnTreeviewItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            labelJSONNoLandingsMessage.Visibility = Visibility.Collapsed;
            rowJSONGrid.Height = new GridLength(0);
            rowJSONMetadata.Height = new GridLength(0);

            labelJSONFile.Content = "";
            gridJSONContent.Visibility = Visibility.Collapsed;
            _isJSONData = false;
            _jsonDateDownloadnode = null;

            if (treeViewJSONNavigator.HasItems)
            {
                string itemTag = ((TreeViewItem)treeViewJSONNavigator.SelectedItem).Tag?.ToString();
                switch (itemTag)
                {
                    case "NSAP_ODK.Entities.Database.FileInfoJSONMetadata":
                        JSONContainsLandingData = true;
                        _selectedJSONMetaData = (FileInfoJSONMetadata)((TreeViewItem)e.NewValue).Tag;
                        JSONFileName = _selectedJSONMetaData.JSONFileInfo.FullName;
                        if (_downloadedJsonMetadata != null && _selectedJSONMetaData.Koboserver == null)
                        {
                            _selectedJSONMetaData.Koboserver = NSAPEntities.KoboServerViewModel.KoboserverCollection.
                                FirstOrDefault(t => t.FormName == _downloadedJsonMetadata.FormName && t.Owner == _downloadedJsonMetadata.DBOwner);
                        }
                        if (!BatchUpload)
                        {
                            
                            if(!ProcessJsonFileForDisplay(_selectedJSONMetaData))
                            {
                                //no landings from JSON
                                labelJSONNoLandingsMessage.Visibility = Visibility.Visible;
                            }
                        }
                        else
                        {
                            try
                            {
                                JSON = File.ReadAllText(JSONFileName);
                                if (!string.IsNullOrEmpty(JSON))
                                {
                                    if(JSON.Contains("!DOCTYPE html"))
                                    {
                                        if(JSON.Contains("500 Error"))
                                        {
                                            
                                        }
                                        JSONContainsLandingData = false;
                                    }
                                    else if (JSON.Contains("repeat_landings_count"))
                                    {
                                        IsMultiVessel = true;
                                        MultiVesselGear_UnloadServerRepository.JSON = JSON;
                                        MultiVesselGear_UnloadServerRepository.CreateLandingsFromJSON();
                                        NSAPEntities.NSAPRegionViewModel.GetNSAPRegionFromMultiVesselLanding(MultiVesselGear_UnloadServerRepository.MultiVesselLandings[0]);
                                    }
                                    else if (JSON.Contains("G_lss/sampling_date"))
                                    {
                                        IsOptimizedMultiVessel = true;
                                        IsMultiVessel = true;
                                        MultiVessel_Optimized_UnloadServerRepository.JSON = JSON; ;
                                        MultiVessel_Optimized_UnloadServerRepository.CreateLandingsFromJSON();
                                        NSAPEntities.NSAPRegionViewModel.GetNSAPRegionFromMultiVesselLanding(MultiVessel_Optimized_UnloadServerRepository.MultiVesselLandings[0]);
                                    }
                                    else
                                    {
                                        VesselUnloadServerRepository.JSON = JSON;
                                        VesselUnloadServerRepository.CreateLandingsFromJSON();
                                        VesselUnloadServerRepository.ResetLists(includeJSON: false);
                                        if (VesselUnloadServerRepository.VesselLandings?.Count > 0)
                                        {
                                            NSAPEntities.NSAPRegionViewModel.GetNSAPRegionFromSingleVesselLanding(VesselUnloadServerRepository.VesselLandings[0]);
                                        }
                                    }

                                    if (_selectedJSONMetaData.DownloadedJsonMetadata != null)
                                    {
                                        DownloadedJsonMetadata djmd = _selectedJSONMetaData.DownloadedJsonMetadata;

                                        if (_jsonFileForUploadCount != null)
                                        {
                                            labelJSONFile.Content = $"JSON file from {djmd.DBOwner} {djmd.FormName} {djmd.DateDownloaded} # {_selectedJSONMetaData.ItemNumber} of {_jsonFileForUploadCount}";
                                        }
                                        else
                                        {
                                            labelJSONFile.Content = $"JSON file from {djmd.DBOwner} {djmd.FormName} {djmd.DateDownloaded}";
                                        }
                                    }
                                    else
                                    {

                                        if (IsOptimizedMultiVessel)
                                        {
                                            labelJSONFile.Content = $"({_selectedJSONMetaData.ItemNumber} of {_jsonFileForUploadCount}) JSON file from {_selectedJSONMetaData.JSONFileInfo.Name}: # of items - {MultiVessel_Optimized_UnloadServerRepository.SampledVesselLandings.Count} ";
                                        }
                                        else if (IsMultiVessel)
                                        {

                                            labelJSONFile.Content = $"({_selectedJSONMetaData.ItemNumber} of {_jsonFileForUploadCount}) JSON file from {_selectedJSONMetaData.JSONFileInfo.Name}: # of items - {MultiVesselGear_UnloadServerRepository.SampledVesselLandings.Count} ";

                                        }
                                        else
                                        {
                                            labelJSONFile.Content = $"({_selectedJSONMetaData.ItemNumber} of {_jsonFileForUploadCount}) JSON file from {_selectedJSONMetaData.JSONFileInfo.Name}: # of items - {VesselUnloadServerRepository.VesselLandings.Count} ";

                                            VesselUnloadServerRepository.CurrentJSONFileName = JSONFileName;
                                        }
                                    }
                                }




                                //if (_selectedJSONMetaData.JSONFile != null)
                                //{
                                _jsonFileUseCreationDateForHistory = _selectedJSONMetaData.JSONFileInfo.CreationTime;


                                if (UnmatchedJSONAnalysisResultWindow.Instance != null)
                                {
                                    UnmatchedJSONAnalysisResultWindow.Instance.UnmatchedFieldsFromJSONFile = NSAPEntities.UnmatchedFieldsFromJSONFileViewModel.GetItem(Path.GetFileName(_selectedJSONMetaData.JSONFileInfo.FullName));
                                    UnmatchedJSONAnalysisResultWindow.Instance.ShowAnalysis();
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Log(ex);
                            }
                        }
                        //}
                        //AnalyzeJsonForMismatch.Reset();
                        //AnalyzeJsonForMismatch.JSONFileName = md.JSONFile.FullName;
                        //AnalyzeJsonForMismatch.VesselLandings = VesselUnloadServerRepository.VesselLandings;
                        //AnalyzeJsonForMismatch.Analyze();
                        //if (AnalyzeJsonForMismatch.ResultStatus == "")
                        //{
                        //    AnalyzeJsonForMismatch.Save();
                        //}
                        break;
                    case "NSAP_ODK.Entities.Database.DownloadedJsonMetadata":
                        _downloadedJsonMetadata = (DownloadedJsonMetadata)((TreeViewItem)treeViewJSONNavigator.SelectedItem).Tag;
                        ShowJSONMetadata(_downloadedJsonMetadata);
                        _jsonFileForUploadCount = ((TreeViewItem)treeViewJSONNavigator.SelectedItem).Items.Count;
                        break;
                    default:
                        try
                        {
                            if (File.Exists(itemTag))
                            {
                                FileInfoJSONMetadata fjmd = new FileInfoJSONMetadata { JSONFileInfo = new FileInfo(itemTag) };
                                _jsonFileUseCreationDateForHistory = fjmd.JSONFileInfo.CreationTime;
                                ProcessJsonFileForDisplay(fjmd);
                            }
                        }
                        catch
                        {
                            //ignore
                        }
                        break;

                }

                if (DateTime.TryParse(((TreeViewItem)treeViewJSONNavigator.SelectedItem).Header.ToString(), out DateTime v))
                {
                    _jsonDateDownloadnode = treeViewJSONNavigator.SelectedItem as TreeViewItem;
                }
            }

        }

        private void ClearJSONTreeRootNodes()
        {
            treeViewJSONNavigator.Items.Clear();
        }
        private void ShowJSONMetadata(DownloadedJsonMetadata djmd)
        {
            labelJSONFile.Content = $"Metadata for selected downloaded JSON files downloaded on {djmd.DateDownloaded:MMM-dd-yyyy HH:mm}";
            rowJSONMetadata.Height = new GridLength(1, GridUnitType.Star);
            propertyGridMetadata.SelectedObject = djmd;
            propertyGridMetadata.AutoGenerateProperties = true;
        }
        private void OnTreeMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            bool addUnmatchedJSONMenu = false;
            ContextMenu cm = new ContextMenu();
            MenuItem m = null;
            string tag = ((TreeViewItem)treeViewJSONNavigator.SelectedItem).Tag?.ToString();

            if (DateTime.TryParse(((TreeViewItem)treeViewJSONNavigator.SelectedItem).Header.ToString(), out DateTime v))
            {

                var metadata = (DownloadedJsonMetadata)((TreeViewItem)treeViewJSONNavigator.SelectedItem).Tag;

                //when the date node of downloaded JSON file tree node is selected 
                if (metadata.DownloadType == "all")
                {

                    m = new MenuItem { Header = "Reupload all", Name = "menuReuploadAll" };
                    m.Click += OnMenuClick;
                    cm.Items.Add(m);

                    m = new MenuItem { Header = "Update xFormIdentifier", Name = "menuUpdateXformIdentifier" };
                    m.Click += OnMenuClick;
                    cm.Items.Add(m);

                    m = new MenuItem { Header = "Update landing sites", Name = "menuUpdateLandingSites" };
                    m.Click += OnMenuClick;
                    cm.Items.Add(m);

                    m = new MenuItem { Header = "Locate unsaved landings uploaded to the server", Name = "menuLocateUnsaved" };
                    m.Click += OnMenuClick;
                    cm.Items.Add(m);

                    if (Debugger.IsAttached)
                    {
                        m = new MenuItem { Header = "Locate missing landing site infor", Name = "menuLocateMissingLSInfo" };
                        m.Click += OnMenuClick;
                        cm.Items.Add(m);
                    }
                }
                else
                {
                    //when root node of json tree view of downloaded JSON is selected but download type is not download all from server
                    m = new MenuItem { Header = "Upload all", Name = "menuUploadAllJsonFiles" };
                    m.Click += OnMenuClick;
                    cm.Items.Add(m);
                }

            }
            else if (File.Exists(tag))
            {
                FileInfo fi = new FileInfo(tag);
                if (fi.Extension.ToLower() == ".json")
                {
                    addUnmatchedJSONMenu = true;
                    //when a single history json node is clicked in the treeview
                    _isJSONData = true;
                    m = new MenuItem { Header = "Upload", Name = "menuUploadJsonFile" };
                    m.Click += OnMenuClick;
                    cm.Items.Add(m);
                }
            }
            else if (tag != null && tag.Contains("FileInfoJSONMetadata"))
            {
                addUnmatchedJSONMenu = true;
                //when a singel downloaded JSON file is selected
                m = new MenuItem { Header = "Upload", Name = "menuUploadJsonFile" };
                m.Click += OnMenuClick;
                cm.Items.Add(m);
            }
            else if (((TreeViewItem)treeViewJSONNavigator.SelectedItem).Header.ToString() == "Upload history JSON files")
            {
                //when root node of history json tree view is clicked
                m = new MenuItem { Header = "Upload all", Name = "menuUploadAllJsonHistoryFiles" };
                m.Click += OnMenuClick;
                cm.Items.Add(m);

                m = new MenuItem { Header = "Analyze all JSON", Name = "menuAnalyzeAllJSON" };
                m.Click += OnMenuClick;
                cm.Items.Add(m);
            }

            if (addUnmatchedJSONMenu)
            {
                m = new MenuItem { Header = "Analyze JSON", Name = "menuAnalyzeJSON" };
                m.Click += OnMenuClick;
                cm.Items.Add(m);
            }

            if (cm.Items.Count > 0)
            {
                cm.IsOpen = true;
            }
        }

        private void OnMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {

            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;

        }
    }
}