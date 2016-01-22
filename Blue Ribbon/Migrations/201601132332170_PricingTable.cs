namespace Blue_Ribbon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PricingTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PricingStructure",
                c => new
                    {
                        PricingStructureId = c.Int(nullable: false, identity: true),
                        T1 = c.Double(nullable: false),
                        T2 = c.Double(nullable: false),
                        T3 = c.Double(nullable: false),
                        P1 = c.Double(nullable: false),
                        P2 = c.Double(nullable: false),
                        P3 = c.Double(nullable: false),
                        V1 = c.Double(nullable: false),
                        V2 = c.Double(nullable: false),
                        V3 = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.PricingStructureId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PricingStructure");
        }
    }
}
