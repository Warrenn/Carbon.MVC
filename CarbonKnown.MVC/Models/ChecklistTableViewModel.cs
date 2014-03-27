using System;
using System.Collections.Generic;

namespace CarbonKnown.MVC.Models
{
    public class ChecklistTableViewModel
    {
        public string Heading { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<ChecklistTableRow> Rows { get; set; } 
    }
}