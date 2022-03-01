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
    /// Interaction logic for SelectColumnToUpdateWindow.xaml
    /// </summary>
    public partial class SelectColumnToUpdateWindow : Window
    {
        private RadioButton _checkedButton;
        private static SelectColumnToUpdateWindow _instance;

        public static SelectColumnToUpdateWindow GetInstance()
        {
            if (_instance == null)
                _instance = new SelectColumnToUpdateWindow();

            return _instance;
        }
        public SelectColumnToUpdateWindow()
        {
            InitializeComponent();
            Closing += SelectColumnToUpdateWindow_Closing;
            Loaded += SelectColumnToUpdateWindow_Loaded;
        }

        private void SelectColumnToUpdateWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void SelectColumnToUpdateWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _instance = null;
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch(((Button)sender).Name)
            {
                case "buttonOk":
                    if (_checkedButton != null)
                    {
                        ((DownloadFromServerWindow)Owner).ButtonSelectedColumn = _checkedButton;
                        DialogResult = true;
                    }
                    else
                    {
                        MessageBox.Show(
                            "Select a column for updating",
                            "NSAP-ODK Database",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                            );
                    }
                    
                    break;
                case "buttonCancel":
                    DialogResult = false;
                    break;
            }
        }

        private void OnRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            _checkedButton = (RadioButton)sender;
        }
    }
}
