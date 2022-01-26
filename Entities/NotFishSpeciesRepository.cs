using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace NSAP_ODK.Entities
{
    internal class NotFishSpeciesRepository
    {
        public List<NotFishSpecies> ListSpeciesNotFish { get; set; }

        public NotFishSpeciesRepository()
        {
            ListSpeciesNotFish = getSpeciesNotFish();
        }

        private List<NotFishSpecies> getSpeciesNotFish()
        {
            List<NotFishSpecies> thisList = new List<NotFishSpecies>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $"Select * from notFishSpecies";

                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        thisList.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            NotFishSpecies spnf = new NotFishSpecies();
                            spnf.SpeciesID = Convert.ToInt32(dr["SpeciesID"]);
                            spnf.Genus = dr["Genus"].ToString();
                            spnf.Species = dr["Species"].ToString();
                            spnf.Taxa = NSAPEntities.TaxaViewModel.GetTaxa(dr["Taxa"].ToString());
                            if (double.TryParse(dr["MaxSize"].ToString(), out double v))
                            {
                                spnf.MaxSize = v;
                            }
                            if (dr["SizeIndicator"].ToString().Length > 0)
                            {
                                spnf.SizeType = NSAPEntities.SizeTypeViewModel.GetSizeType(dr["SizeIndicator"].ToString());
                            }
                            thisList.Add(spnf);
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

        public bool Add(NotFishSpecies nfs)
        {
            string sizeType = nfs.SizeType == null ? "null" : $"'{nfs.SizeType.Code}'";
            string maxSize = nfs.MaxSize == null ? "null" : nfs.MaxSize.ToString();
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();

                var sql = @"Insert into notFishSpecies(SpeciesID, Genus, Species, Taxa, SizeIndicator, MaxSize)
                           Values (?,?,?,?,?,?)";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    update.Parameters.Add("@id", OleDbType.Integer).Value = nfs.SpeciesID;
                    update.Parameters.Add("@genus", OleDbType.VarChar).Value = nfs.Genus;
                    update.Parameters.Add("@species", OleDbType.VarChar).Value = nfs.Species;
                    update.Parameters.Add("@taxa", OleDbType.VarChar).Value = nfs.Taxa.Code;
                    update.Parameters.Add("@indicator", OleDbType.VarChar).Value = nfs.SizeType.Code;
                    if (nfs.MaxSize == null)
                    {
                        update.Parameters.Add("@maxsize", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@maxsize", OleDbType.Double).Value = nfs.MaxSize;
                    }
                    try
                    {
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch (OleDbException dbex)
                    {
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

        public bool Update(NotFishSpecies nfs)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                using (OleDbCommand update = conn.CreateCommand())
                {

                    update.Parameters.Add("@genus", OleDbType.VarChar).Value = nfs.Genus;
                    update.Parameters.Add("@species", OleDbType.VarChar).Value = nfs.Species;
                    update.Parameters.Add("@taxa", OleDbType.VarChar).Value = nfs.Taxa.Code;
                    if (nfs.SizeType == null)
                    {
                        update.Parameters.Add("@indicator", OleDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@indicator", OleDbType.VarChar).Value = nfs.SizeType.Code;
                    }
                    if (nfs.MaxSize == null)
                    {
                        update.Parameters.Add("@maxsize", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@maxsize", OleDbType.Double).Value = nfs.MaxSize;
                    }
                    update.Parameters.Add("@id", OleDbType.Integer).Value = nfs.SpeciesID;
                    update.CommandText = @"Update notFishSpecies set
                                Genus = @genus,
                                Species=@species,
                                Taxa = @taxa,
                                SizeIndicator=@indicator,
                                MaxSize = @maxSize
                                WHERE SpeciesID = @id";
                    try
                    {
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch (OleDbException dbex)
                    {
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

        public bool Delete(int id)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $"Delete * from notFishSpecies where SpeciesID={id}";
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
        public int MaxRecordNumber()
        {
            int max_rec_no = 0;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                const string sql = "SELECT Max(SpeciesID) AS max_record_no FROM notFishSpecies";
                using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                {
                    max_rec_no = (int)getMax.ExecuteScalar();
                }
            }
            return max_rec_no;
        }
    }
}