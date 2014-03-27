using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace CarbonKnown.DAL.Models
{
    public class DataEntry
    {
        [Key]
        public Guid Id { get; set; }

        public int PagingId { get; set; }
        public DateTime EditDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CostCode { get; set; }
        public decimal? Money { get; set; }
        public decimal? Units { get; set; }
        public virtual DataSource Source { get; set; }
        public Guid SourceId { get; set; }
        public string UserName { get; set; }
        public int? RowNo { get; set; }
        public virtual ICollection<DataError> Errors { get; set; }
        public Guid CalculationId { get; set; }
        public virtual Calculation Calculation { get; set; }

        public string EntryType
        {
            get { return NonProxyType().Name; }
            set { }
        }

        public Type NonProxyType()
        {
            var entityType = GetType();
            if ((entityType.BaseType != null) && (entityType.Namespace == "System.Data.Entity.DynamicProxies"))
            {
                return entityType.BaseType;
            }
            return entityType;
        }

        public int Hash
        {
            get
            {
                Func<object, int> nullHash = obj => (obj == null) ? 0 : obj.GetHashCode();
                var returnValue = 17;
                returnValue = returnValue*23 + nullHash(StartDate);
                returnValue = returnValue*23 + nullHash(EndDate);
                returnValue = returnValue*23 + nullHash(CostCode);
                returnValue = returnValue*23 + nullHash(Money);
                returnValue = returnValue*23 + nullHash(Units);
                if (typeof(DataEntry) == NonProxyType()) return returnValue;
                Func<PropertyInfo, int> propertyHash = info =>
                    {
                        var value = info.GetValue(this);
                        return nullHash(value);
                    };
                unchecked
                {
                    returnValue = NonProxyType()
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                        .Where(property => property.CanRead && property.GetValue(this) != null)
                        .Aggregate(returnValue, (current, property) => current*23 + propertyHash(property));
                    return returnValue;
                }
            }
            set { }
        }
    }
}