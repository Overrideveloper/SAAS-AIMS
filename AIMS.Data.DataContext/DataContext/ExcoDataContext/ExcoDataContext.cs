using AIMS.Data.DataObjects.Entities.Exco;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.DataContext.DataContext.ExcoDataContext
{
    public class ExcoDataContext: DbContext
    {
        public ExcoDataContext()
            :base("name = Aims")
        {
        }

        public virtual DbSet<Exco> Exco { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
