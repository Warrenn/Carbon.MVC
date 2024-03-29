﻿using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CarbonKnown.Calculation;
using CarbonKnown.MVC.DAL;
using CarbonKnown.WCF.AirTravelRoute;

namespace CarbonKnown.MVC.Service
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class AirTravelRoute :
        DataEntryServiceBase<CarbonKnown.DAL.Models.AirTravel.AirTravelRouteData, AirTravelRouteDataContract>,
        IAirTravelRouteService
    {
		protected static readonly Guid CalculationId = new Guid("4451ffcf-4851-46be-88e9-a2930a82a312");

	    public AirTravelRoute(ISourceDataContext context, ICalculationFactory calculationFactory) : base(context, calculationFactory)
	    {
	    }

	    public override Guid GetCalculationId()
	    {
	        return CalculationId;
	    }        
		
		public override void SetEntryValues(CarbonKnown.DAL.Models.AirTravel.AirTravelRouteData instance, AirTravelRouteDataContract dataEntry)
        {
            base.SetEntryValues(instance, dataEntry);
            instance.TravelClass = (CarbonKnown.DAL.Models.AirTravel.TravelClass)dataEntry.TravelClass;
            instance.Reversal = dataEntry.Reversal;
            instance.FromCode = dataEntry.FromCode;
            instance.ToCode = dataEntry.ToCode;
        }
    }
}
