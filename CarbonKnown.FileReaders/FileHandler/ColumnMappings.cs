using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CarbonKnown.FileReaders.FileHandler
{
    public class ColumnMappings<T>
        where T : class
    {
        protected readonly IDictionary<string, ColumnMapping> InternalMappings =
            new SortedDictionary<string, ColumnMapping>();

        public virtual void AddMapping<T2>(Expression<Func<T, T2>> memberAssignment, params string[] possibleNames)
        {
            var conversion = TryParser.CreateAssignmentAction(memberAssignment);
            AddMapping(memberAssignment, conversion, possibleNames);
        }

        public virtual void AddMapping<T2>(
            Expression<Func<T, T2>> memberAssignment,
            Action<T, object> conversion,
            params string[] possibleNames)
        {
            var memberExpression = memberAssignment.Body as MemberExpression;
            if (memberExpression == null) throw new ArgumentException("Expression body must be a MemberExpression");
            var propertyName = memberExpression.Member.Name;
            var mapping = new ColumnMapping(propertyName);
            if (InternalMappings.ContainsKey(propertyName))
            {
                mapping = InternalMappings[propertyName];
            }
            mapping.ColumnNames.AddRange(possibleNames.Except(mapping.ColumnNames));
            mapping.AssignmentAction = conversion;
            InternalMappings[propertyName] = mapping;
        }

        public virtual IEnumerable<Action<T, object>> GetConversions(params string[] providedColumns)
        {
            var conversionActions =
                from providedColumn in providedColumns
                from mappingPair in InternalMappings
                from columnName in mappingPair.Value.ColumnNames
                where string.Equals(providedColumn, columnName, StringComparison.InvariantCultureIgnoreCase)
                select mappingPair.Value.AssignmentAction;
            return conversionActions;
        }

        public virtual IDictionary<string,IEnumerable<string>> GetMissingColumns(IEnumerable<string> providedColumns)
        {
            var missingColumns =
                from mappingPair in InternalMappings
                where !mappingPair.Value.ColumnNames.Intersect(providedColumns, StringComparer.InvariantCultureIgnoreCase).Any()
                select mappingPair;
            var returnDictionary = missingColumns
                .ToDictionary(pair => pair.Key, pair => pair.Value.ColumnNames.AsEnumerable());
            return returnDictionary;
        } 

        protected sealed class ColumnMapping
        {
            public ColumnMapping(string propertyName)
            {
                ColumnNames = new List<string> {propertyName};
            }

            public List<string> ColumnNames { get; private set; }
            public Action<T, object> AssignmentAction { get; set; }
        }
    }
}
