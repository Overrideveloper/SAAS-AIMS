namespace AIMS.Data.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Member",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Surname = c.String(nullable: false, unicode: false),
                        MidName = c.String(unicode: false),
                        FirstName = c.String(nullable: false, unicode: false),
                        MatricNumber = c.String(nullable: false, maxLength: 20, storeType: "nvarchar"),
                        StateOfOrigin = c.Int(nullable: false),
                        YearOfAdmission = c.DateTime(nullable: false, precision: 0),
                        LevelOfAdmission = c.Int(nullable: false),
                        CreatedBy = c.Long(nullable: false),
                        DateCreated = c.DateTime(nullable: false, precision: 0),
                        DateLastModified = c.DateTime(nullable: false, precision: 0),
                        LastModifiedBy = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.MatricNumber, unique: true);
        }
        
        public override void Down()
        {
            DropTable("dbo.Session");
            DropIndex("dbo.Member", new[] { "MatricNumber" });
            DropTable("dbo.Member");
        }
    }
}
