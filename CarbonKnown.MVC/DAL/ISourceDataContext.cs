using System;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Source;

namespace CarbonKnown.MVC.DAL
{
    public interface ISourceDataContext : IDisposable
    {
        IDataEntriesUnitOfWork CreateUnitOfWork();
        T AddDataSource<T>(T source) where T : DataSource;
        T GetDataSource<T>(Guid sourceId) where T : DataSource;
        FileDataSource GetFileDataSource(Guid? sourceId, string hash);
        T UpdateDataSource<T>(T source) where T : DataSource;
        SourceError AddSourceError(SourceError error);
        UserProfile GetUserProfile(string userName);
        void RemoveSource(Guid sourceId);
        void RemoveSourceErrors(Guid sourceId);
        void RemoveSourceCalculations(Guid sourceId);
        bool SourceContainsErrors(Guid sourceId);
        bool SourceContainsDataEntriesInError(Guid sourceId);
        bool EntryContainsErrors(Guid entryId);
        T GetDataEntry<T>(Guid entryId) where T : DataEntry;
        void RemoveDataErrors(Guid entryId);
        T AddDataEntry<T>(T entry) where T : DataEntry;
        T UpdateDataEntry<T>(T entry) where T : DataEntry;
        DataError AddDataError(DataError error);
        bool FileIsDuplicate(FileDataSource fileSource);
    }
}
