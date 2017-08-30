namespace AIMS.Data.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate27 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExpenseCategory",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Title = c.String(nullable: false, unicode: false),
                        SessionID = c.Long(nullable: false),
                        CreatedBy = c.Long(nullable: false),
                        DateCreated = c.DateTime(nullable: false, precision: 0),
                        DateLastModified = c.DateTime(nullable: false, precision: 0),
                        LastModifiedBy = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Session", t => t.SessionID, cascadeDelete: true)
                .Index(t => t.SessionID);
            
            CreateTable(
                "dbo.ExpenseItem",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Title = c.String(nullable: false, unicode: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExpenseCategoryID = c.Long(nullable: false),
                        CreatedBy = c.Long(nullable: false),
                        DateCreated = c.DateTime(nullable: false, precision: 0),
                        DateLastModified = c.DateTime(nullable: false, precision: 0),
                        LastModifiedBy = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ExpenseCategory", t => t.ExpenseCategoryID, cascadeDelete: true)
                .Index(t => t.ExpenseCategoryID);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExpenseCategory", "SessionID", "dbo.Session");
            DropForeignKey("dbo.ExpenseItem", "ExpenseCategoryID", "dbo.ExpenseCategory");
            DropIndex("dbo.ExpenseItem", new[] { "ExpenseCategoryID" });
            DropIndex("dbo.ExpenseCategory", new[] { "SessionID" });
            DropTable("dbo.ExpenseItem");
            DropTable("dbo.ExpenseCategory");
        }
    }
}
