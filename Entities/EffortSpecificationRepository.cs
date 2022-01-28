using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using NSAP_ODK.Utilities;
using System.Data;
namespace NSAP_ODK.Entities
{
    public class EffortSpecificationRepository
    {
        public List<EffortSpecification> EffortSpecifications { get; set; }

        public EffortSpecificationRepository()
        {
            EffortSpecifications = getEffortSpecifications();
        }

        private List<EffortSpecification> getEffortSpecifications()
        {
            List<EffortSpecification> theList = new List<EffortSpecification>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $"Select * from effortSpecification";


                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        theList.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            EffortSpecification es = new EffortSpecification();
                            es.ID = Convert.ToInt32(dr["EffortSpecificationID"]);
                            es.IsForAllTypesFishing = (bool)dr["ForAllTypeOfFishing"];
                            es.Name = dr["EffortSpecification"].ToString();
                            es.ValueType = (ODKValueType)Convert.ToInt32(dr["ValueType"]);
                            theList.Add(es);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);

                }
                return theList;
            }
        }
        public bool Add(EffortSpecification es)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = "Insert into effortSpecification(EffortSpecificationID, EffortSpecification, ForAllTypeOfFishing, ValueType) Values (?,?,?,?)";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    update.Parameters.Add("@id", OleDbType.Integer).Value = es.ID;
                    update.Parameters.Add("@effort_spec", OleDbType.VarChar).Value = es.Name;
                    update.Parameters.Add("@is_for_all", OleDbType.Boolean).Value = es.IsForAllTypesFishing;
                    update.Parameters.Add("@type", OleDbType.Integer).Value = (int)es.ValueType;
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
            return success;
        }

        public bool Update(EffortSpecification es)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();

                using (OleDbCommand update = conn.CreateCommand())
                {

                    update.Parameters.Add("@effort_spec", OleDbType.VarChar).Value = es.Name;
                    update.Parameters.Add("@is_for_all", OleDbType.Boolean).Value = es.IsForAllTypesFishing;
                    update.Parameters.Add("@type", OleDbType.Integer).Value = (int)es.ValueType;
                    update.Parameters.Add("@id", OleDbType.Integer).Value = es.ID;

                    update.CommandText = @"Update effortSpecification set
                                EffortSpecification = @effort_spec,
                                ForAllTypeOfFishing=@is_for_all,
                                ValueType = @type
                                WHERE EffortSpecificationID = @id";

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
            return success;
        }


        public bool Delete(int id)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                
                using (OleDbCommand update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", OleDbType.Integer).Value = id;
                    update.CommandText="Delete * from effortSpecification where EffortSpecificationID=@id";
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
                const string sql = "SELECT Max(EffortSpecificationID) AS max_id FROM effortSpecification";
                using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                {
                    max_rec_no = (int)getMax.ExecuteScalar();
                }
            }
            return max_rec_no;
        }
    }
}
