using Npoi.Mapper;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class TrackedLandingCentroidViewModel
    {
        private IWorkbook _wkBook;

        public List<TrackedLandingCentroid> TrackedLandingCentroids { get; private set; }
        public void ReadExcelData(string fileName)
        {
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {

                    try
                    {
                        _wkBook = new XSSFWorkbook(fs);
                    }
                    catch
                    {
                        _wkBook = null;
                    }

                    // If reading fails, try to read workbook as XLS:
                    if (_wkBook == null)
                    {
                        _wkBook = new HSSFWorkbook(fs);
                    }

                    if (_wkBook.NumberOfSheets == 1)
                    {
                        var importer = new Mapper(_wkBook);
                        for (int n = 0; n < _wkBook.NumberOfSheets; n++)
                        {

                            TrackedLandingCentroids = new List<TrackedLandingCentroid>();
                            var items = importer.Take<TrackedLandingCentroid>(n);
                            foreach (var item in items.OrderByDescending(t => t.Value.Start))
                            {
                                var row = item.Value;
                                TrackedLandingCentroids.Add(row);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
