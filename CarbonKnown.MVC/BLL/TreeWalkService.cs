using System.Collections.Generic;
using System.Linq;
using CarbonKnown.DAL;
using CarbonKnown.MVC.Models;

namespace CarbonKnown.MVC.BLL
{
    public class TreeWalkService : ITreeWalkService
    {
        private readonly DataContext context;

        public TreeWalkService(DataContext context)
        {
            this.context = context;
        }

        public IEnumerable<CrumbNode> ActivityGroupChildren(DashboardRequest request)
        {
            var groupId = request.ActivityGroupId;
            if (groupId == null)
            {
                var configuration = SliceService.Configurations[request.Section];
                var nodes = configuration
                    .ActivityIds
                    .Select(id => context.ActivityGroups.Find(id))
                    .Select(@group => new CrumbNode(@group.Name, @group.Id, request.CostCode));
                return nodes;
            }
            return context
                .ActivityGroups
                .Where(@group => @group.ParentGroupId == groupId)
                .OrderBy(@group => @group.OrderId)
                .ToArray()
                .Select(@group => new CrumbNode(@group.Name, @group.Id, request.CostCode));
        }

        public IEnumerable<CrumbNode> CostCentreChildren(DashboardRequest request)
        {
            var costCode = request.CostCode;
            return context
                .CostCentres
                .Where(centre => centre.ParentCostCentreCostCode == costCode)
                .OrderBy(centre => centre.OrderId)
                .ToArray()
                .Select(centre => new CrumbNode(centre.Name, request.ActivityGroupId, centre.CostCode));
        }

        public IEnumerable<CrumbNode> ActivityGroupTreeWalk(DashboardRequest request)
        {
            var configuration = SliceService.Configurations[request.Section];
            var terminalGuid = request.ActivityGroupId;
            var groups = context
                .ActivityGroupsTreeWalk(terminalGuid)
                .ToArray();
            var initialNode = new CrumbNode(configuration.DisplayName, costCode: request.CostCode);
            var take = true;
            var nodes = groups
                .TakeWhile(@group =>
                    {
                        if ((take) && (configuration.ActivityIds.Contains(@group.Id)))
                        {
                            take = false;
                            return true;
                        }
                        return take;
                    })
                .Select(@group => new CrumbNode(@group.Name, @group.Id, request.CostCode))
                .ToList();
            nodes.Add(initialNode);
            return nodes;
        }

        public IEnumerable<CrumbNode> CostCentreTreeWalk(DashboardRequest request)
        {
            return context
                .CostCentreTreeWalk(request.CostCode)
                .Select(centre => new CrumbNode(centre.Name, request.ActivityGroupId, centre.CostCode));
        }
    }
}