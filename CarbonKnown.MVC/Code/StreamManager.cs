using System;
using System.IO;
using System.Threading.Tasks;
using CarbonKnown.MVC.Properties;

namespace CarbonKnown.MVC.Code
{
    public class StreamManager : IStreamManager
    {
        private readonly UploadConfig uploadConfig = UploadConfig.Instance;

        public string StageStream(Guid sourceId, string originalFileName, Stream stream)
        {
            var path = uploadConfig.StagingFolder;
            var extension = Path.GetExtension(originalFileName);
            var fileName = string.Format("{0}{1}", sourceId, extension);
            var fullPath = Path.Combine(path, fileName);
            stream.Position = 0;
            using (var fileStream = File.Open(fullPath, FileMode.OpenOrCreate))
            {
                stream.CopyTo(fileStream);
            }
            return fullPath;
        }

        public string PrepareForExtraction(Guid sourceId, string handlerName, string currentFileName)
        {
            if (!File.Exists(currentFileName)) return currentFileName;
            if (string.IsNullOrEmpty(handlerName)) throw new ArgumentNullException("handlerName");
            var fileType = uploadConfig.FileTypes.GetItemByKey(handlerName);
            if (fileType == null)
                throw new ArgumentException(string.Format(Resources.HandlerNotFoundErrorMessage, handlerName));
            var newFileName = currentFileName;
            var fileName = Path.GetFileName(currentFileName);
            if (!string.IsNullOrEmpty(fileName))
            {
                newFileName = Path.Combine(fileType.Folder, fileName);
                File.Copy(currentFileName, newFileName, true);
                File.Delete(currentFileName);
            }
            return newFileName;
        }

        public void RemoveStream(Guid sourceId, string currentFileName)
        {
            if (!File.Exists(currentFileName)) return;
            File.Delete(currentFileName);
        }

        public async Task<MemoryStream> RetrieveData(Guid sourceId, string currentFileName)
        {
            if (!File.Exists(currentFileName)) return (MemoryStream) Stream.Null;
            using (var sourceStream =
                new FileStream(
                    currentFileName,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    4096,
                    true))
            {
                var destination = new MemoryStream();
                await sourceStream.CopyToAsync(destination);
                return destination;
            }
        }
    }
}