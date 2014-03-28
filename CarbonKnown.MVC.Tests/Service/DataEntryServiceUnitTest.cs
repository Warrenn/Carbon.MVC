using System;
using System.Collections.ObjectModel;
using System.Linq;
using CarbonKnown.Calculation;
using CarbonKnown.Calculation.DAL;
using CarbonKnown.Calculation.Properties;
using CarbonKnown.DAL.Models;
using CarbonKnown.MVC.DAL;
using CarbonKnown.MVC.Service;
using CarbonKnown.WCF.DataEntry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using DataSource = CarbonKnown.DAL.Models.DataSource;

namespace CarbonKnown.MVC.Tests.Service
{
    [TestClass]
    public class DataEntryServiceUnitTest
    {
        private Guid newSourceId;
        private Guid newCalculationId;
        private Guid newEntryId;
        private Mock<ICalculationDataContext> mockCalcDataContext;
        private Mock<ISourceDataContext> mockContext;
        private Mock<CalculationBase<DataEntryTest>> mockCalculation;
        private Mock<DataEntryServiceBase<DataEntryTest,DataEntryDataContract>> mockService;
        private Mock<ICalculationFactory> mockCalculationFactory;

        [TestInitialize]
        public void Initialize()
        {
            newSourceId = Guid.NewGuid();
            newEntryId = Guid.NewGuid();
            newCalculationId = Guid.NewGuid();

            mockCalcDataContext = new Mock<ICalculationDataContext>();
            mockCalcDataContext
                .Setup(context => context.CostCodeValid(It.IsAny<string>()))
                .Returns(true);
            mockCalculation = new Mock<CalculationBase<DataEntryTest>>(mockCalcDataContext.Object) {CallBase = true};
            mockCalculationFactory = new Mock<ICalculationFactory>();
            mockCalculationFactory
                .Setup(factory => factory.ResolveCalculation(It.IsAny<Guid>()))
                .Returns(mockCalculation.Object);
            mockContext = new Mock<ISourceDataContext>();
            mockContext
                .Setup(context => context.GetDataSource<DataSource>(It.IsAny<Guid>()))
                .Returns(new DataSource
                    {
                        Id = newSourceId,
                        UserName = "SourceUserName",
                        InputStatus = SourceStatus.PendingCalculation
                    });
            mockContext
                .Setup(context => context.AddDataEntry(It.IsAny<DataEntryTest>()))
                .Returns((DataEntryTest entry) => entry);
            mockContext
                .Setup(context => context.UpdateDataEntry(It.IsAny<DataEntryTest>()))
                .Returns((DataEntryTest entry) => entry);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockCalculation = null;
            mockService = null;
        }

        private DataEntryDataContract GetDataEntryDataContractWithErrors()
        {
            var dataContract = new DataEntryDataContract
                {
                    CostCode = null,
                    EndDate = null,
                    Money = null,
                    StartDate = null,
                    SourceId = newSourceId,
                    EntryId = newEntryId
                };
            return dataContract;
        }

        private DataEntryDataContract GetDataEntryDataContractWithoutErrors()
        {
            var dataContract = new DataEntryDataContract
                {
                    CostCode = "CostCode",
                    Money = 123.9M,
                    StartDate = DateTime.Now,
                    SourceId = newSourceId,
                    RowNo = 1,
                    Units = 123.0M,
                    EntryId = newEntryId
                };
            dataContract.EndDate = dataContract.StartDate;
            return dataContract;
        }

        private DataEntryServiceBase<DataEntryTest, DataEntryDataContract> CreateService()
        {
            mockService = new
                Mock<DataEntryServiceBase<DataEntryTest, DataEntryDataContract>>
                (mockContext.Object, mockCalculationFactory.Object)
                {
                    CallBase = true
                };
            mockService
                .Setup(@base => @base.GetCalculationId())
                .Returns(newCalculationId);
            return mockService.Object;
        }

        [TestMethod]
        public void UpsertDataEntryMustNotGetEntryIfSourceIsNotFound()
        {
            //Arrange
            var entry = GetDataEntryDataContractWithoutErrors();
            mockContext
                .Setup(context => context.GetDataSource<DataSource>(It.Is<Guid>(guid => guid == newSourceId)))
                .Returns((DataSource) null)
                .Verifiable();

            //Act
            var result = CreateService().UpsertDataEntry(entry);

            //Assert
            mockContext.Verify();
            mockContext
                .Verify(context => context.GetDataEntry<DataEntryTest>(It.IsAny<Guid>()), Times.Never);
            Assert.AreEqual(CarbonKnown.WCF.DataEntry.DataErrorType.SourceNotFound, result.Errors.First().ErrorType);
            Assert.AreEqual(DataSourceServiceResources.SourceNotFound, result.Errors.First().ErrorMessage);
        }

        [TestMethod]
        public void UpsertDataEntryMustNotGetEntryIfSourceNotExtractingOrPendingCalculation()
        {
            //Arrange
            var entry = GetDataEntryDataContractWithoutErrors();
            mockContext
                .Setup(context => context.GetDataSource<DataSource>(It.Is<Guid>(guid => guid == newSourceId)))
                .Returns(new DataSource{InputStatus = SourceStatus.Calculated})
                .Verifiable();

            //Act
            var result = CreateService().UpsertDataEntry(entry);

            //Assert
            mockContext.Verify();
            mockContext
                .Verify(context => context.GetDataEntry<DataEntryTest>(It.IsAny<Guid>()), Times.Never);
            Assert.AreEqual(CarbonKnown.WCF.DataEntry.DataErrorType.InvalidState, result.Errors.First().ErrorType);
            Assert.AreEqual(DataSourceServiceResources.FileSourceIsNotPendingCalculation, result.Errors.First().ErrorMessage);
        }

        [TestMethod]
        public void UpsertDataEntryMustGetCalculationIdFromBase()
        {
            //Arrange
            var entry = GetDataEntryDataContractWithoutErrors();

            //Act
            CreateService().UpsertDataEntry(entry);


            //Assert
            mockService
                .Verify(@base => @base.GetCalculationId(), Times.Once);
        }

        [TestMethod]
        public void UpsertDataEntryMustReSetTheDefaultValues()
        {
            //Arrange
            var entry = GetDataEntryDataContractWithoutErrors();
            CreateService();
            mockService
                .Setup(@base => @base.SetEntryValues(
                    It.IsAny<DataEntryTest>(),
                    It.IsAny<DataEntryDataContract>()))
                .Callback(
                    (DataEntryTest det, DataEntryDataContract dec) =>
                        {
                            det.Id = Guid.NewGuid();
                            det.SourceId = Guid.NewGuid();
                            det.CalculationId = Guid.NewGuid();
                            det.EditDate = DateTime.Today.AddDays(10);
                            det.Errors = new Collection<DataError>(new[] {new DataError()});
                        })
                .Verifiable();

            //Act
            mockService.Object.UpsertDataEntry(entry);

            //Assert
            mockService.Verify();
            mockContext
                .Verify(context => context.AddDataEntry(It.Is<DataEntryTest>(
                    test =>
                    (test.CalculationId == newCalculationId) &&
                    (test.Id == newEntryId) &&
                    (test.SourceId == newSourceId) &&
                    (test.EditDate.Date == DateTime.Today) &&
                    (test.Errors.Count == 0))), Times.Once);
        }

        [TestMethod]
        public void UpsertDataEntryMustAddIfEntryNotFound()
        {
            //Arrange
            var entry = GetDataEntryDataContractWithoutErrors();
            CreateService();
            mockContext
                .Setup(context => context.GetDataEntry<DataEntryTest>(It.Is<Guid>(
                    guid => guid == newEntryId)))
                .Returns((DataEntryTest) null);

            //Act
            mockService.Object.UpsertDataEntry(entry);

            //Assert
            mockService.Verify();
            mockContext
                .Verify(context => context.AddDataEntry(It.Is<DataEntryTest>(
                    test =>
                    (test.CalculationId == newCalculationId) &&
                    (test.Id == newEntryId) &&
                    (test.SourceId == newSourceId) &&
                    (test.EditDate.Date == DateTime.Today) &&
                    (test.Errors.Count == 0))), Times.Once);
        }

        [TestMethod]
        public void UpsertDataEntryUpdateIfEntryFound()
        {
            //Arrange
            var entry = GetDataEntryDataContractWithoutErrors();
            CreateService();
            mockContext
                .Setup(context => context.GetDataEntry<DataEntryTest>(It.Is<Guid>(
                    guid => guid == newEntryId)))
                .Returns(new DataEntryTest());

            //Act
            mockService.Object.UpsertDataEntry(entry);

            //Assert
            mockService.Verify();
            mockContext
                .Verify(context => context.UpdateDataEntry(It.Is<DataEntryTest>(
                    test =>
                    (test.CalculationId == newCalculationId) &&
                    (test.Id == newEntryId) &&
                    (test.SourceId == newSourceId) &&
                    (test.EditDate.Date == DateTime.Today) &&
                    (test.Errors.Count == 0))), Times.Once);
        }

        [TestMethod]
        public void UpsertDataEntryMustRemovePreviousErrorsIfUpdating()
        {
            //Arrange
            var entry = GetDataEntryDataContractWithoutErrors();
            CreateService();
            mockContext
                .Setup(context => context.GetDataEntry<DataEntryTest>(It.Is<Guid>(
                    guid => guid == newEntryId)))
                .Returns(new DataEntryTest());

            //Act
            mockService.Object.UpsertDataEntry(entry);

            //Assert
            mockService.Verify();
            mockContext
                .Verify(context => context.RemoveDataErrors(It.Is<Guid>(
                    entryId =>entryId == newEntryId)), Times.Once);
        }

        [TestMethod]
        public void UpsertDataEntryMustReturnErrorIfCalculationIsNotFound()
        {
            //Arrange
            var entry = GetDataEntryDataContractWithoutErrors();
            mockCalculationFactory
                .Setup(factory => factory.ResolveCalculation(It.IsAny<Guid>()))
                .Returns((ICalculation) null);

            //Act
            var result = CreateService().UpsertDataEntry(entry);

            //Assert
            mockContext
                .Verify(context => context.AddDataError(It.Is<DataError>(
                    error =>
                    (error.DataEntryId == newEntryId) &&
                    (error.ErrorType == CarbonKnown.DAL.Models.DataErrorType.CalculationNotFound) &&
                    (error.Message == DataSourceServiceResources.CalculationNotFound))));
            Assert.AreEqual(CarbonKnown.WCF.DataEntry.DataErrorType.CalculationNotFound, result.Errors.First().ErrorType);
            Assert.AreEqual(DataSourceServiceResources.CalculationNotFound, result.Errors.First().ErrorMessage);
            Assert.AreEqual(newEntryId, result.EntryId);
        }

        [TestMethod]
        public void UpsertDataEntryMustPersistEachValidationError()
        {
            //Arrange
            var contractWithError = GetDataEntryDataContractWithErrors();

            //Act
            CreateService().UpsertDataEntry(contractWithError);

            //Assert
            mockContext
                .Verify(context =>
                        context.AddDataError(
                            It.Is<DataError>(
                                error => (
                                             (error.Column == "Money") &&
                                             (error.ErrorType == CarbonKnown.DAL.Models.DataErrorType.MissingValue) &&
                                             (error.Message == string.Format(Resources.MissingValueMessage, "Money")) &&
                                             (error.DataEntryId == newEntryId))
                                         )), Times.Once);
            mockContext
                .Verify(context =>
                        context.AddDataError(
                            It.Is<DataError>(
                                error => (
                                             (error.Column == "CostCode") &&
                                             (error.ErrorType == CarbonKnown.DAL.Models.DataErrorType.MissingValue) &&
                                             (error.Message == string.Format(Resources.MissingValueMessage, "CostCode")) &&
                                             (error.DataEntryId == newEntryId))
                                         )), Times.Once);
            mockContext
                .Verify(context =>
                        context.AddDataError(
                            It.Is<DataError>(
                                error => (
                                             (error.Column == "EndDate") &&
                                             (error.ErrorType == CarbonKnown.DAL.Models.DataErrorType.MissingValue) &&
                                             (error.Message == string.Format(Resources.MissingValueMessage, "EndDate")) &&
                                             (error.DataEntryId == newEntryId))
                                         )), Times.Once);
            mockContext
                .Verify(context =>
                        context.AddDataError(
                            It.Is<DataError>(
                                error => (
                                             (error.Column == "StartDate") &&
                                             (error.ErrorType == CarbonKnown.DAL.Models.DataErrorType.MissingValue) &&
                                             (error.Message == string.Format(Resources.MissingValueMessage, "StartDate")) &&
                                             (error.DataEntryId == newEntryId))
                                         )), Times.Once);
        }

        [TestMethod]
        public void UpsertDataEntryMustReturnAllValidationErrors()
        {
            //Arrange
            var contractWithError = GetDataEntryDataContractWithErrors();

            //Act
            var result = CreateService().UpsertDataEntry(contractWithError);

            //Assert
            var errors = result.Errors.ToArray();
            Assert.AreEqual(4, errors.Length);
            var actualError = errors[0];
            Assert.AreEqual(CarbonKnown.WCF.DataEntry.DataErrorType.MissingValue, actualError.ErrorType);
            Assert.AreEqual(string.Format(Resources.MissingValueMessage, "StartDate"), actualError.ErrorMessage);
            actualError = errors[1];
            Assert.AreEqual(CarbonKnown.WCF.DataEntry.DataErrorType.MissingValue, actualError.ErrorType);
            Assert.AreEqual(string.Format(Resources.MissingValueMessage, "EndDate"), actualError.ErrorMessage);
            actualError = errors[2];
            Assert.AreEqual(CarbonKnown.WCF.DataEntry.DataErrorType.MissingValue, actualError.ErrorType);
            Assert.AreEqual(string.Format(Resources.MissingValueMessage, "CostCode"), actualError.ErrorMessage);
            actualError = errors[3];
            Assert.AreEqual(CarbonKnown.WCF.DataEntry.DataErrorType.MissingValue, actualError.ErrorType);
            Assert.AreEqual(string.Format(Resources.MissingValueMessage, "Money"), actualError.ErrorMessage);
        }

        public class DataEntryTest : DataEntry
        {
             
        }
    }
}
