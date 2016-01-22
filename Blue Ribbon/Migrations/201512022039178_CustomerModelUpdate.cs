namespace Blue_Ribbon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomerModelUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customer", "LastReviewCheck", c => c.DateTime(nullable: false));
            DropColumn("dbo.Customer", "test");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Customer", "test", c => c.Int(nullable: false));
            DropColumn("dbo.Customer", "LastReviewCheck");
        }
    }
}
