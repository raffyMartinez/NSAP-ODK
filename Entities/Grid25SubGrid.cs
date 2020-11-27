using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public class Grid25SubGrid
    {
        public Grid25GridCell Parent { get; private set; }
        public byte SubGrid { get; private set; }

        public Grid25SubGrid(Grid25GridCell parent, byte subGrid)
        {
            Parent = parent;
            SubGrid = subGrid;
        }
    }
}
