namespace SAAS_AIMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "AssociationName", c => c.String(unicode: false));
            AddColumn("dbo.AspNetUsers", "CollegeChapter", c => c.String(unicode: false));
            AddColumn("dbo.AspNetUsers", "State", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "State");
            DropColumn("dbo.AspNetUsers", "CollegeChapter");
            DropColumn("dbo.AspNetUsers", "AssociationName");
        }
    }
}
