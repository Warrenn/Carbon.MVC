using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CarbonKnown.MVC.BLL;
using CarbonKnown.MVC.Models;

namespace CarbonKnown.MVC.Controllers
{
    [Authorize]
    [RoutePrefix("api/slice")]
    public class SliceController : ApiController
    {
        private readonly ISliceService service;

        public SliceController(ISliceService service)
        {
            this.service = service;
        }

        [HttpGet]
        [Route("activitygroup")]
        [ResponseType(typeof (DashboardSummary))]
        public virtual async Task<IHttpActionResult> ActivityGroup(DashboardRequest request)
        {
            var result = await Task.Run(() => service.ActivityGroup(request));
            return Ok(result);
        }

        [HttpGet]
        [Route("costcentre")]
        [ResponseType(typeof(DashboardSummary))]
        public virtual async Task<IHttpActionResult> CostCentre(DashboardRequest request)
        {
            var result = await Task.Run(() => service.CostCentre(request));
            return Ok(result);
        }
    }
}