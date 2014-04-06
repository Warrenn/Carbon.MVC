using System;
using System.Data.Entity.Hierarchy;

namespace CarbonKnown.MVC.Models
{
    public class SliceDataModel
    {
        public string CostCode { get; set; }
        public Guid? ActivityGroupId { get; set; }
        public HierarchyId ActivityGroupNode { get; set; }
        public HierarchyId CentreNode { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string UomLong { get; set; }
        public string UomShort { get; set; }
        public int OrderId { get; set; }
        public string Color { get; set; }
    }
}