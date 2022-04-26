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
using NSAP_ODK.Entities.Database.FromJson;
using Newtonsoft.Json.Linq;
using Xceed.Wpf.Toolkit.PropertyGrid;
using NSAP_ODK.Utilities;
using System.IO;
using Ookii.Dialogs.Wpf;
using NSAP_ODK.Entities.Database;
using System.Net;
using Microsoft.Win32;
using System.Net.Http.Headers;
using System.Windows.Threading;

namespace NSAP_ODK.Views
{
    public enum ServerIntent
    {
        IntentDownloadData,
        IntentUploadMedia
    }

    /// <summary>
    /// Interaction logic for DownloadFromServerWindow.xaml
    /// </summary>
    /// 
    public partial class DownloadFromServerWindow : Window
    {
        private List<KoboForm> _koboForms;
        private string _user;
        private string _password;
        //private string _version;
        private string _versionID;
        private string _formID;
        private string _description;
        private string _downloadOption;
        private ODKResultsWindow _parentWindow;
        private string _jsonOption;
        private string _downloadType;
        private DateTime? _lastSubmittedDate;
        private string _csvSaveToFolder;
        private List<Metadata> _metadataFilesForReplacement = new List<Metadata>();
        private bool _updateCancelled;
        private DispatcherTimer _timer;
        private FormSummary _formSummary;
        private int _count;
        private string _token;
        private string _xlsFormVersion;
        private bool _replaceCSVFilesSuccess;
        private int _numberOfSubmissions;
        public DownloadFromServerWindow(ODKResultsWindow parentWindow)
        {
            InitializeComponent();
            _parentWindow = parentWindow;
            ServerIntent = ServerIntent.IntentDownloadData;
        }

        public ServerIntent ServerIntent { get; set; }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }
        private void AddFormIDToTree()
        {
            treeForms.Items.Clear();
            if (_koboForms.Count > 0)
            {

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
                    }
                }
                ((TreeViewItem)treeForms.Items[0]).IsSelected = true;
                ((TreeViewItem)treeForms.Items[0]).IsExpanded = true;
                GridGrids.Visibility = Visibility.Visible;
            }
        }



        private bool GetCSVSaveLocationFromSaveAsDialog()
        {
            VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();
            fbd.UseDescriptionForTitle = true;
            fbd.Description = "Locate folder containing csv files";

            if (_csvSaveToFolder != null && _csvSaveToFolder.Length > 0)
            {
                fbd.SelectedPath = _csvSaveToFolder;
            }


            if ((bool)fbd.ShowDialog() && fbd.SelectedPath.Length > 0)
            {
                _csvSaveToFolder = fbd.SelectedPath;
                GenerateCSV.FolderSaveLocation = _csvSaveToFolder;
                return true;
            }
            else
            {
                return false;
            }
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
            if (GetCSVSaveLocationFromSaveAsDialog())
            {
                ProgressBar.Value = 0;
                ProgressBar.Maximum = _metadataFilesForReplacement.Count;
                HttpRequestMessage request;
                HttpResponseMessage response;
                string base64authorization;
                var files = Directory.GetFiles(_csvSaveToFolder).Select(s => new FileInfo(s));
                if (files.Any())
                {
                    using (var httpClient = new HttpClient())
                    {
                        //_metadataFilesForReplacement is a list of files that are for replacement
                        foreach (Metadata metadata in _metadataFilesForReplacement)
                        {
                            var f = files.Where(t => t.Extension == ".csv" && t.Name == metadata.data_value).FirstOrDefault();
                            if (f != null)
                            {
                                string delete_call = $"{baseURL}/files/{_formSummary.KoboForm.koboform_files.GetFileUID(metadata.data_value)}/";
                                using (request = new HttpRequestMessage(new HttpMethod("DELETE"), delete_call))
                                {
                                    base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_user}:{_password}"));
                                    request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");
                                    response = await httpClient.SendAsync(request);
                                    try
                                    {
                                        if (response.IsSuccessStatusCode)
                                        {
                                            using (request = new HttpRequestMessage(new HttpMethod("POST"), $"{baseURL}/files.json"))
                                            {
                                                request.Headers.TryAddWithoutValidation("Authorization", $"Token {_token}");

                                                var multipartContent = new MultipartFormDataContent();
                                                multipartContent.Add(new ByteArrayContent(File.ReadAllBytes(f.FullName)), "content", $"{f.Name}");
                                                multipartContent.Add(new StringContent("form_media"), "file_type");
                                                multipartContent.Add(new StringContent("default"), "description");
                                                multipartContent.Add(new StringContent($"{{\"filename\": \"{f.Name}\"}}"), "metadata");

                                                request.Content = multipartContent;

                                                response = await httpClient.SendAsync(request);
                                                if (response.IsSuccessStatusCode)
                                                {
                                                    replacedCount++;
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
                                patchrequest.Headers.TryAddWithoutValidation("Authorization", $"Token {_token}");

                                var contentList = new List<string>();
                                contentList.Add("active=true");
                                contentList.Add($"version_id={_versionID}");
                                patchrequest.Content = new StringContent(string.Join("&", contentList));
                                patchrequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                                response = await httpClient.SendAsync(patchrequest);
                                if (response.IsSuccessStatusCode)
                                {

                                }
                                else
                                {

                                }
                            }
                        }

                    }
                }
            }
            else
            {
                _updateCancelled = true;
            }

            return replacedCount;
        }


        private async Task<int> ProcesssCSVFiles()
        {
            _updateCancelled = false;
            int replacedCount = 0;
            string baseURL = "https://kc.kobotoolbox.org/api/v1/metadata/";
            if (GetCSVSaveLocationFromSaveAsDialog())
            {
                ProgressBar.Value = 0;
                ProgressBar.Maximum = _metadataFilesForReplacement.Count;

                var files = Directory.GetFiles(_csvSaveToFolder).Select(s => new FileInfo(s));
                if (files.Any())
                {
                    foreach (Metadata metadata in _metadataFilesForReplacement)
                    {
                        var f = files.Where(t => t.Extension == ".csv" && t.Name == metadata.data_value).FirstOrDefault();
                        if (f != null)
                        {
                            string api_call = $"{baseURL}{metadata.id}";
                            using (var httpClient = new HttpClient())
                            {
                                using (var request = new HttpRequestMessage(new HttpMethod("DELETE"), api_call))
                                {
                                    var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_user}:{_password}"));
                                    //request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");
                                    request.Headers.TryAddWithoutValidation("Authorization", $"Token {_token}");
                                    try
                                    {
                                        var response = await httpClient.SendAsync(request);
                                        if (response.IsSuccessStatusCode)
                                        {
                                            response = await UploadMediaToServer(f);
                                            if (response.IsSuccessStatusCode)
                                            {
                                                replacedCount++;
                                                ProgressBar.Value = replacedCount;
                                                labelProgress.Content = $"Updated {replacedCount} of {_metadataFilesForReplacement.Count} files";
                                            }
                                        }
                                        else
                                        {

                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }

                                //break;
                            }
                        }



                    }
                }
            }
            else
            {
                _updateCancelled = true;
            }


            return replacedCount;
        }

        private async Task<HttpResponseMessage> UploadMediaToServer1(FileInfo file)
        {
            HttpResponseMessage result = null;
            string baseURL = "https://kc.kobotoolbox.org/api/v1/metadata.json";
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), baseURL))
                {
                    var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_user}:{_password}"));
                    request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");

                    var multipartContent = new MultipartFormDataContent();
                    multipartContent.Add(new StringContent(_formID), "xform");
                    multipartContent.Add(new StringContent(file.Name), "data_value");
                    multipartContent.Add(new StringContent("media"), "data_type");
                    multipartContent.Add(new ByteArrayContent(File.ReadAllBytes(file.FullName)), "data_file", file.Name);
                    multipartContent.Add(new StringContent("text/csv"), "data_file_type");
                    request.Content = multipartContent;

                    result = await httpClient.SendAsync(request);
                }
            }
            return result;
        }

        private async Task<HttpResponseMessage> UploadMediaToServer(FileInfo file)
        {
            HttpResponseMessage result = null;
            string baseURL = "https://kc.kobotoolbox.org/api/v1/metadata.json";
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), baseURL))
                {
                    var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_user}:{_password}"));
                    request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");

                    var multipartContent = new MultipartFormDataContent();
                    multipartContent.Add(new StringContent(_formID), "xform");
                    multipartContent.Add(new StringContent(file.Name), "data_value");
                    multipartContent.Add(new StringContent("media"), "data_type");
                    multipartContent.Add(new ByteArrayContent(File.ReadAllBytes(file.FullName)), "data_file", file.Name);
                    multipartContent.Add(new StringContent("text/csv"), "data_file_type");
                    request.Content = multipartContent;

                    result = await httpClient.SendAsync(request);
                }
            }
            return result;
        }
        public RadioButton ButtonSelectedColumn { get; set; }
        private async void OnButtonClick(object sender, RoutedEventArgs e)
        {
            _downloadType = "";
            string api_call = "";
            switch (((Button)sender).Name)
            {
                case "buttonSpecifyColumn":
                    var scuw = new SelectColumnToUpdateWindow();
                    scuw.Owner = this;
                    scuw.ShowDialog();

                    break;
                case "buttonUpload":
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
                                                 "NSAP-ODK Database",
                                                 MessageBoxButton.OK,
                                                 MessageBoxImage.Information);

                                return;
                            }
                        }


                        var result = await UploadMediaToServer(fileInfo);
                        if (result.IsSuccessStatusCode)
                        {
                            MessageBox.Show($"Successfully uploaded {fileInfo.Name}", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show($"Server returned with status {result.ReasonPhrase}\r\nFile was not uploaded", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    break;
                case "buttonReplace":
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
                                MessageBox.Show("All files were replaced", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else

                            {
                                MessageBox.Show("Some (not all) files were replaced", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        else
                        {
                            if (!_updateCancelled)
                            {
                                MessageBox.Show($"No files were replaced", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Select at least one media file", "NSAP-ODK", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    break;
                case "ButtonDownload":

                    if (!Directory.Exists(Global.Settings.JSONFolder) || Global.Settings.JSONFolder.Length == 0)
                    {
                        MessageBox.Show("Please specify folder to save downloaded data from the server",
                            "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);

                        SettingsWindow sw = new SettingsWindow();
                        sw.ShowDialog();
                        return;
                    }

                    _downloadType = "data";
                    ButtonDownload.IsEnabled = false;
                    switch (_downloadOption)
                    {
                        case "excel":
                            ProgressBar.Value = 0;
                            ProgressBar.Maximum = 5;
                            if (_parentWindow.ODKServerDownload == ODKServerDownload.ServerDownloadVesselUnload)
                            {
                                using (var httpClient = new HttpClient())
                                {
                                    api_call = $"https://kc.kobotoolbox.org/api/v1/data/{_formID}.xls";
                                    using (var request = new HttpRequestMessage(new HttpMethod("GET"), api_call))
                                    {
                                        ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.ContactingServer });
                                        var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_user}:{_password}"));
                                        request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");
                                        try
                                        {
                                            var response = await httpClient.SendAsync(request);
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
                                                MessageBox.Show("Something went wrong\r\nYou may want to try again");
                                            }
                                        }
                                        catch (HttpRequestException)
                                        {
                                            MessageBox.Show("Request time out\r\nYou may try again");
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.Log(ex);
                                            ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.StoppedDueToError });
                                        }
                                    }

                                }

                            }
                            else if (_parentWindow.ODKServerDownload == ODKServerDownload.ServerDownloadLandings)
                            {
                                MessageBox.Show("Excel download not yet implemented");
                            }
                            break;
                        case "json":
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
                                        "NSAP-ODK Database",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Information
                                        );
                                }
                                else
                                {
                                    using (var httpClient = new HttpClient())
                                    {
                                        switch (_jsonOption)
                                        {
                                            case "all":
                                                api_call = $"https://kc.kobotoolbox.org/api/v1/data/{_formID}?format=json";
                                                break;
                                            case "all_not_downloaded":
                                                string lastSubmissionDate = ((DateTime)_lastSubmittedDate).ToString("yyyy-MM-ddThh:mm:ss");
                                                api_call = $"https://kc.kobotoolbox.org/api/v1/data/{_formID}?format=json&query={{\"_submission_time\":{{\"$gt\":\"{lastSubmissionDate}\"}}}}";
                                                break;
                                            case "specify_date_range":
                                                string start_date = ((DateTime)dateStart.Value).ToString("yyyy-MM-dd");
                                                string end_date = ((DateTime)dateEnd.Value).ToString("yyyy-MM-dd");
                                                api_call = $"https://kc.kobotoolbox.org/api/v1/data/{_formID}?format=json&query={{\"_submission_time\":{{\"$gt\":\"{start_date}\",\"$lt\":\"{end_date}\"}}}}";
                                                break;
                                            case "specify_range_records":
                                                string start_date1 = ((DateTime)dateStart2.Value).ToString("yyyy-MM-dd");
                                                api_call = $"https://kc.kobotoolbox.org/api/v1/data/{_formID}?format=json&query={{\"_submission_time\":{{\"$gt\":\"{start_date1}\"}}}}&limit={TextBoxLimit.Text}";
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
                                                MessageBox.Show("Please select a user name");
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
                                            var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_user}:{_password}"));
                                            request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");
                                            try
                                            {
                                                ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.DownloadingData });

                                                var response = await httpClient.SendAsync(request);
                                                var bytes = await response.Content.ReadAsByteArrayAsync();
                                                Encoding encoding = Encoding.GetEncoding("utf-8");
                                                string the_response = encoding.GetString(bytes, 0, bytes.Length);
                                                ((ODKResultsWindow)Owner).JSON = the_response;
                                                ((ODKResultsWindow)Owner).FormID = _formID;
                                                ((ODKResultsWindow)Owner).Description = _description;
                                                ((ODKResultsWindow)Owner).Count = _count;


                                                DateTime? versionDate = null;

                                                switch (_parentWindow.ODKServerDownload)
                                                {
                                                    case ODKServerDownload.ServerDownloadVesselUnload:
                                                        if (DateTime.TryParse(_xlsFormVersion, out DateTime versionDate1))
                                                        {
                                                            versionDate = versionDate1;
                                                            if (versionDate >= new DateTime(2021, 10, 1))
                                                            {
                                                                VesselUnloadServerRepository.JSON = VesselLandingFixDownload.JsonNewToOldVersion(the_response);
                                                            }
                                                            else
                                                            {
                                                                throw new Exception("catch and effort version is not handled");
                                                            }
                                                        }
                                                        else
                                                        {
                                                            VesselUnloadServerRepository.JSON = the_response;
                                                        }
                                                        VesselUnloadServerRepository.ResetLists();
                                                        VesselUnloadServerRepository.CreateLandingsFromJSON();
                                                        VesselUnloadServerRepository.FillDuplicatedLists();


                                                        break;
                                                    case ODKServerDownload.ServerDownloadLandings:
                                                        LandingSiteBoatLandingsFromServerRepository.JSON = the_response;
                                                        LandingSiteBoatLandingsFromServerRepository.CreateLandingSiteBoatLandingsFromJson();
                                                        break;
                                                }


                                                ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.GotJSONString, JSONString = the_response });
                                                ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.ConvertDataToEntities });

                                                switch (_parentWindow.ODKServerDownload)
                                                {
                                                    case ODKServerDownload.ServerDownloadVesselUnload:
                                                        _parentWindow.MainSheets = VesselUnloadServerRepository.VesselLandings;
                                                        break;
                                                    case ODKServerDownload.ServerDownloadLandings:
                                                        _parentWindow.MainSheetsLanding = LandingSiteBoatLandingsFromServerRepository.LandingSiteBoatLandings;
                                                        break;
                                                }
                                                ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.FinishedDownload });
                                                Close();

                                            }
                                            catch (HttpRequestException)
                                            {
                                                MessageBox.Show("Request time out\r\nYou may try again");
                                            }
                                            catch (Exception ex)
                                            {
                                                Logger.Log(ex);
                                                ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.StoppedDueToError });
                                            }
                                        }

                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Please select a download option for JSON");
                            }
                            break;
                    }
                    ButtonDownload.IsEnabled = true;
                    break;

                case "ButtonLogin":
                    if (TextBoxUserName.Text.Trim().Length == 0 || TextBoxPassword.Password.Trim().Length == 0)
                    {
                        MessageBox.Show("User name and password are required", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        string user_name = "";
                        string password = "";
                        _downloadType = "login";
                        ProgressBar.Value = 0;
                        ProgressBar.Maximum = 4;
                        ButtonLogin.IsEnabled = false;
                        using (var httpClient = new HttpClient())
                        {
                            ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.ContactingServer });

                            var token_call = "https://kf.kobotoolbox.org/token/?format=json";
                            using (var token_request = new HttpRequestMessage(new HttpMethod("GET"), token_call))
                            {
                                user_name = TextBoxUserName.Text;
                                password = TextBoxPassword.Password;
                                var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user_name}:{password}"));
                                token_request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");
                                try
                                {
                                    ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.DownloadingData });
                                    var response = await httpClient.SendAsync(token_request);
                                    var bytes = await response.Content.ReadAsByteArrayAsync();
                                    Encoding encoding = Encoding.GetEncoding("utf-8");
                                    string the_response = encoding.GetString(bytes, 0, bytes.Length);
                                    var arr = the_response.Trim('}').Split(':');
                                    _token = arr[1].Trim('"');
                                }
                                catch (HttpRequestException)
                                {
                                    MessageBox.Show("Request time out\r\nYou may try again", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Log(ex);
                                    ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.StoppedDueToError });
                                }
                            }


                            api_call = "https://kc.kobotoolbox.org/api/v1/forms?format=json";
                            using (var request = new HttpRequestMessage(new HttpMethod("GET"), api_call))
                            {
                                var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user_name}:{password}"));
                                request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");
                                try
                                {
                                    ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.DownloadingData });
                                    var response = await httpClient.SendAsync(request);
                                    var bytes = await response.Content.ReadAsByteArrayAsync();
                                    Encoding encoding = Encoding.GetEncoding("utf-8");
                                    string the_response = encoding.GetString(bytes, 0, bytes.Length);
                                    if (the_response.Contains("Invalid username/password"))
                                    {
                                        MessageBox.Show("Invalid username or password\r\nTry again",
                                            "NSAP ODK Database",
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Information);
                                    }
                                    else
                                    {
                                        _user = TextBoxUserName.Text;
                                        _password = TextBoxPassword.Password;
                                        TextBoxUserName.Text = "";
                                        TextBoxPassword.Clear();
                                        ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.GotJSONString, JSONString = the_response });
                                        _koboForms = KoboForms.MakeFormObjects(the_response);

                                        foreach (KoboForm kf in _koboForms)
                                        {
                                            KoboForms.ResetVersion_and_ID();
                                            if (await KoboForms.GetVersionFromXLSForm(kf, user_name, password))
                                            {
                                                kf.xlsform_version = KoboForms.XLSFormVersion;
                                                kf.xlsForm_idstring = KoboForms.XLSForm_idString;
                                                kf.Version_ID = KoboForms.Version_ID;
                                                //kf.eFormVersion = KoboForms.e
                                                ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.GotXLSFormVersion, FormName = kf.title });
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
                                    MessageBox.Show("Request time out\r\nYou may try again");
                                }
                                catch (Exception ex)
                                {
                                    Logger.Log(ex);
                                    ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.StoppedDueToError });
                                }
                            }
                            ButtonLogin.IsEnabled = true;
                        }
                    }
                    break;

                case "ButtonClose":
                    Close();
                    break;

            }
        }

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
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), apiCall))
                {
                    ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.ContactingServer });
                    var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_user}:{_password}"));
                    request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");
                    try
                    {
                        ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.DownloadingData });

                        var response = await httpClient.SendAsync(request);
                        try
                        {
                            var bytes = await response.Content.ReadAsByteArrayAsync();

                            Encoding encoding = Encoding.GetEncoding("utf-8");
                            string the_response = encoding.GetString(bytes, 0, bytes.Length);
                            var arr = apiCall.Split('?', ',')[1].Split('\"');
                            string field_name = arr[1];
                            switch(field_name)
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
                        MessageBox.Show("Request time out\r\nYou may try again");
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                        ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.StoppedDueToError });
                    }
                }
            }
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
                case DownloadFromServerIntent.DownloadingData:
                    ProgressBar.Value += 1;
                    labelProgress.Content = "Downloading data";
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
                    MessageBox.Show("Downloading is stopped due to error", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    break;
            }
        }

        private async void UploadToMedia()
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "http://https//<kc_url>/api/v1/metadata.json"))
                {
                    request.Headers.TryAddWithoutValidation("Authorization", "Token <token>");

                    var multipartContent = new MultipartFormDataContent();
                    multipartContent.Add(new StringContent(File.ReadAllText("xform_id>")), "xform");
                    multipartContent.Add(new StringContent(File.ReadAllText("filename>")), "data_value");
                    multipartContent.Add(new StringContent("media"), "data_type");
                    multipartContent.Add(new ByteArrayContent(File.ReadAllBytes("<path/to/file>")), "data_file", System.IO.Path.GetFileName("<path/to/file>"));
                    request.Content = multipartContent;

                    var response = await httpClient.SendAsync(request);
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

            if (_formSummary.LastSaveDateInDatabase.Length > 0 && DateTime.TryParse(_formSummary.LastSaveDateInDatabase, out DateTime v))
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
        }
        private void OnTreeItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            propertyGrid.Visibility = Visibility.Collapsed;
            gridFormUsers.Visibility = Visibility.Collapsed;
            gridDownload.Visibility = Visibility.Collapsed;
            panelReplace.Visibility = Visibility.Collapsed;
            var treeViewItem = (TreeViewItem)e.NewValue;
            if (treeViewItem != null)
            {
                switch (treeViewItem.Tag.ToString())
                {
                    case "form_id":
                        _formID = treeViewItem.Header.ToString();
                        _formSummary = new FormSummary(_koboForms.FirstOrDefault(t => t.formid == int.Parse(_formID)));
                        _versionID = _formSummary.KoboForm.Version_ID;
                        _numberOfSubmissions = _formSummary.NumberOfSubmissions;
                        //_xlsFormVersion = _formSummary.XLSForm_Version;
                        SetODKServerDownloadType(_formSummary);
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
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "LastSaveDateInDatabase", DisplayName = "Date of last submission", DisplayOrder = 14, Description = "Date of last sumission", Category = "Saved in database" });

                        propertyGrid.Visibility = Visibility.Visible;

                        break;

                    case "form_media":
                        panelReplace.Visibility = Visibility.Visible;
                        gridFormUsers.Visibility = Visibility.Visible;
                        _formID = ((TreeViewItem)treeViewItem.Parent).Header.ToString();
                        _formSummary = new FormSummary(_koboForms.FirstOrDefault(t => t.formid == int.Parse(_formID)));
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

            if (summary.Title.Contains("Daily landings and catch estimate"))
            {
                _parentWindow.ODKServerDownload = ODKServerDownload.ServerDownloadLandings;
            }
            else if (summary.Title.Contains("Fisheries landing survey"))
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
            this.SavePlacement();
        }

        private void OnFormLoaded(object sender, RoutedEventArgs e)
        {
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

            _timer = new DispatcherTimer();
            _timer.Tick += OnTimerTick;

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
            }
        }

        private void OnButtonChecked(object sender, RoutedEventArgs e)
        {
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
    }
}
