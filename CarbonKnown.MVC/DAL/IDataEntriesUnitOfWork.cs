using System;
using System.Collections.Generic;
using CarbonKnown.DAL.Models;

namespace CarbonKnown.MVC.DAL
{
    public interface IDataEntriesUnitOfWork : IDisposable
    {
        IEnumerable<DataEntry> GetDataEntriesForSource(Guid sourceId);
        CarbonEmissionEntry AddCarbonEmissionEntry(CarbonEmissionEntry entry);
        void CommitWork();
    }
}
