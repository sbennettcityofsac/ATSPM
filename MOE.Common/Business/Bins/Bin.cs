﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOE.Common.Models;

namespace MOE.Common.Business
{
    public class Bin
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Sum { get; set; }
        public double Average { get; set; }
    }
}