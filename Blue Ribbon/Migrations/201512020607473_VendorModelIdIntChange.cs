namespace Blue_Ribbon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VendorModelIdIntChange : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Campaign", "VendorID", "dbo.Vendor");
            DropIndex("dbo.Campaign", new[] { "VendorID" });
            DropPrimaryKey("dbo.Vendor");
            AddColumn("dbo.Campaign", "Vendor_VendorId", c => c.Int());
            AlterColumn("dbo.Campaign", "VendorID", c => c.String());
            AlterColumn("dbo.Vendor", "VendorId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Vendor", "VendorId");
            CreateIndex("dbo.Campaign", "Vendor_VendorId");
            AddForeignKey("dbo.Campaign", "Vendor_VendorId", "dbo.Vendor", "VendorId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Campaign", "Vendor_VendorId", "dbo.Vendor");
            DropIndex("dbo.Campaign", new[] { "Vendor_VendorId" });
            DropPrimaryKey("dbo.Vendor");
            AlterColumn("dbo.Vendor", "VendorId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Campaign", "VendorID", c => c.String(maxLength: 128));
            DropColumn("dbo.Campaign", "Vendor_VendorId");
            AddPrimaryKey("dbo.Vendor", "VendorId");
            CreateIndex("dbo.Campaign", "VendorID");
            AddForeignKey("dbo.Campaign", "VendorID", "dbo.Vendor", "VendorId");
        }
    }
}
