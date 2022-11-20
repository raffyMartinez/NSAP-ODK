using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NSAP_ODK.Entities.Media_csv
{
    public class VesselSelectRepository
    {
        string _pathToCSV;
        FisheriesSector _sector;

        public List<VesselSelect> VesselSelects { get; set; }
        public VesselSelectRepository(string pathToCSV, FisheriesSector sector)
        {
            _pathToCSV = pathToCSV;
            _sector = sector;
            VesselSelects = getVesselSelect();
        }

        private List<VesselSelect> getVesselSelect()
        {
            List<VesselSelect> values = File.ReadAllLines(_pathToCSV)
                                           .Where(t => t.Length > 0)
                                           .Skip(1)
                                           .Select(v => ParseCSVLine(v))
                                           .ToList();
            return values;
        }

        private VesselSelect ParseCSVLine(string csvLine)
        {
            //var arr = csvLine.Split(',');
            var arr = Utilities.Global.SplitQualified(csvLine, ',', '"', true);
            var nsapRegionID = arr[3];
            VesselSelect vs;
            try
            {
                vs = new VesselSelect
                {
                    RowID = Convert.ToInt32(arr[1]),
                    Name = arr[2].Replace("\"", ""),
                    NSAPRegionID = nsapRegionID,
                    FisheriesSector = _sector,
                    NSAPRegion = GetNSAPRegion(nsapRegionID)
                };
            }
            catch (Exception ex)
            {
                return null;
            }
            return vs;
        }

        private NSAPRegion GetNSAPRegion(string id)
        {
            return NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(id);
        }
    }
}
