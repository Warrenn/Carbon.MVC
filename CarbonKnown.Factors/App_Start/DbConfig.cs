using System.Data.Entity;
using CarbonKnown.Factors.DAL;
using CarbonKnown.Factors.Migrations;

namespace CarbonKnown.Factors.App_Start
{
    public class DbConfig
    {
        public static void ConfigureDb()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, Configuration>());
            using (var context = new DataContext())
            {
                context.Database.Initialize(true);
            }
        }
    }
}