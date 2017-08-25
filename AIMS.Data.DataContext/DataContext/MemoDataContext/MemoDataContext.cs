using AIMS.Data.DataObjects.Entities.Memo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.DataContext.DataContext.MemoDataContext
{
    public class MemoDataContext : DbContext
    {
        public MemoDataContext()
            : base("name = Aims")
        {
        }

        public virtual DbSet<Memo> Memos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public System.Data.Entity.DbSet<AIMS.Data.DataObjects.Entities.Session.Session> Sessions { get; set; }
    }
}
