using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class SpeciesName_Weight
    {
        public SpeciesName_Weight(string name_weight)
        {
            if (name_weight.Length > 0)
            {
                string wt = "";
                for (int x = 0; x < name_weight.Length; x++)
                {
                    var c = name_weight[x];
                    if (name_weight[x] >= '0' && name_weight[x] <= '9' || name_weight[x] == '.')
                    {
                        wt += name_weight[x];
                    }
                    else
                    {
                        SpeciesName += name_weight[x];
                    }
                }

                SpeciesName = SpeciesName.Trim(new char[] { ' ', '-' });

                if (double.TryParse(wt, out double v))
                {
                    Weight = v;
                }
            }
        }
        public string SpeciesName { get; set; }
        public double? Weight { get; set; }

        public override string ToString()
        {
            if (Weight != null)
            {
                return $"{SpeciesName}-{(double)Weight}"; 
            }
            else
            {
                return $"{SpeciesName}"; 
            }
        }

        public static List<SpeciesName_Weight> ParseMultiSpeciesRow(string multisSpeciesRow)
        {
            List<SpeciesName_Weight> list = new List<SpeciesName_Weight>();
            foreach(var item in multisSpeciesRow.Split('\n'))
            {
                if (item.Length > 0)
                {
                    list.Add(new SpeciesName_Weight(item));
                }
            }
            return list;
        }
    }
}
