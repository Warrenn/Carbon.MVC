using System;
using System.Collections.Generic;
using System.Linq;
using CarbonKnown.DAL.Models;

namespace CarbonKnown.MVC.DAL
{
    public interface ISummaryDataContext
    {
        decimal TotalUnits(
            DateTime startDate,
            DateTime endDate,
            Guid? groupId,
            string costCode);

        decimal TotalEmissions(
            DateTime startDate,
            DateTime endDate,
            Guid? groupId,
            string costCode);

        IEnumerable<SliceTotal>
            TotalsByCostCentre(
            DateTime startDate,
            DateTime endDate,
            Guid? groupId,
            string costCode);

        IEnumerable<SliceTotal>
            TotalsByActivityGroup(
            DateTime startDate,
            DateTime endDate,
            Guid? groupId,
            string costCode);

        IEnumerable<AverageData>
            AverageCo2(
            DateTime startDate,
            DateTime endDate,
            Guid? groupId,
            string costCode);

        IEnumerable<AverageData>
            AverageMoney(
            DateTime startDate,
            DateTime endDate,
            Guid? groupId,
            string costCode);

        IEnumerable<AverageData>
            AverageUnits(
            DateTime startDate,
            DateTime endDate,
            Guid? groupId,
            string costCode);

        IEnumerable<CurrencySummary> 
            CurrenciesSummary(
            DateTime startDate,
            DateTime endDate,
            Guid? groupId,
            string costCode);

        IQueryable<AuditHistory> 
            AuditHistory(
            DateTime startDate,
            DateTime endDate,
            Guid? groupId,
            string costCode);
    }
}