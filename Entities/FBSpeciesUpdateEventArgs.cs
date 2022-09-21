using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public enum FBSpeciesUpdateType
    {
        UpdateTypeNone,
        UpdateTypeReadingUpdateFile,
        UpdateTypeCreatingUpdateList,
        UpdateTypeAddingSpecies,
        UpdateTypeUpdatingSpecies,
        UpdateTypeFinishedUpdatingFBSpecies,
        UpdateTypeFetchedFbSpeciesList
    }
    public class FBSpeciesUpdateEventArgs:EventArgs
    {
        public FBSpeciesUpdateType UpdateType { get; set; }
        public int RowCountInUpdateFile { get; set; }

        public int FBSpeciesUpdateCount { get; set; }

        public int FbSpeciesCount { get; set; }

        public FBSpecies CurrentSpecies { get; set; }
    }
}
