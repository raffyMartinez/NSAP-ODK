using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using MySql.Data.MySqlClient;
using NSAP_ODK.NSAPMysql;

namespace NSAP_ODK.Entities
{
    public class FishingVesselRepository
    {
        public List<FishingVessel> FishingVessels { get; set; }

        public FishingVesselRepository()
        {
            FishingVessels = getFishingVessels();
        }

        private List<FishingVessel>getFishingVesselsMySQL()
        {
            List<FishingVessel> thisList = new List<FishingVessel>();
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Select * from fishing_vessel";
                    conn.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        FishingVessel fv = new FishingVessel();
                        fv.ID = Convert.ToInt32( dr["vessel_id"]);
                        fv.NameOfOwner = dr["name_of_owner"].ToString();
                        fv.Name = dr["vessel_name"].ToString();
                        if(double.TryParse(dr["length"].ToString(),out double len))
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
                        thisList.Add(fv);
                    }
                }
            }
            return thisList;
        }
        private List<FishingVessel> getFishingVessels()
        {
            List<FishingVessel> listFishingVessel = new List<FishingVessel>();
            if (Global.Settings.UsemySQL)
            {
                listFishingVessel = getFishingVesselsMySQL();
            }
            else
            {
                var dt = new DataTable();
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    try
                    {
                        conection.Open();
                        string query = $@"SELECT * from fishingVessel";

                        var adapter = new OleDbDataAdapter(query, conection);
                        adapter.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            listFishingVessel.Clear();
                            foreach (DataRow dr in dt.Rows)
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
                                listFishingVessel.Add(fv);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }

            return listFishingVessel;
        }
        private bool AddToMySQL(FishingVessel fv)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = fv.ID;
                    if (string.IsNullOrEmpty(fv.Name))
                    {
                        update.Parameters.Add("@name", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@name", MySqlDbType.VarChar).Value = fv.Name;
                    }
                    if (string.IsNullOrEmpty(fv.NameOfOwner))
                    {
                        update.Parameters.Add("@owner_name", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@owner_name", MySqlDbType.VarChar).Value = fv.NameOfOwner;
                    }
                    if (fv.Length == null)
                    {
                        update.Parameters.Add("@len", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@len", MySqlDbType.Double).Value = fv.Length;
                    }
                    if (fv.Depth == null)
                    {
                        update.Parameters.Add("@dep", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@dep", MySqlDbType.Double).Value = fv.Depth;
                    }
                    if (fv.Breadth == null)
                    {
                        update.Parameters.Add("@brd", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@brd", MySqlDbType.Double).Value = fv.Breadth;
                    }
                    if (string.IsNullOrEmpty(fv.RegistrationNumber))
                    {
                        update.Parameters.Add("@reg_number", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@reg_number", MySqlDbType.VarChar).Value = fv.RegistrationNumber;
                    }
                    update.Parameters.Add("@sector", MySqlDbType.Int32).Value = (int)fv.FisheriesSector;
                    update.CommandText = @"Insert into fishingVessel (vessel_id, vessel_name, name_of_owner, length,depth,breadth,registration_number,sector)
                           Values(@id,@name,@owner_name,@len,@dep,@brd,@reg_number,@sector)";
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
        public bool Add(FishingVessel fv)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = AddToMySQL(fv);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    var sql = @"Insert into fishingVessel (VesselID, VesselName, NameOfOwner, Length,Depth,Breadth,RegistrationNumber,Sector)
                           Values(?,?,?,?,?,?,?,?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@id", OleDbType.Integer).Value = fv.ID;
                        if (string.IsNullOrEmpty(fv.Name))
                        {
                            update.Parameters.Add("@name", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@name", OleDbType.VarChar).Value = fv.Name;
                        }
                        if (string.IsNullOrEmpty(fv.NameOfOwner))
                        {
                            update.Parameters.Add("@owner_name", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@owner_name", OleDbType.VarChar).Value = fv.NameOfOwner;
                        }
                        if (fv.Length == null)
                        {
                            update.Parameters.Add("@len", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@len", OleDbType.Double).Value = fv.Length;
                        }
                        if (fv.Depth == null)
                        {
                            update.Parameters.Add("@dep", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@dep", OleDbType.Double).Value = fv.Depth;
                        }
                        if (fv.Breadth == null)
                        {
                            update.Parameters.Add("@brd", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@brd", OleDbType.Double).Value = fv.Breadth;
                        }
                        if (string.IsNullOrEmpty(fv.RegistrationNumber))
                        {
                            update.Parameters.Add("@reg_number", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@reg_number", OleDbType.VarChar).Value = fv.RegistrationNumber;
                        }
                        update.Parameters.Add("@sector", OleDbType.Integer).Value = (int)fv.FisheriesSector;
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
        private bool UpdateMySQL(FishingVessel fv)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    if (string.IsNullOrEmpty(fv.Name))
                    {
                        update.Parameters.Add("@name", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@name", MySqlDbType.VarChar).Value = fv.Name;
                    }
                    if (string.IsNullOrEmpty(fv.NameOfOwner))
                    {
                        update.Parameters.Add("@owner_name", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@owner_name", MySqlDbType.VarChar).Value = fv.NameOfOwner;
                    }
                    if (fv.Length == null)
                    {
                        update.Parameters.Add("@len", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@len", MySqlDbType.Double).Value = fv.Length;
                    }
                    if (fv.Depth == null)
                    {
                        update.Parameters.Add("@dep", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@dep", MySqlDbType.Double).Value = fv.Depth;
                    }
                    if (fv.Breadth == null)
                    {
                        update.Parameters.Add("@brd", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@brd", MySqlDbType.Double).Value = fv.Breadth;
                    }
                    if (string.IsNullOrEmpty(fv.RegistrationNumber))
                    {
                        update.Parameters.Add("@reg_number", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@reg_number", MySqlDbType.VarChar).Value = fv.RegistrationNumber;
                    }
                    update.Parameters.Add("@sector", MySqlDbType.Int32).Value = (int)fv.FisheriesSector;
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = fv.ID;

                    update.CommandText = @"Update fishing_vessel set
                                vessel_name = @name,
                                name_of_owner=@owner_name,
                                length = @len,
                                depth = @dep,
                                breadth = @brd,
                                registration_number = @reg_number,
                                sector = @sector
                                WHERE vessel_id=@id";

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
        public bool Update(FishingVessel fv)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = UpdateMySQL(fv);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    using (OleDbCommand update = conn.CreateCommand())
                    {

                        if (string.IsNullOrEmpty(fv.Name))
                        {
                            update.Parameters.Add("@name", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@name", OleDbType.VarChar).Value = fv.Name;
                        }
                        if (string.IsNullOrEmpty(fv.NameOfOwner))
                        {
                            update.Parameters.Add("@owner_name", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@owner_name", OleDbType.VarChar).Value = fv.NameOfOwner;
                        }
                        if (fv.Length == null)
                        {
                            update.Parameters.Add("@len", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@len", OleDbType.Double).Value = fv.Length;
                        }
                        if (fv.Depth == null)
                        {
                            update.Parameters.Add("@dep", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@dep", OleDbType.Double).Value = fv.Depth;
                        }
                        if (fv.Breadth == null)
                        {
                            update.Parameters.Add("@brd", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@brd", OleDbType.Double).Value = fv.Breadth;
                        }
                        if (string.IsNullOrEmpty(fv.RegistrationNumber))
                        {
                            update.Parameters.Add("@reg_number", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@reg_number", OleDbType.VarChar).Value = fv.RegistrationNumber;
                        }
                        update.Parameters.Add("@sector", OleDbType.Integer).Value = (int)fv.FisheriesSector;
                        update.Parameters.Add("@id", OleDbType.Integer).Value = fv.ID;

                        update.CommandText = @"Update fishingVessel set
                                VesselName = @name,
                                NameOfOwner=@owner_name,
                                Length = @len,
                                Depth = @dep,
                                Breadth = @brd,
                                RegistrationNumber = @reg_number,
                                Sector = @sector
                                WHERE VesselID=@id";
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
                    update.CommandText =  "Delete * from fishing_vessel where vessel_id=@id";
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
                    //var sql = $"Delete * from fishingVessel where VesselID={id}";
                    using (OleDbCommand update = conn.CreateCommand())
                    {
                        update.Parameters.Add("@id", OleDbType.Integer).Value = id;
                        update.CommandText = "Delete * from fishingVessel where VesselID=@id";
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
                        cmd.CommandText ="SELECT Max(vessel_id) AS max_id FROM fishing_vessel";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(VesselID) AS max_id FROM fishingVessel";
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