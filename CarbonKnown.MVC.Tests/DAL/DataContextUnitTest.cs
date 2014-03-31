using System;
using System.Data.Entity.Hierarchy;
using System.Linq;
using CarbonKnown.DAL;
using CarbonKnown.DAL.Models;
using CarbonKnown.MVC.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CarbonKnown.MVC.Tests.DAL
{
    [TestClass]
    public class DataContextUnitTest
    {
        [TestMethod]
        public void TotalsByCostCentreMustGetTotalFromNodesInTreeWalkOnly()
        {
            //Arrange
            var today = DateTime.Today;
            var searchActivityId = Guid.NewGuid();
            var activityId1 = Guid.NewGuid();
            var activityId2 = Guid.NewGuid();
            var searchCostCentre = "000000s";
            var costCentre1 = "0000001";
            var costCentre2 = "0000002";
            var costCentre3 = "0000003";
            var mockDbContext = new Mock<DataContext> {CallBase = true};
            mockDbContext
                .SetupGet(c => c.ActivityGroups)
                .Returns(new FakeDbSet<ActivityGroup>(new[]
                {
                    new ActivityGroup
                    {
                        Id = activityId1,
                        ParentGroupId = null,
                        Node = new HierarchyId("/")
                    },
                    new ActivityGroup
                    {
                        Id = searchActivityId,
                        ParentGroupId = activityId1,
                        Node = new HierarchyId("/1/")
                    },
                    new ActivityGroup
                    {
                        Id = Guid.NewGuid(),
                        ParentGroupId = searchActivityId,
                        Node = new HierarchyId("/1/1/")
                    },
                    new ActivityGroup
                    {
                        Id = activityId2,
                        ParentGroupId = searchActivityId,
                        Node = new HierarchyId("/1/2/")
                    },
                    new ActivityGroup
                    {
                        Id = Guid.NewGuid(),
                        ParentGroupId = activityId2,
                        Node = new HierarchyId("/1/2/1/")
                    },
                    new ActivityGroup
                    {
                        Id = Guid.NewGuid(),
                        ParentGroupId = activityId1,
                        Node = new HierarchyId("/2/")
                    }
                }));
            mockDbContext
                .SetupGet(c => c.CostCentres)
                .Returns(new FakeDbSet<CostCentre>(new[]
                {
                    new CostCentre
                    {
                        CostCode = costCentre1,
                        ParentCostCentreCostCode = null,
                        Node = new HierarchyId("/")
                    },
                    new CostCentre
                    {
                        CostCode = searchCostCentre,
                        ParentCostCentreCostCode = costCentre1,
                        Node = new HierarchyId("/1/")
                    },
                    new CostCentre
                    {
                        CostCode = costCentre3,
                        ParentCostCentreCostCode = searchCostCentre,
                        Node = new HierarchyId("/1/1/")
                    },
                    new CostCentre
                    {
                        CostCode = costCentre2,
                        ParentCostCentreCostCode = searchCostCentre,
                        Node = new HierarchyId("/1/2/")
                    },
                    new CostCentre
                    {
                        CostCode = Guid.NewGuid().ToString(),
                        ParentCostCentreCostCode = costCentre2,
                        Node = new HierarchyId("/1/2/1/")
                    },
                    new CostCentre
                    {
                        CostCode = Guid.NewGuid().ToString(),
                        ParentCostCentreCostCode = costCentre1,
                        Node = new HierarchyId("/2/")
                    }
                }));
            mockDbContext
                .SetupGet(c => c.CarbonEmissionEntries)
                .Returns(new FakeDbSet<CarbonEmissionEntry>(new[]
                {
                    new CarbonEmissionEntry
                    {
                        ActivityGroupNode = new HierarchyId("/"),
                        CostCentreNode = new HierarchyId("/"),
                        EntryDate = today,
                        Units = 1,
                        CarbonEmissions = 1
                    },
                    new CarbonEmissionEntry
                    {
                        ActivityGroupNode = new HierarchyId("/1/"),
                        CostCentreNode = new HierarchyId("/1/"),
                        EntryDate = today,
                        Units = 2,
                        CarbonEmissions = 2
                    },
                    new CarbonEmissionEntry
                    {
                        ActivityGroupNode = new HierarchyId("/1/1/"),
                        CostCentreNode = new HierarchyId("/1/1/"),
                        EntryDate = today,
                        Units = 4,
                        CarbonEmissions = 4
                    },
                    new CarbonEmissionEntry
                    {
                        ActivityGroupNode = new HierarchyId("/1/2/"),
                        CostCentreNode = new HierarchyId("/1/2/"),
                        EntryDate = today,
                        Units = 8,
                        CarbonEmissions = 8
                    },
                    new CarbonEmissionEntry
                    {
                        ActivityGroupNode = new HierarchyId("/1/2/1/"),
                        CostCentreNode = new HierarchyId("/1/2/1/"),
                        EntryDate = today,
                        Units = 10,
                        CarbonEmissions = 10
                    },
                    new CarbonEmissionEntry
                    {
                        ActivityGroupNode = new HierarchyId("/2/"),
                        CostCentreNode = new HierarchyId("/1/2/1/"),
                        EntryDate = today,
                        Units = 20,
                        CarbonEmissions = 20
                    },
                    new CarbonEmissionEntry
                    {
                        ActivityGroupNode = new HierarchyId("/1/2/1/"),
                        CostCentreNode = new HierarchyId("/2/"),
                        EntryDate = today,
                        Units = 40,
                        CarbonEmissions = 40
                    },
                    new CarbonEmissionEntry
                    {
                        ActivityGroupNode = new HierarchyId("/2/"),
                        CostCentreNode = new HierarchyId("/2/"),
                        EntryDate = today,
                        Units = 80,
                        CarbonEmissions = 80
                    }
                }));
            var ctx = mockDbContext.Object;
            var sut = new Mock<SummaryDataContext>(ctx).Object;

            //Act
            var totals = sut.TotalsByCostCentre(today, today, searchActivityId, searchCostCentre);
            var array = totals.ToArray();

            //Assert
            Assert.AreEqual(2, array.Length);
        }

        [TestMethod]
        public void TotalsByCostCentreMustGetTotalFromNodesInActivityTreeWalkOnly()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void TotalsByCostCentreMustIncludeParentNode()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void TotalsByActivityGroupMustGetTotalFromNodesInCostCentreTreeWalkOnly()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void TotalsByActivityGroupMustGetTotalFromNodesInActivityTreeWalkOnly()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void TotalsByActivityGroupeMustIncludeParentNode()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void AverageUnitsMustUseChildNodes()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void AverageUnitsMustGroupByYearMonth()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void AverageMoneyMustUseChildNodes()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void AverageMoneyMustGroupByYearMonth()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void AverageCo2MustUseChildNodes()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void AverageCo2MustGroupByYearMonth()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void TotalEmissionsMustTotalOnlyChildNodes()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void TotalUnitsMustTotalOnlyChildNodes()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void CurrenciesSummaryMustTotalOnlyChildNodes()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void CurrenciesSummaryMustGroupByCurrencyCode()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void AuditHistoryMustTotalOnlyChildNodes()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void AuditHistoryMustIncludeAllRelevantSources()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void AuditHistoryMustGroupBySourceId()
        {
            Assert.Inconclusive();
        }

    }
}
