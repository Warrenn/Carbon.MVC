using System;
using System.Collections.ObjectModel;
using System.Data.Entity.Hierarchy;
using System.Web.Mvc;
using CarbonKnown.DAL;
using CarbonKnown.DAL.Models;
using CarbonKnown.MVC.Controllers;
using CarbonKnown.MVC.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CarbonKnown.MVC.Tests.Controllers
{
    [TestClass]
    public class CostCentreControllerUnitTest
    {
        private const string CostCentre0 = "0000000";
        private const string CostCentre1 = "0000001";
        private const string CostCentre2 = "0000002";
        private const string CostCentre3 = "0000003";
        private const string CostCentre4 = "0000004";
        private const string CostCentre5 = "0000005";
        private const string CostCentre6 = "0000006";
        private const string CostCentre7 = "0000007";
        private const string CostCentre8 = "0000008";
        private CostCentre[] centres;
        private CarbonEmissionEntry[] entries;
        private Mock<FakeDbSet<CostCentre>> mockCentres;
        private Mock<FakeDbSet<CarbonEmissionEntry>> mockEntries;

        public Mock<DataContext> CreateContext()
        {
            centres = new[]
            {
                new CostCentre
                {
                    CostCode = CostCentre0,
                    ParentCostCentreCostCode = null,
                    Node = new HierarchyId("/"),
                    OrderId = 0
                },
                new CostCentre
                {
                    CostCode = CostCentre1,
                    ParentCostCentreCostCode = CostCentre0,
                    Node = new HierarchyId("/1/"),
                    OrderId = 100
                },
                new CostCentre
                {
                    CostCode = CostCentre2,
                    ParentCostCentreCostCode = CostCentre1,
                    Node = new HierarchyId("/1/1/"),
                    OrderId = 100
                },
                new CostCentre
                {
                    CostCode = CostCentre3,
                    ParentCostCentreCostCode = CostCentre1,
                    Node = new HierarchyId("/1/2/"),
                    OrderId = 200
                },
                new CostCentre
                {
                    CostCode = CostCentre4,
                    ParentCostCentreCostCode = CostCentre3,
                    Node = new HierarchyId("/1/2/1/"),
                    OrderId = 100
                },
                new CostCentre
                {
                    CostCode = CostCentre5,
                    ParentCostCentreCostCode = CostCentre0,
                    Node = new HierarchyId("/2/"),
                    OrderId = 200
                },
                new CostCentre
                {
                    CostCode = CostCentre6,
                    ParentCostCentreCostCode = CostCentre5,
                    Node = new HierarchyId("/2/1/"),
                    OrderId = 100
                },
                new CostCentre
                {
                    CostCode = CostCentre7,
                    ParentCostCentreCostCode = CostCentre0,
                    Node = new HierarchyId("/3/"),
                    OrderId = 300
                },
                new CostCentre
                {
                    CostCode = CostCentre8,
                    ParentCostCentreCostCode = CostCentre0,
                    Node = new HierarchyId("/4/"),
                    OrderId = 400
                }
            };

            entries = new[]
            {
                new CarbonEmissionEntry
                {
                    CostCentreNode = new HierarchyId("/"),
                    SourceEntry = new DataEntry
                    {
                        CostCode = CostCentre0
                    }
                },
                new CarbonEmissionEntry
                {
                    CostCentreNode = new HierarchyId("/1/"),
                    SourceEntry = new DataEntry
                    {
                        CostCode = CostCentre1
                    }
                },
                new CarbonEmissionEntry
                {
                    CostCentreNode = new HierarchyId("/1/1/"),
                    SourceEntry = new DataEntry
                    {
                        CostCode = CostCentre2
                    }
                },
                new CarbonEmissionEntry
                {
                    CostCentreNode = new HierarchyId("/1/2/"),
                    SourceEntry = new DataEntry
                    {
                        CostCode = CostCentre3
                    }
                },
                new CarbonEmissionEntry
                {
                    CostCentreNode = new HierarchyId("/1/2/1/"),
                    SourceEntry = new DataEntry
                    {
                        CostCode = CostCentre4
                    }
                },
                new CarbonEmissionEntry
                {
                    CostCentreNode = new HierarchyId("/1/2/1/"),
                    SourceEntry = new DataEntry
                    {
                        CostCode = CostCentre4
                    }
                },
                new CarbonEmissionEntry
                {
                    CostCentreNode = new HierarchyId("/2/"),
                    SourceEntry = new DataEntry
                    {
                        CostCode = CostCentre5
                    }
                },
                new CarbonEmissionEntry
                {
                    CostCentreNode = new HierarchyId("/1/2/"),
                    SourceEntry = new DataEntry
                    {
                        CostCode = CostCentre3
                    }
                },
                new CarbonEmissionEntry
                {
                    CostCentreNode = new HierarchyId("/1/2/1/"),
                    SourceEntry = new DataEntry
                    {
                        CostCode = CostCentre4
                    }
                }
            };

            mockCentres = new Mock<FakeDbSet<CostCentre>>(new object[] { centres }) { CallBase = true };
            mockEntries = new Mock<FakeDbSet<CarbonEmissionEntry>>(new object[] { entries }) { CallBase = true };
            mockCentres
                .Setup(set => set.Find(It.IsAny<object[]>()))
                .Returns(() => null);

            var mockDbContext = new Mock<DataContext> {CallBase = true};
            return mockDbContext;
        }

        [TestMethod]
        public void NewCostCentreMustUseParentHeirarchyInNode()
        {
            //Arrange
            var ctx = CreateContext();
            mockCentres
                .Setup(set => set.Find(It.Is<object[]>(objects => (string) objects[0] == CostCentre1)))
                .Returns(() => centres[1]);
            mockCentres
                .Setup(set => set.Create())
                .Returns(new CostCentre())
                .Verifiable();
            mockCentres
                .Setup(set => set.Add(It.Is<CostCentre>(c => c.Node == new HierarchyId("/1/3/"))))
                .Verifiable();
            ctx
                .SetupGet(c => c.CostCentres)
                .Returns(mockCentres.Object);
            var sut = new CostCentreController(ctx.Object);
            var newCentre = new CostCentreModel
            {
                costCode = "new",
                parentCostCode = CostCentre1,
                name = "name",
                color = "FFFFFF",
                currencyCode = new Select2Model()
            };

            //Act
            sut.UpsertCostCentre(newCentre);

            //Assert
            mockCentres.Verify();
        }

        [TestMethod]
        public void ReparentingReparentsAllEmissionEntriesThatMatchNodeAndDescendants()
        {
            //Arrange
            var ctx = CreateContext();
            centres[1].ParentCostCentre = centres[0];
            mockCentres
                .Setup(set => set.Find(It.Is<object[]>(objects => (string)objects[0] == CostCentre1)))
                .Returns(() => centres[1]);
            mockCentres
                .Setup(set => set.Find(It.Is<object[]>(objects => (string)objects[0] == CostCentre5)))
                .Returns(() => centres[5]);

            ctx
                .SetupGet(c => c.CostCentres)
                .Returns(mockCentres.Object);
            ctx
                .SetupGet(c => c.CarbonEmissionEntries)
                .Returns(mockEntries.Object);
            var sut = new CostCentreController(ctx.Object);

            //Act
            sut.ReParent(CostCentre1, CostCentre5);

            //Assert
            Assert.AreEqual(new HierarchyId("/2/2/"), entries[1].CostCentreNode);
            Assert.AreEqual(new HierarchyId("/2/2/1/"), entries[2].CostCentreNode);
            Assert.AreEqual(new HierarchyId("/2/2/2/"), entries[3].CostCentreNode);
            Assert.AreEqual(new HierarchyId("/2/2/2/1/"), entries[4].CostCentreNode);
            Assert.AreEqual(new HierarchyId("/2/2/2/1/"), entries[5].CostCentreNode);
            Assert.AreEqual(new HierarchyId("/2/2/2/"), entries[7].CostCentreNode);
            Assert.AreEqual(new HierarchyId("/2/2/2/1/"), entries[8].CostCentreNode);
        }

        [TestMethod]
        public void ReparentingCostCentreMustUpdateChildren()
        {
            //Arrange
            var ctx = CreateContext();
            centres[1].ParentCostCentre = centres[0];
            mockCentres
                .Setup(set => set.Find(It.Is<object[]>(objects => (string)objects[0] == CostCentre1)))
                .Returns(() => centres[1]);
            mockCentres
                .Setup(set => set.Find(It.Is<object[]>(objects => (string) objects[0] == CostCentre5)))
                .Returns(() => centres[5]);

            ctx
                .SetupGet(c => c.CostCentres)
                .Returns(mockCentres.Object);
            ctx
                .SetupGet(c => c.CarbonEmissionEntries)
                .Returns(mockEntries.Object);
            var sut = new CostCentreController(ctx.Object);

            //Act
            sut.ReParent(CostCentre1, CostCentre5);

            //Assert
            Assert.AreEqual(new HierarchyId("/2/2/"), centres[1].Node);
            Assert.AreEqual(new HierarchyId("/2/2/1/"), centres[2].Node);
            Assert.AreEqual(new HierarchyId("/2/2/2/"), centres[3].Node);
            Assert.AreEqual(new HierarchyId("/2/2/2/1/"), centres[4].Node);
        }

        [TestMethod]
        public void ReOrderingDescendinglyMustUpdateChildNodesAsWell()
        {
            //Arrange
            var ctx = CreateContext();
            mockCentres
                .Setup(set => set.Find(It.Is<object[]>(objects => (string)objects[0] == CostCentre0)))
                .Returns(() => centres[0]);
            mockCentres
                .Setup(set => set.Find(It.Is<object[]>(objects => (string)objects[0] == CostCentre1)))
                .Returns(() => centres[1]);

            ctx
                .SetupGet(c => c.CostCentres)
                .Returns(mockCentres.Object);
            ctx
                .SetupGet(c => c.CarbonEmissionEntries)
                .Returns(mockEntries.Object);
            var sut = new CostCentreController(ctx.Object);

            //Act
            sut.ReOrder(CostCentre1, 3);

            //Assert
            Assert.AreEqual(new HierarchyId("/3/"), centres[1].Node);
            Assert.AreEqual(new HierarchyId("/3/1/"), centres[2].Node);
            Assert.AreEqual(new HierarchyId("/3/2/"), centres[3].Node);
            Assert.AreEqual(new HierarchyId("/3/2/1/"), centres[4].Node);
            Assert.AreEqual(new HierarchyId("/1/"), centres[5].Node);
            Assert.AreEqual(new HierarchyId("/1/1/"), centres[6].Node);
            Assert.AreEqual(new HierarchyId("/2/"), centres[7].Node);
        }

        [TestMethod]
        public void ReOrderingAscendinglyMustUpdateChildNodesAsWell()
        {
            //Arrange
            var ctx = CreateContext();
            mockCentres
                .Setup(set => set.Find(It.Is<object[]>(objects => (string)objects[0] == CostCentre0)))
                .Returns(() => centres[0]);
            mockCentres
                .Setup(set => set.Find(It.Is<object[]>(objects => (string)objects[0] == CostCentre7)))
                .Returns(() => centres[7]);

            ctx
                .SetupGet(c => c.CostCentres)
                .Returns(mockCentres.Object);
            ctx
                .SetupGet(c => c.CarbonEmissionEntries)
                .Returns(mockEntries.Object);
            var sut = new CostCentreController(ctx.Object);

            //Act
            sut.ReOrder(CostCentre7, 1);

            //Assert
            Assert.AreEqual(new HierarchyId("/2/"), centres[1].Node);
            Assert.AreEqual(new HierarchyId("/2/1/"), centres[2].Node);
            Assert.AreEqual(new HierarchyId("/2/2/"), centres[3].Node);
            Assert.AreEqual(new HierarchyId("/2/2/1/"), centres[4].Node);
            Assert.AreEqual(new HierarchyId("/3/"), centres[5].Node);
            Assert.AreEqual(new HierarchyId("/3/1/"), centres[6].Node);
            Assert.AreEqual(new HierarchyId("/1/"), centres[7].Node);
        }
    }
}
