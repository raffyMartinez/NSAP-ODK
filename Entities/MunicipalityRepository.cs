using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using MySql.Data.MySqlClient;
using NSAP_ODK.NSAPMysql;
namespace NSAP_ODK.Entities
{
    public class MunicipalityRepository
    {
        public List<Municipality> Municipalities { get; set; }

        public MunicipalityRepository(Province province)
        {
            Municipalities = getMunicipalities(province);
        }

        private bool AddToMySQL(Municipality m)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {

                conn.Open();
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@prv_no", MySqlDbType.Int32).Value = m.Province.ProvinceID;
                    update.Parameters.Add("@name", MySqlDbType.VarChar).Value = m.MunicipalityName;


                    if (m.Longitude == null)
                    {
                        update.Parameters.Add("@lon", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@lon", MySqlDbType.Double).Value = m.Longitude;
                    }
                    if (m.Latitude == null)
                    {
                        update.Parameters.Add("@lat", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@lat", MySqlDbType.Double).Value = m.Latitude;
                    }
                    update.Parameters.AddWithValue("@is_coastal", m.IsCoastal);
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = m.MunicipalityID;
                    update.CommandText= @"Insert into municipalities(prov_no,municipality, x_coord, y_coord, is_coastal,mun_no)
                                        Values (@prv_no,@name,@lon,@lat,@is_coastal,@id)";
                    try
                    {
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
        private List<Municipality> getFromMySQL(Province province)
        {
            List<Municipality> thisList = new List<Municipality>();


            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"Select * from municipalities where prov_no={province.ProvinceID}";
                    conn.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Municipality m = new Municipality();
                        m.Province = province;
                        m.MunicipalityID = (int)dr["mun_no"];
                        m.MunicipalityName = dr["municipality"].ToString();


                        if (dr["y_Coord"].GetType().Name != "DBNull")
                        {
                            m.Latitude = Convert.ToDouble(dr["y_Coord"]);
                        }
                        if (dr["x_Coord"].GetType().Name != "DBNull")
                        {
                            m.Longitude = Convert.ToDouble(dr["x_Coord"]);
                        }
                        m.IsCoastal = (bool)dr["is_coastal"];
                        thisList.Add(m);
                    }
                }
            }

            return thisList;
        }
        public static int MunicipalityMaxRecordNumber()
        {
            int maxRecordNumber = 0;
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                const string sql = "SELECT Max([MunNo]) AS MaxRowID FROM Municipalities";

                using (OleDbCommand getCount = new OleDbCommand(sql, conn))
                {
                    try
                    {
                        maxRecordNumber = (int)getCount.ExecuteScalar();
                    }
                    catch (OleDbException oex)
                    {
                        Logger.Log(oex);
                        maxRecordNumber = 0;
                    }
                    catch
                    {
                        maxRecordNumber = 0;
                    }
                }
            }
            return maxRecordNumber;
        }

        private List<Municipality> getMunicipalities(Province province)
        {
            List<Municipality> listMunicipalities = new List<Municipality>();
            if (Global.Settings.UsemySQL)
            {
                listMunicipalities = getFromMySQL(province);
            }
            else
            {
                var dt = new DataTable();
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    try
                    {
                        conection.Open();
                        string query = $"Select * from Municipalities where ProvNo={province.ProvinceID}";

                        var adapter = new OleDbDataAdapter(query, conection);
                        adapter.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            listMunicipalities.Clear();
                            foreach (DataRow dr in dt.Rows)
                            {
                                Municipality m = new Municipality();
                                m.Province = province;
                                m.MunicipalityID = (int)dr["MunNo"];
                                m.MunicipalityName = dr["Municipality"].ToString();


                                if (dr["yCoord"].GetType().Name != "DBNull")
                                {
                                    m.Latitude = Convert.ToDouble(dr["yCoord"]);
                                }
                                if (dr["xCoord"].GetType().Name != "DBNull")
                                {
                                    m.Longitude = Convert.ToDouble(dr["xCoord"]);
                                }
                                m.IsCoastal = (bool)dr["IsCoastal"];
                                listMunicipalities.Add(m);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }

                }
            }
            return listMunicipalities;
        }

        public bool Add(Municipality m)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = AddToMySQL(m);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    var sql = "Insert into Municipalities(ProvNo, Municipality, xCoord, yCoord, IsCoastal,MunNo) Values (?,?,?,?,?,?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@prv_no", OleDbType.Integer).Value = m.Province.ProvinceID;
                        update.Parameters.Add("@name", OleDbType.VarChar).Value = m.MunicipalityName;


                        if (m.Longitude == null)
                        {
                            update.Parameters.Add("@lon", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@lon", OleDbType.Double).Value = m.Longitude;
                        }
                        if (m.Latitude == null)
                        {
                            update.Parameters.Add("@lat", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@lat", OleDbType.Double).Value = m.Latitude;
                        }
                        update.Parameters.Add("@is_coastal", OleDbType.Boolean).Value = m.IsCoastal;
                        update.Parameters.Add("@id", OleDbType.Integer).Value = m.MunicipalityID;
                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException odbex)
                        {
                            Logger.Log(odbex);
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
        private bool UpdateMySQL(Municipality m)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@prv_no", MySqlDbType.Int32).Value = m.Province.ProvinceID;
                    update.Parameters.Add("@name", MySqlDbType.VarChar).Value = m.MunicipalityName;


                    if (m.Longitude == null)
                    {
                        update.Parameters.Add("@lon", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@lon", MySqlDbType.Double).Value = m.Longitude;
                    }
                    if (m.Latitude == null)
                    {
                        update.Parameters.Add("@lat", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@lat", MySqlDbType.Double).Value = m.Latitude;
                    }
                    update.Parameters.AddWithValue("@is_coastal",m.IsCoastal);
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = m.MunicipalityID;

                    update.CommandText = @"Update municipalities set
                                            prov_no = @prv_no,
                                            municipality=@name',
                                            x_coord = @lon,
                                            y_coord = @lat,
                                            is_coastal = @is_coastal
                                        WHERE MunNo = @id";

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
        public bool Update(Municipality m)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = UpdateMySQL(m);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    using (OleDbCommand update = conn.CreateCommand())
                    {
                        update.Parameters.Add("@prv_no", OleDbType.Integer).Value = m.Province.ProvinceID;
                        update.Parameters.Add("@name", OleDbType.VarChar).Value = m.MunicipalityName;


                        if (m.Longitude == null)
                        {
                            update.Parameters.Add("@lon", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@lon", OleDbType.Double).Value = m.Longitude;
                        }
                        if (m.Latitude == null)
                        {
                            update.Parameters.Add("@lat", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@lat", OleDbType.Double).Value = m.Latitude;
                        }
                        update.Parameters.Add("@is_coastal", OleDbType.Boolean).Value = m.IsCoastal;
                        update.Parameters.Add("@id", OleDbType.Integer).Value = m.MunicipalityID;

                        update.CommandText = @"Update Municipalities set
                                            ProvNo = @prv_no,
                                            Municipality=@name',
                                            xCoord = @lon,
                                            yCoord = @lat,
                                            IsCoastal = @is_coastal
                                        WHERE MunNo = @id";

                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException odbex)
                        {
                            Logger.Log(odbex);
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
                    update.CommandText = "Delete * from municipalities where mun_no=@id";
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
                        update.CommandText = "Delete * from Municipalities where MunNo=@id";
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
                        cmd.CommandText =  "SELECT Max(mun_no) AS max_record_no FROM municipalities";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(MunNo) AS max_record_no FROM Municipalities";
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