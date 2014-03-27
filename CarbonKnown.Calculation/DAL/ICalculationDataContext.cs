using System;
using System.Collections.Generic;
using CarbonKnown.DAL.Models;

namespace CarbonKnown.Calculation.DAL
{
    public interface ICalculationDataContext
    {
        decimal? CourierRouteDistance(string code1, string code2);
 
        decimal? AirRouteDistance(string code1, string code2);

        decimal? FactorValue(DateTime effectiveDate, Guid factorId);

        decimal? FactorValue(DateTime effectiveDate, string factorName);

        IEnumerable<string> AirRouteCodes(string code);

        IEnumerable<string> CourierRouteCodes(string code);

        DateTime? CalculationDateForFactorName(string factorName);

        DateTime? CalculationDateForFactorId(Guid factorId);

        bool CostCodeValid(string costCode);

        Variance Variance(Guid calculationId, string column);

        bool EntryIsDuplicate(Guid entryId, int hash);
    }
}
