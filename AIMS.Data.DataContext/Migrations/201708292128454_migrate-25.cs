namespace AIMS.Data.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate25 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IncomeCategory",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Title = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.IncomeItem",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Title = c.String(nullable: false, unicode: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IncomeCategoryID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.IncomeCategory", t => t.IncomeCategoryID, cascadeDelete: true)
                .Index(t => t.IncomeCategoryID);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IncomeItem", "IncomeCategoryID", "dbo.IncomeCategory");
            DropIndex("dbo.IncomeItem", new[] { "IncomeCategoryID" });
            DropTable("dbo.IncomeItem");
            DropTable("dbo.IncomeCategory");
        }
    }
}
