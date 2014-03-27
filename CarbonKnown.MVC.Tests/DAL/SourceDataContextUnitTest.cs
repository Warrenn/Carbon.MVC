using System;
using CarbonKnown.DAL;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Source;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CarbonKnown.MVC.Tests.DAL
{
    [TestClass]
    public class SourceDataContextUnitTest
    {
        [TestMethod]
        public void GetFileDataSourceMustUseHashIfProvided()
        {
            //Arrange
            var sourceId = Guid.NewGuid();
            var expectedId = Guid.NewGuid();
            var dataSource = new[]
                {
                    new FileDataSource {Id = sourceId, FileHash = "not right"},
                    new FileDataSource {Id = expectedId, FileHash = "hash"}
                };
            var mockContext = new Mock<DataContext>();
            var mockSet = new Mock<FakeDbSet<FileDataSource>>((object) dataSource);
            var mockContainer = new Mock<IUnityContainer>();
            mockContext
                .Setup(context => context.Set<FileDataSource>())
                .Returns(mockSet.Object);
            var sut = new SourceDataContext(() => mockContext.Object, mockContainer.Object);

            //Act
            var actual = sut.GetFileDataSource(sourceId, "hash");

            //Assert
            Assert.AreEqual(expectedId, actual.Id);
        }

        [TestMethod]
        public void GetFileDataSourceReturnsNullIfHashAndSourceAreNull()
        {
            //Arrange
            var dataSource = new[]
                {
                    new FileDataSource {Id = Guid.NewGuid(), FileHash = "not right"},
                    new FileDataSource {Id = Guid.NewGuid(), FileHash = "hash"}
                };
            var mockContext = new Mock<DataContext>();
            var mockSet = new Mock<FakeDbSet<FileDataSource>>((object)dataSource);
            var mockContainer = new Mock<IUnityContainer>();
            mockContext
                .Setup(context => context.Set<FileDataSource>())
                .Returns(mockSet.Object);
            var sut = new SourceDataContext(() => mockContext.Object, mockContainer.Object);

            //Act
            var actual = sut.GetFileDataSource(null, null);

            //Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void GetFileDataSourceUsesSourceIdIfHashIsNull()
        {
            //Arrange
            var sourceId = Guid.NewGuid();
            var dataSource = new[]
                {
                    new FileDataSource {Id = sourceId, FileHash = "hash2"},
                    new FileDataSource {Id = Guid.NewGuid(), FileHash = "hash"}
                };
            var mockContext = new Mock<DataContext>();
            var mockSet = new Mock<FakeDbSet<FileDataSource>>((object)dataSource);
            mockSet
                .Setup(set => set.Find(It.Is<Guid>(o => o == sourceId)))
                .Returns(dataSource[0]);
            var mockContainer = new Mock<IUnityContainer>();
            mockContext
                .Setup(context => context.Set<FileDataSource>())
                .Returns(mockSet.Object);
            var sut = new SourceDataContext(() => mockContext.Object, mockContainer.Object);

            //Act
            var actual = sut.GetFileDataSource(sourceId, null);

            //Assert
            Assert.AreEqual(sourceId, actual.Id);
        }

        [TestMethod]
        public void AddDataSourceCallsAddOnDbSet()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var mockSet = new Mock<FakeDbSet<ManualDataSource>>();
            var mockContainer = new Mock<IUnityContainer>();
            mockContext
                .Setup(context => context.Set<ManualDataSource>())
                .Returns(mockSet.Object);
            var sourceId = Guid.NewGuid();
            var entry = new ManualDataSource { Id = sourceId };
            var sut = new SourceDataContext(() => mockContext.Object, mockContainer.Object);

            //Act
            sut.AddDataSource(entry);

            //Assert
            mockSet.Verify(set => set.Add(It.Is<ManualDataSource>(e => e.Id == sourceId)));
        }

        [TestMethod]
        public void RemoveSourceMustRemoveSourceErrors()
        {
            //Arrange
            var sourceId = Guid.NewGuid();
            var sourceErrors = new[]
                {
                    new SourceError {DataSourceId = sourceId, Id = 1},
                    new SourceError {DataSourceId = sourceId, Id = 2},
                    new SourceError {DataSourceId = sourceId, Id = 3}
                };
            var mockSet = new Mock<FakeDbSet<SourceError>>((object) sourceErrors);
            var mockFileSet = new Mock<FakeDbSet<FileDataSource>>();
            var mockManualSet = new Mock<FakeDbSet<ManualDataSource>>();
            var mockContainer = new Mock<IUnityContainer>();
            var mockContext = new Mock<DataContext>();
            mockContext
                .Setup(context => context.SourceErrors)
                .Returns(mockSet.Object);
            mockContext
                .Setup(context => context.Set<FileDataSource>())
                .Returns(mockFileSet.Object);
            mockContext
                .Setup(context => context.Set<ManualDataSource>())
                .Returns(mockManualSet.Object);
            var sut = new SourceDataContext(() => mockContext.Object, mockContainer.Object);

            //Act
            sut.RemoveSource(sourceId);

            //Assert
            mockSet
                .Verify(set => set.Remove(It.Is<SourceError>(
                    error =>
                    (error.DataSourceId == sourceId) &&
                    ((error.Id == 1) ||
                     (error.Id == 2) ||
                     (error.Id == 3)))), Times.Exactly(3));
        }

        [TestMethod]
        public void RemoveSourceErrorsMustRemoveAllSourceErrors()
        {
            //Arrange
            var sourceId = Guid.NewGuid();
            var sourceErrors = new[]
                {
                    new SourceError {DataSourceId = sourceId, Id = 1},
                    new SourceError {DataSourceId = sourceId, Id = 2},
                    new SourceError {DataSourceId = sourceId, Id = 3}
                };
            var mockSet = new Mock<FakeDbSet<SourceError>>((object)sourceErrors);
            var mockContainer = new Mock<IUnityContainer>();
            var mockContext = new Mock<DataContext>();
            mockContext
                .Setup(context => context.SourceErrors)
                .Returns(mockSet.Object);
            var sut = new SourceDataContext(() => mockContext.Object, mockContainer.Object);

            //Act
            sut.RemoveSourceErrors(sourceId);

            //Assert
            mockSet
                .Verify(set => set.Remove(It.Is<SourceError>(
                    error =>
                    (error.DataSourceId == sourceId) &&
                    ((error.Id == 1) ||
                     (error.Id == 2) ||
                     (error.Id == 3)))), Times.Exactly(3));
        }

        [TestMethod]
        public void RemoveSourceErrorsMustCallSaveContext()
        {
            //Arrange
            var sourceId = Guid.NewGuid();
            var mockSet = new Mock<FakeDbSet<SourceError>>();
            var mockContainer = new Mock<IUnityContainer>();
            var mockContext = new Mock<DataContext>();
            mockContext
                .Setup(context => context.SourceErrors)
                .Returns(mockSet.Object);
            var sut = new SourceDataContext(() => mockContext.Object, mockContainer.Object);

            //Act
            sut.RemoveSourceErrors(sourceId);

            //Assert
            mockContext
                .Verify(context => context.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void RemoveSourceMustRemoveMatchingSourceId()
        {
            //Arrange
            var sourceId = Guid.NewGuid();
            var mockSet = new Mock<FakeDbSet<SourceError>>();
            var mockFileSet = new Mock<FakeDbSet<FileDataSource>>();
            var manualFileSet = new Mock<FakeDbSet<ManualDataSource>>();
            var mockContainer = new Mock<IUnityContainer>();
            var mockContext = new Mock<DataContext>();
            mockContext
                .Setup(context => context.Set<FileDataSource>())
                .Returns(mockFileSet.Object);
            mockContext
                .Setup(context => context.Set<ManualDataSource>())
                .Returns(manualFileSet.Object);
            mockContext
                .Setup(context => context.SourceErrors)
                .Returns(mockSet.Object);
            var sut = new SourceDataContext(() => mockContext.Object, mockContainer.Object);

            //Act
            sut.RemoveSource(sourceId);

            //Assert
            mockFileSet
                .Verify(set => set.Find(It.Is<Guid>(guid => guid == sourceId)), Times.Once);
            manualFileSet
                .Verify(set => set.Find(It.Is<Guid>(guid => guid == sourceId)), Times.Once);
            mockFileSet
                .Verify(set => set.Remove(It.IsAny<FileDataSource>()), Times.Once);
            manualFileSet
                .Verify(set => set.Remove(It.IsAny<ManualDataSource>()), Times.Once);
        }

        [TestMethod]
        public void RemoveSourceCalculationsMustRemoveAllCalculationThatMatchSourceID()
        {
            //Arrange
            var sourceId = Guid.NewGuid();
            var entries = new[]
                {
                    new CarbonEmissionEntry {SourceEntry = new DataEntry {SourceId = sourceId}},
                    new CarbonEmissionEntry {SourceEntry = new DataEntry {SourceId = sourceId}},
                    new CarbonEmissionEntry {SourceEntry = new DataEntry {SourceId = sourceId}}
                };
            var mockSet = new Mock<FakeDbSet<CarbonEmissionEntry>>((object) entries);
            var mockContainer = new Mock<IUnityContainer>();
            var mockContext = new Mock<DataContext>();
            mockContext
                .Setup(context => context.CarbonEmissionEntries)
                .Returns(mockSet.Object);
            var sut = new SourceDataContext(() => mockContext.Object, mockContainer.Object);

            //Act
            sut.RemoveSourceCalculations(sourceId);

            //Assert
            mockSet
                .Verify(set => set.Remove(It.IsAny<CarbonEmissionEntry>()), Times.Exactly(3));
        }

        [TestMethod]
        public void RemoveSourceCalculationsMustCallSaveChanges()
        {
            //Arrange
            var sourceId = Guid.NewGuid();
            var mockSet = new Mock<FakeDbSet<CarbonEmissionEntry>>();
            var mockContainer = new Mock<IUnityContainer>();
            var mockContext = new Mock<DataContext>();
            mockContext
                .Setup(context => context.CarbonEmissionEntries)
                .Returns(mockSet.Object);
            var sut = new SourceDataContext(() => mockContext.Object, mockContainer.Object);

            //Act
            sut.RemoveSourceCalculations(sourceId);

            //Assert
            mockContext
                .Verify(context => context.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void RemoveSourceMustSaveChanges()
        {
            //Arrange
            var sourceId = Guid.NewGuid();
            var mockSet = new Mock<FakeDbSet<SourceError>>();
            var mockContainer = new Mock<IUnityContainer>();
            var mockContext = new Mock<DataContext>();
            var mockFileSet = new Mock<FakeDbSet<FileDataSource>>();
            var manualFileSet = new Mock<FakeDbSet<ManualDataSource>>();
            mockContext
                .Setup(context => context.Set<FileDataSource>())
                .Returns(mockFileSet.Object);
            mockContext
                .Setup(context => context.Set<ManualDataSource>())
                .Returns(manualFileSet.Object);
            mockContext
                .Setup(context => context.SourceErrors)
                .Returns(mockSet.Object);
            var sut = new SourceDataContext(() => mockContext.Object, mockContainer.Object);

            //Act
            sut.RemoveSource(sourceId);

            //Assert
            mockContext
                .Verify(context => context.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void CreateUnitOfWorkMustCreateANewInstanceOfIDataEntriesUnitOfWork()
        {
            //Arrange
            var mockSet = new Mock<FakeDbSet<SourceError>>();
            var container = new UnityContainer();
            var mockContext = new Mock<DataContext>();
            container.RegisterType<IDataEntriesUnitOfWork, DataEntriesUnitOfWork>();
            mockContext
                .Setup(context => context.SourceErrors)
                .Returns(mockSet.Object);
            var sut = new SourceDataContext(() => mockContext.Object, container);

            //Act
            var actual = sut.CreateUnitOfWork();

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual, typeof (IDataEntriesUnitOfWork));
        }

        [TestMethod]
        public void SourceContainsErrorsMustReturnTrueIfSourceContainsErrorEntries()
        {
            //Arrange
            var sourceId = Guid.NewGuid();
            var errors = new[]
                {
                    new DataError
                        {
                            DataEntry = new DataEntry {SourceId = sourceId},
                            ErrorType = DataErrorType.StartDateGreaterThanEndDate
                        },
                    new DataError
                        {
                            DataEntry = new DataEntry {SourceId = sourceId},
                            ErrorType = DataErrorType.MissingValue
                        }
                };
            var mockSet = new Mock<FakeDbSet<DataError>>((object) errors);
            var container = new UnityContainer();
            var mockContext = new Mock<DataContext>();
            mockContext
                .Setup(context => context.Set<DataError>())
                .Returns(mockSet.Object);
            var sut = new SourceDataContext(() => mockContext.Object, container);

            //Act
            var actual = sut.SourceContainsDataEntriesInError(sourceId);

            //Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void GetDataEntryMustCallFindOnSet()
        {
            //Arrange
            var entryId = Guid.NewGuid();
            var mockSet = new Mock<FakeDbSet<DataEntry>>();
            var container = new UnityContainer();
            var mockContext = new Mock<DataContext>();
            mockContext
                .Setup(context => context.Set<DataEntry>())
                .Returns(mockSet.Object);
            var sut = new SourceDataContext(() => mockContext.Object, container);

            //Act
            sut.GetDataEntry<DataEntry>(entryId);

            //Assert
            mockSet
                .Verify(set => set.Find(It.Is<Guid>(guid => guid == entryId)));
        }

        [TestMethod]
        public void RemoveDataErrorsMustRemoveAllErrorsWithMatchingDataEntryId()
        {
            //Arrange
            var entryId = Guid.NewGuid();
            var errors = new[]
                {
                    new DataError {Id = 0, DataEntryId = entryId},
                    new DataError {Id = 1, DataEntryId = entryId},
                    new DataError {Id = 2, DataEntryId = Guid.NewGuid()}
                };
            var mockSet = new Mock<FakeDbSet<DataError>>((object) errors);
            var container = new UnityContainer();
            var mockContext = new Mock<DataContext>();
            mockContext
                .Setup(context => context.DataErrors)
                .Returns(mockSet.Object);
            var sut = new SourceDataContext(() => mockContext.Object, container);

            //Act
            sut.RemoveDataErrors(entryId);

            //Assert
            mockSet
                .Verify(set => set.Remove(It.Is<DataError>(error => error.DataEntryId == entryId)), Times.Exactly(2));
        }

        [TestMethod]
        public void RemoveDataErrorsMustSaveChanges()
        {
            //Arrange
            var entryId = Guid.NewGuid();
            var mockSet = new Mock<FakeDbSet<DataError>>();
            var container = new UnityContainer();
            var mockContext = new Mock<DataContext>();
            mockContext
                .Setup(context => context.DataErrors)
                .Returns(mockSet.Object);
            var sut = new SourceDataContext(() => mockContext.Object, container);

            //Act
            sut.RemoveDataErrors(entryId);

            //Assert
            mockContext
                .Verify(context => context.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void AddDataEntryMustSaveChanges()
        {
            //Arrange
            var entry = new DataEntry();
            var mockSet = new Mock<FakeDbSet<DataEntry>>();
            var container = new UnityContainer();
            var mockContext = new Mock<DataContext>();
            mockContext
                .Setup(context => context.Set<DataEntry>())
                .Returns(mockSet.Object);
            var sut = new SourceDataContext(() => mockContext.Object, container);

            //Act
            sut.AddDataEntry(entry);

            //Assert
            mockContext
                .Verify(context => context.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void AddDataEntryMustCallSetAdd()
        {
            //Arrange
            var entryId = Guid.NewGuid();
            var entry = new DataEntry {Id = entryId};
            var mockSet = new Mock<FakeDbSet<DataEntry>>();
            var container = new UnityContainer();
            var mockContext = new Mock<DataContext>();
            mockContext
                .Setup(context => context.Set<DataEntry>())
                .Returns(mockSet.Object);
            var sut = new SourceDataContext(() => mockContext.Object, container);

            //Act
            sut.AddDataEntry(entry);

            //Assert
            mockSet
                .Verify(set => set.Add(It.Is<DataEntry>(dataEntry => dataEntry.Id == entryId)));
        }

        [TestMethod]
        public void UpdateDataEntryMustCallSetAttach()
        {
            //Arrange
            var entryId = Guid.NewGuid();
            var entry = new DataEntry {Id = entryId};
            var mockSet = new Mock<FakeDbSet<DataEntry>>();
            var container = new UnityContainer();
            var mockContext = new Mock<DataContext>();
            mockContext
                .Setup(context => context.Set<DataEntry>())
                .Returns(mockSet.Object);
            var sut = new SourceDataContext(() => mockContext.Object, container);

            //Act
            sut.UpdateDataEntry(entry);

            //Assert
            mockSet
                .Verify(set => set.Attach(It.Is<DataEntry>(dataEntry => dataEntry.Id == entryId)));
        }

        [TestMethod]
        public void UpdateDataEntryMustSetStateToModified()
        {
            //Arrange
            var entryId = Guid.NewGuid();
            var entry = new DataEntry {Id = entryId};
            var mockSet = new Mock<FakeDbSet<DataEntry>>();
            mockSet
                .Setup(set => set.Attach(It.Is<DataEntry>(dataEntry => dataEntry.Id == entryId)))
                .Returns((DataEntry e) => e)
                .Verifiable();
            var container = new UnityContainer();
            var mockContext = new Mock<DataContext>();
            mockContext
                .Setup(context => context.Set<DataEntry>())
                .Returns(mockSet.Object);
            var sut = new SourceDataContext(() => mockContext.Object, container);

            //Act
            sut.UpdateDataEntry(entry);

            //Assert
            mockSet.Verify();
            mockContext
                .Verify(context => context.SetState(
                    It.Is<DataEntry>(dataEntry => dataEntry.Id == entryId),
                    It.Is<EntityState>(state => state == EntityState.Modified)));
        }

        [TestMethod]
        public void UpdateDataEntryMustSaveChanges()
        {
            //Arrange
            var entry = new DataEntry();
            var mockSet = new Mock<FakeDbSet<DataEntry>>();
            var container = new UnityContainer();
            var mockContext = new Mock<DataContext>();
            mockContext
                .Setup(context => context.Set<DataEntry>())
                .Returns(mockSet.Object);
            var sut = new SourceDataContext(() => mockContext.Object, container);

            //Act
            sut.UpdateDataEntry(entry);

            //Assert
            mockContext
                .Verify(context => context.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void SourceContainsErrorsMustReturnFalseIfSourceIfThereAreNoErrorEntries()
        {
            //Arrange
            var sourceId = Guid.NewGuid();
            var errors = new[]
                {
                    new DataError
                        {
                            DataEntry = new DataEntry {SourceId = sourceId},
                            ErrorType = DataErrorType.DuplicateEntry
                        },
                    new DataError
                        {
                            DataEntry = new DataEntry {SourceId = sourceId},
                            ErrorType = DataErrorType.BelowVarianceMinimum
                        }
                };
            var mockSet = new Mock<FakeDbSet<DataError>>((object)errors);
            var container = new UnityContainer();
            var mockContext = new Mock<DataContext>();
            mockContext
                .Setup(context => context.Set<DataError>())
                .Returns(mockSet.Object);
            var sut = new SourceDataContext(() => mockContext.Object, container);

            //Act
            var actual = sut.SourceContainsDataEntriesInError(sourceId);

            //Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void SourceContainsErrorsMustReturnFalseIfSourceIdIsNotFound()
        {
            //Arrange
            var sourceId = Guid.NewGuid();
            var mockSet = new Mock<FakeDbSet<DataError>>();
            var container = new UnityContainer();
            var mockContext = new Mock<DataContext>();
            mockContext
                .Setup(context => context.Set<DataError>())
                .Returns(mockSet.Object);
            var sut = new SourceDataContext(() => mockContext.Object, container);

            //Act
            var actual = sut.SourceContainsDataEntriesInError(sourceId);

            //Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void AddDataErrorMustCallSaveChanges()
        {
            //Arrange
            var error = new DataError();
            var mockContext = new Mock<DataContext>();
            var mockSet = new Mock<FakeDbSet<DataError>>();
            mockContext
                .Setup(context => context.DataErrors)
                .Returns(mockSet.Object);
            var mockContainer = new Mock<IUnityContainer>();
            var sut = new SourceDataContext(() => mockContext.Object, mockContainer.Object);

            //Act
            sut.AddDataError(error);

            //Assert
            mockContext.Verify(context => context.SaveChanges());
        }

        [TestMethod]
        public void AddDataErrorMustCallAddOnSet()
        {
            //Arrange
            var error = new DataError {Id = 87};
            var mockContext = new Mock<DataContext>();
            var mockSet = new Mock<FakeDbSet<DataError>>();
            mockContext
                .Setup(context => context.DataErrors)
                .Returns(mockSet.Object);
            var mockContainer = new Mock<IUnityContainer>();
            var sut = new SourceDataContext(() => mockContext.Object, mockContainer.Object);

            //Act
            sut.AddDataError(error);

            //Assert
            mockSet.Verify(set => set.Add(It.Is<DataError>(dataError => dataError.Id == 87)));
        }

        [TestMethod]
        public void AddDataSourceMustCallSaveChanges()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var mockSet = new Mock<FakeDbSet<ManualDataSource>>();
            mockContext
                .Setup(context => context.Set<ManualDataSource>())
                .Returns(mockSet.Object);
            var mockContainer = new Mock<IUnityContainer>();
            var sourceId = Guid.NewGuid();
            var entry = new ManualDataSource { Id = sourceId };
            var sut = new SourceDataContext(() => mockContext.Object, mockContainer.Object);

            //Act
            sut.AddDataSource(entry);

            //Assert
            mockContext.Verify(context => context.SaveChanges());
        }

        [TestMethod]
        public void AddSourceErrorCallsAddOnSourceError()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var mockSet = new Mock<FakeDbSet<SourceError>>();
            var mockContainer = new Mock<IUnityContainer>();
            mockContext
                .Setup(context => context.SourceErrors)
                .Returns(mockSet.Object);
            var entry = new SourceError { Id = 7 };
            var sut = new SourceDataContext(() => mockContext.Object, mockContainer.Object);

            //Act
            sut.AddSourceError(entry);

            //Assert
            mockSet.Verify(set => set.Add(It.Is<SourceError>(e => e.Id == 7)));
        }

        [TestMethod]
        public void AddSourceErrorMustCallSaveChanges()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var mockContainer = new Mock<IUnityContainer>();
            var mockSet = new Mock<FakeDbSet<SourceError>>();
            mockContext
                .Setup(context => context.SourceErrors)
                .Returns(mockSet.Object);
            var entry = new SourceError { Id = 7 };
            var sut = new SourceDataContext(() => mockContext.Object, mockContainer.Object);

            //Act
            sut.AddSourceError(entry);

            //Assert
            mockContext.Verify(context => context.SaveChanges());
        }

        [TestMethod]
        public void GetDataSourceMustReturnMatchingId()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var mockContainer = new Mock<IUnityContainer>();
            var sourceId = Guid.NewGuid();
            var dataSources = new[]
                {
                    new FileDataSource {Id = sourceId, UserName = "testuser"},
                    new FileDataSource {Id = Guid.NewGuid()}
                };
            var mockSet = new Mock<FakeDbSet<FileDataSource>>((object) dataSources);
            mockSet
                .Setup(set => set.Find(It.Is<Guid>(guid => guid == sourceId)))
                .Returns(dataSources[0]);
            mockContext
                .Setup(context => context.Set<FileDataSource>())
                .Returns(mockSet.Object);
            var sut = new SourceDataContext(() => mockContext.Object, mockContainer.Object);

            //Act
            var actual = sut.GetDataSource<FileDataSource>(sourceId);

            //Assert
            Assert.AreEqual(sourceId, actual.Id);
            Assert.AreEqual("testuser", actual.UserName);
        }

        [TestMethod]
        public void GetDataSourceMustReturnNullWhenThereIsNoMatch()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var mockContainer = new Mock<IUnityContainer>();
            var sourceId = Guid.NewGuid();
            var dataSources = new[]
                {
                    new FileDataSource {Id = Guid.NewGuid(), UserName = "testuser"},
                    new FileDataSource {Id = Guid.NewGuid()}
                };
            var mockSet = new Mock<FakeDbSet<FileDataSource>>((object)dataSources);
            mockContext
                .Setup(context => context.Set<FileDataSource>())
                .Returns(mockSet.Object);
            var sut = new SourceDataContext(() => mockContext.Object, mockContainer.Object);

            //Act
            var actual = sut.GetDataSource<FileDataSource>(sourceId);

            //Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void GetUserProfileMustReturnMatchingUserName()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var mockSet = new FakeDbSet<UserProfile>(new[]
                {
                    new UserProfile {UserId = 3, UserName = "testuser"},
                    new UserProfile {UserId = 5}
                });
            mockContext
                .Setup(context => context.UserProfiles)
                .Returns(mockSet);
            var mockContainer = new Mock<IUnityContainer>();
            var sut = new SourceDataContext(() => mockContext.Object, mockContainer.Object);

            //Act
            var actual = sut.GetUserProfile("testuser");

            //Assert
            Assert.AreEqual(3, actual.UserId);
        }

        [TestMethod]
        public void GetUserProfileMustReturnNullWhenThereIsNoMatch()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var mockSet = new FakeDbSet<UserProfile>(new[]
                {
                    new UserProfile {UserId = 3, UserName = "testuser"},
                    new UserProfile {UserId = 5}
                });
            mockContext
                .Setup(context => context.UserProfiles)
                .Returns(mockSet);
            var mockContainer = new Mock<IUnityContainer>();
            var sut = new SourceDataContext(() => mockContext.Object, mockContainer.Object);

            //Act
            var actual = sut.GetUserProfile("dontfindme");

            //Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void UpdateDataSourceMustAttachAndReturnDataSource()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var sourceId = Guid.NewGuid();
            var source = new FileDataSource {Id = sourceId, UserName = "testuser"};
            var mockSet = new Mock<FakeDbSet<FileDataSource>>();
            mockSet
                .Setup(set => set.Attach(It.Is<FileDataSource>(dataSource => dataSource.Id == sourceId)))
                .Returns(source)
                .Verifiable();
            mockContext
                .Setup(context => context.Set<FileDataSource>())
                .Returns(mockSet.Object);
            var mockContainer = new Mock<IUnityContainer>();
            var sut = new SourceDataContext(() => mockContext.Object, mockContainer.Object);

            //Act
            var actual = sut.UpdateDataSource(source);

            //Assert
            mockSet.Verify();
            Assert.AreEqual(source, actual);
        }

        [TestMethod]
        public void UpdateDataSourceMustSetTheStateToModified()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var sourceId = Guid.NewGuid();
            var source = new FileDataSource { Id = sourceId, UserName = "testuser" };
            var mockSet = new Mock<FakeDbSet<FileDataSource>>();
            mockSet
                .Setup(set => set.Attach(It.Is<FileDataSource>(dataSource => dataSource.Id == sourceId)))
                .Returns(source)
                .Verifiable();
            mockContext
                .Setup(context => context.Set<FileDataSource>())
                .Returns(mockSet.Object);
            var mockContainer = new Mock<IUnityContainer>();
            var sut = new SourceDataContext(() => mockContext.Object, mockContainer.Object);

            //Act
            sut.UpdateDataSource(source);

            //Assert
            mockContext.Verify(context => context.SetState(
                It.Is<FileDataSource>(dataSource => dataSource.Id == sourceId),
                It.Is<EntityState>(state => state == EntityState.Modified)));
        }

        [TestMethod]
        public void UpdateDataSourceMustCallSaveChanges()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var sourceId = Guid.NewGuid();
            var source = new FileDataSource { Id = sourceId, UserName = "testuser" };
            var mockSet = new Mock<FakeDbSet<FileDataSource>>();
            mockSet
                .Setup(set => set.Attach(It.Is<FileDataSource>(dataSource => dataSource.Id == sourceId)))
                .Returns(source)
                .Verifiable();
            mockContext
                .Setup(context => context.Set<FileDataSource>())
                .Returns(mockSet.Object);
            var mockContainer = new Mock<IUnityContainer>();
            var sut = new SourceDataContext(() => mockContext.Object, mockContainer.Object);

            //Act
            sut.UpdateDataSource(source);

            //Assert
            mockContext.Verify(context => context.SaveChanges());
        }

    }
}
