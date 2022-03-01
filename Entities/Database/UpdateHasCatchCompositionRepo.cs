using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{

    public  class UpdateHasCatchCompositionResultItem
    {
        public  int _id { get; set; }

        [JsonProperty("catch_comp_group/include_catchcomp")]
        public  string CatchCompGroupIncludeCatchcomp { get; set; }

        public  string _uuid { get; set; }
    }

    public class UpdateHasCatchCompositionRoot
    {

        public  int count { get; set; }
        public  string next { get; set; }
        public  object previous { get; set; }
        public  List<UpdateHasCatchCompositionResultItem> results { get; set; }
    }

    public static class UpdateHasCatchCompositionRepository
    {
        public static UpdateHasCatchCompositionRoot Updates { get; internal set; }
        public static string JSON { get; set; }

        public static void CreateCatchCompUpdatesFromJSON()
        {
            Updates = JsonConvert.DeserializeObject<UpdateHasCatchCompositionRoot>(JSON);
        }

        public static Task<int> UpdateDatabase(int round)
        {

            return NSAPEntities.VesselUnloadViewModel.UpdateHasCatchCompositionColumnsAsync(Updates.results,round);

        }

        private static void VesselUnloadViewModel_ColumnUpdatedEvent(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }

}
