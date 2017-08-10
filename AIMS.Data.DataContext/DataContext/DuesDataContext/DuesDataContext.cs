using AIMS.Data.DataObjects.Entities.Dues;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.DataContext.DataContext.DuesDataContext
{
    public class DuesDataContext : DbContext
    {
        public DuesDataContext()
            : base("name = Aims")
        {
        }

        public virtual DbSet<Dues> Dues { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public System.Data.Entity.DbSet<AIMS.Data.DataObjects.Entities.Member.Member> Members { get; set; }
    }
}
