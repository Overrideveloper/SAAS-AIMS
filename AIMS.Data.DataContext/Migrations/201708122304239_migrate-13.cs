namespace AIMS.Data.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate13 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Role",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Title = c.String(nullable: false, unicode: false),
                        CanManageSessions = c.Boolean(nullable: false),
                        CanManageMembers = c.Boolean(nullable: false),
                        CanManageEvents = c.Boolean(nullable: false),
                        CanManageExecutives = c.Boolean(nullable: false),
                        CanManageExpenses = c.Boolean(nullable: false),
                        CanManageIncome = c.Boolean(nullable: false),
                        CanManageMeetings = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
        }
        
        public override void Down()
        {
            DropTable("dbo.Role");
        }
    }
}
