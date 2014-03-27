using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CarbonKnown.FileReaders
{
    public static class TryParser
    {
        private delegate bool TryParseDelegate<T>(string stringValue, out T instance) where T : struct;

        private static readonly ConcurrentDictionary<Type, Lazy<Delegate>> TryParseMethods =
            new ConcurrentDictionary<Type, Lazy<Delegate>>();

        private static readonly string[] Formats = new[]
            {
                "dd/MM/yyyy",
                "dd/MM/yyyy HH:mm:ss",
                "dd/MM/yyyy hh:mm:ss tt",
                "dd-MM-yyyy",
                "dd-MM-yyyy hh:mm:ss tt",
                "dd-MM-yyyy HH:mm:ss",
                "yyyy/MM/dd",
                "yyyy/MM/dd hh:mm:ss tt",
                "yyyy/MM/dd HH:mm:ss",
                "yyyy-MM-dd",
                "yyyy-MM-dd hh:mm:ss tt",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-ddTHH:mm:ss",
                "o",
            };

        public static DateTime? DateTime(object value)
        {
            if (value is DateTime) return (DateTime?) value;
            var stringValue = string.Format("{0}", value);
            if (string.IsNullOrEmpty(stringValue)) return null;

            DateTime date;

            if (System.DateTime.TryParseExact(
                stringValue, Formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, out date))
                return date;

            date = new DateTime(1899, 12, 30);

            double doubleValue;
            if (double.TryParse(stringValue, out doubleValue) &&
                (doubleValue <= 2958465) &&
                (doubleValue >= -693593))
                return date.AddDays(doubleValue);
            return null;
        }

        public static T? Nullable<T>(object value)
            where T : struct
        {
            if (value is T) return (T) value;
            var stringValue = string.Format("{0}", value);
            if (string.IsNullOrEmpty(stringValue)) return null;
            T returnvalue;
            var tryParse = GetDelegate<T>(TryParseMethods);
            if ((tryParse != null) && (tryParse(stringValue, out returnvalue))) return returnvalue;
            return null;
        }

        public static Action<TClass, object> CreateAssignmentAction<TClass, T>(
            Expression<Func<TClass, T>> memberAssignment)
            where TClass : class
        {
            var memberExpression = memberAssignment.Body as MemberExpression;
            if (memberExpression == null) throw new ArgumentException("Expression body must be a MemberExpression");
            var memberType = typeof (T);
            var fooParameter = Expression.Parameter(typeof (TClass));
            var valueParameter = Expression.Parameter(typeof (T));
            var propertyInfo = typeof (TClass).GetProperty(memberExpression.Member.Name);
            var assignment = Expression.Assign(Expression.MakeMemberAccess(fooParameter, propertyInfo), valueParameter);
            var assign = Expression.Lambda<Action<TClass, T>>(assignment, fooParameter, valueParameter);
            var assignFunction = assign.Compile();
            var nullableInfo = typeof (TryParser).GetMethod("Nullable");
            Func<object, T> convertFunction = null;
            if (memberType.IsGenericType && memberType.GetGenericTypeDefinition() == typeof (Nullable<>))
            {
                var genericParameter = memberType.GenericTypeArguments[0];
                if (genericParameter != typeof (DateTime))
                {
                    var convertMethod = nullableInfo.MakeGenericMethod(genericParameter);
                    convertFunction = o => (T) convertMethod.Invoke(null, new[] {o});
                }
                else
                {
                    convertFunction = o => (T) ((object) DateTime(o));
                }
            }
            if ((convertFunction == null) && memberType.IsValueType)
            {
                var convertMethod = nullableInfo.MakeGenericMethod(memberType);
                if (memberType != typeof (DateTime))
                {
                    convertFunction = o => (T) (convertMethod.Invoke(null, new[] {o}) ?? default(T));
                }
                else
                {
                    convertFunction = o => (T) ((object) DateTime(o) ?? default(T));
                }
            }
            if (convertFunction == null)
            {
                convertFunction = o => (T) o;
            }

            Action<TClass, object> complete = (foo, o) =>
                {
                    var v = convertFunction(o);
                    assignFunction(foo, v);
                };
            return complete;
        }

        private static TryParseDelegate<T> GetDelegate<T>(ConcurrentDictionary<Type, Lazy<Delegate>> dictionary)
            where T : struct
        {
            var type = typeof (T);

            if (type.IsEnum) return Enum.TryParse<T>;

            var lazy = dictionary
                .GetOrAdd(type, new Lazy<Delegate>(() =>
                    {
                        var method =
                            type
                                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                                .FirstOrDefault(m =>
                                                (m.Name == "TryParse") &&
                                                (m.GetParameters().Count() == 2) &&
                                                (m.GetParameters()[0].ParameterType == typeof (string)) &&
                                                (m.GetParameters()[1].IsOut));
                        if (method == null) return null;
                        var returnValue =
                            Delegate.CreateDelegate(typeof (TryParseDelegate<T>), method);
                        return returnValue;
                    }));
            return (TryParseDelegate<T>) lazy.Value;
        }
    }
}
