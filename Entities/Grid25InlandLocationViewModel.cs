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



        public Grid25InlandLocationViewModel()
        {
            Grids25InlandLocations = new Grid25InlandLocationRepo();
            Grid25InlandLocationCollection = new ObservableCollection<Grid25InlandLocation>(Grids25InlandLocations.Grid25InlandLocations);
        }
        public List<Grid25InlandLocation> GetAllGrid25InlandCells()
        {
            return Grid25InlandLocationCollection.ToList();
        }


        
    }
}
