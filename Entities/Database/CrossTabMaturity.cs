﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class CrossTabMaturity
    {
        public CrossTabCommon CrossTabCommon { get; set; }
        public double? Length { get; set; }
        public double? Weight { get; set; }
        public string Sex { get; set; }
        public string MaturityStage { get; set; }
        public string GutContent { get; set; }

    }
}