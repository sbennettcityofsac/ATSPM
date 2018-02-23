﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;

namespace MOE.Common.Models
{
    public class EventCountAggregation : Aggregation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        [Required]
        public override DateTime BinStartTime { get; set; }

        [Required]
        public string SignalId { get; set; }

        [Required]
        public int EventCount { get; set; }

        public sealed class
            EventCountAggregationClassMap : ClassMap<EventCountAggregation>
        {
            public EventCountAggregationClassMap()
            {
                Map(m => m.Id).Name("Record Number");
                Map(m => m.BinStartTime).Name("Bin Start Time");
                Map(m => m.SignalId).Name("Signal ID");
                Map(m => m.EventCount).Name("Event Count");
            }
        }
    }
}