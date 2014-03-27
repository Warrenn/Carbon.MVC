using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarbonKnown.DAL.Models
{
    public class GraphSeries
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public virtual ActivityGroup ActivityGroup { get; set; }
        public Guid ActivityGroupId { get; set; }

        public virtual CostCentre CostCentre { get; set; }
        public string CostCentreCostCode { get; set; }

        public string Name { get; set; }
        public TargetType TargetType { get; set; }
        public bool IncludeTarget { get; set; }
    }
}
