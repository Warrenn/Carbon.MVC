using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CarbonKnown.MVC.BLL;
using CarbonKnown.MVC.Controllers;
using CarbonKnown.WCF.DataSource;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CarbonKnown.MVC.Tests.Controllers
{
    //todo: needs refactoring
    //[TestClass]
    //public class FileUploadControllerUnitTest
    //{
    //    //todo: needs refactoring
    //    [TestMethod]
    //    public void UploadFileMustReturnErrorIfNoFilesAreUploaded()
    //    {
    //        //Arrange
    //        var fakeContext = new FakeHttpContext();
    //        var mockFiles = new Mock<HttpFileCollectionBase>();
    //        fakeContext
    //            .Request
    //            .Setup(@base => @base.Files)
    //            .Returns(mockFiles.Object);
    //        var mockService = new Mock<FileDataSourceService>();
    //        var sut = new FileDataSourceController(null, null, null);
    //        sut.ControllerContext = new ControllerContext(fakeContext.Object, new RouteData(), sut);
    //        var sourceId = Guid.NewGuid();

    //        //Act
    //        var results = sut.UploadFile();

    //        //Assert
    //        Assert.IsInstanceOfType(results, typeof (JsonResult));
    //        var data = ((JsonResult)results).Data as SourceResultDataContract;
    //        Assert.IsNotNull(data);
    //        Assert.AreEqual(sourceId, data.SourceId);
    //        Assert.AreEqual(1, data.ErrorMessages.Count);
    //        Assert.AreEqual(Resources.InvalidUpload, data.ErrorMessages[0]);
    //        Assert.IsFalse(data.Succeeded);
    //    }

    //    [TestMethod]
    //    public void UploadFileMustUpdateTheOriginalfilenameFilehandlerAndReferenceNotes()
    //    {
    //        //Arrange
    //        var fakeContext = new FakeHttpContext();
    //        var mockFiles = new Mock<HttpFileCollectionBase>();
    //        var mockFile = new Mock<HttpPostedFileBase>();
    //        mockFile
    //            .Setup(@base => @base.FileName)
    //            .Returns("newFileName");
    //        mockFile
    //            .Setup(@base => @base.InputStream)
    //            .Returns(new MemoryStream());
    //        mockFiles
    //            .Setup(@base => @base.Count)
    //            .Returns(1);
    //        mockFiles
    //            .Setup(@base => @base[It.Is<int>(i => i == 0)])
    //            .Returns(mockFile.Object);
    //        fakeContext
    //            .Request
    //            .Setup(@base => @base.Files)
    //            .Returns(mockFiles.Object);
    //        var mockService = new Mock<IFileDataSourceService>();
    //        var sut = new FileDataSourceController(mockService.Object);
    //        sut.ControllerContext = new ControllerContext(fakeContext.Object, new RouteData(), sut);
    //        var sourceId = Guid.NewGuid();

    //        //Act
    //        var results = sut.UploadFile("handler", "notes", sourceId);

    //        //Assert
    //        mockService
    //            .Verify(service => service.UpsertFileDataSource(
    //                It.Is<FileStreamDataContract>(
    //                    source =>
    //                    (source.SourceId == sourceId) &&
    //                    (source.HandlerName == "handler") &&
    //                    (source.ReferenceNotes == "notes") &&
    //                    (source.OriginalFileName == "newFileName"))));
    //        Assert.IsInstanceOfType(results, typeof (JsonResult));
    //    }

    //    [TestMethod]
    //    public void UploadFileReturnTheResultOfInsertfileDatasourceInJSON()
    //    {
    //        //Arrange
    //        var fakeContext = new FakeHttpContext();
    //        var mockFiles = new Mock<HttpFileCollectionBase>();
    //        var mockFile = new Mock<HttpPostedFileBase>();
    //        mockFile
    //            .Setup(@base => @base.FileName)
    //            .Returns("newFileName");
    //        mockFile
    //            .Setup(@base => @base.InputStream)
    //            .Returns(new MemoryStream());
    //        mockFiles
    //            .Setup(@base => @base.Count)
    //            .Returns(1);
    //        mockFiles
    //            .Setup(@base => @base[It.Is<int>(i => i == 0)])
    //            .Returns(mockFile.Object);
    //        fakeContext
    //            .Request
    //            .Setup(@base => @base.Files)
    //            .Returns(mockFiles.Object);
    //        var returnResult = new SourceResultDataContract
    //            {
    //                SourceId = Guid.NewGuid(),
    //                Succeeded = true,
    //                ErrorMessages = new Collection<string>(new[] {"error"})
    //            };
    //        var mockService = new Mock<IFileDataSourceService>();
    //        mockService
    //            .Setup(service => service.UpsertFileDataSource(It.IsAny<FileStreamDataContract>()))
    //            .Returns(returnResult);
    //        var sut = new FileDataSourceController(mockService.Object);
    //        sut.ControllerContext = new ControllerContext(fakeContext.Object, new RouteData(), sut);
    //        var sourceId = Guid.NewGuid();

    //        //Act
    //        var results = sut.UploadFile("handler", "notes", sourceId);

    //        //Assert
    //        Assert.IsInstanceOfType(results, typeof(JsonResult));
    //        var data = ((JsonResult) results).Data as SourceResultDataContract;
    //        Assert.IsNotNull(data);
    //        Assert.AreEqual(returnResult.SourceId, data.SourceId);
    //        Assert.AreEqual(returnResult.ErrorMessages[0], data.ErrorMessages[0]);
    //        Assert.AreEqual(returnResult.Succeeded, data.Succeeded);
    //    }

    //    [TestMethod]
    //    public void ChangeFileHandlerMustOnlyUpdateTheHandlerName()
    //    {
    //        //Arrange
    //        var fakeContext = new FakeHttpContext();
    //        var sourceId = Guid.NewGuid();
    //        var mockService = new Mock<IFileDataSourceService>();
    //        var sut = new FileDataSourceController(mockService.Object);
    //        sut.ControllerContext = new ControllerContext(fakeContext.Object, new RouteData(), sut);

    //        //Act
    //        sut.ChangeFileHandler("handler", sourceId);

    //        //Assert
    //        mockService
    //            .Verify(service => service.UpsertFileDataSource(It.Is<FileStreamDataContract>(
    //                contract =>
    //                    (contract.SourceId == sourceId) &&
    //                    (contract.HandlerName == "handler"))),Times.Once);
    //    }

    //    [TestMethod]
    //    public void ChangeFileHandlerMustReturnTheResultOfChangefileDataInJSON()
    //    {
    //        //Arrange
    //        var fakeContext = new FakeHttpContext();
    //        var sourceId = Guid.NewGuid();
    //        var mockService = new Mock<IFileDataSourceService>();
    //        var sut = new FileDataSourceController(mockService.Object);
    //        var returnResult = new SourceResultDataContract
    //        {
    //            SourceId = Guid.NewGuid(),
    //            Succeeded = true,
    //            ErrorMessages = new Collection<string>(new[] { "error" })
    //        };
    //        mockService
    //            .Setup(service => service.UpsertFileDataSource(It.IsAny<FileStreamDataContract>()))
    //            .Returns(returnResult);
    //        sut.ControllerContext = new ControllerContext(fakeContext.Object, new RouteData(), sut);

    //        //Act
    //        var results = sut.ChangeFileHandler("handler", sourceId);

    //        //Assert
    //        Assert.IsInstanceOfType(results, typeof(JsonResult));
    //        var data = ((JsonResult)results).Data as SourceResultDataContract;
    //        Assert.IsNotNull(data);
    //        Assert.AreEqual(returnResult.SourceId, data.SourceId);
    //        Assert.AreEqual(returnResult.ErrorMessages[0], data.ErrorMessages[0]);
    //        Assert.AreEqual(returnResult.Succeeded, data.Succeeded);
    //    }

    //    [TestMethod]
    //    public void CancelExtractionReturnTheResultOfCancelFileSourceExtractionInJSON()
    //    {
    //        //Arrange
    //        var fakeContext = new FakeHttpContext();
    //        var sourceId = Guid.NewGuid();
    //        var mockService = new Mock<IFileDataSourceService>();
    //        var sut = new FileDataSourceController(mockService.Object);
    //        sut.ControllerContext = new ControllerContext(fakeContext.Object, new RouteData(), sut);
    //        var returnResult = new SourceResultDataContract
    //        {
    //            SourceId = Guid.NewGuid(),
    //            Succeeded = true,
    //            ErrorMessages = new Collection<string>(new[] { "error" })
    //        };
    //        mockService
    //            .Setup(service => service.CancelFileSourceExtraction(It.IsAny<Guid>()))
    //            .Returns(returnResult);

    //        //Act
    //        var results = sut.CancelExtraction(sourceId);

    //        //Assert
    //        Assert.IsInstanceOfType(results, typeof(JsonResult));
    //        var data = ((JsonResult)results).Data as SourceResultDataContract;
    //        Assert.IsNotNull(data);
    //        Assert.AreEqual(returnResult.SourceId, data.SourceId);
    //        Assert.AreEqual(returnResult.ErrorMessages[0], data.ErrorMessages[0]);
    //        Assert.AreEqual(returnResult.Succeeded, data.Succeeded);
    //    }

    //    [TestMethod]
    //    public void ExtractFileDataMustChangeTheFileDataUsingTheHandlernameAndReferencenotes()
    //    {
    //        //Arrange
    //        var fakeContext = new FakeHttpContext();
    //        var sourceId = Guid.NewGuid();
    //        var mockService = new Mock<IFileDataSourceService>();
    //        var sut = new FileDataSourceController(mockService.Object);
    //        sut.ControllerContext = new ControllerContext(fakeContext.Object, new RouteData(), sut);
    //        mockService
    //            .Setup(service => service.UpsertFileDataSource(It.IsAny<FileStreamDataContract>()))
    //            .Returns(new SourceResultDataContract());

    //        //Act
    //        sut.ExtractFileData("handler", "referenceNotes", sourceId);

    //        //Assert
    //        mockService
    //            .Verify(service => service.UpsertFileDataSource(It.Is<FileStreamDataContract>(
    //                contract => 
    //                (contract.SourceId == sourceId) &&
    //                (contract.HandlerName == "handler") &&
    //                (contract.ReferenceNotes == "referenceNotes"))),Times.Once);
    //    }

    //    [TestMethod]
    //    public void ExtractFileDataMustNotExtractDataIfTheChangeOfDatafailed()
    //    {
    //        //Arrange
    //        var fakeContext = new FakeHttpContext();
    //        var sourceId = Guid.NewGuid();
    //        var mockService = new Mock<IFileDataSourceService>();
    //        var sut = new FileDataSourceController(mockService.Object);
    //        sut.ControllerContext = new ControllerContext(fakeContext.Object, new RouteData(), sut);
    //        mockService
    //            .Setup(service => service.UpsertFileDataSource(It.IsAny<FileStreamDataContract>()))
    //            .Returns(new SourceResultDataContract{Succeeded = false});

    //        //Act
    //        sut.ExtractFileData("handler", "referenceNotes", sourceId);

    //        //Assert
    //        mockService
    //            .Verify(service => service.ExtractData(It.IsAny<Guid>()), Times.Never);
    //    }

    //    [TestMethod]
    //    public void ExtractFileDataMustReturnTheResultOfExtractDataInJSON()
    //    {
    //        //Arrange
    //        var fakeContext = new FakeHttpContext();
    //        var sourceId = Guid.NewGuid();
    //        var mockService = new Mock<IFileDataSourceService>();
    //        var sut = new FileDataSourceController(mockService.Object);
    //        sut.ControllerContext = new ControllerContext(fakeContext.Object, new RouteData(), sut);
    //        var returnResult = new SourceResultDataContract
    //        {
    //            SourceId = Guid.NewGuid(),
    //            Succeeded = true,
    //            ErrorMessages = new Collection<string>(new[] { "error" })
    //        };
    //        mockService
    //            .Setup(service => service.UpsertFileDataSource(It.IsAny<FileStreamDataContract>()))
    //            .Returns(new SourceResultDataContract { Succeeded = true });
    //        mockService
    //            .Setup(service => service.ExtractData(It.Is<Guid>(guid => guid == sourceId)))
    //            .Returns(returnResult);

    //        //Act
    //        var results = sut.ExtractFileData("handler", "referenceNotes", sourceId);

    //        //Assert
    //        Assert.IsInstanceOfType(results, typeof(JsonResult));
    //        var data = ((JsonResult)results).Data as SourceResultDataContract;
    //        Assert.IsNotNull(data);
    //        Assert.AreEqual(returnResult.SourceId, data.SourceId);
    //        Assert.AreEqual(returnResult.ErrorMessages[0], data.ErrorMessages[0]);
    //        Assert.AreEqual(returnResult.Succeeded, data.Succeeded);
    //    }
    //}
}
