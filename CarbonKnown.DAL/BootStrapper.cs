using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace CarbonKnown.DAL
{
    public static class BootStrapper
    {
        private static readonly Lazy<ConcurrentDictionary<Type, Action<DbModelBuilder>>> ModelBuilders = new Lazy
            <ConcurrentDictionary<Type, Action<DbModelBuilder>>>(
            () => new ConcurrentDictionary<Type, Action<DbModelBuilder>>(), LazyThreadSafetyMode.ExecutionAndPublication);
        private static readonly Lazy<Type[]> EntityTypesLazy = new Lazy<Type[]>(GetEntityTypes); 
        private static readonly Lazy<Type[]> DerivedTypesLazy = new Lazy<Type[]>(GetDerivedTypes); 

        
        public static Type[] EntityTypes
        {
            get { return EntityTypesLazy.Value; }
        }

        public static Type[] DerivedTypes
        {
            get { return DerivedTypesLazy.Value; }
        }

        public static void AddAppDomain()
        {
            var derivedTypes = DerivedTypes;
            var addModelMethod = typeof (BootStrapper).GetMethod("AddModel", BindingFlags.Static | BindingFlags.Public);
            foreach (var genericMethod in derivedTypes.Select(type => addModelMethod.MakeGenericMethod(type)))
            {
                genericMethod.Invoke(null, null);
            }
        }

        private static Type[] GetDerivedTypes()
        {
            return 
                (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                 from type in GetLoadableTypes(assembly)
                 from entityType in EntityTypes
                 where
                     (entityType != null) &&
                     (type != null) &&
                     (entityType != type) &&
                     entityType.IsAssignableFrom(type)
                 select type).ToArray();
        }

        private static Type[] GetEntityTypes()
        {
            return
                (from property in typeof (DataContext).GetProperties()
                 let propertyType = property.PropertyType
                 where propertyType.GUID == (typeof (DbSet<>).GUID)
                 select propertyType.GenericTypeArguments[0]).ToArray();
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

        public static void AddModel<T>() where T : class
        {
            var typeKey = typeof (T);
            var builders = ModelBuilders.Value;
            if (builders.ContainsKey(typeKey)) return;
            Action<DbModelBuilder> build = builder => builder.Entity<T>().ToTable(typeKey.Name);
            ModelBuilders.Value.TryAdd(typeKey, build);
        }

        public static void AddConfiguration<T>(EntityTypeConfiguration<T> configuration) where T : class
        {
            var typeKey = typeof (T);
            var builders = ModelBuilders.Value;
            if (builders.ContainsKey(typeKey)) return;
            Action<DbModelBuilder> build = builder => builder.Configurations.Add(configuration);
            ModelBuilders.Value.TryAdd(typeKey, build);
        }

        public static void AddBuild<T>(Action<DbModelBuilder> build) where T : class
        {
            var typeKey = typeof (T);
            var builders = ModelBuilders.Value;
            if (builders.ContainsKey(typeKey)) return;
            ModelBuilders.Value.TryAdd(typeKey, build);
        }

        public static void AddAssembly(Assembly assembly)
        {
            var configTypes =
                (from type in assembly.GetTypes()
                 where
                     (type.BaseType != null) &&
                     (type.BaseType.IsGenericType) &&
                     (type.BaseType.GetGenericTypeDefinition() == typeof (EntityTypeConfiguration<>))
                 select type);
            var addConfigMethod = typeof (BootStrapper).GetMethod("AddConfiguration",
                                                                  BindingFlags.Public | BindingFlags.Static);

            foreach (var configType in configTypes)
            {
                var entityConfig = Activator.CreateInstance(configType);
                var modelType = configType.GenericTypeArguments[0];
                addConfigMethod
                    .MakeGenericMethod(modelType)
                    .Invoke(null, new[] {entityConfig});
            }
        }

        internal static IEnumerable<Action<DbModelBuilder>> GetRegistrations()
        {
            return ModelBuilders.Value.Values.AsEnumerable();
        }
    }
}
