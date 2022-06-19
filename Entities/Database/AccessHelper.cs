using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using NSAP_ODK.Utilities;
using System.Data;

namespace NSAP_ODK.Entities.Database
{
    public static class AccessHelper
    {

        public static string GetColumnNamesCSV(string tableName)
        {
            string csv = "";
            using (var con = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = $"Select * from {tableName}";
                    con.Open();
                    using (var reader = cmd.ExecuteReader(System.Data.CommandBehavior.SchemaOnly))
                    {
                        var table = reader.GetSchemaTable();
                        var nameCol = table.Columns["ColumnName"];
                        foreach(DataRow row in table.Rows)
                        {
                            csv += $"{row[nameCol]},";
                        }
                    }
                }
            }
            return csv.Trim(',');
        }
    }
}
