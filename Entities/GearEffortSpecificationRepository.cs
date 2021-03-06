﻿using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace NSAP_ODK.Entities
{
    internal class GearEffortSpecificationRepository
    {
        public List<GearEffortSpecification> GearEffortSpecifications { get; set; }

        public GearEffortSpecificationRepository(Gear gear)
        {
            GearEffortSpecifications = getGearEffortSpecifications(gear);
        }

        public static int AllGearEffortSpecificationMaxRecordNumber()
        {
            int maxRecordNumber = 0;
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                const string sql = "SELECT Max([RowId]) AS MaxRowID FROM GearEffortSpecification";

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

        private List<GearEffortSpecification> getGearEffortSpecifications(Gear gear)
        {
            List<GearEffortSpecification> thisList = new List<GearEffortSpecification>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $"Select * from GearEffortSpecification where GearCode='{gear.Code}'";

                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        thisList.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            GearEffortSpecification ges = new GearEffortSpecification();
                            ges.EffortSpecification = NSAPEntities.EffortSpecificationViewModel.GetEffortSpecification(Convert.ToInt32(dr["EffortSpec"]));
                            ges.Gear = gear;
                            ges.RowID = Convert.ToInt32(dr["RowId"]);
                            thisList.Add(ges);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
                return thisList;
            }
        }

        public bool Add(GearEffortSpecification ges)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into GearEffortSpecification (GearCode, EffortSpec, RowId)
                           Values
                           ('{ges.Gear.Code}',{ges.EffortSpecification.ID},{ges.RowID})";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }

        public bool Update(GearEffortSpecification ges)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update GearEffortSpecification set
                                GearCode = '{ges.Gear.Code}',
                                EffortSpec = {ges.EffortSpecification.ID}
                            WHERE RowdId={ges.RowID}";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }

        public bool Delete(int id)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $"Delete * from GearEffortSpecification where RowId={id}";
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
        public int MaxRecordNumber()
        {
            int max_rec_no = 0;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                const string sql = "SELECT Max(RowId) AS max_record_no FROM GearEffortSpecification";
                using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                {
                    max_rec_no = (int)getMax.ExecuteScalar();
                }
            }
            return max_rec_no;
        }
    }
}