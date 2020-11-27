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
    public class FishingGroundRepository
    {
        public List<FishingGround> FishingGrounds{ get; set; }

        public FishingGroundRepository()
        {
            FishingGrounds = getFishingGrounds();
        }

        private List<FishingGround> getFishingGrounds()
        {
            List<FishingGround> listFishingGrounds = new List<FishingGround>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $"Select * from fishingGround";


                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        listFishingGrounds.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            FishingGround fg = new FishingGround();
                            fg.Code = dr["FishingGroundCode"].ToString();
                            fg.Name = dr["FishingGroundName"].ToString();
                            listFishingGrounds.Add(fg);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);

                }
                return listFishingGrounds;
            }
        }

        public bool Add(FishingGround fg)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into fishingGround(FishingGroundName,FishingGroundCode)
                           Values ('{fg.Name}','{fg.Code}')";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }

        public bool Update(FishingGround fg)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update fishingGround set
                                FishingGroundName = '{fg.Name}'
                            WHERE FishingGroundCode = '{fg.Code}'";
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
                var sql = $"Delete * from fishingGround where FishingGroundCode='{code}'";
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
