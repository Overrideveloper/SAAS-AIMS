using AIMS.Data.DataObjects.Entities.Income;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.DataContext.DataContext.IncomeDataContext
{
    public class IncomeDataContext: DbContext
    {
        public IncomeDataContext()
            :base("name = Aims")
        {
        }

        public virtual DbSet<IncomeCategory> IncomeCategory { get; set; }
        public virtual DbSet<IncomeItem> IncomeItem { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
