namespace AIMS.Data.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Dues", "CreatedBy", c => c.Long(nullable: false));
            AddColumn("dbo.Dues", "DateCreated", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.Dues", "DateLastModified", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.Dues", "LastModifiedBy", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Dues", "LastModifiedBy");
            DropColumn("dbo.Dues", "DateLastModified");
            DropColumn("dbo.Dues", "DateCreated");
            DropColumn("dbo.Dues", "CreatedBy");
        }
    }
}
