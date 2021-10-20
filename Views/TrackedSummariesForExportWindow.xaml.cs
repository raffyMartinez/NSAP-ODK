
using NSAP_ODK.Utilities;
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
using Microsoft.Win32;
using NSAP_ODK.Entities.Database;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for TrackedSummariesForExportWindow.xaml
    /// </summary>
    public partial class TrackedSummariesForExportWindow : Window
    {
        private TrackedOperationSummaryViewModel _trackedOperationSummaryViewModel;
        public TrackedSummariesForExportWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (Utilities.Global.Settings.CutOFFUndersizedCW == null)
            {
                textCutoffCW.Text = "11";
            }
            else
            {
                textCutoffCW.Text = Global.Settings.CutOFFUndersizedCW.ToString();
            }
            Height = Height - 60;
            rowStatus.Height = new GridLength(0);
            buttonExport.IsEnabled = false;
        }

        public TrackedLandingCentroidViewModel TrackedLandingCentroidViewModel { get; set; }
        private bool ValidateForm()
        {
            string msg = "Cutoff carapace width cannont be empty";
            if (textCutoffCW.Text.Length > 0)
            {
                msg = "Expected value is a whole number";
                string cutoff = textCutoffCW.Text;
                if (!cutoff.Contains("."))
                {
                    if (int.TryParse(cutoff, out int v))
                    {
                        if (v >= 20)
                        {
                            msg = "Cut-off width cannot exceed max Carapace Width of crab";
                        }
                        else if (v <= 0)
                        {
                            msg = "Cut-off width must be greater than zero";
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            if (msg.Length > 0)
            {
                MessageBox.Show(msg, "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            return false;
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Content)
            {
                case "Ok":
                    if (ValidateForm())
                    {
                        if ((bool)checkAddXY.IsChecked)
                        {
                            //we get the excel file of track centroids and create a view model
                            var ofd = new OpenFileDialog();
                            ofd.Title = "Open excel file of track centroid data";
                            ofd.DefaultExt = ".xlsx";
                            ofd.Filter = "Excel file|*.xls;*.xlsx";
                            if ((bool)ofd.ShowDialog())
                            {
                                TrackedLandingCentroidViewModel = new TrackedLandingCentroidViewModel();
                                TrackedLandingCentroidViewModel.ReadExcelData(ofd.FileName);

                            }

                        }

                        rowStatus.Height = new GridLength(60); ;
                        if (TrackedLandingCentroidViewModel != null)
                        {
                            _trackedOperationSummaryViewModel = new TrackedOperationSummaryViewModel(TrackedLandingCentroidViewModel.TrackedLandingCentroids);
                        }
                        else
                        {
                            _trackedOperationSummaryViewModel = new TrackedOperationSummaryViewModel();
                        }
                        _trackedOperationSummaryViewModel.UndersizedCutoffLength = int.Parse(textCutoffCW.Text);
                        _trackedOperationSummaryViewModel.SummaryRead += On_tosvm_SummaryRead;
                        await _trackedOperationSummaryViewModel.SetupSuammryLlistAsync();
                    }
                    break;
                case "Cancel":
                    Close();
                    break;
                case "Export":
                    ExportToExcel();
                    Close();
                    break;
            }
        }

        private void ExportToExcel()
        {
            string filePath;
            string exportResult;
            if (ExportExcel.GetSaveAsExcelFileName(this, out filePath))
            {

                if (ExportExcel.ExportDatasetToExcel(_trackedOperationSummaryViewModel.TrackedLadningSummaryDataset(), filePath))
                {
                    exportResult = "Successfully exported summary to excel";
                }
                else
                {
                    exportResult = $"Was not successfull in exporting summary to excel\r\n{ExportExcel.ErrorMessage}";
                }

                MessageBox.Show(exportResult, "Export tracked landings summary", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void On_tosvm_SummaryRead(object sender, Entities.TrackedOperationSummaryEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
            {
                switch (e.Intent)
                {
                    case "start":
                        Height += 60;
                        labelCounter.Content = "Setting up tracked vessel landing summary";
                        buttonOk.IsEnabled = false;
                        break;
                    case "setup":

                        progressBarSummary.Value = 0;
                        progressBarSummary.Maximum = e.TotalCountSummary;

                        break;
                    case "reading":
                        progressBarSummary.Value = e.CountOfSummaryRead;
                        labelCounter.Content = $"Reading item {e.CountOfSummaryRead} of {progressBarSummary.Maximum}";
                        break;
                    case "done":
                        progressBarSummary.Visibility = Visibility.Collapsed;
                        progressBarSummary.Value = 0;
                        if (progressBarSummary.Maximum > 0)
                        {
                            labelCounter.Content = $"Done reading {progressBarSummary.Maximum} items!\r\nYou can export to Excel";
                        }
                        else
                        {
                            labelCounter.Content = "No summary items created";
                        }
                        buttonExport.IsEnabled = progressBarSummary.Maximum > 0;
                        break; ;
                }
            }));
        }
    }
}
