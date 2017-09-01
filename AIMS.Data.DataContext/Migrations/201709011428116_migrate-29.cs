namespace AIMS.Data.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate29 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Exco",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        MatricNo = c.String(nullable: false, unicode: false),
                        LastName = c.String(nullable: false, unicode: false),
                        FirstName = c.String(nullable: false, unicode: false),
                        Post = c.Int(nullable: false),
                        SessionID = c.Long(nullable: false),
                        CreatedBy = c.Long(nullable: false),
                        DateCreated = c.DateTime(nullable: false, precision: 0),
                        DateLastModified = c.DateTime(nullable: false, precision: 0),
                        LastModifiedBy = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Session", t => t.SessionID, cascadeDelete: true)
                .Index(t => t.SessionID);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Exco", "SessionID", "dbo.Session");
            DropIndex("dbo.Exco", new[] { "SessionID" });
            DropTable("dbo.Exco");
        }
    }
}
