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
using System.IO;
using NSAP_ODK.Utilities;
using swf = System.Windows.Forms;
using NSAP_ODK.Entities.Database;
using System.Windows.Threading;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for ImportSQLDumpWindow.xaml
    /// </summary>
    public partial class ImportSQLDumpWindow : Window
    {
        private string _importedSQLFile = "";
        private bool _parse_success = false;
        private int _sqlLinesCount = 0;
        private Dictionary<string, List<string>> _sqlDumpsDict = new Dictionary<string, List<string>>();
        public ImportSQLDumpWindow()
        {
            InitializeComponent();
            Closing += ImportSQLDumpWindow_Closing;
            Loaded += ImportSQLDumpWindow_Loaded;
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();

        }

        private void ImportSQLDumpWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //txtSQLDump.Text = "";
            progressLabel.Content = "";
            statusBar.Visibility = Visibility.Collapsed;
            buttonOk.IsEnabled = false;
            CreateTablesInAccess.AccessTableEvent += CreateTablesInAccess_AccessTableEvent;
        }

        private void CreateTablesInAccess_AccessTableEvent(object sender, CreateTablesInAccessEventArgs e)
        {
            switch (e.Intent)
            {
                case "dropping tables":
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              progressBar.IsIndeterminate = true;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              progressLabel.Content = $"Deleting table data";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "tables dropped":
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              progressBar.IsIndeterminate = false;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              progressLabel.Content = $"Table data deleted";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "parsing table":
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              progressBar.Value = e.CurrentTableCount;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              progressLabel.Content = $"Importing data into {e.CurrentTableName}";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "row imported":
                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              if ((e.CurrentRowCount % 10) == 0)
                              {
                                  progressLabel.Content = $"Importing data into {e.CurrentTableName}  (imported count: {e.CurrentRowCount})";
                              }

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "begin import":
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              progressBar.Maximum = e.TotalTableCount;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              progressLabel.Content = "Starting to import sql dump";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "import done":
                    progressBar.Dispatcher.BeginInvoke
                    (
                      DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                      {

                          progressBar.Value = 0;
                                              //do what you need to do on UI Thread
                                              return null;
                      }), null);
                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              progressLabel.Content = "Importing sql dump done";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
            }
        }

        private void ImportSQLDumpWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CreateTablesInAccess.AccessTableEvent -= CreateTablesInAccess_AccessTableEvent;
            this.SavePlacement();
        }

        private async void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonOk":

                    if (_sqlLinesCount > 0)
                    {
                        if (MessageBox.Show("Importing a sql file will replace all the data in your database\r\n\r\n" +
                                           "Do you wish to proceed?",
                                           "NSAP-ODK Database",
                                           MessageBoxButton.YesNo,
                                           MessageBoxImage.Information) == MessageBoxResult.Yes)
                        {
                            if (await Import())
                            {

                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("There is no text parsed from the sql dump file",
                            "NSAP-ODK Database",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                            );
                    }

                    break;
                case "buttonCancel":
                    Close();
                    break;
            }
        }
        public SelectImportActionOption ImportActionOption { get; set; }

        public string ImportIntoMDBFile { get; set; }

        private async Task<List<SQLDumpParsed>> ParseSQLDumpAsync()
        {
            return await Task.Run(() => ParseSQLDump());
        }
        private List<SQLDumpParsed> ParseSQLDump()
        {
            List<SQLDumpParsed> sqlDumps = new List<SQLDumpParsed>();
            int linesParsed = 0;
            string line;
            List<AccessColumn> columns = null;
            List<string> data_lines = null;
            SQLDumpParsed sqlDumpParsed = null;
            using (var sr = new StreamReader(_importedSQLFile))
            {
                bool in_create_table = false;
                bool in_data_lines = false;
                bool in_next_data_line = false;
                string definition_of = "Definition of";
                string create_table = "CREATE TABLE IF NOT EXISTS";
                string insert_into = "INSERT INTO";


                while ((line = sr.ReadLine()) != null)
                {
                    int start_search;
                    int end_search;
                    string table_name;


                    if (line.Contains(definition_of))
                    {
                        start_search = line.IndexOf(definition_of) + definition_of.Length;
                        //end_search = line.IndexOf('`', start_search + 1);
                        table_name = line.Substring(start_search + 1, line.Length - start_search - 1);

                        sqlDumpParsed = new SQLDumpParsed
                        {
                            MySQLTableName = table_name
                        };
                    }


                    if (line.Contains(create_table))
                    {
                        in_create_table = true;
                        columns = new List<AccessColumn>();

                    }
                    else if (in_create_table)
                    {
                        line = line.Trim();
                        if (line.Substring(0, 1) == "`")
                        {
                            var arr = line.Split(' ');

                            columns.Add(new AccessColumn { MySQLColumnName = arr[0].Trim('`'), MySQLType = arr[1] });
                        }
                        else
                        {
                            in_create_table = false;

                            sqlDumpParsed.Columns = columns;
                        }

                    }

                    if (line.Contains("Dumping data for table"))
                    {
                        in_data_lines = true;
                        data_lines = new List<string>();
                        sqlDumpParsed.DataLines = new List<string>();

                    }
                    else if (in_data_lines && line.Contains(insert_into))
                    {
                        //int line_lenght = line.Length;
                        int values_index = line.IndexOf("VALUES") + "VALUES".Length;
                        //start_search = line.IndexOf('`', insert_into.Length);
                        //end_search = line.IndexOf('`', start_search + 1);
                        //table_name = line.Substring(start_search + 1, end_search - start_search - 1);
                        //if (!_sqlDumpsDict.Keys.Contains(table_name))
                        //{
                        //    _sqlDumpsDict.Add(table_name, new List<string>());
                        //}
                        //_sqlDumpsDict[table_name].Add(line);

                        if (line.Length > values_index)
                        {
                            data_lines.Add(line.Substring(values_index, line.Length - values_index - 1));
                            in_next_data_line = false;
                        }
                        else
                        {
                            in_next_data_line = true;
                        }
                    }
                    else if (in_data_lines && line.Contains("ENABLE KEYS"))
                    {
                        in_data_lines = false;
                        in_next_data_line = false;
                        sqlDumpParsed.DataLines = data_lines;

                        sqlDumps.Add(sqlDumpParsed);


                    }
                    else if (in_next_data_line)
                    {
                        data_lines.Add(line.Trim(';'));
                    }

                    linesParsed++;
                }
            }
            _parse_success = sqlDumps?.Count > 0;
            return sqlDumps;
        }




        private async Task<bool> Import()
        {
            var sql_dump = await ParseSQLDumpAsync();
            if (_parse_success)
            {
                CreateTablesInAccess.MDBFile = ImportIntoMDBFile;


                if (ImportActionOption == SelectImportActionOption.ImportActionIntoNewDatabase && !File.Exists(ImportIntoMDBFile))
                {
                    string dat_file = $@"{AppDomain.CurrentDomain.BaseDirectory}nsap_odk.dat";
                    File.Copy(dat_file, ImportIntoMDBFile);
                }
                statusBar.Visibility = Visibility.Visible;
                if (await CreateTablesInAccess.DropTablesAsync())
                {
                    CreateTablesInAccess.ListSQLDumpParsed = sql_dump;
                    if (await CreateTablesInAccess.ImportAsync())
                    {
                        MessageBox.Show("Importing data from sql dump succeeded",
                           "NSAP-ODK Database" ,
                           MessageBoxButton.OK,
                           MessageBoxImage.Information);
                    }
                }

            }

            return _parse_success;
        }


        private bool LoadDumpToControl()
        {
            using (var sr = new StreamReader(_importedSQLFile))
            {
                string line;
                Paragraph p = new Paragraph();
                while ((line = sr.ReadLine()) != null)
                {
                    p.Inlines.Add($"{line}\r\n");
                    _sqlLinesCount++;
                }
                FlowDocument document = new FlowDocument(p);
                document.FontFamily = new FontFamily("Segoe UI");
                document.FontSize = 10;

                flowReader.Document = document;
            }

            return true;
        }
        private void onMenuClicked(object sender, RoutedEventArgs e)
        {
            switch (((MenuItem)sender).Name)
            {
                case "menuSelectSQLFile":
                    swf.OpenFileDialog ofd = new swf.OpenFileDialog();
                    ofd.Title = "Select sql file to import";
                    ofd.Filter = "SQL dump file (*.sql)|*.sql|All files (*.*)|*.*";
                    ofd.DefaultExt = "*.sql";
                    if (ofd.ShowDialog() == swf.DialogResult.OK && File.Exists(ofd.FileName))
                    {
                        _importedSQLFile = ofd.FileName;
                        LoadDumpToControl();
                        buttonOk.IsEnabled = true;
                    }
                    break;
                case "menuClose":
                    Close();
                    break;
            }
        }
    }
}
