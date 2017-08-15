namespace AIMS.Data.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate17 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Meeting", "Semester", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Meeting", "Semester");
        }
    }
}
