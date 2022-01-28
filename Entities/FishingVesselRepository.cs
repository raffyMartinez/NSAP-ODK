using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace NSAP_ODK.Entities
{
    public class FishingVesselRepository
    {
        public List<FishingVessel> FishingVessels { get; set; }

        public FishingVesselRepository()
        {
            FishingVessels = getFishingVessels();
        }

        private List<FishingVessel> getFishingVessels()
        {
            List<FishingVessel> listFishingVessel = new List<FishingVessel>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $@"SELECT * from fishingVessel";

                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        listFishingVessel.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            FishingVessel fv = new FishingVessel();
                            fv.ID = Convert.ToInt32(dr["VesselID"]);
                            fv.Name = dr["VesselName"].ToString();
                            if (double.TryParse(dr["Length"].ToString(), out double len))
                            {
                                fv.Length = len;
                            }
                            if (double.TryParse(dr["Depth"].ToString(), out double dep))
                            {
                                fv.Depth = dep;
                            }
                            if (double.TryParse(dr["Breadth"].ToString(), out double bre))
                            {
                                fv.Breadth = bre;
                            }
                            fv.RegistrationNumber = dr["RegistrationNumber"].ToString();
                            fv.NameOfOwner = dr["NameOfOwner"].ToString();
                            fv.FisheriesSector = (FisheriesSector)Enum.ToObject(typeof(FisheriesSector), Convert.ToInt32(dr["Sector"]));
                            listFishingVessel.Add(fv);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }

            return listFishingVessel;
        }

        public bool Add(FishingVessel fv)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = @"Insert into fishingVessel (VesselID, VesselName, NameOfOwner, Length,Depth,Breadth,RegistrationNumber,Sector)
                           Values(?,?,?,?,?,?,?,?)";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    update.Parameters.Add("@id", OleDbType.Integer).Value = fv.ID;
                    if (string.IsNullOrEmpty(fv.Name))
                    {
                        update.Parameters.Add("@name", OleDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@name", OleDbType.VarChar).Value = fv.Name;
                    }
                    if (string.IsNullOrEmpty(fv.NameOfOwner))
                    {
                        update.Parameters.Add("@owner_name", OleDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@owner_name", OleDbType.VarChar).Value = fv.NameOfOwner;
                    }
                    if (fv.Length == null)
                    {
                        update.Parameters.Add("@len", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@len", OleDbType.Double).Value = fv.Length;
                    }
                    if (fv.Depth == null)
                    {
                        update.Parameters.Add("@dep", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@dep", OleDbType.Double).Value = fv.Depth;
                    }
                    if (fv.Breadth == null)
                    {
                        update.Parameters.Add("@brd", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@brd", OleDbType.Double).Value = fv.Breadth;
                    }
                    if (string.IsNullOrEmpty(fv.RegistrationNumber))
                    {
                        update.Parameters.Add("@reg_number", OleDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@reg_number", OleDbType.VarChar).Value = fv.RegistrationNumber;
                    }
                    update.Parameters.Add("@sector", OleDbType.Integer).Value = (int)fv.FisheriesSector;
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

        public bool Update(FishingVessel fv)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                using (OleDbCommand update = conn.CreateCommand())
                {

                    if (string.IsNullOrEmpty(fv.Name))
                    {
                        update.Parameters.Add("@name", OleDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@name", OleDbType.VarChar).Value = fv.Name;
                    }
                    if (string.IsNullOrEmpty(fv.NameOfOwner))
                    {
                        update.Parameters.Add("@owner_name", OleDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@owner_name", OleDbType.VarChar).Value = fv.NameOfOwner;
                    }
                    if (fv.Length == null)
                    {
                        update.Parameters.Add("@len", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@len", OleDbType.Double).Value = fv.Length;
                    }
                    if (fv.Depth == null)
                    {
                        update.Parameters.Add("@dep", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@dep", OleDbType.Double).Value = fv.Depth;
                    }
                    if (fv.Breadth == null)
                    {
                        update.Parameters.Add("@brd", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@brd", OleDbType.Double).Value = fv.Breadth;
                    }
                    if (string.IsNullOrEmpty(fv.RegistrationNumber))
                    {
                        update.Parameters.Add("@reg_number", OleDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@reg_number", OleDbType.VarChar).Value = fv.RegistrationNumber;
                    }
                    update.Parameters.Add("@sector", OleDbType.Integer).Value = (int)fv.FisheriesSector;
                    update.Parameters.Add("@id", OleDbType.Integer).Value = fv.ID;

                    update.CommandText = @"Update fishingVessel set
                                VesselName = @name,
                                NameOfOwner=@owner_name,
                                Length = @len,
                                Depth = @dep,
                                Breadth = @brd,
                                RegistrationNumber = @reg_number,
                                Sector = @sector
                                WHERE VesselID=@id";
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
                //var sql = $"Delete * from fishingVessel where VesselID={id}";
                using (OleDbCommand update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", OleDbType.Integer).Value = id;
                    update.CommandText="Delete * from fishingVessel where VesselID=@id";
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
                const string sql = "SELECT Max(VesselID) AS max_id FROM fishingVessel";
                using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                {
                    max_rec_no = (int)getMax.ExecuteScalar();
                }
            }
            return max_rec_no;
        }
    }
}