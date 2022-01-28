using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace NSAP_ODK.Entities
{
    internal class FishSpeciesRepository
    {
        public List<FishSpecies> Specieses { get; set; }
        private int _rowNo { get; set; }

        public FishSpeciesRepository()
        {
            Specieses = getSpecies();
        }

        private List<FishSpecies> getSpecies()
        {
            int rowNo = 0;
            List<FishSpecies> listSpecies = new List<FishSpecies>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    //string query = $@"SELECT * from phFish";
                    var query = "SELECT phFish.*, [FBSpecies].[Genus] & ' ' & [FBSpecies].[Species] AS OldName FROM phFish LEFT JOIN FBSpecies ON phFish.SpeciesID = FBSpecies.SpecCode";

                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {

                        listSpecies.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            rowNo = Convert.ToInt32(dr["RowNo"]);
                            if (rowNo > _rowNo)
                            {
                                _rowNo = rowNo;
                            }
                            FishSpecies sp = new FishSpecies(rowNo, dr["Genus"].ToString(), dr["Species"].ToString());
                            if (dr["SpeciesID"].ToString().Length > 0)
                            {
                                sp.SpeciesCode = Convert.ToInt32(dr["SpeciesID"]);
                            }
                            sp.Importance = dr["Importance"].ToString();
                            sp.MainCatchingMethod = dr["MainCatchingMethod"].ToString();
                            sp.Family = dr["Family"].ToString();
                            if (dr["LengthCommon"].ToString().Length > 0)
                            {
                                sp.LengthCommon = Convert.ToDouble(dr["LengthCommon"]);
                            }
                            if (dr["LengthMax"].ToString().Length > 0)
                            {
                                sp.LengthMax = Convert.ToDouble(dr["LengthMax"]);
                            }
                            if (dr["LengthType"].ToString().Length > 0)
                            {
                                sp.LengthType = NSAPEntities.SizeTypeViewModel.GetSizeType(dr["LengthType"].ToString());
                            }
                            sp.NameInOldFishbase = dr["OldName"].ToString().Trim(' ');
                            listSpecies.Add(sp);
                            //break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }

            return listSpecies;
        }

        public bool GetSpeciesListWithSizeIndicator()
        {
            return false;
        }

        public bool Add(FishSpecies fishSpecies)
        {
            bool success = false;

            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();

                var sql = @"Insert into phFish (RowNo, Genus,Species,
                                                 SpeciesID,Importance,Family,
                                                 MainCatchingMethod,LengthCommon,
                                                 LengthMax,LengthType) Values
                                                 (?,?,?,?,?,?,?,?,?,?)";

                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    update.Parameters.Add("@row_no", OleDbType.Integer).Value = fishSpecies.RowNumber;
                    update.Parameters.Add("@genus", OleDbType.VarChar).Value = fishSpecies.GenericName;
                    update.Parameters.Add("@species", OleDbType.VarChar).Value = fishSpecies.SpecificName;
                    if (fishSpecies.SpeciesCode == null)
                    {
                        update.Parameters.Add("@sp_code", OleDbType.Integer).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@sp_code", OleDbType.Integer).Value = fishSpecies.SpeciesCode;
                    }
                    if (fishSpecies.Importance == null)
                    {
                        update.Parameters.Add("@importance", OleDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@importance", OleDbType.VarChar).Value = fishSpecies.Importance;
                    }
                    update.Parameters.Add("@family", OleDbType.VarChar).Value = fishSpecies.Family;
                    if (fishSpecies.MainCatchingMethod == null)
                    {
                        update.Parameters.Add("@catch_method", OleDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@catch_method", OleDbType.VarChar).Value = fishSpecies.MainCatchingMethod;
                    }
                    if (fishSpecies.LengthCommon == null)
                    {
                        update.Parameters.Add("@len_common", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@len_common", OleDbType.Double).Value = fishSpecies.LengthCommon;
                    }
                    if (fishSpecies.LengthMax == null)
                    {
                        update.Parameters.Add("@len_max", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@len_max", OleDbType.Double).Value = fishSpecies.LengthMax;
                    }
                    if (fishSpecies.LengthType == null)
                    {
                        update.Parameters.Add("@len_type", OleDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@len_type", OleDbType.VarChar).Value = fishSpecies.LengthType.Code;
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



        public bool Update(FishSpecies fishSpecies)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();


                using (OleDbCommand update = conn.CreateCommand())
                {

                    update.Parameters.Add("@genus", OleDbType.VarChar).Value = fishSpecies.GenericName;
                    update.Parameters.Add("@species", OleDbType.VarChar).Value = fishSpecies.SpecificName;
                    if (fishSpecies.SpeciesCode == null)
                    {
                        update.Parameters.Add("@sp_code", OleDbType.Integer).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@sp_code", OleDbType.Integer).Value = fishSpecies.SpeciesCode;
                    }
                    update.Parameters.Add("@family", OleDbType.VarChar).Value = fishSpecies.Family;
                    if (fishSpecies.LengthType == null)
                    {
                        update.Parameters.Add("@len_type", OleDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@len_type", OleDbType.VarChar).Value = fishSpecies.LengthType.Code;
                    }
                    if (fishSpecies.LengthMax == null)
                    {
                        update.Parameters.Add("@len_max", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@len_max", OleDbType.Double).Value = fishSpecies.LengthMax;
                    }
                    if (fishSpecies.LengthCommon == null)
                    {
                        update.Parameters.Add("@len_common", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@len_common", OleDbType.Double).Value = fishSpecies.LengthCommon;
                    }
                    if (fishSpecies.Importance == null)
                    {
                        update.Parameters.Add("@importance", OleDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@importance", OleDbType.VarChar).Value = fishSpecies.Importance;
                    }

                    if (fishSpecies.MainCatchingMethod == null)
                    {
                        update.Parameters.Add("@catch_method", OleDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@catch_method", OleDbType.VarChar).Value = fishSpecies.MainCatchingMethod;
                    }
                    update.Parameters.Add("@row_no", OleDbType.Integer).Value = fishSpecies.RowNumber;

                    update.CommandText = @"Update phFish set
                                Genus = @genus,
                                Species=@species,
                                SpeciesID = @sp_code,
                                Family = @family,
                                LengthType=@len_type,
                                LengthMax = @len_max,
                                LengthCommon = @len_common,
                                Importance = @importance,
                                MainCatchingMethod = @catch_method
                            WHERE RowNo = @row_no";

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
                var sql = "Delete * from phFish Where RowNo=?";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    update.Parameters.Add(new OleDbParameter("RowNo", id));
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
                const string sql = "SELECT Max(RowNo) AS max_id FROM phFish";
                using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                {
                    max_rec_no = (int)getMax.ExecuteScalar();
                }
            }
            return max_rec_no;
        }
    }
}