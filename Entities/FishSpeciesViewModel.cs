using Newtonsoft.Json;
using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace NSAP_ODK.Entities
{
    public class OBIResponseRoot
    {
        public int total { get; set; }
        public List<OBIAPIResponse> results { get; set; }
    }
    public class OBIAPIResponse
    {
        public string scientificName { get; set; }
        public string scientificNameAuthorship { get; set; }
        public int taxonID { get; set; }
        public int ncbi_id { get; set; }
        public string taxonRank { get; set; }
        public string taxonomicStatus { get; set; }
        public string acceptedNameUsage { get; set; }
        public int acceptedNameUsageID { get; set; }
        public bool is_marine { get; set; }
        public bool is_brackish { get; set; }
        public bool is_freshwater { get; set; }
        public bool is_terrestrial { get; set; }
        public string kingdom { get; set; }
        public string phylum { get; set; }
        public string subphylum { get; set; }
        public string infraphylum { get; set; }
        public string @class { get; set; }
        public string subclass { get; set; }
        public string order { get; set; }
        public string family { get; set; }
        public string subfamily { get; set; }
        public string genus { get; set; }
        public string species { get; set; }
        public int kingdomid { get; set; }
        public int phylumid { get; set; }
        public int subphylumid { get; set; }
        public int infraphylumid { get; set; }
        public int classid { get; set; }
        public int subclassid { get; set; }
        public int orderid { get; set; }
        public int familyid { get; set; }
        public int subfamilyid { get; set; }
        public int genusid { get; set; }
        public int speciesid { get; set; }
    }

    public class ResponseSpeciesDetail
    {
        public int Count { get; set; }
        public int Returned { get; set; }
        public string Error { get; set; }
        public IList<SpeciesDetail> Data { get; set; }
    }

    public class ResponseTaxa
    {
        public int Count { get; set; }
        public int Returned { get; set; }
        public string Error { get; set; }
        public IList<SpecData> Data { get; set; }
    }

    public class SpeciesDetail
    {
        public string Family { get; set; }
        public string SpecCode { get; set; }
        public string Genus { get; set; }
        public string Species { get; set; }
        public string SpeciesRefNo { get; set; }
        public string Author { get; set; }
        public string FBname { get; set; }
        public string PicPreferredName { get; set; }
        public string PicPreferredNameM { get; set; }
        public string PicPreferredNameF { get; set; }
        public string PicPreferredNameJ { get; set; }
        public string FamCode { get; set; }
        public string Subfamily { get; set; }
        public string GenCode { get; set; }
        public string SubGenCode { get; set; }
        public string BodyShapeI { get; set; }
        public string Source { get; set; }
        public string AuthorRef { get; set; }
        public string Remark { get; set; }
        public string TaxIssue { get; set; }
        public string Fresh { get; set; }
        public string Brack { get; set; }
        public string Saltwater { get; set; }
        public string DemersPelag { get; set; }
        public object Amphibious { get; set; }
        public object AmphibiousRef { get; set; }
        public string AnaCat { get; set; }
        public string MigratRef { get; set; }
        public string DepthRangeShallow { get; set; }
        public string DepthRangeDeep { get; set; }
        public string DepthRangeRef { get; set; }
        public string DepthRangeComShallow { get; set; }
        public string DepthRangeComDeep { get; set; }
        public string DepthComRef { get; set; }
        public string LongevityWild { get; set; }
        public string LongevityWildRef { get; set; }
        public string LongevityCaptive { get; set; }
        public string LongevityCapRef { get; set; }
        public string Vulnerability { get; set; }

        //this is max length
        public string Length { get; set; }

        public string LTypeMaxM { get; set; }
        public string LengthFemale { get; set; }
        public string LTypeMaxF { get; set; }
        public string MaxLengthRef { get; set; }

        //this is common length
        public string CommonLength { get; set; }

        public string LTypeComM { get; set; }
        public string CommonLengthF { get; set; }
        public string LTypeComF { get; set; }
        public string CommonLengthRef { get; set; }
        public string Weight { get; set; }
        public string WeightFemale { get; set; }
        public string MaxWeightRef { get; set; }
        public string Pic { get; set; }
        public string PictureFemale { get; set; }
        public string LarvaPic { get; set; }
        public string EggPic { get; set; }
        public string ImportanceRef { get; set; }
        public string Importance { get; set; }
        public string PriceCateg { get; set; }
        public string PriceReliability { get; set; }
        public string Remarks7 { get; set; }
        public string LandingStatistics { get; set; }
        public string Landings { get; set; }
        public string MainCatchingMethod { get; set; }
        public string II { get; set; }
        public string MSeines { get; set; }
        public string MGillnets { get; set; }
        public string MCastnets { get; set; }
        public string MTraps { get; set; }
        public string MSpears { get; set; }
        public string MTrawls { get; set; }
        public string MDredges { get; set; }
        public string MLiftnets { get; set; }
        public string MHooksLines { get; set; }
        public string MOther { get; set; }
        public string UsedforAquaculture { get; set; }
        public string LifeCycle { get; set; }
        public string AquacultureRef { get; set; }
        public string UsedasBait { get; set; }
        public string BaitRef { get; set; }
        public string Aquarium { get; set; }
        public string AquariumFishII { get; set; }
        public string AquariumRef { get; set; }
        public string GameFish { get; set; }
        public string GameRef { get; set; }
        public string Dangerous { get; set; }
        public string DangerousRef { get; set; }
        public string Electrogenic { get; set; }
        public string ElectroRef { get; set; }
        public string Complete { get; set; }
        public string GoogleImage { get; set; }
        public string Comments { get; set; }
        public string Profile { get; set; }
        public string PD50 { get; set; }
        public string Emblematic { get; set; }
        public string Entered { get; set; }
        public string DateEntered { get; set; }
        public string Modified { get; set; }
        public string DateModified { get; set; }
        public string Expert { get; set; }
        public string DateChecked { get; set; }
        public string TS { get; set; }
        public string image { get; set; }
    }

    public class SpecData
    {
        public string SpecCode { get; set; }
        public string Genus { get; set; }
        public string Species { get; set; }
        public string SpeciesRefNo { get; set; }
        public string Author { get; set; }
        public string FBname { get; set; }

        public string FamCode { get; set; }
        public string GenCode { get; set; }
        public string SubGenCode { get; set; }

        public string Remark { get; set; }
        public string SubFamily { get; set; }
        public string Family { get; set; }
        public string Order { get; set; }
        public string Class { get; set; }
    }

    public class ResultQueryAPI
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public SpeciesDetail SpeciesDetail { get; set; }
    }

    public class FishSpeciesViewModel
    {
        private bool _editSuccess;
        private static HttpClient client = new HttpClient();
        public ObservableCollection<FishSpecies> SpeciesCollection { get; set; }
        private FishSpeciesRepository Specieses { get; set; }

        public FishSpeciesViewModel()
        {
            Specieses = new FishSpeciesRepository();
            SpeciesCollection = new ObservableCollection<FishSpecies>(Specieses.Specieses);
            SpeciesCollection.CollectionChanged += Species_CollectionChanged;
        }

        public DataSet DataSet()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("Fish sepecies");


            DataColumn dc = new DataColumn { ColumnName = "Genus", DataType = typeof(string) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Species", DataType = typeof(string) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Id", DataType = typeof(int) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Family", DataType = typeof(string) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Max length", DataType = typeof(double) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Length type", DataType = typeof(string) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Synonym", DataType = typeof(string) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Importance", DataType = typeof(string) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Main catching method", DataType = typeof(string) };
            dt.Columns.Add(dc);

            foreach (var item in SpeciesCollection)
            {
                var row = dt.NewRow();
                row["Genus"] = item.GenericName;
                row["Species"] = item.SpecificName;
                row["Id"] = item.SpeciesCode;
                row["Family"] = item.Family;
                if (item.LengthMax == null)
                {
                    row["Max length"] = DBNull.Value;
                }
                else
                {
                    row["Max length"] = item.LengthMax;
                }
                row["Length type"] = item.LengthType;
                row["Synonym"] = item.Synonym;
                row["Importance"] = item.Importance;
                row["Main catching method"] = item.MainCatchingMethod;
                dt.Rows.Add(row);
            }
            ds.Tables.Add(dt);
            return ds;
        }

        public async Task<OBIResponseRoot> RequestDataFromOBI(string speciesName)
        {
           
            var bytes = await client.GetByteArrayAsync($"https://api.obis.org/v3/taxon/{speciesName}");
            Encoding encoding = Encoding.GetEncoding("utf-8");
            string response = encoding.GetString(bytes, 0, bytes.Length);
            return  JsonConvert.DeserializeObject<OBIResponseRoot>(response);
        }
        public int NextRecordNumber
        {
            get
            {
                if (SpeciesCollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    //return SpeciesCollection.Max(t => t.RowNumber) + 1;
                    return Specieses.MaxRecordNumber() + 1;
                }
            }
        }

        public async Task<ResultQueryAPI> GetSpeciesDataFromAPI(FishSpecies sp, bool updateDatabase = false)
        {
            ResultQueryAPI rq;
            if (sp.SpeciesCode != null)
            {
                rq = await GetSpeciesDetailEx(sp);
            }
            else
            {
                rq = await GetSpeciesDetail(sp);
            }
            if (updateDatabase)
            {
                sp.Importance = rq.SpeciesDetail.Importance;
                if (sp.SpeciesCode == null)
                {
                    sp.Family = rq.SpeciesDetail.Family;
                    sp.SpeciesCode = int.Parse(rq.SpeciesDetail.SpecCode);
                }
                sp.MainCatchingMethod = rq.SpeciesDetail.MainCatchingMethod;
                if (double.TryParse(rq.SpeciesDetail.CommonLength, out double cl))
                {
                    sp.LengthCommon = cl;
                }
                if (double.TryParse(rq.SpeciesDetail.Length, out double l))
                {
                    sp.LengthMax = l;
                }
                if (rq.SpeciesDetail.LTypeMaxM != null)
                {
                    sp.LengthType = NSAPEntities.SizeTypeViewModel.GetSizeType(rq.SpeciesDetail.LTypeMaxM);
                }
                else if (rq.SpeciesDetail.LTypeMaxF != null)
                {
                    sp.LengthType = NSAPEntities.SizeTypeViewModel.GetSizeType(rq.SpeciesDetail.LTypeMaxF);
                }
                UpdateRecordInRepo(sp);
            }
            return rq;
        }

        public async Task<ResultQueryAPI> GetSpeciesDetailEx(FishSpecies sp)
        {
            ResponseSpeciesDetail specDetail;
            try
            {
                var bytes = await client.GetByteArrayAsync($"https://fishbase.ropensci.org/species/{sp.SpeciesCode}?");
                Encoding encoding = Encoding.GetEncoding("utf-8");
                string response = encoding.GetString(bytes, 0, bytes.Length);
                specDetail = JsonConvert.DeserializeObject<ResponseSpeciesDetail>(response);
            }
            catch (HttpRequestException hex)
            {
                return new ResultQueryAPI() { Success = false, Message = hex.Message, SpeciesDetail = null };
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new ResultQueryAPI() { Success = false, Message = ex.Message, SpeciesDetail = null };
            }
            return new ResultQueryAPI() { Success = true, Message = "", SpeciesDetail = specDetail.Data[0] };
        }

        private static async Task<ResultQueryAPI> GetSpeciesDetail(FishSpecies sp)
        {
            ResponseSpeciesDetail specDetail;
            try
            {
                var bytes = await client.GetByteArrayAsync($"https://fishbase.ropensci.org/taxa?Genus={sp.GenericName}&Species={sp.SpecificName}");
                Encoding encoding = Encoding.GetEncoding("utf-8");
                string response = encoding.GetString(bytes, 0, bytes.Length);
                var specFromTaxa = JsonConvert.DeserializeObject<ResponseTaxa>(response);

                bytes = await client.GetByteArrayAsync($"https://fishbase.ropensci.org/species/{specFromTaxa.Data[0].SpecCode}?");
                encoding = Encoding.GetEncoding("utf-8");
                response = encoding.GetString(bytes, 0, bytes.Length);
                specDetail = JsonConvert.DeserializeObject<ResponseSpeciesDetail>(response);
                specDetail.Data[0].Family = specFromTaxa.Data[0].Family;
            }
            catch (HttpRequestException hex)
            {
                return new ResultQueryAPI() { Success = false, Message = hex.Message, SpeciesDetail = null };
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new ResultQueryAPI() { Success = false, Message = ex.Message, SpeciesDetail = null };
            }
            return new ResultQueryAPI() { Success = true, Message = "", SpeciesDetail = specDetail.Data[0] };
        }

        public List<string>GetAllGenus()
        {
            List<string> listGenus = new List<string>();

            foreach(var g in  SpeciesCollection.OrderBy(t=>t.GenericName).GroupBy(t=>t.GenericName).ToList())
            {
                listGenus.Add(g.Key);
            }
            return listGenus;
        }
        public List<FishSpecies> GetAllSpecies(string search="")
        {
            if (search.Length > 0)
            {
                return SpeciesCollection.Where(t => t.ToString().ToLower().Contains(search.ToLower())).ToList();
            }
            else
            {
                return SpeciesCollection.OrderBy(t => t.ToString()).ToList();
            }
        }


        public List<FishSpeciesForCSV> BuildSpeciesCSVSource()
        {
            List<FishSpeciesForCSV> list = new List<FishSpeciesForCSV>();
            try
            {
                foreach (var item in SpeciesCollection.OrderBy(t => t.GenericName).ThenBy(t => t.SpecificName))
                {
                    list.Add(new FishSpeciesForCSV
                    {
                        SpeciesCode = (int)item.SpeciesCode,
                        Name = item.ToString(),
                        MaxLength = item.LengthMax,
                        LengthType = item.LengthType,
                        SortName = item.ToString()
                    });

                    if (item.NameInOldFishbase.Length > 0 && item.NameInOldFishbase != item.ToString())
                    {
                        list.Add(new FishSpeciesForCSV
                        {
                            SpeciesCode = (int)item.SpeciesCode,
                            Name = $"{item.NameInOldFishbase} <span style=\"color:blue\">({item.ToString()})</span>",
                            MaxLength = item.LengthMax,
                            LengthType = item.LengthType,
                            SortName = item.NameInOldFishbase
                        });
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return list;
        }
        public FishSpecies GetSpecies (string species)
        {
            return SpeciesCollection.FirstOrDefault(t => t.ToString() == species);
        }

        //public FishSpecies GetSpecies

        public FishSpecies GetSpecies(int speciesID)
        {
            return SpeciesCollection.FirstOrDefault(n => n.SpeciesCode == speciesID);
        }

        private void Species_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        _editSuccess= Specieses.Add(SpeciesCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<FishSpecies> tempListOfRemovedItems = e.OldItems.OfType<FishSpecies>().ToList();
                        _editSuccess= Specieses.Delete(tempListOfRemovedItems[0].RowNumber);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<FishSpecies> tempListOfFishers = e.NewItems.OfType<FishSpecies>().ToList();
                        _editSuccess= Specieses.Update(tempListOfFishers[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return SpeciesCollection.Count; }
        }

        public bool AddRecordToRepo(FishSpecies species)
        {
            if (species == null)
                throw new ArgumentNullException("Error: The argument is Null");

            SpeciesCollection.Add(species);
            return _editSuccess;
        }

        public bool UpdateRecordInRepo(FishSpecies species)
        {
            if (species.RowNumber == 0)
                throw new Exception("Error: ID cannot be null");

            int index = 0;
            while (index < SpeciesCollection.Count)
            {
                if (SpeciesCollection[index].RowNumber == species.RowNumber)
                {
                    SpeciesCollection[index] = species;
                    break;
                }
                index++;
            }
            return _editSuccess;
        }

        public bool DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < SpeciesCollection.Count)
            {
                if (SpeciesCollection[index].RowNumber == id)
                {
                    SpeciesCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return _editSuccess;
        }

        public bool SpeciesNameExist(string genus, string species)
        {
            foreach (FishSpecies fs in SpeciesCollection)
            {
                if (fs.GenericName == genus && fs.SpecificName == species)
                {
                    return true;
                }
            }
            return false;
        }

        public EntityValidationResult ValidateFishSpecies(FishSpecies species, bool isNew, string oldGenus, string oldSpecies)
        {
            EntityValidationResult evr = new EntityValidationResult();

            if (string.IsNullOrEmpty(species.SpecificName) || string.IsNullOrEmpty(species.GenericName))
            {
                evr.AddMessage("Generic and specific names must not be empty");
            }
            else if (isNew && SpeciesNameExist(species.GenericName, species.SpecificName))
            {
                evr.AddMessage("Species name already used");
            }

            if (species.Family == null)
            {
                evr.AddMessage("Family cannot be empty");
            }

            if (!isNew && species.GenericName.Length > 0 && species.SpecificName.Length > 0
                && oldGenus != species.GenericName && oldSpecies != species.SpecificName
                && SpeciesNameExist(oldGenus, oldSpecies))
            {
                evr.AddMessage("Species name already used");
            }

            return evr;
        }
    }
}