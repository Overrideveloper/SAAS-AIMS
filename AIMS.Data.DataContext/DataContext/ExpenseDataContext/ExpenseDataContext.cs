using AIMS.Data.DataObjects.Entities.Expense;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.DataContext.DataContext.ExpenseDataContext
{
    public class ExpenseDataContext: DbContext
    {
        public ExpenseDataContext()
            :base("name = Aims")
        {
        }

        public virtual DbSet<ExpenseCategory> ExpenseCategory { get; set; }
        public virtual DbSet<ExpenseItem> ExpenseItem { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
