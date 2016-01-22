namespace Blue_Ribbon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotificationLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NotificationLog",
                c => new
                    {
                        NotificationLogID = c.Int(nullable: false, identity: true),
                        CampaignID = c.Int(nullable: false),
                        LogTimestamp = c.DateTime(nullable: false),
                        Message = c.String(),
                    })
                .PrimaryKey(t => t.NotificationLogID)
                .ForeignKey("dbo.Campaign", t => t.CampaignID, cascadeDelete: true)
                .Index(t => t.CampaignID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NotificationLog", "CampaignID", "dbo.Campaign");
            DropIndex("dbo.NotificationLog", new[] { "CampaignID" });
            DropTable("dbo.NotificationLog");
        }
    }
}
