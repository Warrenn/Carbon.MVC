using System;
using System.Linq;
using CarbonKnown.Calculation;
using CarbonKnown.Calculation.Models;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Source;
using CarbonKnown.MVC.Code;
using CarbonKnown.MVC.Constants;
using CarbonKnown.MVC.DAL;
using CarbonKnown.MVC.Service;
using CarbonKnown.WCF.DataSource;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CarbonKnown.MVC.Tests.Service
{
    [TestClass]
    public class DataSourceServiceUnitTest
    {
        private Guid newSourceId;
        private Guid newEntryId;
        private Mock<ISourceDataContext> mockContext;
        private Mock<IDataEntriesUnitOfWork> mockUnitOfWork;
        private Mock<IEmailManager> mockEmailManager;
        private Mock<ICalculationFactory> mockCalculationFactory;
        private Mock<ICalculation> mockCalculation;
        private Mock<DataSourceService> mockService;
        private Mock<ExceptionManager> mockExceptionManager;

        [TestInitialize]
        public void Initialize()
        {
            newSourceId = Guid.NewGuid();
            newEntryId = Guid.NewGuid();
            mockUnitOfWork = new Mock<IDataEntriesUnitOfWork>();
            mockEmailManager = new Mock<IEmailManager>();
            mockCalculationFactory = new Mock<ICalculationFactory>();
            mockExceptionManager = new Mock<ExceptionManager>();
            mockContext = new Mock<ISourceDataContext>();
            mockContext
                .Setup(context => context.CreateUnitOfWork())
                .Returns(mockUnitOfWork.Object);
            mockCalculation = new Mock<ICalculation>();
            mockCalculationFactory
                .Setup(factory => factory.ResolveCalculation(It.IsAny<Guid>()))
                .Returns(mockCalculation.Object);
            ExceptionPolicy.Reset();
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockUnitOfWork = null;
            mockEmailManager = null;
            mockCalculationFactory = null;
            mockContext = null;
            mockCalculation = null;
            mockService = null;
            ExceptionPolicy.Reset();
        }

        private DataSource GetDataSourceWithEntries(params DataEntry[] entries)
        {
            entries = entries.Select(e =>
                {
                    e.SourceId = newSourceId;
                    return e;
                }).ToArray();
            var mockDataSource = new DataSource
                {
                    Id = newSourceId,
                    DataEntries = entries,
                    InputStatus = SourceStatus.PendingCalculation,
                    DateEdit = DateTime.Now,
                    UserName = "DataSourceUserName",
                    SourceErrors = Enumerable.Empty<SourceError>().ToList()
                };
            UnitOfWorkToReturnEntries(entries);
            return mockDataSource;
        }

        private void UnitOfWorkToReturnEntries(params DataEntry[] entries)
        {
            mockUnitOfWork
                .Setup(context => context.GetDataEntriesForSource(It.IsAny<Guid>()))
                .Returns(entries.AsQueryable());
        }

        private void SetupDataContextToReturnDataSource(DataSource source)
        {
            mockContext
                .Setup(context => context.GetDataSource<DataSource>(It.Is<Guid>(id => id == source.Id)))
                .Returns(source)
                .Verifiable();
        }

        private void SetupFileDataContextToReturnDataSource(FileDataSource source)
        {
            mockContext
                .Setup(context => context.GetDataSource<FileDataSource>(It.Is<Guid>(id => id == source.Id)))
                .Returns(source)
                .Verifiable();
        }

        private DataEntry Get3DayEntry()
        {
            var mockEntry = new Mock<DataEntry>();
            var entry = mockEntry.Object;
            entry.StartDate = DateTime.Now.Date;
            entry.EndDate = entry.StartDate.Value.AddDays(2);
            entry.Errors = Enumerable.Empty<DataError>().ToList();
            entry.Id = newEntryId;
            return entry;
        }

        private void SetupDailyDataCalculateDailyData()
        {
            mockCalculation
                .Setup(@base => @base.CalculateDailyData(It.IsAny<DataEntry>()))
                .Returns(new DailyData())
                .Verifiable();
        }

        private DataSourceService CreateService()
        {
            var mockEntry = new Mock<DataEntry>();
            var entry = mockEntry.Object;
            entry.Id = newEntryId;
            ExceptionPolicy.SetExceptionManager(mockExceptionManager.Object);

            mockService = new Mock<DataSourceService>(
                mockContext.Object,
                mockEmailManager.Object,
                mockCalculationFactory.Object)
                {
                    CallBase = true
                };
            return mockService.Object;
        }

        [TestMethod]
        public void CalculateEmissionsMustNotDoCalculationIfTheSourceIdIsNotFound()
        {
            //Arrange
            mockContext
                .Setup(context => context.GetDataSource<DataSource>(It.Is<Guid>(guid => guid == newSourceId)))
                .Returns((DataSource) null);

            //Act
            var result = CreateService().CalculateEmissions(newSourceId);

            //Assert
            Assert.AreEqual(newSourceId, result.SourceId);
            Assert.AreEqual(DataSourceServiceResources.FileSourceIsNotPendingCalculation, result.ErrorMessages[0]);
            Assert.IsFalse(result.Succeeded);
            mockCalculation
                .Verify(calculation => calculation.CalculateEmission(
                    It.IsAny<DateTime>(),
                    It.IsAny<DailyData>(),
                    It.IsAny<DataEntry>()), Times.Never);
        }

        [TestMethod]
        public void CalculateEmissionsMustNotDoCalculationIfTheSourceIsNotPendingCalculation()
        {
            //Arrange
            mockContext
                .Setup(context => context.GetDataSource<DataSource>(It.Is<Guid>(guid => guid == newSourceId)))
                .Returns(new DataSource {InputStatus = SourceStatus.Extracting});

            //Act
            var result = CreateService().CalculateEmissions(newSourceId);

            //Assert
            Assert.AreEqual(newSourceId, result.SourceId);
            Assert.AreEqual(DataSourceServiceResources.FileSourceIsNotPendingCalculation, result.ErrorMessages[0]);
            Assert.IsFalse(result.Succeeded);
            mockCalculation
                .Verify(calculation => calculation.CalculateEmission(
                    It.IsAny<DateTime>(),
                    It.IsAny<DailyData>(),
                    It.IsAny<DataEntry>()), Times.Never);
        }

        [TestMethod]
        public void CalculateEmissionsMustNotDoCalculationsIfSourceContainsErrors()
        {
            //Arrange
            var entry = Get3DayEntry();
            var source = GetDataSourceWithEntries(entry);
            SetupDataContextToReturnDataSource(source);
            mockContext
                .Setup(context => context.SourceContainsErrors(It.Is<Guid>(id => id == newSourceId)))
                .Returns(true)
                .Verifiable();

            //Act
            var result = CreateService().CalculateEmissions(newSourceId);

            //Assert
            mockContext.Verify();
            mockCalculation
                .Verify(
                    calculation =>
                    calculation.CalculateEmission(It.IsAny<DateTime>(), It.IsAny<DailyData>(), It.IsAny<DataEntry>()),
                    Times.Never);
            Assert.AreEqual(newSourceId, result.SourceId);
            Assert.AreEqual(DataSourceServiceResources.SourceContainsError, result.ErrorMessages[0]);
            Assert.IsFalse(result.Succeeded);
        }

        [TestMethod]
        public void CalculateEmissionsMustNotDoCalculationsIfSoureContainsDataEntriesInError()
        {
            //Arrange
            var source = GetDataSourceWithEntries();
            SetupDataContextToReturnDataSource(source);
            mockContext
                .Setup(context => context.SourceContainsDataEntriesInError(It.Is<Guid>(id => id == newSourceId)))
                .Returns(true)
                .Verifiable();

            //Act
            var result = CreateService().CalculateEmissions(newSourceId);

            //Assert
            mockContext.Verify();
            mockCalculation
                .Verify(
                    calculation =>
                    calculation.CalculateEmission(It.IsAny<DateTime>(), It.IsAny<DailyData>(), It.IsAny<DataEntry>()),
                    Times.Never);
            Assert.AreEqual(newSourceId, result.SourceId);
            Assert.AreEqual(DataSourceServiceResources.SourceContainsDataEntriesInError, result.ErrorMessages[0]);
            Assert.IsFalse(result.Succeeded);
        }

        [TestMethod]
        public void CalculateEmissionsMustRemovePreviousCalculations()
        {
            //Arrange
            var entry = Get3DayEntry();
            var source = GetDataSourceWithEntries(entry);
            SetupDataContextToReturnDataSource(source);
            SetupDailyDataCalculateDailyData();

            //Act
            CreateService().CalculateEmissions(newSourceId);

            //Assert
            mockContext.Verify(
                context => context.RemoveSourceCalculations(It.Is<Guid>(guid => guid == newSourceId)), Times.Once);
        }

        [TestMethod]
        public void CalculateEmissionsMustReturnErrorIfCalculationNotFound()
        {
            //Arrange
            var entry = Get3DayEntry();
            var source = GetDataSourceWithEntries(entry);
            SetupDataContextToReturnDataSource(source);
            mockCalculationFactory
                .Setup(factory => factory.ResolveCalculation(It.IsAny<Guid>()))
                .Returns((ICalculation) null);
            CreateService();
            mockService
                .Setup(service => service.HandleException(It.Is<Exception>(
                    exception => (exception is NullReferenceException) && (exception.Message == DataSourceServiceResources.CalculationNotFound))))
                .Callback(() => { })
                .Verifiable();

            //Act
            var result  = mockService.Object.CalculateEmissions(newSourceId);

            //Assert
            mockService.Verify();
            Assert.AreEqual(newSourceId, result.SourceId);
            Assert.AreEqual(DataSourceServiceResources.ExceptionOccuredDuringCalculation, result.ErrorMessages[0]);
            Assert.IsFalse(result.Succeeded);
        }

        [TestMethod]
        public void CalculateEmissionsMustCreateAndDisposeUnitOfWork()
        {
            //Arrange
            var entry = Get3DayEntry();
            var source = GetDataSourceWithEntries(entry);
            source.InputStatus = SourceStatus.PendingCalculation;
            SetupDataContextToReturnDataSource(source);
            SetupDailyDataCalculateDailyData();

            //Act
            CreateService().CalculateEmissions(newSourceId);

            //Assert
            mockContext
                .Verify(context => context.CreateUnitOfWork(), Times.Once);
            mockUnitOfWork
                .Verify(context => context.Dispose(), Times.Once);
        }
        
        [TestMethod]
        public void CalculateEmissionsMustCalculateEmissionForEachDateInRange()
        {
            //Arrange
            var entry = Get3DayEntry();
            var source = GetDataSourceWithEntries(entry);
            source.Id = newSourceId;
            source.InputStatus = SourceStatus.PendingCalculation;
            mockCalculation
                .Setup(calculation => calculation.GetDayDifference(It.Is<DataEntry>(
                    dataEntry => dataEntry.Id == newEntryId)))
                .Returns(2)
                .Verifiable();
            SetupDataContextToReturnDataSource(source);
            SetupDailyDataCalculateDailyData();

            //Act
            CreateService().CalculateEmissions(newSourceId);

            //Assert
            mockCalculation.Verify();
            mockCalculation.Verify(
                calcBase =>
                calcBase.CalculateEmission(
                    It.IsAny<DateTime>(),
                    It.IsAny<DailyData>(),
                    It.IsAny<DataEntry>()), Times.Exactly(3));
            mockUnitOfWork.Verify(
                context =>
                context.AddCarbonEmissionEntry(
                    It.IsAny<CarbonEmissionEntry>()), Times.Exactly(3));
        }

        [TestMethod]
        public void CalculateEmissionsMustCallDailyRateCalculationOnlyOnce()
        {
            //Arrange
            var entry = Get3DayEntry();
            SetupDailyDataCalculateDailyData();
            var source = GetDataSourceWithEntries(entry);
            SetupDataContextToReturnDataSource(source);

            //Act
            CreateService().CalculateEmissions(newSourceId);

            //Assert
            mockCalculation
                .Verify(@base => @base.CalculateDailyData(It.IsAny<DataEntry>()), Times.Once);
        }

        [TestMethod]
        public void CalculateEmissionsMustMarkTheSourceAsCompleted()
        {
            //Arrange
            var entry = Get3DayEntry();
            var source = GetDataSourceWithEntries(entry);
            SetupDataContextToReturnDataSource(source);
            SetupDailyDataCalculateDailyData();

            //Act
            CreateService().CalculateEmissions(newSourceId);

            //Assert
            mockContext
                .Setup(context => context.UpdateDataSource(
                    It.Is<FileDataSource>(fds => fds.InputStatus == SourceStatus.Calculated)));
        }

        [TestMethod]
        public void CalculateEmissionsMustNotCommitContextIfCalculationNotFound()
        {
            //Arrange
            var entry = Get3DayEntry();
            var source = GetDataSourceWithEntries(entry);
            SetupDataContextToReturnDataSource(source);
            SetupDailyDataCalculateDailyData();
            mockCalculationFactory
                .Setup(factory => factory.ResolveCalculation(It.IsAny<Guid>()))
                .Returns((ICalculation)null);
            CreateService();
            mockService
                .Setup(service => service.HandleException(It.IsAny<Exception>()))
                .Callback(() => { });

            //Act
            mockService.Object.CalculateEmissions(newSourceId);

            //Assert
            mockUnitOfWork.Verify(context => context.CommitWork(), Times.Never);
        }

        [TestMethod]
        public void CalculateEmissionsMustNotCommitContextIfExceptionOccurs()
        {
            //Arrange
            var entry = Get3DayEntry();
            var source = GetDataSourceWithEntries(entry);
            SetupDataContextToReturnDataSource(source);
            SetupDailyDataCalculateDailyData();
            mockCalculation
                .Setup(calculation => calculation.GetDayDifference(It.IsAny<DataEntry>()))
                .Throws<ArithmeticException>();
            CreateService();
            mockService
                .Setup(service => service.HandleException(It.Is<Exception>(
                    exception => (exception is ArithmeticException))))
                .Callback(() => { })
                .Verifiable();

            //Act
            var result = mockService.Object.CalculateEmissions(newSourceId);

            //Assert
            mockService.Verify();
            Assert.AreEqual(newSourceId, result.SourceId);
            Assert.AreEqual(DataSourceServiceResources.ExceptionOccuredDuringCalculation, result.ErrorMessages[0]);
            Assert.IsFalse(result.Succeeded);
            mockUnitOfWork.Verify(context => context.CommitWork(), Times.Never);
        }

        [TestMethod]
        public void CalculateEmissionsMustSendEmailWhenComplete()
        {
            //Arrange
            var entry = Get3DayEntry();
            var source = GetDataSourceWithEntries(entry);
            SetupDataContextToReturnDataSource(source);
            SetupDailyDataCalculateDailyData();
            mockContext
                .Setup(context => context.GetUserProfile(It.IsAny<string>()))
                .Returns(new UserProfile{Email = "testuseremail"})
                .Verifiable();

            //Act
            CreateService().CalculateEmissions(newSourceId);

            //Assert
            mockContext.Verify();
            mockEmailManager
                .Verify(manager => manager
                    .SendMail(
                    It.Is<DataSource>(dataSource => dataSource.Id == newSourceId),
                    It.Is<string>(s => s ==EmailTemplate.CalculationComplete),
                    It.Is<string[]>(strings => strings[0] == "testuseremail")));
        }

        [TestMethod]
        public void RevertCalculationMustNotRemoveCalculationsUnlessInputStatusIsCalculated()
        {
            //Arrange
            var source = new FileDataSource { Id = newSourceId, InputStatus = SourceStatus.PendingCalculation };
            SetupFileDataContextToReturnDataSource(source);

            //Act
            var result = CreateService().RevertCalculation(newSourceId);

            //Assert
            mockContext
                .Verify(context => context.RemoveSourceCalculations(It.IsAny<Guid>()), Times.Never());
            mockContext
                .Verify(context => context.UpdateDataSource(It.IsAny<FileDataSource>()), Times.Never());
            Assert.AreEqual(newSourceId, result.SourceId);
            Assert.AreEqual(DataSourceServiceResources.FileSourceIsNotCalculated, result.ErrorMessages[0]);
            Assert.IsFalse(result.Succeeded);
        }

        [TestMethod]
        public void RevertCalculationMustCallRemoveSourceCalculations()
        {
            //Arrange
            var source = new FileDataSource {Id = newSourceId, InputStatus = SourceStatus.Calculated};
            SetupFileDataContextToReturnDataSource(source);

            //Act
            var result = CreateService().RevertCalculation(newSourceId);

            //Assert
            mockContext
                .Verify(context => context.RemoveSourceCalculations(It.Is<Guid>(id=>id == newSourceId)), Times.Once);
            mockContext
                .Verify(context => context.UpdateDataSource(It.IsAny<FileDataSource>()), Times.Once);
            Assert.AreEqual(newSourceId, result.SourceId);
            Assert.AreEqual(0, result.ErrorMessages.Count);
            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public void RevertCalculationMustSetTheStatusToPendingCalculation()
        {
            //Arrange
            var source = new FileDataSource { Id = newSourceId, InputStatus = SourceStatus.Calculated };
            SetupFileDataContextToReturnDataSource(source);

            //Act
            var result = CreateService().RevertCalculation(newSourceId);

            //Assert
            mockContext
                .Verify(context => context.UpdateDataSource(
                    It.Is<FileDataSource>(ds=>
                        (ds.Id == newSourceId) &&
                        (ds.InputStatus == SourceStatus.PendingCalculation)
                    )), Times.Once);
            Assert.AreEqual(newSourceId, result.SourceId);
            Assert.AreEqual(0, result.ErrorMessages.Count);
            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public void InsertManualDataSourceMustCallAddDataSource()
        {
            //Arrange
            var contract = new ManualDataContract
                {
                    ReferenceNotes = "reference notes"
                };
            CreateService();
            mockContext
                .Setup(context => context.AddDataSource(It.Is<ManualDataSource>(
                    source =>
                    (source.InputStatus == SourceStatus.PendingExtraction) &&
                    (source.ReferenceNotes == "reference notes") &&
                    (source.UserName == "TestUserName"))))
                .Returns(new ManualDataSource {Id = newSourceId})
                .Verifiable();

            //Act
            var result = mockService.Object.InsertManualDataSource(contract);

            //Assert
            mockContext.Verify();
            Assert.AreEqual(newSourceId, result.SourceId);
            Assert.AreEqual(0, result.ErrorMessages.Count);
            Assert.IsTrue(result.Succeeded);
        }

        //todo:needs refactoring
        //[TestMethod]
        //public void ReportSourceErrorMustCallAddsourceError()
        //{
        //    //Arrange
        //    //Act
        //    CreateService().ReportSourceError(newSourceId, SourceErrorType.InvalidColumns, "ErrorMessage");

        //    //Assert
        //    mockContext
        //        .Verify(context => context.AddSourceError(It.Is<SourceError>(
        //            error =>
        //            (error.DataSourceId == newSourceId) &&
        //            (error.ErrorMessage == "ErrorMessage") &&
        //            (error.ErrorType == SourceErrorType.InvalidColumns))));
        //}

        [TestMethod]
        public void ExtractCompletedMustUpdateStatusToPendingCalculation()
        {
            //Arrange
            var source = new FileDataSource { Id = newSourceId, InputStatus = SourceStatus.Extracting,UserName = "user name"};
            SetupFileDataContextToReturnDataSource(source);
            mockContext
                .Setup(context => context.GetUserProfile(It.Is<string>(
                    s =>
                    s == "user name")))
                .Returns(new UserProfile {Email = "test email"})
                .Verifiable();

            //Act
            CreateService().ExtractCompleted(newSourceId);

            //Assert
            mockContext.Verify(context => context.UpdateDataSource(It.Is<FileDataSource>(
                dataSource =>
                (dataSource.Id == newSourceId) &&
                (dataSource.InputStatus == SourceStatus.PendingCalculation))), Times.Once);
            mockContext.Verify();
        }

        [TestMethod]
        public void ExtractCompletedMustNotSendMailIfProfileIsNotFound()
        {
            //Arrange
            var source = new FileDataSource { Id = newSourceId, InputStatus = SourceStatus.Extracting, UserName = "user name" };
            SetupFileDataContextToReturnDataSource(source);
            mockContext
                .Setup(context => context.GetUserProfile(It.IsAny<string>()))
                .Returns((UserProfile)null)
                .Verifiable();

            //Act
            var result = CreateService().ExtractCompleted(newSourceId);

            //Assert
            mockEmailManager.Verify(context => context.SendMail(It.IsAny<FileDataSource>(), It.Is<string>(s => s == EmailTemplate.ExtractComplete), It.IsAny<string[]>()), Times.Never);
            var message = string.Format(DataSourceServiceResources.UserNameNotFound, source.UserName);
            Assert.AreEqual(newSourceId, result.SourceId);
            Assert.AreEqual(message, result.ErrorMessages[0]);
            Assert.IsFalse(result.Succeeded);
        }

        [TestMethod]
        public void ExtractCompletedMustSendAnEmailUsingTheProfileEmailAddress()
        {
            //Arrange
            var source = new FileDataSource { Id = newSourceId, InputStatus = SourceStatus.Extracting, UserName = "user name" };
            SetupFileDataContextToReturnDataSource(source);
            mockContext
                .Setup(context => context.GetUserProfile(It.Is<string>(
                    s =>
                    s == "user name")))
                .Returns(new UserProfile { Email = "test email" })
                .Verifiable();

            
            //Act
            var result =CreateService().ExtractCompleted(newSourceId);

            //Assert
            mockContext.Verify(context => context.UpdateDataSource(It.Is<FileDataSource>(
                dataSource =>
                (dataSource.Id == newSourceId) &&
                (dataSource.InputStatus == SourceStatus.PendingCalculation))), Times.Once);
            mockContext.Verify();
            mockEmailManager
                .Verify(
                    context => context.SendMail(
                        It.Is<FileDataSource>(dataSource => dataSource.Id == newSourceId),
                        It.Is<string>(s => s == EmailTemplate.ExtractComplete),
                        It.Is<string[]>(strings => strings[0] == "test email")), Times.Once);
            Assert.AreEqual(newSourceId, result.SourceId);
            Assert.AreEqual(0, result.ErrorMessages.Count);
            Assert.IsTrue(result.Succeeded);
        }
    }
}
