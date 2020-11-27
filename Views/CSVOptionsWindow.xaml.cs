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
using NSAP_ODK.Entities;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for CSVOptionsWindow.xaml
    /// </summary>
    public partial class CSVOptionsWindow : Window
    {
        public CSVOptionsWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (GenerateCSV.CSVType == CSVType.ExtSelectFromFile)
            {
                radioExtSelectFile.IsChecked = true;
            }
            else if (GenerateCSV.CSVType == CSVType.ExtSelect)
            {
                radioExtSelect.IsChecked = true;
            }
            TxtDelimeter.Text = GenerateCSV.LocationDelimeter.ToString();
        }



        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonOK":

                    GenerateCSV.LocationDelimeter = TxtDelimeter.Text[0];
                    if (radioExtSelect.IsChecked == true)
                    {
                        GenerateCSV.CSVType = CSVType.ExtSelect;
                        Close();
                    }
                    else if (radioExtSelectFile.IsChecked == true)
                    {
                        if (TxtDelimeter.Text.Length == 1)
                        {
                            GenerateCSV.LocationDelimeter = TxtDelimeter.Text[0];
                            GenerateCSV.CSVType = CSVType.ExtSelectFromFile;
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("Delimeter must be exactly one character long only", "Invalid delimeter", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                    }
                        break;
                case "buttonCancel":
                    Close();
                    break;
            }
        }

        private void OnRadioChecked(object sender, RoutedEventArgs e)
        {
            TxtDelimeter.IsEnabled = ((RadioButton)sender).Name == "radioExtSelectFile";
        }
    }
}
