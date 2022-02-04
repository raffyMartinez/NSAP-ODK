using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using NSAP_ODK.Utilities;
using System.Data;
using System.Data.OleDb;
using NSAP_ODK.Entities;
namespace NSAP_ODK.NSAPMysql
{
    public static class MigrateDataToMySQL
    {
        public static void Migrate()
        {
            Global.CreateConnectionString();
            Global.LoadEntities();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                
                try
                {
                    using (var cmd = conection.CreateCommand())
                    {
                        conection.Open();
                        cmd.CommandText = "Select * from fma";
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            FMA fma = new FMA();
                            fma.FMAID = Convert.ToInt32(dr["FMAID"]);
                            fma.Name = dr["FMAName"].ToString();
                            NSAPEntities.FMAViewModel.AddRecordToRepo(fma);
                        }
                    }
                    using (var cmd = conection.CreateCommand())
                    {
                        cmd.CommandText = $@"SELECT * from nsapRegion order by Sequence";
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            NSAPRegion nsr = new NSAPRegion();
                            nsr.Code = dr["Code"].ToString();
                            nsr.Name = dr["RegionName"].ToString();
                            nsr.ShortName = dr["ShortName"].ToString();
                            nsr.Sequence = Convert.ToInt32(dr["Sequence"]);
                            NSAPEntities.NSAPRegionViewModel.AddRecordToRepo(nsr);
                        }
                    }

                    using (var cmd = conection.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * from fishingVessel";
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
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
                            NSAPEntities.FishingVesselViewModel.AddRecordToRepo(fv);
                        }
                    }




                    
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
        }
    }
}
