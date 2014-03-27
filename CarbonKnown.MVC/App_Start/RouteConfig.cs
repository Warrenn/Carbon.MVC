using System.Web.Mvc;
using System.Web.Routing;

namespace CarbonKnown.MVC.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            //InputHistory
            routes.MapRoute("inputhistory", "input",
                            new {controller = "InputHistory", action = "Index"});
            
            //OverviewReport
            routes.MapRoute("reporting", "reporting",
                            new {controller = "OverviewReport", action = "Index"});
            
            //EditSource
            routes.MapRoute("editsource", "input/editsource",
                            new {controller = "EditSource", action = "Index"});
            routes.MapRoute("sourceentries", "source/entries/{action}",
                            new { controller = "EditSource" });
            
            
            //Checklist
            routes.MapRoute("checklist", "input/checklist",
                            new {controller = "Checklist", action = "Index"});
            
            //TraceSource
            routes.MapRoute("tracesource", "input/tracesource",
                            new {controller = "TraceSource", action = "Index"});
            
            //Dashboard
            routes.MapRoute("dashboard", "dashboard/{section}/{dimension}",
                            new {controller = "Dashboard", action = "Index"});
            
            //Comparison
            routes.MapRoute("ComparisonChart", "comparison/chart",
                            new { controller = "Comparison", action = "ComparisonChart" });
            routes.MapRoute("ComparisonData", "comparison/data",
                            new { controller = "Comparison", action = "ComparisonData" });
            routes.MapRoute("AddSeries", "comparison/addseries",
                            new {controller = "Comparison", action = "AddSeries"});
            routes.MapRoute("RemoveSeries", "comparison/removeseries",
                            new {controller = "Comparison", action = "RemoveSeries"});
            
            //Input
            routes.MapRoute("airroutecodes", "input/airroutecodes",
                            new {controller = "Input", action = "AirRouteCodes"});
            routes.MapRoute("courierroutecodes", "input/courierroutecodes",
                            new {controller = "Input", action = "CourierRouteCodes"});
            routes.MapRoute("costcentres", "input/costcentres/{consumptionType}",
                            new { controller = "Input", action = "CostCentres" });
            routes.MapRoute("inputcalculations", "input/calculations/{consumptionType}",
                            new {controller = "Input", action = "Calculations"});
            routes.MapRoute("enterdata", "input/enterdata",
                            new {controller = "Input", action = "EnterData"});
            routes.MapRoute("editdata", "input/editdata",
                            new {controller = "Input", action = "EditData"});

            //Default
            routes.MapRoute("Default", "{controller}/{action}",
                            new {controller = "Dashboard", action = "Index"});
        }
    }
}