using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace CarbonKnown.Factors.WCF
{
    [ServiceContract]
    public interface IFactorsService
    {
        [Caching]
        [OperationContract]
        IEnumerable<FactorValues> FactorValuesById(Guid factorId);

        [Caching]
        [OperationContract]
        IEnumerable<FactorValues> FactorValuesByName(string factorName);

        [Caching]
        [OperationContract]
        IEnumerable<RouteDistance> AirRouteDistances();

        [Caching]
        [OperationContract]
        IEnumerable<RouteDistance> CourierRouteDistances();
    }
}
