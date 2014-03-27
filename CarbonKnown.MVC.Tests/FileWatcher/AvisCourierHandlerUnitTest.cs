using System;
using System.Collections.Generic;
using System.IO;
using CarbonKnown.DAL.Models.CarHire;
using CarbonKnown.FileReaders.AvisCourier;
using CarbonKnown.FileReaders.Readers;
using CarbonKnown.WCF.DataSource;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CarbonKnown.MVC.Tests.FileWatcher
{
    [TestClass]
    public class AvisCourierHandlerUnitTest
    {
        [TestMethod]
        public void TotalKmsDashMustBeValidUnits()
        {
            //Arrange
            var sut = new AvisCourierHandler("test");
            var contract = new AvisCourierDataContract();
            var values = new Dictionary<string, object> {{"TOTAL-KMS", 1234.12345}};
            //Act
            var result = sut.Convert(contract, values);
            //Assert
            Assert.AreEqual(1234.12345M, result.Units);
        }

        [TestMethod]
        public void TotalKmsSpaceMustBeValidUnits()
        {
            //Arrange
            var sut = new AvisCourierHandler("test");
            var contract = new AvisCourierDataContract();
            var values = new Dictionary<string, object> { { "TOTAL KMS", 1234.12345 } };
            //Act
            var result = sut.Convert(contract, values);
            //Assert
            Assert.AreEqual(1234.12345M, result.Units);
        }

        [TestMethod]
        public void CostCentreDashMustBeValidCostCode()
        {
            //Arrange
            var sut = new AvisCourierHandler("test");
            var contract = new AvisCourierDataContract();
            var values = new Dictionary<string, object> { { "COST-CENTRE", "123.345" } };
            //Act
            var result = sut.Convert(contract, values);
            //Assert
            Assert.AreEqual("000000123", result.CostCode);
        }

        [TestMethod]
        public void CostCentreMustNotRound()
        {
            //Arrange
            var sut = new AvisCourierHandler("test");
            var contract = new AvisCourierDataContract();
            var values = new Dictionary<string, object> { { "COST-CENTRE", "123.845" } };
            //Act
            var result = sut.Convert(contract, values);
            //Assert
            Assert.AreEqual("000000123", result.CostCode);
        }

        [TestMethod]
        public void CostCentreSpaceMustBeValidCostCode()
        {
            //Arrange
            var sut = new AvisCourierHandler("test");
            var contract = new AvisCourierDataContract();
            var values = new Dictionary<string, object> { { "COST CENTRE", "123.456" } };
            //Act
            var result = sut.Convert(contract, values);
            //Assert
            Assert.AreEqual("000000123", result.CostCode);
        }

        [TestMethod]
        public void CheckoutdateDashMustBeValidStartDate()
        {
            //Arrange
            var sut = new AvisCourierHandler("test");
            var contract = new AvisCourierDataContract();
            var values = new Dictionary<string, object> {{"CHECK-OUT-DATE", "2013-05-13"}};
            //Act
            var result = sut.Convert(contract, values);
            //Assert
            Assert.AreEqual(new DateTime(2013, 5, 13), result.StartDate);
        }

        [TestMethod]
        public void CheckoutdateSpaceMustBeValidStartDate()
        {
            //Arrange
            var sut = new AvisCourierHandler("test");
            var contract = new AvisCourierDataContract();
            var values = new Dictionary<string, object> { { "CHECK OUT DATE", "2013-05-13" } };
            //Act
            var result = sut.Convert(contract, values);
            //Assert
            Assert.AreEqual(new DateTime(2013, 5, 13), result.StartDate);
        }

        [TestMethod]
        public void CheckindateDashMustBeValidEndDate()
        {
            //Arrange
            var sut = new AvisCourierHandler("test");
            var contract = new AvisCourierDataContract();
            var values = new Dictionary<string, object> { { "CHECK-IN-DATE", "2013-05-13" } };
            //Act
            var result = sut.Convert(contract, values);
            //Assert
            Assert.AreEqual(new DateTime(2013, 5, 13), result.EndDate);
        }

        [TestMethod]
        public void CheckindateSpaceMustBeValidEndDate()
        {
            //Arrange
            var sut = new AvisCourierHandler("test");
            var contract = new AvisCourierDataContract();
            var values = new Dictionary<string, object> { { "CHECK IN DATE", "2013-05-13" } };
            //Act
            var result = sut.Convert(contract, values);
            //Assert
            Assert.AreEqual(new DateTime(2013, 5, 13), result.EndDate);
        }

        [TestMethod]
        public void TotalchargeDashMustBeValidMoney()
        {
            //Arrange
            var sut = new AvisCourierHandler("test");
            var contract = new AvisCourierDataContract();
            var values = new Dictionary<string, object> {{"TOTAL-CHARGE", 1234.12345}};
            //Act
            var result = sut.Convert(contract, values);
            //Assert
            Assert.AreEqual(1234.12345M, result.Money);
        }

        [TestMethod]
        public void TotalchargeSpaceMustBeValidMoney()
        {
            //Arrange
            var sut = new AvisCourierHandler("test");
            var contract = new AvisCourierDataContract();
            var values = new Dictionary<string, object> { { "TOTAL CHARGE", 1234.12345 } };
            //Act
            var result = sut.Convert(contract, values);
            //Assert
            Assert.AreEqual(1234.12345M, result.Money);
        }

        [TestMethod]
        public void CargroupbillDashMustBeValidCarGroupBill()
        {
            //Arrange
            var sut = new AvisCourierHandler("test");
            var contract = new AvisCourierDataContract();
            var values = new Dictionary<string, object> { { "CAR-GROUP-BILL", "D" } };
            //Act
            var result = sut.Convert(contract, values);
            //Assert
            Assert.AreEqual(CarGroupBill.D, result.CarGroupBill);
        }

        [TestMethod]
        public void CargroupbillSpaceMustBeValidCarGroupBill()
        {
            //Arrange
            var sut = new AvisCourierHandler("test");
            var contract = new AvisCourierDataContract();
            var values = new Dictionary<string, object> { { "CAR GROUP BILL", "D" } };
            //Act
            var result = sut.Convert(contract, values);
            //Assert
            Assert.AreEqual(CarGroupBill.D, result.CarGroupBill);
        }

        [TestMethod]
        public void AvisCourierServiceCalledForEachEntry()
        {
            //Arrange
            var sourceId = Guid.NewGuid();
            var mockReader = new Mock<IFileReader>();
            mockReader
                .Setup(reader => reader.ExtractData(It.IsAny<Stream>()))
                .Returns(new[]
                    {
                        new Dictionary<string, object>
                            {
                                {"CAR-GROUP-BILL", "D"}
                            },
                        new Dictionary<string, object>
                            {
                                {"CAR-GROUP-BILL", "E"}
                            },
                        new Dictionary<string, object>
                            {
                                {"CAR-GROUP-BILL", "F"}
                            }
                    });
            var mockService = new Mock<IAvisCourierService>();
            var mockSourceService = new Mock<IDataSourceService>();
            var mockSut = new Mock<AvisCourierHandler>("test") {CallBase = true};
            mockSut
                .Setup(handler => handler.GetService<IAvisCourierService>())
                .Returns(mockService.Object);
            mockSut
                .Setup(handler => handler.GetService<IDataSourceService>())
                .Returns(mockSourceService.Object);
            var sut = mockSut.Object;
            sut.GetReader = (s, guid) => mockReader.Object;
            var fileName = sourceId + ".xlsx";
            var stream = new MemoryStream();
            
            //Act
            sut.ProcessFile(fileName,stream);
            
            //Assert
            mockService
                .Verify(service => service.UpsertDataEntry(
                    It.Is<AvisCourierDataContract>(
                        data => (data.SourceId == sourceId)
                                && (
                                       (data.CarGroupBill == CarGroupBill.D) ||
                                       (data.CarGroupBill == CarGroupBill.E) ||
                                       (data.CarGroupBill == CarGroupBill.F))
                        )), Times.Exactly(3));
        }
    }
}
