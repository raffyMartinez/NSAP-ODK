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
using NSAP_ODK.Entities;
namespace NSAP_ODK.Entities.Database
{
    class SampledLandingSiteRepository
    {
        public List<SampledLandingSite> SampledLandingSites { get; set; }

        public SampledLandingSiteRepository(FishingGround fg, FMA fma, NSAPRegion nsapRegion)
        {
            SampledLandingSites = getSampledLandingSites(fg, fma, nsapRegion);
        }

        private List<SampledLandingSite> getSampledLandingSites(FishingGround fg, FMA fma, NSAPRegion nsapRegion)
        {
            List<SampledLandingSite> thisList = new List<SampledLandingSite>();
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@fg", fg.Code);
                        cmd.Parameters.AddWithValue("@fma", fma.FMAID);
                        cmd.Parameters.AddWithValue("@region", nsapRegion.Code);
                        cmd.CommandText = "";
                        cmd.CommandText = @"SELECT DISTINCT 
                                                dbo_LC_FG_sample_day.land_ctr_id, 
                                                landing_sites.landing_site_name, 
                                                landing_sites.barangay, 
                                                municipalities.municipality, 
                                                municipalities.mun_no,
                                                municipalities.prov_no,
                                                provinces.province_name, 
                                                dbo_LC_FG_sample_day.land_ctr_text
                                        FROM (
                                            provinces 
                                            RIGHT JOIN municipalities 
                                                ON provinces.prov_no = municipalities.prov_no) 
                                            RIGHT JOIN (landing_sites 
                                            RIGHT JOIN dbo_LC_FG_sample_day 
                                                ON landing_sites.landing_site_id = dbo_LC_FG_sample_day.land_ctr_id) 
                                                ON municipalities.mun_no = landing_sites.municipality
                                            WHERE 
                                                dbo_LC_FG_sample_day.ground_id=@fg
                                                AND dbo_LC_FG_sample_day.fma= @fma
                                                AND dbo_LC_FG_sample_day.region_id= @region";

                        try
                        {
                            conn.Open();
                            MySqlDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                var prv = NSAPEntities.ProvinceViewModel.GetProvince(int.Parse(dr["prov_no"].ToString()));
                                SampledLandingSite sls = new SampledLandingSite
                                {
                                    FishingGround = fg,
                                    LandingSiteName = dr["landing_site_name"].ToString(),
                                    Barangay = dr["barangay"].ToString(),
                                    Province = prv,
                                    Municipality = prv.Municipalities.GetMunicipality(int.Parse(dr["mun_no"].ToString())),
                                    LandingSiteText = dr["land_ctr_text"].ToString(),
                                    LandingSiteID = int.Parse(dr["land_ctr_id"].ToString()),
                                };
                                thisList.Add(sls);
                            }
                        }
                        catch (OleDbException oex)
                        {

                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            else
            {
                using (var conn = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@fg", fg.Code);
                        cmd.Parameters.AddWithValue("@fma", fma.FMAID);
                        cmd.Parameters.AddWithValue("@region", nsapRegion.Code);
                        cmd.CommandText = @"SELECT DISTINCT 
                                                dbo_LC_FG_sample_day.land_ctr_id, 
                                                landingSite.LandingSiteName, 
                                                landingSite.Barangay, 
                                                Municipalities.MunNo, 
                                                Municipalities.Municipality, 
                                                Municipalities.ProvNo, 
                                                Provinces.ProvinceName, 
                                                dbo_LC_FG_sample_day.land_ctr_text
                                        FROM (
                                            Provinces 
                                            RIGHT JOIN Municipalities 
                                                ON Provinces.ProvNo = Municipalities.ProvNo) 
                                            RIGHT JOIN (landingSite 
                                            RIGHT JOIN dbo_LC_FG_sample_day 
                                                ON landingSite.LandingSiteID = dbo_LC_FG_sample_day.land_ctr_id) 
                                                ON Municipalities.MunNo = landingSite.Municipality
                                        WHERE dbo_LC_FG_sample_day.ground_id=@fg AND 
                                              dbo_LC_FG_sample_day.fma=@fma AND
                                              dbo_LC_FG_sample_day.region_id=@region;";
                        try
                        {
                            conn.Open();
                            OleDbDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                if (dr["ProvNo"] != DBNull.Value)
                                {
                                    Province prv = NSAPEntities.ProvinceViewModel.GetProvince(int.Parse(dr["ProvNo"]?.ToString()));
                                    SampledLandingSite sls = new SampledLandingSite
                                    {
                                        FishingGround = fg,
                                        LandingSiteName = dr["LandingSiteName"].ToString(),
                                        Barangay = dr["Barangay"].ToString(),
                                        Province = prv,
                                        Municipality = prv.Municipalities.GetMunicipality(int.Parse(dr["MunNo"].ToString())),
                                        LandingSiteText = dr["land_ctr_text"].ToString(),
                                        LandingSiteID = int.Parse(dr["land_ctr_id"].ToString())
                                    };
                                    thisList.Add(sls);
                                }
                                else
                                {
                                    SampledLandingSite sl = new SampledLandingSite
                                    {
                                        FishingGround = fg,
                                        LandingSiteText = dr["land_ctr_text"].ToString(),
                                    };
                                    thisList.Add(sl);
                                }

                            }
                        }
                        catch (OleDbException oex)
                        {

                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            return thisList;
        }
    }
}
