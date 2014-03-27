using System.Collections.Generic;
using CarbonKnown.MVC.Models;

namespace CarbonKnown.MVC.BLL
{
    public interface ITreeWalkService
    {
        IEnumerable<CrumbNode> ActivityGroupTreeWalk(DashboardRequest request);
        IEnumerable<CrumbNode> CostCentreTreeWalk(DashboardRequest request);
        IEnumerable<CrumbNode> ActivityGroupChildren(DashboardRequest request);
        IEnumerable<CrumbNode> CostCentreChildren(DashboardRequest request);
    }
}