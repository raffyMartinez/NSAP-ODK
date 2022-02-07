using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using NSAP_ODK.Utilities;
namespace NSAP_ODK.Entities
{
    public static class MDBTablesRowCount
    {
        public static int GetTableRowCount(string tableName)
        {
            
            int n = 0;
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = conection.CreateCommand())
                {
                    cmd.CommandText = $"Select count(*) from {tableName}";
                    try
                    {
                        conection.Open();
                        n = (int)cmd.ExecuteScalar();
                    }
                    catch (OleDbException oex)
                    {
                        Logger.Log(oex);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return n;
        }
    }
}
