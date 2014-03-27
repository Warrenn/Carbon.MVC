using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using CarbonKnown.Calculation.DAL;
using CarbonKnown.Calculation.Models;
using CarbonKnown.Calculation.Properties;
using CarbonKnown.DAL.Models;

namespace CarbonKnown.Calculation
{
    public abstract class CalculationBase<T> : ICalculation where T : DataEntry
    {
        protected readonly ICalculationDataContext Context;

        private static readonly Lazy<IEnumerable<PropertyDescriptor>> properties =
            new Lazy<IEnumerable<PropertyDescriptor>>(GetProperties);

        private static readonly Lazy<IDictionary<string, PropertyDescriptor>> nullableProperties =
            new Lazy<IDictionary<string, PropertyDescriptor>>(GetNullableProperties);

        protected CalculationBase(ICalculationDataContext context)
        {
            Context = context;
            CanBeNull(arg => arg.Units);
            CanBeNull(arg => arg.RowNo);
        }

        public static IEnumerable<PropertyDescriptor> Properties
        {
            get { return properties.Value; }
        }

        public string ClassName
        {
            get { return typeof (T).Name; }
        }

        private static IEnumerable<PropertyDescriptor> GetProperties()
        {
            var entryType = typeof (T);
            return TypeDescriptor
                .GetProperties(entryType)
                .Cast<PropertyDescriptor>();
        }

        private static IDictionary<string, PropertyDescriptor> GetNullableProperties()
        {
            return Properties
                .Where(d =>
                       (d.PropertyType.IsGenericType &&
                        (d.PropertyType.GetGenericTypeDefinition() == typeof (Nullable<>))) ||
                       (d.PropertyType == typeof (string)))
                .ToDictionary(d => d.Name, d => d);
        }

        protected internal void CanBeNull<T2>(Expression<Func<T, T2>> propertyExpression)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null) throw new ArgumentException("Expression must be a MemberExpression");
            var name = memberExpression.Member.Name;
            if (!nullableProperties.Value.ContainsKey(name)) return;
            nullableProperties.Value.Remove(name);
        }

        public CalculationResult CalculateEmission(DateTime effectiveDate, DailyData dailyData, DataEntry entry)
        {
            return CalculateEmission(effectiveDate, dailyData, entry as T);
        }

        public abstract CalculationResult CalculateEmission(DateTime effectiveDate, DailyData dailyData, T entry);
        
        public virtual IEnumerable<DataError> ValidateEntry(DataEntry entry)
        {
            return ValidateEntry(entry as T);
        }

        public virtual IEnumerable<DataError> ValidateEntry(T entry)
        {
            var extractType = typeof (T);
            foreach (var keyValue in nullableProperties.Value)
            {
                if (keyValue.Value.GetValue(entry) != null) continue;
                yield return new DataError
                    {
                        Column = keyValue.Key,
                        ErrorType = DataErrorType.MissingValue,
                        Message = string.Format(Resources.MissingValueMessage, keyValue.Key)
                    };
            }

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(extractType))
            {
                if ((descriptor.PropertyType != typeof (decimal?)) && 
                    (descriptor.PropertyType != typeof (decimal)))
                    continue;
                var value = descriptor.GetValue(entry) as decimal?;
                if(value == null) continue;
                var variance = Context.Variance(entry.CalculationId, descriptor.Name);
                if (variance == null) continue;
                if (value < variance.MinValue)
                {
                    var error = new DataError
                        {
                            Column = descriptor.Name,
                            ErrorType = DataErrorType.BelowVarianceMinimum,
                            Message = string.Format(Resources.BelowVarianceMinMessage, descriptor.Name, variance.MinValue),
                            DataEntryId = entry.Id
                        };
                    yield return error;
                }
                if (value > variance.MaxValue)
                {
                    var error = new DataError
                    {
                        Column = descriptor.Name,
                        ErrorType = DataErrorType.BelowVarianceMinimum,
                        Message = string.Format(Resources.AboveVarianceMaxMessage, descriptor.Name, variance.MaxValue),
                        DataEntryId = entry.Id
                    };
                    yield return error;
                }
            }

            if (entry.StartDate > entry.EndDate)
            {
                yield return new DataError
                    {
                        Column = "StartDate",
                        DataEntryId = entry.Id,
                        ErrorType = DataErrorType.StartDateGreaterThanEndDate,
                        Message = Resources.StartDateGreaterThanEndDateMessage
                    };
                yield return new DataError
                    {
                        Column = "EndDate",
                        DataEntryId = entry.Id,
                        ErrorType = DataErrorType.StartDateGreaterThanEndDate,
                        Message = Resources.StartDateGreaterThanEndDateMessage
                    };
            }

            if (!Context.CostCodeValid(entry.CostCode))
            {
                yield return new DataError
                    {
                        Column = "CostCode",
                        DataEntryId = entry.Id,
                        ErrorType = DataErrorType.InvalidCostCode,
                        Message = Resources.InvalidCostCodeMessage
                    };
            }

            if (Context.EntryIsDuplicate(entry.Id, entry.Hash))
            {
                yield return new DataError
                    {
                        Column = string.Empty,
                        DataEntryId = entry.Id,
                        ErrorType = DataErrorType.DuplicateEntry,
                        Message = Resources.DuplicateMessage
                    };
            }
        }

        public virtual int GetDayDifference(DataEntry entry)
        {
            return GetDayDifference(entry as T);
        }

        public virtual int GetDayDifference(T entry)
        {
            if (!entry.EndDate.HasValue) return 0;
            if (!entry.StartDate.HasValue) return 0;
            if (entry.EndDate.Value < entry.StartDate.Value) return 0;
            var endDate = entry.EndDate.Value.Date;
            var startDate = entry.StartDate.Value.Date;
            var span = endDate.Subtract(startDate);
            return span.Days;
        }

        public decimal GetFactorValue(Guid factorId, DateTime calculationDate)
        {
            var factorValue = Context.FactorValue(calculationDate, factorId);
            if (factorValue == null)
            {
                var message = string.Format(Resources.FactorValueNotFound, factorId, calculationDate);
                throw new NullReferenceException(message);
            }
            return factorValue.Value;
        }

        public decimal GetFactorValue(string factorName, DateTime calculationDate)
        {
            var factorValue = Context.FactorValue(calculationDate, factorName);
            if (factorValue == null)
            {
                var message = string.Format(Resources.FactorValueNotFound, factorName, calculationDate);
                throw new NullReferenceException(message);
            }
            return factorValue.Value;
        }

        public virtual DailyData CalculateDailyData(DataEntry entry)
        {
            return CalculateDailyData(entry as T);
        }

        public virtual DailyData CalculateDailyData(T entry)
        {
            var days = GetDayDifference(entry) + 1;
            return new DailyData
                {
                    MoneyPerDay = entry.Money/days,
                    UnitsPerDay = entry.Units/days
                };
        }
    }
}
