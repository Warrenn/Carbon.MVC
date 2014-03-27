using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Hierarchy;

namespace CarbonKnown.DAL.Models
{
    public class ActivityGroup
    {
        public Guid Id { get; set; }

        public HierarchyId Node { get; set; }

        [StringLength(6)]
        public string Color { get; set; }

        public int OrderId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GRIDescription { get; set; }
        public string UOMShort { get; set; }
        public string UOMLong { get; set; }
        public Guid? ParentGroupId { get; set; }
        public virtual ActivityGroup ParentGroup { get; set; }
        public virtual ICollection<ActivityGroup> ChildGroups { get; set; }
        public virtual ICollection<Calculation> Calculations { get; set; }
    }
}
