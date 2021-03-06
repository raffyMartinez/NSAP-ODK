﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using NSAP_ODK.Utilities;
namespace NSAP_ODK.Entities.Database
{
    public class JSONFileRepository
    {

        public JSONFileRepository()
        {
            JSONFiles = getJSONFiles();
        }
        public List<JSONFile> JSONFiles { get; private set; }

        public List<JSONFile> getJSONFiles()
        {
            List<JSONFile> thisList = new List<JSONFile>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $"Select * from JSONFile";
                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        thisList.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            JSONFile item = new JSONFile();
                            item.RowID = (int)dr["RowID"];
                            item.FileName= dr["FileName"].ToString();
                            item.Count= (int)dr["Count"];
                            item.Earliest= DateTime.Parse( dr["EarliestDate"].ToString());
                            item.Latest= DateTime.Parse( dr["LatestDate"].ToString());
                            item.DateAdded= DateTime.Parse( dr["DateAdded"].ToString());
                            item.MD5 = dr["MD5"].ToString();
                            item.FormID = dr["FormID"].ToString();
                            item.Description = dr["Description"].ToString();
                            thisList.Add(item);
                        }
                    }
                }
                catch(OleDbException dbex)
                {
                    if (dbex.ErrorCode == -2147217865)
                    {
                        //table not found so we create one
                        CreateTable();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);

                }
                return thisList;
            }
        }

        public bool ClearTable()
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $"Delete * from JSONFile";
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
        private void CreateTable()
        {
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                string sql = @"CREATE TABLE JSONFile 
                                (
                                RowID Int NOT NULL PRIMARY KEY,
                                FileName VarChar,
                                [Count] Int,
                                EarliestDate DateTime,
                                LatestDate DateTime,
                                DateAdded DateTime,
                                MD5 VarChar,
                                FormID VarChar,
                                [Description] VarChar
                                )";
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;

                try
                {
                    cmd.ExecuteNonQuery();

                }               
                catch (OleDbException dbex)
                {
                    //ignore
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }

                cmd.Connection.Close();
                conn.Close();
            }
        }

        public int MaxRecordNumber()
        {
            int max_rec_no = 0;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                const string sql = "SELECT Max(RowID) AS max_record_no FROM JSONFile";
                using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                {
                    try
                    {
                        max_rec_no = (int)getMax.ExecuteScalar();
                    }
                    catch(Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return max_rec_no;
        }

        public bool AddItem(JSONFile jsonFile)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into JSONFile (RowID, FileName, [Count], EarliestDate, LatestDate, DateAdded, MD5, FormID, [Description])
                           Values (?,?,?,?,?,?,?,?,?)";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    update.Parameters.Add("@row_id", OleDbType.Integer).Value = jsonFile.RowID;
                    update.Parameters.Add("@file_name", OleDbType.VarChar).Value = jsonFile.FileName;
                    update.Parameters.Add("@count", OleDbType.Integer).Value = jsonFile.Count;
                    update.Parameters.Add("@earliest_date", OleDbType.Date).Value = jsonFile.Earliest;
                    update.Parameters.Add("@latest_date", OleDbType.Date).Value = jsonFile.Latest;
                    update.Parameters.Add("@date_added", OleDbType.Date).Value = jsonFile.DateAdded;
                    update.Parameters.Add("@md5", OleDbType.VarChar).Value = jsonFile.MD5;
                    update.Parameters.Add("@form_id", OleDbType.VarChar).Value = jsonFile.FormID;
                    update.Parameters.Add("@description", OleDbType.VarChar).Value = jsonFile.Description;
                    try
                    {
                        success = update.ExecuteNonQuery() > 0;
                        update.Parameters.Clear();
                    }
                    catch (OleDbException dbex)
                    {
                        Logger.Log(dbex);
                    }
                    catch(Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return success;
        }

        public bool Update(JSONFile jsf)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                using (OleDbCommand cmd = conn.CreateCommand())
                {
                    cmd.Parameters.Add("@FileName", OleDbType.VarChar).Value = jsf.FileName;
                    cmd.Parameters.Add("@Count", OleDbType.Integer).Value = jsf.Count;
                    cmd.Parameters.Add("@EarliestDate", OleDbType.Date).Value = jsf.Earliest;
                    cmd.Parameters.Add("@LatestDate", OleDbType.Date).Value = jsf.Latest;
                    cmd.Parameters.Add("@DateAdded", OleDbType.Date).Value = jsf.DateAdded;
                    cmd.Parameters.Add("@MD5", OleDbType.VarChar).Value = jsf.MD5;
                    cmd.Parameters.Add("@FormID", OleDbType.VarChar).Value = jsf.FormID;
                    cmd.Parameters.Add("@Description", OleDbType.VarChar).Value = jsf.Description;
                    cmd.Parameters.Add("@RowID", OleDbType.Integer).Value = jsf.RowID;

                    cmd.CommandText = @"Update JSONFile set
                                        FileName = @FileName,
                                        Count = @Count,
                                        EarliestDate = @EarliestDate,
                                        LatestDate = @LatestDate,
                                        DateAdded = @DateAdded,
                                        MD5 = @MD5,
                                        FormID = @FormID,
                                        Description = @Description,
                                        Where RowID = @RowID";
                    try
                    {
                        success = cmd.ExecuteNonQuery() > 0;
                    }
                    catch(OleDbException dbex)
                    {
                        Logger.Log(dbex);
                    }
                    catch(Exception ex)
                    {
                        Logger.Log(ex);
                    }
                                        
                }
            }
            return success;
        }

        public bool Delete(int ID)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $"Delete * from JSONFile where RODID={ID}";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
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
