using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CarbonKnown.MVC.App_Start;
using CarbonKnown.MVC.Code;
using CarbonKnown.WCF.DataEntry;
using CarbonKnown.WCF.DataSource;
using Microsoft.Practices.Unity;

namespace CarbonKnown.MVC.Controllers
{
    public partial class DataSourceController 
    {
        [HttpPost]
        [Route("upsert/accommodation", Name = "UpsertAccommodationData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertAccommodationData(CarbonKnown.WCF.Accommodation.AccommodationDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<CarbonKnown.WCF.Accommodation.IAccommodationService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/airtravelroute", Name = "UpsertAirTravelRouteData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertAirTravelRouteData(CarbonKnown.WCF.AirTravelRoute.AirTravelRouteDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<CarbonKnown.WCF.AirTravelRoute.IAirTravelRouteService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/airtravel", Name = "UpsertAirTravelData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertAirTravelData(CarbonKnown.WCF.AirTravel.AirTravelDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<CarbonKnown.WCF.AirTravel.IAirTravelService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/carhire", Name = "UpsertCarHireData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertCarHireData(CarbonKnown.WCF.CarHire.CarHireDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<CarbonKnown.WCF.CarHire.ICarHireService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/commuting", Name = "UpsertCommutingData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertCommutingData(CarbonKnown.WCF.Commuting.CommutingDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<CarbonKnown.WCF.Commuting.ICommutingService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/courierroute", Name = "UpsertCourierRouteData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertCourierRouteData(CarbonKnown.WCF.CourierRoute.CourierRouteDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<CarbonKnown.WCF.CourierRoute.ICourierRouteService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/courier", Name = "UpsertCourierData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertCourierData(CarbonKnown.WCF.Courier.CourierDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<CarbonKnown.WCF.Courier.ICourierService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/electricity", Name = "UpsertElectricityData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertElectricityData(CarbonKnown.WCF.Electricity.ElectricityDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<CarbonKnown.WCF.Electricity.IElectricityService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/fuel", Name = "UpsertFuelData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertFuelData(CarbonKnown.WCF.Fuel.FuelDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<CarbonKnown.WCF.Fuel.IFuelService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/paper", Name = "UpsertPaperData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertPaperData(CarbonKnown.WCF.Paper.PaperDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<CarbonKnown.WCF.Paper.IPaperService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/refrigerant", Name = "UpsertRefrigerantData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertRefrigerantData(CarbonKnown.WCF.Refrigerant.RefrigerantDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<CarbonKnown.WCF.Refrigerant.IRefrigerantService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/fleet", Name = "UpsertFleetData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertFleetData(CarbonKnown.WCF.Fleet.FleetDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<CarbonKnown.WCF.Fleet.IFleetService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/waste", Name = "UpsertWasteData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertWasteData(CarbonKnown.WCF.Waste.WasteDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<CarbonKnown.WCF.Waste.IWasteService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
        [HttpPost]
        [Route("upsert/water", Name = "UpsertWaterData")]
        [ResponseType(typeof(DataEntryUpsertResultDataContract))]
        public virtual async Task<IHttpActionResult> UpsertWaterData(CarbonKnown.WCF.Water.WaterDataContract data)
        {
            data.UserName = User.Identity.Name;
            var service = Bootstrapper.Container.Resolve<CarbonKnown.WCF.Water.IWaterService>();
            var result = await Task.Run(() => service.UpsertDataEntry(data));
            return Ok(result);
        }
	}
}
