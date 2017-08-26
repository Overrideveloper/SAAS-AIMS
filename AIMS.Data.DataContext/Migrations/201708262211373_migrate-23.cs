namespace AIMS.Data.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate23 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Project", "CreatedBy", c => c.Long(nullable: false));
            AddColumn("dbo.Project", "DateCreated", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.Project", "DateLastModified", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.Project", "LastModifiedBy", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Project", "LastModifiedBy");
            DropColumn("dbo.Project", "DateLastModified");
            DropColumn("dbo.Project", "DateCreated");
            DropColumn("dbo.Project", "CreatedBy");
        }
    }
}
