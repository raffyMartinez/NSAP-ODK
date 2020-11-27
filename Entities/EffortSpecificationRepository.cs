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
                var sql = $@"Insert into effortSpecification(EffortSpecificationID, EffortSpecification, ForAllTypeOfFishing, ValueType)
                           Values ({es.ID},'{es.Name}',{es.IsForAllTypesFishing}, {(int)es.ValueType})";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
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
                var sql = $@"Update effortSpecification set
                                EffortSpecification = '{es.Name}',
                                ForAllTypeOfFishing={es.IsForAllTypesFishing},
                                ValueType = {(int)es.ValueType}
                            WHERE EffortSpecificationID = {es.ID}";
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
                var sql = $"Delete * from effortSpecification where EffortSpecificationID={id}";
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
