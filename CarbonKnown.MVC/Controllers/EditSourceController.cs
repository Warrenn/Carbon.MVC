using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CarbonKnown.DAL;
using CarbonKnown.DAL.Models;
using CarbonKnown.DAL.Models.Source;
using CarbonKnown.MVC.BLL;
using CarbonKnown.MVC.DAL;
using CarbonKnown.MVC.Models;
using CarbonKnown.WCF.DataSource;
using Calcs = CarbonKnown.DAL.Models.Constants.Calculation;

namespace CarbonKnown.MVC.Controllers
{
    [Authorize(Roles = "Admin,Capturer")]
    public partial class EditSourceController : Controller
    {
        private readonly DataContext context;
        private readonly ISourceDataContext sourceDataContext;
        private readonly FileDataSourceService fileService;
        private readonly IDataSourceService dataService;

        private static string ConvertToString<T>(T value)
        {
            if ((value is decimal) ||
                (value is float) ||
                (value is double))
            {
                return string.Format(CultureInfo.CurrentCulture, "{0:" + Constants.Constants.NumberFormat + "}", value);
            }
            if ((value is sbyte) ||
                (value is short) ||
                (value is int) ||
                (value is byte) ||
                (value is long) ||
                (value is ushort) ||
                (value is bool) ||
                (value is char) ||
                (value is uint))
            {
                return string.Format(CultureInfo.CurrentCulture, "{0}", value);
            }
            if (value is DateTime)
            {
                return string.Format(CultureInfo.CurrentCulture, "{0:" + Constants.Constants.DateFormat + "}", value);
            }
            var type = typeof (T);
            var underlyingType = type;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>))
            {
                underlyingType = type.GenericTypeArguments[0];
            }
            if (underlyingType.IsEnum)
            {
                return value == null ? string.Empty : Enum.GetName(underlyingType, value);
            }
            var stringValue = string.Format("{0}", value);
            return HttpUtility.HtmlEncode(stringValue);
        }

        public EditSourceController(
            DataContext context,
            ISourceDataContext sourceDataContext,
            FileDataSourceService fileService,
            IDataSourceService dataService)
        {
            this.context = context;
            this.sourceDataContext = sourceDataContext;
            this.fileService = fileService;
            this.dataService = dataService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Calculate(Guid sourceId)
        {
            Task.Run(() => { dataService.CalculateEmissions(sourceId); });
            return RedirectToAction("Index", "InputHistory");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Cancel(Guid sourceId)
        {
            await Task.Run(() => { fileService.CancelFileSourceExtraction(sourceId); });
            return RedirectToAction("Index", "InputHistory");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Revert(Guid sourceId)
        {
            Task.Run(() => { dataService.RevertCalculation(sourceId); });
            return RedirectToAction("Index", "InputHistory");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExtractData(Guid sourceId)
        {
            Task.Run(() => { fileService.ExtractData(sourceId); });
            return RedirectToAction("Index", "InputHistory");
        }

        [HttpGet]
        public ActionResult Index(Guid sourceId)
        {
            if (context.DataSources.FirstOrDefault(source => source.Id == sourceId) == null)
            {
                return RedirectToAction("Index", "InputHistory");
            }
            if (context.DataEntries.Any(
                entry => (entry.SourceId == sourceId) &&
                         (entry.CalculationId == Calcs.MigrationId)))
            {
                return RedirectToAction("Index", "InputHistory");
            }
            var manualSource = context
                .Set<ManualDataSource>()
                .Include("DataEntries")
                .FirstOrDefault(source => source.Id == sourceId);
            if (manualSource != null)
            {
                var entry = manualSource.DataEntries.First();
                return RedirectToAction("EditData", "Input", new {entryId = entry.Id});
            }
            var model =
                (from source in context.DataSources
                    join fileDataSource in context.Set<FileDataSource>()
                        on source.Id equals fileDataSource.Id into jn
                    from subFileSource in jn.DefaultIfEmpty(null)
                    join feedDataSource in context.Set<FeedDataSource>()
                        on source.Id equals feedDataSource.Id into fjn
                    from subFeedDataSource in fjn.DefaultIfEmpty(null)
                    where source.Id == sourceId
                    select new EditSourceModel
                    {
                        Name = (subFileSource == null) ? subFeedDataSource.ScriptPath : subFileSource.OriginalFileName,
                        EditDate = source.DateEdit,
                        CurrentFileName =
                            (subFileSource == null) ? subFeedDataSource.SourceUrl : subFileSource.CurrentFileName,
                        UserName = source.UserName,
                        Type = (subFileSource == null) ? subFeedDataSource.HandlerName : subFileSource.HandlerName,
                        SourceStatus = source.InputStatus,
                        SourceId = source.Id,
                        Comment = source.ReferenceNotes,
                        SourceErrors = source.SourceErrors
                    }).First();
            model.DataEntries = context
                .DataEntries
                .Count(entry => entry.SourceId == sourceId);
            model.DataEntriesInError =
                (from entry in context.DataEntries.Include("Errors")
                    from error in entry.Errors
                    where
                        (entry.SourceId == sourceId)
                    group error by error.DataEntryId
                    into g
                    select g.Key).Count();
            model.ErrorTypeCount = new Dictionary<DataErrorType, int>();
            foreach (var value in Enum.GetValues(typeof (DataErrorType)))
            {
                var errorType = (DataErrorType) value;
                var amount =
                    (from entry in context.DataEntries.Include("Errors")
                        from error in entry.Errors
                        where
                            (error.ErrorType == errorType) &&
                            (entry.SourceId == sourceId)
                        group error by error.DataEntryId
                        into g
                        select g.Key).Count();
                if (amount > 0) model.ErrorTypeCount[errorType] = amount;
            }

            model.SourceContainsErrors =
                sourceDataContext.SourceContainsErrors(sourceId) ||
                sourceDataContext.SourceContainsDataEntriesInError(sourceId);
            model.Calculations =
                (from entry in context.DataEntries.Include("Calculation")
                    where entry.SourceId == sourceId
                    group entry by entry.Calculation
                    into g
                    select new {g.Key, amount = g.Count()})
                    .ToDictionary(arg => arg.Key, arg => arg.amount);
            return View(model);
        }
    }
}