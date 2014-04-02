using System.Collections.Generic;

namespace CarbonKnown.MVC.Models
{
// ReSharper disable InconsistentNaming
    public class CostCentreModel
    {
        public string costCode { get; set; }
        public string name { get; set; }
        public string color { get; set; }
        public Select2Model currencyCode { get; set; }
        public IEnumerable<string> consumptionTypes { get; set; }
        public string description { get; set; }
        public int orderId { get; set; }
        public string parentCostCode { get; set; }
        public string node { get; set; }
    }
// ReSharper restore InconsistentNaming
}