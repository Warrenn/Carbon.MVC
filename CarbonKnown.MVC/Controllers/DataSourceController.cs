﻿using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CarbonKnown.MVC.Code;
using CarbonKnown.WCF.DataSource;

namespace CarbonKnown.MVC.Controllers
{
    [RoutePrefix("api/datasource")]
    [Authorize(Roles = "Admin,Capturer")]
    public partial class DataSourceController : ApiController
    {
        private readonly IDataSourceService dataService;

        public DataSourceController(IDataSourceService dataService)
        {
            this.dataService = dataService;
        }

        [HttpPost]
        [Route("calculate/{sourceId}", Name = "CalculateEmissions")]
        public virtual void CalculateEmissions(Guid sourceId)
        {
            Task.Run(() => dataService.CalculateEmissions(sourceId));
        }

        [HttpPost]
        [Route("revert/{sourceId}", Name = "RevertCalculations")]
        [ResponseType(typeof(SourceResultDataContract))]
        public virtual async Task<IHttpActionResult> RevertCalculations(Guid sourceId)
        {
            var result = await Task.Run(() => dataService.RevertCalculation(sourceId));
            return Ok(result);
        }

        [HttpPost]
        [Route("insert/manualentry", Name = "InsertManualDataSource")]
        [ResponseType(typeof (SourceResultDataContract))]
        public virtual async Task<IHttpActionResult> InsertManualDataSource(ManualDataContract source)
        {
            source.UserName = User.Identity.Name;
            var result = await Task.Run(() => dataService.InsertManualDataSource(source));
            return Ok(result);
        }

        [HttpPost]
        [Route("insert/datafeed", Name = "InsertDataSourceFeed")]
        [ResponseType(typeof (SourceResultDataContract))]
        public virtual async Task<IHttpActionResult> InsertDataSourceFeed(FeedDataContract source)
        {
            source.UserName = User.Identity.Name;
            var result = await Task.Run(() => dataService.InsertDataSourceFeed(source));
            return Ok(result);
        }
    }
}