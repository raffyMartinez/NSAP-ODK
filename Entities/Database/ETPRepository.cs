using MySql.Data.MySqlClient;
using NSAP_ODK.NSAPMysql;
using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class ETPRepository
    {
        public List<ETP> ETPs { get; set; }

        public ETPRepository(VesselUnload vu)
        {
            ETPs = getETPs(vu);
        }

        private List<ETP> getETPs(VesselUnload vu)
        {
            List<ETP> thisList = new List<ETP>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection())
                {
                    using (OleDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@vu_id", vu.PK);
                        cmd.CommandText = @"Select * from dbo_vessel_unload_etp_interaction
                                          WHERE v_unload_id=@vu_id";
                        var dr = cmd.ExecuteReader();
                        conn.Open();
                        try
                        {
                            while (dr.Read())
                            {
                                ETP etp = new ETP
                                {
                                    VesselUnloadID = vu.PK,
                                    RowID = (int)dr["row_id"],
                                    ETP_Name = dr["etp_name"].ToString()

                                };
                                thisList.Add(etp);
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

        public bool Add(ETP etp)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (OleDbConnection con = new OleDbConnection())
                {
                    using (OleDbCommand cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@vu_id", etp.VesselUnloadID);
                        cmd.Parameters.AddWithValue("@row_id", etp.RowID);
                        cmd.Parameters.AddWithValue("@etp_name", etp.ETP_Name);

                        cmd.CommandText = @"INSERT INTO dbo_vessel_unload_etp_interaction 
                                                (v_unnload_id,
                                                row_id,
                                                etp_name)
                                           VALUES (?,?,?)";

                        con.Open();
                        try
                        {
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch(Exception ex)
                        {

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
                        cmd.CommandText = @"SELECT Max(row_id) AS max_id FROM dbo_vessel_unload_etp_interaction";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = @"SELECT Max(row_id) AS max_id FROM dbo_vessel_unload_etp_interaction";
                    using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                    {
                        try
                        {
                            var r = getMax.ExecuteScalar();
                            if (r != DBNull.Value)
                            {
                                max_rec_no = (int)r;
                            }

                        }
                        catch (OleDbException oex)
                        {

                        }
                        catch (Exception ex)
                        {
                            //ignore
                            //Logger.Log(ex);
                        }
                    }
                }
            }
            return max_rec_no;
        }
        public bool Delete(int pk)
        {
            bool success = false;
            if(Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@row_id", pk);
                        cmd.CommandText = @"DELETE * FROM dbo_vessel_unload_etp_interaction
                                           WHERE row_id=@row_id";

                        con.Open();
                        try
                        {
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch(Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return success;
        }

        public bool DeleteEx(VesselUnload vu)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@vid", vu.PK);
                        cmd.CommandText = @"DELETE * FROM dbo_vessel_unload_etp_interaction
                                           WHERE v_unload_id=@vid";

                        con.Open();
                        try
                        {
                            success = cmd.ExecuteNonQuery() > 0;
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

        public bool Update(ETP etp)
        {
            bool success = false;
            if(Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@vu_id", etp.VesselUnloadID);
                        cmd.Parameters.AddWithValue("@etp_name", etp.ETP_Name);
                        cmd.Parameters.AddWithValue("@row_id", etp.RowID);

                        cmd.CommandText = @"UPDATE dbo_vessel_unload_etp_interaction SET
                                            v_unload_id=@vu_id,
                                            etp_name = @etp_name
                                            WHERE row_id=@row_id";
                        con.Open();
                        try
                        {
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch(Exception ex)
                        {

                        }
                    }
                }
            }
            return success;
        }

    }
}
