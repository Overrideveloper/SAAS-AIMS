namespace AIMS.Data.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate20 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Memo", "SessionID", c => c.Long(nullable: false));
            CreateIndex("dbo.Memo", "SessionID");
            AddForeignKey("dbo.Memo", "SessionID", "dbo.Session", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Memo", "SessionID", "dbo.Session");
            DropIndex("dbo.Memo", new[] { "SessionID" });
            DropColumn("dbo.Memo", "SessionID");
        }
    }
}
