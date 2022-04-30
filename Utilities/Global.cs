using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Net;
using NSAP_ODK.Entities;
using NSAP_ODK.Entities.Database;

namespace NSAP_ODK.Utilities
{
    public static class Global
    {

        public static event EventHandler<EntityLoadedEventArg> EntityLoading;
        public static event EventHandler<EntityLoadedEventArg> EntityLoaded;
        public static event EventHandler RequestLogIn = delegate { };

        public const string UserSettingsFilename = "settings.xml";

        public static string _DefaultSettingspath =
            AppDomain.CurrentDomain.BaseDirectory +
            "\\Settings\\" + UserSettingsFilename;

        public static string _UserSettingsPath =
            AppDomain.CurrentDomain.BaseDirectory +
            "\\Settings\\UserSettings\\" +
            UserSettingsFilename;


        //public static string MDBPath = $"{AppDomain.CurrentDomain.BaseDirectory}/nsap_odk.mdb";

        public static bool HasInternet { get; private set; }
        public static bool HasInternetConnection()
        {
            try
            {
                if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                {
                    using (var client = new WebClient())
                    using (client.OpenRead("http://clients3.google.com/generate_204"))
                    {
                        HasInternet = true;
                        return true;
                    }
                }
                else
                {
                    HasInternet = false;
                    return false;
                }
            }
            catch
            {
                HasInternet = false;
                return false;
            }
        }
        public static string MDBPath { get; internal set; }

        public static string Grid25MDBPath = $"{AppDomain.CurrentDomain.BaseDirectory}/grid25inland.mdb";
        public static bool AppProceed { get; private set; }

        public static string ConnectionString { get; private set; }

        public static string ConnectionStringGrid25 { get; private set; }

        public static string CreateConnectionStringForGrid25()
        {
            ConnectionStringGrid25 = "Provider=Microsoft.JET.OLEDB.4.0;data source=" + Grid25MDBPath;
            return ConnectionStringGrid25;
        }
        static Global()
        {

            // if default settings exist
            if (File.Exists(_UserSettingsPath))
            {
                Settings = Settings.Read(_UserSettingsPath);
            }
            else if (File.Exists(_DefaultSettingspath))
            {
                Settings = Settings.Read(_DefaultSettingspath);
            }
            else
            {
                Logger.Log("Settings file not read");
            }
            //DoAppProceed();
        }

        public static bool IsValidXML(string xml, out string errorMessage)
        {
            errorMessage = "";
            bool success = false;

            using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
            {
                try
                {
                    success = reader.Read();
                }
                catch (XmlException xmlex)
                {
                    errorMessage = xmlex.Message;
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    errorMessage = ex.Message;
                }
            }
            return success;
        }
        public static void CreateConnectionString()
        {
            MDBPath = Settings.MDBPath;
            ConnectionString = "Provider=Microsoft.JET.OLEDB.4.0;data source=" + MDBPath;
        }

        public static void LoadEntities()
        {
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg { IsStarting = true, EntityCount = 18 });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "GPS" } );
            NSAPEntities.GPSViewModel = new GPSViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = NSAPEntities.GPSViewModel.Count });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "FMA" } );
            NSAPEntities.FMAViewModel = new FMAViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = NSAPEntities.FMAViewModel.Count });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Engine" } );
            NSAPEntities.EngineViewModel = new EngineViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg {  Count = NSAPEntities.EngineViewModel.Count });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Fishing vessel" } );
            NSAPEntities.FishingVesselViewModel = new FishingVesselViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg {  Count = NSAPEntities.FishingVesselViewModel.Count });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Fishing ground" } );
            NSAPEntities.FishingGroundViewModel = new FishingGroundViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg {  Count = NSAPEntities.FishingGroundViewModel.Count });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Effort specificattion" } );
            NSAPEntities.EffortSpecificationViewModel = new EffortSpecificationViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg {  Count = NSAPEntities.EffortSpecificationViewModel.Count });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Gear" } );
            NSAPEntities.GearViewModel = new GearViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg {  Count = NSAPEntities.GearViewModel.Count });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "NSAP enumerator" } );
            NSAPEntities.NSAPEnumeratorViewModel = new NSAPEnumeratorViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg {  Count = NSAPEntities.NSAPEnumeratorViewModel.Count });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "NSAP region" } );
            NSAPEntities.NSAPRegionViewModel = new NSAPRegionViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg {  Count = NSAPEntities.NSAPRegionViewModel.Count });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Province" } );
            NSAPEntities.ProvinceViewModel = new ProvinceViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg {  Count = NSAPEntities.ProvinceViewModel.Count });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Landing site" } );
            NSAPEntities.LandingSiteViewModel = new LandingSiteViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg {  Count = NSAPEntities.LandingSiteViewModel.Count });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "NSAP region entities" } );
            var c = NSAPEntities.NSAPRegionViewModel.SetNSAPRegionsWithEntitiesRepositories();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg {  Count = c });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Size type" } );
            NSAPEntities.SizeTypeViewModel = new SizeTypeViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg {  Count = NSAPEntities.SizeTypeViewModel.Count });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Taxa" } );
            NSAPEntities.TaxaViewModel = new TaxaViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg {  Count = NSAPEntities.TaxaViewModel.Count });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Fish species" } );
            NSAPEntities.FishSpeciesViewModel = new FishSpeciesViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg {  Count = NSAPEntities.FishSpeciesViewModel.Count });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Not fish species" } );
            NSAPEntities.NotFishSpeciesViewModel = new NotFishSpeciesViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg {  Count = NSAPEntities.NotFishSpeciesViewModel.Count });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Landing site sampling" } );
            NSAPEntities.LandingSiteSamplingViewModel = new LandingSiteSamplingViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg {  Count = NSAPEntities.LandingSiteSamplingViewModel.Count });


            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Summary item" });
            NSAPEntities.SummaryItemViewModel = new SummaryItemViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = NSAPEntities.SummaryItemViewModel.Count });



            //NSAPEntities.GearUnloadViewModel = new GearUnloadViewModel();
            //NSAPEntities.VesselUnloadViewModel = new VesselUnloadViewModel();
            //NSAPEntities.VesselEffortViewModel = new VesselEffortViewModel();
            //NSAPEntities.VesselCatchViewModel = new VesselCatchViewModel();
            //NSAPEntities.GearSoakViewModel = new GearSoakViewModel();
            //NSAPEntities.FishingGroundGridViewModel = new FishingGroundGridViewModel();
            //NSAPEntities.CatchLenFreqViewModel = new CatchLenFreqViewModel();
            //NSAPEntities.CatchLengthWeightViewModel = new CatchLengthWeightViewModel();
            //NSAPEntities.CatchLengthViewModel = new CatchLengthViewModel();
            //NSAPEntities.CatchMaturityViewModel = new CatchMaturityViewModel();
            NSAPEntities.DBSummary = new DBSummary();
            NSAPEntities.DatabaseEnumeratorSummary = new DatabaseEnumeratorSummary();
            NSAPEntities.JSONFileViewModel = new JSONFileViewModel();
            NSAPEntities.ODKEformVersionViewModel = new ODKEformVersionViewModel();

            EntityLoaded?.Invoke(null, new EntityLoadedEventArg { IsEnding = true });

        }

        public static bool MySQLLogInCancelled { get; set; }
        public static void DoAppProceed()
        {
            AppProceed = Settings != null && File.Exists(Settings.MDBPath);
            if (AppProceed)
            {
                if (Settings.UsemySQL)
                {
                    RequestLogIn(null, EventArgs.Empty);

                }
                else
                {
                    MDBPath = Settings.MDBPath;
                    ConnectionString = "Provider=Microsoft.JET.OLEDB.4.0;data source=" + MDBPath;
                    ConnectionStringGrid25 = "Provider=Microsoft.JET.OLEDB.4.0;data source=" + Grid25MDBPath;
                    AppProceed = Entities.Database.CSVFIleManager.ReadCSVXML();

                    if (Settings.JSONFolder != null && Settings.JSONFolder.Length > 0 && !Directory.Exists(Settings.JSONFolder))
                    {
                        Settings.JSONFolder = "";
                        SaveGlobalSettings();
                    }

                    if (!AppProceed)
                    {
                        Logger.Log(Entities.Database.CSVFIleManager.XMLError);
                    }
                }
            }
            else
            {
                Logger.Log("Database for inland grids and data must both be present in the application directory");
            }
        }
        public static void SetMDBPath(string path)
        {

            Settings = new Settings();
            string jsonFolder = Settings.JSONFolder;
            Settings.MDBPath = path;
            Settings.JSONFolder = jsonFolder;
            SaveGlobalSettings();
            DoAppProceed();
        }
        public static Settings Settings { get; private set; }

        public static void SaveGlobalSettings()
        {
            Settings.Save(_DefaultSettingspath);
        }
        public static void SaveUserSettings()
        {
            Settings.Save(_UserSettingsPath);
        }
        public static bool ParsedCoordinateIsValid(string coordToParse, string x_or_y, out double result)
        {
            result = 0;
            bool success = false;
            switch (x_or_y)
            {
                case "x":
                case "X":
                case "y":
                case "Y":
                    switch (x_or_y)
                    {
                        case "x":
                        case "X":
                            if (double.TryParse(coordToParse, out double v))
                            {
                                if (v >= 0 && v <= 180)
                                {
                                    result = v;
                                    success = true;
                                }
                            }
                            break;
                        case "y":
                        case "Y":
                            if (double.TryParse(coordToParse, out v))
                            {
                                if (v >= -90 && v <= 90)
                                {
                                    result = v;
                                    success = true;
                                }
                            }
                            break;
                    }
                    break;
                default:
                    throw new Exception("Error: expected value must either be x,X,y,Y");

            }


            return success;
        }
        public static bool ParsedDateIsValid(string dateToParse, out DateTime result)
        {
            bool success = false;
            result = DateTime.Now;
            if (DateTime.TryParse(dateToParse, out DateTime inDate))
            {
                if (inDate <= DateTime.Now)
                {
                    result = inDate;
                    success = true;
                }
            }
            return success;
        }
        public static Stream ToStream(this string str)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

    }
}
