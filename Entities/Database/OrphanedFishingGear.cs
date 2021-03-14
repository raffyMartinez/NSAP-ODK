﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class OrphanedFishingGear
    {
        public string Name { get; set; }
        public List<GearUnload> GearUnloads{ get; set; }

        public NSAPRegion Region { get { return GearUnloads[0].Parent.NSAPRegion; } }

        public FMA FMA { get { return GearUnloads[0].Parent.FMA; } }

        public FishingGround FishingGround { get { return GearUnloads[0].Parent.FishingGround; } }

        public int NumberOfUnload { get { return GearUnloads.Count; } }

        public bool ForReplacement { get; set; }
    }
}