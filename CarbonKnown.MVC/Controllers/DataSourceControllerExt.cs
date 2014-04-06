using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CarbonKnown.MVC.App_Start;
using CarbonKnown.MVC.Code;
using CarbonKnown.WCF.Accommodation;
using CarbonKnown.WCF.AirTravel;
using CarbonKnown.WCF.AirTravelRoute;
using CarbonKnown.WCF.CarHire;
using CarbonKnown.WCF.Commuting;
using CarbonKnown.WCF.Courier;
using CarbonKnown.WCF.CourierRoute;
using CarbonKnown.WCF.DataEntry;
using CarbonKnown.WCF.Electricity;
using CarbonKnown.WCF.Fleet;
using CarbonKnown.WCF.Fuel;
using CarbonKnown.WCF.Paper;
using CarbonKnown.WCF.Refrigerant;
using CarbonKnown.WCF.Waste;
using CarbonKnown.WCF.Water;
using Microsoft.Practices.Unity;

namespace CarbonKnown.MVC.Controllers
{
    public partial class DataSourceController 
    {
        [HttpPost]
        [Route("upsert/accommodation", Name = "UpsertAccommodationData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertAccommodationData(AccommodationDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<IAccommodationService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/airtravelroute", Name = "UpsertAirTravelRouteData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertAirTravelRouteData(AirTravelRouteDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<IAirTravelRouteService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/airtravel", Name = "UpsertAirTravelData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertAirTravelData(AirTravelDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<IAirTravelService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/carhire", Name = "UpsertCarHireData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertCarHireData(CarHireDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<ICarHireService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/commuting", Name = "UpsertCommutingData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertCommutingData(CommutingDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<ICommutingService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/courierroute", Name = "UpsertCourierRouteData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertCourierRouteData(CourierRouteDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<ICourierRouteService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/courier", Name = "UpsertCourierData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertCourierData(CourierDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<ICourierService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/electricity", Name = "UpsertElectricityData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertElectricityData(ElectricityDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<IElectricityService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/fuel", Name = "UpsertFuelData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertFuelData(FuelDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<IFuelService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/paper", Name = "UpsertPaperData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertPaperData(PaperDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<IPaperService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/refrigerant", Name = "UpsertRefrigerantData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertRefrigerantData(RefrigerantDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<IRefrigerantService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/fleet", Name = "UpsertFleetData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertFleetData(FleetDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<IFleetService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/waste", Name = "UpsertWasteData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertWasteData(WasteDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<IWasteService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/water", Name = "UpsertWaterData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertWaterData(WaterDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<IWaterService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
	}
}
