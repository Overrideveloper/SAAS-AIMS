namespace AIMS.Data.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate28 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExpenseCategory", "Title", c => c.String(nullable: false, unicode: false));
            DropColumn("dbo.ExpenseCategory", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ExpenseCategory", "Name", c => c.String(nullable: false, unicode: false));
            DropColumn("dbo.ExpenseCategory", "Title");
        }
    }
}
