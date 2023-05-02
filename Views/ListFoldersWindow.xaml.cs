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
    /// Interaction logic for ListFoldersWindow.xaml
    /// </summary>
    public partial class ListFoldersWindow : Window
    {
        public ListFoldersWindow()
        {
            InitializeComponent();
        }

        private void onButtonClick(object sender, RoutedEventArgs e)
        {
            switch(((Button)sender).Name)
            {
                case "buttonOk":
                    if(dataGrid.Items.Count>0)
                    {
                        DialogResult = true;
                    }
                    else
                    {

                    }
                    break;
                case "buttonCancel":
                    DialogResult = false;
                    break;
            }
        }
    }
}
