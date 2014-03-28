using System;
using System.Runtime.ExceptionServices;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CarbonKnown.Calculation;
using CarbonKnown.Calculation.Properties;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Source;
using CarbonKnown.MVC.App_Start;
using CarbonKnown.MVC.Code;
using CarbonKnown.MVC.Constants;
using CarbonKnown.MVC.DAL;
using CarbonKnown.WCF.DataSource;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.Unity;

namespace CarbonKnown.MVC.Service
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class DataSourceService : DataSourceServiceBase, IDataSourceService
    {
        private readonly IEmailManager emailManager;

        public DataSourceService(
            ISourceDataContext context,
            IEmailManager emailManager)
            : base(context)
        {
            this.emailManager = emailManager;
        }

        public virtual IUnityContainer Container
        {
            get { return Bootstrapper.Container; }
        }

        public virtual SourceResultDataContract CalculateEmissions(Guid sourceId)
        {
            var source = Context.GetDataSource<DataSource>(sourceId);
            if ((source == null) || (source.InputStatus != SourceStatus.PendingCalculation))
            {
                return DataContractError(sourceId, DataSourceServiceResources.FileSourceIsNotPendingCalculation);
            }
            if (Context.SourceContainsErrors(sourceId))
            {
                return DataContractError(sourceId, DataSourceServiceResources.SourceContainsError);
            }
            if (Context.SourceContainsDataEntriesInError(sourceId))
            {
                return DataContractError(sourceId, DataSourceServiceResources.SourceContainsDataEntriesInError);
            }
            var returnResult = DataContractSuccess(sourceId);
            try
            {
                Context.RemoveSourceCalculations(sourceId);
                source.InputStatus = SourceStatus.Calculating;
                Context.UpdateDataSource(source);
                using (var unitOfWork = Context.CreateUnitOfWork())
                {
                    var dataEntries = unitOfWork.GetDataEntriesForSource(sourceId);

                    foreach (var entry in dataEntries)
                    {
                        try
                        {
                            var calculation = Container.Resolve<ICalculation>(entry.CalculationId.ToString());
                            if (calculation == null)
                            {
                                var errorMessage = DataSourceServiceResources.CalculationNotFound;
                                errorMessage = string.Format(errorMessage, entry.CalculationId);
                                throw new NullReferenceException(errorMessage);
                            }
                            var totalDays = calculation.GetDayDifference(entry);
                            if ((entry.StartDate == null) || (entry.EndDate == null))
                            {
                                var errorMessage = DataSourceServiceResources.StartDateEndDateNull;
                                errorMessage = string.Format(errorMessage, entry.Id);
                                throw new NullReferenceException(errorMessage);
                            }

                            var startDate = entry.StartDate.Value;
                            var dailyValue = calculation.CalculateDailyData(entry);
                            for (var day = 0; day <= totalDays; day++)
                            {
                                var effectiveDate = startDate.AddDays(day);
                                var calculatedValue = calculation.CalculateEmission(effectiveDate, dailyValue, entry);
                                var emissionEntry = new CarbonEmissionEntry
                                    {
                                        SourceEntry = entry,
                                        EntryDate = effectiveDate,
                                        CarbonEmissions = calculatedValue.Emissions ?? 0,
                                        Money = dailyValue.MoneyPerDay ?? 0,
                                        Units = dailyValue.UnitsPerDay ?? 0,
                                        CalculationDate = calculatedValue.CalculationDate ?? DateTime.Today
                                    };
                                unitOfWork.AddCarbonEmissionEntry(emissionEntry);
                            }
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                var correlationId = Guid.NewGuid();
                                ex.Data["CorrelationId"] = correlationId;
                                var entryError = new DataError
                                    {
                                        Column = string.Empty,
                                        DataEntryId = entry.Id,
                                        ErrorType = DataErrorType.CalculationError,
                                        Message =
                                            string.Format(Resources.CalculationError,
                                                          correlationId)
                                    };
                                Context.AddDataError(entryError);
                            }
                            catch (Exception dbEx)
                            {
                                HandleException(dbEx);
                            }
                            ExceptionDispatchInfo.Capture(ex).Throw();
                        }
                    }

                    unitOfWork.CommitWork();
                }
                source.InputStatus = SourceStatus.Calculated;
                Context.UpdateDataSource(source);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                returnResult.Succeeded = false;
                returnResult.ErrorMessages.Add(DataSourceServiceResources.ExceptionOccuredDuringCalculation);
                try
                {
                    source.InputStatus = SourceStatus.PendingCalculation;
                    Context.UpdateDataSource(source);
                }
                catch (Exception dbEx)
                {
                    HandleException(dbEx);
                }
            }
            var profile = Context.GetUserProfile(source.UserName);
            if (profile == null)
            {
                returnResult.ErrorMessages.Add(
                    string.Format(DataSourceServiceResources.UserNameNotFound, source.UserName));
            }
            else
            {
                emailManager.SendMail(source, EmailTemplate.CalculationComplete, profile.Email);
            }
            return returnResult;
        }

        public virtual void HandleException(Exception ex)
        {
            ExceptionPolicy.HandleException(ex, Policy.DataEntry);
        }

        public virtual SourceResultDataContract RevertCalculation(Guid sourceId)
        {
            var fileSource = Context.GetDataSource<DataSource>(sourceId);
            if ((fileSource == null) || (fileSource.InputStatus != SourceStatus.Calculated))
            {
                return DataContractError(sourceId, DataSourceServiceResources.FileSourceIsNotCalculated);
            }
            Context.RemoveSourceCalculations(sourceId);
            fileSource.InputStatus = SourceStatus.PendingCalculation;
            Context.UpdateDataSource(fileSource);
            return DataContractSuccess(sourceId);
        }

        public virtual SourceResultDataContract InsertManualDataSource(ManualDataContract source)
        {
            var manualData = new ManualDataSource
                {
                    DateEdit = DateTime.Now,
                    InputStatus = SourceStatus.PendingCalculation,
                    ReferenceNotes = source.ReferenceNotes,
                    UserName = source.UserName,
                    DisplayType = source.DisplayType
                };

            manualData = Context.AddDataSource(manualData);
            return DataContractSuccess(manualData.Id);
        }

        public virtual SourceResultDataContract ExtractCompleted(Guid sourceId)
        {
            var source = Context.GetDataSource<FileDataSource>(sourceId);
            if ((source == null) || (source.InputStatus != SourceStatus.Extracting))
            {
                return DataContractError(sourceId, DataSourceServiceResources.FileSourceIsNotExtracting);
            }
            source.InputStatus = SourceStatus.PendingCalculation;
            Context.UpdateDataSource(source);
            var profile = Context.GetUserProfile(source.UserName);
            if (profile == null)
                return DataContractError(sourceId, DataSourceServiceResources.UserNameNotFound, source.UserName);
            emailManager.SendMail(source, EmailTemplate.ExtractComplete, profile.Email);
            return DataContractSuccess(sourceId);
        }

        public virtual bool ContainsErrors(Guid sourceId)
        {
            return Context.SourceContainsErrors(sourceId);
        }
    }
}
