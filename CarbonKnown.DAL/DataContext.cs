using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Source;
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

        public virtual IEnumerable<SliceTotal>
            TotalsByCostCentre(
            DateTime startDate,
            DateTime endDate,
            Guid? groupId,
            string costCode)
        {
            var query =
                from a in ActivityGroups
                where a.ParentGroupId == groupId
                from c in CostCentres
                where c.ParentCostCentreCostCode == costCode
                from e in CarbonEmissionEntries
                where
                    (e.EntryDate >= startDate) &&
                    (e.EntryDate <= endDate) &&
                    (e.ActivityGroupNode.IsDescendantOf(a.Node) ||
                     (e.ActivityGroupNode == a.Node)) &&
                    (e.CostCentreNode.IsDescendantOf(c.Node) ||
                     (e.CostCentreNode == c.Node))
                group new
                {
                    units = e.Units,
                    emissions = e.CarbonEmissions
                } by c.CostCode
                into g
                select new SliceTotal
                {
                    ActivityGroupId = groupId,
                    CostCode = g.Key,
                    TotalCarbonEmissions = g.Sum(arg => arg.emissions),
                    TotalUnits = g.Sum(arg => arg.units)
                };
            return query;
        }

        public virtual IEnumerable<AverageData>
            AverageCo2(
            DateTime startDate,
            DateTime endDate,
            Guid? groupId,
            string costCode)
        {
            var query =
                from a in ActivityGroups
                where a.ParentGroupId == groupId
                from c in CostCentres
                where c.ParentCostCentreCostCode == costCode
                from e in CarbonEmissionEntries
                where
                    (e.EntryDate >= startDate) &&
                    (e.EntryDate <= endDate) &&
                    (e.ActivityGroupNode.IsDescendantOf(a.Node) ||
                     (e.ActivityGroupNode == a.Node)) &&
                    (e.CostCentreNode.IsDescendantOf(c.Node) ||
                     (e.CostCentreNode == c.Node))
                group e.CarbonEmissions by ((e.EntryDate.Year*100) + (e.EntryDate.Month))
                into g
                select new AverageData
                {
                    Average = g.Average(),
                    YearMonth = g.Key
                };
            return query;
        }

        public virtual IEnumerable<AverageData>
            AverageMoney(
            DateTime startDate,
            DateTime endDate,
            Guid? groupId,
            string costCode)
        {
            var query =
                from a in ActivityGroups
                where a.ParentGroupId == groupId
                from c in CostCentres
                where c.ParentCostCentreCostCode == costCode
                from e in CarbonEmissionEntries
                where
                    (e.EntryDate >= startDate) &&
                    (e.EntryDate <= endDate) &&
                    (e.ActivityGroupNode.IsDescendantOf(a.Node) ||
                     (e.ActivityGroupNode == a.Node)) &&
                    (e.CostCentreNode.IsDescendantOf(c.Node) ||
                     (e.CostCentreNode == c.Node))
                group e.Money by ((e.EntryDate.Year * 100) + (e.EntryDate.Month))
                into g
                select new AverageData
                {
                    Average = g.Average(),
                    YearMonth = g.Key
                };
            return query;
        }

        public virtual IEnumerable<AverageData>
            AverageUnits(
            DateTime startDate,
            DateTime endDate,
            Guid? groupId,
            string costCode)
        {
            var query =
                from a in ActivityGroups
                where a.ParentGroupId == groupId
                from c in CostCentres
                where c.ParentCostCentreCostCode == costCode
                from e in CarbonEmissionEntries
                where
                    (e.EntryDate >= startDate) &&
                    (e.EntryDate <= endDate) &&
                    (e.ActivityGroupNode.IsDescendantOf(a.Node) ||
                     (e.ActivityGroupNode == a.Node)) &&
                    (e.CostCentreNode.IsDescendantOf(c.Node) ||
                     (e.CostCentreNode == c.Node))
                group e.Units by ((e.EntryDate.Year * 100) + (e.EntryDate.Month))
                into g
                select new AverageData
                {
                    Average = g.Average(),
                    YearMonth = g.Key
                };
            return query;
        }

        public virtual IEnumerable<SliceTotal>
            TotalsByActivityGroup(
            DateTime startDate,
            DateTime endDate,
            Guid? groupId,
            string costCode)
        {
            var query =
                from a in ActivityGroups
                where a.ParentGroupId == groupId
                from c in CostCentres
                where c.ParentCostCentreCostCode == costCode
                from e in CarbonEmissionEntries
                where
                    (e.EntryDate >= startDate) &&
                    (e.EntryDate <= endDate) &&
                    (e.ActivityGroupNode.IsDescendantOf(a.Node) ||
                     (e.ActivityGroupNode == a.Node)) &&
                    (e.CostCentreNode.IsDescendantOf(c.Node) ||
                     (e.CostCentreNode == c.Node))
                group new
                {
                    units = e.Units,
                    emissions = e.CarbonEmissions
                } by a.Id
                into g
                select new SliceTotal
                {
                    ActivityGroupId = g.Key,
                    CostCode = costCode,
                    TotalCarbonEmissions = g.Sum(arg => arg.emissions),
                    TotalUnits = g.Sum(arg => arg.units)
                };
            return query;
        }

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

        public virtual IQueryable<AuditHistory> AuditHistory(
            DateTime startDate,
            DateTime endDate,
            Guid? groupId,
            string costCode)
        {
            var query =
                from entry in DataEntries
                from a in ActivityGroups
                where a.ParentGroupId == groupId
                from c in CostCentres
                where c.ParentCostCentreCostCode == costCode
                from e in CarbonEmissionEntries
                where
                    (e.EntryDate >= startDate) &&
                    (e.EntryDate <= endDate) &&
                    (e.ActivityGroupNode.IsDescendantOf(a.Node) ||
                     (e.ActivityGroupNode == a.Node)) &&
                    (e.CostCentreNode.IsDescendantOf(c.Node) ||
                     (e.CostCentreNode == c.Node)) &&
                    (e.DataEntryId == entry.Id)
                group new
                {
                    e.Units,
                    e.Money,
                    e.CarbonEmissions
                } by entry.SourceId
                into g
                from source in DataSources
                join fileDataSource in Set<FileDataSource>() on
                    source.Id equals fileDataSource.Id into filejoin
                from subFileSource in filejoin.DefaultIfEmpty()
                join manualDataSource in Set<ManualDataSource>() on
                    source.Id equals manualDataSource.Id into manualjoin
                from subManualSource in manualjoin.DefaultIfEmpty()
                where source.Id == g.Key
                select new AuditHistory
                {
                    CurrentFileName = (subFileSource == null) ? null: subFileSource.CurrentFileName,
                    Name = (subFileSource == null) ? "Manual Entry" : subFileSource.OriginalFileName,
                    DateEdit = source.DateEdit,
                    UserName = source.UserName,
                    HandlerName = (subFileSource == null) ? subManualSource.DisplayType : subFileSource.HandlerName,
                    Emissions = g.Sum(arg => arg.CarbonEmissions),
                    Cost = g.Sum(arg => arg.Money),
                    Units = g.Sum(arg => arg.Units),
                    SourceId = g.Key
                };
            return query;
        }

        public virtual IEnumerable<CurrencySummary> CurrenciesSummary(
            DateTime startDate,
            DateTime endDate,
            Guid? groupId,
            string costCode)
        {
            var query =
                from a in ActivityGroups
                where a.ParentGroupId == groupId
                from c in CostCentres
                where c.ParentCostCentreCostCode == costCode
                from e in CarbonEmissionEntries
                where
                    (e.EntryDate >= startDate) &&
                    (e.EntryDate <= endDate) &&
                    (e.ActivityGroupNode.IsDescendantOf(a.Node) ||
                     (e.ActivityGroupNode == a.Node)) &&
                    (e.CostCentreNode.IsDescendantOf(c.Node) ||
                     (e.CostCentreNode == c.Node))
                group new
                {
                    e.Money
                } by (c.CurrencyCode)
                into g
                from cu in Currencies
                where cu.Code == g.Key
                select new CurrencySummary
                {
                    Code = g.Key,
                    TotalMoney = g.Sum(arg => arg.Money),
                    Locale = cu.Locale,
                    Name = cu.Name,
                    Symbol = cu.Symbol
                };
            return query;
        }

        public virtual decimal TotalUnits(
            DateTime startDate,
            DateTime endDate,
            Guid? groupId,
            string costCode)
        {
            var query =
                from a in ActivityGroups
                where a.ParentGroupId == groupId
                from c in CostCentres
                where c.ParentCostCentreCostCode == costCode
                from e in CarbonEmissionEntries
                where
                    (e.EntryDate >= startDate) &&
                    (e.EntryDate <= endDate) &&
                    (e.ActivityGroupNode.IsDescendantOf(a.Node) ||
                     (e.ActivityGroupNode == a.Node)) &&
                    (e.CostCentreNode.IsDescendantOf(c.Node) ||
                     (e.CostCentreNode == c.Node))
                select (decimal?)e.Units;
            var total = query.Sum(arg => arg);
            return total ?? 0M;
        }

        public virtual decimal TotalEmissions(
            DateTime startDate,
            DateTime endDate,
            Guid? groupId,
            string costCode)
        {
            var query =
                from a in ActivityGroups
                where a.ParentGroupId == groupId
                from c in CostCentres
                where c.ParentCostCentreCostCode == costCode
                from e in CarbonEmissionEntries
                where
                    (e.EntryDate >= startDate) &&
                    (e.EntryDate <= endDate) &&
                    (e.ActivityGroupNode.IsDescendantOf(a.Node) ||
                     (e.ActivityGroupNode == a.Node)) &&
                    (e.CostCentreNode.IsDescendantOf(c.Node) ||
                     (e.CostCentreNode == c.Node))
                select (decimal?)e.CarbonEmissions;
            var total = query.Sum(arg => arg);
            return total ?? 0M;
        }

        public virtual void SetState<T>(T entity, EntityState state) where T : class
        {
            var entry = Entry(entity);
            entry.State = state;
        }

        public virtual new DbSet<T> Set<T>() where T : class
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
