using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NSAP_ODK.Utilities;

namespace NSAP_ODK.Mapping.views
{
    /// <summary>
    /// Interaction logic for SaveMapForm.xaml
    /// </summary>
    public partial class SaveMapForm : Window
    {

        private string _fileName;
        private MapWindowForm _parent;
        private static SaveMapForm _instance;



        public static SaveMapForm GetInstance(MapWindowForm parent)
        {
            if(_instance==null)
            {
                _instance = new SaveMapForm(parent);
            }
            return _instance;
        }
        public SaveMapForm(MapWindowForm parent)
        {
            InitializeComponent();
            _parent = parent;
            Loaded += OnFormLoad;
        }



        private void OnFormLoad(object sender, RoutedEventArgs e)
        {
            if(Global.Settings.SuggestedDPI==null)
            {
                Global.Settings.SuggestedDPI = 150;
            }
            txtSave.Text = Global.Settings.SuggestedDPI.ToString();
        }

        public static string GetSavedMapsFolder()
        {
            if ( Global.Settings.FolderToSaveMapImages==null ||Global.Settings.FolderToSaveMapImages.Length == 0)
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            else
            {
                return Global.Settings.FolderToSaveMapImages;
            }

        }

        private bool Validate()
        {
            string msg="";
            if (int.TryParse(txtSave.Text, out int v))
            {
                if (v > 0)
                {
                    Global.Settings.SuggestedDPI = v;
                }
                else
                {
                    msg = "DPI settings must be greater than zero";
                }
            }
            else
            {
                msg = "DPI settings must be a numeric value";
            }

            if(msg.Length>0)
            {
                System.Windows.MessageBox.Show(msg, Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void onButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult result=System.Windows.Forms.DialogResult.Cancel;
            switch (((System.Windows.Controls.Button)sender).Name)
            {
                case "btnOk":
                    if (Validate() && _fileName!=null &&  _fileName.Length > 0
                        && txtSave.Text.Length > 0
                        && int.Parse(txtSave.Text) <= (int)globalMapping.SuggestedDPI)
                    {
                        if (_parent.SaveMapToImage(_parent.MapControl, int.Parse(txtSave.Text), _fileName, maintainOnePointLineWidth: true, saveToLayout: (bool)chkLayout.IsChecked))
                        {
                            if(int.TryParse(txtSave.Text,out int v))
                            {
                                if (v > 0)
                                {
                                    Global.Settings.SuggestedDPI = v;
                                    Global.SaveGlobalSettings();
                                }
                            }
                            Close();
                        }
                    }
                    else
                    {

                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.Title = "Provide file name of image file";
                        sfd.Filter = "jpeg|*.jpg|tiff|*.tif";
                        sfd.FilterIndex = 2;
                        sfd.AddExtension = true;
                        sfd.InitialDirectory = GetSavedMapsFolder();
                        result = sfd.ShowDialog();
                        if (result == System.Windows.Forms.DialogResult.OK && sfd.FileName.Length > 0)
                        {
                            _fileName = sfd.FileName;
                            if (_parent.SaveMapToImage(_parent.MapControl, int.Parse(txtSave.Text), _fileName, maintainOnePointLineWidth: true, saveToLayout: (bool)chkLayout.IsChecked))
                            {
                                SaveMapParameters.SetParameters(int.Parse(txtSave.Text), _fileName);
                            }
                            else
                            {
                                if (_parent.SuggestedDPI > 0)
                                {
                                    txtSave.Text = ((int)_parent.SuggestedDPI).ToString();
                                }
                                else

                                {
                                    Logger.Log("Was not able to save map to image");
                                }
                            }
                        }
                    }
                    
                    break;
                case "btnCancel":
                    //result is default value of Cancel
                    break;
            }
            DialogResult = result==System.Windows.Forms.DialogResult.OK;

        }
    }
}
