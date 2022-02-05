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
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                try
                {
                    if (false)
                    {
                        #region FMA
                        using (var cmd = conn.CreateCommand())
                        {

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
                        #endregion

                        #region nsap region
                        using (var cmd = conn.CreateCommand())
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
                        #endregion

                        #region fishing vessel
                        using (var cmd = conn.CreateCommand())
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
                        #endregion

                        #region nsap enumerator
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Select * from NSAPEnumerator";
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                NSAPEnumerator ns = new NSAPEnumerator();
                                ns.ID = Convert.ToInt32(dr["EnumeratorID"]);
                                ns.Name = dr["EnumeratorName"].ToString();
                                NSAPEntities.NSAPEnumeratorViewModel.AddRecordToRepo(ns);
                            }
                        }
                        #endregion

                        #region nsap region enumerator
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Select * from NSAPRegionEnumerator";
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                NSAPRegionEnumerator nre = new NSAPRegionEnumerator();
                                nre.RowID = Convert.ToInt32(dr["RowID"]);
                                nre.EnumeratorID = Convert.ToInt32(dr["EnumeratorID"]);
                                nre.NSAPRegion = NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(dr["NSAPRegionCode"].ToString());
                                nre.DateStart = Convert.ToDateTime(dr["DateStart"]);
                                if (DateTime.TryParse(dr["DateEnd"].ToString(), out DateTime v))
                                {
                                    nre.DateEnd = v;
                                }
                                var re = NSAPEntities.NSAPRegionViewModel.GetNSAPRegionWithEntitiesRepository(nre.NSAPRegion);
                                re.AddEnumerator(nre);
                            }
                        }
                        #endregion

                        #region provinces and municipalities
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Select * from Provinces";
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                Province p = new Province();
                                p.ProvinceID = Convert.ToInt32(dr["ProvNo"]);
                                p.ProvinceName = dr["ProvinceName"].ToString();
                                p.RegionCode = dr["NSAPRegion"].ToString();
                                p.SetMunicipalities();
                                NSAPEntities.ProvinceViewModel.AddRecordToRepo(p);
                                using (var cmd1 = conn.CreateCommand())
                                {
                                    cmd1.CommandText = $"Select * from Municipalities where ProvNo={p.ProvinceID}";
                                    var dr1 = cmd1.ExecuteReader();
                                    while (dr1.Read())
                                    {
                                        Municipality m = new Municipality();
                                        m.Province = p;
                                        m.MunicipalityID = (int)dr1["MunNo"];
                                        m.MunicipalityName = dr1["Municipality"].ToString();


                                        if (dr1["yCoord"].GetType().Name != "DBNull")
                                        {
                                            m.Latitude = Convert.ToDouble(dr1["yCoord"]);
                                        }
                                        if (dr1["xCoord"].GetType().Name != "DBNull")
                                        {
                                            m.Longitude = Convert.ToDouble(dr1["xCoord"]);
                                        }
                                        m.IsCoastal = (bool)dr1["IsCoastal"];
                                        p.Municipalities.AddRecordToRepo(m);
                                    }
                                }
                            }
                        }
                        #endregion

                        #region landing sites
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"SELECT ls.LandingSiteID, ls.LandingSiteName, 
                                    ls.Municipality, m.ProvNo, ls.Latitude, ls.Longitude, ls.Barangay
                                    FROM Municipalities as m INNER JOIN landingSite as ls ON m.MunNo = ls.Municipality";
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                Province province = NSAPEntities.ProvinceViewModel.GetProvince(Convert.ToInt32(dr["ProvNo"]));
                                LandingSite ls = new LandingSite();
                                ls.LandingSiteID = Convert.ToInt32(dr["LandingSiteId"]);
                                ls.LandingSiteName = dr["LandingSiteName"].ToString();
                                if (dr["Municipality"].ToString().Length > 0)
                                {
                                    ls.Municipality = province.Municipalities.GetMunicipality(Convert.ToInt32(dr["Municipality"]));
                                }

                                if (dr["Barangay"].GetType().Name != "DBNull")
                                {
                                    ls.Barangay = dr["Barangay"].ToString();
                                }

                                if (dr["Latitude"].GetType().Name != "DBNull")
                                {
                                    ls.Latitude = Convert.ToDouble(dr["Latitude"]);
                                }
                                if (dr["Longitude"].GetType().Name != "DBNull")
                                {
                                    ls.Longitude = Convert.ToDouble(dr["Longitude"]);
                                }
                                NSAPEntities.LandingSiteViewModel.AddRecordToRepo(ls);
                            }
                        }
                        #endregion

                        #region effort specs
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Select * from effortSpecification";
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {

                                EffortSpecification es = new EffortSpecification();
                                es.ID = Convert.ToInt32(dr["EffortSpecificationID"]);
                                es.IsForAllTypesFishing = (bool)dr["ForAllTypeOfFishing"];
                                es.Name = dr["EffortSpecification"].ToString();
                                es.ValueType = (ODKValueType)Convert.ToInt32(dr["ValueType"]);
                                NSAPEntities.EffortSpecificationViewModel.AddRecordToRepo(es);

                            }
                        }
                        #endregion

                        #region gear and gear effort specs
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Select * from gear";
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                Gear g = new Gear();
                                g.GearName = dr["GearName"].ToString();
                                g.CodeOfBaseGear = dr["GenericCode"].ToString();
                                g.IsGenericGear = (bool)dr["IsGeneric"];
                                g.Code = dr["GearCode"].ToString();
                                if (NSAPEntities.GearViewModel.AddRecordToRepo(g))
                                {
                                    g.GearEffortSpecificationViewModel = new GearEffortSpecificationViewModel(g);
                                    using (var cmd1 = conn.CreateCommand())
                                    {
                                        cmd1.Parameters.AddWithValue("@gear", g.Code);
                                        cmd1.CommandText = "Select * from GearEffortSpecification where GearCode=@gear";
                                        var dr1 = cmd1.ExecuteReader();
                                        while (dr1.Read())
                                        {
                                            GearEffortSpecification ges = new GearEffortSpecification();
                                            ges.EffortSpecification = NSAPEntities.EffortSpecificationViewModel.GetEffortSpecification(Convert.ToInt32(dr1["EffortSpec"]));
                                            ges.Gear = g;
                                            ges.RowID = Convert.ToInt32(dr1["RowId"]);
                                            g.GearEffortSpecificationViewModel.AddRecordToRepo(ges);
                                        }
                                    }
                                }

                            }
                        }
                        #endregion

                        #region fishing grounds
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Select * from fishingGround";
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                FishingGround fg = new FishingGround();
                                fg.Code = dr["FishingGroundCode"].ToString();
                                fg.Name = dr["FishingGroundName"].ToString();
                                NSAPEntities.FishingGroundViewModel.AddRecordToRepo(fg);
                            }
                        }
                        #endregion

                        #region nsap region FMA, NSAPREgion FMA Fishing grounds, fishing ground landing sites
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Select * from NSAPRegionFMA";
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                NSAPRegionFMA nrf = new NSAPRegionFMA();
                                nrf.RowID = Convert.ToInt32(dr["RowID"]);
                                nrf.NSAPRegion = NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(dr["NSAPRegion"].ToString());
                                nrf.FMA = NSAPEntities.FMAViewModel.GetFMA(Convert.ToInt32(dr["FMA"]));
                                var re = NSAPEntities.NSAPRegionViewModel.GetNSAPRegionWithEntitiesRepository(nrf.NSAPRegion);
                                if (re.AddNSAPRegionFMA(nrf))
                                {
                                    #region NSAPREgion FMA Fishing grounds
                                    using (var cmd1 = conn.CreateCommand())
                                    {
                                        cmd1.Parameters.AddWithValue("@nrf", nrf.RowID);
                                        cmd1.CommandText = "Select * from NSAPRegionFMAFishingGrounds where RegionFMA=@nrf";
                                        var dr1 = cmd1.ExecuteReader();
                                        try
                                        {
                                            while (dr1.Read())
                                            {
                                                NSAPRegionFMAFishingGround nrfg = new NSAPRegionFMAFishingGround();
                                                nrfg.RegionFMA = nrf;
                                                nrfg.FishingGroundCode = dr1["FishingGround"].ToString();
                                                nrfg.RowID = Convert.ToInt32(dr1["RowID"]);
                                                nrfg.DateStart = Convert.ToDateTime(dr1["DateStart"]);
                                                if (DateTime.TryParse(dr1["DateEnd"].ToString(), out DateTime v))
                                                {
                                                    nrfg.DateEnd = v;
                                                }
                                                if (re.AddFMAFishingGround(nrfg))
                                                {
                                                    #region fishing ground landing sites
                                                    using (var cmd2 = conn.CreateCommand())
                                                    {
                                                        cmd2.Parameters.AddWithValue("@nrfg", nrfg.RowID);
                                                        cmd2.CommandText = "Select * from NSAPRegionLandingSite where NSAPRegionFMAFishingGround=@nrfg";
                                                        var dr2 = cmd2.ExecuteReader();
                                                        while (dr2.Read())
                                                        {
                                                            NSAPRegionFMAFishingGroundLandingSite nrfls = new NSAPRegionFMAFishingGroundLandingSite();
                                                            nrfls.RowID = Convert.ToInt32(dr2["RowID"]);
                                                            nrfls.NSAPRegionFMAFishingGround = nrfg;
                                                            nrfls.LandingSite = NSAPEntities.LandingSiteViewModel.GetLandingSite(Convert.ToInt32(dr2["LandingSiteID"]));
                                                            nrfls.DateStart = Convert.ToDateTime(dr2["DateStart"]);
                                                            if (DateTime.TryParse(dr2["DateEnd"].ToString(), out DateTime x))
                                                            {
                                                                nrfls.DateEnd = x;
                                                            }
                                                            re.AddFMAFishingGroundLandingSite(nrfls);
                                                        }
                                                    }
                                                    #endregion
                                                }

                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.Log(ex);
                                        }
                                    }
                                    #endregion
                                }

                            }
                        }
                        #endregion

                        #region NSAP Region gear
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Select * from NSAPRegionGear";
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                NSAPRegionGear nrg = new NSAPRegionGear();
                                nrg.RowID = Convert.ToInt32(dr["RowID"]);
                                nrg.NSAPRegion = NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(dr["NSAPRegionCode"].ToString());
                                nrg.GearCode = dr["GearCode"].ToString();
                                nrg.DateStart = Convert.ToDateTime(dr["DateStart"]);
                                if (DateTime.TryParse(dr["DateEnd"].ToString(), out DateTime v))
                                {
                                    nrg.DateEnd = v;
                                }
                                var re = NSAPEntities.NSAPRegionViewModel.GetNSAPRegionWithEntitiesRepository(nrg.NSAPRegion);
                                re.AddGear(nrg);
                            }
                        }
                        #endregion

                        #region GPS
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Select * from gps";
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                GPS gps = new GPS();
                                gps.Code = dr["GPSCode"].ToString();
                                gps.AssignedName = dr["AssignedName"].ToString();
                                gps.Brand = dr["Brand"].ToString();
                                gps.Model = dr["Model"].ToString();
                                if (dr["DeviceType"] != null && dr["DeviceType"].ToString().Length > 0)
                                {
                                    gps.DeviceType = (DeviceType)Enum.Parse(typeof(DeviceType), dr["DeviceType"].ToString());
                                }
                                else
                                {
                                    gps.DeviceType = DeviceType.DeviceTypeGPS;
                                }
                                NSAPEntities.GPSViewModel.AddRecordToRepo(gps);
                            }
                        }
                        #endregion

                        #region engine
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Select * from engine";
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                Engine en = new Engine();
                                en.EngineID = Convert.ToInt32(dr["EngineID"]);
                                en.ManufacturerName = dr["ManufacturerName"].ToString();
                                en.ModelName = dr["ModelName"].ToString();
                                if (double.TryParse(dr["Horsepower"].ToString(), out double v))
                                {
                                    en.HorsePower = v;
                                }
                                NSAPEntities.EngineViewModel.AddRecordToRepo(en);
                            }
                        }

                        #endregion

                        #region fishing vessel
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Select * from fishingVessel";
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                FishingVessel fv = new FishingVessel();
                                fv.NameOfOwner = dr["NameOfOwner"].ToString();
                                fv.Name = dr["VesselName"].ToString();
                                if (double.TryParse(dr["length"].ToString(), out double len))
                                {
                                    fv.Length = len;
                                }
                                if (double.TryParse(dr["depth"].ToString(), out double dep))
                                {
                                    fv.Depth = dep;
                                }
                                if (double.TryParse(dr["breadth"].ToString(), out double bre))
                                {
                                    fv.Breadth = bre;
                                }
                                fv.RegistrationNumber = dr["registration_number"].ToString();
                                fv.FisheriesSector = (FisheriesSector)Enum.ToObject(typeof(FisheriesSector), Convert.ToInt32(dr["Sector"]));
                                NSAPEntities.FishingVesselViewModel.AddRecordToRepo(fv);
                            }
                        }
                        #endregion

                        #region NSAP Region Vessel
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Select * from NSAPRegionVessel";
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                NSAPRegionFishingVessel nrfv = new NSAPRegionFishingVessel();
                                nrfv.FishingVesselID = Convert.ToInt32(dr["VesselID"]);
                                nrfv.NSAPRegion = NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(dr["NSAPRegionCode"].ToString());
                                nrfv.DateStart = Convert.ToDateTime(dr["DateStart"]);
                                if (DateTime.TryParse(dr["DateEnd"].ToString(), out DateTime v))
                                {
                                    nrfv.DateEnd = v;
                                }
                                nrfv.RowID = Convert.ToInt32(dr["RowID"]);
                                var re = NSAPEntities.NSAPRegionViewModel.GetNSAPRegionWithEntitiesRepository(nrfv.NSAPRegion);
                                re.AddFishingVessel(nrfv);
                            }
                        }
                        #endregion

                        #region fishbase species
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Select * from FBSpecies";
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                FishBaseSpecies fbs = new FishBaseSpecies(dr["Genus"].ToString(), dr["Species"].ToString(), Convert.ToInt32(dr["SpecCode"]));
                                fbs.SaveToMySQL();
                            }
                        }
                        #endregion
                    }

                    #region size type
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select * from sizeTypes";
                        var dr = cmd.ExecuteReader();
                        while(dr.Read())
                        {
                            SizeType st = new SizeType {Code=dr["SizeTypeCode"].ToString(), Name = dr["SizeTypeName"].ToString() };
                            NSAPEntities.SizeTypeViewModel.AddRecordToRepo(st);
                        }
                    }
                    #endregion

                    #region ph fish
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT phFish.*, [FBSpecies].[Genus] & ' ' & [FBSpecies].[Species] AS OldName
                                             FROM phFish LEFT JOIN FBSpecies ON phFish.SpeciesID = FBSpecies.SpecCode";
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            FishSpecies fs = new FishSpecies(Convert.ToInt32(dr["RowNo"]), dr["Genus"].ToString(), dr["Species"].ToString());
                            if (int.TryParse(dr["SpeciesID"].ToString(), out int v))
                            {
                                fs.SpeciesCode = v;
                            }
                            fs.Importance = dr["Importance"].ToString();
                            fs.Family = dr["Family"].ToString();
                            fs.MainCatchingMethod = dr["MainCatchingMethod"].ToString();
                            if (double.TryParse(dr["LengthCommon"].ToString(), out double lc))
                            {
                                fs.LengthCommon = lc;
                            }
                            if (double.TryParse(dr["LengthMax"].ToString(), out double lm))
                            {
                                fs.LengthMax = lm;
                            }
                            if (dr["LengthType"].ToString().Length > 0)
                            {
                                fs.LengthType = NSAPEntities.SizeTypeViewModel.GetSizeType(dr["LengthType"].ToString());
                            }
                            fs.NameInOldFishbase = dr["OldName"].ToString().Trim(' ');
                            NSAPEntities.FishSpeciesViewModel.AddRecordToRepo(fs);
                        }
                    }
                    #endregion
                }
                catch (MySqlException msex)
                {

                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
        }
    }
}
