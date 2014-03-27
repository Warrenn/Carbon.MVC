using System;

namespace CarbonKnown.MVC.Models
{
    public class AuditHistoryModel
    {
        public Guid? ActivityGroupId { get; set; }

        public string CostCode { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}