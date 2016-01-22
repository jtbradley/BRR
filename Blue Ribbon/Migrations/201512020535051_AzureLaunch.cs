namespace Blue_Ribbon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AzureLaunch : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Campaign",
                c => new
                    {
                        CampaignID = c.Int(nullable: false, identity: true),
                        ASIN = c.String(),
                        VendorID = c.String(maxLength: 128),
                        Name = c.String(),
                        OpenCampaign = c.Boolean(nullable: false),
                        Category = c.String(),
                        ImageUrl = c.String(),
                        Description = c.String(),
                        TextValid = c.Boolean(nullable: false),
                        TextGoal = c.Int(nullable: false),
                        PhotoValid = c.Boolean(nullable: false),
                        PhotoGoal = c.Int(nullable: false),
                        VideoValid = c.Boolean(nullable: false),
                        VideoGoal = c.Int(nullable: false),
                        CurrentRequests = c.Int(nullable: false),
                        ReviewsStillNeeded = c.Int(nullable: false),
                        RetailPrice = c.String(),
                        SalePrice = c.String(),
                        SalePriceNumerical = c.Double(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        CloseDate = c.DateTime(),
                        CalculatedDiscount = c.Double(nullable: false),
                        VendorsPurchaseInstructions = c.String(),
                        VendorsPurchaseURL = c.String(),
                    })
                .PrimaryKey(t => t.CampaignID)
                .ForeignKey("dbo.Vendor", t => t.VendorID)
                .Index(t => t.VendorID);
            
            CreateTable(
                "dbo.ItemRequest",
                c => new
                    {
                        ItemRequestID = c.Int(nullable: false, identity: true),
                        CampaignID = c.Int(nullable: false),
                        CustomerID = c.String(maxLength: 128),
                        RequestDate = c.DateTime(nullable: false),
                        ActiveRequest = c.Boolean(nullable: false),
                        ReviewType = c.Int(nullable: false),
                        Selected = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ItemRequestID)
                .ForeignKey("dbo.Campaign", t => t.CampaignID, cascadeDelete: true)
                .ForeignKey("dbo.Customer", t => t.CustomerID)
                .Index(t => t.CampaignID)
                .Index(t => t.CustomerID);
            
            CreateTable(
                "dbo.Customer",
                c => new
                    {
                        CustomerID = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        test = c.Int(nullable: false),
                        Qualified = c.Boolean(nullable: false),
                        JoinDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerID);
            
            CreateTable(
                "dbo.Review",
                c => new
                    {
                        ReviewID = c.Int(nullable: false, identity: true),
                        CampaignID = c.Int(nullable: false),
                        CustomerID = c.String(maxLength: 128),
                        SelectedDate = c.DateTime(nullable: false),
                        DiscountCode = c.String(),
                        Reviewed = c.Boolean(nullable: false),
                        ProductRating = c.Int(),
                        VideoReview = c.Boolean(nullable: false),
                        PhotoReview = c.Boolean(nullable: false),
                        ReviewLength = c.Int(nullable: false),
                        ReviewLink = c.String(),
                        ReviewTypeExpected = c.Int(nullable: false),
                        ReviewDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ReviewID)
                .ForeignKey("dbo.Campaign", t => t.CampaignID, cascadeDelete: true)
                .ForeignKey("dbo.Customer", t => t.CustomerID)
                .Index(t => t.CampaignID)
                .Index(t => t.CustomerID);
            
            CreateTable(
                "dbo.Vendor",
                c => new
                    {
                        VendorId = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        JoinDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.VendorId);
            
            CreateTable(
                "dbo.DiscountCode",
                c => new
                    {
                        DiscountCodeID = c.Int(nullable: false, identity: true),
                        CampaignID = c.Int(nullable: false),
                        Code = c.String(),
                    })
                .PrimaryKey(t => t.DiscountCodeID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Campaign", "VendorID", "dbo.Vendor");
            DropForeignKey("dbo.Review", "CustomerID", "dbo.Customer");
            DropForeignKey("dbo.Review", "CampaignID", "dbo.Campaign");
            DropForeignKey("dbo.ItemRequest", "CustomerID", "dbo.Customer");
            DropForeignKey("dbo.ItemRequest", "CampaignID", "dbo.Campaign");
            DropIndex("dbo.Review", new[] { "CustomerID" });
            DropIndex("dbo.Review", new[] { "CampaignID" });
            DropIndex("dbo.ItemRequest", new[] { "CustomerID" });
            DropIndex("dbo.ItemRequest", new[] { "CampaignID" });
            DropIndex("dbo.Campaign", new[] { "VendorID" });
            DropTable("dbo.DiscountCode");
            DropTable("dbo.Vendor");
            DropTable("dbo.Review");
            DropTable("dbo.Customer");
            DropTable("dbo.ItemRequest");
            DropTable("dbo.Campaign");
        }
    }
}
