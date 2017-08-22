namespace SAAS_AIMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate5 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "UserName", c => c.String(nullable: false, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "UserName", c => c.String(unicode: false));
        }
    }
}
