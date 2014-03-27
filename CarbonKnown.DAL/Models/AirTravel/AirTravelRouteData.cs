namespace CarbonKnown.DAL.Models.AirTravel
{
    public class AirTravelRouteData : DataEntry
    {
        public TravelClass TravelClass { get; set; }
        public bool Reversal { get; set; }
        public string FromCode { get; set; }
        public string ToCode { get; set; }
    }
}
