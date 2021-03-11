using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using NSAP_ODK.Utilities;
namespace NSAP_ODK.Entities.Database
{
    class VesselCatchRepository
    {
        public List<VesselCatch> VesselCatches { get; set; }

        public VesselCatchRepository()
        {
            VesselCatches = getVesselCatches();
        }
        public int MaxRecordNumber()
        {
            int max_rec_no = 0;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                const string sql = "SELECT Max(catch_id) AS max_id FROM dbo_vessel_catch";
                using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                {
                    max_rec_no = (int)getMax.ExecuteScalar();
                }
            }
            return max_rec_no;
        }
        private List<VesselCatch> getVesselCatches()
        {
            List<VesselCatch> thisList = new List<VesselCatch>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $"Select * from dbo_vessel_catch";


                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        thisList.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            VesselCatch item = new VesselCatch();
                            item.PK = (int)dr["catch_id"];
                            item.VesselUnloadID = (int)dr["v_unload_id"];
                            item.SpeciesID = string.IsNullOrEmpty(dr["species_id"].ToString()) ? null : (int?)dr["species_id"];
                            item.Catch_kg = string.IsNullOrEmpty(dr["catch_kg"].ToString()) ? null : (double?)dr["catch_kg"];
                            item.Sample_kg = string.IsNullOrEmpty(dr["samp_kg"].ToString()) ? null : (double?)dr["samp_kg"];
                            item.TaxaCode = dr["taxa"].ToString();
                            item.SpeciesText = dr["species_text"].ToString();
                            thisList.Add(item);
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

        public bool Add(VesselCatch item)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into dbo_vessel_catch(catch_id, v_unload_id,species_id, catch_kg,samp_kg,taxa,species_text)
                           Values 
                           (
                                {item.PK},
                                {item.VesselUnloadID},
                                {(item.SpeciesID == null ? "null" : item.SpeciesID.ToString())},
                                {(item.Catch_kg == null ? "null" : item.Catch_kg.ToString())},
                                {(item.Sample_kg == null ? "null" : item.Sample_kg.ToString())}, 
                                '{item.TaxaCode}',
                                '{item.SpeciesText}'
                           )";
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
                    }
                }
            }
            return success;
        }

        public bool Update(VesselCatch item)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update dbo_vessel_catch set
                                v_unload_id={item.VesselUnloadID},
                                species_id = {(item.SpeciesID == null ? "null" : item.SpeciesID.ToString())},
                                catch_kg = {(item.Catch_kg == null ? "null" : item.Catch_kg.ToString())},
                                samp_kg = {(item.Sample_kg == null ? "null" : item.Sample_kg.ToString())},
                                taxa = '{item.TaxaCode}',
                                species_text = '{item.SpeciesText}'
                            WHERE catch_id = {item.PK}";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    try
                    {
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch(OleDbException dbex)
                    {
                        //ignore
                    }
                    catch(Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return success;
        }

        public bool ClearTable()
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $"Delete * from dbo_vessel_catch";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    try
                    {
                        update.ExecuteNonQuery();
                        success = true;
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

        public bool Delete(int id)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $"Delete * from dbo_vessel_catch where catch_id={id}";
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
