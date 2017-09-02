namespace AIMS.Data.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate30 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Roles", "CanManageMemos", c => c.Boolean(nullable: false));
            AddColumn("dbo.Roles", "CanManageProjects", c => c.Boolean(nullable: false));
            AddColumn("dbo.Roles", "CanManageBudget", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Roles", "CanManageMemos");
            DropColumn("dbo.Roles", "CanManageProjects");
            DropColumn("dbo.Roles", "CanManageBudget");
        }
    }
}
