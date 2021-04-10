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
            string lenType = fishSpecies.LengthType == null ? "null" : $"'{fishSpecies.LengthType.Code}'";
            string maxLength = fishSpecies.LengthMax == null ? "null" : fishSpecies.LengthMax.ToString();
            string commonLength = fishSpecies.LengthCommon == null ? "null" : fishSpecies.LengthCommon.ToString();
            string speciesFBID = fishSpecies.SpeciesCode == null ? "null" : fishSpecies.SpeciesCode.ToString();
            string importance = string.IsNullOrEmpty(fishSpecies.Importance) ? "null" : $"'{fishSpecies.Importance}'";
            string mainCatchingmethod = string.IsNullOrEmpty(fishSpecies.MainCatchingMethod) ? "null" : $"'{fishSpecies.MainCatchingMethod}'";
            bool success = false;

            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();

                var sql = $@"Insert into phFish (RowNo, Genus,Species,
                                                 SpeciesID,Importance,Family,
                                                 MainCatchingMethod,LengthCommon,
                                                 LengthMax,LengthType) Values
                            ({fishSpecies.RowNumber}, '{fishSpecies.GenericName}', '{fishSpecies.SpecificName}',
                            {speciesFBID}, {importance},'{fishSpecies.Family}',
                            {mainCatchingmethod},{commonLength}, {maxLength}, {lenType})";

                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
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
                string lenType = fishSpecies.LengthType == null ? "null" : $"'{fishSpecies.LengthType.Code}'";
                string maxLength = fishSpecies.LengthMax == null ? "null" : fishSpecies.LengthMax.ToString();
                string commonLength = fishSpecies.LengthCommon == null ? "null" : fishSpecies.LengthCommon.ToString();
                string speciesFBID = fishSpecies.SpeciesCode == null ? "null" : fishSpecies.SpeciesCode.ToString();
                string importance = string.IsNullOrEmpty(fishSpecies.Importance) ? "null" : $"'{fishSpecies.Importance}'";
                string mainCatchingmethod = string.IsNullOrEmpty(fishSpecies.MainCatchingMethod) ? "null" : $"'{fishSpecies.MainCatchingMethod}'";

                var sql = $@"Update phFish set
                                Genus = '{fishSpecies.GenericName}',
                                Species='{fishSpecies.SpecificName}',
                                Family = '{fishSpecies.Family}',
                                SpeciesID = {speciesFBID},
                                LengthType={lenType},
                                LengthMax = {maxLength},
                                LengthCommon = {commonLength},
                                Importance = {importance},
                                MainCatchingMethod = {mainCatchingmethod}
                            WHERE RowNo = {fishSpecies.RowNumber}";

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
                var sql = "Delete * from phFish Where RowNo=?";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    try
                    {
                        update.Parameters.Add(new OleDbParameter("RowNo", id));
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