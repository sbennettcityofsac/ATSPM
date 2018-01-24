﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOE.Common.Business.Bins
{
    public class BinsContainer
    {
        public List<Bin> Bins { get; set; } = new List<Bin>();
        public int SumValue { get { return Bins.Sum(b => b.Sum); }}
        public int AverageValue { get { return Convert.ToInt32(Math.Round(Bins.Average(b => b.Sum))); } }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public BinsContainer(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }
    }
}