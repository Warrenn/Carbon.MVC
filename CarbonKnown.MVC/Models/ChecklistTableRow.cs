using System.Collections.Generic;

namespace CarbonKnown.MVC.Models
{
    public class ChecklistTableRow
    {
        public string CostCode { get; set; }
        public string Heading { get; set; }
        public IEnumerable<ChecklistTableColumn> Columns { get; set; } 
    }
}