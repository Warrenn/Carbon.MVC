using System.Collections.Generic;

namespace CarbonKnown.MVC.Models
{
    // ReSharper disable InconsistentNaming
    public class DashboardSummary
    {
        public decimal total { get; set; }
        public decimal yoy { get; set; }
        public bool displayTotal { get; set; }
        public string costCentre { get; set; }
        public string activityGroup { get; set; }
        public string co2label { get; set; }
        public IEnumerable<string> currencies { get; set; }
        public IEnumerable<SliceModel> slices { get; set; } 
    }

    // ReSharper restore InconsistentNaming
}
