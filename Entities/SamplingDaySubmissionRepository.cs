using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using NSAP_ODK.Utilities;
using MySql.Data.MySqlClient;
using NSAP_ODK.NSAPMysql;
namespace NSAP_ODK.Entities
{
    public class SamplingDaySubmissionRepository
    {
        public List<SamplingDaySubmission> SamplingDaySubmissions { get; set; }

        public SamplingDaySubmissionRepository()
        {
            SamplingDaySubmissions = getSamplingDaySubmissions();
        }

        public bool Add(SamplingDaySubmission si)
        {
            return true;
        }

        public bool Update(SamplingDaySubmission si)
        {
            return true;
        }

        public bool Delete(SamplingDaySubmission si)
        {
            return true;
        }
        private List<SamplingDaySubmission> getSamplingDaySubmissions()
        {
            List<SamplingDaySubmission> samplingDaySubmissions = new List<SamplingDaySubmission>();
            if(Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "Select unload_day_id, sdate, land_ctr_id, land_ctr_text, ground_id, type_of_sampling from dbo_LC_FG_sample_day";
                        try
                        {
                            con.Open();
                            var dr = cmd.ExecuteReader();
                            while(dr.Read())
                            {
                                int day_id = (int)dr["unload_day_id"];
                                SamplingDaySubmission sds = new SamplingDaySubmission
                                {
                                    SamplingDayID = day_id,
                                    SamplingDate = (DateTime)dr["sdate"],
                                    FishingGroundID = dr["ground_id"].ToString(),
                                    LandingSiteSampling = NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteSampling(day_id),
                                    TypeOfSampling = dr["type_of_sampling"].ToString()
                                };
                                if (dr["land_ctr_id"]!=DBNull.Value)
                                {
                                    sds.LandingSiteID = (int)dr["land_ctr_id"];
                                }
                                else
                                {
                                    sds.LandingSiteText = dr["land_ctr_text"].ToString();
                                }
                                samplingDaySubmissions.Add(sds);
                            }
                        }
                        catch(Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return samplingDaySubmissions;
        }
    }
}
