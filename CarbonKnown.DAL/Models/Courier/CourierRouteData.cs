namespace CarbonKnown.DAL.Models.Courier
{
    public class CourierRouteData : DataEntry
    {
        public ServiceType? ServiceType { get; set; }
        public decimal? ChargeMass { get; set; }
        public string FromCode { get; set; }
        public string ToCode { get; set; }
        public bool Reversal { get; set; }
    }
}
