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
    public class FMARepository
    {
        public List<FMA> FMAs { get; set; }

        public FMARepository()
        {
            FMAs = getFMAs();
        }

        private List<FMA> getFMAs()
        {
            List<FMA> thisList = new List<FMA>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $"Select * from fma";


                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        thisList.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            FMA fma = new FMA();
                            fma.FMAID = Convert.ToInt32( dr["FMAID"]);
                            fma.Name = dr["FMAName"].ToString();
                            thisList.Add(fma);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);

                }
                return thisList;
            }
        }
    }
}
