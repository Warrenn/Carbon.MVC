namespace CarbonKnown.DAL.Models.Fleet
{
    public class FleetData : DataEntry
    {
        public FleetScope? Scope { get; set; }
        public FuelType? FuelType { get; set; }
    }
}
