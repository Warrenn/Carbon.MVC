namespace CarbonKnown.DAL.Models.Courier
{
    public class CourierData:DataEntry
    {
        public ServiceType? ServiceType { get; set; }
        public decimal? ChargeMass { get; set; }
    }
}
