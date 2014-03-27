using System;

namespace CarbonKnown.MVC.Models
{
// ReSharper disable InconsistentNaming
    public class CensusModel
    {
        public int id { get; set; }
        public string displayName { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string scopeBoundries { get; set; }
        public int employeesCovered { get; set; }
        public int totalEmployees { get; set; }
        public decimal squareMeters { get; set; }
    }
// ReSharper restore InconsistentNaming
}