using System;
using System.Collections.Generic;
using System.Linq;
using CarbonKnown.DAL;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Source;
using CarbonKnown.MVC.Models;

namespace CarbonKnown.MVC.DAL
{
    public class SummaryDataContext : ISummaryDataContext
    {
        private readonly DataContext context;

        public SummaryDataContext(DataContext context)
        {
            this.context = context;
        }

        public virtual IQueryable<EmissionSummaryModel> EmissionData(
            DateTime startDate,
            DateTime endDate,
            Guid? groupId,
            string costCode)

        {
            var query =
                from a in context.ActivityGroups
                from c in context.CostCentres
                from e in context.CarbonEmissionEntries
                where
                    (a.ParentGroupId == groupId) &&
                    (c.ParentCostCentreCostCode == costCode) &&
                    (e.EntryDate >= startDate) &&
                    (e.EntryDate <= endDate) &&
                    (e.ActivityGroupNode.IsDescendantOf(a.Node) ||
                     (e.ActivityGroupNode == a.Node)) &&
                    (e.CostCentreNode.IsDescendantOf(c.Node) ||
                     (e.CostCentreNode == c.Node))
                select new EmissionSummaryModel
                {
                    ActivityGroup = a,
                    CostCentre = c,
                    Entry = e
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
                from e in EmissionData(startDate, endDate, groupId, costCode)
                select (decimal?) e.Entry.Units;
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
                from e in EmissionData(startDate, endDate, groupId, costCode)
                select (decimal?) e.Entry.CarbonEmissions;
            var total = query.Sum(arg => arg);
            return total ?? 0M;
        }

        public virtual IEnumerable<SliceTotal>
            TotalsByCostCentre(
            DateTime startDate,
            DateTime endDate,
            Guid? groupId,
            string costCode)
        {
            var query =
                from e in EmissionData(startDate, endDate, groupId, costCode)
                group new
                {
                    units = e.Entry.Units,
                    emissions = e.Entry.CarbonEmissions
                } by e.CostCentre.CostCode
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

        public virtual IEnumerable<SliceTotal>
            TotalsByActivityGroup(
            DateTime startDate,
            DateTime endDate,
            Guid? groupId,
            string costCode)
        {
            var query =
                from e in EmissionData(startDate, endDate, groupId, costCode)
                group new
                {
                    units = e.Entry.Units,
                    emissions = e.Entry.CarbonEmissions
                } by e.ActivityGroup.Id
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

        public virtual IEnumerable<AverageData>
            AverageCo2(
            DateTime startDate,
            DateTime endDate,
            Guid? groupId,
            string costCode)
        {
            var query =
                from e in EmissionData(startDate, endDate, groupId, costCode)
                group e.Entry.CarbonEmissions by
                    ((e.Entry.EntryDate.Year*100) + (e.Entry.EntryDate.Month))
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
                from e in EmissionData(startDate, endDate, groupId, costCode)
                group e.Entry.Money by
                    ((e.Entry.EntryDate.Year*100) + (e.Entry.EntryDate.Month))
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
                from e in EmissionData(startDate, endDate, groupId, costCode)
                group e.Entry.Units by
                    ((e.Entry.EntryDate.Year*100) + (e.Entry.EntryDate.Month))
                into g
                select new AverageData
                {
                    Average = g.Average(),
                    YearMonth = g.Key
                };
            return query;
        }

        public virtual IEnumerable<CurrencySummary> 
            CurrenciesSummary(
            DateTime startDate,
            DateTime endDate,
            Guid? groupId,
            string costCode)
        {
            var query =
                from e in EmissionData(startDate, endDate, groupId, costCode)
                group new
                {
                    e.Entry.Money
                } by (e.CostCentre.CurrencyCode)
                into g
                    from cu in context.Currencies
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

        public virtual IQueryable<AuditHistory> 
            AuditHistory(
            DateTime startDate,
            DateTime endDate,
            Guid? groupId,
            string costCode)
        {
            var query =
                from e in EmissionData(startDate, endDate, groupId, costCode)
                group new
                {
                    e.Entry.Units,
                    e.Entry.Money,
                    e.Entry.CarbonEmissions
                } by e.Entry.SourceEntry.SourceId
                into g
                    from source in context.DataSources
                    join fileDataSource in context.Set<FileDataSource>() on
                    source.Id equals fileDataSource.Id into filejoin
                from subFileSource in filejoin.DefaultIfEmpty()
                    join manualDataSource in context.Set<ManualDataSource>() on
                    source.Id equals manualDataSource.Id into manualjoin
                from subManualSource in manualjoin.DefaultIfEmpty()
                where source.Id == g.Key
                select new AuditHistory
                {
                    CurrentFileName = (subFileSource == null) ? null : subFileSource.CurrentFileName,
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
    }
}
