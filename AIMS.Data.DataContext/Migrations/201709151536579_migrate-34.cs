namespace AIMS.Data.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate34 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Dues", "CreatedBy", c => c.String(nullable: false, unicode: false));
            AlterColumn("dbo.Dues", "LastModifiedBy", c => c.String(nullable: false, unicode: false));
            AlterColumn("dbo.Event", "CreatedBy", c => c.String(nullable: false, unicode: false));
            AlterColumn("dbo.Event", "LastModifiedBy", c => c.String(nullable: false, unicode: false));
            AlterColumn("dbo.Exco", "CreatedBy", c => c.String(nullable: false, unicode: false));
            AlterColumn("dbo.Exco", "LastModifiedBy", c => c.String(nullable: false, unicode: false));
            AlterColumn("dbo.ExpenseCategory", "CreatedBy", c => c.String(nullable: false, unicode: false));
            AlterColumn("dbo.ExpenseCategory", "LastModifiedBy", c => c.String(nullable: false, unicode: false));
            AlterColumn("dbo.ExpenseItem", "CreatedBy", c => c.String(nullable: false, unicode: false));
            AlterColumn("dbo.ExpenseItem", "LastModifiedBy", c => c.String(nullable: false, unicode: false));
            AlterColumn("dbo.IncomeCategory", "CreatedBy", c => c.String(nullable: false, unicode: false));
            AlterColumn("dbo.IncomeCategory", "LastModifiedBy", c => c.String(nullable: false, unicode: false));
            AlterColumn("dbo.IncomeItem", "CreatedBy", c => c.String(nullable: false, unicode: false));
            AlterColumn("dbo.IncomeItem", "LastModifiedBy", c => c.String(nullable: false, unicode: false));
            AlterColumn("dbo.Meeting", "CreatedBy", c => c.String(nullable: false, unicode: false));
            AlterColumn("dbo.Meeting", "LastModifiedBy", c => c.String(nullable: false, unicode: false));
            AlterColumn("dbo.Member", "CreatedBy", c => c.String(nullable: false, unicode: false));
            AlterColumn("dbo.Member", "LastModifiedBy", c => c.String(nullable: false, unicode: false));
            AlterColumn("dbo.Memo", "CreatedBy", c => c.String(nullable: false, unicode: false));
            AlterColumn("dbo.Memo", "LastModifiedBy", c => c.String(nullable: false, unicode: false));
            AlterColumn("dbo.Project", "CreatedBy", c => c.String(nullable: false, unicode: false));
            AlterColumn("dbo.Project", "LastModifiedBy", c => c.String(nullable: false, unicode: false));
            AlterColumn("dbo.Session", "CreatedBy", c => c.String(nullable: false, unicode: false));
            AlterColumn("dbo.Session", "LastModifiedBy", c => c.String(nullable: false, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Dues", "CreatedBy", c => c.Long(nullable: false));
            AlterColumn("dbo.Dues", "LastModifiedBy", c => c.Long(nullable: false));
            AlterColumn("dbo.Event", "CreatedBy", c => c.Long(nullable: false));
            AlterColumn("dbo.Event", "LastModifiedBy", c => c.Long(nullable: false));
            AlterColumn("dbo.Exco", "CreatedBy", c => c.Long(nullable: false));
            AlterColumn("dbo.Exco", "LastModifiedBy", c => c.Long(nullable: false));
            AlterColumn("dbo.ExpenseCategory", "CreatedBy", c => c.Long(nullable: false ));
            AlterColumn("dbo.ExpenseCategory", "LastModifiedBy", c => c.Long(nullable: false ));
            AlterColumn("dbo.ExpenseItem", "CreatedBy", c => c.Long(nullable: false ));
            AlterColumn("dbo.ExpenseItem", "LastModifiedBy", c => c.Long(nullable: false ));
            AlterColumn("dbo.IncomeCategory", "CreatedBy", c => c.Long(nullable: false ));
            AlterColumn("dbo.IncomeCategory", "LastModifiedBy", c => c.Long(nullable: false ));
            AlterColumn("dbo.IncomeItem", "CreatedBy", c => c.Long(nullable: false ));
            AlterColumn("dbo.IncomeItem", "LastModifiedBy", c => c.Long(nullable: false ));
            AlterColumn("dbo.Meeting", "CreatedBy", c => c.Long(nullable: false ));
            AlterColumn("dbo.Meeting", "LastModifiedBy", c => c.Long(nullable: false ));
            AlterColumn("dbo.Member", "CreatedBy", c => c.Long(nullable: false ));
            AlterColumn("dbo.Member", "LastModifiedBy", c => c.Long(nullable: false ));
            AlterColumn("dbo.Memo", "CreatedBy", c => c.Long(nullable: false ));
            AlterColumn("dbo.Memo", "LastModifiedBy", c => c.Long(nullable: false ));
            AlterColumn("dbo.Project", "CreatedBy", c => c.Long(nullable: false ));
            AlterColumn("dbo.Project", "LastModifiedBy", c => c.Long(nullable: false ));
            AlterColumn("dbo.Session", "CreatedBy", c => c.Long(nullable: false ));
            AlterColumn("dbo.Session", "LastModifiedBy", c => c.Long(nullable: false ));
        }
    }
}
