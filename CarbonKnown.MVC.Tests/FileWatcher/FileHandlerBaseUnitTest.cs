using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using CarbonKnown.FileReaders;
using CarbonKnown.FileReaders.FileHandler;
using CarbonKnown.FileReaders.Readers;
using CarbonKnown.WCF.DataEntry;
using CarbonKnown.WCF.DataSource;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CarbonKnown.MVC.Tests.FileWatcher
{
    [TestClass]
    public class FileHandlerBaseUnitTest
    {
        private Mock<FileHandlerBase<DataEntryDataContract>> mockHandler;
        private Mock<IFileReader> mockReader;
        private Mock<IDataSourceService> mockDataSourceService;
        private readonly Stream testStream = new MemoryStream();

        private readonly IDictionary<string, object> inputData =
            new Dictionary<string, object>
                {
                    {"STARTDATE", "03/08/2013"},
                    {"ENDDATE", "04/08/2013"},
                    {"MONEY", 1.0M},
                    {"UNITS", 1.0M},
                    {"COSTCODE", "123456789"},
                };

        [TestInitialize]
        public void Initialize()
        {
            mockHandler = new Mock<FileHandlerBase<DataEntryDataContract>>(string.Empty) {CallBase = true};
            mockReader = new Mock<IFileReader>();
            mockDataSourceService = new Mock<IDataSourceService>();
            mockHandler
                .Setup(@base => @base.GetService<IDataSourceService>())
                .Returns(mockDataSourceService.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockHandler = null;
            mockReader = null;
            mockDataSourceService = null;
        }

        private void ArrangeHandlerBase(IEnumerable<IDictionary<string, object>> rowData)
        {
            mockReader
                .Setup(reader => reader.ExtractData(It.IsAny<Stream>()))
                .Returns(rowData);

            mockHandler
                .Setup(handler => handler.GetReaderFromType(It.IsAny<string>(),It.IsAny<Guid>()))
                .Returns(mockReader.Object);

        }

        private FileHandlerBase<DataEntryDataContract> CreateInstance()
        {
            return mockHandler.Object;
        }

        [TestMethod]
        public void ServiceCanBeFoundViaType()
        {
            //Arrange
            var wrapper = new ClientServiceWrapper<IAvisCourierService>("baseurl/manage");

            //Act
            var service = wrapper.Service;
            
            //Assert;
            Assert.IsInstanceOfType(service, typeof (IAvisCourierService));
        }

        [TestMethod]
        public void ServiceCanUsePort()
        {
            //Arrange
            var wrapper = new ClientServiceWrapper<IAvisCourierService>("baseurl:1669/manage");

            //Act
            var service = wrapper.Service;
            
            //Assert;
            Assert.IsInstanceOfType(service, typeof (IAvisCourierService));
        }

        [TestMethod]
        public void ServiceCanBeFoundViaTypeAndName()
        {
            //Arrange
            var wrapper = new ClientServiceWrapper<IDataSourceService>("baseurl", "source");

            //Act
            var service = wrapper.Service;
            
            //Assert;
            Assert.IsInstanceOfType(service, typeof(IDataSourceService));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WrapperMustThrowExceptionWhenInCorrectServiceIsFound()
        {
            //Arrange
            var wrapper = new ClientServiceWrapper<Object>("baseurl", "source");
            
            //Act
            var service = wrapper.Service;
            
            //Assert;
            Assert.IsNull(service);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void WrapperMustThrowExceptionWhenServiceIsNotFound()
        {
            //Arrange
            var wrapper = new ClientServiceWrapper<Object>();
            
            //Act
            var service = wrapper.Service;
            
            //Assert;
            Assert.IsNull(service);
            Assert.Fail();
        }

        [TestMethod]
        public void MissingColumnsMustReturnAllMissingColumns()
        {
            //Arrange
            var firstRow = new[] {new Dictionary<string, object> {{"invalidcolumn", "invalidvalue"}}};
            mockReader
                .Setup(reader => reader.ExtractData(It.IsAny<Stream>()))
                .Returns(firstRow)
                .Verifiable();
            var handler = mockHandler.Object;
            handler.GetReader = (s, guid) => mockReader.Object;

            var fileName = string.Format("{0}.xlsx", Guid.NewGuid());

            //Act
            var missingColumns = handler.MissingColumns(fileName, new MemoryStream());

            //Assert
            Assert.AreEqual(5,missingColumns.Count);
            var keys = missingColumns.Keys.ToArray();
            Assert.AreEqual("CostCode", keys[0]);
            var values = missingColumns[keys[0]].ToArray();
            Assert.AreEqual(1, values.Length);
            Assert.AreEqual("CostCode", values[0]);

            Assert.AreEqual("EndDate", keys[1]);
            values = missingColumns[keys[1]].ToArray();
            Assert.AreEqual(1, values.Length);
            Assert.AreEqual("EndDate", values[0]);

            Assert.AreEqual("Money", keys[2]);
            values = missingColumns[keys[2]].ToArray();
            Assert.AreEqual(1, values.Length);
            Assert.AreEqual("Money", values[0]);

            Assert.AreEqual("StartDate", keys[3]);
            values = missingColumns[keys[3]].ToArray();
            Assert.AreEqual(1, values.Length);
            Assert.AreEqual("StartDate", values[0]);

            Assert.AreEqual("Units", keys[4]);
            values = missingColumns[keys[4]].ToArray();
            Assert.AreEqual(1, values.Length);
            Assert.AreEqual("Units", values[0]);
        }

        [TestMethod]
        public void MissingColumnsAreEmptyIfNoColumnsAreMissing()
        {
            //Arrange
            var firstRow = new[]
                {
                    new Dictionary<string, object>
                        {
                            { "CostCode", "invalidvalue" },
                            { "EndDate", "invalidvalue" },
                            { "Money", "invalidvalue" },
                            { "StartDate", "invalidvalue" },
                            { "Units", "invalidvalue" }
                        }
                };
            mockReader
                .Setup(reader => reader.ExtractData(It.IsAny<Stream>()))
                .Returns(firstRow)
                .Verifiable();
            var handler = mockHandler.Object;
            handler.GetReader = (s, guid) => mockReader.Object;

            var fileName = string.Format("{0}.xlsx", Guid.NewGuid());

            //Act
            var missingColumns = handler.MissingColumns(fileName, new MemoryStream());

            //Assert
            Assert.AreEqual(0, missingColumns.Count);
        }

        [TestMethod]
        public void MissingColumnsIsCaseInSensitive()
        {
            //Arrange
            var firstRow = new[]
                {
                    new Dictionary<string, object>
                        {
                            { "COSTCODE", "invalidvalue" },
                            { "ENDdate", "invalidvalue" },
                            { "MONEY", "invalidvalue" },
                            { "startDATE", "invalidvalue" },
                            { "units", "invalidvalue" }
                        }
                };
            mockReader
                .Setup(reader => reader.ExtractData(It.IsAny<Stream>()))
                .Returns(firstRow)
                .Verifiable();
            var handler = mockHandler.Object;
            handler.GetReader = (s, guid) => mockReader.Object;

            var fileName = string.Format("{0}.xlsx", Guid.NewGuid());

            //Act
            var missingColumns = handler.MissingColumns(fileName, new MemoryStream());

            //Assert
            Assert.AreEqual(0, missingColumns.Count);
        }

        [TestMethod]
        public void MissingColumnsOnlyReadsTheFirstLine()
        {
            //Arrange
            var firstRow = new[]
                {
                    new Dictionary<string, object> { { "invalidcolumn", "invalidvalue" } },
                    new Dictionary<string, object>
                        {
                            { "CostCode", "invalidvalue" },
                            { "EndDate", "invalidvalue" },
                            { "Money", "invalidvalue" },
                            { "StartDate", "invalidvalue" },
                            { "Units", "invalidvalue" }
                        }
                };
            mockReader
                .Setup(reader => reader.ExtractData(It.IsAny<Stream>()))
                .Returns(firstRow)
                .Verifiable();
            var handler = mockHandler.Object;
            handler.GetReader = (s, guid) => mockReader.Object;

            var fileName = string.Format("{0}.xlsx", Guid.NewGuid());

            //Act
            var missingColumns = handler.MissingColumns(fileName, new MemoryStream());

            //Assert
            Assert.AreEqual(5, missingColumns.Count);
            var keys = missingColumns.Keys.ToArray();
            Assert.AreEqual("CostCode", keys[0]);
            var values = missingColumns[keys[0]].ToArray();
            Assert.AreEqual(1, values.Length);
            Assert.AreEqual("CostCode", values[0]);

            Assert.AreEqual("EndDate", keys[1]);
            values = missingColumns[keys[1]].ToArray();
            Assert.AreEqual(1, values.Length);
            Assert.AreEqual("EndDate", values[0]);

            Assert.AreEqual("Money", keys[2]);
            values = missingColumns[keys[2]].ToArray();
            Assert.AreEqual(1, values.Length);
            Assert.AreEqual("Money", values[0]);

            Assert.AreEqual("StartDate", keys[3]);
            values = missingColumns[keys[3]].ToArray();
            Assert.AreEqual(1, values.Length);
            Assert.AreEqual("StartDate", values[0]);

            Assert.AreEqual("Units", keys[4]);
            values = missingColumns[keys[4]].ToArray();
            Assert.AreEqual(1, values.Length);
            Assert.AreEqual("Units", values[0]);
        }

        [TestMethod]
        public void MappingMustSetTheExpectedFieldValue()
        {
            //Arrange
            ArrangeHandlerBase(new[] {inputData});
            var instance = CreateInstance();

            //Act
            instance.ProcessFile(@"C:\test\test.xlsx", testStream);

            //Assert
            mockHandler
                .Verify(@base => @base.UpsertDataEntry(It.Is<DataEntryDataContract>(
                    contract =>
                    (contract.CostCode == "123456789") &&
                    (contract.Money == 1.0M) &&
                    (contract.Units == 1.0M) &&
                    (contract.StartDate == new DateTime(2013, 08, 03)) &&
                    (contract.EndDate == new DateTime(2013, 08, 04)))));
        }

        [TestMethod]
        public void MustUseCorrectReaderForFileExtension()
        {
            //Arrange
            var instance = CreateInstance();
            instance.FileReaders.Add(".test", () => mockReader.Object);

            //Act
            instance.ProcessFile("C:\\test\\aaa.test", testStream);

            //Assert
            mockReader
                .Verify(reader => reader.ExtractData(It.IsAny<Stream>()), Times.Once);
        }

        [TestMethod]
        public void FileNameMustBeUsedAsSourceId()
        {
            //Arrange
            var newId = Guid.NewGuid();
            var fileName = string.Format("C:\\test\\{0}.xlsx", newId);

            //Act
            var actualValue = FileHandlerBase<DataEntryDataContract>.GetSourceId(fileName);

            //Assert
            Assert.AreEqual(newId, actualValue);
        }

        [TestMethod]
        public void ExtensionNotFoundMustReportAnError()
        {
            //Arrange
            var newId = Guid.NewGuid();
            var fileName = string.Format("C:\\test\\{0}.test", newId);
            var instance = CreateInstance();

            //Act
            instance.ProcessFile(fileName, testStream);

            //Assert
            mockHandler
                .Verify(@base => @base.ReportError(
                    It.Is<Guid>(guid => guid == newId),
                    It.Is<SourceErrorType>(type => type == SourceErrorType.FileTypeNotFound),
                    It.IsAny<string>()));
            mockHandler
                .Verify(@base => @base.ExtractCompleted(
                    It.Is<Guid>(guid => guid == newId)));
        }

        [TestMethod]
        public void MustConvertDataFromExtract()
        {
            //Arrange
            ArrangeHandlerBase(new[] {inputData});
            var instance = CreateInstance();

            //Act
            instance.ProcessFile(@"C:\test\test.xlsx", testStream);

            //Assert
            mockHandler
                .Verify(@base => @base.Convert(
                    It.IsAny<DataEntryDataContract>(),
                    It.Is<IDictionary<string, object>>(
                        dictionary =>
                        (dictionary["STARTDATE"] == "03/08/2013") &&
                        (dictionary["ENDDATE"] == "04/08/2013") &&
                        (dictionary["COSTCODE"] == "123456789") &&
                        ((decimal)dictionary["UNITS"] == 1m) &&
                        ((decimal) dictionary["MONEY"] == 1m) &&
                        (dictionary.Count == 5))));
        }

        [TestMethod]
        public void MustUpsertExtractedData()
        {
            //Arrange
            ArrangeHandlerBase(new[] {inputData});
            var instance = CreateInstance();

            //Act
            instance.ProcessFile(@"C:\test\test.xlsx", testStream);

            //Assert
            mockHandler
                .Verify(@base => @base.UpsertDataEntry(
                    It.IsAny<DataEntryDataContract>()));
        }
    }
}
