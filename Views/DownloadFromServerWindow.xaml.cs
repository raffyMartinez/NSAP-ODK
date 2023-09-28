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
using System.Net.Http;
using Newtonsoft.Json;
using NSAP_ODK.Entities;
using NSAP_ODK.Entities.Database;
using Newtonsoft.Json.Linq;
using Xceed.Wpf.Toolkit.PropertyGrid;
using NSAP_ODK.Utilities;
using System.IO;
using Ookii.Dialogs.Wpf;
using System.Net;
using Microsoft.Win32;
using System.Net.Http.Headers;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace NSAP_ODK.Views
{
    public enum ServerIntent
    {
        IntentDownloadData,
        IntentUploadMedia,
        IntentDownloadCSVMedia

    }

    /// <summary>
    /// Interaction logic for DownloadFromServerWindow.xaml
    /// </summary>
    /// 
    public partial class DownloadFromServerWindow : Window
    {
        public static event Action RefreshDatabaseSummaryTable;
        public event EventHandler<DownloadMediaFromServerEventArgs> DownloadMediaFromServerEvent;

        private string _koboforms_json = $"{Global._KoboFormsFolder}\\koboforms.json";
        private int? _totalCountSampledLandings;
        //private int _numberOfSubmissions;
        private int? _numberToDownloadPerBatch;
        private List<KoboForm> _koboForms;
        private List<KoboForm> _koboForms_old;
        private string _versionID;
        private string _formID;
        private string _description;
        private string _downloadOption;
        private ODKResultsWindow _parentWindow;
        private string _jsonOption;
        private string _downloadType;
        private DateTime? _lastSubmittedDate;
        private string _csvSaveToFolder;
        private string _downloadedMediaSaveFolder;
        private List<Metadata> _metadataFilesForReplacement = new List<Metadata>();
        private bool _updateCancelled;
        private DispatcherTimer _timer;
        private FormSummary _formSummary;
        private int _count;
        //private string _token;
        private string _xlsFormVersion;
        private bool _replaceCSVFilesSuccess;
        private bool _hasDownloadOptions = false;
        private HttpClient _httpClient;
        private static string _userNameStatic;
        private static string _passwordStatic;
        private static string _tokenStatic;
        public DownloadFromServerWindow(ODKResultsWindow parentWindow)
        {
            InitializeComponent();
            _parentWindow = parentWindow;
            ServerIntent = ServerIntent.IntentDownloadData;
        }
        public DownloadFromServerWindow()
        {
            InitializeComponent();
            ServerIntent = ServerIntent.IntentDownloadCSVMedia;
        }

        public ServerIntent ServerIntent { get; set; }



        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }
        public bool DownloadCSVFromServer { get; set; }
        public bool LogInAsAnotherUser { get; set; }
        public bool RefreshDatabaseSummry { get; set; }
        private void AddFormIDToTree()
        {
            treeForms.Items.Clear();
            if (_koboForms.Count > 0)
            {
                NSAPEntities.KoboServerViewModel.KoboForms = _koboForms;
                foreach (var form in _koboForms)
                {
                    int item = treeForms.Items.Add(new TreeViewItem { Header = form.formid, Tag = "form_id" });
                    ((TreeViewItem)treeForms.Items[item]).Items.Add(new TreeViewItem { Header = "Users", Tag = "form_users", });
                    ((TreeViewItem)treeForms.Items[item]).Items.Add(new TreeViewItem { Header = "Media", Tag = "form_media", });
                    switch (ServerIntent)
                    {
                        case ServerIntent.IntentDownloadData:
                            ((TreeViewItem)treeForms.Items[item]).Items.Add(new TreeViewItem { Header = "Download", Tag = "form_download" });
                            break;
                        case ServerIntent.IntentUploadMedia:
                            ((TreeViewItem)treeForms.Items[item]).Items.Add(new TreeViewItem { Header = "Upload", Tag = "upload_media" });
                            break;
                        case ServerIntent.IntentDownloadCSVMedia:
                            break;
                    }
                }

                ((TreeViewItem)treeForms.Items[0]).IsSelected = true;
                ((TreeViewItem)treeForms.Items[0]).IsExpanded = true;
                GridGrids.Visibility = Visibility.Visible;
                if (RefreshDatabaseSummry)
                {
                    RefreshDatabaseSummaryTable?.Invoke();
                }
            }
        }



        private bool GetSaveLocationFromSaveAsDialog(string option)
        {
            bool returnValue = false;
            VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();
            fbd.UseDescriptionForTitle = true;


            if (_csvSaveToFolder != null && _csvSaveToFolder.Length > 0)
            {
                fbd.SelectedPath = _csvSaveToFolder;
            }

            switch (option)
            {
                case "csv location":
                    fbd.Description = "Locate folder containing csv files";
                    if ((bool)fbd.ShowDialog() && fbd.SelectedPath.Length > 0)
                    {
                        _csvSaveToFolder = fbd.SelectedPath;
                        GenerateCSV.FolderSaveLocation = _csvSaveToFolder;
                        returnValue = true;
                    }

                    break;
                case "media download":
                    fbd.Description = "Locate where media files are to be saved";
                    if ((bool)fbd.ShowDialog() && fbd.SelectedPath.Length > 0)
                    {
                        _downloadedMediaSaveFolder = fbd.SelectedPath;
                        returnValue = true;
                    }

                    break;
            }
            return returnValue;
        }

        /// <summary>
        /// used to update csv lookup files used by ODK collect
        /// it will delete the file in the server then upload the replacement file from the local computer
        /// </summary>
        /// <param name="assetID"></param>
        /// <returns></returns>
        private async Task<int> ProcesssCSVFiles(string assetID)
        {
            _updateCancelled = false;
            int replacedCount = 0;
            string baseURL = $"https://kf.kobotoolbox.org/api/v2/assets/{assetID}";
            if (GetSaveLocationFromSaveAsDialog(option: "csv location"))
            {
                ProgressBar.IsIndeterminate = true;
                ProgressBar.Value = 0;
                ProgressBar.Maximum = _metadataFilesForReplacement.Count;
                HttpRequestMessage request;
                HttpResponseMessage response;
                string base64authorization;
                var files = Directory.GetFiles(_csvSaveToFolder).Select(s => new FileInfo(s));
                if (files.Any())
                {
                    foreach (Metadata metadata in _metadataFilesForReplacement)
                    {

                        var f = files.Where(t => t.Extension == ".csv" && t.Name == metadata.data_value).FirstOrDefault();
                        if (f != null)
                        {
                            string delete_call = $"{baseURL}/files/{_formSummary.KoboForm.koboform_files.GetFileUID(metadata.data_value)}/";
                            using (request = new HttpRequestMessage(new HttpMethod("DELETE"), delete_call))
                            {
                                base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_userNameStatic}:{_passwordStatic}"));
                                request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");
                                response = await _httpClient.SendAsync(request);
                                try
                                {
                                    if (response.IsSuccessStatusCode)
                                    {
                                        using (request = new HttpRequestMessage(new HttpMethod("POST"), $"{baseURL}/files.json"))
                                        {
                                            request.Headers.TryAddWithoutValidation("Authorization", $"Token {_tokenStatic}");

                                            var multipartContent = new MultipartFormDataContent();
                                            multipartContent.Add(new ByteArrayContent(File.ReadAllBytes(f.FullName)), "content", $"{f.Name}");
                                            multipartContent.Add(new StringContent("form_media"), "file_type");
                                            multipartContent.Add(new StringContent("default"), "description");
                                            multipartContent.Add(new StringContent($"{{\"filename\": \"{f.Name}\"}}"), "metadata");

                                            request.Content = multipartContent;

                                            response = await _httpClient.SendAsync(request);
                                            if (response.IsSuccessStatusCode)
                                            {
                                                if (replacedCount == 0)
                                                {
                                                    ProgressBar.IsIndeterminate = false;
                                                }
                                                replacedCount++;

                                                ProgressBar.Value = replacedCount;
                                                labelProgress.Content = $"{f.Name} was successfully uploaded to the server";
                                            }
                                            else
                                            {

                                            }
                                        }
                                    }
                                    else
                                    {

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Log(ex);
                                }
                            }
                        }
                    }


                    _replaceCSVFilesSuccess = replacedCount == _metadataFilesForReplacement.Count;

                    //redeploy form
                    if (_replaceCSVFilesSuccess)
                    {
                        using (var patchrequest = new HttpRequestMessage(new HttpMethod("PATCH"), $"{baseURL}/deployment/?format=json"))
                        {
                            patchrequest.Headers.TryAddWithoutValidation("Authorization", $"Token {_tokenStatic}");

                            var contentList = new List<string>();
                            contentList.Add("active=true");
                            contentList.Add($"version_id={_versionID}");
                            patchrequest.Content = new StringContent(string.Join("&", contentList));
                            patchrequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                            response = await _httpClient.SendAsync(patchrequest);
                            if (response.IsSuccessStatusCode)
                            {

                            }
                            else
                            {

                            }
                        }
                    }

                    //}
                }
            }
            else
            {
                _updateCancelled = true;
            }

            return replacedCount;
        }

        private async Task<HttpResponseMessage> UploadMediaToServer(FileInfo file)
        {
            HttpResponseMessage result = null;
            string baseURL = "https://kc.kobotoolbox.org/api/v1/metadata.json";
            //using (var httpClient = new HttpClient())
            //{
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), baseURL))
            {
                var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_userNameStatic}:{_passwordStatic}"));
                request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");

                var multipartContent = new MultipartFormDataContent();
                multipartContent.Add(new StringContent(_formID), "xform");
                multipartContent.Add(new StringContent(file.Name), "data_value");
                multipartContent.Add(new StringContent("media"), "data_type");
                multipartContent.Add(new ByteArrayContent(File.ReadAllBytes(file.FullName)), "data_file", file.Name);
                multipartContent.Add(new StringContent("text/csv"), "data_file_type");
                request.Content = multipartContent;

                result = await _httpClient.SendAsync(request);
            }
            //}
            return result;
        }
        public static bool HasLoggedInToServer { get; private set; }
        public string DownloadAsJSONNotes { get; set; }
        public bool SaveDownloadAsJSON { get; set; }
        public bool DownloadOptionDownloadAll { get; set; }
        public bool FormIsMultiVessel { get; set; }
        public bool FormIsMultiGear { get; set; }
        public int? NumberToDownloadPerBatch
        {
            get { return _numberToDownloadPerBatch; }
            set
            {
                _numberToDownloadPerBatch = value;
                ButtonDownload.Width = 100;
                if (_numberToDownloadPerBatch != null)
                {
                    ButtonDownload.Width = 300;
                    ButtonDownload.Content = $"Batch download ({(int)_numberToDownloadPerBatch})";
                }
            }
        }
        public RadioButton ButtonSelectedColumn { get; set; }

        private async Task<bool> DownloadFormMedia()
        {
            int countDownloaded = 0;
            ProgressBar.Value = 0;
            if (GetSaveLocationFromSaveAsDialog(option: "media download"))
            {
                var kf = _koboForms.FirstOrDefault(t => t.formid == _formSummary.FormID);
                ProgressBar.Maximum = kf.metadata.Count;
                labelProgress.Content = "Starting to download media files from server";
                //DownloadMediaFromServerEvent?.Invoke(null, new DownloadMediaFromServerEventArgs { Intent = "start", MediaFileCount = kf.metadata.Count });
                foreach (var media in kf.metadata)
                {
                    //using (var client = new HttpClient())
                    //{
                    using (var s = await _httpClient.GetStreamAsync(media.data_file))
                    {
                        var fileName = $@"{_downloadedMediaSaveFolder }\{media.data_value}";
                        using (var fs = new FileStream(fileName, FileMode.OpenOrCreate))
                        {
                            await s.CopyToAsync(fs);
                            countDownloaded++;
                            ProgressBar.Value = countDownloaded;
                            labelProgress.Content = $"Downloaded media file {countDownloaded} of {ProgressBar.Maximum} to {fileName}";
                            //DownloadMediaFromServerEvent?.Invoke(null, new DownloadMediaFromServerEventArgs { MediaFileName = fileName, Intent = "downloading", MediaFileDownloadedCount = countDownloaded });
                        }
                    }
                    //}
                }
            }
            //DownloadMediaFromServerEvent?.Invoke(null, new DownloadMediaFromServerEventArgs { Intent = "done" });
            labelProgress.Content = "Finished downloading media files";
            _timer.Interval = TimeSpan.FromSeconds(3);
            _timer.Start();
            return countDownloaded > 0;
        }

        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
        }

        private async void OnButtonClick(object sender, RoutedEventArgs e)
        {
            _downloadType = "";
            string api_call = "";
            switch (((Button)sender).Name)
            {
                case "buttonDownloadMedia":
                    #region buttonDownloadMedia
                    if (DownloadCSVFromServer && this.Owner.GetType().Name == "EditWindowEx" && await DownloadFormMedia())
                    {
                        Global.CSVMediaSaveFolder = _downloadedMediaSaveFolder;
                        DialogResult = true;
                        //((ODKResultsWindow)Owner).CSVFileDownloaded();
                    }
                    break;
                #endregion
                case "ButtonLogout":
                    #region ButtonLogout
                    _userNameStatic = "";
                    _passwordStatic = "";
                    _tokenStatic = "";
                    HasLoggedInToServer = false;
                    ((ODKResultsWindow)Owner).EnableLoginFromADifferentUser(enable: false);
                    CloseWindow();
                    break;
                #endregion
                case "buttonSpecifyColumn":
                    #region buttonSpecifyColumn
                    var scuw = new SelectColumnToUpdateWindow();
                    scuw.Owner = this;
                    scuw.ShowDialog();

                    break;
                #endregion
                case "buttonUpload":
                    #region buttonUpload
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Title = "Select CSV file for uploading";
                    ofd.Filter = "CSV file (*.csv)|*.csv";
                    ofd.DefaultExt = ".csv";
                    if ((bool)ofd.ShowDialog() && File.Exists(ofd.FileName))
                    {


                        var fileInfo = new FileInfo(ofd.FileName);

                        foreach (Metadata metadata in dataGrid.Items)
                        {
                            if (metadata.data_value == fileInfo.Name)
                            {
                                MessageBox.Show("You cannot upload a file that is already listed\r\n" +
                                                 "You can replace the file instead",
                                                 Utilities.Global.MessageBoxCaption,
                                                 MessageBoxButton.OK,
                                                 MessageBoxImage.Information);

                                return;
                            }
                        }


                        var result = await UploadMediaToServer(fileInfo);
                        if (result.IsSuccessStatusCode)
                        {
                            MessageBox.Show($"Successfully uploaded {fileInfo.Name}", Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show($"Server returned with status {result.ReasonPhrase}\r\nFile was not uploaded", Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    break;
                #endregion
                case "buttonReplace":
                    #region buttonReplace
                    _replaceCSVFilesSuccess = false;
                    _metadataFilesForReplacement.Clear();
                    foreach (Metadata metadata in dataGrid.Items)
                    {
                        if (metadata.replace)
                        {
                            _metadataFilesForReplacement.Add(metadata);
                        }
                    }

                    if (_metadataFilesForReplacement.Count > 0)
                    {
                        var processedCount = await ProcesssCSVFiles(_formSummary.KPI_id_uid);
                        if (processedCount > 0)
                        {
                            foreach (Metadata md in dataGrid.Items)
                            {
                                md.replace = false;
                            }

                            dataGrid.Items.Refresh();
                            _timer.Start();
                            if (_replaceCSVFilesSuccess)
                            {
                                //call to redeploy form
                                RedeplyForm();
                                MessageBox.Show("All files were replaced", Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else

                            {
                                MessageBox.Show("Some (not all) files were replaced", Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        else
                        {
                            if (!_updateCancelled)
                            {
                                MessageBox.Show($"No files were replaced", Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Select at least one media file", Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    break;
                #endregion
                case "ButtonDownload":
                    #region ButtonDownload
                    bool proceed = true;

                    int defaultDLSize = Utilities.Settings.DefaultDownloadSizeForBatchMode;
                    if (Global.Settings.DownloadSizeForBatchMode != null)
                    {
                        defaultDLSize = (int)Global.Settings.DownloadSizeForBatchMode;
                    }
                    if (_formSummary.KoboForm.title.Contains("Multi-VesselGear"))
                    {
                        defaultDLSize = Utilities.Settings.DefaultDownloadSizeForBatchModeMultiVessel;
                        if (Global.Settings.DownloadSizeForBatchModeMultiVessel != null)
                        {
                            defaultDLSize = (int)Global.Settings.DownloadSizeForBatchModeMultiVessel;
                        }
                    }
                    int sizeToDownload = _formSummary.NumberOfSubmissions - _formSummary.NumberSavedToDatabase;
                    


                    if (!_hasDownloadOptions)
                    {
                        DownloadOptionDownloadAll = sizeToDownload <= defaultDLSize;

                        bool showDLOptions = _formSummary.NumberSavedToDatabase == 0 && sizeToDownload > defaultDLSize;

                        if (!showDLOptions && _formSummary.NumberSavedToDatabase > 0)
                        {
                            showDLOptions = !_hasDownloadOptions && ((bool)rbAll.IsChecked || (sizeToDownload >= defaultDLSize &&
                                 (bool)rbNotDownloaded.IsChecked));
                        }


                        //if ( (_formSummary.NumberSavedToDatabase==0 && sizeToDownload>defaultDLSize) || !_hasDownloadOptions && ((bool)rbAll.IsChecked || (sizeToDownload >= defaultDLSize &&
                        //         (bool)rbNotDownloaded.IsChecked)))
                        if (showDLOptions)
                        {

                            DownloadFromServerOptionsWindow dsow = new DownloadFromServerOptionsWindow();
                            dsow.KoboForm = _formSummary.KoboForm;
                            dsow.Owner = this;
                            dsow.CountItemsToDownload = sizeToDownload;
                            if ((bool)rbAll.IsChecked)
                            {
                                dsow.CountItemsToDownload = _formSummary.NumberOfSubmissions;
                            }

                            proceed = (bool)dsow.ShowDialog();
                            _hasDownloadOptions = proceed;
                            return;
                        }
                        else
                        {
                            _hasDownloadOptions = true;
                        }
                    }

                    if (proceed)
                    {
                        if (DownloadOptionDownloadAll)
                        {
                            _hasDownloadOptions = false;

                            if (!Directory.Exists(Global.Settings.JSONFolder) || Global.Settings.JSONFolder.Length == 0)
                            {
                                MessageBox.Show("Please specify folder to save downloaded data from the server",
                                    Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);

                                SettingsWindow sw = new SettingsWindow();
                                sw.ShowDialog();
                                return;
                            }

                            _downloadType = "data";
                            ButtonDownload.IsEnabled = false;
                            switch (_downloadOption)
                            {
                                case "excel":
                                    #region excel
                                    ProgressBar.Value = 0;
                                    ProgressBar.Maximum = 5;
                                    if (_parentWindow.ODKServerDownload == ODKServerDownload.ServerDownloadVesselUnload)
                                    {
                                        //using (var httpClient = new HttpClient())
                                        //{
                                        api_call = $"https://kc.kobotoolbox.org/api/v1/data/{_formID}.xls";
                                        using (var request = new HttpRequestMessage(new HttpMethod("GET"), api_call))
                                        {
                                            ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.ContactingServer });
                                            var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_userNameStatic}:{_passwordStatic}"));
                                            if (request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}"))
                                            {
                                                try
                                                {
                                                    var response = await _httpClient.SendAsync(request);
                                                    string fileName = response.Content.Headers.ContentDisposition.FileName;
                                                    ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.DownloadingData });
                                                    var bytes = await response.Content.ReadAsByteArrayAsync();


                                                    if (fileName.Length > 0 && bytes.Length > 0)
                                                    {
                                                        VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();
                                                        fbd.RootFolder = Environment.SpecialFolder.MyDocuments;
                                                        fbd.UseDescriptionForTitle = true;
                                                        fbd.Description = "Locate folder for saving downloaded Excel file";

                                                        if ((bool)fbd.ShowDialog() && fbd.SelectedPath.Length > 0)
                                                        {
                                                            string downnloadedFile = $"{fbd.SelectedPath}/{fileName}";
                                                            File.WriteAllBytes(downnloadedFile, bytes);
                                                            ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.ConvertDataToExcel });
                                                            ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.ConvertDataToEntities });
                                                            _parentWindow.ExcelFileDownloaded = downnloadedFile;
                                                            ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.FinishedDownload });
                                                            Close();

                                                        }
                                                    }
                                                    else if (fileName.Length == 0)
                                                    {
                                                        MessageBox.Show("Something went wrong\r\nYou may want to try again", Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                                    }
                                                }
                                                catch (HttpRequestException)
                                                {
                                                    MessageBox.Show("Request time out\r\nYou may try again", Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                                }
                                                catch (Exception ex)
                                                {
                                                    Logger.Log(ex);
                                                    ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.StoppedDueToError });
                                                }
                                            }
                                        }

                                        //}

                                    }
                                    else if (_parentWindow.ODKServerDownload == ODKServerDownload.ServerDownloadLandings)
                                    {
                                        MessageBox.Show("Excel download not yet implemented", Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                    }
                                    break;
                                #endregion
                                case "json":
                                    #region json
                                    ProgressBar.Value = 0;
                                    ProgressBar.Maximum = 5;
                                    if (!string.IsNullOrEmpty(_jsonOption))
                                    {
                                        if (_jsonOption == "download_all_for_review")
                                        {
                                            if (ButtonSelectedColumn != null)
                                            {
                                                string field = "";
                                                switch (ButtonSelectedColumn.Tag.ToString())
                                                {
                                                    case "has catch composition":
                                                        field = "catch_comp_group/include_catchcomp";
                                                        break;
                                                    case "xform identifier":
                                                        field = "_xform_id_string";
                                                        break;
                                                }

                                                await ProcessDownloadForReviewEx(field);
                                            }

                                            MessageBox.Show($"Finished updating database {_updateCount} rows in column {ButtonSelectedColumn.Tag.ToString()}",
                                                Utilities.Global.MessageBoxCaption,
                                                MessageBoxButton.OK,
                                                MessageBoxImage.Information
                                                );
                                        }
                                        else
                                        {
                                            switch (_jsonOption)
                                            {
                                                case "all":
                                                    if (_numberToDownloadPerBatch != null)
                                                    {
                                                        DateTime dt = DateTime.Parse(_formSummary.DateCreated).Date;
                                                        api_call = $"https://kc.kobotoolbox.org/api/v1/data/{_formID}?format=json&query={{\"_submission_time\":{{\"$gt\":\"{dt}\"}}}}&limit={(int)_numberToDownloadPerBatch}";
                                                    }
                                                    else
                                                    {
                                                        api_call = $"https://kc.kobotoolbox.org/api/v1/data/{_formID}?format=json";
                                                    }
                                                    break;
                                                case "all_not_downloaded":
                                                    string lastSubmissionDate = (((DateTime)_lastSubmittedDate)).Date.ToString("yyyy-MM-ddTHH:mm:ss");
                                                    api_call = $"https://kc.kobotoolbox.org/api/v1/data/{_formID}?format=json&query={{\"_submission_time\":{{\"$gte\":\"{lastSubmissionDate}\"}}}}";
                                                    break;
                                                case "predownload_sampling_count":
                                                    api_call = $"https://kc.kobotoolbox.org/api/v1/data/{_formID}?fields=[\"landing_site_sampling_group/count_sampled_landings\",\"today\"]&format=json";
                                                    break;
                                                case "specify_date_range":
                                                    string start_date = ((DateTime)dateStart.Value).ToString("yyyy-MM-dd");
                                                    string end_date = ((DateTime)dateEnd.Value).ToString("yyyy-MM-dd");
                                                    api_call = $"https://kc.kobotoolbox.org/api/v1/data/{_formID}?format=json&query={{\"_submission_time\":{{\"$gte\":\"{start_date}\",\"$lte\":\"{end_date}\"}}}}";
                                                    break;
                                                case "download_by_week":
                                                    string download_date = DateTime.Today.AddDays(int.Parse(txtDaysToDownload.Text) * -1).ToString("yyyy-MM-dd");
                                                    api_call = $"https://kc.kobotoolbox.org/api/v1/data/{_formID}?format=json&query={{\"_submission_time\":{{\"$gte\":\"{download_date}\"}}}}";
                                                    break;
                                                case "specify_range_records":
                                                    string start_date1 = ((DateTime)dateStart2.Value).ToString("yyyy-MM-dd");
                                                    api_call = $"https://kc.kobotoolbox.org/api/v1/data/{_formID}?format=json&query={{\"_submission_time\":{{\"$gte\":\"{start_date1}\"}}}}&limit={TextBoxLimit.Text}";
                                                    break;
                                                case "download_specific_form":
                                                    string uuid = txtFormUUID.Text;
                                                    //api_call = $"https://kc.kobotoolbox.org/api/v1/data/{_formID}?format=json&query={{\"_uuid\":{{\"$eq\":\"{uuid}\"}}}}";
                                                    //api_call = $"https://kc.kobotoolbox.org/api/v1/data/{_formID}?format=json&query={{\"meta/instanceID\":{{\"$eq\":\"{uuid}\"}}}}";
                                                    api_call = $"https://kc.kobotoolbox.org/api/v1/data/{_formID}?format=json&query={{\"_id\":{{\"$eq\":\"{uuid}\"}}}}";
                                                    break;
                                            }

                                            if ((bool)CheckFilterUser.IsChecked || (bool)CheckLimitoTracked.IsChecked)
                                            {
                                                string first_part = "";
                                                string last_part = "";
                                                string user = "";
                                                string toInsert = "";

                                                if (ComboUser.SelectedItem != null)
                                                {
                                                    user = ((ComboBoxItem)ComboUser.SelectedItem).Content.ToString();
                                                }

                                                if ((bool)CheckFilterUser.IsChecked && user.Length == 0)
                                                {
                                                    MessageBox.Show("Please select a user name", Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                                                    return;
                                                }

                                                if (user.Length > 0)
                                                {
                                                    toInsert = $"\"user_name\":\"{user}\",";
                                                }

                                                if ((bool)CheckLimitoTracked.IsChecked)
                                                {
                                                    toInsert += "\"soak_time_group/include_tracking\":\"yes\",";
                                                }


                                                if (api_call.Contains("query"))
                                                {
                                                    int index = api_call.IndexOf("query={") + "query={".Length;
                                                    first_part = api_call.Substring(0, index);
                                                    last_part = api_call.Substring(index, api_call.Length - index);

                                                    api_call = first_part + toInsert + last_part;
                                                }
                                                else
                                                {
                                                    api_call += $"&query={{{toInsert.Trim(',')}}}";
                                                }
                                            }

                                            using (var request = new HttpRequestMessage(new HttpMethod("GET"), api_call))
                                            {
                                                ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.ContactingServer });
                                                var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_userNameStatic}:{_passwordStatic}"));
                                                request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");
                                                try
                                                {
                                                    ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.DownloadingData });

                                                    
                                                    var response = await _httpClient.SendAsync(request);
                                                    var bytes = await response.Content.ReadAsByteArrayAsync();
                                                    Encoding encoding = Encoding.GetEncoding("utf-8");
                                                    StringBuilder the_response = new StringBuilder(encoding.GetString(bytes, 0, bytes.Length));
                                                    ((ODKResultsWindow)Owner).FormID = _formID;
                                                    ((ODKResultsWindow)Owner).Description = _description;
                                                    ((ODKResultsWindow)Owner).Count = _count;
                                                    ((ODKResultsWindow)Owner).Koboform = _formSummary.KoboForm;


                                                    DateTime? versionDate = null;

                                                    switch (_parentWindow.ODKServerDownload)
                                                    {
                                                        case ODKServerDownload.ServerDownloadVesselUnload:
                                                            string json;
                                                            if (_formSummary.IsMultiVessel)
                                                            {
                                                                json = the_response.ToString();
                                                            }
                                                            else if (DateTime.TryParse(_xlsFormVersion, out DateTime versionDate1))
                                                            {
                                                                versionDate = versionDate1;
                                                                if (versionDate >= new DateTime(2021, 10, 1))
                                                                {
                                                                    json = VesselLandingFixDownload.JsonNewToOldVersion(the_response.ToString());
                                                                    //VesselUnloadServerRepository.JSON = VesselLandingFixDownload.JsonNewToOldVersion(the_response);
                                                                }
                                                                else
                                                                {
                                                                    throw new Exception("catch and effort version is not handled");
                                                                }
                                                            }
                                                            else
                                                            {
                                                                json = the_response.ToString();
                                                                //VesselUnloadServerRepository.JSON = the_response;
                                                            }
                                                            ((ODKResultsWindow)Owner).JSON = json;
                                                            if (_formSummary.IsMultiVessel)
                                                            {
                                                                MultiVesselGear_UnloadServerRepository.JSON = json;
                                                                if (_jsonOption == "predownload_sampling_count")
                                                                {
                                                                    MultiVesselGear_UnloadServerRepository.CreateLandingCountsFromJSON();
                                                                    _totalCountSampledLandings = MultiVesselGear_UnloadServerRepository.ListOfLandingsCount.Sum(t => t.CountSampledLandings);
                                                                    MessageBox.Show($"There are {_totalCountSampledLandings} sampled fish landings available for download from the server", Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                                                                    return;
                                                                }
                                                                else
                                                                {
                                                                    MultiVesselGear_UnloadServerRepository.CreateLandingsFromJSON();
                                                                }
                                                            }
                                                            else
                                                            {
                                                                VesselUnloadServerRepository.JSON = json;
                                                                VesselUnloadServerRepository.ResetLists();
                                                                VesselUnloadServerRepository.CreateLandingsFromJSON();
                                                                VesselUnloadServerRepository.FillDuplicatedLists();
                                                            }

                                                            break;
                                                        case ODKServerDownload.ServerDownloadLandings:
                                                            ((ODKResultsWindow)Owner).JSON = the_response.ToString();
                                                            BoatLandingsFromServerRepository.JSON = the_response.ToString();
                                                            BoatLandingsFromServerRepository.CreateBoatLandingsFromJson();
                                                            //LandingSiteBoatLandingsFromServerRepository.JSON = the_response.ToString();
                                                            //LandingSiteBoatLandingsFromServerRepository.CreateLandingSiteBoatLandingsFromJson();
                                                            break;
                                                    }


                                                    ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.GotJSONString, JSONString = the_response.ToString() });
                                                    ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.ConvertDataToEntities });

                                                    switch (_parentWindow.ODKServerDownload)
                                                    {
                                                        case ODKServerDownload.ServerDownloadVesselUnload:
                                                            if (_formSummary.IsMultiVessel)
                                                            {
                                                                _parentWindow.MultiVesselMainSheets = MultiVesselGear_UnloadServerRepository.SampledVesselLandings;
                                                            }
                                                            else
                                                            {
                                                                _parentWindow.MainSheets = VesselUnloadServerRepository.VesselLandings;
                                                            }
                                                            break;
                                                        case ODKServerDownload.ServerDownloadLandings:
                                                            _parentWindow.MainSheetsLanding = BoatLandingsFromServerRepository.BoatLandings;
                                                            break;
                                                    }
                                                    ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.FinishedDownload });

                                                    the_response.Clear();
                                                    the_response = null;

                                                    Close();

                                                }
                                                catch (HttpRequestException http_ex)
                                                {
                                                    Logger.Log(http_ex);
                                                    MessageBox.Show("Request time out\r\nYou may try again", Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                                                }
                                                catch (TaskCanceledException t_ex)
                                                {

                                                }
                                                catch (Exception ex)
                                                {
                                                    Logger.Log(ex);
                                                    ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.StoppedDueToError });
                                                }
                                            }

                                            //}
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Please select a download option for JSON", Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                                    }
                                    break;
                                    #endregion
                            }
                        }
                        else if (SaveDownloadAsJSON)
                        {
                            DownloadToJSONByBatch();
                            _hasDownloadOptions = false;
                        }
                    }
                    ButtonDownload.IsEnabled = true;
                    break;
                #endregion
                case "ButtonLogin":
                    #region ButtonLogin
                    if (TextBoxUserName.Text.Trim().Length == 0 || TextBoxPassword.Password.Trim().Length == 0)
                    {
                        MessageBox.Show("User name and password are required", Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        HttpResponseMessage response;
                        byte[] bytes;
                        Encoding encoding;
                        string the_response;


                        _downloadType = "login";
                        ProgressBar.Value = 0;
                        ProgressBar.Maximum = 4;
                        ButtonLogin.IsEnabled = false;
                        ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.ContactingServer });
                        var token_call = "https://kf.kobotoolbox.org/token/?format=json";
                        using (var token_request = new HttpRequestMessage(new HttpMethod("GET"), token_call))
                        {
                            var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{TextBoxUserName.Text}:{TextBoxPassword.Password}"));
                            if (token_request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}"))
                            {
                                try
                                {
                                    ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.DownloadingData });
                                    response = await _httpClient.SendAsync(token_request);
                                    bytes = await response.Content.ReadAsByteArrayAsync();
                                    encoding = Encoding.GetEncoding("utf-8");

                                    //response is token
                                    the_response = encoding.GetString(bytes, 0, bytes.Length);
                                    var arr = the_response.Trim('}').Split(':');
                                    _tokenStatic = arr[1].Trim('"');
                                    _userNameStatic = TextBoxUserName.Text;
                                    _passwordStatic = TextBoxPassword.Password;
                                    if (Owner.GetType().Name == "ODKResultsWindow")
                                    {
                                        ((ODKResultsWindow)Owner).EnableLoginFromADifferentUser();
                                    }
                                    if (await SetupKoboforms(base64authorization))
                                    {
                                        ButtonLogout.IsEnabled = true;
                                        HasLoggedInToServer = true;
                                    }
                                }
                                catch (HttpRequestException)
                                {
                                    MessageBox.Show("Request time out\r\nYou may try again", Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Log(ex);
                                    ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.StoppedDueToError });
                                }
                            }
                        }
                        ButtonLogin.IsEnabled = true;
                    }
                    break;

                case "ButtonClose":
                    CloseWindow();
                    break;
                    #endregion
            }
        }

        private void CloseWindow()
        {

            Close();
        }

        private void ResetFormNumericIDs()
        {
            Global.Settings.NSAPFishCatchMonitoringKoboserverServerNumericID = "";
            Global.Settings.FisheriesLandingSurveyNumericID = "";
            Global.Settings.TBL_TWSPKoboserverServerNumericID = "";
        }
        private async Task<bool> SetupKoboforms(string base64authorization)
        {
            bool success = true;
            string api_call = "https://kc.kobotoolbox.org/api/v1/forms?format=json";
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), api_call))
            {
                if (request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}"))
                {
                    try
                    {
                        ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.DownloadingData });
                        //server response for ok connection
                        var response = await _httpClient.SendAsync(request);
                        var bytes = await response.Content.ReadAsByteArrayAsync();
                        var encoding = Encoding.GetEncoding("utf-8");

                        var the_response = encoding.GetString(bytes, 0, bytes.Length);
                        //the_response is just json containing forms that you have access, list of media attachements, and basic description of the eform such as
                        // formid, owner, title, list of users with permissions, description
                        //encrypted (yes or no), id_string, date_created, date_modified, last_submission_time
                        //uuid, number of submissions

                        if (the_response.Contains("Invalid username/password"))
                        {
                            MessageBox.Show("Invalid username or password\r\nTry again",
                                Utilities.Global.MessageBoxCaption,
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                        }
                        else
                        {
                            TextBoxUserName.Text = "";
                            TextBoxPassword.Clear();
                            ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.GotJSONString, JSONString = the_response });

                            //we save the koboform if not yet saved
                            //if (!File.Exists(_koboforms_json))
                            //{
                            //    using (StreamWriter sw = File.CreateText(_koboforms_json))
                            //    {
                            //        sw.Write(the_response);
                            //        sw.Close();
                            //    }
                            //}
                            //else
                            //{
                            //    //we creare a list of koboforms from the saved file
                            //    StreamReader sr = File.OpenText(_koboforms_json);
                            //    _koboForms_old = KoboForms.MakeFormObjects(sr.ReadToEnd());
                            //}

                            if (File.Exists(_koboforms_json))
                            {
                                //we creare a list of koboforms from the saved file
                                using (StreamReader sr = File.OpenText(_koboforms_json))
                                {
                                    _koboForms_old = KoboForms.MakeFormObjects(sr.ReadToEnd());
                                }
                            }
                            //we save the latest koboforms as a json file
                            using (StreamWriter sw = File.CreateText(_koboforms_json))
                            {
                                sw.Write(the_response);
                                sw.Close();
                            }




                            //from the_respones, make koboform objects
                            _koboForms = KoboForms.MakeFormObjects(the_response);


                            ResetFormNumericIDs();

                            //we compare the old forms with the ones downloaded
                            //foreach koboform, we get the version (the one showed in the logoscreen of the eform)
                            //as well as the media attachments (specifically csvs) for each eform
                            foreach (KoboForm kf in _koboForms)
                            {
                                KoboForm old_kf = null;
                                if (_koboForms_old != null)
                                {
                                    old_kf = _koboForms_old.FirstOrDefault(t => t.formid == kf.formid);
                                }
                                KoboForms.ResetVersion_and_ID();

                                // test the downloaded koboform is unchanged from the one saved
                                if (old_kf != null && old_kf.hash == kf.hash && old_kf.date_modified == kf.date_modified)
                                {
                                    await KoboForms.GetVersionFromXLSForm(kf);
                                    ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.GotXLSFormVersion, FormName = kf.title });
                                }
                                else
                                {

                                    if (await KoboForms.GetVersionFromXLSForm(kf, _userNameStatic, _passwordStatic, _httpClient))
                                    {
                                        kf.xlsform_version = KoboForms.XLSFormVersion;
                                        kf.xlsForm_idstring = KoboForms.XLSForm_idString;
                                        //kf.Version_ID = KoboForms.Version_ID;
                                        //kf.eFormVersion = KoboForms.e
                                        ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.GotXLSFormVersion, FormName = kf.title });
                                    }
                                }

                            }

                            AddFormIDToTree();
                            ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.FinishedDownload });


                            _timer.Interval = TimeSpan.FromSeconds(3);
                            _timer.Start();
                        }
                    }
                    catch (HttpRequestException)
                    {
                        MessageBox.Show("Request time out\r\nYou may try again", Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                        success = false;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                        ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.StoppedDueToError });
                        success = false;
                    }
                }
            }
            return success;
        }
        private async Task ProcessAPICall(string api_call, int? currentLoop = null, int? loops = null, string filename = null)
        {
            //using (var httpClient = new HttpClient())
            //{
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), api_call))
            {
                ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.ContactingServer });
                var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_userNameStatic}:{_passwordStatic}"));
                request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");
                try
                {
                    ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.DownloadingData, Loop = currentLoop, Loops = loops });

                    var response = await _httpClient.SendAsync(request);


                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    Encoding encoding = Encoding.GetEncoding("utf-8");
                    StringBuilder the_response = new StringBuilder(encoding.GetString(bytes, 0, bytes.Length));
                    //string the_response = encoding.GetString(bytes, 0, bytes.Length);
                    //((ODKResultsWindow)Owner).JSON = the_response;
                    ((ODKResultsWindow)Owner).FormID = _formID;
                    ((ODKResultsWindow)Owner).Description = _description;
                    ((ODKResultsWindow)Owner).Count = _count;


                    DateTime? versionDate = null;


                    //StringBuilder final_json = null; ;
                    switch (_parentWindow.ODKServerDownload)
                    {
                        case ODKServerDownload.ServerDownloadVesselUnload:
                            //string json;
                            if (DateTime.TryParse(_xlsFormVersion, out DateTime versionDate1))
                            {
                                versionDate = versionDate1;
                                if (versionDate >= new DateTime(2021, 10, 1))
                                {
                                    the_response = new StringBuilder(VesselLandingFixDownload.JsonNewToOldVersion(the_response.ToString()));
                                    //json = VesselLandingFixDownload.JsonNewToOldVersion(the_response.ToString());
                                    //VesselUnloadServerRepository.JSON = VesselLandingFixDownload.JsonNewToOldVersion(the_response);
                                }
                                else
                                {
                                    throw new Exception("catch and effort version is not handled");
                                }
                            }
                            else
                            {
                                //final_json = new StringBuilder(the_response.ToString());
                                //json = the_response.ToString();
                                VesselUnloadServerRepository.JSON = the_response.ToString();
                            }

                            if (string.IsNullOrEmpty(filename))
                            {
                                ((ODKResultsWindow)Owner).JSON = the_response.ToString();
                                VesselUnloadServerRepository.JSON = the_response.ToString();
                                VesselUnloadServerRepository.ResetLists();
                                VesselUnloadServerRepository.CreateLandingsFromJSON();
                                VesselUnloadServerRepository.FillDuplicatedLists();
                            }
                            else
                            {
                                ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.SavingToJSONTextFile, FileName = filename });
                                File.WriteAllText(filename, the_response.ToString());
                            }


                            break;
                        case ODKServerDownload.ServerDownloadLandings:
                            ((ODKResultsWindow)Owner).JSON = the_response.ToString();
                            LandingSiteBoatLandingsFromServerRepository.JSON = the_response.ToString();
                            LandingSiteBoatLandingsFromServerRepository.CreateLandingSiteBoatLandingsFromJson();
                            break;
                    }

                    if (string.IsNullOrEmpty(filename))
                    {
                        ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.GotJSONString, JSONString = the_response.ToString() });
                        ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.ConvertDataToEntities });


                        switch (_parentWindow.ODKServerDownload)
                        {
                            case ODKServerDownload.ServerDownloadVesselUnload:
                                _parentWindow.MainSheets = VesselUnloadServerRepository.VesselLandings;
                                break;
                            case ODKServerDownload.ServerDownloadLandings:
                                _parentWindow.MainSheetsLanding = BoatLandingsFromServerRepository.BoatLandings;
                                break;
                        }
                        ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.FinishedDownload });
                        Close();
                    }
                    else
                    {
                        ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.FinishedDownload });
                        File.WriteAllText(filename, the_response.ToString());
                    }
                    //final_json.Clear();
                    //final_json = null;

                    the_response.Clear();
                    the_response = null;

                }
                catch (HttpRequestException _htex)
                {
                    //MessageBox.Show("Request time out\r\nYou may try again");
                    //_downloadBatchCancel = true;
                    Logger.Log(_htex);
                    _downloadBatchCancel = true;
                }
                catch (Exception ex)
                
                {
                    Logger.Log(ex);
                    ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.StoppedDueToError });
                    _downloadBatchCancel = true;
                }
            }

        }


        private async void DownloadToJSONByBatch()
        {
            int downloadSuccessCount = 0;
            string folderToSave;
            VistaFolderBrowserDialog vfbd = new VistaFolderBrowserDialog();
            vfbd.Description = "Select folder for saving downloaded JSON files";
            vfbd.UseDescriptionForTitle = true;
            vfbd.ShowNewFolderButton = true;
            if (Global.Settings.JSONFolder == null)
            {
                vfbd.SelectedPath = System.AppDomain.CurrentDomain.BaseDirectory;
            }
            else if (Global.Settings.JSONFolder.Length == 0)
            {
                vfbd.SelectedPath = System.AppDomain.CurrentDomain.BaseDirectory;
            }
            else
            {
                vfbd.SelectedPath = Global.Settings.JSONFolder;
            }

            if ((bool)vfbd.ShowDialog() && vfbd.SelectedPath.Length > 0)
            {
                Global.Settings.JSONFolder = vfbd.SelectedPath;
                folderToSave = vfbd.SelectedPath;

                int downloadSize = _formSummary.NumberOfSubmissions - _formSummary.NumberSavedToDatabase;
                if (!DownloadOptionDownloadAll && _jsonOption == "all")
                {
                    downloadSize = _formSummary.NumberOfSubmissions;
                }

                int loops = 1;
                if (NumberToDownloadPerBatch < downloadSize)
                {
                    loops = downloadSize / (int)NumberToDownloadPerBatch;
                    if (loops * (int)NumberToDownloadPerBatch < downloadSize) loops++;
                }

                DateTime createdOn = DateTime.Now;

                string fileName = $@"{folderToSave}\{_formSummary.Owner}_{_formSummary.Title}_{createdOn:dd-MMM-yyyy_HH_mm}";
                DownloadedJsonMetadata djmd = new DownloadedJsonMetadata
                {
                    BatchSize = NumberToDownloadPerBatch,
                    DownloadSize = downloadSize,
                    NumberOfFiles = loops,
                    DBOwner = _formSummary.Owner,
                    FormName = _formSummary.Title,
                    DateDownloaded = createdOn,
                    DownloadType = _jsonOption,
                    FormVersion = _formSummary.EFormVersion
                };
                //using (FileStream fs = new FileStream($@"{fileName}_info.xml", FileMode.Create))
                //{
                //    XmlSerializer xSer = new XmlSerializer(typeof(DownloadedJsonMetadata));
                //    xSer.Serialize(
                //        fs,
                //        djmd
                //    //new DownloadedJsonMetadata
                //    //{
                //    //    BatchSize = NumberToDownloadPerBatch,
                //    //    DownloadSize = downloadSize,
                //    //    NumberOfFiles = loops,
                //    //    DBOwner = _formSummary.Owner,
                //    //    FormName = _formSummary.Title,
                //    //    DateDownloaded = createdOn,
                //    //    DownloadType = _jsonOption,
                //    //    FormVersion = _formSummary.EFormVersion
                //    //}
                //    );
                //}

                ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.SetNumberOfLoops, Loops = loops });
                _downloadTimeout = false;
                for (int loop = 0; loop < loops; loop++)
                {
                    string api_call = $"https://kc.kobotoolbox.org/api/v1/data/{_formID}?format=json&sort={{\"_id\":1}}&start={((int)NumberToDownloadPerBatch * loop) + 1}&limit={NumberToDownloadPerBatch}";
                    if (_jsonOption == "all_not_downloaded")
                    {
                        api_call = $"https://kc.kobotoolbox.org/api/v1/data/{_formID}?format=json&sort={{\"_id\":1}}&start={_formSummary.NumberSavedToDatabase + (loop * (int)NumberToDownloadPerBatch) + 1}&limit={NumberToDownloadPerBatch}";
                        //api_call = $"https://kf.kobotoolbox.org/api/v2/data/{_formID}?format=json&sort={{\"_id\":1}}&start={_formSummary.NumberSavedToDatabase + (loop * (int)NumberToDownloadPerBatch) + 1}&limit={NumberToDownloadPerBatch}";
                    }
                    if (!_downloadTimeout && !_downloadBatchCancel)
                    {
                        await ProcessAPICall(api_call, loop, loops, $@"{fileName}_{loop + 1}.json");
                        if (!_downloadTimeout && !_downloadBatchCancel)
                        {
                            downloadSuccessCount++;
                        }
                    }
                    else
                    {
                        break;
                    }
                }


                string msg = "Downloading JSON done!";
                if (_downloadTimeout)
                {
                    djmd.IsTimeOut = true;
                    msg = "Timeout error. Batch downloading not finished\r\n";
                    if (downloadSuccessCount > 0)
                    {
                        msg += $"Only {downloadSuccessCount} file(s) out of {loops} were downloaded.\r\n" +
                             "Internet connectivity could be slow";
                    }
                    else
                    {
                        msg += "No files were successfully downloaded\r\nInternet connectivity could be slow";
                    }

                }
                else if (_downloadBatchCancel)
                {
                    djmd.IsCancelled = true;
                    msg = "Part of batch downloading was cancelled\r\n";
                    if (downloadSuccessCount > 0)
                    {
                        msg += $"Only {downloadSuccessCount} file(s) out of {loops} were downloaded.\r\n" +
                             "Internet connectivity could be slow";
                    }
                    else
                    {
                        msg += "No files were successfully downloaded\r\nInternet connectivity could be slow";
                    }
                }

                djmd.NumberOfFilesDownloaded = downloadSuccessCount;
                using (FileStream fs = new FileStream($@"{fileName}_info.xml", FileMode.Create))
                {
                    XmlSerializer xSer = new XmlSerializer(typeof(DownloadedJsonMetadata));
                    xSer.Serialize(
                        fs,
                        djmd
                    //new DownloadedJsonMetadata
                    //{
                    //    BatchSize = NumberToDownloadPerBatch,
                    //    DownloadSize = downloadSize,
                    //    NumberOfFiles = loops,
                    //    DBOwner = _formSummary.Owner,
                    //    FormName = _formSummary.Title,
                    //    DateDownloaded = createdOn,
                    //    DownloadType = _jsonOption,
                    //    FormVersion = _formSummary.EFormVersion
                    //}
                    );
                }

                MessageBox.Show(msg, Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
        }
        private bool _downloadBatchCancel = false;
        private bool _downloadTimeout = false;
        private int _updateCount;
        private int _updateRounds;
        private async Task ProcessDownloadForReviewEx(string field)
        {
            var api_call = $"https://kf.kobotoolbox.org/api/v2/assets/{_formSummary.KPI_id_uid}/data/?fields=[\"{field}\",\"_uuid\"]&format=json&sort={{\"_id\":1}}";
            var call = "";
            int rowStart = 0;
            double limit = 30000; //30,000 is download limit 
            NSAPEntities.VesselUnloadViewModel.DatabaseUpdatedEvent += VesselUnloadViewModel_ColumnUpdatedEvent;
            if (int.TryParse(textRowStart.Text, out int v))
            {
                rowStart = v;
            }


            if (_formSummary.NumberOfSubmissions > limit && (_formSummary.NumberOfSubmissions - rowStart) > limit)
            {
                double rounds;
                if (rowStart > 0)
                {
                    rounds = (_formSummary.NumberOfSubmissions - rowStart) / limit;
                }
                else
                {
                    rounds = _formSummary.NumberOfSubmissions / limit;
                }
                _updateRounds = (int)rounds + 1;
                for (int x = 1; x <= (int)rounds + 1; x++)
                {
                    call = api_call + $"&start={limit * (x - 1)}";
                    _updateCount += await ProcessDownloadForReview(call, x);
                }
            }
            else
            {
                _updateRounds = 1;
                call = api_call + $"&start={rowStart}";
                _updateCount = await ProcessDownloadForReview(call, 1);
            }
            NSAPEntities.VesselUnloadViewModel.DatabaseUpdatedEvent -= VesselUnloadViewModel_ColumnUpdatedEvent;
        }

        private int _updateColumnRound;
        private int _rowsForUpdating;
        private void VesselUnloadViewModel_ColumnUpdatedEvent(object sender, EventArgs e)
        {
            UpdateDatabaseColumnEventArg ev = (UpdateDatabaseColumnEventArg)e;
            switch (ev.Intent)
            {
                case "start":
                    _rowsForUpdating = ev.RowsToUpdate;
                    _updateColumnRound = ev.Round;
                    ProgressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              ProgressBar.Value = 0;
                              ProgressBar.Maximum = _rowsForUpdating;
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "row updated":
                    labelProgress.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              labelProgress.Content = $"Round {_updateColumnRound} of {_updateRounds}: Updated row {ev.RunningCount} of {_rowsForUpdating}";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    ProgressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              ProgressBar.Value++;
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "finished":
                    labelProgress.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              labelProgress.Content = $"Finished updating {_updateCount} rows";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    break;
            }
        }

        private async Task<int> ProcessDownloadForReview(string apiCall, int round)
        {
            var result = 0;
            //using (var httpClient = new HttpClient())
            //{
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), apiCall))
            {
                ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.ContactingServer });
                var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_userNameStatic}:{_passwordStatic}"));
                request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");
                try
                {
                    ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.DownloadingData });

                    var response = await _httpClient.SendAsync(request);
                    try
                    {
                        var bytes = await response.Content.ReadAsByteArrayAsync();

                        Encoding encoding = Encoding.GetEncoding("utf-8");
                        string the_response = encoding.GetString(bytes, 0, bytes.Length);
                        var arr = apiCall.Split('?', ',')[1].Split('\"');
                        string field_name = arr[1];
                        switch (field_name)
                        {
                            case "_xform_id_string":
                                UpdateXFormIdentifierRepository.JSON = the_response;
                                UpdateXFormIdentifierRepository.CreateXFormIdentifierUpdatesFromJSON();
                                result = await UpdateXFormIdentifierRepository.UpdateDatabase(round);
                                break;
                            case "catch_comp_group/include_catchcomp":
                                UpdateHasCatchCompositionRepository.JSON = the_response;
                                UpdateHasCatchCompositionRepository.CreateCatchCompUpdatesFromJSON();
                                result = await UpdateHasCatchCompositionRepository.UpdateDatabase(round);
                                break;
                        }

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
                catch (HttpRequestException)
                {
                    MessageBox.Show("Request time out\r\nYou may try again", Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.StoppedDueToError });
                }
            }
            //}
            return result;
        }
        private void RedeplyForm()
        {

        }


        private void ShowStatus(DownloadFromServerEventArg e)
        {
            switch (e.Intent)
            {
                case DownloadFromServerIntent.ContactingServer:
                    ProgressBar.Value += 1;
                    labelProgress.Content = "Contacting server";
                    break;
                case DownloadFromServerIntent.GotXLSFormVersion:
                    ProgressBar.Value += 1;
                    labelProgress.Content = $"Retrieved version info for {e.FormName}";
                    break;
                case DownloadFromServerIntent.SetNumberOfLoops:
                    ProgressBar.Maximum = (int)e.Loops;
                    break;
                case DownloadFromServerIntent.DownloadingData:

                    labelProgress.Content = "Downloading data";

                    if (e.Loops != null)
                    {
                        ProgressBar.Value = (int)e.Loop;
                        labelProgress.Content = $"Downloading data {e.Loop + 1} of {e.Loops}";
                    }
                    break;
                case DownloadFromServerIntent.SavingToJSONTextFile:
                    ProgressBar.Value += 1;
                    labelProgress.Content = $"Saved to {e.FileName}";
                    break;
                case DownloadFromServerIntent.ConvertDataToExcel:
                    ProgressBar.Value += 1;
                    labelProgress.Content = "Converting data to Excel";
                    break;
                case DownloadFromServerIntent.GotJSONString:
                    ProgressBar.Value += 1;
                    labelProgress.Content = "Downloaded metadata of eforms";
                    break;
                case DownloadFromServerIntent.ConvertDataToEntities:
                    ProgressBar.Value += 1;
                    labelProgress.Content = "Converting data to entities";
                    break;
                case DownloadFromServerIntent.FinishedDownload:
                    ProgressBar.Value += 1;
                    switch (_downloadType)
                    {
                        case "data":
                            labelProgress.Content = "Finished download";
                            break;
                        case "login":
                            labelProgress.Content = "Log-in successful";
                            break;
                    }
                    break;
                case DownloadFromServerIntent.FinishedDownloadAndSavedJSONFile:
                    ProgressBar.Value += 1;
                    break;
                case DownloadFromServerIntent.StoppedDueToError:
                    labelProgress.Content = "Stopped due to error";
                    if (NumberToDownloadPerBatch == null)
                    {
                        MessageBox.Show("Downloading is stopped due to error", Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    break;
            }
        }

        private async void UploadToMedia()
        {
            //using (var httpClient = new HttpClient())
            //{
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "http://https//<kc_url>/api/v1/metadata.json"))
            {
                request.Headers.TryAddWithoutValidation("Authorization", "Token <token>");

                var multipartContent = new MultipartFormDataContent();
                multipartContent.Add(new StringContent(File.ReadAllText("xform_id>")), "xform");
                multipartContent.Add(new StringContent(File.ReadAllText("filename>")), "data_value");
                multipartContent.Add(new StringContent("media"), "data_type");
                multipartContent.Add(new ByteArrayContent(File.ReadAllBytes("<path/to/file>")), "data_file", System.IO.Path.GetFileName("<path/to/file>"));
                request.Content = multipartContent;

                var response = await _httpClient.SendAsync(request);
            }
            //}
        }

        private void UncheckAllRadioButtons()
        {
            foreach (var c in stackPanelJSON.Children)
            {
                switch (c.GetType().Name)
                {
                    case "RadioButton":
                        if (((RadioButton)c).Name != "rbAll")
                        {
                            ((RadioButton)c).IsChecked = false;
                        }
                        break;
                }
            }
        }
        private void SetDownloadOptionsVisibility()
        {
            foreach (var c in stackPanelJSON.Children)
            {
                switch (c.GetType().Name)
                {
                    case "CheckBox":
                        ((CheckBox)c).Visibility = Visibility.Collapsed;
                        break;
                    case "RadioButton":
                        if (((RadioButton)c).Name != "rbAll")
                        {
                            ((RadioButton)c).Visibility = Visibility.Visible;
                        }
                        break;
                    case "WrapPanel":
                        ((WrapPanel)c).Visibility = Visibility.Collapsed;
                        break;
                }
            }



            if (_formSummary.LastSubmittedDateInDatabase.Length > 0 && DateTime.TryParse(_formSummary.LastSubmittedDateInDatabase, out DateTime v))
            {
                _lastSubmittedDate = v;
                rbNotDownloaded.IsChecked = true;
            }
            else
            {
                foreach (var c in stackPanelJSON.Children)
                {
                    switch (c.GetType().Name)
                    {
                        case "CheckBox":
                            ((CheckBox)c).Visibility = Visibility.Collapsed;
                            break;
                        case "RadioButton":
                            if (((RadioButton)c).Name != "rbAll")
                            {
                                ((RadioButton)c).Visibility = Visibility.Collapsed;
                            }
                            break;
                        case "WrapPanel":
                            ((WrapPanel)c).Visibility = Visibility.Collapsed;
                            break;
                    }
                }

            }

            if (_formSummary.IsMultiVessel)
            {
                rbDownloadCount.Visibility = Visibility.Visible;
            }
            else
            {
                rbDownloadCount.Visibility = Visibility.Collapsed;
            }
        }
        private void OnTreeItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _totalCountSampledLandings = null;
            propertyGrid.Visibility = Visibility.Collapsed;
            gridFormUsers.Visibility = Visibility.Collapsed;
            gridDownload.Visibility = Visibility.Collapsed;
            panelReplace.Visibility = Visibility.Collapsed;
            var treeViewItem = (TreeViewItem)e.NewValue;
            FormIsMultiVessel = false;
            if (treeViewItem != null)
            {
                switch (treeViewItem.Tag.ToString())
                {
                    case "form_id":
                        _formID = treeViewItem.Header.ToString();

                        _formSummary = new FormSummary(_koboForms.FirstOrDefault(t => t.formid == int.Parse(_formID)));
                        FormIsMultiVessel = _formSummary.Description.Contains("Multi-Vessel");
                        _versionID = _formSummary.KoboForm.Version_ID;
                        // _numberOfSubmissions = _formSummary.NumberOfSubmissions;
                        //_xlsFormVersion = _formSummary.XLSForm_Version;
                        if (_parentWindow != null)
                        {
                            SetODKServerDownloadType(_formSummary);
                        }
                        SetDownloadOptionsVisibility();

                        propertyGrid.SelectedObject = _formSummary;
                        propertyGrid.AutoGenerateProperties = false;
                        propertyGrid.IsCategorized = true;


                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Title", DisplayName = "Name", DisplayOrder = 1, Description = "Name of the form", Category = "Server data" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Description", DisplayName = "Description", DisplayOrder = 2, Description = "Description of the form", Category = "Server data" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FormID", DisplayName = "Form ID", DisplayOrder = 3, Description = "Form identifier", Category = "Server data" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Owner", DisplayName = "Owner", DisplayOrder = 4, Description = "User name of form owner", Category = "Server data" });

                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "XLSForm_IDString", DisplayName = "XLSForm ID", DisplayOrder = 5, Description = "ID from XLSForm", Category = "Server data" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "XLSForm_Version", DisplayName = "XLSForm version", DisplayOrder = 6, Description = "Version from XLSForm", Category = "Server data" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "EFormVersion", DisplayName = "e-Form version", DisplayOrder = 7, Description = "Version of e-form", Category = "Server data" });

                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DateCreated", DisplayName = "Date created", DisplayOrder = 8, Description = "Date created", Category = "Server data" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DateModified", DisplayName = "Date modified", DisplayOrder = 9, Description = "Date modified", Category = "Server data" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DateLastSubmission", DisplayName = "Date of last submission", DisplayOrder = 10, Description = "Date of last submission", Category = "Server data" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "NumberOfSubmissions", DisplayName = "Number of submissions", DisplayOrder = 11, Description = "Number of submissions", Category = "Server data" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "NumberOfUsers", DisplayName = "Number of users", DisplayOrder = 12, Description = "Number of users", Category = "Server data" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "NumberSavedToDatabase", DisplayName = "Number of submissions", DisplayOrder = 13, Description = "Number of submissions saved", Category = "Saved in database" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "LastSaveDateInDatabase", DisplayName = "Date of last download from server", DisplayOrder = 14, Description = "Date of last download from server", Category = "Saved in database" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "LastSubmittedDateInDatabase", DisplayName = "Date of last submission", DisplayOrder = 15, Description = "Date of last sumission", Category = "Saved in database" });

                        propertyGrid.Visibility = Visibility.Visible;

                        break;

                    case "form_media":
                        panelReplace.Visibility = Visibility.Visible;
                        gridFormUsers.Visibility = Visibility.Visible;
                        _formID = ((TreeViewItem)treeViewItem.Parent).Header.ToString();
                        _formSummary = new FormSummary(_koboForms.FirstOrDefault(t => t.formid == int.Parse(_formID)));
                        FormIsMultiVessel = _formSummary.Description.Contains("Multi-Vessel");
                        _versionID = _formSummary.KoboForm.Version_ID;
                        dataGrid.DataContext = _koboForms.FirstOrDefault(t => t.formid == int.Parse(_formID)).metadata_active;
                        dataGrid.AutoGenerateColumns = false;
                        dataGrid.Visibility = Visibility.Visible;
                        dataGrid.SelectionUnit = DataGridSelectionUnit.Cell;
                        dataGrid.IsReadOnly = false;

                        dataGrid.Columns.Clear();
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "File", Binding = new Binding("data_value"), IsReadOnly = true });
                        dataGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "For replacement", Binding = new Binding("replace") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Description", Binding = new Binding("Description"), IsReadOnly = true });

                        break;
                    case "form_users":
                        _formID = ((TreeViewItem)treeViewItem.Parent).Header.ToString();
                        gridFormUsers.Visibility = Visibility.Visible;
                        dataGrid.DataContext = _koboForms.FirstOrDefault(t => t.formid == int.Parse(_formID)).users;
                        dataGrid.Visibility = Visibility.Visible;
                        dataGrid.AutoGenerateColumns = false;
                        dataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
                        dataGrid.IsReadOnly = true;

                        dataGrid.Columns.Clear();
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "User", Binding = new Binding("user") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Role", Binding = new Binding("role") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Permissions", Binding = new Binding("all_permissions") });
                        break;
                    case "form_download":

                        _formID = ((TreeViewItem)treeViewItem.Parent).Header.ToString();
                        var koboform = _koboForms.FirstOrDefault(t => t.formid == int.Parse(_formID));


                        //_version = koboform.version;
                        //_xlsFormIdString = koboform.xlsForm_idstring;
                        _xlsFormVersion = koboform.xlsform_version;
                        _formSummary = new FormSummary(koboform);
                        _description = _formSummary.Description;
                        FormIsMultiVessel = _description.Contains("Multi-Vessel");
                        _count = _formSummary.NumberOfSubmissions;
                        SetODKServerDownloadType(_formSummary);
                        SetDownloadOptionsVisibility();
                        gridDownload.Visibility = Visibility.Visible;
                        ((ComboBoxItem)comboboxDownloadOption.Items[0]).IsSelected = true;

                        ComboUser.Items.Clear();
                        foreach (var user in _koboForms.FirstOrDefault(t => t.formid == int.Parse(_formID)).users)
                        {
                            ComboUser.Items.Add(new ComboBoxItem { Content = user.user });
                        }
                        ((ODKResultsWindow)Owner).IsMultiGear = _formSummary.IsMultiGear;
                        ((ODKResultsWindow)Owner).IsMultiVessel = _formSummary.IsMultiVessel;
                        break;
                    case "upload_media":
                        gridDownload.Visibility = Visibility.Visible;
                        break;
                }
            }

        }
        private void SetODKServerDownloadType(FormSummary fs)
        {
            var summary = new FormSummary(_koboForms.FirstOrDefault(t => t.formid == int.Parse(_formID)));

            if (summary.Title.Contains("Daily landings and catch estimate") || summary.Title.Contains("NSAP Fishing boats landed and TWSP"))
            {
                _parentWindow.ODKServerDownload = ODKServerDownload.ServerDownloadLandings;
            }
            else if (summary.Title.Contains("Fisheries landing survey") || summary.Title.Contains("NSAP Fish Catch Monitoring e-Form"))
            {
                _parentWindow.ODKServerDownload = ODKServerDownload.ServerDownloadVesselUnload;
            }
        }
        private ItemsControl GetSelectedTreeViewItemParent(TreeViewItem item)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem || parent is TreeView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as ItemsControl;

        }

        private void OnFormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //_httpClient.Dispose();
            Global.SaveGlobalSettings();
            this.SavePlacement();
        }

        private async void OnFormLoaded(object sender, RoutedEventArgs e)
        {
            _httpClient = MainWindow.HttpClient;
            //_httpClient.Timeout = new TimeSpan(0, 10, 0);
            Title = "Download from server";
            GridGrids.Visibility = Visibility.Collapsed;
            rbAll.IsChecked = true;
            panelFilterByUser.Visibility = Visibility.Collapsed;
            labelProgress.Content = "";
            TextBoxUserName.Focus();

            switch (ServerIntent)
            {
                case ServerIntent.IntentUploadMedia:
                    stackPanelDownload.Visibility = Visibility.Collapsed;
                    stackPanelUploadMedia.Visibility = Visibility.Visible;
                    ButtonDownload.Visibility = Visibility.Collapsed;
                    ButtonUploadMedia.Visibility = Visibility.Visible;
                    labelTitle.Content = "Upload media (CSV files) to the server";
                    break;
                case ServerIntent.IntentDownloadData:
                    stackPanelDownload.Visibility = Visibility.Visible;
                    stackPanelUploadMedia.Visibility = Visibility.Collapsed;
                    ButtonDownload.Visibility = Visibility.Visible;
                    ButtonUploadMedia.Visibility = Visibility.Collapsed;
                    labelTitle.Content = "Download submitted forms from server";
                    break;
            }

            txtFormUUID.Text = "";
            //panelDLSpecificFormUsingUUID.Visibility = Visibility.Collapsed;
            //if (System.Diagnostics.Debugger.IsAttached)
            //{
            //    panelDLSpecificFormUsingUUID.Visibility = Visibility.Visible;
            //}


            _timer = new DispatcherTimer();
            _timer.Tick += OnTimerTick;

            if (!LogInAsAnotherUser && _userNameStatic?.Length > 0 && _passwordStatic?.Length > 0)
            {
                TextBoxUserName.IsEnabled = false;
                TextBoxPassword.IsEnabled = false;
                ButtonLogin.IsEnabled = false;


                var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_userNameStatic}:{_passwordStatic}"));
                if (await SetupKoboforms(base64authorization))
                {
                    AddFormIDToTree();
                    ButtonLogout.IsEnabled = true;
                }
            }

            if (ServerIntent != ServerIntent.IntentDownloadCSVMedia)
            {
                buttonDownloadMedia.Visibility = Visibility.Collapsed;
            }

        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            ProgressBar.Value = 0;
            labelProgress.Content = "";
        }

        private void OnComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            stackPanelJSON.Visibility = Visibility.Collapsed;
            _downloadOption = ((ComboBoxItem)e.AddedItems[0]).Tag.ToString();
            if (_downloadOption == "json")
            {
                stackPanelJSON.Visibility = Visibility.Visible;

                //panelWeeksToDownload.Visibility = Visibility.Visible;

                if (System.Diagnostics.Debugger.IsAttached)
                {
                    panelDLSpecificFormUsingUUID.Visibility = Visibility.Visible;
                    if (_formSummary.NumberSavedToDatabase == 0)
                    {
                        panelDLSpecificFormUsingUUID.Visibility = Visibility.Collapsed;
                    }
                    //panelDLSpecificFormUsingUUID.Visibility = Visibility.Visible;
                }
            }
        }

        private void OnButtonChecked(object sender, RoutedEventArgs e)
        {

            switch (((RadioButton)sender).Name)
            {
                case "rbDownloadSpecificForm":
                    UncheckAllRadioButtons();
                    rbDownloadByPastWeeks.IsChecked = false;
                    txtFormUUID.IsEnabled = true;
                    break;
                case "rbDownloadByPastWeeks":
                    UncheckAllRadioButtons();
                    rbDownloadSpecificForm.IsChecked = false;
                    txtDaysToDownload.IsEnabled = true;
                    break;
                default:
                    rbDownloadSpecificForm.IsChecked = false;
                    rbDownloadByPastWeeks.IsChecked = false;
                    txtDaysToDownload.IsEnabled = false;
                    txtFormUUID.IsEnabled = false;
                    break;
            }
            _jsonOption = ((RadioButton)sender).Tag.ToString();
            panelDateRange.Visibility = _jsonOption == "specify_date_range" ? Visibility.Visible : Visibility.Collapsed;
            panelStartDateRecords.Visibility = _jsonOption == "specify_range_records" ? Visibility.Visible : Visibility.Collapsed;
            panelDownloadAgain.Visibility = _jsonOption == "download_all_for_review" ? Visibility.Visible : Visibility.Collapsed;
        }



        private void OnCheckStateChange(object sender, RoutedEventArgs e)
        {
            switch (((CheckBox)sender).Name)
            {
                case "CheckCheckAll":
                    foreach (Metadata item in dataGrid.Items)
                    {
                        item.replace = (bool)CheckCheckAll.IsChecked;
                    }
                    dataGrid.Items.Refresh();
                    break;
                case "CheckFilterUser":
                    panelFilterByUser.Visibility = (bool)CheckFilterUser.IsChecked ? Visibility.Visible : Visibility.Collapsed;
                    break;
            }

        }

        private void OnRadioButtonMouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
