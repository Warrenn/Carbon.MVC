using System;
using System.ServiceModel;

namespace CarbonKnown.WCF.DataSource
{
    [ServiceContract]
    public interface IDataSourceService : IDisposable
    {
        [OperationContract]
        SourceResultDataContract CalculateEmissions(Guid sourceId);

        [OperationContract]
        SourceResultDataContract RevertCalculation(Guid sourceId);

        [OperationContract]
        SourceResultDataContract InsertManualDataSource(ManualDataContract source);

        [OperationContract]
        void ReportSourceError(Guid sourceId, SourceErrorType errorType, string errorMessage);

        [OperationContract]
        SourceResultDataContract ExtractCompleted(Guid sourceId);

        [OperationContract]
        bool ContainsErrors(Guid sourceId);
    }
}