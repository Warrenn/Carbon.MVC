using CarbonKnown.Factors.DAL;

namespace CarbonKnown.Factors.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed partial class Configuration : DbMigrationsConfiguration<DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(DataContext context)
        {
            GeneratedSeed(context);
        }
    }
}
