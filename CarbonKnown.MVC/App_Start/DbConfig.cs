using System.Data.Entity;
using CarbonKnown.DAL;
using CarbonKnown.DAL.Migrations;
using WebMatrix.WebData;

namespace CarbonKnown.MVC.App_Start
{
    public class DbConfig
    {
        public static void ConfigureDb()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, Configuration>());
            using (var ctx = new DataContext())
            {
                ctx.Database.Initialize(false);
                WebSecurity
                    .InitializeDatabaseConnection(
                        Constant.ConnectionStringName,
                        "UserProfiles",
                        "UserId",
                        "UserName",
                        true);
            }
        }
    }
}