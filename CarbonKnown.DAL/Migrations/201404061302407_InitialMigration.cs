namespace CarbonKnown.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActivityGroups",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Node = c.HierarchyId(),
                        Color = c.String(maxLength: 6),
                        OrderId = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        GRIDescription = c.String(),
                        UOMShort = c.String(),
                        UOMLong = c.String(),
                        ParentGroupId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ActivityGroups", t => t.ParentGroupId)
                .Index(t => t.ParentGroupId);
            
            CreateTable(
                "dbo.Calculations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ConsumptionType = c.Int(nullable: false),
                        AssemblyQualifiedName = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DataEntries",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PagingId = c.Int(nullable: false, identity: true),
                        EditDate = c.DateTime(nullable: false),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        CostCode = c.String(),
                        Money = c.Decimal(precision: 22, scale: 8),
                        Units = c.Decimal(precision: 22, scale: 8),
                        SourceId = c.Guid(nullable: false),
                        UserName = c.String(),
                        RowNo = c.Int(),
                        CalculationId = c.Guid(nullable: false),
                        EntryType = c.String(),
                        Hash = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Calculations", t => t.CalculationId)
                .ForeignKey("dbo.DataSources", t => t.SourceId, cascadeDelete: true)
                .Index(t => t.SourceId)
                .Index(t => t.CalculationId);
            
            CreateTable(
                "dbo.DataErrors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DataEntryId = c.Guid(nullable: false),
                        Column = c.String(),
                        ErrorType = c.Int(nullable: false),
                        Message = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataEntries", t => t.DataEntryId, cascadeDelete: true)
                .Index(t => t.DataEntryId);
            
            CreateTable(
                "dbo.DataSources",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserName = c.String(),
                        DateEdit = c.DateTime(nullable: false),
                        InputStatus = c.Int(nullable: false),
                        ReferenceNotes = c.String(),
                        SourceType = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SourceErrors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DataSourceId = c.Guid(nullable: false),
                        ErrorType = c.Int(nullable: false),
                        ErrorMessage = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataSources", t => t.DataSourceId, cascadeDelete: true)
                .Index(t => t.DataSourceId);
            
            CreateTable(
                "dbo.Factors",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.CarbonEmissionEntries",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    EntryDate = c.DateTime(nullable: false),
                    ActivityGroupNode = c.HierarchyId(),
                    CostCentreNode = c.HierarchyId(),
                    CalculationDate = c.DateTime(nullable: false),
                    Units = c.Decimal(nullable: false, precision: 22, scale: 8),
                    Money = c.Decimal(nullable: false, precision: 22, scale: 8),
                    CarbonEmissions = c.Decimal(nullable: false, precision: 22, scale: 8),
                    DataEntryId = c.Guid(nullable: false),
                    CostCentreCostCode = c.String(maxLength: 128),
                    ActivityGroupId = c.Guid(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ActivityGroups", t => t.ActivityGroupId)
                .ForeignKey("dbo.CostCentres", t => t.CostCentreCostCode)
                .ForeignKey("dbo.DataEntries", t => t.DataEntryId, cascadeDelete: true)
                .Index(t => t.DataEntryId)
                .Index(t => t.CostCentreCostCode)
                .Index(t => t.ActivityGroupId)
                .Index(t => new {t.EntryDate, t.ActivityGroupNode, t.CostCentreNode});
            
            CreateTable(
                "dbo.CostCentres",
                c => new
                    {
                        CostCode = c.String(nullable: false, maxLength: 128),
                        Node = c.HierarchyId(),
                        Color = c.String(maxLength: 6),
                        CurrencyCode = c.String(),
                        OrderId = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        ConsumptionType = c.Int(),
                        ParentCostCentreCostCode = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.CostCode)
                .ForeignKey("dbo.CostCentres", t => t.ParentCostCentreCostCode)
                .Index(t => t.ParentCostCentreCostCode);
            
            CreateTable(
                "dbo.Census",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DisplayName = c.String(),
                        CompanyName = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        EmployeesCovered = c.Int(nullable: false),
                        TotalEmployees = c.Int(nullable: false),
                        SquareMeters = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScopeBoundries = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EmissionTargets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InitialDate = c.DateTime(nullable: false),
                        InitialAmount = c.Decimal(nullable: false, precision: 22, scale: 8),
                        TargetDate = c.DateTime(nullable: false),
                        TargetAmount = c.Decimal(nullable: false, precision: 22, scale: 8),
                        TargetType = c.Int(nullable: false),
                        ActivityGroupId = c.Guid(nullable: false),
                        CostCentreCostCode = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ActivityGroups", t => t.ActivityGroupId, cascadeDelete: true)
                .ForeignKey("dbo.CostCentres", t => t.CostCentreCostCode)
                .Index(t => t.ActivityGroupId)
                .Index(t => t.CostCentreCostCode);
            
            CreateTable(
                "dbo.FootNotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CensusId = c.Int(nullable: false),
                        Comments = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Census", t => t.CensusId, cascadeDelete: true)
                .Index(t => t.CensusId);
            
            CreateTable(
                "dbo.GraphSeries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ActivityGroupId = c.Guid(nullable: false),
                        CostCentreCostCode = c.String(maxLength: 128),
                        Name = c.String(),
                        TargetType = c.Int(nullable: false),
                        IncludeTarget = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ActivityGroups", t => t.ActivityGroupId, cascadeDelete: true)
                .ForeignKey("dbo.CostCentres", t => t.CostCentreCostCode)
                .Index(t => t.ActivityGroupId)
                .Index(t => t.CostCentreCostCode);
            
            CreateTable(
                "dbo.UserProfiles",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Email = c.String(),
                        UserName = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Variances",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CalculationId = c.Guid(nullable: false),
                        ColumnName = c.String(),
                        MaxValue = c.Decimal(nullable: false, precision: 22, scale: 8),
                        MinValue = c.Decimal(nullable: false, precision: 22, scale: 8),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Calculations", t => t.CalculationId, cascadeDelete: true)
                .Index(t => t.CalculationId);
            
            CreateTable(
                "dbo.CalculationActivityGroups",
                c => new
                    {
                        Calculation_Id = c.Guid(nullable: false),
                        ActivityGroup_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Calculation_Id, t.ActivityGroup_Id })
                .ForeignKey("dbo.Calculations", t => t.Calculation_Id, cascadeDelete: true)
                .ForeignKey("dbo.ActivityGroups", t => t.ActivityGroup_Id, cascadeDelete: true)
                .Index(t => t.Calculation_Id)
                .Index(t => t.ActivityGroup_Id);
            
            CreateTable(
                "dbo.FactorCalculations",
                c => new
                    {
                        Factor_Id = c.Guid(nullable: false),
                        Calculation_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Factor_Id, t.Calculation_Id })
                .ForeignKey("dbo.Factors", t => t.Factor_Id, cascadeDelete: true)
                .ForeignKey("dbo.Calculations", t => t.Calculation_Id, cascadeDelete: true)
                .Index(t => t.Factor_Id)
                .Index(t => t.Calculation_Id);
            
            CreateTable(
                "dbo.CensusCostCentres",
                c => new
                    {
                        Census_Id = c.Int(nullable: false),
                        CostCentre_CostCode = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Census_Id, t.CostCentre_CostCode })
                .ForeignKey("dbo.Census", t => t.Census_Id, cascadeDelete: true)
                .ForeignKey("dbo.CostCentres", t => t.CostCentre_CostCode, cascadeDelete: true)
                .Index(t => t.Census_Id)
                .Index(t => t.CostCentre_CostCode);
            
            CreateTable(
                "dbo.FuelData",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FuelType = c.Int(),
                        UOM = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataEntries", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.CourierData",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ServiceType = c.Int(),
                        ChargeMass = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataEntries", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.ManualDataSource",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DisplayType = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataSources", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.CarHireData",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CarGroupBill = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataEntries", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.WaterData",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataEntries", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.AirTravelRouteData",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TravelClass = c.Int(nullable: false),
                        Reversal = c.Boolean(nullable: false),
                        FromCode = c.String(),
                        ToCode = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataEntries", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.ElectricityData",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ElectricityType = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataEntries", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.CourierRouteData",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ServiceType = c.Int(),
                        ChargeMass = c.Decimal(precision: 18, scale: 2),
                        FromCode = c.String(),
                        ToCode = c.String(),
                        Reversal = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataEntries", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.RefrigerantData",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RefrigerantType = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataEntries", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.FileDataSource",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FileHash = c.String(),
                        OriginalFileName = c.String(),
                        CurrentFileName = c.String(),
                        HandlerName = c.String(),
                        MediaType = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataSources", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.WasteData",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        WasteType = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataEntries", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.AccommodationData",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataEntries", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.PaperData",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PaperType = c.Int(),
                        PaperUom = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataEntries", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.CommutingData",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CommutingType = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataEntries", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.FleetData",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Scope = c.Int(),
                        FuelType = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataEntries", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.AirTravelData",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TravelClass = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataEntries", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AirTravelData", "Id", "dbo.DataEntries");
            DropForeignKey("dbo.FleetData", "Id", "dbo.DataEntries");
            DropForeignKey("dbo.CommutingData", "Id", "dbo.DataEntries");
            DropForeignKey("dbo.PaperData", "Id", "dbo.DataEntries");
            DropForeignKey("dbo.AccommodationData", "Id", "dbo.DataEntries");
            DropForeignKey("dbo.WasteData", "Id", "dbo.DataEntries");
            DropForeignKey("dbo.FileDataSource", "Id", "dbo.DataSources");
            DropForeignKey("dbo.RefrigerantData", "Id", "dbo.DataEntries");
            DropForeignKey("dbo.CourierRouteData", "Id", "dbo.DataEntries");
            DropForeignKey("dbo.ElectricityData", "Id", "dbo.DataEntries");
            DropForeignKey("dbo.AirTravelRouteData", "Id", "dbo.DataEntries");
            DropForeignKey("dbo.WaterData", "Id", "dbo.DataEntries");
            DropForeignKey("dbo.CarHireData", "Id", "dbo.DataEntries");
            DropForeignKey("dbo.ManualDataSource", "Id", "dbo.DataSources");
            DropForeignKey("dbo.CourierData", "Id", "dbo.DataEntries");
            DropForeignKey("dbo.FuelData", "Id", "dbo.DataEntries");
            DropForeignKey("dbo.Variances", "CalculationId", "dbo.Calculations");
            DropForeignKey("dbo.GraphSeries", "CostCentreCostCode", "dbo.CostCentres");
            DropForeignKey("dbo.GraphSeries", "ActivityGroupId", "dbo.ActivityGroups");
            DropForeignKey("dbo.FootNotes", "CensusId", "dbo.Census");
            DropForeignKey("dbo.EmissionTargets", "CostCentreCostCode", "dbo.CostCentres");
            DropForeignKey("dbo.EmissionTargets", "ActivityGroupId", "dbo.ActivityGroups");
            DropForeignKey("dbo.CarbonEmissionEntries", "DataEntryId", "dbo.DataEntries");
            DropForeignKey("dbo.CarbonEmissionEntries", "CostCentreCostCode", "dbo.CostCentres");
            DropForeignKey("dbo.CostCentres", "ParentCostCentreCostCode", "dbo.CostCentres");
            DropForeignKey("dbo.CensusCostCentres", "CostCentre_CostCode", "dbo.CostCentres");
            DropForeignKey("dbo.CensusCostCentres", "Census_Id", "dbo.Census");
            DropForeignKey("dbo.CarbonEmissionEntries", "ActivityGroupId", "dbo.ActivityGroups");
            DropForeignKey("dbo.ActivityGroups", "ParentGroupId", "dbo.ActivityGroups");
            DropForeignKey("dbo.FactorCalculations", "Calculation_Id", "dbo.Calculations");
            DropForeignKey("dbo.FactorCalculations", "Factor_Id", "dbo.Factors");
            DropForeignKey("dbo.SourceErrors", "DataSourceId", "dbo.DataSources");
            DropForeignKey("dbo.DataEntries", "SourceId", "dbo.DataSources");
            DropForeignKey("dbo.DataErrors", "DataEntryId", "dbo.DataEntries");
            DropForeignKey("dbo.DataEntries", "CalculationId", "dbo.Calculations");
            DropForeignKey("dbo.CalculationActivityGroups", "ActivityGroup_Id", "dbo.ActivityGroups");
            DropForeignKey("dbo.CalculationActivityGroups", "Calculation_Id", "dbo.Calculations");
            DropIndex("dbo.AirTravelData", new[] { "Id" });
            DropIndex("dbo.FleetData", new[] { "Id" });
            DropIndex("dbo.CommutingData", new[] { "Id" });
            DropIndex("dbo.PaperData", new[] { "Id" });
            DropIndex("dbo.AccommodationData", new[] { "Id" });
            DropIndex("dbo.WasteData", new[] { "Id" });
            DropIndex("dbo.FileDataSource", new[] { "Id" });
            DropIndex("dbo.RefrigerantData", new[] { "Id" });
            DropIndex("dbo.CourierRouteData", new[] { "Id" });
            DropIndex("dbo.ElectricityData", new[] { "Id" });
            DropIndex("dbo.AirTravelRouteData", new[] { "Id" });
            DropIndex("dbo.WaterData", new[] { "Id" });
            DropIndex("dbo.CarHireData", new[] { "Id" });
            DropIndex("dbo.ManualDataSource", new[] { "Id" });
            DropIndex("dbo.CourierData", new[] { "Id" });
            DropIndex("dbo.FuelData", new[] { "Id" });
            DropIndex("dbo.CensusCostCentres", new[] { "CostCentre_CostCode" });
            DropIndex("dbo.CensusCostCentres", new[] { "Census_Id" });
            DropIndex("dbo.FactorCalculations", new[] { "Calculation_Id" });
            DropIndex("dbo.FactorCalculations", new[] { "Factor_Id" });
            DropIndex("dbo.CalculationActivityGroups", new[] { "ActivityGroup_Id" });
            DropIndex("dbo.CalculationActivityGroups", new[] { "Calculation_Id" });
            DropIndex("dbo.Variances", new[] { "CalculationId" });
            DropIndex("dbo.GraphSeries", new[] { "CostCentreCostCode" });
            DropIndex("dbo.GraphSeries", new[] { "ActivityGroupId" });
            DropIndex("dbo.FootNotes", new[] { "CensusId" });
            DropIndex("dbo.EmissionTargets", new[] { "CostCentreCostCode" });
            DropIndex("dbo.EmissionTargets", new[] { "ActivityGroupId" });
            DropIndex("dbo.CostCentres", new[] { "ParentCostCentreCostCode" });
            DropIndex("dbo.CarbonEmissionEntries", new[] { "ActivityGroupId" });
            DropIndex("dbo.CarbonEmissionEntries", new[] { "CostCentreCostCode" });
            DropIndex("dbo.CarbonEmissionEntries", new[] { "DataEntryId" });
            DropIndex("dbo.SourceErrors", new[] { "DataSourceId" });
            DropIndex("dbo.DataErrors", new[] { "DataEntryId" });
            DropIndex("dbo.DataEntries", new[] { "CalculationId" });
            DropIndex("dbo.DataEntries", new[] { "SourceId" });
            DropIndex("dbo.ActivityGroups", new[] { "ParentGroupId" });
            DropTable("dbo.AirTravelData");
            DropTable("dbo.FleetData");
            DropTable("dbo.CommutingData");
            DropTable("dbo.PaperData");
            DropTable("dbo.AccommodationData");
            DropTable("dbo.WasteData");
            DropTable("dbo.FileDataSource");
            DropTable("dbo.RefrigerantData");
            DropTable("dbo.CourierRouteData");
            DropTable("dbo.ElectricityData");
            DropTable("dbo.AirTravelRouteData");
            DropTable("dbo.WaterData");
            DropTable("dbo.CarHireData");
            DropTable("dbo.ManualDataSource");
            DropTable("dbo.CourierData");
            DropTable("dbo.FuelData");
            DropTable("dbo.CensusCostCentres");
            DropTable("dbo.FactorCalculations");
            DropTable("dbo.CalculationActivityGroups");
            DropTable("dbo.Variances");
            DropTable("dbo.UserProfiles");
            DropTable("dbo.GraphSeries");
            DropTable("dbo.FootNotes");
            DropTable("dbo.EmissionTargets");
            DropTable("dbo.Census");
            DropTable("dbo.CostCentres");
            DropTable("dbo.CarbonEmissionEntries");
            DropTable("dbo.Factors");
            DropTable("dbo.SourceErrors");
            DropTable("dbo.DataSources");
            DropTable("dbo.DataErrors");
            DropTable("dbo.DataEntries");
            DropTable("dbo.Calculations");
            DropTable("dbo.ActivityGroups");
        }
    }
}
