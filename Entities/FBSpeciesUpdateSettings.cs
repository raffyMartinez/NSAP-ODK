using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public enum FBSpeciesUpdateStatus
    {
        FBSpeciesStatus_SettingsNotFound,
        FBSpeciesStatus_UpdatedNoChanges,
        FBSpeciesStatus_UpdatedWithChanges,
    }

    public enum FBSpeciesUpdateMode
    {
        UpdateModeUpdateDoNotAdd,
        UpdateModeUpdateAndAdd
    }
    public class FBSpeciesUpdateSettings
    {
        public int UpdateFileRowCount { get; set; }
        public long FileSize { get; set; }
        public string FileName { get; set; }

        public int FBSpeciesCount { get; set; }

        public FBSpeciesUpdateMode UpdateMode { get; set; }
    }


}
