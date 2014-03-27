namespace CarbonKnown.DAL.Models
{
    public class CurrencySummary
    {
        public string Code { get; set; }
        public string Locale { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public decimal TotalMoney { get; set; }
    }
}
