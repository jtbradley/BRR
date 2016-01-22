namespace Blue_Ribbon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CampaignDailyModifcation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Campaign", "DailyLimit", c => c.Int(nullable: false));
            AddColumn("dbo.Campaign", "DailyLimitReached", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Campaign", "DailyLimitReached");
            DropColumn("dbo.Campaign", "DailyLimit");
        }
    }
}
