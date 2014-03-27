namespace CarbonKnown.Factors.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed partial class Configuration : DbMigrationsConfiguration<DAL.DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(DAL.DataContext context)
        {
            GeneratedSeed(context);
        }
    }
}
