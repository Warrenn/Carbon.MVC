using System;
using System.Data.Entity.Hierarchy;
using System.Linq;
using CarbonKnown.DAL;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Source;
using CarbonKnown.MVC.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CarbonKnown.MVC.Tests.DAL
{
    [TestClass]
    public class DataContextUnitTest
    {
        private const string SearchCostCentre = "000000s";
        private const string CostCentre1 = "0000001";
        private const string CostCentre2 = "0000002";
        private const string CostCentre3 = "0000003";
        private readonly Guid searchActivityId = Guid.NewGuid();
        private readonly Guid activityId1 = Guid.NewGuid();
        private readonly Guid activityId2 = Guid.NewGuid();
        private readonly Guid activityId3 = Guid.NewGuid();
        private readonly Guid sourceEntryId1 = Guid.NewGuid();
        private readonly Guid sourceEntryId2 = Guid.NewGuid();
        private readonly Guid sourceEntryId3 = Guid.NewGuid();
        private readonly DateTime today = DateTime.Today;

        public Mock<DataContext> CreateContext()
        {
            var mockDbContext = new Mock<DataContext> {CallBase = true};
            mockDbContext
                .SetupGet(c => c.DataSources)
                .Returns(new FakeDbSet<DataSource>(new[]
                {
                    new DataSource
                    {
                        Id = sourceEntryId1
                    },
                    new DataSource
                    {
                        Id = sourceEntryId2
                    },
                    new DataSource
                    {
                        Id = sourceEntryId3
                    }
                }));
            mockDbContext
                .Setup(c => c.Set<FileDataSource>())
                .Returns(new FakeDbSet<FileDataSource>(new[]
                {
                    new FileDataSource
                    {
                        Id = sourceEntryId1
                    },
                    new FileDataSource
                    {
                        Id = sourceEntryId2
                    }
                }));
            mockDbContext
                .Setup(c => c.Set<ManualDataSource>())
                .Returns(new FakeDbSet<ManualDataSource>(new[]
                {
                    new ManualDataSource
                    {
                        Id = sourceEntryId3
                    }
                }));
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
                        Id = activityId3,
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
                .SetupGet(c => c.Currencies)
                .Returns(new FakeDbSet<Currency>(new[]
                {
                    new Currency
                    {
                        Code = "A",
                        Locale = "A locale",
                        Name = "A name",
                        Symbol = "A symbol"
                    },
                    new Currency
                    {
                        Code = "B",
                        Locale = "B locale",
                        Name = "B name",
                        Symbol = "B symbol"
                    },
                    new Currency
                    {
                        Code = "C",
                        Locale = "C locale",
                        Name = "C name",
                        Symbol = "C symbol"
                    },
                    new Currency
                    {
                        Code = "D",
                        Locale = "D locale",
                        Name = "D name",
                        Symbol = "D symbol"
                    }
                }));
            mockDbContext
                .SetupGet(c => c.CostCentres)
                .Returns(new FakeDbSet<CostCentre>(new[]
                {
                    new CostCentre
                    {
                        CostCode = CostCentre1,
                        ParentCostCentreCostCode = null,
                        Node = new HierarchyId("/"),
                        CurrencyCode = "D"
                    },
                    new CostCentre
                    {
                        CostCode = SearchCostCentre,
                        ParentCostCentreCostCode = CostCentre1,
                        Node = new HierarchyId("/1/"),
                        CurrencyCode = "D"
                    },
                    new CostCentre
                    {
                        CostCode = CostCentre3,
                        ParentCostCentreCostCode = SearchCostCentre,
                        Node = new HierarchyId("/1/1/"),
                        CurrencyCode = "A"
                    },
                    new CostCentre
                    {
                        CostCode = CostCentre2,
                        ParentCostCentreCostCode = SearchCostCentre,
                        Node = new HierarchyId("/1/2/"),
                        CurrencyCode = "B"
                    },
                    new CostCentre
                    {
                        CostCode = Guid.NewGuid().ToString(),
                        ParentCostCentreCostCode = CostCentre2,
                        Node = new HierarchyId("/1/2/1/"),
                        CurrencyCode = "B"
                    },
                    new CostCentre
                    {
                        CostCode = Guid.NewGuid().ToString(),
                        ParentCostCentreCostCode = CostCentre1,
                        Node = new HierarchyId("/2/"),
                        CurrencyCode = "C"
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
                        Money = 1,
                        CarbonEmissions = 1,
                        SourceEntry = new DataEntry
                        {
                            Id = Guid.NewGuid(),
                            SourceId = Guid.NewGuid()
                        }
                    },
                    new CarbonEmissionEntry
                    {
                        ActivityGroupNode = new HierarchyId("/1/"),
                        CostCentreNode = new HierarchyId("/1/"),
                        EntryDate = today,
                        Units = 2,
                        Money = 2,
                        CarbonEmissions = 2,
                        SourceEntry = new DataEntry
                        {
                            Id = Guid.NewGuid(),
                            SourceId = Guid.NewGuid()
                        }
                    },
                    new CarbonEmissionEntry
                    {
                        ActivityGroupNode = new HierarchyId("/1/1/"),
                        CostCentreNode = new HierarchyId("/1/1/"),
                        EntryDate = today,
                        Units = 4,
                        Money = 4,
                        CarbonEmissions = 4,
                        SourceEntry = new DataEntry
                        {
                            Id = Guid.NewGuid(),
                            SourceId = sourceEntryId1
                        }
                    },
                    new CarbonEmissionEntry
                    {
                        ActivityGroupNode = new HierarchyId("/1/2/"),
                        CostCentreNode = new HierarchyId("/1/2/"),
                        EntryDate = today,
                        Units = 8,
                        Money = 8,
                        CarbonEmissions = 8,
                        SourceEntry = new DataEntry
                        {
                            Id = Guid.NewGuid(),
                            SourceId = sourceEntryId2
                        }
                    },
                    new CarbonEmissionEntry
                    {
                        ActivityGroupNode = new HierarchyId("/1/2/1/"),
                        CostCentreNode = new HierarchyId("/1/2/1/"),
                        EntryDate = today,
                        Units = 10,
                        Money = 10,
                        CarbonEmissions = 10,
                        SourceEntry = new DataEntry
                        {
                            Id = Guid.NewGuid(),
                            SourceId = sourceEntryId3
                        }
                    },
                    new CarbonEmissionEntry
                    {
                        ActivityGroupNode = new HierarchyId("/2/"),
                        CostCentreNode = new HierarchyId("/1/2/1/"),
                        EntryDate = today,
                        Units = 20,
                        Money = 20,
                        CarbonEmissions = 20,
                        SourceEntry = new DataEntry
                        {
                            Id = Guid.NewGuid(),
                            SourceId = Guid.NewGuid()
                        }
                    },
                    new CarbonEmissionEntry
                    {
                        ActivityGroupNode = new HierarchyId("/1/2/1/"),
                        CostCentreNode = new HierarchyId("/2/"),
                        EntryDate = today,
                        Units = 40,
                        Money = 40,
                        CarbonEmissions = 40,
                        SourceEntry = new DataEntry
                        {
                            Id = Guid.NewGuid(),
                            SourceId = Guid.NewGuid()
                        }
                    },
                    new CarbonEmissionEntry
                    {
                        ActivityGroupNode = new HierarchyId("/2/"),
                        CostCentreNode = new HierarchyId("/2/"),
                        EntryDate = today,
                        Units = 80,
                        Money = 80,
                        CarbonEmissions = 80,
                        SourceEntry = new DataEntry
                        {
                            Id = Guid.NewGuid(),
                            SourceId = Guid.NewGuid()
                        }
                    },
                    new CarbonEmissionEntry
                    {
                        ActivityGroupNode = new HierarchyId("/1/2/"),
                        CostCentreNode = new HierarchyId("/1/2/"),
                        EntryDate = today.AddMonths(3),
                        Units = 100,
                        Money = 100,
                        CarbonEmissions = 100,
                        SourceEntry = new DataEntry
                        {
                            Id = Guid.NewGuid(),
                            SourceId = Guid.NewGuid()
                        }
                    },
                    new CarbonEmissionEntry
                    {
                        ActivityGroupNode = new HierarchyId("/1/2/1/"),
                        CostCentreNode = new HierarchyId("/1/2/1/"),
                        EntryDate = today.AddMonths(2),
                        Units = 200,
                        Money = 200,
                        CarbonEmissions = 200,
                        SourceEntry = new DataEntry
                        {
                            Id = Guid.NewGuid(),
                            SourceId = Guid.NewGuid()
                        }
                    }
                }));
            return mockDbContext;
        }

        [TestMethod]
        public void TotalsByCostCentreMustGetTotalFromNodesInTreeWalkOnly()
        {
            //Arrange
            var ctx = CreateContext().Object;
            var sut = new SummaryDataContext(ctx);

            //Act
            var totals = sut.TotalsByCostCentre(today, today, searchActivityId, SearchCostCentre);
            var array = totals.ToArray();

            //Assert
            Assert.AreEqual(2, array.Length);
            Assert.AreEqual(CostCentre3, array[0].CostCode);
            Assert.AreEqual(CostCentre2, array[1].CostCode);
            Assert.AreEqual(4, array[0].TotalCarbonEmissions);
            Assert.AreEqual(18, array[1].TotalCarbonEmissions);
            Assert.AreEqual(4, array[0].TotalUnits);
            Assert.AreEqual(18, array[1].TotalUnits);
            Assert.AreEqual(searchActivityId, array[0].ActivityGroupId);
            Assert.AreEqual(searchActivityId, array[1].ActivityGroupId);
        }

        [TestMethod]
        public void TotalsByActivityGroupMustGetTotalFromNodesInTreeWalkOnly()
        {
            //Arrange
            var ctx = CreateContext().Object;
            var sut = new SummaryDataContext(ctx);

            //Act
            var totals = sut.TotalsByActivityGroup(today, today, searchActivityId, SearchCostCentre);
            var array = totals.ToArray();

            //Assert
            Assert.AreEqual(2, array.Length);
            Assert.AreEqual(activityId3, array[0].ActivityGroupId);
            Assert.AreEqual(activityId2, array[1].ActivityGroupId);
            Assert.AreEqual(4, array[0].TotalCarbonEmissions);
            Assert.AreEqual(18, array[1].TotalCarbonEmissions);
            Assert.AreEqual(4, array[0].TotalUnits);
            Assert.AreEqual(18, array[1].TotalUnits);
            Assert.AreEqual(SearchCostCentre, array[0].CostCode);
            Assert.AreEqual(SearchCostCentre, array[1].CostCode);
        }

        [TestMethod]
        public void AverageUnitsMustUseChildNodes()
        {
            //Arrange
            var ctx = CreateContext().Object;
            var sut = new SummaryDataContext(ctx);

            //Act
            var totals = sut.AverageUnits(today, today.AddMonths(3), searchActivityId, SearchCostCentre);
            var array = totals.ToArray();

            //Assert
            Assert.AreEqual(array.Length, 3);
            Assert.AreEqual(((decimal) 22/3), array[0].Average);
            Assert.AreEqual(100, array[1].Average);
            Assert.AreEqual(200, array[2].Average);
        }


        [TestMethod]
        public void AverageUnitsMustGroupByYearMonth()
        {
            //Arrange
            var ctx = CreateContext().Object;
            var sut = new SummaryDataContext(ctx);

            //Act
            var totals = sut.AverageUnits(today, today.AddMonths(3), searchActivityId, SearchCostCentre);
            var array = totals.ToArray();

            //Assert
            Assert.AreEqual(array.Length, 3);
            var yearDate = (today.Year*100) + (today.Month);
            Assert.AreEqual(yearDate, array[0].YearMonth);
            var nextMonth = today.AddMonths(3);
            yearDate = (nextMonth.Year*100) + (nextMonth.Month);
            Assert.AreEqual(yearDate, array[1].YearMonth);
            nextMonth = today.AddMonths(2);
            yearDate = (nextMonth.Year*100) + (nextMonth.Month);
            Assert.AreEqual(yearDate, array[2].YearMonth);
        }

        [TestMethod]
        public void AverageMoneyMustUseChildNodes()
        {
            //Arrange
            var ctx = CreateContext().Object;
            var sut = new SummaryDataContext(ctx);

            //Act
            var totals = sut.AverageMoney(today, today.AddMonths(3), searchActivityId, SearchCostCentre);
            var array = totals.ToArray();

            //Assert
            Assert.AreEqual(array.Length, 3);
            Assert.AreEqual(((decimal) 22/3), array[0].Average);
            Assert.AreEqual(100, array[1].Average);
            Assert.AreEqual(200, array[2].Average);
        }

        [TestMethod]
        public void AverageMoneyMustGroupByYearMonth()
        {
            //Arrange
            var ctx = CreateContext().Object;
            var sut = new SummaryDataContext(ctx);

            //Act
            var totals = sut.AverageMoney(today, today.AddMonths(3), searchActivityId, SearchCostCentre);
            var array = totals.ToArray();

            //Assert
            Assert.AreEqual(array.Length, 3);
            var yearDate = (today.Year*100) + (today.Month);
            Assert.AreEqual(yearDate, array[0].YearMonth);
            var nextMonth = today.AddMonths(3);
            yearDate = (nextMonth.Year*100) + (nextMonth.Month);
            Assert.AreEqual(yearDate, array[1].YearMonth);
            nextMonth = today.AddMonths(2);
            yearDate = (nextMonth.Year*100) + (nextMonth.Month);
            Assert.AreEqual(yearDate, array[2].YearMonth);
        }

        [TestMethod]
        public void AverageCo2MustUseChildNodes()
        {
            //Arrange
            var ctx = CreateContext().Object;
            var sut = new SummaryDataContext(ctx);

            //Act
            var totals = sut.AverageCo2(today, today.AddMonths(3), searchActivityId, SearchCostCentre);
            var array = totals.ToArray();

            //Assert
            Assert.AreEqual(array.Length, 3);
            Assert.AreEqual(((decimal) 22/3), array[0].Average);
            Assert.AreEqual(100, array[1].Average);
            Assert.AreEqual(200, array[2].Average);
        }

        [TestMethod]
        public void AverageCo2MustUseGroupByYearMonth()
        {
            //Arrange
            var ctx = CreateContext().Object;
            var sut = new SummaryDataContext(ctx);

            //Act
            var totals = sut.AverageCo2(today, today.AddMonths(3), searchActivityId, SearchCostCentre);
            var array = totals.ToArray();

            //Assert
            Assert.AreEqual(array.Length, 3);
            var yearDate = (today.Year*100) + (today.Month);
            Assert.AreEqual(yearDate, array[0].YearMonth);
            var nextMonth = today.AddMonths(3);
            yearDate = (nextMonth.Year*100) + (nextMonth.Month);
            Assert.AreEqual(yearDate, array[1].YearMonth);
            nextMonth = today.AddMonths(2);
            yearDate = (nextMonth.Year*100) + (nextMonth.Month);
            Assert.AreEqual(yearDate, array[2].YearMonth);
        }


        [TestMethod]
        public void TotalEmissionsMustTotalOnlyChildNodes()
        {
            //Arrange
            var ctx = CreateContext().Object;
            var sut = new SummaryDataContext(ctx);

            //Act
            var total = sut.TotalEmissions(today, today, searchActivityId, SearchCostCentre);

            //Assert
            Assert.AreEqual(22, total);
        }

        [TestMethod]
        public void TotalUnitsMustTotalOnlyChildNodes()
        {
            //Arrange
            var ctx = CreateContext().Object;
            var sut = new SummaryDataContext(ctx);

            //Act
            var total = sut.TotalUnits(today, today, searchActivityId, SearchCostCentre);

            //Assert
            Assert.AreEqual(22, total);
        }

        [TestMethod]
        public void CurrenciesSummaryMustTotalOnlyChildNodes()
        {
            //Arrange
            var ctx = CreateContext().Object;
            var sut = new SummaryDataContext(ctx);

            //Act
            var totals = sut.CurrenciesSummary(today, today, searchActivityId, SearchCostCentre);
            var array = totals.ToArray();

            //Assert
            Assert.AreEqual(2, array.Length);
            Assert.AreEqual(4,array[0].TotalMoney);
            Assert.AreEqual(18,array[1].TotalMoney);
        }

        [TestMethod]
        public void CurrenciesSummaryMustGroupByCurrencyCode()
        {
            //Arrange
            var ctx = CreateContext().Object;
            var sut = new SummaryDataContext(ctx);

            //Act
            var totals = sut.CurrenciesSummary(today, today, searchActivityId, SearchCostCentre);
            var array = totals.ToArray();

            //Assert
            Assert.AreEqual(2, array.Length);
            Assert.AreEqual("A", array[0].Code);
            Assert.AreEqual("A locale", array[0].Locale);
            Assert.AreEqual("A name", array[0].Name);
            Assert.AreEqual("A symbol", array[0].Symbol);
            Assert.AreEqual("B", array[1].Code);
            Assert.AreEqual("B locale", array[1].Locale);
            Assert.AreEqual("B name", array[1].Name);
            Assert.AreEqual("B symbol", array[1].Symbol);
        }

        [TestMethod]
        public void AuditHistoryMustTotalOnlyChildNodes()
        {
            //Arrange
            var ctx = CreateContext().Object;
            var sut = new SummaryDataContext(ctx);

            //Act
            var totals = sut.AuditHistory(today, today, searchActivityId, SearchCostCentre);
            var array = totals.ToArray();

            //Assert
            Assert.AreEqual(3, array.Length);
            Assert.AreEqual(4, array[0].Cost);
            Assert.AreEqual(4, array[0].Units);
            Assert.AreEqual(4, array[0].Emissions);
            Assert.AreEqual(8, array[1].Cost);
            Assert.AreEqual(8, array[1].Units);
            Assert.AreEqual(8, array[1].Emissions);
            Assert.AreEqual(10, array[2].Cost);
            Assert.AreEqual(10, array[2].Units);
            Assert.AreEqual(10, array[2].Emissions);
        }


        [TestMethod]
        public void AuditHistoryMustGroupBySourceId()
        {
            //Arrange
            var ctx = CreateContext().Object;
            var sut = new SummaryDataContext(ctx);

            //Act
            var totals = sut.AuditHistory(today, today, searchActivityId, SearchCostCentre);
            var array = totals.ToArray();

            //Assert
            Assert.AreEqual(3, array.Length);
            Assert.AreEqual(sourceEntryId1, array[0].SourceId);
            Assert.AreEqual(sourceEntryId2, array[1].SourceId);
            Assert.AreEqual(sourceEntryId3, array[2].SourceId);
        }

    }
}
