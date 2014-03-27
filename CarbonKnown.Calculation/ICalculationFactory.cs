using System;

namespace CarbonKnown.Calculation
{
    public interface ICalculationFactory
    {
        ICalculation ResolveCalculation(Guid calculationId);
    }
}