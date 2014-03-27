using System;
using System.Collections.Generic;
using CarbonKnown.DAL.Models;

namespace CarbonKnown.MVC.Models
{
    public class OverviewReportModel
    {
        public IEnumerable<Census> CensusItems { get; set; }
        public int CensusId { get; set; }
        public string[] CostCodes { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CompanyName { get; set; }
        public int EmployeesCovered { get; set; }
        public int TotalEmployees { get; set; }
        public decimal EmployeePerc { get; set; }
        public decimal SquareMeters { get; set; }
        public string ScopeBoundries { get; set; }
    }
}