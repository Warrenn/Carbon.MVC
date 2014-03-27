namespace CarbonKnown.MVC.Models
{
// ReSharper disable InconsistentNaming
    public class VarianceModel
    {
        public int id { get; set; }
        public Select2Model calculation { get; set; }
        public string columnName { get; set; }
        public decimal maxValue { get; set; }
        public decimal minValue { get; set; }
    }
// ReSharper restore InconsistentNaming
}