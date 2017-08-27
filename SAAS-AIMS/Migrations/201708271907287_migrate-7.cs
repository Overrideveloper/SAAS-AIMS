namespace SAAS_AIMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Email", c => c.String(unicode: false));
            AddColumn("dbo.AspNetUsers", "RoleID", c => c.Long());
            CreateIndex("dbo.AspNetUsers", "RoleID");
            AddForeignKey("dbo.AspNetUsers", "RoleID", "dbo.Roles", "ID", cascadeDelete: true);
            AlterColumn("dbo.AspNetUsers", "UserName", c => c.String(nullable: false, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "UserName", c => c.String(unicode: false));
            DropForeignKey("dbo.AspNetUsers", "RoleID", "dbo.Rols");
            DropIndex("dbo.AspNetUsers", new[] { "RoleID" });
            DropColumn("dbo.AspNetUsers", "RoleID");
            DropColumn("dbo.AspNetUsers", "Email");
        }
    }
}
