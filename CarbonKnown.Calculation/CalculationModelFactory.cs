using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using CarbonKnown.DAL.Models;
using Calc = CarbonKnown.DAL.Models.Calculation;

namespace CarbonKnown.Calculation
{
    public static class CalculationModelFactory
    {
        private static readonly Lazy<IDictionary<Type, Calc>> LazyCalculations =
            new Lazy<IDictionary<Type, Calc>>(GetCalculationsFromReflection);

        private static readonly Lazy<IDictionary<Guid, Type>> LazyEntryTypes =
            new Lazy<IDictionary<Guid, Type>>(GetEntryTypes);

        public static IDictionary<Type, Calc> Calculations
        {
            get { return LazyCalculations.Value; }
        }

        public static IDictionary<Guid, Type> EntryTypes
        {
            get { return LazyEntryTypes.Value; }
        }

        public static IEnumerable<PropertyDescriptor> GetCustomProperties(Guid calculationId)
        {
            var type = EntryTypes[calculationId];
            var descriptors =
                from descriptor in
                    TypeDescriptor
                    .GetProperties(type)
                    .Cast<PropertyDescriptor>()
                from info in type
                    .GetProperties(
                        BindingFlags.Public |
                        BindingFlags.Instance |
                        BindingFlags.DeclaredOnly)
                where descriptor.Name == info.Name
                select descriptor;
            return descriptors;
        }

        private static IDictionary<Guid, Type> GetEntryTypes()
        {
            var returnValue = new SortedDictionary<Guid, Type>();
            var calculationBaseType = typeof (CalculationBase<>);
            foreach (var calculation in Calculations)
            {
                var entryType = GetDataEntryType(calculation.Key, calculationBaseType);
                returnValue.Add(calculation.Value.Id, entryType);
            }
            return returnValue;
        }

        private static Type GetDataEntryType(Type givenType, Type genericType)
        {
            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return givenType.GetGenericArguments()[0];
            }
            return GetDataEntryType(givenType.BaseType, genericType);
        }

        private static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType))
                return true;


            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            var baseType = givenType.BaseType;
            return baseType != null && IsAssignableToGenericType(baseType, genericType);
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        private static IDictionary<Type, Calc> GetCalculationsFromReflection()
        {
            var calculationBaseType = typeof (CalculationBase<>);
            var calculationTypes =
                (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                 from type in GetLoadableTypes(assembly)
                 from attribute in type
                     .GetCustomAttributes(typeof (CalculationAttribute), false)
                     .OfType<CalculationAttribute>()
                 where
                     (attribute != null) &&
                     IsAssignableToGenericType(type, calculationBaseType)
                 select
                     new {key = type, value = attribute})
                    .ToDictionary(
                        a => a.key,
                        a => new CarbonKnown.DAL.Models.Calculation
                            {
                                Id = a.value.CalculationId,
                                ActivityGroups =
                                    a.value.ActivityIdGuids.Select(id => new ActivityGroup {Id = id}).ToList(),
                                AssemblyQualifiedName = a.key.AssemblyQualifiedName,
                                Name = a.value.Name,
                                Factors = a.value.FactorIdGuids.Select(id => new Factor {Id = id}).ToList(),
                                ConsumptionType = a.value.ConsumptionType
                            });
            return calculationTypes;
        }
    }
}
