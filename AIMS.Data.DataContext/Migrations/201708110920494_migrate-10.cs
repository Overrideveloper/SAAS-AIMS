namespace AIMS.Data.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate10 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Meeting",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Title = c.String(nullable: false, unicode: false),
                        Date = c.DateTime(nullable: false, precision: 0),
                        Venue = c.String(nullable: false, unicode: false),
                        FileUpload = c.String(unicode: false),
                        SessionID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Session", t => t.SessionID, cascadeDelete: true)
                .Index(t => t.SessionID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Meeting", "SessionID", "dbo.Session");
            DropIndex("dbo.Meeting", new[] { "SessionID" });
            DropTable("dbo.Meeting");
        }
    }
}
