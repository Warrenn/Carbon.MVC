using System;
using System.Collections.Generic;
using CarbonKnown.Calculation.CarHire;
using CarbonKnown.Calculation.DAL;
using CarbonKnown.Calculation.Models;
using CarbonKnown.DAL.Models.CarHire;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CarbonKnown.MVC.Tests.Calculation
{
    [TestClass]
    public class AvisCourierCalculationUnitTest
    {
        [TestMethod]
        public void MustGetTheCorrectFactorIdForEachCarGroupBillType()
        {
            //Arrange
            IDictionary<CarGroupBill, Guid> carGroupBillMapping =
                new SortedDictionary<CarGroupBill, Guid>
                    {
                        {CarGroupBill.A, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.A_Economy_1_4},
                        {CarGroupBill.B, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.B_Compact_1_4_2_l_Petrol},
                        {CarGroupBill.C, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.C_Intermediate_1_6_l_Petrol},
                        {CarGroupBill.D, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.D_Intermediate_1_6_l_Petrol},
                        {CarGroupBill.E, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.E_Standard_2_0_2_4_l_Petrol},
                        {CarGroupBill.F, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.F_Full_Size_1_8_2_l_Petrol},
                        {CarGroupBill.G, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.G_Premium_1_8_2_l_Petrol},
                        {CarGroupBill.H, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.H_Hybrid_HYB},
                        {CarGroupBill.I, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.I_Compact_Petrol},
                        {CarGroupBill.J, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.J_Luxury_2_3_2_5_l_Petrol},
                        {CarGroupBill.K, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.K_Speciality_SUV_2_4_2_5_l_Petrol},
                        {CarGroupBill.L, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.L_Speciality_Leisure_4X4_3_l},
                        {CarGroupBill.M, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.M_Economy_1_1_1_4_Petrol},
                        {CarGroupBill.N, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.N_Speciality_People_Carrier},
                        {CarGroupBill.O, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.O_Full_Size_1_8_2_l_Petrol},
                        {CarGroupBill.P, CarbonKnown.DAL.Models.Constants.Factors.CarGroupBill.P_Full_Size_1_4_l_Petrol},
                    };
            var calculations = new Dictionary<CarGroupBill, Mock<ICalculationDataContext>>();
            DateTime? currentDate = DateTime.Now;
            var dailyData = new DailyData {UnitsPerDay = 1};

            //Act
            foreach (var keyValue in carGroupBillMapping)
            {
                var mockContext = new Mock<ICalculationDataContext>();
                var sut = new CarHireCalculation(mockContext.Object);
                var entry = new CarHireData {CarGroupBill = keyValue.Key};
                mockContext
                    .Setup(context => context.FactorValue(It.IsAny<DateTime>(), It.IsAny<Guid>()))
                    .Returns(1);
                sut.CalculateEmission(currentDate.Value, dailyData, entry);
                calculations.Add(keyValue.Key, mockContext);
            }

            //Assert
            foreach (var keyValue in carGroupBillMapping)
            {
                var mockContext = calculations[keyValue.Key];
                mockContext
                    .Verify(context => context.FactorValue(
                        It.Is<DateTime>(d => d == currentDate),
                        It.Is<Guid>(g => g == keyValue.Value)), Times.Once);
            }
        }

        [TestMethod]
        public void IfTheCarGroupBillIsNullReturnNull()
        {
            //Arrange
            var currentDate = DateTime.Now;
            var dailyData = new DailyData {UnitsPerDay = 1};
            var mockContext = new Mock<ICalculationDataContext>();
            var sut = new CarHireCalculation(mockContext.Object);
            var entry = new CarHireData {CarGroupBill = null};
            mockContext
                .Setup(context => context.FactorValue(It.IsAny<DateTime>(), It.IsAny<Guid>()))
                .Returns(1);

            //Act
            var result = sut.CalculateEmission(currentDate, dailyData, entry);

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void IfTheFactorValueIsNotFoundReturnNull()
        {
            //Arrange
            var currentDate = DateTime.Now;
            var dailyData = new DailyData {UnitsPerDay = 1};
            var mockContext = new Mock<ICalculationDataContext>();
            var sut = new CarHireCalculation(mockContext.Object);
            var entry = new CarHireData {CarGroupBill = CarGroupBill.A};
            mockContext
                .Setup(context => context.FactorValue(It.IsAny<DateTime>(), It.IsAny<Guid>()))
                .Returns((decimal?) null);

            //Act
            var result = sut.CalculateEmission(currentDate, dailyData, entry);

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TheResultIsTheFactorValueTimesTheUnitsPerDay()
        {
            //Arrange
            var currentDate = DateTime.Now;
            var dailyData = new DailyData {UnitsPerDay = 7};
            var mockContext = new Mock<ICalculationDataContext>();
            var sut = new CarHireCalculation(mockContext.Object);
            var entry = new CarHireData {CarGroupBill = CarGroupBill.A};
            mockContext
                .Setup(context => context.FactorValue(It.IsAny<DateTime>(), It.IsAny<Guid>()))
                .Returns(4);

            //Act
            var result = sut.CalculateEmission(currentDate, dailyData, entry);

            //Assert
            Assert.AreEqual(28, result);
        }
    }
}

