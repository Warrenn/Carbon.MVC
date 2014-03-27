using System;
using System.Data.Entity;
using System.Linq;
using CarbonKnown.DAL;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Source;

namespace CarbonKnown.MVC.DAL
{
    public class SourceDataContext : ISourceDataContext
    {
        private readonly DataContext context;

        public SourceDataContext()
            : this(new DataContext())
        {

        }

        public SourceDataContext(DataContext context)
        {
            this.context = context;
        }

        public virtual IDataEntriesUnitOfWork CreateUnitOfWork()
        {
            return new DataEntriesUnitOfWork();
        }

        public virtual T AddDataSource<T>(T source) where T : DataSource
        {
            var returnValue = context.Set<T>().Add(source);
            context.SaveChanges();
            return returnValue;
        }

        public virtual T GetDataSource<T>(Guid sourceId) where T : DataSource
        {
            var returnValue = context.Set<T>().Find(sourceId);
            return returnValue;
        }

        public FileDataSource GetFileDataSource(Guid? sourceId, string hash)
        {
            if ((sourceId == null) && (string.IsNullOrEmpty(hash))) return null;
            return string.IsNullOrEmpty(hash)
                ? context.Set<FileDataSource>().Find(sourceId)
                : context.Set<FileDataSource>().FirstOrDefault(source => source.FileHash == hash);
        }

        public virtual T UpdateDataSource<T>(T source) where T : DataSource
        {
            var returnvalue = context.Set<T>().Attach(source);
            context.SetState(returnvalue, EntityState.Modified);
            context.SaveChanges();
            return returnvalue;
        }

        public virtual SourceError AddSourceError(SourceError error)
        {
            var returnValue = context.SourceErrors.Add(error);
            context.SaveChanges();
            return returnValue;
        }

        public virtual UserProfile GetUserProfile(string userName)
        {
            return context
                .UserProfiles
                .FirstOrDefault(profile => profile.UserName == userName);
        }

        public virtual void RemoveSource(Guid sourceId)
        {
            foreach (var sourceError in context.SourceErrors.Where(error => error.DataSourceId == sourceId))
            {
                context.SourceErrors.Remove(sourceError);
            }
            var fileDataSource = context.Set<FileDataSource>().FirstOrDefault(source => source.Id == sourceId);
            if (fileDataSource != null)
            {
                context.Set<FileDataSource>().Remove(fileDataSource);
            }
            var manualDataSource = context.Set<ManualDataSource>().FirstOrDefault(source => source.Id == sourceId);
            if (manualDataSource != null)
            {
                context.Set<ManualDataSource>().Remove(manualDataSource);
            }
            context.SaveChanges();
        }

        public void RemoveSourceErrors(Guid sourceId)
        {
            foreach (var sourceError in context.SourceErrors.Where(error => error.DataSourceId == sourceId))
            {
                context.SourceErrors.Remove(sourceError);
            }
            context.SaveChanges();
        }

        public virtual void RemoveSourceCalculations(Guid sourceId)
        {
            foreach (var entry in context.CarbonEmissionEntries.Where(entry => entry.SourceEntry.SourceId == sourceId))
            {
                context.CarbonEmissionEntries.Remove(entry);
            }
            context.SaveChanges();
        }

        public virtual bool SourceContainsErrors(Guid sourceId)
        {
            return context
                .Set<SourceError>()
                .Any(error => error.DataSourceId == sourceId);
        }

        public virtual bool SourceContainsDataEntriesInError(Guid sourceId)
        {
            return context
                .DataErrors
                .Include("DataEntry")
                .Any(error =>
                    ((error.DataEntry.SourceId == sourceId) &&
                     (error.ErrorType != DataErrorType.DuplicateEntry) &&
                     (error.ErrorType != DataErrorType.BelowVarianceMinimum) &&
                     (error.ErrorType != DataErrorType.AboveVarianceMaximum)));
        }

        public bool EntryContainsErrors(Guid entryId)
        {
            return context
                .DataErrors
                .Any(error =>
                    ((error.DataEntryId == entryId) &&
                     (error.ErrorType != DataErrorType.DuplicateEntry) &&
                     (error.ErrorType != DataErrorType.BelowVarianceMinimum) &&
                     (error.ErrorType != DataErrorType.AboveVarianceMaximum)));
        }

        public T GetDataEntry<T>(Guid entryId) where T : DataEntry
        {
            return context.Set<T>().Find(entryId);
        }

        public void RemoveDataErrors(Guid entryId)
        {
            foreach (var source in context.DataErrors.Where(error => error.DataEntryId == entryId))
            {
                context.DataErrors.Remove(source);
            }
            context.SaveChanges();
        }

        public T AddDataEntry<T>(T entry) where T : DataEntry
        {
            var returnValue = context.Set<T>().Add(entry);
            context.SaveChanges();
            return returnValue;
        }

        public T UpdateDataEntry<T>(T entry) where T : DataEntry
        {
            var returnvalue = context.Set<T>().Attach(entry);
            context.SetState(returnvalue, EntityState.Modified);
            context.SaveChanges();
            return returnvalue;
        }

        public DataError AddDataError(DataError error)
        {
            var returnValue = context.DataErrors.Add(error);
            context.SaveChanges();
            return returnValue;
        }

        public bool FileIsDuplicate(FileDataSource fileSource)
        {
            var hash = fileSource.FileHash;
            var sourceId = fileSource.Id;
            return context.Set<FileDataSource>().Any(source => (source.Id != sourceId) && (source.FileHash == hash));
        }

        public virtual void Dispose()
        {
            context.Dispose();
        }
    }
}