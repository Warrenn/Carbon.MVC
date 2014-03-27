using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Hierarchy;

namespace CarbonKnown.DAL.Models
{
    public class CarbonEmissionEntry
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime EntryDate { get; set; }

        public HierarchyId ActivityGroupNode { get; set; }

        public HierarchyId CostCentreNode { get; set; }

        public Guid DataEntryId { get; set; }

        public virtual DataEntry SourceEntry { get; set; }
        public DateTime CalculationDate { get; set; }
        public decimal Units { get; set; }
        public decimal Money { get; set; }
        public decimal CarbonEmissions { get; set; }
    }
}
