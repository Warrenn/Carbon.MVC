using System;
using System.Collections.ObjectModel;
using CarbonKnown.Calculation;
using CarbonKnown.DAL.Models;
using CarbonKnown.MVC.DAL;
using CarbonKnown.WCF.DataEntry;
using DataErrorType = CarbonKnown.WCF.DataEntry.DataErrorType;

namespace CarbonKnown.MVC.Service
{
    public abstract class DataEntryServiceBase<TEntry, TContract> : ServiceBase
        where TEntry : DataEntry, new()
        where TContract : DataEntryDataContract
    {
        private readonly ICalculationFactory calculationFactory;

        protected DataEntryServiceBase(
            ISourceDataContext context,
            ICalculationFactory calculationFactory)
            : base(context)
        {
            this.calculationFactory = calculationFactory;
        }

        public abstract Guid GetCalculationId();

        public virtual void SetEntryValues(TEntry instance, TContract dataEntry)
        {
            instance.CostCode = dataEntry.CostCode;
            instance.StartDate = dataEntry.StartDate;
            instance.EndDate = dataEntry.EndDate;
            instance.Money = dataEntry.Money;
            instance.Units = dataEntry.Units;
            instance.RowNo = dataEntry.RowNo;
        }

        protected static DataEntryUpsertResultDataContract
            DataEntryError(Guid entryId, DataErrorType errorType, string errorMessage)
        {
            return new DataEntryUpsertResultDataContract
                {
                    EntryId = entryId,
                    Errors = new[]
                        {
                            new DataEntryErrorDataContract
                                {
                                    ErrorType = errorType,
                                    ErrorMessage = errorMessage
                                }
                        }
                };
        }

        public virtual DataEntryUpsertResultDataContract UpsertDataEntry(TContract dataEntry)
        {
            var sourceId = dataEntry.SourceId;
            var source = Context.GetDataSource<DataSource>(sourceId);
            var entryId = dataEntry.EntryId ?? Guid.NewGuid();
            var username = dataEntry.UserName;
            if (string.IsNullOrEmpty(username))
            {
                username = source.UserName;
            }
            if ((source == null))
            {
                return DataEntryError(
                    entryId,
                    DataErrorType.SourceNotFound,
                    DataSourceServiceResources.SourceNotFound);
            }
            if ((source.InputStatus != SourceStatus.Extracting) &&
                (source.InputStatus != SourceStatus.PendingCalculation))
            {
                return DataEntryError(
                    entryId,
                    DataErrorType.InvalidState,
                    DataSourceServiceResources.FileSourceIsNotPendingCalculation);
            }

            var instance = Context.GetDataEntry<TEntry>(entryId);

            Action<TEntry> setDefaults = entry =>
                {
                    entry.Id = entryId;
                    entry.SourceId = sourceId;
                    entry.EditDate = DateTime.Now;
                    entry.CalculationId = GetCalculationId();
                    entry.UserName = username;
                    entry.Errors = new Collection<DataError>();
                };

            if (instance == null)
            {
                instance = new TEntry();
                SetEntryValues(instance, dataEntry);
                setDefaults(instance);
                instance = Context.AddDataEntry(instance);
            }
            else
            {
                Context.RemoveDataErrors(entryId);
                SetEntryValues(instance, dataEntry);
                setDefaults(instance);
                instance = Context.UpdateDataEntry(instance);
            }

            var calculation = calculationFactory.ResolveCalculation(instance.CalculationId);
            if (calculation == null)
            {
                var error = new DataError
                    {
                        DataEntryId = entryId,
                        ErrorType = CarbonKnown.DAL.Models.DataErrorType.CalculationNotFound,
                        Message = DataSourceServiceResources.CalculationNotFound
                    };
                Context.AddDataError(error);
                return DataEntryError(
                    entryId,
                    DataErrorType.CalculationNotFound,
                    DataSourceServiceResources.CalculationNotFound);
            }

            var returnValue = new DataEntryUpsertResultDataContract { EntryId = entryId, Succeeded = true };
            foreach (var error in calculation.ValidateEntry(instance))
            {
                error.DataEntryId = entryId;
                returnValue.Errors.Add(new DataEntryErrorDataContract
                    {
                        ErrorType = (DataErrorType)error.ErrorType,
                        ErrorMessage = error.Message
                    });
                Context.AddDataError(error);
            }
            returnValue.Succeeded = !Context.EntryContainsErrors(entryId);
            return returnValue;
        }
    }
}
