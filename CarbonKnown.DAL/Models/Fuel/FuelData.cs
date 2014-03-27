namespace CarbonKnown.DAL.Models.Fuel
{
    public class FuelData : DataEntry
    {
        public FuelType? FuelType { get; set; }
        public UnitOfMeasure? UOM { get; set; } 
    }
}
