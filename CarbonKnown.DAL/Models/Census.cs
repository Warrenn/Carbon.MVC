using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarbonKnown.DAL.Models
{
    public class Census
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string DisplayName { get; set; }
        public string CompanyName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int EmployeesCovered { get; set; }
        public int TotalEmployees { get; set; }
        public decimal SquareMeters { get; set; }
        public string ScopeBoundries { get; set; }
        public virtual ICollection<CostCentre> CostCentres { get; set; }
    }
}
