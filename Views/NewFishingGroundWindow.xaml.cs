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
    /// Interaction logic for NewFishingGroundWindow.xaml
    /// </summary>
    public partial class NewFishingGroundWindow : Window
    {
        public NewFishingGroundWindow()
        {
            InitializeComponent();
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonOk":
                    FishingGround fg = new FishingGround { Code = txtCode.Text, Name = txtName.Text };
                    List<EntityValidationMessage> msgs;
                    if (NSAPEntities.FishingGroundViewModel.EntityValidated(fg, out msgs, true) && NSAPEntities.FishingGroundViewModel.AddRecordToRepo(fg))
                    {
                        ((UnrecognizedFGsWindows)Owner).NewFishingGround = fg;
                        Close();
                    }
                    else
                    {
                        string errors = "";
                        foreach (var m in msgs)
                        {
                            if (m.MessageType == MessageType.Error)
                            {
                                errors += $"{m.Message}\r\n";
                            }
                        }

                        if (errors.Length > 0)
                        {
                            MessageBox.Show($"There were errors adding a new fishing ground:\r\n\r\n{errors}",
                                "NSAP-ODK Database",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information
                                );
                        }
                    }
                    break;

                case "buttonCancel":
                    Close();
                    break;
            }
        }
    }
}
