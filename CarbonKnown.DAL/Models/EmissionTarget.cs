using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarbonKnown.DAL.Models
{
    public class EmissionTarget
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime InitialDate { get; set; }
        public decimal InitialAmount { get; set; }
        public DateTime TargetDate { get; set; }
        public decimal TargetAmount { get; set; }
        public TargetType TargetType { get; set; }
        public virtual ActivityGroup ActivityGroup { get; set; }
        public Guid ActivityGroupId { get; set; }
        public virtual CostCentre CostCentre { get; set; }
        public string CostCentreCostCode { get; set; }
    }
}
