namespace Blue_Ribbon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateCampModelVendorInt : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Campaign", "Vendor_VendorId", "dbo.Vendor");
            DropIndex("dbo.Campaign", new[] { "Vendor_VendorId" });
            DropColumn("dbo.Campaign", "VendorID");
            RenameColumn(table: "dbo.Campaign", name: "Vendor_VendorId", newName: "VendorID");
            AlterColumn("dbo.Campaign", "VendorID", c => c.Int(nullable: false));
            AlterColumn("dbo.Campaign", "VendorID", c => c.Int(nullable: false));
            CreateIndex("dbo.Campaign", "VendorID");
            AddForeignKey("dbo.Campaign", "VendorID", "dbo.Vendor", "VendorId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Campaign", "VendorID", "dbo.Vendor");
            DropIndex("dbo.Campaign", new[] { "VendorID" });
            AlterColumn("dbo.Campaign", "VendorID", c => c.Int());
            AlterColumn("dbo.Campaign", "VendorID", c => c.String());
            RenameColumn(table: "dbo.Campaign", name: "VendorID", newName: "Vendor_VendorId");
            AddColumn("dbo.Campaign", "VendorID", c => c.String());
            CreateIndex("dbo.Campaign", "Vendor_VendorId");
            AddForeignKey("dbo.Campaign", "Vendor_VendorId", "dbo.Vendor", "VendorId");
        }
    }
}
