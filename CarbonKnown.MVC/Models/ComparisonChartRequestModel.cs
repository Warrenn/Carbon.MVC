using System;
using CarbonKnown.DAL.Models;

namespace CarbonKnown.MVC.Models
{
// ReSharper disable InconsistentNaming
    public class ComparisonChartRequestModel
    {
        public TargetType targetType { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
    }
// ReSharper restore InconsistentNaming
}