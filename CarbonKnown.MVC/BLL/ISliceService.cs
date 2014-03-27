using CarbonKnown.MVC.Models;

namespace CarbonKnown.MVC.BLL
{
    public interface ISliceService
    {
        DashboardSummary ActivityGroup(DashboardRequest request);
        DashboardSummary CostCentre(DashboardRequest request);
    }
}
