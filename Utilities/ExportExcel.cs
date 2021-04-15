using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using Microsoft.Win32;
using ClosedXML.Excel;
using System.Windows;

namespace NSAP_ODK.Utilities
{
    public static class ExportExcel
    {
        public static string ErrorMessage { get; internal set; }

        public static string GetSaveAsExcelFileName(Window owner, string excelFileToSave)
        {

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Provide excel filename to save data";
            sfd.Filter = "Excel (*.xlsx)|*.xlsx|Excel (*.xls)|*.xls";
            sfd.FilterIndex = 1;
            sfd.FileName = excelFileToSave;
            if ((bool)sfd.ShowDialog(owner))
            {
                return sfd.FileName;
                
            }
            return "";
        }
        public static bool GetSaveAsExcelFileName(Window owner, out string excelFileToSave)
        {
            excelFileToSave = "";
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Provide excel filename to save data";
            sfd.Filter = "Excel (*.xlsx)|*.xlsx|Excel (*.xls)|*.xls";
            sfd.FilterIndex = 1;
            if ((bool)sfd.ShowDialog(owner))
            {
                excelFileToSave=  sfd.FileName;
                return true;
            }
            return false;
        }

        
        public static bool ExportDatasetToExcel(DataSet dataSet, string fileName)
        {
            var wb = new XLWorkbook();
            try
            {
                foreach (DataTable dt in dataSet.Tables)
                {
                    wb.Worksheets.Add(dt, dt.TableName);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return false;
            }
            try
            {
                wb.SaveAs(fileName);
            }
            catch(System.IO.IOException)
            {
                ErrorMessage = "Target file is open. Cannot save";
                return false;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
                return false;
            }
            return true;
        }
    }
}
