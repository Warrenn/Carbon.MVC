using System;
using CarbonKnown.Factors.DAL;
using CarbonKnown.Factors.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CarbonKnown.MVC.Tests.Factors
{
    //todo:needs refactoring
    //[TestClass]
    //public class FactorsUnitTest
    //{
    //    [TestMethod]
    //    public void FactorValueByIdMustGetMatchingValues()
    //    {
    //        //Arrange
    //        var mockContext = new Mock<DataContext>();
    //        var factorId = Guid.NewGuid();
    //        var mockSet = new Mock<FakeDbSet<FactorValue>>((object)new[]
    //            {
    //                new FactorValue {FactorId = factorId, EffectiveDate = DateTime.Now.AddDays(-2), Value = 123.1234M},
    //                new FactorValue {FactorId = factorId, EffectiveDate = DateTime.Now.AddDays(-3), Value = 12.1234M},
    //                new FactorValue
    //                    {
    //                        FactorId = Guid.NewGuid(),
    //                        EffectiveDate = DateTime.Now.AddDays(-1),
    //                        Value = 1.1234M
    //                    },
    //                new FactorValue {FactorId = factorId, EffectiveDate = DateTime.Now.AddDays(-1), Value = 1234.1234M},
    //                new FactorValue {FactorId = factorId, EffectiveDate = DateTime.Today, Value = 12345.1234M}
    //            });
    //        mockContext
    //            .SetupGet(context => context.FactorValues)
    //            .Returns(mockSet.Object);
    //        var sut = new CarbonKnown.Factors.Service.Factors(mockContext.Object);

    //        //Act
    //        var actual = sut.FactorValuesById(factorId);

    //        //Assert
    //        Assert.AreEqual(4, actual.Length);
    //    }

    //    [TestMethod]
    //    public void FactorValueByIdMustGetMatchingValuesInDescendingDateOrder()
    //    {
    //        //Arrange
    //        var mockContext = new Mock<DataContext>();
    //        var factorId = Guid.NewGuid();
    //        var mockSet = new Mock<FakeDbSet<FactorValue>>((object)new[]
    //            {
    //                new FactorValue {FactorId = factorId, EffectiveDate = DateTime.Now.AddDays(-2), Value = 3M},
    //                new FactorValue
    //                    {
    //                        FactorId = Guid.NewGuid(),
    //                        EffectiveDate = DateTime.Now.AddDays(-1),
    //                        Value = 2M
    //                    },
    //                new FactorValue {FactorId = factorId, EffectiveDate = DateTime.Now.AddDays(-3), Value = 4M},
    //                new FactorValue {FactorId = factorId, EffectiveDate = DateTime.Now.AddDays(-4), Value = 5M},
    //                new FactorValue {FactorId = factorId, EffectiveDate = DateTime.Today, Value = 1M}
    //            });
    //        mockContext
    //            .SetupGet(context => context.FactorValues)
    //            .Returns(mockSet.Object);
    //        var sut = new CarbonKnown.Factors.Service.Factors(mockContext.Object);

    //        //Act
    //        var actual = sut.FactorValuesById(factorId);

    //        //Assert
    //        Assert.AreEqual(1M, actual[0].FactorValue);
    //        Assert.AreEqual(3M, actual[1].FactorValue);
    //        Assert.AreEqual(4M, actual[2].FactorValue);
    //        Assert.AreEqual(5M, actual[3].FactorValue);
    //    }

    //    [TestMethod]
    //    public void FactorValueByIdMustBeAZeroLengthArrayNullWhenThereIsNoFactorIdMatch()
    //    {
    //        //Arrange
    //        var mockContext = new Mock<DataContext>();
    //        var factorId = Guid.NewGuid();
    //        var mockSet = new Mock<FakeDbSet<FactorValue>>((object)new[]
    //            {
    //                new FactorValue {FactorId = factorId, EffectiveDate = DateTime.Now.AddDays(-2), Value = 123.1234M},
    //                new FactorValue {FactorId = factorId, EffectiveDate = DateTime.Now.AddDays(-3), Value = 12.1234M},
    //                new FactorValue
    //                    {
    //                        FactorId = Guid.NewGuid(),
    //                        EffectiveDate = DateTime.Now.AddDays(-1),
    //                        Value = 1.1234M
    //                    },
    //                new FactorValue {FactorId = factorId, EffectiveDate = DateTime.Now.AddDays(-1), Value = 1234.1234M},
    //                new FactorValue {FactorId = factorId, EffectiveDate = DateTime.Today, Value = 12345.1234M}
    //            });
    //        mockContext
    //            .SetupGet(context => context.FactorValues)
    //            .Returns(mockSet.Object);
    //        var sut = new CarbonKnown.Factors.Service.Factors(mockContext.Object);

    //        //Act
    //        var actual = sut.FactorValuesById(Guid.NewGuid());

    //        //Assert
    //        Assert.AreEqual(0, actual.Length);
    //    }

    //    [TestMethod]
    //    public void FactorValueByNameMustGetValuesWithMatchingFactorNames()
    //    {
    //        //Arrange
    //        var mockContext = new Mock<DataContext>();
    //        var factorName = "Name";
    //        var mockSet = new Mock<FakeDbSet<FactorValue>>((object)new[]
    //            {
    //                new FactorValue
    //                    {
    //                        FactorId = Guid.NewGuid(),
    //                        EffectiveDate = DateTime.Now.AddDays(-2),
    //                        Value = 123.1234M,
    //                        Factor = new Factor
    //                            {
    //                                FactorName = factorName
    //                            }
    //                    },
    //                new FactorValue
    //                    {
    //                        FactorId = Guid.NewGuid(),
    //                        EffectiveDate = DateTime.Now.AddDays(-3),
    //                        Value = 12.1234M,
    //                        Factor = new Factor
    //                            {
    //                                FactorName = factorName
    //                            }
    //                    },
    //                new FactorValue
    //                    {
    //                        FactorId = Guid.NewGuid(),
    //                        EffectiveDate = DateTime.Now.AddDays(-1),
    //                        Value = 1.1234M,
    //                        Factor = new Factor
    //                            {
    //                                FactorName = "dontreturnme"
    //                            }
    //                    },
    //                new FactorValue
    //                    {
    //                        FactorId = Guid.NewGuid(),
    //                        EffectiveDate = DateTime.Now.AddDays(-1),
    //                        Value = 1234.1234M,
    //                        Factor = new Factor
    //                            {
    //                                FactorName = factorName
    //                            }
    //                    },
    //                new FactorValue
    //                    {
    //                        FactorId = Guid.NewGuid(),
    //                        EffectiveDate = DateTime.Today,
    //                        Value = 12345.1234M,
    //                        Factor = new Factor
    //                            {
    //                                FactorName = factorName
    //                            }
    //                    }
    //            });
    //        mockSet
    //            .Setup(set => set.Include(It.Is<string>(s => s == "Factor")))
    //            .Returns(mockSet.Object)
    //            .Verifiable();
    //        mockContext
    //            .SetupGet(context => context.FactorValues)
    //            .Returns(mockSet.Object);
    //        var sut = new CarbonKnown.Factors.Service.Factors(mockContext.Object);

    //        //Act
    //        var actual = sut.FactorValuesByName(factorName);

    //        //Assert
    //        Assert.AreEqual(4, actual.Length);
    //    }

    //    [TestMethod]
    //    public void FactorValueByNameMustGetValuesInDescendingDateOrder()
    //    {
    //        //Arrange
    //        var mockContext = new Mock<DataContext>();
    //        var factorName = "Name";
    //        var mockSet = new Mock<FakeDbSet<FactorValue>>((object)new[]
    //            {
    //                new FactorValue
    //                    {
    //                        FactorId = Guid.NewGuid(),
    //                        EffectiveDate = DateTime.Now.AddDays(-4),
    //                        Value = 4M,
    //                        Factor = new Factor
    //                            {
    //                                FactorName = factorName
    //                            }
    //                    },
    //                new FactorValue
    //                    {
    //                        FactorId = Guid.NewGuid(),
    //                        EffectiveDate = DateTime.Now.AddDays(-1),
    //                        Value = 1M,
    //                        Factor = new Factor
    //                            {
    //                                FactorName = factorName
    //                            }
    //                    },
    //                new FactorValue
    //                    {
    //                        FactorId = Guid.NewGuid(),
    //                        EffectiveDate = DateTime.Now.AddDays(-3),
    //                        Value = 3M,
    //                        Factor = new Factor
    //                            {
    //                                FactorName = "dontreturnme"
    //                            }
    //                    },
    //                new FactorValue
    //                    {
    //                        FactorId = Guid.NewGuid(),
    //                        EffectiveDate = DateTime.Now.AddDays(-2),
    //                        Value = 2M,
    //                        Factor = new Factor
    //                            {
    //                                FactorName = factorName
    //                            }
    //                    },
    //                new FactorValue
    //                    {
    //                        FactorId = Guid.NewGuid(),
    //                        EffectiveDate = DateTime.Today,
    //                        Value = 0M,
    //                        Factor = new Factor
    //                            {
    //                                FactorName = factorName
    //                            }
    //                    }
    //            });
    //        mockSet
    //            .Setup(set => set.Include(It.Is<string>(s => s == "Factor")))
    //            .Returns(mockSet.Object)
    //            .Verifiable();
    //        mockContext
    //            .SetupGet(context => context.FactorValues)
    //            .Returns(mockSet.Object);
    //        var sut = new CarbonKnown.Factors.Service.Factors(mockContext.Object);

    //        //Act
    //        var actual = sut.FactorValuesByName(factorName);

    //        //Assert
    //        Assert.AreEqual(0M, actual[0].FactorValue);
    //        Assert.AreEqual(1M, actual[1].FactorValue);
    //        Assert.AreEqual(2M, actual[2].FactorValue);
    //        Assert.AreEqual(4M, actual[3].FactorValue);
    //    }

    //    [TestMethod]
    //    public void FactorValueByNameMustIncludeFactorInQuery()
    //    {
    //        //Arrange
    //        var mockContext = new Mock<DataContext>();
    //        var factorName = "Name";
    //        var mockSet = new Mock<FakeDbSet<FactorValue>>((object)new[]
    //            {
    //                new FactorValue
    //                    {
    //                        FactorId = Guid.NewGuid(),
    //                        EffectiveDate = DateTime.Now.AddDays(-2),
    //                        Value = 123.1234M,
    //                        Factor = new Factor
    //                            {
    //                                FactorName = factorName
    //                            }
    //                    },
    //                new FactorValue
    //                    {
    //                        FactorId = Guid.NewGuid(),
    //                        EffectiveDate = DateTime.Now.AddDays(-3),
    //                        Value = 12.1234M,
    //                        Factor = new Factor
    //                            {
    //                                FactorName = factorName
    //                            }
    //                    },
    //                new FactorValue
    //                    {
    //                        FactorId = Guid.NewGuid(),
    //                        EffectiveDate = DateTime.Now.AddDays(-1),
    //                        Value = 1.1234M,
    //                        Factor = new Factor
    //                            {
    //                                FactorName = "dontreturnme"
    //                            }
    //                    },
    //                new FactorValue
    //                    {
    //                        FactorId = Guid.NewGuid(),
    //                        EffectiveDate = DateTime.Now.AddDays(-1),
    //                        Value = 1234.1234M,
    //                        Factor = new Factor
    //                            {
    //                                FactorName = factorName
    //                            }
    //                    },
    //                new FactorValue
    //                    {
    //                        FactorId = Guid.NewGuid(),
    //                        EffectiveDate = DateTime.Today,
    //                        Value = 12345.1234M,
    //                        Factor = new Factor
    //                            {
    //                                FactorName = factorName
    //                            }
    //                    }
    //            });
    //        mockSet
    //            .Setup(set => set.Include(It.Is<string>(s => s == "Factor")))
    //            .Returns(mockSet.Object)
    //            .Verifiable();
    //        mockContext
    //            .SetupGet(context => context.FactorValues)
    //            .Returns(mockSet.Object);
    //        var sut = new CarbonKnown.Factors.Service.Factors(mockContext.Object);

    //        //Act
    //        var actual = sut.FactorValuesByName(factorName);

    //        //Assert
    //        mockSet.Verify();
    //    }

    //    [TestMethod]
    //    public void FactorValueByNameMustReturnAZeroLengthArrayWhenThereIsNoFactorNameMatch()
    //    {
    //        //Arrange
    //        var mockContext = new Mock<DataContext>();
    //        var factorName = "Name";
    //        var mockSet = new Mock<FakeDbSet<FactorValue>>((object)new[]
    //            {
    //                new FactorValue
    //                    {
    //                        FactorId = Guid.NewGuid(),
    //                        EffectiveDate = DateTime.Now.AddDays(-2),
    //                        Value = 123.1234M,
    //                        Factor = new Factor
    //                            {
    //                                FactorName = factorName
    //                            }
    //                    },
    //                new FactorValue
    //                    {
    //                        FactorId = Guid.NewGuid(),
    //                        EffectiveDate = DateTime.Now.AddDays(-3),
    //                        Value = 12.1234M,
    //                        Factor = new Factor
    //                            {
    //                                FactorName = factorName
    //                            }
    //                    },
    //                new FactorValue
    //                    {
    //                        FactorId = Guid.NewGuid(),
    //                        EffectiveDate = DateTime.Now.AddDays(-1),
    //                        Value = 1.1234M,
    //                        Factor = new Factor
    //                            {
    //                                FactorName = "dontreturnme"
    //                            }
    //                    },
    //                new FactorValue
    //                    {
    //                        FactorId = Guid.NewGuid(),
    //                        EffectiveDate = DateTime.Now.AddDays(-1),
    //                        Value = 1234.1234M,
    //                        Factor = new Factor
    //                            {
    //                                FactorName = factorName
    //                            }
    //                    },
    //                new FactorValue
    //                    {
    //                        FactorId = Guid.NewGuid(),
    //                        EffectiveDate = DateTime.Today,
    //                        Value = 12345.1234M,
    //                        Factor = new Factor
    //                            {
    //                                FactorName = factorName
    //                            }
    //                    }
    //            });
    //        mockSet
    //            .Setup(set => set.Include(It.Is<string>(s => s == "Factor")))
    //            .Returns(mockSet.Object)
    //            .Verifiable();
    //        mockContext
    //            .SetupGet(context => context.FactorValues)
    //            .Returns(mockSet.Object);
    //        var sut = new CarbonKnown.Factors.Service.Factors(mockContext.Object);

    //        //Act
    //        var actual = sut.FactorValuesByName( "dontfindme");

    //        //Assert
    //        Assert.AreEqual(0, actual.Length);
    //    }

    //    [TestMethod]
    //    public void AirRouteDistanceMustReturnAValueForMatchingCode1AndCode2()
    //    {
    //        //Arrange
    //        var mockContext = new Mock<DataContext>();
    //        var mockSet = new Mock<FakeDbSet<AirRouteDistance>>((object) new[]
    //            {
    //                new AirRouteDistance {Code1 = "Code1", Code2 = "Code2", Distance = 12345.12345M},
    //                new AirRouteDistance {Code1 = "Code1a", Code2 = "Code2", Distance = 1234.12345M},
    //                new AirRouteDistance {Code1 = "Code1a", Code2 = "Code2a", Distance = 123.12345M},
    //                new AirRouteDistance {Code1 = "Code1b", Code2 = "Code2", Distance = 12.12345M},
    //                new AirRouteDistance {Code1 = "Code1b", Code2 = "Code2a", Distance = 1.12345M}
    //            });
    //        mockContext
    //            .SetupGet(context => context.AirRouteDistances)
    //            .Returns(mockSet.Object);
    //        var sut = new CarbonKnown.Factors.Service.Factors(mockContext.Object);

    //        //Act
    //        var actual = sut.AirRouteDistance("Code1",  "Code2");

    //        //Assert
    //        Assert.AreEqual(12345.12345M, actual);
    //    }

    //    [TestMethod]
    //    public void AirRouteDistanceMustReturnAValueForMatchingSwappedCode1AndCode2()
    //    {
    //        //Arrange
    //        var mockContext = new Mock<DataContext>();
    //        var mockSet = new Mock<FakeDbSet<AirRouteDistance>>((object)new[]
    //            {
    //                new AirRouteDistance {Code1 = "Code1", Code2 = "Code2", Distance = 12345.12345M},
    //                new AirRouteDistance {Code1 = "Code1a", Code2 = "Code2", Distance = 1234.12345M},
    //                new AirRouteDistance {Code1 = "Code1a", Code2 = "Code2a", Distance = 123.12345M},
    //                new AirRouteDistance {Code1 = "Code1b", Code2 = "Code2", Distance = 12.12345M},
    //                new AirRouteDistance {Code1 = "Code1b", Code2 = "Code2a", Distance = 1.12345M}
    //            });
    //        mockContext
    //            .SetupGet(context => context.AirRouteDistances)
    //            .Returns(mockSet.Object);
    //        var sut = new CarbonKnown.Factors.Service.Factors(mockContext.Object);

    //        //Act
    //        var actual = sut.AirRouteDistance("Code2", "Code1");

    //        //Assert
    //        Assert.AreEqual(12345.12345M, actual);
    //    }

    //    [TestMethod]
    //    public void AirRouteDistanceMustReturnNullIfThereIsNoCode1Match()
    //    {
    //        //Arrange
    //        var mockContext = new Mock<DataContext>();
    //        var mockSet = new Mock<FakeDbSet<AirRouteDistance>>((object)new[]
    //            {
    //                new AirRouteDistance {Code1 = "Code1", Code2 = "Code2", Distance = 12345.12345M},
    //                new AirRouteDistance {Code1 = "Code1a", Code2 = "Code2", Distance = 1234.12345M},
    //                new AirRouteDistance {Code1 = "Code1a", Code2 = "Code2a", Distance = 123.12345M},
    //                new AirRouteDistance {Code1 = "Code1b", Code2 = "Code2", Distance = 12.12345M},
    //                new AirRouteDistance {Code1 = "Code1b", Code2 = "Code2a", Distance = 1.12345M}
    //            });
    //        mockContext
    //            .SetupGet(context => context.AirRouteDistances)
    //            .Returns(mockSet.Object);
    //        var sut = new CarbonKnown.Factors.Service.Factors(mockContext.Object);

    //        //Act
    //        var actual = sut.AirRouteDistance("Code1NoMatch", "Code2");

    //        //Assert
    //        Assert.IsNull(actual);
    //    }

    //    [TestMethod]
    //    public void AirRouteDistanceMustReturnNullIfThereIsNoCode2Match()
    //    {
    //        //Arrange
    //        var mockContext = new Mock<DataContext>();
    //        var mockSet = new Mock<FakeDbSet<AirRouteDistance>>((object)new[]
    //            {
    //                new AirRouteDistance {Code1 = "Code1", Code2 = "Code2", Distance = 12345.12345M},
    //                new AirRouteDistance {Code1 = "Code1a", Code2 = "Code2", Distance = 1234.12345M},
    //                new AirRouteDistance {Code1 = "Code1a", Code2 = "Code2a", Distance = 123.12345M},
    //                new AirRouteDistance {Code1 = "Code1b", Code2 = "Code2", Distance = 12.12345M},
    //                new AirRouteDistance {Code1 = "Code1b", Code2 = "Code2a", Distance = 1.12345M}
    //            });
    //        mockContext
    //            .SetupGet(context => context.AirRouteDistances)
    //            .Returns(mockSet.Object);
    //        var sut = new CarbonKnown.Factors.Service.Factors(mockContext.Object);

    //        //Act
    //        var actual = sut.AirRouteDistance("Code1", "Code2NoMatch");

    //        //Assert
    //        Assert.IsNull(actual);
    //    }

    //    [TestMethod]
    //    public void CourierRouteDistanceMustReturnAValueForMatchingCode1AndCode2()
    //    {
    //        //Arrange
    //        var mockContext = new Mock<DataContext>();
    //        var mockSet = new Mock<FakeDbSet<CourierRouteDistance>>((object)new[]
    //            {
    //                new CourierRouteDistance {Code1 = "Code1", Code2 = "Code2", Distance = 12345.12345M},
    //                new CourierRouteDistance {Code1 = "Code1a", Code2 = "Code2", Distance = 1234.12345M},
    //                new CourierRouteDistance {Code1 = "Code1a", Code2 = "Code2a", Distance = 123.12345M},
    //                new CourierRouteDistance {Code1 = "Code1b", Code2 = "Code2", Distance = 12.12345M},
    //                new CourierRouteDistance {Code1 = "Code1b", Code2 = "Code2a", Distance = 1.12345M}
    //            });
    //        mockContext
    //            .SetupGet(context => context.CourierRouteDistances)
    //            .Returns(mockSet.Object);
    //        var sut = new CarbonKnown.Factors.Service.Factors(mockContext.Object);

    //        //Act
    //        var actual = sut.CourierRouteDistance("Code1", "Code2");

    //        //Assert
    //        Assert.AreEqual(12345.12345M, actual);
    //    }

    //    [TestMethod]
    //    public void CourierRouteDistanceMustReturnAValueForMatchingSwappedCode1AndCode2()
    //    {
    //        //Arrange
    //        var mockContext = new Mock<DataContext>();
    //        var mockSet = new Mock<FakeDbSet<CourierRouteDistance>>((object)new[]
    //            {
    //                new CourierRouteDistance {Code1 = "Code1", Code2 = "Code2", Distance = 12345.12345M},
    //                new CourierRouteDistance {Code1 = "Code1a", Code2 = "Code2", Distance = 1234.12345M},
    //                new CourierRouteDistance {Code1 = "Code1a", Code2 = "Code2a", Distance = 123.12345M},
    //                new CourierRouteDistance {Code1 = "Code1b", Code2 = "Code2", Distance = 12.12345M},
    //                new CourierRouteDistance {Code1 = "Code1b", Code2 = "Code2a", Distance = 1.12345M}
    //            });
    //        mockContext
    //            .SetupGet(context => context.CourierRouteDistances)
    //            .Returns(mockSet.Object);
    //        var sut = new CarbonKnown.Factors.Service.Factors(mockContext.Object);

    //        //Act
    //        var actual = sut.CourierRouteDistance("Code2", "Code1");

    //        //Assert
    //        Assert.AreEqual(12345.12345M, actual);
    //    }

    //    [TestMethod]
    //    public void CourierRouteDistanceMustReturnNullIfThereIsNoCode1Match()
    //    {
    //        //Arrange
    //        var mockContext = new Mock<DataContext>();
    //        var mockSet = new Mock<FakeDbSet<CourierRouteDistance>>((object)new[]
    //            {
    //                new CourierRouteDistance {Code1 = "Code1", Code2 = "Code2", Distance = 12345.12345M},
    //                new CourierRouteDistance {Code1 = "Code1a", Code2 = "Code2", Distance = 1234.12345M},
    //                new CourierRouteDistance {Code1 = "Code1a", Code2 = "Code2a", Distance = 123.12345M},
    //                new CourierRouteDistance {Code1 = "Code1b", Code2 = "Code2", Distance = 12.12345M},
    //                new CourierRouteDistance {Code1 = "Code1b", Code2 = "Code2a", Distance = 1.12345M}
    //            });
    //        mockContext
    //            .SetupGet(context => context.CourierRouteDistances)
    //            .Returns(mockSet.Object);
    //        var sut = new CarbonKnown.Factors.Service.Factors(mockContext.Object);

    //        //Act
    //        var actual = sut.CourierRouteDistance("Code1NoMatch", "Code2");

    //        //Assert
    //        Assert.IsNull(actual);
    //    }

    //    [TestMethod]
    //    public void CourierRouteDistanceMustReturnNullIfThereIsNoCode2Match()
    //    {
    //        //Arrange
    //        var mockContext = new Mock<DataContext>();
    //        var mockSet = new Mock<FakeDbSet<CourierRouteDistance>>((object)new[]
    //            {
    //                new CourierRouteDistance {Code1 = "Code1", Code2 = "Code2", Distance = 12345.12345M},
    //                new CourierRouteDistance {Code1 = "Code1a", Code2 = "Code2", Distance = 1234.12345M},
    //                new CourierRouteDistance {Code1 = "Code1a", Code2 = "Code2a", Distance = 123.12345M},
    //                new CourierRouteDistance {Code1 = "Code1b", Code2 = "Code2", Distance = 12.12345M},
    //                new CourierRouteDistance {Code1 = "Code1b", Code2 = "Code2a", Distance = 1.12345M}
    //            });
    //        mockContext
    //            .SetupGet(context => context.CourierRouteDistances)
    //            .Returns(mockSet.Object);
    //        var sut = new CarbonKnown.Factors.Service.Factors(mockContext.Object);

    //        //Act
    //        var actual = sut.CourierRouteDistance("Code1", "Code2NoMatch");

    //        //Assert
    //        Assert.IsNull(actual);
    //    }
    //}
}
