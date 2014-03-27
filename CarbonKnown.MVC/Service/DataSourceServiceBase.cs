using System;
using System.Collections.ObjectModel;
using CarbonKnown.DAL.Models;
using CarbonKnown.MVC.DAL;
using CarbonKnown.WCF.DataSource;
using SourceErrorType = CarbonKnown.WCF.DataSource.SourceErrorType;
using SourceErrorTypeDAL = CarbonKnown.DAL.Models.SourceErrorType;

namespace CarbonKnown.MVC.Service
{
    public class DataSourceServiceBase : ServiceBase
    {
        public DataSourceServiceBase(ISourceDataContext context)
            :base(context)
        {
        }

        public virtual void ReportSourceError(Guid sourceId, SourceErrorType errorType, string errorMessage)
        {
            var error = new SourceError
                {
                    DataSourceId = sourceId,
                    ErrorMessage = errorMessage,
                    ErrorType = (SourceErrorTypeDAL)errorType
                };
            Context.AddSourceError(error);
        }

        public virtual SourceResultDataContract DataContractSuccess(Guid sourceId)
        {
            return new SourceResultDataContract
                {
                    SourceId = sourceId,
                    Succeeded = true,
                    ErrorMessages = new Collection<string>()
                };
        }

        public virtual SourceResultDataContract DataContractError(
            Guid sourceId,
            string errorMessage,
            params object[] args)
        {
            var message = string.Format(errorMessage, args);
            return new SourceResultDataContract
                {
                    Succeeded = false,
                    SourceId = sourceId,
                    ErrorMessages = new Collection<string> {message}
                };
        }
    }
}