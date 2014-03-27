using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Web.Mvc;
using CarbonKnown.MVC.Tests.Properties;
using CarbonKnown.Print;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PdfSharp.Pdf.IO;

namespace CarbonKnown.MVC.Tests.Print
{
    [TestClass]
    public class UnitTestPrintAction
    {
        [TestMethod]
        public void BrowserShouldRenderTheCorrectImage()
        {
            //Arrange
            var resetEvent = new AutoResetEvent(false);
            Bitmap bitmap;
            using (var browser = new Browser("<html><head></head><body>Test Content</body></html>", resetEvent))
            {
                WaitHandle.WaitAll(new WaitHandle[] {resetEvent});
                bitmap = browser.BitmapResult;
            }

            var memStream = new MemoryStream();
            bitmap.Save(memStream, ImageFormat.Jpeg);

            //Act
            var actualBytes = memStream.ToArray();

            //Assert
            for (var i = 0; i < Resources.JpegBrowser.Length; i++)
            {
                Assert.AreEqual(Resources.JpegBrowser[i], actualBytes[i]);
            }
        }

        [TestMethod]
        public void PrintActionResultNeedsToCreateAValidJpeg()
        {
            //Arrange
            var memoryStream = new MemoryStream();
            var httpContext = new FakeHttpContext(outputStream: memoryStream);
            var httpController = new Mock<Controller>();
            var controllerContext = httpController
                .Object
                .SetFakeControllerContext(httpContext.Object,
                                          routeValues:
                                              new Dictionary<string, string>
                                                  {
                                                      {"action", "testaction"},
                                                      {"controller", "testcontroller"}
                                                  });
            var mockActionResult = PrintResult.PrintToJpeg();
            mockActionResult.RetrieveHtml = (viewName, masterName, model, context) =>
                                            "<html><head></head><body>Test Content</body></html>";

            //Act
            mockActionResult.ExecuteResult(controllerContext);

            //Assert
            var actualBytes = memoryStream.ToArray();
            for (var i = 0; i < actualBytes.Length; i++)
            {
                Assert.AreEqual(Resources.JpegBrowser[i], actualBytes[i]);
            }
        }

        [TestMethod]
        public void PrintActionResultNeedsToCreateAValidPdf()
        {
            //Arrange
            var httpContext = new FakeHttpContext();
            var httpController = new Mock<Controller>();
            var controllerContext = httpController
                .Object
                .SetFakeControllerContext(httpContext.Object,
                                       routeValues:
                                           new Dictionary<string, string>
                                               {
                                                   {"action", "testaction"},
                                                   {"controller", "testcontroller"}
                                               });
            var mockActionResult = PrintResult.PrintToPdf();
            mockActionResult.RetrieveHtml = (viewName, masterName, model, context) =>
                "<html><head></head><body>Test Content</body></html>";

            //Act
            mockActionResult.ExecuteResult(controllerContext);

            //Assert
            httpContext.Response
                       .Verify(@base => @base.BinaryWrite(It.Is<byte[]>(
                           actualBytes => PdfReader.TestPdfFile(actualBytes) == 14)));
        }
    }
}
