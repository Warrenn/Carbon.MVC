using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CarbonKnown.DAL;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Source;
using CarbonKnown.MVC.Code;
using CarbonKnown.MVC.Models;

namespace CarbonKnown.MVC.Controllers
{
    [Authorize(Roles = "Admin,Capturer")]
    public class InputHistoryController : Controller
    {
        private readonly DataContext context;

        public InputHistoryController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SelectSource(Guid sourceId)
        {
            var fileSource = context.Set<FileDataSource>().FirstOrDefault(source => source.Id == sourceId);
            if ((fileSource == null) || (!System.IO.File.Exists(fileSource.CurrentFileName)))
            {
                return RedirectToAction("Index", "EditSource", new {sourceId});
            }
            var url = Url.HttpRouteUrl("filedownload", new {sourceId});
            return RedirectPermanent(url);
        }

        [HttpGet]
        public ActionResult History(DataTableParamModel request)
        {
            var query =
                (from source in context.DataSources
                    join fileDataSource in context.Set<FileDataSource>() on source.Id equals fileDataSource.Id into
                        filejoin
                    from subFileSource in filejoin.DefaultIfEmpty()
                    join feedDataSource in context.Set<FeedDataSource>() on source.Id equals feedDataSource.Id into fjn
                    from subFeedDataSource in fjn.DefaultIfEmpty(null)
                    join manualDataSource in context.Set<ManualDataSource>() on source.Id equals manualDataSource.Id
                        into
                        manualjoin
                    from subManualSource in manualjoin.DefaultIfEmpty()
                    select new InputHistoryDataModel
                    {
                        Name = (subFileSource != null)
                            ? subFileSource.OriginalFileName
                            : (subFeedDataSource == null) ? "Manual Entry" : subFeedDataSource.SourceUrl,
                        EditDate = source.DateEdit,
                        UserName = source.UserName,
                        Type = (subManualSource != null)
                            ? subManualSource.DisplayType
                            : (subFileSource != null) ? subFileSource.HandlerName : subFeedDataSource.HandlerName,
                        Status = source.InputStatus,
                        Id = source.Id
                    });
            var builder = new DataTableResultModelBuilder<InputHistoryDataModel>();
            builder.AddQueryable(query);
            builder.AddDataExpression(arg => new object[]
            {
                HttpUtility.HtmlEncode(arg.Name),
                arg.EditDate.ToString(Constants.Constants.DateFormat),
                HttpUtility.HtmlEncode(arg.UserName),
                arg.Type,
                Enum.GetName(typeof (SourceStatus), arg.Status),
                arg.Id.ToString(),
                Url.RouteUrl("editsource", new {SourceId = arg.Id}),
                Url.Action("SelectSource", new {SourceId = arg.Id})
            });
            SourceStatus status;
            if (Enum.TryParse(request.sSearch, true, out status))
            {
                builder.AddSearchFilter(model => model.Status == status);
            }
            else
            {
                builder.AddSearchFilter(arg =>
                    arg.Name.Contains(request.sSearch) ||
                    arg.UserName.Contains(request.sSearch) ||
                    arg.Type.Contains(request.sSearch));
            }
            builder.AddSortExpression(data => data.Name);
            builder.AddSortExpression(data => data.EditDate);
            builder.AddSortExpression(data => data.UserName);
            builder.AddSortExpression(data => data.Type);
            builder.AddSortExpression(data => data.Status);
            var result = builder.BuildResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}