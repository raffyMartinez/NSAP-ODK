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

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for DownloadFromServerWindow.xaml
    /// </summary>
    public partial class DownloadFromServerWindow : Window
    {
        private List<KoboForm> _koboForms;
        private string _user;
        private string _password;
        private string _formID;
        private string _downloadOption;
        private ODKResultsWindow _parentWindow;
        private string _jsonOption;
        private string _downloadType;
        private DateTime? _lastSubmittedDate;
        public DownloadFromServerWindow(ODKResultsWindow parentWindow)
        {
            InitializeComponent();
            _parentWindow = parentWindow;
        }


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
                    ((TreeViewItem)treeForms.Items[item]).Items.Add(new TreeViewItem { Header = "Download", Tag = "form_download", });
                }
                ((TreeViewItem)treeForms.Items[0]).IsSelected = true;
                ((TreeViewItem)treeForms.Items[0]).IsExpanded = true;
                GridGrids.Visibility = Visibility.Visible;
            }
        }
        private async void OnButtonClick(object sender, RoutedEventArgs e)
        {
            _downloadType = "";
            string api_call = "";
            switch (((Button)sender).Name)
            {
                case "ButtonDownload":
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
                                                    //string downnloadedFile = $"{fbd.SelectedPath}/{_formID}.xlsx";
                                                    File.WriteAllBytes(downnloadedFile, bytes);
                                                    ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.ConvertDataToExcel });
                                                    //MessageBox.Show("Excel file saved!");
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

                                            switch (_parentWindow.ODKServerDownload)
                                            {
                                                case ODKServerDownload.ServerDownloadVesselUnload:
                                                    VesselUnloadServerRepository.ResetLists();
                                                    VesselUnloadServerRepository.CreateLandingsFromJSON(the_response);
                                                    VesselUnloadServerRepository.FillDuplicatedLists();
                                                    break;
                                                case ODKServerDownload.ServerDownloadLandings:
                                                    LandingSiteBoatLandingsFromServerRepository.CreateLandingSiteBoatLandingsFromJson(the_response);
                                                    break;
                                            }





                                            ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.GotJSONString, JSONString = the_response });
                                            //KoboAPI.GetUnloadNextRecord();

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
                            else
                            {
                                MessageBox.Show("Please select a download option for JSON");
                            }
                            break;
                    }
                    ButtonDownload.IsEnabled = true;
                    break;
                case "ButtonLogin":
                    _downloadType = "login";
                    ProgressBar.Value = 0;
                    ProgressBar.Maximum = 4;
                    ButtonLogin.IsEnabled = false;
                    using (var httpClient = new HttpClient())
                    {
                        ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.ContactingServer });
                        api_call = "https://kc.kobotoolbox.org/api/v1/forms?format=json";
                        using (var request = new HttpRequestMessage(new HttpMethod("GET"), api_call))
                        {
                            var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{TextBoxUserName.Text}:{TextBoxPassword.Password}"));
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
                                        "Invalid username or password",
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
                                    _koboForms = KoboForms.MakeFormObjects(JsonConvert.DeserializeObject<JArray>(the_response));
                                    AddFormIDToTree();
                                    ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.FinishedDownload });
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
                    break;
                case "ButtonClose":
                    Close();
                    break;

            }
        }

        private void ShowStatus(DownloadFromServerEventArg e)
        {
            switch (e.Intent)
            {
                case DownloadFromServerIntent.ContactingServer:
                    ProgressBar.Value += 1;
                    labelProgress.Content = "Contacting server";
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
                    labelProgress.Content = "Got JSON string";
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
                case DownloadFromServerIntent.StoppedDueToError:
                    labelProgress.Content = "Stopped due to error";
                    break;
            }
        }
        private void OnTreeItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            propertyGrid.Visibility = Visibility.Collapsed;
            dataGrid.Visibility = Visibility.Collapsed;
            gridDownload.Visibility = Visibility.Collapsed;
            var treeViewItem = (TreeViewItem)e.NewValue;
            if (treeViewItem != null)
            {
                switch (treeViewItem.Tag.ToString())
                {
                    case "form_id":
                        _formID = treeViewItem.Header.ToString();
                        var summary = new FormSummary(_koboForms.FirstOrDefault(t => t.formid == int.Parse(_formID)));
                        SetODKServerDownloadType();
                        if (DateTime.TryParse(summary.LastSaveDateInDatabase, out DateTime v))
                        {
                            _lastSubmittedDate = v;
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

                        propertyGrid.SelectedObject = summary;
                        propertyGrid.AutoGenerateProperties = false;
                        propertyGrid.IsCategorized = true;


                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Title", DisplayName = "Name", DisplayOrder = 1, Description = "Name of the form", Category = "Server data" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Description", DisplayName = "Description", DisplayOrder = 2, Description = "Description of the form", Category = "Server data" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FormID", DisplayName = "Form ID", DisplayOrder = 3, Description = "Form identifier", Category = "Server data" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DateCreated", DisplayName = "Date created", DisplayOrder = 4, Description = "Date created", Category = "Server data" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DateModified", DisplayName = "Date modified", DisplayOrder = 5, Description = "Date modified", Category = "Server data" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DateLastSubmission", DisplayName = "Date of last submission", DisplayOrder = 6, Description = "Date of last submission", Category = "Server data" });
                        //propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "SubmittedToday", DisplayName="Number of submissions for today", DisplayOrder=7, Description ="Number of forms submitted today", Category="Server data" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "NumberOfSubmissions", DisplayName = "Number of submissions", DisplayOrder = 8, Description = "Number of submissions", Category = "Server data" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "NumberOfUsers", DisplayName = "Number of users", DisplayOrder = 9, Description = "Number of users", Category = "Server data" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "NumberSavedToDatabase", DisplayName = "Number of submissions", DisplayOrder = 10, Description = "Number of submissions saved", Category = "Saved in database" });
                        propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "LastSaveDateInDatabase", DisplayName = "Date of last submission", DisplayOrder = 11, Description = "Date of last sumission", Category = "Saved in database" });
                        propertyGrid.Visibility = Visibility.Visible;

                        break;
                    case "form_users":
                        _formID = ((TreeViewItem)treeViewItem.Parent).Header.ToString();

                        //var parentItem = GetSelectedTreeViewItemParent(treeViewItem);
                        dataGrid.DataContext = _koboForms.FirstOrDefault(t => t.formid == int.Parse(_formID)).users;
                        dataGrid.Visibility = Visibility.Visible;
                        dataGrid.AutoGenerateColumns = false;

                        dataGrid.Columns.Clear();
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "User", Binding = new Binding("user") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Role", Binding = new Binding("role") });
                        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Permissions", Binding = new Binding("all_permissions") });
                        break;
                    case "form_download":
                        _formID = ((TreeViewItem)treeViewItem.Parent).Header.ToString();
                        SetODKServerDownloadType();
                        gridDownload.Visibility = Visibility.Visible;
                        ((ComboBoxItem)comboboxDownloadOption.Items[0]).IsSelected = true;

                        ComboUser.Items.Clear();
                        foreach (var user in _koboForms.FirstOrDefault(t => t.formid == int.Parse(_formID)).users)
                        {
                            ComboUser.Items.Add(new ComboBoxItem { Content = user.user });
                        }

                        break;
                }
            }

        }
        private void SetODKServerDownloadType()
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

        }



        private void OnCheckStateChange(object sender, RoutedEventArgs e)
        {
            panelFilterByUser.Visibility = (bool)CheckFilterUser.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
