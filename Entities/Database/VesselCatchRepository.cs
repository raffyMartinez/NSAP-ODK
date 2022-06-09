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
    class VesselCatchRepository
    {
        public List<VesselCatch> VesselCatches { get; set; }

        public VesselCatchRepository(VesselUnload vu)
        {
            VesselCatches = getVesselCatches(vu);
        }
        public VesselCatchRepository(bool isNew=false   )
        {
            if (!isNew)
            {
                VesselCatches = getVesselCatches();
            }
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
                        cmd.CommandText = "SELECT Max(catch_id) AS max_id FROM dbo_vessel_catch";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(catch_id) AS max_id FROM dbo_vessel_catch";
                    using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                    {
                        max_rec_no = (int)getMax.ExecuteScalar();
                    }
                }
            }
            return max_rec_no;
        }
        private List<VesselCatch> getFromMySQL(VesselUnload vu = null)
        {
            List<VesselCatch> thisList = new List<VesselCatch>();
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = "Select * from dbo_vessel_catch";
                    if (vu != null)
                    {
                        cmd.Parameters.AddWithValue("@parentID", vu.PK);
                        cmd.CommandText = "Select * from dbo_vessel_catch where v_unload_id=@parentID";
                    }

                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        VesselCatch item = new VesselCatch();
                        item.Parent = vu;
                        item.PK = (int)dr["catch_id"];
                        item.VesselUnloadID = (int)dr["v_unload_id"];
                        item.SpeciesID = string.IsNullOrEmpty(dr["species_id"].ToString()) ? null : (int?)dr["species_id"];
                        item.Catch_kg = string.IsNullOrEmpty(dr["catch_kg"].ToString()) ? null : (double?)dr["catch_kg"];
                        item.Sample_kg = string.IsNullOrEmpty(dr["samp_kg"].ToString()) ? null : (double?)dr["samp_kg"];
                        item.TaxaCode = dr["taxa"].ToString();
                        item.SpeciesText = dr["species_text"].ToString();
                        item.CatchLenFreqViewModel = new CatchLenFreqViewModel(item);
                        item.CatchLengthViewModel = new CatchLengthViewModel(item);
                        item.CatchLengthWeightViewModel = new CatchLengthWeightViewModel(item);
                        item.CatchMaturityViewModel = new CatchMaturityViewModel(item);
                        thisList.Add(item);
                    }
                }
            }
            return thisList;
        }
        private List<VesselCatch> getVesselCatches(VesselUnload vu = null)
        {
            List<VesselCatch> thisList = new List<VesselCatch>();
            if (Global.Settings.UsemySQL)
            {
                thisList = getFromMySQL(vu);
            }
            else
            {
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = conection.CreateCommand())
                    {
                        try
                        {
                            conection.Open();
                            cmd.CommandText = "Select * from dbo_vessel_catch";

                            if (vu != null)
                            {
                                cmd.Parameters.AddWithValue("@parentID", vu.PK);
                                cmd.CommandText = "Select * from dbo_vessel_catch where v_unload_id=@parentID";
                            }

                            thisList.Clear();
                            OleDbDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                VesselCatch item = new VesselCatch();
                                item.Parent = vu;
                                item.PK = (int)dr["catch_id"];
                                item.VesselUnloadID = (int)dr["v_unload_id"];
                                item.SpeciesID = string.IsNullOrEmpty(dr["species_id"].ToString()) ? null : (int?)dr["species_id"];
                                item.Catch_kg = string.IsNullOrEmpty(dr["catch_kg"].ToString()) ? null : (double?)dr["catch_kg"];
                                item.Sample_kg = string.IsNullOrEmpty(dr["samp_kg"].ToString()) ? null : (double?)dr["samp_kg"];
                                item.TaxaCode = dr["taxa"].ToString();
                                item.SpeciesText = dr["species_text"].ToString();
                                item.CatchLenFreqViewModel = new CatchLenFreqViewModel(item);
                                item.CatchLengthViewModel = new CatchLengthViewModel(item);
                                item.CatchLengthWeightViewModel = new CatchLengthWeightViewModel(item);
                                item.CatchMaturityViewModel = new CatchMaturityViewModel(item);
                                thisList.Add(item);
                            }

                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);

                        }
                    }

                }
            }
            return thisList;
        }
        private bool AddToMySQL(VesselCatch item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@pk", MySqlDbType.Int32).Value = item.PK;

                    update.Parameters.Add("@parent_id", MySqlDbType.Int32).Value = item.VesselUnloadID;

                    if (item.SpeciesID == null)
                    {
                        update.Parameters.Add("@species_id", MySqlDbType.Int32).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@species_id", MySqlDbType.Int32).Value = item.SpeciesID;
                    }

                    if (item.Catch_kg == null)
                    {
                        update.Parameters.Add("@catch_kg", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@catch_kg", MySqlDbType.Double).Value = item.Catch_kg;
                    }

                    if (item.Sample_kg == null)
                    {
                        update.Parameters.Add("@sample_kg", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@sample_kg", MySqlDbType.Double).Value = item.Sample_kg;
                    }

                    update.Parameters.Add("@taxa", MySqlDbType.VarChar).Value = item.TaxaCode;

                    if (item.SpeciesText == null)
                    {
                        update.Parameters.Add("@species_text", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@species_text", MySqlDbType.VarChar).Value = item.SpeciesText;
                    }

                    update.CommandText = @"Insert into dbo_vessel_catch(catch_id, v_unload_id, species_id, catch_kg, samp_kg, taxa, species_text)
                            Values (@pk, @parent_id, @species_id, @catch_kg, @sample_kg, @taxa, @species_text)";
                    try
                    {
                        conn.Open();
                        success = update.ExecuteNonQuery() > 0;
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
        public bool Add(VesselCatch item)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = AddToMySQL(item);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    var sql = @"Insert into dbo_vessel_catch(catch_id, v_unload_id, species_id, catch_kg, samp_kg, taxa, species_text)
                            Values (?, ?, ?, ?, ?, ?, ?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@pk", OleDbType.Integer).Value = item.PK;
                        update.Parameters.Add("@parent_id", OleDbType.Integer).Value = item.VesselUnloadID;
                        if (item.SpeciesID == null)
                        {
                            update.Parameters.Add("@species_id", OleDbType.Integer).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@species_id", OleDbType.Integer).Value = item.SpeciesID;
                        }

                        if (item.Catch_kg == null)
                        {
                            update.Parameters.Add("@catch_kg", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@catch_kg", OleDbType.Double).Value = item.Catch_kg;
                        }

                        if (item.Sample_kg == null)
                        {
                            update.Parameters.Add("@sample_kg", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@sample_kg", OleDbType.Double).Value = item.Sample_kg;
                        }
                        update.Parameters.Add("@taxa", OleDbType.VarChar).Value = item.TaxaCode;
                        if (item.SpeciesText == null)
                        {
                            update.Parameters.Add("@species_text", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@species_text", OleDbType.VarChar).Value = item.SpeciesText;
                        }

                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException dbex)
                        {
                            //Console.WriteLine($"item pk is {item.PK}");
                            switch (dbex.ErrorCode)
                            {
                                case -2147467259:
                                    //error because of duplicated key or index
                                    break;
                                default:
                                    Logger.Log(dbex);
                                    break;
                            }
                            //Logger.Log(dbex);
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
        private bool UpdateMySQL(VesselCatch item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Parameters.Add("@v_unload_id", MySqlDbType.Int32).Value = item.Parent.PK;

                    if (item.SpeciesID == null)
                    {
                        cmd.Parameters.Add("@species_id", MySqlDbType.Int32).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@species_id", MySqlDbType.Int32).Value = item.SpeciesID;
                    }

                    if (item.Catch_kg == null)
                    {
                        cmd.Parameters.Add("@catch_kg", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@catch_kg", MySqlDbType.Double).Value = (double)item.Catch_kg;
                    }

                    if (item.Sample_kg == null)
                    {
                        cmd.Parameters.Add("@sample_kg", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@sample_kg", MySqlDbType.Double).Value = (double)item.Sample_kg;
                    }

                    cmd.Parameters.Add("@taxa", MySqlDbType.VarChar).Value = item.TaxaCode;
                    cmd.Parameters.Add("@species_text", MySqlDbType.VarChar).Value = item.SpeciesText;
                    cmd.Parameters.Add("@catch_id", MySqlDbType.Int32).Value = item.PK;

                    cmd.CommandText = @"Update dbo_vessel_catch set
                                v_unload_id=@v_unload_id,
                                species_id = @species_id,
                                catch_kg = @catch_kg,
                                samp_kg = @sample_kg,
                                taxa = @taxa,
                                species_text = @species_text
                            WHERE catch_id = @catch_id";

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
        public bool Update(VesselCatch item)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = UpdateMySQL(item);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    using (OleDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.Parameters.Add("@v_unload_id", OleDbType.Integer).Value = item.Parent.PK;

                        if (item.SpeciesID == null)
                        {
                            cmd.Parameters.Add("@species_id", OleDbType.Integer).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@species_id", OleDbType.Integer).Value = item.SpeciesID;
                        }

                        if (item.Catch_kg == null)
                        {
                            cmd.Parameters.Add("@catch_kg", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@catch_kg", OleDbType.Double).Value = (double)item.Catch_kg;
                        }

                        if (item.Sample_kg == null)
                        {
                            cmd.Parameters.Add("@sample_kg", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@sample_kg", OleDbType.Double).Value = (double)item.Sample_kg;
                        }

                        cmd.Parameters.Add("@taxa", OleDbType.VarChar).Value = item.TaxaCode;
                        cmd.Parameters.Add("@species_text", OleDbType.VarChar).Value = item.SpeciesText;
                        cmd.Parameters.Add("@catch_id", OleDbType.Integer).Value = item.PK;

                        cmd.CommandText = @"Update dbo_vessel_catch set
                                v_unload_id=@v_unload_id,
                                species_id = @species_id,
                                catch_kg = @catch_kg,
                                samp_kg = @sample_kg,
                                taxa = @taxa,
                                species_text = @species_text
                            WHERE catch_id = @catch_id";
                        try
                        {
                            var ressultCount = cmd.ExecuteNonQuery();
                            success = ressultCount > 0;
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

        public static bool ClearTable()
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $"Delete * from dbo_vessel_catch";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    try
                    {
                        update.ExecuteNonQuery();
                        success = true;
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
        private bool DeleteMySQL(int id)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    update.CommandText = "Delete  from dbo_vessel_catch where catch_id=@id";
                    try
                    {
                        conn.Open();
                        success = update.ExecuteNonQuery() > 0;
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
                success = DeleteMySQL(id);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    using (OleDbCommand update = conn.CreateCommand())
                    {
                        update.Parameters.Add("@id", OleDbType.Integer).Value = id;
                        update.CommandText = "Delete * from dbo_vessel_catch where catch_id=@id";
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
    }
}
