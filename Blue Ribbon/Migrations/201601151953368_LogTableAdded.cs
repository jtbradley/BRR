namespace Blue_Ribbon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogTableAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TaskLog",
                c => new
                    {
                        TaskLogId = c.Int(nullable: false, identity: true),
                        TaskDescription = c.String(),
                        SuccessDatestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TaskLogId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TaskLog");
        }
    }
}
