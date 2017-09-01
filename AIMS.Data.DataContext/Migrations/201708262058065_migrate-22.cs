namespace AIMS.Data.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate22 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Project",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Title = c.String(nullable: false, unicode: false),
                        Description = c.String(nullable: false, unicode: false),
                        FileUpload = c.String(nullable: false, unicode: false),
                        SessionID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Session", t => t.SessionID, cascadeDelete: true)
                .Index(t => t.SessionID);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Project", "SessionID", "dbo.Session");
            DropIndex("dbo.Project", new[] { "SessionID" });
            DropTable("dbo.Project");
        }
    }
}
