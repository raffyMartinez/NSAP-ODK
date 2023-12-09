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
using NSAP_ODK.Utilities;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for ServerLoginWindow.xaml
    /// </summary>
    public partial class ServerLoginWindow : Window
    {
        private static string _userNameStatic;
        private static string _passwordStatic;
        private static string _tokenStatic;
        private HttpClient _httpClient;
        private DispatcherTimer _timer;
        private bool _logInSuccess;
        public ServerLoginWindow()
        {
            InitializeComponent();
            Closing += ServerLoginWindow_Closing;
            Loaded += ServerLoginWindow_Loaded;
            Closed += ServerLoginWindow_Closed;
            textPassword.KeyUp += TextBoxKeyUp;
        }

        private async void TextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (sender.GetType().Name == "PasswordBox" && e.Key == Key.Enter)
            {
                await DoLogIn();
            }
        }

        private void ServerLoginWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_logInSuccess)
            {
                DialogResult = true;
            }
        }

        public static bool RemoveLoginInformation()
        {
            _userNameStatic = null;
            _passwordStatic = null;
            _tokenStatic = null;
            return true;
        }
        public static string UserNameStatic { get { return _userNameStatic; } }
        public static string PasswordStatic { get { return _passwordStatic; } }

        public static string TokenStatic { get { return _tokenStatic; } }

        private void ServerLoginWindow_Closed(object sender, EventArgs e)
        {
            _timer.Tick -= OnTimerTick;
        }

        public static ServerLogInInformation GetLogInInformation()
        {

            return new ServerLogInInformation
            {
                UserName = _userNameStatic,
                Password = _passwordStatic,
                Token = _tokenStatic
            };


        }
        private void ServerLoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _httpClient = MainWindow.HttpClient;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(3);
            _timer.Tick += OnTimerTick;

            textUserName.Focus();
        }
        private async Task DoLogIn()
        {
            if (textUserName.Text.Trim().Length == 0 || textPassword.Password.Trim().Length == 0)
            {
                MessageBox.Show("User name and password are required", Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                labelLogInStatus.Visibility = Visibility.Visible;
                labelLogInStatus.Content = "Logging in, please wait";

                HttpResponseMessage response;
                byte[] bytes;
                Encoding encoding;
                string the_response;

                var token_call = "https://kf.kobotoolbox.org/token/?format=json";
                using (var token_request = new HttpRequestMessage(new HttpMethod("GET"), token_call))
                {
                    var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{textUserName.Text}:{textPassword.Password}"));
                    if (token_request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}"))
                    {
                        try
                        {
                            //ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.DownloadingData });
                            response = await _httpClient.SendAsync(token_request);
                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                bytes = await response.Content.ReadAsByteArrayAsync();
                                encoding = Encoding.GetEncoding("utf-8");

                                //response is token
                                the_response = encoding.GetString(bytes, 0, bytes.Length);
                                var arr = the_response.Trim('}').Split(':');
                                _tokenStatic = arr[1].Trim('"');
                                _userNameStatic = textUserName.Text;
                                _passwordStatic = textPassword.Password;
                                if (Owner.GetType().Name == "ODKResultsWindow")
                                {
                                    ((ODKResultsWindow)Owner).EnableLoginFromADifferentUser();
                                }

                                _logInSuccess = true;
                                labelLogInStatus.Content = "Log-in succeeded";
                                _timer.Start();

                            }
                            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                            {
                                MessageBox.Show("Provide correct user name and password",
                                    Global.MessageBoxCaption,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error
                                    );
                            }
                            else
                            {
                                MessageBox.Show(
                                    $"Server responded with status {response.ReasonPhrase}",
                                    Global.MessageBoxCaption,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);

                            }
                            //if (await SetupKoboforms(base64authorization))
                            //{
                            //    ButtonLogout.IsEnabled = true;
                            //    HasLoggedInToServer = true;
                            //}
                        }
                        catch (HttpRequestException)
                        {
                            MessageBox.Show("Request time out\r\nYou may try again", Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                            //ShowStatus(new DownloadFromServerEventArg { Intent = DownloadFromServerIntent.StoppedDueToError });
                        }
                    }
                }

            }
        }
        private async void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonLogin":
                    await DoLogIn();
                    break;
                case "buttonCancel":
                    DialogResult = false;
                    break;
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            if (_logInSuccess)
            {
                DialogResult = true;
            }
        }
    }
}
