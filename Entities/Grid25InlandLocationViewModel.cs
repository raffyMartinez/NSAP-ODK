using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
namespace NSAP_ODK.Entities
{
    public class Grid25InlandLocationViewModel
    {
        public ObservableCollection<Grid25InlandLocation> Grid25InlandLocationCollection { get; set; }
        private Grid25InlandLocationRepo Grids25InlandLocations { get; set; }

        public int Count { get { return Grid25InlandLocationCollection.Count; } }

        public Grid25InlandLocationViewModel()
        {
            Grids25InlandLocations = new Grid25InlandLocationRepo();
            Grid25InlandLocationCollection = new ObservableCollection<Grid25InlandLocation>(Grids25InlandLocations.Grid25InlandLocations);
        }

        public bool GridIsInland(Grid25GridCell gridCell)
        {
            return Grid25InlandLocationCollection.FirstOrDefault(t => t.Grid25GridCell.Equals(gridCell)) != null;
        }
        public List<Grid25InlandLocation> GetAllGrid25InlandCells(string utmZone = "")
        {
            if (utmZone.Length == 0)
            {
                return Grid25InlandLocationCollection.ToList();
            }
            else
            {
                UTMZone z = null;
                switch (utmZone)
                {
                    case "50N":
                        z = new UTMZone(zoneNumber: 50, zoneNS: 'N');
                        break;
                    case "51N":
                        z = new UTMZone(zoneNumber: 51, zoneNS: 'N');
                        break;
                }

                if (z != null)
                {
                    return Grid25InlandLocationCollection.Where(t => t.Grid25GridCell.UTMZone == z).ToList();
                }
                else
                {
                    return null;
                }
            }
        }



    }
}
