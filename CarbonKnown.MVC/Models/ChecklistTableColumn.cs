using System;

namespace CarbonKnown.MVC.Models
{
    public class ChecklistTableColumn
    {
        public Guid ActivitiyId { get; set; }
        public string Heading { get; set; }
        public bool ContainsValues { get; set; }
    }
}