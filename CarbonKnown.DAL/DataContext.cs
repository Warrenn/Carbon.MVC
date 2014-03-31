using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using CarbonKnown.DAL.Models;
using Calculation = CarbonKnown.DAL.Models.Calculation;

namespace CarbonKnown.DAL
{
    public class DataContext : DbContext
    {
        public DataContext()
            : base(Constant.ConnectionStringName)
        {
            Database.CommandTimeout = 0;
        }

        public virtual DbSet<ActivityGroup> ActivityGroups { get; set; }
        public virtual DbSet<CarbonEmissionEntry> CarbonEmissionEntries { get; set; }
        public virtual DbSet<CostCentre> CostCentres { get; set; }
        public virtual DbSet<DataEntry> DataEntries { get; set; }
        public virtual DbSet<DataSource> DataSources { get; set; }
        public virtual DbSet<DataError> DataErrors { get; set; }
        public virtual DbSet<Census> Census { get; set; }
        public virtual DbSet<EmissionTarget> EmissionTargets { get; set; }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }
        public virtual DbSet<Calculation> Calculations { get; set; }
        public virtual DbSet<FootNote> FootNotes { get; set; }
        public virtual DbSet<SourceError> SourceErrors { get; set; }
        public virtual DbSet<Variance> Variances { get; set; }
        public virtual DbSet<Factor> Factors { get; set; }
        public virtual DbSet<GraphSeries> GraphSeries { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual IEnumerable<ActivityGroup> ActivityGroupsTreeWalk(Guid? groupId)
        {
            var id = groupId;

            while (id != null)
            {
                var group = ActivityGroups.Find(id);
                if (group == null) yield break;
                yield return group;
                id = group.ParentGroupId;
            }
        }
        public virtual IEnumerable<CostCentre> CostCentreTreeWalk(string costCode)
        {
            var code = costCode;

            while (string.IsNullOrEmpty(code))
            {
                var centre = CostCentres.Find(code);
                if (centre == null) yield break;
                yield return centre;
                code = centre.ParentCostCentreCostCode;
            }
        }
        public virtual void SetState<T>(T entity, EntityState state) where T : class
        {
            var entry = Entry(entity);
            entry.State = state;
        }
        public new virtual DbSet<T> Set<T>() where T : class
        {
            BootStrapper.AddModel<T>();
            return base.Set<T>();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            BootStrapper.AddAppDomain();
            CreateInternalModels(modelBuilder);
            foreach (var registration in BootStrapper.GetRegistrations())
            {
                registration(modelBuilder);
            }
            base.OnModelCreating(modelBuilder);
        }
        protected virtual void CreateInternalModels(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CarbonEmissionEntry>().Property(entry => entry.Units).HasPrecision(22, 8);
            modelBuilder.Entity<CarbonEmissionEntry>().Property(entry => entry.Money).HasPrecision(22, 8);
            modelBuilder.Entity<CarbonEmissionEntry>().Property(entry => entry.CarbonEmissions).HasPrecision(22, 8);
            modelBuilder.Entity<DataEntry>().Property(entry => entry.Money).HasPrecision(22, 8);
            modelBuilder.Entity<DataEntry>().Property(entry => entry.Units).HasPrecision(22, 8);
            modelBuilder.Entity<DataEntry>()
                .HasRequired(e => e.Calculation)
                .WithMany(calculation => calculation.DataEntries)
                .HasForeignKey(entry => entry.CalculationId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<DataEntry>()
                .Property(entry => entry.PagingId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<EmissionTarget>().Property(entry => entry.TargetAmount).HasPrecision(22, 8);
            modelBuilder.Entity<EmissionTarget>().Property(entry => entry.InitialAmount).HasPrecision(22, 8);
            modelBuilder.Entity<Variance>().Property(entry => entry.MaxValue).HasPrecision(22, 8);
            modelBuilder.Entity<Variance>().Property(entry => entry.MinValue).HasPrecision(22, 8);
        }
    }
}
