using System;
using System.Collections.Generic;

namespace CarbonKnown.MVC.BLL
{
    public class DashboardConfiguration
    {
        public string DisplayName { get; set; }
        public IEnumerable<Guid> ActivityIds { get; set; }
        public bool ShowCo2 { get; set; }
    }
}