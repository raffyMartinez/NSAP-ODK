using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using NSAP_ODK.Utilities;
namespace NSAP_ODK.Entities
{
    public class SpeciesMultiTaxaRepository
    {
        public List<SpeciesMultiTaxa> ListSpeciesMultiTaxa { get; set; }

        public SpeciesMultiTaxaRepository()
        {
            ListSpeciesMultiTaxa = getList();
        }

        private List<SpeciesMultiTaxa> getList()
        {
            List<SpeciesMultiTaxa> thisList = new List<SpeciesMultiTaxa>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    const string query = @"SELECT 
                                            'FIS' AS taxa_key, 
                                            phFish.SpeciesID AS name_key, 
                                            [genus] & ' ' & [species] AS label_key, 
                                            phFish.LengthMax AS len_max, 
                                            phFish.LengthType AS len_type
                                        FROM phFish;

                                        union all

                                        SELECT 
                                            taxa.TaxaCode AS taxa, 
                                            notFishSpecies.SpeciesID AS name_key, 
                                            [Genus] & ' ' & [Species] AS label_key, 
                                            notFishSpecies.MaxSize, 
                                            notFishSpecies.SizeIndicator
                                        FROM taxa INNER JOIN 
                                            notFishSpecies ON taxa.TaxaCode = notFishSpecies.Taxa;";


                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        thisList.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            SpeciesMultiTaxa smt = new SpeciesMultiTaxa();
                            smt.SpeciesID = Convert.ToInt32(dr["name_key"]);
                            smt.SpeciesName = dr["label_key"].ToString();
                            smt.Taxa = NSAPEntities.TaxaViewModel.GetTaxa(dr["taxa_key"].ToString());    
                            if(double.TryParse(dr["len_max"].ToString(),out Double v))
                            {
                                smt.Size = v;
                            }
                            if(dr["len_type"].ToString().Length>0)
                            {
                                smt.SizeType = NSAPEntities.SizeTypeViewModel.GetSizeType(dr["len_type"].ToString());
                            }
                            thisList.Add(smt);
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
    }
}
