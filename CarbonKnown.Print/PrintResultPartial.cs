using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace CarbonKnown.Print
{
    public partial class PrintResult
    {
        private static readonly Action<string, string, Bitmap, HttpResponseBase>
            PrintToJpegCommand = (controllerName, actionName, bitmap, response) =>
                {
                    response.ContentType = "image/jpeg";
                    response.AddHeader("content-disposition",
                                       string.Format("attachment;filename={0}_{1}.jpg", controllerName,
                                                     actionName));

                    bitmap.Save(response.OutputStream, ImageFormat.Jpeg);
                };

        private static readonly Action<string, string, Bitmap, HttpResponseBase>
            PrintToPdfCommand = (controllerName, actionName, bitmap, response) =>
                {
                    response.ContentType = "application/pdf";
                    response.AddHeader("content-disposition",
                                       string.Format("attachment;filename={0}_{1}.pdf", controllerName,
                                                     actionName));
                    var document = new PdfDocument();
                    var page = document.AddPage();
                    page.Orientation = PageOrientation.Landscape;
                    var image = XImage.FromGdiPlusImage(bitmap);
                    page.Width = image.PointWidth;
                    page.Height = image.PointHeight;
                    var gfx = XGraphics.FromPdfPage(page);
                    gfx.DrawImage(image, 0, 0);
                    using (var memoryStream = new MemoryStream())
                    {
                        document.Save(memoryStream, false);
                        response.BinaryWrite(memoryStream.ToArray());
                    }
                };

        public static PrintResult PrintToPdf()
        {
            return new PrintResult(null, null, null, PrintToPdfCommand);
        }

        public static PrintResult PrintToPdf(object model)
        {
            return new PrintResult(null, null, model, PrintToPdfCommand);
        }

        public static PrintResult PrintToPdf(string viewName)
        {
            return new PrintResult(viewName, null, null, PrintToPdfCommand);
        }

        public static PrintResult PrintToPdf(string viewName, object model)
        {
            return new PrintResult(viewName, null, model, PrintToPdfCommand);
        }

        public static PrintResult PrintToPdf(string viewName, string masterName)
        {
            return new PrintResult(viewName, masterName, null, PrintToPdfCommand);
        }

        public static PrintResult PrintToJpeg()
        {
            return new PrintResult(null, null, null, PrintToJpegCommand);
        }

        public static PrintResult PrintToJpeg(object model)
        {
            return new PrintResult(null, null, model, PrintToJpegCommand);
        }

        public static PrintResult PrintToJpeg(string viewName)
        {
            return new PrintResult(viewName, null, null, PrintToJpegCommand);
        }

        public static PrintResult PrintToJpeg(string viewName, object model)
        {
            return new PrintResult(viewName, null, model, PrintToJpegCommand);
        }

        public static PrintResult PrintToJpeg(string viewName, string masterName)
        {
            return new PrintResult(viewName, masterName, null, PrintToJpegCommand);
        }
    }
}
