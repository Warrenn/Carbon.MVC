using System;

namespace CarbonKnown.DAL.Models
{
    public class AuditHistory
    {
        public Guid SourceId { get; set; }
        public DateTime DateEdit { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string CurrentFileName { get; set; }
        public decimal Units { get; set; }
        public decimal Emissions { get; set; }
        public decimal Cost { get; set; }
        public string HandlerName { get; set; }
    }
}