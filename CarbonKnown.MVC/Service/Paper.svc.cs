﻿using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CarbonKnown.Calculation;
using CarbonKnown.MVC.DAL;
using CarbonKnown.WCF.Paper;

namespace CarbonKnown.MVC.Service
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class Paper :
        DataEntryServiceBase<CarbonKnown.DAL.Models.Paper.PaperData, PaperDataContract>,
        IPaperService
    {
		protected static readonly Guid CalculationId = new Guid("b3e38e60-4e6b-4c5b-a422-3500fc8dabc4");

	    public Paper(ISourceDataContext context, ICalculationFactory calculationFactory) : base(context, calculationFactory)
	    {
	    }

	    public override Guid GetCalculationId()
	    {
	        return CalculationId;
	    }        
		
		public override void SetEntryValues(CarbonKnown.DAL.Models.Paper.PaperData instance, PaperDataContract dataEntry)
        {
            base.SetEntryValues(instance, dataEntry);
            instance.PaperType = (CarbonKnown.DAL.Models.Paper.PaperType?)dataEntry.PaperType;
            instance.PaperUom = (CarbonKnown.DAL.Models.Paper.PaperUom?)dataEntry.PaperUom;
        }
    }
}
