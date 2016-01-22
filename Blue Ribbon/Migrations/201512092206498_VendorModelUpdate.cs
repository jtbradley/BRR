namespace Blue_Ribbon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VendorModelUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vendor", "ContactName", c => c.String());
            AddColumn("dbo.Vendor", "PhoneNumber", c => c.String());
            AddColumn("dbo.Vendor", "Email", c => c.String());
            AddColumn("dbo.Vendor", "VendorNotes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Vendor", "VendorNotes");
            DropColumn("dbo.Vendor", "Email");
            DropColumn("dbo.Vendor", "PhoneNumber");
            DropColumn("dbo.Vendor", "ContactName");
        }
    }
}
