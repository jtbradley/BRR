namespace Blue_Ribbon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReviewModelAddCustomerAlert : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Review", "CustomerAlert", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Review", "CustomerAlert");
        }
    }
}
