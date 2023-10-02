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

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for DeleteOptionWindow.xaml
    /// </summary>
    /// 
    public enum DeleteChoice
    {
        deleteChoiceNone,
        deleteChoiceMultiVesselGear,
        deleteChoiceSingleVesselGear,
        deleteChoiceBoth
    }
    public partial class DeleteOptionWindow : Window
    {
        public DeleteOptionWindow()
        {
            InitializeComponent();
        }
        public DeleteChoice DeleteChoice { get; set; }

        private DeleteChoice SelectAction()
        {

            if ((bool)chkDeleteMultiVesselEform.IsChecked && (bool)chkDeleteSingleeForm.IsChecked)
            {
                return DeleteChoice.deleteChoiceBoth;
            }
            else if ((bool)chkDeleteMultiVesselEform.IsChecked)
            {
                return DeleteChoice.deleteChoiceMultiVesselGear;
            }
            else if ((bool)chkDeleteSingleeForm.IsChecked)
            {
                return DeleteChoice.deleteChoiceSingleVesselGear;
            }
            return DeleteChoice.deleteChoiceNone;
        }
        private void onButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonOk":
                    DeleteChoice = SelectAction();
                    if (DeleteChoice != DeleteChoice.deleteChoiceNone)
                    {
                        if (DeleteChoice == DeleteChoice.deleteChoiceBoth)
                        {
                            var r = MessageBox.Show(
                                "This will delete all fish landing data\r\n\r\nPlease confirm",
                                Global.MessageBoxCaption,
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question);


                            DialogResult = r == MessageBoxResult.Yes;
                        }
                        else
                        {
                            DialogResult = true;
                        }
                    }
                    break;
                case "buttonCancel":
                    break;
            }
        }
    }
}
