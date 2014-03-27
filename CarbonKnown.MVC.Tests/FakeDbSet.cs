using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace CarbonKnown.MVC.Tests
{
    public class FakeDbSet<T> : DbSet<T>, IQueryable<T>
        where T : class
    {
        private readonly IQueryable<T> innerQueryable;

        public FakeDbSet()
            :this(new T[]{})
        {
            
        }

        public FakeDbSet(IEnumerable<T> enumerable)
        {
            innerQueryable = enumerable.AsQueryable();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return innerQueryable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Expression Expression
        {
            get { return innerQueryable.Expression; }
        }

        public Type ElementType
        {
            get { return innerQueryable.ElementType; }
        }

        public IQueryProvider Provider
        {
            get { return innerQueryable.Provider; }
        }
    }
}
