using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using MySql.Data.MySqlClient;
using NSAP_ODK.NSAPMysql;
namespace NSAP_ODK.Entities
{

    public class NotFishSpeciesRepository
    {
        private List<NotFishSpecies> inverts = new List<NotFishSpecies>();
        public List<NotFishSpecies> ListSpeciesNotFish { get; set; }


        public static bool UpdateTableDefinition()
        {
            bool success = false;
            List<NotFishSpecies> temp = new List<NotFishSpecies>();

            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $"Select * from notFishSpecies";

                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            NotFishSpecies spnf = new NotFishSpecies();
                            spnf.SpeciesID = Convert.ToInt32(dr["SpeciesID"]);
                            spnf.Genus = dr["Genus"].ToString();
                            spnf.Species = dr["Species"].ToString();
                            spnf.Taxa = NSAPEntities.TaxaViewModel.GetTaxa(dr["Taxa"].ToString());
                            if (double.TryParse(dr["MaxSize"].ToString(), out double v))
                            {
                                spnf.MaxSize = v;
                            }
                            if (dr["SizeIndicator"].ToString().Length > 0)
                            {
                                spnf.SizeType = NSAPEntities.SizeTypeViewModel.GetSizeType(dr["SizeIndicator"].ToString());
                            }
                            temp.Add(spnf);
                        }
                    }


                    using (var cmd = conection.CreateCommand())
                    {
                        try
                        {
                            cmd.CommandText = "DROP INDEX altkey on notFishSpecies";
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "ALTER TABLE notFishSpecies DROP Column Genus";
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "ALTER TABLE notFishSpecies DROP Column Species";
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "ALTER TABLE notFishSpecies ADD Column Genus varchar(60)";
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "ALTER TABLE notFishSpecies ADD Column Species varchar(60)";
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "CREATE UNIQUE INDEX altkey ON notFishSpecies(Genus,Species)";
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "ALTER TABLE notFishSpecies ADD Column Name varchar(60)";
                            cmd.ExecuteNonQuery();


                            foreach (NotFishSpecies item in temp)
                            {
                                using (var cmd1 = conection.CreateCommand())
                                {
                                    cmd1.Parameters.AddWithValue("@g", item.Genus);
                                    cmd1.Parameters.AddWithValue("@s", item.Species);
                                    cmd1.Parameters.AddWithValue("@id", item.SpeciesID);

                                    cmd1.CommandText = "Update notFishSpecies SET Genus=@g, Species=@s WHERE SpeciesID=@id";
                                    var r = cmd1.ExecuteNonQuery();
                                }
                            }

                            success = true;
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
            return success;

        }
        public NotFishSpeciesRepository()
        {
            ListSpeciesNotFish = getSpeciesNotFish();
        }
        private List<NotFishSpecies> getFromMySQL()
        {
            List<NotFishSpecies> thisList = new List<NotFishSpecies>();

            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Select * from not_fish_species";
                    conn.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        NotFishSpecies spnf = new NotFishSpecies();
                        spnf.SpeciesID = Convert.ToInt32(dr["species_id"]);
                        spnf.Genus = dr["genus"].ToString();
                        spnf.Species = dr["species"].ToString();
                        spnf.Taxa = NSAPEntities.TaxaViewModel.GetTaxa(dr["taxa"].ToString());
                        if (double.TryParse(dr["max_size"].ToString(), out double v))
                        {
                            spnf.MaxSize = v;
                        }
                        if (dr["size_indicator"].ToString().Length > 0)
                        {
                            spnf.SizeType = NSAPEntities.SizeTypeViewModel.GetSizeType(dr["size_indicator"].ToString());
                        }
                        thisList.Add(spnf);
                    }
                }
            }
            return thisList;
        }
        private List<NotFishSpecies> getSpeciesNotFish()
        {
            List<NotFishSpecies> thisList = new List<NotFishSpecies>();
            if (Global.Settings.UsemySQL)
            {
                thisList = getFromMySQL();
            }
            else
            {

                var dt = new DataTable();
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    try
                    {
                        conection.Open();
                        string query = $"Select * from notFishSpecies";

                        var adapter = new OleDbDataAdapter(query, conection);
                        adapter.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            thisList.Clear();
                            foreach (DataRow dr in dt.Rows)
                            {
                                NotFishSpecies spnf = new NotFishSpecies();
                                spnf.SpeciesID = Convert.ToInt32(dr["SpeciesID"]);
                                spnf.Genus = dr["Genus"].ToString();
                                spnf.Species = dr["Species"].ToString();
                                spnf.Name = dr["Name"].ToString();
                                spnf.Taxa = NSAPEntities.TaxaViewModel.GetTaxa(dr["Taxa"].ToString());
                                if (double.TryParse(dr["MaxSize"].ToString(), out double v))
                                {
                                    spnf.MaxSize = v;
                                }
                                if (dr["SizeIndicator"].ToString().Length > 0)
                                {
                                    spnf.SizeType = NSAPEntities.SizeTypeViewModel.GetSizeType(dr["SizeIndicator"].ToString());
                                }
                                thisList.Add(spnf);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }

            }
            return thisList;
        }
        private bool AddToMySQL(NotFishSpecies nfs)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = nfs.SpeciesID;
                    update.Parameters.Add("@genus", MySqlDbType.VarChar).Value = nfs.Genus;
                    update.Parameters.Add("@species", MySqlDbType.VarChar).Value = nfs.Species;
                    update.Parameters.Add("@taxa", MySqlDbType.VarChar).Value = nfs.Taxa.Code;
                    if (nfs.SizeType == null)
                    {
                        update.Parameters.AddWithValue("@indicator", DBNull.Value);
                    }
                    else
                    {
                        update.Parameters.Add("@indicator", MySqlDbType.VarChar).Value = nfs.SizeType.Code;
                    }
                    if (nfs.MaxSize == null)
                    {
                        update.Parameters.Add("@maxsize", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@maxsize", MySqlDbType.Double).Value = nfs.MaxSize;
                    }

                    update.CommandText = "Insert into not_fish_species(species_id, genus, species, taxa, size_indicator, max_size) Values (@id,@genus,@species,@taxa,@indicator,@maxsize)";
                    try
                    {
                        conn.Open();
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException msex)
                    {
                        Logger.Log(msex.InnerException.Message);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return success;
        }


        public bool Add(NotFishSpecies nfs)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = AddToMySQL(nfs);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    var sql = "Insert into notFishSpecies(SpeciesID, Genus, Species, Name, Taxa, SizeIndicator, MaxSize) Values (?,?,?,?,?,?,?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@id", OleDbType.Integer).Value = nfs.SpeciesID;

                        if (nfs.Genus == null)
                        {
                            update.Parameters.AddWithValue("@genus", DBNull.Value);
                        }
                        else
                        {
                            update.Parameters.AddWithValue("@genus", nfs.Genus);
                        }

                        if (nfs.Species == null)
                        {
                            update.Parameters.AddWithValue("@species", DBNull.Value);
                        }
                        else
                        {
                            update.Parameters.AddWithValue("@species", nfs.Species);
                        }


                        if (nfs.Name == null)
                        {
                            update.Parameters.AddWithValue("@name", DBNull.Value);
                        }
                        else
                        {
                            update.Parameters.AddWithValue("@name", nfs.Name);
                        }

                        update.Parameters.Add("@taxa", OleDbType.VarChar).Value = nfs.Taxa.Code;
                        if (nfs.SizeType == null)
                        {
                            update.Parameters.AddWithValue("@indicator", DBNull.Value);
                        }
                        else
                        {
                            update.Parameters.Add("@indicator", OleDbType.VarChar).Value = nfs.SizeType.Code;
                        }

                        if (nfs.MaxSize == null)
                        {
                            update.Parameters.Add("@maxsize", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@maxsize", OleDbType.Double).Value = nfs.MaxSize;
                        }
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
        private bool UpdateMySQL(NotFishSpecies nfs)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@genus", MySqlDbType.VarChar).Value = nfs.Genus;
                    update.Parameters.Add("@species", MySqlDbType.VarChar).Value = nfs.Species;
                    update.Parameters.Add("@taxa", MySqlDbType.VarChar).Value = nfs.Taxa.Code;
                    if (nfs.SizeType == null)
                    {
                        update.Parameters.Add("@indicator", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@indicator", MySqlDbType.VarChar).Value = nfs.SizeType.Code;
                    }
                    if (nfs.MaxSize == null)
                    {
                        update.Parameters.Add("@maxsize", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@maxsize", MySqlDbType.Double).Value = nfs.MaxSize;
                    }
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = nfs.SpeciesID;
                    update.CommandText = @"Update not_fish_species set
                                genus = @genus,
                                species=@species,
                                taxa = @taxa,
                                size_indicator=@indicator,
                                max_size = @maxSize
                                WHERE species_id = @id";

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
        public bool Update(NotFishSpecies nfs)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = UpdateMySQL(nfs);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    using (OleDbCommand update = conn.CreateCommand())
                    {

                        update.Parameters.Add("@genus", OleDbType.VarChar).Value = nfs.Genus;
                        update.Parameters.Add("@species", OleDbType.VarChar).Value = nfs.Species;
                        update.Parameters.Add("@name", OleDbType.VarChar).Value = nfs.Name;
                        update.Parameters.Add("@taxa", OleDbType.VarChar).Value = nfs.Taxa.Code;
                        if (nfs.SizeType == null)
                        {
                            update.Parameters.Add("@indicator", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@indicator", OleDbType.VarChar).Value = nfs.SizeType.Code;
                        }
                        if (nfs.MaxSize == null)
                        {
                            update.Parameters.Add("@maxsize", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@maxsize", OleDbType.Double).Value = nfs.MaxSize;
                        }
                        update.Parameters.Add("@id", OleDbType.Integer).Value = nfs.SpeciesID;
                        update.CommandText = @"Update notFishSpecies set
                                Genus = @genus,
                                Species=@species,
                                Name = @name,
                                Taxa = @taxa,
                                SizeIndicator=@indicator,
                                MaxSize = @maxSize
                                WHERE SpeciesID = @id";
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
        private bool DeleteMySQL(int id)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    update.CommandText = "Delete  from not_fish_species where species_id=@id";
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
                        update.CommandText = $"Delete * from notFishSpecies where SpeciesID=@id";
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
                        cmd.CommandText = "SELECT Max(species_id) AS max_record_no FROM not_fish_species";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(SpeciesID) AS max_record_no FROM notFishSpecies";
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