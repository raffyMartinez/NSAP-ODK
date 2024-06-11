using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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


namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        private DateTime? _commandArgsFilterDate;
        private string _currentEntity;
        public SplashWindow()
        {
            InitializeComponent();
        }

        public string[] CommandArgs { get; set; }
        private async Task LoadEntitiesAsync()
        {
            Utilities.Global.EntityLoading += Global_EntityLoading;
            Utilities.Global.EntityLoaded += Global_EntityLoaded;
            await Task.Run(() => LoadEntities());
            Utilities.Global.EntityLoaded -= Global_EntityLoaded;
            Utilities.Global.EntityLoading -= Global_EntityLoading;
            LabelLoading.Content = "Finished reading database";
            //CSVFIleManager.ReadCSVXML();
            Close();
            //MessageBox.Show("Finished loading data");
        }


        private void Global_EntityLoading(object sender, EntityLoadedEventArg e)
        {
            _currentEntity = e.Name;
            LabelLoading.Dispatcher.BeginInvoke
                (
                  DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                  {
                      LabelLoading.Content = $"Loading {_currentEntity}";
                      //do what you need to do on UI Thread
                      return null;
                  }), null);
        }

        private void EntityRead(string entity)
        {

            //if (LabelEntityRead.InvokeRequired)
            //    LabelEntityRead.Invoke(new Action(() => LabelEntityRead.Content = entity);
        }

        private bool CommandArgsIsValidDate()
        {
            bool isValid = false;
            string args = "";
            for (int x = 0; x < CommandArgs.Count(); x++)
            {
                args += $"{CommandArgs[x]} ";
            }
            args = args.Trim();

            if (DateTime.TryParse(args, out DateTime dt))
            {
                if (dt < DateTime.Now)
                {
                    _commandArgsFilterDate = dt;
                    Global.Filter1 = dt;
                    isValid = true;
                }
            }
            return isValid;
        }
        private async void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            labelSubLabel.Visibility = Visibility.Collapsed;
            labelVersion.Content = $"Version: {Assembly.GetExecutingAssembly().GetName().Version.ToString()}";
            //ProgressBarRead.IsIndeterminate = true;
            if (CommandArgs != null)
            {
                //if ((bool)CommandArgs.Contains("filtered") || (bool)CommandArgs.Contains("server_id") || CommandArgsIsValidDate())
                if ((bool)CommandArgs.Contains("filtered") || (bool)CommandArgs.Contains("server_id") || Global.CommandArgsIsValidDate())
                {
                    labelSubLabel.Visibility = Visibility.Visible;
                    labelSubLabel.Content = "Database will load with filtered data";
                }
            }
            await LoadEntitiesAsync();
            //ProgressBarRead.IsIndeterminate = false; // Maybe hide it, too



        }
        private void LoadEntities()
        {
            Utilities.Global.LoadEntities();

            // this will be run only once after updating the regions-enumerators table  
            // where date of first sampling is added to the table

            //if(NSAPRegionWithEntitiesRepository.EnumeratorFirstSamplingDateRequired)
            //{
            //    NSAPEntities.NSAPEnumeratorViewModel.FirstSamplingOfEnumerators = NSAPEntities.NSAPEnumeratorViewModel.GetFirstSamplingOfEnumerators();
            //}

        }

        private void Global_EntityLoaded(object sender, EntityLoadedEventArg e)
        {
            if (e.IsStarting)
            {
                ProgressBarRead.Dispatcher.BeginInvoke
                (
                    DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                    {
                        ProgressBarRead.Maximum = e.EntityCount;
                        ProgressBarRead.Value = 0;
                        //do what you need to do on UI Thread
                        return null;
                    }), null);
            }
            else if (e.IsEnding)
            {
                LabelLoading.Dispatcher.BeginInvoke
                    (
                      DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                      {
                          LabelLoading.Content = $"Finished loading all entities";
                          //do what you need to do on UI Thread
                          return null;
                      }), null);
            }
            else
            {
                ProgressBarRead.Dispatcher.BeginInvoke
                (
                    DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                    {
                        ProgressBarRead.Value++;
                        //do what you need to do on UI Thread
                        return null;
                    }), null);

                LabelLoading.Dispatcher.BeginInvoke
                    (
                      DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                      {
                          LabelLoading.Content = $"Finished loading {_currentEntity}";
                          //do what you need to do on UI Thread
                          return null;
                      }), null);
            }
        }
    }
}
