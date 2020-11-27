using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace NSAP_ODK.Utilities
{
    public static class Global
    {
        public const string UserSettingsFilename = "settings.xml";

        public static string _DefaultSettingspath =
            AppDomain.CurrentDomain.BaseDirectory +
            "\\Settings\\" + UserSettingsFilename;

        public static string _UserSettingsPath =
            AppDomain.CurrentDomain.BaseDirectory +
            "\\Settings\\UserSettings\\" +
            UserSettingsFilename;


        //public static string MDBPath = $"{AppDomain.CurrentDomain.BaseDirectory}/nsap_odk.mdb";

        public static string MDBPath { get; internal set; }

        public static string Grid25MDBPath = $"{AppDomain.CurrentDomain.BaseDirectory}/grid25inland.mdb";
        public static bool AppProceed { get; private set; }

        public static string ConnectionString { get; private set; }

        public static string ConnectionStringGrid25 { get; private set; }
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


            DoAppProceed();



        }

        private static void DoAppProceed()
        {
            AppProceed = Settings != null && File.Exists(Settings.MDBPath);
            if (AppProceed)
            {
                MDBPath = Settings.MDBPath;
                ConnectionString = "Provider=Microsoft.JET.OLEDB.4.0;data source=" + MDBPath;
                ConnectionStringGrid25 = "Provider=Microsoft.JET.OLEDB.4.0;data source=" + Grid25MDBPath;
            }
            else
            {
                Logger.Log("Database for inland grids and data must both be present in the application directory");
            }
        }
        public static void SetMDBPath(string path)
        {
            Settings = new Settings();
            Settings.MDBPath = path;
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
