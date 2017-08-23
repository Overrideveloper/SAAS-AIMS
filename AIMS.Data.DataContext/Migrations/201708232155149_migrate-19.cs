namespace AIMS.Data.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate19 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Memo",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Description = c.String(nullable: false, unicode: false),
                        FileUpload = c.String(nullable: false, unicode: false),
                        CreatedBy = c.Long(nullable: false),
                        DateCreated = c.DateTime(nullable: false, precision: 0),
                        DateLastModified = c.DateTime(nullable: false, precision: 0),
                        LastModifiedBy = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
        }
        
        public override void Down()
        {
            DropTable("dbo.Memo");
        }
    }
}
