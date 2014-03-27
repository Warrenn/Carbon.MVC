using System;
using CarbonKnown.DAL.Models;

namespace CarbonKnown.MVC.Models
{
    // ReSharper disable InconsistentNaming
    public class ComparisonAddSeriesModel
    {
        public Guid activityId { get; set; }
        public string name { get; set; }
        public string costCode { get; set; }
        public bool target { get; set; }
        public TargetType targetType { get; set; }
    }

    // ReSharper restore InconsistentNaming
}