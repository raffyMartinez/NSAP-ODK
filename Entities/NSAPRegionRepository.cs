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
    public class NSAPRegionRepository
    {
        public List<NSAPRegion> NSAPRegions { get; set; }

        public NSAPRegionRepository()
        {
            NSAPRegions = getNSAPRegions();
        }

        private List<NSAPRegion> getNSAPRegions()
        {
            List<NSAPRegion> listRepositories = new List<NSAPRegion>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $@"SELECT * from nsapRegion order by Sequence";

                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        listRepositories.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            NSAPRegion nsr = new NSAPRegion();
                            nsr.Code= dr["Code"].ToString();
                            nsr.Name= dr["RegionName"].ToString();
                            nsr.ShortName = dr["ShortName"].ToString();
                            nsr.Sequence = Convert.ToInt32(dr["Sequence"]);
                            listRepositories.Add(nsr);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);

                }
            }

            return listRepositories;
        }

        public bool Add(NSAPRegion nsr)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into nsapRegion (Code, RegionName, ShortName,Sequence)
                           Values 
                           ('{nsr.Code}','{nsr.Name}','{nsr.ShortName}',{nsr.Sequence})";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }

        public bool Update(NSAPRegion nsr)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update nsapRegion set RegionName = '{nsr.Name}', ShortName='{nsr.ShortName}', Sequence={nsr.Sequence}
                            WHERE Code={nsr.Code}";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }

        public bool Delete(string code)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $"Delete * from nsapRegion where Code={code}";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    try
                    {
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch (OleDbException)
                    {
                        success = false;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                        success = false;
                    }
                }
            }
            return success;
        }
    }
}
