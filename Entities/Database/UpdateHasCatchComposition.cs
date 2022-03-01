using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    class UpdateHasCatchComposition
    {
        public string RowGUID { get; set; }
        public string HasCatchCompositionString { get; set; }

        public bool HasCatchComposition { 
            get 
            { 
                return HasCatchCompositionString == "yes" ? true : false; 
            } 
        }
    }
}
