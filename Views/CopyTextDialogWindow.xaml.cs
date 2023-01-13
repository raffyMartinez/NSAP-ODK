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
    /// Interaction logic for CopyTextDialogWindow.xaml
    /// </summary>
    public partial class CopyTextDialogWindow : Window
    {
        private static CopyTextDialogWindow _instance;
        public CopyTextDialogWindow()
        {
            InitializeComponent();
            Closing += OnCopyTextDialogWindow_Closing;
        }

        private void OnCopyTextDialogWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _instance = null;
        }

        public static CopyTextDialogWindow GetInstance()
        {
            if(_instance==null)
            {
                _instance = new CopyTextDialogWindow();
            }
            return _instance;
        }
        public DataGrid DataGrid { get; set; }
        public Type DataContextType { get; set; }
        public  object DataGridDataContext { get; set; }
        private async void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            switch(((Button)sender).Content)
            {
                case "Yes":
                    if (DataGrid.Items.Count > 0)
                    {
                        buttonNo.IsEnabled = false;
                        buttonYes.IsEnabled = true;
                        CopyTextFromDataGrid.DataGrid = DataGrid;
                        CopyTextFromDataGrid.DataGridContext = DataGridDataContext;
                        CopyTextFromDataGrid.CopyTextFromDataGridEvent += OnCopyTextFromDataGrid_CopyTextFromDataGridEvent;
                        //CopyTextFromDataGrid.DataGrid = DataGrid;
                        if (await CopyTextFromDataGrid.CopyTextAsync())
                        {

                        }
                    }
                    break;
                case "No":
                    break;
                case "Cancel":
                    break;
            }
        }

        private void OnCopyTextFromDataGrid_CopyTextFromDataGridEvent(object sender, CopyTextFromDataGridEventArg e)
        {
            switch(e.Intent)
            {
                case "start":
                    statusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              statusBar.Maximum = 100;
                              statusBar.IsIndeterminate = true;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    statusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              statusLabel.Content = "Coying text. Please wait...";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "end":
                    statusBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              statusBar.Value=100;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    statusLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              statusLabel.Content = "Finished copying to clipboard";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
            }
        }
    }
}
