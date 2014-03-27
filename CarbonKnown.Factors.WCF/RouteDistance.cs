using System;

namespace CarbonKnown.Factors.WCF
{
    public class RouteDistance
    {
        public string Code1 { get; set; }
        public string Code2 { get; set; }
        public DateTime? CalculationDate { get; set; }
        public decimal? Distance { get; set; }
    }
}
