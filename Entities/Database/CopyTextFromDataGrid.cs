using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NSAP_ODK.Entities.Database
{
    public class CopyTextFromDataGridEventArg:EventArgs
    {
        public string Intent { get; set; }
    }
    public static class CopyTextFromDataGrid
    {
        public static event EventHandler<CopyTextFromDataGridEventArg> CopyTextFromDataGridEvent;
        public static DataGrid DataGrid { get; set; }
        public static object DataGridContext { get; set; }
        public static Task<bool>CopyTextAsync()
        {
            return Task.Run(()=>CopyText());
        }
        public static bool CopyText()
        {
            CopyTextFromDataGridEvent?.Invoke(null, new CopyTextFromDataGridEventArg { Intent = "start"});

            var smode = DataGrid.SelectionMode;
            DataGrid.SelectionMode = DataGridSelectionMode.Extended;
            DataGrid.SelectAllCells();
            DataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, DataGrid);
            var resultat = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
            //var result = (string)Clipboard.GetData(DataFormats.Text);
            DataGrid.UnselectAllCells();
            DataGrid.SelectionMode = smode;
            CopyTextFromDataGridEvent?.Invoke(null, new CopyTextFromDataGridEventArg { Intent = "end" });
            return resultat.Length>0;
        }

    }
}
