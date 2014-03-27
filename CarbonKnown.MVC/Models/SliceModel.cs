using System;

namespace CarbonKnown.MVC.Models
{
    // ReSharper disable InconsistentNaming
    public class SliceModel
    {
        public string costCode { get; set; }
        public Guid? activityGroupId { get; set; }
        public string color { get; set; }
        public string title { get; set; }
        public string sliceId { get; set; }
        public decimal amount { get; set; }
        public string co2label { get; set; }
        public string description { get; set; }
        public decimal units { get; set; }
        public string uom { get; set; }
    }

    // ReSharper restore InconsistentNaming
}