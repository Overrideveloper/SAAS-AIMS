namespace AIMS.Data.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate11 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Meeting", "CreatedBy", c => c.Long(nullable: false));
            AddColumn("dbo.Meeting", "DateCreated", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.Meeting", "DateLastModified", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.Meeting", "LastModifiedBy", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Meeting", "LastModifiedBy");
            DropColumn("dbo.Meeting", "DateLastModified");
            DropColumn("dbo.Meeting", "DateCreated");
            DropColumn("dbo.Meeting", "CreatedBy");
        }
    }
}
