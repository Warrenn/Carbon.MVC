using System;
using CarbonKnown.MVC.DAL;

namespace CarbonKnown.MVC.Service
{
    public class ServiceBase : IDisposable
    {
        protected readonly ISourceDataContext Context;

        public ServiceBase(ISourceDataContext context)
        {
            Context = context;
        }

        public virtual void Dispose()
        {
            if (Context == null) return;
            Context.Dispose();
        }
    }
}