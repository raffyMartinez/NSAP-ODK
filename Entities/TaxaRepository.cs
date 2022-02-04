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
namespace NSAP_ODK.Entities
{
    public class TaxaRepository
    {
        public List<Taxa> Taxas { get; set; }

        public TaxaRepository()
        {
            Taxas = getTaxas();
        }
        private List<Taxa> getFromMySQL()
        {
            List<Taxa> thisList = new List<Taxa>();

            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Select * from taxa";
                    conn.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Taxa t = new Taxa();
                        t.Code = dr["taxa_code"].ToString();
                        t.Name = dr["taxa"].ToString();
                        t.Description = dr["description"].ToString();
                        thisList.Add(t);
                    }
                }
            }
            return thisList;
        }

        private List<Taxa> getTaxas()
        {
            List<Taxa> listTaxa = new List<Taxa>();
            if (Global.Settings.UsemySQL)
            {
                listTaxa = getFromMySQL();
            }
            else
            {

                var dt = new DataTable();
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    try
                    {
                        conection.Open();
                        string query = $"Select * from taxa";


                        var adapter = new OleDbDataAdapter(query, conection);
                        adapter.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            listTaxa.Clear();
                            foreach (DataRow dr in dt.Rows)
                            {
                                Taxa t = new Taxa();
                                t.Code = dr["TaxaCode"].ToString();
                                t.Name = dr["Taxa"].ToString();
                                t.Description = dr["Description"].ToString();
                                listTaxa.Add(t);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);

                    }
                }

            }
            return listTaxa;
        }

        private bool AddToMySQL(Taxa t)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Parameters.Add("@code", MySqlDbType.VarChar).Value = t.Code;
                    cmd.Parameters.Add("@taxa", MySqlDbType.VarChar).Value = t.Name;
                    cmd.Parameters.Add("@description", MySqlDbType.VarChar).Value = t.Description;
                    cmd.CommandText = "Insert into taxa (taxa_code, taxa, description)Values (@code, @taxa, @description)";
                    try
                    {
                        conn.Open();
                        success = cmd.ExecuteNonQuery() > 0;
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
            return success;
        }
        public bool Add(Taxa t)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = AddToMySQL(t);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    var sql = "Insert into taxa (TaxaCode, Taxa, Description)Values (?,?,?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@code", OleDbType.VarChar).Value = t.Code;
                        update.Parameters.Add("@taxa", OleDbType.VarChar).Value = t.Name;
                        update.Parameters.Add("@description", OleDbType.VarChar).Value = t.Description;
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

        public bool Update(Taxa t)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();

                using (OleDbCommand update = conn.CreateCommand())
                {
                    update.Parameters.Add("@code", OleDbType.VarChar).Value = t.Code;
                    update.Parameters.Add("@taxa", OleDbType.VarChar).Value = t.Name;
                    update.Parameters.Add("@description", OleDbType.VarChar).Value = t.Description;

                    update.CommandText = @"Update taxa set
                                            TaxaCode = @code,
                                            Taxa = @taxa,
                                            Description=@description
                                        WHERE TaxaCode = @code";

                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }

        public bool Delete(string code)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();

                using (OleDbCommand update = conn.CreateCommand())
                {
                    update.Parameters.Add("@code", OleDbType.VarChar).Value = code;
                    update.CommandText = "Delete * from taxa  where TaxaCode=@code";
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
            return success;
        }
    }
}
