using System;
using System.Collections.Generic;
using System.IO;
using CarbonKnown.Calculation;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Source;
using CarbonKnown.FileReaders;
using CarbonKnown.FileReaders.FileHandler;
using CarbonKnown.WCF.DataSource;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using DataSourceService = CarbonKnown.MVC.BLL.DataSourceService;

namespace CarbonKnown.MVC.Tests.Service
{
    [TestClass]
    public class FileDataSourceServiceUnitTest
    {
        private Guid newSourceId;
        private Guid newEntryId;
        private Mock<ISourceDataContext> mockContext;
        private Mock<IDataEntriesUnitOfWork> mockUnitOfWork;
        private Mock<IEmailManager> mockEmailManager;
        private Mock<IStreamManager> mockStreamManager;
        private Mock<ICalculationFactory> mockCalculationFactory;
        private Mock<IHandlerFactory> mockHandlerFactory;
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
            mockStreamManager = new Mock<IStreamManager>();
            mockCalculationFactory = new Mock<ICalculationFactory>();
            mockHandlerFactory = new Mock<IHandlerFactory>();
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
            mockStreamManager = null;
            mockCalculationFactory = null;
            mockHandlerFactory = null;
            mockContext = null;
            mockCalculation = null;
            mockService = null;
            ExceptionPolicy.Reset();
        }

        private void SetupFileDataContextToReturnDataSource(FileDataSource source)
        {
            mockContext
                .Setup(context => context.GetDataSource<FileDataSource>(It.Is<Guid>(id => id == source.Id)))
                .Returns(source)
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
                mockHandlerFactory.Object,
                mockStreamManager.Object)
                {
                    CallBase = true
                };
            mockService
                .Setup(service => service.GetUserName())
                .Returns("TestUserName");
            return mockService.Object;
        }

        [TestMethod]
        public void GetHashMustBeNullIfStreamIsNull()
        {
            //Arrange
            var sut = CreateService();
            
            //Act
            var result = sut.GetHash(null);

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetHashMustBeNullIfStreamHasNoLength()
        {
            //Arrange
            var sut = CreateService();
            var stream = new MemoryStream();
            
            //Act
            var result = sut.GetHash(stream);

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetHashMustUseSHA1Algorithm()
        {
            //Arrange
            var sut = CreateService();
            var stream = new MemoryStream(new byte[] {100});

            //Act
            var result = sut.GetHash(stream);

            //Assert
            Assert.AreEqual("<686ÏNffi¢]¢€¡†\\-(t",result);
        }

        [TestMethod]
        public void UpsertFileDataSourceMustCallAddDatasourceWhenInserting()
        {
            //Arrange
            var contract = new FileStreamDataContract
                {
                    ReferenceNotes = "reference notes",
                    OriginalFileName = "Original name",
                    HandlerName = "handler name",
                    FileStream = new MemoryStream()
                };
            mockContext
                .Setup(context => context.AddDataSource(It.Is<FileDataSource>(
                    source =>
                    (source.InputStatus == SourceStatus.PendingExtraction) &&
                    (source.ReferenceNotes == "reference notes") &&
                    (source.OriginalFileName == "Original name") &&
                    (source.HandlerName == "handler name") &&
                    (source.UserName == "TestUserName"))))
                .Returns(new FileDataSource {Id = newSourceId, InputStatus = SourceStatus.PendingExtraction})
                .Verifiable();

            //Act
            CreateService().UpsertFileDataSource(contract);

            //Assert
            mockContext.Verify();
        }

        [TestMethod]
        public void UpsertFilesourceMustUpdateCurrentFileNameWithTheNewName()
        {
            //Arrange
            var source = new FileDataSource { Id = newSourceId, InputStatus = SourceStatus.PendingExtraction };
            mockStreamManager
                .Setup(manager => manager.StageStream(
                    It.Is<Guid>(guid => guid == newSourceId),
                    It.Is<string>(s => s == "Original Name"),
                    It.IsAny<Stream>()))
                .Returns("NewFileName")
                .Verifiable();
            mockContext
                .Setup(context => context.AddDataSource(It.IsAny<FileDataSource>()))
                .Returns(source);

            var uploadContract = new FileStreamDataContract
                {
                    SourceId = newSourceId,
                    OriginalFileName = "Original Name",
                    FileStream = new MemoryStream()
                };

            //Act
            CreateService().UpsertFileDataSource(uploadContract);

            //Assert
            mockService.Verify();
            mockStreamManager.Verify();
            mockContext.Verify(context => context.UpdateDataSource(It.Is<FileDataSource>(
                dataSource =>
                (dataSource.Id == newSourceId) &&
                (dataSource.InputStatus == SourceStatus.PendingExtraction) &&
                (dataSource.CurrentFileName == "NewFileName"))), Times.Once);
        }

        [TestMethod]
        public void UpsertFileDataSourceMustCancelfInsertingAndSourceIdNotNull()
        {
            //Arrange
            var source = new FileDataSource { Id = newSourceId };
            mockContext
                .Setup(context => context.AddDataSource(It.IsAny<FileDataSource>()))
                .Returns(source)
                .Verifiable();

            var uploadContract = new FileStreamDataContract
            {
                SourceId = newSourceId,
                OriginalFileName = "Original Name",
                FileStream = new MemoryStream()
            };

            //Act
            CreateService().UpsertFileDataSource(uploadContract);

            //Assert
            mockContext.Verify();
            mockService
                .Verify(service => service.CancelFileSourceExtraction(It.Is<Guid>(guid => guid == newSourceId)));
        }

        [TestMethod]
        public void UpsertFileDataSourceMustSetTheFileHashIfInserting()
        {
            //Arrange
            var source = new FileDataSource { Id = newSourceId };
            mockContext
                .Setup(context => context.AddDataSource(It.IsAny<FileDataSource>()))
                .Returns(source)
                .Verifiable();

            var uploadContract = new FileStreamDataContract
            {
                SourceId = newSourceId,
                OriginalFileName = "Original Name",
                FileStream = new MemoryStream(new byte[] { 100 })
            };

            //Act
            CreateService().UpsertFileDataSource(uploadContract);

            //Assert
             mockContext
                .Verify(context => context.AddDataSource(
                    It.Is<FileDataSource>(
                    dataSource => 
                    (dataSource.OriginalFileName == "Original Name") &&
                    (dataSource.FileHash) == "<686ÏNffi¢]¢€¡†\\-(t")));
        }

        [TestMethod]
        public void UpsertFileDataSourceMustCallValidateIfInserting()
        {
            //Arrange
            var source = new FileDataSource { Id = newSourceId, InputStatus = SourceStatus.PendingExtraction };
            mockContext
                .Setup(context => context.AddDataSource(It.IsAny<FileDataSource>()))
                .Returns(source)
                .Verifiable();
            mockContext
                .Setup(context => context.GetFileDataSource(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns((FileDataSource)null)
                .Verifiable();

            var uploadContract = new FileStreamDataContract
            {
                SourceId = newSourceId,
                OriginalFileName = "Original Name",
                FileStream = new MemoryStream(new byte[] { 100 })
            };

            //Act
            CreateService().UpsertFileDataSource(uploadContract);

            //Assert
            mockService
                .Verify(service => service.ValidateSource(It.Is<Guid>(guid => guid == newSourceId)));
        }

        [TestMethod]
        public void UpsertFileDataSourceMustNotCallAddIfUpdating()
        {
            //Arrange
            var source = new FileDataSource { Id = newSourceId, InputStatus = SourceStatus.PendingExtraction };
            mockContext
                .Setup(context => context.GetFileDataSource(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(source)
                .Verifiable();

            var uploadContract = new FileStreamDataContract
            {
                SourceId = newSourceId,
                OriginalFileName = "Original Name",
                FileStream = new MemoryStream(new byte[] { 100 })
            };

            //Act
            CreateService().UpsertFileDataSource(uploadContract);

            //Assert
            mockContext
                .Verify(context => context.AddDataSource(It.IsAny<FileDataSource>()),Times.Never);
        }

        [TestMethod]
        public void UpsertFileDataSourceMustCallValidateIfUpdating()
        {
            //Arrange
            var source = new FileDataSource { Id = newSourceId,InputStatus =  SourceStatus.PendingExtraction};
            mockContext
                .Setup(context => context.GetFileDataSource(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(source)
                .Verifiable();

            var uploadContract = new FileStreamDataContract
            {
                SourceId = newSourceId,
                OriginalFileName = "Original Name",
                FileStream = new MemoryStream(new byte[] { 100 })
            };

            //Act
            CreateService().UpsertFileDataSource(uploadContract);

            //Assert
            mockService
                .Verify(service => service.ValidateSource(It.Is<Guid>(guid => guid == newSourceId)));
        }

        [TestMethod]
        public void UpsertFileDataSourceMustReturnErrorIfFoundAndNotPendingExtraction()
        {
            //Arrange
            var source = new FileDataSource { Id = newSourceId, InputStatus = SourceStatus.PendingCalculation };
            mockContext
                .Setup(context => context.GetFileDataSource(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(source)
                .Verifiable();

            //Act
            var result = CreateService().CancelFileSourceExtraction(newSourceId);

            //Assert
            mockContext.Verify(context => context.UpdateDataSource(It.IsAny<FileDataSource>()), Times.Never);
            mockContext.Verify(context => context.AddDataSource(It.IsAny<FileDataSource>()), Times.Never);
            mockStreamManager.Verify(context => context.StageStream(It.IsAny<Guid>(), It.IsAny<string>(),It.IsAny<Stream>()), Times.Never);
            Assert.AreEqual(newSourceId, result.SourceId);
            Assert.AreEqual(DataSourceServiceResources.FileSourceIsNotPendingExtraction, result.ErrorMessages[0]);
            Assert.IsFalse(result.Succeeded);
        }

        [TestMethod]
        public void UpsertFileDataMustUpdateAllTheNewValuesWhenUpdating()
        {
            //Arrange
            var source = new FileDataSource { Id = newSourceId, InputStatus = SourceStatus.PendingExtraction };
            mockContext
                .Setup(context => context.GetFileDataSource(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(source)
                .Verifiable();
            var contract = new FileStreamDataContract
                {
                    SourceId = newSourceId,
                    HandlerName = "newHandlerName",
                    OriginalFileName = "newFileName",
                    ReferenceNotes = "newNotes"
                };
            CreateService();
            mockService
                .Setup(service => service.GetUserName())
                .Returns("testUserName")
                .Verifiable();
            mockService
                .Setup(service => service.ValidateSource(It.IsAny<Guid>()))
                .Returns((SourceResultDataContract)null)
                .Verifiable();

            //Act
            mockService.Object.UpsertFileDataSource(contract);

            //Assert
            mockService.Verify();
            mockContext.Verify();
            mockContext
                .Verify(context => context.UpdateDataSource(It.Is<FileDataSource>(
                    dataSource =>
                    (dataSource.Id == newSourceId) &&
                    (dataSource.UserName == "testUserName") &&
                    (dataSource.OriginalFileName == "newFileName") &&
                    (dataSource.HandlerName == "newHandlerName") &&
                    (dataSource.ReferenceNotes == "newNotes"))), Times.Once);
        }

        [TestMethod]
        public void UpsertFileDataMustNotUpdateHandlerNameIfNull()
        {
            //Arrange
            var source = new FileDataSource
                {
                    Id = newSourceId, 
                    InputStatus = SourceStatus.PendingExtraction,
                    HandlerName = "oldHandlerName",
                    OriginalFileName = "oldFileName",
                    ReferenceNotes ="oldReferenceNotes"
                };
            mockContext
                .Setup(context => context.GetFileDataSource(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(source)
                .Verifiable();
            CreateService();
            mockService
                .Setup(service => service.GetUserName())
                .Returns("testUserName")
                .Verifiable();
            mockService
                .Setup(service => service.ValidateSource(It.IsAny<Guid>()))
                .Returns((SourceResultDataContract)null)
                .Verifiable();
            var contract = new FileStreamDataContract
            {
                SourceId = newSourceId,
                OriginalFileName = "newFileName",
                ReferenceNotes = "newNotes"
            };            

            //Act
            mockService.Object.UpsertFileDataSource(contract);

            //Assert
            mockService.Verify();
            mockContext.Verify();
            mockContext
                .Verify(context => context.UpdateDataSource(It.Is<FileDataSource>(
                    dataSource =>
                    (dataSource.Id == newSourceId) &&
                    (dataSource.UserName == "testUserName") &&
                    (dataSource.OriginalFileName == "newFileName") &&
                    (dataSource.HandlerName == "oldHandlerName") &&
                    (dataSource.ReferenceNotes == "newNotes"))));
        }

        [TestMethod]
        public void UpsertFileDataMustNotUpdateOriginalFileNameIfNull()
        {
            //Arrange
            var source = new FileDataSource
                {
                    Id = newSourceId, 
                    InputStatus = SourceStatus.PendingExtraction,
                    HandlerName = "oldHandlerName",
                    OriginalFileName = "oldFileName",
                    ReferenceNotes ="oldReferenceNotes"
                };
            mockContext
                .Setup(context => context.GetFileDataSource(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(source)
                .Verifiable();
            CreateService();
            mockService
                .Setup(service => service.GetUserName())
                .Returns("testUserName")
                .Verifiable();
            mockService
                .Setup(service => service.ValidateSource(It.IsAny<Guid>()))
                .Returns((SourceResultDataContract)null)
                .Verifiable();
            var contract = new FileStreamDataContract
            {
                SourceId = newSourceId,
                HandlerName = "newHandlerName",
                ReferenceNotes = "newNotes"
            };            

            //Act
            mockService.Object.UpsertFileDataSource(contract);

            //Assert
            mockService.Verify();
            mockContext.Verify();
            mockContext
                .Verify(context => context.UpdateDataSource(It.Is<FileDataSource>(
                    dataSource =>
                    (dataSource.Id == newSourceId) &&
                    (dataSource.UserName == "testUserName") &&
                    (dataSource.OriginalFileName == "oldFileName") &&
                    (dataSource.HandlerName == "newHandlerName") &&
                    (dataSource.ReferenceNotes == "newNotes"))));
        }

        [TestMethod]
        public void UpsertFileDataMustNotUpdateReferenceNotesIfNull()
        {
            //Arrange
            var source = new FileDataSource
                {
                    Id = newSourceId, 
                    InputStatus = SourceStatus.PendingExtraction,
                    HandlerName = "oldHandlerName",
                    OriginalFileName = "oldFileName",
                    ReferenceNotes ="oldReferenceNotes"
                };
            mockContext
                .Setup(context => context.GetFileDataSource(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(source)
                .Verifiable();
            CreateService();
            mockService
                .Setup(service => service.GetUserName())
                .Returns("testUserName")
                .Verifiable();
            mockService
                .Setup(service => service.ValidateSource(It.IsAny<Guid>()))
                .Returns((SourceResultDataContract)null)
                .Verifiable();
            var contract = new FileStreamDataContract
            {
                SourceId = newSourceId,
                HandlerName = "newHandlerName",
                OriginalFileName = "newFileName",
            };            

            //Act
            mockService.Object.UpsertFileDataSource(contract);

            //Assert
            mockService.Verify();
            mockContext.Verify();
            mockContext
                .Verify(context => context.UpdateDataSource(It.Is<FileDataSource>(
                    dataSource =>
                    (dataSource.Id == newSourceId) &&
                    (dataSource.UserName == "testUserName") &&
                    (dataSource.OriginalFileName == "newFileName") &&
                    (dataSource.HandlerName == "newHandlerName") &&
                    (dataSource.ReferenceNotes == "oldReferenceNotes"))));
        }

        [TestMethod]
        public void CancelFileSourceExtractionMustNotRemoveSourceIfNotPendingExtraction()
        {
            //Arrange
            var source = new FileDataSource { Id = newSourceId, InputStatus = SourceStatus.PendingCalculation };
            SetupFileDataContextToReturnDataSource(source);

            //Act
            var result = CreateService().CancelFileSourceExtraction(newSourceId);

            //Assert
            mockContext.Verify(context => context.RemoveSource(It.IsAny<Guid>()), Times.Never);
            mockStreamManager.Verify(context => context.RemoveStream(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
            Assert.AreEqual(newSourceId, result.SourceId);
            Assert.AreEqual(DataSourceServiceResources.FileSourceIsNotPendingExtraction, result.ErrorMessages[0]);
            Assert.IsFalse(result.Succeeded);
        }

        [TestMethod]
        public void CancelFileSourceExtractionMustCallRemoveSource()
        {
            //Arrange
            var source = new FileDataSource { Id = newSourceId, InputStatus = SourceStatus.PendingExtraction, CurrentFileName = "CurrentFileName" };
            SetupFileDataContextToReturnDataSource(source);

            //Act
            var result = CreateService().CancelFileSourceExtraction(newSourceId);

            //Assert
            mockContext.Verify(context => context.RemoveSource(It.Is<Guid>(guid => guid == newSourceId)), Times.Once);
            mockStreamManager.Verify(context => context.RemoveStream(
                It.Is<Guid>(guid => guid == newSourceId),
                It.Is<string>(s => s == "CurrentFileName")), Times.Once);
            Assert.AreEqual(newSourceId, result.SourceId);
            Assert.AreEqual(0, result.ErrorMessages.Count);
            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public void ExtractDataMustNotUpdateIfInputStatusIsNotPendingExtraction()
        {
            //Arrange
            var source = new FileDataSource { Id = newSourceId, InputStatus = SourceStatus.PendingCalculation };
            SetupFileDataContextToReturnDataSource(source);

            //Act
            var result = CreateService().CancelFileSourceExtraction(newSourceId);

            //Assert
            mockContext.Verify(context => context.UpdateDataSource(It.IsAny<FileDataSource>()), Times.Never);
            mockContext.Verify(context => context.GetUserProfile(It.IsAny<string>()), Times.Never);
            mockEmailManager.Verify(context => context.SendMail(It.IsAny<FileDataSource>(), It.IsAny<string>()), Times.Never);
            Assert.AreEqual(newSourceId, result.SourceId);
            Assert.AreEqual(DataSourceServiceResources.FileSourceIsNotPendingExtraction, result.ErrorMessages[0]);
            Assert.IsFalse(result.Succeeded);
        }

        [TestMethod]
        public void ExtractDataMustNotUpdateIfSourceContainsErrors()
        {
            //Arrange
            var source = new FileDataSource { Id = newSourceId, InputStatus = SourceStatus.PendingExtraction };
            SetupFileDataContextToReturnDataSource(source);
            mockContext
                .Setup(context => context.SourceContainsErrors(It.Is<Guid>(guid => guid == newSourceId)))
                .Returns(true);

            //Act
            var result = CreateService().ExtractData(newSourceId);

            //Assert
            mockContext.Verify(context => context.UpdateDataSource(It.IsAny<FileDataSource>()), Times.Never);
            mockStreamManager.Verify(
                context => context.PrepareForExtraction(
                    It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            Assert.AreEqual(newSourceId, result.SourceId);
            Assert.AreEqual(DataSourceServiceResources.SourceContainsError, result.ErrorMessages[0]);
            Assert.IsFalse(result.Succeeded);
        }

        [TestMethod]
        public void ExtractDataMustNotValidateIfInputstatusIsNotPendingExtraction()
        {
            //Arrange
            var source = new FileDataSource { Id = newSourceId, InputStatus = SourceStatus.PendingCalculation };
            SetupFileDataContextToReturnDataSource(source);

            //Act
            var result = CreateService().ExtractData(newSourceId);

            //Assert
            mockContext.Verify(context => context.UpdateDataSource(It.IsAny<FileDataSource>()), Times.Never);
            mockContext.Verify(context => context.SourceContainsErrors(It.IsAny<Guid>()), Times.Never);
            mockStreamManager.Verify(
                context => context.PrepareForExtraction(
                    It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            Assert.AreEqual(newSourceId, result.SourceId);
            Assert.AreEqual(DataSourceServiceResources.FileSourceIsNotPendingExtraction, result.ErrorMessages[0]);
            Assert.IsFalse(result.Succeeded);
        }

        [TestMethod]
        public void ExtractDataMustCallStreamManagerPrepareForExtraction()
        {
            //Arrange
            var source = new FileDataSource
                {
                    Id = newSourceId,
                    InputStatus = SourceStatus.PendingExtraction,
                    HandlerName = "HandlerName",
                    CurrentFileName = "CurrentFileName"
                };
            SetupFileDataContextToReturnDataSource(source);
            mockContext
                .Setup(context => context.SourceContainsErrors(It.Is<Guid>(guid => guid == newSourceId)))
                .Returns(false);

            //Act
            var result = CreateService().ExtractData(newSourceId);

            //Assert
            mockContext.Verify(context => context.UpdateDataSource(It.IsAny<FileDataSource>()), Times.Once);
            mockStreamManager.Verify(
                context => context.PrepareForExtraction(
                    It.Is<Guid>(guid => guid == newSourceId),
                    It.Is<string>(s => s == "HandlerName"),
                    It.Is<string>(s => s == "CurrentFileName")), Times.Once);
            Assert.AreEqual(newSourceId, result.SourceId);
            Assert.AreEqual(0, result.ErrorMessages.Count);
            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public void ExtractDataMustSetTheStatusToExtracting()
        {
            //Arrange
            var source = new FileDataSource
            {
                Id = newSourceId,
                InputStatus = SourceStatus.PendingExtraction,
                HandlerName = "HandlerName",
                CurrentFileName = "CurrentFileName"
            };
            SetupFileDataContextToReturnDataSource(source);
            mockContext
                .Setup(context => context.SourceContainsErrors(It.Is<Guid>(guid => guid == newSourceId)))
                .Returns(false);

            //Act
            var result = CreateService().ExtractData(newSourceId);

            //Assert
            mockContext.Verify(context => context.UpdateDataSource(It.Is<FileDataSource>(
                dataSource =>
                (dataSource.Id == newSourceId) &&
                (dataSource.InputStatus == SourceStatus.Extracting))), Times.Once);
            mockStreamManager.Verify(
                context => context.PrepareForExtraction(
                    It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(newSourceId, result.SourceId);
            Assert.AreEqual(0, result.ErrorMessages.Count);
            Assert.IsTrue(result.Succeeded);
        }


        [TestMethod]
        public void ValidateSourceMustReturnAnErrorIfTheInputStatusIsNotPendingCalculation()
        {
            //Arrange
            var source = new FileDataSource {Id = newSourceId, InputStatus = SourceStatus.PendingCalculation};
            SetupFileDataContextToReturnDataSource(source);

            //Act
            var result = CreateService().ValidateSource(newSourceId);

            //Assert
            mockStreamManager.Verify(manager => manager.RetrieveData(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
            mockHandlerFactory.Verify(factory => factory.CreateHandler(It.IsAny<string>()), Times.Never);
            Assert.AreEqual(newSourceId, result.SourceId);
            Assert.AreEqual(DataSourceServiceResources.FileSourceIsNotPendingExtraction, result.ErrorMessages[0]);
            Assert.IsFalse(result.Succeeded);
        }

        [TestMethod]
        public void ValidateSourceMustRemovePreviousErrorsInSource()
        {
            //Arrange
            var source = new FileDataSource
            {
                Id = newSourceId,
                InputStatus = SourceStatus.PendingExtraction,
                CurrentFileName = "CurrentFileName"
            };
            SetupFileDataContextToReturnDataSource(source);
            CreateService();

            mockContext
                .Setup(context => context.UpdateDataSource(It.IsAny<FileDataSource>()))
                .Returns(source);
            mockService
                .Setup(
                    service =>
                    service.ReportSourceError(It.IsAny<Guid>(), It.IsAny<SourceErrorType>(), It.IsAny<string>()))
                .Callback(() => { });

            //Act
            mockService.Object.ValidateSource(newSourceId);

            //Assert
            mockContext
                .Verify(context => context.RemoveSourceErrors(It.Is<Guid>(guid => guid == newSourceId)), Times.Once);
            mockStreamManager
                .Verify(manager => manager.RetrieveData(It.IsAny<Guid>(), It.IsAny<string>()),Times.Never);
        }

        [TestMethod]
        public void ValidateSourceMustReportAnInvalidHandler()
        {
            //Arrange
            var source = new FileDataSource
                {
                    Id = newSourceId,
                    InputStatus = SourceStatus.PendingExtraction,
                    CurrentFileName = "CurrentFileName",
                    HandlerName = "HandlerName"
                };
            var message = string.Format(DataSourceServiceResources.InvalidHandlerMessage, "HandlerName");
            SetupFileDataContextToReturnDataSource(source);
            CreateService();
            mockHandlerFactory
                .Setup(factory => factory.CreateHandler(It.Is<string>(s => s == "HandlerName")))
                .Returns((IFileHandler) null)
                .Verifiable();
            mockService
                .Setup(service => service.ReportSourceError(
                    It.Is<Guid>(guid => guid == newSourceId),
                    It.Is<SourceErrorType>(type => type == SourceErrorType.FileTypeNotFound),
                    It.Is<string>(s => s == message)))
                .Callback(() => { })
                .Verifiable();

            //Act
            var result = mockService.Object.ValidateSource(newSourceId);

            //Assert
            mockService.Verify();
            mockHandlerFactory.Verify();
            mockStreamManager
                .Verify(manager => manager.RetrieveData(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
            Assert.AreEqual(newSourceId, result.SourceId);
            Assert.AreEqual(message, result.ErrorMessages[0]);
            Assert.IsFalse(result.Succeeded);
        }

        [TestMethod]
        public void ValidateSourceMustReportEachMissingColumn()
        {
            //Arrange
            var source = new FileDataSource
            {
                Id = newSourceId,
                InputStatus = SourceStatus.PendingExtraction,
                CurrentFileName = "CurrentFileName",
                HandlerName = "HandlerName"
            };
            SetupFileDataContextToReturnDataSource(source);
            CreateService();
            var mockHandler = new Mock<IFileHandler>();
            mockHandler
                .Setup(handler => handler.MissingColumns(
                    It.Is<string>(s => s == "CurrentFileName"),
                    It.IsAny<Stream>()))
                .Returns(new Dictionary<string, IEnumerable<string>>
                    {
                        {"col1", new[] {"match1a", "match1b"}},
                        {"col2", new[] {"match2a", "match2b"}},
                        {"col3", new[] {"match3a", "match3b"}},
                        {"col4", new[] {"match4a", "match4b"}},
                    })
                .Verifiable();
            mockStreamManager
                .Setup(manager => manager.RetrieveData(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(new MemoryStream())
                .Verifiable();
            mockHandlerFactory
                .Setup(factory => factory.CreateHandler(It.Is<string>(s => s == "HandlerName")))
                .Returns(mockHandler.Object)
                .Verifiable();
            mockContext
                .Setup(context => context.UpdateDataSource(It.IsAny<FileDataSource>()))
                .Returns(source);
            mockService
                .Setup(
                    service =>
                    service.ReportSourceError(It.IsAny<Guid>(), It.IsAny<SourceErrorType>(), It.IsAny<string>()))
                .Callback(() => { })
                .Verifiable();

            //Act
            var result = mockService.Object.ValidateSource(newSourceId);

            //Assert
            mockContext.Verify();
            mockService.Verify();
            mockHandlerFactory.Verify();
            mockHandler.Verify();
            mockStreamManager.Verify();
            mockService
                .Verify(
                    service =>
                    service.ReportSourceError(
                        It.Is<Guid>(guid => guid == newSourceId),
                        It.Is<SourceErrorType>(type => type == SourceErrorType.MissingFields),
                        It.IsAny<string>()), Times.Exactly(4));

            Assert.AreEqual(4, result.ErrorMessages.Count);
            Assert.AreEqual(newSourceId, result.SourceId);
            Assert.IsFalse(result.Succeeded);
            var message = string.Format(DataSourceServiceResources.MissingRowsMessage, "col1", "match1a,match1b");
            Assert.AreEqual(message, result.ErrorMessages[0]);
            message = string.Format(DataSourceServiceResources.MissingRowsMessage, "col2", "match2a,match2b");
            Assert.AreEqual(message, result.ErrorMessages[1]);
            message = string.Format(DataSourceServiceResources.MissingRowsMessage, "col3", "match3a,match3b");
            Assert.AreEqual(message, result.ErrorMessages[2]);
            message = string.Format(DataSourceServiceResources.MissingRowsMessage, "col4", "match4a,match4b");
            Assert.AreEqual(message, result.ErrorMessages[3]);
        }
    }
}
