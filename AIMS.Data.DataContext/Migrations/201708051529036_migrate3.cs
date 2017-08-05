namespace AIMS.Data.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Member", "Gender", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Member", "Gender");
        }
    }
}
