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
    /// Interaction logic for UpdateFBSpeciesOptionWindow.xaml
    /// </summary>
    public partial class UpdateFBSpeciesOptionWindow : Window
    {
        private EditWindowEx _parent;
        public UpdateFBSpeciesOptionWindow(EditWindowEx parent)
        {
            InitializeComponent();
            _parent = parent;
        }
        public FBSpeciesUpdateMode UpdateMode { get; set; }
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonCancel":
                    DialogResult = false;
                    break;
                case "buttonOk":
                    if ((bool)rbUpdateNoAdd.IsChecked)
                    {
                        UpdateMode = FBSpeciesUpdateMode.UpdateModeUpdateDoNotAdd;
                    }
                    else
                    {
                        UpdateMode = FBSpeciesUpdateMode.UpdateModeUpdateAndAdd;
                    }
                    DialogResult = true;
                    break;
            }

        }
    }
}
