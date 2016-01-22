namespace Blue_Ribbon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCustomerToVendorTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vendor", "CustomerID", c => c.String(maxLength: 128));
            CreateIndex("dbo.Vendor", "CustomerID");
            AddForeignKey("dbo.Vendor", "CustomerID", "dbo.Customer", "CustomerID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Vendor", "CustomerID", "dbo.Customer");
            DropIndex("dbo.Vendor", new[] { "CustomerID" });
            DropColumn("dbo.Vendor", "CustomerID");
        }
    }
}
