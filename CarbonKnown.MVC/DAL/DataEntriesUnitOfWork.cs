using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CarbonKnown.Calculation;
using CarbonKnown.DAL;
using CarbonKnown.DAL.Models;

namespace CarbonKnown.MVC.DAL
{
    public class DataEntriesUnitOfWork : IDataEntriesUnitOfWork
    {
        private readonly DataContext context;
        private bool commit;

        private static readonly ConcurrentDictionary<Type, Lazy<MethodInfo>> MethodInfoLookup =
            new ConcurrentDictionary<Type, Lazy<MethodInfo>>();

        private static readonly Lazy<MethodInfo> FindMethodInfoLazy =
            new Lazy<MethodInfo>(() => typeof (DataEntriesUnitOfWork).GetMethod("FindEntry"));

        public DataEntriesUnitOfWork()
            : this(() => new DataContext())
        {

        }

        public DataEntriesUnitOfWork(Func<DataContext> factory)
        {
            context = factory();
        }

        private DataEntry FindEntryForType(Type type, Guid entryId)
        {
            var methodInfo =MethodInfoLookup
                .GetOrAdd(type, t => new Lazy<MethodInfo>(() => FindMethodInfoLazy.Value.MakeGenericMethod(t)));
            var entry = methodInfo.Value.Invoke(this, new object[] {entryId});
            return entry as DataEntry;
        }

        public T FindEntry<T>(Guid entryId) where T : DataEntry
        {
            return context.Set<T>().Find(entryId);
        }


        public virtual void Dispose()
        {
            if (context == null) return;
            try
            {
                if (commit)
                {
                    context.SaveChanges();
                }
            }
            finally
            {
                context.Dispose();
            }
        }

        public IEnumerable<DataEntry> GetDataEntriesForSource(Guid sourceId)
        {
            foreach (var dataEntry in context
                .Set<DataEntry>()
                .Where(entry => entry.SourceId == sourceId))
            {
                var calculationId = dataEntry.CalculationId;
                if (!CalculationModelFactory.EntryTypes.Keys.Contains(calculationId))
                {
                    yield return dataEntry;
                    continue;
                }
                var type = CalculationModelFactory.EntryTypes[calculationId];
                var castedEntry = FindEntryForType(type, dataEntry.Id);
                if (castedEntry == null)
                {
                    yield return dataEntry;
                    continue;
                }
                yield return castedEntry;
            }
        }

        public T GetDataEntry<T>(Guid entryId) where T : DataEntry
        {
            return context.Set<T>().Find(entryId);
        }

        public T CreateDataEntry<T>() where T : DataEntry
        {
            return context.Set<T>().Create();
        }

        public CarbonEmissionEntry AddCarbonEmissionEntry(CarbonEmissionEntry entry)
        {
            return context.CarbonEmissionEntries.Add(entry);
        }

        public void RemoveDataErrors(Guid entryId)
        {
            foreach (var error in context.DataErrors.Where(error => error.DataEntryId == entryId))
            {
                context.DataErrors.Remove(error);
            }
        }

        public void AddDataError(DataError error)
        {
            context.DataErrors.Add(error);
        }


        public void CommitWork()
        {
            commit = true;
        }
    }
}