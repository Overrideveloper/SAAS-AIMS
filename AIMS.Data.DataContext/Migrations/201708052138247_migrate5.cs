namespace AIMS.Data.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate5 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Member", "YearOfAdmission", c => c.String(nullable: false, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Member", "YearOfAdmission", c => c.DateTime(nullable: false, precision: 0));
        }
    }
}
