using System.Collections.Generic;

namespace CarbonKnown.MVC.Models
{
// ReSharper disable InconsistentNaming
    public class ComparisonChartViewModel
    {
        public IEnumerable<string> categories { get; set; }
        public ComparisonSeriesViewModel[] series { get; set; }
        public ComparisonChartRequestModel request { get; set; }
    }

// ReSharper restore InconsistentNaming
}