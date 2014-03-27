using System;
using System.Collections.Generic;

namespace CarbonKnown.MVC.Models
{
    public class ChecklistTableSettings
    {
        public string CostCode { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string Heading { get; set; }
        public IDictionary<Guid, string> ActivityColumns { get; set; } 
    }
}