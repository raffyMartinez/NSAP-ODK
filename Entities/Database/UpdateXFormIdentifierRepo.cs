using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NSAP_ODK.Entities.Database
{
    public class UpdateXFormIdentifierItem
    {
        public int _id { get; set; }
        public string _xform_id_string { get; set; }
        public string _uuid { get; set; }
    }

    public class UpdateXFormIdentifierRoot
    {
        public int count { get; set; }
        public object next { get; set; }
        public object previous { get; set; }
        public List<UpdateXFormIdentifierItem> results { get; set; }
    }
    class UpdateXFormIdentifierRepository
    {
        public static UpdateXFormIdentifierRoot Updates { get; internal set; }
        public static string JSON { get; set; }

        public static void CreateXFormIdentifierUpdatesFromJSON()
        {
            Updates = JsonConvert.DeserializeObject<UpdateXFormIdentifierRoot>(JSON);
        }

        public static Task<int> UpdateDatabase(int round)
        {

            return NSAPEntities.VesselUnloadViewModel.UpdateXFormIdentifierColumnAsync(Updates.results, round);

        }
    }
}
