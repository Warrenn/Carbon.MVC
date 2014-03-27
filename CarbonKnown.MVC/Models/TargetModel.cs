using System;

namespace CarbonKnown.MVC.Models
{
// ReSharper disable InconsistentNaming
    public class TargetModel
    {
        public int id { get; set; }
        public DateTime initialDate { get; set; }
        public DateTime targetDate { get; set; }
        public decimal initialAmount { get; set; }
        public decimal targetAmount { get; set; }
        public string targetType { get; set; }
        public string activityGroupName { get; set; }
        public Guid activityGroupId { get; set; }
        public string costCode { get; set; }
        public string costCentreName { get; set; }
    }
// ReSharper restore InconsistentNaming
}