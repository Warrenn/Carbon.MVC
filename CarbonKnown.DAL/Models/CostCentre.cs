using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Hierarchy;

namespace CarbonKnown.DAL.Models
{
    public class CostCentre
    {
        [Key]
        [Column(Order = 1)]
        public string CostCode { get; set; }
        
        public HierarchyId Node { get; set; }

        [StringLength(6)]
        public string Color { get; set; }

        public virtual Currency Currency { get; set; }
        public string CurrencyCode { get; set; }
        public int OrderId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ConsumptionType? ConsumptionType { get; set; }
        public virtual CostCentre ParentCostCentre { get; set; }
        public string ParentCostCentreCostCode { get; set; }
        public virtual ICollection<CostCentre> ChildrenCostCentres { get; set; }
        public virtual ICollection<Census> Census { get; set; }
    }
}
