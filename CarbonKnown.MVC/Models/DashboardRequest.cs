using System;
using CarbonKnown.MVC.Properties;

namespace CarbonKnown.MVC.Models
{
    public class DashboardRequest
    {
        public string CostCode { get; set; }
        public Guid? ActivityGroupId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Dimension Dimension { get; set; }
        public Section Section { get; set; }
        public string SelectedSliceId { get; set; }

        public static DashboardRequest Default
        {
            get
            {
                var today = DateTime.Today;
                var startDate = new DateTime(today.Year, today.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                return new DashboardRequest
                    {
                        CostCode = Settings.Default.RootCostCentre,
                        Dimension = Dimension.ActivityGroup,
                        StartDate = startDate,
                        EndDate = endDate,
                        ActivityGroupId = null,
                        Section = Section.Overview
                    };
            }
        }
    }
}