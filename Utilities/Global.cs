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
using Newtonsoft.Json;
using System.Windows;

namespace NSAP_ODK.Utilities
{
    public enum DBView
    {
        dbviewSummary,
        dbviewDownloadHistory,
        dbviewCalendar
    }
    public static class Global
    {

        public static event EventHandler<EntityLoadedEventArg> EntityLoading;
        public static event EventHandler<EntityLoadedEventArg> EntityLoaded;
        public static event EventHandler RequestLogIn = delegate { };

        public const string UserSettingsFilename = "settings.xml";

        public static string _KoboFormsFolder = $@"{AppDomain.CurrentDomain.BaseDirectory}Koboforms";
        public static string _DefaultSettingspath =
            AppDomain.CurrentDomain.BaseDirectory +
            "\\Settings\\" + UserSettingsFilename;

        public static string _UserSettingsPath =
            AppDomain.CurrentDomain.BaseDirectory +
            "\\Settings\\UserSettings\\" +
            UserSettingsFilename;


        private static string _msgBoxCaption = "NSAP-ODK Database";

        //public static string MDBPath = $"{AppDomain.CurrentDomain.BaseDirectory}/nsap_odk.mdb";


        //public static Stream ToStream(this string str)
        //{
        //    MemoryStream stream = new MemoryStream();
        //    StreamWriter writer = new StreamWriter(stream);
        //    writer.Write(str);
        //    writer.Flush();
        //    stream.Position = 0;
        //    return stream;
        //}
        //public static bool CancelOperation { get; set; }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        public static string MessageBoxCaption { get { return _msgBoxCaption; } }
        public static double? Add(this double? num1, double? num2)
        {
            return num1.GetValueOrDefault() + num2.GetValueOrDefault();
        }
        public static string CSVMediaSaveFolder { get; set; }
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

            if (!Directory.Exists(_KoboFormsFolder))
            {
                Directory.CreateDirectory(_KoboFormsFolder);
            }
            else
            {

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
            //ConnectionString = "Provider=Microsoft.JET.OLEDB.4.0;data source=" + MDBPath;
            ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;data source=" + MDBPath;
        }


        /// <summary>
        /// Returns a collection of strings that are derived by splitting the given source string at
        /// characters given by the 'delimiter' parameter.  However, a substring may be enclosed between
        /// pairs of the 'qualifier' character so that instances of the delimiter can be taken as literal
        /// parts of the substring.  The method was originally developed to split comma-separated text
        /// where quotes could be used to qualify text that contains commas that are to be taken as literal
        /// parts of the substring.  For example, the following source:
        ///     A, B, "C, D", E, "F, G"
        /// would be split into 5 substrings:
        ///     A
        ///     B
        ///     C, D
        ///     E
        ///     F, G
        /// When enclosed inside of qualifiers, the literal for the qualifier character may be represented
        /// by two consecutive qualifiers.  The two consecutive qualifiers are distinguished from a closing
        /// qualifier character.  For example, the following source:
        ///     A, "B, ""C"""
        /// would be split into 2 substrings:
        ///     A
        ///     B, "C"
        /// </summary>
        /// <remarks>Originally based on: https://stackoverflow.com/a/43284485/2998072</remarks>
        /// <param name="source">The string that is to be split</param>
        /// <param name="delimiter">The character that separates the substrings</param>
        /// <param name="qualifier">The character that is used (in pairs) to enclose a substring</param>
        /// <param name="toTrim">If true, then whitespace is removed from the beginning and end of each
        /// substring.  If false, then whitespace is preserved at the beginning and end of each substring.
        /// </param>
        public static List<String> SplitQualified(this String source, Char delimiter, Char qualifier,
                                    Boolean toTrim)
        {
            // Avoid throwing exception if the source is null
            if (String.IsNullOrEmpty(source))
                return new List<String> { "" };

            var results = new List<String>();
            var result = new StringBuilder();
            Boolean inQualifier = false;

            // The algorithm is designed to expect a delimiter at the end of each substring, but the
            // expectation of the caller is that the final substring is not terminated by delimiter.
            // Therefore, we add an artificial delimiter at the end before looping through the source string.
            String sourceX = source + delimiter;

            // Loop through each character of the source
            for (var idx = 0; idx < sourceX.Length; idx++)
            {
                // If current character is a delimiter
                // (except if we're inside of qualifiers, we ignore the delimiter)
                if (sourceX[idx] == delimiter && inQualifier == false)
                {
                    // Terminate the current substring by adding it to the collection
                    // (trim if specified by the method parameter)
                    results.Add(toTrim ? result.ToString().Trim() : result.ToString());
                    result.Clear();
                }
                // If current character is a qualifier
                else if (sourceX[idx] == qualifier)
                {
                    // ...and we're already inside of qualifier
                    if (inQualifier)
                    {
                        // check for double-qualifiers, which is escape code for a single
                        // literal qualifier character.
                        if (idx + 1 < sourceX.Length && sourceX[idx + 1] == qualifier)
                        {
                            idx++;
                            result.Append(sourceX[idx]);
                            continue;
                        }
                        // Since we found only a single qualifier, that means that we've
                        // found the end of the enclosing qualifiers.
                        inQualifier = false;
                        continue;
                    }
                    else
                        // ...we found an opening qualifier
                        inQualifier = true;
                }
                // If current character is neither qualifier nor delimiter
                else
                    result.Append(sourceX[idx]);
            }

            return results;
        }
        public static string[] CommandArgs { get; set; }
        public static void LoadEntities()
        {
            bool tablesUpdated = false;
            LandingSiteSamplingRepository.UpdateColumns();
            tablesUpdated = true;

            EntityLoaded?.Invoke(null, new EntityLoadedEventArg { IsStarting = true, EntityCount = 19 });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "GPS" });
            NSAPEntities.GPSViewModel = new GPSViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = NSAPEntities.GPSViewModel.Count });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "FMA" });
            NSAPEntities.FMAViewModel = new FMAViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = NSAPEntities.FMAViewModel.Count });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Engine" });
            NSAPEntities.EngineViewModel = new EngineViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = NSAPEntities.EngineViewModel.Count });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Fishing vessel" });
            NSAPEntities.FishingVesselViewModel = new FishingVesselViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = NSAPEntities.FishingVesselViewModel.Count });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Fishing ground" });
            NSAPEntities.FishingGroundViewModel = new FishingGroundViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = NSAPEntities.FishingGroundViewModel.Count });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Effort specificattion" });
            NSAPEntities.EffortSpecificationViewModel = new EffortSpecificationViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = NSAPEntities.EffortSpecificationViewModel.Count });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Gear" });
            NSAPEntities.GearViewModel = new GearViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = NSAPEntities.GearViewModel.Count });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "NSAP enumerator" });
            NSAPEntities.NSAPEnumeratorViewModel = new NSAPEnumeratorViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = NSAPEntities.NSAPEnumeratorViewModel.Count });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "NSAP region" });
            NSAPEntities.NSAPRegionViewModel = new NSAPRegionViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = NSAPEntities.NSAPRegionViewModel.Count });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Province" });
            NSAPEntities.ProvinceViewModel = new ProvinceViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = NSAPEntities.ProvinceViewModel.Count });

            EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Landing site" });
            NSAPEntities.LandingSiteViewModel = new LandingSiteViewModel();
            EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = NSAPEntities.LandingSiteViewModel.Count });

            if (GearAtLandingSiteDaysPerMonthRepository.CheckForGearLandingSiteTable())
            {
                EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Gear at landing site operations per month" });
                NSAPEntities.GearAtLandingSiteDaysPerMonthViewModel = new GearAtLandingSiteDaysPerMonthViewModel();
                EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = NSAPEntities.GearAtLandingSiteDaysPerMonthViewModel.Count });

                EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "NSAP region entities" });
                var c = NSAPEntities.NSAPRegionViewModel.SetNSAPRegionsWithEntitiesRepositories();
                EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = c });

                EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Size type" });
                NSAPEntities.SizeTypeViewModel = new SizeTypeViewModel();
                EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = NSAPEntities.SizeTypeViewModel.Count });

                EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Taxa" });
                NSAPEntities.TaxaViewModel = new TaxaViewModel();
                EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = NSAPEntities.TaxaViewModel.Count });

                EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Fish species" });
                NSAPEntities.FishSpeciesViewModel = new FishSpeciesViewModel();
                EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = NSAPEntities.FishSpeciesViewModel.Count });



                var cols = CreateTablesInAccess.GetColumnNames("notFishSpecies");
                bool proceed = cols.Contains("Name") || NotFishSpeciesRepository.UpdateTableDefinition();
                if (proceed)
                {
                    EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Not fish species" });
                    NSAPEntities.NotFishSpeciesViewModel = new NotFishSpeciesViewModel();
                    EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = NSAPEntities.NotFishSpeciesViewModel.Count });
                }

                //if (LandingSiteSamplingRepository.UpdateColumns())
                if (tablesUpdated)
                {

                    EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Landing site sampling" });
                    NSAPEntities.LandingSiteSamplingViewModel = new LandingSiteSamplingViewModel();
                    EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = NSAPEntities.LandingSiteSamplingViewModel.Count });

                    EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Landing site submission" });
                    NSAPEntities.LandingSiteSamplingSubmissionViewModel = new LandingSiteSamplingSubmissionViewModel();
                    EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = NSAPEntities.LandingSiteSamplingSubmissionViewModel.Count() });

                    EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Summary item" });
                    NSAPEntities.SummaryItemViewModel = new SummaryItemViewModel();
                    EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = NSAPEntities.SummaryItemViewModel.Count });

                    EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Koboserver" });
                    NSAPEntities.KoboServerViewModel = new KoboServerViewModel();
                    EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = NSAPEntities.KoboServerViewModel.Count() });


                    EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "Sampling day submission" });
                    NSAPEntities.SamplingDaySubmissionViewModel = new SamplingDaySubmissionViewModel();
                    EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = NSAPEntities.SamplingDaySubmissionViewModel.Count() });
                    //EntityLoading?.Invoke(null, new EntityLoadedEventArg { Name = "TotalWeightSp" });
                    //NSAPEntities.TotalWtSpViewModel = new TotalWtSpViewModel();
                    //EntityLoaded?.Invoke(null, new EntityLoadedEventArg { Count = NSAPEntities.TotalWtSpViewModel.Count() });

                    NSAPEntities.DBSummary = new DBSummary();
                    NSAPEntities.DatabaseEnumeratorSummary = new DatabaseEnumeratorSummary();
                    NSAPEntities.JSONFileViewModel = new JSONFileViewModel();
                    NSAPEntities.ODKEformVersionViewModel = new ODKEformVersionViewModel();
                    NSAPEntities.UnmatchedFieldsFromJSONFileViewModel = new UnmatchedFieldsFromJSONFileViewModel();

                    NSAPEntities.ResetEntititesCurrentIDs();
                    VesselUnloadServerRepository.ResetGroupIDState();

                    EntityLoaded?.Invoke(null, new EntityLoadedEventArg { IsEnding = true });
                }
            }
        }

        public static string Filter2DateString()
        {
            if (Filter2 != null)
            {
                DateTime date = (DateTime)Filter2;
                int m = 0;
                int d = 0;
                if (date.Month == 0)
                {
                    m = 1;
                }
                else
                {
                    m = date.Month;
                }
                if (date.Day == 0)
                {
                    d = 1;
                }
                else
                {
                    d = date.Day;
                }
                return new DateTime(((DateTime)Filter2).Year, m, d).ToString("MM/dd/yyyy");
            }
            else
            {
                return string.Empty;
            }
        }
        public static string Filter1DateString()
        {
            if (Filter1 != null)
            {
                DateTime date = (DateTime)Filter1;
                int m = 0;
                int d = 0;
                if (date.Month == 0)
                {
                    m = 1;
                }
                else
                {
                    m = date.Month;
                }
                if (date.Day == 0)
                {
                    d = 1;
                }
                else
                {
                    d = date.Day;
                }
                return new DateTime(((DateTime)Filter1).Year, m, d).ToString("MM/dd/yyyy");
            }
            else
            {
                return string.Empty;
            }
        }
        public static string FilterServerID { get; set; } = string.Empty;
        public static DateTime? Filter1 { get; set; }
        public static DateTime? Filter2 { get; set; }
        public static bool StringIsOnlyASCIILettersAndDigits(string s)
        {
            return s.All(c => (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'));
        }
        public static bool MySQLLogInCancelled { get; set; }

        public static string FilterError { get; set; }
        public static void DoAppProceed()
        {
            AppProceed = Settings != null && File.Exists(Settings.MDBPath);
            if (AppProceed)
            {
                if (CommandArgs != null && CommandArgs.Count() > 0)// && CommandArgs[0] == "filtered")
                {
                    switch (CommandArgs[0])
                    {
                        case "filtered":

                            if (CommandArgs.Count() > 1)
                            {
                                for (int x = 1; x < CommandArgs.Count(); x++)
                                {
                                    if (DateTime.TryParse(CommandArgs[x], out DateTime d))
                                    {
                                        if (x == 1)
                                        {
                                            Filter1 = d;
                                            Settings.DbFilter = d.ToString("MMM-dd-yyyy");
                                        }
                                        else
                                        {
                                            Filter2 = d;
                                            Settings.DbFilter += $" - {d.ToString("MMM-dd-yyyy")}";
                                        }
                                    }
                                    else if (int.TryParse(CommandArgs[x], out int i))
                                    {
                                        if (i >= 2000)
                                        {
                                            if (x == 1)
                                            {
                                                Filter1 = new DateTime(i, 1, 1);
                                                Settings.DbFilter = ((DateTime)Filter1).ToString("MMM-dd-yyyy");
                                            }
                                            else
                                            {
                                                Filter2 = new DateTime(i + 1, 1, 1);
                                                Settings.DbFilter += $" - {((DateTime)Filter2).ToString("MMM-dd-yyyy")}";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        AppProceed = false;
                                        FilterError = "Arguments after filtered must be valid dates";
                                        return;
                                    }
                                }
                            }
                            else if (CommandArgs.Count() == 1)
                            {
                                //if (Settings.DbFilter == null)
                                if (string.IsNullOrEmpty( Settings.DbFilter))
                                {
                                    Filter1 = new DateTime(2023, 1, 1);
                                    Settings.DbFilter = ((DateTime)Filter1).ToString("MMM-dd-yyyy");
                                }
                                else
                                {
                                    string[] arr = Settings.DbFilter.Replace(" - ", " ").Split(' ');
                                    for (int x = 0; x < arr.Count(); x++)
                                    {
                                        if (DateTime.TryParse(arr[x], out DateTime d))
                                        {
                                            if (x == 0)
                                            {
                                                Filter1 = d;
                                            }
                                            else
                                            {
                                                Filter2 = d;
                                            }
                                        }
                                        else if (int.TryParse(arr[x], out int i))
                                        {
                                            if (i >= 2000)
                                            {
                                                if (x == 0)
                                                {
                                                    Filter1 = new DateTime(i, 1, 1);
                                                }
                                                else
                                                {
                                                    Filter2 = new DateTime(i + 1, 1, 1);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            AppProceed = false;
                                            FilterError = "Arguments after filtered must be valid dates";
                                            return;
                                        }
                                    }
                                }
                            }


                            if (Filter1 != null && Filter2 != null && Filter1 > Filter2)
                            {
                                AppProceed = false;
                                FilterError = "First date in filter must be before second date";
                                return;
                            }
                            break;
                        case "server_id":
                            if (CommandArgs.Count() == 2)
                            {
                                FilterServerID = CommandArgs[1];
                            }
                            else if (CommandArgs.Count() == 1)
                            {
                                if (!string.IsNullOrEmpty(Settings.ServerFilter))
                                {
                                    FilterServerID = Settings.ServerFilter;
                                }
                            }
                            else
                            {
                                AppProceed = false;
                                FilterError = "Cannot understand filter for server ID";
                            }
                            break;
                    }
                }

                if (Settings.UsemySQL)
                {
                    RequestLogIn(null, EventArgs.Empty);

                }
                else
                {
                    MDBPath = Settings.MDBPath;
                    ConnectionString = "Provider=Microsoft.JET.OLEDB.4.0;data source=" + MDBPath;
                    ConnectionStringGrid25 = "Provider=Microsoft.JET.OLEDB.4.0;data source=" + Grid25MDBPath;
                    AppProceed = CSVFIleManager.ReadCSVXML();

                    if (Settings.JSONFolder != null && Settings.JSONFolder.Length > 0 && !Directory.Exists(Settings.JSONFolder))
                    {
                        Settings.JSONFolder = "";
                        SaveGlobalSettings();
                    }

                    if (!AppProceed)
                    {
                        Logger.Log(CSVFIleManager.XMLError);
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

        public static T Clone<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
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
