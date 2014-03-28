using System;
using CarbonKnown.DAL.Models.Constants;
using CarbonKnown.MVC.Controllers;
using CarbonKnown.MVC.Models;
using CarbonKnown.Print;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CarbonKnown.MVC.Tests.Controllers
{
    //todo: needs refactoring
    //[TestClass]
    //public class DashboardControllerUnitTest
    //{
    //    [TestMethod]
    //    public void IndexMustReturnDefaultModelWhenRequestIsNull()
    //    {
    //        //Arrange
    //        var today = DateTime.Today;
    //        var startDate = new DateTime(today.Year, today.Month, 1);
    //        var endDate = startDate.AddMonths(1).AddDays(-1);
    //        var mockCostCentreController = new Mock<CostCentreService>(null);
    //        var mockFactorGroupController = new Mock<FactorGroupService>(null);
    //        var mockSummaryController = new Mock<SummaryService>(null);
    //        var sut = new DashboardController(
    //            mockCostCentreController.Object,
    //            mockFactorGroupController.Object,
    //            mockSummaryController.Object);

    //        //Act
    //        var result = sut.Index(null) as ViewResult;

    //        //Assert
    //        Assert.IsNotNull(result);
    //        var model = result.Model as DashboardRequest;
    //        Assert.IsNotNull(model);
    //        Assert.AreEqual(Settings.Default.RootCostCentre, model.CostCode);
    //        Assert.AreEqual(Dimension.FactorGroup, model.Dimension);
    //        Assert.AreEqual(startDate, model.StartDate);
    //        Assert.AreEqual(endDate, model.EndDate);
    //        Assert.AreEqual(Activity.OverviewId, model.ActivityGroupId);
    //        Assert.AreEqual(Section.Overview, model.Section);
    //    }

    //    [TestMethod]
    //    public void IndexMustReturnGivenDashboardRequestWhenProvided()
    //    {
    //        //Arrange
    //        var startDate = new DateTime();
    //        var endDate = startDate.AddDays(1);
    //        var groupId = Guid.NewGuid();
    //        var request = new DashboardRequest
    //            {
    //                ActivityGroupId = groupId,
    //                CostCode = "CostCode",
    //                Dimension = "Dimension",
    //                Section = "Section",
    //                StartDate = startDate,
    //                EndDate = endDate
    //            };
    //        var mockCostCentreController = new Mock<CostCentreService>(null);
    //        var mockFactorGroupController = new Mock<FactorGroupService>(null);
    //        var mockSummaryController = new Mock<SummaryService>(null);
    //        var sut = new DashboardController(
    //            mockCostCentreController.Object,
    //            mockFactorGroupController.Object,
    //            mockSummaryController.Object);

    //        //Act
    //        var result = sut.Index(request) as ViewResult;

    //        //Assert
    //        Assert.IsNotNull(result);
    //        var model = result.Model as DashboardRequest;
    //        Assert.IsNotNull(model);
    //        Assert.AreEqual("CostCode", model.CostCode);
    //        Assert.AreEqual("Dimension", model.Dimension);
    //        Assert.AreEqual(startDate, model.StartDate);
    //        Assert.AreEqual(endDate, model.EndDate);
    //        Assert.AreEqual(groupId, model.ActivityGroupId);
    //        Assert.AreEqual("Section", model.Section);
    //    }

    //    [TestMethod]
    //    public void OverviewRequestMustCallOverviewControllerAction()
    //    {
    //        //Arrange
    //        var request = new DashboardRequest
    //        {
    //            Dimension = "costcentre",
    //            Section = "overview"
    //        };
    //        var mockCostCentreController = new Mock<CostCentreService>(null);
    //        var mockFactorGroupController = new Mock<FactorGroupService>(null);
    //        var mockSummaryController = new Mock<SummaryService>(null);

    //        var sut = new DashboardController(
    //            mockCostCentreController.Object,
    //            mockFactorGroupController.Object,
    //            mockSummaryController.Object);

    //        //Act
    //        var result = sut.PrintJpg(request, string.Empty) as PrintResult;

    //        //Assert
    //        Assert.IsNotNull(result);
    //        var model = result.Model as PrintModel;
    //        Assert.IsNotNull(model);
    //        mockCostCentreController
    //            .Verify(controller => controller.Overview(It.IsAny<DashboardRequest>()));
    //        mockSummaryController
    //            .Verify(controller => controller.Overview(It.IsAny<DashboardRequest>()));
    //    }

    //    [TestMethod]
    //    public void WaterRequestMustCallWaterControllerAction()
    //    {
    //        //Arrange
    //        var request = new DashboardRequest
    //        {
    //            Dimension = "costcentre",
    //            Section = "water"
    //        };
    //        var mockCostCentreController = new Mock<CostCentreService>(null);
    //        var mockFactorGroupController = new Mock<FactorGroupService>(null);
    //        var mockSummaryController = new Mock<SummaryService>(null);

    //        var sut = new DashboardController(
    //            mockCostCentreController.Object,
    //            mockFactorGroupController.Object,
    //            mockSummaryController.Object);

    //        //Act
    //        var result = sut.PrintJpg(request, string.Empty) as PrintResult;

    //        //Assert
    //        Assert.IsNotNull(result);
    //        var model = result.Model as PrintModel;
    //        Assert.IsNotNull(model);
    //        mockCostCentreController
    //            .Verify(controller => controller.Water(It.IsAny<DashboardRequest>()));
    //        mockSummaryController
    //            .Verify(controller => controller.Water(It.IsAny<DashboardRequest>()));
    //    }

    //    [TestMethod]
    //    public void ElectricityRequestMustCallElectricityControllerAction()
    //    {
    //        //Arrange
    //        var request = new DashboardRequest
    //        {
    //            Dimension = "costcentre",
    //            Section = "electricity"
    //        };
    //        var mockCostCentreController = new Mock<CostCentreService>(null);
    //        var mockFactorGroupController = new Mock<FactorGroupService>(null);
    //        var mockSummaryController = new Mock<SummaryService>(null);

    //        var sut = new DashboardController(
    //            mockCostCentreController.Object,
    //            mockFactorGroupController.Object,
    //            mockSummaryController.Object);

    //        //Act
    //        var result = sut.PrintJpg(request, string.Empty) as PrintResult;

    //        //Assert
    //        Assert.IsNotNull(result);
    //        var model = result.Model as PrintModel;
    //        Assert.IsNotNull(model);
    //        mockCostCentreController
    //            .Verify(controller => controller.Electricity(It.IsAny<DashboardRequest>()));
    //        mockSummaryController
    //            .Verify(controller => controller.Electricity(It.IsAny<DashboardRequest>()));
    //    }

    //    [TestMethod]
    //    public void PaperRequestMustCallPaperControllerAction()
    //    {
    //        //Arrange
    //        var request = new DashboardRequest
    //        {
    //            Dimension = "costcentre",
    //            Section = "paper"
    //        };
    //        var mockCostCentreController = new Mock<CostCentreService>(null);
    //        var mockFactorGroupController = new Mock<FactorGroupService>(null);
    //        var mockSummaryController = new Mock<SummaryService>(null);

    //        var sut = new DashboardController(
    //            mockCostCentreController.Object,
    //            mockFactorGroupController.Object,
    //            mockSummaryController.Object);

    //        //Act
    //        var result = sut.PrintJpg(request, string.Empty) as PrintResult;

    //        //Assert
    //        Assert.IsNotNull(result);
    //        var model = result.Model as PrintModel;
    //        Assert.IsNotNull(model);
    //        mockCostCentreController
    //            .Verify(controller => controller.Paper(It.IsAny<DashboardRequest>()));
    //        mockSummaryController
    //            .Verify(controller => controller.Paper(It.IsAny<DashboardRequest>()));
    //    }

    //    [TestMethod]
    //    public void WasteRequestMustCallWasteControllerAction()
    //    {
    //        //Arrange
    //        var request = new DashboardRequest
    //        {
    //            Dimension = "costcentre",
    //            Section = "waste"
    //        };
    //        var mockCostCentreController = new Mock<CostCentreService>(null);
    //        var mockFactorGroupController = new Mock<FactorGroupService>(null);
    //        var mockSummaryController = new Mock<SummaryService>(null);

    //        var sut = new DashboardController(
    //            mockCostCentreController.Object,
    //            mockFactorGroupController.Object,
    //            mockSummaryController.Object);

    //        //Act
    //        var result = sut.PrintJpg(request, string.Empty) as PrintResult;

    //        //Assert
    //        Assert.IsNotNull(result);
    //        var model = result.Model as PrintModel;
    //        Assert.IsNotNull(model);
    //        mockCostCentreController
    //            .Verify(controller => controller.Waste(It.IsAny<DashboardRequest>()));
    //        mockSummaryController
    //            .Verify(controller => controller.Waste(It.IsAny<DashboardRequest>()));
    //    }

    //    [TestMethod]
    //    public void TravelRequestMustCallTravelControllerAction()
    //    {
    //        //Arrange
    //        var request = new DashboardRequest
    //        {
    //            Dimension = "costcentre",
    //            Section = "travel"
    //        };
    //        var mockCostCentreController = new Mock<CostCentreService>(null);
    //        var mockFactorGroupController = new Mock<FactorGroupService>(null);
    //        var mockSummaryController = new Mock<SummaryService>(null);

    //        var sut = new DashboardController(
    //            mockCostCentreController.Object,
    //            mockFactorGroupController.Object,
    //            mockSummaryController.Object);

    //        //Act
    //        var result = sut.PrintJpg(request, string.Empty) as PrintResult;

    //        //Assert
    //        Assert.IsNotNull(result);
    //        var model = result.Model as PrintModel;
    //        Assert.IsNotNull(model);
    //        mockCostCentreController
    //            .Verify(controller => controller.Travel(It.IsAny<DashboardRequest>()));
    //        mockSummaryController
    //            .Verify(controller => controller.Travel(It.IsAny<DashboardRequest>()));
    //    }

    //    [TestMethod]
    //    public void FleetRequestMustCallFleetControllerAction()
    //    {
    //        //Arrange
    //        var request = new DashboardRequest
    //        {
    //            Dimension = "costcentre",
    //            Section = "fleet"
    //        };
    //        var mockCostCentreController = new Mock<CostCentreService>(null);
    //        var mockFactorGroupController = new Mock<FactorGroupService>(null);
    //        var mockSummaryController = new Mock<SummaryService>(null);

    //        var sut = new DashboardController(
    //            mockCostCentreController.Object,
    //            mockFactorGroupController.Object,
    //            mockSummaryController.Object);

    //        //Act
    //        var result = sut.PrintJpg(request, string.Empty) as PrintResult;

    //        //Assert
    //        Assert.IsNotNull(result);
    //        var model = result.Model as PrintModel;
    //        Assert.IsNotNull(model);
    //        mockCostCentreController
    //            .Verify(controller => controller.Fleet(It.IsAny<DashboardRequest>()));
    //        mockSummaryController
    //            .Verify(controller => controller.Fleet(It.IsAny<DashboardRequest>()));
    //    }

    //    [TestMethod]
    //    public void CourierRequestMustCallCourierControllerAction()
    //    {
    //        //Arrange
    //        var request = new DashboardRequest
    //        {
    //            Dimension = "costcentre",
    //            Section = "courier"
    //        };
    //        var mockCostCentreController = new Mock<CostCentreService>(null);
    //        var mockFactorGroupController = new Mock<FactorGroupService>(null);
    //        var mockSummaryController = new Mock<SummaryService>(null);
    //        mockCostCentreController
    //            .Setup(controller => controller.Courier(It.IsAny<DashboardRequest>()))
    //            .Returns(new SliceModel[]{})
    //            .Verifiable();
    //        mockSummaryController
    //            .Setup(controller => controller.Courier(It.IsAny<DashboardRequest>()))
    //            .Returns(new Summary())
    //            .Verifiable();

    //        var sut = new DashboardController(
    //            mockCostCentreController.Object,
    //            mockFactorGroupController.Object,
    //            mockSummaryController.Object);

    //        //Act
    //        var result = sut.PrintJpg(request, string.Empty) as PrintResult;

    //        //Assert
    //        Assert.IsNotNull(result);
    //        var model = result.Model as PrintModel;
    //        Assert.IsNotNull(model);
    //        mockCostCentreController.VerifyAll();
    //        mockSummaryController.VerifyAll();
    //    }

    //    [TestMethod]
    //    public void FactorGroupCallMustUseTheFactorGroupController()
    //    {
    //        //Arrange
    //        var request = new DashboardRequest
    //        {
    //            Dimension = "factorgroup",
    //            Section = "courier"
    //        };
    //        var mockCostCentreController = new Mock<CostCentreService>(null);
    //        var mockFactorGroupController = new Mock<FactorGroupService>(null);
    //        var mockSummaryController = new Mock<SummaryService>(null);

    //        var sut = new DashboardController(
    //            mockCostCentreController.Object,
    //            mockFactorGroupController.Object,
    //            mockSummaryController.Object);

    //        //Act
    //        var result = sut.PrintJpg(request, string.Empty) as PrintResult;

    //        //Assert
    //        Assert.IsNotNull(result);
    //        var model = result.Model as PrintModel;
    //        Assert.IsNotNull(model);
    //        mockFactorGroupController
    //            .Verify(controller => controller.Courier(It.IsAny<DashboardRequest>()));
    //        mockSummaryController
    //            .Verify(controller => controller.Courier(It.IsAny<DashboardRequest>()));
    //    }
    //}
}
