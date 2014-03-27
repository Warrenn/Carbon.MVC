namespace CarbonKnown.DAL.Models
{
    public enum SourceStatus
    {
        PendingExtraction = 1,
        Extracting = 2,
        PendingCalculation = 3,
        Calculating = 4,
        Calculated = 5,
    }
}