using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using NSAP_ODK.Utilities;

namespace NSAP_ODK.Entities
{
    public class Grid25InlandLocationRepo
    {
        public List<Grid25InlandLocation> Grid25InlandLocations { get; set; }

        public Grid25InlandLocationRepo()
        {
            Grid25InlandLocations = getGrid25InlandLocations();
        }

        private List<Grid25InlandLocation> getGrid25InlandLocations()
        {
            List<Grid25InlandLocation> listCells = new List<Grid25InlandLocation>();
            var dt = new DataTable();
            if(Global.ConnectionStringGrid25==null)
            {
                Global.CreateConnectionStringForGrid25();
            }

            using (var conection = new OleDbConnection(Global.ConnectionStringGrid25))
            {
                try
                {
                    conection.Open();
                    string query = $"Select * from tblGrid25Inland";


                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        listCells.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            Grid25InlandLocation gcl = new Grid25InlandLocation();
                            Grid25GridCell gridCell = new Grid25GridCell(new UTMZone(dr["zone"].ToString()), dr["grid_name"].ToString());
                            gcl.Grid25GridCell = gridCell;
                            gcl.RowID = Convert.ToInt32(dr["RowID"]);
                            listCells.Add(gcl);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);

                }
                return listCells;
            }
        }

    }
}
