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
    /// Interaction logic for SelectDeleteActionDialog.xaml
    /// </summary>
    public partial class SelectDeleteActionDialog : Window
    {
        public SelectDeleteActionDialog()
        {
            InitializeComponent();
            DeleteAction = DeleteAction.deleteActionIgnore;
        }
        public DeleteAction DeleteAction { get; set; }
        private void onButtonClick(object sender, RoutedEventArgs e)
        {
            switch(((Button)sender).Name)
            {
                case "buttonIgnore":
                    DeleteAction = DeleteAction.deleteActionIgnore;
                    break;
                case "buttonDelete":
                    DeleteAction = DeleteAction.deleteActionDelete;
                    break;
                case "buttonRemove":
                    DeleteAction = DeleteAction.deleteActionRemove;
                    break;
            }
            DialogResult = true;
        }
    }
}
