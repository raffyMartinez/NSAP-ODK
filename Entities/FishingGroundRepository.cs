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
                var sql = "Insert into fishingGround(FishingGroundName,FishingGroundCode) Values (?,?)";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    update.Parameters.Add("@fg_name", OleDbType.VarChar).Value = fg.Name;
                    update.Parameters.Add("@fg_code", OleDbType.VarChar).Value = fg.Code;
                    try
                    {
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch (OleDbException dbex)
                    {
                        switch (dbex.ErrorCode)
                        {
                            case -2147467259:
                                //database is corrupt
                                CorruptedDatabase = true;
                                break;
                        }
                        Logger.Log(dbex);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }

            return success;
        }

        public bool CorruptedDatabase { get; private set; }


        public bool Update(FishingGround fg)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                using (OleDbCommand update = conn.CreateCommand())
                {
                    update.Parameters.Add("@fgname", OleDbType.VarChar).Value = fg.Name;
                    update.Parameters.Add("@fgcode", OleDbType.VarChar).Value = fg.Code;
                    update.CommandText = @"UPDATE fishingground SET
                                           FishingGroundName=@fgname
                                           WHERE FishingGroundCode=@fgcode";
                    try
                    {
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch(OleDbException dbex)
                    {
                        Logger.Log(dbex);
                    }
                    catch(Exception ex)
                    {
                        Logger.Log(ex);
                    }
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
                
                using (OleDbCommand update = conn.CreateCommand())
                {
                    update.Parameters.Add("@code", OleDbType.VarChar).Value = code;
                    update.CommandText="Delete * from fishingGround where FishingGroundCode=@code";
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
