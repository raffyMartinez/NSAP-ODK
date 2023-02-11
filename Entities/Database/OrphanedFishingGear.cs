using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class OrphanedFishingGear
    {
        public string Name { get; set; }
        public List<GearUnload> GearUnloads { get; set; }

        public NSAPRegion Region
        {
            get
            {
                if (GearUnloads.Count > 0)
                {
                    return GearUnloads[0].Parent.NSAPRegion;
                }
                else
                {
                    return null;
                }
            }
        }

        public FMA FMA
        {
            get
            {
                if (GearUnloads.Count == 0)
                {
                    return null;
                }
                else
                {
                    return GearUnloads[0].Parent.FMA;
                }
            }
        }
        public string EnumeratorNameList
        {
            get
            {
                var e = "";
                int count = 0;
                foreach (var item in EnumeratorNames)
                {
                    e += $"{item}, ";
                    count++;
                    if (count % 3 == 0)
                    {
                        e += "\r\n";
                    }
                }
                return e.Trim(new char[] { ',', ' ', '\r', '\n' });
            }
        }
        public List<string> EnumeratorNames
        {
            get
            {
                HashSet<string> enumerators = new HashSet<string>();
                if (GearUnloads.Count > 0)
                {
                    foreach (var item in GearUnloads)
                    {
                        if(item.VesselUnloadViewModel==null)
                        {
                            item.VesselUnloadViewModel = new VesselUnloadViewModel(parent: item);
                        }
                        //var h = item.VesselUnloadViewModel.VesselUnloadCollection.Select(t => t.EnumeratorName).ToHashSet();
                        enumerators.UnionWith(item.VesselUnloadViewModel.VesselUnloadCollection.Select(t => t.EnumeratorName).ToHashSet());
                    }
                    return enumerators.ToList();
                }
                else
                {
                    return null;
                }
            }
        }
        public FishingGround FishingGround
        {
            get
            {
                if (GearUnloads.Count > 0)
                {
                    return GearUnloads[0].Parent.FishingGround;
                }
                else
                {
                    return null;
                }
            }
        }

        public int NumberOfUnload { get { return GearUnloads.Count; } }

        public bool ForReplacement { get; set; }
    }
}
