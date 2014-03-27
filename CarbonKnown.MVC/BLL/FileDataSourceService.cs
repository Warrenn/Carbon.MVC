using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Source;
using CarbonKnown.FileReaders;
using CarbonKnown.MVC.Code;
using CarbonKnown.MVC.Constants;
using CarbonKnown.MVC.DAL;
using CarbonKnown.MVC.Service;
using CarbonKnown.WCF.DataSource;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace CarbonKnown.MVC.BLL
{
    public class FileDataSourceService :
        DataSourceServiceBase
    {
        private readonly IHandlerFactory handlerFactory;
        private readonly IStreamManager streamManager;
        private static readonly object UpsertFileDataSourceLock = new object();

        public FileDataSourceService(
            ISourceDataContext context,
            IHandlerFactory handlerFactory,
            IStreamManager streamManager)
            : base(context)
        {
            this.streamManager = streamManager;
            this.handlerFactory = handlerFactory;
        }

        public virtual Task<SourceResultDataContract> ExtractData(Guid sourceId)
        {
            return Task.Run(() =>
                {
                    var fileSource = Context.GetDataSource<FileDataSource>(sourceId);
                    if ((fileSource == null) || (fileSource.InputStatus != SourceStatus.PendingExtraction))
                    {
                        return DataContractError(sourceId, DataSourceServiceResources.FileSourceIsNotPendingExtraction);
                    }
                    if (Context.SourceContainsErrors(sourceId))
                    {
                        return DataContractError(sourceId, DataSourceServiceResources.SourceContainsError);
                    }
                    var newFileName = streamManager.PrepareForExtraction(sourceId, fileSource.HandlerName, fileSource.CurrentFileName);
                    fileSource.InputStatus = SourceStatus.Extracting;
                    fileSource.CurrentFileName = newFileName;
                    Context.UpdateDataSource(fileSource);
                    return DataContractSuccess(sourceId);
                });
        }

        public virtual string GetHash(Stream stream)
        {
            if ((stream == null) || (stream.Length <= 0)) return null;
            var algorithm = SHA1.Create();
            var hashBytes = algorithm.ComputeHash(stream);
            var hash = Encoding.Default.GetString(hashBytes);
            return hash;
        }

        public virtual SourceResultDataContract ValidateSource(Guid sourceId)
        {
            var fileSource = Context.GetDataSource<FileDataSource>(sourceId);
            if ((fileSource == null) || (fileSource.InputStatus != SourceStatus.PendingExtraction))
            {
                return DataContractError(sourceId, DataSourceServiceResources.FileSourceIsNotPendingExtraction);
            }
            Context.RemoveSourceErrors(sourceId);
            var returnResult = DataContractSuccess(sourceId);
            var handler = handlerFactory.CreateHandler(fileSource.HandlerName);
            if (Context.FileIsDuplicate(fileSource))
            {
                var message = DataSourceServiceResources.DuplicateFile;
                ReportSourceError(sourceId, WCF.DataSource.SourceErrorType.DuplicateFile, message);
                returnResult.Succeeded = false;
                returnResult.ErrorMessages.Add(message);
                return returnResult;
            }
            if (handler == null)
            {
                var message = string.Format(DataSourceServiceResources.InvalidHandlerMessage, fileSource.HandlerName);
                ReportSourceError(sourceId, WCF.DataSource.SourceErrorType.FileTypeNotFound, message);
                returnResult.Succeeded = false;
                returnResult.ErrorMessages.Add(message);
                return returnResult;
            }
            try
            {
                using (var fileStream = streamManager.RetrieveData(sourceId, fileSource.CurrentFileName).Result)
                {
                    foreach (var missingColumn in handler.MissingColumns(fileSource.CurrentFileName, fileStream))
                    {
                        var message = DataSourceServiceResources.MissingRowsMessage;
                        var columns = string.Join(",", missingColumn.Value);
                        message = string.Format(message, missingColumn.Key, columns);
                        ReportSourceError(sourceId, WCF.DataSource.SourceErrorType.MissingFields, message);
                        returnResult.Succeeded = false;
                        returnResult.ErrorMessages.Add(message);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, Policy.DataEntry);
                var message = DataSourceServiceResources.UnReadableFile;
                returnResult.Succeeded = false;
                returnResult.ErrorMessages.Add(message);
                ReportSourceError(sourceId, WCF.DataSource.SourceErrorType.UnReadableFile, message);
            }
            return returnResult;
        }

        public virtual Task<SourceResultDataContract> CancelFileSourceExtraction(Guid sourceId)
        {
            return Task.Run(() =>
                {
                    var fileSource = Context.GetDataSource<FileDataSource>(sourceId);
                    if ((fileSource == null) || (fileSource.InputStatus != SourceStatus.PendingExtraction))
                    {
                        return DataContractError(sourceId, DataSourceServiceResources.FileSourceIsNotPendingExtraction);
                    }
                    Context.RemoveSource(sourceId);
                    streamManager.RemoveStream(sourceId, fileSource.CurrentFileName);
                    return DataContractSuccess(sourceId);
                });
        }


        public virtual Task<SourceResultDataContract> UpsertFileDataSource(FileDataContract fileData, Stream stream)
        {
            return Task.Run(() =>
                {
                    lock (UpsertFileDataSourceLock)
                    {
                        var hash = GetHash(stream);
                        var fileSource = Context.GetFileDataSource(fileData.SourceId, hash);
                        if (fileSource == null)
                        {
                            fileSource = new FileDataSource
                                {
                                    OriginalFileName = fileData.OriginalFileName,
                                    DateEdit = DateTime.Now,
                                    InputStatus = SourceStatus.PendingExtraction,
                                    ReferenceNotes = fileData.ReferenceNotes,
                                    UserName = fileData.UserName,
                                    HandlerName = fileData.HandlerName,
                                    FileHash = hash,
                                    MediaType = fileData.MediaType
                                };
                            fileSource = Context.AddDataSource(fileSource);
                            var newFileName = streamManager
                                .StageStream(
                                    fileSource.Id,
                                    fileData.OriginalFileName,
                                    stream);
                            fileSource.CurrentFileName = newFileName;
                        }
                        if (fileSource.InputStatus != SourceStatus.PendingExtraction)
                        {
                            return DataContractError(fileSource.Id,
                                                     DataSourceServiceResources.FileSourceIsNotPendingExtraction);
                        }
                        fileSource.OriginalFileName = fileData.OriginalFileName ?? fileSource.OriginalFileName;
                        fileSource.HandlerName = fileData.HandlerName ?? fileSource.HandlerName;
                        fileSource.MediaType = fileData.MediaType ?? fileSource.MediaType;
                        fileSource.UserName = fileData.UserName;
                        fileSource.DateEdit = DateTime.Now;
                        fileSource.ReferenceNotes = fileData.ReferenceNotes ?? fileSource.ReferenceNotes;
                        Context.UpdateDataSource(fileSource);
                        return ValidateSource(fileSource.Id);
                    }
                });
        }
    }
}