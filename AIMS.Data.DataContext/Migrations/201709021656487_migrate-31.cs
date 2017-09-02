namespace AIMS.Data.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate31 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Roles", "CanManageUsers", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Roles", "CanManageUsers");
        }
    }
}
