using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CarbonKnown.MVC.BLL;
using CarbonKnown.MVC.Models;

namespace CarbonKnown.MVC.Controllers
{
    [Authorize]
    [RoutePrefix("api/treewalk")]
    public class TreeWalkController : ApiController
    {
        private readonly ITreeWalkService service;

        public TreeWalkController(ITreeWalkService service)
        {
            this.service = service;
        }

        [HttpGet]
        [Route("children/activitygroup", Name = "ChildrenActivityGroup")]
        [ResponseType(typeof(IEnumerable<CrumbNode>))]
        public virtual async Task<IHttpActionResult> ChildrenActivityGroup(DashboardRequest request)
        {
            var result = await Task.Run(() => service.ActivityGroupChildren(request));
            return Ok(result);
        }

        [HttpGet]
        [Route("children/costcentre", Name = "ChildrenCostCentre")]
        [ResponseType(typeof(IEnumerable<CrumbNode>))]
        public virtual async Task<IHttpActionResult> ChildrenCostCentre(DashboardRequest request)
        {
            var result = await Task.Run(() => service.CostCentreChildren(request));
            return Ok(result);
        }

        [HttpGet]
        [Route("activitygroup", Name = "TreeWalkActivity")]
        [ResponseType(typeof(IEnumerable<CrumbNode>))]
        public virtual async Task<IHttpActionResult> ActivityGroup(DashboardRequest request)
        {
            var result = await Task.Run(() => service.ActivityGroupTreeWalk(request));
            return Ok(result);
        }

        [HttpGet]
        [Route("costcentre", Name = "TreeWalkCostCentre")]
        [ResponseType(typeof(IEnumerable<CrumbNode>))]
        public virtual async Task<IHttpActionResult> CostCentre(DashboardRequest request)
        {
            var result = await Task.Run(() => service.CostCentreTreeWalk(request));
            return Ok(result);
        }

    }
}