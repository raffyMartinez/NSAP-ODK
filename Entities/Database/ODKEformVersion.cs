﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class ODKEformVersion
    {
        public string Version { get; set; }
        public int Count { get; set; }
        public DateTime FirstSubmission { get; set; }
    }
}
