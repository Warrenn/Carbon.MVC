using System;
using System.Collections.Generic;
using CarbonKnown.Calculation.Models;
using CarbonKnown.DAL.Models;

namespace CarbonKnown.Calculation
{
    public interface ICalculation
    {
        CalculationResult CalculateEmission(DateTime effectiveDate, DailyData dailyData, DataEntry entry);
        IEnumerable<DataError> ValidateEntry(DataEntry entry);
        int GetDayDifference(DataEntry entry);
        DailyData CalculateDailyData(DataEntry entry);
    }
}