namespace CarbonKnown.Factors.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AirRouteDistances",
                c => new
                    {
                        Code1 = c.String(nullable: false, maxLength: 128),
                        Code2 = c.String(nullable: false, maxLength: 128),
                        CalculationDate = c.DateTime(),
                        Distance = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => new { t.Code1, t.Code2 });
            
            CreateTable(
                "dbo.CourierRouteDistances",
                c => new
                    {
                        Code1 = c.String(nullable: false, maxLength: 128),
                        Code2 = c.String(nullable: false, maxLength: 128),
                        CalculationDate = c.DateTime(),
                        Distance = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => new { t.Code1, t.Code2 });
            
            CreateTable(
                "dbo.Factors",
                c => new
                    {
                        FactorId = c.Guid(nullable: false, identity: true),
                        FactorGroup = c.String(),
                        FactorName = c.String(),
                    })
                .PrimaryKey(t => t.FactorId);
            
            CreateTable(
                "dbo.FactorValues",
                c => new
                    {
                        FactorId = c.Guid(nullable: false),
                        EffectiveDate = c.DateTime(nullable: false),
                        CalculationDate = c.DateTime(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => new { t.FactorId, t.EffectiveDate })
                .ForeignKey("dbo.Factors", t => t.FactorId, cascadeDelete: true)
                .Index(t => t.FactorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FactorValues", "FactorId", "dbo.Factors");
            DropIndex("dbo.FactorValues", new[] { "FactorId" });
            DropTable("dbo.FactorValues");
            DropTable("dbo.Factors");
            DropTable("dbo.CourierRouteDistances");
            DropTable("dbo.AirRouteDistances");
        }
    }
}
