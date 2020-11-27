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
            string length = fv.Length == null ? "null" : fv.Length.ToString();
            string breadth = fv.Breadth == null ? "null" : fv.Breadth.ToString();
            string depth = fv.Depth == null ? "null" : fv.Depth.ToString();
            string vesselName = string.IsNullOrEmpty(fv.Name) ? "" : fv.Name;
            string ownerName = string.IsNullOrEmpty(fv.NameOfOwner) ? "" : fv.NameOfOwner;
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into fishingVessel (VesselID, VesselName, NameOfOwner, Length,Depth,Breadth,RegistrationNumber,Sector)
                           Values
                           ({fv.ID},'{vesselName}','{ownerName}',{length},{depth},{breadth},'{fv.RegistrationNumber}',{(int)fv.FisheriesSector})";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }

        public bool Update(FishingVessel fv)
        {
            string length = fv.Length == null ? "null" : fv.Length.ToString();
            string breadth = fv.Breadth == null ? "null" : fv.Breadth.ToString();
            string depth = fv.Depth == null ? "null" : fv.Depth.ToString();
            string vesselName = string.IsNullOrEmpty(fv.Name) ? "" : fv.Name;
            string ownerName = string.IsNullOrEmpty(fv.NameOfOwner) ? "" : fv.NameOfOwner;
            string regNo = string.IsNullOrEmpty(fv.RegistrationNumber) ? "" : fv.RegistrationNumber;
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update fishingVessel set
                                VesselName = '{vesselName}',
                                NameOfOwner='{ownerName}',
                                Length = {length},
                                Depth = {depth},
                                Breadth = {breadth},
                                RegistrationNumber = '{regNo}',
                                Sector = {(int)fv.FisheriesSector}
                            WHERE VesselID={fv.ID}";
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
                var sql = $"Delete * from fishingVessel where VesselID={id}";
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