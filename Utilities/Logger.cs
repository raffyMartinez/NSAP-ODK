using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
namespace NSAP_ODK.Utilities
{
    public enum LogType
    {
        Logfile,
        ItemSets_csv,
        Gear_csv,
        Species_csv,
        EffortSpec_csv,
        SizeMeasure_csv,
        VesselName_csv,
        FMACode_csv,
        FishingGroundCode_csv,
        MajorGrid_csv,
        InlandGridCells_csv,
        FMAID_csv
    }
    public static class Logger
    {
        private static string _appVersion = $"Version: {Assembly.GetExecutingAssembly().GetName().Version.ToString()}";
        private static LogType _logType = LogType.Logfile;
        private static string _filepath = "";

        public static string LogTypeModifier { get; set; }

        public static void LogMethodName([CallerMemberName] string methodname = null, string className = null, bool isExiting = false)
        {
            if (Debugger.IsAttached)
            {
                if (isExiting)
                {
                    if (className != null)
                    {
                        Log($"exiting {className}.{methodname}");
                    }
                    else
                    {
                        Log($"exiting {methodname}");
                    }
                }
                else
                {
                    if (className != null)
                    {
                        Log($"inside {className}.{methodname}");
                    }
                    else
                    {
                        Log($"inside {methodname}");
                    }
                }
            }
        }
        public static LogType LogType
        {
            get { return _logType; }
            set
            {
                _logType = value;
                switch (_logType)
                {
                    case LogType.Logfile:
                        _filepath = $"{AppDomain.CurrentDomain.BaseDirectory}/log.txt";
                        break;

                    default:
                        break;
                }

                if (_logType != LogType.Logfile && File.Exists(_filepath))
                {
                    File.Delete(_filepath);
                }
            }
        }


        public static string FilePath
        {
            get { return _filepath; }
            set
            {
                if (value.Length > 0)
                    _filepath = value;
            }
        }

        public static void SetFilePathToDefault()
        {
            _filepath = $"{AppDomain.CurrentDomain.BaseDirectory}/log.txt";
        }

        static Logger()
        {
            SetFilePathToDefault();
        }
        public static void LogMissingCatchInfo(string s)
        {
            string missingCatchLogFile = $"{AppDomain.CurrentDomain.BaseDirectory}/log_missing_catch.txt";
            if (!File.Exists(missingCatchLogFile))
            {
                using (StreamWriter writer = new StreamWriter(missingCatchLogFile, true))
                {
                    writer.WriteLine("RowID,# missing,FormID,Version,Gear,Enumerator,Landing site,Sampling date,File name,Date processed");
                }
            }
            using (StreamWriter writer = new StreamWriter(missingCatchLogFile, true))
            {
                writer.WriteLine(s);
            }
        }

        public static void LogUploadJSONToLocalDB(string s)
        {
            string log_uploadJSON_to_local_db = $"{AppDomain.CurrentDomain.BaseDirectory}/log_uploadJSON_to_local_db.txt";
            using (StreamWriter writer = new StreamWriter(log_uploadJSON_to_local_db, true))
            {
                writer.WriteLine($"{s}\tTS:{DateTime.Now}");
            }
        }

        public static void Log(string s, bool simpleLog = false, bool addNewLine = true)
        {
            using (StreamWriter writer = new StreamWriter(_filepath, true))
            {
                if (!simpleLog)
                {
                    writer.WriteLine($"Message: {s} Date :{DateTime.Now.ToString()} Version:{_appVersion}");
                }
                else
                {
                    writer.WriteLine(s);
                }
                if (addNewLine)
                {
                    writer.WriteLine(Environment.NewLine);
                }
            }
        }

        public static void Log(string s, Exception ex)
        {
            using (StreamWriter writer = new StreamWriter(_filepath, true))
            {
                writer.WriteLine($"Log message:{s}\r\nError: {ex.Message}\r\n{ex.StackTrace}\r\n Date :{DateTime.Now.ToString()}\r\nVersion:{_appVersion}");
                writer.WriteLine(Environment.NewLine);
            }
        }

        public static void Log(Exception ex)
        {
            using (StreamWriter writer = new StreamWriter(_filepath, true))
            {
                writer.WriteLine($"Error: {ex.Message}\r\n{ex.StackTrace}\r\n Date :{DateTime.Now.ToString()}\r\nVersion:{_appVersion}");
                writer.WriteLine(Environment.NewLine);
            }
        }
    }
}