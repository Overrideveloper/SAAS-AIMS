namespace AIMS.Data.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate21 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Memo", "Date", c => c.DateTime(nullable: false, precision: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Memo", "Date");
        }
    }
}
