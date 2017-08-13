namespace SAAS_AIMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "RoleID", c => c.Long());
            CreateIndex("dbo.AspNetUsers", "RoleID");
            AddForeignKey("dbo.AspNetUsers", "RoleID", "dbo.Role", "ID", cascadeDelete: true);
        }

        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "RoleID", "dbo.Role");
            DropIndex("dbo.AspNetUsers", new[] { "RoleID" });
            DropColumn("dbo.AspNetUsers", "RoleID");
        }
    }
}
