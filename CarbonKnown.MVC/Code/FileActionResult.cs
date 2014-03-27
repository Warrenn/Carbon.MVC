using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace CarbonKnown.MVC.Code
{
    public class FileActionResult : IHttpActionResult
    {
        private readonly HttpRequestMessage request;
        private readonly MemoryStream content;
        private readonly MediaTypeHeaderValue contentType;
        private readonly long? contentLength;
        private readonly ContentDispositionHeaderValue contentDisposition;

        public FileActionResult(
            HttpRequestMessage request,
            MemoryStream content,
            MediaTypeHeaderValue contentType,
            long? contentLength,
            ContentDispositionHeaderValue contentDisposition)
        {
            this.request = request;
            this.content = content;
            this.contentType = contentType;
            this.contentLength = contentLength;
            this.contentDisposition = contentDisposition;
        }
        
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response;
            content.Position = 0;
            if (request.Headers.Range != null)
            {
                try
                {
                    response = request.CreateResponse(HttpStatusCode.PartialContent);
                    response.Content = new ByteRangeStreamContent(content, request.Headers.Range, contentType);
                }
                catch (InvalidByteRangeException invalidByteRangeException)
                {
                    response = request.CreateErrorResponse(invalidByteRangeException);
                    return Task.FromResult(response);
                }
            }
            else
            {
                response = request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StreamContent(content);
            }

            if (contentLength.HasValue)
            {
                response.Content.Headers.ContentLength = contentLength;
            }
            response.Content.Headers.ContentType = contentType;
            response.Content.Headers.ContentDisposition = contentDisposition;

            return Task.FromResult(response);
        }
    }
}