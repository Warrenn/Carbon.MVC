using System;
using System.Linq;
using CarbonKnown.Calculation;
using CarbonKnown.Calculation.DAL;
using CarbonKnown.Calculation.Models;
using CarbonKnown.Calculation.Properties;
using CarbonKnown.DAL.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CarbonKnown.MVC.Tests.Calculation
{
    [TestClass]
    public class CalculationBaseUnitTest
    {
        private DataClass entry;
        private Mock<CalculationBase<DataClass>> mockBase;
        private Mock<ICalculationDataContext> mockContext;

        [TestInitialize]
        public void Initialize()
        {
            mockContext = new Mock<ICalculationDataContext>();
            mockContext
                .Setup(context => context.CostCodeValid(It.IsAny<string>()))
                .Returns(true);
            entry = new DataClass
                {
                    EndDate = DateTime.Now.AddDays(1),
                    Money = 0,
                    StartDate = DateTime.Now,
                    CostCode = string.Empty,
                    UserName = string.Empty,
                    Options = StringSplitOptions.RemoveEmptyEntries,
                    StringValue = string.Empty
                };
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockBase = null;
            mockContext = null;
            entry = null;
        }

        private void SetupBase()
        {
            mockBase = new Mock<CalculationBase<DataClass>>(mockContext.Object) {CallBase = true};
            mockBase
                .Setup(
                    @base => @base.CalculateEmission(It.IsAny<DateTime>(), It.IsAny<DailyData>(), It.IsAny<DataClass>()))
                .Returns(0)
                .Verifiable();
        }
        
        [TestMethod]
        public void ReturnErrorForMissingCostCode()
        {
            //Arrange
            entry.CostCode = null;
            mockContext
                .Setup(context => context.CostCodeValid(It.IsAny<string>()))
                .Returns(true);
            SetupBase();
            
            //Act
            var result = mockBase.Object.ValidateEntry(entry);

            //Assert
            var actual = result.ToArray();
            Assert.AreEqual(1, actual.Length);
            var actualError = actual[0];
            Assert.AreEqual("CostCode", actualError.Column);
            Assert.AreEqual(DataErrorType.MissingValue, actualError.ErrorType);
            Assert.AreEqual(string.Format(Resources.MissingValueMessage, "CostCode"), actualError.Message);
        }
        
        [TestMethod]
        public void ReferenceNoCanBeNullByDefault()
        {
            //Arrange
            entry.RowNo = null;
            SetupBase();
            
            //Act
            var result = mockBase.Object.ValidateEntry(entry);

            //Assert
            var actual = result.ToArray();
            Assert.AreEqual(0, actual.Length);
        }
        
        [TestMethod]
        public void UnitsCanBeNullByDefault()
        {
            //Arrange
            entry.Units = null;
            SetupBase();
            
            //Act
            var result = mockBase.Object.ValidateEntry(entry);

            //Assert
            var actual = result.ToArray();
            Assert.AreEqual(0, actual.Length);
        }
        
        [TestMethod]
        public void ReturnErrorForMissingUserName()
        {
            //Arrange
            entry.UserName = null;
            SetupBase();
            
            //Act
            var result = mockBase.Object.ValidateEntry(entry);

            //Assert
            var actual = result.ToArray();
            Assert.AreEqual(1, actual.Length);
            var actualError = actual[0];
            Assert.AreEqual("UserName", actualError.Column);
            Assert.AreEqual(DataErrorType.MissingValue, actualError.ErrorType);
            Assert.AreEqual(string.Format(Resources.MissingValueMessage, "UserName"), actualError.Message);
        }


        [TestMethod]
        public void ReturnErrorForMissingMoney()
        {
            //Arrange
            entry.Money = null;
            entry.StartDate = DateTime.Now;
            entry.EndDate = entry.StartDate.Value.AddDays(1);
            mockBase = new Mock<CalculationBase<DataClass>>(mockContext.Object) {CallBase = true};

            //Act
            var result = mockBase.Object.ValidateEntry(entry);

            //Assert
            var actual = result.ToArray();
            Assert.AreEqual(1, actual.Length);
            var actualError = actual[0];
            Assert.AreEqual("Money", actualError.Column);
            Assert.AreEqual(DataErrorType.MissingValue, actualError.ErrorType);
            Assert.AreEqual(string.Format(Resources.MissingValueMessage, "Money"), actualError.Message);
        }

        [TestMethod]
        public void MustCheckNullableMembersByDefault()
        {
            //Arrange
            entry.Options = null;
            SetupBase();

            //Act
            var result = mockBase.Object.ValidateEntry(entry);

            //Assert
            var actual = result.ToArray();
            Assert.AreEqual(1, actual.Length);
            var actualError = actual[0];
            Assert.AreEqual("Options", actualError.Column);
            Assert.AreEqual(DataErrorType.MissingValue, actualError.ErrorType);
            Assert.AreEqual(string.Format(Resources.MissingValueMessage, "Options"), actualError.Message);
        }

        [TestMethod]
        public void MustCheckStringMembersByDefault()
        {
            //Arrange
            entry.StringValue = null;
            SetupBase();

            //Act
            var result = mockBase.Object.ValidateEntry(entry);

            //Assert
            var actual = result.ToArray();
            Assert.AreEqual(1, actual.Length);
            var actualError = actual[0];
            Assert.AreEqual("StringValue", actualError.Column);
            Assert.AreEqual(DataErrorType.MissingValue, actualError.ErrorType);
            Assert.AreEqual(string.Format(Resources.MissingValueMessage, "StringValue"), actualError.Message);
        }

        [TestMethod]
        public void MustNotCheckExplicitlyOptedOutMembers()
        {
            //Arrange
            entry.StringValue = null;
            entry.Options = null;
            SetupBase();
            var calcBase = mockBase.Object;
            calcBase.CanBeNull(@class => @class.Options);
            calcBase.CanBeNull(@class => @class.StringValue);

            //Act
            var result = mockBase.Object.ValidateEntry(entry);

            //Assert
            var actual = result.ToArray();
            Assert.AreEqual(0, actual.Length);
        }

        [TestMethod]
        public void ReturnErrorForMissingStartDate()
        {
            //Arrange
            entry.StartDate = null;
            mockBase = new Mock<CalculationBase<DataClass>>(mockContext.Object) {CallBase = true};

            //Act
            var result = mockBase.Object.ValidateEntry(entry);

            //Assert
            var actual = result.ToArray();
            Assert.AreEqual(1, actual.Length);
            var actualError = actual[0];
            Assert.AreEqual("StartDate", actualError.Column);
            Assert.AreEqual(DataErrorType.MissingValue, actualError.ErrorType);
            Assert.AreEqual(string.Format(Resources.MissingValueMessage, "StartDate"), actualError.Message);
        }

        [TestMethod]
        public void ReturnErrorForMissingEndDate()
        {
            //Arrange
            entry.EndDate = null;
            mockBase = new Mock<CalculationBase<DataClass>>(mockContext.Object) {CallBase = true};

            //Act
            var result = mockBase.Object.ValidateEntry(entry);

            //Assert
            var actual = result.ToArray();
            Assert.AreEqual(1, actual.Length);
            var actualError = actual[0];
            Assert.AreEqual("EndDate", actualError.Column);
            Assert.AreEqual(DataErrorType.MissingValue, actualError.ErrorType);
            Assert.AreEqual(string.Format(Resources.MissingValueMessage, "EndDate"), actualError.Message);
        }

        [TestMethod]
        public void ReturnErrorWhenStartDateGreaterThanEndDate()
        {
            //Arrange
            var expectedSpan = TimeSpan.FromDays(1);
            entry.EndDate = DateTime.Now;
            entry.StartDate = entry.EndDate.Value.Add(expectedSpan);
            mockBase = new Mock<CalculationBase<DataClass>>(mockContext.Object) {CallBase = true};

            //Act
            var result = mockBase.Object.ValidateEntry(entry);

            //Assert
            var actual = result.ToArray();
            Assert.AreEqual(2, actual.Length);
            var actualError = actual[0];
            Assert.AreEqual("StartDate", actualError.Column);
            Assert.AreEqual(DataErrorType.StartDateGreaterThanEndDate, actualError.ErrorType);
            Assert.AreEqual(Resources.StartDateGreaterThanEndDateMessage, actualError.Message);
            actualError = actual[1];
            Assert.AreEqual("EndDate", actualError.Column);
            Assert.AreEqual(DataErrorType.StartDateGreaterThanEndDate, actualError.ErrorType);
            Assert.AreEqual(Resources.StartDateGreaterThanEndDateMessage, actualError.Message);
        }

        [TestMethod]
        public void GetDayDifferenceCorrectly()
        {
            //Arrange
            var expectedSpan = TimeSpan.FromDays(1);
            entry.StartDate = DateTime.Now;
            entry.EndDate = entry.StartDate.Value.Add(expectedSpan);
            mockBase = new Mock<CalculationBase<DataClass>>(mockContext.Object) {CallBase = true};

            //Act
            var result = mockBase.Object.GetDayDifference(entry);

            //Assert
            Assert.AreEqual(1, result);
        }


        [TestMethod]
        public void AddCustomValidationToErrors()
        {
            //Arrange
            mockBase = new Mock<CalculationBase<DataClass>>(mockContext.Object) {CallBase = true};
            mockBase
                .Setup(b => b.ValidateEntry(It.IsAny<DataClass>()))
                .Returns(new[]
                    {
                        new DataError
                            {
                                Column = "TestColumn",
                                ErrorType = DataErrorType.MissingValue,
                                Message = "TestMessage"
                            }
                    });

            //Act
            var result = mockBase.Object.ValidateEntry(entry);

            //Assert
            var actual = result.ToArray();
            Assert.AreEqual(1, actual.Length);
            var actualError = actual[0];
            Assert.AreEqual("TestColumn", actualError.Column);
            Assert.AreEqual(DataErrorType.MissingValue, actualError.ErrorType);
            Assert.AreEqual("TestMessage", actualError.Message);
        }

        [TestMethod]
        public void CalculateDailyDataDefaultsMustDivideMoneyByDays()
        {
            //Arrange
            var expectedSpan = TimeSpan.FromDays(2);
            entry.StartDate = DateTime.Now;
            entry.EndDate = entry.StartDate.Value.Add(expectedSpan);
            entry.Money = 30;
            entry.Units = 60;
            mockBase = new Mock<CalculationBase<DataClass>>(mockContext.Object) {CallBase = true};

            //Act
            var result = mockBase.Object.CalculateDailyData(entry);

            //Assert
            Assert.AreEqual(20, result.UnitsPerDay);
            Assert.AreEqual(10, result.MoneyPerDay);
        }

        [TestMethod]
        public void CalculateDailyDataDefaultsMustNotDivideMoneyWhenZeroDays()
        {
            //Arrange
            entry.StartDate = DateTime.Now;
            entry.EndDate = entry.StartDate;
            entry.Money = 30;
            entry.Units = 60;
            mockBase = new Mock<CalculationBase<DataClass>>(mockContext.Object) {CallBase = true};

            //Act
            var result = mockBase.Object.CalculateDailyData(entry);

            //Assert
            Assert.AreEqual(60, result.UnitsPerDay);
            Assert.AreEqual(30, result.MoneyPerDay);
        }

        [TestMethod]
        public void ReturnErrorForDuplicateData()
        {
            //Arrange
            mockContext
                .Setup(context => context.EntryIsDuplicate(It.IsAny<Guid>(), It.IsAny<int>()))
                .Returns(true);
            mockBase = new Mock<CalculationBase<DataClass>>(mockContext.Object) {CallBase = true};

            //Act
            var result = mockBase.Object.ValidateEntry(entry);

            //Assert
            var actual = result.ToArray();
            Assert.AreEqual(1, actual.Length);
            var actualError = actual[0];
            Assert.AreEqual(string.Empty, actualError.Column);
            Assert.AreEqual(DataErrorType.DuplicateEntry, actualError.ErrorType);
            Assert.AreEqual(Resources.DuplicateMessage, actualError.Message);
        }

        [TestMethod]
        public void ReturnErrorWhenVarianceExceeded()
        {
            //Arrange
            mockContext
                .Setup(context => context.ExceedsVariance(
                    It.IsAny<Guid>(),
                    It.Is<string>(c => c == "Money"),
                    It.IsAny<decimal?>()))
                .Returns(true);
            mockBase = new Mock<CalculationBase<DataClass>>(mockContext.Object) {CallBase = true};

            //Act
            var result = mockBase.Object.ValidateEntry(entry);

            //Assert
            var actual = result.ToArray();
            Assert.AreEqual(1, actual.Length);
            var actualError = actual[0];
            Assert.AreEqual("Money", actualError.Column);
            Assert.AreEqual(DataErrorType.BelowVarianceMinimum, actualError.ErrorType);
            Assert.AreEqual(string.Format(Resources.BelowVarianceMinMessage, "Money"), actualError.Message);
        }

        [TestMethod]
        public void ReturnErrorWhenCostCodeIsInvalid()
        {
            //Arrange
            mockContext
                .Setup(context => context.CostCodeValid(It.IsAny<string>()))
                .Returns(false);
            mockBase = new Mock<CalculationBase<DataClass>>(mockContext.Object) {CallBase = true};

            //Act
            var result = mockBase.Object.ValidateEntry(entry);

            //Assert
            var actual = result.ToArray();
            Assert.AreEqual(1, actual.Length);
            var actualError = actual[0];
            Assert.AreEqual("CostCode", actualError.Column);
            Assert.AreEqual(DataErrorType.InvalidCostCode, actualError.ErrorType);
            Assert.AreEqual(Resources.InvalidCostCodeMessage, actualError.Message);
        }

        public class DataClass : DataEntry
        {
            public StringSplitOptions? Options { get; set; }
            public string StringValue { get; set; }
        }
    }
}
