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
using NSAP_ODK.Entities.Database;
namespace NSAP_ODK.NSAPMysql
{
    public static class MigrateDataToMySQL
    {
        public static event EventHandler MigrateEvent;
        public static Task<bool> MigrateTablesAsync()
        {
            return Task.Run(() => Migrate());
        }
        private static void ManageEvent(string intent, string tableName = null)
        {
            EventHandler h = MigrateEvent;
            string _description = null; ;
            if (h != null)
            {
                switch (tableName)
                {
                    case "fma":
                        _description = "Fisheries management area";
                        break;
                }

                switch (intent)
                {
                    case "start":
                        var ev = new MigrateDataEventArg
                        {
                            TableName = tableName,
                            Description = _description,
                            OriginalRowCount = MDBTablesRowCount.GetTableRowCount(tableName),
                            Intent = intent
                        };
                        h(null, ev);
                        break;
                    case "copying":
                        ev = new MigrateDataEventArg
                        {
                            RowCopied = true,
                            Intent = intent
                        };
                        h(null, ev);
                        break;
                    case "finished":
                        h(null, new MigrateDataEventArg { Intent = intent });
                        break;
                }

            }
        }
        public static bool Migrate()
        {
            bool success = false;
            Global.CreateConnectionString();
            Global.LoadEntities();
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                try
                {
                    if (false)
                    { }


                    #region FMA
                    ManageEvent("start", "fma");
                    using (var cmd = conn.CreateCommand())
                    {

                        cmd.CommandText = "Select * from fma";
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            FMA fma = new FMA();
                            fma.FMAID = Convert.ToInt32(dr["FMAID"]);
                            fma.Name = dr["FMAName"].ToString();
                            if (NSAPEntities.FMAViewModel.AddRecordToRepo(fma))
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }
                    #endregion

                    #region nsap region
                    ManageEvent("start", "nsapRegion");
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
                            if (NSAPEntities.NSAPRegionViewModel.AddRecordToRepo(nsr))
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }
                    NSAPEntities.NSAPRegionViewModel.SetNSAPRegionsWithEntitiesRepositories();
                    #endregion

                    #region fishing vessel
                    ManageEvent("start", "fishingVessel");
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
                            if (NSAPEntities.FishingVesselViewModel.AddRecordToRepo(fv))
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }
                    #endregion

                    #region nsap enumerator
                    ManageEvent("start", "NSAPEnumerator");
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select * from NSAPEnumerator";
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            NSAPEnumerator ns = new NSAPEnumerator();
                            ns.ID = Convert.ToInt32(dr["EnumeratorID"]);
                            ns.Name = dr["EnumeratorName"].ToString();
                            if (NSAPEntities.NSAPEnumeratorViewModel.AddRecordToRepo(ns))
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }
                    #endregion

                    #region nsap region enumerator
                    ManageEvent("start", "NSAPRegionEnumerator");
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
                            if (re.AddEnumerator(nre))
                            {
                                ManageEvent("copying");
                            }
                        }
                    }
                    #endregion

                    #region provinces and municipalities
                    ManageEvent("start", "Provinces");
                    using (var cmd = conn.CreateCommand())
                    {

                        cmd.CommandText = "Select * from Provinces";
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            bool municipalitiesCopied = false;
                            Province p = new Province();
                            p.ProvinceID = Convert.ToInt32(dr["ProvNo"]);
                            p.ProvinceName = dr["ProvinceName"].ToString();
                            p.RegionCode = dr["NSAPRegion"].ToString();
                            p.SetMunicipalities();
                            NSAPEntities.ProvinceViewModel.AddRecordToRepo(p);
                            using (var cmd1 = conn.CreateCommand())
                            {
                                municipalitiesCopied = false;
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
                                    if (p.Municipalities.AddRecordToRepo(m))
                                    {
                                        municipalitiesCopied = true;
                                    }
                                }
                            }
                            if (municipalitiesCopied)
                            {
                                ManageEvent("copying");
                            }
                        }
                    }
                    #endregion

                    #region landing sites
                    ManageEvent("start", "landingSite");
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
                            if (NSAPEntities.LandingSiteViewModel.AddRecordToRepo(ls))
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }
                    #endregion

                    #region effort specs
                    ManageEvent("start", "effortSpecification");
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
                            if (NSAPEntities.EffortSpecificationViewModel.AddRecordToRepo(es))
                            {
                                ManageEvent(intent: "copying");
                            }

                        }
                    }
                    #endregion

                    #region gear and gear effort specs
                    ManageEvent("start", "gear");
                    using (var cmd = conn.CreateCommand())
                    {
                        bool gearEffortCopied = false;
                        cmd.CommandText = "Select * from gear";
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            gearEffortCopied = false;
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
                                        if (g.GearEffortSpecificationViewModel.AddRecordToRepo(ges))
                                        {
                                            gearEffortCopied = true;
                                        }
                                    }
                                }
                            }
                            if (gearEffortCopied)
                            {
                                ManageEvent("copying");
                            }
                        }
                    }
                    #endregion

                    #region fishing grounds
                    ManageEvent("start", "fishingGround");
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select * from fishingGround";
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            FishingGround fg = new FishingGround();
                            fg.Code = dr["FishingGroundCode"].ToString();
                            fg.Name = dr["FishingGroundName"].ToString();
                            if (NSAPEntities.FishingGroundViewModel.AddRecordToRepo(fg))
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }
                    #endregion

                    #region nsap region FMA, NSAPREgion FMA Fishing grounds, fishing ground landing sites
                    ManageEvent("start", "NSAPRegionFMA");
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

                                ManageEvent(intent: "copying");
                            }

                        }
                    }
                    #endregion

                    #region NSAP Region gear
                    ManageEvent("start", "NSAPRegionGear");
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
                            if (re.AddGear(nrg))
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }
                    #endregion

                    #region GPS
                    ManageEvent("start", "gps");
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
                            if (NSAPEntities.GPSViewModel.AddRecordToRepo(gps))
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }
                    #endregion

                    #region engine
                    ManageEvent("start", "engine");
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
                            if (NSAPEntities.EngineViewModel.AddRecordToRepo(en))
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }

                    #endregion

                    #region fishing vessel
                    ManageEvent("start", "fishingVessel");
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
                            if (NSAPEntities.FishingVesselViewModel.AddRecordToRepo(fv))
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }
                    #endregion

                    #region NSAP Region Vessel
                    ManageEvent("start", "NSAPRegionVessel");
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
                            if (re.AddFishingVessel(nrfv))
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }
                    #endregion

                    #region fishbase species
                    ManageEvent("start", "FBSpecies");
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select * from FBSpecies";
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            FishBaseSpecies fbs = new FishBaseSpecies(dr["Genus"].ToString(), dr["Species"].ToString(), Convert.ToInt32(dr["SpecCode"]));
                            if (fbs.SaveToMySQL())
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }
                    #endregion

                    #region size type
                    ManageEvent("start", "sizeTypes");
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select * from sizeTypes";
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            SizeType st = new SizeType { Code = dr["SizeTypeCode"].ToString(), Name = dr["SizeTypeName"].ToString() };
                            if (NSAPEntities.SizeTypeViewModel.AddRecordToRepo(st))
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }
                    #endregion

                    #region ph fish
                    ManageEvent("start", "phFish");
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
                            if (NSAPEntities.FishSpeciesViewModel.AddRecordToRepo(fs))
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }
                    #endregion

                    #region taxa
                    ManageEvent("start", "taxa");
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select * from taxa";
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            Taxa t = new Taxa { Code = dr["TaxaCode"].ToString(), Description = dr["Description"].ToString(), Name = dr["Taxa"].ToString() };
                            NSAPEntities.TaxaViewModel.AddRecordToRepo(t);
                        }
                    }
                    #endregion

                    #region not fish species
                    ManageEvent("start", "notFishSpecies");
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select * from notFishSpecies";
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            NotFishSpecies nfs = new NotFishSpecies { Genus = dr["Genus"].ToString(), Species = dr["Species"].ToString(), SpeciesID = Convert.ToInt32(dr["SpeciesID"]) };
                            nfs.Taxa = NSAPEntities.TaxaViewModel.GetTaxa(dr["Taxa"].ToString());
                            nfs.SizeType = NSAPEntities.SizeTypeViewModel.GetSizeType(dr["SizeIndicator"].ToString());
                            if (double.TryParse(dr["MaxSize"].ToString(), out double v))
                            {
                                nfs.MaxSize = v;
                            }
                            if (NSAPEntities.NotFishSpeciesViewModel.AddRecordToRepo(nfs))
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }
                    #endregion

                    #region landing site sampling
                    ManageEvent("start", "dbo_LC_FG_sample_day");
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT dbo_LC_FG_sample_day.*, 
                                        dbo_LC_FG_sample_day_1.datetime_submitted, 
                                        dbo_LC_FG_sample_day_1.user_name, 
                                        dbo_LC_FG_sample_day_1.device_id, 
                                        dbo_LC_FG_sample_day_1.XFormIdentifier, 
                                        dbo_LC_FG_sample_day_1.DateAdded, 
                                        dbo_LC_FG_sample_day_1.FromExcelDownload, 
                                        dbo_LC_FG_sample_day_1.form_version, 
                                        dbo_LC_FG_sample_day_1.RowID, 
                                        dbo_LC_FG_sample_day_1.EnumeratorID, 
                                        dbo_LC_FG_sample_day_1.EnumeratorText
                                        FROM dbo_LC_FG_sample_day 
                                            LEFT JOIN dbo_LC_FG_sample_day_1 
                                            ON dbo_LC_FG_sample_day.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id";
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            LandingSiteSampling lss = new LandingSiteSampling { PK = Convert.ToInt32(dr["unload_day_id"]), NSAPRegionID = dr["region_id"].ToString(), SamplingDate = Convert.ToDateTime(dr["sdate"]), FMAID = Convert.ToInt32(dr["fma"]), FishingGroundID = dr["ground_id"].ToString() };
                            if (int.TryParse(dr["land_ctr_id"].ToString(), out int v))
                            {
                                lss.LandingSiteID = v;
                            }
                            lss.Remarks = dr["remarks"].ToString();
                            lss.IsSamplingDay = Convert.ToBoolean(dr["sampleday"]);
                            lss.LandingSiteText = dr["land_ctr_text"].ToString();
                            lss.DateSubmitted = dr["datetime_submitted"] == DBNull.Value ? null : (DateTime?)dr["datetime_submitted"];
                            lss.UserName = dr["user_name"].ToString();
                            lss.DeviceID = dr["device_id"].ToString();
                            lss.XFormIdentifier = dr["XFormIdentifier"].ToString();
                            lss.DateAdded = dr["DateAdded"] == DBNull.Value ? null : (DateTime?)dr["DateAdded"];
                            lss.FromExcelDownload = dr["FromExcelDownload"] == DBNull.Value ? false : (bool)dr["FromExcelDownload"];
                            lss.FormVersion = dr["form_version"].ToString();
                            lss.RowID = dr["RowID"].ToString();
                            lss.EnumeratorID = dr["EnumeratorID"] == DBNull.Value ? null : (int?)int.Parse(dr["EnumeratorID"].ToString());
                            lss.EnumeratorText = dr["EnumeratorText"].ToString();
                            if (NSAPEntities.LandingSiteSamplingViewModel.AddRecordToRepo(lss))
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }
                    #endregion

                    #region gear unload
                    ManageEvent("start", "dbo_gear_unload");
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select * from dbo_gear_unload";
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            GearUnload gu = new GearUnload { PK = Convert.ToInt32(dr["unload_gr_id"]), GearID = dr["gr_id"].ToString(), LandingSiteSamplingID = Convert.ToInt32(dr["unload_day_id"]) };
                            if (int.TryParse(dr["boats"].ToString(), out int b))
                            {
                                gu.Boats = b;
                            }
                            if (double.TryParse(dr["catch"].ToString(), out double c))
                            {
                                gu.Catch = c;
                            }
                            gu.GearUsedText = dr["gr_text"].ToString();
                            gu.Remarks = dr["remarks"].ToString();
                            if (NSAPEntities.GearUnloadViewModel.AddRecordToRepo(gu))
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }

                    #endregion

                    #region vessel unload
                    ManageEvent("start", "dbo_vessel_unload");
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT dvu.*, dvu1.* FROM dbo_vessel_unload As dvu
                                                INNER JOIN dbo_vessel_unload_1 As dvu1 ON dvu.v_unload_id = dvu1.v_unload_id;";
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            VesselUnload item = new VesselUnload();
                            item.PK = (int)dr["dvu.v_unload_id"];
                            item.GearUnloadID = (int)dr["unload_gr_id"];
                            item.IsBoatUsed = (bool)dr["is_boat_used"];
                            item.VesselID = string.IsNullOrEmpty(dr["boat_id"].ToString()) ? null : (int?)dr["boat_id"];
                            item.VesselText = dr["boat_text"].ToString();
                            item.WeightOfCatch = string.IsNullOrEmpty(dr["catch_total"].ToString()) ? null : (double?)dr["catch_total"];
                            item.WeightOfCatchSample = string.IsNullOrEmpty(dr["catch_samp"].ToString()) ? null : (double?)dr["catch_samp"];
                            item.Boxes = string.IsNullOrEmpty(dr["boxes_total"].ToString()) ? null : (int?)dr["boxes_total"];
                            item.BoxesSampled = string.IsNullOrEmpty(dr["boxes_samp"].ToString()) ? null : (int?)dr["boxes_samp"];
                            //item.RaisingFactor = dr["raising_factor"] == DBNull.Value ? null : (double?)dr["raising_factor"];
                            item.NSAPEnumeratorID = string.IsNullOrEmpty(dr["EnumeratorID"].ToString()) ? null : (int?)dr["EnumeratorID"];
                            item.EnumeratorText = dr["EnumeratorText"].ToString();

                            item.OperationIsSuccessful = (bool)dr["Success"];
                            item.OperationIsTracked = (bool)dr["Tracked"];
                            item.FishingTripIsCompleted = (bool)dr["trip_is_completed"];
                            item.ODKRowID = dr["RowID"].ToString();
                            item.DepartureFromLandingSite = string.IsNullOrEmpty(dr["DepartureLandingSite"].ToString()) ? null : (DateTime?)dr["DepartureLandingSite"];
                            item.ArrivalAtLandingSite = string.IsNullOrEmpty(dr["ArrivalLandingSite"].ToString()) ? null : (DateTime?)dr["ArrivalLandingSite"];
                            item.XFormIdentifier = dr["XFormIdentifier"].ToString();
                            item.XFormDate = dr["XFormDate"] == DBNull.Value ? null : (DateTime?)dr["xform_date"];
                            item.UserName = dr["user_name"].ToString();
                            item.DeviceID = dr["device_id"].ToString();
                            item.DateTimeSubmitted = (DateTime)dr["datetime_submitted"];
                            item.FormVersion = dr["form_version"].ToString();
                            item.GPSCode = dr["GPS"].ToString();
                            item.SamplingDate = (DateTime)dr["SamplingDate"];
                            item.Notes = dr["notes"].ToString();
                            item.DateAddedToDatabase = dr["DateAdded"] == DBNull.Value ? null : (DateTime?)dr["DateAdded"];
                            item.FromExcelDownload = (bool)dr["FromExcelDownload"];
                            if (NSAPEntities.VesselUnloadViewModel.AddRecordToRepo(item))
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }
                    #endregion

                    #region vessel catch
                    ManageEvent("start", "dbo_vessel_catch");
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select * from dbo_vessel_catch";
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            VesselCatch vc = new VesselCatch { PK = Convert.ToInt32(dr["catch_id"]), VesselUnloadID = Convert.ToInt32(dr["v_unload_id"]) };
                            if (int.TryParse(dr["species_id"].ToString(), out int v))
                            {
                                vc.SpeciesID = v;
                            }
                            if (double.TryParse(dr["catch_kg"].ToString(), out double c))
                            {
                                vc.Catch_kg = c;
                            }
                            if (double.TryParse(dr["samp_kg"].ToString(), out double s))
                            {
                                vc.Sample_kg = s;
                            }
                            vc.TaxaCode = dr["taxa"].ToString();
                            vc.SpeciesText = dr["species_text"].ToString();
                            if (NSAPEntities.VesselCatchViewModel.AddRecordToRepo(vc))
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }
                    #endregion

                    #region fishing ground grids
                    ManageEvent("start", "dbo_fg_grid");
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select * from dbo_fg_grid";
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            FishingGroundGrid fgg = new FishingGroundGrid { PK = Convert.ToInt32(dr["fg_grid_id"]), VesselUnloadID = Convert.ToInt32(dr["v_unload_id"]), UTMZoneText = dr["utm_zone"].ToString(), Grid = dr["grid25"].ToString() };
                            if (NSAPEntities.FishingGroundGridViewModel.AddRecordToRepo(fgg))
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }
                    #endregion

                    #region gear soak
                    ManageEvent("start", "dbo_gear_soak");
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select * from dbo_gear_soak";
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            GearSoak gs = new GearSoak
                            {
                                PK = Convert.ToInt32(dr["gear_soak_id"]),
                                VesselUnloadID = Convert.ToInt32(dr["v_unload_id"]),
                            };

                            if (DateTime.TryParse(dr["time_set"].ToString(), out DateTime ts))
                            {
                                gs.TimeAtSet = ts;
                            }

                            if (DateTime.TryParse(dr["time_hauled"].ToString(), out DateTime th))
                            {
                                gs.TimeAtHaul = th;
                            }

                            var p_set = dr["wpt_set"].ToString();
                            if (p_set.Length > 0)
                            {
                                gs.WaypointAtSet = p_set;
                            }

                            var p_haul = dr["wpt_haul"].ToString();
                            if (p_haul.Length > 0)
                            {
                                gs.WaypointAtSet = p_haul;
                            }

                            if (NSAPEntities.GearSoakViewModel.AddRecordToRepo(gs))
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }
                    #endregion

                    #region vessel effort
                    ManageEvent("start", "dbo_vessel_effort");
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select * from dbo_vessel_effort";
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            VesselEffort ve = new VesselEffort
                            {
                                PK = Convert.ToInt32(dr["effort_row_id"]),
                                VesselUnloadID = Convert.ToInt32(dr["v_unload_id"]),
                                EffortSpecID = Convert.ToInt32(dr["effort_spec_id"])
                            };

                            if (double.TryParse(dr["effort_value_numeric"].ToString(), out double v))
                            {
                                ve.EffortValueNumeric = v;
                            }

                            ve.EffortValueText = dr["effort_value_text"].ToString();
                            if (NSAPEntities.VesselEffortViewModel.AddRecordToRepo(ve))
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }
                    #endregion

                    #region length of catch
                    ManageEvent("start", "dbo_catch_len");
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select * from dbo_catch_len";
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            CatchLength cl = new CatchLength
                            {
                                PK = Convert.ToInt32(dr["catch_len_id"]),
                                VesselCatchID = Convert.ToInt32(dr["catch_id"]),
                                Length = Convert.ToDouble(dr["length"])
                            };

                            if (NSAPEntities.CatchLengthViewModel.AddRecordToRepo(cl))
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }
                    #endregion

                    #region len-weight of catch
                    ManageEvent("start", "dbo_catch_len_wt");
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select * from dbo_catch_len_wt";
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            CatchLengthWeight clw = new CatchLengthWeight
                            {
                                PK = Convert.ToInt32(dr["catch_len_wt_id"]),
                                VesselCatchID = Convert.ToInt32(dr["catch_id"]),
                                Length = Convert.ToDouble(dr["length"]),
                                Weight = Convert.ToDouble(dr["weight"])
                            };
                            if (NSAPEntities.CatchLengthWeightViewModel.AddRecordToRepo(clw))
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }
                    #endregion

                    #region len-freq of catch
                    ManageEvent("start", "dbo_catch_len_freq");
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select * from dbo_catch_len_freq";
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            CatchLenFreq clf = new CatchLenFreq
                            {
                                PK = Convert.ToInt32(dr["catch_len_freq_id"]),
                                VesselCatchID = Convert.ToInt32(dr["catch_id"]),
                                LengthClass = Convert.ToInt32(dr["len_class"]),
                                Frequency = Convert.ToInt32(dr["freq"])
                            };
                            if (NSAPEntities.CatchLenFreqViewModel.AddRecordToRepo(clf))
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }
                    #endregion


                    #region maturity
                    ManageEvent("start", "dbo_catch_maturity");
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select * from dbo_catch_maturity";
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            CatchMaturity cm = new CatchMaturity
                            {
                                PK = Convert.ToInt32(dr["catch_maturity_id"]),
                                VesselCatchID = Convert.ToInt32(dr["catch_id"])
                            };
                            if (double.TryParse(dr["length"].ToString(), out double l))
                            {
                                cm.Length = l;
                            }
                            if (double.TryParse(dr["weight"].ToString(), out double w))
                            {
                                cm.Weight = w;
                            }
                            cm.SexCode = dr["sex"].ToString();
                            cm.MaturityCode = dr["maturity"].ToString();
                            cm.WeightGutContent = dr["gut_content_wt"] == DBNull.Value ? null : (double?)dr["gut_content_wt"];
                            cm.GutContentCode = dr["gut_content_code"].ToString();
                            if (double.TryParse(dr["gonadWt"].ToString(), out double gw))
                            {
                                cm.GonadWeight = gw;
                            }
                            if (NSAPEntities.CatchMaturityViewModel.AddRecordToRepo(cm))
                            {
                                ManageEvent(intent: "copying");
                            }
                        }
                    }
                    #endregion

                    success = true;
                    ManageEvent(intent: "finished");

                }


                catch (MySqlException msex)
                {

                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
            return success;
        }
    }
}
