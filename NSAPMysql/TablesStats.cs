﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using NSAP_ODK.Utilities;
namespace NSAP_ODK.NSAPMysql
{
    public static class TablesStats
    {
        public static List<TableStats> TablesStatistics { get; private set; } = new List<TableStats>();

        public static uint? GetMaxAllowedPacketSize()
        {
            if (MySQLConnect.UserCanCreateDatabase)
            {
                using (MySqlConnection dbconn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (MySqlCommand c = dbconn.CreateCommand())
                    {
                        //c.Parameters.Add("@dbname", MySqlDbType.VarChar).Value = "nsap_odk";
                        c.CommandText = "Select @@global.max_allowed_packet;";
                        dbconn.Open();
                        try
                        {
                            return uint.Parse(c.ExecuteScalar().ToString());
                        }
                        catch (Exception ex)
                        {
                            return null;
                        }
                    }
                }
            }
            return null;
        }


        public static bool ApplySQLChanges(uint max_packet_size)
        {
            bool success = false;
            if (MySQLConnect.UserCanCreateDatabase)
            {
                using (MySqlConnection dbconn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (MySqlCommand c = dbconn.CreateCommand())
                    {
                        c.Parameters.Add("@packet_size", MySqlDbType.UInt32).Value = max_packet_size;
                        c.CommandText = "SET GLOBAL max_allowed_packet=@packet_size";
                        dbconn.Open();
                        try
                        {
                            c.ExecuteNonQuery();
                            success = true;
                        }
                        catch (Exception ex)
                        {
                            NSAP_ODK.Utilities.Logger.Log(ex);
                        }
                    }
                }
            }
            return success;
        }
        public static void GetStats()
        {
            TablesStatistics.Clear();
            if (MySQLConnect.UserCanCreateDatabase)
            {
                using (MySqlConnection dbconn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (MySqlCommand c = dbconn.CreateCommand())
                    {
                        c.Parameters.Add("@dbname", MySqlDbType.VarChar).Value = "nsap_odk";
                        c.CommandText = "Select TABLE_NAME,CREATE_TIME,TABLE_COMMENT from information_schema.TABLES where TABLE_SCHEMA=@dbName";
                        dbconn.Open();
                        var dr = c.ExecuteReader();
                        {
                            while (dr.Read())
                            {
                                TableStats ts = new TableStats();
                                ts.TableName = dr["TABLE_NAME"].ToString();
                                ts.Created = Convert.ToDateTime(dr["CREATE_TIME"]);
                                ts.Comment = Convert.ToString(dr["TABLE_COMMENT"]);
                                GetTableStats(ts);

                                TablesStatistics.Add(ts);
                            }
                        }
                    }
                }
            }
        }

        private static void GetTableStats(TableStats ts)
        {
            using (MySqlConnection dbconn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (MySqlCommand c = dbconn.CreateCommand())
                {
                    //c.Parameters.Add("@tableName", MySqlDbType.VarChar).Value = ts.TableName;
                    //c.CommandText = "Select count(*) from @tableName";
                    c.CommandText = $"Select Count(*) from {ts.TableName}";
                    try
                    {
                        dbconn.Open();
                        //var r = c.ExecuteReader();
                        ts.Rows = Convert.ToInt32(c.ExecuteScalar());
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
        }
    }
}
