using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CarbonKnown.FileReaders.Constants;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.PolicyInjection;
using Microsoft.Practices.EnterpriseLibrary.Logging.PolicyInjection;

namespace CarbonKnown.FileReaders.FileHandler
{
    public interface IFileHandler : IDisposable
    {
        [LogCallHandler(
            Categories = new[] {PolicyName.FileHandler},
            Severity = TraceEventType.Information,
            LogAfterCall = false)]
        [ExceptionCallHandler(PolicyName.FileHandlerProcessFile)]
        void ProcessFile(string fullPath, Stream stream);

        [LogCallHandler(
            Categories = new[] {PolicyName.FileHandler},
            Severity = TraceEventType.Information,
            LogAfterCall = false)]
        [ExceptionCallHandler(PolicyName.FileHandlerMissingColumns)]
        IDictionary<string, IEnumerable<string>> MissingColumns(string fullPath, Stream fileStream);

        [LogCallHandler(
            Categories = new[] {PolicyName.FileHandler},
            Severity = TraceEventType.Information,
            LogAfterCall = false)]
        [ExceptionCallHandler(PolicyName.ProcessComplete)]
        void ProcessComplete(string fullPath);
        
        [LogCallHandler(
            Categories = new[] {PolicyName.FileHandler},
            Severity = TraceEventType.Information,
            LogAfterCall = false)]
        [ExceptionCallHandler(PolicyName.FileHandlerReport)]
        void ReportError(string fullPath, Exception exception);
    }
}
