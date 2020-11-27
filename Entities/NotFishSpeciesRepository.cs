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

                var sql = $@"Insert into notFishSpecies(SpeciesID, Genus, Species, Taxa, SizeIndicator, MaxSize)
                           Values ({nfs.SpeciesID},'{nfs.Genus}','{nfs.Species}','{nfs.Taxa.Code}',{sizeType},{maxSize})";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
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
                string sizeType = nfs.SizeType == null ? "null" : $"'{nfs.SizeType.Code}'";
                string maxSize = nfs.MaxSize == null ? "null" : nfs.MaxSize.ToString();
                var sql = $@"Update notFishSpecies set
                                Genus = '{nfs.Genus}',
                                Species='{nfs.Species}',
                                Taxa = '{nfs.Taxa.Code}',
                                SizeIndicator={sizeType},
                                MaxSize = {maxSize}
                            WHERE SpeciesID = {nfs.SpeciesID}";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
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