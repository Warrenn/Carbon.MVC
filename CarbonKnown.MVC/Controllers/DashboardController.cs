using System.Web.Mvc;
using CarbonKnown.MVC.BLL;
using CarbonKnown.MVC.Models;
using CarbonKnown.Print;

namespace CarbonKnown.MVC.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ISliceService service;

        public DashboardController(ISliceService service)
        {
            this.service = service;
        }

        [HttpGet]
        public ActionResult Index(DashboardRequest request)
        {
            if ((request == null) ||
                (string.IsNullOrEmpty(request.CostCode)))
            {
                request = DashboardRequest.Default;
            }
            return View(request);
        }

        private DashboardSummary CreateSummary(DashboardRequest request)
        {
            return request.Dimension == Dimension.ActivityGroup ? service.ActivityGroup(request) : service.CostCentre(request);
        }

        [HttpGet]
        public ActionResult PrintPdf(DashboardRequest request)
        {
            var model = new DashboardPrintModel
                {
                    Summary = CreateSummary(request),
                    Request = request
                };
            return PrintResult.PrintToPdf("Print", model);
        }

        [HttpGet]
        public ActionResult PrintJpg(DashboardRequest request)
        {
            var model = new DashboardPrintModel
            {
                Summary = CreateSummary(request),
                Request = request
            };
            return PrintResult.PrintToJpeg("Print", model);
        }
    }
}
