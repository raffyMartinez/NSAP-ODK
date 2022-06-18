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
using swf = System.Windows.Forms;
using System.IO;
using NSAP_ODK.Entities.Database;

namespace NSAP_ODK.Views
{

    public enum SelectImportActionOption
    {
        ImportActionNone,
        ImportActionIntoNewDatabase,
        ImportActionIntoExistingDatabase,
        ImportActionIntoCurrentDatabase
    }
    /// <summary>
    /// Interaction logic for SelectImportActionWindow.xaml
    /// </summary>
    public partial class SelectImportActionWindow : Window
    {

        public string MDBFile { get; private set; }

        public SelectImportActionOption ImportActionOption { get; set; }
        public SelectImportActionWindow()
        {
            InitializeComponent();
        }

        private void onButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonOk":
                    ((ODKResultsWindow)Owner).ImportActionOption = ImportActionOption;
                    ((ODKResultsWindow)Owner).ImportIntoMDBFile = MDBFile;
                    ((ODKResultsWindow)Owner).OpenImportedDatabaseInApplication = (bool)chkOpenAfterImport.IsChecked;
                    DialogResult = true;
                    break;
                case "buttonSelect":
                    buttonOk.IsEnabled = SelectMDBFileToImportInto();
                    break;
                case "buttonCancel":
                    DialogResult = false;
                    break; ;
            }
        }
        private bool FileDialogForMDBFile(ref string mdbfilePath)
        {
            mdbfilePath = "";
            swf.CommonDialog dialog;
            if (ImportActionOption == SelectImportActionOption.ImportActionIntoNewDatabase)
            {
                dialog = new swf.SaveFileDialog();
                ((swf.SaveFileDialog)dialog).Title = "Create new Microsoft Access Database (MDB) file";
                ((swf.SaveFileDialog)dialog).Filter = "MDB file (*.mdb)|*.mdb|All files (*.)|*.*";
                ((swf.SaveFileDialog)dialog).DefaultExt = "*.mdb";
            }
            else
            {
                dialog = new swf.OpenFileDialog();
                ((swf.OpenFileDialog)dialog).Title = "Select Microsoft Access Database (MDB) file";
                ((swf.OpenFileDialog)dialog).Filter = "MDB file (*.mdb)|*.mdb|All files (*.)|*.*";
                ((swf.OpenFileDialog)dialog).DefaultExt = "*.mdb";
            }

            if (dialog.ShowDialog() == swf.DialogResult.OK)
            {

                if (ImportActionOption == SelectImportActionOption.ImportActionIntoExistingDatabase)
                {
                    mdbfilePath = ((swf.OpenFileDialog)dialog).FileName;
                    return File.Exists(mdbfilePath);
                }
                else
                {
                    mdbfilePath = ((swf.SaveFileDialog)dialog).FileName;
                    return true;
                }
            }
            else
            {
                return false;
            }

        }

        private bool SelectMDBFileToImportInto()
        {
            MDBFile = "";
            switch (ImportActionOption)
            {
                case SelectImportActionOption.ImportActionIntoCurrentDatabase:
                    MDBFile = Utilities.Global.Settings.MDBPath;

                    break;

                case SelectImportActionOption.ImportActionIntoNewDatabase:
                case SelectImportActionOption.ImportActionIntoExistingDatabase:
                    string filePath = "";
                    if (FileDialogForMDBFile(ref filePath))
                    {
                        MDBFile = filePath;
                    }
                    break;
            }
            if (ImportActionOption != SelectImportActionOption.ImportActionNone && MDBFile.Length > 0)
            {
                return true;
            }

            return false;
        }
        private void OnRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            buttonOk.IsEnabled = false;
            bool enableButton = false;
            ImportActionOption = SelectImportActionOption.ImportActionNone;
            switch (((RadioButton)sender).Name)
            {
                case "rbImportNewDB":
                    ImportActionOption = SelectImportActionOption.ImportActionIntoNewDatabase;
                    enableButton = true;
                    break;
                case "rbImportExistingDB":
                    ImportActionOption = SelectImportActionOption.ImportActionIntoExistingDatabase;
                    enableButton = true;
                    break;
                case "rbImportCurrentDB":
                    ImportActionOption = SelectImportActionOption.ImportActionIntoCurrentDatabase;
                    buttonOk.IsEnabled = true;
                    break;
            }
            buttonSelect.IsEnabled = enableButton;
        }
    }
}
