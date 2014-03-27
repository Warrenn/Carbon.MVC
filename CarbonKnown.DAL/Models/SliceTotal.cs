using System;

namespace CarbonKnown.DAL.Models
{
    public class SliceTotal
    {
        public decimal TotalUnits { get; set; }
        public decimal TotalCarbonEmissions { get; set; }
        public Guid? ActivityGroupId { get; set; }
        public string CostCode { get; set; }
    }
}
