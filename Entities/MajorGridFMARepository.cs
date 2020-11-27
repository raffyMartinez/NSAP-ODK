using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using NSAP_ODK.Utilities;
using System.Diagnostics;
namespace NSAP_ODK.Entities
{
    public class MajorGridFMARepository
    {
        public List<MajorGridFMA> MajorGridFMAs { get; set; }

        public MajorGridFMARepository()
        {
            MajorGridFMAs = getMajorGridFMAs();
        }

        private List<MajorGridFMA> getMajorGridFMAs()
        {
            List<MajorGridFMA> thisList = new List<MajorGridFMA>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionStringGrid25))
            {
                try
                {
                    conection.Open();
                    string query = $"Select * from majorGrid_fma";


                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        thisList.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            int fmaNo = Convert.ToInt32(dr["fma"]);
                            if(fmaNo>0)
                            {
                                //Debugger.Break();
                            }

                            MajorGridFMA mgf = new MajorGridFMA
                            {
                                RowID = Convert.ToInt32(dr["RowId"].ToString()),
                                FMA = NSAPEntities.FMAViewModel.GetFMA(Convert.ToInt32(dr["fma"])),
                                MajorGridNumber = Convert.ToInt32(dr["majorGrid"]),
                                UTMZone = new UTMZone(dr["UTMZone"].ToString())
                            };
                            thisList.Add(mgf);
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
