using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using CarbonKnown.FileReaders.Constants;
using CarbonKnown.FileReaders.Properties;
using CarbonKnown.FileReaders.Readers;
using CarbonKnown.WCF.DataEntry;
using CarbonKnown.WCF.DataSource;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace CarbonKnown.FileReaders.FileHandler
{
    public abstract class FileHandlerBase<TDataContract> : IFileHandler
        where TDataContract : DataEntryDataContract, new()
    {
        protected readonly string Host;
        protected IFileReader FileReader = null;

        private readonly ConcurrentDictionary<Type, Lazy<IDisposable>> serviceWrappers =
            new ConcurrentDictionary<Type, Lazy<IDisposable>>();

        public Func<string, Guid, IFileReader> GetReader;

        protected internal readonly IEnumerable<string> DefaultCostCodeColumns = new[]
            {
                "SENDERREFERENCE",
                "SENDER REFERENCE",
                "SENDER_REFERENCE",
                "SENDER-REFERENCE",
                "COSTCENTRE",
                "COST_CENTRE",
                "COST-CENTRE",
                "COST CENTRE",
                "COSTCENTER",
                "COST_CENTER",
                "COST-CENTER",
                "COST CENTER"
            };

        protected internal readonly IDictionary<string, Func<IFileReader>> FileReaders = new
            SortedDictionary<string, Func<IFileReader>>
            {
                {".xls", () => new XlsFileReader()},
                {".xlsx", () => new XlsxFileReader()},
                {".csv", () => new CsvFileReader()},
            };

        protected ColumnMappings<TDataContract> ColumnMappings =
            new ColumnMappings<TDataContract>();

        protected FileHandlerBase()
            : this(string.Empty)
        {
        }

        protected FileHandlerBase(string host)
        {
            Host = host;
            GetReader = GetReaderFromType;
        }

        public virtual void CallService<T>(Action<T> serviceCall) where T : class
        {
            try
            {
                var service = GetService<T>();
                serviceCall(service);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, PolicyName.Default);
            }
        }

        public virtual T GetService<T>() where T : class
        {
            var key = typeof (T);
            var lazy = serviceWrappers
                .GetOrAdd(key, new Lazy<IDisposable>(() => new ClientServiceWrapper<T>(Host)));
            return ((ClientServiceWrapper<T>) lazy.Value).Service;
        }

        protected void MapStartDateColumns(params string[] columns)
        {
            ColumnMappings
                .AddMapping(
                    contract => contract.StartDate,
                    (c, o) => c.StartDate = TryParser.DateTime(o),
                    columns);
        }

        protected void MapEndDateColumns(params string[] columns)
        {
            ColumnMappings
                .AddMapping(
                    contract => contract.EndDate,
                    (c, o) => c.EndDate = TryParser.DateTime(o),
                    columns);
        }

        protected void MapMoneyColumns(params string[] columns)
        {
            ColumnMappings
                .AddMapping(
                    contract => contract.Money,
                    (c, o) => c.Money = TryParser.Nullable<decimal>(o),
                    columns);
        }

        protected void MapUnitsColumns(params string[] columns)
        {
            ColumnMappings
                .AddMapping(
                    contract => contract.Units,
                    (c, o) => c.Units = TryParser.Nullable<decimal>(o),
                    columns);
        }

        protected void MapCostCodeColumns(params string[] columns)
        {
            if ((columns == null) || (columns.Length == 0))
            {
                columns = DefaultCostCodeColumns.ToArray();
            }

            ColumnMappings
                .AddMapping(
                    contract => contract.CostCode,
                    (c, o) =>
                        {
                            var number = TryParser.Nullable<decimal>(o);
                            if (number == null)
                            {
                                c.CostCode = string.Format("{0}", o).Trim();
                            }
                            else
                            {
                                number = Math.Floor(number.Value);
                                c.CostCode = string.Format("{0:000000000}", number);
                            }
                        },
                    columns);
        }

        protected void MapColumns<T>(
            Expression<Func<TDataContract, T>> memberAssignment, 
            params string[] columns)
        {
            ColumnMappings.AddMapping(memberAssignment, columns);
        }

        public virtual void MapColumns<T>(
            Expression<Func<TDataContract, T>> memberAssignment,
            Action<TDataContract, object> conversion,
            params string[] possibleNames)
        {
            ColumnMappings.AddMapping(memberAssignment, conversion, possibleNames);
        }

        public static Guid GetSourceId(string fullPath)
        {
            var name = Path.GetFileNameWithoutExtension(fullPath) ?? string.Empty;
            Guid sourceId;
            Guid.TryParse(name, out sourceId);
            return sourceId;
        }

        public virtual TDataContract Convert(TDataContract contract, IDictionary<string, object> entry)
        {
            foreach (var pair in entry)
            {
                foreach (var conversion in ColumnMappings.GetConversions(pair.Key))
                {
                    try
                    {
                        conversion(contract, pair.Value);
                    }
                    catch (Exception ex)
                    {
                        ExceptionPolicy.HandleException(ex, PolicyName.FileHandlerConvert);
                    }
                }
            }
            return contract;
        }

        public virtual void ExtractCompleted(Guid sourceId)
        {
            CallService<IDataSourceService>(service => service.ExtractCompleted(sourceId));
        }

        public virtual void ReportError(Guid sourceId, CarbonKnown.WCF.DataSource.SourceErrorType errorType, string errorMessage)
        {
            CallService<IDataSourceService>(service => service.ReportSourceError(sourceId, errorType, errorMessage));
        }

        public virtual IDictionary<string, IEnumerable<string>> MissingColumns(string fullPath, Stream fileStream)
        {
            var sourceId = GetSourceId(fullPath);
            FileReader = GetReader(fullPath, sourceId);
            if (FileReader == null) return new Dictionary<string, IEnumerable<string>>();
            using (FileReader)
            {
                var firstRow = FileReader.ExtractData(fileStream).FirstOrDefault();
                return firstRow == null ? 
                    new Dictionary<string, IEnumerable<string>>() : 
                    ColumnMappings.GetMissingColumns(firstRow.Keys);
            }
        }

        public virtual void ReportError(string fullPath, Exception exception)
        {
            var sourceId = GetSourceId(fullPath);
            ReportError(sourceId, CarbonKnown.WCF.DataSource.SourceErrorType.ExceptionOccured, exception.Message);
        }

        public abstract void UpsertDataEntry(TDataContract contract);

        public virtual void ProcessFile(string fullPath, Stream stream)
        {
            var sourceId = GetSourceId(fullPath);
            FileReader = GetReader(fullPath, sourceId);
            if (FileReader == null) return;
            using (FileReader)
            {
                var rowNumber = 1;
                foreach (var entry in FileReader.ExtractData(stream))
                {
                    var contract = new TDataContract
                        {
                            SourceId = sourceId,
                            RowNo = rowNumber,
                        };
                    contract = Convert(contract, entry);
                    UpsertDataEntry(contract);
                    rowNumber++;
                }
                ProcessComplete(fullPath);
            }
        }

        public virtual IFileReader GetReaderFromType(string fullPath,Guid sourceId)
        {
            var extension = Path.GetExtension(fullPath);
            if ((!FileReaders.ContainsKey(extension)) || (FileReaders[extension] == null))
            {
                var message = string.Format(Resources.ExtensionNotFoundErrorMessage, extension);
                ReportError(sourceId, CarbonKnown.WCF.DataSource.SourceErrorType.FileTypeNotFound, message);
                ProcessComplete(fullPath);
                return null;
            }
            return FileReaders[extension]();
        }

        public virtual void Dispose()
        {
            FileReader.Dispose(PolicyName.Disposable);
            foreach (var serviceWrapper in serviceWrappers)
            {
                serviceWrapper.Value.Value.Dispose(PolicyName.Disposable);
            }
        }
        
        public virtual void ProcessComplete(string fullPath)
        {
            var sourceId = GetSourceId(fullPath);
            ExtractCompleted(sourceId);
        }
    }
}
