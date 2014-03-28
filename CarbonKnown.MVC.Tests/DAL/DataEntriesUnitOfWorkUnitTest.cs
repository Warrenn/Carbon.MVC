using System;
using System.Collections.Generic;
using System.Linq;
using CarbonKnown.DAL;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.CarHire;
using CarbonKnown.MVC.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CarbonKnown.MVC.Tests.DAL
{
    [TestClass]
    public class DataEntriesUnitOfWorkUnitTest
    {
        [TestMethod]
        public void AddCarbonEmissionEntryMustCallDbSetAdd()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var mockSet = new Mock<FakeDbSet<CarbonEmissionEntry>>();
            mockContext
                .Setup(context => context.CarbonEmissionEntries)
                .Returns(mockSet.Object);
            var entry = new CarbonEmissionEntry {Id = 3};
            var sut = new DataEntriesUnitOfWork(() => mockContext.Object);

            //Act
            sut.AddCarbonEmissionEntry(entry);

            //Assert
            mockSet.Verify(set => set.Add(It.Is<CarbonEmissionEntry>(e => e.Id == 3)));
        }

        [TestMethod]
        public void AddDataErrorMustCallDataErrorsAdd()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var mockSet = new Mock<FakeDbSet<DataError>>();
            mockContext
                .Setup(context => context.DataErrors)
                .Returns(mockSet.Object);
            var entry = new DataError {Id = 3};
            var sut = new DataEntriesUnitOfWork(() => mockContext.Object);

            //Act
            sut.AddDataError(entry);

            //Assert
            mockSet.Verify(set => set.Add(It.Is<DataError>(e => e.Id == 3)));
        }

        [TestMethod]
        public void GetDataEntriesMustReturnDerivedDataEntryClasses()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var sourceId = Guid.NewGuid();
            var dataEntryEntries = new List<DataEntry>
                {
                    new DataEntry
                        {
                            Id = Guid.NewGuid(), 
                            SourceId = sourceId, 
                            CostCode = "testcode",
                            CalculationId = CarbonKnown.DAL.Models.Constants.Calculation.CarHireId
                        },
                    new CarHireData
                        {
                            Id = Guid.NewGuid(), 
                            SourceId = sourceId,
                            CarGroupBill = CarGroupBill.E,
                            CostCode = "testcodeaf1",
                            CalculationId = CarbonKnown.DAL.Models.Constants.Calculation.CarHireId
                        },
                    new CarHireData
                        {
                            Id = Guid.NewGuid(), 
                            SourceId = sourceId,
                            CarGroupBill = CarGroupBill.G,
                            CostCode = "testcodeaf",
                            CalculationId = CarbonKnown.DAL.Models.Constants.Calculation.CarHireId
                        },
                    new DataEntry
                        {
                            Id = Guid.NewGuid(), 
                            SourceId = Guid.NewGuid(), 
                            CostCode = "testcodea"
                        },
                    new DataEntry
                        {
                            Id = Guid.NewGuid(), 
                            SourceId = Guid.NewGuid(), 
                            CostCode = "testcodeb"
                        }
                };
            var avisEntries = new List<CarHireData>
                {
                    dataEntryEntries[1] as CarHireData,
                    dataEntryEntries[2] as CarHireData
                };
            var mockDataEntrySet = new Mock<FakeDbSet<DataEntry>>((object)dataEntryEntries) { CallBase = true };
            var mockAvisSet = new Mock<FakeDbSet<CarHireData>>((object)avisEntries) { CallBase = true };
            mockAvisSet
                .Setup(set => set.Find(It.IsAny<object[]>()))
                .Returns((object[] args) => avisEntries.FirstOrDefault(entry => entry.Id == (Guid)args[0]));
            mockContext
                .Setup(context => context.Set<DataEntry>())
                .Returns(mockDataEntrySet.Object);
            mockContext
                .Setup(context => context.Set<CarHireData>())
                .Returns(mockAvisSet.Object);
            var sut = new DataEntriesUnitOfWork(() => mockContext.Object);

            //Act
            var actual = sut.GetDataEntriesForSource(sourceId).ToArray();

            //Assert
            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual(typeof (DataEntry), actual[0].GetType());
            Assert.AreEqual(typeof(CarHireData), actual[1].GetType());
            Assert.AreEqual(typeof(CarHireData), actual[2].GetType());
            Assert.AreEqual(CarGroupBill.E, ((CarHireData) actual[1]).CarGroupBill);
            Assert.AreEqual(CarGroupBill.G, ((CarHireData)actual[2]).CarGroupBill);
        }

        [TestMethod]
        public void GetDataEntryMustMatchById()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var testId = Guid.NewGuid();
            var entries = new[]
                {
                    new DataEntry {Id = testId, CostCode = "testcode"},
                    new DataEntry {Id = Guid.NewGuid()}
                };
            var mockSet = new Mock<FakeDbSet<DataEntry>>((object)entries) { CallBase = true };
            mockSet
                .Setup(set => set.Find(It.IsAny<object[]>()))
                .Returns((object[] args) => entries.FirstOrDefault(entry => entry.Id == (Guid)args[0]));
            mockContext
                .Setup(context => context.Set<DataEntry>())
                .Returns(mockSet.Object);
            var sut = new DataEntriesUnitOfWork(() => mockContext.Object);

            //Act
            var actual = sut.GetDataEntry<DataEntry>(testId);

            //Assert
            Assert.AreEqual(testId, actual.Id);
            Assert.AreEqual("testcode", actual.CostCode);
        }

        [TestMethod]
        public void GetDataEntryMustReturnNullWhenNoMatchFound()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var testId = Guid.NewGuid();
            var entries = new[]
                {
                    new DataEntry {Id = Guid.NewGuid(), CostCode = "testcode"},
                    new DataEntry {Id = Guid.NewGuid()}
                };
            var mockSet = new Mock<FakeDbSet<DataEntry>>((object) entries) {CallBase = true};
            mockSet
                .Setup(set => set.Find(It.IsAny<object[]>()))
                .Returns((object[] args) => entries.FirstOrDefault(entry => entry.Id == (Guid) args[0]));
            mockContext
                .Setup(context => context.Set<DataEntry>())
                .Returns(mockSet.Object);
            var sut = new DataEntriesUnitOfWork(() => mockContext.Object);

            //Act
            var actual = sut.GetDataEntry<DataEntry>(testId);

            //Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void CreateDataEntryMustCallCreateFromSet()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var testId = Guid.NewGuid();
            var entry = new DataEntry
                {
                    Id = testId
                };
            var mockSet = new Mock<FakeDbSet<DataEntry>>((object)new []
                {
                    entry
                });
            mockSet
                .Setup(set => set.Create())
                .Returns(entry);
            mockContext
                .Setup(context => context.Set<DataEntry>())
                .Returns(mockSet.Object);
            var sut = new DataEntriesUnitOfWork(() => mockContext.Object);

            //Act
            var actual = sut.CreateDataEntry<DataEntry>();

            //Assert
            Assert.AreEqual(testId, actual.Id);
            mockSet.Verify(set => set.Create());
        }

        [TestMethod]
        public void RemoveDataErrorsMustOnlyIncludeMatchingDataEntryId()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var dataEntryId = Guid.NewGuid();
            var mockSet = new Mock<FakeDbSet<DataError>>(
                (object) new[]
                    {
                        new DataError {Id = 1, DataEntryId = dataEntryId},
                        new DataError {Id = 2, DataEntryId = dataEntryId},
                        new DataError {Id = 3, DataEntryId = dataEntryId},
                        new DataError {Id = 4, DataEntryId = Guid.NewGuid()},
                        new DataError {Id = 5, DataEntryId = Guid.NewGuid()}
                    });

            mockContext
                .Setup(context => context.DataErrors)
                .Returns(mockSet.Object);
            var sut = new DataEntriesUnitOfWork(() => mockContext.Object);

            //Act
            sut.RemoveDataErrors(dataEntryId);

            //Assert
            mockSet.Verify(set => set.Remove(
                It.IsAny<DataError>()), Times.Exactly(3));
            mockSet.Verify(set => set.Remove(
                It.Is<DataError>(entry => entry.Id == 1)), Times.Once);
            mockSet.Verify(set => set.Remove(
                It.Is<DataError>(entry => entry.Id == 2)), Times.Once);
            mockSet.Verify(set => set.Remove(
                It.Is<DataError>(entry => entry.Id == 3)), Times.Once);
        }

        [TestMethod]
        public void DisposeMustCallContextSaveChanges()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var sut = new DataEntriesUnitOfWork(() => mockContext.Object);

            //Act
            sut.CommitWork();
            sut.Dispose();

            //Assert
            mockContext.Verify(context => context.SaveChanges());
        }
    }
}
