using System;
using System.IO;
using System.Reactive;
using CarbonKnown.FileReaders.FileHandler;
using CarbonKnown.FileWatcherService;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Reactive.Concurrency;

namespace CarbonKnown.MVC.Tests.FileWatcher
{
    [TestClass]
    public class FolderMonitorUnitTest : ReactiveTest
    {
        [TestMethod]
        public void FireWhenANewFileArrivesInFolder()
        {
            //Arrange
            var testFolder = Environment.CurrentDirectory;
            var testFile = Path.Combine(testFolder, "test.file");
            var scheduler = new TestScheduler();
            var fileHandlerMock = new Mock<IFileHandler>();
            var monitorMock = new Mock<FolderMonitor>(testFolder, fileHandlerMock.Object) {CallBase = true};

            var service = monitorMock.Object;
            service.Interval = TimeSpan.FromSeconds(1);
            service.RetryCount = 3;
            service.WatcherObservable = scheduler
                .CreateHotObservable(
                    OnNext(10, new EventPattern<FileSystemEventArgs>(
                                   null,
                                   new FileSystemEventArgs(WatcherChangeTypes.Created, testFolder, testFile))));
            service.RetryScheduler = scheduler;
            service.StartMonitoring();

            //Act
            scheduler.AdvanceBy(TimeSpan.FromMinutes(1).Ticks);
            
            //Assert
            fileHandlerMock.Verify(handler => handler.ReportError(It.IsAny<string>(), It.IsAny<Exception>()));
        }

        [TestMethod]
        public void WhenAHandleToTheFileCannotBeObtainedCallFailureMethod()
        {
            //Arrange
            var testFolder = Environment.CurrentDirectory;
            var scheduler = new TestScheduler();
            var fileHandlerMock = new Mock<IFileHandler>();
            var monitorMock = new Mock<FolderMonitor>(testFolder, fileHandlerMock.Object) { CallBase = true };

            var service = monitorMock.Object;
            service.Interval = TimeSpan.FromSeconds(1);
            service.RetryCount = 3;
            service.WatcherObservable = scheduler
                .CreateHotObservable(
                    OnNext(10, new EventPattern<FileSystemEventArgs>(
                                   null,
                                   new FileSystemEventArgs(WatcherChangeTypes.Created, testFolder, "test.file"))));
            service.RetryScheduler = scheduler;
            service.StartMonitoring();

            //Act
            scheduler.AdvanceBy(TimeSpan.FromMinutes(1).Ticks);
            
            //Assert
            fileHandlerMock.Verify(
                handler =>
                handler.ReportError(It.Is<string>(v => v == Path.Combine(testFolder, "test.file")),
                                    It.Is<Exception>(e => e is FileNotFoundException)));
        }

        [TestMethod]
        public void WhenAHandleToTheFileCanBeObtainedCallSucessMethod()
        {
            //Arrange
            var scheduler = new TestScheduler();
            var filePath = Path.GetTempFileName();
            var testFolder = Path.GetDirectoryName(filePath);
            var fileHandlerMock = new Mock<IFileHandler>();
            var monitorMock = new Mock<FolderMonitor>(testFolder, fileHandlerMock.Object) { CallBase = true };
            var testFile = Path.GetFileName(filePath);
            fileHandlerMock
                .Setup(handler => handler.ProcessFile(It.Is<string>(v => v == filePath), It.IsAny<Stream>()))
                .Callback((string path, Stream s) =>
                {
                    s.Dispose();
                    File.Delete(path);
                })
                .Verifiable();

            var service = monitorMock.Object;
            service.Interval = TimeSpan.FromSeconds(1);
            service.RetryCount = 3;
            service.WatcherObservable = scheduler
                .CreateHotObservable(
                    OnNext(10, new EventPattern<FileSystemEventArgs>(
                                   null,
                                   new FileSystemEventArgs(WatcherChangeTypes.Created, testFolder, testFile))));
            service.RetryScheduler = scheduler;
            service.StartMonitoring();

            //Act
            scheduler.AdvanceBy(TimeSpan.FromMinutes(1).Ticks);

            //Assert
            fileHandlerMock.VerifyAll();
        }

        [TestMethod]
        public void OnlyCallProcessOnlyOnceEvenIfItThrows()
        {
            //Arrange
            var scheduler = new TestScheduler();
            var filePath = Path.GetTempFileName();
            var testFolder = Path.GetDirectoryName(filePath);
            var fileHandlerMock = new Mock<IFileHandler>();
            var monitorMock = new Mock<FolderMonitor>(testFolder, fileHandlerMock.Object) { CallBase = true };
            var testFile = Path.GetFileName(filePath);
            fileHandlerMock
                .Setup(handler => handler.ProcessFile(It.Is<string>(v => v == filePath), It.IsAny<Stream>()))
                .Callback((string path, Stream s) =>
                    {
                        s.Dispose();
                        throw new Exception();
                    })
                .Verifiable();

            var service = monitorMock.Object;
            service.Interval = TimeSpan.FromSeconds(1);
            service.RetryCount = 3;
            service.WatcherObservable = scheduler
                .CreateHotObservable(
                    OnNext(10, new EventPattern<FileSystemEventArgs>(
                                   null,
                                   new FileSystemEventArgs(WatcherChangeTypes.Created, testFolder, testFile))));
            service.RetryScheduler = scheduler;
            service.StartMonitoring();

            //Act
            try
            {
                scheduler.AdvanceBy(TimeSpan.FromMinutes(1).Ticks);
            }
            catch (Exception)
            {
            }

            //CleanUp
            File.Delete(filePath);

            //Assert
            fileHandlerMock.Verify(handler => handler.ProcessFile(It.Is<string>(v => v == filePath), It.IsAny<Stream>()),
                               Times.Once);
            fileHandlerMock.Verify(handler => handler.ReportError(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
        }

        [TestMethod]
        public void KeepLoopingUntilHandleCanBeObtained()
        {
            //Arrange
            var scheduler = new TestScheduler();
            var filePath = Path.GetTempFileName();
            var testFolder = Path.GetDirectoryName(filePath);
            var fileHandlerMock = new Mock<IFileHandler>();
            var monitorMock = new Mock<FolderMonitor>(testFolder, fileHandlerMock.Object) { CallBase = true };
            var testFile = Path.GetFileName(filePath);
            Stream lockStream = File.Open(filePath, FileMode.Open);
            scheduler.Schedule(TimeSpan.FromSeconds(35), lockStream.Dispose);
            fileHandlerMock
                .Setup(handler => handler.ProcessFile(It.Is<string>(v => v == filePath), It.IsAny<Stream>()))
                .Callback((string path, Stream s) =>
                    {
                        s.Dispose();
                        File.Delete(path);
                    })
                .Verifiable();

            var service = monitorMock.Object;
            service.Interval = TimeSpan.FromSeconds(20);
            service.RetryCount = 5;
            service.WatcherObservable = scheduler
                .CreateHotObservable(
                    OnNext(30,
                           new EventPattern<FileSystemEventArgs>(null,
                                                                 new FileSystemEventArgs(
                                                                     WatcherChangeTypes.Created, testFolder,
                                                                     testFile)))
                );
            service.RetryScheduler = scheduler;
            service.StartMonitoring();

            //Act           
            scheduler.AdvanceBy(TimeSpan.FromMinutes(1).Ticks);
            
            //Assert
            fileHandlerMock.VerifyAll();
        }
    }
}
