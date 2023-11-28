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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for LSSMessageBox.xaml
    /// </summary>
    public partial class LSSMessageBox : Window
    {
        public static bool ShowAsDialog(string title, string text)
        {
            LSSMessageBox lssm = new LSSMessageBox(title, text);
            return (bool)lssm.ShowDialog();
        }
        public string DialogTitle { get; set; }
        public string DialogText { get; set; }
        public LSSMessageBox(string title, string text)
        {
            DialogTitle = title;
            DialogText = text;
            Loaded += LSSMessageBox_Loaded;

            InitializeComponent();
        }

        private void LSSMessageBox_Loaded(object sender, RoutedEventArgs e)
        {
            labelTitle.Content = DialogTitle;
            textDescription.Text = DialogText;
            Title = Global.MessageBoxCaption;
        }

        public LSSMessageBox()
        {
            InitializeComponent();
        }

        private void onButtonClick(object sender, RoutedEventArgs e)
        {
            switch(((System.Windows.Controls.Button)sender).Name)
            {
                case "buttonView":
                    DialogResult = true;
                    break;
                case "buttonClose":
                    DialogResult = false;
                    break;
            }
        }
    }
}
