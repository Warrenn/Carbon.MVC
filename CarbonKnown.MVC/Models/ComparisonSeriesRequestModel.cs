using System;
using CarbonKnown.DAL.Models;

namespace CarbonKnown.MVC.Models
{
// ReSharper disable InconsistentNaming
    public class ComparisonSeriesRequestModel
    {
        public string costCode { get; set; }
        public Guid? activityId { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public TargetType targetType { get; set; }
        public bool target { get; set; }
    }
// ReSharper restore InconsistentNaming
}