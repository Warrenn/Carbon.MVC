using System.Web;
using CarbonKnown.Factors.App_Start;

namespace CarbonKnown.Factors
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            DbConfig.ConfigureDb();
        }
    }
}