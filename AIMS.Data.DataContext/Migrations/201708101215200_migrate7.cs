namespace AIMS.Data.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate7 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Dues",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Title = c.String(nullable: false, unicode: false),
                        Level = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MemberID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Member", t => t.MemberID, cascadeDelete: true)
                .Index(t => t.MemberID);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Dues", "MemberID", "dbo.Member");
            DropIndex("dbo.Dues", new[] { "MemberID" });
            DropTable("dbo.Dues");
        }
    }
}
