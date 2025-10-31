using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Utilities;
using System.Data;
using System.Data.OleDb;
using MySql.Data.MySqlClient;
using NSAP_ODK.NSAPMysql;
using NSAP_ODK.Entities.Database;


namespace NSAP_ODK.Entities
{
    class NSAPEnumeratorRepository
    {
        public List<NSAPEnumerator> NSAPEnumerators { get; set; }

        public NSAPEnumeratorRepository()
        {
            NSAPEnumerators = getNSAPEnumerators();
        }

        public static List<OrphanedEnumeratorFromCarrierLandings> GetOrphanedEnumeratorFromCarrierLandings()
        {
            List<OrphanedEnumeratorFromCarrierLandings> list = new List<OrphanedEnumeratorFromCarrierLandings>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@sampling_type", "cbl");
                        cmd.CommandText = @"SELECT 
                                                nsapRegion.Code AS region_code, 
                                                fma.FMAID as fma_id, 
                                                landingSite.LandingSiteName, 
                                                dbo_LC_FG_sample_day_1.EnumeratorText
                                            FROM 
                                                fma 
                                            INNER JOIN (
                                                landingSite RIGHT JOIN 
                                                (nsapRegion INNER JOIN 
                                                (dbo_LC_FG_sample_day INNER JOIN 
                                                dbo_LC_FG_sample_day_1 ON 
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) ON 
                                                nsapRegion.Code = dbo_LC_FG_sample_day.region_id) ON 
                                                landingSite.LandingSiteID = dbo_LC_FG_sample_day.land_ctr_id) ON 
                                                fma.FMAID = dbo_LC_FG_sample_day.fma
                                            WHERE 
                                                dbo_LC_FG_sample_day.type_of_sampling=@sampling_type AND 
                                                dbo_LC_FG_sample_day.land_ctr_id Is Null
                                            GROUP BY 
                                                nsapRegion.Code, 
                                                fma.FMAID, 
                                                landingSite.LandingSiteName, 
                                                dbo_LC_FG_sample_day_1.EnumeratorText
                                            HAVING
                                                dbo_LC_FG_sample_day_1.EnumeratorText Is Not Null";
                        try
                        {
                            con.Open();
                            var dr = cmd.ExecuteReader();
                            while(dr.Read())
                            {
                                
                                OrphanedEnumeratorFromCarrierLandings oecl = new OrphanedEnumeratorFromCarrierLandings
                                {
                                    ForReplacement=false,
                                    Region = NSAPEntities.NSAPRegionViewModel.GetNSAPRegion( (string)dr["region_code"]),
                                    FMA = NSAPEntities.FMAViewModel.GetFMA( (int)dr["fma_id"]),
                                    Enumerator =(string)dr["EnumeratorText"],
                                    LandingSite = dr["LandingSiteName"]==DBNull.Value?string.Empty:(string)dr["LandingSiteName"]
                                };
                                list.Add(oecl);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return list;
        }
        public static List<NSAPRegionEnumerator> GetNSAPRegionEnumerators()
        {
            List<NSAPRegionEnumerator> nsapRegionEns = new List<NSAPRegionEnumerator>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "Select * from NSAPRegionEnumerator";
                        con.Open();
                        try
                        {
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                NSAPRegionEnumerator nren = new NSAPRegionEnumerator
                                {
                                    RowID = (int)dr["RowID"],
                                    NSAPRegionCode = dr["NSAPRegionCode"].ToString(),
                                    EnumeratorID = (int)dr["EnumeratorID"],
                                    DateStart = (DateTime)dr["DateStart"]
                                };
                                if (dr["DateEnd"] != DBNull.Value)
                                {
                                    nren.DateEnd = (DateTime)dr["DateEnd"];
                                }
                                if (dr["DateFirstSampling"] != DBNull.Value)
                                {
                                    nren.DateFirstSampling = (DateTime)dr["DateFirstSampling"];
                                }
                                nsapRegionEns.Add(nren);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return nsapRegionEns;
        }
        private List<NSAPEnumerator> getFromMySQL()
        {
            List<NSAPEnumerator> thisList = new List<NSAPEnumerator>();

            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Select * from nsap_enumerators";
                    conn.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        NSAPEnumerator ns = new NSAPEnumerator();
                        ns.ID = Convert.ToInt32(dr["enumerator_id"]);
                        ns.Name = dr["enumerator_name"].ToString();
                        thisList.Add(ns);
                    }
                }
            }
            return thisList;
        }
        private List<NSAPEnumerator> getNSAPEnumerators()
        {
            List<NSAPEnumerator> listEnumerators = new List<NSAPEnumerator>();
            if (Global.Settings.UsemySQL)
            {
                listEnumerators = getFromMySQL();
            }
            else
            {
                var dt = new DataTable();
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    try
                    {
                        conection.Open();
                        string query = $@"SELECT * from NSAPEnumerator";

                        var adapter = new OleDbDataAdapter(query, conection);
                        adapter.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            listEnumerators.Clear();
                            foreach (DataRow dr in dt.Rows)
                            {
                                NSAPEnumerator ns = new NSAPEnumerator();
                                ns.ID = Convert.ToInt32(dr["EnumeratorID"]);
                                ns.Name = dr["EnumeratorName"].ToString();
                                listEnumerators.Add(ns);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);

                    }
                }
            }

            return listEnumerators;
        }

        private bool AddToMySQl(NSAPEnumerator ns)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = ns.ID;
                    update.Parameters.Add("@name", MySqlDbType.VarChar).Value = ns.Name;
                    update.CommandText = "Insert into nsap_enumerators (enumerator_id, enumerator_name) Values (@id,@name)";
                    conn.Open();
                    try
                    {
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException msex)
                    {
                        switch (msex.ErrorCode)
                        {
                            case -2147467259:
                                //duplicated unique index error
                                break;
                            default:
                                Logger.Log(msex);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }

                }
            }
            return success;
        }
        public bool Add(NSAPEnumerator ns)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = AddToMySQl(ns);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    var sql = "Insert into NSAPEnumerator (EnumeratorID, EnumeratorName) Values (?,?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@enumerator_id", OleDbType.Integer).Value = ns.ID;
                        update.Parameters.Add("@enumertor_name", OleDbType.VarChar).Value = ns.Name;
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
            }
            return success;
        }
        private bool UpdateInMySQL(NSAPEnumerator ns)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@enumerator_name", MySqlDbType.VarChar).Value = ns.Name;
                    update.Parameters.Add("@enumerator_id", MySqlDbType.Int32).Value = ns.ID;
                    update.CommandText = "Update nsap_enumerators set enumerator_name = @enumerator_name WHERE enumerator_id = @enumerator_id";

                    try
                    {
                        conn.Open();
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException msx)
                    {
                        Logger.Log(msx);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return success;
        }
        public bool Update(NSAPEnumerator ns)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = UpdateInMySQL(ns);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    using (OleDbCommand update = conn.CreateCommand())
                    {
                        update.Parameters.Add("@enumerator_name", OleDbType.VarChar).Value = ns.Name;
                        update.Parameters.Add("@enumerator_id", OleDbType.Integer).Value = ns.ID;
                        update.CommandText = "Update NSAPENumerator set EnumeratorName = @enumerator_name WHERE EnumeratorID = @enumerator_id";
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
            }
            return success;
        }

        private bool DeleteInMySQL(int id)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    cmd.CommandText = "Delete from nsap_enumerators where enumerator_id = @id";
                    try
                    {
                        conn.Open();
                        success = cmd.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException msex)
                    {
                        Logger.Log(msex);
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
            if (Global.Settings.UsemySQL)
            {
                success = DeleteInMySQL(id);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    using (OleDbCommand update = conn.CreateCommand())
                    {
                        update.Parameters.Add("@id", OleDbType.Integer).Value = id;
                        update.CommandText = "Delete * from NSAPENumerator where EnumeratorID=@id";
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
            }
            return success;
        }
        public int MaxRecordNumber()
        {
            int max_rec_no = 0;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = "SELECT Max(enumerator_id) AS max_id FROM nsap_enumerators";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(EnumeratorID) AS max_record_no FROM NSAPENumerator";
                    using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                    {
                        max_rec_no = (int)getMax.ExecuteScalar();
                    }
                }
            }
            return max_rec_no;
        }

    }
}
