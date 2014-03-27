using System;

namespace CarbonKnown.Calculation.Models
{
    public class CalculationResult
    {
        public DateTime? CalculationDate { get; set; }
        public Guid ActivityGroupId { get; set; }
        public decimal? Emissions { get; set; }
    }
}
