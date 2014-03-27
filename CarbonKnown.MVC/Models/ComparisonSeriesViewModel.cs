using System;
using System.Collections.Generic;

namespace CarbonKnown.MVC.Models
{
// ReSharper disable InconsistentNaming
    public class ComparisonSeriesViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string uom { get; set; }
        public string costCode { get; set; }
        public Guid? activityId { get; set; }
        public bool target { get; set; }
        public IEnumerable<decimal> data { get; set; } 
    }
// ReSharper restore InconsistentNaming
}