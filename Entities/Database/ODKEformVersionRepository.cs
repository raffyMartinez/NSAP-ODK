using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using NSAP_ODK.Utilities;
using MySql.Data.MySqlClient;
using NSAP_ODK.NSAPMysql;
namespace NSAP_ODK.Entities.Database
{
    public class ODKEformVersionRepository
    {

        public List<ODKEformVersion> ODKEformVersions { get; set; }
        public ODKEformVersionRepository()
        {
            ODKEformVersions = GetODKEformVersions();
        }

        private List<ODKEformVersion>GetODKEformVersions()
        {
            List<ODKEformVersion> thisList = new List<ODKEformVersion>();
            if(Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = @"SELECT t1.form_version, Count(t1.form_version) AS n, Min(t1.datetime_submitted) AS submitted_on
                                            FROM nsap_odk.dbo_vessel_unload_1 as t1
                                            GROUP BY t1.form_version
                                            order by t1.datetime_submitted";

                        try
                        {
                            MySqlDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                ODKEformVersion item = new ODKEformVersion();
                                item.Version = dr["form_version"].ToString().Replace("Version ", "");
                                item.Count = int.Parse(dr["n"].ToString());
                                item.FirstSubmission = (DateTime)dr["submitted_on"];
                                thisList.Add(item);
                            }
                        }
                        catch(Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            else
            {
                using (var conn = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = @"SELECT form_version, Count(form_version) AS n, Min(datetime_submitted) AS submitted_on
                                            FROM dbo_vessel_unload_1
                                            GROUP BY dbo_vessel_unload_1.form_version";

                        try
                        {
                            OleDbDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                ODKEformVersion item = new ODKEformVersion();
                                item.Version = dr["form_version"].ToString().Replace("Version ", "");
                                item.Count = (int)dr["n"];
                                item.FirstSubmission = (DateTime)dr["submitted_on"];
                                thisList.Add(item);
                            }
                        }
                        catch(Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return thisList;
        }

    }
}
