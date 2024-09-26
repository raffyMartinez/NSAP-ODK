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
            return false;
        }
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch(((Button)sender).Name)
            {
                case "buttonOk":
                    if(Validated())
                    {
                        DialogResult = true;
                    }
                    break;
                case "buttonCance":
                    DialogResult = false;
                    break;
            }
        }
    }
}
