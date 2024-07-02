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
        private bool _ignoreRemove;
        public SelectDeleteActionDialog(bool ignoreRemove=false)
        {
            InitializeComponent();
            DeleteAction = DeleteAction.deleteActionIgnore;
            _ignoreRemove = ignoreRemove;
            Loaded += SelectDeleteActionDialog_Loaded;
        }

        private void SelectDeleteActionDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if(_ignoreRemove)
            {
                labelTitle.Content = "Are you sure you want to delete?";
                textBlock.Text = "Deleting removes the species from the region's watch list database.\r\n\r\nIgnore if you are not sure";
                buttonRemove.Visibility = Visibility.Collapsed;
            }
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
