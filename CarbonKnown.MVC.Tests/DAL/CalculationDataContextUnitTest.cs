using System;
using CarbonKnown.DAL;
using CarbonKnown.DAL.Models;
using CarbonKnown.Factors.WCF;
using CarbonKnown.MVC.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CarbonKnown.MVC.Tests.DAL
{
    [TestClass]
    public class CalculationDataContextUnitTest
    {

        [TestMethod]
        public void FactorValueMustCallTheSuppliedService()
        {
            //Arrange
            var mockService = new Mock<IFactorsService> {CallBase = true};
            var sut = new CalculationDataContext(new DataContext(), mockService.Object);
            var effectiveDate = DateTime.Now;
            var factorId = Guid.NewGuid();

            //Act
            sut.FactorValue(effectiveDate, factorId);

            //Assert
            mockService.Verify(service => service.FactorValuesById(
                It.Is<Guid>(g => g == factorId)), Times.Once);
        }

        [TestMethod]
        public void ReturnTrueForMatchingCostCodes()
        {
            //Arrange
            var mockService = new Mock<IFactorsService> {CallBase = true};
            var mockContext = new Mock<DataContext> {CallBase = true};
            var mockCostCentres = new FakeDbSet<CostCentre>(new[]
                {
                    new CostCentre {CostCode = "123"},
                    new CostCentre {CostCode = "1234"}
                });

            mockContext
                .SetupGet(context => context.CostCentres)
                .Returns(mockCostCentres);

            var sut = new CalculationDataContext(mockContext.Object, mockService.Object);

            //Act
            var actual = sut.CostCodeValid("123");

            //Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void ReturnFalseIfThereAreNoCostCodes()
        {
            //Arrange
            var mockService = new Mock<IFactorsService> {CallBase = true};
            var mockContext = new Mock<DataContext> {CallBase = true};
            var mockCostCentres = new FakeDbSet<CostCentre>(new[]
                {
                    new CostCentre {CostCode = "123"},
                    new CostCentre {CostCode = "1234"}
                });

            mockContext
                .SetupGet(context => context.CostCentres)
                .Returns(mockCostCentres);

            var sut = new CalculationDataContext(mockContext.Object, mockService.Object);

            //Act
            var actual = sut.CostCodeValid("1235");

            //Assert
            Assert.IsFalse(actual);
        }

        //todo:needs refactoring
        //[TestMethod]
        //public void ExceedsVarianceIsFalseIfValueIsNull()
        //{
        //    //Arrange
        //    var mockService = new Mock<IFactorsService> {CallBase = true};
        //    var mockContext = new Mock<DataContext> {CallBase = true};
        //    var calculationId = Guid.NewGuid();
        //    var mockVariance = new FakeDbSet<Variance>(new[]
        //        {
        //            new Variance
        //                {
        //                    CalculationId = calculationId,
        //                    ColumnName = "column1",
        //                    MaxValue = 123M,
        //                },
        //            new Variance
        //                {
        //                    CalculationId = calculationId,
        //                    ColumnName = "column2",
        //                    MaxValue = 123M,
        //                }
        //        });

        //    mockContext
        //        .SetupGet(context => context.Variances)
        //        .Returns(mockVariance);

        //    var sut = new CalculationDataContext(mockContext.Object, mockService.Object);

        //    //Act
        //    var actual = sut.Variance(calculationId, "clolumn1");

        //    //Assert
        //    Assert.IsFalse(actual);
        //}
        //todo:needs refactoring
        //[TestMethod]
        //public void ExceedsVarianceIsFalseIfValueIsLessForAMatchingCalculationIdAndColumn()
        //{
        //    //Arrange
        //    var mockService = new Mock<IFactorsService> {CallBase = true};
        //    var mockContext = new Mock<DataContext> {CallBase = true};
        //    var calculationId = Guid.NewGuid();
        //    var mockVariance = new FakeDbSet<Variance>(new[]
        //        {
        //            new Variance
        //                {
        //                    CalculationId = calculationId,
        //                    ColumnName =  "column1",
        //                    MaxValue = 123M,
        //                },
        //            new Variance
        //                {
        //                    CalculationId = calculationId,
        //                    ColumnName = "column2",
        //                    MaxValue = 123M,
        //                }
        //        });

        //    mockContext
        //        .SetupGet(context => context.Variances)
        //        .Returns(mockVariance);

        //    var sut = new CalculationDataContext(mockContext.Object, mockService.Object);

        //    //Act
        //    var actual = sut.ExceedsVariance(calculationId, "column1", 123M - 1);

        //    //Assert
        //    Assert.IsFalse(actual);
        //}
        //todo:needs refactoring
        //[TestMethod]
        //public void ExceedsVarianceIsFalseIfThereIsNoMatchingCalculationId()
        //{
        //    //Arrange
        //    var mockService = new Mock<IFactorsService> {CallBase = true};
        //    var mockContext = new Mock<DataContext> {CallBase = true};
        //    var calculationId = Guid.NewGuid();
        //    var mockVariance = new FakeDbSet<Variance>(new[]
        //        {
        //            new Variance
        //                {
        //                    CalculationId = calculationId,
        //                    ColumnName = "column1",
        //                    MaxValue = 123M,
        //                },
        //            new Variance
        //                {
        //                    CalculationId = calculationId,
        //                    ColumnName = "column2",
        //                    MaxValue = 123M,
        //                }
        //        });

        //    mockContext
        //        .SetupGet(context => context.Variances)
        //        .Returns(mockVariance);

        //    var sut = new CalculationDataContext(mockContext.Object, mockService.Object);

        //    //Act
        //    var actual = sut.ExceedsVariance(Guid.NewGuid(), "column1", 123M);

        //    //Assert
        //    Assert.IsFalse(actual);
        //}
        //todo:needs refactoring
        //[TestMethod]
        //public void ExceedsVarianceIsFalseIfTherIsNoMatchingColumnName()
        //{
        //    //Arrange
        //    var mockService = new Mock<IFactorsService> {CallBase = true};
        //    var mockContext = new Mock<DataContext> {CallBase = true};
        //    var calculationId = Guid.NewGuid();
        //    var mockVariance = new FakeDbSet<Variance>(new[]
        //        {
        //            new Variance
        //                {
        //                    CalculationId = calculationId,
        //                    ColumnName =  "column1",
        //                    MaxValue = 123M,
        //                },
        //            new Variance
        //                {
        //                    CalculationId = calculationId,
        //                    ColumnName = "column2",
        //                    MaxValue = 123M,
        //                }
        //        });

        //    mockContext
        //        .SetupGet(context => context.Variances)
        //        .Returns(mockVariance);

        //    var sut = new CalculationDataContext(mockContext.Object, mockService.Object);

        //    //Act
        //    var actual = sut.ExceedsVariance(calculationId, "column3", 123M);

        //    //Assert
        //    Assert.IsFalse(actual);
        //}
        //todo:needs refactoring
        //[TestMethod]
        //public void ExceedsVarianceIsFalseIfValueIsEqualToMatchingColumnNameAndCalculationId()
        //{
        //    //Arrange
        //    var mockService = new Mock<IFactorsService> {CallBase = true};
        //    var mockContext = new Mock<DataContext> {CallBase = true};
        //    var calculationId = Guid.NewGuid();
        //    var mockVariance = new FakeDbSet<Variance>(new[]
        //        {
        //            new Variance
        //                {
        //                    CalculationId = calculationId,
        //                    ColumnName =  "column1",
        //                    MaxValue = 123M,
        //                },
        //            new Variance
        //                {
        //                    CalculationId = calculationId,
        //                    ColumnName = "column2",
        //                    MaxValue = 123M,
        //                }
        //        });

        //    mockContext
        //        .SetupGet(context => context.Variances)
        //        .Returns(mockVariance);

        //    var sut = new CalculationDataContext(mockContext.Object, mockService.Object);

        //    //Act
        //    var actual = sut.ExceedsVariance(calculationId, "column1", 123M);

        //    //Assert
        //    Assert.IsFalse(actual);
        //}
        //todo:needs refactoring
        //[TestMethod]
        //public void ExceedsVarianceIsTrueIfValueIsGreaterThanMatchingColumnNameAndCalculationId()
        //{
        //    //Arrange
        //    var mockService = new Mock<IFactorsService> {CallBase = true};
        //    var mockContext = new Mock<DataContext> {CallBase = true};
        //    var calculationId = Guid.NewGuid();
        //    var mockVariance = new FakeDbSet<Variance>(new[]
        //        {
        //            new Variance
        //                {
        //                    CalculationId = calculationId,
        //                    ColumnName = "column1",
        //                    MaxValue = 123M,
        //                },
        //            new Variance
        //                {
        //                    CalculationId = calculationId,
        //                    ColumnName = "column2",
        //                    MaxValue = 123M,
        //                }
        //        });

        //    mockContext
        //        .SetupGet(context => context.Variances)
        //        .Returns(mockVariance);

        //    var sut = new CalculationDataContext(mockContext.Object, mockService.Object);

        //    //Act
        //    var actual = sut.ExceedsVariance(calculationId, "column1", 123M + 1);

        //    //Assert
        //    Assert.IsTrue(actual);
        //}

        [TestMethod]
        public void EntryIsDuplicateIsTrueForAMatchingHash()
        {
            //Arrange
            var mockService = new Mock<IFactorsService> {CallBase = true};
            var mockContext = new Mock<DataContext> {CallBase = true};
            var testEntry = new TestEntry {TestValue = "test", Id = Guid.NewGuid()};
            var mockDataEntry = new FakeDbSet<DataEntry>(new[]
                {
                    new TestEntry {TestValue = "test"},
                    new TestEntry {TestValue = "test1"}
                });

            mockContext
                .SetupGet(context => context.DataEntries)
                .Returns(mockDataEntry);

            var sut = new CalculationDataContext(mockContext.Object, mockService.Object);

            //Act
            var actual = sut.EntryIsDuplicate(testEntry.Id, testEntry.Hash);

            //Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void EntryIsDuplicateIsFalseForNonMatchingHash()
        {
            //Arrange
            var mockService = new Mock<IFactorsService> { CallBase = true };
            var mockContext = new Mock<DataContext> { CallBase = true };
            var entryId = Guid.NewGuid();
            var testEntry = new TestEntry { TestValue = "test3",Id = entryId};
            var mockDataEntry = new FakeDbSet<DataEntry>(new[]
                {
                    new TestEntry {TestValue = "test"},
                    new TestEntry {TestValue = "test"},
                    testEntry
                });

            mockContext
                .SetupGet(context => context.DataEntries)
                .Returns(mockDataEntry);

            var sut = new CalculationDataContext(mockContext.Object, mockService.Object);

            //Act
            var actual = sut.EntryIsDuplicate(entryId, testEntry.Hash);

            //Assert
            Assert.IsFalse(actual);
        }

        public class TestEntry : DataEntry
        {
            public string TestValue { get; set; }
        }
    }
}
