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
    public class ETPInteractionRepository
    {
        public List<ETP_Interaction> ETP_Interactions { get; set; }

        public ETPInteractionRepository(VesselUnload vu)
        {
            ETP_Interactions = getETP_Interactions(vu);
        }

        private List<ETP_Interaction>getETP_Interactions(VesselUnload vu)
        {
            List<ETP_Interaction> thisList = new List<ETP_Interaction>();
            if(Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@vid", vu.PK);

                        cmd.CommandText = @"SELECT * FROM dbo_vessel_unload_etp_interaction_type
                                            WHERE v_unload_id=@vid";
                        con.Open();
                        var dr = cmd.ExecuteReader();
                        while(dr.Read())
                        {
                            ETP_Interaction inter = new ETP_Interaction
                            {
                                VesselUnloadID = (int)dr["v_unload_id"],
                                RowID = (int)dr["row_id"],
                                Interaction = dr["etp_interaction"].ToString(),
                                OtherInteraction=dr["other_interaction"].ToString()
                            };
                            thisList.Add(inter);
                        }
                    }
                }
            }
            return thisList;
        }

        public bool Add(ETP_Interaction interaction)
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
                        cmd.Parameters.AddWithValue("@vu_id",interaction.VesselUnloadID);
                        cmd.Parameters.AddWithValue("@row_id",interaction.RowID);
                        cmd.Parameters.AddWithValue("@interaction",interaction.Interaction);
                        cmd.Parameters.AddWithValue("@other",interaction.OtherInteraction);

                        cmd.CommandText = @"INSERT INTO dbo_vessel_unload_etp_interaction_type (
                                                v_unload_id,
                                                row_id,
                                                etp_interaction,
                                                other_interaction)
                                            VALUES(?,?,?,?)";

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
                        cmd.CommandText = @"SELECT Max(row_id) AS max_id FROM dbo_vessel_unload_etp_interaction_type";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = @"SELECT Max(row_id) AS max_id FROM dbo_vessel_unload_etp_interaction_type";
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
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@row_id", pk);
                        cmd.CommandText = @"DELETE * FROM dbo_vessel_unload_etp_interaction_type
                                           WHERE row_id=@row_id";

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
                        cmd.CommandText = @"DELETE * FROM dbo_vessel_unload_etp_interaction_type
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

        public bool Update(ETP_Interaction interaction)
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
                        cmd.Parameters.AddWithValue("@vu_id", interaction.VesselUnloadID);
                        cmd.Parameters.AddWithValue("@row_id", interaction.RowID);
                        cmd.Parameters.AddWithValue("@interaction", interaction.Interaction);
                        cmd.Parameters.AddWithValue("@other", interaction.OtherInteraction);

                        cmd.CommandText = @"INSERT INTO dbo_vessel_unload_etp_interaction_type SET
                                                v_unload_id=@vu_id,
                                                row_id=@row_id,
                                                etp_interaction=@interaction
                                                other_interaction=@other";

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
    }
}
