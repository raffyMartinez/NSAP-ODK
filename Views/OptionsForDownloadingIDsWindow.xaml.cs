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

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for OptionsForDownloadingIDsWindow.xaml
    /// </summary>
    public partial class OptionsForDownloadingIDsWindow : Window
    {
        public OptionsForDownloadingIDsWindow()
        {
            InitializeComponent();
        }
        private bool Validated()
        {
            bool isValidated = true;
            if (dateStart.SelectedDate == null)
            {
                isValidated= false;
            }
            else
            {
                if (dateEnd.SelectedDate != null)
                {
                    isValidated=(DateTime)dateStart.SelectedDate < (DateTime)dateEnd.SelectedDate;
                }
            }
            return isValidated;
        }

        public DateTime? StartSubmissionDate { get; internal set; }
        public DateTime? EndSubmissionDate { get; internal set; }

        public bool IgnoreDate { get; set; }
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch(((Button)sender).Name)
            {
                case "buttonOk":
                    if(Validated())
                    {
                        StartSubmissionDate = dateStart.SelectedDate;
                        EndSubmissionDate = dateEnd.SelectedDate;
                        DialogResult = true;
                    }
                    else
                    {
                        string msg = "";
                        if(dateStart.SelectedDate==null)
                        {
                            msg = "Please provide a start date and optioanally an end date";
                        }
                        else
                        {
                            msg = "Start date must be before end date"; ;
                        }
                        MessageBox.Show(
                            msg,
                            Utilities.Global.MessageBoxCaption,
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                            );
                    }

                    break;
                case "buttonCancel":
                    DialogResult = false;
                    break;
                case "buttonIgnore":
                    IgnoreDate = true;
                    DialogResult = true;
                    break;
            }
        }
    }
}
