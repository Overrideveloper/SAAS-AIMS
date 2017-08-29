namespace AIMS.Data.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate26 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.IncomeCategory", "SessionID", c => c.Long(nullable: false));
            AddColumn("dbo.IncomeCategory", "CreatedBy", c => c.Long(nullable: false));
            AddColumn("dbo.IncomeCategory", "DateCreated", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.IncomeCategory", "DateLastModified", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.IncomeCategory", "LastModifiedBy", c => c.Long(nullable: false));
            AddColumn("dbo.IncomeItem", "CreatedBy", c => c.Long(nullable: false));
            AddColumn("dbo.IncomeItem", "DateCreated", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.IncomeItem", "DateLastModified", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.IncomeItem", "LastModifiedBy", c => c.Long(nullable: false));
            CreateIndex("dbo.IncomeCategory", "SessionID");
            AddForeignKey("dbo.IncomeCategory", "SessionID", "dbo.Session", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IncomeCategory", "SessionID", "dbo.Session");
            DropIndex("dbo.IncomeCategory", new[] { "SessionID" });
            DropColumn("dbo.IncomeItem", "LastModifiedBy");
            DropColumn("dbo.IncomeItem", "DateLastModified");
            DropColumn("dbo.IncomeItem", "DateCreated");
            DropColumn("dbo.IncomeItem", "CreatedBy");
            DropColumn("dbo.IncomeCategory", "LastModifiedBy");
            DropColumn("dbo.IncomeCategory", "DateLastModified");
            DropColumn("dbo.IncomeCategory", "DateCreated");
            DropColumn("dbo.IncomeCategory", "CreatedBy");
            DropColumn("dbo.IncomeCategory", "SessionID");
        }
    }
}
