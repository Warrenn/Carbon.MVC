namespace CarbonKnown.WCF.DataEntry
{
    public enum DataErrorType
    {
        DuplicateEntry = 1,
        BelowVarianceMinimum = 2,
        AboveVarianceMaximum = 3,
        MissingValue = 4,
        StartDateGreaterThanEndDate = 5,
        InvalidCostCode = 6,
        SourceNotFound = 7,
        InvalidState = 8,
        CalculationNotFound = 9,
        EmissionFactorNotFound = 10,
        CalculationError = 11
    }
}
