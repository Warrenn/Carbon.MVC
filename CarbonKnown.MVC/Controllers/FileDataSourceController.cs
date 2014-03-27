using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CarbonKnown.DAL.Models.Source;
using CarbonKnown.MVC.BLL;
using CarbonKnown.MVC.Code;
using CarbonKnown.MVC.DAL;
using CarbonKnown.WCF.DataSource;

namespace CarbonKnown.MVC.Controllers
{
    [RoutePrefix("api/file")]
    [Authorize(Roles = "Admin,Capturer")]
    public class FileDataSourceController : ApiController
    {
        private readonly ISourceDataContext context;
        private readonly FileDataSourceService service;
        private readonly IStreamManager streamManager;

        public FileDataSourceController(
            ISourceDataContext context,
            FileDataSourceService service,
            IStreamManager streamManager)
        {
            this.context = context;
            this.service = service;
            this.streamManager = streamManager;
        }

        [HttpPost]
        [Route("upload", Name = "UploadFile")]
        [ResponseType(typeof (SourceResultDataContract))]
        public virtual async Task<IHttpActionResult> UploadFile()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new Exception();
            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);
            var fileInfo = new FileDataContract {UserName = User.Identity.Name};
            Stream buffer = null;
            foreach (var content in provider.Contents)
            {
                var name = content.Headers.ContentDisposition.Name.Trim('\"');
                if (string.Equals(name, "referenceNotes", StringComparison.InvariantCultureIgnoreCase))
                {
                    fileInfo.ReferenceNotes = await content.ReadAsStringAsync();
                    continue;
                }
                if (string.Equals(name, "fileHandler", StringComparison.InvariantCultureIgnoreCase))
                {
                    fileInfo.HandlerName = await content.ReadAsStringAsync();
                    continue;
                }
                if (string.Equals(name, "mediaType", StringComparison.InvariantCultureIgnoreCase))
                {
                    fileInfo.MediaType = await content.ReadAsStringAsync();
                    continue;
                }
                if (!string.Equals(name, "file", StringComparison.InvariantCultureIgnoreCase)) continue;
                fileInfo.OriginalFileName = content.Headers.ContentDisposition.FileName.Trim('\"');
                buffer = await content.ReadAsStreamAsync();
            }
            var result = await service.UpsertFileDataSource(fileInfo, buffer);

            return Ok(result);
        }


        [HttpPost]
        [Route("cancel/{sourceId}", Name = "CancelFile")]
        [ResponseType(typeof(SourceResultDataContract))]
        public async Task<IHttpActionResult> CancelFileSourceExtraction(Guid sourceId)
        {
            var result = await service.CancelFileSourceExtraction(sourceId);
            return Ok(result);
        }

        [HttpPost]
        [Route("extract/{sourceId}", Name = "ExtractData")]
        [ResponseType(typeof(SourceResultDataContract))]
        public async Task<IHttpActionResult> ExtractData(Guid sourceId)
        {
            var result = await service.ExtractData(sourceId);
            return Ok(result);
        }

        [HttpGet]
        [Route("download/{sourceId}", Name = "filedownload")]
        public virtual async Task<IHttpActionResult> Download(Guid sourceId)
        {
            var fileSource = context.GetDataSource<FileDataSource>(sourceId);
            if (fileSource == null)
            {
                return NotFound();
            }
            var memoryStream = await streamManager.RetrieveData(sourceId, fileSource.CurrentFileName);
            var contentLength = memoryStream.Length;

            MediaTypeHeaderValue mediaType;
            MediaTypeHeaderValue.TryParse(fileSource.MediaType, out mediaType);
            var contentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = fileSource.OriginalFileName,
                    Size = contentLength
                };
            var result = new FileActionResult(
                Request,
                memoryStream,
                mediaType,
                memoryStream.Length,
                contentDisposition);
            return result;
        }
    }
}