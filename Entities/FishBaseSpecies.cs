using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using NSAP_ODK.NSAPMysql;
namespace NSAP_ODK.Entities
{
    public class FishBaseSpecies
    {
        public FishBaseSpecies(string genus, string species, int spCode)
        {
            Genus = genus;
            Species = species;
            SpecCode = spCode;
        }

        public string Genus { get; set; }
        public string Species { get; set; }
        public int SpecCode { get; set; }
        public int? SpeciesRefNo { get; set; }
        public string Author { get; set; }
        public int? AuthorRef { get; set; }
        public string FBName { get; set; }
        public int? FamilyCode { get; set; }
        public string SubFamily { get; set; }

        public bool SaveToMySQL()
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Parameters.AddWithValue("@genus", Genus);
                    cmd.Parameters.AddWithValue("@species", Species);
                    cmd.Parameters.AddWithValue("@sp_code", SpecCode);
                    cmd.CommandText = "Insert into fb_species (genus, species,spec_code) values (@genus, @species, @sp_code)";
                    try
                    {
                        conn.Open();
                        success= cmd.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException msex)
                    {
                        switch (msex.ErrorCode)
                        {
                            case -2147467259:
                                //duplicated unique index error
                                break;
                            default:
                                Utilities.Logger.Log(msex);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Utilities.Logger.Log(ex);
                    }
                }
            }
            return success;
        }
    }
}
